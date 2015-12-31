// <copyright file="IPasswordGrantAuthenticator.cs" company="Stormpath, Inc.">
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
    /// Represents a Password Grant-specific <see cref="IOauthAuthenticator{TRequest, TResult}">OAuth 2.0 Authenticator</see> used to
    /// authenticate an account and exchange its credentials for a valid OAuth 2.0 token.
    /// </summary>
    /// <example>
    /// // Create a new Password Grant request
    /// var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
    ///     .SetLogin("lskywalker@tattooine.rim")
    ///     .SetPassword("whataPieceofjunk$1138")
    ///     .Build();
    ///
    /// // Execute it against the application
    /// var authenticateResult = await myApplication
    ///     .NewPasswordGrantAuthenticator()
    ///     .AuthenticateAsync(passwordGrantRequest);
    /// </example>
    public interface IPasswordGrantAuthenticator : IOauthAuthenticator<IPasswordGrantRequest, IOauthGrantAuthenticationResult>
    {
    }
}
