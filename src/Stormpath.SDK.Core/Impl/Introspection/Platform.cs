// <copyright file="Platform.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Reflection;

namespace Stormpath.SDK.Impl.Introspection
{
    internal sealed class Platform : IPlatform
    {
        public bool IsPlatformUnix { get; private set; }

        public bool IsRunningOnMono { get; private set; }

        public string MonoRuntimeVersion { get; private set; }

        public string FrameworkVersion { get; private set; }

        public string OsName { get; private set; }

        public string OsVersion { get; private set; }

        private Platform()
        {
        }

        public static Platform Analyze()
        {
            var info = new Platform();

            info.IsPlatformUnix = GetIsPlatformUnix();

            info.IsRunningOnMono = Type.GetType("Mono.Runtime") != null;

            info.MonoRuntimeVersion = info.IsRunningOnMono
                ? GetMonoRuntimeVersion()
                : null;

            info.FrameworkVersion = info.IsRunningOnMono
                ? GetMonoDotNetFrameworkVersion()
                : GetDotNetFrameworkVersion();

            info.OsName = GetOSName();

            info.OsVersion = info.IsRunningOnMono
                ? GetMonoOSVersion()
                : GetWindowsOSVersion();

            return info;
        }

        private static bool GetIsPlatformUnix()
        {
            var p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }

        private static string GetMonoRuntimeVersion()
        {
            var monoRuntimeType = Type.GetType("Mono.Runtime");
            if (monoRuntimeType == null)
            {
                return "unknown-2";
            }

            var displayNameMethod = monoRuntimeType.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            if (displayNameMethod == null)
            {
                return "unknown-3";
            }

            var fullVersionString = displayNameMethod.Invoke(null, null)?.ToString();
            if (string.IsNullOrEmpty(fullVersionString))
            {
                return "unknown-4";
            }

            if (!fullVersionString.Contains(" "))
            {
                return fullVersionString;
            }

            var version = fullVersionString.Split(' ')?[0];
            if (string.IsNullOrEmpty(version))
            {
                return fullVersionString;
            }

            return version;
        }

        private static string GetMonoDotNetFrameworkVersion()
        {
            var version = $"{Environment.Version.Major}.{Environment.Version.Minor}";

            return version;
        }

        private static string GetDotNetFrameworkVersion()
        {
            var olderFrameworkVersion = GetDotNet1Thru4Version();
            var newerFrameworkVersion = GetDotNet45OrNewerVersion();

            if (!string.IsNullOrEmpty(newerFrameworkVersion))
            {
                return newerFrameworkVersion;
            }

            if (!string.IsNullOrEmpty(olderFrameworkVersion))
            {
                return olderFrameworkVersion;
            }

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
                {
                    return string.Empty;
                }

                var frameworkVersion = latestVersion.Remove(0, 1); // trim leading 'v'

                int servicePackVersion = 0;
                bool servicePackExists = int.TryParse(installedVersions.OpenSubKey(latestVersion)?.GetValue("SP", 0)?.ToString(), out servicePackVersion);
                if (!servicePackExists || servicePackVersion == 0)
                {
                    return frameworkVersion;
                }

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
                    {
                        return string.Empty;
                    }

                    if (releaseKey >= 393273)
                    {
                        return "4.6RC+";
                    }

                    if (releaseKey >= 379893)
                    {
                        return "4.5.2";
                    }

                    if (releaseKey >= 378675)
                    {
                        return "4.5.1";
                    }

                    if (releaseKey >= 378389)
                    {
                        return "4.5";
                    }

                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string GetOSName()
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

        private static string GetMonoOSVersion()
        {
            var operatingSystemVersion = Environment.OSVersion.Version;
            var version = $"{operatingSystemVersion.Major}.{operatingSystemVersion.Minor}.{operatingSystemVersion.MajorRevision}.{operatingSystemVersion.MinorRevision}";
            if (!string.IsNullOrEmpty(Environment.OSVersion.ServicePack))
            {
                version = $"{version}-{Environment.OSVersion.ServicePack}";
            }

            return version;
        }

        private static string GetWindowsOSVersion()
        {
            return $"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}";
        }
    }
}
#endif