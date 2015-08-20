// <copyright file="QueryString_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Impl.Http.Support;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class QueryString_tests
    {
        [Fact]
        public void With_no_parameters()
        {
            var uri = new Uri("http://foo/bar");
            var qs = new QueryString(uri);

            qs.ToString().ShouldBe(string.Empty);
            qs.ToString(canonical: true).ShouldBe(string.Empty);
        }

        [Fact]
        public void Creating_from_string()
        {
            var qs = new QueryString("http://foo.bar/baz/?test1=2&foo=bar");

            qs.ToString().ShouldBe("foo=bar&test1=2");
        }

        [Fact]
        public void Keys_and_value_case_is_preserved()
        {
            var qs = new QueryString(new Uri("http://foo.bar/baz?TEST=foo&bar=BAZ"));

            qs.ToString().ShouldBe("bar=BAZ&TEST=foo");
        }

        [Fact]
        public void Keys_are_sorted()
        {
            var qs = new QueryString(new Uri("https://foo.bar/?zulu=5&beta=foo&alpha=9"));

            qs.ToString(canonical: true).ShouldBe("alpha=9&beta=foo&zulu=5");
        }

        [Fact]
        public void Keys_with_no_values_are_ok()
        {
            var qs = new QueryString(new Uri("https://foo.bar?foo=bar&baz=&three"));

            qs.ToString().ShouldBe("baz=&foo=bar&three=");
        }

        [Fact]
        public void Canonicalize_flag_is_observed()
        {
            var qs = new QueryString("search=start*&test=one two&tilde=~");

            qs.ToString().ShouldBe("search=start*&test=one+two&tilde=%7E");
            qs.ToString(canonical: true).ShouldBe("search=start%2A&test=one%20two&tilde=~");
        }
    }
}
