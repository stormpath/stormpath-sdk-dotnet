// <copyright file="AuthorizationHeaderValue.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents an HTTP Authorization header.
    /// </summary>
    public sealed class AuthorizationHeaderValue : ImmutableValueObject<AuthorizationHeaderValue>
    {
        private readonly string scheme;
        private readonly string parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationHeaderValue"/> class
        /// with the specified <see cref="Scheme"/> and <see cref="Parameter"/>.
        /// </summary>
        /// <param name="scheme">The authorization scheme (e.g. Basic).</param>
        /// <param name="parameter">The authorization token.</param>
        public AuthorizationHeaderValue(string scheme, string parameter)
        {
            this.scheme = scheme;
            this.parameter = parameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationHeaderValue"/> class
        /// with the specified <see cref="Scheme"/> and <see cref="Parameter"/>.
        /// </summary>
        /// <param name="schemeAndParameter">A combined scheme and parameter value.</param>
        /// <exception cref="ArgumentException"><paramref name="schemeAndParameter"/> cannot be parsed.</exception>
        internal AuthorizationHeaderValue(string schemeAndParameter)
        {
            var segments = schemeAndParameter.Split(' ');
            if (segments.Length != 2)
                throw new ArgumentException("Invalid Authorization header format.", nameof(schemeAndParameter));

            this.scheme = segments[0];
            this.parameter = segments[1];
        }

        /// <summary>
        /// Gets the authorization scheme.
        /// </summary>
        /// <value>An authorization scheme name (e.g. Basic).</value>
        public string Scheme => this.scheme;

        /// <summary>
        /// Gets the authorization parameter.
        /// </summary>
        /// <value>The authorization token.</value>
        public string Parameter => this.parameter;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.scheme} {this.parameter}";
        }
    }
}
