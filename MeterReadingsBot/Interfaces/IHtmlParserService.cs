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
    /// <param name="xPath">Параметр фильтрования.</param>
    /// <returns>Комбинированная коллекция таблиц.</returns>
    public HtmlNodeCollection GetReadingsNodes(string html, string xPath);
    #endregion
}
