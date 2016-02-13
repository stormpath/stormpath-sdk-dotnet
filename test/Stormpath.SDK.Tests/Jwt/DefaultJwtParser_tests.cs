// <copyright file="DefaultJwtParser_tests.cs" company="Stormpath, Inc.">
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
using System.Text;
using Shouldly;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.Jwt
{
    public class DefaultJwtParser_tests
    {
        private readonly Random random;

        public DefaultJwtParser_tests()
        {
            this.random = new Random();
        }

        private byte[] RandomKey()
        {
            var key = new byte[64];
            this.random.NextBytes(key);

            return key;
        }

        private static IJwtBuilder GetBuilder(byte[] key)
        {
            IJwtBuilder builder = new DefaultJwtBuilder(Serializers.Create().JsonNetSerializer().Build());
            builder.SignWith(key);

            return builder;
        }

        private static IJwtParser GetParser(byte[] key)
        {
            IJwtParser parser = new DefaultJwtParser(Serializers.Create().JsonNetSerializer().Build());
            parser.SetSigningKey(key);

            return parser;
        }

        [Fact]
        public void Throws_when_parsing_junk_argument()
        {
            var junkPayload = "{;aklsjd;fkajsd;fkjasd;lfkj}";
            var badJwt = $"{Base64.EncodeUrlSafe(@"{""alg"":""none""}", Encoding.UTF8)}.{Base64.EncodeUrlSafe(junkPayload, Encoding.UTF8)}.";

            var parser = GetParser(null);

            Should.Throw<MalformedJwtException>(() => parser.Parse(badJwt));
        }

        [Fact]
        public void Throws_for_unsupported_algorithm()
        {
            var badAlgorithmName = "whatever";
            var header = @"{""alg"":""" + badAlgorithmName + @"""}";
            var payload = @"{""subject"":""Joe""}";
            var badJwt = $"{Base64.EncodeUrlSafe(header, Encoding.UTF8)}.{Base64.EncodeUrlSafe(payload, Encoding.UTF8)}.";

            var parser = GetParser(null);

            Should.Throw<MalformedJwtException>(() => parser.Parse(badJwt));
        }

        [Fact]
        public void Throws_for_bad_signature()
        {
            var header = @"{""alg"":""HS256""}";
            var payload = @"{""subject"":""Joe""}";
            var badSignature = ";aklsjdf;kajsd;fkjas;dklfj";
            var badJwt = $"{Base64.EncodeUrlSafe(header, Encoding.UTF8)}.{Base64.EncodeUrlSafe(payload, Encoding.UTF8)}.{badSignature}";

            var parser = GetParser(this.RandomKey());

            Should.Throw<JwtSignatureException>(() => parser.Parse(badJwt));
        }

        [Fact]
        public void Verifies_claim_exists()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetAudience("Foo!");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireClaim("aud", "Foo!");

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_when_expected_claim_is_mismatched()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetAudience("Bar?");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireClaim("aud", "Foo!");

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_when_expected_claim_is_missing()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("Bar?");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireClaim("aud", "Foo!");

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_when_expected_claim_name_is_null()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetAudience("Bar?");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);

            Should.Throw<ArgumentNullException>(() => parser.RequireClaim(null, "Bar?"));
        }

        [Fact]
        public void Throws_for_missing_audience()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireAudience("Fail");

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_audience()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetAudience("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireAudience("Fail");

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Verifies_audience_claim()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetAudience("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireAudience("UnitTesting");

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_expiration()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireExpiration(DateTimeOffset.Now);

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_expiration()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetExpiration(DateTimeOffset.Now.AddHours(1));
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireExpiration(DateTimeOffset.Now);

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Verifies_expiration_claim()
        {
            var key = this.RandomKey();
            var fakeDate = DateTimeOffset.Now.AddDays(1);

            var builder = GetBuilder(key);
            builder.SetExpiration(fakeDate);
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireExpiration(fakeDate);

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_id()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireId("Fail");

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_id()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetId("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireId("Fail");

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Verifies_id_claim()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetId("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireId("UnitTesting");

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_issuedAt()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireIssuedAt(DateTimeOffset.Now);

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        public void Throws_for_mismatched_issuedAt()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetIssuedAt(DateTimeOffset.Now.AddHours(-1));
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireIssuedAt(DateTimeOffset.Now);

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        public void Verifies_issuedAt_claim()
        {
            var key = this.RandomKey();
            var fakeDate = DateTimeOffset.Now.AddDays(1);

            var builder = GetBuilder(key);
            builder.SetIssuedAt(fakeDate);
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireIssuedAt(fakeDate);

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_issuer()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireIssuer("Fail");

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_issuer()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetIssuer("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireIssuer("Fail");

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Verifies_issuer_claim()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetIssuer("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireIssuer("UnitTesting");

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_notBefore()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireNotBefore(DateTimeOffset.Now);

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_notBefore()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetNotBeforeDate(DateTimeOffset.Now.AddHours(-1));
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireNotBefore(DateTimeOffset.Now);

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        public void Verifies_notBefore_claim()
        {
            var key = this.RandomKey();
            var fakeDate = DateTimeOffset.Now.AddDays(-1);

            var builder = GetBuilder(key);
            builder.SetNotBeforeDate(fakeDate);
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireNotBefore(fakeDate);

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_subject()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetIssuer("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireSubject("Fail");

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_subject()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireSubject("Fail");

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Verifies_subject_claim()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireSubject("UnitTesting");

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_missing_custom_date_claim()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetSubject("UnitTesting");
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireClaim("aDate", DateTimeOffset.Now);

            Should.Throw<MissingClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_for_mismatched_custom_date_claim()
        {
            var key = this.RandomKey();

            var builder = GetBuilder(key);
            builder.SetClaim("aDate", DateTimeOffset.Now.AddHours(-1));
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireClaim("aDate", DateTimeOffset.Now);

            Should.Throw<MismatchedClaimException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Verifies_custom_date_claim()
        {
            var key = this.RandomKey();
            var fakeDate = DateTimeOffset.Now.AddDays(1);

            var builder = GetBuilder(key);
            builder.SetClaim("aDate", fakeDate);
            var jwt = builder.Build().ToString();

            var parser = GetParser(key);
            parser.RequireClaim("aDate", fakeDate);

            Should.NotThrow(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_when_expired()
        {
            var key = this.RandomKey();

            var jwt = GetBuilder(key)
                .SetSubject("UnitTesting")
                .SetExpiration(DateTimeOffset.Now.AddSeconds(-1))
                .Build()
                .ToString();

            var parser = GetParser(key);

            Should.Throw<ExpiredJwtException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_when_premature()
        {
            var key = this.RandomKey();

            var jwt = GetBuilder(key)
                .SetSubject("UnitTesting")
                .SetNotBeforeDate(DateTimeOffset.Now.AddMinutes(1))
                .Build()
                .ToString();

            var parser = GetParser(key);

            Should.Throw<PrematureJwtException>(() => parser.Parse(jwt));
        }

        [Fact]
        public void Throws_when_signature_does_not_match()
        {
            var jwt = GetBuilder(this.RandomKey())
                .SetSubject("UnitTesting")
                .Build()
                .ToString();

            var parser = GetParser(this.RandomKey());

            Should.Throw<JwtSignatureException>(() => parser.Parse(jwt));
        }
    }
}
