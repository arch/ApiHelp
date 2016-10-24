// Copyright (c) love.net team. All rights reserved.

using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.ApiHelp {
    internal static class StreamExtensions {
        public static Stream AssignParameters(this Stream template, params Tuple<string, string>[] templateParams) {
            using (var sr = new StreamReader(template)) {
                var sb = new StringBuilder(sr.ReadToEnd());
                foreach (var param in templateParams) {
                    sb.Replace(param.Item1, param.Item2);
                }

                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }

        public static void ToResponse(this Stream content, HttpResponse response) {
            response.StatusCode = 200;
            response.ContentType = "text/html";
            content.CopyTo(response.Body);
        }
    }
}
