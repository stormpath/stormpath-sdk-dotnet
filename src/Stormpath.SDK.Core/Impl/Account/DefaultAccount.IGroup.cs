// <copyright file="DefaultAccount.IGroup.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Group;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultAccount
    {
        Task<IGroupMembership> IAccount.AddGroupAsync(IGroup group, CancellationToken cancellationToken)
            => DefaultGroupMembership.CreateAsync(this, group, this.GetInternalAsyncDataStore(), cancellationToken);

        async Task<IGroupMembership> IAccount.AddGroupAsync(string hrefOrName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            var group = await this.FindGroupInDirectoryAsync(hrefOrName, this.Directory.Href, cancellationToken).ConfigureAwait(false);
            if (group == null)
            {
                throw new InvalidOperationException("The specified group was not found in the account's directory.");
            }

            return await DefaultGroupMembership.CreateAsync(this, group, this.GetInternalAsyncDataStore(), cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> IAccount.RemoveGroupAsync(IGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            IGroupMembership foundMembership = null;
            await this.AsInterface.GetGroupMemberships().ForEachAsync(
                item =>
                {
                    if ((item as IInternalGroupMembership).GroupHref.Equals(group.Href, StringComparison.OrdinalIgnoreCase))
                    {
                        foundMembership = item;
                    }

                    return foundMembership != null;
                }, cancellationToken).ConfigureAwait(false);

            if (foundMembership == null)
            {
                throw new InvalidOperationException("This account does not belong to the specified group.");
            }

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> IAccount.RemoveGroupAsync(string hrefOrName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            IGroupMembership foundMembership = null;
            var iterator = this.AsInterface.GetGroupMemberships();
            while (await iterator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var item in iterator.CurrentPage)
                {
                    IGroup group = await item.GetGroupAsync(cancellationToken).ConfigureAwait(false);
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

                if (foundMembership != null)
                {
                    break;
                }
            }

            if (foundMembership == null)
            {
                throw new InvalidOperationException("This account does not belong to the specified group.");
            }

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> IAccount.IsMemberOfGroupAsync(string hrefOrName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
            {
                throw new ArgumentNullException(nameof(hrefOrName));
            }

            IGroup foundGroup = null;
            await this.AsInterface.GetGroups().ForEachAsync(
                item =>
                {
                    if (item.Name.Equals(hrefOrName, StringComparison.OrdinalIgnoreCase) ||
                        item.Href.Equals(hrefOrName, StringComparison.OrdinalIgnoreCase))
                    {
                        foundGroup = item;
                    }

                    return foundGroup != null;
                }, cancellationToken).ConfigureAwait(false);

            return foundGroup != null;
        }

        private async Task<IGroup> FindGroupInDirectoryAsync(string hrefOrName, string directoryHref, CancellationToken cancellationToken)
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
                    group = await this.GetInternalAsyncDataStore().GetResourceAsync<IGroup>(hrefOrName, cancellationToken).ConfigureAwait(false);

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

            group = await (await this.AsInterface.GetDirectoryAsync().ConfigureAwait(false))
                .GetGroups()
                .Where(x => x.Name == hrefOrName)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return group; // or null
        }
    }
}
