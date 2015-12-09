// <copyright file="AccountStoreMappingCacheInvalidationFilter.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    /// <summary>
    /// When IAccountStoreMapping objects are created or updated, this filter will refresh
    /// the cache and identity map by retrieving all of the account store mappings for the related application.
    /// This filter does not mutate the request or result.
    /// </summary>
    internal sealed class AccountStoreMappingCacheInvalidationFilter : IAsynchronousFilter, ISynchronousFilter
    {
        IResourceDataResult ISynchronousFilter.Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            var result = chain.Filter(request, logger);

            if (!IsCreateOrUpdate(request))
            {
                return result;
            }

            if (!IsAccountStoreMapping(result))
            {
                return result;
            }

            var applicationHref = GetApplicationHref(result);
            if (string.IsNullOrEmpty(applicationHref))
            {
                return result;
            }

            var application = chain.DataStore.GetResourceSkipCache<IApplication>(applicationHref);
            var allMappings = application.GetAccountStoreMappings().Synchronously().ToList();

            logger.Trace($"AccountStoreMapping update detected; refreshing all {allMappings.Count} AccountStoreMappings in cache for application '{applicationHref}'", "AccountStoreMappingCacheInvalidationFilter.Filter");

            return result;
        }

        async Task<IResourceDataResult> IAsynchronousFilter.FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            var result = await chain.FilterAsync(request, logger, cancellationToken).ConfigureAwait(false);

            if (!IsCreateOrUpdate(request))
            {
                return result;
            }

            if (!IsAccountStoreMapping(result))
            {
                return result;
            }

            var applicationHref = GetApplicationHref(result);
            if (string.IsNullOrEmpty(applicationHref))
            {
                return result;
            }

            var application = await chain.DataStore.GetResourceSkipCacheAsync<IApplication>(applicationHref, cancellationToken).ConfigureAwait(false);
            var allMappings = await application.GetAccountStoreMappings().ToListAsync(cancellationToken).ConfigureAwait(false);

            logger.Trace($"AccountStoreMapping update detected; refreshing all {allMappings.Count} AccountStoreMappings in cache for application '{applicationHref}'", "AccountStoreMappingCacheInvalidationFilter.FilterAsync");

            return result;
        }

        private static bool IsCreateOrUpdate(IResourceDataRequest request)
            => request.Action == ResourceAction.Create
            || request.Action == ResourceAction.Update;

        private static bool IsAccountStoreMapping(IResourceDataResult result)
        {
            var type = result.Type;
            return typeof(IAccountStoreMapping).IsAssignableFrom(type)
                || typeof(IAccountStoreMapping<IApplicationAccountStoreMapping>).IsAssignableFrom(type)
                || typeof(IAccountStoreMapping<IOrganizationAccountStoreMapping>).IsAssignableFrom(type);
        }

        private static string GetApplicationHref(IResourceDataResult result)
        {
            object application;

            if (!result.Body.TryGetValue("application", out application))
            {
                return null;
            }

            return (application as IEmbeddedProperty)?.Href;
        }
    }
}
