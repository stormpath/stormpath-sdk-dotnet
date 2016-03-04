// <copyright file="IScopedClientFactory.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// Represents a <see cref="IClient">Client</see> instance scoped for a specific request or transaction.
    /// </summary>
    public interface IScopedClientFactory
    {
        /// <summary>
        /// Creates a new scoped <see cref="IClient">Client</see> from an existing client instance.
        /// </summary>
        /// <param name="options">The options for this scoped client.</param>
        /// <returns>A scoped instance of the existing <see cref="IClient">Client</see>.</returns>
        IClient Create(ScopedClientOptions options = null);
    }
}
