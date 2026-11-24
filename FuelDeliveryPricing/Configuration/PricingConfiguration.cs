using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Configuration;

public class PricingConfiguration
{
    public decimal DiscountThresholdSmall { get; set; }
    public decimal DiscountThresholdLarge { get; set; }

    public decimal DiscountSmall { get; set; }
    public decimal DiscountLarge { get; set; }

    public decimal DeliveryPricePerKm { get; set; }
    public decimal SmallLoadThreshold { get; set; }
    public decimal SmallLoadFee { get; set; }

    public decimal GasolineExcise { get; set; }
    public decimal DieselExcise { get; set; }
    public decimal LpgExcise { get; set; }

    public decimal VatBefore2024 { get; set; }
    public decimal VatFrom2024 { get; set; }

    public DateOnly VatChangeDate { get; set; }

    public void Validate()
    {
        if (DiscountThresholdSmall < 0 || DiscountThresholdLarge <= DiscountThresholdSmall)
            throw new InvalidOperationException(
                "Pricing discount thresholds must be non-negative and the large threshold must exceed the small threshold.");

        if (DiscountSmall < 0 || DiscountLarge < DiscountSmall || DiscountLarge > 1)
            throw new InvalidOperationException(
                "Discount rates must be between 0 and 1, and the large discount cannot be smaller than the small discount.");

        if (DeliveryPricePerKm < 0 || SmallLoadThreshold < 0 || SmallLoadFee < 0)
            throw new InvalidOperationException(
                "Delivery price, small-load threshold, and small-load fee must be non-negative.");

        if (GasolineExcise < 0 || DieselExcise < 0 || LpgExcise < 0)
            throw new InvalidOperationException("Excise rates must be non-negative.");

        if (VatBefore2024 < 0 || VatBefore2024 > 1 || VatFrom2024 < 0 || VatFrom2024 > 1)
            throw new InvalidOperationException("VAT rates must be between 0 and 1.");

        if (VatChangeDate == default)
            throw new InvalidOperationException("VatChangeDate must be configured.");
    }
}

