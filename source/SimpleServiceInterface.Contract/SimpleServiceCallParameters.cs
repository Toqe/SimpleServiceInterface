using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Contract
{
    public class SimpleServiceCallParameters
    {
        public object[] Parameters { get; set; }

        public Type[] GenericParameters { get; set; }

        public Expression QueryExpression { get; set; }
    }
}
