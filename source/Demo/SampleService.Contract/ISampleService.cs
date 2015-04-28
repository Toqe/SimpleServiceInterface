using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        IQueryable<SampleType> GetAll();

        void GetWithException();

        TResult Do<TResult>(Expression<Func<ISampleService, TResult>> expression);
    }
}
