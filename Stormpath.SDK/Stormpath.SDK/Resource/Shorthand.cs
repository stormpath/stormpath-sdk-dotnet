// <copyright file="Shorthand.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Resource
{
    public struct Shorthand
    {
        private int year;
        private int? month;
        private int? day;
        private int? hour;
        private int? minute;
        private int? second;
        private TimeSpan offset;

        private Shorthand(int year, TimeSpan offset)
        {
            this.year = year;
            this.month = null;
            this.day = null;
            this.hour = null;
            this.minute = null;
            this.second = null;
            this.offset = offset;
        }

        private Shorthand(int year, int month, TimeSpan offset)
        {
            this.year = year;
            this.month = month;
            this.day = null;
            this.hour = null;
            this.minute = null;
            this.second = null;
            this.offset = offset;
        }

        private Shorthand(int year, int month, int day, TimeSpan offset)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = null;
            this.minute = null;
            this.second = null;
            this.offset = offset;
        }

        private Shorthand(int year, int month, int day, int hour, TimeSpan offset)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = null;
            this.second = null;
            this.offset = offset;
        }

        private Shorthand(int year, int month, int day, int hour, int minute, TimeSpan offset)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = null;
            this.offset = offset;
        }

        private Shorthand(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.offset = offset;
        }

        public static implicit operator DateTimeOffset(Shorthand shorthand)
        {
            return new DateTimeOffset(
                shorthand.year,
                shorthand.month ?? 0,
                shorthand.day ?? 0,
                shorthand.hour ?? 0,
                shorthand.minute ?? 0,
                shorthand.second ?? 0,
                shorthand.offset);
        }

        private static TimeSpan GetLocalOffset()
        {
            return TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
        }

        public static Shorthand Year(int year)
        {
            return Year(year, GetLocalOffset());
        }

        public static Shorthand Year(int year, TimeSpan offset)
        {
            return new Shorthand(year, offset);
        }

        public static Shorthand Month(int year, int month)
        {
            return Month(year, month, GetLocalOffset());
        }

        public static Shorthand Month(int year, int month, TimeSpan offset)
        {
            return new Shorthand(year, month, offset);
        }

        public static Shorthand Day(int year, int month, int day)
        {
            return Day(year, month, day, GetLocalOffset());
        }

        public static Shorthand Day(int year, int month, int day, TimeSpan offset)
        {
            return new Shorthand(year, month, day, offset);
        }

        public static Shorthand Hour(int year, int month, int day, int hour)
        {
            return Hour(year, month, day, hour, GetLocalOffset());
        }

        public static Shorthand Hour(int year, int month, int day, int hour, TimeSpan offset)
        {
            return new Shorthand(year, month, day, hour, offset);
        }

        public static Shorthand Minute(int year, int month, int day, int hour, int minute)
        {
            return Minute(year, month, day, hour, minute, GetLocalOffset());
        }

        public static Shorthand Minute(int year, int month, int day, int hour, int minute, TimeSpan offset)
        {
            return new Shorthand(year, month, day, hour, minute, offset);
        }

        public static Shorthand Second(int year, int month, int day, int hour, int minute, int second)
        {
            return Second(year, month, day, hour, minute, second, GetLocalOffset());
        }

        public static Shorthand Second(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
        {
            return new Shorthand(year, month, day, hour, minute, second, offset);
        }
    }
}