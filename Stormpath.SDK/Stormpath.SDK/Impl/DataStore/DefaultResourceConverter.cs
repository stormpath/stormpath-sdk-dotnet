// <copyright file="DefaultResourceConverter.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

using System.Linq;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Shared;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultResourceConverter : IResourceConverter
    {
        private IResourceConverter AsInterface => this;

        Map IResourceConverter.ToMap(AbstractResource resource)
        {
            var propertyNames = resource.GetUpdatedPropertyNames();

            var resultMap = propertyNames.Select(name =>
            {
                var rawValue = resource.GetProperty(name);
                var value = this.SanitizeValue(resource, name, rawValue);
                return new { name, value };
            }).ToDictionary(
                map => map.name,
                map => map.value);

            return resultMap;
        }

        private object SanitizeValue(AbstractResource resource, string propertyName, object rawValue)
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
    }
}
