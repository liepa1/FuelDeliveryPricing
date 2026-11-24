using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Models.Results;

namespace FuelDeliveryPricing.Services.Output
{
    public class CsvWriter : ICsvWriter
    {
        private readonly FileConfiguration _configuration;

        private readonly string _outputPath;


        public CsvWriter(FileConfiguration configuration)
        {
            _configuration = configuration;
            _outputPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                configuration.OutputFolder);

            Directory.CreateDirectory(_outputPath);
        }

        public void WritePrices(List<DeliveryResult> prices, string fileName)
        {
            string path = Path.Combine(_outputPath, fileName);

            using var writer = new StreamWriter(path);

            writer.WriteLine(_configuration.Name);
            writer.WriteLine("ShipmentID,NetFuelPrice,LogisticsCost,TotalTax,GrandTotal");

            foreach (var price in prices)
            {
                writer.WriteLine(
                    $"{price.ShipmentId}," +
                    $"{price.NetFuelPrice.ToString("F2", CultureInfo.InvariantCulture)}," +
                    $"{price.LogisticsCost.ToString("F2", CultureInfo.InvariantCulture)}," +
                    $"{price.TotalTax.ToString("F2", CultureInfo.InvariantCulture)}," +
                    $"{price.GrandTotal.ToString("F2", CultureInfo.InvariantCulture)}"
                );
            }
        }

        public void WriteErrors(List<ShipmentError> errors)
        {
            string path = Path.Combine(_outputPath, _configuration.ErrorsOutput);

            using var writer = new StreamWriter(path);

            writer.WriteLine(_configuration.Name);
            writer.WriteLine("ShipmentID,ErrorCode");

            foreach (var error in errors)
            {
                writer.WriteLine($"{error.ShipmentId},{error.Code}");
            }
        }

        public void WriteReport(ReportResult report, string fileName)
        {
            string path = Path.Combine(_outputPath, fileName);

            using var writer = new StreamWriter(path);

            writer.WriteLine(_configuration.Name);
            writer.WriteLine("ShipmentCount,MostExpensiveID,TotalVAT");


            writer.WriteLine(
                    $"{report.DeliveryCount}," +
                    $"{report.MostExpensiveDeliveryId ?? "N/A"}," +
                    $"{report.TotalVat.ToString("F2", CultureInfo.InvariantCulture)}"
                    );

        }

        public void WriteInterestResults(List<InterestResult> results, string fileName)
        {
            string outputDirectory = Path.Combine(
                Directory.GetCurrentDirectory(), _configuration.OutputFolder);

            Directory.CreateDirectory(outputDirectory);

            string filePath = Path.Combine(outputDirectory, fileName);

            using var writer = new StreamWriter(filePath, false);

            writer.WriteLine(_configuration.Name);
            writer.WriteLine("ShipmentID,DueDate,DaysLate,LateInterest");

            foreach (var result in results)
            {
                writer.WriteLine(
                    $"{result.ShipmentId}," +
                    $"{result.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
                    $"{result.LateDays}," +
                    $"{result.Interest.ToString("F2", CultureInfo.InvariantCulture)}");
            }
        }
    }
}
