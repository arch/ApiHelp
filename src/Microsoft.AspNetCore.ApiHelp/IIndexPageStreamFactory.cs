// Copyright (c) love.net team. All rights reserved.

using System.IO;

namespace Microsoft.AspNetCore.ApiHelp {
    /// <summary>
    /// Defines the interface(s) for creating the stream of Index.html file.
    /// </summary>
    public interface IIndexPageStreamFactory {
        /// <summary>
        /// Creates the stream of the Index.html file.
        /// </summary>
        /// <returns>Stream.</returns>
        Stream Create();
    }
}
