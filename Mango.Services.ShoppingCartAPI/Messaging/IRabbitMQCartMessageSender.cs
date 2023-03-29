using Mango.MessageBus;

namespace Mango.Services.ShoppingCartAPI.Messaging
{
    public interface IRabbitMQCartMessageSender
    {
        void SendMessage(BaseMessage baseMessage, string queueName);
    }
}