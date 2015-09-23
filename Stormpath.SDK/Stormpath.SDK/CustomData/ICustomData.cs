// <copyright file="ICustomData.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.CustomData
{
    /// <summary>
    /// A dictionary resource embedded within another resource
    /// (such as an <see cref="Account.IAccount"/> or <see cref="Directory.IDirectory"/>)
    /// that allows you to store arbitrary name/value pairs.
    /// </summary>
    public interface ICustomData : IResource, ISaveable<ICustomData>, IDeletable, IAuditable, IEnumerable<KeyValuePair<string, object>>
    {
        object this[string key] { get; set; }

        bool ContainsKey(string key);

        object Get(string key);

        IReadOnlyCollection<string> Keys { get; }

        void Put(string key, object value);

        void Put(IDictionary<string, object> values);

        void Put(KeyValuePair<string, object> item);

        object Remove(string key);

        bool TryGetValue(string key, out object value);

        IReadOnlyCollection<object> Values { get; }

        bool IsEmptyOrDefault();
    }
}
