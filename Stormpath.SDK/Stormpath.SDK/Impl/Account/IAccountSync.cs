// <copyright file="IAccountSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Account
{
    internal interface IAccountSync : ISaveableSync<IAccount>, IDeletableSync, IExtendableSync
    {
        /// <summary>
        /// Synchronously gets the account's parent <see cref="IDirectory"/> (where the account is stored).
        /// </summary>
        /// <returns>The directory.</returns>
        IDirectory GetDirectory();

        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Account resource.
        /// </summary>
        /// <returns>The tenant.</returns>
        ITenant GetTenant();

        /// <summary>
        /// Synchronously assigns this account to the specified <see cref="IGroup"/>.
        /// </summary>
        /// <param name="group">The Group this account will be added to.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created reflecting the account-to-group association.
        /// </returns>
        IGroupMembership AddGroup(IGroup group);

        /// <summary>
        /// Synchronously assigns this account to the specified <see cref="IGroup"/> represented
        /// by its (case-insensitive) <c>name</c> or <c>href</c>.
        /// </summary>
        /// <param name="hrefOrName">The <c>href</c> or name of the group to add.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created reflecting the account-to-group association.
        /// </returns>
        IGroupMembership AddGroup(string hrefOrName);

        /// <summary>
        /// Synchronously removes this <see cref="IAccount"/> from the specified <see cref="IGroup"/>.
        /// </summary>
        /// <param name="group">The group object from which the account must be removed.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="InvalidOperationException">The account does not belong to the specified group.</exception>
        bool RemoveGroup(IGroup group);

        /// <summary>
        /// Synchronously removes this <see cref="IAccount"/> from the specified <see cref="IGroup"/>
        /// represented by its (case-insensitive) <c>name</c> or <c>href</c>.
        /// </summary>
        /// <param name="hrefOrName">The <c>href</c> or name of the group object from which the account must be removed.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="InvalidOperationException">The account does not belong to the specified group.</exception>
        bool RemoveGroup(string hrefOrName);

        /// <summary>
        /// Synchronously gets whether this account belongs to the group whose name or <c>href</c> is
        /// (case-insensitive) equal to the specified value.
        /// </summary>
        /// <param name="hrefOrName">The <c>href</c> or name of the group to check.</param>
        /// <returns><c>true</c> if the account belongs to the specified group.</returns>
        bool IsMemberOfGroup(string hrefOrName);
    }
}
