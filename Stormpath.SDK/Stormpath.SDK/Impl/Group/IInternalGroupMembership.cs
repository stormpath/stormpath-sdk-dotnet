// <copyright file="IInternalGroupMembership.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Group
{
    /// <summary>
    /// Represents an abstract group membership object with a raw Account and Group href.
    /// </summary>
    internal interface IInternalGroupMembership
    {
        /// <summary>
        /// Gets the Account href.
        /// </summary>
        /// <value>The Account href.</value>
        string AccountHref { get; }

        /// <summary>
        /// Gets the Group href.
        /// </summary>
        /// <value>The Group href.</value>
        string GroupHref { get; }
    }
}
