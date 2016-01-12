// <copyright file="IGroupMembershipSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Group
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IGroupMembership">Group Membership</see>.
    /// </summary>
    internal interface IGroupMembershipSync : IDeletableSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IGroupMembership.GetAccountAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Account.</returns>
        IAccount GetAccount();

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroupMembership.GetGroupAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Group.</returns>
        IGroup GetGroup();
    }
}
