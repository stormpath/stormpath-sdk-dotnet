// <copyright file="DefaultDataStore.GetAsync.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore.Filters;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed partial class DefaultDataStore
    {
        async Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            var result = await this.GetResourceDataAsync<T>(href, cancellationToken).ConfigureAwait(false);

            return this.resourceFactory.Create<T>(result.Body);
        }

        Task<T> IDataStore.GetResourceAsync<T>(string href, Action<IRetrievalOptions<T>> options, CancellationToken cancellationToken)
        {
            var optionsInstance = new DefaultRetrievalOptions<T>();
            options(optionsInstance);

            var queryString = optionsInstance.ToString();
            if (!string.IsNullOrEmpty(queryString))
            {
                href = $"{href}?{queryString}";
            }

            return this.AsAsyncInterface.GetResourceAsync<T>(href, cancellationToken);
        }

        async Task<T> IInternalAsyncDataStore.GetResourceAsync<T>(string href, Func<IDictionary<string, object>, Type> typeLookup, CancellationToken cancellationToken)
        {
            var result = await this.GetResourceDataAsync<T>(href, cancellationToken).ConfigureAwait(false);

            var targetType = typeLookup(result.Body);
            if (targetType == null)
            {
                throw new ApplicationException("No type mapping could be found for this resource.");
            }

            return (T)this.resourceFactory.Create(targetType, result.Body);
        }

        Task<CollectionResponsePage<T>> IInternalAsyncDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            this.logger.Trace($"Asynchronously getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollectionAsync<T>");

            return this.AsAsyncInterface.GetResourceAsync<CollectionResponsePage<T>>(href, cancellationToken);
        }

        private Task<IResourceDataResult> GetResourceDataAsync<T>(string href, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }

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
            return chain.FilterAsync(request, this.logger, cancellationToken);
        }

    }
}
