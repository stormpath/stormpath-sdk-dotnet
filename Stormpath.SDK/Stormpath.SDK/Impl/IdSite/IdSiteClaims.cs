// <copyright file="IdSiteClaims.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.IdSite
{
    internal static class IdSiteClaims
    {
        public static readonly string RedirectUri = "cb_uri";
        public static readonly string Path = "path";
        public static readonly string OrganizationNameKey = "onk";
        public static readonly string ShowOrganizationField = "sof";
        public static readonly string UseSubdomain = "usd";
        public static readonly string State = "state";
        public static readonly string ResponseId = "irt";
        public static readonly string IsNewSubject = "isNewSub";
        public static readonly string Status = "status";
        public static readonly string Error = "err";

        public static readonly string JwtRequest = "jwtRequest";
        public static readonly string JwtResponse = "jwtResponse";
    }
}
