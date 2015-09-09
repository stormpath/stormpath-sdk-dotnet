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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.DataStore.Cache;
using Stormpath.SDK.Impl.DataStore.FilterChain;
using Stormpath.SDK.Impl.Error;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultDataStore : IInternalDataStore, IDisposable
    {
        private readonly ILogger logger;

        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly ICacheManager cacheManager;
        private readonly ICacheResolver cacheResolver;
        private readonly IMapSerializer serializer;
        private readonly IResourceFactory resourceFactory;
        private readonly IResourceConverter resourceConverter;
        private readonly IAsynchronousFilterChain defaultAsyncFilters;
        private readonly ISynchronousFilterChain defaultSyncFilters;
        private readonly UriQualifier uriQualifier;

        private bool disposed = false;

        private IInternalDataStore AsInterface => this;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

#pragma warning disable SA1124 // Do not use regions
        #region Constructor and setup
#pragma warning restore SA1124 // Do not use regions

        internal DefaultDataStore(IRequestExecutor requestExecutor, string baseUrl, ILogger logger)
            : this(requestExecutor, baseUrl, logger, new NullCacheManager())
        {
        }

        internal DefaultDataStore(IRequestExecutor requestExecutor, string baseUrl, ILogger logger, ICacheManager cacheManager)
        {
            if (requestExecutor == null)
                throw new ArgumentNullException(nameof(requestExecutor));
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager), "Use NullCacheManager if you wish to turn off caching.");

            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;
            this.cacheManager = cacheManager;
            this.cacheResolver = new DefaultCacheResolver(this.cacheManager, new DefaultCacheRegionNameResolver());

            this.serializer = new JsonNetMapMarshaller();
            this.resourceFactory = new DefaultResourceFactory(this);
            this.resourceConverter = new DefaultResourceConverter();

            this.uriQualifier = new UriQualifier(baseUrl);
            this.logger = logger;

            this.defaultAsyncFilters = this.BuildDefaultAsyncFilterChain();
            this.defaultSyncFilters = this.BuildDefaultSyncFilterChain();
        }

        private IAsynchronousFilterChain BuildDefaultAsyncFilterChain()
        {
            var asyncFilterChain = new DefaultAsynchronousFilterChain();

            return asyncFilterChain;
        }

        private ISynchronousFilterChain BuildDefaultSyncFilterChain()
        {
            var syncFilterChain = new DefaultSynchronousFilterChain();

            return syncFilterChain;
        }

        #endregion

        T IDataStore.Instantiate<T>()
        {
            return this.resourceFactory.Create<T>();
        }

#pragma warning disable SA1124 // Do not use regions
        #region Helper methods
#pragma warning restore SA1124 // Do not use regions

        private void ApplyDefaultRequestHeaders(IHttpRequest request)
        {
            request.Headers.Accept = "application/json";
            request.Headers.UserAgent = UserAgentBuilder.GetUserAgent();
        }

        private IDictionary<string, object> GetBody<T>(IHttpResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.HasBody)
                return this.serializer.Deserialize(response.Body, typeof(T));
            else
                return new Dictionary<string, object>();
        }

        private bool IsCachingEnabled()
        {
            return this.cacheManager != null && !(this.cacheManager is NullCacheManager);
        }

        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region Asynchronous path
#pragma warning restore SA1124 // Do not use regions

        async Task<T> IDataStore.GetResourceAsync<T>(string resourcePath, CancellationToken cancellationToken)
        {
            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(resourcePath));
            this.logger.Trace($"Getting resource type {typeof(T).Name} from: {canonicalUri.ToString()}", "DefaultDataStore.GetResourceAsync<T>");

            var request = new DefaultHttpRequest(HttpMethod.Get, canonicalUri);
            var response = await this.SendToExecutorAsync(request, cancellationToken).ConfigureAwait(false);

            var json = response.Body;
            var map = this.serializer.Deserialize(json, typeof(T));
            var resource = this.resourceFactory.Create<T>(map);

            return resource;
        }

        Task<CollectionResponsePage<T>> IInternalDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            this.logger.Trace($"Getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollectionAsync<T>");

            return this.AsInterface.GetResourceAsync<CollectionResponsePage<T>>(href, cancellationToken);
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: null,
                cancellationToken: cancellationToken);
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            return this.AsInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: options,
                cancellationToken: cancellationToken);
        }

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsInterface.CreateAsync<T, TReturned>(
                parentHref,
                resource,
                options: null,
                cancellationToken: cancellationToken);
        }

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            QueryString queryParams = null;
            if (options != null)
                queryParams = new QueryString(options.GetQueryString());

            return this.SaveCoreAsync<T, TReturned>(
                resource, parentHref,
                queryParams: queryParams,
                cancellationToken: cancellationToken);
        }

        Task<T> IInternalDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException("Resource href must not be null.");

            return SaveCoreAsync<T, T>(
                resource,
                href,
                queryParams: null,
                cancellationToken: cancellationToken);
        }

        Task<bool> IInternalDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
        {
            return this.DeleteCoreAsync(resource, cancellationToken);
        }

        private async Task<TReturned> SaveCoreAsync<T, TReturned>(T resource, string href, QueryString queryParams, CancellationToken cancellationToken)
            where T : IResource
            where TReturned : IResource
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            var uri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href), queryParams);
            this.logger.Trace($"Saving resource of type {typeof(T).Name} to {href}", "DefaultDataStore.SaveCoreAsync");

            var propertiesMap = this.resourceConverter.ToMap(abstractResource);
            var body = this.serializer.Serialize(propertiesMap);

            var request = new DefaultHttpRequest(HttpMethod.Post, uri, null, null, body, "application/json");

            var response = await this.SendToExecutorAsync(request, cancellationToken).ConfigureAwait(false);
            var map = GetBody<T>(response);
            var createdResource = this.resourceFactory.Create<TReturned>(map);

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
            this.logger.Trace($"Deleting resource {uri.ToString()}", "DefaultDataStore.DeleteCoreAsync");

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
                DefaultError error = null;
                if (response.HasBody)
                {
                    error = new DefaultError(GetBody<IError>(response));
                }
                else
                {
                    var properties = new Dictionary<string, object>();
                    properties.Add("status", response.HttpStatus);
                    properties.Add("code", response.HttpStatus);
                    properties.Add("moreInfo", "HTTP error");
                    properties.Add("developerMessage", response.ResponsePhrase);
                    error = new DefaultError(properties);
                }

                throw new ResourceException(error);
            }

            return response;
        }

        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region Synchronous path
#pragma warning restore SA1124 // Do not use regions

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

        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region IDisposable
#pragma warning restore SA1124 // Do not use regions

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

        #endregion
    }
}
