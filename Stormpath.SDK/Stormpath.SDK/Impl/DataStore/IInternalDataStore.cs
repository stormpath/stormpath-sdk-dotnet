// <copyright file="IInternalDataStore.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Api;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal interface IInternalDataStore : IDataStore
    {
        IRequestExecutor RequestExecutor { get; }

        ICacheResolver CacheResolver { get; }

        string BaseUrl { get; }

        IClientApiKey ApiKey { get; }

        T InstantiateWithHref<T>(string href)
            where T : IResource;

        T InstantiateWithData<T>(IDictionary<string, object> properties)
            where T : IResource;
    }
}
