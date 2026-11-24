using FuelDeliveryPricing.Application;
using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Services;
using FuelDeliveryPricing.Services.Input;
using FuelDeliveryPricing.Services.Output;


try
{
    var (pricingConfig, fileConfig, validationConfig, interestConfig) =
        ConfigurationLoader.Load();

    var workflow = new PricingWorkflow(
        new JsonReader(),
        new ShipmentValidator(validationConfig),
        new PaymentValidator(validationConfig),
        new PricingService(pricingConfig),
        new ReportService(),
        new InterestService(interestConfig),
        new ConsoleInput(validationConfig),
        new CsvWriter(fileConfig),
        fileConfig);

    await workflow.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"The run failed: {ex.Message}");
    Environment.ExitCode = 1;
}
