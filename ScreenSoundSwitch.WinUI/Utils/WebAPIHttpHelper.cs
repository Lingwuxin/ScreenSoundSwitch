using ScreenSoundSwitch.WinUI.Data;
using ScreenSoundSwitch.WinUI.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        public async Task<JwtResponseDto> Login(UserModel user)
        {
            var loginData = new
            {
                user.Email,
                user.Password
            };

            string json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{baseUrl}user/login", content);

            if (!response.IsSuccessStatusCode)
            {
                // 登录失败处理
                Debug.WriteLine($"登录失败: {response.StatusCode}");
                return null;
            }

            // 读取响应内容
            string responseContent = await response.Content.ReadAsStringAsync();

            // 反序列化为 JWT 响应模型
            var jwtResponse = JsonSerializer.Deserialize<JwtResponseDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            SetToken(jwtResponse.Token);
            // 返回 token（或根据需要也可以返回整个对象）
            return jwtResponse;
        }

        // 注册方法
        public async Task<HttpResponseMessage> Register(UserModel user)
        {
            var registerData = new
            {
                user.Email,
                user.Username,
                user.Password
            };
            Debug.WriteLine(registerData);
            string json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{baseUrl}user/register", content);
            Debug.Write(response.Content);
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