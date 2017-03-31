
namespace Freenom
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class FreenomClient
    {
        private const string LoginUrl = "https://my.freenom.com/clientarea.php";

        private HttpClient _client = new HttpClient();

        private string _email;

        private string _password;

        private string _token;

        public FreenomClient(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public bool IsAuthorized { get; private set; }

        public async Task LoginAsync()
        {
            string response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://my.freenom.com/clientarea.php"))
            {
                response = await (await _client.SendAsync(request)).Content.ReadAsStringAsync();

                if (!response.Contains("login"))
                {
                    IsAuthorized = true;
                    return;
                }

                _token = ExtractValue(response, "token\"");
            }

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://my.freenom.com/dologin.php"))
            {
                string command = string.Format("token={0}&username={1}&password={2}", _token, _email, _password);
                request.Content = new StringContent(command, Encoding.UTF8, "application/x-www-form-urlencoded");
                response = await (await _client.SendAsync(request)).Content.ReadAsStringAsync();
            }

            if (response.Contains("Logout")) IsAuthorized = true;
            else
            {
                IsAuthorized = false;
                throw new AuthorizationException(_email + ":" + _password);
            }
        }

        public async Task<List<DNSRecord>> GetDNSRecordsAsync(string domain)
        {
            if (!IsAuthorized)
            {
                throw new NotAuthorizedException();
            }

            long domainId = await GetDomainIdAsync(domain);

            string path = string.Format("https://my.freenom.com/clientarea.php?managedns={0}&domainid={1}", domain, domainId);
            string response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path))
            {
                response = await (await _client.SendAsync(request)).Content.ReadAsStringAsync();
            }

            List<DNSRecord> res = new List<DNSRecord>();
            int i = 0;
            do
            {
                DNSRecord rec = new DNSRecord();
                rec.Line = ExtractValue(response, string.Format("records[{0}][line]", i));
                if (rec.Line == null) break;
                rec.Name = ExtractValue(response, string.Format("records[{0}][name]", i));
                rec.Value = ExtractValue(response, string.Format("records[{0}][value]", i));
                rec.Type = ExtractValue(response, string.Format("records[{0}][type]", i)).ToDNSRecordsTypes();
                rec.TTL = int.Parse(ExtractValue(response, string.Format("records[{0}][ttl]", i)));

                res.Add(rec);
                i++;
            } while (true);

            return res;
        }

        public async Task SetDNSRecordsAsync(string domain, List<DNSRecord> records)
        {
            if (!IsAuthorized)
            {
                throw new NotAuthorizedException();
            }

            long domainId = await GetDomainIdAsync(domain);

            string path = string.Format("https://my.freenom.com/clientarea.php?managedns={0}&domainid={1}", domain, domainId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path))
            {
                string command = BuildModifyRequest(records);
                request.Content = new StringContent(command, Encoding.UTF8, "application/x-www-form-urlencoded");
                string response = await (await _client.SendAsync(request)).Content.ReadAsStringAsync();
                if (!response.Contains("Record added successfully"))
                {
                    throw new InvalidDNSRecordsException();
                }
            }
        }
        
        private string BuildModifyRequest(List<DNSRecord> records)
        {
            string res = string.Format("token={0}&dnsaction=modify&", _token);
            for (int i = 0; i < records.Count; i++)
            {
                res += string.Format(
                    "records[{0}][line]=&records[{0}][type]={1}&records[{0}][name]={2}&records[{0}][ttl]={3}&records[{0}][value]={4}", 
                    i,
                    records[i].Type.ToString(),
                    records[i].Name,
                    records[i].TTL,
                    records[i].Value
                    );

                if (records.Count > 1 && i < records.Count - 1)
                {
                    res += "&";
                }
            }

            return res;
        }

        private string ExtractValue(string content, string paramName, string valueName = "value=\"")
        {
            int i = content.IndexOf(paramName);
            if (i == -1) return null;

            i = content.IndexOf(valueName, i);
            string res = null;

            if (i > 0)
            {
                int j = content.IndexOf("\"", i + valueName.Length);
                res = content.Substring(i, j - i);
                res = res.Remove(0, valueName.Length);
            }

            return res;
        }

        private async Task<long> GetDomainIdAsync(string domain)
        {
            if (!IsAuthorized)
            {
                throw new NotAuthorizedException();
            }

            string response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://my.freenom.com/clientarea.php?action=domains"))
            {
                response = await (await _client.SendAsync(request)).Content.ReadAsStringAsync();
            }

            string s = ExtractValue(response, domain, "domaindetails&id=");
            if (s == null) throw new InvalidDomainException(domain);
            else return long.Parse(s);
        }
    }
}
