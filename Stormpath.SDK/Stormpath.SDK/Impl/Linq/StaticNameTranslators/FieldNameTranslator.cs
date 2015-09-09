// <copyright file="FieldNameTranslator.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Linq.StaticNameTranslators
{
    internal static class FieldNameTranslator
    {
        public static bool TryGetValue(string methodName, out string fieldName)
        {
            bool found = false;
            switch (methodName)
            {
                case "Email":
                    fieldName = "email";
                    found = true;
                    break;
                case "GivenName":
                    fieldName = "givenName";
                    found = true;
                    break;
                case "MiddleName":
                    fieldName = "middleName";
                    found = true;
                    break;
                case "Surname":
                    fieldName = "surname";
                    found = true;
                    break;
                case "Username":
                    fieldName = "username";
                    found = true;
                    break;
                case "Name":
                    fieldName = "name";
                    found = true;
                    break;
                case "Description":
                    fieldName = "description";
                    found = true;
                    break;
                case "Status":
                    fieldName = "status";
                    found = true;
                    break;
                default:
                    fieldName = null;
                    break;
            }

            return found;
        }
    }
}
