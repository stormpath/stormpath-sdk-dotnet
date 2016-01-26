// <copyright file="ISamlServiceProvider.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Saml
{
    /// <summary>
    /// Represents a SAML Service Provider associated with a <see cref="ISamlPolicy">SAML Policy.</see>
    /// </summary>
    public interface ISamlServiceProvider : IResource, IAuditable
    {
        /// <summary>
        /// Gets the endpoint resource used to initiate SAML-based Single Sign-On.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The endpoint resource used to initiate SAML-based Single Sign-On.</returns>
        Task<ISsoInitiationEndpoint> GetSsoInitiationEndpointAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
