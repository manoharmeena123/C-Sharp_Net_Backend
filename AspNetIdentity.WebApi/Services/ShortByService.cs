using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetIdentity.Core.Common
{
    public class ShortByService<T> : IShortByService<T> where T : BaseModelClass
    {
        public void ShortValue(ShortByEnumClass enumEntity, DateTime today, ref List<T> listEntity)
        {
            DateTime oldDate = today;
            switch (enumEntity)
            {
                case ShortByEnumClass.Recently_Updated:
                    oldDate = today.AddDays(-7);
                    break;
                case ShortByEnumClass.Updated_In_Last_1_Month:
                    oldDate = today.AddMonths(-1);
                    break;
                case ShortByEnumClass.Updated_In_Last_3_Months:
                    oldDate = today.AddMonths(-3);
                    break;
                case ShortByEnumClass.Updated_In_Last_6_Months:
                    oldDate = today.AddMonths(-6);
                    break;
                case ShortByEnumClass.Updated_In_Last_1_Year:
                    oldDate = today.AddYears(-1);
                    break;
            }
            listEntity = listEntity
                .Where(x => x.CreatedOn.Date <= today.Date
                    && x.CreatedOn.Date >= oldDate.Date)
                .OrderByDescending(x => x.CreatedOn)
                .ToList();
        }

    }
    public class ShortByEnumService : IShortByEnumService
    {
        public List<ResponseShortByEnumClass> GetEnumList()
        {
            return
                System.Enum.GetValues(typeof(ShortByEnumClass))
                .Cast<ShortByEnumClass>()
                    .Select(x => new ResponseShortByEnumClass
                    {
                        Key = (int)x,
                        Value = x.ToString().Replace("_", " "),
                    })
                    .ToList();
        }
    }
}

