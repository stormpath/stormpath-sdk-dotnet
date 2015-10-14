﻿// <copyright file="AbstractProvider.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    /// <summary>
    /// Base class for Provider-specific <see cref="IProvider"/> resources.
    /// </summary>
    internal abstract class AbstractProvider : AbstractInstanceResource, IProvider
    {
        private static readonly string ProviderIdPropertyName = "providerId";

        public AbstractProvider(IInternalDataStore dataStore)
            : this(dataStore, null)
        {
        }

        public AbstractProvider(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore)
        {
            this.ResetAndUpdate(properties);
            this.SetProperty(ProviderIdPropertyName, this.ConcreteProviderId);
        }

        string IProvider.ProviderId
            => this.GetProperty<string>(ProviderIdPropertyName);

        protected abstract string ConcreteProviderId { get; }
    }
}