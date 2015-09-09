// <copyright file="ResourceAction.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class ResourceAction : Enumeration
    {
        public static ResourceAction Create = new ResourceAction(0, "CREATE");
        public static ResourceAction Read = new ResourceAction(1, "READ");
        public static ResourceAction Update = new ResourceAction(2, "UPDATE");
        public static ResourceAction Delete = new ResourceAction(3, "DELETE");

        private ResourceAction()
        {
        }

        private ResourceAction(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static ResourceAction Parse(string action)
        {
            switch (action.ToUpper())
            {
                case "CREATE": return Create;
                case "READ": return Read;
                case "UPDATE": return Update;
                case "DELETE": return Delete;
                default:
                    throw new ApplicationException($"Could not parse HTTP method value '{action.ToUpper()}'");
            }
        }
    }
}
