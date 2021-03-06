﻿// <copyright file="IClock.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Utility
{
    /// <summary>
    /// Injectable clock interface for testing.
    /// </summary>
    internal interface IClock
    {
        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> object that is set to the current date and time
        /// on the current computer, with the offset set to the local time's offset from
        /// Coordinated Universal Time (UTC).
        /// </summary>
        /// <value>
        /// A System.DateTimeOffset object whose date and time is the current local time
        /// and whose offset is the local time zone's offset from Coordinated Universal Time (UTC).
        /// </value>
        DateTimeOffset Now { get; }
    }
}
