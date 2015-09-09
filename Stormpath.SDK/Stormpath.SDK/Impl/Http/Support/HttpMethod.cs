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

        private HttpMethod()
        {
        }

        private HttpMethod(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static HttpMethod Parse(string method)
        {
            switch (method.ToUpper())
            {
                case "GET": return Get;
                case "HEAD": return Head;
                case "POST": return Post;
                case "PUT": return Put;
                case "PATCH": return Patch;
                case "DELETE": return Delete;
                case "OPTIONS": return Options;
                case "TRACE": return Trace;
                case "CONNECT": return Connect;
                default:
                    throw new ApplicationException($"Could not parse HTTP method value '{method.ToUpper()}'");
            }
        }

        public static HttpMethod Parse(HttpMethod existing)
        {
            return Parse(existing.DisplayName);
        }
    }
}
