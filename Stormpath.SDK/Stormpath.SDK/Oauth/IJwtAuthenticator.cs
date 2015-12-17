// <copyright file="IJwtAuthenticator.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Represents a an <see cref="IOauthAuthenticator{TRequest, TResult}">OAuth 2.0 Authenticator</see> used to
    /// authenticate a JSON Web Token against Stormpath.
    /// </summary>
    /// <remarks>
    /// This validation is performed against the Stormpath server. If you want to validate the token locally
    /// (no network request), call the <see cref=""/> method before performing the authentication.
    /// </remarks>
    /// <example>
    /// </example>
    public interface IJwtAuthenticator : IOauthAuthenticator<IJwtAuthenticationRequest, IJwtAuthenticationResult>
    {
        /// <summary>
        /// Instructs the authenticator to use local validation rather than making a network call to the Stormpath API.
        /// </summary>
        /// <remarks>
        /// Local validation is faster as there is no network traffic involved. However, using Stormpath to validate
        /// the token ensures that the token can be validated against the state of your application and account.
        /// <para>
        /// <b>Local validation ensures</b>
        /// <list type="bullet">
        ///     <item>
        ///         <description>Token hasn't been tampered with</description>
        ///     </item>
        ///     <item>
        ///         <description>Token hasn't expired</description>
        ///     </item>
        ///     <item>
        ///         <description>Issuer is Stormpath</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Remote validation ensures</b>
        /// <list type="bullet">
        ///     <item>
        ///         <description>All criteria of local validation, plus:</description>
        ///     </item>
        ///     <item>
        ///         <description>Token hasn't been revoked</description>
        ///     </item>
        ///     <item>
        ///         <description>Account hasn't been disabled, and hasn't been deleted</description>
        ///     </item>
        ///     <item>
        ///         <description>Issuing application is still enabled, and hasn't been deleted</description>
        ///     </item>
        ///     <item>
        ///         <description>Account is still in account store for the issuing application</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns>This instance for method chaining.</returns>
        IJwtAuthenticator WithLocalValidation();
    }
}
