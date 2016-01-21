// <copyright file="ApiAuthenticationException.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Error;

namespace Stormpath.SDK.Auth
{
    public abstract class ApiAuthenticationException : ResourceException
    {
        private static readonly int ExceptionStatus = 401;
        private static readonly int ExceptionCode = 401;
        private static readonly string ExceptionMoreInfo = "http://docs.stormpath.com/dotnet";
        private static readonly string ExceptionDefaultDeveloperMessage = "Authentication with a valid API Key is required.";
        private static readonly string ExceptionDefaultClientMessage = "Authentication Required";

        internal ApiAuthenticationException(string developerMessage)
            : base(DefaultError.WithMessage(developerMessage))
        {
        }
    }
}
