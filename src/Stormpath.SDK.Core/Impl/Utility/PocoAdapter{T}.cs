using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Shared;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Utility
{
    /// <summary>
    /// Used to marshall dictionaries in Custom Data to POCOs.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    internal sealed class PocoAdapter<T> : PocoAdapter
    {
        public PocoAdapter()
            : base(typeof(T))
        {
        }

        public new T Adapt(IDictionary<string, object> data, StringComparer comparer = null)
        {
            return (T)base.Adapt(data, comparer);
        }
    }

    internal class PocoAdapter
    {
        private readonly Type _targetType;
        private readonly TypeInfo _targetTypeInfo;

        public PocoAdapter(Type targetType)
        {
            _targetType = targetType;
            _targetTypeInfo = targetType.GetTypeInfo();

            if (_targetTypeInfo.IsValueType)
            {
                throw new ArgumentException($"The type '{_targetTypeInfo.Name}' cannot be retrieved because it is not a reference type.");
            }

            var hasDefaultConstructor = _targetTypeInfo.DeclaredConstructors.All(c => c.GetParameters().Length == 0);
            if (!hasDefaultConstructor)
            {
                throw new ArgumentException($"The type '{_targetTypeInfo.Name}' cannot be retrieved because it does not have a parameterless constructor.");
            }
        }

        public bool IsSupportedData(object data, out Map dictionary)
        {
            // TODO remove this when refactoring how deserialization works
            var dataAsExpandedProperty = data as ExpandedProperty;
            if (dataAsExpandedProperty != null)
            {
                dictionary = dataAsExpandedProperty.Data;
                return true;
            }

            dictionary = data as IDictionary<string, object>;

            return dictionary != null;
        }

        public object Adapt(IDictionary<string, object> data, StringComparer comparer = null)
        {
            var result = Activator.CreateInstance(_targetType);

            if (comparer != null)
            {
                data = new Dictionary<string, object>(data, comparer);
            }

            foreach (var property in _targetTypeInfo.DeclaredProperties.Where(p => p.CanWrite))
            {
                object value;
                if (!data.TryGetValue(property.Name, out value) || value == null)
                {
                    continue;
                }

                if (!IsClass(property.PropertyType))
                {
                    ConvertAndSet(result, property, value);
                    continue;
                }

                IDictionary<string, object> propertyDictionary;
                if (!IsSupportedData(value, out propertyDictionary))
                {
                    continue;
                }

                // Recurse (assume it's another POCO)
                var adapter = new PocoAdapter(property.PropertyType);
                property.SetValue(result, adapter.Adapt(propertyDictionary, comparer));
            }

            return result;
        }

        private static void ConvertAndSet(object obj, PropertyInfo property, object value)
        {
            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var convertedValue = value == null
                ? null
                : Convert.ChangeType(value, targetType);
            property.SetValue(obj, convertedValue);
        }

        private static bool IsClass(Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            var typeInfo = type.GetTypeInfo();

            if (typeof(StringEnumeration).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                return false;
            }

            return type.GetTypeInfo().IsClass;
        }
    }
}
