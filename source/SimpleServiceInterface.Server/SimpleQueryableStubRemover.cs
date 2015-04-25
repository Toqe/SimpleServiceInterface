using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Server
{
    public class SimpleQueryableStubRemover : ExpressionVisitor
    {
        private object replacement;

        private Type type;

        public SimpleQueryableStubRemover(object replacement, Type type)
        {
            this.replacement = replacement;
            this.type = type;
        }

        public Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p.Name == "__SimpleQueryableContext__")
            {
                return Expression.Constant(this.replacement, this.type);
            }

            return p;
        }
    }
}
