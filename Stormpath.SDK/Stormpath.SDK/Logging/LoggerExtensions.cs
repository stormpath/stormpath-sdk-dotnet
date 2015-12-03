// <copyright file="LoggerExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK
{
    /// <summary>
    /// Provides a set of static methods for sending log messages to an <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log an event at the Trace severity level, with the specified <paramref name="message"/> and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the event.</param>
        public static void Trace(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Trace, message, source, null));
        }

        /// <summary>
        /// Log an event at the Info severity level, with the specified <paramref name="message"/> and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the event.</param>
        public static void Info(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Info, message, source, null));
        }

        /// <summary>
        /// Log an event at the Warn severity level, with the specified <paramref name="message"/> and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the event.</param>
        public static void Warn(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Warn, message, source, null));
        }

        /// <summary>
        /// Log an event at the Error severity level, with the specified <paramref name="message"/> and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the event.</param>
        public static void Error(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Error, message, source, null));
        }

        /// <summary>
        /// Log an event at the Fatal severity level, with the specified <paramref name="message"/> and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="message">The log message.</param>
        /// <param name="source">The source of the event.</param>
        public static void Fatal(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Fatal, message, source, null));
        }

        /// <summary>
        /// Log an event at the Warn severity level, with the specified <paramref name="exception"/> data, <paramref name="message"/>, and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="exception">The exception associated with this event.</param>
        /// <param name="message">The log message; or <c>null</c> to use <see cref="Exception.Message"/>.</param>
        /// <param name="source">The source of the event, or <c>null</c> to use <see cref="Exception.Source"/>.</param>
        public static void Warn(this ILogger logger, Exception exception, string message = null, string source = null)
        {
            var logMessage = message.Nullable() ?? exception.Message;
            var logSource = source.Nullable() ?? exception.Source;
            logger?.Log(new LogEntry(LogLevel.Warn, logMessage, source, exception));
        }

        /// <summary>
        /// Log an event at the Error severity level, with the specified <paramref name="exception"/> data, <paramref name="message"/>, and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="exception">The exception associated with this event.</param>
        /// <param name="message">The log message; or <c>null</c> to use <see cref="Exception.Message"/>.</param>
        /// <param name="source">The source of the event, or <c>null</c> to use <see cref="Exception.Source"/>.</param>
        public static void Error(this ILogger logger, Exception exception, string message = null, string source = null)
        {
            var logMessage = message.Nullable() ?? exception.Message;
            var logSource = source.Nullable() ?? exception.Source;
            logger?.Log(new LogEntry(LogLevel.Error, logMessage, source, exception));
        }

        /// <summary>
        /// Log an event at the Fatal severity level, with the specified <paramref name="exception"/> data, <paramref name="message"/>, and <paramref name="source"/>.
        /// </summary>
        /// <param name="logger">The logger interface.</param>
        /// <param name="exception">The exception associated with this event.</param>
        /// <param name="message">The log message; or <c>null</c> to use <see cref="Exception.Message"/>.</param>
        /// <param name="source">The source of the event, or <c>null</c> to use <see cref="Exception.Source"/>.</param>
        public static void Fatal(this ILogger logger, Exception exception, string message = null, string source = null)
        {
            var logMessage = message.Nullable() ?? exception.Message;
            var logSource = source.Nullable() ?? exception.Source;
            logger?.Log(new LogEntry(LogLevel.Fatal, logMessage, source, exception));
        }
    }
}
