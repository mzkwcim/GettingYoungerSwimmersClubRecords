using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingYoungerSwimmersClubRecords
{
    internal class WebScrapingSystem
    {
        public static void RecordHolder(HtmlDocument htmlDocument, int i)
        {
            string name = DataFormatingSystem.ToTitleString(htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']")[i].InnerText);
            string time = htmlDocument.DocumentNode.SelectNodes("//td[@class='time']")[i].InnerText;
            string dystans = DataFormatingSystem.StrokeTranslation(htmlDocument.DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText);
            string data = DataFormatingSystem.DateTranslation(htmlDocument.DocumentNode.SelectNodes("//td[@class='date']")[i].InnerText);
            string miasto = htmlDocument.DocumentNode.SelectNodes("//td[@class='city']")[i].InnerText.Replace("&nbsp;", " ");
            Console.WriteLine($"{dystans} {name} {time} {data} {miasto}");
        }
        public static double GetPlacesModulo25Celling(string s) => Math.Ceiling(Convert.ToInt32(WebScrapingSystem.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']")[WebScrapingSystem.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']").Count - 1].InnerText.Split(" ")[4]) / 25.0);
        public static int GetAbsolutePlaces(string s) => Convert.ToInt32(WebScrapingSystem.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']")[WebScrapingSystem.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']").Count - 1].InnerText.Split(" ")[4]);
        public static HtmlAgilityPack.HtmlDocument Loader(string url)
        {
            HttpClient httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url).Result;
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }
        public static string[] URL(string url)
        {
            var URLlink = Loader(url).DocumentNode.SelectNodes("//td[@class='swimstyle']//a[@href]");
            string[] linki = new string[URLlink.Count];
            for (int i = 0; i < URLlink.Count; i++)
            {
                linki[i] = "https://www.swimrankings.net/index.php" + URLlink[i].GetAttributeValue("href", "").Replace("amp;", "");
            }
            return linki;
        }
    }
}
