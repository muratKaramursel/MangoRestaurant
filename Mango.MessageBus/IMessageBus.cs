namespace Mango.MessageBus
{
    public interface IMessageBus
    {
        Task Publish(BaseMessage baseMessage, string topicName);
    }
}