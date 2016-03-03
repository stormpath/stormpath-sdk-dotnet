﻿// <copyright file="SpecifiedPrecisionToken.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Linq
{
    /// <summary>
    /// Represents a token specifying an explicit number of decimal places to use in a LINQ-to-Stormpath query.
    /// </summary>
    public struct SpecifiedPrecisionToken
    {
        public SpecifiedPrecisionToken(double value)
        {
            this.Value = value;
            this.Places = null;
        }

        public SpecifiedPrecisionToken(double value, int places)
        {
            this.Value = value;
            this.Places = places;
        }

        public override string ToString()
        {
            return this.Places == null
                ? this.Value.ToString()
                : this.Value.ToString($"#.{new string('0', this.Places.Value)}");
        }

        public double Value { get; private set; }

        public int? Places { get; private set; }

        public static implicit operator float(SpecifiedPrecisionToken token)
        {
            throw new NotSupportedException("Direct calls are not supported. Use WithPlaces() from inside a LINQ Where predicate.");
        }

        public static implicit operator double(SpecifiedPrecisionToken token)
        {
            throw new NotSupportedException("Direct calls are not supported. Use WithPlaces() from inside a LINQ Where predicate.");
        }

        public static bool operator >(double left, SpecifiedPrecisionToken right)
        {
            throw new NotSupportedException("Direct calls are not supported. Use WithPlaces() from inside a LINQ Where predicate.");
        }

        public static bool operator <(double left, SpecifiedPrecisionToken right)
        {
            throw new NotSupportedException("Direct calls are not supported. Use WithPlaces() from inside a LINQ Where predicate.");
        }
    }
}