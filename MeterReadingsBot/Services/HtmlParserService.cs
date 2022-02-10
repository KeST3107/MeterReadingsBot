using HtmlAgilityPack;
using MeterReadingsBot.Interfaces;

namespace MeterReadingsBot.Services
{
    public class HtmlParserService : IHtmlParserService
    {
        public HtmlNodeCollection GetReadingsNodes(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectSingleNode("//table[@bordercolor='darkblue']").SelectNodes("tr|th");
            return nodes;
        }
    }
}
