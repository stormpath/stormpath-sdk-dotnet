// <copyright file="DefaultApplication.IAccountStore.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IAsyncQueryable<IAccountStoreMapping> IApplication.GetAccountStoreMappings()
            => new CollectionResourceQueryable<IAccountStoreMapping>(this.AccountStoreMappings.Href, this.GetInternalAsyncDataStore());

        async Task<IAccountStore> IApplication.GetDefaultAccountStoreAsync(CancellationToken cancellationToken)
        {
            if (this.DefaultAccountStoreMapping.Href == null)
            {
                return null;
            }

            var accountStoreMapping = await this.GetInternalAsyncDataStore()
                .GetResourceAsync<IAccountStoreMapping>(this.DefaultAccountStoreMapping.Href, cancellationToken)
                .ConfigureAwait(false);

            return accountStoreMapping == null
                ? null
                : await accountStoreMapping.GetAccountStoreAsync().ConfigureAwait(false);
        }

        async Task<IAccountStore> IApplication.GetDefaultGroupStoreAsync(CancellationToken cancellationToken)
        {
            if (this.DefaultGroupStoreMapping.Href == null)
            {
                return null;
            }

            var groupStoreMapping = await this.GetInternalAsyncDataStore().GetResourceAsync<IAccountStoreMapping>(this.DefaultAccountStoreMapping.Href, cancellationToken).ConfigureAwait(false);

            return groupStoreMapping == null
                ? null
                : await groupStoreMapping.GetAccountStoreAsync().ConfigureAwait(false);
        }

        async Task IApplication.SetDefaultAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(nameof(accountStore.Href));
            }

            IAccountStoreMapping newOrExistingMapping = null;

            await this.AsInterface.GetAccountStoreMappings().ForEachAsync(
                mapping =>
            {
                bool isPassedAccountStore = (mapping as AccountStore.DefaultAccountStoreMapping)?.AccountStore?.Href.Equals(accountStore.Href) ?? false;
                if (isPassedAccountStore)
                {
                    newOrExistingMapping = mapping;
                }

                return newOrExistingMapping != null; // break if found
            }, cancellationToken).ConfigureAwait(false);

            if (newOrExistingMapping == null)
            {
                newOrExistingMapping = await this.AsInterface
                    .AddAccountStoreAsync(accountStore, cancellationToken).ConfigureAwait(false);
            }

            newOrExistingMapping.SetDefaultAccountStore(true);
            await newOrExistingMapping.SaveAsync(cancellationToken).ConfigureAwait(false);
            this.SetLinkProperty(DefaultAccountStoreMappingPropertyName, newOrExistingMapping.Href);

            await this.AsInterface.SaveAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task IApplication.SetDefaultGroupStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(nameof(accountStore.Href));
            }

            IAccountStoreMapping newOrExistingMapping = null;

            await this.AsInterface.GetAccountStoreMappings().ForEachAsync(
                mapping =>
                {
                    bool isPassedAccountStore = (mapping as AccountStore.DefaultAccountStoreMapping)?.AccountStore?.Href.Equals(accountStore.Href) ?? false;
                    if (isPassedAccountStore)
                    {
                        newOrExistingMapping = mapping;
                    }

                    return newOrExistingMapping != null; // break if found
                }, cancellationToken).ConfigureAwait(false);

            if (newOrExistingMapping == null)
            {
                newOrExistingMapping = await this.AsInterface
                    .AddAccountStoreAsync(accountStore, cancellationToken).ConfigureAwait(false);
            }

            newOrExistingMapping.SetDefaultGroupStore(true);
            await newOrExistingMapping.SaveAsync(cancellationToken).ConfigureAwait(false);
            this.SetLinkProperty(DefaultGroupStoreMappingPropertyName, newOrExistingMapping.Href);

            await this.AsInterface.SaveAsync(cancellationToken).ConfigureAwait(false);
        }

        Task<IAccountStoreMapping> IApplication.CreateAccountStoreMappingAsync(IAccountStoreMapping mapping, CancellationToken cancellationToken)
        {
            var href = "/accountStoreMappings";

            mapping.SetApplication(this);
            return this.GetInternalAsyncDataStore().CreateAsync(href, mapping, cancellationToken);
        }

        Task<IAccountStoreMapping> IApplication.AddAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
        {
            var accountStoreMapping = this.GetInternalAsyncDataStore()
                .Instantiate<IAccountStoreMapping>()
                .SetAccountStore(accountStore)
                .SetApplication(this)
                .SetListIndex(int.MaxValue);

            return this.AsInterface.CreateAccountStoreMappingAsync(accountStoreMapping, cancellationToken);
        }

        async Task<IAccountStoreMapping> IApplication.AddAccountStoreAsync(string hrefOrName, CancellationToken cancellationToken)
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
                if (splitHrefOrName.Length == this.AsInterface.Href.Split('/').Length)
                {
                    if (splitHrefOrName[4].Equals("directories", StringComparison.InvariantCultureIgnoreCase))
                    {
                        isDirectoryType = true;
                    }
                    else if (splitHrefOrName[4].Equals("groups", StringComparison.InvariantCultureIgnoreCase))
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
                            accountStore = await this.GetInternalAsyncDataStore().GetResourceAsync<IDirectory>(hrefOrName, cancellationToken).ConfigureAwait(false);
                        }
                        else if (isDirectoryType == false)
                        {
                            accountStore = await this.GetInternalAsyncDataStore().GetResourceAsync<IGroup>(hrefOrName, cancellationToken).ConfigureAwait(false);
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
                var directory = await this.GetSingleTenantDirectoryAsync(x => x.Where(d => d.Name == hrefOrName), cancellationToken).ConfigureAwait(false);
                var group = await this.GetSingleTenantGroupAsync(x => x.Where(g => g.Name == hrefOrName), cancellationToken).ConfigureAwait(false);

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
                return await this.AsInterface.AddAccountStoreAsync(accountStore, cancellationToken).ConfigureAwait(false);
            }

            // Could not find any resource matching the hrefOrName value
            return null;
        }

        async Task<IAccountStoreMapping> IApplication.AddAccountStoreAsync<T>(Func<IAsyncQueryable<T>, IAsyncQueryable<T>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            IAccountStore foundAccountStore = null;
            if (typeof(T) == typeof(IDirectory))
            {
                var directoryQuery = query as Func<IAsyncQueryable<IDirectory>, IAsyncQueryable<IDirectory>>;
                foundAccountStore = await this.GetSingleTenantDirectoryAsync(directoryQuery, cancellationToken).ConfigureAwait(false);
            }
            else if (typeof(T) == typeof(IGroup))
            {
                var groupQuery = query as Func<IAsyncQueryable<IGroup>, IAsyncQueryable<IGroup>>;
                foundAccountStore = await this.GetSingleTenantGroupAsync(groupQuery, cancellationToken).ConfigureAwait(false);
            }

            if (foundAccountStore != null)
            {
                return await this.AsInterface.AddAccountStoreAsync(foundAccountStore, cancellationToken).ConfigureAwait(false);
            }

            // No account store can be added
            return null;
        }

        private async Task<IDirectory> GetSingleTenantDirectoryAsync(Func<IAsyncQueryable<IDirectory>, IAsyncQueryable<IDirectory>> queryAction, CancellationToken cancellationToken)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = await this.AsInterface.GetTenantAsync(cancellationToken).ConfigureAwait(false);
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

        private async Task<IGroup> GetSingleTenantGroupAsync(Func<IAsyncQueryable<IGroup>, IAsyncQueryable<IGroup>> queryAction, CancellationToken cancellationToken)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = await this.AsInterface.GetTenantAsync(cancellationToken).ConfigureAwait(false);
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
    }
}
