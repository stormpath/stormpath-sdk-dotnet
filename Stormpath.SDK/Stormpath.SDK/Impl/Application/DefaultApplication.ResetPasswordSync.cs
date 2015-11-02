// <copyright file="DefaultApplication.ResetPasswordSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IPasswordResetToken IApplicationSync.SendPasswordResetEmail(string email)
        {
            var token = this.GetInternalSyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);

            return this.GetInternalSyncDataStore().Create(this.PasswordResetToken.Href, (IPasswordResetToken)token);
        }

        IAccount IApplicationSync.VerifyPasswordResetToken(string token)
        {
            string href = $"{this.PasswordResetToken.Href}/{token}";

            var validTokenResponse = this.GetInternalSyncDataStore().GetResource<IPasswordResetToken>(href);
            return validTokenResponse.GetAccount();
        }

        IAccount IApplicationSync.ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            var href = $"{this.PasswordResetToken.Href}/{token}";
            var passwordResetToken = this.GetInternalSyncDataStore().Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            passwordResetToken.SetPassword(newPassword);

            var responseToken = this.GetInternalSyncDataStore().Create(href, (IPasswordResetToken)passwordResetToken);
            return responseToken.GetAccount();
        }

        IPasswordResetToken IApplicationSync.SendPasswordResetEmail(string email, IAccountStore accountStore)
        {
            var token = this.GetInternalSyncDataStore()
                .Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);
            token.SetAccountStore(accountStore);

            return this.GetInternalSyncDataStore().Create(this.PasswordResetToken.Href, (IPasswordResetToken)token);
        }

        IPasswordResetToken IApplicationSync.SendPasswordResetEmail(string email, string hrefOrNameKey)
        {
            var token = this.GetInternalSyncDataStore()
                .Instantiate<IPasswordResetToken>() as DefaultPasswordResetToken;
            token.SetEmail(email);
            token.SetAccountStore(hrefOrNameKey);

            return this.GetInternalSyncDataStore().Create(this.PasswordResetToken.Href, (IPasswordResetToken)token);
        }
    }
}
