// <copyright file="IIdSiteTokenAuthenticator.cs" company="Stormpath, Inc.">
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
    /// Represents a an <see cref="IOauthAuthenticator{TRequest, TResult}">OAuth 2.0 Authenticator</see> used to
    /// validate and exchange an ID Site response for an access token.
    /// </summary>
    /// <example>
    /// // Create a new ID Site Token Authentication request
    /// var idSiteTokenRequest = OauthRequests.NewIdSiteTokenAuthenticationRequest()
    ///     .SetJwt(id_site_jwt_response)
    ///     .Build();
    ///
    /// // Execute it against the application
    /// var idSiteTokenResult = await myApplication
    ///     .NewIdSiteTokenAuthenticator()
    ///     .AuthenticateAsync(idSiteTokenRequest);
    /// </example>
    public interface IIdSiteTokenAuthenticator : IOauthAuthenticator<IIdSiteTokenAuthenticationRequest, IOauthGrantAuthenticationResult>
    {
    }
}
