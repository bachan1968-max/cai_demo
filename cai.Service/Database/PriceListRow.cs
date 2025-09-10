using System;
using System.ComponentModel.DataAnnotations;

namespace cai.Service.Database
{
    public class PriceListRow
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PriceListId { get; set; }
        public long ExternalItemId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }

        public PriceListRow(Guid priceListId, long externalItemId, int amount, decimal price)
        {
            Id = Guid.NewGuid();
            PriceListId = priceListId;
            ExternalItemId = externalItemId;
            Amount = amount;
            Price = price;
        }
    }
}
