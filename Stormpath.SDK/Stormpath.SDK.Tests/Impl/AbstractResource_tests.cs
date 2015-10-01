// <copyright file="AbstractResource_tests.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class AbstractResource_tests
    {
        private readonly IDataStore dataStore;

        public AbstractResource_tests()
        {
            this.dataStore = new StubDataStore(FakeJson.Account, "http://api.foo.bar");
        }

        [Fact]
        public void Instantiated_resources_are_not_linked()
        {
            var account1 = this.dataStore.Instantiate<IAccount>();
            var account2 = this.dataStore.Instantiate<IAccount>();

            account1.MiddleName.ShouldBeNullOrEmpty();
            account2.MiddleName.ShouldBeNullOrEmpty();

            account1.SetMiddleName("hello world!");
            account2.MiddleName.ShouldBeNullOrEmpty();
        }

        [Fact]
        public async Task Loaded_resources_are_linked()
        {
            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/foo");
            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/foo");

            account1.MiddleName.ShouldBeNullOrEmpty();
            account2.MiddleName.ShouldBeNullOrEmpty();

            account1.SetMiddleName("hello world!");
            account2.MiddleName.ShouldBe("hello world!");
        }
    }
}
