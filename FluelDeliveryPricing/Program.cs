using System.Text.Json;
using FuelDeliveryPricing.Services;
using FuelDeliveryPricing.Configuration;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using System.Formats.Asn1;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using FuelDeliveryPricing.Models.Results;
using FuelDeliveryPricing.Models;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Fuel Delivery Pricing System");

var (pricingConfiguration, fileConfiguration, validationConfiguration, interestConfiguration) = ConfigurationLoader.Load();


var reader = new ShipmentReader();
var pricingService = new PricingService(pricingConfiguration);
ConsoleInput consoleInputService = new ConsoleInput(validationConfiguration);

CsvWriter writer = new();
var reportService = new ReportService();
PaymentValidator paymentValidator = new(validationConfiguration);
var shipmentValidator = new ShipmentValidator(validationConfiguration);
var interestService = new InterestService(interestConfiguration);


string shipmentsPath = Path.Combine(Directory.GetCurrentDirectory(), fileConfiguration.ShipmentsInput);
using FileStream stream = File.OpenRead(shipmentsPath);
using JsonDocument document = await JsonDocument.ParseAsync(stream);

//using JsonDocument document = reader.Read(fileConfiguration.ShipmentsInput);

var (validShipments,errors) = shipmentValidator.Validate(document.RootElement);


List<DeliveryResult> task1Output = new List<DeliveryResult>();

foreach (var val in validShipments)
{
    var result = pricingService.CalculatePrice(val);
    task1Output.Add(result);
}


decimal customVatRate = consoleInputService.ReadNonNegativeDecimal(
    "Enter custom VAT rate for Private clients (%): ") / 100m;

decimal customGasolineExciseRate = consoleInputService.ReadNonNegativeDecimal(
    "Enter custom Excise rate for Gasoline: ");

List<DeliveryResult> task2Results = [];

foreach (var shipment in validShipments)
{
    Console.WriteLine(shipment.Id);
    DeliveryResult result = pricingService.CalculatePrice(
        shipment,
        customVatRate,
        customGasolineExciseRate);

    task2Results.Add(result);
}


var (startDate, endDate) = (new DateOnly(2024,01,01), new DateOnly(2024, 01, 30));
    //consoleInputService.ReadDateRange();


ReportResult report = reportService.GenerateReport(task2Results, startDate, endDate);

using JsonDocument document2 = reader.Read(fileConfiguration.PaymentsInput);
List<Payment> validPayments = paymentValidator.Validate(document2.RootElement);
var interestResult = interestService.CalculateInterest(validPayments, task1Output);

writer.WritePrices(task1Output, fileConfiguration.Task1Output);
writer.WritePrices(task2Results, fileConfiguration.Task2Output);
writer.WriteErrors(errors);
writer.WriteReport(report, fileConfiguration.Bonus1Output);
writer.WriteInterestResults(interestResult, fileConfiguration.Bonus2Output);




return;
