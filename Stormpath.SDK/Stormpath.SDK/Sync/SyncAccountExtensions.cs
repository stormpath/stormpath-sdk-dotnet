// <copyright file="SyncAccountExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IAccount">Account</see>.
    /// </summary>
    public static class SyncAccountExtensions
    {
        /// <summary>
        /// Synchronously gets The account.'s parent <see cref="Directory.IDirectory">Directory</see> (where The account. is stored).
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>This account's directory.</returns>
        public static IDirectory GetDirectory(this IAccount account)
            => (account as IAccountSync).GetDirectory();

        /// <summary>
        /// Synchronously assigns this account to the specified <see cref="Group.IGroup">Group</see>.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="group">The Group this account will be added to.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership">Group Membership</see> resource created reflecting The account.-to-group association.
        /// </returns>
        public static IGroupMembership AddGroup(this IAccount account, IGroup group)
            => (account as IAccountSync).AddGroup(group);

        /// <summary>
        /// Synchronously assigns this account to the specified <see cref="Group.IGroup">Group</see> represented
        /// by its (case-insensitive) name or <c>href</c>.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="hrefOrName">The <c>href</c> or name of the group to add.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership">Group Membership</see> resource created reflecting The account.-to-group association.
        /// </returns>
        public static IGroupMembership AddGroup(this IAccount account, string hrefOrName)
            => (account as IAccountSync).AddGroup(hrefOrName);

        /// <summary>
        /// Synchronously removes this <see cref="IAccount">Account</see> from the specified <see cref="Group.IGroup">Group</see>.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="group">The group object from which The account. must be removed.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="System.InvalidOperationException">The account. does not belong to the specified group.</exception>
        public static bool RemoveGroup(this IAccount account, IGroup group)
            => (account as IAccountSync).RemoveGroup(group);

        /// <summary>
        /// Synchronously removes this <see cref="IAccount">Account</see> from the specified <see cref="Group.IGroup">Group</see>
        /// represented by its (case-insensitive) name or <c>href</c>.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="hrefOrName">The <c>href</c> or name of the group object from which The account. must be removed.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="System.InvalidOperationException">The account. does not belong to the specified group.</exception>
        public static bool RemoveGroup(this IAccount account, string hrefOrName)
            => (account as IAccountSync).RemoveGroup(hrefOrName);

        /// <summary>
        /// Synchronously gets whether this account belongs to the group whose name or <c>href</c> is
        /// (case-insensitive) equal to the specified value.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="hrefOrName">The <c>href</c> or name of the group to check.</param>
        /// <returns><see langword="true"/> if The account. belongs to the specified group.</returns>
        public static bool IsMemberOfGroup(this IAccount account, string hrefOrName)
            => (account as IAccountSync).IsMemberOfGroup(hrefOrName);

        /// <summary>
        /// Synchronously gets the <see cref="IProviderData"/> Resource belonging to the account.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>The ProviderData Resource belonging to the account.</returns>
        public static IProviderData GetProviderData(this IAccount account)
            => (account as IAccountSync).GetProviderData();
    }
}
