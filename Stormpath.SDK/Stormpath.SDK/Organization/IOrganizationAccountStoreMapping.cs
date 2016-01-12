// <copyright file="IOrganizationAccountStoreMapping.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;

namespace Stormpath.SDK.Organization
{
    /// <summary>
    /// An <see cref="IOrganizationAccountStoreMapping"/> represents the assignment of an <see cref="IAccountStore">Account Store</see>
    /// (either a <see cref="Group.IGroup">Group</see> or <see cref="Directory.IDirectory">Directory</see>) to an <see cref="IOrganization">Organization</see>.
    /// </summary>
    /// <remarks>
    /// When an <see cref="IOrganizationAccountStoreMapping"/> is created, the Accounts in the Account Store are granted access to become users
    /// of the linked <see cref="IOrganization">Organization</see>. The <see cref="IAccountStoreMapping{T}.ListIndex">order</see> in which Account Stores are assigned
    /// to an Organization determines how login attempts work in Stormpath.
    /// <para>
    /// Additionally, an <see cref="IOrganizationAccountStoreMapping"/> may be designated as the default Account Store.
    /// This causes any accounts created directly by the Organization to be dispatched to and saved in the associated Account Store,
    /// since Organizations cannot store Accounts directly.
    /// </para>
    /// <para>
    /// Similarly, an <see cref="IOrganizationAccountStoreMapping"/> may be designated as the default Group Store.
    /// This causes any Groups created directly by the Organization to be dispatched to and saved in the associated Account Store,
    /// since an Organization cannot store Groups directly.
    /// <b>Note:</b> A Group cannot store other Groups.  Therefore, the default group store must be a <see cref="Directory.IDirectory">Directory</see>.
    /// </para>
    /// </remarks>
    /// <seealso cref="IAccountStoreContainer{T}.CreateAccountStoreMappingAsync(T, CancellationToken)"/>
    public interface IOrganizationAccountStoreMapping : IAccountStoreMapping<IOrganizationAccountStoreMapping>
    {
        /// <summary>
        /// Sets the <see cref="IOrganization">Organization</see> represented by this <see cref="IOrganizationAccountStoreMapping"/> resource.
        /// </summary>
        /// <param name="organization">The <see cref="IOrganization">Organization</see> represented by this <see cref="IOrganizationAccountStoreMapping">Account Store Mapping</see>.</param>
        /// <returns>This instance for method chaining.</returns>
        IOrganizationAccountStoreMapping SetOrganization(IOrganization organization);

        /// <summary>
        /// Gets the <see cref="IOrganization">Organization</see> represented by this mapping.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The mapping's <see cref="IOrganization">Organization</see>.</returns>
        Task<IOrganization> GetOrganizationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
