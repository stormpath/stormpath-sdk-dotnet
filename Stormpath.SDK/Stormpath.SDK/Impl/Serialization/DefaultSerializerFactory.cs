﻿// <copyright file="DefaultSerializerFactory.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Impl.Serialization
{
    internal sealed class DefaultSerializerFactory : ISerializerFactory
    {
        ISerializerBuilder ISerializerFactory.Default()
        {
            Type defaultSerializerType = null;
            try
            {
                defaultSerializerType = DefaultSerializerLoader.Load();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while loading the default serializer. See the inner exception for details.", ex);
            }

            return new AbstractSerializerBuilder<IJsonSerializer>(defaultSerializerType);
        }
    }
}
