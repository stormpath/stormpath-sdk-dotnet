// <copyright file="DefaultRetrievalOptions.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Resource
{
    internal sealed class DefaultRetrievalOptions<T> : IRetrievalOptions<T>
    {
        private IQueryable<T> proxy = Enumerable.Empty<T>().AsQueryable();
        private bool dirty = false;

        private IRetrievalOptions<T> AsInterface => this;

        IRetrievalOptions<T> IRetrievalOptions<T>.Expand(Expression<Func<T, Func<CancellationToken, Task>>> selector)
        {
            this.proxy = this.proxy.Expand(selector);
            this.dirty = true;

            return this;
        }

        IRetrievalOptions<T> IRetrievalOptions<T>.Expand(Expression<Func<T, Func<IResource>>> selector)
        {
            this.proxy = this.proxy.Expand(selector);
            this.dirty = true;

            return this;
        }

        IRetrievalOptions<T> IRetrievalOptions<T>.Expand(Expression<Func<T, Func<IAsyncQueryable>>> selector, int? offset, int? limit)
        {
            this.proxy = this.proxy.Expand(selector, offset, limit);
            this.dirty = true;

            return this;
        }

        string IRetrievalOptions<T>.GetQueryString()
        {
            if (!this.dirty)
                return string.Empty;

            var queryModel = QueryModelCompiler.Compile(this.proxy.Expression);
            var arguments = RequestBuilder.GetArguments(queryModel);

            return string.Join(",", arguments);
        }

        public override string ToString()
            => this.AsInterface.GetQueryString();
    }
}
