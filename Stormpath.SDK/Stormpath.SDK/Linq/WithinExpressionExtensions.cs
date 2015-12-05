// <copyright file="WithinExpressionExtensions.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

namespace Stormpath.SDK
{
    /// <summary>
    /// Provides a set of static methods for making shorthand date queries within LINQ-to-Stormpath.
    /// </summary>
    public static class WithinExpressionExtensions
    {
        /// <summary>
        /// Asserts that a <see cref="DateTimeOffset"/> falls within a particular year.
        /// </summary>
        /// <param name="field">The date field to compare.</param>
        /// <param name="year">The year.</param>
        /// <returns><see langword="true"/> if this <see cref="DateTimeOffset"/> falls within the specified <paramref name="year"/>.</returns>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        public static bool Within(this DateTimeOffset field, int year)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        /// <summary>
        /// Asserts that a <see cref="DateTimeOffset"/> falls within a particular month and year.
        /// </summary>
        /// <param name="field">The date field to compare.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns><see langword="true"/> if this <see cref="DateTimeOffset"/> falls within the specified
        /// <paramref name="month"/> and <paramref name="year"/>.</returns>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        public static bool Within(this DateTimeOffset field, int year, int month)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        /// <summary>
        /// Asserts that a <see cref="DateTimeOffset"/> falls within a particular day, month, and year.
        /// </summary>
        /// <param name="field">The date field to compare.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns><see langword="true"/> if this <see cref="DateTimeOffset"/> falls within the specified
        /// <paramref name="day"/>, <paramref name="month"/>, and <paramref name="year"/>.</returns>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        public static bool Within(this DateTimeOffset field, int year, int month, int day)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        /// <summary>
        /// Asserts that a <see cref="DateTimeOffset"/> falls within a particular hour
        /// on a particular day, month, and year.
        /// </summary>
        /// <param name="field">The date field to compare.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <returns><see langword="true"/> if this <see cref="DateTimeOffset"/> falls within the specified <paramref name="hour"/>
        /// on the specified <paramref name="day"/>, <paramref name="month"/>, and <paramref name="year"/>.</returns>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        public static bool Within(this DateTimeOffset field, int year, int month, int day, int hour)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        /// <summary>
        /// Asserts that a <see cref="DateTimeOffset"/> falls within a particular hour and minute
        /// on a particular day, month, and year.
        /// </summary>
        /// <param name="field">The date field to compare.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <returns><see langword="true"/> if this <see cref="DateTimeOffset"/> falls within the specified <paramref name="minute"/> and <paramref name="hour"/>
        /// on the specified <paramref name="day"/>, <paramref name="month"/>, and <paramref name="year"/>.</returns>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        public static bool Within(this DateTimeOffset field, int year, int month, int day, int hour, int minute)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        /// <summary>
        /// Asserts that a <see cref="DateTimeOffset"/> falls within a particular hour, minute, and second
        /// on a particular day, month, and year.
        /// </summary>
        /// <param name="field">The date field to compare.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns><see langword="true"/> if this <see cref="DateTimeOffset"/> falls within the specified
        /// <paramref name="second"/>, <paramref name="minute"/>, and <paramref name="hour"/>
        /// on the specified <paramref name="day"/>, <paramref name="month"/>, and <paramref name="year"/>.</returns>
        /// <exception cref="NotSupportedException">This method can only be used from inside a LINQ Where predicate.</exception>
        public static bool Within(this DateTimeOffset field, int year, int month, int day, int hour, int minute, int second)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }
    }
}
