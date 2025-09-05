
namespace cai.Domain
{
    public class HttpClientSettings
    {
        public int Timeout { get; set; }
        public string B2bApiUrl { get; set; }
        public int RetryRequestCount { get; set; }
        public int RetryRequestInterval { get; set; }

    }
}
