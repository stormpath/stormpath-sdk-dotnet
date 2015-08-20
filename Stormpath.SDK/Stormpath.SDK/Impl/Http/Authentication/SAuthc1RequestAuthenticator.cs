// <copyright file="SAuthc1RequestAuthenticator.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    internal sealed class SAuthc1RequestAuthenticator : IRequestAuthenticator
    {
        private static readonly string IDTerminator = "sauthc1_request";
        private static readonly string Algorithm = "HMAC-SHA-256";
        private static readonly string AuthenticationScheme = "SAuthc1";
        private static readonly string SAUTHC1Id = "sauthc1Id";
        private static readonly string SAUTHC1SignedHeaders = "sauthc1SignedHeaders";
        private static readonly string SAUTHC1Signature = "sauthc1Signature";
        private static readonly string Newline = "\n";

        void IRequestAuthenticator.Authenticate(HttpRequestMessage request, IClientApiKey apiKey)
        {
            var utcNow = DateTimeOffset.UtcNow;
            var dateTimeString = Iso8601.Format(utcNow);
            var dateString = utcNow.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

            var nonce = Guid.NewGuid().ToString();
            var requestBody = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var requestBodyHash = ToHex(Hash(requestBody, Encoding.UTF8));
            var signedHeadersString = GetSortedHeaderKeys(request.Headers);

            var canonicalRequest = new StringBuilder()
                .Append(request.Method.Method.ToUpper())
                .Append(Newline)
                .Append(CanonicalizeResourcePath(request))
                .Append(Newline)
                .Append(CanonicalizeQueryString(request))
                .Append(Newline)
                .Append(CanonicalizeHeaders(request))
                .Append(Newline)
                .Append(signedHeadersString)
                .Append(Newline)
                .Append(requestBodyHash)
                .ToString();

            var id = new StringBuilder()
                .Append(apiKey.GetId())
                .Append("/")
                .Append(dateString)
                .Append("/")
                .Append(nonce)
                .Append("/")
                .Append(IDTerminator)
                .ToString();

            var canonicalRequestHash = ToHex(Hash(canonicalRequest, Encoding.UTF8));
            var stringToSign = new StringBuilder()
                .Append(Algorithm)
                .Append(Newline)
                .Append(dateTimeString)
                .Append(Newline)
                .Append(id)
                .Append(Newline)
                .Append(canonicalRequestHash)
                .ToString();

            byte[] secret = Encoding.UTF8.GetBytes($"{AuthenticationScheme}{apiKey.GetSecret()}");
            byte[] signDate = SignHmac256(dateString, secret, Encoding.UTF8);
            byte[] signNonce = SignHmac256(nonce, signDate, Encoding.UTF8);
            byte[] signTerminator = SignHmac256(IDTerminator, signNonce, Encoding.UTF8);
            byte[] signature = SignHmac256(stringToSign, signTerminator, Encoding.UTF8);

            var authorizationHeader = new StringBuilder()
                .Append(AuthenticationScheme)
                .Append(SAUTHC1Id)
                .Append("=")
                .Append(id)
                .Append(", ")
                .Append(SAUTHC1SignedHeaders)
                .Append("=")
                .Append(signedHeadersString)
                .Append(", ")
                .Append(SAUTHC1Signature)
                .Append("=")
                .Append(ToHex(signature))
                .ToString();
            request.Headers.Add("Authorization", authorizationHeader);
        }

        private static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes)
                .Replace("-", string.Empty)
                .ToLower();
        }

        private static byte[] Hash(string text, Encoding encoding)
        {
            try
            {
                using (var hash = SHA256.Create())
                {
                    return hash.ComputeHash(encoding.GetBytes(text));
                }
            }
            catch (Exception e)
            {
                throw new RequestAuthenticationException("Unable to compute hash while signing request.", e);
            }
        }

        private static byte[] SignHmac256(string data, byte[] key, Encoding encoding)
        {
            return SignHmac256(encoding.GetBytes(data), key);
        }

        private static byte[] SignHmac256(byte[] data, byte[] key)
        {
            try
            {
                using (var hmac = new HMACSHA256(key))
                {
                    return hmac.ComputeHash(data);
                }
            }
            catch (Exception e)
            {
                throw new RequestAuthenticationException("Unable to calculate a request signature.", e);
            }
        }

        // Return all lowercase header names (keys) separated by semicolon
        // e.g. header1;header2;header3
        private static string GetSortedHeaderKeys(HttpRequestHeaders headers)
        {
            var sortedKeys = headers
                .Select(x => x.Key.ToLower())
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            return string.Join(";", sortedKeys);
        }

        private static string CanonicalizeQueryString(HttpRequestMessage request)
        {
            return new QueryString(request.RequestUri.Query).ToString(true);
        }

        private static string CanonicalizeResourcePath(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        private static string CanonicalizeHeaders(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
