using Mango.Services.OrderAPI.Models.Entities;
using Mango.Services.OrderAPI.Models.Messages;
using Mango.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;
        private readonly IRabbitMQOrderMessageSender _rabbitMQOrderMessageSender;

        private readonly IModel _channel;

        public RabbitMQCheckoutConsumer(OrderRepository orderRepository, IRabbitMQOrderMessageSender rabbitMQOrderMessageSender)
        {
            _orderRepository = orderRepository;
            _rabbitMQOrderMessageSender = rabbitMQOrderMessageSender;

            ConnectionFactory connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            IConnection connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "checkoutmessagequeue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EventingBasicConsumer eventingBasicConsumer = new(_channel);

            eventingBasicConsumer.Received += (ch, ea) =>
            {
                string content = Encoding.UTF8.GetString(ea.Body.ToArray());

                CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content);

                HandleMessage(checkoutHeaderDto).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("checkoutmessagequeue", false, eventingBasicConsumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CheckoutHeaderDto checkoutHeaderDto)
        {
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

            _rabbitMQOrderMessageSender.SendMessage(
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
                "orderpaymentprocesstopic"
            );
        }
    }
}