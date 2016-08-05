using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.CustomData
{
    /// <summary>
    /// Used to marshall dictionaries in Custom Data to POCOs.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    internal sealed class PocoAdapter<T>
    {
        private readonly TypeInfo _targetTypeInfo;

        public PocoAdapter()
        {
            _targetTypeInfo = typeof(T).GetTypeInfo();

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

        public T Adapt(IDictionary<string, object> data)
        {
            var result = Activator.CreateInstance(typeof(T));

            foreach (var property in _targetTypeInfo.DeclaredProperties.Where(p => p.CanWrite))
            {
                object value;
                if (data.TryGetValue(property.Name, out value))
                {
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(result, convertedValue);
                }
            }

            return (T)result;
        }
    }
}
