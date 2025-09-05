using Newtonsoft.Json;

namespace cai.Domain
{
    public class B2bResponse
    {
        [JsonProperty("Header")]
        public B2bHeaderResponse Header { get; set;}

        [JsonProperty("Body")]
        public B2bBodyResponse Body { get; set; }
    }
}
