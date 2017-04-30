using SampleService.Contract;
using SimpleServiceInterface.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public IQueryable<SampleType> GetAll()
        {
            return new List<SampleType>()
            {
                new SampleType() { Id = 1, Date = new DateTime(2017, 4, 30) },
                new SampleType() { Id = 2, Date = new DateTime(2017, 4, 30) },
                new SampleType() { Id = 3, Date = new DateTime(2017, 5, 1) },
            }.AsQueryable();
        }

        public void GetWithException()
        {
            throw new ArgumentException("This is a test exception");
        }

        public TResult Do<TResult>(Expression<Func<ISampleService, TResult>> expression)
        {
            return expression.Compile()(this);
        }
    }
}