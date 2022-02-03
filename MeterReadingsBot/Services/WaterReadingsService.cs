namespace MeterReadingsBot.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using MeterReadingsBot.Interfaces;
    using MeterReadingsBot.Models;

    public class WaterReadingsService : IWaterReadingsService
    {
        private readonly IHttpService _httpService;
        private readonly IRequestProvider _requestProvider;
        private readonly IHtmlParserService _htmlParserService;

        public WaterReadingsService(IHttpService httpService, IRequestProvider requestProvider, IHtmlParserService htmlParserService)
        {
            _httpService = httpService;
            _requestProvider = requestProvider;
            _htmlParserService = htmlParserService;
        }

        public async Task<HttpResponseMessage> SendReadingsAsync(int personalNumber, int hot)
        {
            var request = _requestProvider.SendReadings(personalNumber, hot);
            var response = await _httpService.PostAsync(request.Item1, request.Item2);
            return response;
        }

        public async Task<ClientInfoDto> GetClientInfo(int personalNumber)
        {
            var response = await GetInformationAsync(personalNumber);
            var stringHtml = response.Content.ReadAsStringAsync().Result;
            if (stringHtml.Contains("Номер не найден")) return null;
            var nodes = _htmlParserService.GetNodes(stringHtml);
            return new ClientInfoDto
            {
                Address = nodes[1].ChildNodes[4].InnerText,
                FullName = nodes[1].ChildNodes[5].InnerText
            };
        }

        public async Task<HttpResponseMessage> GetInformationAsync(int personalNumber)
        {
            var request = _requestProvider.GetInformation(personalNumber);
            var response = await _httpService.PostAsync(request.Item1, request.Item2);
            return response;
        }
    }
}
