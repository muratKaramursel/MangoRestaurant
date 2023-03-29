using Mango.Services.EmailAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        #region DBSets
        public DbSet<EmailLog> EmailLogs { get; set; }
        #endregion DBSets
    }
}