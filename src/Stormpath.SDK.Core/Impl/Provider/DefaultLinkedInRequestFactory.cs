// <copyright file="DefaultLinkedInRequestFactory.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultLinkedInRequestFactory : ILinkedInRequestFactory
    {
        private readonly IInternalDataStore dataStore;

        public DefaultLinkedInRequestFactory(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        ILinkedInAccountRequestBuilder IProviderRequestFactory<ILinkedInAccountRequestBuilder, ILinkedInCreateProviderRequestBuilder>.Account()
            => new DefaultLinkedInAccountRequestBuilder(this.dataStore);

        ILinkedInCreateProviderRequestBuilder IProviderRequestFactory<ILinkedInAccountRequestBuilder, ILinkedInCreateProviderRequestBuilder>.Builder()
            => new DefaultLinkedInCreateProviderRequestBuilder(this.dataStore);
    }
}
