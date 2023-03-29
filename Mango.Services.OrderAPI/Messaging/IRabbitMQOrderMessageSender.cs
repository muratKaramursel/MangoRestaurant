using Mango.MessageBus;

namespace Mango.Services.OrderAPI.Messaging
{
    public interface IRabbitMQOrderMessageSender
    {
        void SendMessage(BaseMessage baseMessage, string queueName);
    }
}