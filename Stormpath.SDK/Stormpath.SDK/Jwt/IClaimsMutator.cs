// <copyright file="IClaimsMutator.cs" company="Stormpath, Inc.">
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
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Jwt
{
    public interface IClaimsMutator<T>
        where T : IClaimsMutator<T>
    {
        /// <summary>
        /// Sets the JWT <c>iss</c> (issuer) value. A <see langword="null"/> value will remove the property from the JSON map.
        /// </summary>
        /// <param name="iss">The JWT <c>iss</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetIssuer(string iss);

        /// <summary>
        /// Sets the JWT <c>sub</c> (subject) value. A <see langword="null"/> value will remove the property from the JSON map.
        /// </summary>
        /// <param name="sub">The JWT <c>sub</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetSubject(string sub);

        /// <summary>
        /// Sets the JWT <c>aud</c> (audience) value. A <see langword="null"/> value will remove the property from the JSON map.
        /// </summary>
        /// <param name="aud">The JWT <c>aud</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetAudience(string aud);

        /// <summary>
        /// Sets the JWT <c>exp</c> (expiration) timestamp. A <see langword="null"/> value will remove the property from the JSON map.
        /// <para>A JWT obtained after this timestamp should not be used.</para>
        /// </summary>
        /// <param name="exp">The JWT <c>exp</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetExpiration(DateTimeOffset? exp);

        /// <summary>
        /// Sets the JWT <c>nbf</c> (not before) timestamp. A <see langword="null"/> value will remove the property from the JSON map.
        /// <para>A JWT obtained before this timestamp should not be used.</para>
        /// </summary>
        /// <param name="nbf">The JWT <c>nbf</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetNotBeforeDate(DateTimeOffset? nbf);

        /// <summary>
        /// Sets the JWT <c>iat</c> (issued at) timestamp. A <see langword="null"/> value will remove the property from the JSON map.
        /// <para>The value is the timestamp when the JWT was created.</para>
        /// </summary>
        /// <param name="iat">The JWT <c>iat</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetIssuedAt(DateTimeOffset? iat);

        /// <summary>
        /// Sets the JWT <c>jti</c> (issuer) value. A <see langword="null"/> value will remove the property from the JSON map.
        /// <para>This value is a CaSe-SenSiTiVe unique identifier for the JWT. If specified, this value MUST be assigned in a
        /// manner that ensures that there is a negligible probability that the same value will be accidentally
        /// assigned to a different data object. The ID can be used to prevent the JWT from being replayed.</para>
        /// </summary>
        /// <param name="jti">The JWT <c>jti</c> value or <see langword="null"/> to remove the property from the JSON map.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetId(string jti);

        /// <summary>
        /// Sets a custom JWT Claims parameter value.  A <see langword="null"/> value will remove the property from the JSON map.
        /// </summary>
        /// <param name="claimName">The JWT Claims property name</param>
        /// <param name="value">the value to set for the specified Claims property name</param>
        /// <returns>This instance for method chaining.</returns>
        T SetClaim(string claimName, object value);
    }
}
