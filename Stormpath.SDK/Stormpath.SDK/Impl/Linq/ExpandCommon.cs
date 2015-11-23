// <copyright file="ExpandCommon.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq
{
    internal static class ExpandCommon
    {
        public static IAsyncQueryable<TSource> CreateQuery<TSource, TExpandable, TExpanded>(IAsyncQueryable<TSource> source, Expression<Func<TExpandable, TExpanded>> selector)
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Expand, selector),
                    Expression.Quote(selector)));
        }

        public static IQueryable<TSource> CreateQuery<TSource, TExpandable, TExpanded>(IQueryable<TSource> source, Expression<Func<TExpandable, TExpanded>> selector)
        {
            return source.Provider.CreateQuery<TSource>(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Expand, selector),
                    Expression.Quote(selector)));
        }

        private static void Expand(Expression expression)
        {
        }
    }
}
