using SimpleServiceInterface.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleService.WebhostedServer
{
    public class SampleHandler : SimpleServiceServerSystemWebHttpHandler
    {
        public SampleHandler()
            : base(DefaultImplementations.TypeFinder, DefaultImplementations.InstanceBuilder)
        {
        }
    }
}