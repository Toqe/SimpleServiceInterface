using SimpleServiceInterface.Contract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Client
{
    internal class SimpleQueryableContext<T> : IOrderedQueryable<T>
    {
        public SimpleQueryableContext(
            SimpleServiceMethodCallInterceptor callInterceptor,
            string url,
            ParameterInfo[] parameterInfos,
            SimpleServiceCallParameters parameters)
        {
            this.Provider = new SimpleQueryableProvider<T>(callInterceptor, url, parameterInfos, parameters);
            this.Expression = Expression.Constant(this);
        }

        internal SimpleQueryableContext(IQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }
    }
}
