// <copyright file="RedisTestFixture.cs" company="Stormpath, Inc.">
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
using System.Diagnostics;
using StackExchange.Redis;

namespace Stormpath.SDK.Extensions.Cache.Redis.Tests
{
    public class RedisTestFixture : IDisposable
    {
        private static readonly string RedisConnectionString = "localhost:6379,allowAdmin=true";

        public IConnectionMultiplexer Connection { get; private set; }

        public RedisTestFixture()
        {
            if (!Debugger.IsAttached)
                throw new NotImplementedException();

            this.Connection = ConnectionMultiplexer.Connect(RedisConnectionString);
            var server = this.Connection.GetServer("localhost", 6379);
            server.FlushAllDatabases();
        }

        public void Dispose()
        {
        }
    }
}
