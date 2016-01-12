// <copyright file="IDataStoreSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="SDK.DataStore.IDataStore"/>.
    /// </summary>
    internal interface IDataStoreSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.DataStore.IDataStore.GetResourceAsync{T}(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource"/> value.</typeparam>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <returns>The Resource.</returns>
        T GetResource<T>(string href);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.DataStore.IDataStore.GetResourceAsync{T}(string, Action{IRetrievalOptions{T}}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource"/> value.</typeparam>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>The Resource.</returns>
        T GetResource<T>(string href, Action<IRetrievalOptions<T>> responseOptions);
    }
}
