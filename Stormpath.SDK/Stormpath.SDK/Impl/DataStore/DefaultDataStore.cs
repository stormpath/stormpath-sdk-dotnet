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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Error;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.DataStore.Filters;
using Stormpath.SDK.Impl.Error;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Serialization;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultDataStore : IInternalDataStore, IDisposable
    {
        private readonly ILogger logger;

        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly ICacheProvider cacheProvider;
        private readonly ICacheResolver cacheResolver;
        private readonly JsonSerializationProvider serializer;
        private readonly IResourceFactory resourceFactory;
        private readonly IResourceConverter resourceConverter;
        private readonly IAsynchronousFilterChain defaultAsyncFilters;
        private readonly ISynchronousFilterChain defaultSyncFilters;
        private readonly UriQualifier uriQualifier;

        private bool disposed = false;

        private IInternalDataStore AsInterface => this;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

        internal DefaultDataStore(IRequestExecutor requestExecutor, string baseUrl, IJsonSerializer serializer, ILogger logger, ICacheProvider cacheProvider)
        {
            if (requestExecutor == null)
                throw new ArgumentNullException(nameof(requestExecutor));
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider), "Use NullCacheManager if you wish to turn off caching.");

            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;
            this.cacheProvider = cacheProvider;
            this.cacheResolver = new DefaultCacheResolver(cacheProvider, new DefaultCacheRegionNameResolver());

            this.serializer = new JsonSerializationProvider(serializer);
            this.resourceFactory = new DefaultResourceFactory(this);
            this.resourceConverter = new DefaultResourceConverter();

            this.uriQualifier = new UriQualifier(baseUrl);
            this.logger = logger;

            this.defaultAsyncFilters = this.BuildDefaultAsyncFilterChain();
            this.defaultSyncFilters = this.BuildDefaultSyncFilterChain();
        }

        // *** Helper methods ***
        private IAsynchronousFilterChain BuildDefaultAsyncFilterChain()
        {
            var asyncFilterChain = new DefaultAsynchronousFilterChain();

            if (this.IsCachingEnabled())
            {
                asyncFilterChain.Add(new ReadCacheFilter(this.baseUrl, this.cacheResolver));
                asyncFilterChain.Add(new WriteCacheFilter(this.cacheResolver));
            }

            return asyncFilterChain;
        }

        private ISynchronousFilterChain BuildDefaultSyncFilterChain()
        {
            var syncFilterChain = new DefaultSynchronousFilterChain();

            if (this.IsCachingEnabled())
            {
                syncFilterChain.Add(new ReadCacheFilter(this.baseUrl, this.cacheResolver));
                syncFilterChain.Add(new WriteCacheFilter(this.cacheResolver));
            }

            return syncFilterChain;
        }

        T IDataStore.Instantiate<T>()
        {
            return this.resourceFactory.Create<T>();
        }

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
            return this.cacheProvider != null && !(this.cacheProvider is NullCacheProvider);
        }

        private QueryString CreateQueryStringFromCreationOptions(ICreationOptions options)
        {
            QueryString queryParams = null;
            if (options != null)
                queryParams = new QueryString(options.GetQueryString());

            return queryParams;
        }

        private ResourceAction GetPostAction(IResourceDataRequest request, IHttpResponse httpResponse)
        {
            if (httpResponse.HttpStatus == 201)
                return ResourceAction.Create;

            return request.Action;
        }

        private IHttpResponse HandleResponseOrError(IHttpResponse response)
        {
            if (response.ErrorType > ResponseErrorType.None)
            {
                DefaultError error = null;
                error = response.HasBody
                    ? new DefaultError(GetBody<IError>(response))
                    : DefaultError.FromHttpResponse(response);

                throw new ResourceException(error);
            }

            return response;
        }

        // DataStore methods
        async Task<T> IDataStore.GetResourceAsync<T>(string resourcePath, CancellationToken cancellationToken)
        {
            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(resourcePath));
            this.logger.Trace($"Asynchronously getting resource type {typeof(T).Name} from: {canonicalUri.ToString()}", "DefaultDataStore.GetResourceAsync<T>");

            IAsynchronousFilterChain chain = new DefaultAsynchronousFilterChain(this.defaultAsyncFilters as DefaultAsynchronousFilterChain)
                .Add(new DefaultAsynchronousFilter(async (req, next, logger, ct) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Get, req.Uri);

                    var response = await this.ExecuteAsync(httpRequest, ct).ConfigureAwait(false);
                    var body = this.GetBody<T>(response);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.HttpStatus, body);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Read, typeof(T), canonicalUri);
            var result = await chain.ExecuteAsync(request, this.logger, cancellationToken).ConfigureAwait(false);

            return this.resourceFactory.Create<T>(result.Body);
        }

        T IInternalDataStore.GetResource<T>(string resourcePath)
        {
            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(resourcePath));
            this.logger.Trace($"Synchronously getting resource type {typeof(T).Name} from: {canonicalUri.ToString()}", "DefaultDataStore.GetResource<T>");

            ISynchronousFilterChain chain = new DefaultSynchronousFilterChain(this.defaultSyncFilters as DefaultSynchronousFilterChain)
                .Add(new DefaultSynchronousFilter((req, next, logger) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Get, req.Uri);

                    var response = this.Execute(httpRequest);
                    var body = this.GetBody<T>(response);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.HttpStatus, body);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Read, typeof(T), canonicalUri);
            var result = chain.Filter(request, this.logger);

            return this.resourceFactory.Create<T>(result.Body);
        }

        Task<CollectionResponsePage<T>> IInternalDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            this.logger.Trace($"Asynchronously getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollectionAsync<T>");

            return this.AsInterface.GetResourceAsync<CollectionResponsePage<T>>(href, cancellationToken);
        }

        CollectionResponsePage<T> IInternalDataStore.GetCollection<T>(string href)
        {
            this.logger.Trace($"Synchronously getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollection<T>");

            return this.AsInterface.GetResource<CollectionResponsePage<T>>(href);
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: null,
                cancellationToken: cancellationToken);
        }

        T IInternalDataStore.Create<T>(string parentHref, T resource)
        {
            return this.AsInterface.Create<T, T>(
                parentHref,
                resource,
                options: null);
        }

        Task<T> IInternalDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            return this.AsInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: options,
                cancellationToken: cancellationToken);
        }

        T IInternalDataStore.Create<T>(string parentHref, T resource, ICreationOptions options)
        {
            return this.AsInterface.Create<T, T>(
                parentHref,
                resource,
                options: options);
        }

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsInterface.CreateAsync<T, TReturned>(
                parentHref,
                resource,
                options: null,
                cancellationToken: cancellationToken);
        }

        TReturned IInternalDataStore.Create<T, TReturned>(string parentHref, T resource)
        {
            return this.AsInterface.Create<T, TReturned>(
                parentHref,
                resource,
                options: null);
        }

        Task<TReturned> IInternalDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            return this.SaveCoreAsync<T, TReturned>(
                resource, parentHref,
                queryParams: this.CreateQueryStringFromCreationOptions(options),
                create: true,
                cancellationToken: cancellationToken);
        }

        TReturned IInternalDataStore.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options)
        {
            return this.SaveCore<T, TReturned>(
                resource, parentHref,
                create: true,
                queryParams: this.CreateQueryStringFromCreationOptions(options));
        }

        Task<T> IInternalDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(resource.Href));

            return SaveCoreAsync<T, T>(
                resource,
                href,
                queryParams: null,
                create: false,
                cancellationToken: cancellationToken);
        }

        T IInternalDataStore.Save<T>(T resource)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(resource.Href));

            return SaveCore<T, T>(
                resource,
                href,
                create: false,
                queryParams: null);
        }

        Task<bool> IInternalDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
        {
            return this.DeleteCoreAsync(resource, cancellationToken);
        }

        bool IInternalDataStore.Delete<T>(T resource)
        {
            return this.DeleteCore(resource);
        }

        private async Task<TReturned> SaveCoreAsync<T, TReturned>(T resource, string href, QueryString queryParams, bool create, CancellationToken cancellationToken)
            where T : IResource
            where TReturned : IResource
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href), queryParams);
            this.logger.Trace($"Asynchronously saving resource of type {typeof(T).Name} to {href}", "DefaultDataStore.SaveCoreAsync");

            IAsynchronousFilterChain chain = new DefaultAsynchronousFilterChain(this.defaultAsyncFilters as DefaultAsynchronousFilterChain)
                .Add(new DefaultAsynchronousFilter(async (req, next, logger, ct) =>
                {
                    var postBody = this.serializer.Serialize(req.Properties);
                    var httpRequest = new DefaultHttpRequest(
                        HttpMethod.Post,
                        req.Uri,
                        queryParams: null,
                        headers: null,
                        body: postBody,
                        bodyContentType: "application/json");

                    var response = await this.ExecuteAsync(httpRequest, ct).ConfigureAwait(false);
                    var responseBody = this.GetBody<T>(response);
                    var responseAction = this.GetPostAction(req, response);

                    bool responseHasExpectedData = responseBody.Any() || response.HttpStatus == 202;
                    if (!responseHasExpectedData)
                        throw new ResourceException(DefaultError.WithMessage("Unable to obtain resource data from the API server."));

                    return new DefaultResourceDataResult(responseAction, typeof(T), req.Uri, response.HttpStatus, responseBody);
                }));

            var propertiesMap = this.resourceConverter.ToMap(abstractResource);
            var requestAction = create
                ? ResourceAction.Create
                : ResourceAction.Update;
            var request = new DefaultResourceDataRequest(requestAction, typeof(T), canonicalUri, propertiesMap);

            var result = await chain.ExecuteAsync(request, this.logger, cancellationToken).ConfigureAwait(false);
            return this.resourceFactory.Create<TReturned>(result.Body);
        }

        private TReturned SaveCore<T, TReturned>(T resource, string href, QueryString queryParams, bool create)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href), queryParams);
            this.logger.Trace($"Synchronously saving resource of type {typeof(T).Name} to {href}", "DefaultDataStore.SaveCore");

            ISynchronousFilterChain chain = new DefaultSynchronousFilterChain(this.defaultSyncFilters as DefaultSynchronousFilterChain)
                .Add(new DefaultSynchronousFilter((req, next, logger) =>
                {
                    var postBody = this.serializer.Serialize(req.Properties);
                    var httpRequest = new DefaultHttpRequest(
                        HttpMethod.Post,
                        req.Uri,
                        queryParams: null,
                        headers: null,
                        body: postBody,
                        bodyContentType: "application/json");

                    var response = this.Execute(httpRequest);
                    var responseBody = this.GetBody<T>(response);
                    var responseAction = this.GetPostAction(req, response);

                    bool responseHasExpectedData = responseBody.Any() || response.HttpStatus == 202;
                    if (!responseHasExpectedData)
                        throw new ResourceException(DefaultError.WithMessage("Unable to obtain resource data from the API server."));

                    return new DefaultResourceDataResult(responseAction, typeof(T), req.Uri, response.HttpStatus, responseBody);
                }));

            var propertiesMap = this.resourceConverter.ToMap(abstractResource);
            var requestAction = create
                ? ResourceAction.Create
                : ResourceAction.Update;
            var request = new DefaultResourceDataRequest(requestAction, typeof(T), canonicalUri, propertiesMap);

            var result = chain.Filter(request, this.logger);
            return this.resourceFactory.Create<TReturned>(result.Body);
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
            this.logger.Trace($"Asynchronously deleting resource {uri.ToString()}", "DefaultDataStore.DeleteCoreAsync");

            IAsynchronousFilterChain chain = new DefaultAsynchronousFilterChain(this.defaultAsyncFilters as DefaultAsynchronousFilterChain)
                .Add(new DefaultAsynchronousFilter(async (req, next, logger, ct) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Delete, req.Uri);
                    var response = await this.ExecuteAsync(httpRequest, ct).ConfigureAwait(false);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.HttpStatus, body: null);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Delete, typeof(T), uri);
            var result = await chain.ExecuteAsync(request, this.logger, cancellationToken).ConfigureAwait(false);

            bool successfullyDeleted = result.HttpStatus == 204;
            return successfullyDeleted;
        }

        private bool DeleteCore<T>(T resource)
            where T : IResource, IDeletable
        {
            var abstractResource = resource as AbstractResource;
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            if (string.IsNullOrEmpty(resource.Href))
                throw new ArgumentNullException(nameof(resource.Href));

            var uri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(resource.Href));
            this.logger.Trace($"Synchronously deleting resource {uri.ToString()}", "DefaultDataStore.DeleteCore");

            ISynchronousFilterChain chain = new DefaultSynchronousFilterChain(this.defaultSyncFilters as DefaultSynchronousFilterChain)
                .Add(new DefaultSynchronousFilter((req, next, logger) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Delete, req.Uri);
                    var response = this.Execute(httpRequest);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.HttpStatus, body: null);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Delete, typeof(T), uri);
            var result = chain.Filter(request, this.logger);

            bool successfullyDeleted = result.HttpStatus == 204;
            return successfullyDeleted;
        }

        private async Task<IHttpResponse> ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            this.ApplyDefaultRequestHeaders(request);

            var response = await this.requestExecutor
                .ExecuteAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return this.HandleResponseOrError(response);
        }

        private IHttpResponse Execute(IHttpRequest request)
        {
            this.ApplyDefaultRequestHeaders(request);

            var response = this.requestExecutor.Execute(request);

            return this.HandleResponseOrError(response);
        }

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
