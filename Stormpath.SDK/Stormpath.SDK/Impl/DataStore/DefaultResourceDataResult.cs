// <copyright file="DefaultResourceDataResult.cs" company="Stormpath, Inc.">
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
    internal sealed class DefaultResourceDataResult : IResourceDataResult
    {
        private readonly ResourceAction action;
        private readonly Type type;
        private readonly CanonicalUri uri;
        private readonly int httpStatus;
        private readonly IDictionary<string, object> body;

        public DefaultResourceDataResult(ResourceAction action, Type returnType, CanonicalUri uri, int httpStatus, IDictionary<string, object> body)
        {
            this.action = action;
            this.type = returnType;
            this.uri = uri;
            this.httpStatus = httpStatus;
            this.body = body;
        }

        ResourceAction IResourceDataResult.Action => this.action;

        IDictionary<string, object> IResourceDataResult.Body => this.body;

        Type IResourceDataResult.Type => this.type;

        CanonicalUri IResourceDataResult.Uri => this.uri;

        int IResourceDataResult.HttpStatus => this.httpStatus;
    }
}
