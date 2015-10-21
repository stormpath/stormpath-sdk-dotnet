// <copyright file="IOrderedAsyncQueryable.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Linq
{
    /// <summary>
    /// Represents an ordered collection of items in a data source that can be queried asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public interface IOrderedAsyncQueryable<T> : IAsyncQueryable<T>
    {
    }
}
