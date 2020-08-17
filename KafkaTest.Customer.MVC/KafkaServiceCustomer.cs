using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTest.Customer.MVC
{
    internal class KafkaServiceCustomer<IEventMessage> : IDisposable
    {
        private readonly ILogger<KafkaServiceCustomer<IEventMessage>> _logger;
        private IConfiguration Configuration;
        private readonly IConsumer<Ignore, string> customer;

        public KafkaServiceCustomer(ILogger<KafkaServiceCustomer<IEventMessage>> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;

            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = Configuration.GetSection("Kafka:Server").Value,
                    GroupId = "consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                this.customer = new ConsumerBuilder<Ignore, string>(config)
                                        .Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection Failed");
                throw new Exception("Connection Failed");
            }
        }

        public void ReceiveMessage(Action<IEventMessage> MessageProcess, CancellationToken stoppingToken = default)
        {
            try
            {
                this.customer.Subscribe(Configuration.GetSection("Kafka:Topic").Value);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var message = customer.Consume(stoppingToken);

                    MessageProcess(JsonConvert.DeserializeObject<IEventMessage>(message.Message.Value));
                }
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, "Receive failed");
                throw;
            }
        }

        public void Dispose()
        {
            if (customer != null)
            {
                customer.Close();
                customer.Dispose();
            }
        }
    }
}
