// <copyright file="DefaultJsonSerializerBuilder.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Impl.Serialization;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultJsonSerializerBuilder : IJsonSerializerBuilder
    {
        private readonly ITypeLoader<IJsonSerializer> defaultLibraryLoader;

        private IJsonSerializer instance;

        public DefaultJsonSerializerBuilder()
            : this(new DefaultJsonSerializerLoader())
        {
        }

        internal DefaultJsonSerializerBuilder(ITypeLoader<IJsonSerializer> defaultLibraryLoader)
        {
            this.defaultLibraryLoader = defaultLibraryLoader;
        }

        IJsonSerializerBuilder ISerializerConsumer<IJsonSerializerBuilder>.SetSerializer(IJsonSerializer serializer)
        {
            this.instance = serializer;
            return this;
        }

        IJsonSerializer IJsonSerializerBuilder.Build()
        {
            if (this.instance != null)
                return this.instance;

            IJsonSerializer defaultSerializer = null;
            bool foundDefaultLibrary = this.defaultLibraryLoader.TryLoad(out defaultSerializer);
            if (!foundDefaultLibrary)
                throw new ApplicationException("Could not find a valid JSON serializer. Include Stormpath.SDK.JsonNetSerializer.dll in the application path.");

            return defaultSerializer;
        }
    }
}
