// <copyright file="DefaultAccount.IGroupSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultAccount
    {
        IGroupMembership IAccountSync.AddGroup(IGroup group)
            => DefaultGroupMembership.Create(this, group, this.GetInternalSyncDataStore());

        IGroupMembership IAccountSync.AddGroup(string hrefOrName)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            var group = this.FindGroupInDirectory(hrefOrName, this.Directory.Href);
            if (group == null)
            {
                throw new InvalidOperationException("The specified group was not found in the account's directory.");
            }

            return DefaultGroupMembership.Create(this, group, this.GetInternalSyncDataStore());
        }

        bool IAccountSync.RemoveGroup(IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            IGroupMembership foundMembership = null;

            foreach (var item in this.AsInterface.GetGroupMemberships().Synchronously())
            {
                if ((item as IInternalGroupMembership).GroupHref.Equals(group.Href, StringComparison.OrdinalIgnoreCase))
                {
                    foundMembership = item;
                }

                if (foundMembership != null)
                {
                    break;
                }
            }

            if (foundMembership == null)
            {
                throw new InvalidOperationException("This account does not belong to the specified group.");
            }

            return foundMembership.Delete();
        }

        bool IAccountSync.RemoveGroup(string hrefOrName)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            IGroupMembership foundMembership = null;
            foreach (var item in this.AsInterface.GetGroupMemberships().Synchronously())
            {
                IGroup group = item.GetGroup();
                if (group.Href.Equals(hrefOrName, StringComparison.OrdinalIgnoreCase) ||
                    group.Name.Equals(hrefOrName, StringComparison.OrdinalIgnoreCase))
                {
                    foundMembership = item;
                }

                if (foundMembership != null)
                {
                    break;
                }
            }

            if (foundMembership == null)
            {
                throw new InvalidOperationException("This account does not belong to the specified group.");
            }

            return foundMembership.Delete();
        }

        bool IAccountSync.IsMemberOfGroup(string hrefOrName)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            IGroup foundGroup = null;
            foreach (var item in this.AsInterface.GetGroups().Synchronously())
            {
                if (item.Name.Equals(hrefOrName, StringComparison.OrdinalIgnoreCase) ||
                        item.Href.Equals(hrefOrName, StringComparison.OrdinalIgnoreCase))
                {
                    foundGroup = item;
                }

                if (foundGroup != null)
                {
                    break;
                }
            }

            return foundGroup != null;
        }

        private IGroup FindGroupInDirectory(string hrefOrName, string directoryHref)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            if (string.IsNullOrEmpty(directoryHref))
            {
                throw new ArgumentNullException(nameof(directoryHref));
            }

            IGroup group = null;

            bool looksLikeHref = hrefOrName.Split('/').Length > 4;
            if (looksLikeHref)
            {
                try
                {
                    group = this.GetInternalSyncDataStore().GetResource<IGroup>(hrefOrName);

                    if ((group as DefaultGroup)?.Directory.Href == directoryHref)
                    {
                        return group;
                    }
                }
                catch (ResourceException)
                {
                    // It looked like an href, but no group was found.
                    // We'll try looking it up by name.
                }
            }

            group = this.AsInterface.GetDirectory()
                .GetGroups()
                .Synchronously()
                .Where(x => x.Name == hrefOrName)
                .FirstOrDefault();

            return group; // or null
        }
    }
}
