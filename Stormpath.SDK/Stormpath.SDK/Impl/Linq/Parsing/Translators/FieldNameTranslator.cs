using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Linq.Parsing.Translators
{
    internal class FieldNameTranslator : AbstractNameTranslator
    {
        private static Dictionary<string, string> fieldNameMap = new Dictionary<string, string>()
        {
            ["Email"] = "email",
            ["GivenName"] = "givenName",
            ["MiddleName"] = "middleName",
            ["Surname"] = "surname",
            ["Username"] = "username",
            ["Name"] = "name",
            ["Description"] = "description",
            ["Status"] = "status"
        };

        public FieldNameTranslator()
            : base(fieldNameMap)
        {
        }
    }
}
