// <copyright file="Jwt_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Text;
using Shouldly;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Jwt
{
    public class Jwt_tests
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                new Dictionary<string, object>() { ["FirstName"] = "Bob", ["Age"] = 37 },
                "ABC",
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJGaXJzdE5hbWUiOiJCb2IiLCJBZ2UiOjM3fQ.cr0xw8c_HKzhFBMQrseSPGoJ0NPlRp_3BKzP96jwBdY"
            };

            yield return new object[]
            {
                new Dictionary<string, object>() { ["sub"] = "1234567890", ["name"] = "John Doe", ["admin"] = true },
                "secret",
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.pcHcZspUvuiqIPVB_i_qmcvCJv63KLUgIAKIlXI1gY8"
            };
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void When_encoding(IDictionary<string, object> payload, string signingKey, string expected)
        {
            IJwtBuilder builder = new DefaultJwtBuilder(Serializers.Create().JsonNetSerializer().Build());

            var jwt = builder
                .SetClaims(payload)
                .SignWith(signingKey, Encoding.UTF8)
                .Build();

            jwt.ToString().ShouldBe(expected);
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void When_decoding(IDictionary<string, object> expectedPayload, string signingKey, string jwt)
        {
            IJwtParser parser = new DefaultJwtParser(Serializers.Create().JsonNetSerializer().Build());

            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            var decoded = parser
                .SetSigningKey(signingKeyBytes)
                .Parse(jwt);

            decoded.Body.ToDictionary().ShouldBe(expectedPayload);
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void When_verifying(IDictionary<string, object> ignored, string signingKey, string jwt)
        {
            IJwtParser parser = new DefaultJwtParser(Serializers.Create().JsonNetSerializer().Build());

            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            var decoded = parser
                .SetSigningKey(signingKeyBytes)
                .Parse(jwt);

            var validator = new JwtSignatureValidator(signingKeyBytes);

            validator.IsValid(decoded).ShouldBeTrue();
        }
    }
}
