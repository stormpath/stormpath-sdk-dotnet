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

using System;
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public sealed class FakeHttpResponse : IHttpResponse
    {
        private readonly int statusCode;
        private readonly string body;
        private readonly string bodyContentType;

        public FakeHttpResponse(int statusCode, string body = null, string bodyContentType = null)
        {
            this.statusCode = statusCode;
            this.body = body;
            this.bodyContentType = null;
        }

        string IHttpMessage.Body => this.body;

        string IHttpMessage.BodyContentType => this.bodyContentType;

        bool IHttpMessage.HasBody => !string.IsNullOrEmpty(this.body);

        HttpHeaders IHttpMessage.Headers => new HttpHeaders();

        string IHttpResponse.ResponsePhrase
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int IHttpResponse.StatusCode => this.statusCode;

        bool IHttpResponse.TransportError => false;
    }
}
