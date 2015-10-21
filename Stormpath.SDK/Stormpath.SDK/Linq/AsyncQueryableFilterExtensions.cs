// <copyright file="AsyncQueryableFilterExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK
{
    public static class AsyncQueryableFilterExtensions
    {
        /// <summary>
        /// Filters the items in a collection by searching all fields for a string.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="caseInsensitiveMatch">The string to search for. Matching is case-insensitive.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> that contains elements from the input sequence that contain <paramref name="caseInsensitiveMatch"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="caseInsensitiveMatch"/> is null or empty.</exception>
        public static IAsyncQueryable<TSource> Filter<TSource>(this IAsyncQueryable<TSource> source, string caseInsensitiveMatch)
            where TSource : IResource
        {
            if (string.IsNullOrEmpty(caseInsensitiveMatch))
                throw new ArgumentNullException(nameof(caseInsensitiveMatch));

            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Sync.FilterExtensions.Filter, (IQueryable<TSource>)null, caseInsensitiveMatch),
                    source.Expression,
                    Expression.Constant(caseInsensitiveMatch)));
        }
    }
}
