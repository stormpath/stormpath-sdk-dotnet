﻿// <copyright file="WhereMemberExpression.cs" company="Stormpath, Inc.">
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

using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions
{
    internal class WhereMemberExpression : ParsedExpression
    {
        public WhereMemberExpression(string field, object value, WhereComparison comparison)
        {
            this.FieldName = field;
            this.Value = value;
            this.Comparison = comparison;
        }

        public string FieldName { get; private set; }

        public object Value { get; private set; }

        public WhereComparison Comparison { get; private set; }

        protected internal override Expression Accept(CompilingExpressionVisitor visitor)
        {
            return visitor.VisitWhereMember(this);
        }
    }
}
