// <copyright file="IApplicationSync.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Application
{
    internal interface IApplicationSync : ISaveableWithOptionsSync<IApplication>, IDeletableSync, IExtendableSync, IAccountCreationActionsSync, IGroupCreationActionsSync
    {
        IAuthenticationResult AuthenticateAccount(IAuthenticationRequest request);

        IAuthenticationResult AuthenticateAccount(IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions);

        IAuthenticationResult AuthenticateAccount(Action<UsernamePasswordRequestBuilder> requestBuilder);

        IAuthenticationResult AuthenticateAccount(Action<UsernamePasswordRequestBuilder> requestBuilder, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions);

        IAuthenticationResult AuthenticateAccount(string username, string password);

        bool TryAuthenticateAccount(string username, string password);

        void SendVerificationEmail(Action<EmailVerificationRequestBuilder> requestBuilderAction);

        void SendVerificationEmail(string usernameOrEmail);

        ITenant GetTenant();

        IAccountStore GetDefaultAccountStore();

        void SetDefaultAccountStore(IAccountStore accountStore);

        IAccountStore GetDefaultGroupStore();

        void SetDefaultGroupStore(IAccountStore accountStore);

        IAccountStoreMapping CreateAccountStoreMapping(IAccountStoreMapping mapping);

        IAccountStoreMapping AddAccountStore(IAccountStore accountStore);

        IAccountStoreMapping AddAccountStore(string hrefOrName);

        IAccountStoreMapping AddAccountStore<T>(Func<IQueryable<T>, IQueryable<T>> query)
            where T : IAccountStore;

        IAccount ResetPassword(string token, string newPassword);

        IPasswordResetToken SendPasswordResetEmail(string email);

        IPasswordResetToken SendPasswordResetEmail(string email, IAccountStore accountStore);

        IPasswordResetToken SendPasswordResetEmail(string email, string hrefOrNameKey);

        IAccount VerifyPasswordResetToken(string token);

        IProviderAccountResult GetAccount(IProviderAccountRequest request);
    }
}
