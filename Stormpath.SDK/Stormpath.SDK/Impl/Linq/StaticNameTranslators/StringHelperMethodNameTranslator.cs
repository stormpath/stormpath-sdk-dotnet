// <copyright file="StringHelperMethodNameTranslator.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Impl.Linq.RequestModel;

namespace Stormpath.SDK.Impl.Linq.StaticNameTranslators
{
    internal static class StringHelperMethodNameTranslator
    {
        private static readonly Dictionary<string, StringAttributeMatchingType>
            ValidNames = new Dictionary<string, StringAttributeMatchingType>()
            {
                { "Equals", StringAttributeMatchingType.Equals },
                { "StartsWith", StringAttributeMatchingType.StartsWith },
                { "EndsWith", StringAttributeMatchingType.EndsWith },
                { "Contains", StringAttributeMatchingType.Contains }
            };

        public static bool TryGetValue(string methodName, out StringAttributeMatchingType matchingType)
        {
            return ValidNames.TryGetValue(methodName, out matchingType);
        }
    }
}
