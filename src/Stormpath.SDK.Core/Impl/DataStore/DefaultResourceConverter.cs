// <copyright file="DefaultResourceConverter.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Shared;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultResourceConverter : IResourceConverter
    {
        private IResourceConverter AsInterface => this;

        public Map ToMap(AbstractResource resource)
        {
            var propertyNames = resource.GetUpdatedPropertyNames();

            var resultMap = propertyNames.Select(name =>
            {
                var rawValue = resource.GetProperty(name);
                var value = this.SanitizeValue(rawValue);
                return new { name, value };
            }).ToDictionary(
                map => map.name,
                map => map.value);

            return resultMap;
        }

        public Map ToMap(object resource)
        {
            if (resource == null)
            {
                return null;
            }

            var typeInfo = resource.GetType().GetTypeInfo();

            return typeInfo.DeclaredProperties
                .Where(p => p.CanRead)
                .Select(p =>
                {
                    var rawValue = p.GetValue(resource);

                    object value = null;
                    if (rawValue != null)
                    {
                        value = IsPrimitive(p.PropertyType)
                            ? SanitizeValue(rawValue)
                            : ToMap(rawValue);
                    }

                    return new {name = ToCamelCase(p.Name), value};
                })
                .Where(pair => pair.value != null)
                .ToDictionary(pair => pair.name, pair => pair.value);
        }

        private object SanitizeValue(object rawValue)
        {
            var asEmbedded = rawValue as IEmbeddedProperty;
            if (asEmbedded != null)
            {
                return new { href = asEmbedded.Href };
            }

            var asEnumeration = rawValue as StringEnumeration;
            if (asEnumeration != null)
            {
                return asEnumeration.Value.ToLower();
            }

            var asEmbeddedResource = rawValue as AbstractResource;
            if (asEmbeddedResource != null)
            {
                return this.AsInterface.ToMap(asEmbeddedResource);
            }

            // Already a primitive-like type, no need to sanitize
            return rawValue;
        }

        private bool IsPrimitive(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            var typeInfo = type.GetTypeInfo();

            if (typeof(StringEnumeration).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                return true;
            }

            return type.GetTypeInfo().IsPrimitive;
        }

        private static string ToCamelCase(string input)
        {
            return string.IsNullOrEmpty(input)
                ? input
                : $"{input[0].ToString().ToLower()}{input.Substring(1)}";
        }
    }
}
