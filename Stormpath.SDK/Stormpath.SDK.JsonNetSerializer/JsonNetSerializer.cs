// <copyright file="JsonNetSerializer.cs" company="Stormpath, Inc.">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Extensions.Serialization
{
    /// <summary>
    /// JSON.NET-based deserializer for Stormpath.SDK.
    /// </summary>
    public sealed class JsonNetSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
        /// </summary>
        public JsonNetSerializer()
        {
            this.serializerSettings = new JsonSerializerSettings();
            this.serializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            this.serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }

        /// <inheritdoc/>
        string IJsonSerializer.Serialize(IDictionary<string, object> map)
        {
            var serialized = JsonConvert.SerializeObject(map);

            return serialized;
        }

        /// <inheritdoc/>
        IDictionary<string, object> IJsonSerializer.Deserialize(string json)
        {
            var deserializedMap = (JObject)JsonConvert.DeserializeObject(json, this.serializerSettings);
            var sanitizedMap = this.Sanitize(deserializedMap);

            return sanitizedMap;
        }

        /// <summary>
        /// Converts a nested tree of <see cref="JObject"/>s into nested <see cref="IDictionary{TKey, TValue}"/>s.
        /// </summary>
        /// <remarks>JSON.NET deserializes everything into nested JObjects. We want IDictionaries all the way down.</remarks>
        /// <param name="map">Deserialized <see cref="JObject"/> from JSON.NET</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/> of primitive items, and embedded objects as nested <see cref="IDictionary{TKey, TValue}"/></returns>
        private IDictionary<string, object> Sanitize(JObject map)
        {
            var result = new Dictionary<string, object>(map.Count);

            foreach (var prop in map.Properties())
            {
                var name = prop.Name;
                object value = null;

                switch (prop.Value.Type)
                {
                    case JTokenType.Array:
                        var nested = new List<IDictionary<string, object>>();
                        foreach (var child in prop.Value.Children())
                        {
                            nested.Add(this.Sanitize((JObject)child));
                        }

                        value = nested;
                        break;

                    case JTokenType.Object:
                        value = this.Sanitize((JObject)prop.Value);
                        break;

                    case JTokenType.Date:
                        value = prop.Value.ToObject<DateTimeOffset>();
                        break;

                    case JTokenType.Integer:
                        var raw = prop.Value.ToString();
                        int intResult;
                        long longResult;

                        if (int.TryParse(raw, out intResult))
                            value = intResult;
                        else if (long.TryParse(raw, out longResult))
                            value = longResult;
                        else
                            throw new ArgumentException("Unknown integer type encountered during parsing.");
                        break;

                    case JTokenType.Boolean:
                        value = bool.Parse(prop.Value.ToString());
                        break;

                    case JTokenType.Null:
                        value = null;
                        break;

                    default:
                        value = prop.Value.ToString();
                        break;
                }

                result.Add(name, value);
            }

            return result;
        }
    }
}
