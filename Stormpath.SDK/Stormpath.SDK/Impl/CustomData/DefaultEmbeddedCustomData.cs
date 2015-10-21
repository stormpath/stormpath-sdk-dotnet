// <copyright file="DefaultEmbeddedCustomData.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.CustomData
{
    internal class DefaultEmbeddedCustomData : IEmbeddedCustomData
    {
        private readonly DefaultCustomData customDataProxy;
        private bool deleteAll;

        public DefaultEmbeddedCustomData(IInternalDataStore dataStore, string parentHref)
        {
            this.customDataProxy = dataStore.InstantiateWithHref<ICustomData>(GenerateEmbeddedCustomDataId(parentHref)) as DefaultCustomData;
            this.deleteAll = false;
        }

        private static string GenerateEmbeddedCustomDataId(string parentHref)
            => $"proxy-{parentHref}/customData";

        internal bool DeleteAll
            => this.deleteAll;

        internal bool HasDeletedProperties()
            => this.deleteAll || this.customDataProxy.HasDeletedProperties();

        internal bool HasUpdatedCustomDataProperties()
            => this.customDataProxy.HasUpdatedProperties();

        internal Task<bool> DeleteRemovedCustomDataPropertiesAsync(string parentHref, CancellationToken cancellationToken)
            => this.customDataProxy.DeleteRemovedPropertiesAsync(parentHref, cancellationToken);

        internal bool DeleteRemovedCustomDataProperties(string parentHref)
            => this.customDataProxy.DeleteRemovedProperties(parentHref);

        internal IReadOnlyDictionary<string, object> UpdatedCustomDataProperties
            => this.customDataProxy.GetUpdatedProperties();

        void IEmbeddedCustomData.Clear()
            => this.deleteAll = true;

        void IEmbeddedCustomData.Put(KeyValuePair<string, object> item)
            => (this.customDataProxy as ICustomData).Put(item);

        void IEmbeddedCustomData.Put(IEnumerable<KeyValuePair<string, object>> values)
            => (this.customDataProxy as ICustomData).Put(values);

        void IEmbeddedCustomData.Put(object customData)
            => (this.customDataProxy as ICustomData).Put(customData);

        void IEmbeddedCustomData.Put(string key, object value)
            => (this.customDataProxy as ICustomData).Put(key, value);

        void IEmbeddedCustomData.Remove(string key)
            => (this.customDataProxy as ICustomData).Remove(key); // ignore returned value
    }
}
