// <copyright file="UnixDate.cs" company="Stormpath, Inc.">
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

using System;

namespace Stormpath.SDK.Impl.Utility
{
    internal static class UnixDate
    {
        private static readonly DateTimeOffset UnixEpoch 
            = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private static readonly long MaxValue =
            ToLong(new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero));

        /// <summary>
        /// Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>Seconds since Unix epoch.</returns>
        public static long ToLong(DateTimeOffset date)
            => (long)Math.Round((date.ToUniversalTime() - UnixEpoch).TotalSeconds);

        /// <summary>
        /// Gets this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>Seconds since Unix epoch, or <see langword="null"/> if <paramref name="date"/> is null.</returns>
        public static long? ToLong(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return ToLong(date.Value);
        }

        public static DateTimeOffset FromLong(long timestamp)
        {
            bool decodeAsMilliseconds = timestamp > MaxValue;

            return decodeAsMilliseconds
                ? UnixEpoch.AddSeconds(timestamp / 1000)
                : UnixEpoch.AddSeconds(timestamp);
        }

        public static DateTimeOffset? FromLong(long? timestamp)
        {
            if (!timestamp.HasValue)
            {
                return null;
            }

            return FromLong(timestamp.Value);
        }
    }
}
