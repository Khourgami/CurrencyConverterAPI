using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Domain.Errors
{
    public static class ErrorCodes
    {
        public const string Validation = "DOMAIN.VALIDATION";
        public const string CurrencyNotSupported = "DOMAIN.CURRENCY_NOT_SUPPORTED";
        public const string CurrencyBlacklisted = "DOMAIN.CURRENCY_BLACKLISTED";
        public const string RateNotFound = "DOMAIN.RATE_NOT_FOUND";
        public const string Pagination = "DOMAIN.PAGINATION_INVALID";
        public const string DateRange = "DOMAIN.DATERANGE_INVALID";
    }
}
