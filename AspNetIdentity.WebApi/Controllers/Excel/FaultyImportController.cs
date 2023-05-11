using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Excel
{
    [Authorize]
    [RoutePrefix("api/faultyimport")]
    public class FaultyImportController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region API To Get Faulty Import Data Group In Employee  
        /// <summary>
        /// Created By Harshit Mitra On 19-09-2022
        /// API >> Get >> api/faultyimport/getemployeefaultyimport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeefaultyimport")]
        public async Task<ResponseBodyModel> GetEmployeeFaultyImport()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faultyReport = await _db.EmployeeFaultyLogGroups.Where(x => x.CompanyId ==
                        claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                if (faultyReport.Count > 0)
                {
                    var employeeList = await _db.Employee.Where(x => x.CompanyId ==
                        claims.companyId).Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName,

                        }).ToListAsync();
                    faultyReport.ForEach(x => x.CreatedByName = employeeList.Where(y => y.EmployeeId == x.CreatedBy).Select(y => y.DisplayName).FirstOrDefault());
                    res.Message = "Faulty Employees Reports";
                    res.Status = true;
                    res.Data = faultyReport;
                }
                else
                {
                    res.Message = "No Faulty Employees Imported";
                    res.Status = false;
                    res.Data = faultyReport;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region API To Get Faulty Import Data Group In Employee  
        /// <summary>
        /// Created By Harshit Mitra On 19-09-2022
        /// API >> Get >> api/faultyimport/getemployeefaultyimportdata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeefaultyimportdata")]
        public async Task<ResponseBodyModel> GetEmployeeFaultyImport(Guid groupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faultyReport = await _db.EmployeeFaultyLogs.Include("Group").Where(x => x.Group.GroupId == groupId).ToListAsync();
                if (faultyReport.Count > 0)
                {
                    res.Message = "Faulty Employees Reports";
                    res.Status = true;
                    res.Data = faultyReport;
                }
                else
                {
                    res.Message = "No Faulty Employees Imported";
                    res.Status = false;
                    res.Data = faultyReport;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion
    }
}
