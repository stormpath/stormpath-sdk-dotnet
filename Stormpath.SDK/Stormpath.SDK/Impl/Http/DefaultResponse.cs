// <copyright file="DefaultResponse.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultResponse : HttpMessageBase, IHttpResponse
    {
        private readonly int httpStatus;
        private readonly HttpHeaders headers;
        private readonly string body;

        public DefaultResponse(int httpStatus, HttpHeaders headers, string body)
        {
            this.httpStatus = httpStatus;
            this.headers = headers;
            this.body = body.Nullable() ?? string.Empty;
        }

        public override string Body => body;

        public override HttpHeaders Headers => headers;

        public int HttpStatus => httpStatus;

        public bool IsError => IsServerError(HttpStatus) || IsClientError(HttpStatus);

        private static bool IsServerError(int code) => code >= 500 && code < 600;

        private static bool IsClientError(int code) => code >= 400 && code < 500;
    }
}
