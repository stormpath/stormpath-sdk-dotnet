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
    internal sealed class WhereAttributeTermInProgressModel
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

        // Used when parsing a Within extension expression
        public DatetimeShorthandAttributeTermModel ShorthandModel { get; set; }

        public bool IsStringTermComplete()
        {
            return this.TargetType == typeof(StringAttributeTermModel) &&
                !string.IsNullOrEmpty(this.StringValue) &&
                this.StringMatchType.HasValue;
        }

        public void ResetStringTerm()
        {
            this.TargetType = null;
            this.StringValue = null;
            this.StringMatchType = null;
        }

        public bool IsDateTermComplete()
        {
            return this.TargetType == typeof(DatetimeAttributeTermModel) &&
                this.DateValue.HasValue &&
                this.DateGreaterThan.HasValue &&
                this.DateOrEqual.HasValue;
        }

        public void ResetDateTerm()
        {
            this.TargetType = null;
            this.DateValue = null;
            this.DateGreaterThan = null;
            this.DateOrEqual = null;
        }

        public bool IsShorthandDateTermComplete()
        {
            return this.ShorthandModel != null;
        }

        public void ResetShorthandDateTerm()
        {
            this.ShorthandModel = null;
        }
    }
}
