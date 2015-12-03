// <copyright file="DirectoryCreationOptionsBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// A builder to construct <see cref="IDirectoryCreationOptions"/> objects.
    /// </summary>
    public sealed class DirectoryCreationOptionsBuilder
    {
        private IProvider provider;

        /// <summary>
        /// Specifies a Provider to create this Directory for.
        /// </summary>
        /// <param name="request">The Provider creation instance.</param>
        /// <returns>This instance for method chaining.</returns>
        public DirectoryCreationOptionsBuilder ForProvider(ICreateProviderRequest request)
        {
            this.provider = request.GetProvider();
            return this;
        }

        /// <summary>
        /// Gets the response options to apply to the request.
        /// </summary>
        /// <value>The response options to apply to the request.</value>
        /// <example>
        /// To request and cache custom data along with this request:
        /// <code>
        /// builder.ResponseOptions.Expand(x => x.GetCustomDataAsync());
        /// </code>
        /// </example>
        public IRetrievalOptions<IDirectory> ResponseOptions { get; } = new DefaultRetrievalOptions<IDirectory>();

        /// <summary>
        /// Creates a new <see cref="IDirectoryCreationOptions"/> instance based on the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IDirectoryCreationOptions"/> instance.</returns>
        public IDirectoryCreationOptions Build()
        {
            return new DefaultDirectoryCreationOptions(this.provider, this.ResponseOptions);
        }
    }
}
