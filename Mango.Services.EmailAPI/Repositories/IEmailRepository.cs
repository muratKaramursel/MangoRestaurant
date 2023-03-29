using Mango.Services.EmailAPI.Models.Entities;

namespace Mango.Services.EmailAPI.Repositories
{
    public interface IEmailRepository
    {
        Task LogEmail(EmailLog emailLog);
    }
}