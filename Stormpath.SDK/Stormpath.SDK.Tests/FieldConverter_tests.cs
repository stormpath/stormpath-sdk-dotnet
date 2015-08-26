// <copyright file="FieldConverter_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.DataStore.FieldConverters;
using Stormpath.SDK.Impl.Resource;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class FieldConverter_tests
    {
        public class BaseConverter
        {
            [Fact]
            public void Returns_null_and_false_when_expected_type_does_not_match()
            {
                var fakeAccountFieldConverter = new FieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: _ => new ConverterResult(true, new object()));
                var dummyField = new JProperty("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IApplication));

                result.Success.ShouldBe(false);
                result.Result.ShouldBe(null);
            }

            [Fact]
            public void Returns_value_when_any_type_is_supported()
            {
                var stringFieldConverter = new FieldConverter(t => new ConverterResult(true, t.First.ToString()));
                var dummyField = new JProperty("foo", "bar");
                var applicationTarget = Type.GetType(nameof(IApplication));

                var result = stringFieldConverter.TryConvertField(dummyField, applicationTarget);

                result.Success.ShouldBe(true);
                result.Result.ShouldBe("bar");
            }

            [Fact]
            public void Returns_value_when_expected_type_matches()
            {
                var fakeAccountFieldConverter = new FieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: _ => new ConverterResult(true, "good!"));
                var dummyField = new JProperty("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBe(true);
                result.Result.ShouldBe("good!");
            }

            [Fact]
            public void Returns_false_when_converter_fails()
            {
                var failingConverter = new FieldConverter(_ => ConverterResult.Failed);
                var dummyField = new JProperty("foo", "bar");

                var result = failingConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBe(false);
                result.Result.ShouldBe(null);
            }
        }

        public class LinkPropertyConverter
        {
            [Fact]
            public void Link_property_is_materialized()
            {
                var jsonObject = JObject.Parse(@"{ link: { href: ""http://foobar/myprop"" } }");
                var converter = DefaultFieldConverters.LinkPropertyConverter;

                var result = converter.TryConvertField(jsonObject.Properties().First(), FieldConverter.AnyType);

                result.Result.ShouldBe(new LinkProperty("http://foobar/myprop"));
                result.Success.ShouldBe(true);
            }
        }
    }
}
