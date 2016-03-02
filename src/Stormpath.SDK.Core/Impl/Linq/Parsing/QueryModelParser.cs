// <copyright file="QueryModelParser.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stormpath.SDK.Impl.Linq.Parsing.RangedTerms;
using Stormpath.SDK.Impl.Linq.QueryModel;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class QueryModelParser
    {
        private static readonly Type[] SupportedNumericIntegerTypes = new Type[]
        {
            typeof(short),
            typeof(int),
            typeof(long),
        };

        private static readonly Type[] SupportedNumericDecimalTypes = new Type[]
        {
            typeof(float),
            typeof(double),
        };

        public static IList<string> GetArguments(CollectionResourceQueryModel queryModel)
        {
            var builder = new QueryModelParser(queryModel);
            return builder.GenerateArguments();
        }

        private readonly CollectionResourceQueryModel queryModel;
        private readonly Dictionary<string, string> arguments;

        public QueryModelParser(CollectionResourceQueryModel queryModel)
        {
            this.queryModel = queryModel;
            this.arguments = new Dictionary<string, string>();
        }

        private List<string> GenerateArguments()
        {
            if (!this.arguments.Any())
            {
                this.HandleFilter();
                this.HandleLimit();
                this.HandleOffset();
                this.HandleOrderByThenBy();
                this.HandleWhere();
                this.HandleExpand();
            }

            var argumentList = this.arguments
                .Select(x => $"{x.Key}={x.Value}")
                .ToList();

            return argumentList;
        }

        private void HandleFilter()
        {
            if (!string.IsNullOrEmpty(this.queryModel.FilterTerm))
            {
                this.arguments.Add("q", this.queryModel.FilterTerm);
            }
        }

        private void HandleLimit()
        {
            if (this.queryModel.Limit > 0)
            {
                this.arguments.Add("limit", this.queryModel.Limit.Value.ToString());
            }
        }

        private void HandleOffset()
        {
            if (this.queryModel.Offset > 0)
            {
                this.arguments.Add("offset", this.queryModel.Offset.Value.ToString());
            }
        }

        private void HandleOrderByThenBy()
        {
            if (!this.queryModel.OrderByTerms.Any())
            {
                return;
            }

            var orderByArgument = new StringBuilder();
            bool addedOne = false;

            foreach (var clause in this.queryModel.OrderByTerms)
            {
                if (addedOne)
                {
                    orderByArgument.Append(",");
                }

                var direction = clause.Direction == OrderByDirection.Descending ? " desc" : string.Empty;
                orderByArgument.Append($"{clause.FieldName}{direction}");
                addedOne = true;
            }

            if (addedOne)
            {
                this.arguments.Add("orderBy", orderByArgument.ToString());
            }
        }

        private void HandleWhere()
        {
            var stringTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(string));

            var datetimeTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DateTimeOffset));

            var datetimeShorthandTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DatetimeShorthandModel));

            var integerTerms = this.queryModel.WhereTerms
                .Where(x => SupportedNumericIntegerTypes.Contains(x.Type));

            var decimalTerms = this.queryModel.WhereTerms
                .Where(x => SupportedNumericDecimalTypes.Contains(x.Type));

            var compiledArguments = HandleWhereStringTerms(stringTerms)
                .Concat(HandleWhereRangedDateTerms(datetimeTerms))
                .Concat(HandleWhereDateShorthandTerms(datetimeShorthandTerms))
                .Concat(HandleWhereRangedIntegerTerms(integerTerms))
                .Concat(HandleWhereRangedDecimalTerms(decimalTerms));

            foreach (var arg in compiledArguments)
            {
                this.arguments.Add(arg.Key, arg.Value);
            }

            var remainingTerms = this.queryModel.WhereTerms
                .Except(stringTerms)
                .Except(datetimeTerms)
                .Except(datetimeShorthandTerms)
                .Except(integerTerms)
                .Except(decimalTerms);

            if (remainingTerms.Any())
            {
                throw new NotSupportedException("One or more Where conditions were not parsed correctly.");
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> HandleWhereStringTerms(IEnumerable<WhereTerm> terms)
        {
            foreach (var term in terms)
            {
                var value = string.Empty;

                switch (term.Comparison)
                {
                    case WhereComparison.Contains:
                        value = $"*{term.Value.ToString()}*";
                        break;
                    case WhereComparison.EndsWith:
                        value = $"*{term.Value.ToString()}";
                        break;
                    case WhereComparison.StartsWith:
                        value = $"{term.Value.ToString()}*";
                        break;
                    case WhereComparison.Equal:
                        value = term.Value.ToString();
                        break;
                    default:
                        throw new NotSupportedException($"The comparison operator {term.Comparison.ToString()} is not supported on this field.");
                }

                yield return new KeyValuePair<string, string>(term.FieldName, value);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> HandleWhereRangedDateTerms(IEnumerable<WhereTerm> terms)
        {
            var compiled = new RangedDatetimeWhereTermParser().Parse(terms);
            return compiled;
        }

        private IEnumerable<KeyValuePair<string, string>> HandleWhereDateShorthandTerms(IEnumerable<WhereTerm> terms)
        {
            var shorthandAttributeBuilder = new StringBuilder();

            foreach (var term in terms)
            {
                var shorthandModel = term.Value as DatetimeShorthandModel;
                if (shorthandModel == null)
                {
                    throw new ArgumentException("One or more Within constraints are invalid.");
                }

                shorthandAttributeBuilder.Clear();

                shorthandAttributeBuilder.Append(shorthandModel.Year);
                if (shorthandModel.Month.HasValue)
                {
                    shorthandAttributeBuilder.Append($"-{shorthandModel.Month.Value:D2}");
                }

                if (shorthandModel.Month.HasValue &&
                    shorthandModel.Day.HasValue)
                {
                    shorthandAttributeBuilder.Append($"-{shorthandModel.Day.Value:D2}");
                }

                if (shorthandModel.Month.HasValue &&
                    shorthandModel.Day.HasValue &&
                    shorthandModel.Hour.HasValue)
                {
                    shorthandAttributeBuilder.Append($"T{shorthandModel.Hour.Value:D2}");
                }

                if (shorthandModel.Month.HasValue &&
                    shorthandModel.Day.HasValue &&
                    shorthandModel.Hour.HasValue &&
                    shorthandModel.Minute.HasValue)
                {
                    shorthandAttributeBuilder.Append($":{shorthandModel.Minute.Value:D2}");
                }

                if (shorthandModel.Month.HasValue &&
                    shorthandModel.Day.HasValue &&
                    shorthandModel.Hour.HasValue &&
                    shorthandModel.Hour.HasValue &&
                    shorthandModel.Second.HasValue)
                {
                    shorthandAttributeBuilder.Append($":{shorthandModel.Second.Value:D2}");
                }

                yield return new KeyValuePair<string, string>(term.FieldName, shorthandAttributeBuilder.ToString());
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> HandleWhereRangedIntegerTerms(IEnumerable<WhereTerm> terms)
        {
            var compiled = new RangedLongWhereTermParser().Parse(terms);
            return compiled;
        }

        private static IEnumerable<KeyValuePair<string, string>> HandleWhereRangedDecimalTerms(IEnumerable<WhereTerm> terms)
        {
            var compiled = new RangedDoubleWhereTermParser().Parse(terms);
            return compiled;
        }

        private void HandleExpand()
        {
            if (!this.queryModel.ExpandTerms.Any())
            {
                return;
            }

            var expansionArgument = new StringBuilder();
            bool addedOne = false;

            foreach (var item in this.queryModel.ExpandTerms)
            {
                if (addedOne)
                {
                    expansionArgument.Append(",");
                }

                expansionArgument.Append(item.PropertyName);

                bool hasSubparameters = item.Offset.HasValue || item.Limit.HasValue;
                if (hasSubparameters)
                {
                    expansionArgument.Append("(");
                }

                if (item.Offset.HasValue)
                {
                    expansionArgument.Append($"offset:{item.Offset.Value}");
                }

                if (item.Limit.HasValue)
                {
                    if (item.Offset.HasValue)
                    {
                        expansionArgument.Append(",");
                    }

                    expansionArgument.Append($"limit:{item.Limit.Value}");
                }

                if (hasSubparameters)
                {
                    expansionArgument.Append(")");
                }

                addedOne = true;
            }

            if (addedOne)
            {
                this.arguments.Add("expand", expansionArgument.ToString());
            }
        }
    }
}
