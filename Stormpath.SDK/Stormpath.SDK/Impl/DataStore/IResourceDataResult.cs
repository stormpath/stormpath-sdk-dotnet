// <copyright file="IResourceDataResult.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Represents the result of an API request for a Stormpath resource.
    /// </summary>
    internal interface IResourceDataResult
    {
        /// <summary>
        /// Gets the resulting HTTP action.
        /// </summary>
        /// <value>The HTTP action.</value>
        ResourceAction Action { get; }

        /// <summary>
        /// Gets the request URL.
        /// </summary>
        /// <value>The request URL.</value>
        CanonicalUri Uri { get; }

        /// <summary>
        /// Gets the resource type as a .NET <see cref="System.Type"/>.
        /// </summary>
        /// <value>The resource type.</value>
        Type Type { get; }

        /// <summary>
        /// Gets the response HTTP status code.
        /// </summary>
        /// <value>The response HTTP status code.</value>
        int HttpStatus { get; }

        /// <summary>
        /// Gets the body of the response.
        /// </summary>
        /// <value>The response body.</value>
        IDictionary<string, object> Body { get; }
    }
}
