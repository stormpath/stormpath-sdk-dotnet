// <copyright file="ICacheProvider.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Base interface for <see cref="ISynchronousCacheProvider"/> and <see cref="IAsynchronousCacheProvider"/>.
    /// </summary>
    public interface ICacheProvider : IDisposable
    {
        /// <summary>
        /// Gets whether this <see cref="ICacheProvider"/> instance supports synchronous operations.
        /// </summary>
        /// <value>
        /// For any objects implementing <see cref="ISynchronousCacheProvider"/>, this should return <c>true</c>.
        /// </value>
        bool IsSynchronousSupported { get; }

        /// <summary>
        /// Gets whether this <see cref="ICacheProvider"/> instance supports asynchronous operations.
        /// </summary>
        /// <value>
        /// For any objects implementing <see cref="IAsynchronousCacheProvider"/>, this should return <c>true</c>.
        /// </value>
        bool IsAsynchronousSupported { get; }
    }
}
