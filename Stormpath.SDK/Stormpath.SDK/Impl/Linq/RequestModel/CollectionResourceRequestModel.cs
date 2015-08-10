// <copyright file="CollectionResourceRequestModel.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Linq.RequestModel
{
    internal class CollectionResourceRequestModel
    {
        public int? Offset { get; set; }

        public int? Limit { get; set; }

        public string FilterTerm { get; set; }

        public List<StringAttributeTermModel> StringAttributeTerms { get; } = new List<StringAttributeTermModel>();

        public List<DatetimeAttributeTermModel> DatetimeAttributeTerms { get; } = new List<DatetimeAttributeTermModel>();

        public List<DatetimeShorthandAttributeTermModel> DatetimeShorthandAttributeTerms { get; } = new List<DatetimeShorthandAttributeTermModel>();

        public List<OrderByModel> OrderByTerms { get; } = new List<OrderByModel>();

        public List<ExpansionTerm> Expansions { get; } = new List<ExpansionTerm>();

        public void AddAttributeTerm(AbstractAttributeTermModel model)
        {
            var modelAsString = model as StringAttributeTermModel;
            if (modelAsString != null)
            {
                this.StringAttributeTerms.Add(modelAsString);
                return; // done
            }

            var modelAsDatetime = model as DatetimeAttributeTermModel;
            if (modelAsDatetime != null)
            {
                this.DatetimeAttributeTerms.Add(modelAsDatetime);
                return; // done
            }

            var modelAsDatetimeShorthand = model as DatetimeShorthandAttributeTermModel;
            if (modelAsDatetimeShorthand != null)
            {
                this.DatetimeShorthandAttributeTerms.Add(modelAsDatetimeShorthand);
                return; // done
            }

            throw new NotSupportedException("Unknown attribute term model type.");
        }

        public void AddAttributeTerms(IEnumerable<AbstractAttributeTermModel> models)
        {
            foreach (var model in models)
            {
                AddAttributeTerm(model);
            }
        }

        public void AddDatetimeAttributeTerm(DatetimeAttributeTermModel model)
        {
            this.DatetimeAttributeTerms.Add(model);
        }

        public void AddOrderBy(string field, bool descending = false)
        {
            this.OrderByTerms.Add(new OrderByModel(field, descending));
        }

        public void AddExpansion(string field, int? offset = null, int? limit = null)
        {
            this.Expansions.Add(new ExpansionTerm(field, offset, limit));
        }
    }
}
