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

        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public SimpleServiceClient()
        {
        }

        public T GetService<T>(string url) where T : class
        {
            return this.generator.CreateInterfaceProxyWithoutTarget<T>(new SimpleServiceMethodCallInterceptor(url, this.jsonSerializer));
        }
    }
}
