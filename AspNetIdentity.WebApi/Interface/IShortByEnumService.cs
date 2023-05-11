using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Model;
using System;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Interface
{
    public interface IShortByService<T> where T : BaseModelClass
    {
        void ShortValue(ShortByEnumClass enumEntity, DateTime today, ref List<T> listEntity);
    }
    public interface IShortByEnumService
    {
        List<ResponseShortByEnumClass> GetEnumList();
    }
}
