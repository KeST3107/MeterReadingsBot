﻿using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Exceptions;
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
    private const string NumberNotFined = "Номер не найден";
    private const string PersonnelInformationXPath = "//div[@class='table-responsive']";
    private const string WaterReadingsXPath = "//table[@borderclass='darkblue']//tr//td//div[@class]";
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
    public async Task<ClientDto> GetClientInfoAsync(long personnelNumber, CancellationToken cancellationToken)
    {
        var uri = new Uri(_settings.GetClientUri);
        var content = new StringContent($"nomer={personnelNumber}", Encoding.UTF8, MediaType);
        var response = await _httpClientService.PostAsync(uri, content, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK) throw new TelegramMessageException("Получение данных клиента завершилась некорректно, отправьте ДА, чтобы попробовать еще раз.");
        var stringHtml = response.Content.ReadAsStringAsync()
            .Result;
        if (stringHtml.Contains(NumberNotFined)) throw new TelegramMessageException("Лицевой счет не найден. Введите заново.");
        var personalInfoNodes = _htmlParserService.GetReadingsNodes(stringHtml, PersonnelInformationXPath)[0]
            .ChildNodes[1]
            .InnerText.Split("\n");
        var waterReadingsNodes = _htmlParserService.GetReadingsNodes(stringHtml, WaterReadingsXPath);
        var lastSendDate = GetLastSendDate(waterReadingsNodes);
        if (lastSendDate != DateTime.MinValue)
        {
            var dateTimeNow = DateTime.Now;
            if (lastSendDate.Year == dateTimeNow.Year && lastSendDate.Month == dateTimeNow.Month && lastSendDate.Day <= dateTimeNow.Day)
                throw new TelegramMessageException("Данный клиент уже отправлял показания в этом месяце, попробуйте другой лицевой счет.");

        }
        return new ClientDto(personalInfoNodes[3].TrimStart(' '), personalInfoNodes[4].TrimStart(' '), personalInfoNodes[2].Split(' ')[3], lastSendDate != DateTime.MinValue ? lastSendDate : null);
    }

    /// <inheritdoc />
    public async Task SendWaterReadingsOKiTSAsync(Client clientInfo, CancellationToken cancellationToken)
    {
        var uri = new Uri(_settings.SendReadingsUri);
        var personnelNumber = clientInfo.PersonalNumber;
        var hotWaterBathroom = clientInfo.HotWaterBathroom;
        var contentMessage = clientInfo.HotWaterKitchen == null ? $"nomer={personnelNumber}&0={hotWaterBathroom}" : $"nomer={personnelNumber}&0={hotWaterBathroom}&1={clientInfo.HotWaterKitchen}";
        var content = new StringContent(contentMessage, Encoding.UTF8, MediaType);
        var response = await _httpClientService.PostAsync(uri, content, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK) throw new TelegramMessageException("Отправка показаний завершилась некорректно, попробуйте еще раз.");
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

    private DateTime GetLastSendDate(HtmlNodeCollection waterReadingsNodes)
    {
        var values = waterReadingsNodes.Where(x => x.InnerText.Contains('в'))
            .Select(x =>
            {
                var stringDatetime = x.InnerText.Remove(x.InnerText.IndexOf('в') - 1, 2);
                var success = DateTime.TryParseExact(stringDatetime,
                    "dd.MM.yy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var value);
                ;
                return new
                {
                    success,
                    value
                };
            })
            .Where(x => x.success && x.value < DateTime.Now.AddDays(1))
            .Select(x => x.value)
            .OrderBy(x => x);
        return values
            .FirstOrDefault();
    }

    private string GetRecipientMailByAddress(string clientInfoAddress)
    {
        return clientInfoAddress.ToLower().Contains("выч") ? _settings.DTVSEmail : _settings.KTLSEmail;
    }
    #endregion
}
