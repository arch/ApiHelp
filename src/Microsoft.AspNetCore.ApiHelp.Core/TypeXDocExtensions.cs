// Copyright (c) love.net team. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.AspNetCore.ApiHelp.Core {
    /// <summary>
    /// Provides extension methods for reading XML comments from reflected members.
    /// </summary>
    internal static class TypeXDocExtensions {
        private static Dictionary<string, XDocument> _cachedXDoc;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static TypeXDocExtensions() {
            _cachedXDoc = new Dictionary<string, XDocument>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the XML documentation (summary tag) for the specified member.
        /// </summary>
        /// <param name="member">The reflected member.</param>
        /// <returns>The contents of the summary tag for the member.</returns>
        public static string XmlDoc(this MemberInfo member) {
            var assembly = member.Module.Assembly;

            var assemblyName = assembly.GetName();
            if (_cachedXDoc.ContainsKey(assemblyName.FullName)) {
                var xml = _cachedXDoc[assemblyName.FullName];

                return XmlDoc(member, xml);
            }
            else {
                var assemblyPath = Path.GetDirectoryName(assembly.Location);
                var path = Path.Combine(assemblyPath, assemblyName.Name + ".xml");

                return XmlDoc(member, path);
            }
        }

        /// <summary>
        /// Returns the XML documentation (summary tag) for the specified member.
        /// </summary>
        /// <param name="member">The reflected member.</param>
        /// <param name="pathToXmlFile">Path to the XML documentation file.</param>
        /// <returns>The contents of the summary tag for the member.</returns>
        public static string XmlDoc(this MemberInfo member, string pathToXmlFile) {
            var assemblyName = member.Module.Assembly.GetName();
            XDocument xml = null;

            if (_cachedXDoc.ContainsKey(assemblyName.FullName)) {
                xml = _cachedXDoc[assemblyName.FullName];
            }
            else {
                if (!File.Exists(pathToXmlFile)) {
                    //throw new FileNotFoundException($"Cannot find the xml file: {pathToXmlFile}", pathToXmlFile);
                    return null;
                }
                _cachedXDoc[assemblyName.FullName] = (xml = XDocument.Load(pathToXmlFile));
            }

            return XmlDoc(member, xml);
        }

        /// <summary>
        /// Returns the XML documentation (summary tag) for the specified member.
        /// </summary>
        /// <param name="member">The reflected member.</param>
        /// <param name="xml">XML documentation.</param>
        /// <returns>The contents of the summary tag for the member.</returns>
        public static string XmlDoc(this MemberInfo member, XDocument xml) {
            return xml.XPathEvaluate(
                string.Format(
                    "string(/doc/members/member[@name='{0}']/summary)",
                    GetMemberElementName(member)
                )
            ).ToString().Trim();
        }

        /// <summary>
        /// Returns the XML documentation (returns/param tag) for the specified parameter.
        /// </summary>
        /// <param name="parameter">The reflected parameter (or return value).</param>
        /// <returns>The contents of the returns/param tag for the parameter.</returns>
        /// <example>
        /// Console.WriteLine(typeof(SomeClass).GetMethod("SomeMethod").GetParameter("someParam").XmlDoc());
        /// Console.WriteLine(typeof(SomeClass).GetMethod("SomeMethod").ReturnParameter.XmlDoc());
        /// </example>
        public static string XmlDoc(this ParameterInfo parameter) {
            var assembly = parameter.Member.Module.Assembly;

            var assemblyName = assembly.GetName();
            if (_cachedXDoc.ContainsKey(assemblyName.FullName)) {
                var xml = _cachedXDoc[assemblyName.FullName];

                return XmlDoc(parameter, xml);
            }
            else {
                var assemblyPath = Path.GetDirectoryName(assembly.Location);
                var path = Path.Combine(assemblyPath, assemblyName.Name + ".xml");

                return XmlDoc(parameter, path);
            }
        }

        /// <summary>
        /// Returns the XML documentation (returns/param tag) for the specified parameter.
        /// </summary>
        /// <param name="parameter">The reflected parameter (or return value).</param>
        /// <param name="pathToXmlFile">Path to the XML documentation file.</param>
        /// <returns>The contents of the returns/param tag for the parameter.</returns>
        public static string XmlDoc(this ParameterInfo parameter, string pathToXmlFile) {
            var assemblyName = parameter.Member.Module.Assembly.GetName();
            XDocument xml = null;

            if (_cachedXDoc.ContainsKey(assemblyName.FullName)) {
                xml = _cachedXDoc[assemblyName.FullName];
            }
            else {
                if (!File.Exists(pathToXmlFile)) {
                    //throw new FileNotFoundException("Cannot find the xml file", pathToXmlFile);
                    return null;
                }

                _cachedXDoc[assemblyName.FullName] = (xml = XDocument.Load(pathToXmlFile));
            }

            return XmlDoc(parameter, xml);
        }

        /// <summary>
        /// Returns the XML documentation (returns/param tag) for the specified parameter.
        /// </summary>
        /// <param name="parameter">The reflected parameter (or return value).</param>
        /// <param name="xml">XML documentation.</param>
        /// <returns>The contents of the returns/param tag for the parameter.</returns>
        public static string XmlDoc(this ParameterInfo parameter, XDocument xml) {
            if (parameter.IsRetval || string.IsNullOrEmpty(parameter.Name)) {
                return xml.XPathEvaluate(
                    string.Format(
                        "string(/doc/members/member[@name='{0}']/returns)",
                        GetMemberElementName(parameter.Member)
                    )
                ).ToString().Trim();
            }
            else {
                return xml.XPathEvaluate(
                    string.Format(
                        "string(/doc/members/member[@name='{0}']/param[@name='{1}'])",
                        GetMemberElementName(parameter.Member),
                        parameter.Name
                    )
                ).ToString().Trim();
            }
        }

        /// <summary>
        /// Returns the expected name for a member element in the XML documentation file.
        /// </summary>
        /// <param name="member">The reflected member.</param>
        /// <returns>The name of the member element.</returns>
        private static string GetMemberElementName(MemberInfo member) {
            char prefixCode;
            string memberName = member.DeclaringType.FullName + "." + member.Name;

            switch (member.MemberType) {
                case MemberTypes.Constructor:
                    // XML documentation uses slightly different constructor names
                    memberName = memberName.Replace(".ctor", "#ctor");
                    goto case MemberTypes.Method;
                case MemberTypes.Method:
                    prefixCode = 'M';
                    // parameters are listed according to their type, not their name
                    string paramTypesList = string.Join(
                        ",",
                        ((MethodBase)member).GetParameters()
                            .Cast<ParameterInfo>()
                            .Select(x => x.ParameterType.FullName
                        ).ToArray()
                    );
                    if (!string.IsNullOrEmpty(paramTypesList)) {
                        memberName += "(" + paramTypesList + ")";
                    }
                    break;
                case MemberTypes.Event:
                    prefixCode = 'E';
                    break;
                case MemberTypes.Field:
                    prefixCode = 'F';
                    break;
                case MemberTypes.NestedType:
                    // XML documentation uses slightly different nested type names
                    memberName = memberName.Replace('+', '.');
                    goto case MemberTypes.TypeInfo;
                case MemberTypes.TypeInfo:
                    prefixCode = 'T';
                    break;
                case MemberTypes.Property:
                    prefixCode = 'P';
                    break;
                default:
                    throw new ArgumentException("Unknown member type", nameof(member));
            }

            // elements are of the form "M:Namespace.Class.Method"
            return string.Format("{0}:{1}", prefixCode, memberName);
        }
    }
}
