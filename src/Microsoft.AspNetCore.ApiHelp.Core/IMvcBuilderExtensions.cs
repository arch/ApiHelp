// Copyright (c) love.net team. All rights reserved.

using System;
using Microsoft.AspNetCore.ApiHelp.Core;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Extensions for configuring API help services using an <see cref="IMvcBuilder"/>.
    /// </summary>
    public static class IMvcBuilderExtensions {
        /// <summary>
        /// Configuring API help services using an <see cref="IMvcBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> interface for configuring MVC services.</param>
        /// <param name="setupAction">An action to configure the <see cref="ApiHelpOptions"/>.</param>
        /// <returns>The <see cref="IMvcBuilder" /> interface for configuring MVC services.</returns>
        public static IMvcBuilder AddApiHelp(this IMvcBuilder builder, Action<ApiHelpOptions> setupAction = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            AddApiHelpServices(builder.Services);

            if (setupAction != null) {
                builder.Services.Configure(setupAction);
            }
            else {
                builder.Services.Configure<ApiHelpOptions>(options => {
                    options.IncludeSupportedMediaType = true;
                    options.IgnoreObsoleteApi = true;
                    options.GenerateStrategy = DocumentGenerateStrategy.Eager;
                });
            }

            return builder;
        }

        private static void AddApiHelpServices(IServiceCollection services) {
            services.Configure<MvcOptions>(c =>
                c.Conventions.Add(new ApiHelpApplicationModelConvention()));
        }
    }
}
