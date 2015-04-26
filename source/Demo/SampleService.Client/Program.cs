using SampleService.Contract;
using SimpleServiceInterface.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var result = instance.SayMyName("Tobias");
            var result2 = instance.SayHelloWorld();
            var result3 = instance.GetMany();
            var result4 = instance.Get();

            Exception result5 = null;

            try
            {
                instance.GetWithException();
            }
            catch (Exception ex)
            {
                result5 = ex;
            }

            var result6 = instance.GetAll()
                .Where(x => x.Id < 3)
                .Select(x => new Tuple<long, DateTime>(x.Id, x.Date2))
                .OrderByDescending(i => i.Item1)
                .ToList();

            Console.WriteLine(result);
            Console.WriteLine(result2);
            Console.WriteLine(string.Join(", ", result3));
            Console.WriteLine(result4);
            Console.WriteLine(result5);
            Console.WriteLine("StackTrace on server: " + result5.Data["__SimpleServiceInterface__OriginalStackTrace__"]);
            Console.WriteLine(string.Join(",", result6.Select(r => string.Format("{{ Id: '{0}', Date2: '{1}' }}", r.Item1, r.Item2))));
            Console.ReadLine();
        }
    }
}
