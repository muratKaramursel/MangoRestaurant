using EmailProcessor;
using Mango.Services.EmailAPI.Models.Entities;
using Mango.Services.EmailAPI.Models.Messages;
using Mango.Services.EmailAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly EmailRepository _emailRepository;
        private readonly IEmailManager _emailManager;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

        //ExchangeType.Fanout
        private string _queuName  = string.Empty;

        public RabbitMQPaymentConsumer(EmailRepository emailRepository, IEmailManager emailManager)
        {
            _emailRepository = emailRepository;
            _emailManager = emailManager;

            ConnectionFactory connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            //ExchangeType.Direct
            //_channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            //_channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            //_channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");

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
            //_channel.BasicConsume(PaymentEmailUpdateQueueName, false, eventingBasicConsumer);

            //ExchangeType.Fanout
            _channel.BasicConsume(_queuName, false, eventingBasicConsumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            string title = $"{DateTime.Now:dd/MM/yyyy} Order Information";
            string body = $"Purchase Status: {updatePaymentResultMessage.Status}";

            bool emailSendResult = _emailManager.SendEmail(title, body, new List<string>() { updatePaymentResultMessage.Email }, null, null);

            await _emailRepository.LogEmail(
                new EmailLog()
                {
                    EmailTitle = title,
                    EmailBody = body,
                    EmailAddress = updatePaymentResultMessage?.Email,
                    EmailSent = DateTime.Now,
                    Sended = emailSendResult
                }
            );
        }
    }
}