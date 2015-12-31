// <copyright file="IAccountExpandables.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Oauth;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Linq.Expandables
{
    /// <summary>
    /// Represents resources that can be expanded from an <see cref="Account.IAccount"/>.
    /// </summary>
    public interface IAccountExpandables :
        IExpandableApplications,
        IExpandableCustomData,
        IExpandableDirectory,
        IExpandableGroupMemberships,
        IExpandableGroups,
        IExpandableTenant
    {
        /// <summary>
        /// Expands the <c>providerData</c> resource.
        /// </summary>
        /// <returns>Not applicable.</returns>
        IProviderData GetProviderData();

        /// <summary>
        /// Expands the <c>accessTokens</c> collection with the default pagination options.
        /// </summary>
        /// <returns>Not applicable.</returns>
        IAsyncQueryable<IAccessToken> GetAccessTokens();

        /// <summary>
        /// Expands the <c>accessTokens</c> collection with the specified pagination options.
        /// </summary>
        /// <param name="offset">The pagination offset, or <see langword="null"/> use the default value.</param>
        /// <param name="limit">The pagination limit, or <see langword="null"/> use the default value.</param>
        /// <returns>Not applicable.</returns>
        IAsyncQueryable<IAccessToken> GetAccessTokens(int? offset, int? limit);

        /// <summary>
        /// Expands the <c>refreshTokens</c> collection with the default pagination options.
        /// </summary>
        /// <returns>Not applicable.</returns>
        IAsyncQueryable<IAccessToken> GetRefreshTokens();

        /// <summary>
        /// Expands the <c>refreshTokens</c> collection with the specified pagination options.
        /// </summary>
        /// <param name="offset">The pagination offset, or <see langword="null"/> use the default value.</param>
        /// <param name="limit">The pagination limit, or <see langword="null"/> use the default value.</param>
        /// <returns>Not applicable.</returns>
        IAsyncQueryable<IAccessToken> GetRefreshTokens(int? offset, int? limit);
    }
}
