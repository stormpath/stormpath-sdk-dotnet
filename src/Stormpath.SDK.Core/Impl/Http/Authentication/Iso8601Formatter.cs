// <copyright file="Iso8601Formatter.cs" company="Stormpath, Inc.">
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

using System;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    internal static class Iso8601Formatter
    {
        public static string StripSeparators(string iso8601FormattedDatetime)
        {
            if (!iso8601FormattedDatetime.EndsWith("Z"))
            {
                throw new Exception("ISO 8601 datetime must be in UTC.");
            }

            bool containsMillisecondComponent = iso8601FormattedDatetime.Contains(".");
            if (containsMillisecondComponent)
            {
                iso8601FormattedDatetime = iso8601FormattedDatetime.Substring(0, iso8601FormattedDatetime.LastIndexOf("."));
            }

            var result = iso8601FormattedDatetime
                .Replace("-", string.Empty)
                .Replace(":", string.Empty)
                .Replace(".", string.Empty);
            result = result + "Z";

            return result;
        }
    }
}