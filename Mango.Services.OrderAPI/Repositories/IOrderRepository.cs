using Mango.Services.OrderAPI.Models.Entities;

namespace Mango.Services.OrderAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    }
}