// <copyright file="DefaultDataStore.cs" company="Stormpath, Inc.">
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
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Error;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:DoNotUseRegions", Justification = "Reviewed.")]
    internal sealed class DefaultDataStore : IInternalDataStore, IDisposable
    {
        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly IMapSerializer serializer;
        private readonly IResourceConstructor resourceFactory;
        private readonly IResourceDeconstructor resourceConverter;

        private readonly UriCanonicalizer uriCanonicalizer;

        private bool disposed = false;

        internal DefaultDataStore(IRequestExecutor requestExecutor, string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;

            this.serializer = new JsonNetMapMarshaller();
            this.resourceFactory = new DefaultResourceConstructor(this);
            this.resourceConverter = new DefaultResourceDeconstructor();

            this.uriCanonicalizer = new UriCanonicalizer(baseUrl);
        }

        private IInternalDataStore This => this;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

        async Task<T> IDataStore.GetResourceAsync<T>(string resourcePath, CancellationToken cancellationToken)
        {
            var canonicalUri = uriCanonicalizer.Create(resourcePath);
            var request = new DefaultRequest(HttpMethod.Get, canonicalUri);
            var response = await SendToExecutorAsync(request, cancellationToken);
            var json = response.Body;

            var map = serializer.Deserialize(json, typeof(T));
            var resource = resourceFactory.Create<T>(map);

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

            var uri = uriCanonicalizer.Create(href, queryParams);
            var properties = resourceConverter.ToMap(abstractResource);

            // TODO move the following into filter chain?
            var body = serializer.Serialize(properties);

            var request = new DefaultRequest(HttpMethod.Post, uri, null, null, body);

            // var response = await requestExecutor.ExecuteAsync(request);
            throw new NotImplementedException();
        }

        private async Task<IHttpResponse> SendToExecutorAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            ApplyDefaultRequestHeaders(request);

            var response = await requestExecutor.ExecuteAsync(request, cancellationToken);

            if (response.IsError)
            {
                var error = new DefaultError(GetBody<IError>(response));
                throw new ResourceException(error);
            }

            return response;
        }

        private void ApplyDefaultRequestHeaders(IHttpRequest request)
        {
            request.Headers.Accept = "application/json";
            request.Headers.UserAgent = UserAgentBuilder.GetUserAgent();

            if (request.HasBody)
                request.Headers.ContentType = "application/json";
        }

        private Hashtable GetBody<T>(IHttpResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.HasBody)
                return serializer.Deserialize(response.Body, typeof(T));
            else
                return new Hashtable();
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
