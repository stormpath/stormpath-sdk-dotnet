// <copyright file="CollectionTestHarness.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tests.Fakes;

namespace Stormpath.SDK.Tests.Helpers
{
    public sealed class CollectionTestHarness<T>
        where T : IResource
    {
        internal IInternalAsyncDataStore DataStoreAsync { get; private set; }

        public string Href { get; set; }

        public IAsyncQueryable<T> Queryable { get; private set; }

        internal static CollectionTestHarness<TType> Create<TType>(string collectionHref, IInternalAsyncDataStore mockDataStore = null)
            where TType : class, IResource
        {
            var ds = mockDataStore ?? new FakeDataStore<TType>(Enumerable.Empty<TType>());

            return new CollectionTestHarness<TType>()
            {
                DataStoreAsync = ds,
                Href = collectionHref,
                Queryable = new CollectionResourceQueryable<TType>(collectionHref, ds)
            };
        }
    }
}
