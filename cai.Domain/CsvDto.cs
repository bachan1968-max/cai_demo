using System;
using CsvHelper.Configuration;

namespace cai.Domain
{
    public class CsvDto
    { 
        public long ExternalItemId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }

        public CsvDto(long externalItemId, int amount, decimal price)
        {
            ExternalItemId = externalItemId;
            Amount = amount;
            Price = price;
        }
    }

    public class CsvDtoMap : ClassMap<CsvDto> 
    {
        public CsvDtoMap()
        {
            Map(m => m.ExternalItemId).Index(1).Name("Товар Но. Поставщика");
            Map(m => m.Amount).Index(2).Name("Количество");
            Map(m => m.Price).Index(3).Name("Цена");
        }
    }
}
