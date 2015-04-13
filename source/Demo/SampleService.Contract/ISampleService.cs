using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Contract
{
    public interface ISampleService
    {
        string SayHelloWorld();

        void SayNothing();

        string SayMyName(string name);

        SampleType Get();
    }
}
