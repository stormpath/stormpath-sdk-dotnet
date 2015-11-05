// <copyright file="String_ToBase64.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Impl.Extensions;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Extensions
{
    public class String_ToBase64_tests
    {
        [Fact]
        public void Throws_when_string_is_null()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                ((string)null).ToBase64(System.Text.Encoding.UTF8);
            });
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("foobar", "Zm9vYmFy")]
        [InlineData("tt?", "dHQ/")] // See String_ToUrlSafeBase64_tests
        [InlineData("tt~", "dHR+")] // See String_ToUrlSafeBase64_tests
        [InlineData("f", "Zg==")] // See String_ToUrlSafeBase64_tests
        public void Encodes_UTF8_base64_string(string plaintext, string expectedEncoded)
        {
            plaintext.ToBase64(System.Text.Encoding.UTF8).ShouldBe(expectedEncoded);
        }
    }
}
