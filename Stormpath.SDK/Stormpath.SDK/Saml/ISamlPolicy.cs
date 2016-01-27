// <copyright file="ISamlPolicy.cs" company="Stormpath, Inc.">
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
    /// Represents a SAML Policy associated with an <see cref="Application.IApplication">Application</see>.
    /// </summary>
    public interface ISamlPolicy : IResource, IAuditable
    {
        /// <summary>
        /// Gets the <see cref="ISamlServiceProvider">SAML Service Provider</see> resource associated with this <see cref="ISamlPolicy">SAML Policy</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="ISamlServiceProvider">SAML Service Provider</see>.</returns>
        Task<ISamlServiceProvider> GetSamlServiceProviderAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
