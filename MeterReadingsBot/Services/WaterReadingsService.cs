using System;
using System.Net.Http;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Models;

namespace MeterReadingsBot.Services
{
    public class WaterReadingsService : IWaterReadingsService
    {
        private readonly IHtmlParserService _htmlParserService;
        private readonly IHttpService _httpService;
        private readonly IRequestProvider _requestProvider;

        public WaterReadingsService(IHttpService httpService, IRequestProvider requestProvider,
            IHtmlParserService htmlParserService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _htmlParserService = htmlParserService ?? throw new ArgumentNullException(nameof(htmlParserService));
        }

        public async Task<HttpResponseMessage> SendReadingsAsync(int personalNumber, int hot)
        {
            var request = _requestProvider.SendReadings(personalNumber, hot);
            var response = await _httpService.PostAsync(request.Item1, request.Item2);
            return response;
        }

        public async Task<ClientInfoDto> GetClientInfoAsync(int personalNumber)
        {
            var request = _requestProvider.GetInformation(personalNumber);
            var response = await _httpService.PostAsync(request.Item1, request.Item2);
            var stringHtml = response.Content.ReadAsStringAsync().Result;
            if (stringHtml.Contains("Номер не найден")) throw new Exception();
            var nodes = _htmlParserService.GetReadingsNodes(stringHtml);
            return new ClientInfoDto
            {
                Address = nodes[1].ChildNodes[4].InnerText,
                FullName = nodes[1].ChildNodes[5].InnerText,

            };
        }
    }
}
