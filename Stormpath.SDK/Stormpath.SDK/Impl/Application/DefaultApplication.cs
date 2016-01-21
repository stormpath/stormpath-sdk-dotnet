// <copyright file="DefaultApplication.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication : AbstractExtendableInstanceResource, IApplication, IApplicationSync
    {
        private static readonly string AccountStoreMappingsPropertyName = "accountStoreMappings";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string ApiKeysPropertyName = "apiKeys";
        private static readonly string AuthTokensPropertyName = "authTokens";
        private static readonly string DefaultAccountStoreMappingPropertyName = AccountStoreContainerShared.DefaultAccountStoreMappingPropertyName;
        private static readonly string DefaultGroupStoreMappingPropertyName = AccountStoreContainerShared.DefaultGroupStoreMappingPropertyName;
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string LoginAttemptsPropertyName = "loginAttempts";
        private static readonly string NamePropertyName = "name";
        private static readonly string OAuthPolicyPropertyName = "oAuthPolicy";
        private static readonly string PasswordResetTokensPropertyName = "passwordResetTokens";
        private static readonly string StatusPropertyName = "status";
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

        string IApplication.Description => this.GetStringProperty(DescriptionPropertyName);

        internal IEmbeddedProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IApplication.Name => this.GetStringProperty(NamePropertyName);

        internal IEmbeddedProperty LoginAttempts => this.GetLinkProperty(LoginAttemptsPropertyName);

        internal IEmbeddedProperty OAuthPolicy => this.GetLinkProperty(OAuthPolicyPropertyName);

        internal IEmbeddedProperty PasswordResetToken => this.GetLinkProperty(PasswordResetTokensPropertyName);

        ApplicationStatus IApplication.Status => this.GetProperty<ApplicationStatus>(StatusPropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        internal IEmbeddedProperty VerificationEmails => this.GetLinkProperty(VerificationEmailsPropertyName);

        IApplication IApplication.SetDescription(string description)
        {
            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        IApplication IApplication.SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IApplication IApplication.SetStatus(ApplicationStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

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
    }
}
