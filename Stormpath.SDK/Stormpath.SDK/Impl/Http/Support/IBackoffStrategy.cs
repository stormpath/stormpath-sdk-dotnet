// <copyright file="IBackoffStrategy.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Http.Support
{
    /// <summary>
    /// Represents a progressive retry backoff algorithm.
    /// </summary>
    internal interface IBackoffStrategy
    {
        /// <summary>
        /// Gets the number of milliseconds to wait for the specified <paramref name="retryCount"/>.
        /// </summary>
        /// <param name="retryCount">The number of retries.</param>
        /// <returns>The number of milliseconds to wait.</returns>
        int GetDelayMilliseconds(int retryCount);
    }
}
