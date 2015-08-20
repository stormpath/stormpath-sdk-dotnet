// <copyright file="UserAgentBuilder.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Http
{
    internal static class UserAgentBuilder
    {
        // Lazy ensures this only runs once and is cached.
        private static readonly Lazy<string> SdkUserAgentValue = new Lazy<string>(() => GetSdkUserAgentImpl());

        private static string GetSdkUserAgentImpl()
        {
            return string.Join(
                " ",
                string.Join(" ", GetInstalledIntegrationsInfo()),
                GetSdkInfo(),
                GetRuntimeInfo(),
                GetPlatformInfo())
            .Trim();
        }

        public static string GetUserAgent()
        {
            return SdkUserAgentValue.Value;
        }

        private static IEnumerable<string> GetInstalledIntegrationsInfo()
        {
            // TODO
            return Enumerable.Empty<string>();
        }

        private static string GetSdkInfo()
        {
            var version = typeof(UserAgentBuilder).Assembly.GetName()
                .Version.ToString()
                .Replace(".0", string.Empty); // remove unnecessary .0.0

            return $"stormpath-sdk-csharp/{version}";
        }

        private static string GetRuntimeInfo()
        {
            string runtimeInfo;

            if (PlatformHelper.IsRunningOnMono())
            {
                runtimeInfo = $"mono/{PlatformHelper.GetMonoRuntimeVersion()} mono-dotnetframework/{PlatformHelper.GetMonoDotNetFrameworkVersion()}";
            }
            else
            {
                runtimeInfo = $"dotnetframework/{PlatformHelper.GetDotNetFrameworkVersion()}";
            }

            return runtimeInfo;
        }

        private static string GetPlatformInfo()
        {
            return $"{PlatformHelper.GetOSName()}/{PlatformHelper.GetOSVersion()}";
        }
    }
}
