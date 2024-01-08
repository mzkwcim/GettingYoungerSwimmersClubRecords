using HtmlAgilityPack;
Loops.MainLoop();
class Scraper
{
    public static double GetPlacesModulo25Celling(string s) => Math.Ceiling(Convert.ToInt32(Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']")[Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']").Count - 1].InnerText.Split(" ")[4]) / 25.0);
    public static int GetAbsolutePlaces(string s) => Convert.ToInt32(Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']")[Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='navigation']").Count - 1].InnerText.Split(" ")[4]);
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
class Loops
{
    public static void MainLoop()
    {
        GenderLoop("https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65774&gender=1&season=-1&course=SCM&stroke=0&agegroup=0");
    }
    public static void GenderLoop(string url)
    {
        for (int i = 0 ; i < 2 ; i++)
        {
            PoolCourseLoop(url.Replace("gender=1", $"gender={i + 1}"));
        }
    }
    public static void PoolCourseLoop(string url)
    {
        for (int i = 0 ; i < 2 ; i++ )
        {
            LinksLoop((i == 0) ? url : url.Replace("course=SCM", "course=LCM"));
            Console.ReadKey();
        }
    }
    public static void LinksLoop(string url)
    {
        string[] linki = Scraper.URL(url);
        for (int i = 0 ; i < linki.Length ; i++ )
        {
            Console.WriteLine(GetMasterTime(linki[i]));
        }
    }
    public static string GetMasterTime(string singleUrl)
    {
        int escape = 0, inter = 0, helper = 0;
        string master = "";
        string distance = Scraper.Loader(singleUrl).DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText;
        Console.WriteLine(distance);
        if (!distance.Contains("Lap") && !(distance.Split(" ").Length == 4))
        {
            while (escape == 0)
            {
                try
                {
                    master += Scraper.Loader(singleUrl.Replace("firstPlace=1", $"firstPlace={(inter * 25) + 1}")).DocumentNode.SelectSingleNode("//a[@class='time'][sup]").InnerText;
                    if (!String.IsNullOrEmpty(master))
                    {
                        Console.WriteLine("jestem tutaj");
                        var time = Scraper.Loader(singleUrl.Replace("firstPlace=1", $"firstPlace={(inter * 25) + 1}")).DocumentNode.SelectNodes("//td[@class='time']");
                        int gaga = 0;
                        foreach (var c in time)
                        {
                            if (c.InnerText == master)
                            {
                                /*
                                string gosc = Scraper.Loader(singleUrl.Replace("firstPlace=1", $"firstPlace={(inter * 25) + 1}")).DocumentNode.SelectNodes("//td[@class='fullname']")[gaga].InnerText;
                                Console.Write(gosc + " ");
                                */
                                break;
                            }
                            gaga++;
                        }
                        string gosc = Scraper.Loader(singleUrl.Replace("firstPlace=1", $"firstPlace={(inter * 25) + 1}")).DocumentNode.SelectNodes("//td[@class='fullname']")[gaga].InnerText;
                        Console.WriteLine(gaga + " " + gosc);
                        Console.WriteLine(ListGetter.BirthDate(singleUrl.Replace("firstPlace=1", $"firstPlace={(inter * 25) + 1}"), gaga));
                    }
                    escape = 1;
                }
                catch
                {
                    Console.WriteLine("Na tej stronie nie było rekordu");
                    inter++;
                }
                helper++;
            }
        }
        return master;
    }
}
class Calculator
{
    public static int EndWith(int j, double places, string s) => (j != places - 1) ? 25 : Scraper.GetAbsolutePlaces(s) - (((int)places - 1) * 25);
}
class ListGetter
{
    public static int BirthDate(string s, int gaga)
    {
        return Convert.ToInt32(Scraper.Loader(s).DocumentNode.SelectNodes("//td[@class='rankingPlace']")[(gaga*5)-4].InnerText);
    }
    public static List<string> BirthDates(string s)
    {
        double places = Scraper.GetPlacesModulo25Celling(s);
        List<string> birthDates = new List<string>();
        int counter = 1;
        for (int j = 0; j < places; j++)
        {
            var birthDate = HelperList(counter, s, "//td[@class='rankingPlace']");
            int gotto = Calculator.EndWith(j, places, (counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
            for (int i = 0; i < gotto * 5; i++)
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
        for (int i = 0; i < bd.Count; i++)
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
class Sanitizer
{
    public static string DateSanitizer(string converter) => converter.Split("&nbsp;")[converter.Split("&nbsp;").Length - 1];
    public static string TextSanitizer(string date) => (!date.Any(Char.IsLetter)) ? date : "";
    public static string DotsSanitizer(string date) => (!date.Contains(".") && !date.Contains("-")) ? date : "";
    public static string LapChecker(string distance) => distance.Contains("Lap") ? distance = "" : distance;
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
