// <copyright file="AsyncQueryProviderWrapper.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Linq.Parsing.Structure;

namespace Stormpath.SDK.Impl.Linq
{
    internal class AsyncQueryProviderWrapper : IAsyncQueryProvider
    {
        private readonly IQueryParser queryParser;
        private readonly IAsyncQueryExecutor executor;

        public AsyncQueryProviderWrapper(IQueryParser queryParser, IAsyncQueryExecutor executor)
        {
            this.queryParser = queryParser;
            this.executor = executor;
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var queryModel = queryParser.GetParsedQuery(expression);

            throw new NotImplementedException();
        }
    }
}
