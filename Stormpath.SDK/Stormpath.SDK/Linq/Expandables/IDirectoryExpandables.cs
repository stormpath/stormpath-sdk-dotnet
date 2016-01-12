// <copyright file="IDirectoryExpandables.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Linq.Expandables
{
    /// <summary>
    /// Represents resources that can be expanded from a <see cref="Directory.IDirectory">Directory</see>.
    /// </summary>
    public interface IDirectoryExpandables :
        IExpandableAccounts,
        IExpandableApplications,
        IExpandableCustomData,
        IExpandableGroups,
        IExpandableTenant
    {
        /// <summary>
        /// Expands the <c>provider</c> resource.
        /// </summary>
        /// <returns>Not applicable.</returns>
        IProvider GetProvider();
    }
}
