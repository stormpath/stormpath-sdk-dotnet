// <copyright file="IClaimsMutator.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// Represents common actions that set JWT claims.
    /// </summary>
    /// <typeparam name="T">The type containing claims..</typeparam>
    public interface IClaimsMutator<T>
        where T : IClaimsMutator<T>
    {
        /// <summary>
        /// Sets the Issuer (<c>iss</c>) claim. A <see langword="null"/> value will remove the claim.
        /// </summary>
        /// <param name="iss">The JWT <c>iss</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetIssuer(string iss);

        /// <summary>
        /// Sets the Subject (<c>sub</c>) claim. A <see langword="null"/> value will remove the claim.
        /// </summary>
        /// <param name="sub">The JWT <c>sub</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetSubject(string sub);

        /// <summary>
        /// Sets the Audience (<c>aud</c>) claim. A <see langword="null"/> value will remove the claim.
        /// </summary>
        /// <param name="aud">The JWT <c>aud</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetAudience(string aud);

        /// <summary>
        /// Sets the Expiration Time (<c>exp</c>) claim. A <see langword="null"/> value will remove the claim.
        /// <para>A JWT obtained after this timestamp should not be used.</para>
        /// </summary>
        /// <param name="exp">The JWT <c>exp</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetExpiration(DateTimeOffset? exp);

        /// <summary>
        /// Sets the Not Before (<c>nbf</c>) claim. A <see langword="null"/> value will remove the claim.
        /// <para>A JWT obtained before this timestamp should not be used.</para>
        /// </summary>
        /// <param name="nbf">The JWT <c>nbf</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetNotBeforeDate(DateTimeOffset? nbf);

        /// <summary>
        /// Sets the Issued At (<c>iat</c>) claim. A <see langword="null"/> value will remove the claim.
        /// <para>The value is the timestamp when the JWT was created.</para>
        /// </summary>
        /// <param name="iat">The JWT <c>iat</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetIssuedAt(DateTimeOffset? iat);

        /// <summary>
        /// Sets the JWT ID (<c>jti</c>) claim value. A <see langword="null"/> value will remove the claim.
        /// </summary>
        /// <remarks>This value is a CaSe-SenSiTiVe unique identifier for the JWT. If specified, this value MUST be assigned in a
        /// manner that ensures that there is a negligible probability that the same value will be accidentally
        /// assigned to a different data object. The ID can be used to prevent the JWT from being replayed.</remarks>
        /// <param name="jti">The JWT <c>jti</c> claim value or <see langword="null"/> to remove the claim.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetId(string jti);

        /// <summary>
        /// Sets a custom claim value.  A <see langword="null"/> value will remove the claim.
        /// </summary>
        /// <param name="claimName">The claim name.</param>
        /// <param name="value">The claim value.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetClaim(string claimName, object value);
    }
}
