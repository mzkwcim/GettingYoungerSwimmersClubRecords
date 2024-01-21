using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingYoungerSwimmersClubRecords
{
    internal class DataSanitizingSystem
    {
        public static string DateSanitizer(string converter) => converter.Split("&nbsp;")[converter.Split("&nbsp;").Length - 1];
        public static string TextSanitizer(string date) => (!date.Any(Char.IsLetter)) ? date : "";
        public static string DotsSanitizer(string date) => (!date.Contains(".") && !date.Contains("-")) ? date : "";
        public static string LapChecker(string distance) => distance.Contains("Lap") ? distance = "" : distance;
    }
}
