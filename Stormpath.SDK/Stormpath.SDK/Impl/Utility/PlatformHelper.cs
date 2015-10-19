// <copyright file="PlatformHelper.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Reflection;

namespace Stormpath.SDK.Impl.Utility
{
    internal static class PlatformHelper
    {
        // Lazy will cache this result so the semi-expensive reflection call only happens once.
        private static readonly Lazy<bool> IsRunningOnMonoValue = new Lazy<bool>(() =>
        {
            return Type.GetType("Mono.Runtime") != null;
        });

        public static bool IsRunningOnMono()
        {
            return IsRunningOnMonoValue.Value;
        }

        public static string GetMonoRuntimeVersion()
        {
            if (!IsRunningOnMono())
                return "unknown-1";

            var monoRuntimeType = Type.GetType("Mono.Runtime");
            if (monoRuntimeType == null)
                return "unknown-2";

            var displayNameMethod = monoRuntimeType.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            if (displayNameMethod == null)
                return "unknown-3";

            var fullVersionString = displayNameMethod.Invoke(null, null)?.ToString();
            if (string.IsNullOrEmpty(fullVersionString))
                return "unknown-4";

            if (!fullVersionString.Contains(" "))
                return fullVersionString;

            var version = fullVersionString.Split(' ')?[0];
            if (string.IsNullOrEmpty(version))
                return fullVersionString;

            return version;
        }

        public static string GetMonoDotNetFrameworkVersion()
        {
            if (!IsRunningOnMono())
                return "unknown";

            var version = $"{Environment.Version.Major}.{Environment.Version.Minor}";

            return version;
        }

        public static string GetDotNetFrameworkVersion()
        {
            var olderFrameworkVersion = GetDotNet1Thru4Version();
            var newerFrameworkVersion = GetDotNet45OrNewerVersion();

            if (!string.IsNullOrEmpty(newerFrameworkVersion))
                return newerFrameworkVersion;

            if (!string.IsNullOrEmpty(olderFrameworkVersion))
                return olderFrameworkVersion;

            return "unknown";
        }

        private static string GetDotNet1Thru4Version()
        {
            // Returns empty string if no results are found
            try
            {
                var installedVersions = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
                var installedVersionNames = installedVersions?.GetSubKeyNames();
                var latestVersion = installedVersionNames?.LastOrDefault();
                if (string.IsNullOrEmpty(latestVersion))
                    return string.Empty;

                var frameworkVersion = latestVersion.Remove(0, 1); // trim leading 'v'

                int servicePackVersion = 0;
                bool servicePackExists = int.TryParse(installedVersions.OpenSubKey(latestVersion)?.GetValue("SP", 0)?.ToString(), out servicePackVersion);
                if (!servicePackExists || servicePackVersion == 0)
                    return frameworkVersion;

                return $"{frameworkVersion}-SP{servicePackVersion}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string GetDotNet45OrNewerVersion()
        {
            // Returns empty string if no results are found
            try
            {
                using (var ndpKey = Microsoft.Win32.RegistryKey
                    .OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
                    .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
                {
                    int releaseKey;
                    if (!int.TryParse(ndpKey?.GetValue("Release")?.ToString(), out releaseKey))
                        return string.Empty;

                    if (releaseKey >= 393273)
                        return "4.6RC+";
                    if (releaseKey >= 379893)
                        return "4.5.2";
                    if (releaseKey >= 378675)
                        return "4.5.1";
                    if (releaseKey >= 378389)
                        return "4.5";

                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetOSName()
        {
            int platformID = (int)Environment.OSVersion.Platform;

            switch (platformID)
            {
                case (int)PlatformID.MacOSX:
                case (int)PlatformID.Unix:
                case 128:
                    return "Unix";
                case (int)PlatformID.Win32NT:
                case (int)PlatformID.Win32S:
                case (int)PlatformID.Win32Windows:
                case (int)PlatformID.WinCE:
                    return "Windows";
                case (int)PlatformID.Xbox:
                    return "Xbox";
                default:
                    return $"UnkownOS(PlatformID-{platformID})";
            }
        }

        public static string GetOSVersion()
        {
            return IsRunningOnMono()
                ? GetMonoOSVersion()
                : WindowsVersionHelper.GetWindowsOSVersion();
        }

        private static string GetMonoOSVersion()
        {
            var operatingSystemVersion = Environment.OSVersion.Version;
            var version = $"{operatingSystemVersion.Major}.{operatingSystemVersion.Minor}.{operatingSystemVersion.MajorRevision}.{operatingSystemVersion.MinorRevision}";
            if (!string.IsNullOrEmpty(Environment.OSVersion.ServicePack))
                version = $"{version}-{Environment.OSVersion.ServicePack}";

            return version;
        }
    }
}
