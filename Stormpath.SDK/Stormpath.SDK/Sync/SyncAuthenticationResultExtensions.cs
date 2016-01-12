// <copyright file="SyncAuthenticationResultExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.Auth;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IAuthenticationResult"/>.
    /// </summary>
    public static class SyncAuthenticationResultExtensions
    {
        /// <summary>
        /// Synchronously gets the successfully authenticated <see cref="IAccount">Account</see>.
        /// </summary>
        /// <param name="authResult">The authentication result.</param>
        /// <returns>The <see cref="IAccount">Account</see> that was successfully authenticated.</returns>
        public static IAccount GetAccount(this IAuthenticationResult authResult)
            => (authResult as IAuthenticationResultSync).GetAccount();
    }
}
