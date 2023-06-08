using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Models;
using MeterReadingsBot.Settings;

namespace MeterReadingsBot.Services;

/// <summary>
///     Представляет сервис передачи и получения информации о показаниях воды.
/// </summary>
public class WaterReadingsService : IWaterReadingsService
{
    #region Data
    #region Consts
    private const string MediaType = "application/x-www-form-urlencoded";
    #endregion

    #region Fields
    private readonly IEmailService _emailService;
    private readonly IHtmlParserService _htmlParserService;
    private readonly IHttpClientService _httpClientService;
    private readonly WaterReadingsServiceSettings _settings;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="WaterReadingsService" />
    /// </summary>
    /// <param name="httpClientService">Сервис клиента http.</param>
    /// <param name="htmlParserService">Парсер html страниц.</param>
    /// <param name="emailService">Сервис для работы с email.</param>
    /// <param name="settings">Настройки сервиса.</param>
    /// <exception cref="ArgumentNullException">Если один из параметров не задан.</exception>
    public WaterReadingsService(IHttpClientService httpClientService, IHtmlParserService htmlParserService, IEmailService emailService, WaterReadingsServiceSettings settings)
    {
        _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        _htmlParserService = htmlParserService ?? throw new ArgumentNullException(nameof(htmlParserService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    #endregion

    #region IWaterReadingsService members
    /// <inheritdoc />
    public async Task<ClientDto> GetClientInfoAsync(int personnelNumber)
    {
        var uri = new Uri(_settings.GetClientUri);
        var content = new StringContent($"nomer={personnelNumber}", Encoding.UTF8, MediaType);
        var response = await _httpClientService.PostAsync(uri, content);
        var stringHtml = response.Content.ReadAsStringAsync()
            .Result;
        if (stringHtml.Contains("Номер не найден")) return null;
        var nodes = _htmlParserService.GetReadingsNodes(stringHtml)[0]
            .ChildNodes[1]
            .InnerText.Split("\n");
        return new ClientDto(nodes[3].TrimStart(' '), nodes[4].TrimStart(' '), nodes[2].Split(' ')[3]);
    }

    /// <inheritdoc />
    public async Task<HttpStatusCode> SendWaterReadingsOKiTSAsync(Client clientInfo, CancellationToken cancellationToken)
    {
        var uri = new Uri(_settings.SendReadingsUri);
        var personnelNumber = clientInfo.PersonalNumber;
        var hotWaterBathroom = clientInfo.HotWaterBathroom;
        var contentMessage = clientInfo.HotWaterKitchen == null ? $"nomer={personnelNumber}&0={hotWaterBathroom}" : $"nomer={personnelNumber}&0={hotWaterBathroom}&1={clientInfo.HotWaterKitchen}";
        var content = new StringContent(contentMessage, Encoding.UTF8, MediaType);
        var response = await _httpClientService.PostAsync(uri, content, cancellationToken);
        return response.StatusCode;
    }

    /// <inheritdoc />
    public async Task SendWaterReadingsToGorvodokanalAsync(Client clientInfo)
    {
        var bodyMessage = CreateBodyMessage(clientInfo);
        var recipientMail = GetRecipientMailByAddress(clientInfo.Address);
        await _emailService.SendMessageAsync(clientInfo.Address, bodyMessage, recipientMail);
    }
    #endregion

    #region Private
    private string CreateBodyMessage(Client clientInfo)
    {
        var coldWaterKitchenMessage = clientInfo.ColdWaterKitchen == null ? "" : $"Холодная вода кухня: {clientInfo.ColdWaterKitchen}\n";
        var hotWaterKitchenMessage = clientInfo.HotWaterKitchen == null ? "" : $"Горячая вода кухня: {clientInfo.HotWaterKitchen}\n";
        return "Показания\n" +
               $"Холодная вода санузел: {clientInfo.ColdWaterBathroom}\n" +
               $"Горячая вода санузел: {clientInfo.HotWaterBathroom}\n" +
               coldWaterKitchenMessage +
               hotWaterKitchenMessage +
               "Данное сообщение сформировано автоматически и не требует ответа!";
    }

    private string GetRecipientMailByAddress(string clientInfoAddress)
    {
        return clientInfoAddress.ToLower().Contains("выч") ? _settings.DTVSEmail : _settings.KTLSEmail;
    }
    #endregion
}
