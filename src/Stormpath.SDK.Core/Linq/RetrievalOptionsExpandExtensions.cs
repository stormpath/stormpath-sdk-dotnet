// <copyright file="RetrievalOptionsExpandExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq.Expandables;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK
{
    /// <summary>
    /// Provides a set of static methods for getting expanded responses from creation and update requests.
    /// </summary>
    public static class RetrievalOptionsExpandExtensions
    {
        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IAccount> Expand(this IRetrievalOptions<IAccount> options, Expression<Func<IAccountExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IAccount>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IApplication> Expand(this IRetrievalOptions<IApplication> options, Expression<Func<IApplicationExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IApplication>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IDirectory> Expand(this IRetrievalOptions<IDirectory> options, Expression<Func<IDirectoryExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IDirectory>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IGroup> Expand(this IRetrievalOptions<IGroup> options, Expression<Func<IGroupExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IGroup>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IOrganization> Expand(this IRetrievalOptions<IOrganization> options, Expression<Func<IOrganizationExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IOrganization>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IOrganizationAccountStoreMapping> Expand(this IRetrievalOptions<IOrganizationAccountStoreMapping> options, Expression<Func<IOrganizationAccountStoreMappingExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IOrganizationAccountStoreMapping>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IGroupMembership> Expand(this IRetrievalOptions<IGroupMembership> options, Expression<Func<IGroupMembershipExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IGroupMembership>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IAccountStoreMapping> Expand(this IRetrievalOptions<IAccountStoreMapping> options, Expression<Func<IAccountStoreMappingExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IAccountStoreMapping>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IApplicationAccountStoreMapping> Expand(this IRetrievalOptions<IApplicationAccountStoreMapping> options, Expression<Func<IAccountStoreMappingExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IApplicationAccountStoreMapping>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<ITenant> Expand(this IRetrievalOptions<ITenant> options, Expression<Func<ITenantExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<ITenant>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IAuthenticationResult> Expand(this IRetrievalOptions<IAuthenticationResult> options, Expression<Func<IAuthenticationResultExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IAuthenticationResult>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IApiKey> Expand(this IRetrievalOptions<IApiKey> options, Expression<Func<IApiKeyExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IApiKey>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IFactor> Expand(this IRetrievalOptions<IFactor> options, Expression<Func<IFactorExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IFactor>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<ISmsFactor> Expand(this IRetrievalOptions<ISmsFactor> options, Expression<Func<ISmsFactorExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<ISmsFactor>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IPhone> Expand(this IRetrievalOptions<IPhone> options, Expression<Func<IPhoneExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IPhone>).SetProxy(x => x.Expand(selector));
            return options;
        }

        /// <summary>
        /// Retrieves additional data in this request from a linked resource. This has no effect if caching is disabled on the <see cref="Client.IClient">Client</see> object.
        /// </summary>
        /// <param name="options">The options for this request.</param>
        /// <param name="selector">A function to select a resource-returning method to expand.</param>
        /// <returns>The current instance for method chaining.</returns>
        public static IRetrievalOptions<IChallenge> Expand(this IRetrievalOptions<IChallenge> options, Expression<Func<IChallengeExpandables, object>> selector)
        {
            (options as DefaultRetrievalOptions<IChallenge>).SetProxy(x => x.Expand(selector));
            return options;
        }
    }
}
