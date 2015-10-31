// <copyright file="DefaultApplication.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication : AbstractExtendableInstanceResource, IApplication, IApplicationSync
    {
        private static readonly string AccountStoreMappingsPropertyName = "accountStoreMappings";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string ApiKeysPropertyName = "apiKeys";
        private static readonly string AuthTokensPropertyName = "authTokens";
        private static readonly string DefaultAccountStoreMappingPropertyName = "defaultAccountStoreMapping";
        private static readonly string DefaultGroupStoreMappingPropertyName = "defaultGroupStoreMapping";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string LoginAttemptsPropertyName = "loginAttempts";
        private static readonly string NamePropertyName = "name";
        private static readonly string OAuthPolicyPropertyName = "oAuthPolicy";
        private static readonly string PasswordResetTokensPropertyName = "passwordResetTokens";
        private static readonly string StatusPropertyName = "status";
        private static readonly string TenantPropertyName = "tenant";
        private static readonly string VerificationEmailsPropertyName = "verificationEmails";

        public DefaultApplication(ResourceData data)
            : base(data)
        {
        }

        private new IApplication AsInterface => this;

        private IApplicationSync AsSyncInterface => this;

        internal IEmbeddedProperty AccountStoreMappings => this.GetLinkProperty(AccountStoreMappingsPropertyName);

        internal IEmbeddedProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal IEmbeddedProperty ApiKeys => this.GetLinkProperty(ApiKeysPropertyName);

        internal IEmbeddedProperty AuthTokens => this.GetLinkProperty(AuthTokensPropertyName);

        internal IEmbeddedProperty DefaultAccountStoreMapping => this.GetLinkProperty(DefaultAccountStoreMappingPropertyName);

        internal IEmbeddedProperty DefaultGroupStoreMapping => this.GetLinkProperty(DefaultGroupStoreMappingPropertyName);

        string IApplication.Description => this.GetProperty<string>(DescriptionPropertyName);

        internal IEmbeddedProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IApplication.Name => this.GetProperty<string>(NamePropertyName);

        internal IEmbeddedProperty LoginAttempts => this.GetLinkProperty(LoginAttemptsPropertyName);

        internal IEmbeddedProperty OAuthPolicy => this.GetLinkProperty(OAuthPolicyPropertyName);

        internal IEmbeddedProperty PasswordResetToken => this.GetLinkProperty(PasswordResetTokensPropertyName);

        ApplicationStatus IApplication.Status => this.GetProperty<ApplicationStatus>(StatusPropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        internal IEmbeddedProperty VerificationEmails => this.GetLinkProperty(VerificationEmailsPropertyName);

        IApplication IApplication.SetDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        IApplication IApplication.SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IApplication IApplication.SetStatus(ApplicationStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        Task<IAuthenticationResult> IApplication.AuthenticateAccountAsync(IAuthenticationRequest request, CancellationToken cancellationToken)
        {
            var dispatcher = new AuthenticationRequestDispatcher();

            return dispatcher.AuthenticateAsync(this.GetInternalAsyncDataStore(), this, request, null, cancellationToken);
        }

        IAuthenticationResult IApplicationSync.AuthenticateAccount(IAuthenticationRequest request)
        {
            var dispatcher = new AuthenticationRequestDispatcher();

            return dispatcher.Authenticate(this.GetInternalSyncDataStore(), this, request, null);
        }

        Task<IAuthenticationResult> IApplication.AuthenticateAccountAsync(IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions, CancellationToken cancellationToken)
        {
            var options = new DefaultRetrievalOptions<IAuthenticationResult>();
            responseOptions(options);

            var dispatcher = new AuthenticationRequestDispatcher();

            return dispatcher.AuthenticateAsync(this.GetInternalAsyncDataStore(), this, request, options, cancellationToken);
        }

        IAuthenticationResult IApplicationSync.AuthenticateAccount(IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions)
        {
            var options = new DefaultRetrievalOptions<IAuthenticationResult>();
            responseOptions(options);

            var dispatcher = new AuthenticationRequestDispatcher();

            return dispatcher.Authenticate(this.GetInternalSyncDataStore(), this, request, options);
        }

        Task<IAuthenticationResult> IApplication.AuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken)
        {
            var request = new UsernamePasswordRequest(username, password) as IAuthenticationRequest;

            return this.AsInterface.AuthenticateAccountAsync(request, cancellationToken);
        }

        IAuthenticationResult IApplicationSync.AuthenticateAccount(string username, string password)
        {
            var request = new UsernamePasswordRequest(username, password) as IAuthenticationRequest;

            return this.AuthenticateAccount(request);
        }

        async Task<bool> IApplication.TryAuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken)
        {
            try
            {
                var loginResult = await this.AsInterface.AuthenticateAccountAsync(username, password, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IApplicationSync.TryAuthenticateAccount(string username, string password)
        {
            try
            {
                var loginResult = this.AuthenticateAccount(username, password);
                return true;
            }
            catch
            {
                return false;
            }
        }

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, creationOptionsAction, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction)
            => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, account, creationOptionsAction);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken)
             => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, creationOptions, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(IAccount account, IAccountCreationOptions creationOptions)
             => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, account, creationOptions);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, CancellationToken cancellationToken)
             => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(IAccount account)
             => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, account);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(string givenName, string surname, string email, string password, object customData, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, givenName, surname, email, password, customData, cancellationToken);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(string givenName, string surname, string email, string password, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, givenName, surname, email, password, null, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(string givenName, string surname, string email, string password, object customData)
            => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, givenName, surname, email, password, customData);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
             => this.GetInternalSyncDataStore().Delete(this);

        Task<IApplication> ISaveable<IApplication>.SaveAsync(CancellationToken cancellationToken)
             => this.SaveAsync<IApplication>(cancellationToken);

        IApplication ISaveableSync<IApplication>.Save()
            => this.Save<IApplication>();

        Task<IApplication> ISaveableWithOptions<IApplication>.SaveAsync(Action<IRetrievalOptions<IApplication>> options, CancellationToken cancellationToken)
             => this.SaveAsync(options, cancellationToken);

        IApplication ISaveableWithOptionsSync<IApplication>.Save(Action<IRetrievalOptions<IApplication>> options)
             => this.Save(options);

        Task<IPasswordResetToken> IApplication.SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken)
        {
            var token = this.GetInternalAsyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);

            return this.GetInternalAsyncDataStore().CreateAsync(this.PasswordResetToken.Href, (IPasswordResetToken)token, cancellationToken);
        }

        IPasswordResetToken IApplicationSync.SendPasswordResetEmail(string email)
        {
            var token = this.GetInternalAsyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);

            return this.GetInternalSyncDataStore().Create(this.PasswordResetToken.Href, (IPasswordResetToken)token);
        }

        async Task<IAccount> IApplication.VerifyPasswordResetTokenAsync(string token, CancellationToken cancellationToken)
        {
            string href = $"{this.PasswordResetToken.Href}/{token}";

            var validTokenResponse = await this.GetInternalAsyncDataStore().GetResourceAsync<IPasswordResetToken>(href, cancellationToken).ConfigureAwait(false);
            return await validTokenResponse.GetAccountAsync(cancellationToken).ConfigureAwait(false);
        }

        IAccount IApplicationSync.VerifyPasswordResetToken(string token)
        {
            string href = $"{this.PasswordResetToken.Href}/{token}";

            var validTokenResponse = this.GetInternalAsyncDataStore().GetResource<IPasswordResetToken>(href);
            return validTokenResponse.GetAccount();
        }

        async Task<IAccount> IApplication.ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            var href = $"{this.PasswordResetToken.Href}/{token}";
            var passwordResetToken = this.GetInternalAsyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            passwordResetToken.SetPassword(newPassword);

            var responseToken = await this.GetInternalAsyncDataStore().CreateAsync(href, (IPasswordResetToken)passwordResetToken, cancellationToken).ConfigureAwait(false);
            return await responseToken.GetAccountAsync(cancellationToken).ConfigureAwait(false);
        }

        IAccount IApplicationSync.ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            var href = $"{this.PasswordResetToken.Href}/{token}";
            var passwordResetToken = this.GetInternalAsyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            passwordResetToken.SetPassword(newPassword);

            var responseToken = this.GetInternalSyncDataStore().Create(href, (IPasswordResetToken)passwordResetToken);
            return responseToken.GetAccount();
        }

        Task IApplication.SendVerificationEmailAsync(string usernameOrEmail, CancellationToken cancellationToken)
            => this.AsInterface.SendVerificationEmailAsync(request => request.Login = usernameOrEmail, cancellationToken);

        Task IApplication.SendVerificationEmailAsync(Action<EmailVerificationRequestBuilder> requestBuilderAction, CancellationToken cancellationToken)
        {
            var builder = new EmailVerificationRequestBuilder(this.GetInternalAsyncDataStore());
            requestBuilderAction(builder);

            if (string.IsNullOrEmpty(builder.Login))
                throw new ArgumentNullException(nameof(builder.Login));

            var href = $"{(this as IResource).Href}/verificationEmails";

            return this.GetInternalAsyncDataStore().CreateAsync(href, builder.Build(), cancellationToken);
        }

        void IApplicationSync.SendVerificationEmail(string usernameOrEmail)
            => this.SendVerificationEmail(request => request.Login = usernameOrEmail);

        void IApplicationSync.SendVerificationEmail(Action<EmailVerificationRequestBuilder> requestBuilderAction)
        {
            var builder = new EmailVerificationRequestBuilder(this.GetInternalAsyncDataStore());
            requestBuilderAction(builder);

            if (string.IsNullOrEmpty(builder.Login))
                throw new ArgumentNullException(nameof(builder.Login));

            var href = $"{(this as IResource).Href}/verificationEmails";

            this.GetInternalSyncDataStore().Create(href, builder.Build());
        }

        Task<IGroup> IGroupCreationActions.CreateGroupAsync(IGroup group, CancellationToken cancellationToken)
            => GroupCreationActionsShared.CreateGroupAsync(this.GetInternalAsyncDataStore(), this.Groups.Href, group, cancellationToken);

        IGroup IGroupCreationActionsSync.CreateGroup(IGroup group)
            => GroupCreationActionsShared.CreateGroup(this.GetInternalSyncDataStore(), this.Groups.Href, group);

        Task<IGroup> IGroupCreationActions.CreateGroupAsync(IGroup group, Action<GroupCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
            => GroupCreationActionsShared.CreateGroupAsync(this.GetInternalAsyncDataStore(), this.Groups.Href, group, creationOptionsAction, cancellationToken);

        IGroup IGroupCreationActionsSync.CreateGroup(IGroup group, Action<GroupCreationOptionsBuilder> creationOptionsAction)
            => GroupCreationActionsShared.CreateGroup(this.GetInternalSyncDataStore(), this.Groups.Href, group, creationOptionsAction);

        Task<IGroup> IGroupCreationActions.CreateGroupAsync(IGroup group, IGroupCreationOptions creationOptions, CancellationToken cancellationToken)
            => GroupCreationActionsShared.CreateGroupAsync(this.GetInternalAsyncDataStore(), this.Groups.Href, group, creationOptions, cancellationToken);

        IGroup IGroupCreationActionsSync.CreateGroup(IGroup group, IGroupCreationOptions creationOptions)
            => GroupCreationActionsShared.CreateGroup(this.GetInternalSyncDataStore(), this.Groups.Href, group, creationOptions);

        Task<IProviderAccountResult> IApplication.GetAccountAsync(IProviderAccountRequest request, CancellationToken cancellationToken)
            => new ProviderAccountResolver(this.GetInternalAsyncDataStore()).ResolveProviderAccountAsync(this.AsInterface.Href, request, cancellationToken);

        IProviderAccountResult IApplicationSync.GetAccount(IProviderAccountRequest request)
            => new ProviderAccountResolver(this.GetInternalAsyncDataStore()).ResolveProviderAccount(this.AsInterface.Href, request);

        Task<ITenant> IApplication.GetTenantAsync(CancellationToken cancellationToken)
            => this.GetTenantAsync(this.Tenant.Href, cancellationToken);

        ITenant IApplicationSync.GetTenant()
            => this.GetTenant(this.Tenant.Href);

        IAsyncQueryable<IAccount> IApplication.GetAccounts()
             => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IGroup> IApplication.GetGroups()
            => new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalAsyncDataStore());

        Task<IAccount> IApplication.ResetPasswordAsync(string token, string newPassword, IAccountStore accountStore, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();//todo
        }

        Task<IAccount> IApplication.ResetPasswordAsync(string token, string newPassword, string hrefOrNameKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();//todo
        }

        Task<IPasswordResetToken> IApplication.SendPasswordResetEmailAsync(string email, IAccountStore accountStore, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();//todo
        }

        Task<IPasswordResetToken> IApplication.SendPasswordResetEmailAsync(string email, string hrefOrNameKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();//todo
        }

        IAccount IApplicationSync.ResetPassword(string token, string newPassword, IAccountStore accountStore)
        {
            throw new NotImplementedException();//todo
        }

        IAccount IApplicationSync.ResetPassword(string token, string newPassword, string hrefOrNameKey)
        {
            throw new NotImplementedException();//todo
        }

        IPasswordResetToken IApplicationSync.SendPasswordResetEmail(string email, IAccountStore accountStore)
        {
            throw new NotImplementedException();//todo
        }

        IPasswordResetToken IApplicationSync.SendPasswordResetEmail(string email, string hrefOrNameKey)
        {
            throw new NotImplementedException();//todo
        }
    }
}
