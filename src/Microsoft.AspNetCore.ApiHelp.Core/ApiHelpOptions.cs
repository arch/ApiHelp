// Copyright (c) love.net team. All rights reserved.

namespace Microsoft.AspNetCore.ApiHelp.Core {
    /// <summary>
    /// Represents the all options will be used for configuring Api help toolchain.
    /// </summary>
    public class ApiHelpOptions {
        /// <summary>
        /// Gets or sets a value indicating whether include supported media type.
        /// </summary>
        /// <value><c>true</c> if include supported media type; otherwise, <c>false</c>.</value>
        public bool IncludeSupportedMediaType { get; set; } = true;
        /// <summary>
        /// Gets or sets the value indicating whether ignore obsoleted API or not.
        /// </summary>
        /// <value>The value indicating whether ignore obsoleted API or not.</value>
        public bool IgnoreObsoleteApi { get; set; } = true;

        /// <summary>
        /// Gets or sets the API document loading policy.
        /// </summary>
        /// <value>The API document loading policy.</value>
        public DocumentGenerateStrategy GenerateStrategy { get; set; } = DocumentGenerateStrategy.Lazy;
    }
}
