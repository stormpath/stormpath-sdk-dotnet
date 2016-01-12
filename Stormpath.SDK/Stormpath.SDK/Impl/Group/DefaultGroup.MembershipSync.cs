// <copyright file="DefaultGroup.MembershipSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed partial class DefaultGroup
    {
        IGroupMembership IGroupSync.AddAccount(IAccount account)
            => DefaultGroupMembership.Create(account, this, this.GetInternalSyncDataStore());

        IGroupMembership IGroupSync.AddAccount(string hrefOrEmailOrUsername)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
            {
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));
            }

            var account = this.FindAccount(hrefOrEmailOrUsername);
            if (account == null)
            {
                throw new InvalidOperationException($"No matching account for {nameof(hrefOrEmailOrUsername)} '{hrefOrEmailOrUsername}' found.");
            }

            return DefaultGroupMembership.Create(account, this, this.GetInternalSyncDataStore());
        }

        bool IGroupSync.RemoveAccount(IAccount account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            IGroupMembership foundMembership = null;
            foreach (var item in this.AsInterface.GetAccountMemberships().Synchronously())
            {
                if ((item as IInternalGroupMembership).AccountHref.Equals(account.Href, StringComparison.InvariantCultureIgnoreCase))
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
                throw new InvalidOperationException("The specified account does not belong to this group.");
            }

            return foundMembership.Delete();
        }

        bool IGroupSync.RemoveAccount(string hrefOrEmailOrUsername)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
            {
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));
            }

            IGroupMembership foundMembership = null;
            foreach (var item in this.AsInterface.GetAccountMemberships().Synchronously())
            {
                IAccount account = item.GetAccount();
                if (account.Href.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase) ||
                    account.Email.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase) ||
                    account.Username.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase))
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
                throw new InvalidOperationException("The specified account does not belong to this group.");
            }

            return foundMembership.Delete();
        }

        private IAccount FindAccount(string hrefOrEmailOrUsername)
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
                    account = this.GetInternalSyncDataStore().GetResource<IAccount>(hrefOrEmailOrUsername);

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

            var directory = this.AsInterface.GetDirectory();

            account = directory
                    .GetAccounts()
                    .Synchronously()
                    .Where(x => x.Email == hrefOrEmailOrUsername)
                    .FirstOrDefault();

            if (account == null)
            {
                account = directory
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Username == hrefOrEmailOrUsername)
                .FirstOrDefault();
            }

            return account; // or null
        }
    }
}
