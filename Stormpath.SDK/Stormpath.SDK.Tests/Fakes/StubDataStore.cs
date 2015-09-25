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
    public sealed class StubDataStore : IInternalDataStore, IInternalDataStoreSync, IDisposable
    {
        private readonly DefaultDataStore instance;

        private IInternalDataStore FakeDataStore => this.instance;

        private IInternalDataStoreSync FakeDataStoreSync => this.instance;

        public StubDataStore(string resourceJson, string baseHref, ILogger logger = null)
        {
            var fakeRequestExecutor = new StubRequestExecutor(resourceJson);
            var useLogger = logger == null
                ? new SDK.Impl.NullLogger()
                : logger;

            this.instance = new DefaultDataStore(fakeRequestExecutor.Object, baseHref, new JsonNetSerializer(), useLogger, new NullCacheProvider());
        }

        string IInternalDataStore.BaseUrl => this.FakeDataStore.BaseUrl;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.FakeDataStore.RequestExecutor;

        T IInternalDataStoreSync.Create<T>(string parentHref, T resource) => this.FakeDataStoreSync.Create(parentHref, resource);

        T IInternalDataStoreSync.Create<T>(string parentHref, T resource, ICreationOptions options) => this.FakeDataStoreSync.Create(parentHref, resource, options);

        TReturned IInternalDataStoreSync.Create<T, TReturned>(string parentHref, T resource) => this.FakeDataStoreSync.Create<T, TReturned>(parentHref, resource);

        TReturned IInternalDataStoreSync.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options) => this.FakeDataStoreSync.Create<T, TReturned>(parentHref, resource);

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken) => this.FakeDataStore.CreateAsync(parentHref, resource, cancellationToken);

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken) => this.FakeDataStore.CreateAsync(parentHref, resource, options, cancellationToken);

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken) => this.FakeDataStore.CreateAsync<T, TReturned>(parentHref, resource, cancellationToken);

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken) => this.FakeDataStore.CreateAsync<T, TReturned>(parentHref, resource, options, cancellationToken);

        bool IInternalDataStoreSync.Delete<T>(T resource) => this.FakeDataStoreSync.Delete(resource);

        Task<bool> IInternalDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken) => this.FakeDataStore.DeleteAsync(resource, cancellationToken);

        Task<bool> IInternalDataStore.DeletePropertyAsync(string parentHref, string propertyName, CancellationToken cancellationToken)
            => this.FakeDataStore.DeletePropertyAsync(parentHref, propertyName, cancellationToken);

        CollectionResponsePage<T> IInternalDataStoreSync.GetCollection<T>(string href) => this.FakeDataStoreSync.GetCollection<T>(href);

        Task<CollectionResponsePage<T>> IInternalDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken) => this.FakeDataStore.GetCollectionAsync<T>(href, cancellationToken);

        T IDataStoreSync.GetResource<T>(string href) => this.FakeDataStoreSync.GetResource<T>(href);

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken) => this.FakeDataStore.GetResourceAsync<T>(href, cancellationToken);

        T IDataStore.Instantiate<T>() => this.FakeDataStore.Instantiate<T>();

        T IInternalDataStoreSync.Save<T>(T resource) => this.FakeDataStoreSync.Save(resource);

        Task<T> IInternalDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken) => this.FakeDataStore.SaveAsync(resource, cancellationToken);

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
                    this.instance.Dispose();
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
        #endregion
    }
}
