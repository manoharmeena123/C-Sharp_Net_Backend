using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet.NewProject.DashboardList
{
    /// <summary>
    /// Created By Ravi Vyas On 17-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newdashboarddatalist")]
    public class NewProjectDashboardDataController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for get All Dashboard Task List
        /// <summary>
        /// API>>GET>>api/newdashboarddatalist/getalldashboardtasklist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldashboardtasklist")]
        public async Task<IHttpActionResult> GetAllDashboardTaskList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                              .Select(x => new
                              {
                                  TaskId = x.TaskId,
                                  TaskTitle = x.TaskTitle,
                                  Discription = x.Discription,
                                  AssiganEmployeeId = x.AssignEmployeeId,
                                  ProjectId = x.ProjectId,
                                  ProjectName = _db.ProjectLists.Where(p => p.ID == x.ProjectId).Select(p => p.ProjectName).FirstOrDefault(),
                                  AssignEmployeeName = _db.Employee.Where(e => e.EmployeeId == x.AssignEmployeeId).Select(e => e.DisplayName).FirstOrDefault(),

                              }).ToListAsync();
                if (getData.Count > 0)
                {
                    res.Message = "Data Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newdashboarddatalist/getalldashboardtasklist| " +
                              "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region APi for Get All Dashboard Project List

        /// <summary>
        /// API>>GET>>api/newdashboarddatalist/getalldashboardprojectlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldashboardprojectlist")]
        public async Task<IHttpActionResult> GetAllDashboardProjectList(int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await (from ap in _db.AssignProjects
                                     where ap.IsActive && !ap.IsDeleted /*&& ap.CompanyId == tokenData.companyId*/
                                     select new
                                     {
                                         EmployeeId = ap.EmployeeId,
                                         EmployeeName = _db.Employee.Where(x => x.EmployeeId == ap.EmployeeId).Select(x => x.DisplayName).FirstOrDefault(),
                                         AssignProject = _db.ProjectLists.Where(p => p.ID == ap.ProjectId).Select(p => new
                                         {
                                             p.ID,
                                             p.ProjectName,

                                         }).ToList(),

                                     }).ToListAsync();


                if (getData.Count > 0)
                {
                    res.Message = "Data Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = getData.Count,
                            Counts = (int)count,
                            List = getData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                        return Ok(res);
                    }

                    else
                    {
                        res.Data = getData;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newdashboarddatalist/getalldashboardtasklist| " +
                              "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region APi for Get All Dashboard Not Assign Project Employeee List
        ///Created By Ravi Vyas on 17-01-2023
        /// <summary>
        /// API>>GET>>api/newdashboarddatalist/getallnotassinproject
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallnotassinproject")]
        public async Task<IHttpActionResult> GetAllNotAssignProjectList(int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var TotalProjectNotAssign = await (from e in _db.Employee
                                                   join u in _db.User on e.EmployeeId equals u.EmployeeId
                                                   join ap in _db.AssignProjects on e.EmployeeId equals ap.EmployeeId into r
                                                   from empty in r.DefaultIfEmpty()
                                                   where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted && e.EmployeeId != empty.EmployeeId
                                                   select new
                                                   {
                                                       EmployeId = e.EmployeeId,
                                                       EmployeeName = e.DisplayName,
                                                   }).ToListAsync();


                if (TotalProjectNotAssign.Count > 0)
                {
                    res.Message = "Data Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = TotalProjectNotAssign;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = TotalProjectNotAssign;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newdashboarddatalist/getallnotassinproject| " +
                              "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

    }
}
