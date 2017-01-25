// <copyright file="ProviderTypeConverter.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Provider;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Provider
{
    internal static class ProviderTypeConverter
    {
        private static readonly Type DefaultProviderType = typeof(IProvider);
        // TODO default provider data type?

        private static readonly IReadOnlyDictionary<string, Type> TypeLookupTable = new Dictionary<string, Type>()
        {
            ["stormpath"] = typeof(IProvider),
            ["facebook"] = typeof(IFacebookProvider),
            ["github"] = typeof(IGithubProvider),
            ["google"] = typeof(IGoogleProvider),
            ["linkedin"] = typeof(ILinkedInProvider),
            ["saml"] = typeof(ISamlProvider),
            ["ad"] = typeof(IAdLdapProvider),
            ["twitter"] = typeof(ITwitterProvider),
            ["oauth2"] = typeof(IOauth2Provider)
        };

        private static readonly IReadOnlyDictionary<string, Type> DataTypeLookupTable = new Dictionary<string, Type>()
        {
            ["stormpath"] = typeof(IProviderData),
            ["facebook"] = typeof(IFacebookProviderData),
            ["github"] = typeof(IGithubProviderData),
            ["google"] = typeof(IGoogleProviderData),
            ["linkedin"] = typeof(ILinkedInProviderData),
        };

        private static string GetProviderType(Map properties)
        {
            object rawProviderId;

            if (properties.TryGetValue("providerType", out rawProviderId))
            {
                return rawProviderId.ToString();
            }

            // Fall back to providerId for older resources
            if (properties.TryGetValue("providerId", out rawProviderId))
            {
                return rawProviderId.ToString();
            }

            return null;
        }

        public static Type TypeLookup(Map properties)
        {
            var providerId = GetProviderType(properties);
            if (string.IsNullOrEmpty(providerId))
            {
                return DefaultProviderType;
            }

            Type provider = null;
            TypeLookupTable.TryGetValue(providerId, out provider);

            return provider ?? DefaultProviderType;
        }

        public static Type DataTypeLookup(Map properties)
        {
            var providerId = GetProviderType(properties);
            if (string.IsNullOrEmpty(providerId))
            {
                return null;
            }

            Type providerData = null;
            DataTypeLookupTable.TryGetValue(providerId, out providerData);

            return providerData;
        }
    }
}
