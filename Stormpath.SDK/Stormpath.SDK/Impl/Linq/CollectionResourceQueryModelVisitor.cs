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
    internal class CollectionResourceQueryModelVisitor : QueryModelVisitorBase
    {
        public CollectionResourceRequestModel ParsedModel { get; private set; } = new CollectionResourceRequestModel();

        public static CollectionResourceRequestModel GenerateRequestModel(QueryModel queryModel)
        {
            var visitor = new CollectionResourceQueryModelVisitor();
            visitor.VisitQueryModel(queryModel);
            return visitor.ParsedModel;
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            // Todo Any
            // TODO First
            // TODO Count/LongCount
            // Todo DefaultIfEmpty
            // Todo ElementAt[OrDefault]
            // TODO Single
            var takeResultOperator = resultOperator as TakeResultOperator;
            if (takeResultOperator != null)
            {
                var expression = takeResultOperator.Count;
                if (expression.NodeType == ExpressionType.Constant)
                {
                    ParsedModel.Limit = (int)((ConstantExpression)expression).Value;
                }
                else
                {
                    throw new NotSupportedException("Unsupported expression in Take clause.");
                }

                return; // done
            }

            var skipResultOperator = resultOperator as SkipResultOperator;
            if (skipResultOperator != null)
            {
                var expression = skipResultOperator.Count as ConstantExpression;
                if (expression == null)
                    throw new NotSupportedException("Unsupported expression in Skip clause.");

                ParsedModel.Offset = (int)expression.Value;
                return; // done
            }

            var expandResultOperator = resultOperator as ExpandResultOperator;
            if (expandResultOperator != null)
            {
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

                    ParsedModel.Expansions.Add(new ExpansionTerm(expandField));
                    return; // done
                }

                if (CollectionLinkMethodTranslator.TryGetValue(methodCallExpression.Method.Name, out expandField))
                {
                    ParsedModel.Expansions.Add(new ExpansionTerm(expandField,
                        (int?)expandResultOperator.Offset.Value,
                        (int?)expandResultOperator.Limit.Value));
                    return; // done
                }

                throw new NotSupportedException("The selected method does not support expansions.");
            }

            bool isUnsupported =
                resultOperator is AllResultOperator ||
                resultOperator is AggregateResultOperator ||
                resultOperator is AggregateFromSeedResultOperator ||
                resultOperator is AverageResultOperator ||
                resultOperator is CastResultOperator ||
                resultOperator is ContainsResultOperator ||
                resultOperator is DistinctResultOperator ||
                resultOperator is ExceptResultOperator ||
                resultOperator is GroupResultOperator ||
                resultOperator is IntersectResultOperator ||
                resultOperator is LastResultOperator ||
                resultOperator is MaxResultOperator ||
                resultOperator is MinResultOperator ||
                resultOperator is OfTypeResultOperator ||
                resultOperator is ReverseResultOperator ||
                resultOperator is SumResultOperator ||
                resultOperator is UnionResultOperator;
            if (isUnsupported)
            {
                throw new NotSupportedException("One or more LINQ operators are not supported.");
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            // Handle custom .Filter extension method
            var filterClause = whereClause as FilterClause;
            if (filterClause != null)
            {
                ParsedModel.FilterTerm = filterClause.Term;
                return; // done
            }

            this.ParsedModel.AddAttributeTerms(
                CollectionResourceWhereExpressionVisitor.GenerateModels(whereClause.Predicate));

            base.VisitWhereClause(whereClause, queryModel, index);
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
            // TODO This also applies to LINQ query sytax: from x select x is fine
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
