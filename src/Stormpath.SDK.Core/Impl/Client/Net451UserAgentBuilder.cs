// <copyright file="Net451UserAgentBuilder.cs" company="Stormpath, Inc.">
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

#if NET45 || NET451
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Stormpath.SDK.Impl.Introspection;

namespace Stormpath.SDK.Impl.Client
{
    internal class Net451UserAgentBuilder : IUserAgentBuilder
    {
        private readonly IPlatform platform;
        private readonly ISdk sdk;
        private readonly string language;

        // Lazy ensures this only runs once and is cached.
        private readonly Lazy<string> userAgentValue;

        public Net451UserAgentBuilder(IPlatform platformInfo, ISdk sdkInfo, string language)
        {
            //this.platform = platformInfo;
            //this.sdk = sdkInfo;
            this.language = language;

            this.userAgentValue = new Lazy<string>(Generate);
        }

        public string GetUserAgent() => this.userAgentValue.Value;

        private string Generate()
        {
            var sdkToken = $"stormpath-sdk-dotnet/{GetSdkVersion()}";

            var languageToken = string.IsNullOrEmpty(this.language)
                ? string.Empty
                : $"lang/{language.ToLower()}";

            var runtimeToken = $"runtime/{RuntimeInformation.FrameworkDescription}";

            var osToken = $"os/{RuntimeInformation.OSDescription}";

            return string.Join(
                " ",
                sdkToken,
                languageToken,
                runtimeToken,
                osToken)
            .Trim();
        }

        private static string GetSdkVersion()
        {
            var sdkVersion = typeof(Client.DefaultClient).GetTypeInfo()
                .Assembly
                .GetName()
                .Version;

            return $"{sdkVersion.Major}.{sdkVersion.Minor}.{sdkVersion.Build}";
        }

        //private string GetRuntimeInfo()
        //{
        //    string runtimeInfo;

        //    if (this.platform.IsRunningOnMono)
        //    {
        //        runtimeInfo = $"mono/{this.platform.MonoRuntimeVersion} mono-dotnetframework/{this.platform.FrameworkVersion}";
        //    }
        //    else
        //    {
        //        runtimeInfo = $"dotnetframework/{this.platform.FrameworkVersion}";
        //    }

        //    return runtimeInfo;
        //}
    }
}
#endif
