using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.DashboardController;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet.Project_Related_Work
{
    /// <summary>
    /// Create By Ravi Vyas On 13-03-2023
    /// </summary>

    [Authorize]
    [RoutePrefix("api/subproject")]
    public class SubProjectController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for get All Project And SubProject Api 
        /// <summary>
        ///  API>>GET>>api/subproject/assignprojectsubproject
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("assignprojectsubproject")]
        public async Task<IHttpActionResult> GetProjectAndSubProject()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TotaTaskCountAndEmployeeProjectSubProject response = new TotaTaskCountAndEmployeeProjectSubProject();
            try
            {
                List<GetProjectSubProjectClass> getProjects = new List<GetProjectSubProjectClass>();

                var projectData = await (from p in _db.ProjectLists
                                         join a in _db.AssignProjects on p.ID equals a.ProjectId
                                         //join sp in _db.AssignProjects on p.ID equals sp.SubProjectId
                                         where a.IsActive && !a.IsDeleted && a.CompanyId == tokenData.companyId
                                         where tokenData.IsAdminInCompany ?
                                         a.CompanyId == tokenData.companyId :
                                         a.EmployeeId == tokenData.employeeId
                                         select new GetProjectSubProjectClass
                                         {
                                             Project = p,
                                             AssignProject = a,
                                         })
                                     .ToListAsync();
                getProjects = projectData;


                var projectId = getProjects.Select(x => x.Project.ID).Distinct().ToList();
                var taskPermission = await _db.TaskPermissions.Where(x => projectId.Contains(x.ProjectId)).ToListAsync();
                var finalResponse = projectId.
                                    Select(x => new
                                    {
                                        ProjectId = x,
                                        ProjectName = getProjects.Where(z => z.Project.ID == x && z.Project.SubProjectId == 0).Select(z => z.Project.ProjectName).FirstOrDefault(),
                                        ProjectDiscription = getProjects.Where(z => z.Project.ID == x && z.Project.SubProjectId == 0).Select(z => z.Project.ProjectDiscription).FirstOrDefault(),
                                        IsTaskCreate = taskPermission.Where(z => z.ProjectId == x && z.AssigneEmployeeId == tokenData.employeeId).Select(z => z.IsCreateTask).FirstOrDefault(),
                                        IsApproved = taskPermission.Where(z => z.ProjectId == x && z.AssigneEmployeeId == tokenData.employeeId).Select(z => z.IsApprovedTask).FirstOrDefault(),
                                        ManagerName = _db.AssignProjects.Where(z => z.ProjectId == x && z.IsProjectManager == true)
                                                     .Distinct()
                                                     .Select(z => new
                                                     {

                                                         FullName = _db.Employee.Where(a => a.EmployeeId == z.EmployeeId).Select(a => a.DisplayName).FirstOrDefault(),
                                                     })
                                                     .FirstOrDefault(),

                                        EmployeeName = (from a in _db.AssignProjects
                                                        join r in _db.EmployeeRoleInProjects on a.EmployeeRoleInProjectId equals r.EmployeeRoleInProjectId
                                                        into q
                                                        from result in q.DefaultIfEmpty()
                                                        where a.ProjectId == x && !a.IsProjectManager
                                                        select new EmployeeData1
                                                        {
                                                            EmployeeId = a.EmployeeId,
                                                            FullName = _db.Employee.Where(z => z.EmployeeId == a.EmployeeId).Select(z => z.DisplayName).FirstOrDefault(),
                                                            RoleName = result == null ? null : result.RoleName,
                                                            IsManager = a.IsProjectManager
                                                        })
                                                        .Distinct()
                                                        .ToList(),
                                        SubProject = getProjects.Where(z => z.Project.SubProjectId == x)
                                                               .Select(z => new
                                                               {
                                                                   z.Project.ID,
                                                                   z.Project.ProjectName,
                                                                   z.Project.ProjectDiscription
                                                               })
                                                               .ToList(),

                                    })
                                    .OrderByDescending(x => x.ProjectId)
                                    .ToList();

                response.ProjectResponseModel = finalResponse;
                if (response != null)
                {
                    res.Message = "All Task Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new
                    {
                        IsAdmin = tokenData.IsAdminInCompany,
                        ProjectList = response,
                    };
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Task  Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new
                    {
                        IsAdmin = tokenData.IsAdminInCompany,
                        ProjectList = response,
                    };
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/subproject/assignprojectsubproject | " +
                                           "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class GetProjectSubProjectClass
        {
            public ProjectList Project { get; set; }
            public AssignProject AssignProject { get; set; }
        }
        public class TotaTaskCountAndEmployeeProjectSubProject
        {
            public object ProjectResponseModel { get; set; }
            public int TotalTask { get; set; } = 0;
            public int TotalClosedTask { get; set; } = 0;
        }

        #endregion

        #region Api for get sub Project
        /// <summary>
        /// API>>GET>>api/subproject/getsubproject?projectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsubproject")]
        public async Task<IHttpActionResult> GetSubProject(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.ProjectLists.Where(x => x.SubProjectId == projectId && x.IsActive &&
                                              !x.IsDeleted && x.CompanyId == tokenData.companyId)
                                               .Select(x => new
                                               {
                                                   ProjectId = x.SubProjectId,
                                                   SubjProjectId = x.ID,
                                                   ProjectName = x.ProjectName
                                               })
                                              .ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);

                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/subproject/getsubproject | " +
                              "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api for Get Sub Project By Project Id
        /// <summary>
        /// GET>>api/subproject/apigetsubproject
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("apigetsubproject")]
        public async Task<IHttpActionResult> GetSubProjectById(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.ProjectLists.Where(x => x.IsActive && !x.IsDeleted &&
                                              x.SubProjectId == projectId)
                                             .Select(x => new
                                             {
                                                 SubProjectId = x.ID,
                                                 SubProjectName = x.ProjectName
                                             }).ToListAsync();
                if (getData.Count > 0)
                {
                    res.Message = "Get SubProject Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/subproject/apigetsubproject | " +
                                           "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api for GetProject and SubProject In Dashboard

        //public async Task<IHttpActionResult> GetProjectAndSubProjectDashboard()
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var getProjectSubProject = await _db.ProjectLists.
        //                                      Where(x=>x.IsActive && !x.IsDeleted &&
        //                                      x.CompanyId == tokenData.companyId)
        //                                      .Select(x=> new
        //                                      {

        //                                      }).ToListAsync();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        #endregion

        #region Api for Get All Project Of Company
        /// <summary>
        ///  api/subproject/getallcompanyprojectdata 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcompanyprojectdata")]
        public async Task<IHttpActionResult> GetAllCompanyProjectList()
        {

            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectData = await _db.ProjectLists.
                                            Where(x => x.CompanyId == tokenData.companyId &&
                                            !x.IsDeleted && x.IsActive && x.SubProjectId == 0).
                                            Select
                                            (x => new ProjectListData
                                            {
                                                ProjectId = x.ID,
                                                ProjectName = x.ProjectName
                                            }).
                                            ToListAsync();

                if (projectData.Count > 0)
                {
                    res.Message = "Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = projectData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = projectData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/subproject/getallcompanyprojectdata | " +
                                           "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class ProjectListData
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
        }

        #endregion
    }
}
