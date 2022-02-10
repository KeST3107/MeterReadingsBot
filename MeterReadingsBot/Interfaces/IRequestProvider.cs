using System;
using System.Net.Http;

namespace MeterReadingsBot.Interfaces
{
    public interface IRequestProvider
    {
        public (Uri, StringContent) SendReadings(int personalNumber, int hot);
        public (Uri, StringContent) GetInformation(int personalNumber);
    }
}
