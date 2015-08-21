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
using System.Threading.Tasks;
using NSubstitute;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tests.Helpers
{
    public sealed class CollectionTestHarness<T>
        where T : IResource
    {
        internal IDataStore DataStore { get; private set; }

        public string Href { get; set; }

        public ICollectionResourceQueryable<T> Queryable { get; private set; }

        internal static CollectionTestHarness<TType> Create<TType>(string collectionHref, IDataStore mockDataStore = null)
            where TType : class, IResource
        {
            var ds = mockDataStore ?? MockEmptyDataStore<TType>();

            return new CollectionTestHarness<TType>()
            {
                DataStore = ds,
                Href = collectionHref,
                Queryable = new CollectionResourceQueryable<TType>(collectionHref, ds)
            };
        }

        private static IDataStore MockEmptyDataStore<TType>()
            where TType : class, IResource
        {
            var emptyMock = Substitute.For<IDataStore>();
            emptyMock.GetCollectionAsync<TType>(Arg.Any<string>()).Returns(
                    Task.FromResult(new CollectionResponsePage<TType>()
                    {
                        Limit = 0,
                        Offset = 0,
                        Size = 0,
                        Items = Enumerable.Empty<TType>().ToList()
                    }));
            return emptyMock;
        }
    }
}
