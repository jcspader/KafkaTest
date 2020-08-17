using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace KafkaTest.Customer.MVC
{
    internal class KafkaHub : Hub
    {
        private KafkaServiceCustomer<EventMessage> _service;

        public KafkaHub(KafkaServiceCustomer<EventMessage> kafkaServiceCustomer)
        {
            _service = kafkaServiceCustomer;
        }

        public override Task OnConnectedAsync()
        {
            _service.ReceiveMessage(Broadcast);

            return base.OnConnectedAsync();
        }

        internal void Broadcast<IEventMessage>(IEventMessage eventMessage)
        {
            Clients.All.SendAsync("kafkaService", eventMessage);
        }
    }
}