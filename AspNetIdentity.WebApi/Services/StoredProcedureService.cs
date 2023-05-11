using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace AspNetIdentity.WebApi.Services
{
    public class StoredProcedureService<T> : IStoredProcedureService<T>
    {
        private readonly ApplicationDbContext _context;

        public StoredProcedureService()
        {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<T> ExecWithStoreProcedure(string sql, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(sql, parameters);
        }
    }
}