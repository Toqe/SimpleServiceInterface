using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Server
{
    public class SimpleServiceServerSystemWebHttpHandlerWithDefaultImplementations : SimpleServiceServerSystemWebHttpHandler
    {
        public SimpleServiceServerSystemWebHttpHandlerWithDefaultImplementations()
            : base(DefaultImplementations.TypeFinder, DefaultImplementations.InstanceBuilder)
        {
        }
    }
}
