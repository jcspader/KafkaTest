using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTest.Producer.HelloWorld
{
    public class WorkerKafka : BackgroundService
    {
        private readonly ILogger<WorkerKafka> _logger;
        private readonly KafkaServiceProducer<EventMessage> _service;

        private readonly string InstanceId = Guid.NewGuid().ToString().Substring(0, 8);

        public WorkerKafka(ILogger<WorkerKafka> logger, KafkaServiceProducer<EventMessage> service)
        {
            _logger = logger;
            _service = service;

            _logger.LogWarning($"InstanceId: #{InstanceId}");
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Starting Worker...");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Stopping Worker...");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new EventMessage()
                {
                    CreatedAt = DateTimeOffset.Now,
                    InstanceId = InstanceId,

                    RequestId = Guid.NewGuid(),
                    Message = "Hello World"
                };

                if (await _service.SendMessage(message, stoppingToken))
                {
                    _logger.LogInformation($"Message Sent: {message}");
                }

                await Task.Delay(1000 * 5, stoppingToken);
            }
        }

    }
}
