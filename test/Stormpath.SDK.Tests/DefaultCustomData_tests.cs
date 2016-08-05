// <copyright file="DefaultCustomData_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Dynamic;
using Shouldly;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class DefaultCustomData_tests
    {
        private readonly IInternalDataStore dataStore;

        public DefaultCustomData_tests()
        {
            this.dataStore = TestDataStore.Create();
        }

        public static IEnumerable<object[]> ValidTypes()
        {
            yield return new object[] { short.MinValue };
            yield return new object[] { int.MaxValue };
            yield return new object[] { long.MinValue };
            yield return new object[] { float.MaxValue };
            yield return new object[] { double.MinValue };
            yield return new object[] { decimal.MaxValue };
            yield return new object[] { byte.MinValue };
            yield return new object[] { true };
            yield return new object[] { "foobar" };
            yield return new object[] { 'x' };
            yield return new object[] { new string[] { "foo", "bar" } };
            yield return new object[] { new SimplePoco { Foo = "stuff", Bar = 123} };
            yield return new object[] { new { SomeString = "anonymous ftw!", bar = 456 }};
        }

        public static IEnumerable<object[]> InvalidTypes()
        {
            yield return new[] { new object() };
        }

        private ICustomData GetInstance(IDictionary<string, object> properties = null)
        {
            return this.dataStore.InstantiateWithData<ICustomData>(properties);
        }

        [Fact]
        public void Get_returns_null_for_unknown_key()
        {
            var customData = this.GetInstance();

            customData.Get("foo").ShouldBeNull();
        }

        [Fact]
        public void Get_after_removing_returns_null()
        {
            var customData = this.GetInstance();

            customData.Put("foo", "bar");
            customData.Remove("foo");

            customData.Get("foo").ShouldBe(null);
        }

        [Fact]
        public void Get_iso8601_as_DateTimeOffset()
        {
            var customData = this.GetInstance();

            customData.Put("lastLogin", "2016-01-01T12:45:05.123-07:00");

            var dto = customData.Get<DateTimeOffset>("lastLogin");
            dto.Year.ShouldBe(2016);
            dto.Month.ShouldBe(1);
            dto.Day.ShouldBe(1);
            dto.Hour.ShouldBe(12);
            dto.Minute.ShouldBe(45);
            dto.Second.ShouldBe(5);
            dto.Millisecond.ShouldBe(123);
            dto.Offset.ShouldBe(TimeSpan.FromHours(-7));
        }

        [Fact]
        public void Get_iso8601_duration_as_TimeSpan()
        {
            var customData = this.GetInstance();

            customData.Put("lastSession", "PT39M5S");

            var ts = customData.Get<TimeSpan>("lastSession");
            ts.Minutes.ShouldBe(39);
            ts.Seconds.ShouldBe(5);
            ts.TotalSeconds.ShouldBe(2345);
        }

        [Fact]
        public void Get_throws_for_DateTime()
        {
            var customData = this.GetInstance();

            // We want people to use DateTimeOffset!
            Should.Throw<Exception>(() => customData.Get<DateTime>("lastLogin"));
        }

        [Fact]
        public void Get_guid_as_guid()
        {
            var customData = this.GetInstance();
            var dummyId = Guid.NewGuid();

            customData.Put("id", dummyId);

            customData.Get<Guid>("id").ShouldBe(dummyId);
        }

        [Fact]
        public void Put_single_item()
        {
            var customData = this.GetInstance();

            customData.Put("foo", "bar");

            customData.Get("foo").ShouldBe("bar");
        }

        [Fact]
        public void Put_single_item_with_indexer()
        {
            var customData = this.GetInstance();

            customData["foo"] = 123;

            customData["foo"].ShouldBe(123);
        }

        [Fact]
        public void Put_KeyValuePair_item()
        {
            var customData = this.GetInstance();
            var newItem = new KeyValuePair<string, object>("foo", 987);

            customData.Put(newItem);

            customData.Get("foo").ShouldBe(987);
        }

        [Fact]
        public void Put_multiple_items()
        {
            var customData = this.GetInstance();
            var newItems = new Dictionary<string, object>()
            {
                { "foo", "bar" }, { "baz", 123 }
            };

            customData.Put(newItems);

            customData["foo"].ShouldBe("bar");
            customData["baz"].ShouldBe(123);
        }

        [Fact]
        public void Put_anonymous_items()
        {
            var customData = this.GetInstance();

            customData.Put(new { foo = "bar", baz = 123 });

            customData["foo"].ShouldBe("bar");
            customData["baz"].ShouldBe(123);
        }

        [Fact]
        public void Put_dynamic_items()
        {
            var customData = this.GetInstance();

            dynamic data = new ExpandoObject();
            data.foo = "bar";
            data.baz = 123;

            customData.Put(data);

            customData["foo"].ShouldBe("bar");
            customData["baz"].ShouldBe(123);
        }

        [Fact]
        public void Put_array_of_supported_items()
        {
            var customData = this.GetInstance();

            customData.Put("foobars", new string[] { "foo", "bar" });

            customData.Get<IEnumerable<string>>("foobars").ShouldBeSubsetOf(new string[] { "foo", "bar" });
        }

        [Fact]
        public void Put_list_of_supported_items()
        {
            var customData = this.GetInstance();

            customData.Put("foobars", new List<string>() { "foo", "bar" });

            customData.Get<IEnumerable<string>>("foobars").ShouldBeSubsetOf(new string[] { "foo", "bar" });
        }

        [Fact]
        public void Put_DateTimeOffset_as_iso8601_string()
        {
            var customData = this.GetInstance();

            customData.Put("lastLogin", new DateTimeOffset(2016, 1, 1, 12, 45, 5, 123, TimeSpan.FromHours(-7)));

            customData.Get("lastLogin").ShouldBe("2016-01-01T12:45:05.123-07:00");
        }

        [Fact]
        public void Put_DateTime_as_iso8601_string()
        {
            var customData = this.GetInstance();

            customData.Put("lastLogin", new DateTime(2016, 1, 1, 12, 45, 5, 123, DateTimeKind.Utc));

            customData.Get("lastLogin").ShouldBe("2016-01-01T12:45:05.123Z");
        }

        [Fact]
        public void Put_TimeSpan_as_iso8601_string()
        {
            var customData = this.GetInstance();

            customData.Put("lastSession", TimeSpan.FromMinutes(39).Add(TimeSpan.FromSeconds(5)));

            customData.Get("lastSession").ShouldBe("PT39M5S");
        }

        [Fact]
        public void Put_guid_stores_as_string()
        {
            var customData = this.GetInstance();
            var dummyId = Guid.NewGuid();

            customData.Put("id", dummyId);

            customData.Get("id").ShouldBe(dummyId.ToString());
        }

        [Theory]
        [InlineData("href")]
        [InlineData("createdAt")]
        [InlineData("modifiedAt")]
        public void Put_throws_for_reserved_key_name(string key)
        {
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(() => customData.Put(key, "quz"));
        }

        [Theory]
        [InlineData("foo&bar")]
        [InlineData("-test")]
        public void Put_throws_for_invalid_key_name(string key)
        {
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(() => customData.Put(key, "quz"));
        }

        [Theory]
        [MemberData(nameof(ValidTypes))]
        public void Put_accepts_valid_primitives(object value)
        {
            var customData = this.GetInstance();

            customData.Put("testing", value);

            customData["testing"].ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(InvalidTypes))]
        public void Put_throws_for_invalid_primitives(object value)
        {
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(
                () => customData.Put("bad", value), $"This should not be allowed in customData: {value}");
        }

        [Theory]
        [MemberData(nameof(ValidTypes))]
        public void Put_accepts_primitives_in_keyValue_pairs(object value)
        {
            var customData = this.GetInstance();

            customData.Put(new KeyValuePair<string, object>("testing", value));

            customData["testing"].ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(InvalidTypes))]
        public void Put_throws_for_invalid_primitives_in_keyValue_pairs(object value)
        {
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(
                () => customData.Put(new KeyValuePair<string, object>("bad", value)), $"This should not be allowed in customData: {value}");
        }

        [Theory]
        [MemberData(nameof(ValidTypes))]
        public void Put_accepts_primitives_in_dictionary(object value)
        {
            var customData = this.GetInstance();

            customData.Put(new Dictionary<string, object>() { ["testing"] = value });

            customData["testing"].ShouldBe(value);
        }

        [Theory]
        [MemberData(nameof(InvalidTypes))]
        public void Put_throws_for_invalid_primitives_in_dictionary(object value)
        {
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                customData.Put(new Dictionary<string, object>() { [value.GetType().Name] = value });
            });

            customData.IsEmptyOrDefault().ShouldBeTrue();
        }

        [Fact]
        public void Put_accepts_primitives_in_anonymous_type()
        {
            var validValueTypesAnon = new
            {
                aShort = short.MinValue,
                aInt = int.MaxValue,
                aLong = long.MinValue,
                aFloat = float.MaxValue,
                aDouble = double.MinValue,
                aDecimal = decimal.MaxValue,
                aByte = byte.MinValue,
                aBool = true,
                aString = "foobar",
                aChar = 'x'
            };
            var customData = this.GetInstance();

            customData.Put(validValueTypesAnon);

            customData.Count.ShouldBe(11); // 10 properties in the anonymous type, plus href field
        }

        [Fact]
        public void Put_throws_for_invalid_primitives_in_anonymous_type()
        {
            var invalidValueTypesAnon = new
            {
                aObject = new object(),
                aGuid = Guid.NewGuid(),
                aStringBuilder = new System.Text.StringBuilder("foobar!"),
                aLazyBool = new Lazy<bool>(() => false),
                aDictionary = new Dictionary<int, bool>()
                {
                    [123] = true
                }
            };
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                customData.Put(invalidValueTypesAnon);
            });

            customData.IsEmptyOrDefault().ShouldBeTrue();
        }

        [Fact]
        public void ContainsKey_after_put_is_true()
        {
            var customData = this.GetInstance();

            customData.Put("bar", "baz");

            customData.ContainsKey("bar").ShouldBeTrue();
        }

        [Theory]
        [InlineData("href")]
        [InlineData("createdAt")]
        [InlineData("modifiedAt")]
        public void Remove_throws_for_reserved_key_names(string key)
        {
            var customData = this.GetInstance();

            Should.Throw<ArgumentOutOfRangeException>(() => customData.Remove(key));
        }

        [Fact]
        public void TryGetValue_returns_true_for_existent_key()
        {
            var customData = this.GetInstance();

            customData.Put("foo", 123);

            object value;
            customData.TryGetValue("foo", out value).ShouldBeTrue();
        }

        [Fact]
        public void TryGetValue_returns_false_for_nonexistent_key()
        {
            var customData = this.GetInstance();

            object value;
            customData.TryGetValue("nope", out value).ShouldBeFalse();
        }

        [Fact]
        public void IsEmptyOrDefault_ignores_reserved_keys()
        {
            var instanceData = new Dictionary<string, object>()
            {
                { "href", "http://foo/bar" },
                { "createdAt", DateTimeOffset.UtcNow },
                { "modifiedAt", DateTimeOffset.UtcNow }
            };
            var customData = this.GetInstance(instanceData);

            customData.IsEmptyOrDefault().ShouldBeTrue();
        }

        [Fact]
        public void IsEmptyOrDefault_returns_false_for_nonreserved_keys()
        {
            var instanceData = new Dictionary<string, object>()
            {
                { "myStuff", "foobarbaz" },
            };
            var customData = this.GetInstance(instanceData);

            customData.IsEmptyOrDefault().ShouldBeFalse();
        }

        [Fact]
        public void Count_ignores_pending_deleted_items()
        {
            var instanceData = new Dictionary<string, object>()
            {
                { "foo", "bar" },
            };

            var customData = this.GetInstance(instanceData);
            customData.Count.ShouldBe(2);

            customData.Remove("foo");
            customData.Count.ShouldBe(1);
        }

        [Fact]
        public void Count_includes_pending_items()
        {
            var instanceData = new Dictionary<string, object>()
            {
                { "foo", "bar" },
            };
            var customData = this.GetInstance(instanceData);
            customData.Count.ShouldBe(2);

            customData.Put("baz", 123);
            customData.Count.ShouldBe(3);
        }

        [Fact]
        public void Iterating_over_collection()
        {
            var instanceData = new Dictionary<string, object>()
            {
                { "href", "http://foo/bar" },
                { "createdAt", DateTimeOffset.UtcNow },
                { "modifiedAt", DateTimeOffset.UtcNow }
            };
            var customData = this.GetInstance(instanceData);

            var breadcrumbs = new List<string>();
            foreach (var item in customData)
            {
                breadcrumbs.Add(item.Key);
            }

            breadcrumbs.ShouldContain("href");
            breadcrumbs.ShouldContain("createdAt");
            breadcrumbs.ShouldContain("modifiedAt");
            breadcrumbs.Count.ShouldBe(3);
        }
    }
}
