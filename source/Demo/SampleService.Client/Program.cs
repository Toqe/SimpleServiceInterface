using SampleService.Contract;
using SimpleServiceInterface.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        void Run()
        {
            // for SampleService.WebhostedServer
            // var baseUri = "http://localhost:49056/";

            // for SampleService.SelfhostedServer
            var baseUri = "http://localhost:8080/";

            var client = new SimpleServiceClient();
            var instance = client.GetService<ISampleService>(baseUri + "SampleService");

            var result1 = instance.SayMyName("Tobias");
            Console.WriteLine(result1);

            var result2 = instance.SayHelloWorld();
            Console.WriteLine(result2);

            var result3 = instance.GetMany();
            Console.WriteLine(string.Join(", ", result3));

            var result4 = instance.Get();
            Console.WriteLine(result4);

            Exception result5 = null;

            try
            {
                instance.GetWithException();
            }
            catch (Exception ex)
            {
                result5 = ex;
            }

            Console.WriteLine(result5);
            Console.WriteLine(string.Format("StackTrace on server: {0}", result5.Data["__SimpleServiceInterface__OriginalStackTrace__"]));

            var result6 = instance.GetAll()
                .Where(x => x.Id < 3)
                .Select(x => new Tuple<long, DateTime>(x.Id, x.Date2))
                .OrderByDescending(i => i.Item1)
                .ToList();
            Console.WriteLine(string.Join(",", result6.Select(r => string.Format("{{ Id: '{0}', Date2: '{1}' }}", r.Item1, r.Item2))));

            var result7 = instance.GetAll()
                .Max(x => x.Id);
            Console.WriteLine("Max id: " + result7);

            var result8 = instance.GetAll()
                .GroupBy(x => x.Date)
                .Select(x => new Tuple<DateTime, int>(x.Key, x.Count()))
                .ToList();
            Console.WriteLine(string.Join(",", result8.Select(r => "Group key " + r.Item1 + ", value count " + r.Item2)));

            var name = "TestXyz!";
            Expression<Func<ISampleService, Tuple<string, SampleType>>> expressionForResult9 = 
                (ISampleService sampleService) => new Tuple<string, SampleType>(sampleService.SayMyName(name), sampleService.Get());
            var result9 = instance.Do(expressionForResult9);
            Console.WriteLine(string.Format("{0} - {1}", result9.Item1, result9.Item2));

            Console.WriteLine();
            Console.WriteLine("All tests finished. Press [ENTER] to exit.");
            Console.ReadLine();
        }
    }
}
