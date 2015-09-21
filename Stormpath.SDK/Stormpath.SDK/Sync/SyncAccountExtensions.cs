// <copyright file="SyncAccountExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    public static class SyncAccountExtensions
    {
        /// <summary>
        /// Gets the account's parent <see cref="IDirectory"/> (where the account is stored).
        /// </summary>
        /// <param name="account">The account</param>
        /// <returns>This account's directory.</returns>
        public static IDirectory GetDirectory(this IAccount account) => (account as IAccountSync).GetDirectory();

        /// <summary>
        /// Gets the Stormpath <see cref="ITenant"/> that owns this Account resource.
        /// </summary>
        /// <param name="account">The account</param>
        /// <returns>This account's tenant.</returns>
        public static ITenant GetTenant(this IAccount account) => (account as IAccountSync).GetTenant();
    }
}
