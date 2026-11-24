using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Models;
using FuelDeliveryPricing.Configuration;
using System.Globalization;
using FuelDeliveryPricing.Models.Results;

namespace FuelDeliveryPricing.Services
{
    public class PricingService
    {
        private readonly PricingConfiguration _configuration;

        public PricingService(PricingConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DeliveryResult CalculatePrice(Shipment shipment, decimal? customVat = null, decimal? customGasolineExciseRate = null)
        {
            // 1. Determine discount rate
            decimal discountRate = 0m;

            if (shipment.Volume > _configuration.DiscountThresholdLarge)
                discountRate = _configuration.DiscountLarge;
            else if (shipment.Volume > _configuration.DiscountThresholdSmall)
                discountRate = _configuration.DiscountSmall;

            // 2. Calculate net fuel price
            decimal netFuelPrice = Round(
                shipment.Volume *
                shipment.BasePrice *
                (1 - discountRate)
            );

            // 3. Calculate logistics cost
            decimal logisticsCost = Round(
                shipment.Distance *
                _configuration.DeliveryPricePerKm +
                (shipment.Volume < _configuration.SmallLoadThreshold
                    ? _configuration.SmallLoadFee
                    : 0m)
            );

            // 4. Determine excise rate
            decimal exciseRate = shipment.Type switch
            {
                FuelType.Gasoline => customGasolineExciseRate ?? _configuration.GasolineExcise,
                FuelType.Diesel => _configuration.DieselExcise,
                FuelType.LPG => _configuration.LpgExcise,
                _ => throw new ArgumentException("Unknown fuel type.")
            };

            
            decimal excise = Round(shipment.Volume * exciseRate);

            

            // 5. Determine VAT rate
            decimal vatRate = shipment.Client switch
            {
                ClientType.Exempt => 0m,

                ClientType.Private => customVat ?? (shipment.Date <
                             _configuration.VatChangeDate
                    ? _configuration.VatBefore2024
                    : _configuration.VatFrom2024),

                _ => throw new ArgumentException("Unknown client type.")
            };

            // 6. Calculate VAT
            decimal vat = Round(
                (netFuelPrice + logisticsCost + excise) * vatRate
            );

            // 7. Calculate total tax
            decimal totalTax = Round(excise + vat);

            // 8. Calculate grand total
            decimal grandTotal = Round(
                netFuelPrice + logisticsCost + totalTax
            );

            return new DeliveryResult
            {
                ShipmentId = shipment.Id!,
                NetFuelPrice = netFuelPrice,
                LogisticsCost = logisticsCost,
                Excise = excise,
                Vat = vat,
                TotalTax = totalTax,
                GrandTotal = grandTotal,
                Date = shipment.Date
            };
        }

        private static decimal Round(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }


    }
}
