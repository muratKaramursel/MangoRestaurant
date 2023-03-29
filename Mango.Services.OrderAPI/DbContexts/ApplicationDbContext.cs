using Mango.Services.OrderAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        #region DBSets
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        #endregion DBSets
    }
}