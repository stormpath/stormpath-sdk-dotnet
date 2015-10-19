// <copyright file="AccountCreationActionsShared.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Both <see cref="SDK.Application.IApplication"/> and <see cref="SDK.Directory.IDirectory"/> implement
    /// <see cref="IAccountCreationActions"/>, so this shared class wraps the methods up in a DRY way.
    /// </summary>
    internal static class AccountCreationActionsShared
    {
        public static Task<IAccount> CreateAccountAsync(IInternalAsyncDataStore dataStore, string accountsHref, IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            var builder = new AccountCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return dataStore.CreateAsync(accountsHref, account, options, cancellationToken);
        }

        public static IAccount CreateAccount(IInternalSyncDataStore dataStoreSync, string accountsHref, IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new AccountCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return dataStoreSync.Create(accountsHref, account, options);
        }

        public static Task<IAccount> CreateAccountAsync(IInternalAsyncDataStore dataStore, string accountsHref, IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken)
            => dataStore.CreateAsync(accountsHref, account, creationOptions, cancellationToken);

        public static IAccount CreateAccount(IInternalSyncDataStore dataStoreSync, string accountsHref, IAccount account, IAccountCreationOptions creationOptions)
            => dataStoreSync.Create(accountsHref, account, creationOptions);

        public static Task<IAccount> CreateAccountAsync(IInternalAsyncDataStore dataStore, string accountsHref, IAccount account, CancellationToken cancellationToken)
            => dataStore.CreateAsync(accountsHref, account, cancellationToken);

        public static IAccount CreateAccount(IInternalSyncDataStore dataStoreSync, string accountsHref, IAccount account)
            => dataStoreSync.Create(accountsHref, account);

        public static Task<IAccount> CreateAccountAsync(IInternalAsyncDataStore dataStore, string accountsHref, string givenName, string surname, string email, string password, object customData, CancellationToken cancellationToken)
        {
            var account = CreateAccountWith(dataStore, givenName, surname, email, password, customData);

            return CreateAccountAsync(dataStore, accountsHref, account, cancellationToken: cancellationToken);
        }

        public static IAccount CreateAccount(IInternalSyncDataStore dataStoreSync, string accountsHref, string givenName, string surname, string email, string password, object customData = null)
        {
            var account = CreateAccountWith(dataStoreSync, givenName, surname, email, password, customData);

            return CreateAccount(dataStoreSync, accountsHref, account);
        }

        private static IAccount CreateAccountWith(IInternalDataStore dataStore, string givenName, string surname, string email, string password, object customData)
        {
            var account = dataStore.Instantiate<IAccount>();
            account.SetGivenName(givenName);
            account.SetSurname(surname);
            account.SetEmail(email);
            account.SetPassword(password);
            account.CustomData.Put(customData);

            return account;
        }
    }
}
