// <copyright file="AccountStoreContainerShared.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.AccountStore
{
    /// <summary>
    /// Contains shared methods for manipulating AccountStores.
    /// </summary>
    internal static class AccountStoreContainerShared
    {
        public static readonly string DefaultAccountStoreMappingPropertyName = "defaultAccountStoreMapping";
        public static readonly string DefaultGroupStoreMappingPropertyName = "defaultGroupStoreMapping";

        /// <summary>
        /// Gets the default Account or Group Store at the given <c>href</c>, if it exists.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="storeMappingHref">The AccountStoreMapping <c>href</c>.</param>
        /// <param name="internalDataStore">The <see cref="IInternalAsyncDataStore"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccountStore">Account Store</see>, or <see langword="null"/>.</returns>
        public static async Task<IAccountStore> GetDefaultStoreAsync<TMapping>(
            string storeMappingHref,
            IInternalAsyncDataStore internalDataStore,
            CancellationToken cancellationToken)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (string.IsNullOrEmpty(storeMappingHref))
            {
                return null;
            }

            var accountStoreMapping = await internalDataStore
                .GetResourceAsync<TMapping>(storeMappingHref, cancellationToken)
                .ConfigureAwait(false);

            return accountStoreMapping == null
                ? null
                : await accountStoreMapping.GetAccountStoreAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronously gets the default Account or Group Store at the given <c>href</c>, if it exists.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="accountStoreMappingHref">The AccountStoreMapping <c>href</c>.</param>
        /// <param name="internalDataStore">The <see cref="IInternalAsyncDataStore"/>.</param>
        /// <returns>The <see cref="IAccountStore">Account Store</see>, or <see langword="null"/>.</returns>
        public static IAccountStore GetDefaultStore<TMapping>(string accountStoreMappingHref, IInternalSyncDataStore internalDataStore)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (string.IsNullOrEmpty(accountStoreMappingHref))
            {
                return null;
            }

            var accountStoreMapping = internalDataStore
                .GetResource<TMapping>(accountStoreMappingHref);

            return accountStoreMapping == null
                ? null
                : accountStoreMapping.GetAccountStore();
        }

        /// <summary>
        /// Sets the default Account or Group store.
        /// </summary>
        /// <typeparam name="T">The parent resource type.</typeparam>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="parent">The parent resource.</param>
        /// <param name="store">The new default Account or Group store.</param>
        /// <param name="isAccountStore">Determines whether this store should be the default Account (<see langword="true"/>) or Group (<see langword="false"/>) Store.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task SetDefaultStoreAsync<T, TMapping>(
            ISaveable<T> parent,
            IAccountStore store,
            bool isAccountStore,
            CancellationToken cancellationToken)
            where T : IResource, IAccountStoreContainer<TMapping>
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (string.IsNullOrEmpty(store?.Href))
            {
                throw new ArgumentNullException(nameof(store.Href));
            }

            var container = parent as IAccountStoreContainer<TMapping>;
            if (parent == null)
            {
                throw new Exception("SetDefaultStore must be used with a supported AccountStoreContainer.");
            }

            IAccountStoreMapping<TMapping> newOrExistingMapping = null;
            await container.GetAccountStoreMappings().ForEachAsync(
                mapping =>
                {
                    bool isPassedAccountStore = (mapping as AbstractAccountStoreMapping<TMapping>)?.AccountStore?.Href.Equals(store.Href) ?? false;
                    if (isPassedAccountStore)
                    {
                        newOrExistingMapping = mapping;
                    }

                    return newOrExistingMapping != null; // break if found
                }, cancellationToken).ConfigureAwait(false);

            if (newOrExistingMapping == null)
            {
                newOrExistingMapping = await container
                    .AddAccountStoreAsync(store, cancellationToken).ConfigureAwait(false);
            }

            if (isAccountStore)
            {
                newOrExistingMapping.SetDefaultAccountStore(true);
            }
            else
            {
                newOrExistingMapping.SetDefaultGroupStore(true);
            }

            await newOrExistingMapping.SaveAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronously sets the default Account or Group store.
        /// </summary>
        /// <typeparam name="T">The parent resource type.</typeparam>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="parent">The parent resource.</param>
        /// <param name="store">The new default Account or Group store.</param>
        /// <param name="isAccountStore">Determines whether this store should be the default Account (<see langword="true"/>) or Group (<see langword="false"/>) Store.</param>
        public static void SetDefaultStore<T, TMapping>(ISaveable<T> parent, IAccountStore store, bool isAccountStore)
            where T : IResource, IAccountStoreContainer<TMapping>
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (string.IsNullOrEmpty(store?.Href))
            {
                throw new ArgumentNullException(nameof(store.Href));
            }

            var container = parent as IAccountStoreContainer<TMapping>;
            if (parent == null)
            {
                throw new Exception("SetDefaultStore must be used with a supported AccountStoreContainer.");
            }

            TMapping newOrExistingMapping = null;
            foreach (var mapping in container.GetAccountStoreMappings().Synchronously())
            {
                bool isPassedAccountStore = (mapping as AbstractAccountStoreMapping<TMapping>)?.AccountStore?.Href.Equals(store.Href) ?? false;
                if (isPassedAccountStore)
                {
                    newOrExistingMapping = mapping;
                }

                if (newOrExistingMapping != null)
                {
                    break;
                }
            }

            if (newOrExistingMapping == null)
            {
                newOrExistingMapping = container
                    .AddAccountStore(store);
            }

            if (isAccountStore)
            {
                newOrExistingMapping.SetDefaultAccountStore(true);
            }
            else
            {
                newOrExistingMapping.SetDefaultGroupStore(true);
            }

            newOrExistingMapping.Save();
        }

        /// <summary>
        /// Creates a new Account Store Mapping.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="mapping">The new mapping to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static async Task<TMapping> CreateAccountStoreMappingAsync<TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalAsyncDataStore internalDataStore,
            IAccountStoreMapping<TMapping> mapping,
            CancellationToken cancellationToken)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            SetContainer(mapping, container);
            var href = ResolveEndpointHref(container);

            return (TMapping)(await internalDataStore.CreateAsync(href, mapping, cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Synchronously creates a new Account Store Mapping.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="mapping">The new mapping to create.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static TMapping CreateAccountStoreMapping<TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalSyncDataStore internalDataStore,
            IAccountStoreMapping<TMapping> mapping)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            SetContainer(mapping, container);
            var href = ResolveEndpointHref(container);

            return (TMapping)internalDataStore.Create(href, mapping);
        }

        /// <summary>
        /// Adds an Account Store to this resource.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="accountStore">The Account Store to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static async Task<TMapping> AddAccountStoreAsync<TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalAsyncDataStore internalDataStore,
            IAccountStore accountStore,
            CancellationToken cancellationToken)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            var accountStoreMapping = internalDataStore
                .Instantiate<IAccountStoreMapping<TMapping>>()
                .SetAccountStore(accountStore)
                .SetListIndex(int.MaxValue);
            SetContainer(accountStoreMapping, container);

            return (TMapping)(await CreateAccountStoreMappingAsync(container, internalDataStore, accountStoreMapping, cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Synchronously adds an Account Store to this resource.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="accountStore">The Account Store to add.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static TMapping AddAccountStore<TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalSyncDataStore internalDataStore,
            IAccountStore accountStore)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            var accountStoreMapping = internalDataStore
                .Instantiate<IAccountStoreMapping<TMapping>>()
                .SetAccountStore(accountStore)
                .SetListIndex(int.MaxValue);
            SetContainer(accountStoreMapping, container);

            return CreateAccountStoreMapping(container, internalDataStore, accountStoreMapping);
        }

        /// <summary>
        /// Adds an Account Store to this resource by <c>href</c> or name.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="hrefOrName">The name or <c>href</c> of the Account Store to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static async Task<TMapping> AddAccountStoreAsync<TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalAsyncDataStore internalDataStore,
            string hrefOrName,
            CancellationToken cancellationToken)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            IAccountStore accountStore = null;

            var splitHrefOrName = hrefOrName.Split('/');
            bool looksLikeHref = splitHrefOrName.Length > 4;
            if (looksLikeHref)
            {
                bool? isDirectoryType = null;
                if (splitHrefOrName.Length == container.Href.Split('/').Length)
                {
                    if (splitHrefOrName[4].Equals("directories", StringComparison.OrdinalIgnoreCase))
                    {
                        isDirectoryType = true;
                    }
                    else if (splitHrefOrName[4].Equals("groups", StringComparison.OrdinalIgnoreCase))
                    {
                        isDirectoryType = false;
                    }
                }

                if (isDirectoryType != null)
                {
                    try
                    {
                        if (isDirectoryType == true)
                        {
                            accountStore = await internalDataStore.GetResourceAsync<IDirectory>(hrefOrName, cancellationToken).ConfigureAwait(false);
                        }
                        else if (isDirectoryType == false)
                        {
                            accountStore = await internalDataStore.GetResourceAsync<IGroup>(hrefOrName, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (ResourceException)
                    {
                        // It looked like an href, but no resource was found.
                        // We'll try looking it up by name.
                    }
                }
            }

            if (accountStore == null)
            {
                // Try to find both a Directory and a Group with the given name
                var directory = await GetSingleTenantDirectoryAsync(container, x => x.Where(d => d.Name == hrefOrName), cancellationToken).ConfigureAwait(false);
                var group = await GetSingleTenantGroupAsync(container, x => x.Where(g => g.Name == hrefOrName), cancellationToken).ConfigureAwait(false);

                if (directory != null && group != null)
                {
                    throw new ArgumentException(
                        "There is both a Directory and a Group matching the provided name in the current tenant. " +
                        "Please provide the href of the intended Resource instead of its name in order to unambiguously identify it.");
                }

                accountStore = directory != null
                    ? directory as IAccountStore
                    : group as IAccountStore;
            }

            if (accountStore != null)
            {
                return (TMapping)await AddAccountStoreAsync(container, internalDataStore, accountStore, cancellationToken).ConfigureAwait(false);
            }

            // Could not find any resource matching the hrefOrName value
            return null;
        }

        /// <summary>
        /// Synchronously adds an Account Store to this resource by <c>href</c> or name.
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="hrefOrName">The name or <c>href</c> of the Account Store to add.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static TMapping AddAccountStore<TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalSyncDataStore internalDataStore,
            string hrefOrName)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            IAccountStore accountStore = null;

            var splitHrefOrName = hrefOrName.Split('/');
            bool looksLikeHref = splitHrefOrName.Length > 4;
            if (looksLikeHref)
            {
                bool? isDirectoryType = null;
                if (splitHrefOrName.Length == container.Href.Split('/').Length)
                {
                    if (splitHrefOrName[4].Equals("directories", StringComparison.OrdinalIgnoreCase))
                    {
                        isDirectoryType = true;
                    }
                    else if (splitHrefOrName[4].Equals("groups", StringComparison.OrdinalIgnoreCase))
                    {
                        isDirectoryType = false;
                    }
                }

                if (isDirectoryType != null)
                {
                    try
                    {
                        if (isDirectoryType == true)
                        {
                            accountStore = internalDataStore.GetResource<IDirectory>(hrefOrName);
                        }
                        else if (isDirectoryType == false)
                        {
                            accountStore = internalDataStore.GetResource<IGroup>(hrefOrName);
                        }
                    }
                    catch (ResourceException)
                    {
                        // It looked like an href, but no resource was found.
                        // We'll try looking it up by name.
                    }
                }
            }

            if (accountStore == null)
            {
                // Try to find both a Directory and a Group with the given name
                var directory = GetSingleTenantDirectory(container, x => x.Where(d => d.Name == hrefOrName));
                var group = GetSingleTenantGroup(container, x => x.Where(g => g.Name == hrefOrName));

                if (directory != null && group != null)
                {
                    throw new ArgumentException(
                        "There is both a Directory and a Group matching the provided name in the current tenant. " +
                        "Please provide the href of the intended Resource instead of its name in order to unambiguously identify it.");
                }

                accountStore = directory != null
                    ? directory as IAccountStore
                    : group as IAccountStore;
            }

            if (accountStore != null)
            {
                return AddAccountStore(container, internalDataStore, accountStore);
            }

            // Could not find any resource matching the hrefOrName value
            return null;
        }

        /// <summary>
        /// Adds an Account Store to this resource based on the result of a query.
        /// </summary>
        /// <typeparam name="T">The Account Store type.</typeparam>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="query">A query that selects a single Account Store.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static async Task<TMapping> AddAccountStoreAsync<T, TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalAsyncDataStore internalDataStore,
            Func<IAsyncQueryable<T>, IAsyncQueryable<T>> query,
            CancellationToken cancellationToken)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            IAccountStore foundAccountStore = null;
            if (typeof(T) == typeof(IDirectory))
            {
                var directoryQuery = query as Func<IAsyncQueryable<IDirectory>, IAsyncQueryable<IDirectory>>;
                foundAccountStore = await GetSingleTenantDirectoryAsync(container, directoryQuery, cancellationToken).ConfigureAwait(false);
            }
            else if (typeof(T) == typeof(IGroup))
            {
                var groupQuery = query as Func<IAsyncQueryable<IGroup>, IAsyncQueryable<IGroup>>;
                foundAccountStore = await GetSingleTenantGroupAsync(container, groupQuery, cancellationToken).ConfigureAwait(false);
            }

            if (foundAccountStore != null)
            {
                return (TMapping)await AddAccountStoreAsync(container, internalDataStore, foundAccountStore, cancellationToken).ConfigureAwait(false);
            }

            // No account store can be added
            return null;
        }

        /// <summary>
        /// Synchronously adds an Account Store to this resource based on the result of a query.
        /// </summary>
        /// <typeparam name="T">The Account Store type.</typeparam>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="container">The Account Store container.</param>
        /// <param name="internalDataStore">The internal data store.</param>
        /// <param name="query">A query that selects a single Account Store.</param>
        /// <returns>The new <see cref="IAccountStoreMapping"/>.</returns>
        public static TMapping AddAccountStore<T, TMapping>(
            IAccountStoreContainer<TMapping> container,
            IInternalSyncDataStore internalDataStore,
            Func<IQueryable<T>, IQueryable<T>> query)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            IAccountStore foundAccountStore = null;
            if (typeof(T) == typeof(IDirectory))
            {
                var directoryQuery = query as Func<IQueryable<IDirectory>, IQueryable<IDirectory>>;
                foundAccountStore = GetSingleTenantDirectory(container, directoryQuery);
            }
            else if (typeof(T) == typeof(IGroup))
            {
                var groupQuery = query as Func<IQueryable<IGroup>, IQueryable<IGroup>>;
                foundAccountStore = GetSingleTenantGroup(container, groupQuery);
            }

            if (foundAccountStore != null)
            {
                return AddAccountStore(container, internalDataStore, foundAccountStore);
            }

            // No account store can be added
            return null;
        }

        /// <summary>
        /// Gets a single <see cref="IDirectory">Directory</see> from a query.
        /// </summary>
        /// <param name="parent">The parent resource that contains a link to a <see cref="ITenant">Tenant</see>.</param>
        /// <param name="queryAction">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The located <see cref="IDirectory">Directory</see>.</returns>
        private static async Task<IDirectory> GetSingleTenantDirectoryAsync(
            IHasTenant parent,
            Func<IAsyncQueryable<IDirectory>, IAsyncQueryable<IDirectory>> queryAction,
            CancellationToken cancellationToken)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = await parent.GetTenantAsync(cancellationToken).ConfigureAwait(false);
            var queryable = tenant.GetDirectories();
            queryable = queryAction(queryable);

            IDirectory foundDirectory = null;
            await queryable.ForEachAsync(
                dir =>
                {
                    if (foundDirectory != null)
                    {
                        throw new ArgumentException("The provided query matched more than one Directory in the current Tenant.", nameof(queryAction));
                    }

                    foundDirectory = dir;
                }, cancellationToken).ConfigureAwait(false);

            return foundDirectory;
        }

        /// <summary>
        /// Synchronously gets a single <see cref="IDirectory">Directory</see> from a query.
        /// </summary>
        /// <param name="parent">The parent resource that contains a link to a <see cref="ITenant">Tenant</see>.</param>
        /// <param name="queryAction">The query.</param>
        /// <returns>The located <see cref="IDirectory">Directory</see>.</returns>
        private static IDirectory GetSingleTenantDirectory(
            IHasTenant parent,
            Func<IQueryable<IDirectory>, IQueryable<IDirectory>> queryAction)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = parent.GetTenant();
            var queryable = tenant.GetDirectories().Synchronously();
            queryable = queryAction(queryable);

            IDirectory foundDirectory = null;
            foreach (var dir in queryable)
            {
                if (foundDirectory != null)
                {
                    throw new ArgumentException("The provided query matched more than one Directory in the current Tenant.", nameof(queryAction));
                }

                foundDirectory = dir;
            }

            return foundDirectory;
        }

        /// <summary>
        /// Gets a single <see cref="IGroup">Group</see> from a query.
        /// </summary>
        /// <param name="parent">The parent resource that contains a link to a <see cref="ITenant">Tenant</see>.</param>
        /// <param name="queryAction">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The located <see cref="IGroup">Group</see>.</returns>
        private static async Task<IGroup> GetSingleTenantGroupAsync(
            IHasTenant parent,
            Func<IAsyncQueryable<IGroup>, IAsyncQueryable<IGroup>> queryAction,
            CancellationToken cancellationToken)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = await parent.GetTenantAsync(cancellationToken).ConfigureAwait(false);
            var queryable = tenant.GetGroups();
            queryable = queryAction(queryable);

            IGroup foundGroup = null;
            await queryable.ForEachAsync(
                group =>
                {
                    if (foundGroup != null)
                    {
                        throw new ArgumentException("The provided query matched more than one Directory in the current Tenant.", nameof(queryAction));
                    }

                    foundGroup = group;
                }, cancellationToken).ConfigureAwait(false);

            return foundGroup;
        }

        /// <summary>
        /// Synchronously gets a single <see cref="IGroup">Group</see> from a query.
        /// </summary>
        /// <param name="parent">The parent resource that contains a link to a <see cref="ITenant">Tenant</see>.</param>
        /// <param name="queryAction">The query.</param>
        /// <returns>The located <see cref="IGroup">Group</see>.</returns>
        private static IGroup GetSingleTenantGroup(
            IHasTenant parent,
            Func<IQueryable<IGroup>, IQueryable<IGroup>> queryAction)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = parent.GetTenant();
            var queryable = tenant.GetGroups().Synchronously();
            queryable = queryAction(queryable);

            IGroup foundGroup = null;
            foreach (var group in queryable)
            {
                if (foundGroup != null)
                {
                    throw new ArgumentException("The provided query matched more than one Directory in the current Tenant.", nameof(queryAction));
                }

                foundGroup = group;
            }

            return foundGroup;
        }

        /// <summary>
        /// TODO: Remove this when refactoring AbstractAccountStoreMapping (breaking change)
        /// </summary>
        /// <typeparam name="TMapping">The Account Store Mapping type.</typeparam>
        /// <param name="mapping">The account store mapping.</param>
        /// <param name="container">The container object (an Application or Organization).</param>
        private static void SetContainer<TMapping>(IAccountStoreMapping<TMapping> mapping, IAccountStoreContainer<TMapping> container)
            where TMapping : class, IAccountStoreMapping<TMapping>
        {
            var asApplication = container as IApplication;
            if (asApplication != null)
            {
                (mapping as IApplicationAccountStoreMapping).SetApplication(asApplication);
                return;
            }

            var asOrganization = container as IOrganization;
            if (asOrganization != null)
            {
                (mapping as IOrganizationAccountStoreMapping).SetOrganization(asOrganization);
                return;
            }

            throw new NotImplementedException("Unknown container type!");
        }

        private static string ResolveEndpointHref<TMapping>(IAccountStoreContainer<TMapping> container)
            where TMapping : IAccountStoreMapping<TMapping>
        {
            if (container is IApplication)
            {
                return "/accountStoreMappings";
            }

            if (container is IOrganization)
            {
                return "/organizationAccountStoreMappings";
            }

            throw new NotSupportedException($"This Account Store container type is not supported: {container.GetType().Name}");
        }
    }
}
