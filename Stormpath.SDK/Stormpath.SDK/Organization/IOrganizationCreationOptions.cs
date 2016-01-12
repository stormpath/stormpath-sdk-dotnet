// <copyright file="IOrganizationCreationOptions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Organization
{
    /// <summary>
    /// Represents options for an <see cref="IOrganization">Organization</see> creation request.
    /// </summary>
    public interface IOrganizationCreationOptions : ICreationOptions
    {
        /// <summary>
        /// Gets a value indicating whether to create a new <see cref="Directory.IDirectory">Directory</see> along with the Organization.
        /// </summary>
        /// <value>
        /// <para>
        /// If <see langword="true"/>, a new Directory will be created. The new Directory will automatically be assigned as the Organization's
        /// default Account and Group store.
        /// If the <see cref="DirectoryName"/> property is not null, the new Directory will be created with that name.
        /// Otherwise, the Directory will be automatically named based on heuristics to ensure a guaranteed unique name based on the Organization.
        /// </para>
        /// <para>
        /// If <see langword="false"/>, no directory will be created.
        /// </para>
        /// </value>
        bool CreateDirectory { get; }

        /// <summary>
        /// Gets the name to use when creating a new <see cref="Directory.IDirectory">Directory</see> for this Organization.
        /// </summary>
        /// <value>
        /// The name to assign to the new Directory. This only has an effect if <see cref="CreateDirectory"/> is <see langword="true"/>.
        /// </value>
        string DirectoryName { get; }
    }
}
