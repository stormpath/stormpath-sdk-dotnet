// <copyright file="IJwtBuilder.cs" company="Stormpath, Inc.">
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

using System.Text;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// A Builder used to construct <see cref="IJwt"/> instances.
    /// </summary>
    public interface IJwtBuilder : IClaimsMutator<IJwtBuilder>
    {
        /// <summary>
        /// Sets the JWT header.
        /// </summary>
        /// <param name="header">The header key/value pairs.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtBuilder SetHeader(Map header);

        IJwtBuilder SetHeaderParameter(string name, object value);

        /// <summary>
        /// Sets the JWT claims.
        /// </summary>
        /// <param name="claims">An instance of <see cref="IJwtClaims"/> representing the claims.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtBuilder SetClaims(IJwtClaims claims);

        /// <summary>
        /// Sets the JWT claims.
        /// </summary>
        /// <param name="claims">The key/value pairs representing the claims.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtBuilder SetClaims(Map claims);

        /// <summary>
        /// Sets the key used to sign the JWT.
        /// </summary>
        /// <remarks>Only the HMAC SHA-256 (HS256) algorithm is currently supported.</remarks>
        /// <param name="signingKey">The signing key bytes.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtBuilder SignWith(byte[] signingKey);

        /// <summary>
        /// Sets the key used to sign the JWT.
        /// </summary>
        /// <remarks>Only the HMAC SHA-256 (HS256) algorithm is currently supported.</remarks>
        /// <param name="signingKeyString">The string representation of the signing key.</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use when getting the byte representation of the <paramref name="signingKeyString"/>.</param>
        /// <returns>This instance for method chaining.</returns>
        IJwtBuilder SignWith(string signingKeyString, Encoding encoding);

        /// <summary>
        /// Constructs a new <see cref="IJwt"/> instance from the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IJwt"/>.</returns>
        IJwt Build();
    }
}
