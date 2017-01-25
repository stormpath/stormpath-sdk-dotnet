// <copyright file="IOauth2Provider.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Generic OAuth 2.0 <see cref="IProvider">Provider</see> Resource.
    /// </summary>
    public interface IOauth2Provider : IProvider
    {
        /// <summary>
        /// Gets the access token type used by this provider.
        /// </summary>
        /// <value>The access token type.</value>
        string AccessTokenType { get; }

        /// <summary>
        /// Gets the OAuth 2.0 provider's Authorization Endpoint.
        /// </summary>
        /// <value>The Authorization Endpoint.</value>
        string AuthorizationEndpoint { get; }

        /// <summary>
        /// Gets the OAuth 2.0 provider's ID field name.
        /// </summary>
        /// <value>The OAuth 2.0 provider's ID field name.</value>
        string IdField { get; }

        /// <summary>
        /// Gets the OAuth 2.0 provider's Resource Endpoint.
        /// </summary>
        /// <value>The Resource Endpoint.</value>
        string ResourceEndpoint { get; }

        // TODO scope

        /// <summary>
        /// Gets the OAuth 2.0 provider's Token Endpoint.
        /// </summary>
        /// <value>The Token Endpoint.</value>
        string TokenEndpoint { get; }

        // TODO userInfoMappingRules
    }
}
