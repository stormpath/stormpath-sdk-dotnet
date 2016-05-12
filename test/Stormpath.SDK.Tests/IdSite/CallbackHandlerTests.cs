// <copyright file="CallbackHandlerTests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.IdSite;

namespace Stormpath.SDK.Tests.IdSite
{
    public class CallbackHandlerTests
    {
        private static readonly string RegisteredResponse =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiIsImtpZCI6IjJFVjcwQUhSVFlGMEpPQTdPRUZPM1NNMjkifQ.eyJpc3" +
            "MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1wYXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92" +
            "MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIiwiZXhwIjoyNTAyNDY2NjUwMDAsImlhdCI6MTQwNzE5ODU1MCwianRpIj" +
            "oiNDM2dmtrSGdrMXgzMDU3cENQcVRhaCIsImlydCI6IjFkMDJkMzM1LWZiZmMtNGVhOC1iODM2LTg1YjllMmE2ZjJhMCIsImlzTmV3" +
            "U3ViIjpmYWxzZSwic3RhdHVzIjoiUkVHSVNURVJFRCJ9.0n-Sp5zDtiOIJ_Zq6IYdrDHoU3i95XPKhlH-2n9ALdg";

        private static readonly string AuthenticatedResponse = 
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiIsImtpZCI6IjJFVjcwQUhSVFlGMEpPQTdPRUZPM1NNMjkifQ.eyJpc3" +
            "MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1wYXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92" +
            "MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIiwiZXhwIjoyNTAyNDY2NjUwMDAsImlhdCI6MTQwNzE5ODU1MCwianRpIj" +
            "oiNDM2dmtrSGdrMXgzMDU3cENQcVRhaCIsImlydCI6IjFkMDJkMzM1LWZiZmMtNGVhOC1iODM2LTg1YjllMmE2ZjJhMCIsImlzTmV3" +
            "U3ViIjpmYWxzZSwic3RhdHVzIjoiQVVUSEVOVElDQVRFRCJ9.jR_T2G0obYuVIf-Gxer5pCieglfzwdWfNoMn505-hEw";

        private static readonly string LogoutResponse = 
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiIsImtpZCI6IjJFVjcwQUhSVFlGMEpPQTdPRUZPM1NNMjkifQ.eyJpc3" +
            "MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1wYXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92" +
            "MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIiwiZXhwIjoyNTAyNDY2NjUwMDAsImlhdCI6MTQwNzE5ODU1MCwianRpIj" +
            "oiNDM2dmtrSGdrMXgzMDU3cENQcVRhaCIsImlydCI6IjFkMDJkMzM1LWZiZmMtNGVhOC1iODM2LTg1YjllMmE2ZjJhMCIsImlzTmV3" +
            "U3ViIjpmYWxzZSwic3RhdHVzIjoiTE9HT1VUIn0.eQJAg2ils97GdtokcaK4T98CwfizM7ZCmuTqew9gf2Y";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { nameof(RegisteredResponse), RegisteredResponse, IdSiteResultStatus.Registered.Value, false, null };
            yield return new object[] { nameof(AuthenticatedResponse), AuthenticatedResponse, IdSiteResultStatus.Authenticated.Value, false, null };
            yield return new object[] { nameof(LogoutResponse), LogoutResponse, IdSiteResultStatus.Logout.Value, false, null };
        }
    }
}
