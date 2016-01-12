// <copyright file="DefaultGoogleAccountRequestBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultGoogleAccountRequestBuilder : AbstractProviderAccountRequestBuilder<IGoogleAccountRequestBuilder>, IGoogleAccountRequestBuilder
    {
        private string code;

        public DefaultGoogleAccountRequestBuilder(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        IGoogleAccountRequestBuilder IGoogleAccountRequestBuilder.SetCode(string code)
        {
            this.code = code;
            return this;
        }

        protected override string ConcreteProviderId
            => ProviderType.Google;

        protected override IProviderAccountRequest BuildConcrete()
        {
            if (string.IsNullOrEmpty(this.accessToken) && string.IsNullOrEmpty(this.code))
            {
                throw new ApplicationException($"Either '{nameof(this.code)}' or '{nameof(this.accessToken)}' properties must exist in a Google account request.");
            }

            var providerData = this.dataStore.Instantiate<IGoogleProviderData>() as DefaultGoogleProviderData;

            if (!string.IsNullOrEmpty(this.accessToken))
            {
                providerData.SetAccessToken(this.accessToken);
            }
            else
            {
                providerData.SetCode(this.code);
            }

            return new DefaultProviderAccountRequest(providerData);
        }
    }
}
