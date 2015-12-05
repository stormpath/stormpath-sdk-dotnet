// <copyright file="DefaultDataStore.Save.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed partial class DefaultDataStore
    {
        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.SaveAsync<T>(resource, string.Empty, cancellationToken);
        }

        Task<T> IInternalAsyncDataStore.SaveAsync<T>(T resource, string queryString, CancellationToken cancellationToken)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(resource.Href));
            }

            var queryParams = new QueryString(queryString);

            return this.SaveCoreAsync<T, T>(
                resource,
                href,
                queryParams: queryParams,
                create: false,
                cancellationToken: cancellationToken);
        }

        T IInternalSyncDataStore.Save<T>(T resource)
        {
            return this.AsSyncInterface.Save<T>(resource, string.Empty);
        }

        T IInternalSyncDataStore.Save<T>(T resource, string queryString)
        {
            var href = resource?.Href;
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(resource.Href));
            }

            var queryParams = new QueryString(queryString);

            return this.SaveCore<T, T>(
                resource,
                href,
                create: false,
                queryParams: queryParams);
        }

    }
}
