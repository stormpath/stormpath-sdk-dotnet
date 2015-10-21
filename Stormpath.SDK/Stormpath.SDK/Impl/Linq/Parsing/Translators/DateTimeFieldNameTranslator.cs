using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Linq.Parsing.Translators
{
    internal class DateTimeFieldNameTranslator : AbstractNameTranslator
    {
        private static Dictionary<string, string> fieldNameMap = new Dictionary<string, string>()
        {
            ["CreatedAt"] = "createdAt",
            ["ModifiedAt"] = "modifiedAt",
        };

        public DateTimeFieldNameTranslator()
            : base(fieldNameMap)
        {
        }
    }
}
