using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Configuration
{
    public class FileConfiguration
    {
        public string Name { get; set; } = "";
        public string ShipmentsInput { get; set; } = "";
        public string PaymentsInput { get; set; } = "";
        public string OutputFolder { get; set; } = "";
        public string Task1Output { get; set; } = "";
        public string Task2Output { get; set; } = "";
        public string Bonus1Output { get; set; } = "";
        public string Bonus2Output { get; set; } = "";
        public string ErrorsOutput { get; set; } = "";

        public void Validate()
        {
            RequireValue(Name);
                        
            string[] outputNames =
            [Task1Output, Task2Output, Bonus1Output, Bonus2Output, ErrorsOutput];

            foreach (string outputName in outputNames)
            {
                RequireValue(outputName);
            }
                        
            if (outputNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != outputNames.Length)
            {
                throw new InvalidOperationException("Files output names must be unique.");
            }
        }
             

        private static void RequireValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"File names must be configured.");
            }
        }


    } 
}
