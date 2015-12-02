// <copyright file="IAsyncQueryable{T}.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Linq
{
    /// <summary>
    /// Represents a collection of items in a data source that can be queried asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public interface IAsyncQueryable<T>
    {
        /// <summary>
        /// Gets the current page of results after a call to <see cref="MoveNextAsync(CancellationToken)"/>.
        /// </summary>
        /// <value>A list of items of type <typeparamref name="T"/>.</value>
        /// <exception cref="System.InvalidOperationException">The collection has not yet been iterated.</exception>
        IEnumerable<T> CurrentPage { get; }

        /// <summary>
        /// Attempts to asynchronously retrieve the next page of items from the data source.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result represents whether the collection retrieved items.</returns>
        Task<bool> MoveNextAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the expression tree that is associated with this instance of <see cref="IAsyncQueryable{T}"/>
        /// </summary>
        /// <value>The <see cref="System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="IAsyncQueryable{T}"/>.</value>
        Expression Expression { get; }

        /// <summary>
        /// Gets the asynchronous query provider that is associated with this data source.
        /// </summary>
        /// <value>The <see cref="IAsyncQueryProvider{T}"/> that is associated with this data source.</value>
        IAsyncQueryProvider<T> Provider { get; }
    }
}
