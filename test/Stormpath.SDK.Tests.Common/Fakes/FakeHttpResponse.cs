// <copyright file="FakeHttpResponse.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Http;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public sealed class FakeHttpResponse : IHttpResponse
    {
        public FakeHttpResponse(int statusCode, string body = null, string bodyContentType = null)
            : this(statusCode, null, null, body, bodyContentType, false)
        {
        }

        public FakeHttpResponse(int statusCode, string responsePhrase, HttpHeaders headers, string body, string bodyContentType, bool transportError)
        {
            this.StatusCode = statusCode;
            this.ResponsePhrase = responsePhrase;
            this.Headers = headers;
            this.Body = body;
            this.BodyContentType = bodyContentType;
            this.TransportError = transportError;
        }

        public int StatusCode { get; private set; }

        public bool TransportError { get; private set; }

        public string ResponsePhrase { get; private set; }

        public HttpHeaders Headers { get; private set; }

        public bool HasBody { get; private set; }

        public string Body { get; private set; }

        public string BodyContentType { get; private set; }
    }
}
