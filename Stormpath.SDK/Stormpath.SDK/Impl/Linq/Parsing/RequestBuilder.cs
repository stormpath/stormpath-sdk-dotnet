using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class RequestBuilder
    {
        public static IList<string> GetArguments(CollectionResourceQueryModel queryModel)
        {
            var builder = new RequestBuilder();
            return builder.GenerateArgumentsFromModel(queryModel);
        }

        private List<string> GenerateArgumentsFromModel(CollectionResourceQueryModel queryModel)
        {
            var arguments = new Dictionary<string, string>();

            // From .Take()
            if (queryModel?.Limit > 0)
                arguments.Add("limit", queryModel.Limit.Value.ToString());

            // From .Skip()
            if (queryModel?.Offset > 0)
                arguments.Add("offset", queryModel.Offset.Value.ToString());

            var argumentList = arguments
                .Select(x => $"{x.Key}={x.Value}")
                .ToList();

            return argumentList;
        }
    }
}
