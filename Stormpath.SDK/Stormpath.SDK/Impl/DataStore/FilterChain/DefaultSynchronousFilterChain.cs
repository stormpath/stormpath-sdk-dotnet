// <copyright file="DefaultSynchronousFilterChain.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.FilterChain
{
    internal sealed class DefaultSynchronousFilterChain : ISynchronousFilterChain
    {
        private readonly List<ISynchronousFilter> filters;

        public DefaultSynchronousFilterChain()
        {
            this.filters = new List<ISynchronousFilter>();
        }

        public DefaultSynchronousFilterChain(DefaultSynchronousFilterChain original)
            : this(original.filters)
        {
        }

        internal DefaultSynchronousFilterChain(IEnumerable<ISynchronousFilter> filters)
        {
            this.filters = new List<ISynchronousFilter>(filters);
        }

        private ISynchronousFilterChain AsInterface => this;

        public DefaultSynchronousFilterChain Add(ISynchronousFilter filter)
        {
            this.filters.Add(filter);
            return this;
        }

        IResourceDataResult ISynchronousFilterChain.Execute(IResourceDataRequest request, ILogger logger)
        {
            bool hasFilters = this.filters?.Any() ?? false;
            if (!hasFilters)
                throw new ApplicationException("Empty filter chain");

            if (this.filters.Count == 1)
            {
                return this.filters.Single().Execute(
                    request,
                    chain: null,
                    logger: logger);
            }

            var remainingChain = new DefaultSynchronousFilterChain(this.filters.Skip(1).ToList());
            return this.filters.First().Execute(request, remainingChain, logger);
        }
    }
}
