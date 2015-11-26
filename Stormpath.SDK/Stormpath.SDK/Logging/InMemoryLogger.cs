// <copyright file="InMemoryLogger.cs" company="Stormpath, Inc.">
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
using System.Text;

namespace Stormpath.SDK.Logging
{
    public sealed class InMemoryLogger : ILogger
    {
        private readonly StringBuilder builder
            = new StringBuilder();

        public InMemoryLogger()
        {
        }

        void ILogger.Log(LogEntry entry)
        {
            var logEntryBuilder = new StringBuilder()
                .Append($"{DateTimeOffset.Now.ToString()} [{entry.Severity.ToString().ToUpper()}] ");

            if (!string.IsNullOrEmpty(entry.Source))
                logEntryBuilder.Append($"{entry.Source} - ");

            logEntryBuilder.Append(entry.Message);

            if (entry.Exception != null)
                logEntryBuilder.Append($" (Exception: '{entry.Exception.Message}' in {entry.Exception.Source} (Inner: {entry.Exception.InnerException?.ToString()}))");

            this.builder.AppendLine(logEntryBuilder.ToString());
        }

        public override string ToString()
            => this.builder.ToString();
    }
}
