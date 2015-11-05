// <copyright file="DefaultJwtClaimsBuilder_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Jwt;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Jwt
{
    public class DefaultJwtClaimsBuilder_tests
    {
        private readonly IJwtClaimsBuilder builder;

        public DefaultJwtClaimsBuilder_tests()
        {
            this.builder = new DefaultJwtClaimsBuilder();
        }

        private static IDictionary<string, object> GetClaims(IJwtClaimsBuilder builder)
        {
            return builder.Build().ToDictionary();
        }

        [Fact]
        public void Building_empty_jwt()
        {
            var data = GetClaims(this.builder);

            data.Any().ShouldBeFalse();
        }

        [Fact]
        public void Setting_and_removing_string_claim()
        {
            this.builder
                .SetAudience("Me!")
                .SetAudience(null);

            var data = GetClaims(this.builder);

            data.Any().ShouldBeFalse();
        }

        [Fact]
        public void Setting_and_removing_date_claim()
        {
            this.builder
                .SetExpiration(DateTimeOffset.Now)
                .SetExpiration(null);

            var data = GetClaims(this.builder);

            data.Any().ShouldBeFalse();
        }

        [Fact]
        public void Setting_all_default_claims()
        {
            var fakeExpiration = new DateTimeOffset(2016, 01, 01, 12, 30, 00, TimeSpan.Zero);
            var fakeIssuedAt = new DateTimeOffset(2015, 01, 01, 12, 30, 00, TimeSpan.Zero);
            var fakeNotBefore = new DateTimeOffset(2015, 06, 01, 12, 30, 00, TimeSpan.Zero);

            this.builder
                .SetAudience("Count Dooku")
                .SetExpiration(fakeExpiration)
                .SetId("jwt-id")
                .SetIssuedAt(fakeIssuedAt)
                .SetIssuer("Lord Sidious")
                .SetNotBeforeDate(fakeNotBefore)
                .SetSubject("Secret Plans");

            var data = GetClaims(this.builder);

            data.ShouldContain(kvp => kvp.Key == "aud" && (string)kvp.Value == "Count Dooku");
            data.ShouldContain(kvp => kvp.Key == "exp" && (DateTimeOffset)kvp.Value == fakeExpiration);
            data.ShouldContain(kvp => kvp.Key == "jti" && (string)kvp.Value == "jwt-id");
            data.ShouldContain(kvp => kvp.Key == "iat" && (DateTimeOffset)kvp.Value == fakeIssuedAt);
            data.ShouldContain(kvp => kvp.Key == "iss" && (string)kvp.Value == "Lord Sidious");
            data.ShouldContain(kvp => kvp.Key == "nbf" && (DateTimeOffset)kvp.Value == fakeNotBefore);
            data.ShouldContain(kvp => kvp.Key == "sub" && (string)kvp.Value == "Secret Plans");
        }

        [Fact]
        public void Setting_custom_claim()
        {
            this.builder
                .SetClaim("foo", "bar!");

            var data = GetClaims(this.builder);

            data.ShouldContain(kvp => kvp.Key == "foo" && (string)kvp.Value == "bar!");
        }
    }
}
