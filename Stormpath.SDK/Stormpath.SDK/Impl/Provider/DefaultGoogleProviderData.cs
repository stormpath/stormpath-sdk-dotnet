// <copyright file="DefaultGoogleProviderData.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultGoogleProviderData : AbstractProviderData, IGoogleProviderData
    {
        private static readonly string AccessTokenPropertyName = "accessToken";
        private static readonly string RefreshTokenPropertyName = "refreshToken";
        private static readonly string CodePropertyName = "code";

        public DefaultGoogleProviderData(ResourceData data)
            : base(data)
        {
        }

        string IGoogleProviderData.AccessToken
            => this.GetStringProperty(AccessTokenPropertyName);

        internal IGoogleProviderData SetAccessToken(string accessToken)
        {
            this.SetProperty(AccessTokenPropertyName, accessToken);
            return this;
        }

        string IGoogleProviderData.RefreshToken
            => this.GetStringProperty(RefreshTokenPropertyName);

        internal string Code
            => this.GetStringProperty(CodePropertyName);

        internal IGoogleProviderData SetCode(string code)
        {
            this.SetProperty(CodePropertyName, code);
            return this;
        }

        protected override string ConcreteProviderId
            => ProviderType.Google;
    }
}
