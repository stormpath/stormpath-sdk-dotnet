// <copyright file="IAsyncExecutor{TResult}.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Executor
{
    /// <summary>
    /// A query executor for <see cref="SDK.Linq.IAsyncQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal interface IAsyncExecutor<TResult>
    {
        /// <summary>
        /// Gets the current request URL.
        /// </summary>
        /// <value>The current request URL.</value>
        string CurrentHref { get; }

        /// <summary>
        /// Gets the compiled query model.
        /// </summary>
        /// <value>The compiled query model.</value>
        CollectionResourceQueryModel CompiledModel { get; }

        /// <summary>
        /// Gets the current paging offset.
        /// </summary>
        /// <value>The current paging offset.</value>
        long Offset { get; }

        /// <summary>
        /// Gets the current paging limit.
        /// </summary>
        /// <value>The current paging limit.</value>
        long Limit { get; }

        /// <summary>
        /// Gets the current paging size.
        /// </summary>
        /// <value>The current paging size.</value>
        long Size { get; }

        /// <summary>
        /// Gets the current page of results.
        /// </summary>
        /// <value>The current page of results.</value>
        IEnumerable<TResult> CurrentPage { get; }

        /// <summary>
        /// Attempts to move to the next page of collection results.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see langword="true"/> if the operation succeeded and the current values have been updated;
        /// <see langword="false"/> if the iterator has been exhausted.</returns>
        Task<bool> MoveNextAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Synchronously attempts to move to the next page of collection results.
        /// </summary>
        /// <returns><see langword="true"/> if the operation succeeded and the current values have been updated;
        /// <see langword="false"/> if the iterator has been exhausted.</returns>
        bool MoveNext();
    }
}
