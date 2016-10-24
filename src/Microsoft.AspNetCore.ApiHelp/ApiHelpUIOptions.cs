// Copyright (c) love.net team. All rights reserved.

namespace Microsoft.AspNetCore.ApiHelp {
    /// <summary>
    /// Represents the all options will be used to configure Api help UI.
    /// </summary>
    public class ApiHelpUIOptions {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; } = "API toolchain for ASP.NET Core";
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; } = "A toolchain for ASP.NET Core to automatically generating API documentation.";
        /// <summary>
        /// Gets or sets the base route.
        /// </summary>
        /// <value>The base route.</value>
        public string BaseRoute { get; set; } = "api/help/ui";
        /// <summary>
        /// Gets the index path.
        /// </summary>
        /// <value>The index path.</value>
        public string IndexPath => BaseRoute.Trim('/') + "/index.html";
        /// <summary>
        /// Gets or sets the UI.
        /// </summary>
        /// <value>The UI.</value>
        public ApiHelpUI UI { get; set; } = ApiHelpUI.Swagger;
    }
}
