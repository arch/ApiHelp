// Copyright (c) Arch team. All rights reserved.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ApiHelp.Core;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Extensions for configuring API help services using an <see cref="IMvcCoreBuilder"/>.
    /// </summary>
    public static class IMvcCoreBuilderExtensions {
        /// <summary>
        /// Configuring API help services using an <see cref="IMvcCoreBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder" /> interface for configuring essential MVC services.</param>
        /// <param name="setupAction">An action to configure the <see cref="ApiHelpOptions"/>.</param>
        /// <returns>The <see cref="IMvcCoreBuilder" /> interface for configuring essential MVC services.</returns>
        public static IMvcCoreBuilder AddApiHelp(this IMvcCoreBuilder builder, Action<ApiHelpOptions> setupAction = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddApiExplorer();

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
