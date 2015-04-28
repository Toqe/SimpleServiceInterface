using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Client
{
    public class SimpleServiceClient
    {
        private readonly ProxyGenerator generator = new ProxyGenerator();

        private readonly JsonSerializer defaultJsonSerializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.All };

        public SimpleServiceClient()
        {
            this.DefaultWebRequestBuilder = new DefaultWebRequestBuilder().Build;
        }

        public WebRequestBuilder DefaultWebRequestBuilder { get; set; }

        public T GetService<T>(string url) where T : class
        {
            return this.GetService<T>(url, null);
        }

        public T GetService<T>(string url, WebRequestBuilder webRequestBuilder) where T : class
        {
            var methodCallInterceptor = new SimpleServiceMethodCallInterceptor(url);
            methodCallInterceptor.JsonSerializer = this.defaultJsonSerializer;
            methodCallInterceptor.WebRequestBuilder = webRequestBuilder != null ? webRequestBuilder : this.DefaultWebRequestBuilder;
            return this.generator.CreateInterfaceProxyWithoutTarget<T>(methodCallInterceptor);
        }
    }
}
