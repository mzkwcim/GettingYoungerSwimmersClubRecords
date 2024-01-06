using HtmlAgilityPack;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Diagnostics.Metrics;
class MainC
{
    public static void Main(string[] args)
    {
        string[] ar = Scraper.URL("https://www.swimrankings.net/index.php?page=rankingDetail&clubId=76965&gender=1&course=SCM&agegroup=11&stroke=0&season=-1");
        foreach (string s in ar)
        {
            GetYoungerOne(s);
        }
    }
    public static void GetYoungerOne(string s)
    {
        string dystans = Sanitizer.LapChecker(Scraper.Loader(s).DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText);
        if (!String.IsNullOrEmpty(dystans))
        {
            ConsoleWritter.AgeGroup(s, ListGetter.BirthDates(s), ListGetter.EstablishmentDate(s), 10);
        }
    }
}
class ConsoleWritter
{
    public static void AgeGroup(string s, List<string> birthDates, List<string> establishmentDates, int age)
    {
        double places = Scraper.GetPlacesModulo25Celling(s);
        int counter = 1;
        int helper = 0;
        for (int j = 0; j < places && helper != 1; j++)
        {
            var htmlDocument = Scraper.Loader((counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
            int gotto = Calculator.EndWith(j, places, s);
            for (int i = 0; i < gotto; i++)
            {
                if (Convert.ToInt32(establishmentDates[i]) - Convert.ToInt32(birthDates[i]) == age)
                {
                    string name = Format.ToTitleString(htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']")[i].InnerText);
                    string time = htmlDocument.DocumentNode.SelectNodes("//td[@class='time']")[i].InnerText;
                    string dystans = Format.StrokeTranslation(htmlDocument.DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText);
                    string data = Format.DateTranslation(htmlDocument.DocumentNode.SelectNodes("//td[@class='date']")[i].InnerText);
                    string miasto = htmlDocument.DocumentNode.SelectNodes("//td[@class='city']")[i].InnerText.Replace("&nbsp;"," ");
                    Console.WriteLine($"{dystans} {name} {time} {data} {miasto}");
                    helper = 1;
                    break;
                }
            }
            counter += 25;
        }
    }
}
class ListGetter
{
    public static List<string> BirthDates(string s)
    {
        double places = Scraper.GetPlacesModulo25Celling(s);
        List<string> birthDates = new List<string>();
        int counter = 1;
        for (int j = 0; j < places; j++)
        {
            var birthDate = HelperList(counter, s, "//td[@class='rankingPlace']");
            int gotto = Calculator.EndWith(j, places, s);
            for (int i = 0; i < gotto*5; i++)
            {
                string date = Sanitizer.DotsSanitizer(Sanitizer.TextSanitizer(birthDate[i].InnerText));
                if (!String.IsNullOrEmpty(date))
                {
                    birthDates.Add(date);
                }
            }
            counter += 25;
        }
        return birthDates;
    }
    public static List<string> EstablishmentDate(string s)
    {
        double places = Scraper.GetPlacesModulo25Celling(s);
        int counter = 1;
        List<string> establishmentDates = new List<string>();
        for (int j = 0; j < places; j++)
        {
            HtmlAgilityPack.HtmlNodeCollection establishmentDate = HelperList(counter, s, "//td[@class='date']");
            int gotto = Calculator.EndWith(j, places, s);
            for (int i = 0; i < gotto; i++)
            {
                string date = Sanitizer.DateSanitizer(establishmentDate[i].InnerText);
                if (!String.IsNullOrEmpty(date))
                {
                    establishmentDates.Add(date);
                }
            }
            counter += 25;
        }
        return establishmentDates;
    }
    public static HtmlAgilityPack.HtmlNodeCollection HelperList(int counter, string s, string helper) => Scraper.Loader((counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}")).DocumentNode.SelectNodes(helper);

}
class Calculator
{
    public static int EndWith(int j, double places, string s) => (j != places - 1) ? 25 : Scraper.GetAbsolutePlaces(s) - (((int)places - 1) * 25);
}
class Scraper
{
    public static double GetPlacesModulo25Celling(string s) => Math.Ceiling(Convert.ToInt32(Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']")[Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']").Count - 1].InnerText.Split(" ")[4]) / 25.0);
    public static int GetAbsolutePlaces(string s) => Convert.ToInt32(Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']")[Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']").Count - 1].InnerText.Split(" ")[4]);
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
class Format
{
    public static string DateTranslation(string converter)
    {
        string date;
        string[] words = converter.Split("&nbsp;");
        if (words.Length == 3)
        {
            switch (words[1])
            {
                case "Jan":
                    date = words[0] + " Stycznia " + words[2];
                    return date;
                case "Feb":
                    date = words[0] + " Lutego " + words[2];
                    return date;
                case "Mar":
                    date = words[0] + " Marca " + words[2];
                    return date;
                case "Apr":
                    date = words[0] + " Kwietnia " + words[2];
                    return date;
                case "May":
                    date = words[0] + " Maja " + words[2];
                    return date;
                case "Jun":
                    date = words[0] + " Czerwca " + words[2];
                    return date;
                case "Jul":
                    date = words[0] + " Lipca " + words[2];
                    return date;
                case "Aug":
                    date = words[0] + " Sierpnia " + words[2];
                    return date;
                case "Sep":
                    date = words[0] + " Września " + words[2];
                    return date;
                case "Oct":
                    date = words[0] + " Października " + words[2];
                    return date;
                case "Nov":
                    date = words[0] + " Listopada " + words[2];
                    return date;
                case "Dec":
                    date = words[0] + " Grudnia " + words[2];
                    return date;
                default:
                    return "";
            }
        }
        else
        {
            switch (words[2])
            {
                case "Jan":
                    date = words[1] + " Stycznia " + words[3];
                    return date;
                case "Feb":
                    date = words[1] + " Lutego " + words[3];
                    return date;
                case "Mar":
                    date = words[1] + " Marca " + words[3];
                    return date;
                case "Apr":
                    date = words[1] + " Kwietnia " + words[3];
                    return date;
                case "May":
                    date = words[1] + " Maja " + words[3];
                    return date;
                case "Jun":
                    date = words[1] + " Czerwca " + words[3];
                    return date;
                case "Jul":
                    date = words[1] + " Lipca " + words[3];
                    return date;
                case "Aug":
                    date = words[1] + " Sierpnia " + words[3];
                    return date;
                case "Sep":
                    date = words[1] + " Września " + words[3];
                    return date;
                case "Oct":
                    date = words[1] + " Października " + words[3];
                    return date;
                case "Nov":
                    date = words[1] + " Listopada " + words[3];
                    return date;
                case "Dec":
                    date = words[1] + " Grudnia " + words[3];
                    return date;
                default:
                    return "";
            }
        }
    }
    public static string StrokeTranslation(string distance)
    {
        try
        {
            string stroke;
            string[] words = distance.Split(' ');
            switch (words[1])
            {
                case "Freestyle":
                    stroke = words[0] + " Dowolnym";
                    return stroke;
                case "Backstroke":
                    stroke = words[0] + " Grzbietowym";
                    return stroke;
                case "Breaststroke":
                    stroke = words[0] + " Klasycznym";
                    return stroke;
                case "Butterfly":
                    stroke = words[0] + " Motylkowym";
                    return stroke;
                case "Medley":
                    stroke = words[0] + " Zmiennym";
                    return stroke;
                default:
                    return "";
            }
        }
        catch
        {
            return "";
        }
    }
    public static string ToTitleString(string fullname)
    {
        string[] dividedFullName = fullname.Replace(",", "").Split(' ');
        string[] newFullName = new string[dividedFullName.Length];
        for (int i = 0; i < dividedFullName.Length; i++)
        {
            int adder = 0;
            foreach (char c in dividedFullName[i])
            {
                newFullName[i] += (adder == 0) ? c.ToString().ToUpper() : c.ToString().ToLower();
                adder++;
            }
        }
        string newFullName2 = (string.Join(" ", newFullName)).Replace(",", "");
        return newFullName2;
    }
}
class Sanitizer
{
    public static string DateSanitizer(string converter) => converter.Split("&nbsp;")[converter.Split("&nbsp;").Length - 1];
    public static string TextSanitizer(string date) => (!date.Any(Char.IsLetter)) ? date : "";
    public static string DotsSanitizer(string date) => (!date.Contains(".") && !date.Contains("-")) ? date : "";
    public static string LapChecker(string distance) => distance.Contains("Lap") ? distance="" : distance;
}
