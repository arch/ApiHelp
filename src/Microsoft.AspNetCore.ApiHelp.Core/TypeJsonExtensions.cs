// Copyright (c) Arch team. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.ApiHelp.Core {
    /// <summary>
    /// The extension methods for <see cref="Type"/> to enable an application to read XML comments at run time represents by a Json object.
    /// </summary>
    internal static class TypeJsonExtensions {
        public static JToken Schema(this Type type, params string[] excludeProperties) {
            if (type == null || type == typeof(void)) {
                return null;
            }

            type = type.UnwrapNullableType();

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum || type.IsPrimitive()) {
                return JToken.FromObject(type.GetDefaultValue() ?? type.Name);
            }
            else if (typeInfo.IsGenericType && type.IsCollectionType()) {
                var array = new JArray();
                var elementType = type.GenericTypeArguments[0];
                if (elementType.IsPrimitive()) {
                    array.Add(JToken.FromObject(elementType.GetDefaultValue() ?? elementType.Name));
                }
                else {
                    array.Add(elementType.Schema());
                }

                return array;
            }

            var json = new JObject();
            var properties = typeInfo.GetProperties();
            if (excludeProperties.Length > 0) {
                properties = properties.Where(p => !excludeProperties.Contains(p.Name)).ToArray();
            }
            foreach (var prop in properties) {
                var propertyType = prop.PropertyType.UnwrapNullableType();
                if (propertyType.IsPrimitive()) {
                    var defaultValue = JToken.FromObject(propertyType.GetDefaultValue() ?? prop.XmlDoc() ?? prop.Name);
                    json.Add(prop.Name, defaultValue);
                }
                else if (propertyType.GetTypeInfo().IsGenericType && propertyType.IsCollectionType()) {
                    var array = new JArray();
                    var elementType = propertyType.GenericTypeArguments[0];
                    if (elementType.IsPrimitive()) {
                        var defaultValue = JToken.FromObject(elementType.GetDefaultValue() ?? prop.XmlDoc() ?? prop.Name);
                        array.Add(defaultValue);
                    }
                    else {
                        array.Add(elementType.Schema());
                    }

                    json.Add(prop.Name, array);
                }
                else {
                    json.Add(prop.Name, propertyType.Schema());
                }
            }

            return json;
        }

        public static JToken Scaffold(this Type type) {
            if (type == null || type == typeof(void)) {
                return null;
            }

            var unwarp = type.UnwrapNullableType();

            var json = new JObject();

            var typeInfo = unwarp.GetTypeInfo();
            if (typeInfo.IsEnum) {
                // Summary
                json.Add("Summary", unwarp.GetEnumXDoc());
                // Type
                json.Add("Type", Enum.GetUnderlyingType(unwarp).Name);

                // Optional
                var isNullable = type != unwarp;
                if (isNullable) {
                    json.Add("IsOptional", true);
                }
                else {
                    json.Add("IsOptional", false);
                }

                return json;
            }
            else if (type.IsPrimitive()) {
                // Optional
                var isNullable = type != unwarp;
                if (isNullable) {
                    json.Add("Type", unwarp.Name);
                    json.Add("IsOptional", true);
                    return json;
                }
                else {
                    return type.Name;
                }
            }
            else if (typeInfo.IsGenericType && type.IsCollectionType()) {
                var array = new JArray();
                var elementType = type.GenericTypeArguments[0];
                if (elementType.IsPrimitive()) {
                    array.Add(JToken.FromObject(elementType.GetDefaultValue() ?? elementType.Name));
                }
                else {
                    array.Add(elementType.Scaffold());
                }

                return array;
            }

            var properties = typeInfo.GetProperties();
            foreach (var prop in properties) {
                if (prop.PropertyType.IsPrimitive()) {
                    var propJson = new JObject();
                    var propertyType = prop.PropertyType.UnwrapNullableType();
                    // Summary & type
                    if (propertyType.GetTypeInfo().IsEnum) {
                        propJson.Add("Summary", propertyType.GetEnumXDoc());
                        propJson.Add("Type", Enum.GetUnderlyingType(propertyType).Name);
                    }
                    else {
                        propJson.Add("Summary", prop.XmlDoc() ?? prop.Name);
                        propJson.Add("Type", propertyType.Name);
                    }

                    // Optional
                    var isNullable = propertyType != prop.PropertyType;
                    if (isNullable) {
                        propJson.Add("IsOptional", true);
                    }
                    else {
                        propJson.Add("IsOptional", false);
                    }

                    json.Add(prop.Name, propJson);
                }
                else if (prop.PropertyType.GetTypeInfo().IsGenericType && prop.PropertyType.IsCollectionType()) {
                    var array = new JArray();
                    var elementType = prop.PropertyType.GenericTypeArguments[0];
                    if (elementType.IsPrimitive()) {
                        var inner = new JObject();
                        var unwrapType = elementType.UnwrapNullableType();
                        // Summary & type
                        if (unwrapType.GetTypeInfo().IsEnum) {
                            inner.Add("Summary", unwrapType.GetEnumXDoc());
                            inner.Add("Type", Enum.GetUnderlyingType(unwrapType).Name);
                        }
                        else {
                            inner.Add("Summary", prop.XmlDoc() ?? prop.Name);
                            inner.Add("Type", unwrapType.Name);
                        }

                        // Optional
                        var isNullable = unwrapType != elementType;
                        if (isNullable) {
                            inner.Add("IsOptional", true);
                        }
                        else {
                            inner.Add("IsOptional", false);
                        }

                        array.Add(inner);
                    }
                    else {
                        array.Add(elementType.Scaffold());
                    }

                    json.Add(prop.Name, array);
                }
                else {
                    json.Add(prop.Name, prop.PropertyType.Scaffold());
                }
            }

            return json;
        }

        internal static JToken GetEnumXDoc(this Type enumType) {
            var typeInfo = enumType.UnwrapNullableType().GetTypeInfo();
            if (!typeInfo.IsEnum) {
                throw new ArgumentException($"{nameof(enumType)} must be a Enum", nameof(enumType));
            }

            var json = new JObject();

            // why: remove the "value__"
            var fields = typeInfo.DeclaredFields.Where(f => !f.IsSpecialName);
            foreach (var field in fields) {
                var xml = field.XmlDoc() ?? field.Name;
                json.Add(field.GetRawConstantValue().ToString(), xml);
            }

            return json;
        }
    }
}
