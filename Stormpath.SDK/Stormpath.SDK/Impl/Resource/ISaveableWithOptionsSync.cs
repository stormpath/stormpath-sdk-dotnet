// <copyright file="ISaveableWithOptionsSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    /// <summary>
    /// Represents a resource that can be saved synchronously with additional options.
    /// </summary>
    /// <typeparam name="T">The <see cref="IResource"/> type.</typeparam>
    internal interface ISaveableWithOptionsSync<T> : ISaveableSync<T>
        where T : IResource
    {
        /// <summary>
        /// Synchronously reates or updates the resource, and returns the persisted resource data with the specified <paramref name="responseOptions"/>.
        /// </summary>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>The persisted resource data.</returns>
        /// <exception cref="Error.ResourceException">The save operation failed.</exception>
        T Save(Action<IRetrievalOptions<T>> responseOptions);
    }
}
