// <copyright file="IJsonSerializer.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;

namespace Stormpath.SDK.Serialization
{
    /// <summary>
    /// A wrapper interface for JSON serialization plugins.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes the specified properties to JSON.
        /// </summary>
        /// <param name="map">The property data to serialize.</param>
        /// <returns>A JSON string representation of <paramref name="map"/></returns>
        string Serialize(IDictionary<string, object> map);

        /// <summary>
        /// Deserializes a JSON string to a property dictionary.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A tree of name-value pairs stored in an <see cref="IDictionary{TKey, TValue}"/>.</returns>
        IDictionary<string, object> Deserialize(string json);
    }
}
