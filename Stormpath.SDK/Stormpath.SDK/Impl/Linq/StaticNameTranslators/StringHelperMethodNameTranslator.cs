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

using Stormpath.SDK.Impl.Linq.RequestModel;

namespace Stormpath.SDK.Impl.Linq.StaticNameTranslators
{
    internal static class StringHelperMethodNameTranslator
    {
        public static bool TryGetValue(string methodName, out StringAttributeMatchingType matchingType)
        {
            bool found = false;
            switch (methodName)
            {
                case "Equals":
                    matchingType = StringAttributeMatchingType.Equals;
                    found = true;
                    break;
                case "StartsWith":
                    matchingType = StringAttributeMatchingType.StartsWith;
                    found = true;
                    break;
                case "EndsWith":
                    matchingType = StringAttributeMatchingType.EndsWith;
                    found = true;
                    break;
                case "Contains":
                    matchingType = StringAttributeMatchingType.Contains;
                    found = true;
                    break;
                default:
                    matchingType = default(StringAttributeMatchingType);
                    break;
            }

            return found;
        }
    }
}
