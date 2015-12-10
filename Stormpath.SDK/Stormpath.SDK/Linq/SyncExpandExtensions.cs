// <copyright file="SyncExpandExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Linq.Expressions;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq.Expandables;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides a set of static methods for making expanded resource requests within synchronous LINQ-to-Stormpath queries.
    /// </summary>
    public static class SyncExpandExtensions
    {
        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IAccount> Expand(this IQueryable<IAccount> source, Expression<Func<IAccountExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IApplication> Expand(this IQueryable<IApplication> source, Expression<Func<IApplicationExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IDirectory> Expand(this IQueryable<IDirectory> source, Expression<Func<IDirectoryExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IGroup> Expand(this IQueryable<IGroup> source, Expression<Func<IGroupExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IOrganization> Expand(this IQueryable<IOrganization> source, Expression<Func<IOrganizationExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IOrganizationAccountStoreMapping> Expand(this IQueryable<IOrganizationAccountStoreMapping> source, Expression<Func<IOrganizationAccountStoreMappingExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IGroupMembership> Expand(this IQueryable<IGroupMembership> source, Expression<Func<IGroupMembershipExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IAccountStoreMapping> Expand(this IQueryable<IAccountStoreMapping> source, Expression<Func<IAccountStoreMappingExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IApplicationAccountStoreMapping> Expand(this IQueryable<IApplicationAccountStoreMapping> source, Expression<Func<IAccountStoreMappingExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<ITenant> Expand(this IQueryable<ITenant> source, Expression<Func<ITenantExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IQueryable<IAuthenticationResult> Expand(this IQueryable<IAuthenticationResult> source, Expression<Func<IAuthenticationResultExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);
    }
}
