// <copyright file="LoggerExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK
{
    public static class LoggerExtensions
    {
        public static void Trace(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Trace, message, source, null));
        }

        public static void Info(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Info, message, source, null));
        }

        public static void Warn(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Warn, message, source, null));
        }

        public static void Error(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Error, message, source, null));
        }

        public static void Fatal(this ILogger logger, string message, string source = null)
        {
            logger?.Log(new LogEntry(LogLevel.Fatal, message, source, null));
        }

        public static void Warn(this ILogger logger, Exception exception, string message = null, string source = null)
        {
            var logMessage = message.Nullable() ?? exception.Message;
            var logSource = source.Nullable() ?? exception.Source;
            logger?.Log(new LogEntry(LogLevel.Warn, logMessage, source, exception));
        }

        public static void Error(this ILogger logger, Exception exception, string message = null, string source = null)
        {
            var logMessage = message.Nullable() ?? exception.Message;
            var logSource = source.Nullable() ?? exception.Source;
            logger?.Log(new LogEntry(LogLevel.Error, logMessage, source, exception));
        }

        public static void Fatal(this ILogger logger, Exception exception, string message = null, string source = null)
        {
            var logMessage = message.Nullable() ?? exception.Message;
            var logSource = source.Nullable() ?? exception.Source;
            logger?.Log(new LogEntry(LogLevel.Fatal, logMessage, source, exception));
        }
    }
}
