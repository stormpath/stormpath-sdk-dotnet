// <copyright file="DefaultGithubAccountRequestBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultGithubAccountRequestBuilder : AbstractProviderAccountRequestBuilder<IGithubAccountRequestBuilder>, IGithubAccountRequestBuilder
    {
        public DefaultGithubAccountRequestBuilder(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        protected override string ConcreteProviderId
            => ProviderType.Github;

        protected override IProviderAccountRequest BuildConcrete()
        {
            if (string.IsNullOrEmpty(this.accessToken))
            {
                throw new ApplicationException($"{nameof(this.accessToken)} is a required property. It must be provided before building.");
            }

            var providerData = this.dataStore.Instantiate<IGithubProviderData>() as DefaultGithubProviderData;
            providerData.SetAccessToken(this.accessToken);

            return new DefaultProviderAccountRequest(providerData);
        }
    }
}
