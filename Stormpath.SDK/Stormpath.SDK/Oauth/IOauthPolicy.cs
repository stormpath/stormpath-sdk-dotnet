// <copyright file="IOauthPolicy.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// An <see cref="IOauthPolicy">OauthPolicy</see> is used to configure different aspects of the OAuth tokens associated with
    /// an <see cref="IApplication">Application</see>.
    /// </summary>
    public interface IOauthPolicy :
        IResource,
        IHasTenant,
        ISaveable<IOauthPolicy>,
        IAuditable
    {
        /// <summary>
        /// Gets the Time to Live (TTL) for the access tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <value>The Time to Live for the access tokens created for the parent <see cref="IApplication">Application</see>.</value>
        TimeSpan AccessTokenTimeToLive { get; }

        /// <summary>
        /// Gets the Time to Live (TTL) for the refresh tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <value>The Time to Live for the refresh tokens created for the parent <see cref="IApplication">Application</see>.</value>
        TimeSpan RefreshTokenTimeToLive { get; }

        /// <summary>
        /// Gets the <c>href</c> corresponding to the endpoint for access tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <value>The <c>href</c> corresponding to the endpoint for access tokens created for the parent <see cref="IApplication">Application</see>.</value>
        string TokenEndpointHref { get; }

        /// <summary>
        /// Sets the Time to Live (TTL) for the access tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="accessTokenTtl">The Time to Live.</param>
        /// <returns>This instance for method chaining.</returns>
        IOauthPolicy SetAccessTokenTimeToLive(TimeSpan accessTokenTtl);

        /// <summary>
        /// Sets the Time to Live (TTL) for the access tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="accessTokenTtl">The Time to Live, expressed in ISO 8601 duration format (for example, <c>PT1H</c> for a 1-hour duration).</param>
        /// <returns>This instance for method chaining.</returns>
        IOauthPolicy SetAccessTokenTimeToLive(string accessTokenTtl);

        /// <summary>
        /// Sets the Time to Live (TTL) for the refresh tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="refreshTokenTtl">The Time to Live.</param>
        /// <returns>This instance for method chaining.</returns>
        IOauthPolicy SetRefreshTokenTimeToLive(TimeSpan refreshTokenTtl);

        /// <summary>
        /// Sets the Time to Live (TTL) for the refresh tokens created for the parent <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="refreshTokenTtl">The Time to Live, expressed in ISO 8601 duration format (for example, <c>PT1H</c> for a 1-hour duration).</param>
        /// <returns>This instance for method chaining.</returns>
        IOauthPolicy SetRefreshTokenTimeToLive(string refreshTokenTtl);

        /// <summary>
        /// Returns the <see cref="IApplication">Application</see> associated with this <see cref="IOauthPolicy">OauthPolicy</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="IOauthPolicy">OauthPolicy</see>.</returns>
        Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
