
namespace Freenom
{
    using System;

    public enum DNSRecordsTypes
    {
        A,
        AAAA,
        CNAME,
        LOC,
        MX,
        NAPTR,
        RP,
        TXT
    }

    internal static class DNSRecordsTypesConverter
    {
        public static DNSRecordsTypes ToDNSRecordsTypes(this string value)
        {
            switch (value)
            {
                case "A": return DNSRecordsTypes.A;
                case "AAAA": return DNSRecordsTypes.AAAA;
                case "CNAME": return DNSRecordsTypes.CNAME;
                case "LOC": return DNSRecordsTypes.LOC;
                case "MX": return DNSRecordsTypes.MX;
                case "NAPTR": return DNSRecordsTypes.NAPTR;
                case "RP": return DNSRecordsTypes.RP;
                case "TXT": return DNSRecordsTypes.TXT;
                default: throw new InvalidCastException(value);
            }
        }
    }
}
