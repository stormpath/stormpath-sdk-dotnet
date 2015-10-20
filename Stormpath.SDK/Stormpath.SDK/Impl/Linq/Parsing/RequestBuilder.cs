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

            // From .OrderBy(x => ?) / .OrderByDescending(x => ?)
            if (queryModel.OrderByTerms.Count > 0)
            {
                var orderByArgument = new StringBuilder();
                bool addedOne = false;

                foreach (var clause in queryModel.OrderByTerms.Reverse<OrderBy>())
                {
                    if (addedOne)
                        orderByArgument.Append(",");
                    var direction = clause.Direction == OrderByDirection.Descending ? " desc" : string.Empty;
                    orderByArgument.Append($"{clause.FieldName}{direction}");
                    addedOne = true;
                }

                if (addedOne)
                    arguments.Add("orderBy", orderByArgument.ToString());
            }

            var argumentList = arguments
                .Select(x => $"{x.Key}={x.Value}")
                .ToList();

            return argumentList;
        }
    }
}
