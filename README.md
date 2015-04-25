# SimpleServiceInterface
is a very simple implementation for connecting services via HTTP based JSON encoded API calls

It supports service methods returning IQueryable<> as return type. The query on the IQueryable is serialized on the client, sent to the server and evaluated there. Only the result of the query is sent back to the client.
