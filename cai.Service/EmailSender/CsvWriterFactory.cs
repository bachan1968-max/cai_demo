using System;
using System.Text;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace cai.Service.EmailSender
{
    public class CsvWriterFactory
    {
        private readonly CsvHelper.Factory _factory;
        public CsvWriterFactory(CsvHelper.Factory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IWriter Create(string fileName)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InstalledUICulture);
            config.Delimiter = ";";
            var csvWriter = _factory.CreateWriter(new StreamWriter(fileName, false,
                Encoding.GetEncoding("windows-1251")), config);
            return csvWriter;
        }
    }
}
