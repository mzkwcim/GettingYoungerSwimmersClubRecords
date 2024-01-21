using HtmlAgilityPack;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Diagnostics.Metrics;
using GettingYoungerSwimmersClubRecords;
class MainC
{
    public static void Main(string[] args)
    {
        string[] ar = WebScrapingSystem.URL("https://www.swimrankings.net/index.php?page=rankingDetail&clubId=76965&gender=1&course=SCM&agegroup=11&stroke=0&season=-1");
        foreach (string s in ar)
        {
            GetYoungerOne(s);
        }
    }
    public static void GetYoungerOne(string s)
    {
        if (!WebScrapingSystem.Loader(s).DocumentNode.SelectSingleNode("//td[@class='titleCenter']").InnerText.Contains("Lap"))
        {
            ConsoleWritter.AgeGroup(s, ListGettingSystem.Age(s), 10);
        }
    }
}
class ConsoleWritter
{
    public static void AgeGroup(string s, List<int> ages, int age)
    {
        double places = WebScrapingSystem.GetPlacesModulo25Celling(s);
        int counter = 1;
        int helper = 0;
        for (int j = 0; j < places && helper != 1; j++)
        {
            HtmlDocument htmlDocument = WebScrapingSystem.Loader((counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
            int gotto = Calculator.EndWith(j, places, (counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
            for (int i = 0; i < gotto; i++)
            {
                if (Convert.ToInt32(ages[(j*25)+i]) == age)
                {
                    WebScrapingSystem.RecordHolder(htmlDocument, i);
                    helper = 1;
                    break;
                }
            }
            counter += 25;
        }
    }
}
class Calculator
{
    public static int EndWith(int j, double places, string s) => (j != places - 1) ? 25 : WebScrapingSystem.GetAbsolutePlaces(s) - (((int)places - 1) * 25);
}
