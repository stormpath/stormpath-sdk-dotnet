// <copyright file="ICacheResolver.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.ThreadSafeMap;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore.Cache
{
    internal interface ICacheResolver
    {
        ISynchronousCache<string, IThreadSafeMap<string, object>> GetCache<T>()
            where T : IResource;

        Task<IAsynchronousCache<string, IThreadSafeMap<string, object>>> GetCacheAsync<T>(CancellationToken cancellationToken)
            where T : IResource;
    }
}
