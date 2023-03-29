using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Models.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;

        private readonly string _serviceBusConnectionString;
        private readonly string _orderPaymentProcessTopic;
        private readonly string _orderPaymentProcessSubscription;
        private readonly string _orderUpdatePaymentResultTopic;
        private readonly ServiceBusProcessor _orderPaymentServiceBusProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, IProcessPayment processPayment)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _processPayment = processPayment;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            _orderPaymentProcessSubscription = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
            _orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            ServiceBusClient serviceBusClient = new(_serviceBusConnectionString);

            _orderPaymentServiceBusProcessor = serviceBusClient.CreateProcessor(_orderPaymentProcessTopic, _orderPaymentProcessSubscription);
        }

        public async Task Start()
        {
            _orderPaymentServiceBusProcessor.ProcessMessageAsync += ProcessPayments;
            _orderPaymentServiceBusProcessor.ProcessErrorAsync += OnError;
            await _orderPaymentServiceBusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _orderPaymentServiceBusProcessor.StopProcessingAsync();
            await _orderPaymentServiceBusProcessor.DisposeAsync();
        }

        private Task OnError(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());

            return Task.CompletedTask;
        }

        private async Task ProcessPayments(ProcessMessageEventArgs processMessageEventArgs)
        {
            ServiceBusReceivedMessage serviceBusReceivedMessage = processMessageEventArgs.Message;

            string serviceBusReceivedMessageBody = Encoding.UTF8.GetString(serviceBusReceivedMessage.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(serviceBusReceivedMessageBody);

            bool result = _processPayment.PaymentProcessor();

            try
            {
                await _messageBus.Publish(
                    new UpdatePaymentResultMessage()
                    {
                        Status = result,
                        OrderId = paymentRequestMessage.OrderId,
                        Email = paymentRequestMessage.Email
                    },
                    _orderUpdatePaymentResultTopic
                );

                await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}