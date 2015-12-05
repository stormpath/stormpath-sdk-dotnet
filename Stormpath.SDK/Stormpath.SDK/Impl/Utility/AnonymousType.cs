// <copyright file="AnonymousType.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.ComponentModel;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Utility
{
    internal static class AnonymousType
    {
        public static Map ToDictionary(object nameValuePairs)
        {
            if (nameValuePairs == null)
            {
                return null;
            }

            bool isConcreteType = !string.IsNullOrEmpty(nameValuePairs.GetType().Namespace);
            if (isConcreteType)
            {
                return null;
            }

            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(nameValuePairs))
            {
                object value = descriptor.GetValue(nameValuePairs);
                dictionary.Add(descriptor.Name, value);
            }

            return dictionary;
        }
    }
}
