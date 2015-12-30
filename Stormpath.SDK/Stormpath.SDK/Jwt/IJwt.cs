// <copyright file="IJwt.cs" company="Stormpath, Inc.">
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

using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// Represents a JSON Web Token.
    /// </summary>
    public interface IJwt
    {
        /// <summary>
        /// Gets the original base64-encoded header value.
        /// </summary>
        /// <value>The original base64-encoded header value.</value>
        string Base64Header { get; }

        /// <summary>
        /// Gets the original base64-encoded payload (body) value.
        /// </summary>
        /// <value>The original base64-encoded payload (body) value.</value>
        string Base64Payload { get; }

        /// <summary>
        /// Gets the original base64-encoded digest (signature) value.
        /// </summary>
        /// <value>The original base64-encoded digest (signature) value.</value>
        string Base64Digest { get; }

        /// <summary>
        /// Gets the JWT header.
        /// </summary>
        /// <value>The JWT header.</value>
        Map Header { get; }

        /// <summary>
        /// Gets the JWT body (payload) claims.
        /// </summary>
        /// <value>The JWT claims.</value>
        IJwtClaims Body { get; }
    }
}
