using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Models
{
    public class Shipment
    {
        public string Id { get; set; }
        public FuelType Type { get; set; }
        public ClientType Client { get; set; }
        public DateOnly Date { get; set; }
        public decimal Volume { get; set; }
        public decimal Distance { get; set; }
        public decimal BasePrice { get; set; }
    }

    public enum FuelType
    {
        Gasoline,
        Diesel,
        LPG
    }
    public enum ClientType
    {
        Private,
        Exempt
    }
}
