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
            // var baseUri = "http://localhost:45532/";

            // for SampleService.SelfhostedServer
            var baseUri = "http://localhost:8080/";

            var client = new SimpleServiceClient();
            var instance = client.GetService<ISampleService>(baseUri + "SampleService");

            var result = instance.SayMyName("Tobias");
            var result2 = instance.SayHelloWorld();
            var result3 = instance.Get();

            Exception result4 = null;

            try
            {
                instance.GetWithException();
            }
            catch (Exception ex)
            {
                result4 = ex;
            }

            Console.WriteLine(result);
            Console.WriteLine(result2);
            Console.WriteLine(result3);
            Console.WriteLine(result4);
            Console.ReadLine();
        }
    }
}
