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
using Stormpath.SDK.Impl.Http.Support;
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
        private readonly IResourceConverter resourceConverter;

        private readonly UriCanonicalizer uriCanonicalizer;

        private bool disposed = false;

        internal InternalDataStore(IRequestExecutor requestExecutor, string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;

            this.mapMarshaller = new JsonNetMapMarshaller();
            this.resourceFactory = new DefaultResourceFactory(this);
            this.resourceConverter = new DefaultResourceConverter();

            this.uriCanonicalizer = new UriCanonicalizer(baseUrl);
        }

        private IInternalDataStore This => this;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

        async Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            if (!href.StartsWith("http"))
                href = $"{baseUrl}/{href}";

            var hrefUri = new Uri(href);
            if (!hrefUri.IsWellFormedOriginalString())
                throw new RequestException($"The URI is not valid: {href}");

            var json = await requestExecutor.GetAsync(hrefUri, cancellationToken);

            var map = mapMarshaller.Deserialize(json, typeof(T));
            var resource = resourceFactory.Instantiate<T>(map);

            return resource;
        }

        Task<CollectionResponsePage<T>> IDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            return This.GetResourceAsync<CollectionResponsePage<T>>(href, cancellationToken);
        }

        Task<T> IDataStore.Save<T>(T resource)
        {
            throw new NotImplementedException();
        }

        Task IDataStore.Save(IResource resource)
        {
            var href = resource?.Href;
            return Save(resource, href, null);
        }

        private Task<AbstractResource> Save(IResource resource, string href, QueryString queryParams)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            var uri = uriCanonicalizer.Canonicalize(href, queryParams);
            var properties = resourceConverter.Convert(abstractResource);

            // TODO move the following into filter chain?
            var body = mapMarshaller.Serialize(properties);

            // var response = await requestExecutor.ExecuteAsync(request);
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
