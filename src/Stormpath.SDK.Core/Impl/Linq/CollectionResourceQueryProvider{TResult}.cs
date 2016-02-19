// <copyright file="CollectionResourceQueryProvider{TResult}.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Stormpath.SDK.Impl.Linq.Executor;
using Stormpath.SDK.Impl.Linq.Sync;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryProvider<TResult> : IAsyncQueryProvider<TResult>, IQueryProvider
    {
        private IAsyncExecutor<TResult> executor;

        public CollectionResourceQueryProvider(IAsyncExecutor<TResult> executor)
        {
            this.executor = executor;
        }

        public IAsyncExecutor<TResult> Executor
        {
            get { return this.executor; }
            set { this.executor = value; }
        }

        // Collection-returning standard query operators call this method.
        IAsyncQueryable<TResult> IAsyncQueryProvider<TResult>.CreateQuery(Expression expression)
            => new CollectionResourceQueryable<TResult>(this, expression);

        public IQueryable CreateQuery(Expression expression)
            => new CollectionResourceQueryable<TResult>(this, expression);

        public IQueryable<T> CreateQuery<T>(Expression expression)
            => new CollectionResourceQueryable<T>(this, expression);

        public object Execute(Expression expression)
        {
            var scalarExecutor = new SyncScalarExecutor<TResult>(this.executor, expression);
            return scalarExecutor.Execute();
        }

        public T Execute<T>(Expression expression)
        {
            bool isEnumerable = typeof(T).Name == "IEnumerable`1";

            var scalarExecutor = new SyncScalarExecutor<TResult>(this.executor, expression);
            return (T)scalarExecutor.Execute();
        }
    }
}
