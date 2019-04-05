using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            // 给请求头添加一下消息，ng就不会提示 no acccess-control-allow-origin错误
          response.Headers.Add("Application-Error",message);
          response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
          response.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
}