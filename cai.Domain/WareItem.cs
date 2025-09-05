using Newtonsoft.Json;

namespace cai.Domain
{
    public class WareItem
    {
        [JsonProperty("ExternalItemId")]
        public string ExternalItemId { get; set; }

        [JsonProperty("WarePriceRUB")]
        public string WarePriceRUB { get; set; }

        [JsonProperty("APIAvailableReservedQty")]
        public string APIAvailableReservedQty { get; set; }

    }

}
