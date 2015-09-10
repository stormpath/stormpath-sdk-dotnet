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
using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore.FieldConverters;
using Stormpath.SDK.Impl.Resource;
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
                    convertAction: unused_ => new FieldConverterResult(true, new object()),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IApplication));

                result.Success.ShouldBe(false);
                result.Value.ShouldBe(null);
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

                result.Success.ShouldBe(true);
                result.Value.ShouldBe("bar");
            }

            [Fact]
            public void Returns_value_when_expected_type_matches()
            {
                var fakeAccountFieldConverter = new FuncFieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: unused_ => new FieldConverterResult(true, "good!"),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBe(true);
                result.Value.ShouldBe("good!");
            }

            [Fact]
            public void Returns_value_when_expected_type_matches_inside_collection_type()
            {
                var fakeAccountFieldConverter = new FuncFieldConverter(
                    appliesToTargetType: typeof(IAccount),
                    convertAction: unused_ => new FieldConverterResult(true, "good!"),
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = fakeAccountFieldConverter.TryConvertField(dummyField, typeof(CollectionResponsePage<IAccount>));

                result.Success.ShouldBe(true);
                result.Value.ShouldBe("good!");
            }

            [Fact]
            public void Returns_false_when_converter_fails()
            {
                var failingConverter = new FuncFieldConverter(
                    unused_ => FieldConverterResult.Failed,
                    converterName: "Test");

                var dummyField = new KeyValuePair<string, object>("foo", "bar");

                var result = failingConverter.TryConvertField(dummyField, typeof(IAccount));

                result.Success.ShouldBe(false);
                result.Value.ShouldBe(null);
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
                result.Success.ShouldBe(true);
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
                result.Success.ShouldBe(true);
            }

            [Fact]
            public void ApplicationStatus_is_materialized()
            {
                var token = new KeyValuePair<string, object>("status", "disabled");
                var converter = new StatusFieldConverters.ApplicationStatusConverter();

                var result = converter.TryConvertField(token, typeof(IApplication));

                result.Value.ShouldBe(ApplicationStatus.Disabled);
                result.Success.ShouldBe(true);
            }

            [Fact]
            public void DirectoryStatus_is_materialized()
            {
                var token = new KeyValuePair<string, object>("status", "enabled");
                var converter = new StatusFieldConverters.DirectoryStatusConverter();

                var result = converter.TryConvertField(token, typeof(IDirectory));

                result.Value.ShouldBe(DirectoryStatus.Enabled);
                result.Success.ShouldBe(true);
            }

            [Fact]
            public void GroupStatus_is_materialized()
            {
                var token = new KeyValuePair<string, object>("status", "disabled");
                var converter = new StatusFieldConverters.GroupStatusConverter();

                var result = converter.TryConvertField(token, typeof(IGroup));

                result.Value.ShouldBe(AccountStatus.Disabled);
                result.Success.ShouldBe(true);
            }
        }
    }
}
