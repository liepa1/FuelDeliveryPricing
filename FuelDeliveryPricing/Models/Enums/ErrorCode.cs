using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Models.Enums
{
    public enum ErrorCode
    {
        MISSING_FIELD,
        UNKNOWN_TYPE,
        UNKNOWN_CLIENT,
        INVALID_DATE,
        INVALID_VALUE,
        DUPLICATE_ID
    }
}
