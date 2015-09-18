// <copyright file="RequestHelper_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class RequestHelper_tests
    {
        public class UrlEncode
        {
            [Fact]
            public void Escapes_string()
            {
                var escaped = RequestHelper.UrlEncode("http://test# space 123/text?var=val&another=two");

                escaped.ShouldBe("http%3A%2F%2Ftest%23+space+123%2Ftext%3Fvar%3Dval%26another%3Dtwo");
            }

            [Fact]
            public void Canonicalizes_special_characters()
            {
                var escaped = RequestHelper.UrlEncode(" *~");
                var canonicalized = RequestHelper.UrlEncode(" *~", canonicalize: true);

                escaped.ShouldBe("+*%7E");
                canonicalized.ShouldBe("%20%2A~");
            }

            [Fact]
            public void Canononicalizes_path_correctly()
            {
                var escaped = RequestHelper.UrlEncode("/");
                var canonicalized = RequestHelper.UrlEncode("/", canonicalize: true);
                var canonicalizedWithPath = RequestHelper.UrlEncode("/", isPath: true, canonicalize: true);

                escaped.ShouldBe("%2F");
                canonicalized.ShouldBe("%2F");
                canonicalizedWithPath.ShouldBe("/");
            }

            [Fact]
            public void Does_not_escape_parenthesis()
            {
                var escaped = RequestHelper.UrlEncode("()");

                escaped.ShouldBe("()");
            }

            [Fact]
            public void Canonicalizes_parenthesis()
            {
                var canonicalized = RequestHelper.UrlEncode("()", canonicalize: true);

                canonicalized.ShouldBe("%28%29");
            }
        }
    }
}
