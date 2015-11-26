// <copyright file="IGithubProvider.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Github-specific <see cref="IProvider"/> Resource.
    /// </summary>
    public interface IGithubProvider : IProvider
    {
        /// <summary>
        /// Gets the Client ID of the Github application.
        /// </summary>
        /// <value>The Client ID of the Github application.</value>
        string ClientId { get; }

        /// <summary>
        /// Gets the Client Secret of the Github application.
        /// </summary>
        /// <value>The Client Secret of the Github application.</value>
        string ClientSecret { get; }
    }
}
