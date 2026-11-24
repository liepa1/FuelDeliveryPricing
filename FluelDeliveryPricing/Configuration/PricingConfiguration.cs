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
        
    }
}

