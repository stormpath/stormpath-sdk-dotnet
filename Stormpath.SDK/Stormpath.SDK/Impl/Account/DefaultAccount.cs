// <copyright file="DefaultAccount.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultAccount : AbstractExtendableInstanceResource, IAccount, IAccountSync
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
        public static readonly string PasswordPropertyName = "password";
        private static readonly string ProviderDataPropertyName = "providerData";
        private static readonly string RefreshTokensPropertyName = "refreshTokens";
        private static readonly string StatusPropertyName = "status";
        private static readonly string SurnamePropertyName = "surname";
        private static readonly string UsernamePropertyName = "username";

        public DefaultAccount(ResourceData data)
            : base(data)
        {
        }

        private new IAccount AsInterface => this;

        internal IEmbeddedProperty AccessTokens => this.GetLinkProperty(AccessTokensPropertyName);

        internal IEmbeddedProperty ApiKeys => this.GetLinkProperty(ApiKeysPropertyName);

        internal IEmbeddedProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        internal IEmbeddedProperty Directory => this.GetLinkProperty(DirectoryPropertyName);

        string IAccount.Email => this.GetStringProperty(EmailPropertyName);

        internal IEmbeddedProperty EmailVerificationToken => this.GetLinkProperty(EmailVerificationTokenPropertyName);

        IEmailVerificationToken IAccount.EmailVerificationToken
        {
            get
            {
                if (string.IsNullOrEmpty(this.EmailVerificationToken.Href))
                {
                    return null;
                }

                var emailVerificationToken = this.GetInternalAsyncDataStore()
                    .InstantiateWithHref<IEmailVerificationToken>(this.EmailVerificationToken.Href);
                return emailVerificationToken;
            }
        }

        string IAccount.FullName => this.GetStringProperty(FullNamePropertyName);

        string IAccount.GivenName => this.GetStringProperty(GivenNamePropertyName);

        internal IEmbeddedProperty GroupMemberships => this.GetLinkProperty(GroupMembershipsPropertyName);

        internal IEmbeddedProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IAccount.MiddleName => this.GetStringProperty(MiddleNamePropertyName);

        internal IEmbeddedProperty ProviderData => this.GetLinkProperty(ProviderDataPropertyName);

        internal IEmbeddedProperty RefreshTokens => this.GetLinkProperty(RefreshTokensPropertyName);

        AccountStatus IAccount.Status => this.GetProperty<AccountStatus>(StatusPropertyName);

        string IAccount.Surname => this.GetStringProperty(SurnamePropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        string IAccount.Username => this.GetStringProperty(UsernamePropertyName);

        IAccount IAccount.SetEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            this.SetProperty(EmailPropertyName, email);
            return this;
        }

        IAccount IAccount.SetGivenName(string givenName)
        {
            if (string.IsNullOrEmpty(givenName))
            {
                throw new ArgumentNullException(nameof(givenName));
            }

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
            {
                throw new ArgumentNullException(nameof(password));
            }

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
            {
                throw new ArgumentNullException(nameof(surname));
            }

            this.SetProperty(SurnamePropertyName, surname);
            return this;
        }

        IAccount IAccount.SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            this.SetProperty(UsernamePropertyName, username);
            return this;
        }

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
             => this.GetInternalSyncDataStore().Delete(this);

        Task<IAccount> ISaveable<IAccount>.SaveAsync(CancellationToken cancellationToken)
             => this.SaveAsync<IAccount>(cancellationToken);

        IAccount ISaveableSync<IAccount>.Save()
             => this.Save<IAccount>();

        Task<IAccount> ISaveableWithOptions<IAccount>.SaveAsync(Action<IRetrievalOptions<IAccount>> options, CancellationToken cancellationToken)
             => this.SaveAsync(options, cancellationToken);

        IAccount ISaveableWithOptionsSync<IAccount>.Save(Action<IRetrievalOptions<IAccount>> options)
             => this.Save(options);
    }
}