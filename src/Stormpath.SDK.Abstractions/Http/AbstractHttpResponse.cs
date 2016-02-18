// <copyright file="AbstractHttpResponse.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    public abstract class AbstractHttpResponse : IHttpResponse
    {
        public AbstractHttpResponse(
            int statusCode,
            string responseMessage,
            HttpHeaders headers,
            string body,
            string contentType,
            bool transportError)
        {
            this.StatusCode = statusCode;
            this.ResponsePhrase = responseMessage;
            this.Headers = headers ?? new HttpHeaders();
            this.Body = body;
            this.BodyContentType = contentType;
            this.TransportError = transportError;
        }

        public string Body { get; private set; }

        public string BodyContentType { get; private set; }

        public bool HasBody => !string.IsNullOrEmpty(this.Body);

        public HttpHeaders Headers { get; private set; }

        public string ResponsePhrase { get; private set; }

        public int StatusCode { get; private set; }

        public bool TransportError { get; private set; }
    }
}
