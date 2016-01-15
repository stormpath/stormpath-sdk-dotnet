// <copyright file="IInternalSyncDataStore.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Represents the synchronous actions a data store can perform.
    /// </summary>
    internal interface IInternalSyncDataStore : IInternalDataStore, IDataStoreSync
    {
        /// <summary>
        /// Gets a collection resource.
        /// </summary>
        /// <typeparam name="T">The inner resource type.</typeparam>
        /// <param name="href">The collection URL.</param>
        /// <returns>A <see cref="CollectionResponsePage{T}"/> containing a single page of results.</returns>
        CollectionResponsePage<T> GetCollection<T>(string href);

        /// <summary>
        /// Gets a resource, and apply a specified type lookup to the returned object.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="href">The resource URL.</param>
        /// <param name="typeLookup">The type lookup</param>
        /// <returns>The resource.</returns>
        T GetResource<T>(string href, Func<Map, Type> typeLookup)
            where T : class, IResource;

        /// <summary>
        /// Directly retrieves the resource at the specified <paramref name="href"/> URL and returns the resource
        /// as an instance of the specified class <typeparamref name="T"/>. The cache is not consulted for reads;
        /// but any returned value <b>is</b> cached.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource">Resource</see> value.</typeparam>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <returns>An instance of the specified class based on data returned from the specified <paramref name="href"/> URL.</returns>
        T GetResourceSkipCache<T>(string href);

        /// <summary>
        /// Creates a new resource on the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <returns>The persisted resource.</returns>
        T Create<T>(string parentHref, T resource)
            where T : class;

        /// <summary>
        /// Creates a new resource on the server with the specified options.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="options">The creation options to use for the request.</param>
        /// <returns>The persisted resource.</returns>
        T Create<T>(string parentHref, T resource, ICreationOptions options)
            where T : class;

        /// <summary>
        /// Creates a new resource on the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <typeparam name="TReturned">The resource type to return.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <returns>The persisted resource.</returns>
        TReturned Create<T, TReturned>(string parentHref, T resource)
            where T : class
            where TReturned : class;

        /// <summary>
        /// Creates a new resource on the server with the specified options.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <typeparam name="TReturned">The resource type to return.</typeparam>
        /// <param name="parentHref">The parent resource URL to send the creation request to.</param>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="headers">The HTTP headers to use for the request.</param>
        /// <returns>The persisted resource.</returns>
        TReturned Create<T, TReturned>(string parentHref, T resource, HttpHeaders headers)
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
        /// <param name="headers">The HTTP headers to use for the request.</param>
        /// <returns>The persisted resource.</returns>
        TReturned Create<T, TReturned>(string parentHref, T resource, ICreationOptions options, HttpHeaders headers)
            where T : class
            where TReturned : class;

        /// <summary>
        /// Saves local resource changes to the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resource">The resource to persist.</param>
        /// <returns>The updated resource.</returns>
        T Save<T>(T resource)
            where T : class, IResource, ISaveable<T>;

        /// <summary>
        /// Saves local resource changes to the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resource">The resource to persist.</param>
        /// <param name="queryString">The additional query string arguments to use in the request.</param>
        /// <returns>The updated resource.</returns>
        T Save<T>(T resource, string queryString)
            where T : class, IResource, ISaveable<T>;

        /// <summary>
        /// Deletes a resource from the server.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resource">The resource to delete.</param>
        /// <returns><see langword="true"/> if the delete operation succeeded; <see langword="false"/> otherwise.</returns>
        bool Delete<T>(T resource)
            where T : class, IResource, IDeletable;

        /// <summary>
        /// Deletes a single resource property.
        /// </summary>
        /// <param name="parentHref">The parent resource URL.</param>
        /// <param name="propertyName">The name of the property to delete</param>
        /// <returns><see langword="true"/> if the delete operation succeeded; <see langword="false"/> otherwise.</returns>
        bool DeleteProperty(string parentHref, string propertyName);
    }
}
