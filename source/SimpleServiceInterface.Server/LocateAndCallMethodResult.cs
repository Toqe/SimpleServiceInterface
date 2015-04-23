using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceInterface.Server
{
    public class LocateAndCallMethodResult
    {
        public bool Found { get; set; }

        public object Result { get; set; }

        public bool ExceptionThrown { get; set; }

        public Exception Exception { get; set; }
    }
}
