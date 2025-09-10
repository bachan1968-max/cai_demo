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
    }
}
