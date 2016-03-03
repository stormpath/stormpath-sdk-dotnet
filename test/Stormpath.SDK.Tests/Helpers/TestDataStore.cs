// <copyright file="TestDataStore.cs" company="Stormpath, Inc.">
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

using System;
using NSubstitute;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Logging;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Fakes;

namespace Stormpath.SDK.Tests.Helpers
{
    public static class TestDataStore
    {
        private static readonly string BaseUrl = "https://api.stormpath.com/v1";

        internal static IInternalDataStore Create(IRequestExecutor requestExecutor = null, ICacheProvider cacheProvider = null, ILogger logger = null, IClient client = null)
        {
            return new DefaultDataStore(
                client: client ?? Substitute.For<IClient>(),
                requestExecutor: requestExecutor ?? Substitute.For<IRequestExecutor>(),
                baseUrl: BaseUrl,
                serializer: Serializers.Create().JsonNetSerializer().Build(),
                logger: logger ?? new NullLogger(),
                userAgentBuilder: new FakeUserAgentBuilder(),
                instanceIdentifier: Guid.NewGuid().ToString(),
                cacheProvider: cacheProvider ?? CacheProviders.Create().DisabledCache(),
                identityMapExpiration: TimeSpan.FromMinutes(10));
        }
    }
}
