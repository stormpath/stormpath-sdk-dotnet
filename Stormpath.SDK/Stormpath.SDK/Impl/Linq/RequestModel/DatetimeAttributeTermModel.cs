// <copyright file="DatetimeAttributeTermModel.cs" company="Stormpath, Inc.">
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
    internal sealed class DatetimeAttributeTermModel : AbstractAttributeTermModel
    {
        public DatetimeAttributeTermModel(
            string field,
            DateTimeOffset? start = null,
            bool? startInclusive = null,
            DateTimeOffset? end = null,
            bool? endInclusive = null)
        {
            this.Field = field;

            if (start.HasValue)
            {
                this.Start = start;
                this.StartInclusive = startInclusive;
            }

            if (end.HasValue)
            {
                this.End = end;
                this.EndInclusive = endInclusive;
            }
        }

        public DateTimeOffset? Start { get; private set; }

        public bool? StartInclusive { get; private set; }

        public DateTimeOffset? End { get; private set; }

        public bool? EndInclusive { get; private set; }
    }
}
