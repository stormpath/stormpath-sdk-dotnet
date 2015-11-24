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
    internal interface IAsyncExecutor<TResult>
    {
        string CurrentHref { get; }

        CollectionResourceQueryModel CompiledModel { get; }

        long Offset { get; }

        long Limit { get; }

        long Size { get; }

        IEnumerable<TResult> CurrentPage { get; }

        Task<bool> MoveNextAsync(CancellationToken cancellationToken);

        bool MoveNext();
    }
}
