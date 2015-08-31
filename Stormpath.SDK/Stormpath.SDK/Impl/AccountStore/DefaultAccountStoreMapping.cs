// <copyright file="DefaultAccountStoreMapping.cs" company="Stormpath, Inc.">
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
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.AccountStore
{
    internal sealed class DefaultAccountStoreMapping : AbstractInstanceResource, IAccountStoreMapping
    {
        private static readonly string AccountStorePropertyName = "accountStore";
        private static readonly string ApplicationPropertyName = "application";
        private static readonly string IsDefaultAccountStorePropertyName = "isDefaultAccountStore";
        private static readonly string IsDefaultGroupStorePropertyName = "isDefaultGroupStore";
        private static readonly string ListIndexPropertyName = "listIndex";

        public DefaultAccountStoreMapping(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultAccountStoreMapping(IInternalDataStore dataStore, Hashtable properties)
            : base(dataStore, properties)
        {
        }

        internal LinkProperty AccountStore => this.GetLinkProperty(AccountStorePropertyName);

        internal LinkProperty Application => this.GetLinkProperty(ApplicationPropertyName);

        bool IAccountStoreMapping.IsDefaultAccountStore => GetProperty<bool>(IsDefaultAccountStorePropertyName);

        bool IAccountStoreMapping.IsDefaultGroupStore => GetProperty<bool>(IsDefaultGroupStorePropertyName);

        int IAccountStoreMapping.ListIndex => GetProperty<int>(ListIndexPropertyName);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
        {
            return this.GetInternalDataStore().DeleteAsync(this, cancellationToken);
        }

        Task<IAccountStore> IAccountStoreMapping.GetAccountStoreAsync(CancellationToken cancellationToken)
        {
            return this.GetInternalDataStore().GetResourceAsync<IAccountStore>(this.AccountStore.Href, cancellationToken);
        }

        Task<IApplication> IAccountStoreMapping.GetApplicationAsync(CancellationToken cancellationToken)
        {
            return this.GetInternalDataStore().GetResourceAsync<IApplication>(this.Application.Href, cancellationToken);
        }

        Task<IAccountStoreMapping> ISaveable<IAccountStoreMapping>.SaveAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
