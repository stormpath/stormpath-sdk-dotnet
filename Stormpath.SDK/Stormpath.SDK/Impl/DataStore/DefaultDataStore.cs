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
    internal sealed class DefaultDataStore : IInternalDataStore, IDisposable
    {
        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly IMapSerializer serializer;
        private readonly IResourceFactory resourceFactory;
        private readonly IResourceConverter resourceConverter;

        private readonly UriQualifier uriQualifier;

        private bool disposed = false;

        internal DefaultDataStore(IRequestExecutor requestExecutor, string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;

            this.serializer = new JsonNetMapMarshaller();
            this.resourceFactory = new DefaultResourceFactory(this);
            this.resourceConverter = new DefaultResourceConverter();

            this.uriQualifier = new UriQualifier(baseUrl);
        }

        private IInternalDataStore IThis => this;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

        T IDataStore.Instantiate<T>()
        {
            return this.resourceFactory.Create<T>();
        }

        async Task<T> IDataStore.GetResourceAsync<T>(string resourcePath, CancellationToken cancellationToken)
        {
            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(resourcePath));
            var request = new DefaultHttpRequest(HttpMethod.Get, canonicalUri);
            var response = await this.SendToExecutorAsync(request, cancellationToken).ConfigureAwait(false);
            var json = response.Body;

            var map = this.serializer.Deserialize(json, typeof(T));
            var resource = this.resourceFactory.Create<T>(map);

            return resource;
        }

        Task<CollectionResponsePage<T>> IDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            return this.IThis.GetResourceAsync<CollectionResponsePage<T>>(href, cancellationToken);
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.IThis.CreateAsync(parentHref, resource, options: null, cancellationToken: cancellationToken);
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            QueryString queryParams = null;
            if (options != null)
                queryParams = new QueryString(options.GetQueryString());

            return this.SaveCoreAsync(
                resource, parentHref,
                queryParams: queryParams,
                cancellationToken: cancellationToken);
        }

        Task<T> IInternalDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            // var href = resource?.Href;
            // return SaveCoreAsync(resource, href, null);
        }

        Task<bool> IInternalDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
        {
            return this.DeleteCoreAsync(resource, cancellationToken);
        }

        private async Task<T> SaveCoreAsync<T>(T resource, string href, QueryString queryParams, CancellationToken cancellationToken)
            where T : IResource
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            var uri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href), queryParams);
            var propertiesMap = this.resourceConverter.ToMap(abstractResource, partialUpdate: false);
            var body = this.serializer.Serialize(propertiesMap);

            var request = new DefaultHttpRequest(HttpMethod.Post, uri, null, null, body, "application/json");

            var response = await this.SendToExecutorAsync(request, cancellationToken).ConfigureAwait(false);
            var map = GetBody<T>(response);
            var createdResource = this.resourceFactory.Create<T>(map);

            return createdResource;
        }

        private async Task<bool> DeleteCoreAsync<T>(T resource, CancellationToken cancellationToken)
            where T : IResource, IDeletable
        {
            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            if (string.IsNullOrEmpty(resource.Href))
                throw new ArgumentNullException(nameof(resource.Href));

            var uri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(resource.Href));
            var request = new DefaultHttpRequest(HttpMethod.Delete, uri);

            var response = await this.SendToExecutorAsync(request, cancellationToken).ConfigureAwait(false);
            return response.HttpStatus == 204;
        }

        private async Task<IHttpResponse> SendToExecutorAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            this.ApplyDefaultRequestHeaders(request);

            var response = await this.requestExecutor
                .ExecuteAsync(request, cancellationToken)
                .ConfigureAwait(false);

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
        }

        private Hashtable GetBody<T>(IHttpResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.HasBody)
                return this.serializer.Deserialize(response.Body, typeof(T));
            else
                return new Hashtable();
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.requestExecutor.Dispose();
                }

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
