using Newtonsoft.Json;

namespace cai.Domain
{
    public class B2bHeaderResponse
    {
        [JsonProperty("Code")]
        public long Code { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

    }
}
