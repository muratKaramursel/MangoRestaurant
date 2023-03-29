using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.OrderHeaders.Add(orderHeader);

                await dbContext.SaveChangesAsync();

                return true;
            }
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                var orderHeaderFromDb = await dbContext.OrderHeaders.SingleOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);

                orderHeaderFromDb.PaymentStatus = paid;

                dbContext.OrderHeaders.Update(orderHeaderFromDb);

                dbContext.SaveChanges();
            }
        }
    }
}