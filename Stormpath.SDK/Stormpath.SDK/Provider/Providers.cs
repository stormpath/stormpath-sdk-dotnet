// <copyright file="Providers.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using Stormpath.SDK.Impl.Provider;

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// </summary>
    public static class Providers
    {
        /// <summary>
        /// Returns a new <see cref="IFacebookRequestFactory"/> instance, used to construct Facebook requests, like Facebook Account creation and retrieval.
        /// </summary>
        /// <returns>A new <see cref="IFacebookRequestFactory"/>.</returns>
        public static IFacebookRequestFactory Facebook()
            => new DefaultFacebookRequestFactory();

        /// <summary>
        /// Returns a new <see cref="IGithubRequestFactory"/> instance, used to construct Github requests, like Github Account creation and retrieval.
        /// </summary>
        /// <returns>A new <see cref="IGithubRequestFactory"/>.</returns>
        public static IGithubRequestFactory Github()
            => null;

        /// <summary>
        /// Returns a new <see cref="IGoogleRequestFactory"/> instance, used to construct Google requests, like Google Directory creation.
        /// </summary>
        /// <returns>A new <see cref="IGoogleRequestFactory"/>.</returns>
        public static IGoogleRequestFactory Google()
            => null;

        /// <summary>
        /// Returns a new <see cref="ILinkedInRequestFactory"/> instance, used to construct LinkedIn requests, like LinkedIn Account creation and retrieval.
        /// </summary>
        /// <returns>A new <see cref="IFacebookRequestFactory"/>.</returns>
        public static ILinkedInRequestFactory LinkedIn()
            => null;
    }
}
