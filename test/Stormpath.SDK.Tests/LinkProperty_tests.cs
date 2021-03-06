﻿// <copyright file="LinkProperty_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Shared;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class LinkProperty_tests
    {
        [Fact]
        public void Comparing_compares_href()
        {
            var link1 = new LinkProperty("http://foo");
            var link2 = new LinkProperty("http://bar");

            (link1 == link2).ShouldBeFalse();
            (link1 != link2).ShouldBeTrue();
        }

        [Fact]
        public void Comparison_is_case_insensitive()
        {
            var link1 = new LinkProperty("http://foo");
            var link2 = new LinkProperty("http://bar");

            link1.ShouldBe(new LinkProperty("HTTP://FOO"));
            link2.ShouldBe(new LinkProperty("HTTP://baR"));
        }

#pragma warning disable SA1131 // Use readable conditions
        [Fact]
        public void Comparing_to_null()
        {
            LinkProperty link1 = null;
            LinkProperty link2 = new LinkProperty("http://foo");

            (link1 == null).ShouldBeTrue();

            (null == link1).ShouldBeTrue();
            link1.ShouldBeNull();

            (link2 == null).ShouldBeFalse();
            (null == link2).ShouldBeFalse();

            link2.ShouldNotBe(link1);
            link2.ShouldNotBe(null);
        }
#pragma warning restore SA1131 // Use readable conditions

        [Fact]
        public void GetHashCode_hashes_href_string()
        {
            var link = new LinkProperty("http://foo");
            int expectedHash = HashCode.Start.Hash("http://foo");

            link.GetHashCode().ShouldBe(expectedHash);
        }
    }
}
