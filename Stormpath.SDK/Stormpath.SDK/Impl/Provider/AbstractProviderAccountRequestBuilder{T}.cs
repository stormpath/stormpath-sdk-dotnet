// <copyright file="AbstractProviderAccountRequestBuilder{T}.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    /// <summary>
    /// Base class that each Provider-specific <see cref="IProviderAccountRequestBuilder{T}"/> can extend to facilitate its work.
    /// </summary>
    /// <typeparam name="T">The builder type.</typeparam>
    internal abstract class AbstractProviderAccountRequestBuilder<T> : IProviderAccountRequestBuilder<T>
        where T : class, IProviderAccountRequestBuilder<T>
    {
        protected readonly IInternalDataStore dataStore;

        protected string accessToken;

        public AbstractProviderAccountRequestBuilder(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        protected abstract string ConcreteProviderId { get; }

        T IProviderAccountRequestBuilder<T>.SetAccessToken(string accessToken)
        {
            this.accessToken = accessToken;
            return this as T;
        }

        IProviderAccountRequest IProviderAccountRequestBuilder<T>.Build()
        {
            var providerId = this.ConcreteProviderId;
            if (string.IsNullOrEmpty(providerId))
            {
                throw new ApplicationException("The Provider ID is missing.");
            }

            return this.BuildConcrete();
        }

        /// <summary>
        /// Delegates responsibility to Provider-specific subclasses when constructing <see cref="IProviderAccountRequest"/> instances,
        /// so subclasses can add their own properties.
        /// </summary>
        /// <returns>The actual request based on the derived builder's current state.</returns>
        protected abstract IProviderAccountRequest BuildConcrete();
    }
}