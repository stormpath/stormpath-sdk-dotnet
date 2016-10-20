using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Resource
{
    /// <summary>
    /// Defines a resource property that is a string-based enumeration.
    /// </summary>
    public abstract class AbstractEnumProperty : StringEnumeration
    {
        /// <summary>
        /// Creates a new <see cref="AbstractEnumProperty"/> instance.
        /// </summary>
        /// <param name="value">The value to use.</param>
        protected AbstractEnumProperty(string value)
            : base(value)
        {
        }
    }
}
