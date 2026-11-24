using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Models;
using FuelDeliveryPricing.Configuration;
using System.Globalization;
using FuelDeliveryPricing.Models.Results;
using FuelDeliveryPricing.Models.Enums;

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

            decimal netFuelPrice = CalculateFuelPrice(shipment.Volume, shipment.BasePrice);

            decimal logisticsCost = CalculateLogisticsCost(shipment.Distance, shipment.Volume);

            decimal excise = CalculateExcize(shipment.Type, shipment.Volume, customGasolineExciseRate);

            decimal vat = CalculateVat(shipment.Client, shipment.Date, netFuelPrice, logisticsCost, excise, customVat);

            decimal totalTax = Round(excise + vat);

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

        private decimal CalculateVat(ClientType client, DateOnly date, decimal netFuelPrice, decimal logisticsCost, decimal excise, decimal? customVat)
        {
            decimal vatRate = client switch
            {
                ClientType.Exempt => 0m,

                ClientType.Private => customVat ?? (date <
                             _configuration.VatChangeDate
                    ? _configuration.VatBefore2024
                    : _configuration.VatFrom2024),

                _ => throw new ArgumentException("Unknown client type.")
            };

            return Round((netFuelPrice + logisticsCost + excise) * vatRate);
        }
        private decimal CalculateExcize(FuelType type, decimal volume, decimal? customGasolineExciseRate)
        {
            decimal exciseRate = type switch
            {
                FuelType.Gasoline => customGasolineExciseRate ?? _configuration.GasolineExcise,
                FuelType.Diesel => _configuration.DieselExcise,
                FuelType.LPG => _configuration.LpgExcise,
                _ => throw new ArgumentException("Unknown fuel type.")
            };
            return Round(volume * exciseRate);
        }

        private decimal CalculateFuelPrice(decimal volume, decimal basePrice)
        {
            decimal discountRate = DetermineDiscountRate(volume);
            return Round(volume * basePrice * (1 - discountRate)
            );
        }

        private decimal CalculateLogisticsCost(decimal distance, decimal volume)
        {
            decimal logisticsCost = Round(
                distance *
                _configuration.DeliveryPricePerKm +
                (volume < _configuration.SmallLoadThreshold
                    ? _configuration.SmallLoadFee
                    : 0m)
            );
            return logisticsCost;

        }
        private decimal DetermineDiscountRate(decimal volume)
        {
            decimal discountRate = 0m;
            if (volume > _configuration.DiscountThresholdLarge)
                discountRate = _configuration.DiscountLarge;
            else if (volume > _configuration.DiscountThresholdSmall)
                discountRate = _configuration.DiscountSmall;

            return discountRate;
        }


    }
}
