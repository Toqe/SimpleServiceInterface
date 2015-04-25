using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Client
{
    internal class SimpleQueryableStubInserter<T> : ExpressionVisitor
    {
        public Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Type == typeof(SimpleQueryableContext<T>))
            {
                return Expression.Parameter(typeof(IQueryable<T>), "__SimpleQueryableContext__");
            }

            return c;
        }
    }
}
