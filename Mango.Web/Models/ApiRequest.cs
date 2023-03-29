using Mango.Web.Models.Enums;

namespace Mango.Web.Models
{
    public class ApiRequest
    {
        public ApiTypes ApiType { get; set; }
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}