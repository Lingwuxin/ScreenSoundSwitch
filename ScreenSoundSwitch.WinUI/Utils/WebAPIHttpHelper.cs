using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace ScreenSoundSwitch.WinUI.Utils
{
    //封装对WebAPI的访问
    internal class WebAPIHttpHelper
    {   
        private string baseUrl = "http://localhost:5000/api/";
        private HttpClient httpClient;
        private HttpRequestHeaderCollection headers;
        private string token;
        WebAPIHttpHelper() 
        { 
            httpClient = new HttpClient();
            headers=httpClient.DefaultRequestHeaders;
        }
        //登录
        public async Task<HttpResponseMessage> Login(string email, string password)
        {
            var content = new HttpStringContent(
                $"{{\"email\":\"{email}\", \"password\":\"{password}\"}}",
                UnicodeEncoding.Utf8,
                "application/json"
            );
            var response = await httpClient.PostAsync(new Uri("http://localhost:5000/api/login"), content);
            return response;
        }


    }
}
