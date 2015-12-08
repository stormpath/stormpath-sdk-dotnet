// <copyright file="SyncAccountStoreContainerExtensions.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.AccountStore;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IAccountStoreContainer"/>.
    /// </summary>
    public static class SyncAccountStoreContainerExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IAccountStore"/> (either a <see cref="Group.IGroup"/> or <see cref="Directory.IDirectory"/>)
        /// used to persist new <see cref="Account.IAccount">Accounts</see>, or <see langword="null"/> if no default Account Store has been designated.
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <returns>The default <see cref="IAccountStore"/> for new <see cref="Account.IAccount">Accounts</see>,
        /// or <see langword="null"/> if no default <see cref="IAccountStore"/> has been designated.</returns>
        /// <example>
        /// Getting and using the default account store:
        /// <code>
        /// var accountStore = application.GetDefaultAccountStore();
        /// var accountStoreAsDirectory = accountStore as IDirectory;
        /// var accountStoreAsGroup = accountStore as IGroup;
        /// if (accountStoreAsDirectory != null)
        ///     // use as directory
        /// else if (accountStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        public static IAccountStore GetDefaultAccountStore(this IAccountStoreContainer container)
            => (container as IAccountStoreContainerSync).GetDefaultAccountStore();

        /// <summary>
        /// Synchronously sets the <see cref="IAccountStore"/> (either a <see cref="Group.IGroup"/> or a <see cref="Directory.IDirectory"/>)
        /// used to persist new <see cref="Account.IAccount">Accounts</see>.
        /// <para>
        /// Because this resource is not an <see cref="IAccountStore"/> itself, it delegates to a Group or Directory
        /// when creating accounts; this method sets the <see cref="IAccountStore"/> to which new account persistence is delegated.
        /// </para>
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new accounts.</param>
        public static void SetDefaultAccountStore(this IAccountStoreContainer container, IAccountStore accountStore)
            => (container as IAccountStoreContainerSync).SetDefaultAccountStore(accountStore);

        /// <summary>
        /// Synchronously gets the <see cref="IAccountStore"/> used to persist new <see cref="Group.IGroup">Groups</see>, or <see langword="null"/>
        /// if no default Account Store has been designated.
        /// <para>
        /// Stormpath's current REST API requires this to be a <see cref="Directory.IDirectory"/>.
        /// However, this could be a <see cref="Group.IGroup"/> in the future, so do not assume it is always a
        /// Directory if you want your code to be function correctly if/when this support is added.
        /// </para>
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <returns>The default <see cref="IAccountStore"/> for new <see cref="Group.IGroup">Groups</see>,
        /// or <see langword="null"/> if no default <see cref="IAccountStore"/> has been designated.</returns>
        /// <example>
        /// Getting and using the default group store:
        /// <code>
        /// var groupStore = application.GetDefaultGroupStore();
        /// var groupStoreAsDirectory = groupStore as IDirectory;
        /// var groupStoreAsGroup = groupStore as IGroup;
        /// if (groupStoreAsDirectory != null)
        ///     // use as directory
        /// else if (groupStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        public static IAccountStore GetDefaultGroupStore(this IAccountStoreContainer container)
            => (container as IAccountStoreContainerSync).GetDefaultGroupStore();

        /// <summary>
        /// Synchronously sets the <see cref="IAccountStore"/> (a <see cref="Directory.IDirectory"/>)
        /// used to persist new <see cref="Group.IGroup">Groups</see>.
        /// <para>
        /// Stormpath's current REST API requires this to be a <see cref="Directory.IDirectory"/>. However, this could be a <see cref="Group.IGroup"/> in the future,
        /// so do not assume it is always a Directory if you want your code to function properly if/when this support is added.
        /// </para>
        /// <para>
        /// Because this resource is not an <see cref="IAccountStore"/> itself, it delegates to a Directory
        /// when creating groups; this method sets the <see cref="IAccountStore"/> to which new group persistence is delegated.
        /// </para>
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new groups.</param>
        public static void SetDefaultGroupStore(this IAccountStoreContainer container, IAccountStore accountStore)
            => (container as IAccountStoreContainerSync).SetDefaultGroupStore(accountStore);

        /// <summary>
        /// Synchronously creates a new <see cref="IAccountStoreMapping"/>, allowing the associated Account Store
        /// to be used a source of accounts that may log in to the Application or Organization.
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping"/> resource to add to the AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The AccountStoreMapping's ListIndex is negative, or the mapping could not be added to the Application or Organization.</exception>
        /// <example>
        /// Setting a new <see cref="IAccountStoreMapping"/>'s <see cref="IAccountStoreMapping.ListIndex"/> to <c>500</c> and then adding the mapping to
        /// an application with an existing 3-item list will automatically save the <see cref="IAccountStoreMapping"/>
        /// at the end of the list and set its <see cref="IAccountStoreMapping.ListIndex"/> value to <c>3</c> (items at index 0, 1, 2 were the original items,
        /// the new fourth item will be at index 3):
        /// <code>
        /// IAccountStore directoryOrGroup = GetDirectoryOrGroup();
        /// IAccountStoreMapping mapping = client.Instantiate&lt;IAccountStoreMapping&gt;();
        /// mapping.SetAccountStore(directoryOrGroup);
        /// mapping.SetListIndex(500);
        /// mapping = application.CreateAccountStoreMapping(mapping);
        /// </code>
        /// </example>
        public static IAccountStoreMapping CreateAccountStoreMapping(this IAccountStoreContainer container, IAccountStoreMapping mapping)
            => (container as IAccountStoreContainerSync).CreateAccountStoreMapping(mapping);

        /// <summary>
        /// Synchronously adds a new <see cref="IAccountStore"/> to the Application or Organization and appends the resulting <see cref="IAccountStoreMapping"/>
        /// to the end of the AccountStoreMapping list.
        /// <para>
        /// If you need to control the order of the added AccountStore, use the <see cref="CreateAccountStoreMapping(IAccountStoreContainer, IAccountStoreMapping)"/> method.
        /// </para>
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <param name="accountStore">The new <see cref="IAccountStore"/> resource to add to the AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application or Organization.</exception>
        /// <example>
        /// <code>
        /// IAccountStore directoryOrGroup = GetDirectoryOrGroup();
        /// IAccountStoreMapping mapping = application.AddAccountStore(directoryOrGroup);
        /// </code>
        /// </example>
        public static IAccountStoreMapping AddAccountStore(this IAccountStoreContainer container, IAccountStore accountStore)
            => (container as IAccountStoreContainerSync).AddAccountStore(accountStore);

        /// <summary>
        /// Synchronously adds a new <see cref="IAccountStore"/> to this Application. The given string can either be an <c>href</c> or a name of a
        /// <see cref="Directory.IDirectory"/> or <see cref="Group.IGroup"/> belonging to the current <see cref="Tenant.ITenant"/>.
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
        /// <param name="container">The Account Store container.</param>
        /// <param name="hrefOrName">Either the <c>href</c> or name of the desired <see cref="Directory.IDirectory"/> or <see cref="Group.IGroup"/>.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application or Organization.</exception>
        /// <exception cref="ArgumentException">The given <paramref name="hrefOrName"/> matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Providing an href:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = application.AddAccountStore("https://api.stormpath.com/v1/groups/2rwq022yMt4u2DwKLfzriP");
        /// </code>
        /// Providing a name:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = application.AddAccountStore("Foo Name");
        /// </code>
        /// </example>
        public static IAccountStoreMapping AddAccountStore(this IAccountStoreContainer container, string hrefOrName)
            => (container as IAccountStoreContainerSync).AddAccountStore(hrefOrName);

        /// <summary>
        /// Synchronously adds a resource of type <typeparamref name="T"/> as a new <see cref="IAccountStore"/> to this Application or Organization. The provided query
        /// must match a single <typeparamref name="T"/> in the current <see cref="Tenant.ITenant">Tenant</see>. If no compatible resource matches the query, this method will return <see langword="null"/>.
        /// </summary>
        /// <param name="container">The Account Store container.</param>
        /// <param name="query">Query to search for a resource of type <typeparamref name="T"/> in the current Tenant.</param>
        /// <typeparam name="T">The type of resource (either a <see cref="Directory.IDirectory"/> or a <see cref="Group.IGroup"/>) to query for.</typeparam>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>, or <see langword="null"/> if there is no resource matching the query.</returns>
        /// <exception cref="Error.ResourceException">The found resource already exists as an account store in the application.</exception>
        /// <exception cref="ArgumentException">The query matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Adding a directory by partial name:
        /// <code>
        /// IAccountStoreMapping mapping = application.AddAccountStore&lt;IDirectory&gt;(dirs => dirs.Where(d => d.Name.StartsWith(partialName)));
        /// </code>
        /// </example>
        public static IAccountStoreMapping AddAccountStore<T>(this IAccountStoreContainer container, Func<IQueryable<T>, IQueryable<T>> query)
            where T : IAccountStore
            => (container as IAccountStoreContainerSync).AddAccountStore(query);
    }
}
