// <copyright file="InternalFactory.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl
{
    internal sealed class InternalFactory
    {
        public IInternalDataStore CreateDataStore(IRequestExecutor requestExecutor, string baseUrl, IJsonSerializer serializer, ILogger logger, ICacheProvider cacheManager)
        {
            return new DefaultDataStore(requestExecutor, baseUrl, serializer, logger, cacheManager);
        }

        public IRequestExecutor CreateRequestExecutor(IClientApiKey apiKey, AuthenticationScheme authenticationScheme, int connectionTimeout, ILogger logger)
        {
            return new NetHttpRequestExecutor(apiKey, authenticationScheme, connectionTimeout, logger);
        }

        public AuthenticationRequestDispatcher GetAuthenticationDispatcher()
        {
            return new AuthenticationRequestDispatcher();
        }
    }
}
