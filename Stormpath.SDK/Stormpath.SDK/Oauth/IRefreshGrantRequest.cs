// <copyright file="IRefreshGrantRequest.cs" company="Stormpath, Inc.">
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
    /// Represents a request for Stormpath to create a new OAuth 2.0 <see cref="IAccessToken">Access Token</see>
    /// for a previously authenticated account.
    /// </summary>
    public interface IRefreshGrantRequest : IOauthGrantRequest
    {
        /// <summary>
        /// Gets the string representation of the JSON Web Token used to generate new OAuth 2.0 access tokens.
        /// </summary>
        /// <value>The string representation of the JSON Web Token used to generate new OAuth 2.0 access tokens.</value>
        string RefreshToken { get; }
    }
}
