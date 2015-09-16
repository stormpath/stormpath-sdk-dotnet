// <copyright file="IApplicationCreationOptions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Application
{
    /// <summary>
    /// Represents options for an <see cref="IApplication"/> creation request.
    /// </summary>
    public interface IApplicationCreationOptions : ICreationOptions
    {
        /// <summary>
        /// Gets a flag that determines whether to create a new <see cref="Directory.IDirectory"/> for the new application's needs.
        /// </summary>
        /// <value>
        /// <para>
        /// If <c>true</c>, a new directory will be created. The new directory will automatically be assigned as the application's default login source.
        /// If the <see cref="DirectoryName"/> property is not null, the new directory will be created with that name.
        /// Otherwise, the directory will be automatically named based on heuristics to ensure a guaranteed unique name based on the application.
        /// </para>
        /// <para>
        /// If <c>false</c>, no directory will be created.
        /// </para>
        /// </value>
        bool CreateDirectory { get; }

        /// <summary>
        /// Gets or sets the name to use when creating a new <see cref="Directory.IDirectory"/>.
        /// </summary>
        /// <value>
        /// The name to assign to the new directory. This only has an effect if <see cref="CreateDirectory"/> is <c>true</c>.
        /// </value>
        string DirectoryName { get; }
    }
}
