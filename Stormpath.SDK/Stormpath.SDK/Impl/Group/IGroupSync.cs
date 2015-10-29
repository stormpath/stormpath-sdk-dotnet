// <copyright file="IGroupSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Group
{
    internal interface IGroupSync : ISaveableWithOptionsSync<IGroup>, IDeletableSync, IExtendableSync
    {
        /// <summary>
        /// Synchronously assigns the specified <see cref="IAccount"/> to this <see cref="IGroup"/>.
        /// </summary>
        /// <param name="account">The account to assignt to this group.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created
        /// reflecting the group-to-account association.
        /// </returns>
        IGroupMembership AddAccount(IAccount account);

        /// <summary>
        /// Synchronously assigns this <see cref="IGroup"/> to the specified <see cref="IAccount"/>
        /// represented by its (case-insensitive) <c>username</c>, <c>email</c>, or <c>href</c>
        /// </summary>
        /// <param name="hrefOrEmailOrUsername">The <c>href</c>, email, or username of the <see cref="IAccount"/> to associate.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created
        /// reflecting the group-to-account association.
        /// </returns>
        /// <exception cref="InvalidOperationException">The specified account could not be found.</exception>
        IGroupMembership AddAccount(string hrefOrEmailOrUsername);

        /// <summary>
        /// Synchronously removes this group's association with the specified <see cref="IAccount"/>.
        /// </summary>
        /// <param name="account">The <see cref="IAccount"/> object to disassociate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="InvalidOperationException">The specified account does not belong to this group.</exception>
        bool RemoveAccount(IAccount account);

        /// <summary>
        /// Synchronously removes this Group's association with the specified <see cref="IAccount"/>
        /// represented by its (case-insensitive) <c>username</c>, <c>email</c>, or <c>href</c>
        /// </summary>
        /// <param name="hrefOrEmailOrUsername">The <see cref="IAccount"/> object to disassociate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="InvalidOperationException">The specified account does not belong to this group.</exception>
        bool RemoveAccount(string hrefOrEmailOrUsername);

        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Group resource.
        /// </summary>
        /// <returns>The tenant.</returns>
        ITenant GetTenant();

        /// <summary>
        /// Synchronously gets the group's parent <see cref="IDirectory"/> (where the group is stored).
        /// </summary>
        /// <returns>This group's parent <see cref="IDirectory"/>.</returns>
        IDirectory GetDirectory();
    }
}
