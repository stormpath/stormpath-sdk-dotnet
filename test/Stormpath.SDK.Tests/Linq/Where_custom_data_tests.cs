// <copyright file="Where_custom_data_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    public class Where_custom_data_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Throws_when_specifying_string_comparison_for_customData_Equals()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => ((string)x.CustomData["foobar"]).Equals("baz", StringComparison.Ordinal))
                    .MoveNextAsync();
            });
        }

#pragma warning disable CS0252 // Unintended reference comparison
        [Fact]
        public async Task Where_customData_string_equalsequals()
        {
            await this.Queryable
                .Where(x => x.CustomData["post"] == "Cell Block 1138")
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }
#pragma warning restore CS0252

#pragma warning disable CS0253 // Unintended reference comparison
        [Fact]
        public async Task Where_customData_string_equalsequals_reversed()
        {
            await this.Queryable
                .Where(x => "Cell Block 1138" == x.CustomData["post"])
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }
#pragma warning restore CS0253

        [Fact]
        public async Task Where_customData_casted_string_equalsequals()
        {
            await this.Queryable
                .Where(x => (string)x.CustomData["post"] == "Cell Block 1138")
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_casted_string_equalsequals_reversed()
        {
            await this.Queryable
                .Where(x => "Cell Block 1138" == (string)x.CustomData["post"])
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_casted_as_string_equalsequals()
        {
            await this.Queryable
                .Where(x => (x.CustomData["post"] as string) == "Cell Block 1138")
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_casted_as_string_equalsequals_reversed()
        {
            await this.Queryable
                .Where(x => "Cell Block 1138" == (x.CustomData["post"] as string))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_string_Equals()
        {
            await this.Queryable
                .Where(x => x.CustomData["post"].Equals("Cell Block 1138"))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_casted_string_Equals()
        {
            await this.Queryable
                .Where(x => ((string)x.CustomData["post"]).Equals("Cell Block 1138"))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_casted_as_string_Equals()
        {
            await this.Queryable
                .Where(x => (x.CustomData["post"] as string).Equals("Cell Block 1138"))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.post=Cell+Block+1138");
        }

        [Fact]
        public async Task Where_customData_string_StartsWith()
        {
            await this.Queryable
                .Where(x => (x.CustomData["saying"] as string).StartsWith("Its a"))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.saying=Its+a*");
        }

        [Fact]
        public async Task Where_customData_string_StartsWith_numbers()
        {
            await this.Queryable
                .Where(x => (x.CustomData["score"] as string).StartsWith("1234"))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=1234*");
        }

        [Fact]
        public async Task Where_customData_string_EndsWith()
        {
            await this.Queryable
                .Where(x => (x.CustomData["saying"] as string).EndsWith("trap!"))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.saying=*trap!");
        }

        [Fact]
        public async Task Where_customData_date_greater_than()
        {
            await this.Queryable
                .Where(x => (DateTimeOffset)x.CustomData["birthday"] > new DateTimeOffset(1983, 05, 01, 00, 00, 00, TimeSpan.Zero))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.birthday=(1983-05-01T00:00:00Z,]");
        }

        [Fact]
        public async Task Where_customData_date_less_than()
        {
            await this.Queryable
                .Where(x => (DateTimeOffset)x.CustomData["birthday"] < new DateTimeOffset(1983, 06, 01, 00, 00, 00, TimeSpan.Zero))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.birthday=[,1983-06-01T00:00:00Z)");
        }

        [Fact]
        public async Task Where_customData_date_between()
        {
            await this.Queryable
                .Where(x => (DateTimeOffset)x.CustomData["birthday"] >= new DateTimeOffset(1983, 05, 01, 00, 00, 00, TimeSpan.Zero)
                         && (DateTimeOffset)x.CustomData["birthday"] <= new DateTimeOffset(1983, 06, 01, 00, 00, 00, TimeSpan.Zero))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.birthday=[1983-05-01T00:00:00Z,1983-06-01T00:00:00Z]");
        }

        [Fact]
        public async Task Where_customData_int_between_inclusive()
        {
            await this.Queryable
                .Where(x => (int)x.CustomData["score"] >= 0 && (int)x.CustomData["score"] <= 100)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=[0,100]");
        }

        [Fact]
        public async Task Where_customData_int_between_exclusive()
        {
            await this.Queryable
                .Where(x => (int)x.CustomData["score"] > 0 && (int)x.CustomData["score"] < 100)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(0,100)");
        }

        [Fact]
        public async Task Where_customData_int_greater_than()
        {
            await this.Queryable
                .Where(x => (int)x.CustomData["score"] > 50)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(50,]");
        }

        [Fact]
        public async Task Where_customData_int_greater_than_with_specified_precision()
        {
            await this.Queryable
                .Where(x => (int)x.CustomData["score"] > 50.WithPlaces(3))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(50.000,]");
        }

        [Fact]
        public async Task Where_customData_int_less_than()
        {
            await this.Queryable
                .Where(x => (int)x.CustomData["score"] < 50)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=[,50)");
        }

        [Fact]
        public async Task Where_customData_float_between()
        {
            await this.Queryable
                .Where(x => (float)x.CustomData["score"] >= 0.01 && (float)x.CustomData["score"] < 1.01)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=[0.01,1.01)");
        }

        [Fact]
        public async Task Where_customData_float_with_specified_int_precision()
        {
            await this.Queryable
                .Where(x => (float)x.CustomData["score"] > 1.WithPlaces(3))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(1.000,]");
        }

        [Fact]
        public async Task Where_customData_float_with_specified_double_precision()
        {
            await this.Queryable
                .Where(x => (float)x.CustomData["score"] > (1.0).WithPlaces(3))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(1.000,]");
        }

        [Fact]
        public async Task Where_customData_double_with_specified_int_precision()
        {
            await this.Queryable
                .Where(x => (double)x.CustomData["score"] > 1.WithPlaces(3))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(1.000,]");
        }

        [Fact]
        public async Task Where_customData_double_with_specified_double_precision()
        {
            await this.Queryable
                .Where(x => (double)x.CustomData["score"] > (1.0).WithPlaces(3))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.score=(1.000,]");
        }
    }
}
