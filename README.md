# SimpleServiceInterface
is a very simple implementation for connecting services via HTTP based JSON encoded API calls

It supports service methods returning IQueryable<> as return type. The query on the IQueryable is serialized on the client, sent to the server and evaluated there. Only the result of the query is sent back to the client.

See source/Demo for an example of usage: Just start the SampleService.SelfhostedServer and connect to it with the SampleService.Client. Every method provided by the SampleService running on the server can be used by the client via JSON-encoded HTTP-calls.
