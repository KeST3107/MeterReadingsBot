using System.Threading;

namespace MeterReadingsBot.Interfaces;

/// <summary>
///     Представляет сервис массовой рассылки.
/// </summary>
public interface IPromotionService
{
    #region Overridable
    /// <summary>
    ///     Запускает массовую рассылку.
    /// </summary>
    /// <param name="message">Сообщение рассылки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="promotionName">Название рассылки.</param>
    void StartPromotion(string message, CancellationToken cancellationToken, string promotionName = null);
    #endregion
}
