// <copyright file="InvalidJwtException.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// Represents an attempt to use a JSON Web Token with an invalid signature.
    /// </summary>
    public sealed class InvalidJwtException : ApplicationException
    {
        /// <summary>
        /// The request is invalid because the JWT parameter is missing.
        /// </summary>
        public static InvalidJwtException JwtRequired = new InvalidJwtException("JWT parameter is required.");

        /// <summary>
        /// The request is invalid because the JWT parameter is not formatted correctly.
        /// </summary>
        public static InvalidJwtException InvalidValue = new InvalidJwtException("The JWT value format is not correct.");

        /// <summary>
        /// The request is invalid because the JWT signature is invalid.
        /// </summary>
        public static InvalidJwtException SignatureError = new InvalidJwtException("The JWT signature is invalid.");

        /// <summary>
        /// The request is invalid because the JWT is expired.
        /// </summary>
        public static InvalidJwtException Expired = new InvalidJwtException("The JWT has already expired.");

        /// <summary>
        /// The request is invalid because the JWT has already been used.
        /// </summary>
        public static InvalidJwtException AlreadyUsed = new InvalidJwtException("This JWT has already been used.");

        /// <summary>
        /// The request is invalid because the response parameter is missing.
        /// </summary>
        public static InvalidJwtException ResponseMissingParameter = new InvalidJwtException("Required response parameter is missing.");

        /// <summary>
        /// The request is invalid because the signing client is different than this client.
        /// </summary>
        public static InvalidJwtException ResponseInvalidApiKeyId = new InvalidJwtException("The client used to sign the response is different than the one used in this DataStore.");

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJwtException"/> class with the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidJwtException(string message)
            : base(message)
        {
        }
    }
}
