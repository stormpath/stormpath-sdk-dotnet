// <copyright file="Synchronously_extension_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Sync;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Synchronously_extension_tests : Linq_test<IAccount>
    {
        [Fact]
        public void When_called_first()
        {
            this.Queryable
                .Synchronously()
                .Expand(x => x.GetCustomData())
                .Filter("foo")
                .Skip(10)
                .Take(5)
                .Where(x => x.Email == "vader@galacticempire.co")
                .ToList();

            this.ShouldBeCalledWithArguments(
                "expand=customData",
                "q=foo",
                "offset=10",
                "limit=5",
                "email=vader%40galacticempire.co");
        }

        [Fact]
        public void When_called_mid_query()
        {
            this.Queryable
                .Expand(x => x.GetCustomData())
                .Filter("foo")
                .Synchronously()
                .Skip(10)
                .Take(5)
                .Where(x => x.Email == "vader@galacticempire.co")
                .ToList();

            this.ShouldBeCalledWithArguments(
                "expand=customData",
                "q=foo",
                "offset=10",
                "limit=5",
                "email=vader%40galacticempire.co");
        }

        [Fact]
        public void When_called_last()
        {
            this.Queryable
                .Expand(x => x.GetCustomData())
                .Filter("foo")
                .Skip(10)
                .Take(5)
                .Where(x => x.Email == "vader@galacticempire.co")
                .Synchronously()
                .ToList();

            this.ShouldBeCalledWithArguments(
                "expand=customData",
                "q=foo",
                "offset=10",
                "limit=5",
                "email=vader%40galacticempire.co");
        }

        [Fact]
        public void Returns_a_vanilla_queryable()
        {
            var synchronousQueryable = this.Queryable
                .Synchronously()
                .Skip(10);

            synchronousQueryable.ShouldBeAssignableTo<IQueryable<IAccount>>();
            synchronousQueryable.ShouldNotBeNull();
        }
    }
}
