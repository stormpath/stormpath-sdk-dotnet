// <copyright file="DefaultGoogleProvider.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultGoogleProvider : AbstractProvider, IGoogleProvider
    {
        private static readonly string ClientIdPropertyName = "clientId";
        private static readonly string ClientSecretPropertyName = "clientSecret";
        private static readonly string RedirectUriPropertyName = "redirectUri";

        public DefaultGoogleProvider(ResourceData data)
            : base(data)
        {
        }

        protected override string ConcreteProviderId
            => ProviderType.Google;

        string IGoogleProvider.ClientId
            => this.GetProperty<string>(ClientIdPropertyName);

        internal IGoogleProvider SetClientId(string clientId)
        {
            this.SetProperty(ClientIdPropertyName, clientId);
            return this;
        }

        string IGoogleProvider.ClientSecret
            => this.GetProperty<string>(ClientSecretPropertyName);

        internal IGoogleProvider SetClientSecret(string clientSecret)
        {
            this.SetProperty(ClientSecretPropertyName, clientSecret);
            return this;
        }

        string IGoogleProvider.RedirectUri
            => this.GetProperty<string>(RedirectUriPropertyName);

        internal IGoogleProvider SetRedirectUri(string redirectUri)
        {
            this.SetProperty(RedirectUriPropertyName, redirectUri);
            return this;
        }
    }
}
