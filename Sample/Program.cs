
namespace Sample
{
    using Freenom;
    using Properties;
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            FreenomClient client = new FreenomClient(
                Resources.ResourceManager.GetString("email"),
                Resources.ResourceManager.GetString("password"));

            try
            {
                client.LoginAsync().Wait();

                string domain = Resources.ResourceManager.GetString("domain");            

                List<DNSRecord> rec = client.GetDNSRecordsAsync(domain).Result;

                rec[0].Value = "8.8.8.8";

                client.SetDNSRecordsAsync(domain, rec).Wait();
            }
            catch (Exception e)
            {
                Exception ex;
                if (e.InnerException != null) ex = e.InnerException;
                else ex = e;

                Console.WriteLine("Exception was thrown: " + ex.GetType());
                Console.WriteLine("Exception message: " + ex.Message);
            }

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}