// Copyright (c) love.net team. All rights reserved.

using System.IO;
using System.Reflection;

namespace Microsoft.AspNetCore.ApiHelp {
    /// <summary>
    /// Represents the default implementation of <see cref="IIndexPageStreamFactory"/> interface.
    /// </summary>
    public class IndexPageStreamFactory : IIndexPageStreamFactory {
        private readonly Assembly _resourceAssembly;
        private readonly string _indexResourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPageStreamFactory" /> class.
        /// </summary>
        /// <param name="indexResourceName">Name of the index resource.</param>
        public IndexPageStreamFactory(string indexResourceName) {
            _resourceAssembly = GetType().GetTypeInfo().Assembly;
            _indexResourceName = indexResourceName;
        }

        /// <summary>
        /// Creates the stream of the Index.html file.
        /// </summary>
        /// <returns>Stream.</returns>
        public Stream Create() => _resourceAssembly.GetManifestResourceStream(_indexResourceName);
    }
}
