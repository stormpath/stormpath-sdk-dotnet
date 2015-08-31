// <copyright file="DefaultHttpResponse.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultHttpResponse : HttpMessageBase, IHttpResponse
    {
        private readonly int httpStatus;
        private readonly string responsePhrase;
        private readonly HttpHeaders headers;
        private readonly string body;
        private readonly string bodyContentType;

        public DefaultHttpResponse(int httpStatus, string responsePhrase, HttpHeaders headers, string body, string bodyContentType)
        {
            this.httpStatus = httpStatus;
            this.responsePhrase = responsePhrase;
            this.headers = headers;
            this.body = body;
            this.bodyContentType = bodyContentType;
        }

        public override string Body => this.body;

        public override string BodyContentType => this.bodyContentType;

        public override HttpHeaders Headers => this.headers;

        public int HttpStatus => this.httpStatus;

        public string ResponsePhrase => this.responsePhrase;

        public bool IsError => IsServerError(this.HttpStatus) || IsClientError(this.HttpStatus);

        private static bool IsServerError(int code) => code >= 500 && code < 600;

        private static bool IsClientError(int code) => code >= 400 && code < 500;
    }
}
