using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemo.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("GRPC Demo");

await CallGrpcServices();

async Task CallGrpcServices()
{
    using var channel = GrpcChannel.ForAddress("https://localhost:5001");

    var client = new Greeter.GreeterClient(channel);

    await CallUnary(client);
    await CallServerStreaming(client);
    await CallClientStreaming(client);
}

async Task CallUnary(Greeter.GreeterClient client)
{
    Console.WriteLine("Unary call");
    Console.ReadLine();

    var unaryResponse = await client.SayHelloAsync(new HelloRequest { Name = "Name #1" });
    Console.WriteLine($"UNARY: {unaryResponse.Message}");
    Console.WriteLine("-----------------------------------");
}

async Task CallServerStreaming(Greeter.GreeterClient client)
{
    Console.WriteLine("Server Streaming call");
    Console.ReadLine();

    var cts = new CancellationTokenSource();
    cts.CancelAfter(TimeSpan.FromSeconds(3.5));

    using var call = client.SayHellos(new HelloRequest { Name = "Name #1" }, cancellationToken: cts.Token);

    try
    {
        await foreach (var message in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"SERVER STREAMING: {message.Message}");
        }
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("SERVER STREAMING END!");
    }

    Console.WriteLine("-----------------------------------");
}

async Task CallClientStreaming(Greeter.GreeterClient client)
{
    Console.WriteLine("Client streaming call");
    Console.ReadLine();

    using var call = client.SayHelloMultipleTimes();

    await call.RequestStream.WriteAsync(new HelloRequest { Name = "Name #1" });
    await call.RequestStream.WriteAsync(new HelloRequest { Name = "Name #2" });

    await call.RequestStream.CompleteAsync();

    var response = await call;
    Console.WriteLine($"CLIENT STREAMING: {response.Message}");
    Console.WriteLine("-----------------------------------");
}
