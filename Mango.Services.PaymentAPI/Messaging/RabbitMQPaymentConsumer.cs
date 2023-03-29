using Mango.Services.PaymentAPI.Models.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IProcessPayment _processPayment;
        private readonly IRabbitMQPaymentMessageSender _rabbitMQPaymentMessageSender;

        private readonly IModel _channel;

        public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQPaymentMessageSender rabbitMQPaymentMessageSender)
        {
            _processPayment = processPayment;
            _rabbitMQPaymentMessageSender = rabbitMQPaymentMessageSender;

            ConnectionFactory connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            IConnection connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EventingBasicConsumer eventingBasicConsumer = new(_channel);

            eventingBasicConsumer.Received += (ch, ea) =>
            {
                string content = Encoding.UTF8.GetString(ea.Body.ToArray());

                PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);

                HandleMessage(paymentRequestMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("orderpaymentprocesstopic", false, eventingBasicConsumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {
            bool result = _processPayment.PaymentProcessor();

            _rabbitMQPaymentMessageSender.SendMessage(
                new UpdatePaymentResultMessage()
                {
                    Status = result,
                    OrderId = paymentRequestMessage.OrderId,
                    Email = paymentRequestMessage.Email
                }
            );
        }
    }
}