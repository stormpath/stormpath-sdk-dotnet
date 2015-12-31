// <copyright file="IRefreshGrantRequestBuilder.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Builder pattern used to construct <see cref="IRefreshGrantRequest"/> instances.
    /// </summary>
    public interface IRefreshGrantRequestBuilder : IOauthAuthenticationRequestBuilder<IRefreshGrantRequest>
    {
        /// <summary>
        /// Sets the <see cref="IRefreshToken">Refresh Token</see> string (JWT) that will be used to generate a new <see cref="IAccessToken">Access Token</see>..
        /// </summary>
        /// <param name="refreshTokenString">The string representation of the Refresh Token.</param>
        /// <returns>This instance for method chaining.</returns>
        IRefreshGrantRequestBuilder SetRefreshToken(string refreshTokenString);

        /// <summary>
        /// Sets the <see cref="IRefreshToken">Refresh Token</see> that will be used to generate a new <see cref="IAccessToken">Access Token</see>.
        /// </summary>
        /// <param name="refreshToken">The Refresh Token.</param>
        /// <returns>This instance for method chaining.</returns>
        IRefreshGrantRequestBuilder SetRefreshToken(IRefreshToken refreshToken);
    }
}
