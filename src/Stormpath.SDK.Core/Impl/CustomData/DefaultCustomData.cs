// <copyright file="DefaultCustomData.cs" company="Stormpath, Inc.">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.CustomData
{
    internal sealed class DefaultCustomData : AbstractInstanceResource, ICustomData, ICustomDataSync
    {
        private static readonly List<string> ReservedKeys = new List<string>()
        {
            HrefPropertyName, CreatedAtPropertyName, ModifiedAtPropertyName
        };

        // Matches any character in a-z, A-Z, 0-9, _, -  (but cannot start with -)
        private static readonly Regex ValidKeyCharactersRegex = new Regex("^[a-zA-Z0-9_]+[a-zA-Z0-9_-]*$", RegexOptions.Compiled);

        public DefaultCustomData(ResourceData data)
            : base(data)
        {
        }

        private new ICustomData AsInterface => this;

        private List<string> GetAvailableKeys()
        {
            var keys = new List<string>();
            keys.AddRange(this.GetResourceData()?.GetUpdatedPropertyNames());
            keys.AddRange(this.GetResourceData()?.GetPropertyNames());

            var deletedProperties = this.GetResourceData()?.GetDeletedPropertyNames();
            keys.RemoveAll(x => deletedProperties.Contains(x));

            return keys;
        }

        internal bool HasDeletedProperties()
            => this.GetResourceData()?.GetDeletedPropertyNames().Any() ?? false;

        internal bool HasUpdatedProperties()
            => this.GetResourceData()?.GetUpdatedPropertyNames().Any() ?? false;

        object ICustomData.this[string key]
        {
            get { return this.AsInterface.Get(key); }

            set { this.AsInterface.Put(key, value); }
        }

        int ICustomData.Count
            => this.GetAvailableKeys().Count();

        internal IReadOnlyDictionary<string, object> GetUpdatedProperties()
            => this.GetResourceData()?.GetUpdatedProperties();

        void ICustomData.Clear()
        {
            var keysToClear = this
                .GetAvailableKeys()
                .Except(ReservedKeys)
                .ToList();

            keysToClear.ForEach(key => this.AsInterface.Remove(key));
        }

        bool ICustomData.ContainsKey(string key)
            => this.GetAvailableKeys().Contains(key);

        object ICustomData.Get(string key)
            => this.GetProperty(key);

        T ICustomData.Get<T>(string key)
        {
            var value = AsInterface.Get(key);

            return UnsanitizeValue<T>(value);
        }

        void ICustomData.Put(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!IsValidKey(key))
            {
                throw new ArgumentOutOfRangeException($"{key} is not a valid key name.");
            }

            if (!IsValidValue(value))
            {
                throw new ArgumentOutOfRangeException($"'{value}' is not a valid value for key '{key}'. Only primitives, strings, GUIDs, dates, and durations can be stored in Custom Data.");
            }

            var sanitizedValue = SanitizeValueForStorage(value);

            this.GetResourceData()?.RemoveProperty(key);

            this.SetProperty(key, sanitizedValue);
        }

        void ICustomData.Put(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            bool isEmpty = !keyValuePairs?.Any() ?? true;
            if (isEmpty)
            {
                return;
            }

            foreach (var kvp in keyValuePairs)
            {
                this.AsInterface.Put(kvp.Key, kvp.Value);
            }
        }

        void ICustomData.Put(object customData)
        {
            // This is probably an anonymous type. But, a user could have passed
            // a type meant for a different overload as an object, so
            // we need to do a little investigation to find out
            // (fail fast if it's just a null)
            if (customData == null)
            {
                return;
            }

            var asEnumerable = customData as IEnumerable<KeyValuePair<object, string>>;
            if (asEnumerable != null)
            {
                this.AsInterface.Put(asEnumerable);
                return;
            }

            if (customData is KeyValuePair<string, object>)
            {
                this.AsInterface.Put((KeyValuePair<string, object>)customData);
                return;
            }

            // Assume it's an anonymous type and convert
            var anonymousAsDictionary = AnonymousType.ToDictionary(customData);
            if (anonymousAsDictionary != null)
            {
                this.AsInterface.Put(anonymousAsDictionary);
                return;
            }

            throw new ArgumentException("Could not parse the supplied Custom Data object. The value must be passed as a key/value pair or an anonymous type.");
        }

        void ICustomData.Put(KeyValuePair<string, object> keyValuePair)
            => this.AsInterface.Put(keyValuePair.Key, keyValuePair.Value);

        object ICustomData.Remove(string key)
        {
            if (ReservedKeys.Contains(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key), $"{key} is a reserved key and cannot be removed.");
            }

            return this.GetResourceData()?.RemoveProperty(key);
        }

        bool ICustomData.TryGetValue(string key, out object value)
        {
            value = this.AsInterface.Get(key);

            return value != null;
        }

        bool ICustomData.IsEmptyOrDefault()
            => this.GetAvailableKeys().All(x => ReservedKeys.Contains(x));

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            foreach (var key in this.GetAvailableKeys())
            {
                yield return new KeyValuePair<string, object>(key, this.AsInterface[key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.AsInterface.GetEnumerator();

        public async Task<bool> DeleteRemovedPropertiesAsync(string parentHref, CancellationToken cancellationToken)
        {
            var propertyDeletionTasks = this.GetResourceData()?.GetDeletedPropertyNames()
                .Select(async x =>
                {
                    var successful =
                        await this.GetInternalAsyncDataStore().DeletePropertyAsync(parentHref, x, cancellationToken).ConfigureAwait(false)
                        && (this.GetResourceData()?.OnDeletingRemovedProperty(x) ?? false);
                    return successful;
                });

            var results = await Task.WhenAll(propertyDeletionTasks).ConfigureAwait(false);

            return results.All(x => x == true);
        }

        public bool DeleteRemovedProperties(string parentHref)
        {
            var results = new List<bool>();
            foreach (var propName in this.GetResourceData()?.GetDeletedPropertyNames())
                {
                var successful = this.GetInternalSyncDataStore().DeleteProperty(parentHref, propName)
                    && (this.GetResourceData()?.OnDeletingRemovedProperty(propName) ?? false);
                results.Add(successful);
            }

            return results.All(x => x == true);
        }

        async Task<ICustomData> ISaveable<ICustomData>.SaveAsync(CancellationToken cancellationToken)
        {
            if (this.HasDeletedProperties())
            {
                await this.DeleteRemovedPropertiesAsync(this.AsInterface.Href, cancellationToken).ConfigureAwait(false);
            }

            return await this.GetInternalAsyncDataStore().SaveAsync<ICustomData>(this, cancellationToken).ConfigureAwait(false);
        }

        ICustomData ISaveableSync<ICustomData>.Save()
        {
            if (this.HasDeletedProperties())
            {
                this.DeleteRemovedProperties(this.AsInterface.Href);
            }

            return this.GetInternalSyncDataStore().Save<ICustomData>(this);
        }

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);

        private static bool IsValidKey(string possibleKey)
        {
            if (possibleKey.Length > 255)
            {
                return false;
            }

            bool isValidCharacters = ValidKeyCharactersRegex.IsMatch(possibleKey);
            if (!isValidCharacters)
            {
                return false;
            }

            if (ReservedKeys.Contains(possibleKey))
            {
                return false;
            }

            return true;
        }

        private static bool IsValidValue(object value)
        {
            var type = value.GetType();
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsArray)
            {
                return IsValidPrimitiveType(typeInfo.GetElementType());
            }

            bool isEnumerableOfT = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeInfo);
            if (isEnumerableOfT && typeInfo.GenericTypeArguments.Count() == 1)
            {
                var innerType = typeInfo.GenericTypeArguments.Single();
                return IsValidPrimitiveType(innerType);
            }

            return IsValidPrimitiveType(type);
        }

        private static bool IsValidPrimitiveType(Type valueType)
        {
            if (valueType.GetTypeInfo().IsPrimitive ||
                valueType == typeof(string)||
                valueType == typeof(decimal) ||
                valueType == typeof(DateTimeOffset) ||
                valueType == typeof(DateTime) ||
                valueType == typeof(TimeSpan) ||
                valueType == typeof(Guid))
            {
                return true;
            }

            return false;
        }

        private static object SanitizeValueForStorage(object value)
        {
            if (value.GetType() == typeof(DateTime))
            {
                value = new DateTimeOffset((DateTime)value);
            }

            if (value.GetType() == typeof(DateTimeOffset))
            {
                value = Iso8601.Format((DateTimeOffset)value, convertToUtc: false);
            }

            if (value.GetType() == typeof(TimeSpan))
            {
                value = Iso8601Duration.Format((TimeSpan)value);
            }

            if (value.GetType() == typeof(Guid))
            {
                value = value.ToString();
            }

            return value;
        }

        private static T UnsanitizeValue<T>(object value)
        {
            if (typeof(T) == typeof(DateTime))
            {
                throw new Exception("Use DateTimeOffset to retrieve dates saved in Custom Data.");
            }

            if (typeof(T) == typeof(DateTimeOffset))
            {
                bool valueAlreadyParsed = value.GetType() == typeof(DateTimeOffset);
                if (!valueAlreadyParsed)
                {
                    return (T)(object)Iso8601.Parse(value.ToString());
                }
            }

            if (typeof(T) == typeof(TimeSpan))
            {
                return (T)(object)Iso8601Duration.Parse(value.ToString());
            }
            
            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)Guid.Parse(value.ToString());
            }

            if (value == null)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}
