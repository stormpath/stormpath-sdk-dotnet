// <copyright file="StubDataStore.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Tests.Fakes
{
    public sealed class StubDataStore : IInternalDataStore, IInternalAsyncDataStore, IInternalSyncDataStore, IDisposable
    {
        private readonly DefaultDataStore proxyInstance;

        private IInternalDataStore ProxyDataStore => this.proxyInstance;

        private IInternalAsyncDataStore ProxyAsyncDataStore => this.proxyInstance;

        private IInternalSyncDataStore ProxySyncDataStore => this.proxyInstance;

        public StubDataStore(string resourceJson, string baseHref, ILogger logger = null)
        {
            var fakeRequestExecutor = new StubRequestExecutor(resourceJson);
            var useLogger = logger == null
                ? new SDK.Impl.Logging.NullLogger()
                : logger;

            this.proxyInstance = new DefaultDataStore(fakeRequestExecutor.Object, baseHref, new JsonNetSerializer(), useLogger, new FakeUserAgentBuilder(), new NullCacheProvider(), TimeSpan.FromMinutes(10));
        }

        string IInternalDataStore.BaseUrl => this.ProxyDataStore.BaseUrl;

        IClientApiKey IInternalDataStore.ApiKey => this.ProxyDataStore.ApiKey;

        IRequestExecutor IInternalDataStore.RequestExecutor
            => this.ProxyDataStore.RequestExecutor;

        ICacheResolver IInternalDataStore.CacheResolver
            => this.ProxyDataStore.CacheResolver;

        IJsonSerializer IInternalDataStore.Serializer
            => this.ProxyDataStore.Serializer;

        T IDataStoreSync.GetResource<T>(string href)
            => this.ProxySyncDataStore.GetResource<T>(href);

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
            => this.ProxyDataStore.GetResourceAsync<T>(href, cancellationToken);

        Task<T> IDataStore.GetResourceAsync<T>(string href, Action<IRetrievalOptions<T>> options, CancellationToken cancellationToken)
            => this.ProxyDataStore.GetResourceAsync<T>(href, options, cancellationToken);

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource)
            => this.ProxySyncDataStore.Create(parentHref, resource);

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource, ICreationOptions options)
            => this.ProxySyncDataStore.Create(parentHref, resource, options);

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource)
            => this.ProxySyncDataStore.Create<T, TReturned>(parentHref, resource);

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options)
            => this.ProxySyncDataStore.Create<T, TReturned>(parentHref, resource);

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.CreateAsync(parentHref, resource, cancellationToken);

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.CreateAsync(parentHref, resource, options, cancellationToken);

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.CreateAsync<T, TReturned>(parentHref, resource, cancellationToken);

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.CreateAsync<T, TReturned>(parentHref, resource, options, cancellationToken);

        bool IInternalSyncDataStore.Delete<T>(T resource)
            => this.ProxySyncDataStore.Delete(resource);

        Task<bool> IInternalAsyncDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.DeleteAsync(resource, cancellationToken);

        Task<bool> IInternalAsyncDataStore.DeletePropertyAsync(string parentHref, string propertyName, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.DeletePropertyAsync(parentHref, propertyName, cancellationToken);

        CollectionResponsePage<T> IInternalSyncDataStore.GetCollection<T>(string href)
            => this.ProxySyncDataStore.GetCollection<T>(href);

        Task<CollectionResponsePage<T>> IInternalAsyncDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.GetCollectionAsync<T>(href, cancellationToken);

        T IDataStore.Instantiate<T>()
            => this.ProxyDataStore.Instantiate<T>();

        T IInternalDataStore.InstantiateWithHref<T>(string href)
            => this.ProxyDataStore.InstantiateWithHref<T>(href);

        T IInternalSyncDataStore.Save<T>(T resource)
            => this.ProxySyncDataStore.Save(resource);

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.SaveAsync(resource, cancellationToken);

        T IInternalSyncDataStore.Save<T>(T resource, string queryString)
            => this.ProxySyncDataStore.Save(resource, queryString);

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, string queryString, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.SaveAsync(resource, queryString, cancellationToken);

        bool IInternalSyncDataStore.DeleteProperty(string parentHref, string propertyName)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalAsyncDataStore.GetResourceAsync<T>(string href, Func<IDictionary<string, object>, Type> typeLookup, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        T IInternalSyncDataStore.GetResource<T>(string href, Func<IDictionary<string, object>, Type> typeLookup)
        {
            throw new NotImplementedException();
        }

        T IInternalDataStore.InstantiateWithData<T>(IDictionary<string, object> properties)
            => this.ProxyDataStore.InstantiateWithData<T>(properties);

        private bool isDisposed = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.ProxyDataStore.Dispose();
                }

                this.isDisposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        T IDataStoreSync.GetResource<T>(string href, Action<IRetrievalOptions<T>> options)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalAsyncDataStore.GetResourceSkipCacheAsync<T>(string href, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        T IInternalSyncDataStore.GetResourceSkipCache<T>(string href)
        {
            throw new NotImplementedException();
        }
    }
}
