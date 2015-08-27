// <copyright file="HttpHeaders_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Http;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class HttpHeaders_tests
    {
        [Fact]
        public void Getting_nonexistent_header_returns_default()
        {
            var headers = new HttpHeaders();

            var nonexistent = headers.GetFirst<string>("nope");

            nonexistent.ShouldBe(null);
        }

        [Fact]
        public void Adding_nonstandard_header_name_is_ok()
        {
            var headers = new HttpHeaders();

            headers.Add("FOOBAR", "123");

            headers.GetFirst<string>("FOOBAR").ShouldBe("123");
        }
    }
}
