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

        public DefaultAccountCreationPolicy(ResourceData data)
            : base(data)
        {
        }

        EmailStatus IAccountCreationPolicy.VerificationEmailStatus
            => this.GetProperty<EmailStatus>(VerificationEmailStatusPropertyName);

        EmailStatus IAccountCreationPolicy.VerificationSuccessEmailStatus
            => this.GetProperty<EmailStatus>(VerificationSuccessEmailStatusPropertyName);

        EmailStatus IAccountCreationPolicy.WelcomeEmailStatus
            => this.GetProperty<EmailStatus>(WelcomeEmailStatusPropertyName);

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

        Task<IAccountCreationPolicy> ISaveable<IAccountCreationPolicy>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IAccountCreationPolicy>(cancellationToken);

        IAccountCreationPolicy ISaveableSync<IAccountCreationPolicy>.Save()
            => this.Save<IAccountCreationPolicy>();
    }
}
