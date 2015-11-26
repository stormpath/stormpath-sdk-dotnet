// <copyright file="GroupCreationOptionsBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Group
{
    /// <summary>
    /// A builder to construct <see cref="IGroupCreationOptions"/> objects.
    /// </summary>
    public sealed class GroupCreationOptionsBuilder
    {
        /// <summary>
        /// Gets or sets the response options to apply to the request.
        /// </summary>
        /// <value>The response options to apply to the request.</value>
        /// <example>
        /// To request and cache custom data along with this request:
        /// <code>
        /// builder.ResponseOptions.Expand(x => x.GetCustomDataAsync());
        /// </code>
        /// </example>
        public IRetrievalOptions<IGroup> ResponseOptions { get; } = new DefaultRetrievalOptions<IGroup>();

        /// <summary>
        /// Creates a new <see cref="IApplicationCreationOptions"/> instance based on the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IApplicationCreationOptions"/> instance.</returns>
        public IGroupCreationOptions Build()
        {
            return new DefaultGroupCreationOptions(this.ResponseOptions);
        }
    }
}
