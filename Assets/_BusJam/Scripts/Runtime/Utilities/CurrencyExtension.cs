using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BusJam.Scripts.Runtime.Utilities
{
    public static class CurrencyExtension
    {
        private static readonly Dictionary<double, string> ValueMap = new()
        {
            { 1000, "K" },
            { 1000000, "M" },
            { 1000000000, "B" },
            { 1000000000000, "T" },
            { 1000000000000000, "q" },
            { 1000000000000000000, "Q" },
            { 1000000000000000000000d, "s" },
            { 1000000000000000000000000d, "S" },
            { 1000000000000000000000000000d, "O" },
            { 1000000000000000000000000000000d, "N" },
            { 1000000000000000000000000000000000d, "D" },
            { 1000000000000000000000000000000000000d, "U" },
            { 1000000000000000000000000000000000000000d, "Du" },
            { 1000000000000000000000000000000000000000000d, "Tr" },
            { 1000000000000000000000000000000000000000000000d, "Qt" },
            { 1000000000000000000000000000000000000000000000000d, "Qd" },
            { 1000000000000000000000000000000000000000000000000000d, "Sd" },
            { 1000000000000000000000000000000000000000000000000000000d, "St" },
            { 1000000000000000000000000000000000000000000000000000000000d, "Oc" }
        };

        public static string FormatNumber(double amount, bool includeDecimalBeforeThousand = true, bool includeDecimalForLargeNumbers = true)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");

            if (amount < 1000)
            {
                return amount.ToString(includeDecimalBeforeThousand ? "F1" : "F0", cultureInfo);
            }

            foreach (var pair in ValueMap)
            {
                if (amount < pair.Key)
                {
                    double roundedNumber = amount / (pair.Key / 1000);
                    string format = includeDecimalForLargeNumbers ? "F1" : "F0";
                    return roundedNumber.ToString(format, cultureInfo) + pair.Value;
                }
            }

            var lastPair = ValueMap.Last();
            double largestRoundedNumber = amount / (lastPair.Key / 1000);
            string largestFormat = includeDecimalForLargeNumbers ? "F1" : "F0";
            return largestRoundedNumber.ToString(largestFormat, cultureInfo) + lastPair.Value;

        }
    }
}