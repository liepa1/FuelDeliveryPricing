using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Configuration
{
    public class FileConfiguration
    {
        public string ShipmentsInput { get; set; } = "";
        public string PaymentsInput { get; set; } = "";
        public string OutputFolder { get; set; } = "";
        public string Task1Output { get; set; } = "";
        public string Task2Output { get; set; } = "";
        public string Bonus1Output { get; set; } = "";
        public string Bonus2Output { get; set; } = "";


        public void Validate()
        {
            if (string.IsNullOrEmpty(OutputFolder)) { throw new Exception("OutputFolder is not configured"); }
        }
    } 
}
