// <copyright file="WindowsVersion.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Utility
{
    internal partial struct WindowsVersion
    {
        private readonly int major;
        private readonly int minor;
        private readonly int? productType;

        public WindowsVersion(int major, int minor, int? productType = null)
        {
            this.major = major;
            this.minor = minor;
            this.productType = productType;
        }
    }

    internal partial struct WindowsVersion : IEquatable<WindowsVersion>
    {
        private static readonly GenericEqualityComparer<WindowsVersion> Comparer = new GenericEqualityComparer<WindowsVersion>(
            (WindowsVersion x, WindowsVersion y) =>
            {
                bool majorMinorMatch = x.major == y.major && x.minor == y.minor;
                bool productTypeMatchesIfExists = x.productType.HasValue
                        ? x.productType == y.productType
                        : true;

                return majorMinorMatch && productTypeMatchesIfExists;
            },
            (WindowsVersion wv) =>
            {
                return HashCode.Start
                .Hash(wv.major)
                .Hash(wv.minor)
                .Hash(wv.productType);
            });

        public bool Equals(WindowsVersion other)
        {
            return Comparer.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WindowsVersion))
                return false;

            return Comparer.Equals(this, (WindowsVersion)obj);
        }

        public override int GetHashCode()
        {
            return Comparer.GetHashCode(this);
        }
    }
}
