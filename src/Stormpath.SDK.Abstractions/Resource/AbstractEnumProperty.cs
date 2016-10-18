using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Resource
{
    public abstract class AbstractEnumProperty : StringEnumeration
    {
        protected AbstractEnumProperty(string value)
            : base(value)
        {
        }
    }
}
