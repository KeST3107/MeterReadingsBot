using HtmlAgilityPack;

namespace MeterReadingsBot.Interfaces;

/// <summary>
///     Определяет парсер html страниц.
/// </summary>
public interface IHtmlParserService
{
    #region Overridable
    /// <summary>
    ///     Получает записи таблиц страницы сайта.
    /// </summary>
    /// <param name="html">Тело страницы.</param>
    /// <returns>Комбинированная коллекция таблиц.</returns>
    public HtmlNodeCollection GetReadingsNodes(string html);
    #endregion
}
