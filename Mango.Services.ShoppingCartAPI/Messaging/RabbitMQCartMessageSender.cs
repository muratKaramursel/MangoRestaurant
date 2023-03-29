using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.ShoppingCartAPI.Messaging
{
    public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;

        public RabbitMQCartMessageSender()
        {
            _connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public void SendMessage(BaseMessage baseMessage, string queueName)
        {
            _connection ??= _connectionFactory.CreateConnection();

            using IModel channel = _connection.CreateModel();

            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

            string jsonMessage = JsonConvert.SerializeObject(baseMessage);

            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}