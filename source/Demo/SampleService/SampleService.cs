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

        public IQueryable<SampleType> GetAll()
        {
            return new List<SampleType>() { new SampleType() { Id = 1 }, new SampleType() { Id = 2 }, new SampleType() { Id = 3 } }.AsQueryable();
        }

        public void GetWithException()
        {
            throw new ArgumentException("You supplied no arguments!");
        }
    }
}
