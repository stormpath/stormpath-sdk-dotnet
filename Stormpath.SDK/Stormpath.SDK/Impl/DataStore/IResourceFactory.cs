// <copyright file="IResourceFactory.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Creates local resource instances from <see cref="Map"/>s of properties.
    /// </summary>
    internal interface IResourceFactory : IDisposable
    {
        /// <summary>
        /// Creates an empty resource of the specified type,
        /// and links it to a previous instance of the resource.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="original">A previously-instantiated version of the resource to link to.</param>
        /// <returns>The new resource.</returns>
        T Create<T>(ILinkable original = null);

        /// <summary>
        /// Creates a resource of the specified type with the specified <paramref name="properties"/>,
        /// and links it to a previous instance of the resource.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="properties">The resource properties.</param>
        /// <param name="original">A previously-instantiated version of the resource to link to.</param>
        /// <returns>The new resource.</returns>
        T Create<T>(Map properties, ILinkable original = null);
    }
}
