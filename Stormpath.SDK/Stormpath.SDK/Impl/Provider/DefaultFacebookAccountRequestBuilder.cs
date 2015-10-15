// <copyright file="DefaultFacebookAccountRequestBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultFacebookAccountRequestBuilder : AbstractProviderAccountRequestBuilder<IFacebookAccountRequestBuilder>, IFacebookAccountRequestBuilder
    {
        public DefaultFacebookAccountRequestBuilder(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        protected override string ConcreteProviderId
            => ProviderType.Facebook;

        protected override IProviderAccountRequest BuildConcrete()
        {
            if (string.IsNullOrEmpty(this.accessToken))
                throw new ApplicationException($"{nameof(this.accessToken)} is a required property. It must be provided before building.");

            var providerData = this.dataStore.Instantiate<IFacebookProviderData>() as DefaultFacebookProviderData;
            providerData.SetAccessToken(this.accessToken);

            return new DefaultProviderAccountRequest(providerData);
        }
    }
}
