// <copyright file="GroupCreationActionsShared.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.Group
{
    /// <summary>
    /// Both <see cref="SDK.Application.IApplication">Application</see> and <see cref="SDK.Directory.IDirectory">Directory</see> implement
    /// <see cref="IGroupCreationActions"/>, so this shared class wraps the methods up in a DRY way.
    /// </summary>
    internal static class GroupCreationActionsShared
    {
        public static Task<IGroup> CreateGroupAsync(IInternalAsyncDataStore internalDataStore, string groupsHref, IGroup group, CancellationToken cancellationToken)
            => internalDataStore.CreateAsync(groupsHref, group, cancellationToken);

        public static IGroup CreateGroup(IInternalSyncDataStore internalDataStore, string groupsHref, IGroup group)
            => internalDataStore.Create(groupsHref, group);

        public static Task<IGroup> CreateGroupAsync(IInternalAsyncDataStore internalDataStore, string groupsHref, IGroup group, Action<GroupCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            var builder = new GroupCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return internalDataStore.CreateAsync(groupsHref, group, options, cancellationToken);
        }

        public static IGroup CreateGroup(IInternalSyncDataStore internalDataStore, string groupsHref, IGroup group, Action<GroupCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new GroupCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return internalDataStore.Create(groupsHref, group, options);
        }

        public static Task<IGroup> CreateGroupAsync(IInternalAsyncDataStore internalDataStore, string groupsHref, IGroup group, IGroupCreationOptions creationOptions, CancellationToken cancellationToken)
            => internalDataStore.CreateAsync(groupsHref, group, creationOptions, cancellationToken);

        public static Task<IGroup> CreateGroupAsync(IInternalAsyncDataStore internalDataStore, string groupsHref, string name, string description, CancellationToken cancellationToken)
        {
            var group = internalDataStore.Instantiate<IGroup>()
                .SetName(name)
                .SetDescription(description)
                .SetStatus(GroupStatus.Enabled);

            return internalDataStore.CreateAsync(groupsHref, group, cancellationToken);
        }

        public static IGroup CreateGroup(IInternalSyncDataStore internalDataStore, string groupsHref, IGroup group, IGroupCreationOptions creationOptions)
            => internalDataStore.Create(groupsHref, group, creationOptions);

        public static IGroup CreateGroup(IInternalSyncDataStore internalDataStore, string groupsHref, string name, string description)
        {
            var group = internalDataStore.Instantiate<IGroup>()
                .SetName(name)
                .SetDescription(description)
                .SetStatus(GroupStatus.Enabled);

            return internalDataStore.Create(groupsHref, group);
        }
    }
}
