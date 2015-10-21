// <copyright file="FieldConverter_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Serialization.FieldConverters;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class FieldConverter_tests
    {
        public class BaseConverter
        {
            [Fact]
            public void Returns_null_and_false_when_expected_type_does_not_match()
            {
                var fakeAccountFieldConverter = new FuncFieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: _ => new FieldConverterResult(true, new object()),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IApplication));

                result.Success.ShouldBeFalse();
                result.Value.ShouldBeNull();
            }

            [Fact]
            public void Returns_value_when_any_type_is_supported()
            {
                var stringFieldConverter = new FuncFieldConverter(
                    t => new FieldConverterResult(true, t.Value.ToString()),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");
                var applicationTarget = Type.GetType(nameof(IApplication));

                var result = stringFieldConverter.TryConvertField(dummyField, applicationTarget);

                result.Success.ShouldBeTrue();
                result.Value.ShouldBe("bar");
            }

            [Fact]
            public void Returns_value_when_expected_type_matches()
            {
                var fakeAccountFieldConverter = new FuncFieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: _ => new FieldConverterResult(true, "good!"),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBeTrue();
                result.Value.ShouldBe("good!");
            }

            [Fact]
            public void Returns_value_when_expected_type_exists_in_list()
            {
                var fakeAccountFieldConverter = new FuncFieldConverter(
                    _ => new FieldConverterResult(true, "good!"),
                    "Test",
                    typeof(IApplication),
                    typeof(IAccount));

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBeTrue();
                result.Value.ShouldBe("good!");
            }

            [Fact]
            public void Returns_value_when_expected_type_matches_inside_collection_type()
            {
                var fakeAccountFieldConverter = new FuncFieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: _ => new FieldConverterResult(true, "good!"),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(CollectionResponsePage<IAccount>));

                result.Success.ShouldBeTrue();
                result.Value.ShouldBe("good!");
            }

            [Fact]
            public void Returns_false_when_converter_fails()
            {
                var failingConverter = new FuncFieldConverter(
                    _ => FieldConverterResult.Failed,
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = failingConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBeFalse();
                result.Value.ShouldBeNull();
            }
        }

        public class LinkPropertyConverter_tests
        {
            [Fact]
            public void Link_property_is_materialized()
            {
                var token = new KeyValuePair<string, object>(
                    "link",
                    new Dictionary<string, object>() { { "href", "http://foobar/myprop" } });
                var converter = new LinkPropertyConverter();

                var result = converter.TryConvertField(token, AbstractFieldConverter.AnyType);

                result.Value.ShouldBe(new LinkProperty("http://foobar/myprop"));
                result.Success.ShouldBeTrue();
            }
        }

        public class StatusConverters
        {
            [Fact]
            public void AccountStatus_is_materialized()
            {
                var token = new KeyValuePair<string, object>("status", "unverified");
                var converter = new StatusFieldConverters.AccountStatusConverter();

                var result = converter.TryConvertField(token, typeof(IAccount));

                result.Value.ShouldBe(AccountStatus.Unverified);
                result.Success.ShouldBeTrue();
            }

            [Fact]
            public void ApplicationStatus_is_materialized()
            {
                var token = new KeyValuePair<string, object>("status", "disabled");
                var converter = new StatusFieldConverters.ApplicationStatusConverter();

                var result = converter.TryConvertField(token, typeof(IApplication));

                result.Value.ShouldBe(ApplicationStatus.Disabled);
                result.Success.ShouldBeTrue();
            }

            [Fact]
            public void DirectoryStatus_is_materialized()
            {
                var token = new KeyValuePair<string, object>("status", "enabled");
                var converter = new StatusFieldConverters.DirectoryStatusConverter();

                var result = converter.TryConvertField(token, typeof(IDirectory));

                result.Value.ShouldBe(DirectoryStatus.Enabled);
                result.Success.ShouldBeTrue();
            }
        }
    }
}
