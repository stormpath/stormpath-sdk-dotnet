// <copyright file="IAsyncQueryProvider.cs" company="Stormpath, Inc.">
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

using System.Linq.Expressions;

namespace Stormpath.SDK.Linq
{
    /// <summary>
    /// Represents a query provider for an asynchronous data source.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public interface IAsyncQueryProvider<T>
    {
        /// <summary>
        /// Appends an <see cref="System.Linq.Expressions.Expression"/> to the existing expression tree
        /// for an <see cref="IAsyncQueryable{T}"/>.
        /// </summary>
        /// <param name="expression">The expression to add to the expression tree.</param>
        /// <returns>A new <see cref="IAsyncQueryable{T}"/> instance with the new expression tree.</returns>
        IAsyncQueryable<T> CreateQuery(Expression expression);
    }
}