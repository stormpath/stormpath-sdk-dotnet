// <copyright file="WhereAttributeTermInProgressModel.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Linq.RequestModel
{
    internal class WhereAttributeTermInProgressModel
    {
        public string Field { get; set; }

        public Type TargetType { get; set; }

        // Used when building an eventual StringAttributeTermModel
        public string StringValue { get; set; }

        public StringAttributeMatchingType? StringMatchType { get; set; } = StringAttributeMatchingType.Equals;

        // Used when building an eventual DatetimeAttributeTermModel
        public DateTimeOffset? DateValue { get; set; }

        public bool? DateGreaterThan { get; set; }

        public bool? DateOrEqual { get; set; }

        public bool IsStringTermComplete()
        {
            return TargetType == typeof(StringAttributeTermModel) &&
                !string.IsNullOrEmpty(StringValue) &&
                StringMatchType.HasValue;
        }

        public bool IsDateTermComplete()
        {
            return TargetType == typeof(DatetimeAttributeTermModel) &&
                DateValue.HasValue &&
                DateGreaterThan.HasValue &&
                DateOrEqual.HasValue;
        }
    }
}
