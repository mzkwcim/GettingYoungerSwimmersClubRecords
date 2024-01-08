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
        if (!Scraper.Loader(s).DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText.Contains("Lap"))
        {
            ConsoleWritter.AgeGroup(s, ListGetter.Age(s), 10);
        }
    }
}
class ConsoleWritter
{
    public static void AgeGroup(string s, List<int> ages, int age)
    {
        double places = Scraper.GetPlacesModulo25Celling(s);
        int counter = 1;
        int helper = 0;
        for (int j = 0; j < places && helper != 1; j++)
        {
            HtmlDocument htmlDocument = Scraper.Loader((counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
            int gotto = Calculator.EndWith(j, places, (counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
            for (int i = 0; i < gotto; i++)
            {
                if (Convert.ToInt32(ages[(j*25)+i]) == age)
                {
                    Scraper.RecordHolder(htmlDocument, i);
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
            int gotto = Calculator.EndWith(j, places, (counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
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
    public static List<int> Age(string s)
    {
        List<string> ed = EstablishmentDate(s);
        List<string> bd = BirthDates(s);
        List<int> age = new List<int>();
        for (int i = 0;i < bd.Count; i++)
        {
            age.Add(Convert.ToInt32(ed[i]) - Convert.ToInt32(bd[i]));
            if (age[i] == 10)
            {
                break;
            }
        }
        return age;
    }

}
class Calculator
{
    public static int EndWith(int j, double places, string s) => (j != places - 1) ? 25 : Scraper.GetAbsolutePlaces(s) - (((int)places - 1) * 25);
}
class Scraper
{
    public static void RecordHolder(HtmlDocument htmlDocument, int i)
    {
        string name = Format.ToTitleString(htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']")[i].InnerText);
        string time = htmlDocument.DocumentNode.SelectNodes("//td[@class='time']")[i].InnerText;
        string dystans = Format.StrokeTranslation(htmlDocument.DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText);
        string data = Format.DateTranslation(htmlDocument.DocumentNode.SelectNodes("//td[@class='date']")[i].InnerText);
        string miasto = htmlDocument.DocumentNode.SelectNodes("//td[@class='city']")[i].InnerText.Replace("&nbsp;", " ");
        Console.WriteLine($"{dystans} {name} {time} {data} {miasto}");
    }
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
        string[] words = converter.Split("&nbsp;");
        return (words.Length == 4) ? $"{words[1]} {GetMonthTranslation(words[2])} {words[3]}" : $"{words[0]} {GetMonthTranslation(words[1])} {words[2]}";
    }
    private static string GetMonthTranslation(string abbreviation)
    {
        switch (abbreviation)
        {
            case "Jan": return "Stycznia";
            case "Feb": return "Lutego";
            case "Mar": return "Marca";
            case "Apr": return "Kwietnia";
            case "May": return "Maja";
            case "Jun": return "Czerwca";
            case "Jul": return "Lipca";
            case "Aug": return "Sierpnia";
            case "Sep": return "Września";
            case "Oct": return "Października";
            case "Nov": return "Listopada";
            case "Dec": return "Grudnia";
            default: return "";
        }
    }
    public static string StrokeTranslation(string distance)
    {
        try
        {
            string[] words = distance.Split(' ');
            switch (words[1])
            {
                case "Freestyle": return words[0] + " Dowolnym";
                case "Backstroke": return words[0] + " Grzbietowym";
                case "Breaststroke": return words[0] + " Klasycznym";
                case "Butterfly": return words[0] + " Motylkowym";
                case "Medley": return words[0] + " Zmiennym";
                default: return "";
            }
        }
        catch
        {
            return "";
        }
    }
    public static string ToTitleString(string fullname)
    {
        string[] words = fullname.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return string.Join(" ", words).Replace(",", "");
    }
}
class Sanitizer
{
    public static string DateSanitizer(string converter) => converter.Split("&nbsp;")[converter.Split("&nbsp;").Length - 1];
    public static string TextSanitizer(string date) => (!date.Any(Char.IsLetter)) ? date : "";
    public static string DotsSanitizer(string date) => (!date.Contains(".") && !date.Contains("-")) ? date : "";
    public static string LapChecker(string distance) => distance.Contains("Lap") ? distance="" : distance;
}
