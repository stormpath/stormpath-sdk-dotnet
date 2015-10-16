// <copyright file="SyncGroupExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    public static class SyncGroupExtensions
    {
        /// <summary>
        /// Synchronously assigns the specified <see cref="IAccount"/> to this <see cref="IGroup"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="account">The account to assignt to this group.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created
        /// reflecting the group-to-account association.
        /// </returns>
        public static IGroupMembership AddAccount(this IGroup group, IAccount account)
             => (group as IGroupSync).AddAccount(account);

        /// <summary>
        /// Synchronously assigns this <see cref="IGroup"/> to the specified <see cref="IAccount"/>
        /// represented by its (case-insensitive) <c>username</c>, <c>email</c>, or <c>href</c>
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="hrefOrEmailOrUsername">The <c>href</c>, email, or username of the <see cref="IAccount"/> to associate.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created
        /// reflecting the group-to-account association.
        /// </returns>
        /// <exception cref="InvalidOperationException">The specified account could not be found.</exception>
        public static IGroupMembership AddAccount(this IGroup group, string hrefOrEmailOrUsername)
            => (group as IGroupSync).AddAccount(hrefOrEmailOrUsername);

        /// <summary>
        /// Synchronously removes this group's association with the specified <see cref="IAccount"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="account">The <see cref="IAccount"/> object to disassociate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="InvalidOperationException">The specified account does not belong to this group.</exception>
        public static bool RemoveAccount(this IGroup group, IAccount account)
            => (group as IGroupSync).RemoveAccount(account);

        /// <summary>
        /// Synchronously removes this Group's association with the specified <see cref="IAccount"/>
        /// represented by its (case-insensitive) <c>username</c>, <c>email</c>, or <c>href</c>
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="hrefOrEmailOrUsername">The <see cref="IAccount"/> object to disassociate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="InvalidOperationException">The specified account does not belong to this group.</exception>
        public static bool RemoveAccount(this IGroup group, string hrefOrEmailOrUsername)
            => (group as IGroupSync).RemoveAccount(hrefOrEmailOrUsername);

        /// <summary>
        /// Synchronously gets the group's parent <see cref="IDirectory"/> (where the group is stored).
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>This group's parent <see cref="IDirectory"/>.</returns>
        public static IDirectory GetDirectory(this IGroup group)
            => (group as IGroupSync).GetDirectory();

        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Group resource.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>This account's tenant.</returns>
        public static ITenant GetTenant(this IGroup group)
            => (group as IGroupSync).GetTenant();
    }
}
