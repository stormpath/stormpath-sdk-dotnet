// <copyright file="DefaultJsonSerializerLoader.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Impl.Serialization
{
    internal sealed class DefaultJsonSerializerLoader : TypeLoader<IJsonSerializer>
    {
        private static readonly string FileName = "Stormpath.SDK.JsonNetSerializer.dll";
        private static readonly string FullyQualifiedType = "Stormpath.SDK.Extensions.Serialization.JsonNetSerializer";

        public DefaultJsonSerializerLoader()
            : base(FileName, FullyQualifiedType)
        {
        }
    }
}
