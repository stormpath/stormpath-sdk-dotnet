// <copyright file="UserAgentBuilder.cs" company="Stormpath, Inc.">
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

#if !NET45 && !NET451
using System;
using System.Runtime.InteropServices;

namespace Stormpath.SDK.Impl.Client
{
    internal class DnxUserAgentBuilder : IUserAgentBuilder
    {
        private readonly string language;

        // Lazy ensures this only runs once and is cached.
        private readonly Lazy<string> userAgentValue;

        public DnxUserAgentBuilder(string language)
        {
            this.language = language;

            this.userAgentValue = new Lazy<string>(Generate);
        }

        public string GetUserAgent() => this.userAgentValue.Value;

        private string Generate()
        {
            var sdkVersion = Introspection.Sdk.Analyze().Version;

            var sdkToken = $"stormpath-sdk-dotnet/{sdkVersion}";

            var languageToken = string.IsNullOrEmpty(this.language)
                ? string.Empty
                : $"lang/{language.ToLower()}";

            var runtimeToken = $"runtime/{RuntimeInformation.FrameworkDescription}";

            var osToken = $"os/{RuntimeInformation.OSDescription}";

            return string.Join(" ",
                sdkToken,
                languageToken,
                runtimeToken,
                osToken)
            .Trim();
        }
    }
}
#endif