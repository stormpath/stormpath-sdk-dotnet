// <copyright file="DefaultDataStore.Create.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Http;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed partial class DefaultDataStore
    {
        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: null,
                headers: null,
                cancellationToken: cancellationToken);
        }

        Task<T> IInternalAsyncDataStore.CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, T>(
                parentHref,
                resource,
                options: options,
                headers: null,
                cancellationToken: cancellationToken);
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, TReturned>(
                parentHref,
                resource,
                options: null,
                headers: null,
                cancellationToken: cancellationToken);
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, HttpHeaders headers, CancellationToken cancellationToken)
        {
            return this.AsAsyncInterface.CreateAsync<T, TReturned>(
                parentHref,
                resource,
                options: null,
                headers: headers,
                cancellationToken: cancellationToken);
        }

        Task<TReturned> IInternalAsyncDataStore.CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, HttpHeaders headers, CancellationToken cancellationToken)
        {
            return this.SaveCoreAsync<T, TReturned>(
                resource,
                parentHref,
                queryParams: this.CreateQueryStringFromCreationOptions(options),
                headers: headers,
                create: true,
                cancellationToken: cancellationToken);
        }

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource)
        {
            return this.AsSyncInterface.Create<T, T>(
                parentHref,
                resource,
                options: null,
                headers: null);
        }

        T IInternalSyncDataStore.Create<T>(string parentHref, T resource, ICreationOptions options)
        {
            return this.AsSyncInterface.Create<T, T>(
                parentHref,
                resource,
                options: options,
                headers: null);
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource)
        {
            return this.AsSyncInterface.Create<T, TReturned>(
                parentHref,
                resource,
                options: null,
                headers: null);
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource, HttpHeaders headers)
        {
            return this.AsSyncInterface.Create<T, TReturned>(
                parentHref,
                resource,
                options: null,
                headers: headers);
        }

        TReturned IInternalSyncDataStore.Create<T, TReturned>(string parentHref, T resource, ICreationOptions options, HttpHeaders headers)
        {
            return this.SaveCore<T, TReturned>(
                resource,
                parentHref,
                queryParams: this.CreateQueryStringFromCreationOptions(options),
                headers: headers,
                create: true);
        }
    }
}
