using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Client
{
    public delegate WebRequest WebRequestBuilder(string url, ParameterInfo[] parameterInfos, SimpleServiceCallParameters parameters, Type returnType);
}
