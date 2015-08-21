// <copyright file="DefaultApplication.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplication : AbstractExtendableInstanceResource, IApplication
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

        public DefaultApplication(IDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultApplication(IDataStore dataStore, Hashtable properties)
            : base(dataStore, properties)
        {
        }

        internal LinkProperty AccountStoreMappings => GetLinkProperty(AccountStoreMappingsPropertyName);

        internal LinkProperty Accounts => GetLinkProperty(AccountsPropertyName);

        internal LinkProperty ApiKeys => GetLinkProperty(ApiKeysPropertyName);

        internal LinkProperty AuthTokens => GetLinkProperty(AuthTokensPropertyName);

        internal LinkProperty DefaultAccountStoreMapping => GetLinkProperty(DefaultAccountStoreMappingPropertyName);

        internal LinkProperty DefaultGroupStoreMapping => GetLinkProperty(DefaultGroupStoreMappingPropertyName);

        string IApplication.Description => GetProperty<string>(DescriptionPropertyName);

        internal LinkProperty Groups => GetLinkProperty(GroupsPropertyName);

        string IApplication.Name => GetProperty<string>(NamePropertyName);

        internal LinkProperty LoginAttempts => GetLinkProperty(LoginAttemptsPropertyName);

        internal LinkProperty OAuthPolicy => GetLinkProperty(OAuthPolicyPropertyName);

        internal LinkProperty PasswordResetToken => GetLinkProperty(PasswordResetTokensPropertyName);

        ApplicationStatus IApplication.Status => GetProperty<ApplicationStatus>(StatusPropertyName);

        internal LinkProperty Tenant => GetLinkProperty(TenantPropertyName);

        internal LinkProperty VerificationEmails => GetLinkProperty(VerificationEmailsPropertyName);
    }
}
