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
    internal class DefaultWebRequestBuilder
    {
        public WebRequest Build(string url, ParameterInfo[] parameterInfos, SimpleServiceCallParameters parameters, Type returnType)
        {
            var result = HttpWebRequest.Create(url);
            result.Method = "POST";
            return result;
        }
    }
}
