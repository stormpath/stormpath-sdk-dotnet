// <copyright file="SyncScalarExecutor{TItem}.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Linq.Executor;

namespace Stormpath.SDK.Impl.Linq.Sync
{
    internal sealed class SyncScalarExecutor<TItem>
    {
        private readonly IAsyncExecutor<TItem> executor;

        public SyncScalarExecutor(IAsyncExecutor<TItem> executor, Expression expression)
        {
            this.executor = new CollectionResourceExecutor<TItem>(executor, expression);
        }

        public object Execute()
        {
            this.executor.MoveNext();

            var resultOperator = this.executor.CompiledModel.ResultOperator;

            if (resultOperator == Parsing.ResultOperator.Any)
                return this.executor.CurrentPage.Any();

            if (resultOperator == Parsing.ResultOperator.Count)
                return Convert.ToInt32(this.executor.Size);

            if (resultOperator == Parsing.ResultOperator.LongCount)
                return this.executor.Size;

            if (resultOperator == Parsing.ResultOperator.First)
            {
                return this.executor.CompiledModel.ResultDefaultIfEmpty
                    ? this.executor.CurrentPage.FirstOrDefault()
                    : this.executor.CurrentPage.First();
            }

            if (resultOperator == Parsing.ResultOperator.Single)
            {
                return this.executor.CompiledModel.ResultDefaultIfEmpty
                    ? this.executor.CurrentPage.SingleOrDefault()
                    : this.executor.CurrentPage.Single();
            }

            throw new NotSupportedException("This result operator is not supported.");
        }
    }
}
