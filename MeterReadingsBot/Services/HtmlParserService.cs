using HtmlAgilityPack;
using MeterReadingsBot.Interfaces;

namespace MeterReadingsBot.Services;

/// <summary>
///     Представляет парсер html страниц.
/// </summary>
public class HtmlParserService : IHtmlParserService
{
    #region IHtmlParserService members
    /// <inheritdoc />
    public HtmlNodeCollection GetReadingsNodes(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var nodes = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']");
        return nodes;
    }
    #endregion
}
