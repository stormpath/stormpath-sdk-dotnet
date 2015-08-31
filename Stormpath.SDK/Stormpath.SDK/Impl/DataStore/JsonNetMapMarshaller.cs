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
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Impl.DataStore.FieldConverters;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class JsonNetMapMarshaller : IMapSerializer
    {
        private readonly JsonSerializerSettings serializerSettings;

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
                DefaultFieldConverters.NullConverter,
                DefaultFieldConverters.FallbackConverter);

        public JsonNetMapMarshaller()
        {
            this.serializerSettings = new JsonSerializerSettings();
            this.serializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            this.serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }

        string IMapSerializer.Serialize(Dictionary<string, object> map)
        {
            var serialized = JsonConvert.SerializeObject(map);

            return serialized;
        }

        Hashtable IMapSerializer.Deserialize(string json, Type type)
        {
            var deserializedMap = (JObject)JsonConvert.DeserializeObject(json, this.serializerSettings);
            var sanitizedMap = this.Sanitize(deserializedMap, type);

            return sanitizedMap;
        }

        /// <summary>
        /// JSON.NET deserializes everything into nested JObjects. We want Hashtables all the way down.
        /// </summary>
        /// <param name="map">Deserialized JObject from JSON.NET</param>
        /// <param name="type">Target resource type (as an interface)</param>
        /// <returns>Hashtable of primitive items, and embedded objects as Hashtables</returns>
        private Hashtable Sanitize(JObject map, Type type)
        {
            // TODO there is probably a cleaner way of doing all of this. IDictionaries in the AbstractResource constructor?
            var result = new Hashtable(map.Count);

            foreach (var prop in map.Properties())
            {
                var name = prop.Name;
                object value = null;

                if (prop.Value.Type == JTokenType.Array)
                {
                    var nested = new List<Hashtable>();
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
