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

        IEnumerable<string> GetMany();

        SampleType Get();
    }
}
