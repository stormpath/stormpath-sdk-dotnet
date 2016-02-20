// <copyright file="IHttpClientFactory.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents a factory that can create <see cref="IHttpClientBuilder">HTTP client builders</see>. HTTP client plugins can use extension methods to add additional options to this interface.
    /// </summary>
    /// <example>
    /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
    /// </example>
    public interface IHttpClientFactory
    {
    }
}
