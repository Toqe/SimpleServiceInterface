using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Server
{
    public class SimpleServiceServerBaseHandler
    {
        protected readonly string contentType = "application/json";

        protected readonly Encoding encoding = Encoding.UTF8;

        protected readonly JsonSerializer jsonSerializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.Auto };

        protected readonly Func<string, Type> typeFinder;

        protected readonly Func<Type, object> instanceBuilder;

        public SimpleServiceServerBaseHandler(
            Func<string, Type> typeFinder,
            Func<Type, object> instanceBuilder)
        {
            this.typeFinder = typeFinder;
            this.instanceBuilder = instanceBuilder;
        }

        protected LocateAndCallMethodResult LocateAndCallMethod(string relativeUrl, string httpMethod, Func<string, string> getParameter, Stream inputStream)
        {
            var result = new LocateAndCallMethodResult();

            var route = relativeUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (route.Length < 2)
            {
                result.Found = false;
                return result;
            }

            var className = route[0];
            var methodName = route[1];

            var type = this.typeFinder(className);

            if (type == null)
            {
                result.Found = false;
                return result;
            }

            var instance = instanceBuilder(type);

            var method = type.GetMethod(methodName);

            if (method == null)
            {
                result.Found = false;
                return result;
            }

            var parameters = method.GetParameters();
            var parameterValues = new object[parameters.Length];

            if (parameters.Length > 0)
            {
                if (httpMethod == "GET")
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        var parameterStringValue = getParameter(parameter.Name);

                        if (parameterStringValue != null)
                        {
                            if (parameter.ParameterType == typeof(string))
                            {
                                parameterValues[i] = parameterStringValue;
                            }

                            // TODO: Implement auto-conversion for further types
                            // TODO: Provide interface for conversion and mapping of parameters?
                        }
                    }
                }
                else if (httpMethod == "POST")
                {
                    var parameterType = parameters[0].ParameterType;

                    using (var streamReader = new StreamReader(inputStream))
                    {
                        parameterValues[0] = this.jsonSerializer.Deserialize(streamReader, parameterType);
                    }
                }
            }

            var resultValue = method.Invoke(instance, parameterValues);
            result.Found = true;
            result.Result = resultValue;
            return result;
        }
    }
}
