// <copyright file="IApplicationExpandables.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Application;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Linq.Expandables
{
    /// <summary>
    /// Represents resources that can be expanded from an <see cref="IApplication">Application</see>.
    /// </summary>
    public interface IApplicationExpandables :
        IExpandableAccounts,
        IExpandableCustomData,
        IExpandableGroups,
        IExpandableTenant,
        IExpandableDefaultStores
    {
        /// <summary>
        /// Expands the <c>accountStoreMappings</c> collection with the default pagination options.
        /// </summary>
        /// <returns>Not applicable.</returns>
        IAsyncQueryable<IApplicationAccountStoreMapping> GetAccountStoreMappings();

        /// <summary>
        /// Expands the <c>accountStoreMappings</c> collection with the specified pagination options.
        /// </summary>
        /// <param name="offset">The pagination offset, or <see langword="null"/> use the default value.</param>
        /// <param name="limit">The pagination limit, or <see langword="null"/> use the default value.</param>
        /// <returns>Not applicable.</returns>
        IAsyncQueryable<IApplicationAccountStoreMapping> GetAccountStoreMappings(int? offset, int? limit);

        /// <summary>
        /// Expands the <c>oAuthPolicy</c> resource.
        /// </summary>
        /// <returns>Not applicable.</returns>
        IOauthPolicy GetOauthPolicy();
    }
}
