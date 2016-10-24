// Copyright (c) love.net team. All rights reserved.

namespace Microsoft.AspNetCore.ApiHelp.Core {
    /// <summary>
    /// Represents the document generate strategy.
    /// </summary>
    public enum DocumentGenerateStrategy {
        /// <summary>
        /// Indicating eager loading.
        /// </summary>
        Eager,
        /// <summary>
        /// Indicating lazy loading.
        /// </summary>
        Lazy,
    }
}
