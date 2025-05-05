using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Utils
{
    // 封装对 WebAPI 的访问（单例模式）
    internal sealed class WebAPIHttpHelper
    {
        private static readonly Lazy<WebAPIHttpHelper> lazyInstance =
            new(() => new WebAPIHttpHelper());

        public static WebAPIHttpHelper Instance => lazyInstance.Value;

        private readonly string baseUrl = "http://localhost:5253/api/";
        private readonly HttpClient httpClient;
        private string token;

        // 私有构造函数，防止外部实例化
        private WebAPIHttpHelper()
        {
            httpClient = new HttpClient();
        }

        // 登录方法，使用 JSON 格式传参
        public async Task<HttpResponseMessage> Login(UserModel user)
        {
            var loginData = new
            {
                user.Email,
                user.Password
            };

            string json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{baseUrl}User/login", content);
            return response;
        }

        // 注册方法
        public async Task<HttpResponseMessage> Register(UserModel user)
        {
            var registerData = new
            {
                user.Email,
                user.Password
            };

            string json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{baseUrl}register", content);
            return response;
        }

        // 设置 Bearer Token 用于授权
        public void SetToken(string token)
        {
            this.token = token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 本地登出，清除 token 和 header
        public void Logout()
        {
            token = null;
            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // 可选：带网络请求的登出
        public async Task<HttpResponseMessage> LogoutAsync()
        {
            var response = await httpClient.PostAsync($"{baseUrl}logout", null);
            Logout(); // 清除本地 token
            return response;
        }
    }
}