// <copyright file="DefaultAsynchronousFilterChain.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class DefaultAsynchronousFilterChain : IAsynchronousFilterChain
    {
        private readonly IInternalAsyncDataStore dataStore;
        private readonly List<IAsynchronousFilter> filters;

        public DefaultAsynchronousFilterChain(IInternalAsyncDataStore dataStore)
            : this(dataStore, Enumerable.Empty<IAsynchronousFilter>())
        {
        }

        public DefaultAsynchronousFilterChain(DefaultAsynchronousFilterChain original)
            : this(original.dataStore, original.filters)
        {
        }

        internal DefaultAsynchronousFilterChain(IInternalAsyncDataStore dataStore, IEnumerable<IAsynchronousFilter> filters)
        {
            this.dataStore = dataStore;
            this.filters = new List<IAsynchronousFilter>(filters);
        }

        private IAsynchronousFilterChain AsInterface => this;

        IInternalAsyncDataStore IAsynchronousFilterChain.DataStore => this.dataStore;

        public DefaultAsynchronousFilterChain Add(IAsynchronousFilter asyncFilter)
        {
            this.filters.Add(asyncFilter);
            return this;
        }

        Task<IResourceDataResult> IAsynchronousFilterChain.FilterAsync(IResourceDataRequest request, ILogger logger, CancellationToken cancellationToken)
        {
            bool hasFilters = !this.filters.IsNullOrEmpty();
            if (!hasFilters)
            {
                throw new Exception("Empty filter chain");
            }

            if (this.filters.Count == 1)
            {
                return this.filters.Single().FilterAsync(
                    request,
                    chain: null,
                    logger: logger,
                    cancellationToken: cancellationToken);
            }

            var remainingChain = new DefaultAsynchronousFilterChain(this.dataStore, this.filters.Skip(1));
            return this.filters.First().FilterAsync(request, remainingChain, logger, cancellationToken);
        }
    }
}
