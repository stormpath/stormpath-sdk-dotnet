// <copyright file="DefaultSerializerLoader.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Serialization
{
    internal static class DefaultSerializerLoader
    {
#if NET451
        private static readonly string LibraryName = "Stormpath.SDK.JsonNetSerializer";
        private static readonly string FileName = "Stormpath.SDK.JsonNetSerializer.dll";
        private static readonly string FullyQualifiedType = "Stormpath.SDK.Extensions.Serialization.JsonNetSerializer";

        // Caching result so expensive reflection only happens once
        private static readonly Lazy<Type> LoadTypeAction = new Lazy<Type>(() =>
        {
            var loader = new TypeLoader(LibraryName, FileName, FullyQualifiedType);

            return loader.Load();
        });

        public static Type Load()
            => LoadTypeAction.Value;
#endif
    }
}