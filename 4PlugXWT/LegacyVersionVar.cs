using System;
using System.Globalization;

namespace FPlug
{
    public class VersionVar
    {
        public Decimal Version = 0;
        public int VersionPrefix = 0;
        public string OriginalString = "";

        static CultureInfo enUS = new CultureInfo("en-US");

        public bool TryParse(string s)
        {
            if (s == null)
                return false;
            string S = s;
            if (s.ToLower().StartsWith("alpha "))
            {
                VersionPrefix = 0;
                s = s.Substring(6);
            }
            else if (s.ToLower().StartsWith("beta "))
            {
                VersionPrefix = 1;
                s = s.Substring(5);
            }
            else
            {
                VersionPrefix = 2;
            }
            Decimal d;
            if (Decimal.TryParse(s, NumberStyles.Any, enUS, out d))
            {
                Version = d;
                OriginalString = S;
                return true;
            }
            else
            {
                return false;
            }
        }

        public VersionVar()
        {

        }

        public VersionVar(string s)
        {
            TryParse(s);
        }

        public static bool operator ==(VersionVar a, VersionVar b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VersionVar a, VersionVar b)
        {
            return !a.Equals(b);
        }

        public static bool operator >(VersionVar a, VersionVar b)
        {
            if (a.VersionPrefix > b.VersionPrefix)
                return true;
            if (a.VersionPrefix < b.VersionPrefix)
                return false;

            if (a.Version > b.Version)
                return true;
            if (a.Version < b.Version)
                return false;
            return false;
        }

        public static bool operator <(VersionVar a, VersionVar b)
        {
            if (a.VersionPrefix < b.VersionPrefix)
                return true;
            if (a.VersionPrefix > b.VersionPrefix)
                return false;

            if (a.Version < b.Version)
                return true;
            if (a.Version > b.Version)
                return false;
            return false;
        }

        public override bool Equals(object v)
        {
            if (v is VersionVar)
            {
                if (((VersionVar)v).Version != Version)
                    return false;
                if (((VersionVar)v).VersionPrefix != VersionPrefix)
                    return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return OriginalString;
        }
    }
}
