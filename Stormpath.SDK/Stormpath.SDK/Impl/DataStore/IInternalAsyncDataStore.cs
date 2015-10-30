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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal interface IInternalAsyncDataStore : IInternalDataStore
    {
        Task<CollectionResponsePage<T>> GetCollectionAsync<T>(string href, CancellationToken cancellationToken);

        Task<T> GetResourceAsync<T>(string href, Func<IDictionary<string, object>, Type> typeLookup, CancellationToken cancellationToken)
            where T : class, IResource;

        Task<T> GetResourceAsync<T>(string href, IdentityMapOptions identityMapOptions, CancellationToken cancellationToken)
            where T : class, IResource;

        Task<T> CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
            where T : class;

        Task<T> CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            where T : class;

        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
            where T : class
            where TReturned : class;

        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            where T : class
            where TReturned : class;

        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, IdentityMapOptions identityMapOptions, CancellationToken cancellationToken)
            where T : class
            where TReturned : class;

        Task<T> SaveAsync<T>(T resource, CancellationToken cancellationToken)
            where T : class, IResource, ISaveable<T>;

        Task<T> SaveAsync<T>(T resource, string queryString, CancellationToken cancellationToken)
            where T : class, IResource, ISaveable<T>;

        Task<bool> DeleteAsync<T>(T resource, CancellationToken cancellationToken)
            where T : class, IResource, IDeletable;

        Task<bool> DeletePropertyAsync(string parentHref, string propertyName, CancellationToken cancellationToken);
    }
}
