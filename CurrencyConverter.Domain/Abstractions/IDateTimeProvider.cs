using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Domain.Abstractions
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }

    }
}
