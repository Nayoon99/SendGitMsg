using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Text.Json;
using SendGitMsg.model;

namespace SendGitMsg.service
{
    internal class SmsService
    {
        private readonly Config _config;
        private readonly HttpClient _client = new HttpClient();

        public SmsService(Config config)
        {
            _config = config;
            _client.BaseAddress = new Uri("https://api.coolsms.co.kr");
        }

        // 성공 or 실패 반환
        public async Task<bool> SendSmsAsync(string message)
        {
            var payload = new
            {
                api_key = _config.CoolSmsApiKey,
                api_secret = _config.CoolSmsApiSecret,
                to = _config.ToPhone,
                from = _config.FromPhone,
                text = message,
                type = "SMS"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync("/messages/v4/send", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
