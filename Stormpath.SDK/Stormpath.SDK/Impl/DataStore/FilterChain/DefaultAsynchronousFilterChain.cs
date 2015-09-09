// <copyright file="DefaultAsynchronousFilterChain.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.FilterChain
{
    internal sealed class DefaultAsynchronousFilterChain : IAsynchronousFilterChain
    {
        private readonly List<IAsynchronousFilter> filters;

        public DefaultAsynchronousFilterChain()
        {
            this.filters = new List<IAsynchronousFilter>();
        }

        public DefaultAsynchronousFilterChain(DefaultAsynchronousFilterChain original)
            : this(original.filters)
        {
        }

        internal DefaultAsynchronousFilterChain(IEnumerable<IAsynchronousFilter> filters)
        {
            this.filters = new List<IAsynchronousFilter>(filters);
        }

        private IAsynchronousFilterChain AsInterface => this;

        public DefaultAsynchronousFilterChain Add(IAsynchronousFilter asyncFilter)
        {
            this.filters.Add(asyncFilter);
            return this;
        }

        Task<IResourceDataResult> IAsynchronousFilterChain.ExecuteAsync(IResourceDataRequest request, ILogger logger, CancellationToken cancellationToken)
        {
            bool hasFilters = this.filters?.Any() ?? false;
            if (!hasFilters)
                throw new ApplicationException("Empty filter chain");

            if (this.filters.Count == 1)
            {
                return this.filters.Single().ExecuteAsync(
                    request,
                    chain: null,
                    logger: logger,
                    cancellationToken: cancellationToken);
            }

            var remainingChain = new DefaultAsynchronousFilterChain(this.filters.Skip(1).ToList());
            return this.filters.First().ExecuteAsync(request, remainingChain, logger, cancellationToken);
        }
    }
}
