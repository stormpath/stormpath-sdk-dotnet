// <copyright file="AccountCreationOptionsBuilderExamples.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Organization;

namespace DocExamples
{
    public class IAccountStoreContainerExamples
    {
        public async Task GetDefaultAccountStore()
        {
            IApplication application = null;

            #region GetDefaultAccountStore
            var accountStore = await application.GetDefaultAccountStoreAsync();
            var accountStoreAsDirectory = accountStore as IDirectory;
            var accountStoreAsGroup = accountStore as IGroup;
            var accountStoreAsOrganization = accountStore as IOrganization;

            if (accountStoreAsDirectory != null)
            {
                // use as Directory
            }
            else if (accountStoreAsGroup != null)
            {
                // use as Group
            }
            else if (accountStoreAsOrganization != null)
            {
                // use as Organization
            }
            else
            {
                throw new Exception("Unknown Account Store type.");
            }
            #endregion
        }

        public async Task GetDefaultGroupStore()
        {
            IApplication application = null;

            #region GetDefaultGroupStore
            var accountStore = await application.GetDefaultAccountStoreAsync();
            var accountStoreAsDirectory = accountStore as IDirectory;

            // Currently, Group Stores can only be Directories
            if (accountStoreAsDirectory != null)
            {
                // use as Directory
            }
            else
            {
                throw new Exception("Unknown Group Store type.");
            }
            #endregion
        }

        public async Task CreateAccountStoreMapping()
        {
            IAccountStore directoryOrGroup = null;
            IClient client = null;
            IApplication application = null;

            #region CreateAccountStoreMapping
            // Setting a new Account Store Mapping's ListIndex to 500 and then adding the mapping to
            // an application with an existing 3-item list will automatically save the mapping
            // at the end of the list and set its ListIndex value to 3
            // (items at index 0, 1, 2 were the original items, the new fourth item will be at index 3)
            IApplicationAccountStoreMapping mapping = client.Instantiate<IApplicationAccountStoreMapping>();
            mapping.SetAccountStore(directoryOrGroup); // some IAccountStore
            mapping.SetListIndex(500);

            mapping = await application.CreateAccountStoreMappingAsync(mapping);
            #endregion
        }

        public async Task AddAccountStore()
        {
            IAccountStore directoryOrGroup = null;
            IApplication application = null;

            #region AddAccountStore
            IApplicationAccountStoreMapping mapping = 
                await application.AddAccountStoreAsync(directoryOrGroup);  // some IAccountStore
            #endregion
        }

        public async Task AddAccountStoreByHref()
        {
            IApplication application = null;

            #region AddAccountStoreByHref
            IApplicationAccountStoreMapping accountStoreMapping = await application.AddAccountStoreAsync("https://api.stormpath.com/v1/groups/myGroupHref");
            #endregion
        }

        public async Task AddAccountStoreByName()
        {
            IApplication application = null;

            #region AddAccountStoreByName
            IApplicationAccountStoreMapping accountStoreMapping = await application.AddAccountStoreAsync("My Store Name");
            #endregion
        }

        public async Task AddAccountStoreByQuery()
        {
            IApplication application = null;

            #region AddAccountStoreByQuery
            IApplicationAccountStoreMapping mapping = await application.AddAccountStoreAsync<IDirectory>(dirs => dirs.Where(d => d.Name.StartsWith("Partial Na")));
            #endregion
        }
    }
}
