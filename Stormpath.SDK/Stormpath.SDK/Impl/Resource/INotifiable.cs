// <copyright file="INotifiable.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.DataStore;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Resource
{
    /// <summary>
    /// Represents an update callback that can be fired.
    /// </summary>
    internal interface INotifiable
    {
        /// <summary>
        /// Notifies the target object that an update is occurring.
        /// </summary>
        /// <param name="properties">The new resource properties.</param>
        /// <param name="dataStore">The parent data store.</param>
        void OnUpdate(Map properties, IInternalDataStore dataStore);
    }
}
