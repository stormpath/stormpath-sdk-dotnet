// <copyright file="HttpMessageBase.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Http;

namespace Stormpath.SDK.Impl.Http
{
    internal abstract class HttpMessageBase : IHttpMessage
    {
        public abstract string Body { get; }

        public abstract string BodyContentType { get; }

        public bool HasBody => !string.IsNullOrEmpty(this.Body) && !string.IsNullOrEmpty(this.BodyContentType);

        public abstract HttpHeaders Headers { get; }
    }
}
