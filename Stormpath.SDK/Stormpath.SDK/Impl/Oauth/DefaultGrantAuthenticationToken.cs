// <copyright file="DefaultGrantAuthenticationToken.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultGrantAuthenticationToken :
        AbstractInstanceResource,
        IGrantAuthenticationToken,
        IOauthGrantAuthenticationResult,
        IOauthGrantAuthenticationResultSync
    {
        private static readonly string AccessTokenPropertyName = "access_token";
        private static readonly string RefreshTokenPropertyName = "refresh_token";
        private static readonly string TokenTypePropertyName = "token_type";
        private static readonly string ExpiresInPropertyName = "expires_in";
        private static readonly string AccessTokenHrefPropertyName = "stormpath_access_token_href";

        public DefaultGrantAuthenticationToken(ResourceData data)
            : base(data)
        {
        }

        private new IGrantAuthenticationToken AsInterface => this;

        public string AccessTokenString
            => this.GetStringProperty(AccessTokenPropertyName);

        public string RefreshTokenString
            => this.GetStringProperty(RefreshTokenPropertyName);

        public string TokenType
            => this.GetStringProperty(TokenTypePropertyName);

        public long ExpiresIn
            => this.GetLongProperty(ExpiresInPropertyName);

        public string AccessTokenHref
            => this.GetStringProperty(AccessTokenHrefPropertyName);

        public Task<IAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccessToken>(this.AsInterface.AccessTokenHref, cancellationToken);

        IAccessToken IOauthGrantAuthenticationResultSync.GetAccessToken()
            => this.GetInternalSyncDataStore().GetResource<IAccessToken>(this.AsInterface.AccessTokenHref);
    }
}
