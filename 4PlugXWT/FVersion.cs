using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FPlug
{
    public class FVersion
    {
        public decimal Value { get; private set; }
        public int Prefix { get; private set; }

        string stringValue;

        public FVersion(decimal value, int prefix = 2)
        {
            Value = value;
            Prefix = prefix;
            stringValue = prefix == 1 ? "beta " : (prefix == 0 ? "alpha " : "") + value.ToString(CultureInfo.InvariantCulture);
        }

        public static FVersion TryParse(string version)
        {
            int prefix;
            decimal value;

            string s = version.Trim().ToLower();

            if (s.StartsWith("alpha "))
            {
                prefix = 0;
                s = s.Substring(6).Trim();
            }
            else if (s.StartsWith("beta "))
            {
                prefix = 1;
                s = s.Substring(5).Trim();
            }
            else
                prefix = 2;

            if (Decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return new FVersion(value, prefix);

            return null;
        }

        public static bool operator >(FVersion a, FVersion b)
        {
            return a.Prefix > b.Prefix || (a.Prefix == b.Prefix && a.Value > b.Value);
        }

        public static bool operator <(FVersion a, FVersion b)
        {
            return a.Prefix < b.Prefix || (a.Prefix == b.Prefix && a.Value < b.Value);
        }

        public override string ToString()
        {
            return stringValue;
        }
    }
}
