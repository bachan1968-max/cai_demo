using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cai.Domain;
using System.IO;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cai.Service.EmailSender
{
    public class EmailRepository : IEmailRepository
    {
        private readonly string _workDir;
        public readonly ILogger<EmailRepository> _logger;
        protected readonly SmtpConfiguration _smtpConfig;
        public readonly IFluentEmail _mailer;
        public readonly IOptions<AppSettings> _appSettings;
        public EmailRepository(SmtpConfiguration smtpConfig, IFluentEmail mailer, ILogger<EmailRepository> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _smtpConfig = smtpConfig ?? throw new ArgumentNullException(nameof(smtpConfig));
            _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
            _workDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _appSettings.Value.MailTemplatesFolder);
        }
        public async Task SendB2bStock(FileStream file)
        {
            var recipients = new List<Address>();
            foreach (var r in _appSettings.Value.SendB2bStock) recipients.Add(new Address { EmailAddress = r });
            var recipientsBcc = new List<Address>();
            foreach (var r in _appSettings.Value.SendB2bStockBcc) recipientsBcc.Add(new Address { EmailAddress = r });
            var template = Path.Combine(_workDir, "SendB2bStock.cshtml");
            var dt = DateTime.Now;
            var fileName = _appSettings.Value.AttachFileName.Replace("%dd_mm_yyyy%", $"{dt.Day:00} {dt.Month:00} {dt.Year.ToString().Substring (2)}");

            try
            {
                IFluentEmail mail = _mailer.To(recipients).BCC(recipientsBcc)
                    .Subject($"Data on {dt.Day:00} {dt.Month:00} {dt.Year.ToString().Substring(2)}")
                    .UsingTemplateFromFile(template, new { });
                var attach = new Attachment
                {
                    Data = file,
                    Filename = fileName
                };
                mail.Attach(attach);
                var sendResult = await mail.SendAsync();
                if (sendResult.Successful)
                {
                    _logger.LogInformation("Mail sent: {@recipientsStr}", recipients);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed send mail");
            }
        }
    }
}
