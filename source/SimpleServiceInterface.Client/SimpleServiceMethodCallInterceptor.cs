using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";

            if (method.GetParameters().Length > 0)
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    this.jsonSerializer.Serialize(streamWriter, invocation.Arguments[0]);
                    streamWriter.Flush();
                }
            }
            else
            {
                request.ContentLength = 0;
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                if (method.ReturnType != typeof(void))
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = this.jsonSerializer.Deserialize(streamReader, method.ReturnType);
                        invocation.ReturnValue = result;
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
                            // do nothing - throw first exception
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
        }
    }
}
