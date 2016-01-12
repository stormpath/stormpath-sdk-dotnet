// <copyright file="InvalidIdSiteTokenException.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Error;

namespace Stormpath.SDK.IdSite
{
    /// <summary>
    /// This exception indicates that the token is invalid. Reasons for this could be:
    /// An expired token, the issued at time (iat) is after the current time,
    /// the specified organization name key does not exist in your Stormpath Tenant, or
    /// because the specified organization is disabled.
    /// </summary>
    public sealed class InvalidIdSiteTokenException : IdSiteRuntimeException
    {
        internal InvalidIdSiteTokenException(DefaultError error)
            : base(error)
        {
        }
    }
}
