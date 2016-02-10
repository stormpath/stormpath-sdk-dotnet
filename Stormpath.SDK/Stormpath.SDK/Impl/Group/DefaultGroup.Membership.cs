// <copyright file="DefaultGroup.Membership.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed partial class DefaultGroup
    {
        Task<IGroupMembership> IGroup.AddAccountAsync(IAccount account, CancellationToken cancellationToken)
            => DefaultGroupMembership.CreateAsync(account, this, this.GetInternalAsyncDataStore(), cancellationToken);

        async Task<IGroupMembership> IGroup.AddAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
            {
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));
            }

            var account = await this.FindAccountAsync(hrefOrEmailOrUsername, cancellationToken).ConfigureAwait(false);
            if (account == null)
            {
                throw new InvalidOperationException($"No matching account for {nameof(hrefOrEmailOrUsername)} '{hrefOrEmailOrUsername}' found.");
            }

            return await DefaultGroupMembership.CreateAsync(account, this, this.GetInternalAsyncDataStore(), cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> IGroup.RemoveAccountAsync(IAccount account, CancellationToken cancellationToken)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            IGroupMembership foundMembership = null;
            await this.AsInterface.GetAccountMemberships().ForEachAsync(
                item =>
                {
                    if ((item as IInternalGroupMembership).AccountHref.Equals(account.Href, StringComparison.OrdinalIgnoreCase))
                    {
                        foundMembership = item;
                    }

                    return foundMembership != null;
                }, cancellationToken).ConfigureAwait(false);

            if (foundMembership == null)
            {
                throw new InvalidOperationException("The specified account does not belong to this group.");
            }

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> IGroup.RemoveAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
            {
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));
            }

            IGroupMembership foundMembership = null;
            var iterator = this.AsInterface.GetAccountMemberships();
            while (await iterator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var item in iterator.CurrentPage)
                {
                    IAccount account = await item.GetAccountAsync(cancellationToken).ConfigureAwait(false);
                    if (account.Href.Equals(hrefOrEmailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                        account.Email.Equals(hrefOrEmailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                        account.Username.Equals(hrefOrEmailOrUsername, StringComparison.OrdinalIgnoreCase))
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
                throw new InvalidOperationException("The specified account does not belong to this group.");
            }

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task<IAccount> FindAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
            {
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));
            }

            IAccount account = null;

            bool looksLikeHref = hrefOrEmailOrUsername.Split('/').Length > 4;
            if (looksLikeHref)
            {
                try
                {
                    account = await this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(hrefOrEmailOrUsername).ConfigureAwait(false);

                    if ((account as DefaultAccount)?.Directory.Href == this.Directory.Href)
                    {
                        return account;
                    }
                }
                catch (ResourceException)
                {
                    // It looked like an href, but no group was found.
                    // We'll try looking it up by name.
                }
            }

            var directory = await this.AsInterface
                .GetDirectoryAsync(cancellationToken)
                .ConfigureAwait(false);

            account = await directory
                    .GetAccounts()
                    .Where(x => x.Email == hrefOrEmailOrUsername)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

            if (account == null)
            {
                account = await directory
                .GetAccounts()
                .Where(x => x.Username == hrefOrEmailOrUsername)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            }

            return account; // or null
        }
    }
}
