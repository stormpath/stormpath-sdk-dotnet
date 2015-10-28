// <copyright file="DefaultResourceDataRequest.cs" company="Stormpath, Inc.">
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
    internal sealed class DefaultResourceDataRequest : IResourceDataRequest
    {
        private readonly ResourceAction action;
        private readonly Type resourceType;
        private readonly CanonicalUri uri;
        private readonly IDictionary<string, object> properties;

        public DefaultResourceDataRequest(ResourceAction action, Type resourceType, CanonicalUri uri)
            : this(action, resourceType, uri, null)
        {
        }

        public DefaultResourceDataRequest(ResourceAction action, Type resourceType, CanonicalUri uri, IDictionary<string, object> properties)
        {
            this.action = action;
            this.uri = uri;
            this.resourceType = resourceType;
            this.properties = properties;
        }

        ResourceAction IResourceDataRequest.Action => this.action;

        IDictionary<string, object> IResourceDataRequest.Properties => this.properties;

        Type IResourceDataRequest.Type => this.resourceType;

        CanonicalUri IResourceDataRequest.Uri => this.uri;
    }
}
