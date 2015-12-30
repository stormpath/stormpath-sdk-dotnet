// <copyright file="IJwtParser.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// A parser that reads JWT strings and creates <see cref="IJwt"/> instances.
    /// </summary>
    public interface IJwtParser
    {
        /// <summary>
        /// Ensures that the specified <c>jti</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="jti">The JWT ID to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireId(string jti);

        /// <summary>
        /// Ensures that the specified <c>sub</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="subject">The Subject to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireSubject(string subject);

        /// <summary>
        /// Ensures that the specified <c>aud</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="audience">The Audience to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireAudience(string audience);

        /// <summary>
        /// Ensures that the specified <c>iss</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="issuer">The Issuer to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireIssuer(string issuer);

        /// <summary>
        /// Ensures that the specified <c>iat</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="issuedAt">The Issued-At time to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireIssuedAt(DateTimeOffset issuedAt);

        /// <summary>
        /// Ensures that the specified <c>exp</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="expiration">The Expiration time to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireExpiration(DateTimeOffset expiration);

        /// <summary>
        /// Ensures that the specified <c>nbf</c> exists in the parsed JWT.
        /// </summary>
        /// <param name="notBefore">The Not-Before time to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireNotBefore(DateTimeOffset notBefore);

        /// <summary>
        /// Ensures that the specified <paramref name="claimName"/> exists in the parsed JWT,
        /// and that the <paramref name="value"/> matches..
        /// </summary>
        /// <param name="claimName">The claim to expect.</param>
        /// <param name="value">The value to expect.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser RequireClaim(string claimName, object value);

        /// <summary>
        /// The signing key to use when validating the JWT signature.
        /// </summary>
        /// <param name="signingKeyString">The string representation of the signing key.</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use when getting the byte representation of the <paramref name="signingKeyString"/>.</param>
        /// <remarks>Only the HMAC SHA-256 (HS256) algorithm is currently supported.</remarks>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser SetSigningKey(string signingKeyString, Encoding encoding);

        /// <summary>
        /// The signing key to use when validating the JWT signature.
        /// </summary>
        /// <param name="signingKey">The signing key bytes.</param>
        /// <remarks>Only the HMAC SHA-256 (HS256) algorithm is currently supported.</remarks>
        /// <returns>This instance for method chaining.</returns>
        IJwtParser SetSigningKey(byte[] signingKey);

        /// <summary>
        /// Parses a JWT string and returns the decoded contents.
        /// </summary>
        /// <param name="jwt">The JWT string.</param>
        /// <exception cref="ArgumentNullException">The JWT string is empty.</exception>
        /// <exception cref="MalformedJwtException">The JWT does not consist of exactly three parts delimited by a period.</exception>
        /// <exception cref="MalformedJwtException">The JWT string is missing a body/payload.</exception>
        /// <exception cref="MalformedJwtException">The JWT is using an unsupported signing algorithm. Only HS256 is supported.</exception>
        /// <exception cref="JwtSignatureException">The signature is not valid or does not match.</exception>
        /// <exception cref="MissingClaimException">The claim was not found in the JWT.</exception>
        /// <exception cref="MismatchedClaimException">The claim value was not as expected.</exception>
        /// <returns>A new <see cref="IJwt"/> instance reflecting the parsed JWT.</returns>
        IJwt Parse(string jwt);
    }
}
