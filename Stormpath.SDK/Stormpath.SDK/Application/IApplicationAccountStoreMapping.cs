// <copyright file="IApplicationAccountStoreMapping.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;

namespace Stormpath.SDK.Application
{
    /// <summary>
    /// An <see cref="IApplicationAccountStoreMapping"/> represents the assignment of an <see cref="IAccountStore">Account Store</see>
    /// (either a <see cref="Group.IGroup">Group</see> or <see cref="Directory.IDirectory">Directory</see>) to an <see cref="IApplication">Application</see>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IApplicationAccountStoreMapping"/> is created, the Accounts in the Account Store are granted access to become users
    /// of the linked <see cref="IApplication">Application</see>. The <see cref="IAccountStoreMapping.ListIndex">order</see> in which Account Stores are assigned
    /// to an Application determines how login attempts work in Stormpath.
    /// <para>
    /// Additionally, an <see cref="IApplicationAccountStoreMapping"/> may be designated as the default Account Store.
    /// This causes any accounts created directly by the Application to be dispatched to and saved in the associated Account Store,
    /// since Application cannot store Accounts directly.
    /// </para>
    /// <para>
    /// Similarly, an <see cref="IApplicationAccountStoreMapping"/> may be designated as the default Group Store.
    /// This causes any Groups created directly by the Application to be dispatched to and saved in the associated Account Store,
    /// since an Application cannot store Groups directly.
    /// <b>Note:</b> A Group cannot store other Groups.  Therefore, the default group store must be a <see cref="Directory.IDirectory">Directory</see>.
    /// </para>
    /// </remarks>
    /// <seealso cref="IAccountStoreContainer{T}.CreateAccountStoreMappingAsync(IAccountStoreMapping{T}, System.Threading.CancellationToken)"/>
    public interface IApplicationAccountStoreMapping : IAccountStoreMapping<IApplicationAccountStoreMapping>
    {
    }
}
