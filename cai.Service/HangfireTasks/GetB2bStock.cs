using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using cai.Service.B2bInteraction;
using cai.Domain;
using cai.Service.EmailSender;
using System.Collections.Generic;
using System.IO;

namespace cai.Service.HangfireTasks
{
    public class GetB2bStock : HangfireTask
    {
        protected readonly IB2bRepository _b2bRepo;
        protected readonly IOptions<AppSettings> _appSettings;
        protected readonly IEmailRepository _emailRepository;
        protected readonly CsvWriterFactory _csvFactory;

        public GetB2bStock(ILogger<HangfireTask> logger, IB2bRepository b2bRepo, IOptions<AppSettings> appSettings,
             IEmailRepository emailRepository, CsvWriterFactory csvFactory
            ) : base(logger)
        {
            _b2bRepo = b2bRepo ?? throw new ArgumentNullException(nameof(b2bRepo));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _emailRepository = emailRepository ?? throw new ArgumentNullException(nameof(emailRepository));
            _csvFactory = csvFactory ?? throw new ArgumentNullException(nameof(_csvFactory));
        }
        protected override async Task DoJobAsync()
        {
            var items = await _b2bRepo.GetFullStock(_appSettings.Value.B2bUser, _appSettings.Value.B2bUserPassword);
            if (items != null)
            {
                var dtoItems = new List<CsvDto>();

                foreach (var i in items)
                {
                    var priceOk = decimal.TryParse(i.WarePriceRUB, out decimal price);
                    if (!priceOk)
                    {
                        _logger.LogError("Item {ExternalItemId} has wrong price format: {WarePriceRUB}", i.ExternalItemId, i.WarePriceRUB);
                        continue;
                    }
                    if (price == 0) continue;

                    var amountOkReserve = int.TryParse(i.APIAvailableReservedQty, out int amountTotal);
                    if (!amountOkReserve)
                    {
                        _logger.LogError("Item {ExternalItemId} is not a digit: {APIAvailableReservedQty}", i.ExternalItemId, i.APIAvailableReservedQty);
                        continue;
                    }

                    dtoItems.Add(new CsvDto
                    {
                        ExternalItemId = i.ExternalItemId,
                        Price = price.ToString("0.00"),
                        Amount = amountTotal.ToString()
                    });
                }
                var tempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                var fileNameOriginal = Path.Combine(tempFolder, Guid.NewGuid().ToString());
                var csvWriter = _csvFactory.Create(fileNameOriginal);
                csvWriter.Context.RegisterClassMap<CsvDtoMap>();
                csvWriter.WriteRecords(dtoItems);
                csvWriter.Flush();
                csvWriter.Dispose();

                FileStream file = new(fileNameOriginal, FileMode.Open, FileAccess.Read);

                await _emailRepository.SendB2bStock(file);
                file.Close();
                file.Dispose();

                try
                {
                    File.Delete(fileNameOriginal);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed detete temporary file {fileNameOriginal}", fileNameOriginal);
                }
            }
        }
    }
}
