// <copyright file="DefaultError.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections;
using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Error
{
    [Serializable]
    internal sealed class DefaultError : AbstractResource, IError
    {
        private static readonly string StatusPropertyName = "status";
        private static readonly string CodePropertyName = "code";
        private static readonly string MessagePropertyName = "message";
        private static readonly string DevMessagePropertyName = "developerMessage";
        private static readonly string MoreInfoPropertyName = "moreInfo";

        public DefaultError(Hashtable properties)
            : base(null, properties)
        {
        }

        public int Code => GetProperty<int>(CodePropertyName);

        public string DeveloperMessage => GetProperty<string>(DevMessagePropertyName);

        public string Message => GetProperty<string>(MessagePropertyName);

        public string MoreInfo => GetProperty<string>(MoreInfoPropertyName);

        public int Status => GetProperty<int>(StatusPropertyName);
    }
}
