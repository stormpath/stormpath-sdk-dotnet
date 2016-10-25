// <copyright file="DefaultPasswordPolicy.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Mail;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed class DefaultPasswordPolicy :
        AbstractInstanceResource,
        IPasswordPolicy,
        IPasswordPolicySync
    {
        private static readonly string ResetTokenTtlPropertyName = "resetTokenTtl";
        private static readonly string ResetEmailStatusPropertyName = "resetEmailStatus";
        private static readonly string StrengthPropertyName = "strength";
        private static readonly string ResetEmailTemplatesPropertyName = "resetEmailTemplates";
        private static readonly string ResetSuccessEmailStatusPropertyName = "resetSuccessEmailStatus";
        private static readonly string ResetSuccessEmailTemplatesPropertyName = "resetSuccessEmailTemplates";

        public DefaultPasswordPolicy(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty StrengthPolicy => this.GetLinkProperty(StrengthPropertyName);

        int IPasswordPolicy.ResetTokenTtl
            => this.GetIntProperty(ResetTokenTtlPropertyName);

        EmailStatus IPasswordPolicy.ResetEmailStatus
            => GetEnumProperty<EmailStatus>(ResetEmailStatusPropertyName);

        EmailStatus IPasswordPolicy.ResetSuccessEmailStatus
            => GetEnumProperty<EmailStatus>(ResetSuccessEmailStatusPropertyName);

        IPasswordPolicy IPasswordPolicy.SetResetTokenTtl(int hoursToLive)
        {
            if (hoursToLive < 0 || hoursToLive > 169)
            {
                throw new ArgumentOutOfRangeException(nameof(hoursToLive), "Value must be between 0 and 169.");
            }

            this.SetProperty(ResetTokenTtlPropertyName, hoursToLive);
            return this;
        }

        IPasswordPolicy IPasswordPolicy.SetResetEmailStatus(EmailStatus resetEmailStatus)
        {
            this.SetProperty(ResetEmailStatusPropertyName, resetEmailStatus);
            return this;
        }

        IPasswordPolicy IPasswordPolicy.SetResetEmailSuccessStatus(EmailStatus resetSuccessEmailStatus)
        {
            this.SetProperty(ResetSuccessEmailStatusPropertyName, resetSuccessEmailStatus);
            return this;
        }

        Task<IPasswordStrengthPolicy> IPasswordPolicy.GetPasswordStrengthPolicyAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IPasswordStrengthPolicy>(this.StrengthPolicy.Href, cancellationToken);

        IPasswordStrengthPolicy IPasswordPolicySync.GetPasswordStrengthPolicy()
            => this.GetInternalSyncDataStore().GetResource<IPasswordStrengthPolicy>(this.StrengthPolicy.Href);

        Task<IPasswordPolicy> ISaveable<IPasswordPolicy>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IPasswordPolicy>(cancellationToken);

        IPasswordPolicy ISaveableSync<IPasswordPolicy>.Save()
            => this.Save<IPasswordPolicy>();
    }
}
