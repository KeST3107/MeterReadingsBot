using System;
using System.Net.Http;
using System.Text;
using MeterReadingsBot.Interfaces;

namespace MeterReadingsBot.Providers
{
    public class RequestProvider : IRequestProvider
    {
        public (Uri, StringContent) SendReadings(int personalNumber, int hot)
        {
            var uri = new Uri("https://www.kotlas-okits.ru/pokazaniya/read32020.php");
            var content = new StringContent($"nomer={personalNumber}&0={hot}", Encoding.UTF8,
                "application/x-www-form-urlencoded");
            return (uri, content);
        }

        public (Uri, StringContent) GetInformation(int personalNumber)
        {
            var uri = new Uri("https://www.kotlas-okits.ru/pokazaniya/read22020.php");
            var content = new StringContent($"nomer={personalNumber}", Encoding.UTF8,
                "application/x-www-form-urlencoded");
            return (uri, content);
        }
    }
}
