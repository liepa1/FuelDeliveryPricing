using Microsoft.Extensions.Configuration;

namespace FuelDeliveryPricing.Configuration;

public class ConfigurationLoader
{
    public static (PricingConfiguration Pricing,
                   FileConfiguration Files,
                   ValidationConfiguration Validation, 
                   InterestConfiguration Interest) Load()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var pricing = configuration
            .GetRequiredSection("Pricing")
            .Get<PricingConfiguration>()
            ?? throw new InvalidOperationException(
                "Pricing configuration is missing.");

        var files = configuration
            .GetRequiredSection("Files")
            .Get<FileConfiguration>()
            ?? throw new InvalidOperationException(
                "Files configuration is missing.");

        files.Validate();

        var validation = configuration
            .GetRequiredSection("Validation")
            .Get<ValidationConfiguration>()
            ?? throw new InvalidOperationException(
                "Validation configuration is missing.");

        var interest = configuration
            .GetRequiredSection("Interest")
            .Get<InterestConfiguration>()
            ?? throw new InvalidOperationException(
                "Interest Configuration is missing");

        return (pricing, files, validation, interest);
    }
}