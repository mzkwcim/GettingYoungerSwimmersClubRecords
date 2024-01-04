using HtmlAgilityPack;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace Swimrankings
{
    class Scraper
    {
        public static HtmlAgilityPack.HtmlDocument Loader(string url)
        {
            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }
        public static void Main(string[] args)
        {
            string[] ar = URL("https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65773&gender=1&course=SCM&agegroup=11&stroke=0&season=-1");
            foreach (string s in ar)
            {
                GetYoungerOne(s);
                Console.ReadKey();
            }
        }
        public static void GetYoungerOne(string s)
        {
            var htmlDocument = Loader(s);
            var fullname = htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']");
            var birthDate = htmlDocument.DocumentNode.SelectNodes("//td[@class='rankingPlace']");
            var establishmentDate = htmlDocument.DocumentNode.SelectNodes("//td[@class='date']");
            List<string> names = new List<string>();
            List<string> birthDates = new List<string>();
            List<string> establishmentDates = new List<string>();
            for (int i = 0; i < birthDate.Count; i++)
            {
                string date = birthDate[i].InnerText;
                date = DotsSanitizer(TextSanitizer(date));
                if (!String.IsNullOrEmpty(date))
                {
                    birthDates.Add(date);
                }
            }
            for (int i = 0; i < establishmentDate.Count; i++)
            {
                string estdate = establishmentDate[i].InnerText;
                estdate = DateTranslation(estdate);
                establishmentDates.Add(estdate);
            }
            for (int i = 0; i < establishmentDates.Count; i++)
            {
                if (Convert.ToInt32(establishmentDates[i]) - Convert.ToInt32(birthDates[i]) == 10)
                {
                    string haba = htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']")[i].InnerText;
                    string time = htmlDocument.DocumentNode.SelectNodes("//td[@class='time']")[i].InnerText;
                    string dystans = htmlDocument.DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText;
                    string data = htmlDocument.DocumentNode.SelectNodes("//td[@class='date']")[i].InnerText.Replace("&nbsp;", " ");
                    string miasto = htmlDocument.DocumentNode.SelectNodes("//td[@class='city']")[i].InnerText;
                    Console.WriteLine(dystans + " " + haba + " " + time + " " + data + " " + miasto);
                    break;
                }
            }

        }
        public static string DateTranslation(string converter) => converter.Split("&nbsp;")[converter.Split("&nbsp;").Length - 1];
        public static string TextSanitizer(string date) => (!date.Any(Char.IsLetter)) ? date : "";
        public static string DotsSanitizer(string date) => (!date.Contains(".") && !date.Contains("-")) ? date : "";
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