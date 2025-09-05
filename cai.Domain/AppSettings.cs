using System.Collections.Generic;

namespace cai.Domain
{
    public class AppSettings
    {
        public string B2bUser { get; set; }
        public string B2bUserPassword { get; set; }
        public string MailTemplatesFolder { get; set; }
        public List<string> SendB2bStock { get; set; }
        public List<string> SendB2bStockBcc { get; set; }
        public string AttachFileName { get; set; }
    }
}
