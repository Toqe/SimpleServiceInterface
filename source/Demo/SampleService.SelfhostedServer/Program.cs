﻿using Microsoft.Owin.Hosting;
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
            // Load DLL with service, the services are then automatically discovered by the ISimpleService interface
            var executingPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            var dllFullName = Path.Combine(executingPath, "SampleService.dll");
            Assembly.LoadFrom(dllFullName);

            using (WebApp.Start<Startup>("http://localhost:8080"))
            {
                Console.WriteLine("Server started. Press [ENTER] to exit.");
                Console.ReadLine();
            }
        }

        private class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                var handler = new SimpleServiceInterface.Server.Owin.SimpleServiceServerOwin();

                app.Run(context =>
                {
                    return handler.ProcessRequest(context);
                });
            }
        }
    }
}
