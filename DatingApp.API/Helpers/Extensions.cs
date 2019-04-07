using System;
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

        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            
            if(theDateTime.AddYears(age) > DateTime.Today)
              age --;

              return age;
              // 用当前时间减去传入的参数中的时间，年份相减，得到整数，如果今年过了生日，则返回age，如果没过，返回age -1
        }
    }
}