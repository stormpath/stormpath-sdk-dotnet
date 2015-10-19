// <copyright file="DefaultFacebookProvider.cs" company="Stormpath, Inc.">
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
    internal sealed class DefaultFacebookProvider : AbstractProvider, IFacebookProvider
    {
        private static readonly string ClientIdPropertyName = "clientId";
        private static readonly string ClientSecretPropertyName = "clientSecret";

        public DefaultFacebookProvider(ResourceData data)
            : base(data)
        {
        }

        protected override string ConcreteProviderId
            => ProviderType.Facebook;

        string IFacebookProvider.ClientId
            => this.GetProperty<string>(ClientIdPropertyName);

        internal IFacebookProvider SetClientId(string clientId)
        {
            this.SetProperty(ClientIdPropertyName, clientId);
            return this;
        }

        string IFacebookProvider.ClientSecret
            => this.GetProperty<string>(ClientSecretPropertyName);

        internal IFacebookProvider SetClientSecret(string clientSecret)
        {
            this.SetProperty(ClientSecretPropertyName, clientSecret);
            return this;
        }
    }
}
