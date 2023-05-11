using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentity.WebApi.Interface
{
    public interface IStoredProcedureService <T>
    {
        /// <summary>
        /// Method To Excute Store Procudure
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<T> ExecWithStoreProcedure(string sql, params object[] parameters);
    }
}
