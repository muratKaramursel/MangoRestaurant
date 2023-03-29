using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.Messaging
{
    public interface IRabbitMQPaymentMessageSender
    {
        void SendMessage(BaseMessage baseMessage);
    }
}