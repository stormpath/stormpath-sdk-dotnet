// <copyright file="DefaultCustomDataProxy.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.CustomData
{
    internal class DefaultCustomDataProxy : ICustomDataProxy
    {
        private readonly DefaultCustomData proxy;
        private bool deleteAll;

        public DefaultCustomDataProxy(IInternalDataStore dataStore, string parentHref)
        {
            this.proxy = dataStore.InstantiateWithHref<ICustomData>(GenerateProxyId(parentHref)) as DefaultCustomData;
            this.deleteAll = false;
        }

        private static string GenerateProxyId(string parentHref)
            => $"proxy-{parentHref}/customData";

        internal bool DeleteAll
            => this.deleteAll;

        internal bool HasDeletedProperties()
            => this.deleteAll || this.proxy.HasDeletedProperties();

        internal bool HasUpdatedCustomDataProperties()
            => this.proxy.HasUpdatedProperties();

        internal Task<bool> DeleteRemovedCustomDataPropertiesAsync(string parentHref, CancellationToken cancellationToken)
            => this.proxy.DeleteRemovedPropertiesAsync(parentHref, cancellationToken);

        internal bool DeleteRemovedCustomDataProperties(string parentHref)
            => this.proxy.DeleteRemovedProperties(parentHref);

        internal IReadOnlyDictionary<string, object> UpdatedCustomDataProperties
            => this.proxy.GetUpdatedProperties();

        void ICustomDataProxy.Clear()
            => this.deleteAll = true;

        void ICustomDataProxy.Put(KeyValuePair<string, object> item)
            => (this.proxy as ICustomData).Put(item);

        void ICustomDataProxy.Put(IEnumerable<KeyValuePair<string, object>> values)
            => (this.proxy as ICustomData).Put(values);

        void ICustomDataProxy.Put(object customData)
            => (this.proxy as ICustomData).Put(customData);

        void ICustomDataProxy.Put(string key, object value)
            => (this.proxy as ICustomData).Put(key, value);

        void ICustomDataProxy.Remove(string key)
            => (this.proxy as ICustomData).Remove(key); // ignore returned value
    }
}
