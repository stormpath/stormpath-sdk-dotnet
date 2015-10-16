// <copyright file="ResourceData.cs" company="Stormpath, Inc.">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.Resource
{
    internal sealed class ResourceData
    {
        private readonly IInternalDataStore internalDataStore;
        private readonly IInternalAsyncDataStore internalDataStoreAsync;
        private readonly IInternalSyncDataStore internalDataStoreSync;

        private Func<IDictionary<string, object>, IInternalDataStore, IDictionary<string, object>> propertiesMutator;

        private ConcurrentDictionary<string, object> properties;
        private ConcurrentDictionary<string, object> dirtyProperties;
        private ConcurrentDictionary<string, object> deletedProperties;

        private bool isDirty = false;

        public ResourceData(IInternalDataStore dataStore)
        {
            this.internalDataStore = dataStore;
            this.internalDataStoreAsync = dataStore as IInternalAsyncDataStore;
            this.internalDataStoreSync = dataStore as IInternalSyncDataStore;

            this.Update();
        }

        public IInternalDataStore InternalDataStore => this.internalDataStore;

        public IInternalAsyncDataStore InternalAsyncDataStore => this.internalDataStoreAsync;

        public IInternalSyncDataStore InternalSyncDataStore => this.internalDataStoreSync;

        public bool IsDirty => this.isDirty;

        public void SetPropertiesMutator(Func<IDictionary<string, object>, IInternalDataStore, IDictionary<string, object>> mutator)
        {
            this.propertiesMutator = mutator;
        }

        public IReadOnlyList<string> GetPropertyNames()
            => this.properties.Select(x => x.Key).ToList();

        public IReadOnlyList<string> GetUpdatedPropertyNames()
            => this.dirtyProperties.Select(x => x.Key).ToList();

        public IReadOnlyDictionary<string, object> GetUpdatedProperties()
            => new Dictionary<string, object>(this.dirtyProperties);

        public IReadOnlyList<string> GetDeletedPropertyNames()
            => this.deletedProperties.Select(x => x.Key).ToList();

        public object GetProperty(string name)
        {
            if (this.deletedProperties.ContainsKey(name))
                return null;

            object value;

            if (this.dirtyProperties.TryGetValue(name, out value))
                return value;

            if (this.properties.TryGetValue(name, out value))
                return value;

            return null;
        }

        public void SetProperty(string name, object value)
        {
            object dummy;
            this.deletedProperties.TryRemove(name, out dummy);

            this.dirtyProperties.AddOrUpdate(name, value, (key, oldValue) => value);
            this.isDirty = true;
        }

        public object RemoveProperty(string name)
        {
            object removedFromProperties;
            this.properties.TryGetValue(name, out removedFromProperties);

            object removedFromDirtyProperties;
            this.dirtyProperties.TryRemove(name, out removedFromDirtyProperties);

            this.deletedProperties.TryAdd(name, null);
            this.isDirty = true;

            return removedFromDirtyProperties ?? removedFromProperties;
        }

        public bool OnDeletingRemovedProperty(string name)
        {
            object dummy;
            this.deletedProperties.TryRemove(name, out dummy);

            return dummy != null;
        }

        public bool ContainsProperty(string name)
        {
            return
                this.dirtyProperties.ContainsKey(name) ||
                this.properties.ContainsKey(name);
        }

        public void Update(IDictionary<string, object> properties = null)
        {
            if (properties == null)
                properties = new Dictionary<string, object>();

            if (this.propertiesMutator != null)
                properties = this.propertiesMutator(properties, this.internalDataStore);

            this.properties = new ConcurrentDictionary<string, object>(properties);
            this.dirtyProperties = new ConcurrentDictionary<string, object>();
            this.deletedProperties = new ConcurrentDictionary<string, object>();
            this.isDirty = false;
        }
    }
}
