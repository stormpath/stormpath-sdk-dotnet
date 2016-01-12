// <copyright file="AsyncQueryableExpandExtensions.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq.Expressions;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Linq.Expandables;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK
{
    /// <summary>
    /// Provides a set of static methods for making expanded resource requests within asynchronous LINQ-to-Stormpath queries.
    /// </summary>
    public static class AsyncQueryableExpandExtensions
    {
        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IAccount> Expand(this IAsyncQueryable<IAccount> source, Expression<Func<IAccountExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IApplication> Expand(this IAsyncQueryable<IApplication> source, Expression<Func<IApplicationExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IDirectory> Expand(this IAsyncQueryable<IDirectory> source, Expression<Func<IDirectoryExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IGroup> Expand(this IAsyncQueryable<IGroup> source, Expression<Func<IGroupExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IOrganization> Expand(this IAsyncQueryable<IOrganization> source, Expression<Func<IOrganizationExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IOrganizationAccountStoreMapping> Expand(this IAsyncQueryable<IOrganizationAccountStoreMapping> source, Expression<Func<IOrganizationAccountStoreMappingExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IGroupMembership> Expand(this IAsyncQueryable<IGroupMembership> source, Expression<Func<IGroupMembershipExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IAccountStoreMapping> Expand(this IAsyncQueryable<IAccountStoreMapping> source, Expression<Func<IAccountStoreMappingExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IApplicationAccountStoreMapping> Expand(this IAsyncQueryable<IApplicationAccountStoreMapping> source, Expression<Func<IAccountStoreMappingExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<ITenant> Expand(this IAsyncQueryable<ITenant> source, Expression<Func<ITenantExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient"/> object.
        /// </summary>
        /// <param name="source">The source query.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements will include additional data selected by <paramref name="selector"/>.</returns>
        public static IAsyncQueryable<IAuthenticationResult> Expand(this IAsyncQueryable<IAuthenticationResult> source, Expression<Func<IAuthenticationResultExpandables, object>> selector)
            => ExpandCommon.CreateQuery(source, selector);
    }
}
