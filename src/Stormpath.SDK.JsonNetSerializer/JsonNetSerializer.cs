// <copyright file="JsonNetSerializer.cs" company="Stormpath, Inc.">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Extensions.Serialization
{
    /// <summary>
    /// JSON.NET-based serializer for Stormpath.SDK.
    /// </summary>
    public sealed class JsonNetSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
        /// </summary>
        [Obsolete("Don't create serializers directly. Use Serializers.Create().Default() or Create().JsonNetSerializer() instead.")]
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
        /// Converts a nested tree of <see cref="JObject"/> instances into nested <see cref="IDictionary{TKey, TValue}">dictionaries</see>.
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
                var value = this.SanitizeToken(prop.Value);

                result.Add(name, value);
            }

            return result;
        }

        private object SanitizeToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    if (token.Children().Any() &&
                        token.Children().All(t => t.Type == JTokenType.Object))
                    {
                        // Collections of sub-objects get recursively sanitized
                        var nestedObjects = token.Children()
                            .Select(c => this.Sanitize((JObject)c))
                            .ToArray();
                        return nestedObjects;
                    }
                    else
                    {
                        return this.SanitizeArrayOfPrimitives(token);
                    }

                case JTokenType.Object:
                    return this.Sanitize((JObject)token);

                case JTokenType.Date:
                    return token.ToObject<DateTimeOffset>();

                case JTokenType.Integer:
                    var raw = token.ToString();
                    int intResult;
                    long longResult;

                    if (int.TryParse(raw, out intResult))
                    {
                        return intResult;
                    }
                    else if (long.TryParse(raw, out longResult))
                    {
                        return longResult;
                    }
                    else
                    {
                        return raw;
                    }

                case JTokenType.Boolean:
                    return bool.Parse(token.ToString());

                case JTokenType.Null:
                    return null;

                default:
                    return token.ToString();
            }
        }

        private object SanitizeArrayOfPrimitives(JToken token)
        {
            var boxedScalars = token.Children()
                .Select(c => this.SanitizeToken(c));

            var singleTokenType = GetSingleTokenType(token.Children());
            if (singleTokenType != null)
            {
                switch (singleTokenType)
                {
                    case JTokenType.Boolean:
                        return boxedScalars.Cast<bool>().ToArray();

                    case JTokenType.Date:
                        return boxedScalars.Cast<DateTimeOffset>().ToArray();

                    case JTokenType.Integer:
                        if (boxedScalars.All(x => x.GetType() == typeof(int)))
                        {
                            return boxedScalars.Cast<int>().ToArray();
                        }
                        else
                        {
                            return boxedScalars.Cast<long>().ToArray();
                        }

                    case JTokenType.String:
                        return boxedScalars.Cast<string>().ToArray();
                }
            }

            // Fall back to List<object> if consistent inner type cannot be determined
            return boxedScalars.ToArray();
        }

        private static JTokenType? GetSingleTokenType(JEnumerable<JToken> tokens)
        {
            if (!tokens.Any())
            {
                return null;
            }

            var firstType = tokens.First().Type;

            if (tokens.All(t => t.Type == firstType))
            {
                return firstType;
            }
            else
            {
                return null;
            }
        }
    }
}
