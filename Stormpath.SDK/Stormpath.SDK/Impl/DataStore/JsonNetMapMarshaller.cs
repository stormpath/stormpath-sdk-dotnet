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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class JsonNetMapMarshaller : IMapSerializer
    {
        private readonly JsonSerializerSettings serializerSettings;

        public JsonNetMapMarshaller()
        {
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }

        Hashtable IMapSerializer.Deserialize(string json)
        {
            var deserializedMap = (JObject)JsonConvert.DeserializeObject(json, serializerSettings);
            var sanitizedMap = Sanitize(deserializedMap);

            return sanitizedMap;
        }

        /// <summary>
        /// JSON.NET deserializes everything into nested JObjects. We want Hashtables all the way down.
        /// </summary>
        /// <param name="map">Deserialized JObject from JSON.NET</param>
        /// <returns>Hashtable of primitive items, and embedded objects as Hashtables</returns>
        private Hashtable Sanitize(JObject map)
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
                        nested.Add(Sanitize((JObject)child));
                    }

                    value = nested;
                }
                else if (prop.Value.Type == JTokenType.Object)
                {
                    var firstChild = prop.Value.First as JProperty;

                    bool isLinkProperty = prop.Value.Children().Count() == 1
                        && firstChild?.Name == "href";
                    if (isLinkProperty)
                    {
                        value = new LinkProperty(firstChild.Value.ToString());
                    }
                    else
                    {
                        // Unknown object type
                        value = null;
                    }
                }
                else
                {
                    if (prop.Value.Type == JTokenType.Date)
                    {
                        value = prop.Value.Value<DateTimeOffset>();
                    }
                    else
                    {
                        var asString = prop.Value.ToString();
                        value = string.IsNullOrEmpty(asString)
                        ? null
                        : asString;
                    }
                }

                result.Add(name, value);
            }

            return result;
        }
    }
}
