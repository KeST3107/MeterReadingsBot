namespace MeterReadingsBot.Interfaces
{
    using HtmlAgilityPack;

    public interface IHtmlParserService
    {
        public HtmlNodeCollection GetNodes(string html);
    }
}
