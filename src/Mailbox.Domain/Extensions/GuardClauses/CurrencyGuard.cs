using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ardalis.GuardClauses
{
    public static class CurrencyGuard
    {
        private const string PATTERN = @"/(?<SYMBOL>[$€£]){1}\\s*(?<AMOUNT>[\\d.,]+)/g";
        public static string Currency(this IGuardClause guardClause, string input,
            [CallerArgumentExpression("input")] string? parameterName = null)
        {

            if (!Regex.IsMatch(input, PATTERN))
                throw new ArgumentException("Not a valid currency expression", parameterName);

            return input;
        }
    }
}
