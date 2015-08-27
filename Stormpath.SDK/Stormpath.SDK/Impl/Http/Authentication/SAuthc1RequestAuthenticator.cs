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
using System.Security.Cryptography;
using System.Text;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    internal sealed class SAuthc1RequestAuthenticator : IRequestAuthenticator
    {
        private static readonly string StormpathDateHeaderName = "X-Stormpath-Date";
        private static readonly string IDTerminator = "sauthc1_request";
        private static readonly string Algorithm = "HMAC-SHA-256";
        private static readonly string AuthenticationScheme = "SAuthc1";
        private static readonly string SAUTHC1Id = "sauthc1Id";
        private static readonly string SAUTHC1SignedHeaders = "sauthc1SignedHeaders";
        private static readonly string SAUTHC1Signature = "sauthc1Signature";
        private static readonly string Newline = "\n";

        void IRequestAuthenticator.Authenticate(IHttpRequest request, IClientApiKey apiKey)
        {
            var now = DateTimeOffset.UtcNow;
            var nonce = Guid.NewGuid().ToString();
            AuthenticateCore(request, apiKey, now, nonce);
        }

        internal void AuthenticateCore(IHttpRequest request, IClientApiKey apiKey, DateTimeOffset now, string nonce)
        {
            if (request == null)
                throw new RequestAuthenticationException("Request must not be null.");

            if (string.IsNullOrEmpty(request.CanonicalUri?.ToString()))
                throw new RequestAuthenticationException("URL must not be empty.");

            var uri = request.CanonicalUri.ToUri();
            if (!uri.IsAbsoluteUri)
                throw new RequestAuthenticationException("URL must be an absolute path.");

            var relativeResourcePath = uri.AbsolutePath;

            var timestamp = Iso8601.Format(now, withSeparators: false);
            var dateStamp = now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

            // Add HOST header before signing
            var hostHeader = uri.Host;
            if (!uri.IsDefaultPort)
                hostHeader = $"{hostHeader}:{uri.Port}";
            request.Headers.Host = hostHeader;

            // Add X-Stormpath-Date before signing
            request.Headers.Add(StormpathDateHeaderName, timestamp);

            var requestBody = request.Body.Nullable() ?? string.Empty;
            var requestBodyHash = ToHex(Hash(requestBody, Encoding.UTF8));
            var sortedHeaderKeys = GetSortedHeaderNames(request.Headers);

            var canonicalRequest = new StringBuilder()
                .Append(request.Method.ToString().ToUpper())
                .Append(Newline)
                .Append(CanonicalizeResourcePath(relativeResourcePath))
                .Append(Newline)
                .Append(request.CanonicalUri.QueryString.ToString(canonical: true))
                .Append(Newline)
                .Append(CanonicalizeHeaders(request))
                .Append(Newline)
                .Append(sortedHeaderKeys)
                .Append(Newline)
                .Append(requestBodyHash)
                .ToString();

            var id = new StringBuilder()
                .Append(apiKey.GetId()).Append("/")
                .Append(dateStamp).Append("/")
                .Append(nonce).Append("/")
                .Append(IDTerminator)
                .ToString();

            var canonicalRequestHash = ToHex(Hash(canonicalRequest, Encoding.UTF8));
            var stringToSign = new StringBuilder()
                .Append(Algorithm)
                .Append(Newline)
                .Append(timestamp)
                .Append(Newline)
                .Append(id)
                .Append(Newline)
                .Append(canonicalRequestHash)
                .ToString();

            var secretFormat = $"{AuthenticationScheme}{apiKey.GetSecret()}";
            byte[] secret = Encoding.UTF8.GetBytes(secretFormat);
            byte[] signedDate = SignHmac256(dateStamp, secret, Encoding.UTF8);
            byte[] signedNonce = SignHmac256(nonce, signedDate, Encoding.UTF8);
            byte[] signedTerminator = SignHmac256(IDTerminator, signedNonce, Encoding.UTF8);
            byte[] signature = SignHmac256(stringToSign, signedTerminator, Encoding.UTF8);
            var signatureHex = ToHex(signature);

            var authorizationHeaderValue = new StringBuilder()
                .Append(SAUTHC1Id).Append("=").Append(id).Append(", ")
                .Append(SAUTHC1SignedHeaders).Append("=").Append(sortedHeaderKeys).Append(", ")
                .Append(SAUTHC1Signature).Append("=").Append(signatureHex)
                .ToString();
            request.Headers.Authorization = new AuthorizationHeaderValue(AuthenticationScheme, authorizationHeaderValue);
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
        private static string GetSortedHeaderNames(HttpHeaders headers)
        {
            var sortedKeys = headers
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Key.ToLower());

            return string.Join(";", sortedKeys);
        }

        private static string CanonicalizeResourcePath(string relativeResourcePath)
        {
            if (string.IsNullOrEmpty(relativeResourcePath))
                return "/";
            return RequestHelper.UrlEncode(relativeResourcePath, isPath: true, canonicalize: true);
        }

        private static string CanonicalizeHeaders(IHttpRequest request)
        {
            var buffer = new StringBuilder();

            var sortedHeaders = request.Headers
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var header in sortedHeaders)
            {
                buffer.Append(header.Key.ToLower());
                buffer.Append(":");

                if (header.Value.Any())
                {
                    var first = true;
                    foreach (var value in header.Value)
                    {
                        if (!first)
                            buffer.Append(",");
                        buffer.Append(value);
                        first = false;
                    }
                }

                buffer.Append(Newline);
            }

            return buffer.ToString();
        }
    }
}
