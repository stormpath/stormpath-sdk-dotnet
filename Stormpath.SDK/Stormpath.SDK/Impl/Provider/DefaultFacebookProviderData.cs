﻿// <copyright file="DefaultFacebookProviderData.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultFacebookProviderData : AbstractProviderData, IFacebookProviderData
    {
        private static readonly string AccessTokenPropertyName = "accessToken";

        public DefaultFacebookProviderData(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultFacebookProviderData(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore, properties)
        {
        }

        protected override string ConcreteProviderId
            => ProviderType.Facebook.DisplayName;

        string IFacebookProviderData.AccessToken
            => this.GetProperty<string>(AccessTokenPropertyName);

        internal IFacebookProviderData SetAccessToken(string accessToken)
        {
            this.SetProperty(AccessTokenPropertyName, accessToken);
            return this;
        }
    }
}