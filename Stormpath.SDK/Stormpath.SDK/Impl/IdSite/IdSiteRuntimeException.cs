// <copyright file="IdSiteRuntimeException.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Error;

namespace Stormpath.SDK.Impl.IdSite
{
    /// <summary>
    /// A sub-class of <see cref="ResourceException"/> representing an ID Site error.
    /// </summary>
    public class IdSiteRuntimeException : ResourceException
    {
        private static readonly int[] SupportedErrors =
            { 10011, 10012, 11001, 11002, 11003, 12001 };

        internal IdSiteRuntimeException(DefaultError error)
            : base(error)
        {
            if (!Supports(error))
                throw new ArgumentException("Error type not supported; must be one of: " + string.Join(",", SupportedErrors));
        }

        public void Rethrow()
        {
            if (this.Error.Code == 10011
                || this.Error.Code == 10012
                || this.Error.Code == 11001
                || this.Error.Code == 11002
                || this.Error.Code == 11003)
            {
                throw new InvalidIdSiteTokenException(this.Error);
            }

            if (this.Error.Code == 12001)
            {
                throw new IdSiteSessionTimeoutException(this.Error);
            }

            throw new ApplicationException("Error type is unrecognized: " + this.Error.Code);
        }

        private static bool Supports(IError error)
            => SupportedErrors.Contains(error.Code);
    }
}
