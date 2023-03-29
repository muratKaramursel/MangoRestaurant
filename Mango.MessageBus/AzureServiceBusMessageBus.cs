using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly string _connectionString = "Endpoint=sb://mangorestaurantmkm.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=7rRblCDqZJ0o+27Dz3dbKSDAvqf747lgk+ASbM5r63Y=";

        public async Task Publish(BaseMessage baseMessage, string topicName)
        {
            string jsonMessage = JsonConvert.SerializeObject(baseMessage);

            ServiceBusMessage serviceBusMessage = new(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await using ServiceBusClient serviceBusClient = new(_connectionString);
            await using ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(topicName);
            await serviceBusSender.SendMessageAsync(serviceBusMessage);
        }
    }
}