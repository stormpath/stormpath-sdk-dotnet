// <copyright file="PrecisionExpressionExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Linq;

namespace Stormpath.SDK
{
    /// <summary>
    /// Provides a set of static methods for specifying floating-point precision in LINQ-to-Stormpath.
    /// </summary>
    public static class PrecisionExpressionExtensions
    {
        /// <summary>
        /// Specifies the number of decimal places to use in the query.
        /// </summary>
        /// <param name="target">The base number.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        /// <returns>An identifier that contains the specified precision for a LINQ-to-Stormpath query.</returns>
        public static SpecifiedPrecisionToken WithPlaces(this int target, int decimalPlaces)
            => new SpecifiedPrecisionToken(target, decimalPlaces);

        /// <summary>
        /// Specifies the number of decimal places to use in the query.
        /// </summary>
        /// <param name="target">The base number.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        /// <returns>An identifier that contains the specified precision for a LINQ-to-Stormpath query.</returns>
        public static SpecifiedPrecisionToken WithPlaces(this double target, int decimalPlaces)
            => new SpecifiedPrecisionToken(target, decimalPlaces);
    }
}
