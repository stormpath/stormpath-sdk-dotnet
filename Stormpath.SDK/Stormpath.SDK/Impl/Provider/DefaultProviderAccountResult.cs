// <copyright file="DefaultProviderAccountResult.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultProviderAccountResult : AbstractResource, IProviderAccountResult
    {
        private static readonly string IsNewAccountPropertyName = "isNewAccount";
        private static readonly string AccountPropertyName = "account";

        public DefaultProviderAccountResult(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultProviderAccountResult(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore)
        {
            if (properties != null)
            {
                this.SetProperty(IsNewAccountPropertyName, properties[IsNewAccountPropertyName]);
                properties.Remove(IsNewAccountPropertyName);

                var account = this.GetInternalDataStore().Instantiate<IAccount>() as DefaultAccount;
                account.ResetAndUpdate(properties);
                this.SetProperty(AccountPropertyName, account as IAccount);
            }
        }

        IAccount IProviderAccountResult.Account
            => this.GetProperty<IAccount>(AccountPropertyName);

        bool IProviderAccountResult.IsNewAccount
            => this.GetProperty<bool>(IsNewAccountPropertyName);
    }
}
