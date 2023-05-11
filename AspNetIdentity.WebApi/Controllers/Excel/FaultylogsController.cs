//using AspNetIdentity.WebApi.Helper;
//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Models;
//using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace AspNetIdentity.WebApi.Controllers.Excel
//{
//    [Authorize]
//    [RoutePrefix("api/FaultyImport")]
//    public class FaultylogsController : ApiController
//    {


//        private readonly ApplicationDbContext _db = new ApplicationDbContext();
//        #region API To Get Employee Faulty Import Data Logs
//        /// <summary>
//        /// Created By Harshit Mitra On 19-09-2022
//        /// API >> Get >> api/assetsnew/getfaultyimportlog
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getemployeefaultyimportlog")]
//        public async Task<ResponseBodyModel> GetEmployeeFaultyImportLogs(Guid groupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var faultyReportLog = await _db.EmployeeImportFaultieLogs.Include("EmployeeGroups")
//                        .Where(x => x.EmployeeGroups.EmployeeGroupId == groupId).ToListAsync();
//                if (faultyReportLog !=  0)
//                {
//                    res.Message = "Employee Faulty Reports";
//                    res.Status = true;
//                    res.Data = faultyReportLog;
//                }
//                else
//                {
//                    res.Message = "No Faulty Employee Imported";
//                    res.Status = false;
//                    res.Data = faultyReportLog;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }
//        #endregion
//    }
//}
