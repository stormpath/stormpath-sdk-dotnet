// <copyright file="DefaultAccount.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultAccount : AbstractExtendableInstanceResource, IAccount, IAccountSync
    {
        private static readonly string AccessTokensPropertyName = "accessTokens";
        private static readonly string ApiKeysPropertyName = "apiKeys";
        private static readonly string ApplicationsPropertyName = "applications";
        private static readonly string DirectoryPropertyName = "directory";
        private static readonly string EmailPropertyName = "email";
        private static readonly string EmailVerificationTokenPropertyName = "emailVerificationToken";
        private static readonly string FullNamePropertyName = "fullName";
        private static readonly string GivenNamePropertyName = "givenName";
        private static readonly string GroupMembershipsPropertyName = "groupMemberships";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string MiddleNamePropertyName = "middleName";
        private static readonly string PasswordPropertyName = "password";
        private static readonly string ProviderDataPropertyName = "providerData";
        private static readonly string RefreshTokensPropertyName = "refreshTokens";
        private static readonly string StatusPropertyName = "status";
        private static readonly string SurnamePropertyName = "surname";
        private static readonly string TenantPropertyName = "tenant";
        private static readonly string UsernamePropertyName = "username";

        public DefaultAccount(ResourceData data)
            : base(data)
        {
        }

        private IAccount AsInterface => this;

        internal LinkProperty AccessTokens => this.GetLinkProperty(AccessTokensPropertyName);

        internal LinkProperty ApiKeys => this.GetLinkProperty(ApiKeysPropertyName);

        internal LinkProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        internal LinkProperty Directory => this.GetLinkProperty(DirectoryPropertyName);

        string IAccount.Email => this.GetProperty<string>(EmailPropertyName);

        internal LinkProperty EmailVerificationToken => this.GetLinkProperty(EmailVerificationTokenPropertyName);

        IEmailVerificationToken IAccount.EmailVerificationToken
        {
            get
            {
                var emailVerificationToken = this.GetInternalDataStore()
                    .InstantiateWithHref<IEmailVerificationToken>(this.EmailVerificationToken.Href);
                return emailVerificationToken;
            }
        }

        string IAccount.FullName => this.GetProperty<string>(FullNamePropertyName);

        string IAccount.GivenName => this.GetProperty<string>(GivenNamePropertyName);

        internal LinkProperty GroupMemberships => this.GetLinkProperty(GroupMembershipsPropertyName);

        internal LinkProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IAccount.MiddleName => this.GetProperty<string>(MiddleNamePropertyName);

        internal LinkProperty ProviderData => this.GetLinkProperty(ProviderDataPropertyName);

        internal LinkProperty RefreshTokens => this.GetLinkProperty(RefreshTokensPropertyName);

        AccountStatus IAccount.Status => this.GetProperty<AccountStatus>(StatusPropertyName);

        string IAccount.Surname => this.GetProperty<string>(SurnamePropertyName);

        internal LinkProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        string IAccount.Username => this.GetProperty<string>(UsernamePropertyName);

        IAccount IAccount.SetEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            this.SetProperty(EmailPropertyName, email);
            return this;
        }

        IAccount IAccount.SetGivenName(string givenName)
        {
            if (string.IsNullOrEmpty(givenName))
                throw new ArgumentNullException(nameof(givenName));

            this.SetProperty(GivenNamePropertyName, givenName);
            return this;
        }

        IAccount IAccount.SetMiddleName(string middleName)
        {
            this.SetProperty(MiddleNamePropertyName, middleName);
            return this;
        }

        IAccount IAccount.SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            this.SetProperty(PasswordPropertyName, password);
            return this;
        }

        IAccount IAccount.SetStatus(AccountStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        IAccount IAccount.SetSurname(string surname)
        {
            if (string.IsNullOrEmpty(surname))
                throw new ArgumentNullException(nameof(surname));

            this.SetProperty(SurnamePropertyName, surname);
            return this;
        }

        IAccount IAccount.SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            this.SetProperty(UsernamePropertyName, username);
            return this;
        }

        IAsyncQueryable<IGroup> IAccount.GetGroups()
            => new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalDataStore());

        IAsyncQueryable<IGroupMembership> IAccount.GetGroupMemberships()
            => new CollectionResourceQueryable<IGroupMembership>(this.GroupMemberships.Href, this.GetInternalDataStore());

        Task<IDirectory> IAccount.GetDirectoryAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IDirectory>(this.Directory.Href, cancellationToken);

        IDirectory IAccountSync.GetDirectory()
            => this.GetInternalSyncDataStore().GetResource<IDirectory>(this.Directory.Href);

        Task<ITenant> IAccount.GetTenantAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<ITenant>(this.Tenant.Href, cancellationToken);

        ITenant IAccountSync.GetTenant()
            => this.GetInternalSyncDataStore().GetResource<ITenant>(this.Tenant.Href);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
             => this.GetInternalSyncDataStore().Delete(this);

        Task<IAccount> ISaveable<IAccount>.SaveAsync(CancellationToken cancellationToken)
             => this.SaveAsync<IAccount>(cancellationToken);

        IAccount ISaveableSync<IAccount>.Save()
             => this.Save<IAccount>();

        Task<IGroupMembership> IAccount.AddGroupAsync(IGroup group, CancellationToken cancellationToken)
            => DefaultGroupMembership.CreateAsync(this, group, this.GetInternalDataStore(), cancellationToken);

        IGroupMembership IAccountSync.AddGroup(IGroup group)
            => DefaultGroupMembership.Create(this, group, this.GetInternalDataStoreSync());

        async Task<IGroupMembership> IAccount.AddGroupAsync(string hrefOrName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));

            var group = await this.FindGroupInDirectoryAsync(hrefOrName, this.Directory.Href, cancellationToken).ConfigureAwait(false);
            if (group == null)
                throw new InvalidOperationException("The specified group was not found in the account's directory.");

            return await DefaultGroupMembership.CreateAsync(this, group, this.GetInternalDataStore(), cancellationToken).ConfigureAwait(false);
        }

        IGroupMembership IAccountSync.AddGroup(string hrefOrName)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));

            var group = this.FindGroupInDirectory(hrefOrName, this.Directory.Href);
            if (group == null)
                throw new InvalidOperationException("The specified group was not found in the account's directory.");

            return DefaultGroupMembership.Create(this, group, this.GetInternalDataStoreSync());
        }

        async Task<bool> IAccount.RemoveGroupAsync(IGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            IGroupMembership foundMembership = null;
            await this.AsInterface.GetGroupMemberships().ForEachAsync(
                item =>
                {
                    if ((item as IInternalGroupMembership).GroupHref.Equals(group.Href, StringComparison.InvariantCultureIgnoreCase))
                        foundMembership = item;

                    return foundMembership != null;
                }, cancellationToken).ConfigureAwait(false);

            if (foundMembership == null)
                throw new InvalidOperationException("This account does not belong to the specified group.");

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        bool IAccountSync.RemoveGroup(IGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            IGroupMembership foundMembership = null;

            foreach (var item in this.AsInterface.GetGroupMemberships().Synchronously())
            {
                if ((item as IInternalGroupMembership).GroupHref.Equals(group.Href, StringComparison.InvariantCultureIgnoreCase))
                    foundMembership = item;

                if (foundMembership != null)
                    break;
            }

            if (foundMembership == null)
                throw new InvalidOperationException("This account does not belong to the specified group.");

            return foundMembership.Delete();
        }

        async Task<bool> IAccount.RemoveGroupAsync(string hrefOrName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));

            IGroupMembership foundMembership = null;
            var iterator = this.AsInterface.GetGroupMemberships();
            while (await iterator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var item in iterator.CurrentPage)
                {
                    IGroup group = await item.GetGroupAsync(cancellationToken).ConfigureAwait(false);
                    if (group.Href.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase) ||
                        group.Name.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase))
                        foundMembership = item;

                    if (foundMembership != null)
                        break;
                }

                if (foundMembership != null)
                    break;
            }

            if (foundMembership == null)
                throw new InvalidOperationException("This account does not belong to the specified group.");

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        bool IAccountSync.RemoveGroup(string hrefOrName)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));

            IGroupMembership foundMembership = null;
            foreach (var item in this.AsInterface.GetGroupMemberships().Synchronously())
            {
                IGroup group = item.GetGroup();
                if (group.Href.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase) ||
                    group.Name.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase))
                    foundMembership = item;

                if (foundMembership != null)
                    break;
            }

            if (foundMembership == null)
                throw new InvalidOperationException("This account does not belong to the specified group.");

            return foundMembership.Delete();
        }

        async Task<bool> IAccount.IsMemberOfGroupAsync(string hrefOrName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));

            IGroup foundGroup = null;
            await this.AsInterface.GetGroups().ForEachAsync(
                item =>
                {
                    if (item.Name.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase) ||
                        item.Href.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase))
                        foundGroup = item;

                    return foundGroup != null;
                }, cancellationToken).ConfigureAwait(false);

            return foundGroup != null;
        }

        bool IAccountSync.IsMemberOfGroup(string hrefOrName)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));

            IGroup foundGroup = null;
            foreach (var item in this.AsInterface.GetGroups().Synchronously())
            {
                if (item.Name.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase) ||
                        item.Href.Equals(hrefOrName, StringComparison.InvariantCultureIgnoreCase))
                    foundGroup = item;

                if (foundGroup != null)
                    break;
            }

            return foundGroup != null;
        }

        Task<IProviderData> IAccount.GetProviderDataAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().GetResourceAsync<IProviderData>(this.ProviderData.Href, ProviderTypeConverter.DataTypeLookup, cancellationToken);

        IProviderData IAccountSync.GetProviderData()
            => this.GetInternalDataStoreSync().GetResource<IProviderData>(this.ProviderData.Href, ProviderTypeConverter.DataTypeLookup);

        private async Task<IGroup> FindGroupInDirectoryAsync(string hrefOrName, string directoryHref, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));
            if (string.IsNullOrEmpty(directoryHref))
                throw new ArgumentNullException(nameof(directoryHref));

            IGroup group = null;

            bool looksLikeHref = hrefOrName.Split('/').Length > 4;
            if (looksLikeHref)
            {
                try
                {
                    group = await this.GetInternalDataStore().GetResourceAsync<IGroup>(hrefOrName, cancellationToken).ConfigureAwait(false);

                    if ((group as DefaultGroup)?.Directory.Href == directoryHref)
                        return group;
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

        private IGroup FindGroupInDirectory(string hrefOrName, string directoryHref)
        {
            if (string.IsNullOrEmpty(hrefOrName))
                throw new ArgumentNullException(nameof(hrefOrName));
            if (string.IsNullOrEmpty(directoryHref))
                throw new ArgumentNullException(nameof(directoryHref));

            IGroup group = null;

            bool looksLikeHref = hrefOrName.Split('/').Length > 4;
            if (looksLikeHref)
            {
                try
                {
                    group = this.GetInternalDataStoreSync().GetResource<IGroup>(hrefOrName);

                    if ((group as DefaultGroup)?.Directory.Href == directoryHref)
                        return group;
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