// <copyright file="SyncFilterExtensions.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Linq.Expressions;
using Stormpath.SDK.Impl.Linq;

namespace Stormpath.SDK.Sync
{
    public static class SyncFilterExtensions
    {
        /// <summary>
        /// Filters the items in a collection by searching all fields for a string.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref=IAsyncQueryable{TSource}"/> to filter.</param>
        /// <param name="caseInsensitiveMatch">The string to search for. Matching is case-insensitive.</param>
        /// <returns>An <see cref="IAsyncQueryable{T}"/> that contains elements from the input sequence that contain <paramref name="caseInsensitiveMatch"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="caseInsensitiveMatch"/> is null or empty.</exception>
        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> source, string caseInsensitiveMatch)
        {
            return source.Provider.CreateQuery<TSource>(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Filter, source, caseInsensitiveMatch),
                    source.Expression,
                    Expression.Constant(caseInsensitiveMatch)));
        }
    }
}
