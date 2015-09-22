// <copyright file="CollectionResourceQueryModelVisitor.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Stormpath.SDK.Impl.Linq.StaticNameTranslators;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryModelVisitor : QueryModelVisitorBase
    {
        private static readonly int DefaultApiPageLimit = 100;

        private static readonly List<Type> SupportedOperators = new List<Type>()
        {
            typeof(AnyResultOperator),
            typeof(CountResultOperator),
            typeof(LongCountResultOperator),
            typeof(FirstResultOperator),
            typeof(SingleResultOperator),
            typeof(TakeResultOperator),
            typeof(SkipResultOperator),
            typeof(ExpandResultOperator),
        };

        public CollectionResourceRequestModel ParsedModel { get; private set; } = new CollectionResourceRequestModel();

        public static CollectionResourceRequestModel GenerateRequestModel(QueryModel queryModel)
        {
            var visitor = new CollectionResourceQueryModelVisitor();
            visitor.VisitQueryModel(queryModel);
            return visitor.ParsedModel;
        }

        private static bool IsSupportedOperator(ResultOperatorBase resultOperator)
        {
            return SupportedOperators.Contains(resultOperator.GetType());
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            // Used to store the original type the query is executing against
            // (see ExecuteScalar in CollectionResourceQueryExecutor for why we need this)
            this.ParsedModel.CollectionType = fromClause.ItemType;

            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (!IsSupportedOperator(resultOperator))
                throw new NotSupportedException("One or more LINQ operators are not supported.");

            bool shouldReturnOnlyOne =
                resultOperator is AnyResultOperator ||
                resultOperator is CountResultOperator ||
                resultOperator is LongCountResultOperator ||
                resultOperator is FirstResultOperator;
            if (shouldReturnOnlyOne)
            {
                this.ParsedModel.Limit = 1;
                this.ParsedModel.ExecutionPlan.MaxItems = 1;

                return; // done
            }

            if (this.HandleTakeResultOperator(resultOperator))
                return; // done

            if (this.HandleSkipResultOperator(resultOperator))
                return; // done

            if (this.HandleExpandExtensionResultOperator(resultOperator))
                return; // done

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        private bool HandleTakeResultOperator(ResultOperatorBase resultOperator)
        {
            var takeResultOperator = resultOperator as TakeResultOperator;
            if (takeResultOperator == null)
                return false;

            var expression = takeResultOperator.Count as ConstantExpression;
            if (expression == null)
                throw new NotSupportedException("Unsupported expression in Take clause.");

            var value = (int)expression.Value;
            if (value < 1)
                throw new ArgumentOutOfRangeException("Take must be greater than zero.");

            this.ParsedModel.ExecutionPlan.MaxItems = value;
            this.ParsedModel.Limit = value;

            if (value > DefaultApiPageLimit)
                this.ParsedModel.Limit = DefaultApiPageLimit;

            return true;
        }

        private bool HandleSkipResultOperator(ResultOperatorBase resultOperator)
        {
            var skipResultOperator = resultOperator as SkipResultOperator;
            if (skipResultOperator == null)
                return false;

            var expression = skipResultOperator.Count as ConstantExpression;
            if (expression == null)
                throw new NotSupportedException("Unsupported expression in Skip clause.");

            var value = (int)expression.Value;
            if (value < 1)
                throw new ArgumentOutOfRangeException("Skip must be greater than zero.");

            this.ParsedModel.Offset = value;

            return true;
        }

        private bool HandleExpandExtensionResultOperator(ResultOperatorBase resultOperator)
        {
            var expandResultOperator = resultOperator as ExpandResultOperator;
            if (expandResultOperator == null)
                return false;

            var methodCallExpression = expandResultOperator.KeySelector as MethodCallExpression;
            if (methodCallExpression == null)
                throw new NotSupportedException("Expand must be used on a link property method.");

            var expandField = string.Empty;
            if (LinkMethodNameTranslator.TryGetValue(methodCallExpression.Method.Name, out expandField))
            {
                bool paginationParametersPresent =
                    expandResultOperator?.Offset?.Value != null ||
                    expandResultOperator?.Limit?.Value != null;
                if (paginationParametersPresent)
                    throw new NotSupportedException("Pagination options cannot be used on link-only properties.");

                this.ParsedModel.Expansions.Add(new ExpansionTerm(expandField));
                return true; // done
            }

            if (CollectionLinkMethodNameTranslator.TryGetValue(methodCallExpression.Method.Name, out expandField))
            {
                this.ParsedModel.Expansions.Add(
                    new ExpansionTerm(
                        expandField,
                        (int?)expandResultOperator.Offset.Value,
                        (int?)expandResultOperator.Limit.Value));
                return true; // done
            }

            throw new NotSupportedException("The selected method does not support expansions.");
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            // Handle simple cases
            if (this.HandleWhereFilterExtensionMethod(whereClause))
                return; // done

            this.ParsedModel.AddAttributeTerms(
                CollectionResourceWhereExpressionVisitor.GenerateModels(whereClause.Predicate));

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        private bool HandleWhereFilterExtensionMethod(WhereClause whereClause)
        {
            var filterClause = whereClause as FilterClause;
            if (filterClause == null)
                return false;

            if (!string.IsNullOrEmpty(this.ParsedModel.FilterTerm))
                throw new NotSupportedException("Multiple Filter terms are not supported");

            this.ParsedModel.FilterTerm = filterClause.Term;
            return true;
        }

        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
        {
            var memberAccessor = ordering.Expression as MemberExpression;
            if (memberAccessor == null)
                throw new NotSupportedException("Only resource fields are supported in OrderBy.");

            var fieldName = string.Empty;
            if (!FieldNameTranslator.TryGetValue(memberAccessor.Member.Name, out fieldName))
                throw new NotSupportedException($"The field {memberAccessor.Member.Name} is not supported in OrderBy.");

            this.ParsedModel.AddOrderBy(fieldName, descending: ordering.OrderingDirection == OrderingDirection.Desc);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            // We have to determine if this Select is being called internally by a LINQ filter like Take
            // which essentially appends .Select(x => x) at the end of the query. If so, it's fine.
            bool isInternalSelect = queryModel.MainFromClause ==
                (selectClause.Selector as QuerySourceReferenceExpression)?.ReferencedQuerySource as MainFromClause;

            if (!isInternalSelect)
                throw new NotSupportedException("Select is not supported.");

            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("GroupJoin is not supported.");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Join is not supported.");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            throw new NotSupportedException("Join is not supported.");
        }
    }
}
