// <copyright file="MemoryCache.cs" company="Stormpath, Inc.">
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
//
// Contains code modified from aspnet/Caching. Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

#if NET45 || NET451
using System;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    /// <summary>
    /// Registers a callback that fires each time a Gen2 garbage collection occurs,
    /// presumably due to memory pressure.
    /// For this to work no components can have a reference to the instance.
    /// </summary>
    internal class GcNotification
    {
        private readonly Func<object, bool> _callback;
        private readonly object _state;
        private readonly int _initialCollectionCount;

        private GcNotification(Func<object, bool> callback, object state)
        {
            _callback = callback;
            _state = state;
            _initialCollectionCount = GC.CollectionCount(2);
        }

        public static void Register(Func<object, bool> callback, object state)
        {
            var notification = new GcNotification(callback, state);
        }

        ~GcNotification()
        {
            bool reRegister = true;
            try
            {
                // Only invoke the callback after this instance has made it into gen2.
                if (_initialCollectionCount < GC.CollectionCount(2))
                {
                    reRegister = _callback(_state);
                }
            }
            catch (Exception)
            {
                // Never throw from the finalizer thread
            }

            if (reRegister && !Environment.HasShutdownStarted)
            {
                GC.ReRegisterForFinalize(this);
            }
        }
    }
}
#endif