// <copyright file="FakeDataStore{TType}.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Tests.Fakes
{
    internal sealed class FakeDataStore<TType> : IInternalDataStore, IInternalAsyncDataStore, IInternalSyncDataStore
    {
        private static int defaultLimit = 25;
        private static int defaultOffset = 0;

        private readonly List<string> calls = new List<string>();

        public FakeDataStore()
        {
            this.Items = Enumerable.Empty<TType>().ToList();
        }

        public FakeDataStore(IEnumerable<TType> items)
        {
            this.Items = items.ToList();
        }

        public List<TType> Items { get; private set; }

        IRequestExecutor IInternalDataStore.RequestExecutor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ICacheResolver IInternalDataStore.CacheResolver
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IJsonSerializer IInternalDataStore.Serializer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IInternalDataStore.BaseUrl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IClientApiKey IInternalDataStore.ApiKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> GetCalls()
        {
            return this.calls;
        }

        private IInternalDataStore AsInterface => this;

        private IInternalSyncDataStore AsSyncInterface => this;

        async Task<CollectionResponsePage<T>> IInternalAsyncDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();

            return this.AsSyncInterface.GetCollection<T>(href);
        }

        CollectionResponsePage<T> IInternalSyncDataStore.GetCollection<T>(string href)
        {
            bool typesMatch = typeof(T) == typeof(TType);
            if (!typesMatch)
            {
                throw new ArgumentException("Requested type must match type of fake data.");
            }

            this.calls.Add(href);

            var limit = GetLimitFromUrlString(href) ?? defaultLimit;
            var offset = GetOffsetFromUrlString(href) ?? defaultOffset;

            return new CollectionResponsePage<T>()
            {
                Href = href,
                Items = this.Items.OfType<T>().Skip(offset).Take(limit).ToList(),
                Limit = limit,
                Offset = offset,
                Size = this.Items.Count
            };
        }

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<T> IDataStore.GetResourceAsync<T>(string href, Action<IRetrievalOptions<T>> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static Regex limitRegex = new Regex(@"limit=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static int? GetLimitFromUrlString(string href)
        {
            var match = limitRegex.Match(href);
            if (!match.Success)
            {
                return null;
            }

            if (string.IsNullOrEmpty(match.Groups?[1].Value))
            {
                return null;
            }

            return int.Parse(match.Groups[1].Value);
        }

        private static Regex offsetRegex = new Regex(@"offset=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static int? GetOffsetFromUrlString(string href)
        {
            var match = offsetRegex.Match(href);
            if (!match.Success)
            {
                return null;
            }

            if (string.IsNullOrEmpty(match.Groups?[1].Value))
            {
                return null;
            }

            return int.Parse(match.Groups[1].Value);
        }

        T IDataStore.Instantiate<T>()
        {
            throw new NotImplementedException();
        }

        T IInternalDataStore.InstantiateWithHref<T>(string href)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<bool> IInternalAsyncDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        T IDataStoreSync.GetResource<T>(string href)
        {
            throw new NotImplementedException();
        }

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource)
        {
            throw new NotImplementedException();
        }

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource, ICreationOptions options)
        {
            throw new NotImplementedException();
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource)
        {
            throw new NotImplementedException();
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options)
        {
            throw new NotImplementedException();
        }

        T IInternalSyncDataStore.Save<T>(T resource)
        {
            throw new NotImplementedException();
        }

        bool IInternalSyncDataStore.Delete<T>(T resource)
        {
            throw new NotImplementedException();
        }

        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                this.disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        Task<bool> IInternalAsyncDataStore.DeletePropertyAsync(string parentHref, string propertyName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

        T IInternalDataStore.InstantiateWithData<T>(IDictionary<string, object> properties)
        {
            throw new NotImplementedException();
        }

        public T GetResource<T>(string href, Action<IRetrievalOptions<T>> options)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, string queryString, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        T IInternalSyncDataStore.Save<T>(T resource, string queryString)
        {
            throw new NotImplementedException();
        }

        public ICacheResolver GetCacheResolver()
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

        T IDataStoreSync.GetResource<T>(string href, Action<IRetrievalOptions<T>> responseOptions)
        {
            throw new NotImplementedException();
        }
    }
}
