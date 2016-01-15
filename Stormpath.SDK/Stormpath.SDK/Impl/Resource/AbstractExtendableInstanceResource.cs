// <copyright file="AbstractExtendableInstanceResource.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.CustomData;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal abstract class AbstractExtendableInstanceResource : AbstractInstanceResource, IExtendable, IExtendableSync
    {
        public static readonly string CustomDataPropertyName = "customData";

        private ICustomDataProxy customDataProxy;

        protected AbstractExtendableInstanceResource(ResourceData data)
            : base(data)
        {
            this.ResetCustomData();
        }

        public static bool IsExtendable(Type type)
            => typeof(IExtendable).IsAssignableFrom(type);

        internal IEmbeddedProperty CustomData => this.GetLinkProperty(CustomDataPropertyName);

        ICustomDataProxy IExtendable.CustomData => this.customDataProxy;

        ICustomDataProxy IExtendableSync.CustomData => this.customDataProxy;

        Task<ICustomData> IExtendable.GetCustomDataAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<ICustomData>(this.CustomData.Href, cancellationToken);

        ICustomData IExtendableSync.GetCustomData()
            => this.GetInternalSyncDataStore().GetResource<ICustomData>(this.CustomData.Href);

        public void ResetCustomData()
        {
            this.customDataProxy = new DefaultCustomDataProxy(this.GetInternalAsyncDataStore(), this.AsInterface.Href);
        }
    }
}
