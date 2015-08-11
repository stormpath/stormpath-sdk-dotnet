// <copyright file="LinqTestHarness.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class LinqTestHarness<T>
        where T : IResource
    {
        public IDataStore DataStore { get; private set; }

        public string Url { get; private set; }

        public string Resource { get; private set; }

        public ICollectionResourceQueryable<T> Queryable { get; private set; }

        public void WasCalledWithArguments(string arguments)
        {
            DataStore.Received().GetCollection<T>($"{Url}/{Resource}?{arguments}");
        }

        public static LinqTestHarness<TType> Create<TType>(string url, string resource)
            where TType : IResource
        {
            var ds = Substitute.For<IDataStore>();

            return new LinqTestHarness<TType>()
            {
                DataStore = ds,
                Resource = resource,
                Url = url,
                Queryable = new CollectionResourceQueryable<TType>(url, resource, ds)
            };
        }
    }
}
