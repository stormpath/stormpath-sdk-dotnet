// <copyright file="IInternalDataStore.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal interface IInternalDataStore : IDataStore
    {
        IRequestExecutor RequestExecutor { get; }

        string BaseUrl { get; }

        Task<CollectionResponsePage<T>> GetCollectionAsync<T>(string href, CancellationToken cancellationToken);

        Task<T> CreateAsync<T>(string parentHref, T resource, CancellationToken cancellationToken)
            where T : IResource;

        Task<T> CreateAsync<T>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            where T : IResource;

        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, CancellationToken cancellationToken)
            where T : IResource
            where TReturned : IResource;

        Task<TReturned> CreateAsync<T, TReturned>(string parentHref, T resource, ICreationOptions options, CancellationToken cancellationToken)
            where T : IResource
            where TReturned : IResource;

        Task<T> SaveAsync<T>(T resource, CancellationToken cancellationToken)
            where T : IResource, ISaveable<T>;

        Task<bool> DeleteAsync<T>(T resource, CancellationToken cancellationToken)
            where T : IResource, IDeletable;
    }
}
