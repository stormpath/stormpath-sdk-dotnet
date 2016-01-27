// <copyright file="SyncSamlAccountResultExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Saml;
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="ISamlAccountResult"/>.
    /// </summary>
    public static class SyncSamlAccountResultExtensions
    {
        /// <summary>
        /// Synchronously gets the user account returned by the SAML Identity Provider.
        /// </summary>
        /// <param name="samlAccountResult">The <see cref="ISamlAccountResult"/>.</param>
        /// <returns>The user's <see cref="IAccount">Account</see> resource.</returns>
        /// <exception cref="System.ApplicationException">The account is not present.</exception>
        public static IAccount GetAccount(this ISamlAccountResult samlAccountResult)
            => (samlAccountResult as ISamlAccountResultSync).GetAccount();
    }
}
