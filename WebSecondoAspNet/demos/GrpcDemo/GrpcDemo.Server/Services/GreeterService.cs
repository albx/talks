using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcDemo.Server
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var i = 0;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var message = $"Hi {request.Name}! {++i}";
                _logger.LogInformation($"Sending message {message}");

                await responseStream.WriteAsync(new HelloReply { Message = message });

                await Task.Delay(1000);
            }
        }

        public override async Task<HelloReply> SayHelloMultipleTimes(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            List<string> names = new();
            await foreach (var message in requestStream.ReadAllAsync())
            {
                names.Add(message.Name);
            }

            return new HelloReply { Message = $"Hi everybody! {string.Join(",", names)}" };
        }
    }
}
