using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace SimpleServiceInterface.Server
{
    public class SimpleServiceServerSystemWebHttpHandler : SimpleServiceServerBaseHandler, IHttpHandler, IRequiresSessionState
    {
        public SimpleServiceServerSystemWebHttpHandler(
            Func<string, Type> typeFinder,
            Func<Type, object> instanceBuilder)
            : base(typeFinder, instanceBuilder)
        {
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            var relativeUrl = request.AppRelativeCurrentExecutionFilePath;

            if (relativeUrl.StartsWith("~/"))
            {
                relativeUrl = relativeUrl.Substring(2);
            }

            var result = this.LocateAndCallMethod(relativeUrl, request.HttpMethod, paramName => request.Params[paramName], request.InputStream);

            if (!result.Found)
            {
                this.ReturnNotFound(response);
                return;
            }

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

            response.ContentEncoding = this.encoding;
            response.ContentType = this.contentType;
            var outputStream = response.OutputStream;

            using (var streamWriter = new StreamWriter(outputStream, encoding))
            {
                this.jsonSerializer.Serialize(streamWriter, resultValue);
                streamWriter.Flush();
            }
        }

        private void ReturnNotFound(HttpResponse response, string statusDescription = null)
        {
            response.StatusCode = 404;
            response.StatusDescription = statusDescription;
        }

        private void ReturnError(HttpResponse response, string statusDescription = null)
        {
            response.StatusCode = 501;
            response.StatusDescription = statusDescription;
        }
    }
}
