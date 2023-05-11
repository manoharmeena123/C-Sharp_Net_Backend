using AspNetIdentity.WebApi.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetIdentity.WebApi.Helper
{
    public static class CurrencyHelper
    {
        public static List<GetCurrencyList> GetCurrencyName()
        {
            var currencies = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(ci => ci.LCID).Distinct()
            .Select(id => new RegionInfo(id))
            .GroupBy(r => r.ISOCurrencySymbol)
            .Select(g => g.First())
            .OrderBy(x => x.CurrencyEnglishName)
            // .Where(x => x.ISOCurrencySymbol == "INR" || x.CurrencySymbol == "USD") // update By suraj Bundel on27/05/2022
            .Select(r => new GetCurrencyList
            {
                ISOCurrencySymbol = r.ISOCurrencySymbol,
                CurrencyEnglishName = r.CurrencyEnglishName,
                CurrencySymbol = r.CurrencySymbol,
            }).ToList();
            return currencies;
        }

        public static string GetCurrencyNameByIso(string isoCode)
        {
            var currencies = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(ci => ci.LCID).Distinct()
            .Select(id => new RegionInfo(id))
            .GroupBy(r => r.ISOCurrencySymbol)
            .Select(g => g.First())
            .OrderBy(x => x.CurrencyEnglishName)
            .Where(x => x.ISOCurrencySymbol == isoCode) // update By suraj Bundel on27/05/2022
            .Select(r => r.CurrencyEnglishName).FirstOrDefault();
            return currencies;
        }

        public static void CapitalizeFirst()
        {
            string str = "educative";

            if (str.Length == 0)
                System.Console.WriteLine("Empty String");
            else if (str.Length == 1)
                System.Console.WriteLine(char.ToUpper(str[0]));
            else
                System.Console.WriteLine(char.ToUpper(str[0]) + str.Substring(1));
        }


    }
}