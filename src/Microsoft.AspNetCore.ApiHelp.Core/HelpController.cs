// Copyright (c) love.net team. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.ApiHelp.Core {
    /// <summary>
    /// Represents the help controller to generate API documentation.
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class HelpController {
        private readonly IApiDescriptionGroupCollectionProvider _provider;
        private readonly ApiHelpOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpController"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="options">The api help options.</param>
        public HelpController(IApiDescriptionGroupCollectionProvider provider, IOptions<ApiHelpOptions> options) {
            _provider = provider;
            _options = options.Value;
        }

        [HttpGet]
        public JObject Get() {
            var json = new JObject();

            // each group
            foreach (var group in _provider.ApiDescriptionGroups.Items) {
                json.Add(group.GroupName, Handle(group));
            }

            return json;
        }

        [HttpGet("[action]")]
        public JObject Get([FromQuery]ApiHelpInputModel model) {
            var items = _provider.ApiDescriptionGroups.Items
                .SelectMany(group => group.Items.Where(it =>
                    it.RelativePath.Equals(model.RelativePath, StringComparison.OrdinalIgnoreCase)
                    &&
                    (model.HttpMethod == null || it.HttpMethod.Equals(model.HttpMethod, StringComparison.OrdinalIgnoreCase))
                 ));

            // Remove the obsolete
            if (_options.IgnoreObsoleteApi) {
                items = items.Where(item => !item.IsObsolete());
            }

            var json = new JObject();
            foreach (var item in items) {
                json.Add($"{item.HttpMethod} {item.RelativePath}", Handle(item, false));
            }

            return json;
        }

        private JToken Handle(ApiDescriptionGroup group) {
            var items = group.Items.AsEnumerable();
            // Remove the obsolete
            if (_options.IgnoreObsoleteApi) {
                items = items.Where(item => !item.IsObsolete());
            }

            var json = new JObject();
            foreach (var item in items) {
                json.Add($"{item.HttpMethod} {item.RelativePath}", Handle(item));
            }

            return json;
        }

        private JToken Handle(ApiDescription item, bool checkPolicy = true) {
            var json = new JObject();

            json.Add("Summary", item.GetSummary());
            json.Add("HttpMethod", item.HttpMethod);
            json.Add("RelativePath", item.RelativePath);

            if (!checkPolicy || _options.GenerateStrategy == DocumentGenerateStrategy.Eager) {
                json.Add("Request", HandleRequest(item));
                json.Add("Response", HandleResponse(item));
            }

            return json;
        }

        private JToken HandleRequest(ApiDescription item) {
            var json = new JObject();

            if (_options.IncludeSupportedMediaType && item.SupportedRequestFormats?.Count > 0) {
                json.Add("SupportedMediaType", new JArray(item.SupportedRequestMediaTypes()));
            }

            foreach (var parameter in item.ParameterDescriptions.Where(p => p.Source.IsFromRequest)) {
                json.Add(parameter.Name, HandlerParameter(parameter));
            }

            return json;
        }

        private JToken HandleResponse(ApiDescription item) {
            Type type = null;

            var action = item.ActionDescriptor;
            if (action is ControllerActionDescriptor) {
                var controllerAtion = action as ControllerActionDescriptor;
                // We only provide response info if we can figure out a type that is a user-data type.
                // Void /Task object/IActionResult will result in no data.
                type = GetDeclaredReturnType(controllerAtion);
            }
            else {
                var supportedResponseTypes = item.SupportedResponseTypes;
                if (supportedResponseTypes.Count > 0) {
                    type = supportedResponseTypes[0].Type;
                }
            }

            var json = new JObject();

            if (_options.IncludeSupportedMediaType && item.SupportedResponseTypes?.Count > 0) {
                json.Add("SupportedMediaType", new JArray(item.SupportedResponseMediaTypes()));
            }

            json.Add("Scaffold", type.Scaffold());
            json.Add("Schema", type.Schema());

            return json;
        }

        private JToken HandlerParameter(ApiParameterDescription parameter) {
            var json = new JObject();

            json.Add("Source", parameter.Source.Id);
            json.Add("Scaffold", parameter.Type.Scaffold());
            json.Add("Schema", parameter.Type.Schema());

            return json;
        }

        private static Type GetDeclaredReturnType(ControllerActionDescriptor action) {
            var declaredReturnType = action.MethodInfo.ReturnType;
            if (declaredReturnType == typeof(void) ||
                declaredReturnType == typeof(Task)) {
                return typeof(void);
            }

            // Unwrap the type if it's a Task<T>. The Task (non-generic) case was already handled.
            var unwrappedType = GetTaskInnerTypeOrNull(declaredReturnType) ?? declaredReturnType;

            // If the method is declared to return IActionResult or a derived class, that information
            // isn't valuable to the formatter.
            if (typeof(IActionResult).IsAssignableFrom(unwrappedType)) {
                return null;
            }
            else {
                return unwrappedType;
            }
        }

        private static Type GetTaskInnerTypeOrNull(Type type) {
            var genericType = ClosedGenericMatcher.ExtractGenericInterface(type, typeof(Task<>));

            return genericType?.GenericTypeArguments[0];
        }
    }
}
