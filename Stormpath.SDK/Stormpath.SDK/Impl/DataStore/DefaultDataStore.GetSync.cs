// <copyright file="DefaultDataStore.GetSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore.Filters;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed partial class DefaultDataStore
    {
        T IDataStoreSync.GetResource<T>(string href)
        {
            var result = this.GetResourceData<T>(href);

            return this.resourceFactory.Create<T>(result.Body);
        }

        T IDataStoreSync.GetResource<T>(string href, Action<IRetrievalOptions<T>> options)
        {
            var optionsInstance = new DefaultRetrievalOptions<T>();
            options(optionsInstance);

            var queryString = optionsInstance.ToString();
            if (!string.IsNullOrEmpty(queryString))
            {
                href = $"{href}?{queryString}";
            }

            return this.AsSyncInterface.GetResource<T>(href);
        }

        T IInternalSyncDataStore.GetResource<T>(string href, Func<IDictionary<string, object>, Type> typeLookup)
        {
            var result = this.GetResourceData<T>(href);

            var targetType = typeLookup(result.Body);
            if (targetType == null)
            {
                throw new ApplicationException("No type mapping could be found for this resource.");
            }

            return (T)this.resourceFactory.Create(targetType, result.Body);
        }

        CollectionResponsePage<T> IInternalSyncDataStore.GetCollection<T>(string href)
        {
            this.logger.Trace($"Synchronously getting collection page of type {typeof(T).Name} from: {href}", "DefaultDataStore.GetCollection<T>");

            return this.AsSyncInterface.GetResource<CollectionResponsePage<T>>(href);
        }

        private IResourceDataResult GetResourceData<T>(string href)
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }

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
    }
}
