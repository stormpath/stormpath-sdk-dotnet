// <copyright file="AbstractRangedWhereTermParser.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing.RangedTerms
{
    internal abstract class AbstractRangedWhereTermParser<TModel>
        where TModel : AbstractRangedWhereTermWorkingModel, new()
    {
        public IEnumerable<KeyValuePair<string, string>> Parse(IEnumerable<WhereTerm> terms)
        {
            var workingModels = ParseAndConsolidate(terms);
            var attributes = CreateAttributes(workingModels);

            return attributes;
        }

        protected abstract bool HasStart(TModel model);

        protected abstract bool HasEnd(TModel model);

        protected abstract object GetStart(TModel model);

        protected abstract object GetEnd(TModel model);

        protected abstract void SetStart(TModel model, object value);

        protected abstract void SetEnd(TModel model, object value);

        protected abstract string Format(object value);

        private IEnumerable<KeyValuePair<string, TModel>> ParseAndConsolidate(IEnumerable<WhereTerm> terms)
        {
            var workingModels = new Dictionary<string, TModel>();

            foreach (var term in terms)
            {
                if (!workingModels.ContainsKey(term.FieldName))
                {
                    workingModels.Add(term.FieldName, new TModel());
                }

                var workingModel = workingModels[term.FieldName];

                bool isStartTerm =
                    term.Comparison == WhereComparison.GreaterThan ||
                    term.Comparison == WhereComparison.GreaterThanOrEqual;
                bool collision =
                    (isStartTerm && (HasStart(workingModel) || workingModel.StartInclusive != null)) ||
                    (!isStartTerm && (HasEnd(workingModel) || workingModel.EndInclusive != null));
                if (collision)
                {
                    throw new ArgumentException("Error compiling range terms.");
                }

                workingModel.FieldName = term.FieldName;

                bool isInclusive =
                    term.Comparison == WhereComparison.GreaterThanOrEqual ||
                    term.Comparison == WhereComparison.LessThanOrEqual;
                if (isStartTerm)
                {
                    SetStart(workingModel, term.Value);
                    workingModel.StartInclusive = term.Comparison == WhereComparison.GreaterThanOrEqual;
                }
                else
                {
                    SetEnd(workingModel, term.Value);
                    workingModel.EndInclusive = term.Comparison == WhereComparison.LessThanOrEqual;
                }
            }

            return workingModels;
        }

        private IEnumerable<KeyValuePair<string, string>> CreateAttributes(IEnumerable<KeyValuePair<string, TModel>> workingModels)
        {
            var usedArguments = new List<string>();
            var attributeBuilder = new StringBuilder();

            foreach (var model in workingModels.Select(x => x.Value))
            {
                if (usedArguments.Contains(model.FieldName))
                {
                    throw new NotSupportedException($"Multiple constraints on the field {model.FieldName} are not supported.");
                }

                attributeBuilder.Clear();

                if (!HasStart(model))
                {
                    attributeBuilder.Append("[");
                }
                else
                {
                    attributeBuilder.Append(model.StartInclusive ?? true ? "[" : "(");
                    attributeBuilder.Append(Format(GetStart(model)));
                }

                attributeBuilder.Append(",");

                if (!HasEnd(model))
                {
                    attributeBuilder.Append("]");
                }
                else
                {
                    attributeBuilder.Append(Format(GetEnd(model)));
                    attributeBuilder.Append(model.EndInclusive ?? true ? "]" : ")");
                }

                usedArguments.Add(model.FieldName);

                yield return new KeyValuePair<string, string>(model.FieldName, attributeBuilder.ToString());
            }
        }
    }
}
