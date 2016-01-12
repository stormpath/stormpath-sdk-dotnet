// <copyright file="WindowsVersion.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Introspection
{
    internal class WindowsVersion : ImmutableValueObject<WindowsVersion>
    {
        // List from https://msdn.microsoft.com/en-us/library/windows/desktop/ms724833(v=vs.85).aspx
        private static readonly int NTWorkstation = 1;
        private static readonly int NTDomainController = 2;
        private static readonly int NTServer = 3;
        private static readonly Dictionary<WindowsVersion, string> WindowsVersionLookupTable =
            new Dictionary<WindowsVersion, string>()
            {
                { new WindowsVersion(5, 0), "2000" },
                { new WindowsVersion(5, 1), "XP" },
                { new WindowsVersion(5, 2), "Server-2003" },
                { new WindowsVersion(6, 0, NTWorkstation), "Vista" },
                { new WindowsVersion(6, 0, NTDomainController), "Server-2008" },
                { new WindowsVersion(6, 0, NTServer), "Server-2008" },
                { new WindowsVersion(6, 1, NTWorkstation), "7" },
                { new WindowsVersion(6, 1, NTDomainController), "Server-2008-R2" },
                { new WindowsVersion(6, 1, NTServer), "Server-2008-R2" },
                { new WindowsVersion(6, 2, NTWorkstation), "8" },
                { new WindowsVersion(6, 2, NTDomainController), "Server-2012" },
                { new WindowsVersion(6, 2, NTServer), "Server-2012" },
                { new WindowsVersion(6, 3, NTWorkstation), "8.1" },
                { new WindowsVersion(6, 3, NTDomainController), "Server-2012-R2" },
                { new WindowsVersion(6, 3, NTServer), "Server-2012-R2" },
                { new WindowsVersion(10, 0), "10" },
            };

        public int Major { get; private set; }

        public int Minor { get; private set; }

        public int? ProductType { get; private set; }

        internal WindowsVersion(int major, int minor, int? productType = null)
        {
            this.Major = major;
            this.Minor = minor;
            this.ProductType = productType;
        }

        public override string ToString()
        {
            string version = $"{this.Major}.{this.Minor}" + (this.ProductType == null ? string.Empty : "." + this.ProductType);
            WindowsVersionLookupTable.TryGetValue(this, out version);

            return version;
        }

        public static WindowsVersion Analyze()
        {
            var major = Environment.OSVersion.Version.Major;
            var minor = Environment.OSVersion.Version.Minor;
            var productType = GetProductType();

            return new WindowsVersion(major, minor, productType);
        }

        private static int? GetProductType()
        {
            var operatingSystemVersionInfo = default(SafeNativeMethods.OSVERSIONINFOEX);
            operatingSystemVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(SafeNativeMethods.OSVERSIONINFOEX));

            if (!SafeNativeMethods.GetVersionEx(ref operatingSystemVersionInfo))
            {
                return null;
            }

            return operatingSystemVersionInfo.wProductType;
        }
    }
}
