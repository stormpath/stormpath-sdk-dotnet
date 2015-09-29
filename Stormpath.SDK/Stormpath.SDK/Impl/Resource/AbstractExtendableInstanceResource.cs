﻿// <copyright file="AbstractExtendableInstanceResource.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.CustomData;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal abstract class AbstractExtendableInstanceResource : AbstractInstanceResource, IExtendable, IExtendableSync
    {
        private static readonly string CustomDataPropertyName = "customData";

        private DefaultEmbeddedCustomData customDataProxy;

        protected AbstractExtendableInstanceResource(IInternalDataStore dataStore)
            : base(dataStore)
        {
            this.ResetCustomData();
        }

        protected AbstractExtendableInstanceResource(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore, properties)
        {
            this.ResetCustomData();
        }

        internal LinkProperty CustomData => this.GetProperty<LinkProperty>(CustomDataPropertyName);

        IEmbeddedCustomData IExtendable.CustomData => this.customDataProxy;

        IEmbeddedCustomData IExtendableSync.CustomData => this.customDataProxy;

        Task<ICustomData> IExtendable.GetCustomDataAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().GetResourceAsync<ICustomData>(this.CustomData.Href, cancellationToken);

        ICustomData IExtendableSync.GetCustomData()
            => this.GetInternalDataStoreSync().GetResource<ICustomData>(this.CustomData.Href);

        internal void ResetCustomData()
        {
            this.customDataProxy = new DefaultEmbeddedCustomData(this.InternalDataStore);
        }
    }
}
