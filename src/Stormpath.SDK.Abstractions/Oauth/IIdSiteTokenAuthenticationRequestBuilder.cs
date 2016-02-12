// <copyright file="IIdSiteTokenAuthenticationRequestBuilder.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Builder pattern used to construct <see cref="IIdSiteTokenAuthenticationRequest"/> instances.
    /// </summary>
    public interface IIdSiteTokenAuthenticationRequestBuilder : IOauthAuthenticationRequestBuilder<IIdSiteTokenAuthenticationRequest>
    {
        /// <summary>
        /// Sets the ID Site response JSON Web Token that will be used in this request.
        /// </summary>
        /// <param name="jwt">The JSON Web Token (JWT) string.</param>
        /// <returns>This instance for method chaining.</returns>
        IIdSiteTokenAuthenticationRequestBuilder SetJwt(string jwt);
    }
}
