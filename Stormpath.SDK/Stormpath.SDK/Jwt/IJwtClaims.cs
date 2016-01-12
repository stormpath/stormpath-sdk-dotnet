// <copyright file="IJwtClaims.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// Represents the claims of a JSON Web Token.
    /// </summary>
    public interface IJwtClaims
    {
        /// <summary>
        /// Gets the issuer (iss) field.
        /// </summary>
        /// <value>The value of the issuer field.</value>
        string Issuer { get; }

        /// <summary>
        /// Gets the subject (sub) field.
        /// </summary>
        /// <value>The value of the subject field.</value>
        string Subject { get; }

        /// <summary>
        /// Gets the audience (aud) field.
        /// </summary>
        /// <value>The value of the audience field.</value>
        string Audience { get; }

        /// <summary>
        /// Gets the expiration (exp) field.
        /// </summary>
        /// <value>The value of the expiration field.</value>
        DateTimeOffset? Expiration { get; }

        /// <summary>
        /// Gets the not-before (nbf) field.
        /// </summary>
        /// <value>The value of the not-before field.</value>
        DateTimeOffset? NotBefore { get; }

        /// <summary>
        /// Gets the issued-at (IAT) claim.
        /// </summary>
        /// <value>The value of the issued-at field.</value>
        DateTimeOffset? IssuedAt { get; }

        /// <summary>
        /// Gets the JWT ID (jti) field.
        /// </summary>
        /// <value>The value of the issued-at field.</value>
        string Id { get; }

        /// <summary>
        /// Determines whether the claims collection contains the specified <paramref name="claimName"/>.
        /// </summary>
        /// <param name="claimName">The claim name.</param>
        /// <returns><see langword="true"/> if the claim exists; <see langword="false"/> otherwise.</returns>
        bool ContainsClaim(string claimName);

        /// <summary>
        /// Gets a claim from the collection.
        /// </summary>
        /// <param name="claimName">The claim name.</param>
        /// <returns>The claim value, or <see langword="null"/> if the claim does not exist.</returns>
        object GetClaim(string claimName);

        /// <summary>
        /// Builds a <see cref="IDictionary{TKey, TValue}"/> from the current JWT fields.
        /// </summary>
        /// <returns>A new <see cref="IDictionary{TKey, TValue}"/> instance.</returns>
        Map ToDictionary();
    }
}
