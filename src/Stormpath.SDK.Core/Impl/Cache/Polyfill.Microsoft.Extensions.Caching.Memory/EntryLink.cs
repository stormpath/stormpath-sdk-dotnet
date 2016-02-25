// <copyright file="EntryLink.cs" company="Stormpath, Inc.">
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
//
// Contains code modified from aspnet/Caching. Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

#if NET451
using System;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    public class EntryLink : IEntryLink
    {
        private bool _disposed;

        public EntryLink()
            : this(parent: null)
        {
        }

        public EntryLink(EntryLink parent)
        {
            Parent = parent;
        }

        public EntryLink Parent { get; }

        public DateTimeOffset? AbsoluteExpiration { get; private set; }

        public void SetAbsoluteExpiration(DateTimeOffset absoluteExpiration)
        {
            if (!AbsoluteExpiration.HasValue)
            {
                AbsoluteExpiration = absoluteExpiration;
            }
            else if (absoluteExpiration < AbsoluteExpiration.Value)
            {
                AbsoluteExpiration = absoluteExpiration;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                EntryLinkHelpers.DisposeLinkingScope();
            }
        }
    }
}
#endif