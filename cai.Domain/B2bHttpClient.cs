using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace cai.Domain
{
    public class B2bHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptionsMonitor<HttpClientSettings> _settings;
        private readonly ILogger<B2bHttpClient> _logger;

        public B2bHttpClient(HttpClient httpClient, IOptionsMonitor<HttpClientSettings> settings, ILogger<B2bHttpClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient.BaseAddress = new Uri(_settings.CurrentValue.B2bApiUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.CurrentValue.Timeout);
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("charset", "utf-8");
        }

        public async Task<HttpResponseMessage> GetFullStock(string user, string password, CancellationToken ct)
        {
            var url = $"GetFullStock";
            _logger.LogInformation("Запрос: {@b2bApiUrl}", _httpClient.BaseAddress + url);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(""));
            return await _httpClient.GetAsync(url, ct);
        } 
    }
}
