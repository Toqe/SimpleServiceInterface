# SimpleServiceInterface
is a very simple implementation for connecting services via HTTP based JSON encoded API calls

## IQueryable-Support ##
It supports service methods returning `IQueryable<>` as return type. The query on the `IQueryable` is serialized on the client, sent to the server and evaluated there. Only the result of the query is sent back to the client.

## Demo ##
See source/Demo for an example of usage: Just start the `SampleService.SelfhostedServer` and connect to it with the `SampleService.Client`. Every method provided by the SampleService running on the server can be used by the client via JSON-encoded HTTP-calls.

## What problem does this solve? ##
Let's say you have the following interface:
```C#
// your service interface
public interface ISampleService
{
    IQueryable<SampleClass> GetAll();
}

// your data class which contains some sample data fields
public class SampleClass
{
    public long Id { get; set; }

    public Guid Id2 { get; set; }

    private string Text { get; set; }

    public DateTime Date { get; set; }
 }
```

You want to provide this interface as a service via HTTP, for example for your microservices. Microservice B consumes data from Microservice A, which is the data store for SampleClass. But the way Microservice A needs data depends on the actual case. Sometimes it needs to filter only by `Id`, sometimes it also needs to use the `Text` property or the `Date` property also. Other times, you only need a field or two and not all the data for each entry. Or even more, Microservice B sometimes needs a grouped or aggregated (for example min/max) result from Microservice A.

The most naive way would be to just send all the data as a list to the client and the client then can do all the operations it needs to. But that isn't efficient and leads to performance issues, both on serialization/deserialization, CPU time and memory consumption on filtering on the client and also network bandwith.

In a usual webservice, you would define different methods for different purposes or sophisticated query and result objects. But that can lead to regular additions or changes to the interface, which can (at least temporarily) break your application.

Another option is (OData)[http://www.odata.org/], but it comes with the handicap of not being fully LINQ compatible, so you may have to rewrite your queries. (To be fair: If you want to exchange data between different platforms and programming languages, OData is still the best choice for you.)

But in the end, all you want to do is write C# code in your Microservice B like this:
```C#
sampleService.GetAll()
    .Where(x => x.Id < 3)
    .Select(x => new Tuple<long, DateTime>(x.Id, x.Date2))
    .OrderByDescending(i => i.Item1)
    .ToList();
```
And you don't want to worry about CPU, RAM or network usage and you don't want to change the interface with every use case. You just want to query your data as it were local. That also makes testing a lot easier. 

**And this is exactly what `SimpleServiceInterface` can do for you.**

Just 
* create a common interface for your service
* write an implementation for the service
* host the service implementation either as self-hosted service (`SimpleServiceServerOwin`) or in IIS (`SimpleServiceServerSystemWebHttpHandler`)
* use the `SimpleServiceClient` on the client side to use the service
