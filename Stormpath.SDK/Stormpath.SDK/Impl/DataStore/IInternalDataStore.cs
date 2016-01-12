// <copyright file="IInternalDataStore.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Represents the internal properties of a <see cref="IDataStore"/>.
    /// </summary>
    internal interface IInternalDataStore : IDataStore
    {
        /// <summary>
        /// Gets the <see cref="IRequestExecutor"/> used by this data store.
        /// </summary>
        /// <value>The <see cref="IRequestExecutor"/> used by this data store.</value>
        IRequestExecutor RequestExecutor { get; }

        /// <summary>
        /// Gets the <see cref="ICacheResolver"/> used by this data store.
        /// </summary>
        /// <value>The <see cref="ICacheResolver"/> used by this data store.</value>
        ICacheResolver CacheResolver { get; }

        /// <summary>
        /// Gets the <see cref="IJsonSerializer"/> used by this data store.
        /// </summary>
        /// <value>The <see cref="IJsonSerializer"/> used by this data store.</value>
        IJsonSerializer Serializer { get; }

        /// <summary>
        /// Gets the <see cref="IClient"/> that owns this data store.
        /// </summary>
        /// <value>The <see cref="IClient"/> that owns this data store.</value>
        IClient Client { get; }

        /// <summary>
        /// Gets the base URL of the remote server.
        /// </summary>
        /// <value>The base URL of the remote server.</value>
        string BaseUrl { get; }

        /// <summary>
        /// Gets the <see cref="IClientApiKey"/> used by this data store.
        /// </summary>
        /// <value>The <see cref="IClientApiKey"/> used by this data store.</value>
        IClientApiKey ApiKey { get; }

        /// <summary>
        /// Instantiates and returns a new instance of the specified <see cref="IResource"/> type,
        /// with the given <paramref name="href"/>
        /// The instance is merely instantiated and is not saved/synchronized with the server in any way.
        /// </summary>
        /// <typeparam name="T">The <see cref="IResource"/> type to instantiate.</typeparam>
        /// <param name="href">The <c>href</c> of the resource.</param>
        /// <returns>A new instance of the specified resource type</returns>
        T InstantiateWithHref<T>(string href)
            where T : IResource;

        /// <summary>
        /// Instantiates and returns a new instance of the specified <see cref="IResource"/> type,
        /// with the given <paramref name="properties"/>.
        /// The instance is merely instantiated and is not saved/synchronized with the server in any way.
        /// </summary>
        /// <typeparam name="T">The <see cref="IResource"/> type to instantiate.</typeparam>
        /// <param name="properties">The properties to initialize the object with.</param>
        /// <returns>A new instance of the specified resource type</returns>
        T InstantiateWithData<T>(Map properties);
    }
}
