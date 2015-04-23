using SampleService.Contract;
using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleService
{
    public class SampleService : ISampleService, ISimpleService
    {
        public string SayHelloWorld()
        {
            return "hello world";
        }

        public void SayNothing()
        {
        }

        public string SayMyName(string name)
        {
            return string.Format("hello {0}", name);
        }

        public IEnumerable<string> GetMany()
        {
            return new List<string>() { "a", "b", "c", "d" };
        }

        public SampleType Get()
        {
            return new SampleType()
            {
                Date = DateTime.Now,
                Date2 = DateTime.UtcNow,
                Id = long.MaxValue,
                Id2 = Guid.NewGuid()
            };
        }
    }
}
