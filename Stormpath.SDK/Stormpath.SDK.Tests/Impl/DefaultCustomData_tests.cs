// <copyright file="DefaultCustomData_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.CustomData;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultCustomData_tests
    {
        private static ICustomData GetInstance(IDictionary<string, object> properties = null)
        {
            if (properties == null)
                properties = new Dictionary<string, object>();

            var customData = new DefaultCustomData(new FakeDataStore<ICustomData>());
            customData.ResetAndUpdate(properties);

            return customData;
        }

        [Fact]
        public void Get_returns_null_for_unknown_key()
        {
            var customData = GetInstance();

            customData.Get("foo").ShouldBeNull();
        }

        [Fact]
        public void Get_after_removing_returns_null()
        {
            var customData = GetInstance();

            customData.Put("foo", "bar");
            customData.Remove("foo");

            customData.Get("foo").ShouldBe(null);
        }

        [Fact]
        public void Put_single_item()
        {
            var customData = GetInstance();

            customData.Put("foo", "bar");

            customData.Get("foo").ShouldBe("bar");
        }

        [Fact]
        public void Put_single_item_with_indexer()
        {
            var customData = GetInstance();

            customData["foo"] = 123;

            customData["foo"].ShouldBe(123);
        }

        [Fact]
        public void Put_KeyValuePair_item()
        {
            var customData = GetInstance();
            var newItem = new KeyValuePair<string, object>("foo", 987);

            customData.Put(newItem);

            customData.Get("foo").ShouldBe(987);
        }

        [Fact]
        public void Put_multiple_items()
        {
            var customData = GetInstance();
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
            var customData = GetInstance();

            customData.Put(new { foo = "bar", baz = 123 });

            customData["foo"].ShouldBe("bar");
            customData["baz"].ShouldBe(123);
        }

        [Fact]
        public void Put_throws_for_reserved_key_names()
        {
            var customData = GetInstance();

            var reservedNames = new List<string>()
            {
                "href", "createdAt", "modifiedAt",
                "meta", "spMeta", "spmeta", "ionmeta", "ionMeta"
            };

            reservedNames.ForEach(x =>
            {
                Should.Throw<ArgumentOutOfRangeException>(() => customData.Put(x, "quz"));
            });
        }

        [Fact]
        public void Put_throws_for_invalid_key_names()
        {
            var customData = GetInstance();

            var invalidNames = new List<string>()
            {
                "foo&bar", "-test",
            };

            invalidNames.ForEach(x =>
            {
                Should.Throw<ArgumentOutOfRangeException>(() => customData.Put(x, "quz"));
            });
        }

        [Fact]
        public void ContainsKey_after_put_is_true()
        {
            var customData = GetInstance();

            customData.Put("bar", "baz");

            customData.ContainsKey("bar").ShouldBeTrue();
        }

        [Fact]
        public void Remove_throws_for_reserved_key_names()
        {
            var customData = GetInstance();

            var reservedNames = new List<string>()
            {
                "href", "createdAt", "modifiedAt",
            };

            reservedNames.ForEach(x =>
            {
                Should.Throw<ArgumentOutOfRangeException>(() => customData.Remove(x));
            });
        }

        [Fact]
        public void TryGetValue_returns_true_for_existent_key()
        {
            var customData = GetInstance();

            customData.Put("foo", 123);

            object value;
            customData.TryGetValue("foo", out value).ShouldBeTrue();
        }

        [Fact]
        public void TryGetValue_returns_false_for_nonexistent_key()
        {
            var customData = GetInstance();

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
            var customData = GetInstance(instanceData);

            customData.IsEmptyOrDefault().ShouldBeTrue();
        }

        [Fact]
        public void IsEmptyOrDefault_returns_false_for_nonreserved_keys()
        {
            var instanceData = new Dictionary<string, object>()
            {
                { "myStuff", "foobarbaz" },
            };
            var customData = GetInstance(instanceData);

            customData.IsEmptyOrDefault().ShouldBeFalse();
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
            var customData = GetInstance(instanceData);

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
