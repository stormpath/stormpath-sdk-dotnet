// <copyright file="InternalDataStore.cs" company="Stormpath, Inc.">
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class InternalDataStore : IDataStore, IDisposable
    {
        private readonly IResourceFactory resourceFactory;
        private readonly IRequestExecutor requestExecutor;
        private readonly HttpClient client;

        private bool disposed = false;

        public InternalDataStore()
        {
            client = new HttpClient();
        }

        async Task<T> IDataStore.GetResource<T>(string href, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(href, cancellationToken);

            var content = await response.Content.ReadAsStringAsync();

            // var obj =
            throw new NotImplementedException();
        }

        Task<CollectionResponsePageDto<T>> IDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
