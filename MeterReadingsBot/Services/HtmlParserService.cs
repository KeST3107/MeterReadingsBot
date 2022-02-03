namespace MeterReadingsBot.Services
{
    using HtmlAgilityPack;
    using MeterReadingsBot.Interfaces;

    public class HtmlParserService : IHtmlParserService
    {
        public HtmlNodeCollection GetNodes(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectSingleNode("//table[@bordercolor='darkblue']").SelectNodes("tr|th");
            return nodes;
        }
    }
}
