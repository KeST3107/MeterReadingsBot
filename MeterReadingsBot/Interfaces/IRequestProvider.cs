namespace MeterReadingsBot.Interfaces
{
    using System;
    using System.Net.Http;

    public interface IRequestProvider
    {
        public (Uri, StringContent) SendReadings(int personalNumber, int hot);
        public (Uri, StringContent) GetInformation(int personalNumber);
    }
}
