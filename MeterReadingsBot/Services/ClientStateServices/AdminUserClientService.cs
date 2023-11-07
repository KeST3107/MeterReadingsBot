using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Enums;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Repositories;
using MeterReadingsBot.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
///     Представляет сервис взаимодействия суперадминов.
/// </summary>
public class AdminUserClientService : UserClientServiceBase, IUserClientService
{
    #region Data
    #region Fields
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IPromotionService _promotionService;
    private readonly IStartUserClientRepository _startUserClientRepository;
    private readonly AdminUserSettings _settings;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="AdminUserClientService" />.
    /// </summary>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <param name="adminUserRepository">Репозиторий суперадминов.</param>
    /// <param name="botClient">Клиент телеграм бота.</param>
    /// <param name="adminUserSettings">Настройки сервиса.</param>
    /// <param name="promotionService">Сервис массовой рассылки.</param>
    public AdminUserClientService(IStartUserClientRepository startUserClientRepository,
        IAdminUserRepository adminUserRepository,
        ITelegramBotClient botClient,
        AdminUserSettings adminUserSettings,
        IPromotionService promotionService) : base(startUserClientRepository, botClient)
    {
        _startUserClientRepository = startUserClientRepository ?? throw new ArgumentNullException(nameof(startUserClientRepository));
        _adminUserRepository = adminUserRepository ?? throw new ArgumentNullException(nameof(adminUserRepository));
        _settings = adminUserSettings ?? throw new ArgumentNullException(nameof(adminUserSettings));
        _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
    }
    #endregion

    #region IAdminUserClientService members
    /// <inheritdoc />
    public Task<Message> GetUserTaskMessage(Message message, CancellationToken cancellationToken)
    {
        var chatMessage = message.Text.Split(' ').First();
        if (chatMessage == ReturnAnswer)
        {
            ResetUser(message.Chat.Id);
        }
        var adminUserClient = _adminUserRepository.FindBy(message.Chat.Id);
        if (adminUserClient == null) return Usage(message, cancellationToken);
        return adminUserClient.AdminUserState switch
        {
            AdminUserState.Start => GetStartUserTaskMessageAsync(message, cancellationToken),
            AdminUserState.Commands => GetCommandsMessage(adminUserClient, message, cancellationToken),
            AdminUserState.AddAdmin => GetAddAdminMessageAsync(adminUserClient, message, cancellationToken),
            AdminUserState.RemoveAdmin => GetRemoveAdminMessageAsync(adminUserClient, message, cancellationToken),
            AdminUserState.Promotion => GetPromotionMessageAsync(adminUserClient, message, cancellationToken),
            _ => Usage(message, cancellationToken)
        };
    }

    private void ResetUser(long chatId)
    {
        var adminUser = _adminUserRepository.FindBy(chatId);
        adminUser.AdminUserState = AdminUserState.Start;
        _adminUserRepository.Update(adminUser);
    }

    private async Task<Message> GetPromotionMessageAsync(AdminUserClient adminUserClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var promotionMessage = message.Text;
        adminUserClient.AdminUserState = AdminUserState.Commands;
        _promotionService.StartPromotion(promotionMessage, cancellationToken, "от администратора.");
        return await TelegramBotClient.SendTextMessageAsync(chatId, "Рассылка запущена.", cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Message> GetStartUserTaskMessageAsync(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var userName = message.Chat.Username ?? chatId.ToString();
        var adminIds = _adminUserRepository.GetAll().Select(x => x.ChatId);
        if (_settings.AdminChatId != chatId && !adminIds.Contains(chatId))
            return await Usage(message, cancellationToken);


        var adminUserClient = _adminUserRepository.FindBy(chatId) ?? _adminUserRepository.Add(new AdminUserClient(chatId, userName));
        adminUserClient.UserName = userName;
        adminUserClient.AdminUserState = AdminUserState.Commands;
        _adminUserRepository.Update(adminUserClient);
        SetStartUserToAdminUser(chatId);

        var commandsMessage = "Команды администратора бота:\n" +
                              "/add - добавляет нового админа\n" +
                              "/remove - удаляет админа\n" +
                              "/admins - возвращает список админов\n" +
                              "/users - возвращает количество пользователей\n" +
                              "/promotion - настраивает и запускает рассылку\n" +
                              "/exit - выйти с админ режима\n";

        return await TelegramBotClient.SendTextMessageAsync(chatId, commandsMessage, cancellationToken: cancellationToken);
    }
    #endregion

    #region Private
    private async Task<Message> GetAddAdminMessageAsync(AdminUserClient adminUserClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var isConvertible = long.TryParse(message.Text, out var adminId);
        if (isConvertible is false) return await TelegramBotClient.SendTextMessageAsync(chatId, "Введено недопустимое значение.", cancellationToken: cancellationToken);
        var adminUser = _adminUserRepository.FindBy(adminId);
        if (adminUser != null)
        {
            return await TelegramBotClient.SendTextMessageAsync(chatId, "Пользователь уже существует.\n" +
                                                                        "Введите заново.", cancellationToken: cancellationToken);
        }
        _adminUserRepository.Add(new AdminUserClient(adminId, null));
        adminUserClient.AdminUserState = AdminUserState.Commands;
        _adminUserRepository.Update(adminUserClient);
        return await TelegramBotClient.SendTextMessageAsync(chatId, $"Администратор с id: {adminId} добавлен!", cancellationToken: cancellationToken);
    }

    private Task<Message> GetAdminUsersMessage(long chatId, CancellationToken cancellationToken)
    {
        var admins = _adminUserRepository.GetAll();
        var message = "Колчество админов: " + admins.Count;
        foreach (var admin in admins) message += $"\n@{admin.UserName ?? "id" + admin.ChatId.ToString()}";
        return TelegramBotClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }

    private Task<Message> GetCommandsMessage(AdminUserClient adminUserClient, Message message, CancellationToken cancellationToken)
    {
        var chatMessage = message.Text.Split(' ').First();
        var chatId = message.Chat.Id;
        return chatMessage switch
        {
            "/add" => GetStartAddAdminMessageAsync(adminUserClient, chatId, cancellationToken),
            "/remove" => GetStartRemoveAdminMessageAsync(adminUserClient, chatId, cancellationToken),
            "/admins" => GetAdminUsersMessage(chatId, cancellationToken),
            "/users" => TelegramBotClient.SendTextMessageAsync(chatId, "Колчество пользователей: " + _startUserClientRepository.GetAll().Count, cancellationToken: cancellationToken),
            "/promotion" => GetStartPromotionMessageAsync(adminUserClient, chatId, cancellationToken),
            "/exit" => GetExitAdminMessageAsync(adminUserClient, chatId, cancellationToken),
            _ => Usage(message, cancellationToken)
        };
    }

    private async Task<Message> GetStartPromotionMessageAsync(AdminUserClient adminUserClient, long chatId, CancellationToken cancellationToken)
    {
        adminUserClient.AdminUserState = AdminUserState.Promotion;
        _adminUserRepository.Update(adminUserClient);
        return await TelegramBotClient.SendTextMessageAsync(chatId, "Введите сообщение рассылки.", cancellationToken: cancellationToken);
    }

    private async Task<Message> GetExitAdminMessageAsync(AdminUserClient adminUserClient, long chatId, CancellationToken cancellationToken)
    {
        adminUserClient.AdminUserState = AdminUserState.Start;
        _adminUserRepository.Update(adminUserClient);
        SetStartUserToDefault(adminUserClient.ChatId);

        return await TelegramBotClient.SendTextMessageAsync(chatId,
            "Произведен выход из режима админа.",
            cancellationToken: cancellationToken);
    }

    private async Task<Message> GetRemoveAdminMessageAsync(AdminUserClient adminUserClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var isConvertible = long.TryParse(message.Text, out var adminId);
        if (isConvertible is false) return await TelegramBotClient.SendTextMessageAsync(chatId, "Введено недопустимое значение.", cancellationToken: cancellationToken);

        var adminUser = _adminUserRepository.FindBy(adminId);
        if (adminUser == null) return await TelegramBotClient.SendTextMessageAsync(chatId, $"Не найден администратор с id: {adminId}.", cancellationToken: cancellationToken);
        _adminUserRepository.Remove(adminUser);
        adminUserClient.AdminUserState = AdminUserState.Commands;
        return await TelegramBotClient.SendTextMessageAsync(chatId, $"Администратор с id: {adminId} удален!", cancellationToken: cancellationToken);
    }


    private async Task<Message> GetStartAddAdminMessageAsync(AdminUserClient adminUserClient, long chatId, CancellationToken cancellationToken)
    {
        if (chatId != _settings.AdminChatId) return await NotEnoughRightsMessageAsync(chatId, cancellationToken);
        adminUserClient.AdminUserState = AdminUserState.AddAdmin;
        _adminUserRepository.Update(adminUserClient);
        return await TelegramBotClient.SendTextMessageAsync(chatId,
            "Введите идентификатор админа для добавления\n" +
            "Например: 294435946",
            cancellationToken: cancellationToken);
    }

    private async Task<Message> GetStartRemoveAdminMessageAsync(AdminUserClient adminUserClient, long chatId, CancellationToken cancellationToken)
    {
        if (chatId != _settings.AdminChatId) return await NotEnoughRightsMessageAsync(chatId, cancellationToken);
        adminUserClient.AdminUserState = AdminUserState.RemoveAdmin;
        return await TelegramBotClient.SendTextMessageAsync(chatId,
            "Введите идентификатор админа для удаления\n" +
            "Например: 294435946",
            cancellationToken: cancellationToken);
    }

    private async Task<Message> NotEnoughRightsMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        const string usage = "Недостаточно прав для данного действия :(";

        return await TelegramBotClient.SendTextMessageAsync(
            chatId,
            usage,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
    #endregion
}
