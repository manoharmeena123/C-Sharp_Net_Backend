using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface.SubProject;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.Core.ViewModel.SubProjectViewModel.ResponseProjectViewModel;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;
using Req = AspNetIdentity.Core.ViewModel.SubProjectViewModel.RequestProjectViewModel;
using Res = AspNetIdentity.Core.ViewModel.SubProjectViewModel.ResponseProjectViewModel;

namespace AspNetIdentity.WebApi.Services.SubProject
{
    /// <summary>
    /// Created By Ravi Vyas on 03-04-2023
    /// </summary>
    public class SubProjectService : ISubProjectService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly Logger _logger;
        #endregion

        #region Constructor
        public SubProjectService()
        {
            _context = new ApplicationDbContext();
            _logger = LogManager.GetCurrentClassLogger();
        }
        #endregion

        #region Methods

        #region Api for Get Project And SubProjects 
        /// <summary>
        /// Created By Ravi Vyas on 03/04/2023
        /// Api>>Get>> api/newsubproject/getprojectsubprojectdata
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.SubModelResponse>>> GetProjectAndSubProject(ClaimsHelperModel tokenData, int projectId)
        {
            try
            {
                List<Res.SubModelResponse> getData =
                    await (from p in _context.ProjectLists
                           where p.IsActive && !p.IsDeleted && p.CompanyId == tokenData.companyId &&
                            p.SubProjectId == projectId
                           select new Res.SubModelResponse
                           {

                               ProjectId = p.ID,
                               ProjectName = p.ProjectName,
                               ProjectDescription = p.ProjectDiscription,
                               ResourceCount = _context.AssignProjects.Where(s => s.ProjectId == p.ID).Count(),
                               ManagerName = _context.Employee.Where(e => e.EmployeeId == p.ProjectManager).Select(e => e.DisplayName).FirstOrDefault(),
                               ProjectTypeName = p.LeadType,
                               CreatedByName = _context.Employee.Where(e => e.EmployeeId == p.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                               UpdatedByName = _context.Employee.Where(e => e.EmployeeId == p.UpdatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                               CreatedDate = p.CreatedOn,
                               ProjectStatus = p.ProjectStatus.ToString(),
                               TopProjectId = p.SubProjectId
                           })
                           .ToListAsync();

                if (getData.Count == 0)
                    return new ServiceResponse<List<Res.SubModelResponse>>(HttpStatusCode.NoContent, getData, false);
                return new ServiceResponse<List<Res.SubModelResponse>>(HttpStatusCode.OK, getData);

            }
            catch (Exception ex)
            {

                _logger.Error("API : api/newsubproject/getprojectsubprojectdata | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }

        #endregion

        #region Api for Get Assign Project And Sub Project of Login Employee
        /// <summary>
        /// API>> GET>> api/newsubproject/getassignprojectsubproject
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>     
        public async Task<ServiceResponse<List<Res.ResponseProjectSubProject>>> GetProjectSubProjectDashboard(ClaimsHelperModel tokenData)
        {
            try
            {
                List<Res.ResponseProjectSubProject> getData = new List<ResponseProjectSubProject>();

                if (tokenData.IsAdminInCompany)
                {
                    getData =
                           await (from p in _context.ProjectLists
                                 //join ap in _context.AssignProjects on p.ID equals ap.ProjectId
                                 /*where ap.IsActive && !ap.IsDeleted &&*/
                             where p.ProjectStatus == ProjectStatusConstants.Live &&
                             p.CompanyId == tokenData.companyId && p.IsActive && !p.IsDeleted
                             select new Res.ResponseProjectSubProject
                             {
                                 ProjectId = p.ID,
                                 ProjectName = p.ProjectName,
                                 ProjectDescription = p.ProjectDiscription,
                                 IsTaskCreate = _context.TaskPermissions.Where(x => x.ProjectId == p.ID && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsCreateTask).FirstOrDefault(),
                                 IsApproved = _context.TaskPermissions.Where(x => x.ProjectId == p.ID && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsApprovedTask).FirstOrDefault(),
                                 ProjectEmployeeData = (from a in _context.AssignProjects
                                                        join r in _context.EmployeeRoleInProjects on a.EmployeeRoleInProjectId equals r.EmployeeRoleInProjectId
                                                        into q
                                                        from result in q.DefaultIfEmpty()
                                                        join emp in _context.Employee on a.EmployeeId equals emp.EmployeeId
                                                        where a.ProjectId == p.ID && !a.IsProjectManager && a.IsActive && !a.IsDeleted
                                                        select new Res.ResponseMainProjectEmployeeData
                                                        {
                                                            EmployeeId = a.EmployeeId,
                                                            FullName = emp.DisplayName,
                                                            RoleName = result == null ? null : result.RoleName,

                                                        })
                                                        .Distinct()
                                                        .ToList(),
                                 ProjectManagerName = _context.Employee.Where(pm => pm.EmployeeId == p.ProjectManager).Select(pm => pm.DisplayName).FirstOrDefault(),
                                 HasSubProject = (p.SubProjectId == 0),
                                 TopProjectId = p.SubProjectId,
                             })
                             .ToListAsync();

                }
                else
                {
                    getData =
                            await (from p in _context.ProjectLists
                                   join ap in _context.AssignProjects on p.ID equals ap.ProjectId
                                   where p.IsActive && !p.IsDeleted && p.ProjectStatus == ProjectStatusConstants.Live
                                   && p.CompanyId == tokenData.companyId && ap.EmployeeId == tokenData.employeeId && ap.IsActive && !ap.IsDeleted
                                   select new Res.ResponseProjectSubProject
                                   {
                                       ProjectId = p.ID,
                                       ProjectName = p.ProjectName,
                                       ProjectDescription = p.ProjectDiscription,
                                       IsTaskCreate = _context.TaskPermissions.Where(x => x.ProjectId == p.ID && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsCreateTask).FirstOrDefault(),
                                       IsApproved = _context.TaskPermissions.Where(x => x.ProjectId == p.ID && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsApprovedTask).FirstOrDefault(),
                                       ProjectEmployeeData = (from a in _context.AssignProjects
                                                              join r in _context.EmployeeRoleInProjects on a.EmployeeRoleInProjectId equals r.EmployeeRoleInProjectId
                                                              into q
                                                              from result in q.DefaultIfEmpty()
                                                              join emp in _context.Employee on a.EmployeeId equals emp.EmployeeId
                                                              where a.ProjectId == p.ID && !a.IsProjectManager && a.IsActive && !a.IsDeleted
                                                              select new Res.ResponseMainProjectEmployeeData
                                                              {
                                                                  EmployeeId = a.EmployeeId,
                                                                  FullName = emp.DisplayName,
                                                                  RoleName = result == null ? null : result.RoleName,

                                                              })
                                                              .Distinct()
                                                              .ToList(),
                                       ProjectManagerName = _context.Employee.Where(pm => pm.EmployeeId == p.ProjectManager).Select(pm => pm.DisplayName).FirstOrDefault(),
                                       HasSubProject = (p.SubProjectId == 0),
                                       TopProjectId = p.SubProjectId,
                                   })
                                   .ToListAsync();
                }

                var lookup = new Dictionary<int, Res.ResponseProjectSubProject>();
                // actual nested collection to return
                var nested = new List<Res.ResponseProjectSubProject>();
                foreach (Res.ResponseProjectSubProject item in getData)
                {
                    if (lookup.ContainsKey(item.TopProjectId))
                    {
                        // add to the parent's child list 
                        lookup[item.TopProjectId].BaseProject.Add(item);
                    }
                    else
                    {
                        // no parent added yet (or this is the first time)
                        nested.Add(item);
                    }
                    if (!lookup.ContainsKey(item.ProjectId))
                    {
                        lookup.Add(item.ProjectId, item);
                    }

                }

                if (nested.Count == 0)
                    return new ServiceResponse<List<Res.ResponseProjectSubProject>>(HttpStatusCode.NoContent, nested, false);
                return new ServiceResponse<List<ResponseProjectSubProject>>(HttpStatusCode.OK, nested);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/newsubproject/getassignprojectsubproject | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }

        #endregion

        #region Api for Get All Project For Approvel
        /// <summary>
        /// Created By Ravi Vyas On 07-04-2023
        /// API >> GET >> api/newsubproject/getallassignprojectforapprovel
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.ResponseProjectListForApproval>>> GetAllProjectForApproval(ClaimsHelperModel tokenData)
        {
            try
            {
                List<Res.ResponseProjectListForApproval> getData =
                    await (from p in _context.ProjectLists
                           join ap in _context.AssignProjects on p.ID equals ap.ProjectId
                           where p.IsActive && !p.IsDeleted && ap.EmployeeId == tokenData.employeeId
                           select new Res.ResponseProjectListForApproval
                           {
                               ProjectId = ap.ProjectId,
                               ProjectName = p.ProjectName,
                               IsApproved = _context.TaskPermissions.Where(x => x.ProjectId == ap.ProjectId && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsApprovedTask).FirstOrDefault(),
                               IsTaskCreate = _context.TaskPermissions.Where(x => x.ProjectId == ap.ProjectId && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsCreateTask).FirstOrDefault(),
                               TotalPendingTask = _context.TaskApprovels.
                                              Count(a => a.ProjectId == ap.ProjectId && a.IsActive &&
                                              !a.IsDeleted && a.IsApproved == false &&
                                              a.IsRe_Evaluate == false && a.CompanyId == tokenData.companyId),
                           })
                            .Distinct()
                            .ToListAsync();
                if (getData.Count == 0)
                    return new ServiceResponse<List<Res.ResponseProjectListForApproval>>(HttpStatusCode.NoContent, getData, false);
                return new ServiceResponse<List<ResponseProjectListForApproval>>(HttpStatusCode.OK, getData);

            }
            catch (Exception ex)
            {

                _logger.Error("API : api/newsubproject/getallassignprojectforapprovel | " +
                  "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }

        #endregion

        #region Api for Add SubProject in MainProject
        /// <summary>
        /// Created By Ravi Vyas on 10-04-2023
        /// API >> GET >> api/newsubproject/updateprojectinsubproject
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<Req.RequestForUpdateProjectInMainProject>> UpdateProjectInSubProject(ClaimsHelperModel tokenData, Req.RequestForUpdateProjectInMainProject model)
        {
            try
            {

                foreach (var item in model.SubProjectId)
                {

                    var checkData = _context.ProjectLists.Where(x => x.CompanyId == tokenData.companyId && x.ID == item).FirstOrDefault();
                    if (checkData != null)
                    {
                        checkData.SubProjectId = model.ProjectId;
                        checkData.UpdatedBy = tokenData.employeeId;
                        checkData.UpdatedOn = DateTime.Now;
                        _context.Entry(checkData).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                return new ServiceResponse<Req.RequestForUpdateProjectInMainProject>(HttpStatusCode.Accepted, model, true);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/newsubproject/updateprojectinsubproject | " +
                                  "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }

        #endregion  

        #endregion 

    }
}