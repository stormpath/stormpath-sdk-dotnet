// <copyright file="UriQualifier.cs" company="Stormpath, Inc.">
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
using System.Text;

namespace Stormpath.SDK.Impl.Http.Support
{
    internal sealed class UriQualifier
    {
        private readonly string baseUrl;

        public UriQualifier(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("baseUrl");

            this.baseUrl = baseUrl;
        }

        public string EnsureFullyQualified(string href)
        {
            if (string.IsNullOrEmpty(href))
                return href;

            if (IsFullyQualified(href))
                return href;

            var fullyQualified = new StringBuilder(this.baseUrl);
            if (!href.StartsWith("/"))
                fullyQualified.Append("/");

            fullyQualified.Append(href);
            return fullyQualified.ToString();
        }

        public static bool IsFullyQualified(string href)
        {
            bool tooShort = string.IsNullOrEmpty(href) || href.Length < 5;
            if (tooShort)
                return false;

            bool startsWithHttp = href.Substring(0, 4).Equals("http", StringComparison.InvariantCultureIgnoreCase);
            return startsWithHttp;
        }
    }
}
