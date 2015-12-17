// <copyright file="DefaultAccessToken.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultAccessToken : AbstractInstanceResource, IAccessToken
    {
        private static readonly string AccountPropertyName = "account";
        private static readonly string ApplicationPropertyName = "application";
        private static readonly string JwtPropertyName = "jwt";

        public DefaultAccessToken(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        internal IEmbeddedProperty Application => this.GetLinkProperty(ApplicationPropertyName);

        string IAccessToken.Jwt => this.GetStringProperty(JwtPropertyName);

        Task<IAccount> IAccessToken.GetAccountAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IApplication> IAccessToken.GetApplicationAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IApplication>(this.Application.Href);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);
    }
}
