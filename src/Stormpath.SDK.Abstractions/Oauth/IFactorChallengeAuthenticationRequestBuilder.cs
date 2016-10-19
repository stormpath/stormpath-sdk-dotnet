// <copyright file="IFactorChallengeAuthenticationRequestBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Builder pattern used to construct <see cref="IIdSiteTokenAuthenticationRequest"/> instances.
    /// </summary>
    public interface IFactorChallengeAuthenticationRequestBuilder : IOauthAuthenticationRequestBuilder<IFactorChallengeAuthenticationRequest>
    {
        /// <summary>
        /// Sets the <see cref="IChallenge">Challenge</see> to submit to.
        /// </summary>
        /// <remarks>This method delegates to <see cref="SetChallenge(string)"/> with the <c>Href</c> property from <see cref="IChallenge"/>.</remarks>
        /// <param name="challenge">The challenge.</param>
        /// <returns>This instance for method chaining.</returns>
        IFactorChallengeAuthenticationRequestBuilder SetChallenge(IChallenge challenge);

        /// <summary>
        /// Sets the challenge <c>href</c> to submit to.
        /// </summary>
        /// <param name="href">The <c>href</c> of a Challenge resource.</param>
        /// <returns>This instance for method chaining.</returns>
        IFactorChallengeAuthenticationRequestBuilder SetChallenge(string href);

        /// <summary>
        /// Sets the code to submit to the challenge.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>This instance for method chaining.</returns>
        IFactorChallengeAuthenticationRequestBuilder SetCode(string code);
    }
}
