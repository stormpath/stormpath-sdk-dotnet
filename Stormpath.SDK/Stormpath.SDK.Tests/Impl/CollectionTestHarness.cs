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

using NSubstitute;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tests.Impl
{
    public class CollectionTestHarness<T>
        where T : IResource
    {
        internal IDataStore DataStore { get; private set; }

        public string Url { get; private set; }

        public string Resource { get; private set; }

        public ICollectionResourceQueryable<T> Queryable { get; private set; }

        internal static CollectionTestHarness<TType> Create<TType>(string url, string resource, IDataStore mockDataStore = null)
            where TType : IResource
        {
            var ds = mockDataStore ?? Substitute.For<IDataStore>();

            return new CollectionTestHarness<TType>()
            {
                DataStore = ds,
                Resource = resource,
                Url = url,
                Queryable = new CollectionResourceQueryable<TType>(url, resource, ds)
            };
        }
    }
}
