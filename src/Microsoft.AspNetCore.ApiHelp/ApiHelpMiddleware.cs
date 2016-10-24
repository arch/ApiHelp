// Copyright (c) love.net team. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Microsoft.AspNetCore.ApiHelp {
    public class ApiHelpMiddleware {
        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _requestMatcher;
        private readonly ApiHelpUIOptions _options;
        private readonly Assembly _resourceAssembly;
        private readonly string _templateResourceName;

        public ApiHelpMiddleware(RequestDelegate next, ApiHelpUIOptions options) {
            _next = next;
            _options = options;
            _requestMatcher = new TemplateMatcher(TemplateParser.Parse(_options.IndexPath), new RouteValueDictionary());
            _resourceAssembly = GetType().GetTypeInfo().Assembly;

            // set the index resource name.
            _templateResourceName = "Microsoft.AspNetCore.ApiHelp.UI.";
            switch (_options.UI) {
                case ApiHelpUI.Swagger:
                    _templateResourceName += "Swagger.dist.index.html";
                    break;
                case ApiHelpUI.JsonH:
                    _templateResourceName += "JsonH.index.html";
                    break;
                case ApiHelpUI.JsonEditor:
                    _templateResourceName += "JsonEditor.index.html";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
        }

        public async Task Invoke(HttpContext httpContext) {
            if (!RequestingApiHelpUI(httpContext.Request)) {
                await _next(httpContext);
                return;
            }

            var template = _resourceAssembly.GetManifestResourceStream(_templateResourceName);

            template.AssignParameters(new Tuple<string, string>($"%(Title)", _options.Title)).ToResponse(httpContext.Response);
        }

        private bool RequestingApiHelpUI(HttpRequest request) 
            => (request.Method == "GET") && _requestMatcher.TryMatch(request.Path, new RouteValueDictionary());
    }
}
