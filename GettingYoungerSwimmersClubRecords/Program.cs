using HtmlAgilityPack;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
class MainC
{
    public static void Main(string[] args)
    {
        string[] ar = Scraper.URL("https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65773&gender=1&course=SCM&agegroup=11&stroke=0&season=-1");
        foreach (string s in ar)
        {
            GetYoungerOne(s);
        }
    }
    public static void GetYoungerOne(string s)
    {
        ConsoleWritter.WriterLoop(s, ListGetter.BirthDates(s), ListGetter.EstablishmentDate(s));
    }

}
class ConsoleWritter
{
    public static void WriterLoop(string s, List<string> birthDates, List<string> establishmentDates)
    {
        var htmlDocument = Scraper.Loader(s);
        for (int i = 0; i < establishmentDates.Count; i++)
        {
            if (Convert.ToInt32(establishmentDates[i]) - Convert.ToInt32(birthDates[i]) == 10)
            {
                string name = Format.ToTitleString(htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']")[i].InnerText);
                string time = htmlDocument.DocumentNode.SelectNodes("//td[@class='time']")[i].InnerText;
                string dystans = Format.StrokeTranslation(htmlDocument.DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText);
                string data = Format.DateTranslation(htmlDocument.DocumentNode.SelectNodes("//td[@class='date']")[i].InnerText);
                string miasto = htmlDocument.DocumentNode.SelectNodes("//td[@class='city']")[i].InnerText;
                Console.WriteLine(dystans + " " + name + " " + time + " " + data + " " + miasto);
                break;
            }
        }
    }
}
class ListGetter
{
    public static List<string> BirthDates(string s)
    {
        var birthDate = Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='rankingPlace']");
        List<string> birthDates = new List<string>();
        for (int i = 0; i < birthDate.Count; i++)
        {
            string date = Sanitizer.DotsSanitizer(Sanitizer.TextSanitizer(birthDate[i].InnerText));
            if (!String.IsNullOrEmpty(date))
            {
                birthDates.Add(date);
            }
        }
        return birthDates;
    }
    public static List<string> EstablishmentDate(string s)
    {
        var establishmentDate = Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='date']");
        List<string> establishmentDates = new List<string>();
        for (int i = 0; i < establishmentDate.Count; i++)
        {
            establishmentDates.Add(Sanitizer.DateSanitizer(establishmentDate[i].InnerText));
        }
        return establishmentDates;
    }
}
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
}
