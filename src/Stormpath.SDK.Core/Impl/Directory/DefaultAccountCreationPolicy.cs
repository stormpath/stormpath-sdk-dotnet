// <copyright file="DefaultAccountCreationPolicy.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Mail;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed class DefaultAccountCreationPolicy : 
        AbstractInstanceResource,
        IAccountCreationPolicy,
        IAccountCreationPolicySync
    {
        private static readonly string VerificationEmailStatusPropertyName = "verificationEmailStatus";
        private static readonly string VerificationSuccessEmailStatusPropertyName = "verificationSuccessEmailStatus";
        private static readonly string WelcomeEmailStatusPropertyName = "welcomeEmailStatus";
        private const string EmailDomainBlacklistPropertyName = "emailDomainBlacklist";
        private const string EmailDomainWhitelistPropertyName = "emailDomainWhitelist";

        public DefaultAccountCreationPolicy(ResourceData data)
            : base(data)
        {
        }

        EmailStatus IAccountCreationPolicy.VerificationEmailStatus
            => GetEnumProperty<EmailStatus>(VerificationEmailStatusPropertyName);

        EmailStatus IAccountCreationPolicy.VerificationSuccessEmailStatus
            => GetEnumProperty<EmailStatus>(VerificationSuccessEmailStatusPropertyName);

        EmailStatus IAccountCreationPolicy.WelcomeEmailStatus
            => GetEnumProperty<EmailStatus>(WelcomeEmailStatusPropertyName);

        IReadOnlyList<string> IAccountCreationPolicy.EmailDomainBlacklist
            => GetListProperty<string>(EmailDomainBlacklistPropertyName);

        IReadOnlyList<string> IAccountCreationPolicy.EmailDomainWhitelist
            => GetListProperty<string>(EmailDomainWhitelistPropertyName);

        IAccountCreationPolicy IAccountCreationPolicy.SetVerificationEmailStatus(EmailStatus verificationEmailStatus)
        {
            this.SetProperty(VerificationEmailStatusPropertyName, verificationEmailStatus);
            return this;
        }

        IAccountCreationPolicy IAccountCreationPolicy.SetVerificationSuccessEmailStatus(EmailStatus verificationSuccessEmailStatus)
        {
            this.SetProperty(VerificationSuccessEmailStatusPropertyName, verificationSuccessEmailStatus);
            return this;
        }

        IAccountCreationPolicy IAccountCreationPolicy.SetWelcomeEmailStatus(EmailStatus welcomeEmailStatus)
        {
            this.SetProperty(WelcomeEmailStatusPropertyName, welcomeEmailStatus);
            return this;
        }

        IAccountCreationPolicy IAccountCreationPolicy.SetEmailDomainBlacklist(IEnumerable<string> blacklistDomains)
        {
            SetListProperty(EmailDomainBlacklistPropertyName, blacklistDomains);
            return this;
        }

        IAccountCreationPolicy IAccountCreationPolicy.SetEmailDomainWhitelist(IEnumerable<string> whitelistDomains)
        {
            SetListProperty(EmailDomainWhitelistPropertyName, whitelistDomains);
            return this;
        }

        Task<IAccountCreationPolicy> ISaveable<IAccountCreationPolicy>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IAccountCreationPolicy>(cancellationToken);

        IAccountCreationPolicy ISaveableSync<IAccountCreationPolicy>.Save()
            => this.Save<IAccountCreationPolicy>();
    }
}
