// <copyright file="RestSharpAdapter.cs" company="Stormpath, Inc.">
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
using System.Linq;

namespace Stormpath.SDK.Extensions.Http
{
    internal class RestSharpAdapter
    {
        public RestSharp.IRestRequest ToRestRequest(string baseUrl, SDK.Http.IHttpRequest request)
        {
            var resourcePath = request.CanonicalUri.ToString().Replace(baseUrl, string.Empty);
            var method = this.ConvertMethod(request.Method);

            var restRequest = new RestSharp.RestRequest(resourcePath, method);
            restRequest.RequestFormat = RestSharp.DataFormat.Json;
            this.CopyHeaders(request.Headers, restRequest);

            if (request.HasBody)
                restRequest.AddParameter(request.BodyContentType, request.Body, RestSharp.ParameterType.RequestBody);

            return restRequest;
        }

        public SDK.Http.IHttpResponse ToHttpResponse(RestSharp.IRestResponse response)
        {
            bool transportError = false;
            var responseMessages = new List<string>();

            transportError =
                response.ResponseStatus == RestSharp.ResponseStatus.TimedOut ||
                response.ResponseStatus == RestSharp.ResponseStatus.Aborted;

            if (response.ErrorException != null)
                responseMessages.Add(response.ErrorException.Message);
            else if (!string.IsNullOrEmpty(response.ErrorMessage))
                responseMessages.Add(response.ErrorMessage);

            if (!string.IsNullOrEmpty(response.StatusDescription))
                responseMessages.Add(response.StatusDescription);

            var headers = this.ToHttpHeaders(response.Headers);

            return new Impl.Http.DefaultHttpResponse(
                (int)response.StatusCode,
                string.Join(Environment.NewLine, responseMessages),
                headers,
                response.Content,
                response.ContentType,
                transportError);
        }

        private void CopyHeaders(SDK.Http.HttpHeaders httpHeaders, RestSharp.IRestRequest restRequest)
        {
            if (httpHeaders == null)
                return;

            foreach (var header in httpHeaders)
            {
                foreach (var value in header.Value)
                {
                    restRequest.AddHeader(header.Key, value);
                }
            }
        }

        private SDK.Http.HttpHeaders ToHttpHeaders(IList<RestSharp.Parameter> restSharpHeaders)
        {
            var result = new SDK.Http.HttpHeaders();

            foreach (var header in restSharpHeaders.Where(x => x.Type == RestSharp.ParameterType.HttpHeader))
            {
                result.Add(header.Name, header.Value);
            }

            return result;
        }

        private RestSharp.Method ConvertMethod(SDK.Http.HttpMethod httpMethod)
        {
            if (httpMethod == SDK.Http.HttpMethod.Delete)
                return RestSharp.Method.DELETE;

            if (httpMethod == SDK.Http.HttpMethod.Get)
                return RestSharp.Method.GET;

            if (httpMethod == SDK.Http.HttpMethod.Head)
                return RestSharp.Method.HEAD;

            if (httpMethod == SDK.Http.HttpMethod.Options)
                return RestSharp.Method.OPTIONS;

            if (httpMethod == SDK.Http.HttpMethod.Post)
                return RestSharp.Method.POST;

            if (httpMethod == SDK.Http.HttpMethod.Put)
                return RestSharp.Method.PUT;

            throw new ArgumentException($"Unknown method type {httpMethod}", nameof(httpMethod));
        }
    }
}
