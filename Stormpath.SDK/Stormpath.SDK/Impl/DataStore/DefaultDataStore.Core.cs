// <copyright file="DefaultDataStore.Core.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Error;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.CustomData;
using Stormpath.SDK.Impl.DataStore.Filters;
using Stormpath.SDK.Impl.Error;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.IdentityMap;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Serialization;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Resource;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed partial class DefaultDataStore
    {
        private readonly string baseUrl;
        private readonly IRequestExecutor requestExecutor;
        private readonly ICacheResolver cacheResolver;
        private readonly ICacheProvider cacheProvider;
        private readonly JsonSerializationProvider serializer;
        private readonly ILogger logger;
        private readonly IResourceFactory resourceFactory;
        private readonly IResourceConverter resourceConverter;
        private readonly IIdentityMap<ResourceData> identityMap;
        private readonly IAsynchronousFilterChain defaultAsyncFilters;
        private readonly ISynchronousFilterChain defaultSyncFilters;
        private readonly UriQualifier uriQualifier;

        private bool disposed = false;

        private IInternalAsyncDataStore AsAsyncInterface => this;

        private IInternalSyncDataStore AsSyncInterface => this;

        private IAsynchronousFilterChain BuildDefaultAsyncFilterChain()
        {
            var asyncFilterChain = new DefaultAsynchronousFilterChain(this);

            if (this.IsCachingEnabled())
            {
                asyncFilterChain.Add(new ReadCacheFilter(this.baseUrl, this.cacheResolver));
                asyncFilterChain.Add(new WriteCacheFilter(this.cacheResolver, this.resourceFactory));
            }

            asyncFilterChain.Add(new ProviderAccountResultFilter());
            asyncFilterChain.Add(new AccountStoreMappingCacheInvalidationFilter());

            return asyncFilterChain;
        }

        private ISynchronousFilterChain BuildDefaultSyncFilterChain()
        {
            var syncFilterChain = new DefaultSynchronousFilterChain(this);

            if (this.IsCachingEnabled())
            {
                syncFilterChain.Add(new ReadCacheFilter(this.baseUrl, this.cacheResolver));
                syncFilterChain.Add(new WriteCacheFilter(this.cacheResolver, this.resourceFactory));
            }

            syncFilterChain.Add(new ProviderAccountResultFilter());
            syncFilterChain.Add(new AccountStoreMappingCacheInvalidationFilter());

            return syncFilterChain;
        }

        private void ApplyDefaultRequestHeaders(IHttpRequest request)
        {
            request.Headers.Accept = "application/json";
            request.Headers.UserAgent = UserAgentBuilder.GetUserAgent();
        }

        private Map GetBody<T>(IHttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.HasBody)
            {
                return this.serializer.Deserialize(response.Body, typeof(T));
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }

        private bool IsCachingEnabled()
            => this.cacheProvider != null && !(this.cacheProvider is NullCacheProvider);

        private QueryString CreateQueryStringFromCreationOptions(ICreationOptions options)
        {
            QueryString queryParams = null;
            if (options != null)
            {
                queryParams = new QueryString(options.GetQueryString());
            }

            return queryParams;
        }

        private ResourceAction GetPostAction(IResourceDataRequest request, IHttpResponse httpResponse)
        {
            if (httpResponse.StatusCode == 201)
            {
                return ResourceAction.Create;
            }

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

        private async Task<TReturned> SaveCoreAsync<T, TReturned>(T resource, string href, QueryString queryParams, bool create, CancellationToken cancellationToken)
    where T : class
    where TReturned : class
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }

            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href), queryParams);
            this.logger.Trace($"Asynchronously saving resource of type {typeof(T).Name} to {canonicalUri.ToString()}", "DefaultDataStore.SaveCoreAsync");

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
                    {
                        throw new ResourceException(DefaultError.WithMessage("Unable to obtain resource data from the API server."));
                    }

                    if (responseIsProcessing)
                    {
                        this.logger.Warn($"Received a 202 response, returning empty result. Href: '{canonicalUri.ToString()}'", "DefaultDataStore.SaveCoreAsync");
                    }

                    return new DefaultResourceDataResult(responseAction, typeof(TReturned), req.Uri, response.StatusCode, responseBody);
                }));

            Map propertiesMap = null;

            var abstractResource = resource as AbstractResource;
            if (abstractResource != null)
            {
                // Serialize properties
                propertiesMap = this.resourceConverter.ToMap(abstractResource);

                var extendableInstanceResource = abstractResource as AbstractExtendableInstanceResource;
                bool includesCustomData = extendableInstanceResource != null;
                if (includesCustomData)
                {
                    var customDataProxy = (extendableInstanceResource as IExtendable).CustomData as DefaultCustomDataProxy;

                    // Apply custom data deletes
                    if (customDataProxy.HasDeletedProperties())
                    {
                        if (customDataProxy.DeleteAll)
                        {
                            await this.DeleteCoreAsync<ICustomData>(extendableInstanceResource.CustomData.Href, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            await customDataProxy.DeleteRemovedCustomDataPropertiesAsync(extendableInstanceResource.CustomData.Href, cancellationToken).ConfigureAwait(false);
                        }
                    }

                    // Merge in custom data updates
                    if (customDataProxy.HasUpdatedCustomDataProperties())
                    {
                        propertiesMap["customData"] = customDataProxy.UpdatedCustomDataProperties;
                    }

                    // Remove custom data updates from proxy
                    extendableInstanceResource.ResetCustomData();
                }
            }

            // In some cases, all we need to save are custom data property deletions, which is taken care of above.
            // So, we should just refresh with the latest data from the server.
            // This doesn't apply to CREATEs, though, because sometimes we *need* to POST a null body.
            bool nothingToPost = propertiesMap.IsNullOrEmpty();
            if (!create && nothingToPost)
            {
                return await this.AsAsyncInterface.GetResourceAsync<TReturned>(canonicalUri.ToString(), cancellationToken).ConfigureAwait(false);
            }

            var requestAction = create
                ? ResourceAction.Create
                : ResourceAction.Update;
            var request = new DefaultResourceDataRequest(requestAction, typeof(T), canonicalUri, propertiesMap);

            var result = await chain.FilterAsync(request, this.logger, cancellationToken).ConfigureAwait(false);
            return this.resourceFactory.Create<TReturned>(result.Body, resource as ILinkable);
        }

        private TReturned SaveCore<T, TReturned>(T resource, string href, QueryString queryParams, bool create)
            where T : class
            where TReturned : class
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }

            var canonicalUri = new CanonicalUri(this.uriQualifier.EnsureFullyQualified(href), queryParams);
            this.logger.Trace($"Synchronously saving resource of type {typeof(T).Name} to {canonicalUri.ToString()}", "DefaultDataStore.SaveCore");

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
                    {
                        throw new ResourceException(DefaultError.WithMessage("Unable to obtain resource data from the API server."));
                    }

                    if (responseIsProcessing)
                    {
                        this.logger.Warn($"Received a 202 response, returning empty result. Href: '{canonicalUri.ToString()}'", "DefaultDataStore.SaveCoreAsync");
                    }

                    return new DefaultResourceDataResult(responseAction, typeof(TReturned), req.Uri, response.StatusCode, responseBody);
                }));

            Map propertiesMap = null;

            var abstractResource = resource as AbstractResource;
            if (abstractResource != null)
            {
                // Serialize properties
                propertiesMap = this.resourceConverter.ToMap(abstractResource);

                var extendableInstanceResource = abstractResource as AbstractExtendableInstanceResource;
                bool includesCustomData = extendableInstanceResource != null;
                if (includesCustomData)
                {
                    var customDataProxy = (extendableInstanceResource as IExtendableSync).CustomData as DefaultCustomDataProxy;

                    // Apply custom data deletes
                    if (customDataProxy.HasDeletedProperties())
                    {
                        if (customDataProxy.DeleteAll)
                        {
                            this.DeleteCore<ICustomData>(extendableInstanceResource.CustomData.Href);
                        }
                        else
                        {
                            customDataProxy.DeleteRemovedCustomDataProperties(extendableInstanceResource.CustomData.Href);
                        }
                    }

                    // Merge in custom data updates
                    if (customDataProxy.HasUpdatedCustomDataProperties())
                    {
                        propertiesMap["customData"] = customDataProxy.UpdatedCustomDataProperties;
                    }

                    // Remove custom data updates from proxy
                    extendableInstanceResource.ResetCustomData();
                }
            }

            // In some cases, all we need to save are custom data property deletions, which is taken care of above.
            // So, we should just refresh with the latest data from the server.
            // This doesn't apply to CREATEs, though, because sometimes we need to POST a null body.
            bool nothingToPost = propertiesMap.IsNullOrEmpty();
            if (!create && nothingToPost)
            {
                return this.AsSyncInterface.GetResource<TReturned>(canonicalUri.ToString());
            }

            var requestAction = create
                ? ResourceAction.Create
                : ResourceAction.Update;
            var request = new DefaultResourceDataRequest(requestAction, typeof(T), canonicalUri, propertiesMap);

            var result = chain.Filter(request, this.logger);
            return this.resourceFactory.Create<TReturned>(result.Body, resource as ILinkable);
        }

        private async Task<bool> DeleteCoreAsync<T>(string href, CancellationToken cancellationToken)
            where T : IResource
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }

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
            var result = await chain.FilterAsync(request, this.logger, cancellationToken).ConfigureAwait(false);

            bool successfullyDeleted = result.HttpStatus == 204;
            return successfullyDeleted;
        }

        private bool DeleteCore<T>(string href)
            where T : IResource
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }

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
                this.disposed = true;

                if (disposing)
                {
                    this.requestExecutor.Dispose();
                    this.resourceFactory.Dispose();
                    this.cacheProvider.Dispose();
                    this.identityMap.Dispose();
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
