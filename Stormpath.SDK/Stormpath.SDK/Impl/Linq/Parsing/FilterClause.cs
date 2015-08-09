// <copyright file="FilterClause.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using Remotion.Linq.Clauses;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal class FilterClause : WhereClause
    {
        public FilterClause(Expression predicate)
            : base(predicate)
        {
            var filterTerm = predicate as ConstantExpression;
            if (filterTerm == null)
                throw new NotSupportedException("Filter must operate on a constant value.");

            var stringTerm = (string)filterTerm.Value;
            if (string.IsNullOrEmpty(stringTerm))
                throw new NotSupportedException("Filter term cannot be empty.");
            this.Term = stringTerm;
        }

        public string Term { get; private set; }

        public override string ToString()
        {
            return $"Filter({this.Term}";
        }
    }
}
