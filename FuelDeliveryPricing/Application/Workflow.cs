using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Models;
using FuelDeliveryPricing.Models.Results;
using FuelDeliveryPricing.Services;
using FuelDeliveryPricing.Services.Input;
using FuelDeliveryPricing.Services.Input.Interfaces;
using FuelDeliveryPricing.Services.Output;

namespace FuelDeliveryPricing.Application;
public class PricingWorkflow
{
    private readonly IJsonReader _reader;
    private readonly ShipmentValidator _shipmentValidator;
    private readonly PaymentValidator _paymentValidator;
    private readonly PricingService _pricingService;
    private readonly ReportService _reportService;
    private readonly InterestService _interestService;
    private readonly IConsoleInput _consoleInput;
    private readonly ICsvWriter _csvWriter;
    private readonly FileConfiguration _files;

    public PricingWorkflow(
        IJsonReader reader,
        ShipmentValidator shipmentValidator,
        PaymentValidator paymentValidator,
        PricingService pricingService,
        ReportService reportService,
        InterestService interestService,
        IConsoleInput consoleInput,
        ICsvWriter csvWriter,
        FileConfiguration files)
    {
        _reader = reader;
        _shipmentValidator = shipmentValidator;
        _paymentValidator = paymentValidator;
        _pricingService = pricingService;
        _reportService = reportService;
        _interestService = interestService;
        _consoleInput = consoleInput;
        _csvWriter = csvWriter;
        _files = files;
    }

    public async Task RunAsync()
    {
        using var shipmentDocument = await _reader.Read(_files.ShipmentsInput);
        var (shipments, errors) =
            _shipmentValidator.Validate(shipmentDocument.RootElement);

        List<DeliveryResult> standardPrices = shipments
            .Select(shipment => _pricingService.CalculatePrice(shipment))
            .ToList();

        decimal customVatRate = _consoleInput.ReadNonNegativeDecimal(
            "Enter custom VAT rate for Private clients (%): ") / 100m;

        decimal customGasolineExciseRate = _consoleInput.ReadNonNegativeDecimal(
            "Enter custom Excise rate for Gasoline: ");

        List<DeliveryResult> customPrices = shipments
            .Select(shipment => _pricingService.CalculatePrice(
                shipment,
                customVatRate,
                customGasolineExciseRate))
            .ToList();

        var (startDate, endDate) = _consoleInput.ReadDateRange();
        ReportResult report =
            _reportService.GenerateReport(customPrices, startDate, endDate);

        using var paymentDocument = await _reader.Read(_files.PaymentsInput);
        List<Payment> payments =
            _paymentValidator.Validate(paymentDocument.RootElement);

        List<InterestResult> interestResults =
            _interestService.CalculateInterest(payments, standardPrices);

        _csvWriter.WritePrices(standardPrices, _files.Task1Output);
        _csvWriter.WritePrices(customPrices, _files.Task2Output);
        _csvWriter.WriteErrors(errors);
        _csvWriter.WriteReport(report, _files.Bonus1Output);
        _csvWriter.WriteInterestResults(interestResults, _files.Bonus2Output);
    }
}
