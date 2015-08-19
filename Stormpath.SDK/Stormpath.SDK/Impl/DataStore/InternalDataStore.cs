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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class InternalDataStore : IDataStore, IDisposable
    {
        private readonly IRequestExecutor requestExecutor;
        private readonly IMapSerializer mapMarshaller;
        private readonly IResourceFactory resourceFactory;

        private bool disposed = false;

        public InternalDataStore()
            : this(new NetHttpRequestExecutor())
        {
        }

        internal InternalDataStore(IRequestExecutor requestExecutor)
        {
            this.requestExecutor = requestExecutor;
            this.mapMarshaller = new JsonNetMapMarshaller();
            this.resourceFactory = new DefaultResourceFactory(this);
        }

        async Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            var json = await requestExecutor.GetAsync(href, cancellationToken);

            var map = mapMarshaller.Deserialize(json);
            var resource = resourceFactory.Instantiate<T>(map);

            return resource;
        }

        Task<CollectionResponsePageDto<T>> IDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IDataStore.Save(IResource resource)
        {
            throw new NotImplementedException();
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    requestExecutor.Dispose();
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
