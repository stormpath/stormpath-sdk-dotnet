﻿// <copyright file="IResourceDataRequest.cs" company="Stormpath, Inc.">
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
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Represents an API request for a Stormpath resource.
    /// </summary>
    internal interface IResourceDataRequest
    {
        /// <summary>
        /// Gets the request's HTTP action.
        /// </summary>
        /// <value>The HTTP action.</value>
        ResourceAction Action { get; }

        /// <summary>
        /// Gets the request URL.
        /// </summary>
        /// <value>The request URL.</value>
        CanonicalUri Uri { get; }

        /// <summary>
        /// Gets the request's HTTP headers.
        /// </summary>
        /// <value>The HTTP headers.</value>
        HttpHeaders Headers { get; }

        /// <summary>
        /// Gets the resource type as a .NET <see cref="System.Type"/>.
        /// </summary>
        /// <value>The resource type.</value>
        Type Type { get; }

        /// <summary>
        /// Gets additional resource properties used in the request.
        /// </summary>
        /// <value>Additional resource properties.</value>
        Map Properties { get; }

        /// <summary>
        /// Gets a value indicating whether to skip the cache for this request.
        /// </summary>
        /// <value><see langword="true"/> if the cache should be skipped on this request; <see langword="false"/> otherwise. Default: <see langword="false"/>.</value>
        bool SkipCache { get; }
    }
}
