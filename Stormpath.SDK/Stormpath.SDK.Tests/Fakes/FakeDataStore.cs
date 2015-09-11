// <copyright file="FakeDataStore.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tests.Fakes
{
    // TODO: Make this an actual server with valid responses
    public sealed class FakeDataStore<TType> : IInternalDataStore
    {
        private static int defaultLimit = 25;
        private static int defaultOffset = 0;

        private readonly List<string> calls = new List<string>();

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

        string IInternalDataStore.BaseUrl
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

        async Task<CollectionResponsePage<T>> IInternalDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            bool typesMatch = typeof(T) == typeof(TType);
            if (!typesMatch)
                throw new ArgumentException("Requested type must match type of fake data.");

            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
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

        private static Regex limitRegex = new Regex(@"limit=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static int? GetLimitFromUrlString(string href)
        {
            var match = limitRegex.Match(href);
            if (!match.Success)
                return null;
            if (string.IsNullOrEmpty(match.Groups?[1].Value))
                return null;

            return int.Parse(match.Groups[1].Value);
        }

        private static Regex offsetRegex = new Regex(@"offset=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static int? GetOffsetFromUrlString(string href)
        {
            var match = offsetRegex.Match(href);
            if (!match.Success)
                return null;
            if (string.IsNullOrEmpty(match.Groups?[1].Value))
                return null;

            return int.Parse(match.Groups[1].Value);
        }

        T IDataStore.Instantiate<T>()
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<T> IInternalDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<bool> IInternalDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        T IInternalDataStore.GetResource<T>(string href)
        {
            throw new NotImplementedException();
        }

        CollectionResponsePage<T> IInternalDataStore.GetCollection<T>(string href)
        {
            throw new NotImplementedException();
        }

        T IInternalDataStore.Create<T>(string parentHref, T resource)
        {
            throw new NotImplementedException();
        }

        T IInternalDataStore.Create<T>(string parentHref, T resource, ICreationOptions options)
        {
            throw new NotImplementedException();
        }

        TReturned IInternalDataStore.Create<T, TReturned>(string parentHref, T resource)
        {
            throw new NotImplementedException();
        }

        TReturned IInternalDataStore.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options)
        {
            throw new NotImplementedException();
        }

        T IInternalDataStore.Save<T>(T resource)
        {
            throw new NotImplementedException();
        }

        bool IInternalDataStore.Delete<T>(T resource)
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

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
