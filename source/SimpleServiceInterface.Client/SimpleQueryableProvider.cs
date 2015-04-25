using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Remote.Linq;

namespace SimpleServiceInterface.Client
{
    internal class SimpleQueryableProvider<T> : IQueryProvider
    {
        private SimpleServiceMethodCallInterceptor callInterceptor;

        private string url;

        private ParameterInfo[] parameterInfos;

        private SimpleServiceCallParameters parameters;

        public SimpleQueryableProvider(
            SimpleServiceMethodCallInterceptor callInterceptor,
            string url,
            ParameterInfo[] parameterInfos,
            SimpleServiceCallParameters parameters)
        {
            this.callInterceptor = callInterceptor;
            this.url = url;
            this.parameterInfos = parameterInfos;
            this.parameters = parameters;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new SimpleQueryableContext<T>(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new SimpleQueryableContext<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return this.Execute<T>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var visitor = new SimpleQueryableStubInserter<T>();
            var modifiedExpression = visitor.CopyAndModify(expression);

            var callParameters = new SimpleServiceCallParameters()
            {
                Parameters = this.parameters.Parameters,
                QueryExpression = modifiedExpression.ToRemoteLinqExpression()
            };

            var result = this.callInterceptor.Call(this.url, this.parameterInfos, callParameters, typeof(TResult));
            return (TResult)result;
        }
    }
}
