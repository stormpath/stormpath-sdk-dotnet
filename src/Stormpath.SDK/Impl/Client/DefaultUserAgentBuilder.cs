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

using System;
using Microsoft.Extensions.PlatformAbstractions;

namespace Stormpath.SDK.Impl.Client
{
    internal class DefaultUserAgentBuilder : IUserAgentBuilder
    {
        private readonly IRuntimeEnvironment runtime;
        private readonly IApplicationEnvironment app;
        private readonly string frameworkUserAgent;
        private readonly string language;

        // Lazy ensures this only runs once and is cached.
        private readonly Lazy<string> userAgentValue;

        public DefaultUserAgentBuilder(IRuntimeEnvironment runtimeEnvironment, IApplicationEnvironment appEnvironment, string frameworkUserAgent, string language)
        {
            this.runtime = runtimeEnvironment;
            this.app = appEnvironment;
            this.frameworkUserAgent = frameworkUserAgent;
            this.language = language;

            this.userAgentValue = new Lazy<string>(() => this.Generate());
        }

        public string GetUserAgent() => this.userAgentValue.Value;

        private string Generate()
        {
            var sdkToken = $"stormpath-sdk-dotnet/{this.app.ApplicationVersion}";
            var langToken = $"lang/{this.language.ToLower()}";
            var runtimeToken = this.app.RuntimeFramework.FullName;

            var osToken = this.runtime.OperatingSystem;
            if (!string.IsNullOrEmpty(this.runtime.OperatingSystemVersion))
            {
                osToken = $"{osToken}/{this.runtime.OperatingSystemVersion.Replace(" ", "-")}";
            }

            return string.Join(
                " ",
                this.frameworkUserAgent,
                sdkToken,
                langToken,
                runtimeToken,
                osToken)
            .Trim();
        }
    }
}
