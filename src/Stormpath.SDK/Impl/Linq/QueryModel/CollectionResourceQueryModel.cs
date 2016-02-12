// <copyright file="CollectionResourceQueryModel.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Impl.Linq.Parsing;

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal sealed class CollectionResourceQueryModel
    {
        public int? Offset { get; set; }

        public int? Limit { get; set; }

        public string FilterTerm { get; set; }

        public List<WhereTerm> WhereTerms { get; set; }
            = new List<WhereTerm>();

        public List<OrderByTerm> OrderByTerms { get; set; }
            = new List<OrderByTerm>();

        public List<ExpandTerm> ExpandTerms { get; set; }
            = new List<ExpandTerm>();

        public ExecutionPlanModel ExecutionPlan { get; set; }
            = new ExecutionPlanModel();

        public ResultOperator? ResultOperator { get; set; }

        public bool ResultDefaultIfEmpty { get; set; } = false;

        public static CollectionResourceQueryModel Default = new CollectionResourceQueryModel()
        {
            Offset = null,
            Limit = null,
            FilterTerm = null,
            ExecutionPlan = new ExecutionPlanModel()
            {
                MaxItems = null
            },
            ResultOperator = null,
            ResultDefaultIfEmpty = false
        };
    }
}
