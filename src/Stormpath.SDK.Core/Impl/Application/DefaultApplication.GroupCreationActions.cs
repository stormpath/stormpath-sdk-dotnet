﻿// <copyright file="DefaultApplication.GroupCreationActions.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Group;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        Task<IGroup> IGroupCreationActions.CreateGroupAsync(IGroup group, CancellationToken cancellationToken)
            => GroupCreationActionsShared.CreateGroupAsync(this.GetInternalAsyncDataStore(), this.Groups.Href, group, cancellationToken);

        Task<IGroup> IGroupCreationActions.CreateGroupAsync(IGroup group, IGroupCreationOptions creationOptions, CancellationToken cancellationToken)
            => GroupCreationActionsShared.CreateGroupAsync(this.GetInternalAsyncDataStore(), this.Groups.Href, group, creationOptions, cancellationToken);

        Task<IGroup> IGroupCreationActions.CreateGroupAsync(string name, string description, CancellationToken cancellationToken)
            => GroupCreationActionsShared.CreateGroupAsync(this.GetInternalAsyncDataStore(), this.Groups.Href, name, description, cancellationToken);
    }
}
