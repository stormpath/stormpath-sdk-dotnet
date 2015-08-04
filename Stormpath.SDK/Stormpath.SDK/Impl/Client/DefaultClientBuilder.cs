// <copyright file="DefaultClientBuilder.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal class DefaultClientBuilder : IClientBuilder
    {
        private static readonly int DefaultConnectionTimeout = 20 * 1000;

        private string baseUrl = "https://api.stormpath.com/v1";
        private IClientApiKey apiKey;
        private AuthenticationScheme authenticationScheme;
        private int connectionTimeout = DefaultConnectionTimeout;

        IClientBuilder IClientBuilder.SetApiKey(IClientApiKey apiKey)
        {
            if (apiKey == null) throw new ArgumentNullException("API Key cannot be null.");

            this.apiKey = apiKey;
            return this;
        }

        IClientBuilder IClientBuilder.SetAuthenticationScheme(AuthenticationScheme scheme)
        {
            this.authenticationScheme = scheme;
            return this;
        }

        IClientBuilder IClientBuilder.SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException("Base URL cannot be empty.");

            this.baseUrl = baseUrl;
            return this;
        }

        IClientBuilder IClientBuilder.SetConnectionTimeout(int timeout)
        {
            if (timeout < 0) throw new ArgumentOutOfRangeException("Timeout cannot be negative.");

            this.connectionTimeout = timeout;
            return this;
        }

        IClient IClientBuilder.Build()
        {
            if (this.apiKey == null || !this.apiKey.IsValid()) throw new ArgumentException("API Key is not valid or has not been set. Use ClientApiKeys.Builder() to construct one.");

            return new DefaultClient(this.apiKey, this.baseUrl, this.authenticationScheme, this.connectionTimeout);
        }
    }
}
