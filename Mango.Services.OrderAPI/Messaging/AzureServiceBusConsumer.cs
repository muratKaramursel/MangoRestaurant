using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Models.Entities;
using Mango.Services.OrderAPI.Models.Messages;
using Mango.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        private readonly string _serviceBusConnectionString;
        private readonly string _checkoutMessageTopic;
        private readonly string _checkoutMessageSubscription;
        private readonly string _checkoutMessageQueue;
        private readonly string _orderPaymentProcessTopic;
        private readonly string _orderUpdatePaymentResultTopic;
        private readonly string _orderUpdatePaymentResultSubscription;
        private readonly ServiceBusProcessor _checkoutMessageServiceBusProcessor;
        private readonly ServiceBusProcessor _orderUpdatePaymentResultServiceBusProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            _checkoutMessageSubscription = _configuration.GetValue<string>("CheckoutMessageSubscription");
            _checkoutMessageQueue = _configuration.GetValue<string>("CheckoutMessageQueue");
            _orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            _orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");
            _orderUpdatePaymentResultSubscription = _configuration.GetValue<string>("OrderUpdatePaymentResultSubscription");

            ServiceBusClient serviceBusClient = new(_serviceBusConnectionString);

            //_checkoutMessageServiceBusProcessor = serviceBusClient.CreateProcessor(_checkoutMessageTopic, _checkoutMessageSubscription);
            _checkoutMessageServiceBusProcessor = serviceBusClient.CreateProcessor(_checkoutMessageQueue);
            _orderUpdatePaymentResultServiceBusProcessor = serviceBusClient.CreateProcessor(_orderUpdatePaymentResultTopic, _orderUpdatePaymentResultSubscription);
        }

        public async Task Start()
        {
            _checkoutMessageServiceBusProcessor.ProcessErrorAsync += OnError;
            _checkoutMessageServiceBusProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            await _checkoutMessageServiceBusProcessor.StartProcessingAsync();

            _orderUpdatePaymentResultServiceBusProcessor.ProcessErrorAsync += OnError;
            _orderUpdatePaymentResultServiceBusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            await _orderUpdatePaymentResultServiceBusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _checkoutMessageServiceBusProcessor.StopProcessingAsync();
            await _checkoutMessageServiceBusProcessor.DisposeAsync();

            await _orderUpdatePaymentResultServiceBusProcessor.StopProcessingAsync();
            await _orderUpdatePaymentResultServiceBusProcessor.DisposeAsync();
        }

        private Task OnError(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());

            return Task.CompletedTask;
        }

        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs processMessageEventArgs)
        {
            ServiceBusReceivedMessage serviceBusReceivedMessage = processMessageEventArgs.Message;

            string serviceBusReceivedMessageBody = Encoding.UTF8.GetString(serviceBusReceivedMessage.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(serviceBusReceivedMessageBody);

            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickupDateTime = checkoutHeaderDto.PickupDateTime
            };

            foreach (CartDetailDto cartDetail in checkoutHeaderDto.CartDetails)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cartDetail.ProductId,
                    ProductName = cartDetail.Product.Name,
                    Price = cartDetail.Product.Price,
                    Count = cartDetail.Count
                };
                orderHeader.CartTotalItems += cartDetail.Count;
                orderHeader.OrderDetails.Add(orderDetail);
            }

            await _orderRepository.AddOrder(orderHeader);

            try
            {
                await _messageBus.Publish(
                    new PaymentRequestMessage()
                    {
                        FullName = $"{orderHeader.FirstName} {orderHeader.LastName}",
                        CardNumber = orderHeader.CardNumber,
                        CVV = orderHeader.CVV,
                        ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                        OrderId = orderHeader.OrderHeaderId,
                        OrderTotal = orderHeader.OrderTotal,
                        Email = orderHeader.Email
                    },
                    _orderPaymentProcessTopic
                );

                await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs processMessageEventArgs)
        {
            ServiceBusReceivedMessage serviceBusReceivedMessage = processMessageEventArgs.Message;

            string serviceBusReceivedMessageBody = Encoding.UTF8.GetString(serviceBusReceivedMessage.Body);

            UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(serviceBusReceivedMessageBody);

            await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        }
    }
}