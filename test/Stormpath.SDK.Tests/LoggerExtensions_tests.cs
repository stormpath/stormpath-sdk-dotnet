// <copyright file="LoggerExtensions_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Stormpath.SDK.Logging;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class LoggerExtensions_tests
    {
        private void DidReceiveCall(ILogger mockLogger, LogLevel severity, string message)
        {
            mockLogger.Received().Log(Arg.Is<LogEntry>(x =>
                x.Severity == severity &&
                x.Message == message));
        }

        private void DidReceiveCall(ILogger mockLogger, LogLevel severity, Exception exception)
        {
            mockLogger.Received().Log(Arg.Is<LogEntry>(x =>
                x.Severity == severity &&
                x.Message == exception.Message &&
                x.Exception == exception));
        }

        [Fact]
        public void Trace()
        {
            ILogger logger = Substitute.For<ILogger>();

            logger.Trace("Traced.", System.Reflection.MethodBase.GetCurrentMethod().Name);

            this.DidReceiveCall(logger, LogLevel.Trace, "Traced.");
        }

        [Fact]
        public void Info()
        {
            ILogger logger = Substitute.For<ILogger>();

            logger.Info("Info!", System.Reflection.MethodBase.GetCurrentMethod().Name);

            this.DidReceiveCall(logger, LogLevel.Info, "Info!");
        }

        [Fact]
        public void Warn()
        {
            ILogger logger = Substitute.For<ILogger>();

            logger.Warn("Warned.", System.Reflection.MethodBase.GetCurrentMethod().Name);

            this.DidReceiveCall(logger, LogLevel.Warn, "Warned.");
        }

        [Fact]
        public void Error()
        {
            ILogger logger = Substitute.For<ILogger>();

            logger.Error("Error!", System.Reflection.MethodBase.GetCurrentMethod().Name);

            this.DidReceiveCall(logger, LogLevel.Error, "Error!");
        }

        [Fact]
        public void Fatal()
        {
            ILogger logger = Substitute.For<ILogger>();

            logger.Fatal("Fatal :(", System.Reflection.MethodBase.GetCurrentMethod().Name);

            this.DidReceiveCall(logger, LogLevel.Fatal, "Fatal :(");
        }

        [Fact]
        public void Error_with_exception()
        {
            ILogger logger = Substitute.For<ILogger>();
            var exception = new ApplicationException("Something bad happened");

            logger.Error(exception);

            this.DidReceiveCall(logger, LogLevel.Error, exception);
        }

        [Fact]
        public void Fatal_with_exception()
        {
            ILogger logger = Substitute.For<ILogger>();
            var exception = new ApplicationException("Something really bad happened");

            logger.Fatal(exception);

            this.DidReceiveCall(logger, LogLevel.Fatal, exception);
        }

        [Fact]
        public void When_logger_is_null_logging_is_skipped()
        {
            ILogger logger = null;
            logger.Info("Should not throw an error");
        }
    }
}
