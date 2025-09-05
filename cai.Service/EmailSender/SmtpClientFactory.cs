using System;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace cai.Service.EmailSender
{
    public class SmtpClientFactory
    {
        private readonly IOptionsMonitor<SmtpConfiguration> _config;

        public SmtpClientFactory(IOptionsMonitor<SmtpConfiguration> config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(_config)); ;
        }

        public SmtpClient Create()
        {
            var deliveryMethod = Enum.Parse<SmtpDeliveryMethod>(_config.CurrentValue.DeliveryMethod);
            switch (deliveryMethod)
            {
                case SmtpDeliveryMethod.Network:
                    return new()
                    {
                        Host = _config.CurrentValue.SmtpHost,
                        UseDefaultCredentials = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                case SmtpDeliveryMethod.SpecifiedPickupDirectory:
                    return new()
                    {
                        UseDefaultCredentials = true,
                        DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                        PickupDirectoryLocation = _config.CurrentValue.DropFolder
                    };
                default:
                    throw new NotSupportedException($"Not supported delivery method {deliveryMethod}.");
            }
        }
    }

}
