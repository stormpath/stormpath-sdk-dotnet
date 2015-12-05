// <copyright file="IJwtSigner.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Jwt
{
    /// <summary>
    /// Signs a payload using the JSON Web Tokens (JWT) spec.
    /// </summary>
    internal interface IJwtSigner
    {
        /// <summary>
        /// Signs a payload using the JSON Web Tokens (JWT) spec.
        /// </summary>
        /// <param name="jsonPayload">The payload to sign.</param>
        /// <returns>The signed token.</returns>
        string Sign(string jsonPayload);

        /// <summary>
        /// Calculates a signature given a header and payload.
        /// </summary>
        /// <param name="base64Header">The Base-64 header.</param>
        /// <param name="base64JsonPayload">The Base-64 payload.</param>
        /// <returns>The signature.</returns>
        string CalculateSignature(string base64Header, string base64JsonPayload);
    }
}
