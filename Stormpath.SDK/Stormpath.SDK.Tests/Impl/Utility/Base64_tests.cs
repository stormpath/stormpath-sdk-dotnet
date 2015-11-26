// <copyright file="Base64_tests.cs" company="Stormpath, Inc.">
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
using System.Text;
using Shouldly;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class Base64_tests
    {
        [Fact]
        public void Decoding_throws_when_string_is_null()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                Base64.Decode(null, Encoding.UTF8);
            });
        }

        [Fact]
        public void Encoding_throws_when_string_is_null()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                Base64.Encode(null, Encoding.UTF8);
            });
        }

        [Fact]
        public void Encoding_URL_safe_throws_when_string_is_null()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                Base64.EncodeUrlSafe(null, Encoding.UTF8);
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
            Base64.Encode(plaintext, Encoding.UTF8).ShouldBe(expectedEncoded);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("foobar", "Zm9vYmFy")]
        [InlineData("tt?", "dHQ_")]
        [InlineData("tt~", "dHR-")]
        [InlineData("f", "Zg")]
        public void Encodes_UTF8_URL_safe_base64_string(string plaintext, string expectedUrlSafeEncoded)
        {
            Base64.EncodeUrlSafe(plaintext, Encoding.UTF8).ShouldBe(expectedUrlSafeEncoded);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Zm9vYmFy", "foobar")]
        public void Decodes_UTF8_base64_string(string encoded, string expectedDecoded)
        {
            Base64.Decode(encoded, Encoding.UTF8).ShouldBe(expectedDecoded);
        }

        [Theory]
        [InlineData("dHQ/", "dHQ_", "tt?")]
        [InlineData("dHR+", "dHR-", "tt~")]
        [InlineData("Zg==", "Zg", "f")]
        public void Handles_both_standard_and_URL_safe_encoding(string standardEncoding, string urlSafeEncoding, string expectedDecoded)
        {
            Base64.Decode(standardEncoding, Encoding.UTF8).ShouldBe(expectedDecoded);
            Base64.Decode(urlSafeEncoding, Encoding.UTF8).ShouldBe(expectedDecoded);
        }
    }
}
