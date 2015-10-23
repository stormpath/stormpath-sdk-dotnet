// <copyright file="DefaultDataStore.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Error;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.CustomData;
using Stormpath.SDK.Impl.DataStore.Filters;
using Stormpath.SDK.Impl.Error;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.IdentityMap;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Serialization;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultDataStore : IInternalDataStore, IInternalAsyncDataStore, IInternalSyncDataStore, IDisposable
    {
        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly ICacheProvider cacheProvider;
        private readonly ICacheResolver cacheResolver;
        private readonly JsonSerializationProvider serializer;
        private readonly ILogger logger;
        private readonly IResourceFactory resourceFactory;
        private readonly IResourceConverter resourceConverter;
        private readonly IIdentityMap<string, ResourceData> identityMap;
        private readonly IAsynchronousFilterChain defaultAsyncFilters;
        private readonly ISynchronousFilterChain defaultSyncFilters;
        private readonly UriQualifier uriQualifier;

        private bool disposed = false;

        private IInternalAsyncDataStore AsAsyncInterface => this;

        private IInternalSyncDataStore AsSyncInterface => this;

        IRequestExecutor IInternalDataStore.RequestExecutor => this.requestExecutor;

        string IInternalDataStore.BaseUrl => this.baseUrl;

        internal DefaultDataStore(IRequestExecutor requestExecutor, string baseUrl, IJsonSerializer serializer, ILogger logger, ICacheProvider cacheProvider, TimeSpan identityMapExpiration)
        {
            if (requestExecutor == null)
                throw new ArgumentNullException(nameof(requestExecutor));
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider), "Use NullCacheProvider if you wish to turn off caching.");

            this.baseUrl = baseUrl;
            this.requestExecutor = requestExecutor;
            this.cacheProvider = cacheProvider;
            this.cacheResolver = new DefaultCacheResolver(cacheProvider);

            this.serializer = new JsonSerializationProvider(serializer);
            this.identityMap = new MemoryCacheIdentityMap<string, ResourceData>(identityMapExpiration);
            this.resourceFactory = new DefaultResourceFactory(this, this.identityMap);
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
                asyncFilterChain.Add(new WriteCacheFilter(this.cacheResolver, this.resourceFactory));
            }

            asyncFilterChain.Add(new ProviderAccountResultFilter());

            return asyncFilterChain;
        }

        private ISynchronousFilterChain BuildDefaultSyncFilterChain()
        {
            var syncFilterChain = new DefaultSynchronousFilterChain();

            if (this.IsCachingEnabled())
            {
                syncFilterChain.Add(new ReadCacheFilter(this.baseUrl, this.cacheResolver));
                syncFilterChain.Add(new WriteCacheFilter(this.cacheResolver, this.resourceFactory));
            }

            syncFilterChain.Add(new ProviderAccountResultFilter());

            return syncFilterChain;
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
            if (httpResponse.StatusCode == 201)
                return ResourceAction.Create;

            return request.Action;
        }

        private IHttpResponse HandleResponseOrError(IHttpResponse response)
        {
            if (response.TransportError ||
                response.IsClientError() ||
                response.IsServerError())
            {
                DefaultError error = null;
                error = response.HasBody
                    ? new DefaultError(this.GetBody<IError>(response))
                    : DefaultError.FromHttpResponse(response);

                throw new ResourceException(error);
            }

            return response;
        }

        T IDataStore.Instantiate<T>()
            => this.resourceFactory.Create<T>();

        T IInternalDataStore.InstantiateWithData<T>(IDictionary<string, object> properties)
            => this.resourceFactory.Create<T>(properties);

        T IInternalDataStore.InstantiateWithHref<T>(string href)
        {
            var properties = new Dictionary<string, object>()
            {
                ["href"] = href
            };
            return this.AsAsyncInterface.InstantiateWithData<T>(properties);
        }

        async Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            var result = await this.GetResourceDataAsync<T>(href, cancellationToken).ConfigureAwait(false);

            return this.resourceFactory.Create<T>(result.Body);
        }

        T IDataStoreSync.GetResource<T>(string href)
        {
            var result = this.GetResourceData<T>(href);

            return this.resourceFactory.Create<T>(result.Body);
        }

        async Task<T> IInternalAsyncDataStore.GetResourceAsync<T>(string href, Func<IDictionary<string, object>, Type> typeLookup, CancellationToken cancellationToken)
        {
            var result = await this.GetResourceDataAsync<T>(href, cancellationToken).ConfigureAwait(false);

            var targetType = typeLookup(result.Body);
            if (targetType == null)
                throw new ApplicationException("No type mapping could be found for this resource.");

            return this.resourceFactory.Create(targetType, result.Body) as T;
        }

        T IInternalSyncDataStore.GetResource<T>(string href, Func<IDictionary<string, object>, Type> typeLookup)
        {
            var result = this.GetResourceData<T>(href);

            var targetType = typeLookup(result.Body);
            if (targetType == null)
                throw new ApplicationException("No type mapping could be found for this resource.");

            return this.resourceFactory.Create(targetType, result.Body) as T;
        }

        async Task<T> IInternalAsyncDataStore.GetResourceAsync<T>(string href, IdentityMapOptions identityMapOptions, CancellationToken cancellationToken)
        {
            if (identityMapOptions != null && !identityMapOptions.IsValid())
                throw new ApplicationException("Bad identity map options specified.");

            var result = await this.GetResourceDataAsync<T>(href, cancellationToken).ConfigureAwait(false);

            return this.resourceFactory.Create<T>(result.Body, identityMapOptions);
        }

        T IInternalSyncDataStore.GetResource<T>(string href, IdentityMapOptions identityMapOptions)
        {
            if (identityMapOptions != null && !identityMapOptions.IsValid())
                throw new ApplicationException("Bad identity map options specified.");

            var result = this.GetResourceData<T>(href);

            return this.resourceFactory.Create<T>(result.Body, identityMapOptions);
        }

        private Task<IResourceDataResult> GetResourceDataAsync<T>(string href, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href));
            this.logger.Trace($"Asynchronously getting resource type {typeof(T).Name} from: {canonicalUri.ToString()}", "DefaultDataStore.GetResourceAsync<T>");

            IAsynchronousFilterChain chain = new DefaultAsynchronousFilterChain(this.defaultAsyncFilters as DefaultAsynchronousFilterChain)
                .Add(new DefaultAsynchronousFilter(async (req, next, logger, ct) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Get, req.Uri);

                    var response = await this.ExecuteAsync(httpRequest, ct).ConfigureAwait(false);
                    var body = this.GetBody<T>(response);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.StatusCode, body);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Read, typeof(T), canonicalUri);
            return chain.ExecuteAsync(request, this.logger, cancellationToken);
        }

        private IResourceDataResult GetResourceData<T>(string href)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href));
            this.logger.Trace($"Synchronously getting resource type {typeof(T).Name} from: {canonicalUri.ToString()}", "DefaultDataStore.GetResource<T>");

            ISynchronousFilterChain chain = new DefaultSynchronousFilterChain(this.defaultSyncFilters as DefaultSynchronousFilterChain)
                .Add(new DefaultSynchronousFilter((req, next, logger) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Get, req.Uri);

                    var response = this.Execute(httpRequest);
                    var body = this.GetBody<T>(response);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.StatusCode, body);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Read, typeof(T), canonicalUri);
            return chain.Filter(request, this.logger);
        }

        Task<CollectionResponsePage<T>> IInternalAsyncDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            this.logger.Trace($"Asynchronously getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollectionAsync<T>");

            return this.AsAsyncInterface.GetResourceAsync<CollectionResponsePage<T>>(href, cancellationToken);
        }

        CollectionResponsePage<T> IInternalSyncDataStore.GetCollection<T>(string href)
        {
            this.logger.Trace($"Synchronously getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollection<T>");

            return this.AsSyncInterface.GetResource<CollectionResponsePage<T>>(href);
        }

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: null,
                cancellationToken: cancellationToken);
        }

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource)
        {
            return this.AsSyncInterface.Create<T, T>(
                parentHref,
                resource,
                options: null);
        }

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: options,
                cancellationToken: cancellationToken);
        }

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource, ICreationOptions options)
        {
            return this.AsSyncInterface.Create<T, T>(
                parentHref,
                resource,
                options: options);
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, TReturned>(
                parentHref,
                resource,
                options: null,
                cancellationToken: cancellationToken);
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource)
        {
            return this.AsSyncInterface.Create<T, TReturned>(
                parentHref,
                resource,
                options: null);
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            return this.SaveCoreAsync<T, TReturned>(
                resource,
                parentHref,
                queryParams: this.CreateQueryStringFromCreationOptions(options),
                create: true,
                identityMapOptions: null,
                cancellationToken: cancellationToken);
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options)
        {
            return this.SaveCore<T, TReturned>(
                resource,
                parentHref,
                queryParams: this.CreateQueryStringFromCreationOptions(options),
                create: true,
                identityMapOptions: null);
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, IdentityMapOptions identityMapOptions, CancellationToken cancellationToken)
        {
            return this.SaveCoreAsync<T, TReturned>(
                resource,
                parentHref,
                queryParams: null,
                create: true,
                identityMapOptions: identityMapOptions,
                cancellationToken: cancellationToken);
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource, IdentityMapOptions identityMapOptions)
        {
            return this.SaveCore<T, TReturned>(
                resource,
                parentHref,
                create: true,
                queryParams: null,
                identityMapOptions: identityMapOptions);
        }

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(resource.Href));

            return this.SaveCoreAsync<T, T>(
                resource,
                href,
                queryParams: null,
                create: false,
                identityMapOptions: null,
                cancellationToken: cancellationToken);
        }

        T IInternalSyncDataStore.Save<T>(T resource)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(resource.Href));

            return this.SaveCore<T, T>(
                resource,
                href,
                create: false,
                queryParams: null,
                identityMapOptions: null);
        }

        Task<bool> IInternalAsyncDataStore.DeleteAsync<T>(T resource, CancellationToken cancellationToken)
        {
            return this.DeleteCoreAsync<T>(resource.Href, cancellationToken);
        }

        Task<bool> IInternalAsyncDataStore.DeletePropertyAsync(string parentHref, string propertyName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var propertyHref = $"{parentHref}/{propertyName}";

            return this.DeleteCoreAsync<IResource>(propertyHref, cancellationToken);
        }

        bool IInternalSyncDataStore.DeleteProperty(string parentHref, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var propertyHref = $"{parentHref}/{propertyName}";

            return this.DeleteCore<IResource>(propertyHref);
        }

        bool IInternalSyncDataStore.Delete<T>(T resource)
        {
            return this.DeleteCore<T>(resource.Href);
        }

        private async Task<TReturned> SaveCoreAsync<T, TReturned>(T resource, string href, QueryString queryParams, bool create, IdentityMapOptions identityMapOptions, CancellationToken cancellationToken)
            where T : IResource
            where TReturned : class, IResource
        {
            if (identityMapOptions != null && !identityMapOptions.IsValid())
                throw new ApplicationException("Bad identity map options specified.");

            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

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

                    bool responseHasData = responseBody.Any();
                    bool responseIsProcessing = response.StatusCode == 202;
                    bool responseOkay = responseHasData || responseIsProcessing;

                    if (!responseOkay)
                        throw new ResourceException(DefaultError.WithMessage("Unable to obtain resource data from the API server."));

                    if (responseIsProcessing)
                        this.logger.Warn($"Received a 202 response, returning empty result. Href: '{href}'", "DefaultDataStore.SaveCoreAsync");

                    return new DefaultResourceDataResult(responseAction, typeof(TReturned), req.Uri, response.StatusCode, responseBody);
                }));

            IDictionary<string, object> propertiesMap = null;

            var abstractResource = resource as AbstractResource;
            if (abstractResource != null)
            {
                // Serialize properties
                propertiesMap = this.resourceConverter.ToMap(abstractResource);

                var extendableInstanceResource = abstractResource as AbstractExtendableInstanceResource;
                bool includesCustomData = extendableInstanceResource != null;
                if (includesCustomData)
                {
                    var customDataProxy = (extendableInstanceResource as IExtendable).CustomData as DefaultEmbeddedCustomData;

                    // Apply custom data deletes
                    if (customDataProxy.HasDeletedProperties())
                    {
                        if (customDataProxy.DeleteAll)
                            await this.DeleteCoreAsync<ICustomData>(extendableInstanceResource.CustomData.Href, cancellationToken).ConfigureAwait(false);
                        else
                            await customDataProxy.DeleteRemovedCustomDataPropertiesAsync(extendableInstanceResource.CustomData.Href, cancellationToken).ConfigureAwait(false);
                    }

                    // Merge in custom data updates
                    if (customDataProxy.HasUpdatedCustomDataProperties())
                        propertiesMap["customData"] = customDataProxy.UpdatedCustomDataProperties;

                    // Remove custom data updates from proxy
                    extendableInstanceResource.ResetCustomData();
                }
            }

            // In some cases, all we need to save are custom data property deletions, which is taken care of above.
            // So, we should just refresh with the latest data from the server.
            // This doesn't apply to CREATEs, though, because sometimes we need to POST a null body.
            bool nothingToPost = !(propertiesMap?.Any() ?? false);
            if (!create && nothingToPost)
                return await this.AsAsyncInterface.GetResourceAsync<TReturned>(href, cancellationToken).ConfigureAwait(false);

            var requestAction = create
                ? ResourceAction.Create
                : ResourceAction.Update;
            var request = new DefaultResourceDataRequest(requestAction, typeof(T), canonicalUri, propertiesMap);

            var result = await chain.ExecuteAsync(request, this.logger, cancellationToken).ConfigureAwait(false);
            return this.resourceFactory.Create<TReturned>(result.Body, identityMapOptions, resource as ILinkable);
        }

        private TReturned SaveCore<T, TReturned>(T resource, string href, QueryString queryParams, bool create, IdentityMapOptions identityMapOptions)
            where T : IResource
            where TReturned : class, IResource
        {
            if (identityMapOptions != null && !identityMapOptions.IsValid())
                throw new ApplicationException("Bad identity map options specified.");

            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

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

                    bool responseHasData = responseBody.Any();
                    bool responseIsProcessing = response.StatusCode == 202;
                    bool responseOkay = responseHasData || responseIsProcessing;

                    if (!responseOkay)
                        throw new ResourceException(DefaultError.WithMessage("Unable to obtain resource data from the API server."));

                    if (responseIsProcessing)
                        this.logger.Warn($"Received a 202 response, returning empty result. Href: '{href}'", "DefaultDataStore.SaveCoreAsync");

                    return new DefaultResourceDataResult(responseAction, typeof(TReturned), req.Uri, response.StatusCode, responseBody);
                }));

            IDictionary<string, object> propertiesMap = null;

            var abstractResource = resource as AbstractResource;
            if (abstractResource != null)
            {
                // Serialize properties
                propertiesMap = this.resourceConverter.ToMap(abstractResource);

                var extendableInstanceResource = abstractResource as AbstractExtendableInstanceResource;
                bool includesCustomData = extendableInstanceResource != null;
                if (includesCustomData)
                {
                    var customDataProxy = (extendableInstanceResource as IExtendableSync).CustomData as DefaultEmbeddedCustomData;

                    // Apply custom data deletes
                    if (customDataProxy.HasDeletedProperties())
                    {
                        if (customDataProxy.DeleteAll)
                            this.DeleteCore<ICustomData>(extendableInstanceResource.CustomData.Href);
                        else
                            customDataProxy.DeleteRemovedCustomDataProperties(extendableInstanceResource.CustomData.Href);
                    }

                    // Merge in custom data updates
                    if (customDataProxy.HasUpdatedCustomDataProperties())
                        propertiesMap["customData"] = customDataProxy.UpdatedCustomDataProperties;

                    // Remove custom data updates from proxy
                    extendableInstanceResource.ResetCustomData();
                }
            }

            // In some cases, all we need to save are custom data property deletions, which is taken care of above.
            // So, we should just refresh with the latest data from the server.
            // This doesn't apply to CREATEs, though, because sometimes we need to POST a null body.
            bool nothingToPost = !(propertiesMap?.Any() ?? false);
            if (!create && nothingToPost)
                return this.AsSyncInterface.GetResource<TReturned>(href);

            var requestAction = create
                ? ResourceAction.Create
                : ResourceAction.Update;
            var request = new DefaultResourceDataRequest(requestAction, typeof(T), canonicalUri, propertiesMap);

            var result = chain.Filter(request, this.logger);
            return this.resourceFactory.Create<TReturned>(result.Body, identityMapOptions, resource as ILinkable);
        }

        private async Task<bool> DeleteCoreAsync<T>(string href, CancellationToken cancellationToken)
            where T : IResource
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var uri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href));
            this.logger.Trace($"Asynchronously deleting resource {uri.ToString()}", "DefaultDataStore.DeleteCoreAsync");

            IAsynchronousFilterChain chain = new DefaultAsynchronousFilterChain(this.defaultAsyncFilters as DefaultAsynchronousFilterChain)
                .Add(new DefaultAsynchronousFilter(async (req, next, logger, ct) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Delete, req.Uri);
                    var response = await this.ExecuteAsync(httpRequest, ct).ConfigureAwait(false);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.StatusCode, body: null);
                }));

            var request = new DefaultResourceDataRequest(ResourceAction.Delete, typeof(T), uri);
            var result = await chain.ExecuteAsync(request, this.logger, cancellationToken).ConfigureAwait(false);

            bool successfullyDeleted = result.HttpStatus == 204;
            return successfullyDeleted;
        }

        private bool DeleteCore<T>(string href)
            where T : IResource
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            var uri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href));
            this.logger.Trace($"Synchronously deleting resource {uri.ToString()}", "DefaultDataStore.DeleteCore");

            ISynchronousFilterChain chain = new DefaultSynchronousFilterChain(this.defaultSyncFilters as DefaultSynchronousFilterChain)
                .Add(new DefaultSynchronousFilter((req, next, logger) =>
                {
                    var httpRequest = new DefaultHttpRequest(HttpMethod.Delete, req.Uri);
                    var response = this.Execute(httpRequest);

                    return new DefaultResourceDataResult(req.Action, typeof(T), req.Uri, response.StatusCode, body: null);
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

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.requestExecutor.Dispose();
                    this.resourceFactory.Dispose();
                    this.cacheProvider.Dispose();
                }

                this.disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
