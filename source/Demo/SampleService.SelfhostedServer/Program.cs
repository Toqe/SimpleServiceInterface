using Microsoft.Owin.Hosting;
using Owin;
using SimpleServiceInterface.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.SelfhostedServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load DLL with service
            var executingPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            var dllFullName = Path.Combine(executingPath, "SampleService.dll");
            Assembly.LoadFrom(dllFullName);

            using (WebApp.Start<Startup>("http://localhost:8080"))
            {
                Console.ReadLine();
            }
        }

        private class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                var handler = new SimpleServiceInterface.Server.Owin.SimpleServiceServerOwin(DefaultImplementations.TypeFinder, DefaultImplementations.InstanceBuilder);

                app.Run(context =>
                {
                    return handler.ProcessRequest(context);
                });
            }
        }
    }
}
