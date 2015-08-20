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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:DoNotUseRegions", Justification = "Reviewed.")]
    internal sealed class InternalDataStore : IInternalDataStore, IDisposable
    {
        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly IMapSerializer mapMarshaller;
        private readonly IResourceFactory resourceFactory;

        private bool disposed = false;

        internal InternalDataStore(IRequestExecutor requestExecutor, string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;

            this.mapMarshaller = new JsonNetMapMarshaller();
            this.resourceFactory = new DefaultResourceFactory(this);
        }

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

        async Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            if (!href.StartsWith("https://"))
                href = $"{baseUrl}/{href}";

            Uri hrefUri;
            if (!Uri.TryCreate(href, UriKind.Absolute, out hrefUri))
                throw new RequestException($"The URI is not valid: {href}");

            var json = await requestExecutor.GetAsync(hrefUri, cancellationToken);

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

        #region IDisposable implementation

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

        #endregion
    }
}
