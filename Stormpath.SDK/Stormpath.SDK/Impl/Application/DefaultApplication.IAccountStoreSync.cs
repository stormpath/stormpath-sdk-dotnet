// <copyright file="DefaultApplication.IAccountStoreSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IAccountStore IApplicationSync.GetDefaultAccountStore()
        {
            if (this.DefaultAccountStoreMapping.Href == null)
            {
                return null;
            }

            var accountStoreMapping = this.GetInternalAsyncDataStore().GetResource<IAccountStoreMapping>(this.DefaultAccountStoreMapping.Href);
            if (accountStoreMapping == null)
            {
                return null;
            }

            return accountStoreMapping.GetAccountStore();
        }

        IAccountStore IApplicationSync.GetDefaultGroupStore()
        {
            if (this.DefaultGroupStoreMapping.Href == null)
            {
                return null;
            }

            var groupStoreMapping = this.GetInternalSyncDataStore().GetResource<IAccountStoreMapping>(this.DefaultAccountStoreMapping.Href);

            return groupStoreMapping == null
                ? null
                : groupStoreMapping.GetAccountStore();
        }

        // none of the below have been tested
        void IApplicationSync.SetDefaultAccountStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(nameof(accountStore.Href));
            }

            IAccountStoreMapping newOrExistingMapping = null;

            foreach (var mapping in this.AsInterface.GetAccountStoreMappings().Synchronously())
            {
                bool isPassedAccountStore = (mapping as AccountStore.DefaultAccountStoreMapping)?.AccountStore?.Href.Equals(accountStore.Href) ?? false;
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
                newOrExistingMapping = this.AsInterface
                    .AddAccountStore(accountStore);
            }

            newOrExistingMapping.SetDefaultAccountStore(true);
            newOrExistingMapping.Save();
            this.SetLinkProperty(DefaultAccountStoreMappingPropertyName, newOrExistingMapping.Href);

            this.AsInterface.Save();
        }

        void IApplicationSync.SetDefaultGroupStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(nameof(accountStore.Href));
            }

            IAccountStoreMapping newOrExistingMapping = null;

            foreach (var mapping in this.AsInterface.GetAccountStoreMappings().Synchronously())
            {
                bool isPassedAccountStore = (mapping as AccountStore.DefaultAccountStoreMapping)?.AccountStore?.Href.Equals(accountStore.Href) ?? false;
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
                newOrExistingMapping = this.AsInterface
                    .AddAccountStore(accountStore);
            }

            newOrExistingMapping.SetDefaultGroupStore(true);
            newOrExistingMapping.Save();
            this.SetLinkProperty(DefaultGroupStoreMappingPropertyName, newOrExistingMapping.Href);

            this.AsInterface.Save();
        }

        IAccountStoreMapping IApplicationSync.CreateAccountStoreMapping(IAccountStoreMapping mapping)
        {
            var href = "/accountStoreMappings";

            mapping.SetApplication(this);
            return this.GetInternalSyncDataStore().Create(href, mapping);
        }

        IAccountStoreMapping IApplicationSync.AddAccountStore(IAccountStore accountStore)
        {
            var accountStoreMapping = this.GetInternalSyncDataStore()
                .Instantiate<IAccountStoreMapping>()
                .SetAccountStore(accountStore)
                .SetApplication(this)
                .SetListIndex(int.MaxValue);

            return this.AsInterface.CreateAccountStoreMapping(accountStoreMapping);
        }

        IAccountStoreMapping IApplicationSync.AddAccountStore(string hrefOrName)
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
                            accountStore = this.GetInternalSyncDataStore().GetResource<IDirectory>(hrefOrName);
                        }
                        else if (isDirectoryType == false)
                        {
                            accountStore = this.GetInternalSyncDataStore().GetResource<IGroup>(hrefOrName);
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
                var directory = this.GetSingleTenantDirectory(x => x.Where(d => d.Name == hrefOrName));
                var group = this.GetSingleTenantGroup(x => x.Where(g => g.Name == hrefOrName));

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
                return this.AsInterface.AddAccountStore(accountStore);
            }

            // Could not find any resource matching the hrefOrName value
            return null;
        }

        IAccountStoreMapping IApplicationSync.AddAccountStore<T>(Func<IQueryable<T>, IQueryable<T>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            IAccountStore foundAccountStore = null;
            if (typeof(T) == typeof(IDirectory))
            {
                var directoryQuery = query as Func<IQueryable<IDirectory>, IQueryable<IDirectory>>;
                foundAccountStore = this.GetSingleTenantDirectory(directoryQuery);
            }
            else if (typeof(T) == typeof(IGroup))
            {
                var groupQuery = query as Func<IQueryable<IGroup>, IQueryable<IGroup>>;
                foundAccountStore = this.GetSingleTenantGroup(groupQuery);
            }

            if (foundAccountStore != null)
            {
                return this.AsInterface.AddAccountStore(foundAccountStore);
            }

            // No account store can be added
            return null;
        }

        private IDirectory GetSingleTenantDirectory(Func<IQueryable<IDirectory>, IQueryable<IDirectory>> queryAction)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = this.AsInterface.GetTenant();
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

        private IGroup GetSingleTenantGroup(Func<IQueryable<IGroup>, IQueryable<IGroup>> queryAction)
        {
            if (queryAction == null)
            {
                throw new ArgumentNullException(nameof(queryAction));
            }

            var tenant = this.AsInterface.GetTenant();
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
    }
}
