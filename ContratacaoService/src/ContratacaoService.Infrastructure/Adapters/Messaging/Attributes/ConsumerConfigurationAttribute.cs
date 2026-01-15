using ContratacaoService.Infrastructure.Adapters.Messaging.Interfaces;

namespace ContratacaoService.Infrastructure.Adapters.Messaging.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConsumerConfigurationAttribute : Attribute, IConsumerMetadata
    {
        public string QueueName { get; }

        public ConsumerConfigurationAttribute(string queueName)
        {
            QueueName = queueName;
        }
    }
}
