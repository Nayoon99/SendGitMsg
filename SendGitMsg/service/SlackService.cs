using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SendGitMsg.service
{
    internal class SlackService
    {
        private readonly string _webhookUrl;
        private static readonly HttpClient _httpClient = new();

        public SlackService(string webhookUrl)
        {
            _webhookUrl = webhookUrl;
        }

        public async Task<bool> SendAsync(string message) 
        {
            var payload = new
            {
                text = message
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_webhookUrl, content);

            return response.IsSuccessStatusCode;
        }
    }
}
