// <copyright file="IInternalAsyncDataStore.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Represents the asynchronous actions a data store can perform.
    /// </summary>
    internal interface IInternalAsyncDataStore : IInternalDataStore
    {
        /// <summary>
        /// Gets a collection resource.
        /// </summary>
        /// <typeparam name="T">The inner resource type.</typeparam>
        /// <param name="href">The collection URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="CollectionResponsePage{T}"/> containing a single page of results.</returns>
        Task<CollectionResponsePage<T>> GetCollectionAsync<T>(string href, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a resource, and apply a specified type lookup to the returned object.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="href">The resource URL.</param>
        /// <param name="typeLookup">The type lookup</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resource.</returns>
        Task<T> GetResourceAsync<T>(string href, Func<Map, Type> typeLookup, CancellationToken cancellationToken)
            where T : class, IResource;

        /// <summary>
        /// Directly retrieves the resource at the specified <paramref name="href"/> URL and returns the resource
        /// as an instance of the specified class <typeparamref name="T"/>. The cache is not consulted for reads;
        /// but any returned value <b>is</b> cached.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource"/> value.</typeparam>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An instance of the specified class based on data returned from the specified <paramref name="href"/> URL.</returns>
        Task<T> GetResourceSkipCacheAsync<T>(string href, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new resource on the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted resource.</returns>
        Task<T> CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
            where T : class;

        /// <summary>
        /// Creates a new resource on the server with the specified options.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="options">The creation options to use for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted resource.</returns>
        Task<T> CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            where T : class;

        /// <summary>
        /// Creates a new resource on the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <typeparam name="TReturned">The resource type to return.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted resource.</returns>
        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
            where T : class
            where TReturned : class;

        /// <summary>
        /// Creates a new resource on the server with the specified options.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <typeparam name="TReturned">The resource type to return.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="options">The creation options to use for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted resource.</returns>
        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            where T : class
            where TReturned : class;

        /// <summary>
        /// Saves local resource changes to the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated resource.</returns>
        Task<T> SaveAsync<T>(T resource, CancellationToken cancellationToken)
            where T : class, IResource, ISaveable<T>;

        /// <summary>
        /// Saves local resource changes to the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="queryString">The additional query string arguments to use in the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated resource.</returns>
        Task<T> SaveAsync<T>(T resource, string queryString, CancellationToken cancellationToken)
            where T : class, IResource, ISaveable<T>;

        /// <summary>
        /// Deletes a resource from the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resource">The resource to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see langword="true"/> if the delete operation succeeded; <see langword="false"/> otherwise.</returns>
        Task<bool> DeleteAsync<T>(T resource, CancellationToken cancellationToken)
            where T : class, IResource, IDeletable;

        /// <summary>
        /// Deletes a single resource property.
        /// </summary>
        /// <param name="parentHref">The parent resource URL.</param>
        /// <param name="propertyName">The name of the property to delete</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see langword="true"/> if the delete operation succeeded; <see langword="false"/> otherwise.</returns>
        Task<bool> DeletePropertyAsync(string parentHref, string propertyName, CancellationToken cancellationToken);
    }
}
