// <copyright file="AsyncQueryableExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

// Placed in the base library namespace so that these extension methods are available without any extra usings
namespace Stormpath.SDK
{
    public static class AsyncQueryableExtensions
    {
        public static async Task<T> FirstAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException("The sequence has no elements.");

            return source.CurrentPage.First();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                return default(T);

            return source.CurrentPage.FirstOrDefault();
        }
    }
}
