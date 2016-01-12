// <copyright file="NullExecutor{TResult}.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Executor
{
    internal sealed class NullExecutor<TResult> : IAsyncExecutor<TResult>
    {
        CollectionResourceQueryModel IAsyncExecutor<TResult>.CompiledModel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAsyncExecutor<TResult>.CurrentHref
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnumerable<TResult> IAsyncExecutor<TResult>.CurrentPage
            => Enumerable.Empty<TResult>();

        long IAsyncExecutor<TResult>.Limit => 0;

        long IAsyncExecutor<TResult>.Offset => 0;

        long IAsyncExecutor<TResult>.Size => 0;

        bool IAsyncExecutor<TResult>.MoveNext()
            => false;

        Task<bool> IAsyncExecutor<TResult>.MoveNextAsync(CancellationToken cancellationToken)
            => Task.FromResult(false);
    }
}
