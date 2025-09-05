
namespace cai.Service.EmailSender
{
    public class SmtpConfiguration
    {
        public string SmtpHost { get; set; }
        public string DeliveryMethod { get; set; } //SpecifiedPickupDirectory Network
        public string DropFolder { get; set; }
    }
}
