using HtmlAgilityPack;

namespace MeterReadingsBot.Interfaces
{
    public interface IHtmlParserService
    {
        public HtmlNodeCollection GetReadingsNodes(string html);
    }
}
