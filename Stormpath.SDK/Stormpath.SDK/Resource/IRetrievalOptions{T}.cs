// <copyright file="IRetrievalOptions{T}.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Resource
{
    /// <summary>
    /// Base interface for retrieval request options objects.
    /// </summary>
    /// <typeparam name="T">The resource type being retrieved.</typeparam>
    public interface IRetrievalOptions<T> : ICreationOptions
    {
        IRetrievalOptions<T> Expand<TExpand>(Func<T, TExpand> selector);

        IRetrievalOptions<T> Expand<TExpand>(Func<T, TExpand> selector, int? offset = null, int? limit = null);
    }
}
