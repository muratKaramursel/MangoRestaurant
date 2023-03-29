using Mango.Services.ShoppingCartAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        #region DBSets
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        #endregion DBSets
    }
}