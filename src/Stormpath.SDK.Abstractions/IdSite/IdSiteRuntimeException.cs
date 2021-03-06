﻿// <copyright file="IdSiteRuntimeException.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.IdSite
{
    /// <summary>
    /// Represents an ID Site error.
    /// </summary>
    public class IdSiteRuntimeException : ResourceException
    {
        private static readonly int[] SupportedErrors =
            { 10011, 10012, 11001, 11002, 11003, 12001 };

        /// <summary>
        /// Creates a new instance of <see cref="IdSiteRuntimeException"/>.
        /// </summary>
        /// <param name="error">The Stormpath API error.</param>
        public IdSiteRuntimeException(IError error)
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
