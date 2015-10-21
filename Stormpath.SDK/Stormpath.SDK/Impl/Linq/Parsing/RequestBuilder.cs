// <copyright file="RequestBuilder.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Impl.Linq.QueryModel;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class RequestBuilder
    {
        public static IList<string> GetArguments(CollectionResourceQueryModel queryModel)
        {
            var builder = new RequestBuilder(queryModel);
            return builder.GenerateArguments();
        }

        private readonly CollectionResourceQueryModel queryModel;
        private readonly Dictionary<string, string> arguments;

        public RequestBuilder(CollectionResourceQueryModel queryModel)
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
            }

            var argumentList = this.arguments
                .Select(x => $"{x.Key}={x.Value}")
                .ToList();

            return argumentList;
        }

        private void HandleFilter()
        {
            if (!string.IsNullOrEmpty(this.queryModel.FilterTerm))
                this.arguments.Add("q", this.queryModel.FilterTerm);
        }

        private void HandleLimit()
        {
            if (this.queryModel.Limit > 0)
                this.arguments.Add("limit", this.queryModel.Limit.Value.ToString());
        }

        private void HandleOffset()
        {
            if (this.queryModel.Offset > 0)
                this.arguments.Add("offset", this.queryModel.Offset.Value.ToString());
        }

        private void HandleOrderByThenBy()
        {
            if (!this.queryModel.OrderByTerms.Any())
                return;

            var orderByArgument = new StringBuilder();
            bool addedOne = false;

            foreach (var clause in this.queryModel.OrderByTerms)
            {
                if (addedOne)
                    orderByArgument.Append(",");
                var direction = clause.Direction == OrderByDirection.Descending ? " desc" : string.Empty;
                orderByArgument.Append($"{clause.FieldName}{direction}");
                addedOne = true;
            }

            if (addedOne)
                this.arguments.Add("orderBy", orderByArgument.ToString());
        }

        private void HandleWhere()
        {
            var datetimeTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DateTimeOffset))
                .ToList();

            var datetimeShorthandTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DatetimeShorthandModel))
                .ToList();

            var remainingTerms = this.queryModel.WhereTerms
                .Except(datetimeTerms)
                .Except(datetimeShorthandTerms);

            foreach (var term in remainingTerms)
            {
                switch (term.Comparison)
                {
                    case WhereComparison.Contains:
                        this.arguments.Add(term.FieldName, $"*{term.Value.ToString()}*");
                        break;
                    case WhereComparison.EndsWith:
                        this.arguments.Add(term.FieldName, $"*{term.Value.ToString()}");
                        break;
                    case WhereComparison.StartsWith:
                        this.arguments.Add(term.FieldName, $"{term.Value.ToString()}*");
                        break;
                    case WhereComparison.Equal:
                        this.arguments.Add(term.FieldName, term.Value.ToString());
                        break;
                    default:
                        throw new NotSupportedException($"The comparison operator {term.Comparison.ToString()} is not supported on this field.");
                }
            }

            this.HandleWhereDateTerms(datetimeTerms);
            this.HandleWhereDateShorthandTerms(datetimeShorthandTerms);
        }

        private void HandleWhereDateTerms(IList<WhereTerm> terms)
        {
            // Parse and consolidate terms
            var workingModels = new Dictionary<string, DatetimeAttributeTermWorkingModel>();
            foreach (var term in terms)
            {
                if (!workingModels.ContainsKey(term.FieldName))
                    workingModels.Add(term.FieldName, new DatetimeAttributeTermWorkingModel());
                var workingModel = workingModels[term.FieldName];

                bool isStartTerm =
                    term.Comparison == WhereComparison.GreaterThan ||
                    term.Comparison == WhereComparison.GreaterThanOrEqual;
                bool collision =
                    (isStartTerm && (workingModel.Start.HasValue || workingModel.StartInclusive.HasValue)) ||
                    (!isStartTerm && (workingModel.End.HasValue || workingModel.EndInclusive.HasValue));
                if (collision)
                    throw new ArgumentException("Error compiling date terms.");

                workingModel.FieldName = term.FieldName;

                bool isInclusive =
                    term.Comparison == WhereComparison.GreaterThanOrEqual ||
                    term.Comparison == WhereComparison.LessThanOrEqual;
                if (isStartTerm)
                {
                    workingModel.Start = (DateTimeOffset)term.Value;
                    workingModel.StartInclusive = term.Comparison == WhereComparison.GreaterThanOrEqual;
                }
                else
                {
                    workingModel.End = (DateTimeOffset)term.Value;
                    workingModel.EndInclusive = term.Comparison == WhereComparison.LessThanOrEqual;
                }
            }

            // Add terms to query
            var datetimeAttributeBuilder = new StringBuilder();
            foreach (var term in workingModels.Values)
            {
                if (this.arguments.ContainsKey(term.FieldName))
                    throw new NotSupportedException($"Multiple date constraints on field {term.FieldName} are not supported");

                datetimeAttributeBuilder.Clear();

                if (!term.Start.HasValue)
                {
                    datetimeAttributeBuilder.Append("[");
                }
                else
                {
                    datetimeAttributeBuilder.Append(term.StartInclusive ?? true ? "[" : "(");
                    datetimeAttributeBuilder.Append(Iso8601.Format(term.Start.Value));
                }

                datetimeAttributeBuilder.Append(",");

                if (!term.End.HasValue)
                {
                    datetimeAttributeBuilder.Append("]");
                }
                else
                {
                    datetimeAttributeBuilder.Append(Iso8601.Format(term.End.Value));
                    datetimeAttributeBuilder.Append(term.EndInclusive ?? true ? "]" : ")");
                }

                this.arguments.Add(term.FieldName, datetimeAttributeBuilder.ToString());
            }
        }

        private void HandleWhereDateShorthandTerms(IList<WhereTerm> terms)
        {
            var shorthandAttributeBuilder = new StringBuilder();

            foreach (var term in terms)
            {
                //todo move this to validator
                if (this.arguments.ContainsKey(term.FieldName))
                    throw new NotSupportedException($"Multiple date constraints on field {term.FieldName} are not supported");

                var shorthandModel = term.Value as DatetimeShorthandModel;
                if (shorthandModel == null)
                    throw new ArgumentException("One or more Within constraints are invalid.");

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

                this.arguments.Add(term.FieldName, shorthandAttributeBuilder.ToString());
            }
        }
    }
}
