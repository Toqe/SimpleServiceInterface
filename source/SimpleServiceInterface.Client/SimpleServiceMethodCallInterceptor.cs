using Castle.DynamicProxy;
using Newtonsoft.Json;
using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Client
{
    /// <summary>
    /// This class implements the IInterceptor interface from Castle DynamicProxy. The proxy is just a facade. If any method on the proxy object is called,
    /// this interceptor takes over and can dynamically build the result for the method call. It is done by calling the service with the serialized 
    /// parameters and returning the deserialized result as result to the method call.
    /// </summary>
    internal class SimpleServiceMethodCallInterceptor : IInterceptor
    {
        private readonly Type iQueryableGenericType = typeof(IQueryable<>);

        private readonly Type simpleQueryableContextGenericType = typeof(SimpleQueryableContext<>);

        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        private readonly string baseUrl;

        public SimpleServiceMethodCallInterceptor(string baseUrl, JsonSerializer jsonSerializer)
        {
            this.baseUrl = baseUrl;
            this.jsonSerializer = jsonSerializer;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var url = this.baseUrl + (this.baseUrl.EndsWith("/") ? string.Empty : "/") + method.Name;
            var parameterInfos = method.GetParameters();
            var parameters = new SimpleServiceCallParameters() { Parameters = invocation.Arguments };
            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == iQueryableGenericType)
            {
                var simpleQueryableContextType = simpleQueryableContextGenericType.MakeGenericType(returnType.GenericTypeArguments[0]);
                var constructor = simpleQueryableContextType.GetConstructors().FirstOrDefault();
                var instance = constructor.Invoke(new object[] { this, url, parameterInfos, parameters });
                invocation.ReturnValue = instance;
            }
            else
            {
                invocation.ReturnValue = this.Call(url, parameterInfos, parameters, returnType);
            }
        }

        public object Call(string url, ParameterInfo[] parameterInfos, SimpleServiceCallParameters parameters, Type returnType)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                this.jsonSerializer.Serialize(streamWriter, parameters);
                streamWriter.Flush();
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                if (returnType != typeof(void))
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = this.jsonSerializer.Deserialize(streamReader, returnType);
                        return result;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ex.Response is HttpWebResponse)
                {
                    var response = ex.Response as HttpWebResponse;

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        Exception deserializedException = null;

                        try
                        {
                            using (var streamReader = new StreamReader(response.GetResponseStream()))
                            {
                                deserializedException = this.jsonSerializer.Deserialize(streamReader, typeof(Exception)) as Exception;
                            }
                        }
                        catch (Exception)
                        {
                            // do nothing - throw exception catched in first place
                        }

                        if (deserializedException != null)
                        {
                            deserializedException.Data.Add("__SimpleServiceInterface__OriginalStackTrace__", deserializedException.StackTrace);
                            throw deserializedException;
                        }
                    }
                }

                throw;
            }

            return null;
        }
    }
}
