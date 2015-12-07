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
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Group
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IGroup"/>.
    /// </summary>
    internal interface IGroupSync : ISaveableWithOptionsSync<IGroup>, IDeletableSync, IExtendableSync, IAccountStoreSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IGroup.AddAccountAsync(IAccount, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="account">The account to assign to this group.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created reflecting the group-to-account association.
        /// </returns>
        IGroupMembership AddAccount(IAccount account);

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroup.AddAccountAsync(IAccount, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="hrefOrEmailOrUsername">The <c>href</c>, email, or username of the <see cref="IAccount"/> to associate.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership"/> resource created reflecting the group-to-account association.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The specified account could not be found.</exception>
        IGroupMembership AddAccount(string hrefOrEmailOrUsername);

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroup.RemoveAccountAsync(IAccount, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="account">The <see cref="IAccount"/> object to disassociate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="System.InvalidOperationException">The specified account does not belong to this group.</exception>
        bool RemoveAccount(IAccount account);

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroup.RemoveAccountAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="hrefOrEmailOrUsername">The <see cref="IAccount"/> object to disassociate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="System.InvalidOperationException">The specified account does not belong to this group.</exception>
        bool RemoveAccount(string hrefOrEmailOrUsername);

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroup.GetDirectoryAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Directory.</returns>
        IDirectory GetDirectory();
    }
}
