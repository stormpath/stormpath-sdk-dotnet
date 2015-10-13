// <copyright file="ProviderTypeConverter.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal static class ProviderTypeConverter
    {
        public static Func<IDictionary<string, object>, Type> TypeLookup = new Func<IDictionary<string, object>, Type>(properties =>
        {
            object rawProviderId = null;
            string providerId = null;

            if (!properties.TryGetValue("providerId", out rawProviderId))
                return null;

            providerId = (string)rawProviderId;

            if (providerId.Equals("stormpath", StringComparison.InvariantCultureIgnoreCase))
                return typeof(IProvider);

            if (providerId.Equals("facebook", StringComparison.InvariantCultureIgnoreCase))
                return typeof(IFacebookProvider);

            if (providerId.Equals("github", StringComparison.InvariantCultureIgnoreCase))
                return typeof(IGithubProvider);

            if (providerId.Equals("google", StringComparison.InvariantCultureIgnoreCase))
                return typeof(IGoogleProvider);

            if (providerId.Equals("linkedin", StringComparison.InvariantCultureIgnoreCase))
                return typeof(ILinkedInProvider);

            return null;
        });
    }
}
