using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Server.Owin
{
    public class SimpleServiceServerOwin : SimpleServiceServerBaseHandler
    {
        public virtual Task ProcessRequest(IOwinContext context)
        {
            var request = context.Request;
            var response = context.Response;

            var relativeUrl = request.Path.Value;

            if (relativeUrl.StartsWith("/"))
            {
                relativeUrl = relativeUrl.Substring(1);
            }

            var result = this.LocateAndCallMethod(relativeUrl, request.Method, paramName => request.Query[paramName], request.Body);

            if (!result.Found)
            {
                return this.ReturnNotFound(response);
            }

            response.ContentType = this.contentType;
            var resultValue = result.Result;

            if (!result.ExceptionThrown)
            {
                response.StatusCode = 200;
            }
            else
            {
                response.StatusCode = 500;
                resultValue = result.Exception;
            }
            
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, encoding))
                {
                    this.jsonSerializer.Serialize(streamWriter, resultValue);
                    streamWriter.Flush();
                }

                return response.WriteAsync(memoryStream.ToArray());
            }
        }

        protected virtual Task ReturnNotFound(IOwinResponse response, string statusDescription = null)
        {
            response.StatusCode = 404;
            response.ReasonPhrase = statusDescription;
            return response.WriteAsync(string.Empty);
        }

        protected virtual Task ReturnError(IOwinResponse response, string statusDescription = null)
        {
            response.StatusCode = 501;
            response.ReasonPhrase = statusDescription;
            return response.WriteAsync(string.Empty);
        }
    }
}
