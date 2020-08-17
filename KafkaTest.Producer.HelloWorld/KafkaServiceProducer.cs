using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTest.Producer.HelloWorld
{
    public class KafkaServiceProducer<EventMessage>
    {
        private readonly ILogger<WorkerKafka> _logger;
        private IConfiguration Configuration;
        private readonly IProducer<Null, string> producer;

        public KafkaServiceProducer(ILogger<WorkerKafka> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;

            try
            {
                var config = new ProducerConfig { BootstrapServers = Configuration.GetSection("Kafka:Server").Value };

                this.producer = new ProducerBuilder<Null, string>(config)
                                        .Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection Failed");
                throw new Exception("Connection Failed");
            }
        }

        public async Task<bool> SendMessage(IEventMessage message, CancellationToken stoppingToken = default)
        {
            try
            {
                await producer
                        .ProduceAsync(Configuration.GetSection("Kafka:Topic").Value,
                                        new Message<Null, string> { Value = JsonConvert.SerializeObject(message) });
                return true;
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, "Delivery failed");
                return false;
            }
        }

        public void Dispose()
        {
            if (producer != null)
                producer.Dispose();
        }
    }
}
