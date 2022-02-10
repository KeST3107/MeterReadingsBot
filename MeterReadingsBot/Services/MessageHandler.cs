using System;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Services
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IWaterReadingsService _waterReadingsService;
        private readonly IEmailService _emailService;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(IWaterReadingsService waterReadingsService, IEmailService emailService, ILogger<MessageHandler> logger)
        {
            _waterReadingsService = waterReadingsService ?? throw new ArgumentNullException(nameof(waterReadingsService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<string> Handle(Message message)
        {
            var splitMessage = message.Text.Split();

            if (splitMessage.Length < 3 && splitMessage.Length > 1) return "При вводе данных что-то пропустили!\n";

            int.TryParse(splitMessage[0], out var personalNumber);
            int.TryParse(splitMessage[1], out var coldWater);
            int.TryParse(splitMessage[2], out var hotWater);

            if (personalNumber < 9) return "Неправильный лицевой счет!\n";


            var client = new Client
            {
                PersonalNumber = personalNumber,
                ColdWater = coldWater,
                HotWater = hotWater
            };
            ClientInfoDto clientInfo;
            try
            {
                clientInfo = await _waterReadingsService.GetClientInfoAsync(client.PersonalNumber);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Лицевой счет {PersonalNumber} не найден.", client.PersonalNumber);
                return "Лицевой счет не найден\n";
            }

            client.Address = clientInfo.Address;
            client.FullName = clientInfo.FullName;
            try
            {
                await _waterReadingsService.SendReadingsAsync(client.PersonalNumber, client.HotWater);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось передать показания в ОК и ТС по лицевому счету: {PersonalNumber}, горячая вода: {HotWater}.", client.PersonalNumber, client.HotWater);
                return $"Не удалось передать показания в ОК и ТС по лицевому счету: {client.PersonalNumber}, горячая вода: {client.HotWater}";
            }
            _logger.LogInformation("Показания переданы в ОК и ТС по лицевому счету: {PersonalNumber}, горячая вода: {HotWater}.", client.PersonalNumber, client.HotWater);
            try
            {
                await _emailService.SendMessageAsync(client.Address, client.ColdWater, client.HotWater);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось передать показания в СЕВдту-4 по лицевому счету: {PersonalNumber}, холодная вода: {ColdWater}, горячая вода: {HotWater}.", client.PersonalNumber,client.ColdWater, client.HotWater);
                return $"Не удалось передать показания в СЕВдту-4 по лицевому счету: {client.PersonalNumber}, холодная вода: {client.ColdWater}, горячая вода: {client.HotWater}.";
            }
            _logger.LogInformation("Показания переданы в СЕВдту-4 по лицевому счету: {PersonalNumber}, холодная вода: {ColdWater}, горячая вода: {HotWater}.", client.PersonalNumber,client.ColdWater, client.HotWater);
            return $"{client.FullName}\n" +
                   $"{client.Address}\n" +
                   $"Холодная вода: {coldWater}\n" +
                   $"Горячая вода: {hotWater}\n" +
                   "Показания переданы!";
        }
    }
}
