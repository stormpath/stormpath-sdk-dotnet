// <copyright file="ProviderTypeConverter.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal static class ProviderTypeConverter
    {
        private static readonly IReadOnlyDictionary<string, Type> TypeLookupTable = new Dictionary<string, Type>()
        {
            ["stormpath"] = typeof(IProvider),
            ["facebook"] = typeof(IFacebookProvider),
            ["github"] = typeof(IGithubProvider),
            ["google"] = typeof(IGoogleProvider),
            ["linkedin"] = typeof(ILinkedInProvider),
        };

        private static readonly IReadOnlyDictionary<string, Type> DataTypeLookupTable = new Dictionary<string, Type>()
        {
            ["stormpath"] = typeof(IProviderData),
            ["facebook"] = typeof(IFacebookProviderData),
            ["github"] = typeof(IGithubProviderData),
            ["google"] = typeof(IGoogleProviderData),
            ["linkedin"] = typeof(ILinkedInProviderData),
        };

        private static string GetProviderId(IDictionary<string, object> properties)
        {
            object rawProviderId = null;

            if (!properties.TryGetValue("providerId", out rawProviderId))
                return null;

            return ((string)rawProviderId).ToLower();
        }

        public static Type TypeLookup(IDictionary<string, object> properties)
        {
            var providerId = GetProviderId(properties);
            if (string.IsNullOrEmpty(providerId))
                return null;

            Type provider = null;
            TypeLookupTable.TryGetValue(providerId, out provider);

            return provider;
        }

        public static Type DataTypeLookup(IDictionary<string, object> properties)
        {
            var providerId = GetProviderId(properties);
            if (string.IsNullOrEmpty(providerId))
                return null;

            Type providerData = null;
            DataTypeLookupTable.TryGetValue(providerId, out providerData);

            return providerData;
        }
    }
}
