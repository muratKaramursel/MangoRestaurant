using Mango.Web.Models;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BaseResponseDto<T>> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = new();
                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.RequestUri = new Uri(apiRequest.Url);

                if (apiRequest.Data != null)
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");

                switch (apiRequest.ApiType)
                {
                    case Models.Enums.ApiTypes.GET:
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                    case Models.Enums.ApiTypes.POST:
                        httpRequestMessage.Method = HttpMethod.Post;
                        break;
                    case Models.Enums.ApiTypes.PUT:
                        httpRequestMessage.Method = HttpMethod.Put;
                        break;
                    case Models.Enums.ApiTypes.DELETE:
                        httpRequestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        break;
                }

                HttpResponseMessage httpResponseMessage = null;

                using (HttpClient httpClient = _httpClientFactory.CreateClient("MangoAPI"))
                {
                    httpClient.DefaultRequestHeaders.Clear();

                    if (!string.IsNullOrWhiteSpace(apiRequest.AccessToken))
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);

                    httpResponseMessage = httpClient.SendAsync(httpRequestMessage).Result;
                }

                string apiContent = await httpResponseMessage.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BaseResponseDto<T>>(apiContent);
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<T>()
                {
                    IsSuccess = false,
                    Messages = new()
                    {
                        ex.Message,
                        ex.GetBaseException().Message
                    },
                };
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}