// <copyright file="ProviderType.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Provider
{
    /// <summary>
    /// Represents provider-specific names and class mappings.
    /// </summary>
    internal sealed class ProviderType : StringEnumeration
    {
        public static ProviderType Stormpath = new ProviderType("stormpath");

        public static ProviderType Facebook = new ProviderType("facebook");

        public static ProviderType Github = new ProviderType("github");

        public static ProviderType Google = new ProviderType("google");

        public static ProviderType LinkedIn = new ProviderType("linkedin");

        private ProviderType()
        {
        }

        private ProviderType(string value)
            : base(value)
        {
        }
    }
}