using Azure.Messaging.ServiceBus;
using EmailProcessor;
using Mango.Services.EmailAPI.Models.Entities;
using Mango.Services.EmailAPI.Models.Messages;
using Mango.Services.EmailAPI.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailManager _emailManager;

        private readonly string _serviceBusConnectionString;
        private readonly string _orderUpdatePaymentResultTopic;
        private readonly string _orderUpdatePaymentResultSubscription;
        private readonly ServiceBusProcessor _orderUpdatePaymentResultServiceBusProcessor;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration configuration, IEmailManager emailManager)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _emailManager = emailManager;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");
            _orderUpdatePaymentResultSubscription = _configuration.GetValue<string>("OrderUpdatePaymentResultSubscription");

            ServiceBusClient serviceBusClient = new(_serviceBusConnectionString);

            _orderUpdatePaymentResultServiceBusProcessor = serviceBusClient.CreateProcessor(_orderUpdatePaymentResultTopic, _orderUpdatePaymentResultSubscription);
        }

        public async Task Start()
        {
            _orderUpdatePaymentResultServiceBusProcessor.ProcessErrorAsync += OnError;
            _orderUpdatePaymentResultServiceBusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;

            await _orderUpdatePaymentResultServiceBusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _orderUpdatePaymentResultServiceBusProcessor.StopProcessingAsync();
            await _orderUpdatePaymentResultServiceBusProcessor.DisposeAsync();
        }

        private Task OnError(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());

            return Task.CompletedTask;
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs processMessageEventArgs)
        {
            ServiceBusReceivedMessage serviceBusReceivedMessage = processMessageEventArgs.Message;

            string serviceBusReceivedMessageBody = Encoding.UTF8.GetString(serviceBusReceivedMessage.Body);

            UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(serviceBusReceivedMessageBody);

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

            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        }
    }
}