// <copyright file="SamlRuntimeException.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Error;

namespace Stormpath.SDK.Saml
{
    public sealed class SamlRuntimeException : ResourceException
    {
        private static readonly int[] SupportedErrors =
            { 10100, 10101, 10102, 12001 };

        internal SamlRuntimeException(DefaultError error)
            : base(error)
        {
            if (!Supports(error))
            {
                throw new ArgumentException("Error type not supported; must be one of: " + string.Join(",", SupportedErrors));
            }
        }

        private static bool Supports(IError error)
            => SupportedErrors.Contains(error.Code);
    }
}
