using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;

        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

        public RabbitMQPaymentMessageSender()
        {
            _connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public void SendMessage(BaseMessage baseMessage)
        {
            _connection ??= _connectionFactory.CreateConnection();

            using IModel channel = _connection.CreateModel();

            string jsonMessage = JsonConvert.SerializeObject(baseMessage);

            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            //ExchangeType.Direct
            //channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);

            //channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
            //channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");

            //channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            //channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");

            //channel.BasicPublish(exchange: ExchangeName, routingKey: "PaymentOrder", basicProperties: null, body: body);
            //channel.BasicPublish(exchange: ExchangeName, routingKey: "PaymentEmail", basicProperties: null, body: body);

            //ExchangeType.Fanout
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);
            channel.BasicPublish(exchange: ExchangeName, "", basicProperties: null, body: body);
        }
    }
}