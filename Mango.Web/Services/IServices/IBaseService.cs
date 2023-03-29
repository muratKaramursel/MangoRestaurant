using Mango.Web.Models;
using Mango.Web.Models.DTOs;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        Task<BaseResponseDto<T>> SendAsync<T>(ApiRequest apiRequest);
    }
}