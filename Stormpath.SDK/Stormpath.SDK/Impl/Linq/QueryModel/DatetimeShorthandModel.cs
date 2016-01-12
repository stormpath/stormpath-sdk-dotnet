// <copyright file="DatetimeShorthandModel.cs" company="Stormpath, Inc.">
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
// </copyright>

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal sealed class DatetimeShorthandModel
    {
        public DatetimeShorthandModel(
            string field,
            int year,
            int? month = null,
            int? day = null,
            int? hour = null,
            int? minute = null,
            int? second = null)
        {
            this.FieldName = field;

            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
        }

        public string FieldName { get; private set; }

        public int Year { get; private set; }

        public int? Month { get; private set; }

        public int? Day { get; private set; }

        public int? Hour { get; private set; }

        public int? Minute { get; private set; }

        public int? Second { get; private set; }
    }
}
