// <copyright file="AbstractResource.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Resource
{
    internal abstract class AbstractResource : IResource, ILinkable
    {
        public static readonly string HrefPropertyName = "href";
        public static readonly string TenantPropertyName = "tenant";

        private ResourceData resourceData;

        public AbstractResource(ResourceData data)
        {
            this.resourceData = data;
        }

        void ILinkable.Link(ResourceData data)
        {
            Interlocked.Exchange(ref this.resourceData, data);
        }

        public bool IsLinkedTo(AbstractResource other)
            => ReferenceEquals(this.resourceData, other.resourceData);

        protected IResource AsInterface => this;

        public ResourceData GetResourceData() => this.resourceData;

        string IResource.Href
        {
            get
            {
                var href = this.InternalHref;

                bool isEmptyOrDefault =
                    href == null ||
                    href.StartsWith("autogen", StringComparison.OrdinalIgnoreCase);

                return isEmptyOrDefault
                    ? null
                    : href;
            }
        }

        protected string InternalHref => this.GetStringProperty(HrefPropertyName);

        IClient IResource.Client
            => this.GetInternalDataStore()?.Client;

        // todo - set these back to protected when bumping to 1.0
        public IInternalDataStore GetInternalDataStore()
            => this.GetResourceData()?.InternalDataStore;

        public IInternalAsyncDataStore GetInternalAsyncDataStore()
            => this.GetResourceData()?.InternalAsyncDataStore;

        public IInternalSyncDataStore GetInternalSyncDataStore()
            => this.GetResourceData()?.InternalSyncDataStore;

        public Task<ITenant> GetTenantAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<ITenant>(this.GetLinkProperty(TenantPropertyName).Href, cancellationToken);

        public ITenant GetTenant()
            => this.GetInternalSyncDataStore().GetResource<ITenant>(this.GetLinkProperty(TenantPropertyName).Href);

        internal bool IsDirty => this.GetResourceData()?.IsDirty ?? true;

        public IReadOnlyList<string> GetPropertyNames()
            => this.GetResourceData()?.GetPropertyNames();

        public IReadOnlyList<string> GetUpdatedPropertyNames()
            => this.GetResourceData()?.GetUpdatedPropertyNames();

        public object GetProperty(string name)
            => this.GetResourceData()?.GetProperty(name);

        public int GetIntProperty(string name)
            => Convert.ToInt32(this.GetProperty(name) ?? default(int));

        public long GetLongProperty(string name)
            => Convert.ToInt64(this.GetProperty(name) ?? default(long));

        public string GetStringProperty(string name)
            => this.GetProperty(name)?.ToString();

        public bool GetBoolProperty(string name)
            => Convert.ToBoolean(this.GetProperty(name) ?? default(bool));

        public DateTimeOffset? GetDateTimeProperty(string name)
        {
            var value = this.GetProperty(name);

            if (value == null)
            {
                return null;
            }

            return (DateTimeOffset)value;
        }

        public T GetProperty<T>(string name)
            where T : class
            => (T)(this.GetProperty(name) ?? default(T));

        public IEmbeddedProperty GetLinkProperty(string name)
            => (this.GetProperty(name) as IEmbeddedProperty) ?? new LinkProperty(null);

        public IReadOnlyList<T> GetListProperty<T>(string name)
        {
            var boxedList = this.GetProperty(name) as IEnumerable<object>;

            return boxedList == null
                ? Enumerable.Empty<T>().ToArray()
                : boxedList.OfType<T>().ToArray();
        }

        public bool ContainsProperty(string name)
            => this.GetResourceData()?.ContainsProperty(name) ?? false;

        public void SetProperty(string name, object value)
            => this.GetResourceData()?.SetProperty(name, value);

        public void SetProperty<T>(string name, T value)
            => this.SetProperty(name, (object)value);

        public void SetLinkProperty(string name, string href)
            => this.SetProperty(name, new LinkProperty(href));
    }
}
