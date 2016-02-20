// <copyright file="UrlEncode.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class UrlEncoding_tests
    {
        [Fact]
        public void Escapes_string()
        {
            var escaped = UrlEncoding.Encode("http://test# space 123/text?var=val&another=two");

            escaped.ShouldBe("http%3A%2F%2Ftest%23+space+123%2Ftext%3Fvar%3Dval%26another%3Dtwo");
        }

        [Fact]
        public void Canonicalizes_special_characters()
        {
            var escaped = UrlEncoding.Encode(" *~");
            var canonicalized = UrlEncoding.Encode(" *~", canonicalize: true);

            escaped.ShouldBe("+*%7E");
            canonicalized.ShouldBe("%20%2A~");
        }

        [Fact]
        public void Canononicalizes_path_correctly()
        {
            var escaped = UrlEncoding.Encode("/");
            var canonicalized = UrlEncoding.Encode("/", canonicalize: true);
            var canonicalizedWithPath = UrlEncoding.Encode("/", isPath: true, canonicalize: true);

            escaped.ShouldBe("%2F");
            canonicalized.ShouldBe("%2F");
            canonicalizedWithPath.ShouldBe("/");
        }

        [Fact]
        public void Does_not_escape_parenthesis()
        {
            var escaped = UrlEncoding.Encode("()");

            escaped.ShouldBe("()");
        }

        [Fact]
        public void Canonicalizes_parenthesis()
        {
            var canonicalized = UrlEncoding.Encode("()", canonicalize: true);

            canonicalized.ShouldBe("%28%29");
        }
    }
}
