// <copyright file="IAccountStore.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.AccountStore
{
    /// <summary>
    /// An abstract representation of a <see cref="Directory.IDirectory">Directory</see> or <see cref="Group.IGroup">Group</see>.
    /// </summary>
    public interface IAccountStore : IResource, IHasTenant
    {
        /// <summary>
        /// Gets a queryable list of all accounts in the Account Store.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccount}"/> that may be used to list or search accounts.</returns>
        IAsyncQueryable<IAccount> GetAccounts();
    }
}
