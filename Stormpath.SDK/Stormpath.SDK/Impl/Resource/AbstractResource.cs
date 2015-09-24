// <copyright file="AbstractResource.cs" company="Stormpath, Inc.">
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal abstract class AbstractResource : IResource
    {
        protected static readonly string HrefPropertyName = "href";

        internal readonly IInternalDataStore InternalDataStore;
        internal readonly IInternalDataStoreSync InternalDataStoreSync;
        protected readonly ConcurrentDictionary<string, object> properties;
        protected readonly ConcurrentDictionary<string, object> dirtyProperties;

        protected bool isDirty = false;

        public AbstractResource(IInternalDataStore dataStore)
            : this(dataStore, new Dictionary<string, object>())
        {
        }

        public AbstractResource(IInternalDataStore dataStore, IDictionary<string, object> properties)
        {
            this.InternalDataStore = dataStore;
            this.InternalDataStoreSync = dataStore as IInternalDataStoreSync;

            this.properties = new ConcurrentDictionary<string, object>(properties);
            this.dirtyProperties = new ConcurrentDictionary<string, object>();
            this.isDirty = false;
        }

        string IResource.Href => GetProperty<string>(HrefPropertyName);

        protected IInternalDataStore GetInternalDataStore() => this.InternalDataStore;

        protected IInternalDataStoreSync GetInternalDataStoreSync() => this.InternalDataStoreSync;

        internal bool IsDirty => this.isDirty;

        public List<string> GetPropertyNames()
        {
            return this.properties.Keys
                .OfType<string>()
                .ToList();
        }

        public List<string> GetUpdatedPropertyNames()
        {
            return this.dirtyProperties.Keys
                .OfType<string>()
                .ToList();
        }

        public LinkProperty GetLinkProperty(string name)
            => GetProperty<LinkProperty>(name) ?? new LinkProperty(null);

        public T GetProperty<T>(string name)
        {
            var value = this.GetProperty(name);

            return (T)value;
        }

        public object GetProperty(string name)
        {
            object value;

            if (this.dirtyProperties.TryGetValue(name, out value))
                return value;

            if (this.properties.TryGetValue(name, out value))
                return value;

            return null;
        }

        protected bool ContainsProperty(string name)
        {
            return
                this.dirtyProperties.ContainsKey(name) ||
                this.properties.ContainsKey(name);
        }

        public void SetProperty<T>(string name, T value)
            => this.SetProperty(name, (object)value);

        public void SetProperty(string name, object value)
        {
            this.dirtyProperties.AddOrUpdate(name, value, (key, oldValue) => value);
            this.isDirty = true;
        }
    }
}
