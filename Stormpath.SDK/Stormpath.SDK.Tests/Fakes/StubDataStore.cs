// <copyright file="StubDataStore.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Shared;

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
                ? new SDK.Impl.NullLogger()
                : logger;

            this.proxyInstance = new DefaultDataStore(fakeRequestExecutor.Object, baseHref, new JsonNetSerializer(), useLogger, new NullCacheProvider());
        }

        string IInternalDataStore.BaseUrl => this.ProxyDataStore.BaseUrl;

        IRequestExecutor IInternalDataStore.RequestExecutor
            => this.ProxyDataStore.RequestExecutor;

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

        T IDataStoreSync.GetResource<T>(string href)
            => this.ProxySyncDataStore.GetResource<T>(href);

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
            => this.ProxyDataStore.GetResourceAsync<T>(href, cancellationToken);

        T IDataStore.Instantiate<T>()
            => this.ProxyDataStore.Instantiate<T>();

        T IInternalDataStore.InstantiateWithHref<T>(string href)
            => this.ProxyDataStore.InstantiateWithHref<T>(href);

        T IInternalSyncDataStore.Save<T>(T resource)
            => this.ProxySyncDataStore.Save(resource);

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
            => this.ProxyAsyncDataStore.SaveAsync(resource, cancellationToken);

#pragma warning disable SA1124 // Do not use regions
        #region IDisposable Support
#pragma warning restore SA1124 // Do not use regions

        private bool isDisposed = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.proxyInstance.Dispose();
                }

                this.isDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

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
        #endregion
    }
}
