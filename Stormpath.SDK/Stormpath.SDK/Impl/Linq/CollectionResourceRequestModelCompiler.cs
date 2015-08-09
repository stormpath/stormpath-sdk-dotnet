// <copyright file="CollectionResourceRequestModelCompiler.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Linq
{
    internal static class CollectionResourceRequestModelCompiler
    {
        public static List<string> GetArguments(CollectionResourceRequestModel model)
        {
            var arguments = new Dictionary<string, string>();

            // From .Filter()
            if (!string.IsNullOrEmpty(model.FilterTerm))
                arguments.Add("q", model.FilterTerm);

            // From .Where(x => ?)
            if (model.AttributeTerms.Count > 0)
            {
                foreach (var term in model.AttributeTerms)
                {
                    switch (term.MatchType)
                    {
                        case StringAttributeMatchingType.Contains:
                            arguments.Add(term.Field, $"*{term.Value}*");
                            break;
                        case StringAttributeMatchingType.EndsWith:
                            arguments.Add(term.Field, $"*{term.Value}");
                            break;
                        case StringAttributeMatchingType.StartsWith:
                            arguments.Add(term.Field, $"{term.Value}*");
                            break;
                        case StringAttributeMatchingType.Equals:
                        default:
                            arguments.Add(term.Field, term.Value);
                            break;
                    }
                }
            }

            // From .Where(x => [createdAt|modifiedAt] >= ??)
            if (model.DatetimeAttributeTerms.Count > 0)
            {
                var aggregatedTerms = AggregateDatetimeTerms(model.DatetimeAttributeTerms);

                foreach (var term in aggregatedTerms)
                {
                    var datetimeAttribute = new StringBuilder();

                    if (!term.Start.HasValue)
                    {
                        datetimeAttribute.Append("[");
                    }
                    else
                    {
                        datetimeAttribute.Append(term.StartInclusive ?? true ? "[" : "(");
                        datetimeAttribute.Append(Iso8601.Format(term.Start.Value));
                    }

                    datetimeAttribute.Append(",");

                    if (!term.End.HasValue)
                    {
                        datetimeAttribute.Append("]");
                    }
                    else
                    {
                        datetimeAttribute.Append(Iso8601.Format(term.End.Value));
                        datetimeAttribute.Append(term.EndInclusive ?? true ? "]" : ")");
                    }

                    arguments.Add(term.Field, datetimeAttribute.ToString());
                }
            }

            // From .Take()
            if (model?.Limit > 0)
                arguments.Add("limit", model.Limit.Value.ToString());

            // From .Skip()
            if (model?.Offset > 0)
                arguments.Add("offset", model.Offset.Value.ToString());

            // From .OrderBy(x => ?) / .OrderByDescending(x => ?)
            if (model.OrderByTerms.Count > 0)
            {
                var orderByArgument = new StringBuilder();
                bool addedOne = false;

                foreach (var clause in model.OrderByTerms)
                {
                    if (addedOne)
                        orderByArgument.Append(",");
                    var direction = clause.Descending ? " desc" : string.Empty;
                    orderByArgument.Append($"{clause.Field}{direction}");
                    addedOne = true;
                }

                if (addedOne)
                    arguments.Add("orderBy", orderByArgument.ToString());
            }

            // From .Expand()
            if (model.Expansions.Count > 0)
            {
                var expansionArgument = new StringBuilder();
                bool addedOne = false;

                foreach (var item in model.Expansions)
                {
                    if (!item.IsValid()) // && item.CanBeUsedForThisResource
                        throw new ArgumentException($"Expansion term '{item.Field}' is not valid.");

                    if (addedOne)
                        expansionArgument.Append(",");

                    bool hasSubparameters = item.Offset.HasValue || item.Limit.HasValue;
                    expansionArgument.Append(item.Field);

                    if (hasSubparameters)
                        expansionArgument.Append("(");

                    if (item.Offset.HasValue)
                        expansionArgument.Append($"offset:{item.Offset.Value}");

                    if (item.Limit.HasValue)
                    {
                        if (item.Offset.HasValue)
                            expansionArgument.Append(",");
                        expansionArgument.Append($"limit:{item.Limit.Value}");
                    }

                    if (hasSubparameters)
                        expansionArgument.Append(")");

                    addedOne = true;
                }

                if (addedOne)
                    arguments.Add("expand", expansionArgument.ToString());
            }

            var argumentList = arguments
                .Select(x => $"{x.Key}={x.Value}")
                .ToList();

            return argumentList;
        }

        private static List<DatetimeAttributeTermModel> AggregateDatetimeTerms(IEnumerable<DatetimeAttributeTermModel> terms)
        {
            // If a query has multiple terms, they'll be generated like so:
            //   ## a search with both starting and ending dates specified
            //   [0] field: 'createdAt', start: DateTimeOffset, startInclusive: true, end: null, endInclusive: null
            //   [1] field: 'createdAt', start: null, startInclusive: null, end: DateTimeOffset, endInclusive: true
            //   ## a search with only one date specified
            //   [3] field: 'modifiedAt', start: DateTimeOffset, startInclusive: false, end: null, endInclusive: null
            //
            // We need to aggregate this down to 1 term for each field.
            // TODO: refactor this to be less dumb and make this method unnecessary.
            var workingModels = new Dictionary<string, DatetimeAttributeTermWorkingModel>();

            foreach (var term in terms)
            {
                if (!workingModels.ContainsKey(term.Field))
                    workingModels.Add(term.Field, new DatetimeAttributeTermWorkingModel());
                var workingModel = workingModels[term.Field];

                bool collision =
                    (term.Start.HasValue && workingModel.Start.HasValue) ||
                    (term.StartInclusive.HasValue && workingModel.StartInclusive.HasValue) ||
                    (term.End.HasValue && workingModel.End.HasValue) ||
                    (term.EndInclusive.HasValue && workingModel.EndInclusive.HasValue);
                if (collision)
                    throw new ArgumentException("Error compiling date terms.");

                workingModel.Field = term.Field;
                workingModel.Start = term.Start ?? workingModel.Start;
                workingModel.StartInclusive = term.StartInclusive ?? workingModel.StartInclusive;
                workingModel.End = term.End ?? workingModel.End;
                workingModel.EndInclusive = term.EndInclusive ?? workingModel.EndInclusive;
            }

            return workingModels
                .Select(kvp => new DatetimeAttributeTermModel(kvp.Value.Field, kvp.Value.Start, kvp.Value.StartInclusive, kvp.Value.End, kvp.Value.EndInclusive))
                .ToList();
        }

        private class DatetimeAttributeTermWorkingModel
        {
            public string Field { get; set; }

            public DateTimeOffset? Start { get; set; }

            public bool? StartInclusive { get; set; }

            public DateTimeOffset? End { get; set; }

            public bool? EndInclusive { get; set; }
        }
    }
}
