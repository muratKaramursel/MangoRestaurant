using Mango.Services.EmailAPI.DbContexts;
using Mango.Services.EmailAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task LogEmail(EmailLog emailLog)
        {
            using ApplicationDbContext dbContext = new(_dbContextOptions);
            await dbContext.EmailLogs.AddAsync(emailLog);
            await dbContext.SaveChangesAsync();
        }
    }
}