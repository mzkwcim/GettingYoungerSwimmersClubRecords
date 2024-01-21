using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingYoungerSwimmersClubRecords
{
    internal class DataFormatingSystem
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
}
