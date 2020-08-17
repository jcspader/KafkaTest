using System;

namespace KafkaTest
{
    public class EventMessage : IEventMessage
    {
        public Guid RequestId { get; set; }

        public string InstanceId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Created at {CreatedAt.ToString("HH:mm:ss.fff")} - Instance #{InstanceId} - RequestId '{RequestId.ToString().Substring(0, 12)}' - Message: {Message}";
        }
    }
}
