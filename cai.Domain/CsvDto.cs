using System;
using CsvHelper.Configuration;

namespace cai.Domain
{
    public class CsvDto
    {
        
        public string SupplierCode { get; set; } = "MV3046";
        public string ExternalItemId { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString();
    }

    public class CsvDtoMap : ClassMap<CsvDto> 
    {
        public CsvDtoMap()
        {
            Map(m => m.SupplierCode).Index(0).Name("Поставщик Код");
            Map(m => m.ExternalItemId).Index(1).Name("Товар Но. Поставщика");
            Map(m => m.Amount).Index(2).Name("Количество");
            Map(m => m.Price).Index(3).Name("Цена");
            Map(m => m.Date).Index(4).Name("Дата");

        }
    }
}
