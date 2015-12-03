// <copyright file="LogEntry.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Logging
{
    /// <summary>
    /// A log entry to be passed to a <see cref="ILogger"/>.
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>
        /// The severity of the event.
        /// </summary>
        public readonly LogLevel Severity;

        /// <summary>
        /// The log message.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// The source of the event.
        /// </summary>
        public readonly string Source;

        /// <summary>
        /// The exception associated with the event, or <see langword="null"/>.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class
        /// with the specified severity, message, source, and exception.
        /// </summary>
        /// <param name="severity">The severity of the event.</param>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the event.</param>
        /// <param name="exception">The exception associated with the event, or <see langword="null"/>.</param>
        public LogEntry(LogLevel severity, string message, string source, Exception exception)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (severity < LogLevel.Trace || severity > LogLevel.Fatal)
            {
                throw new ArgumentOutOfRangeException(nameof(severity));
            }

            this.Severity = severity;
            this.Message = message;
            this.Source = source;
            this.Exception = exception;
        }
    }
}
