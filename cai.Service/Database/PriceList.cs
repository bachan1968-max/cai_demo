using System;
using System.ComponentModel.DataAnnotations;

namespace cai.Service.Database
{
    public class PriceList
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime SentAt { get; set; }
        public string SentTo { get; set; }

        public PriceList(Guid id, string sentTo)
        {
            Id = id;
            SentAt = DateTime.Now;
            SentTo = sentTo;
        }
    }
}
