// <copyright file="Serialization_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Extensions.Serialization.JsonNet.Tests
{
    public class Serialization_tests
    {
        private readonly IJsonSerializer serializer;

        public Serialization_tests()
        {
            this.serializer = Serializers.Create().JsonNetSerializer().Build();
        }

        [Fact]
        public void Simple_dictionaries_are_serialized_properly()
        {
            IDictionary<string, object> properties = new Dictionary<string, object>()
            {
                { "foo", 123 },
                { "bar", "baz" }
            };
            var expectedJson = @"{""foo"":123,""bar"":""baz""}";

            var result = this.serializer.Serialize(properties);

            result.ShouldBe(expectedJson);
        }
    }
}
