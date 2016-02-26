// <copyright file="IGroupCreationActionsSync.cs" company="Stormpath, Inc.">
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

using System;

namespace Stormpath.SDK.Group
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IGroupCreationActions"/>.
    /// </summary>
    internal interface IGroupCreationActionsSync
    {
        /// <summary>
        /// Synchronous counterpart to <c>IGroupCreationActions.CreateGroupAsync(IGroup, System.Threading.CancellationToken)</c>.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        IGroup CreateGroup(IGroup group);

        /// <summary>
        /// Synchronous counterpart to <c>IGroupCreationActions.CreateGroupAsync(IGroup, Action{GroupCreationOptionsBuilder}, System.Threading.CancellationToken)</c>.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <param name="creationOptionsAction">
        /// An inline builder for an instance of <see cref="IGroupCreationOptions"/>, which will be used when sending the request.
        /// </param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        IGroup CreateGroup(IGroup group, Action<GroupCreationOptionsBuilder> creationOptionsAction);

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroupCreationActions.CreateGroupAsync(IGroup, IGroupCreationOptions, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <param name="creationOptions">An <see cref="IGroupCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        IGroup CreateGroup(IGroup group, IGroupCreationOptions creationOptions);

        /// <summary>
        /// Synchronous counterpart to <see cref="IGroupCreationActions.CreateGroupAsync(string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="name">The new Group's name.</param>
        /// <param name="description">The new Group's description text.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        IGroup CreateGroup(string name, string description);
    }
}
