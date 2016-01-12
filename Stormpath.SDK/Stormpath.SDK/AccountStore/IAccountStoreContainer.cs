// <copyright file="IAccountStoreContainer.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.AccountStore
{
    /// <summary>
    /// Represents <see cref="IResource">Resources</see> that are capable of storing <see cref="IAccountStore">AccountStores</see>,
    /// such as <see cref="Application.IApplication">Applications</see> and <see cref="Organization.IOrganization">Organizations</see>.
    /// </summary>
    /// <typeparam name="TMapping">The Account Store type.</typeparam>
    public interface IAccountStoreContainer<TMapping> : IResource, IHasTenant
        where TMapping : IAccountStoreMapping<TMapping>, ISaveable<TMapping>
    {
        /// <summary>
        /// Gets a queryable list of all Account Store Mappings accessible to the resource.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccountStoreMapping}"/> that may be used to asynchronously list or search <see cref="IAccountStoreMapping{T}">AccountStoreMappings</see>.</returns>
        IAsyncQueryable<TMapping> GetAccountStoreMappings();

        /// <summary>
        /// Gets the <see cref="IAccountStore">Account Store</see> (either a <see cref="Group.IGroup">Group</see> or <see cref="Directory.IDirectory">Directory</see>)
        /// used to persist new <see cref="Account.IAccount">Accounts</see>, or <see langword="null"/> if no default Account Store has been designated.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The default <see cref="IAccountStore">Account Store</see> for new <see cref="Account.IAccount">Accounts</see>,
        /// or <see langword="null"/> if no default <see cref="IAccountStore">Account Store</see> has been designated.</returns>
        /// <example>
        /// Getting and using the default account store:
        /// <code>
        /// var accountStore = await application.GetDefaultAccountStoreAsync();
        /// var accountStoreAsDirectory = accountStore as IDirectory;
        /// var accountStoreAsGroup = accountStore as IGroup;
        /// if (accountStoreAsDirectory != null)
        ///     // use as directory
        /// else if (accountStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        Task<IAccountStore> GetDefaultAccountStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the <see cref="IAccountStore">Account Store</see> (either a <see cref="Group.IGroup">Group</see> or a <see cref="Directory.IDirectory">Directory</see>)
        /// used to persist new <see cref="Account.IAccount">Accounts</see>.
        /// <para>
        /// Because this resource is not an <see cref="IAccountStore">Account Store</see> itself, it delegates to a Group or Directory
        /// when creating accounts; this method sets the <see cref="IAccountStore">Account Store</see> to which new account persistence is delegated.
        /// </para>
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore">Account Store</see> used to persist new accounts.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetDefaultAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IAccountStore">Account Store</see> used to persist new <see cref="Group.IGroup">Groups</see>, or <see langword="null"/>
        /// if no default Account Store has been designated.
        /// <para>
        /// Stormpath's current REST API requires this to be a <see cref="Directory.IDirectory">Directory</see>.
        /// However, this could be a <see cref="Group.IGroup">Group</see> in the future, so do not assume it is always a
        /// Directory if you want your code to be function correctly if/when this support is added.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The default <see cref="IAccountStore">Account Store</see> for new <see cref="Group.IGroup">Groups</see>,
        /// or <see langword="null"/> if no default <see cref="IAccountStore">Account Store</see> has been designated.</returns>
        /// <example>
        /// Getting and using the default group store:
        /// <code>
        /// var groupStore = await application.GetDefaultGroupStoreAsync();
        /// var groupStoreAsDirectory = groupStore as IDirectory;
        /// var groupStoreAsGroup = groupStore as IGroup;
        /// if (groupStoreAsDirectory != null)
        ///     // use as directory
        /// else if (groupStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        Task<IAccountStore> GetDefaultGroupStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the <see cref="IAccountStore">Account Store</see> (a <see cref="Directory.IDirectory">Directory</see>)
        /// used to persist new <see cref="Group.IGroup">Groups</see>.
        /// <para>
        /// Stormpath's current REST API requires this to be a <see cref="Directory.IDirectory">Directory</see>. However, this could be a <see cref="Group.IGroup">Group</see> in the future,
        /// so do not assume it is always a Directory if you want your code to function properly if/when this support is added.
        /// </para>
        /// <para>
        /// Because this resource is not an <see cref="IAccountStore">Account Store</see> itself, it delegates to a Directory
        /// when creating groups; this method sets the <see cref="IAccountStore">Account Store</see> to which new group persistence is delegated.
        /// </para>
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore">Account Store</see> used to persist new groups.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetDefaultGroupStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>, allowing the associated Account Store
        /// to be used a source of accounts that may log in to the Application or Organization.
        /// </summary>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping{T}">Account Store Mapping</see> resource to add to the AccountStoreMapping list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>.</returns>
        /// <exception cref="Error.ResourceException">The AccountStoreMapping's ListIndex is negative, or the mapping could not be added to the Application or Organization.</exception>
        /// <example>
        /// Setting a new <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>'s <see cref="IAccountStoreMapping{T}.ListIndex"/> to <c>500</c> and then adding the mapping to
        /// an application with an existing 3-item list will automatically save the <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>
        /// at the end of the list and set its <see cref="IAccountStoreMapping{T}.ListIndex"/> value to <c>3</c> (items at index 0, 1, 2 were the original items,
        /// the new fourth item will be at index 3):
        /// <code>
        /// IAccountStore directoryOrGroup = await GetDirectoryOrGroupAsync();
        /// IAccountStoreMapping mapping = client.Instantiate&lt;IAccountStoreMapping&gt;();
        /// mapping.SetAccountStore(directoryOrGroup);
        /// mapping.SetListIndex(500);
        /// mapping = await application.CreateAccountStoreMappingAsync(mapping);
        /// </code>
        /// </example>
        Task<TMapping> CreateAccountStoreMappingAsync(TMapping mapping, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new <see cref="IAccountStore">Account Store</see> to the Application or Organization and appends the resulting <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>
        /// to the end of the AccountStoreMapping list.
        /// <para>
        /// If you need to control the order of the added AccountStore, use the <see cref="CreateAccountStoreMappingAsync(TMapping, CancellationToken)"/> method.
        /// </para>
        /// </summary>
        /// <param name="accountStore">The new <see cref="IAccountStore">Account Store</see> resource to add to the AccountStoreMapping list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application or Organization.</exception>
        /// <example>
        /// <code>
        /// IAccountStore directoryOrGroup = await GetDirectoryOrGroupAsync();
        /// IAccountStoreMapping mapping = await application.AddAccountStoreAsync(directoryOrGroup);
        /// </code>
        /// </example>
        Task<TMapping> AddAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new <see cref="IAccountStore">Account Store</see> to this Application. The given string can either be an <c>href</c> or a name of a
        /// <see cref="Directory.IDirectory">Directory</see> or <see cref="Group.IGroup">Group</see> belonging to the current <see cref="Tenant.ITenant"/>.
        /// <para>
        /// If the provided value is an <c>href</c>, this method will get the proper Resource and add it as a new AccountStore in this
        /// Application or Organization without much effort. However, if the provided value is not an <c>href</c>, it will be considered as a name. In this case,
        /// this method will search for both a Directory and a Group whose names equal the provided <paramref name="hrefOrName"/>. If only
        /// one resource exists (either a Directory or a Group), then it will be added as a new AccountStore in this Application or Organization. However,
        /// if there are two resources (a Directory and a Group) matching that name, a <see cref="Error.ResourceException"/> will be thrown.
        /// </para>
        /// <para>
        /// Note: When using names this method is not efficient as it will search for both Directories and Groups within this Tenant
        /// for a matching name. In order to do so, some looping takes place at the client side: groups exist within directories, therefore we need
        /// to loop through every existing directory in order to find the required Group. In contrast, providing the Group's <c>href</c> is much more
        /// efficient as no actual search operation needs to be carried out.
        /// </para>
        /// </summary>
        /// <param name="hrefOrName">Either the <c>href</c> or name of the desired <see cref="Directory.IDirectory">Directory</see> or <see cref="Group.IGroup">Group</see>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application or Organization.</exception>
        /// <exception cref="ArgumentException">The given <paramref name="hrefOrName"/> matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Providing an href:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = await application.AddAccountStoreAsync("https://api.stormpath.com/v1/groups/2rwq022yMt4u2DwKLfzriP");
        /// </code>
        /// Providing a name:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = await application.AddAccountStoreAsync("Foo Name");
        /// </code>
        /// </example>
        Task<TMapping> AddAccountStoreAsync(string hrefOrName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a resource of type <typeparamref name="TSource"/> as a new <see cref="IAccountStore">Account Store</see> to this Application or Organization. The provided query
        /// must match a single <typeparamref name="TSource"/> in the current <see cref="Tenant.ITenant">Tenant</see>. If no compatible resource matches the query, this method will return <see langword="null"/>.
        /// </summary>
        /// <param name="query">Query to search for a resource of type <typeparamref name="TSource"/> in the current Tenant.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TSource">The type of resource (either a <see cref="Directory.IDirectory">Directory</see> or a <see cref="Group.IGroup">Group</see>) to query for.</typeparam>
        /// <returns>The newly-created <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>, or <see langword="null"/> if there is no resource matching the query.</returns>
        /// <exception cref="Error.ResourceException">The found resource already exists as an account store in the application.</exception>
        /// <exception cref="ArgumentException">The query matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Adding a directory by partial name:
        /// <code>
        /// IAccountStoreMapping mapping = await application.AddAccountStoreAsync&lt;IDirectory&gt;(dirs => dirs.Where(d => d.Name.StartsWith(partialName)));
        /// </code>
        /// </example>
        Task<TMapping> AddAccountStoreAsync<TSource>(Func<IAsyncQueryable<TSource>, IAsyncQueryable<TSource>> query, CancellationToken cancellationToken = default(CancellationToken))
            where TSource : IAccountStore;
    }
}
