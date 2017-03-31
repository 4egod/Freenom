
namespace Freenom
{
    public class DNSRecord
    {
        public DNSRecordsTypes Type { get; set; }

        public string Name { get; set; }

        public string Line { get; set; }

        public int TTL { get; set; }

        public string Value { get; set; }
    }
}
