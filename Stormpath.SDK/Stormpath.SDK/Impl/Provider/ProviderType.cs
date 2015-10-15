// <copyright file="ProviderType.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Provider
{
    /// <summary>
    /// Represents provider-specific names and class mappings.
    /// </summary>
    internal sealed class ProviderType : Enumeration
    {
        public static ProviderType Stormpath = new ProviderType(0, "stormpath");

        public static ProviderType Facebook = new ProviderType(1, "facebook");

        public static ProviderType Github = new ProviderType(2, "github");

        public static ProviderType Google = new ProviderType(3, "google");

        public static ProviderType LinkedIn = new ProviderType(4, "linkedin");

        private ProviderType()
        {
        }

        private ProviderType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}