using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingYoungerSwimmersClubRecords
{
    internal class ListGettingSystem
    {
        public static List<string> BirthDates(string s)
        {
            double places = WebScrapingSystem.GetPlacesModulo25Celling(s);
            List<string> birthDates = new List<string>();
            int counter = 1;
            for (int j = 0; j < places; j++)
            {
                var birthDate = HelperList(counter, s, "//td[@class='rankingPlace']");
                int gotto = Calculator.EndWith(j, places, (counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}"));
                for (int i = 0; i < gotto * 5; i++)
                {
                    string date = DataSanitizingSystem.DotsSanitizer(DataSanitizingSystem.TextSanitizer(birthDate[i].InnerText));
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
            double places = WebScrapingSystem.GetPlacesModulo25Celling(s);
            int counter = 1;
            List<string> establishmentDates = new List<string>();
            for (int j = 0; j < places; j++)
            {
                HtmlAgilityPack.HtmlNodeCollection establishmentDate = HelperList(counter, s, "//td[@class='date']");
                int gotto = Calculator.EndWith(j, places, s);
                for (int i = 0; i < gotto; i++)
                {
                    string date = DataSanitizingSystem.DateSanitizer(establishmentDate[i].InnerText);
                    if (!String.IsNullOrEmpty(date))
                    {
                        establishmentDates.Add(date);
                    }
                }
                counter += 25;
            }
            return establishmentDates;
        }
        public static HtmlAgilityPack.HtmlNodeCollection HelperList(int counter, string s, string helper) => WebScrapingSystem.Loader((counter == 1) ? s.Replace("firstPlace=1", $"firstPlace={counter}") : s.Replace($"firstPlace={counter - 25}", $"firstPlace={counter}")).DocumentNode.SelectNodes(helper);
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
}
