// <copyright file="FakeApiKey.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public static class FakeApiKey
    {
        [Obsolete("Replace this before 1.0")]
        public static IClientApiKey Create(bool valid)
        {
            return new MockApiKey()
            {
                Id = "FooId",
                Secret = "FooSecret",
                Valid = valid
            };
        }
    }

    internal class MockApiKey : IClientApiKey
    {
        public string Id { get; set; }

        public string Secret { get; set; }

        public bool Valid { get; set; }

        string IClientApiKey.GetId() => Id;

        string IClientApiKey.GetSecret() => Secret;

        bool IClientApiKey.IsValid() => Valid;
    }
}
