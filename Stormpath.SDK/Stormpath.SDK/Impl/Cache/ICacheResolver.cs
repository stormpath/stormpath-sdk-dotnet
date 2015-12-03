// <copyright file="ICacheResolver.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Impl.Cache
{
    /// <summary>
    /// Resolves resource types to cache regions.
    /// </summary>
    internal interface ICacheResolver
    {
        /// <summary>
        /// Gets a value indicating whether a synchronous cache path is supported.
        /// </summary>
        /// <value><see langword="true"/> if a synchronous cache path is supported; <see langword="false"/> otherwise.</value>
        bool IsSynchronousSupported { get; }

        /// <summary>
        /// Gets a value indicating whether an asynchronous cache path is supported.
        /// </summary>
        /// <value><see langword="true"/> if an asynchronous cache path is supported; <see langword="false"/> otherwise.</value>
        bool IsAsynchronousSupported { get; }

        /// <summary>
        /// Gets the synchronous cache region used to store <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The <see cref="ISynchronousCache"/> used to store the resource.</returns>
        ISynchronousCache GetSyncCache(Type resourceType);

        /// <summary>
        /// Gets the asynchronous cache region used to store <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The <see cref="IAsynchronousCache"/> used to store the resource.</returns>
        IAsynchronousCache GetAsyncCache(Type resourceType);
    }
}
