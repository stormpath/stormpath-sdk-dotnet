// <copyright file="StormpathJsonConverter.cs" company="Stormpath, Inc.">
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
using Newtonsoft.Json;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Deseralizes JSON from the Stormpath API. Expects that all returned fields are:
    /// primitives, or embedded Link objects with a depth of 1.
    /// </summary>
    internal sealed class StormpathJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Hashtable);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var resultMap = new Hashtable();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value.ToString();
                    object propertyValue = null;
                    reader.Read();

                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject:
                            reader.Read();
                            if (reader.TokenType == JsonToken.PropertyName
                                && reader.Value.ToString().Equals("href", StringComparison.OrdinalIgnoreCase))
                            {
                                propertyValue = new LinkProperty(reader.ReadAsString());
                            }

                            break;
                        default:
                            propertyValue = reader.Value;
                            break;
                    }

                    resultMap.Add(propertyName, propertyValue);
                }
            }

            return resultMap;
        }
    }
}
