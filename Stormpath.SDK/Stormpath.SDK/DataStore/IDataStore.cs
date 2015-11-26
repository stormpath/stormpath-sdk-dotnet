// <copyright file="IDataStore.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.DataStore
{
    /// <summary>
    /// A <see cref="IDataStore"/> is the liaison between client SDK components and the raw Stormpath REST API.
    /// It is responsible for converting SDK objects (<see cref="Account.IAccount"/>, <see cref="Application.IApplication"/>, <see cref="Directory.IDirectory"/>, etc.)
    /// into REST HTTP requests, executing those requests, and converting REST HTTP responses back into SDK objects.
    /// </summary>
    public interface IDataStore : IDisposable
    {
        /// <summary>
        /// Instantiates and returns a new instance of the specified <see cref="IResource"/> type.
        /// The instance is merely instantiated and is not saved/synchronized with the server in any way.
        /// <para>This method effectively replaces the <c>new</c> keyword that would have been used otherwise if the
        /// concrete implementation was known (Resource implementation classes are intentionally not exposed to SDK end-users).</para>
        /// </summary>
        /// <typeparam name="T">The <see cref="IResource"/> type to instantiate.</typeparam>
        /// <returns>A new instance of the specified resource type</returns>
        T Instantiate<T>()
            where T : IResource;

        /// <summary>
        /// Retrieves the resource at the specified <paramref name="href"/> URL and returns the resource
        /// as an instance of the specified class <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource"/> value.</typeparam>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An instance of the specified class based on data returned from the specified <paramref name="href"/> URL.</returns>
        Task<T> GetResourceAsync<T>(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the resource at the specified <paramref name="href"/> URL with the specified <see cref="options"/>,
        /// and returns the resource as an instance of the specified class <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource"/> value.</typeparam>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An instance of the specified class based on data returned from the specified <paramref name="href"/> URL.</returns>
        Task<T> GetResourceAsync<T>(string href, Action<IRetrievalOptions<T>> responseOptions, CancellationToken cancellationToken = default(CancellationToken));
    }
}
