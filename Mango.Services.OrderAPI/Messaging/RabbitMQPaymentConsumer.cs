using Mango.Services.OrderAPI.Models.Messages;
using Mango.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;

        private readonly IModel _channel;

        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        //ExchangeType.Fanout
        private string _queuName = string.Empty;

        public RabbitMQPaymentConsumer(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

            ConnectionFactory connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            IConnection connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();

            //ExchangeType.Direct
            //_channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            //_channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
            //_channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");

            //ExchangeType.Fanout
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            _queuName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queuName, ExchangeName, "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer eventingBasicConsumer = new(_channel);

            eventingBasicConsumer.Received += (ch, ea) =>
            {
                string content = Encoding.UTF8.GetString(ea.Body.ToArray());

                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);

                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            //ExchangeType.Direct
            //_channel.BasicConsume(PaymentOrderUpdateQueueName, false, eventingBasicConsumer);

            //ExchangeType.Fanout
            _channel.BasicConsume(_queuName, false, eventingBasicConsumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
        }
    }
}