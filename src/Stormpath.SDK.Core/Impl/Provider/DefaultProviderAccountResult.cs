// <copyright file="DefaultProviderAccountResult.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultProviderAccountResult : AbstractResource, IProviderAccountResult, INotifiable
    {
        private static readonly string IsNewAccountPropertyName = "isNewAccount";
        private static readonly string AccountPropertyName = "account";

        public DefaultProviderAccountResult(ResourceData data)
            : base(data)
        {
        }

        IAccount IProviderAccountResult.Account
            => this.GetProperty<IAccount>(AccountPropertyName);

        bool IProviderAccountResult.IsNewAccount
            => this.GetBoolProperty(IsNewAccountPropertyName);

        void INotifiable.OnUpdate(Map properties, IInternalDataStore dataStore)
        {
            var newProperties = new Dictionary<string, object>(2);

            bool hasProperties = !properties.IsNullOrEmpty();
            if (hasProperties)
            {
                newProperties.Add(IsNewAccountPropertyName, properties[IsNewAccountPropertyName]);
                properties.Remove(IsNewAccountPropertyName);

                var account = dataStore.InstantiateWithData<IAccount>(properties);
                newProperties.Add(AccountPropertyName, account);

                this.GetResourceData()?.Update(newProperties);
            }
        }
    }
}
