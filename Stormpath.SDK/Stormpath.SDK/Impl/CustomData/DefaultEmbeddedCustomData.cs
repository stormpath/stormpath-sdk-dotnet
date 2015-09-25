// <copyright file="DefaultEmbeddedCustomData.cs" company="Stormpath, Inc.">
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

        public DefaultEmbeddedCustomData(IInternalDataStore dataStore)
        {
            this.customDataProxy = new DefaultCustomData(dataStore);
            this.deleteAll = false;
        }

        internal bool DeleteAll
            => this.deleteAll;

        internal bool HasDeletedProperties()
            => this.deleteAll || this.customDataProxy.HasDeletedProperties();

        internal bool HasUpdatedCustomDataProperties()
            => this.customDataProxy.HasUpdatedProperties();

        internal Task<bool> DeleteRemovedCustomDataPropertiesAsync(string parentHref, CancellationToken cancellationToken)
            => this.customDataProxy.DeleteRemovedPropertiesAsync(parentHref, cancellationToken);

        internal IDictionary<string, object> UpdatedCustomDataProperties
            => this.customDataProxy.UpdatedProperties;

        void IEmbeddedCustomData.Clear()
            => this.deleteAll = true;

        void IEmbeddedCustomData.Put(KeyValuePair<string, object> item)
            => (this.customDataProxy as ICustomData).Put(item);

        void IEmbeddedCustomData.Put(IDictionary<string, object> values)
            => (this.customDataProxy as ICustomData).Put(values);

        void IEmbeddedCustomData.Put(string key, object value)
            => (this.customDataProxy as ICustomData).Put(key, value);

        void IEmbeddedCustomData.Remove(string key)
            => (this.customDataProxy as ICustomData).Remove(key); // ignore returned value
    }
}
