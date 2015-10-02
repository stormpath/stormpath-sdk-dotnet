// <copyright file="DefaultJwtClaims.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class DefaultJwtClaims : IJwtClaims
    {
        public static readonly string Issuer = "iss";
        public static readonly string Subject = "sub";
        public static readonly string Audience = "aud";
        public static readonly string Expiration = "exp";
        public static readonly string NotBefore = "nbf";
        public static readonly string IssuedAt = "iat";
        public static readonly string Id = "jti";

        private readonly IDictionary<string, object> claims;

        public DefaultJwtClaims(IDictionary<string, object> claims)
        {
            this.claims = claims;
        }

        private T GetClaim<T>(string claim)
        {
            object value;
            if (!this.claims.TryGetValue(claim, out value) || value == null)
                return default(T);

            return (T)value;
        }

        string IJwtClaims.Audience => this.GetClaim<string>(Audience);

        DateTimeOffset IJwtClaims.Expiration => this.GetClaim<DateTimeOffset>(Expiration);

        string IJwtClaims.Id => this.GetClaim<string>(Id);

        DateTimeOffset IJwtClaims.IssuedAt => this.GetClaim<DateTimeOffset>(IssuedAt);

        string IJwtClaims.Issuer => this.GetClaim<string>(Issuer);

        DateTimeOffset IJwtClaims.NotBefore => this.GetClaim<DateTimeOffset>(NotBefore);

        string IJwtClaims.Subject => this.GetClaim<string>(Subject);

        IDictionary<string, object> IJwtClaims.ToDictionary()
            => new Dictionary<string, object>(this.claims);
    }
}
