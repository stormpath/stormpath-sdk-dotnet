// <copyright file="MemoryCache.cs" company="Stormpath, Inc.">
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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    internal static class EntryLinkHelpers
    {
        private const string ContextLinkDataName = "EntryLinkHelpers.ContextLink";

        public static EntryLink ContextLink
        {
            get
            {
                var handle = CallContext.LogicalGetData(ContextLinkDataName) as ObjectHandle;

                if (handle == null)
                {
                    return null;
                }

                return handle.Unwrap() as EntryLink;
            }
            set
            {
                CallContext.LogicalSetData(ContextLinkDataName, new ObjectHandle(value));
            }
        }

        internal static IEntryLink CreateLinkingScope()
        {
            var parentLink = ContextLink;
            var newLink = new EntryLink(parent: parentLink);
            ContextLink = newLink;
            return newLink;
        }

        internal static void DisposeLinkingScope()
        {
            var currentLink = ContextLink;
            var priorLink = ((EntryLink)currentLink).Parent;
            ContextLink = priorLink;
        }
    }
}
#endif