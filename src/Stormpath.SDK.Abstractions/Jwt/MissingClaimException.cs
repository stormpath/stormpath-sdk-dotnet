// <copyright file="MissingClaimException.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Jwt
{
    /// <summary>
    /// Represents an error indicating that a JSON Web Token has a missing claim.
    /// </summary>
    public class MissingClaimException : InvalidJwtException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingClaimException"/> class with the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MissingClaimException(string message)
            : base(message)
        {
        }
    }
}
