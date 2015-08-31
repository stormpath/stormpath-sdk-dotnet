// <copyright file="HttpMethod.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Http.Support
{
    internal sealed class HttpMethod : Enumeration
    {
        public static HttpMethod Get = new HttpMethod(0, "GET");
        public static HttpMethod Head = new HttpMethod(10, "HEAD");
        public static HttpMethod Post = new HttpMethod(20, "POST");
        public static HttpMethod Put = new HttpMethod(30, "PUT");
        public static HttpMethod Patch = new HttpMethod(40, "PATCH");
        public static HttpMethod Delete = new HttpMethod(50, "DELETE");
        public static HttpMethod Options = new HttpMethod(60, "OPTIONS");
        public static HttpMethod Trace = new HttpMethod(70, "TRACE");
        public static HttpMethod Connect = new HttpMethod(80, "CONNECT");

        private static readonly Dictionary<string, HttpMethod> LookupMap = new Dictionary<string, HttpMethod>()
        {
            { Get.DisplayName, Get },
            { Head.DisplayName, Head },
            { Post.DisplayName, Post },
            { Put.DisplayName, Put },
            { Patch.DisplayName, Patch },
            { Delete.DisplayName, Delete },
            { Options.DisplayName, Options },
            { Trace.DisplayName, Trace },
            { Connect.DisplayName, Connect },
        };

        private HttpMethod()
        {
        }

        private HttpMethod(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static HttpMethod Parse(string method)
        {
            HttpMethod found;
            if (!LookupMap.TryGetValue(method.ToUpper(), out found))
                throw new ApplicationException($"Could not parse HTTP method value '{method.ToUpper()}'");

            return found;
        }

        public static HttpMethod Parse(HttpMethod existing)
        {
            return Parse(existing.DisplayName);
        }
    }
}
