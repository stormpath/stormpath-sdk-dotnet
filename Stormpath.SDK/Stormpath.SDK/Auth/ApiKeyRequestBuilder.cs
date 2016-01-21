// <copyright file="ApiKeyRequestBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Auth;

namespace Stormpath.SDK.Auth
{
    public sealed class ApiKeyRequestBuilder
    {
        private string apiKeyId;
        private string apiKeySecret;

        public ApiKeyRequestBuilder SetId(string id)
        {
            this.apiKeyId = id;
            return this;
        }

        public ApiKeyRequestBuilder SetSecret(string secret)
        {
            this.apiKeySecret = secret;
            return this;
        }

        public IAuthenticationRequest Build()
        {
            return new ApiKeyAuthenticationRequest(this.apiKeyId, this.apiKeySecret);
        }
    }
}
