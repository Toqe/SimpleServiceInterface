using Newtonsoft.Json;
using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using Remote.Linq;

namespace SimpleServiceInterface.Server
{
    public class SimpleServiceServerBaseHandler
    {
        protected readonly Type iQueryableType = typeof(IQueryable);

        protected readonly string contentType = "application/json";

        protected readonly Encoding encoding = Encoding.UTF8;

        protected readonly JsonSerializer jsonSerializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.All };

        public SimpleServiceServerBaseHandler()
        {
            this.UrlModifier = DefaultImplementations.UrlModifier;
            this.TypeFinder = DefaultImplementations.TypeFinder;
            this.InstanceBuilder = DefaultImplementations.InstanceBuilder;
        }

        public Func<string, string> UrlModifier { get; set; }

        public Func<string, Type> TypeFinder { get; set; }

        public Func<Type, object> InstanceBuilder { get; set; }

        protected LocateAndCallMethodResult LocateAndCallMethod(
            string relativeUrl, 
            string httpMethod, 
            Func<string, string> getParameter, 
            Stream inputStream)
        {
            var result = new LocateAndCallMethodResult();

            if (this.UrlModifier != null)
            {
                relativeUrl = this.UrlModifier(relativeUrl);
            }

            var route = relativeUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (route.Length < 2)
            {
                result.Found = false;
                return result;
            }

            var className = route[0];
            var methodName = route[1];

            var type = this.TypeFinder(className);

            if (type == null)
            {
                result.Found = false;
                return result;
            }

            var instance = this.InstanceBuilder(type);

            var method = type.GetMethod(methodName);

            if (method == null)
            {
                result.Found = false;
                return result;
            }

            var parameters = method.GetParameters();
            var parameterValues = new object[parameters.Length];
            var returnType = method.ReturnType;
            bool isReturnTypeIQueryable = iQueryableType.IsAssignableFrom(returnType);
            Expression linqExpression = null;

            if (parameters.Length > 0 || isReturnTypeIQueryable)
            {
                // TODO: Restrict GET-calls to debug mode or something like that.
                // Usually the services provide methods, which change data, so we should make sure they are called only with POST.
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
                            else if (parameter.ParameterType == typeof(Guid) || parameter.ParameterType == typeof(Guid?))
                            {
                                Guid parsedParameter;

                                if (Guid.TryParse(parameterStringValue, out parsedParameter))
                                {
                                    parameterValues[i] = parsedParameter;
                                }
                            }

                            // TODO: Implement auto-conversion for further types
                            // TODO: Provide interface for conversion and mapping of parameters?
                        }
                    }
                }
                else if (httpMethod == "POST")
                {
                    using (var stream = new MemoryStream())
                    {
                        // copy stream, so we can run through it multiple times
                        byte[] buffer = new byte[1024*1024];
                        int read;

                        while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, read);
                        }

                        try
                        {
                            SimpleServiceCallParameters receivedCallParameters = null;
                            stream.Position = 0;

                            using (var streamReader = new StreamReader(stream))
                            {
                                receivedCallParameters = this.jsonSerializer.Deserialize(streamReader, typeof(SimpleServiceCallParameters)) as SimpleServiceCallParameters;
                            }

                            for (var i = 0; i < Math.Min((receivedCallParameters.Parameters ?? new object[0]).Length, parameters.Length); i++)
                            {
                                parameterValues[i] = receivedCallParameters.Parameters[i];
                            }

                            if (isReturnTypeIQueryable && receivedCallParameters.QueryExpression != null)
                            {
                                linqExpression = receivedCallParameters.QueryExpression.ToLinqExpression();
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                // Received object does not seem to be a SimpleServiceCallParametersObject -> try to parse as parameter object
                                object receivedObject = null;
                                stream.Position = 0;

                                using (var streamReader = new StreamReader(stream))
                                {
                                    receivedObject = this.jsonSerializer.Deserialize(streamReader, parameters[0].ParameterType);
                                    parameterValues[0] = receivedObject;
                                }
                            }
                            catch (Exception ex2)
                            {
                                // doesn't matter
                            }
                        }
                    }
                }
            }

            result.Found = true;

            try
            {
                var resultValue = method.Invoke(instance, parameterValues);

                if (isReturnTypeIQueryable && linqExpression != null)
                {
                    var resultValueType = resultValue.GetType();                  
                    var stubRemover = new SimpleQueryableStubRemover(resultValue, resultValueType);
                    var modifiedExpression = stubRemover.CopyAndModify(linqExpression);

                    var queryProvider = ((IQueryable)resultValue).Provider;
                    resultValue = queryProvider.CreateQuery(modifiedExpression);
                }

                result.Result = resultValue;
            }
            catch (TargetInvocationException ex)
            {
                result.ExceptionThrown = true;
                result.Exception = ex.InnerException;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
    }
}
