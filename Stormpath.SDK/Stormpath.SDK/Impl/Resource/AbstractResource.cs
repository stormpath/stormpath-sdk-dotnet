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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal abstract class AbstractResource : IResource
    {
        private static readonly string HrefPropertyName = "href";

        private readonly object writeLock = new object();

        private readonly IInternalDataStore dataStore;
        private readonly Hashtable properties;
        private readonly Hashtable dirtyProperties;

        private bool isDirty = false;

        public AbstractResource(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;

            this.properties = new Hashtable();
            this.dirtyProperties = new Hashtable();
        }

        public AbstractResource(IInternalDataStore dataStore, Hashtable properties)
        {
            this.dataStore = dataStore;

            this.properties = new Hashtable(properties);
            this.dirtyProperties = new Hashtable(properties.Count);
        }

        string IResource.Href => GetProperty<string>(HrefPropertyName);

        protected IInternalDataStore GetInternalDataStore() => this.dataStore;

        public LinkProperty GetLinkProperty(string name)
        {
            return GetProperty<LinkProperty>(name) ?? new LinkProperty(null);
        }

        public T GetProperty<T>(string name)
        {
            var value = this.GetProperty(name);

            return (T)value;
        }

        public object GetProperty(string name)
        {
            object value;

            value = this.dirtyProperties[name];
            if (value != null)
                return value;

            return this.properties[name];
        }

        public void SetProperty<T>(string name, T value)
        {
            this.SetProperty(name, (object)value);
        }

        public void SetProperty(string name, object value)
        {
            lock (this.writeLock)
            {
                if (!this.properties.ContainsKey(name))
                    this.properties.Add(name, value);

                this.dirtyProperties[name] = value;
                this.isDirty = true;
            }
        }

        public void SetProperties(Hashtable newProperties)
        {
            try
            {
                lock (this.writeLock)
                {
                    foreach (DictionaryEntry item in newProperties)
                    {
                        this.properties[item.Key] = item.Value;
                    }

                    this.isDirty = false;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not load properties into resource item.", e);
            }
        }

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
    }
}
