// <copyright file="JsonNetMapMarshaller.cs" company="Stormpath, Inc.">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Impl.DataStore.FieldConverters;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class JsonNetMapMarshaller : IJsonSerializer
    {
        private static readonly FieldConverterList ConverterChain =
            new FieldConverterList(
                DefaultFieldConverters.LinkPropertyConverter,
                DefaultFieldConverters.DateTimeOffsetConverter,
                DefaultFieldConverters.AccountStatusConverter,
                DefaultFieldConverters.ApplicationStatusConverter,
                DefaultFieldConverters.DirectoryStatusConverter,
                DefaultFieldConverters.GroupStatusConverter,
                DefaultFieldConverters.IntConverter,
                DefaultFieldConverters.StringConverter,
                DefaultFieldConverters.BoolConverter,
                DefaultFieldConverters.NullConverter,
                DefaultFieldConverters.FallbackConverter);

        private readonly JsonSerializerSettings serializerSettings;
        private readonly ResourceTypeLookup typeLookup;

        public JsonNetMapMarshaller()
        {
            this.serializerSettings = new JsonSerializerSettings();
            this.serializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            this.serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            this.typeLookup = new ResourceTypeLookup();
        }

        string IJsonSerializer.Serialize(IDictionary<string, object> map)
        {
            var serialized = JsonConvert.SerializeObject(map);

            return serialized;
        }

        IDictionary<string, object> IJsonSerializer.Deserialize(string json, Type type)
        {
            var deserializedMap = (JObject)JsonConvert.DeserializeObject(json, this.serializerSettings);
            var sanitizedMap = this.Sanitize(deserializedMap, type);

            return sanitizedMap;
        }

        /// <summary>
        /// JSON.NET deserializes everything into nested JObjects. We want IDictionaries all the way down.
        /// </summary>
        /// <param name="map">Deserialized <see cref="JObject"/> from JSON.NET</param>
        /// <param name="type">Target resource type (as an interface)</param>
        /// <returns><see cref="IDictionary{string, object}"/> of primitive items, and embedded objects as nested <see cref="IDictionary{string, object}"/></returns>
        private IDictionary<string, object> Sanitize(JObject map, Type type)
        {
            var result = new Dictionary<string, object>(map.Count);

            foreach (var prop in map.Properties())
            {
                var name = prop.Name;
                object value = null;

                bool isSupportedCollectionItems =
                    prop.Value.Type == JTokenType.Array &&
                    this.typeLookup.GetInnerCollectionInterface(type) != null;
                if (isSupportedCollectionItems)
                {
                    var nested = new List<IDictionary<string, object>>();
                    foreach (var child in prop.Value.Children())
                    {
                        nested.Add(this.Sanitize((JObject)child, type));
                    }

                    value = nested;
                }
                else
                {
                    var convertResult = ConverterChain.TryConvertField(prop, type);
                    if (convertResult.Success)
                        value = convertResult.Result;
                }

                result.Add(name, value);
            }

            return result;
        }
    }
}
