// <copyright file="JsonSerializationProvider.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Serialization.FieldConverters;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Impl.Serialization
{
    internal sealed class JsonSerializationProvider
    {
        private static readonly FieldConverterList ConverterChain =
            new FieldConverterList(
                new LinkPropertyConverter(),
                new StatusFieldConverters.AccountStatusConverter(),
                new StatusFieldConverters.ApplicationStatusConverter(),
                new StatusFieldConverters.DirectoryStatusConverter(),
                new StatusFieldConverters.GroupStatusConverter());

        private readonly IJsonSerializer externalSerializer;
        private readonly ResourceTypeLookup typeLookup;

        public JsonSerializationProvider(IJsonSerializer externalSerializer)
        {
            this.externalSerializer = externalSerializer;
            this.typeLookup = new ResourceTypeLookup();
        }

        public string Serialize(IDictionary<string, object> map)
        {
            return this.externalSerializer.Serialize(map);
        }

        public IDictionary<string, object> Deserialize(string json, Type targetType)
        {
            var stringlyTypedProperties = this.externalSerializer.Deserialize(json);
            var stronglyTypedProperties = this.ConvertProperties(stringlyTypedProperties, targetType);

            return stronglyTypedProperties;
        }

        private IDictionary<string, object> ConvertProperties(IDictionary<string, object> properties, Type targetType)
        {
            var result = new Dictionary<string, object>(properties.Count);

            foreach (var prop in properties)
            {
                object value = prop.Value;

                var asListOfObjects = prop.Value as IEnumerable<IDictionary<string, object>>;
                if (asListOfObjects != null)
                {
                    var outputList = new List<IDictionary<string, object>>();
                    foreach (var item in asListOfObjects)
                    {
                        outputList.Add(this.ConvertProperties(item, targetType));
                    }

                    value = outputList;
                }
                else
                {
                    var convertResult = ConverterChain.TryConvertField(prop, targetType);
                    if (convertResult.Success)
                        value = convertResult.Value;
                }

                result.Add(prop.Key, value);
            }

            return result;
        }
    }
}
