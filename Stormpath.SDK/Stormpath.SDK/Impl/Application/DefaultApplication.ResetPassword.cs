// <copyright file="DefaultApplication.ResetPassword.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        Task<IPasswordResetToken> IApplication.SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken)
        {
            var token = this.GetInternalAsyncDataStore()
                .Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);

            return this.GetInternalAsyncDataStore().CreateAsync(this.PasswordResetToken.Href, (IPasswordResetToken)token, cancellationToken);
        }

        Task<IPasswordResetToken> IApplication.SendPasswordResetEmailAsync(string email, IAccountStore accountStore, CancellationToken cancellationToken)
        {
            var token = this.GetInternalAsyncDataStore()
                .Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);
            token.SetAccountStore(accountStore);

            return this.GetInternalAsyncDataStore().CreateAsync(this.PasswordResetToken.Href, (IPasswordResetToken)token, cancellationToken);
        }

        Task<IPasswordResetToken> IApplication.SendPasswordResetEmailAsync(string email, string hrefOrNameKey, CancellationToken cancellationToken)
        {
            var token = this.GetInternalAsyncDataStore()
                .Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);
            token.SetAccountStore(hrefOrNameKey);

            return this.GetInternalAsyncDataStore().CreateAsync(this.PasswordResetToken.Href, (IPasswordResetToken)token, cancellationToken);
        }

        async Task<IAccount> IApplication.VerifyPasswordResetTokenAsync(string token, CancellationToken cancellationToken)
        {
            string href = $"{this.PasswordResetToken.Href}/{token}";

            var validTokenResponse = await this.GetInternalAsyncDataStore().GetResourceAsync<IPasswordResetToken>(href, cancellationToken).ConfigureAwait(false);
            return await validTokenResponse.GetAccountAsync(cancellationToken).ConfigureAwait(false);
        }

        async Task<IAccount> IApplication.ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException(nameof(newPassword));
            }

            var href = $"{this.PasswordResetToken.Href}/{token}";
            var passwordResetToken = this.GetInternalAsyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            passwordResetToken.SetPassword(newPassword);

            var responseToken = await this.GetInternalAsyncDataStore().CreateAsync(href, (IPasswordResetToken)passwordResetToken, cancellationToken).ConfigureAwait(false);
            return await responseToken.GetAccountAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
