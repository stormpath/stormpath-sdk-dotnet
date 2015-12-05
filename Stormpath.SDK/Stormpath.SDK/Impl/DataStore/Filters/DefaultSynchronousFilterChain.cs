// <copyright file="DefaultSynchronousFilterChain.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class DefaultSynchronousFilterChain : ISynchronousFilterChain
    {
        private readonly IInternalSyncDataStore dataStore;
        private readonly List<ISynchronousFilter> filters;

        public DefaultSynchronousFilterChain(IInternalSyncDataStore dataStore)
            : this(dataStore, Enumerable.Empty<ISynchronousFilter>())
        {
        }

        public DefaultSynchronousFilterChain(DefaultSynchronousFilterChain original)
            : this(original.dataStore, original.filters)
        {
        }

        internal DefaultSynchronousFilterChain(IInternalSyncDataStore dataStore, IEnumerable<ISynchronousFilter> filters)
        {
            this.dataStore = dataStore;
            this.filters = new List<ISynchronousFilter>(filters);
        }

        private ISynchronousFilterChain AsInterface => this;

        IInternalSyncDataStore ISynchronousFilterChain.DataStore => this.dataStore;

        public DefaultSynchronousFilterChain Add(ISynchronousFilter filter)
        {
            this.filters.Add(filter);
            return this;
        }

        IResourceDataResult ISynchronousFilterChain.Filter(IResourceDataRequest request, ILogger logger)
        {
            bool hasFilters = !this.filters.IsNullOrEmpty();
            if (!hasFilters)
            {
                throw new ApplicationException("Empty filter chain");
            }

            if (this.filters.Count == 1)
            {
                return this.filters.Single().Filter(
                    request,
                    chain: null,
                    logger: logger);
            }

            var remainingChain = new DefaultSynchronousFilterChain(this.dataStore, this.filters.Skip(1));
            return this.filters.First().Filter(request, remainingChain, logger);
        }
    }
}
