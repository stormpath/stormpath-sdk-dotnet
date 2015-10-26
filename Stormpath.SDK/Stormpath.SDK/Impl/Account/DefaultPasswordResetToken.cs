// <copyright file="DefaultPasswordResetToken.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultPasswordResetToken : AbstractResource, IPasswordResetToken, IPasswordResetTokenSync
    {
        private static readonly string TokenDelimiter = "/passwordResetTokens/";

        private static readonly string EmailPropertyName = "email";
        private static readonly string PasswordPropertyName = "password";
        private static readonly string AccountPropertyName = "account";

        public DefaultPasswordResetToken(ResourceData data)
            : base(data)
        {
        }

        string IPasswordResetToken.Email => this.GetProperty<string>(EmailPropertyName);

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        public IPasswordResetToken SetEmail(string email)
        {
            this.SetProperty(EmailPropertyName, email);
            return this;
        }

        public IPasswordResetToken SetPassword(string password)
        {
            this.SetProperty<string>(PasswordPropertyName, password);
            return this;
        }

        Task<IAccount> IPasswordResetToken.GetAccountAsync(CancellationToken cancellationToken)
             => this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);

        IAccount IPasswordResetTokenSync.GetAccount()
            => this.GetInternalSyncDataStore().GetResource<IAccount>(this.Account.Href);

        string IPasswordResetToken.GetValue()
        {
            var thisHref = this.AsInterface.Href;

            if (string.IsNullOrEmpty(thisHref))
                return null;

            // Return everything after /passwordResetToken/
            return thisHref.Substring(thisHref.IndexOf(TokenDelimiter) + TokenDelimiter.Length);
        }
    }
}
