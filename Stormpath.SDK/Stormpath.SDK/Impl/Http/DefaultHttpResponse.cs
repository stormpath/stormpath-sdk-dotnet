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

using Stormpath.SDK.Http;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultHttpResponse : HttpMessageBase, IHttpResponse
    {
        private readonly int statusCode;
        private readonly string responsePhrase;
        private readonly HttpHeaders headers;
        private readonly string body;
        private readonly string bodyContentType;
        private readonly ResponseErrorType errorType;

        public DefaultHttpResponse(int httpStatus, string responsePhrase, HttpHeaders headers, string body, string bodyContentType, ResponseErrorType errorType)
        {
            this.statusCode = httpStatus;
            this.responsePhrase = responsePhrase;
            this.headers = headers;
            this.body = body;
            this.bodyContentType = bodyContentType;
            this.errorType = errorType;
        }

        public override string Body => this.body;

        public override string BodyContentType => this.bodyContentType;

        public override HttpHeaders Headers => this.headers;

        public int StatusCode => this.statusCode;

        public string ResponsePhrase => this.responsePhrase;

        public ResponseErrorType ErrorType => this.errorType;
    }
}
