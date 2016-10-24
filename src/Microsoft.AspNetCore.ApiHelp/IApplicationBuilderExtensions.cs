// Copyright (c) love.net team. All rights reserved.

using System;
using System.Reflection;
using Microsoft.AspNetCore.ApiHelp;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Builder {
    public static class IApplicationBuilderExtensions {
        public static IApplicationBuilder UseApiHelp(this IApplicationBuilder app, ApiHelpUIOptions uiOptions = null) {
            if(uiOptions == null) {
                uiOptions = new ApiHelpUIOptions();
            }

            // Enable redirect from base path ton index path.
            app.UseMiddleware<RedirectMiddleware>(uiOptions.BaseRoute, uiOptions.IndexPath);

            // Serve indexPath via middleware
            app.UseMiddleware<ApiHelpMiddleware>(uiOptions);

            // Serve all other UI assets as static files
            var options = new FileServerOptions();
            options.RequestPath = $"/{uiOptions.BaseRoute}";
            options.EnableDefaultFiles = false;
            options.StaticFileOptions.ContentTypeProvider = new FileExtensionContentTypeProvider();

            // Debug view the embed files
            var embedFiles = typeof(IApplicationBuilderExtensions).GetTypeInfo().Assembly.GetManifestResourceNames();

            var baseNamespace = "Microsoft.AspNetCore.ApiHelp.UI.";
            switch (uiOptions.UI) {
                case ApiHelpUI.Swagger:
                    baseNamespace += "Swagger.dist";
                    break;
                case ApiHelpUI.JsonH:
                    baseNamespace += "JsonH";
                    break;
                case ApiHelpUI.JsonEditor:
                    baseNamespace += "JsonEditor";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
            options.FileProvider = new EmbeddedFileProvider(typeof(ApiHelpUI).GetTypeInfo().Assembly, baseNamespace);

            app.UseFileServer(options);

            return app;
        }
    }
}
