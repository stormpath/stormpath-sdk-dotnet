// <copyright file="JsonSerializationProvider.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using Stormpath.SDK.Impl.Serialization.FieldConverters;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Serialization
{
    internal sealed class JsonSerializationProvider
    {
        private readonly FieldConverterList converterChain;

        internal readonly IJsonSerializer ExternalSerializer;

        public JsonSerializationProvider(IJsonSerializer externalSerializer)
        {
            this.ExternalSerializer = externalSerializer;

            this.converterChain = new FieldConverterList(
                new LinkPropertyConverter(),
                new ExpandedPropertyConverter(converter: this.ConvertProperties),
                new StatusFieldConverters.AccountStatusConverter(),
                new StatusFieldConverters.ApplicationStatusConverter(),
                new StatusFieldConverters.DirectoryStatusConverter(),
                new StatusFieldConverters.GroupStatusConverter(),
                new StatusFieldConverters.OrganizationStatusConverter());
        }

        public string Serialize(Map map)
        {
            return this.ExternalSerializer.Serialize(map);
        }

        public Map Deserialize(string json, Type targetType)
        {
            var stringlyTypedProperties = this.ExternalSerializer.Deserialize(json);
            var stronglyTypedProperties = this.ConvertProperties(stringlyTypedProperties, targetType);

            return stronglyTypedProperties;
        }

        private Map ConvertProperties(Map properties, Type targetType)
        {
            var result = new Dictionary<string, object>(properties.Count);

            foreach (var prop in properties)
            {
                object value = prop.Value;

                var asListOfObjects = prop.Value as IEnumerable<Map>;
                var asEmbeddedObject = prop.Value as Map;
                if (asListOfObjects != null)
                {
                    var outputList = new List<Map>();
                    foreach (var item in asListOfObjects)
                    {
                        outputList.Add(this.ConvertProperties(item, targetType));
                    }

                    value = outputList;
                }
                else
                {
                    var convertResult = this.converterChain.TryConvertField(prop, targetType);
                    if (convertResult.Success)
                    {
                        value = convertResult.Value;
                    }
                }

                result.Add(prop.Key, value);
            }

            return result;
        }
    }
}
