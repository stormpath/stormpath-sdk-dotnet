// <copyright file="IOrganization.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Group;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Organization
{
    /// <summary>
    /// An Organization is a top-level container for <see cref="Directory.IDirectory">Directory</see> and <see cref="IGroup">Group</see> resources,
    /// and is an <see cref="IAccountStore">Account Store</see> that can be mapped to an <see cref="Application.IApplication">Application</see>
    /// just like a Directory or Group.
    /// <para>Organizations are primarily intended to represent tenants in multi-tenant applications.</para>
    /// </summary>
    public interface IOrganization :
        IResource,
        IHasTenant,
        ISaveableWithOptions<IOrganization>,
        IDeletable,
        IAuditable,
        IExtendable,
        IAccountStore,
        IAccountStoreContainer<IOrganizationAccountStoreMapping>,
        IAccountCreationActions,
        IGroupCreationActions
    {
        /// <summary>
        /// Gets the Organization's name.
        /// </summary>
        /// <value>The Organization's name. An Organization's name must be unique across all Organizations within a Stormpath <see cref="Tenant.ITenant"/>.</value>
        string Name { get; }

        /// <summary>
        /// Gets the Organization's name key.
        /// </summary>
        /// <value>
        /// The Organization's name key. An Organization's name key must be unique across all Organizations within a Stormpath <see cref="Tenant.ITenant"/>,
        /// and must follow DNS hostname rules: it may only consist of a-z, A-Z, 0-9 and - (hyphen). It must not start or end with a hyphen.
        /// The uniqueness constraint is case-insensitive.
        /// </value>
        string NameKey { get; }

        /// <summary>
        /// Gets the Organization's status.
        /// </summary>
        /// <value>The Organization's status. Enabled Organizations can be used as <see cref="IAccountStore">Account Stores</see> for <see cref="Application.IApplication">Applications</see>; Disabled Organizations cannot.</value>
        OrganizationStatus Status { get; }

        /// <summary>
        /// Gets the Organization's description.
        /// </summary>
        /// <value>The description of the Organization.</value>
        string Description { get; }

        /// <summary>
        /// Sets the Organization's name.
        /// </summary>
        /// <param name="name">The Organization's name. Organization names must be unique within a Stormpath <see cref="Tenant.ITenant"/>.</param>
        /// <returns>This instance for method chaining.</returns>
        IOrganization SetName(string name);

        /// <summary>
        /// Sets the Organization's name key.
        /// </summary>
        /// <param name="nameKey">
        /// The Organization's name key. Organization name keys must be unique within a Stormpath <see cref="Tenant.ITenant"/>,
        /// and must follow DNS hostname rules: it may only consist of a-z, A-Z, 0-9 and - (hyphen). It must not start or end with a hyphen.
        /// The uniqueness constraint is case-insensitive.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        IOrganization SetNameKey(string nameKey);

        /// <summary>
        /// Sets the Organization's status.
        /// </summary>
        /// <param name="status">The Organization's status.
        /// <see cref="OrganizationStatus.Enabled"/> Organizations can be used as <see cref="IAccountStore">Account Stores</see>s for <see cref="Application.IApplication">Applications</see>.
        /// <see cref="OrganizationStatus.Disabled"/> Organizations cannot be used as <see cref="IAccountStore">Account Stores</see>.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        IOrganization SetStatus(OrganizationStatus status);

        /// <summary>
        /// Sets the Organization's description.
        /// </summary>
        /// <param name="description">The Organization's description text.</param>
        /// <returns>This instance for method chaining.</returns>
        IOrganization SetDescription(string description);

        /// <summary>
        /// Gets a queryable list of all Groups accessible to this Organization.
        /// It will not only return any group associated directly as an <see cref="IAccountStore">Account Store</see>
        /// but also every Group that exists inside every Directory associated as an Account Store.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search <see cref="IGroup">Groups</see>.</returns>
        /// <example>
        /// var allOrganizationGroups = await myOrg.GetGroups().ToListAsync();
        /// </example>
        IAsyncQueryable<IGroup> GetGroups();
    }
}
