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
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultAccount : AbstractExtendableInstanceResource, IAccount, IAccountSync
    {
        private const string AccessTokensPropertyName = "accessTokens";
        private const string ApiKeysPropertyName = "apiKeys";
        private const string ApplicationsPropertyName = "applications";
        private const string DirectoryPropertyName = "directory";
        private const string EmailPropertyName = "email";
        private const string EmailVerificationTokenPropertyName = "emailVerificationToken";
        private const string EmailVerificationStatusPropertyName = "emailVerificationStatus";
        private const string FullNamePropertyName = "fullName";
        private const string FactorsPropertyName = "factors";
        private const string GivenNamePropertyName = "givenName";
        private const string GroupMembershipsPropertyName = "groupMemberships";
        private const string GroupsPropertyName = "groups";
        private const string MiddleNamePropertyName = "middleName";
        public const string PasswordPropertyName = "password";
        private const string PasswordModifiedAtPropertyName = "passwordModifiedAt";
        private const string PhonesPropertyName = "phones";
        private const string ProviderDataPropertyName = "providerData";
        private const string RefreshTokensPropertyName = "refreshTokens";
        private const string StatusPropertyName = "status";
        private const string SurnamePropertyName = "surname";
        private const string UsernamePropertyName = "username";

        public DefaultAccount(ResourceData data)
            : base(data)
        {
        }

        private new IAccount AsInterface => this;

        private IAccountSync AsSyncInterface => this;

        internal IEmbeddedProperty AccessTokens => this.GetLinkProperty(AccessTokensPropertyName);

        internal IEmbeddedProperty ApiKeys => this.GetLinkProperty(ApiKeysPropertyName);

        internal IEmbeddedProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        internal IEmbeddedProperty Directory => this.GetLinkProperty(DirectoryPropertyName);

        string IAccount.Email => this.GetStringProperty(EmailPropertyName);

        internal IEmbeddedProperty EmailVerificationToken => this.GetLinkProperty(EmailVerificationTokenPropertyName);

        EmailVerificationStatus IAccount.EmailVerificationStatus => GetEnumProperty<EmailVerificationStatus>(EmailVerificationStatusPropertyName);

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

        DateTimeOffset? IAccount.PasswordModifiedAt => this.GetDateTimeProperty(PasswordModifiedAtPropertyName);

        internal IEmbeddedProperty ProviderData => this.GetLinkProperty(ProviderDataPropertyName);

        internal IEmbeddedProperty RefreshTokens => this.GetLinkProperty(RefreshTokensPropertyName);

        AccountStatus IAccount.Status => this.GetEnumProperty<AccountStatus>(StatusPropertyName);

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