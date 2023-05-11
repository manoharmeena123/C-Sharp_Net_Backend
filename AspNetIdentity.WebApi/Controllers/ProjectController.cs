using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using AspNetIdentity.WebApi.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/project")]
    public class ProjectController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private TeamService teamService = new TeamService();
        private CategoryService categoryService = new CategoryService();
        private QuestionService questionService = new QuestionService();
        private FeedbackServices feedbackServices = new FeedbackServices();
        private EmployeeService employeeService = new EmployeeService();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Runing Api section of Project Master

        #region This API's is used for Get All Project
        /// <summary>
        /// API >> Get >> api/project/getallproject
        /// Created by shriya ,Created on 19-05-2022
        /// </summary>
        /// <returns></returns>
        [Route("getallproject")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetAllProject()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, claims.TimeZone);
                GetAllProjectHelperModel allproject = new GetAllProjectHelperModel();
                List<ProjectdataList> ProjectList = new List<ProjectdataList>();
                List<GetProjectHelperClass> DataProject = new List<GetProjectHelperClass>();
                if (claims.IsAdminInCompany)
                {
                    DataProject = (from ad in _db.ProjectLists
                                   join fd in _db.Employee on ad.ProjectManager equals fd.EmployeeId
                                   where ad.IsActive == true && ad.IsDeleted == false &&
                                   ad.CompanyId == claims.companyId && ad.SubProjectId == 0
                                   select new GetProjectHelperClass
                                   {
                                       ID = ad.ID,
                                       ProjectName = ad.ProjectName,
                                       Manger = fd.DisplayName,
                                       DisplayName = fd.DisplayName,
                                       ProjectManager = ad.ProjectManager,
                                       EmployeeId = fd.EmployeeId,
                                       ClientBillableAmount = ad.ClientBillableAmount,
                                       CampanyName = ad.CampanyName,
                                       Technology = ad.Technology,
                                       PaymentType = ad.PaymentType,
                                       IsActive = ad.IsActive,
                                       IsDeleted = ad.IsDeleted,
                                       CreatedBy = ad.CreatedBy,
                                       UpdatedBy = ad.UpdatedBy,
                                       LeadType = ad.LeadType,
                                       IsMailSendToday = today.Date == ad.LastMailSendDate,
                                       ProjectStatus = ad.ProjectStatus.ToString(),
                                       ProjectStatusId = ad.ProjectStatus,
                                       ProjectCreatedDate = ad.CreatedOn
                                   }).ToList().OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    DataProject = (from ad in _db.ProjectLists
                                   join fd in _db.Employee on ad.ProjectManager equals fd.EmployeeId
                                   where ad.IsActive == true && ad.IsDeleted == false && ad.ProjectManager == claims.employeeId &&
                                   ad.CompanyId == claims.companyId && ad.SubProjectId == 0
                                   select new GetProjectHelperClass
                                   {
                                       ID = ad.ID,
                                       ProjectName = ad.ProjectName,
                                       Manger = fd.DisplayName,
                                       DisplayName = fd.DisplayName,
                                       ProjectManager = ad.ProjectManager,
                                       EmployeeId = fd.EmployeeId,
                                       ClientBillableAmount = ad.ClientBillableAmount,
                                       CampanyName = ad.CampanyName,
                                       Technology = ad.Technology,
                                       PaymentType = ad.PaymentType,
                                       IsActive = ad.IsActive,
                                       IsDeleted = ad.IsDeleted,
                                       CreatedBy = ad.CreatedBy,
                                       UpdatedBy = ad.UpdatedBy,
                                       LeadType = ad.LeadType,
                                       IsMailSendToday = today.Date == ad.LastMailSendDate,
                                       ProjectStatus = ad.ProjectStatus.ToString(),
                                       ProjectStatusId = ad.ProjectStatus,
                                       ProjectCreatedDate = ad.CreatedOn
                                   }).ToList().OrderByDescending(x => x.ID).ToList();
                }

                var SumofBillableSalary = 0.0;
                var SumofNonBillableSalary = 0.0;
                var BillAndNonBilEmpSalary = 0.0;
                var ClientBillableAmount = 0.0;

                foreach (var item in DataProject)
                {
                    ProjectdataList ProjectListObj = new ProjectdataList();
                    ProjectListObj.ID = item.ID;
                    ProjectListObj.ProjectName = item.ProjectName;
                    ProjectListObj.EmployeeId = item.EmployeeId;
                    ProjectListObj.ProjectManager = item.Manger;
                    ProjectListObj.ClientBillableAmount = ClientHelper.NumericNumConvToAbbv(item.ClientBillableAmount);
                    ProjectListObj.ClientBillableAmount = (item.ClientBillableAmount).ToString();
                    ClientBillableAmount += Convert.ToDouble(item.ClientBillableAmount);
                    ProjectListObj.CampanyName = item.CampanyName;
                    var technology = item.Technology.Split(',');
                    List<string> ListStage = new List<string>();
                    foreach (var tech in technology)
                    {
                        var i = Convert.ToInt32(tech);
                        var techno = await _db.Technology.Where(x => x.TechnologyId == i).Select(x => x.TechnologyType).FirstOrDefaultAsync();
                        if (techno != null)
                            ListStage.Add(techno);
                    }
                    var tec = String.Join(",", ListStage);

                    ProjectListObj.TechnologyType = tec;
                    ProjectListObj.CurrencyName = item.crc;
                    ProjectListObj.PaymentType = item.PaymentType;
                    ProjectListObj.IsActive = item.IsActive;
                    ProjectListObj.IsDeleted = item.IsDeleted;

                    ProjectListObj.SumofBillableSalary = employeeService.GetTotalBillableCostByProjectId(item.ID);
                    SumofBillableSalary += ProjectListObj.SumofBillableSalary;

                    ProjectListObj.SumofNonBillableSalary = employeeService.GetTotalNonBillableCostByProjectId(item.ID);
                    SumofNonBillableSalary += ProjectListObj.SumofNonBillableSalary;

                    ProjectListObj.billAndNonBilEmpSalary = ProjectListObj.SumofBillableSalary + ProjectListObj.SumofNonBillableSalary;
                    BillAndNonBilEmpSalary = SumofBillableSalary + SumofNonBillableSalary;

                    ProjectListObj.AmountDiffrence = item.ClientBillableAmount - ProjectListObj.billAndNonBilEmpSalary;

                    if (ProjectListObj.AmountDiffrence < 0)
                        ProjectListObj.ProjectHealth = "Bad";
                    else
                        ProjectListObj.ProjectHealth = "Good";
                    ProjectListObj.resourceCount = (from ad in _db.AssignProjects where ad.ProjectId == item.ID && ad.IsDeleted == false && ad.IsActive == true select ad).Count();
                    ProjectListObj.CreatedBy = item.CreatedBy;
                    ProjectListObj.UpdatedBy = item.UpdatedBy;
                    ProjectListObj.ProjectCreatedBy = _db.Employee.Where(x => x.EmployeeId == item.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                    ProjectListObj.LastUpdatedBy = item.UpdatedBy.HasValue ? _db.Employee.Where(x => x.EmployeeId == (int)item.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault() : "";
                    ProjectListObj.LeadType = item.LeadType;
                    ProjectListObj.IsMailSendToday = item.IsMailSendToday;
                    ProjectListObj.ProjectCreatedDate = item.ProjectCreatedDate;
                    ProjectListObj.ProjectStatus = item.ProjectStatus;
                    ProjectListObj.ProjectStatusId = item.ProjectStatusId;
                    ProjectList.Add(ProjectListObj);
                }


                allproject.ProjectList = ProjectList.OrderByDescending(x => x.ID).ToList();

                allproject.SumofBillableSalary = ClientHelper.NumericNumConvToAbbv(SumofBillableSalary);
                allproject.SumofBillableSalary = (SumofBillableSalary).ToString();
                allproject.SumofNonBillableSalary = ClientHelper.NumericNumConvToAbbv(SumofNonBillableSalary);
                allproject.SumofNonBillableSalary = (SumofNonBillableSalary).ToString();
                allproject.BillAndNonBilEmpSalary = ClientHelper.NumericNumConvToAbbv(BillAndNonBilEmpSalary);
                allproject.BillAndNonBilEmpSalary = (BillAndNonBilEmpSalary).ToString();
                allproject.ClientBillableAmount = ClientHelper.NumericNumConvToAbbv(ClientBillableAmount);
                allproject.ClientBillableAmount = (Math.Round(ClientBillableAmount, 2)).ToString();
                allproject.AmountDiffrence = ClientBillableAmount / 12 - BillAndNonBilEmpSalary / 12;

                if (ClientBillableAmount > BillAndNonBilEmpSalary)
                {
                    var MonthlyPro = Math.Abs(allproject.AmountDiffrence);
                    //  allproject.Profit = MonthlyPro.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    allproject.Profit = ClientHelper.NumericNumConvToAbbv(MonthlyPro);
                    allproject.Profit = (Math.Round(MonthlyPro, 2)).ToString();
                }
                else
                {
                    var MonthlyLoss = Math.Abs(allproject.AmountDiffrence);
                    //allproject.Loss = MonthlyLoss.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    allproject.Loss = ClientHelper.NumericNumConvToAbbv(MonthlyLoss);
                    allproject.Loss = (Math.Round(MonthlyLoss, 2)).ToString();
                }

                if (ProjectList.Count != 0)
                {
                    res.Message = "Project Found Succesfully !";
                    res.StatusCode = HttpStatusCode.Found;
                    res.Status = true;
                    res.Data = allproject;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Project Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = allproject;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Status = true;
                res.Message = ex.Message;
                return Ok(res);
            }
        }

        #endregion This API's is used for Get All Project

        #region This API is used  update project
        ///// <summary>
        ///// Created By Sriya Malvi 
        ///// Modify By Ravi Vyas Date 17/05/2022
        ///// </summary>route api/project/UpdateProjectList
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPut]
        [Route("UpdateProjectList")]
        [Authorize]
        public async Task<IHttpActionResult> AddProject(AddProjectModal model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.ProjectLists.
                                     Where(x => x.ID == model.ID && x.IsActive && !x.IsDeleted &&
                                     x.CompanyId == claims.companyId)
                                     .FirstOrDefaultAsync();
                if (checkData != null)
                {
                    checkData.ProjectName = model.ProjectName;
                    checkData.CampanyName = model.CampanyName;
                    checkData.Technology = string.Join(",", model.Technology);
                    checkData.PaymentType = model.PaymentType;
                    if ("Recurring" == model.LeadType)
                    {
                        checkData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                    }
                    else
                    {
                        checkData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                        checkData.StartDate = model.StartDate;
                        checkData.EndDate = model.EndDate;
                    }
                    checkData.Others = model.Others;
                    checkData.FromCurrency = model.FromCurrency;
                    checkData.ToCurrency = model.ToCurrency;
                    checkData.ClientAmount = model.ClientAmount;
                    checkData.ExchangeRate = model.ExchangeRate;
                    checkData.ExchangeDate = DateTime.Now;
                    checkData.LeadType = model.LeadType;
                    checkData.ClientConvertedAmt = model.ClientConvertedAmt;
                    checkData.ProjectDiscription = model.ProjectDiscription;
                    checkData.ProjectStatus = ProjectStatusConstants.Live;
                    checkData.IsProjectManager = true;
                    checkData.CompanyId = claims.companyId;
                    checkData.OrgId = 0;
                    checkData.LinkJson = JsonConvert.SerializeObject(model.Links);
                    checkData.UpdatedOn = DateTime.Now;
                    checkData.UpdatedBy = claims.employeeId;

                    #region Add Project Manager

                    var assignCheck = await _db.AssignProjects.
                                             FirstOrDefaultAsync(a => a.ProjectId == checkData.ID
                                             && a.EmployeeId == checkData.ProjectManager);
                    if (assignCheck != null)
                    {

                        assignCheck.ProjectId = checkData.ID;
                        assignCheck.UpdatedBy = claims.employeeId;
                        assignCheck.UpdatedOn = DateTimeOffset.Now;
                        assignCheck.EmployeeId = model.ProjectManager;
                        assignCheck.Status = "Non-Billable";
                        assignCheck.IsProjectManager = true;
                        assignCheck.CompanyId = claims.companyId;
                        assignCheck.OrgId = claims.orgId;
                    }


                    #endregion

                    #region Add Permission To PM

                    var checkPermission = await _db.TaskPermissions.
                                                FirstOrDefaultAsync(p => p.ProjectId == checkData.ID &&
                                                p.AssigneEmployeeId == checkData.ProjectManager &&
                                                p.CompanyId == claims.companyId);

                    if (checkPermission != null)
                    {
                        checkPermission.ProjectId = model.ID;
                        checkPermission.AssigneEmployeeId = model.ProjectManager;
                        checkPermission.IsCreateTask = true;
                        checkPermission.IsDeleteTask = true;
                        checkPermission.IsApprovedTask = true;
                        checkPermission.IsExeclUploade = true;
                        checkPermission.IsReEvaluetTask = true;
                        checkPermission.IsUpdate = true;
                        checkPermission.IsOtherTaskCreate = true;
                        checkPermission.IsBoardVisible = true;
                        checkPermission.ViewAlProjectTask = true;
                        checkPermission.CompanyId = claims.companyId;
                        checkPermission.OrgId = claims.orgId;
                        checkPermission.CreatedBy = claims.employeeId;
                        checkPermission.CreatedOn = DateTimeOffset.Now;
                        checkPermission.UpdatedBy = claims.employeeId;
                        checkPermission.UpdatedOn = DateTimeOffset.Now;
                    }
                    #endregion


                    _db.Entry(checkPermission).State = EntityState.Modified;
                    _db.Entry(assignCheck).State = EntityState.Modified;
                    checkData.ProjectManager = model.ProjectManager;
                    _db.Entry(checkData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Project Added Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = checkData;
                    return Ok(res);

                }
                else
                {
                    res.Message = "Project Name Already Exits !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }

        }

        #endregion This Api use For Added Project

        #region This Api use For Added Project
        ///// <summary>
        ///// Create By Ankit Jain Date 17/05/2022
        ///// modified by shriya modified on 19-05-2022
        ///// modified  by  shriya  on 13-06-2022
        ///// </summary>route api/project/AddProject
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("AddProject")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddProjectNew(AddProjectModal model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.ProjectLists
                                   .Where(x => x.ProjectName.Trim().ToUpper() == model.ProjectName.Trim().ToUpper() &&
                                   x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted)
                                   .FirstOrDefaultAsync();
                if (checkData == null)
                {
                    ProjectList PreData = new ProjectList();
                    PreData.ProjectName = model.ProjectName;
                    PreData.ProjectManager = model.ProjectManager;
                    PreData.CampanyName = model.CampanyName;
                    PreData.Technology = string.Join(",", model.Technology);
                    PreData.PaymentType = model.PaymentType;
                    if ("Recurring" == model.LeadType)
                    {
                        PreData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                    }
                    else
                    {
                        PreData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                        PreData.StartDate = model.StartDate;
                        PreData.EndDate = model.EndDate;
                    }
                    PreData.SubProjectId = model.SubProjectId;
                    PreData.Others = model.Others;
                    PreData.FromCurrency = model.FromCurrency;
                    PreData.ToCurrency = model.ToCurrency;
                    PreData.ClientAmount = model.ClientAmount;
                    PreData.ExchangeRate = model.ExchangeRate;
                    PreData.ExchangeDate = DateTime.Now;
                    PreData.LeadType = model.LeadType;
                    PreData.ClientConvertedAmt = model.ClientConvertedAmt;
                    PreData.ProjectDiscription = model.ProjectDiscription;
                    PreData.ProjectStatus = ProjectStatusConstants.Live;
                    PreData.IsProjectManager = true;
                    PreData.IsDeleted = false;
                    PreData.IsActive = true;
                    PreData.CreatedBy = claims.employeeId;
                    PreData.CreatedOn = DateTime.Now;
                    PreData.CompanyId = claims.companyId;
                    PreData.OrgId = 0;
                    PreData.LinkJson = JsonConvert.SerializeObject(model.Links);
                    _db.ProjectLists.Add(PreData);
                    await _db.SaveChangesAsync();

                    #region Assign Project Manager

                    AssignProject checkEmp = new AssignProject
                    {
                        ProjectId = PreData.ID,
                        EmployeeId = model.ProjectManager,
                        Status = "Non-Billable",
                        IsProjectManager = true,
                        IsDeleted = false,
                        IsActive = true,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId

                    };
                    _db.AssignProjects.Add(checkEmp);
                    await _db.SaveChangesAsync();

                    #endregion

                    #region Add Permission

                    TaskPermissions obj = new TaskPermissions
                    {
                        ProjectId = PreData.ID,
                        AssigneEmployeeId = PreData.ProjectManager,
                        IsCreateTask = true,
                        ViewAlProjectTask = true,
                        IsDeleteTask = true,
                        IsApprovedTask = true,
                        IsExeclUploade = true,
                        IsReEvaluetTask = true,
                        IsUpdate = true,
                        IsOtherTaskCreate = true,
                        IsBoardVisible = true,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTimeOffset.Now
                    };
                    _db.TaskPermissions.Add(obj);
                    _db.SaveChanges();

                    #endregion

                    #region Add Sprint 

                    Sprint objSprint = new Sprint
                    {
                        SprintName = "BackLog",
                        ProjectId = PreData.ID,
                        StartDate = PreData.CreatedOn,
                        EndDate = DateTimeOffset.Now,
                        SprintStatus = SprintStatusConstant.Draft,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, claims.TimeZone),
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId
                    };
                    _db.Sprints.Add(objSprint);
                    await _db.SaveChangesAsync();
                    _db.SaveChanges();

                    #endregion

                    res.Message = "Project Added Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = PreData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Project Name Already Exist !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }

        #endregion This Api use For Added Project

        #region This API's is used for Delete Project
        /// <summary>
        /// API >> Delete >> api/project/DeleteProjectList
        /// created by shriya , create on 19-05-2022
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("DeleteProjectList")]
        [HttpDelete]
        [Authorize]
        public async Task<ResponseBodyModel> DeleteProjectList(int projectId, int? subProjectId = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ProjectListData = await _db.ProjectLists.Where(x => x.IsDeleted == false && (subProjectId.HasValue) ? x.ID == subProjectId : x.ID == projectId).FirstOrDefaultAsync();
                var AssignProjectListData = _db.AssignProjects.Where(x => x.IsDeleted == false && x.ProjectId == projectId).ToList();/* (subProjectId.HasValue) ? x.SubProjectId == subProjectId :*/

                if (ProjectListData != null)
                {
                    ProjectListData.IsDeleted = true;
                    ProjectListData.IsActive = false;
                    ProjectListData.DeletedBy = claims.employeeId;
                    ProjectListData.DeletedOn = DateTime.Now;
                    _db.Entry(ProjectListData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "ProjectList Deleted Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Record Found!!";
                }
                if (AssignProjectListData.Count > 0)
                {
                    foreach (var item in AssignProjectListData)
                    {
                        item.IsDeleted = true;
                        item.IsActive = false;
                        item.DeletedBy = claims.employeeId;
                        item.DeletedOn = DateTime.Now;
                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    res.Status = true;
                    res.Message = "ProjectList Deleted Successfully";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This API's is used for Delete Project

        #region This Api Use to Get Project Manager

        /// <summary>
        /// Create By Ankit Jain Date 17/05/2022
        /// </summary>route api/project/GetProjectmanager
        /// <returns></returns>
        [HttpGet]
        [Route("GetProjectManager")]
        public async Task<ResponseBodyModel> GetProjectManager()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.orgId == 0)
                {
                    //var projectmanager = await (from user in _db.User
                    //                            join emp in _db.Employee on user.EmployeeId equals emp.EmployeeId
                    //                            where  user.IsActive == true && user.IsDeleted == false && user.CompanyId == claims.companyId
                    //                            select new
                    //                            {
                    //                                emp.EmployeeId,
                    //                                emp.DisplayName,
                    //                                emp.OfficeEmail,
                    //                                //user.LoginId
                    //                            }).ToListAsync();


                    var projectmanager = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                     && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).Select(x => new
                     {
                         x.EmployeeId,
                         x.DisplayName,
                         x.OfficeEmail,
                     }).ToListAsync();


                    //var projectManagers = await db.Employee.Where(x => x.EmployeeTypeId==6 && x.IsDeleted == false && x.IsActive == true).ToListAsync();
                    if (projectmanager != null)
                    {
                        res.Status = true;
                        res.Message = "Project Managers Found";
                        res.Data = projectmanager;
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "No Project Managers Found!!";

                    }
                }
                else
                {
                    //var projectmanager = await (from user in _db.User
                    //                            join emp in _db.Employee on user.EmployeeId equals emp.EmployeeId
                    //                            where user.IsActive == true && user.IsDeleted == false && user.CompanyId == claims.companyId && user.OrgId == claims.orgId
                    //                            select new
                    //                            {
                    //                                emp.EmployeeId,
                    //                                emp.DisplayName,
                    //                                emp.OfficeEmail,
                    //                                //user.LoginId
                    //                            }).ToListAsync();

                    var projectmanager = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                    && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                        x.OfficeEmail,
                    }).ToListAsync();

                    //var projectManagers = await db.Employee.Where(x => x.EmployeeTypeId==6 && x.IsDeleted == false && x.IsActive == true).ToListAsync();
                    if (projectmanager != null)
                    {
                        res.Status = true;
                        res.Message = "Project Managers Found";
                        res.Data = projectmanager;
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "No Project Managers Found!!";
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use to Get Project Manager

        #region API TO GET EMPLOYEE LIST IN PROJECT
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> GET >> api/project/getemployeelistinprojectadd
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeelistinprojectadd")]
        public async Task<IHttpActionResult> GetEmployeeListInProjectAdd(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkEmployeeId = await _db.AssignProjects.
                                            Where(x => x.ProjectId == projectId &&
                                            !x.IsDeleted && x.IsActive).
                                            Select
                                            (x => x.EmployeeId).
                                            ToListAsync();
                var employeeList = await _db.Employee.
                                         Where(x => x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                         !x.IsEmployeeIsLock && !x.IsDeleted && x.IsActive &&
                                         x.CompanyId == tokenData.companyId &&
                                         !checkEmployeeId.Contains(x.EmployeeId)).
                                         Select(x => new
                                         {
                                             x.EmployeeId,
                                             x.DisplayName,
                                             x.OfficeEmail,
                                         })
                                         .ToListAsync();
                if (employeeList.Count == 0)
                {
                    res.Message = "No Employee Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = employeeList;
                    return Ok(res);
                }
                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employeeList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/project/getemployeelistinprojectadd | " +
                    "Project Id : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region This Api used for add employee in project
        /// <summary>
        /// API >> Post >> api/project/AddNewEmployee
        /// created by shriya and created on 18-05-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddNewEmployee")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> AddNewEmployee(AssignProjectModal model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    AssignProject checkEmp = new AssignProject();
                    var exitingEmp = _db.AssignProjects.Where(x => x.IsActive == true && x.IsDeleted == false && x.ProjectId == model.ProjectId).FirstOrDefault();
                    foreach (var item in model.EmployeeId)
                    {
                        checkEmp.ProjectId = model.ProjectId;
                        checkEmp.EmployeeId = item;
                        checkEmp.Status = model.Status;
                        checkEmp.CreatedBy = tokenData.employeeId;
                        checkEmp.CreatedOn = today;
                        checkEmp.CompanyId = tokenData.companyId;
                        checkEmp.EmployeeRoleInProjectId = model.EmployeeRoleInProjectId;
                        _db.AssignProjects.Add(checkEmp);
                        await _db.SaveChangesAsync();
                        TaskPermissions obj = new TaskPermissions
                        {
                            ProjectId = checkEmp.ProjectId,
                            AssigneEmployeeId = checkEmp.EmployeeId,
                            IsCreateTask = true,
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = today,
                        };
                        _db.TaskPermissions.Add(obj);
                        await _db.SaveChangesAsync();
                    }
                    var projectDetails = await (from p in _db.ProjectLists
                                                join ce in _db.Employee on p.CreatedBy equals ce.EmployeeId
                                                join ma in _db.Employee on p.ProjectManager equals ma.EmployeeId
                                                where p.ID == model.ProjectId
                                                select new ProjectDetailClassOnAssigningMail
                                                {
                                                    ProjectName = p.ProjectName,
                                                    AddedByInProject = tokenData.displayName,
                                                    ProjectCreatorName = ce.DisplayName,
                                                    ProjectCreateDate = p.CreatedOn,/*.ToString("dd MMM, yyyy"),*/
                                                    ProjectManagerName = ma.DisplayName,
                                                })
                                                .FirstOrDefaultAsync();
                    HostingEnvironment.QueueBackgroundWorkItem(ct => SendAssigningMail(projectDetails, model.EmployeeId, tokenData.companyId, tokenData.IsSmtpProvided));
                    res.Status = true;
                    res.Message = "Employee Added Successfully";
                    res.Data = checkEmp;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Employee not Added.";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class ProjectDetailClassOnAssigningMail
        {
            public string ProjectName { get; set; }
            public string AddedByInProject { get; set; }
            public string ProjectCreatorName { get; set; }
            public DateTime ProjectCreateDate { get; set; }
            public string ProjectManagerName { get; set; }
        }
        public async Task SendAssigningMail(ProjectDetailClassOnAssigningMail projectDetail, List<int> employeeIds, int companyId, bool IsSmtpProvided)
        {
            try
            {
                var companyData = _db.Company.Where(y => y.CompanyId == companyId)
                   .Select(x => new { x.RegisterAddress, x.RegisterCompanyName }).FirstOrDefault();
                var employeeList = await (from e in _db.Employee
                                          join d in _db.Designation on e.DesignationId equals d.DesignationId
                                          where employeeIds.Contains(e.EmployeeId)
                                          select new SendMailToNewResourceAdd
                                          { DisplayName = e.DisplayName, OfficalEmail = e.OfficeEmail, Designation = d.DesignationName }
                                          ).ToListAsync();
                string attachmentPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                await Task.Delay(100);
                foreach (var item in employeeList)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string innerBody = TaskHelper.AddResourceInProject
                        .Replace("<|EMPLOYEENAME|>", item.DisplayName.Trim())
                        .Replace("<|PROJECTNAME|>", projectDetail.ProjectName.Trim())
                        .Replace("<|DESIGNATIONNAME|>", item.Designation.Trim())
                        .Replace("<|ADDEDBYNAME|>", projectDetail.AddedByInProject.Trim())
                        .Replace("<|CREATERNAME|>", projectDetail.ProjectCreatorName.Trim())
                        .Replace("<|CREATEDDATE|>", projectDetail.ProjectCreateDate.ToString("dd-MMM-yyyy"))
                        .Replace("<|MANAGERNAME|>", projectDetail.ProjectManagerName.Trim());
                    string htmlBody = EmailHelperClass.DefaultMailBody
                        .Replace("<|MAIL_TITLE|>", "Project Notification")
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|INNER_BODY|>", innerBody)
                        .Replace("<|COMPANYNAMEE|>", companyData.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companyData.RegisterAddress);

                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = IsSmtpProvided,
                        Subject = "Project Notification",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { item.OfficalEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public class SendMailToNewResourceAdd
        {
            public string DisplayName { get; set; }
            public string OfficalEmail { get; set; }
            public string Designation { get; set; }
        }
        #endregion This Api used for add employee in project

        #region This API is used for Get  assign  employee by project id
        /// <summary>
        /// Api >> Get >> api/project/GetResourceByProjectId
        /// Created by Shriya On 18-05-2022
        /// update by shriya On 13-06-2022
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [Route("GetResourceByProjectId")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetResourceByProjectId(int ProjectId, int? subProjectId = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                GetDataEmployeeModal getdata = new GetDataEmployeeModal();
                List<DataEmployee> employeeList = new List<DataEmployee>();
                var ProjectAmount = _db.ProjectLists.Where(x => (subProjectId.HasValue) ? x.ID == subProjectId : x.ID == ProjectId && x.IsActive == true
                && x.IsDeleted == false /*&& x.IsProjectManager == true*/).FirstOrDefault();

                var globalseatcost = _db.GSeatCosts.Where(x => x.IsActive == true && x.IsDeleted == false
                && x.CompanyId == claims.companyId && x.Update == false).Select(x => x.GlobalAmount).FirstOrDefault();

                var EmployeeData = await (from ad in _db.AssignProjects
                                          join bd in _db.Employee on ad.EmployeeId equals bd.EmployeeId
                                          join de in _db.Designation on bd.DesignationId equals de.DesignationId
                                          join pr in _db.EmployeeRoleInProjects on ad.EmployeeRoleInProjectId equals pr.EmployeeRoleInProjectId into r
                                          from result in r.DefaultIfEmpty()
                                          where ad.IsActive == true && ad.IsDeleted == false /*&& ad.ProjectId == ProjectId*/
                                          where (subProjectId.HasValue) ? ad.ProjectId == subProjectId : ad.ProjectId == ProjectId
                                          select new
                                          {
                                              bd.EmployeeId,
                                              n = bd.DisplayName,
                                              bd.PrimaryContact,
                                              bd.PersonalEmail,
                                              de.DesignationName,
                                              GrossSalery = bd.GrossSalery,
                                              bd.RoleId,
                                              bd.FirstName,
                                              bd.LastName,
                                              ad.Status,
                                              ad.AssignProjectId,
                                              ad.OccupyPercent,
                                              Role = ad.IsProjectManager == true ? "Project Manager" : result == null ? null : result.RoleName,
                                              IsProjectManager = (ProjectAmount.ProjectManager == bd.EmployeeId),
                                          })
                                          .Distinct()
                                          .OrderBy(x => x.n)
                                          .ToListAsync();
                var SumofBillableSalary = 0.0;
                var SumofNonBillableSalary = 0.0;
                var billAndNonBilEmpSalary = 0.0;
                var BillableResourceCount = 0;
                var NonBillableResourceCount = 0;
                if (EmployeeData != null)
                {
                    foreach (var item in EmployeeData)
                    {
                        DataEmployee data = new DataEmployee();
                        data.EmployeeId = item.EmployeeId;
                        data.FullName = item.n;
                        data.PrimaryContact = item.PrimaryContact;
                        data.Email = item.PersonalEmail;
                        data.Salary = item.GrossSalery / 12;
                        data.Status = item.Status;
                        data.DesignationName = item.DesignationName;
                        data.AssignProjectId = item.AssignProjectId;
                        data.FirstName = item.FirstName;
                        data.LastName = item.LastName;
                        data.RoleName = item.Role;
                        int occupy = (from a in _db.AssignProjects
                                      join p in _db.ProjectLists on a.ProjectId equals p.ID
                                      where a.EmployeeId == data.EmployeeId && a.IsDeleted == false && a.IsActive == true && a.CompanyId == claims.companyId && p.ProjectStatus == ProjectStatusConstants.Live
                                      select new
                                      {

                                      }).Count();

                        int OccupyPercent = occupy == 0 ? 0 : 100 / occupy;
                        data.OccupyPercent = OccupyPercent.ToString() + "%";
                        data.MonthlySalary = ClientHelper.NumericNumConvToAbbv(Math.Round((item.GrossSalery / 12), 2));
                        data.MonthlySalary = (Math.Round((item.GrossSalery / 12), 2)).ToString();
                        if (data.Status == "Billable")
                        {
                            SumofBillableSalary += Convert.ToDouble(item.GrossSalery / 12);
                            BillableResourceCount += 1;
                        }
                        if (data.Status == "Non-Billable")
                        {
                            SumofNonBillableSalary += Convert.ToDouble(item.GrossSalery / 12);
                            NonBillableResourceCount += 1;
                        }
                        billAndNonBilEmpSalary = SumofBillableSalary + SumofNonBillableSalary;
                        data.IsProjectManager = item.IsProjectManager;
                        employeeList.Add(data);
                    }
                    var expense = _db.ProjectExpenseMasters.Where(x => x.ProjectId == ProjectId && x.IsActive == true && x.IsDeleted == false && x.ExpenseDate.Month == DateTime.Now.Month && x.ExpenseDate.Year == DateTime.Now.Year).Select(x => x.ProjectExpId).Distinct().ToList();
                    var currentmonthexp = 0.0;
                    foreach (var item in expense)
                    {
                        var expenseamt = _db.ProjectExpCostAmts.Where(x => x.ExpenseId == item).Select(x => x.Amount).Sum();
                        currentmonthexp = currentmonthexp + expenseamt;
                    }
                    var technologyList = ProjectAmount.Technology.Split(',');
                    var intList = technologyList.Select(s => Convert.ToInt32(s)).ToList();
                    getdata.ProjectName = ProjectAmount.ProjectName;
                    getdata.CompanyName = ProjectAmount.CampanyName;
                    getdata.ClientBillableAmount = ProjectAmount.ClientBillableAmount;
                    getdata.Technologies = String.Join(",", _db.Technology.Where(x => intList.Contains(x.TechnologyId)).Select(x => x.TechnologyType).ToList());
                    getdata.EmployeeList = employeeList;
                    getdata.BillAndNonBilEmpSalary = billAndNonBilEmpSalary;
                    getdata.MontlyBurn = (employeeList.Count * globalseatcost) + billAndNonBilEmpSalary + currentmonthexp; //(update by shriya on 23-06-2022)(Update Current month expens on it by shriya on 21-07-2022)
                                                                                                                           // getdata.MontlyBurns = getdata.MontlyBurn.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    getdata.MontlyBurns = ClientHelper.NumericNumConvToAbbv(getdata.MontlyBurn);
                    getdata.MontlyBurns = (Math.Round(getdata.MontlyBurn, 2)).ToString();
                    getdata.YearlyBurn = (employeeList.Count * globalseatcost) * 12; //(update by shriya on 23-06-2022)
                                                                                     //getdata.YearlyBurns = getdata.YearlyBurn.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    getdata.YearlyBurns = ClientHelper.NumericNumConvToAbbv(getdata.YearlyBurn);
                    getdata.YearlyBurns = Math.Round(getdata.YearlyBurn, 2).ToString();
                    getdata.Diffrence = Math.Abs((ProjectAmount.ClientBillableAmount / 12) - getdata.MontlyBurn);//(update by shriya on 23-06-2022)
                                                                                                                 //getdata.MonthlyDiffer = getdata.Diffrence.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    getdata.MonthlyDiffer = ClientHelper.NumericNumConvToAbbv(getdata.Diffrence);
                    getdata.MonthlyDiffer = Math.Round(getdata.Diffrence, 2).ToString();
                    getdata.SumofBillableSalary = SumofBillableSalary;
                    getdata.SumofNonBillableSalary = SumofNonBillableSalary;
                    getdata.BillableResourceCount = BillableResourceCount;
                    getdata.NonBillableResourceCount = NonBillableResourceCount;
                    //  getdata.ClientBillableAmounts = getdata.ClientBillableAmount.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    getdata.ClientBillableAmounts = ClientHelper.NumericNumConvToAbbv(getdata.ClientBillableAmount);
                    getdata.ClientBillableAmounts = (getdata.ClientBillableAmount).ToString();
                    // getdata.EmployeesMonthlySalary = Math.Round(employeeList.Select(x => x.Salary).ToList().Sum(), 2).ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
                    getdata.EmployeesMonthlySalary = ClientHelper.NumericNumConvToAbbv(Math.Round(employeeList.Select(x => x.Salary).ToList().Sum(), 2));
                    getdata.EmployeesMonthlySalary = (Math.Round(employeeList.Select(x => x.Salary).ToList().Sum(), 2)).ToString();
                    if (ProjectAmount.ClientBillableAmount >= getdata.BillAndNonBilEmpSalary)
                    {
                        getdata.ProjectStatus = true;
                    }
                    else { getdata.ProjectStatus = false; }
                    double plres = 0.0;
                    if (getdata.MontlyBurn != 0)
                    {
                        plres = (ProjectAmount.ClientBillableAmount / 12) - getdata.MontlyBurn;
                    }

                    if (plres > 0)
                    {
                        getdata.Profit = plres;
                        var proper = Math.Round(getdata.Profit / getdata.MontlyBurn * 100, 2);
                        getdata.ProfitPer = (proper).ToString() + "%";
                    }
                    else
                    {
                        getdata.Loss = Math.Abs(plres);
                        if (plres != 0)
                        {
                            var lossper = Math.Round(getdata.Loss / getdata.MontlyBurn * 100, 2);
                            getdata.LossPer = (lossper).ToString() + "%";
                        }
                    }
                    getdata.LeadType = ProjectAmount.LeadType;
                    getdata.Currency = ProjectAmount.ToCurrency;
                    getdata.MonthlyProjectExpense = ClientHelper.NumericNumConvToAbbv(currentmonthexp);
                    getdata.MonthlyProjectExpense = Math.Round(currentmonthexp, 2).ToString();
                    res.Message = "Project assign to employee data list found";
                    res.Status = true;
                    res.Data = getdata;
                }
                else
                {

                }
            }

            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This API is used for Get  assign  employee by project id

        #region This is used for delete resource
        /// <summary>
        /// API >> Delete >> api/project/DeleteResourceByAssignProjectId
        /// </summary>
        /// <param name="AssignProjectId"></param>
        /// <returns></returns>
        [Route("DeleteResourceByAssignProjectId")]
        [HttpDelete]
        [Authorize]
        public async Task<IHttpActionResult> DeleteResourceByProjectId(int AssignProjectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkResource = await _db.AssignProjects.
                                          FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                                          x.AssignProjectId == AssignProjectId);

                if (checkResource != null)
                {
                    checkResource.IsDeleted = true;
                    checkResource.IsActive = false;
                    checkResource.DeletedOn = DateTime.Now;
                    checkResource.DeletedBy = claims.employeeId;
                    _db.Entry(checkResource).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    var checkResourceData = await (from ap in _db.AssignProjects
                                                   join p in _db.ProjectLists on ap.ProjectId equals p.ID
                                                   join e in _db.Employee on ap.EmployeeId equals e.EmployeeId
                                                   join de in _db.Employee on ap.DeletedBy equals de.EmployeeId
                                                   join m in _db.Employee on p.ProjectManager equals m.EmployeeId
                                                   where ap.AssignProjectId == AssignProjectId
                                                   select new DeleteResourceModelClass
                                                   {
                                                       EmployeeId = ap.EmployeeId,
                                                       EmployeeName = e.DisplayName,
                                                       ProjectName = p.ProjectName,
                                                       DeletedByName = de.DisplayName,
                                                       DeletedDate = ap.DeletedOn.Value,
                                                       ManagerMail = m.OfficeEmail,
                                                       EmployeeMail = e.OfficeEmail
                                                   }).FirstOrDefaultAsync();

                    HostingEnvironment.QueueBackgroundWorkItem(ct => SendResourceRemoveMail(checkResourceData, claims.companyId, claims.IsSmtpProvided));

                    res.Message = "Data Deleted Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = checkResource;
                    return Ok(res);


                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = checkResource;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
                return BadRequest("Failed");
            }
        }
        public class DeleteResourceModelClass
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string ProjectName { get; set; }
            public string DeletedByName { get; set; }
            public DateTimeOffset DeletedDate { get; set; }
            public string ManagerMail { get; set; }
            public string EmployeeMail { get; set; }
        }

        #endregion This is used for delete resource

        public async Task SendResourceRemoveMail(DeleteResourceModelClass model, int companyId, bool IsSmtpProvided)
        {
            try
            {
                var companyData = _db.Company.Where(y => y.CompanyId == companyId)
                   .Select(x => new { x.RegisterAddress, x.RegisterCompanyName }).FirstOrDefault();
                string attachmentPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                await Task.Delay(100);

                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefault();
                }
                string innerBody = TaskHelper.DeleteResourceInProject
                    .Replace("<|EMPLOYEENAME|>", model.EmployeeName.Trim())
                    .Replace("<|PROJECTNAME|>", model.ProjectName.Trim())
                    .Replace("<|DELETEDBYNAME|>", model.DeletedByName.Trim())
                    .Replace("<|DELETEDDATE|>", model.DeletedDate.ToString("dd-MMM-yyyy"));
                string htmlBody = EmailHelperClass.DefaultMailBody
                    .Replace("<|MAIL_TITLE|>", "Project Notification")
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|INNER_BODY|>", innerBody)
                    .Replace("<|COMPANYNAMEE|>", companyData.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companyData.RegisterAddress);

                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = IsSmtpProvided,
                    Subject = "Project Notification",
                    MailBody = htmlBody,
                    MailTo = new List<string>()
                    {
                        model.ManagerMail,
                        model.EmployeeMail

                    },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);

            }
            catch (Exception)
            {
                throw;
            }
        }







        #region This Api Used By Get Technology

        /// <summary> route api/project/GetTechnology
        /// Create By ankit Date 17/05/2022
        /// </summary>
        /// <returns></returns>
        [Route("GetTechnology")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTechnology()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var technologyData = await _db.Technology
                                           .Where(x => !x.IsDeleted
                                           && x.IsActive && x.CompanyId == claims.companyId)
                                           .ToListAsync();
                if (technologyData.Count > 0)
                {
                    res.Message = "Record Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = technologyData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Record Found!!";
                    res.StatusCode = HttpStatusCode.Found;
                    res.Status = false;
                    res.Data = technologyData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }

        }

        #endregion This Api Used By Get Technology

        #region This API's is used for Add Technology

        /// <summary>
        /// API >> Post >>
        /// Created by shriya , Create on 19-05-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addtechnology")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddTechnology(AddTechnologyModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitingtech = _db.Technology
                    .Where(x => x.TechnologyType == model.TechnologyType && x.CompanyId == claims.companyId)
                    .Select(x => x.TechnologyType)
                    .FirstOrDefault();
                if (exitingtech == null)
                {
                    Technology tech = new Technology();
                    tech.TechnologyType = model.TechnologyType;
                    tech.IsActive = true;
                    tech.IsDeleted = false;
                    tech.CompanyId = claims.companyId;
                    tech.OrgId = claims.orgId;
                    _db.Technology.Add(tech);
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Technology addded";
                    res.Data = tech;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Technology not added because its exiting technology";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public class AddTechnologyModel
        {
            public string TechnologyType { get; set; }
        }

        #endregion This API's is used for Add Technology

        #region Api To Get Employee
        /// <summary>
        /// Created By Harshit Mitra on 06-07-2022
        /// API >> Get >> api/project/emplist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("emplist")]
        public async Task<ResponseBodyModel> EmployeeListg(int projectId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeAssign = _db.AssignProjects.Where(x => x.ProjectId == projectId
                        && x.IsDeleted == false && x.IsActive == true).Select(x => x.EmployeeId).ToList();
                var employeeList = await (from e in _db.Employee
                                          where e.IsActive == true && e.IsDeleted == false &&
                                          e.CompanyId == claims.companyId && !employeeAssign.Contains(e.EmployeeId)
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,
                                              e.OfficeEmail,
                                          }).OrderBy(x => x.DisplayName).ToListAsync();

                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    res.Data = employeeList;
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.Data = employeeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Employee

        #region This Api use For Get Currency

        /// <summary>
        /// Created By Ankit date 17/05/2022
        /// Changes By shriya Changes on 26-05-2022
        /// </summary>route api/project/GetCurrency
        /// <returns></returns>
        [Route("GetCurrency")]
        [HttpGet]
        public ResponseBodyModel GetCurrency()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var CurrencyData = CurrencyHelper.GetCurrencyName();
                if (CurrencyData != null)
                {
                    res.Status = true;
                    res.Message = "Record Found";
                    res.Data = CurrencyData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Record Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api use For Get Currency

        #region This Api use For Get Country
        /// <summary>
        /// Created By Suraj Bundel date 01/06/2022
        /// Use to get country list
        /// </summary>route api/project/GetCountry
        /// <returns></returns>

        [HttpGet]
        [Route("getcountry")]
        public async Task<ResponseBodyModel> GetCountry()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                List<string> Culturelist = new List<string>();
                CultureInfo[] getcultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                foreach (CultureInfo getCulture in getcultureInfo)
                {
                    RegionInfo getregionInfo = new RegionInfo(getCulture.LCID);
                    if (!(Culturelist.Contains(getregionInfo.EnglishName)))
                    {
                        Culturelist.Add(getregionInfo.EnglishName);
                    }
                }
                Culturelist.Sort();
                res.Status = true;
                res.Message = "get list";
                res.Data = Culturelist;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This Api use For Get Country

        #region add project by client end

        /// <summary>
        /// API >> Post >> api/project/CreateProject
        /// Craeted by Shriya on 01-06-2022
        /// Modified byShriya on 02-06-2022
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        [Route("CreateProject")]
        [HttpPost]
        public async Task<ResponseBodyModel> CreateProject(List<ProjectModalList> Project)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (Project != null)
                {
                    var clientId = Project.First().ClientId;
                    var exitingProject = _db.Project.Where(x => x.ClientId == clientId).ToList();
                    if (exitingProject.Count > 0)
                    {
                        _db.Project.RemoveRange(exitingProject);
                        // db.Entry(exitingProject).State = EntityState.Deleted;
                        _db.SaveChanges();
                    }

                    foreach (var item in Project)
                    {
                        Project projectData = new Project();
                        projectData.ProjectName = item.ProjectName;
                        //projectData.ProjectTypeId = item.ProjectTypeId;
                        //projectData.BillTypeId = item.BillTypeId;
                        //projectData.EstimatedDays = item.EstimatedDays;
                        projectData.ClientId = item.ClientId;
                        //projectData.StatusId = item.StatusId;
                        //projectData.ProjectOwner = item.ProjectOwner;
                        //projectData.EmployeeId = item.EmployeeId;
                        //projectData.ActualStartDate = item.ActualStartDate;
                        //projectData.ActualEndDate = item.ActualEndDate;
                        projectData.ProjectPrice = item.ProjectPrice;
                        projectData.CreatedOn = DateTime.Now;
                        projectData.CreatedBy = claims.employeeId;
                        projectData.IsDeleted = false;
                        projectData.IsActive = true;
                        projectData.CompanyId = claims.companyId;
                        projectData.OrgId = claims.orgId;
                        _db.Project.Add(projectData);
                        await _db.SaveChangesAsync();
                    }
                    res.Status = true;
                    res.Message = "Project Saved Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = "add something wrong";
                }
                //List<int> allKeys = (from kvp in Project select kvp.ClientId).Distinct();
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion add project by client end

        #region get By ID // Get project on behalf of client ID

        /// <summary>
        /// Created By Suraj Bundel On 31/05/2022
        /// Modified By Shriya On 01-06-2022
        /// Get project on behalf of client ID
        /// Get by id -> Api -> api/project/getprojectbyclientid
        /// </summary>
        /// <return></return>
        [Route("getprojectbyclientid")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetProjectbyClientid(int clientid)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                List<ProjectModalList> projectdatalist = new List<ProjectModalList>();
                var getobj = await _db.Project.Where(x => x.IsActive == true && x.IsDeleted == false && x.ClientId == clientid).ToListAsync();
                foreach (var item in getobj)
                {
                    ProjectModalList projectdataObj = new ProjectModalList();
                    projectdataObj.ProjectId = item.ProjectId;
                    projectdataObj.ProjectName = item.ProjectName;
                    projectdataObj.ClientId = item.ClientId;
                    projectdataObj.ProjectPrice = item.ProjectPrice;
                    //projectdataObj.ActualStartDate = item.ActualStartDate;
                    //projectdataObj.ActualEndDate = item.ActualEndDate;
                    //  projectdataObj.ProjectCount = projects.Where(x => x.ClientId == item.ClientId).Count();
                    projectdatalist.Add(projectdataObj);
                }
                if (projectdatalist != null)
                {
                    response.Status = true;
                    response.Message = "Record Found";
                    response.Data = projectdatalist;
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Record Found!";
                    response.Data = projectdatalist;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }
            return response;
        }

        #endregion get By ID // Get project on behalf of client ID

        #region Get resource detail by Assign project Id

        /// <summary>
        /// Created By Shriya Malvi On 23-06-2022
        /// API >> Get >> api/project/getresourcebyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getresourcebyid")]
        public async Task<ResponseBodyModel> GetResourceById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitingResource = await (from ad in _db.AssignProjects
                                             join fd in _db.Employee on ad.EmployeeId equals fd.EmployeeId
                                             join pr in _db.EmployeeRoleInProjects on ad.EmployeeRoleInProjectId equals pr.EmployeeRoleInProjectId into r
                                             from result in r.DefaultIfEmpty()
                                             where ad.IsActive == true && ad.IsDeleted == false &&
                                              ad.CompanyId == claims.companyId && ad.AssignProjectId == Id
                                             select new GetHelperForResource
                                             {
                                                 AssignProjectId = ad.AssignProjectId,
                                                 EmployeeId = ad.EmployeeId,
                                                 EmployeeName = fd.DisplayName,
                                                 RoleName = result == null ? null : result.RoleName,
                                                 RoleId = result == null ? Guid.Empty : result.EmployeeRoleInProjectId,
                                                 Status = ad.Status,
                                                 //EmployeeRoleInProjectConstantId = ad.EmployeeRoleInProjectConstant,
                                                 //EmployeeRoleInProjectConstant = ad.EmployeeRoleInProjectConstant.ToString()
                                             }).FirstOrDefaultAsync();
                if (exitingResource != null)
                {
                    res.Status = true;
                    res.Message = "Resource List Found";
                    res.Data = exitingResource;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Resource List Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get resource detail by Assign project Id

        #region Update resource detail by
        /// <summary>
        /// Create By Shriya Malvi On 23-06-2022
        /// API >> Put >> api/project/updateresourcedetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateresourcedetail")]
        public async Task<ResponseBodyModel> UpdateResourceDetail(GetHelperForResource model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitingdata = await _db.AssignProjects.Where(x => x.IsActive == true && x.IsDeleted == false && x.AssignProjectId == model.AssignProjectId).FirstOrDefaultAsync();
                if (exitingdata != null)
                {
                    exitingdata.Status = model.Status;
                    exitingdata.EmployeeId = model.EmployeeId;
                    exitingdata.UpdatedBy = claims.employeeId;
                    exitingdata.UpdatedOn = DateTime.Now;
                    exitingdata.EmployeeRoleInProjectId = model.RoleId;
                    // exitingdata.EmployeeRoleInProjectConstant = model.EmployeeRoleInProjectConstantId;
                    _db.Entry(exitingdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "resource detail Updated";
                    res.Data = exitingdata;
                }
                else
                {
                    res.Status = false;
                    res.Message = "resource detail not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update resource detail by

        #region Get Project By Project Id
        /// <summary>
        /// Create By Shriya Malvi On 24-06-2022
        /// </summary>
        /// API >> Get >> api/project/getbyprojectid
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyprojectid")]
        public async Task<IHttpActionResult> GetByProjectId(int Id, int? subProjectId = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var project = await (from ad in _db.ProjectLists
                                     join fd in _db.Employee on ad.ProjectManager equals fd.EmployeeId
                                     where ad.IsActive == true && ad.IsDeleted == false &&
                                     ad.CompanyId == claims.companyId
                                     where ((subProjectId.HasValue) ? ad.ID == subProjectId : ad.ID == Id)
                                     select new AddProjectModal
                                     {
                                         ID = ad.ID,
                                         ProjectName = ad.ProjectName,
                                         ProjectManager = ad.ProjectManager,
                                         PMName = fd.DisplayName,
                                         CampanyName = ad.CampanyName,
                                         PaymentMethod = "Monthly",
                                         LeadType = ad.LeadType,
                                         FromCurrency = ad.FromCurrency,
                                         ToCurrency = ad.ToCurrency,
                                         ClientAmount = ad.ClientAmount,
                                         ClientConvertedAmt = ad.ClientConvertedAmt,
                                         StartDate = ad.StartDate,
                                         EndDate = ad.EndDate,
                                         tech = ad.Technology,
                                         ProjectDiscription = ad.ProjectDiscription,
                                         LinkJson = ad.LinkJson,
                                     }).ToListAsync();
                AddProjectModal pdetail = new AddProjectModal();
                foreach (var item in project)
                {
                    pdetail.ID = item.ID;
                    pdetail.ProjectName = item.ProjectName;
                    pdetail.ProjectManager = item.ProjectManager;
                    pdetail.PMName = item.PMName;
                    pdetail.CampanyName = item.CampanyName;
                    pdetail.PaymentMethod = item.PaymentMethod;
                    pdetail.LeadType = item.LeadType;
                    pdetail.FromCurrency = item.FromCurrency;
                    pdetail.ToCurrency = item.ToCurrency;
                    pdetail.ClientAmount = item.ClientAmount;
                    pdetail.Links = JsonConvert.DeserializeObject<List<RepolinkReponse>>(item.LinkJson);
                    //pdetail.Links = pdetail.Links.Count == 0 ? new List<RepolinkReponse>() : JsonConvert.DeserializeObject<List<RepolinkReponse>>(item.LinkJson);
                    pdetail.ClientConvertedAmt = item.ClientConvertedAmt;
                    pdetail.StartDate = item.StartDate;
                    pdetail.EndDate = item.EndDate;
                    pdetail.ProjectDiscription = item.ProjectDiscription;
                    // pdetail.Links = item.Links;
                    var technology = item.tech.Split(',');
                    List<int> ListStage = new List<int>();
                    foreach (var tech in technology)
                    {
                        var i = Convert.ToInt32(tech);
                        var techno = await _db.Technology.Where(x => x.TechnologyId == i).Select(x => x.TechnologyId).FirstOrDefaultAsync();
                        if (techno != 0)
                            ListStage.Add(techno);
                    }
                    //ProjectListObj.TechnologyId = item.TechnologyId;
                    pdetail.Technology = ListStage;
                }
                if (project != null)
                {
                    res.Message = "project detail found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = pdetail;
                    return Ok(res);
                }
                else
                {
                    res.Status = false;
                    res.Message = "project  deatil not found";
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = pdetail;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }

        #endregion Get Project By Project Id

        #region Helper Class for Current running api

        public class AddProjectModal
        {
            public int ID { get; set; } = 0;
            public string ProjectName { get; set; }
            public int ProjectManager { get; set; }
            public string PMName { get; set; }
            public string CampanyName { get; set; }
            public List<int> Technology { get; set; }

            public string PaymentType { get; set; }
            public string LeadType { get; set; }
            public string Others { get; set; }
            public string PaymentMethod { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
            public double ClientAmount { get; set; }
            public string ExchangeDate { get; set; }
            public string ProjectDiscription { get; set; }
            public decimal ExchangeRate { get; set; }
            public string tech { get; set; }
            public int SubProjectId { get; set; } = 0;
            public double ClientConvertedAmt { get; set; }
            public DateTimeOffset? StartDate { get; set; }
            public DateTimeOffset? EndDate { get; set; }
            public int CompanyId { get; set; }
            public string LinkJson { get; set; }
            public List<RepolinkReponse> Links { get; set; } = new List<RepolinkReponse>();
        }

        public class RepolinkReponse
        {
            //public int ProjectId { get; set; } = 0;
            public string LinkName { get; set; } = string.Empty;
            public string Discription { get; set; } = string.Empty;
            public string LinkUrl { get; set; } = string.Empty;

        }

        public class ProjectModalList
        {
            public int ClientId { get; set; }
            public int? ProjectId { get; set; }
            public string ProjectName { get; set; }

            //public decimal? EstimatedDays { get; set; }
            public double ProjectPrice { get; set; }
        }

        /// <summary>
        /// Create by shriya On 10-06-2022
        /// </summary>
        public class AssignProjectModal
        {
            public int ProjectId { get; set; }
            public List<int> EmployeeId { get; set; }
            public string Status { get; set; }
            public Guid EmployeeRoleInProjectId { get; set; } = Guid.Empty;

            //public EmployeeRoleInProjectConstant EmployeeRoleInProjectConstant { get; set; }
        }

        public class ProjectdataList
        {
            public int ID { get; set; }
            public string ProjectName { get; set; }

            //public int ProjectManager { get; set; }
            public string ProjectManager { get; set; }

            public int EmployeeId { get; set; }
            public string ClientBillableAmount { get; set; }
            public double SumofBillableSalary { get; set; }
            public double SumofNonBillableSalary { get; set; }
            public double billAndNonBilEmpSalary { get; set; }
            public double AmountDiffrence { get; set; }
            public string ProjectHealth { get; set; }
            public string PaymentType { get; set; }
            public int resourceCount { get; set; }
            public string CampanyName { get; set; }
            public int Currency { get; set; }
            public int Technology { get; set; }
            public int TechnologyId { get; set; }
            public int CurrencyId { get; set; }
            public object TechnologyType { get; set; }
            public string LeadType { get; set; }
            public string CurrencyName { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public int CreatedBy { get; set; }
            public string ProjectCreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public string LastUpdatedBy { get; set; }
            public bool IsMailSendToday { get; set; }
            public string ProjectStatus { get; set; }
            public ProjectStatusConstants ProjectStatusId { get; set; }
            public DateTimeOffset ProjectCreatedDate { get; set; }
        }

        public class GetProjectHelperClass
        {
            public int ID { get; set; }
            public string ProjectName { get; set; }
            public string Manger { get; set; }
            public string DisplayName { get; set; }
            public int ProjectManager { get; set; }
            public int EmployeeId { get; set; }
            public int ClientBillableAmount { get; set; }
            public string CampanyName { get; set; }
            public string Technology { get; set; }
            public string crc { get; set; }
            public string PaymentType { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public int CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public string LeadType { get; set; }
            public bool IsMailSendToday { get; set; }
            public ProjectStatusConstants ProjectStatusId { get; set; }
            public string ProjectStatus { get; set; }
            public DateTimeOffset ProjectCreatedDate { get; set; }
        }

        public class GetAllProjectHelperModel
        {
            public List<ProjectdataList> ProjectList { get; set; }
            public string ClientBillableAmount { get; set; }
            public string SumofBillableSalary { get; set; }
            public string SumofNonBillableSalary { get; set; }
            public string BillAndNonBilEmpSalary { get; set; }
            public double AmountDiffrence { get; set; }
            public string Profit { get; set; }
            public string Loss { get; set; }
        }

        public class GetHelperForResource
        {
            public int AssignProjectId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Status { get; set; }
            public string RoleName { get; set; }
            public Guid RoleId { get; set; } = Guid.Empty;
            // public EmployeeRoleInProjectConstant EmployeeRoleInProjectConstantId { get; set; }
        }

        #endregion Helper Class for Current running api

        #region Api To Get Employee Occupacy
        /// <summary>
        /// Created By Harshit Mitra on 06-07-0=2022
        /// API>>Get>>api/project/getemployeeoccupacy
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeeoccupacy")]
        public async Task<ResponseBodyModel> GetEmployeeOccupacy()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var empInProject = await _db.AssignProjects.Where(x => x.CompanyId == claims.companyId &&
                            x.IsActive && !x.IsDeleted).ToListAsync();
                var empInProjectList = empInProject.Select(x => x.EmployeeId).ToList();
                var empList = await _db.Employee.Where(x => x.CompanyId == claims.companyId && x.OrgId != 0 &&
                                x.IsActive && !x.IsDeleted && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).ToListAsync();
                var getEmployee = empList
                        .Select(x => new GetUtilizationProjectList
                        {
                            EmployeeId = x.EmployeeId,
                            DisplayName = x.DisplayName,
                            ProjectId = empInProject.Where(e => e.EmployeeId == x.EmployeeId).Select(e => e.ProjectId).ToList(),
                            UsePercent = empInProjectList.Contains(x.EmployeeId) ?
                                         100 / empInProjectList.Where(e => e == x.EmployeeId).ToList().Count : 0,
                            UserGrouping = EmployeeUtilizationConstants.No_Project_Assign,
                        }).ToList();
                var projectList = await _db.ProjectLists.Where(x => x.CompanyId == claims.companyId &&
                            x.IsActive == true && x.IsDeleted == false).ToListAsync();
                getEmployee = getEmployee
                        .Select(x => new GetUtilizationProjectList
                        {
                            EmployeeId = x.EmployeeId,
                            DisplayName = x.DisplayName,
                            UsePercent = x.UsePercent,
                            ProjectId = x.ProjectId,
                            ProjectName = String.Join(",", projectList.Where(p => x.ProjectId.Contains(p.ID)).Select(p => p.ProjectName).ToList()),
                            UserGrouping = x.UsePercent == 0 ? EmployeeUtilizationConstants.No_Project_Assign :
                                    (x.UsePercent > 52) ? EmployeeUtilizationConstants.Free_Employee :
                                    (x.UsePercent > 26) ? EmployeeUtilizationConstants.Under_Utilized :
                                    EmployeeUtilizationConstants.Over_Utilized,
                        }).ToList();

                var utlizationList = Enum.GetValues(typeof(EmployeeUtilizationConstants))
                               .Cast<EmployeeUtilizationConstants>()
                               .Select(x => new
                               {
                                   UtlizationId = (int)x,
                                   EmployeeUtlization = Enum.GetName(typeof(EmployeeUtilizationConstants), x).Replace("_", " "),
                                   getEmployee.Where(e => e.UserGrouping == x).ToList().Count,
                                   EmployeeList = getEmployee.Where(e => e.UserGrouping == x).ToList(),
                               }).ToList();
                res.Message = "Utlization List";
                res.Status = true;
                res.Data = utlizationList;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class GetUtilizationProjectList
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public int UsePercent { get; set; }
            public List<int> ProjectId { get; set; }
            public string ProjectName { get; set; }
            public EmployeeUtilizationConstants UserGrouping { get; set; }
        }

        #endregion Api To Get Employee Occupacy

        #region Api for Update Project Status 
        /// <summary>
        /// API>>POST>>api/project/updateprojectstatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateprojectstatus")]
        public async Task<IHttpActionResult> UpdateProjectStatus(ProjectStatusResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = _db.ProjectLists
                              .Where(x => x.IsActive && !x.IsDeleted &&
                              x.CompanyId == tokenData.companyId &&
                              x.ID == model.ProjectId)
                              .FirstOrDefault();
                if (getData != null)
                {
                    getData.ProjectStatus = model.ProjectStatus;
                    getData.ProjectStatusDiscription = model.ProjectStatusDiscription;
                    getData.ProjectAttachment = model.ProjectAttachment;
                    getData.UpdatedBy = tokenData.employeeId;
                    getData.UpdatedOn = DateTime.Now;

                    _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Project Status Updated Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Project data Not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/project/updateprojectstatus", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        public class ProjectStatusResponse
        {
            public int ProjectId { get; set; }
            public string ProjectStatusDiscription { get; set; }
            public string ProjectAttachment { get; set; }
            public ProjectStatusConstants ProjectStatus { get; set; }

        }

        #endregion

        #region GAT ALL PROJECT STATUS   // dropdown
        /// <summary>
        /// Created By Ravi Vyas on 17-01-2023
        /// API >> Get >>api/project/getallprojectstatusenum
        /// </summary>
        [Route("getallprojectstatusenum")]
        [HttpGet]
        public ResponseBodyModel TaskStatus()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getStatus = Enum.GetValues(typeof(ProjectStatusConstants))
                    .Cast<TaskStatusConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(ProjectStatusConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Status Get Succesfully";
                res.Status = true;
                res.Data = getStatus;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Reason for resignation // dropdown

        #endregion Runing Api section of Project Master

        #region Api for All Employee List When we assign PM
        /// <summary>
        /// Created By Ravi Vyas on 21-12-2022
        /// API>>api/project/getallemployeeassigntopm
        /// </summary>
        /// <param name="serach"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeeassigntopm")]

        public async Task<IHttpActionResult> SerachEmployee()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await _db.Employee.Where(x => x.IsDeleted == false && x.IsActive == true &&
                x.CompanyId == tokenData.companyId && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                ).Select(x => new
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.DisplayName,
                }).ToListAsync();

                if (data.Count > 0)
                {
                    res.Message = "Employee Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = data;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Employee Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = data;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/project/getallemployeeassigntopm", ex.Message);
                return BadRequest("Failed");
            }
        }


        #endregion

        #region seat Cost API

        #region Add and update Cost

        /// <summary>
        ///  Created By Shriya Malvi On 22-06-2022
        ///  API> Post >> api/project/addseatcost
        ///  This for add and edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addseatcost")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddSeatCost(GSeatCost model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var ExitingData = await _db.GSeatCosts.Where(x => x.IsActive == true && x.IsDeleted == false
                                            && x.CompanyId == claims.companyId && x.GSeatCostId == model.GSeatCostId
                                            && x.Update == false).FirstOrDefaultAsync();

                    if (ExitingData == null)
                    {
                        var procost = _db.GSeatCosts.Where(x => x.IsActive == true && x.IsDeleted == false
                        && x.CompanyId == claims.companyId && x.Create == true).ToList();
                        if (procost.Count > 0)
                        {
                            res.Status = false;
                            res.Message = "It add only once";
                        }
                        else
                        {
                            GSeatCost gseatcost = new GSeatCost();
                            gseatcost.GlobalAmount = model.GlobalAmount;
                            gseatcost.Update = false;
                            gseatcost.CompanyId = claims.companyId;
                            gseatcost.OrgId = claims.orgId;
                            gseatcost.CreatedBy = claims.employeeId;
                            gseatcost.CreatedOn = DateTime.Now;
                            gseatcost.IsActive = true;
                            gseatcost.IsDeleted = false;
                            gseatcost.Create = true;
                            _db.GSeatCosts.Add(gseatcost);
                            _db.SaveChanges();

                            res.Status = true;
                            res.Data = gseatcost;
                            res.Message = "Seat Cost Added";
                        }
                    }
                    else
                    {
                        if (ExitingData.GlobalAmount == model.GlobalAmount)
                        {
                            res.Message = "You Inputed Same Amount as Last Seat Cost";
                            res.Status = false;
                        }
                        else
                        {
                            ExitingData.Update = true;
                            ExitingData.UpdatedOn = DateTime.Now;
                            ExitingData.UpdatedBy = claims.employeeId;
                            _db.Entry(ExitingData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            GSeatCost gseatcost = new GSeatCost();
                            gseatcost.GlobalAmount = model.GlobalAmount;
                            gseatcost.Update = false;
                            gseatcost.CompanyId = claims.companyId;
                            gseatcost.OrgId = claims.orgId;
                            gseatcost.CreatedBy = claims.employeeId;
                            gseatcost.CreatedOn = DateTime.Now;
                            gseatcost.IsActive = true;
                            gseatcost.IsDeleted = false;
                            _db.GSeatCosts.Add(gseatcost);
                            _db.SaveChanges();
                            res.Status = true;
                            res.Data = gseatcost;
                            res.Message = "Seat Cost Updated";
                        }
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Seat Cost Not Add";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Add and update Cost

        #region Get Global seat cost updatable row

        /// <summary>
        /// API >> Get >> api/project/getseatcosthistory
        /// Created by Shriya Malvi On 22-06-2022
        /// </summary>
        [HttpGet]
        [Route("getseatcosthistory")]
        public async Task<ResponseBodyModel> GetSeatCostHistory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<GetHelperSeatCost> DataCost = new List<GetHelperSeatCost>();
            try
            {
                DataCost = await (from ad in _db.GSeatCosts
                                  join fd in _db.Employee on ad.UpdatedBy equals fd.EmployeeId
                                  where ad.IsActive && !ad.IsDeleted &&
                                  ad.CompanyId == claims.companyId && ad.Update
                                  select new GetHelperSeatCost
                                  {
                                      GSeatCostId = ad.GSeatCostId,
                                      GlobalAmount = ad.GlobalAmount,
                                      Update = ad.Update,
                                      UpdateDate = ad.UpdatedOn.HasValue ? (DateTime)ad.UpdatedOn : ad.CreatedOn,
                                      UpdateBy = ad.UpdatedBy,
                                      UpdateByName = fd.DisplayName,
                                  }).ToListAsync();

                if (DataCost.Count > 0)
                {
                    res.Data = DataCost.OrderByDescending(x => x.UpdateDate).ToList();
                    res.Status = true;
                    res.Message = "Seat Cost History Found";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Seat Cost History Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Global seat cost updatable row

        #region Get all Global Seat Cost Amount of projects
        /// <summary>
        /// API >> Get >> api/project/getscupdateable
        /// Created bY Shriya Malvi On 22-06-2022
        /// </summary>

        /// <returns></returns>
        [Route("getscupdateable")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetSCUpdateable()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<GetHelperSeatCost> DataCost = new List<GetHelperSeatCost>();
            try
            {
                if (claims.roleType == "Administrator")
                {
                    DataCost = await (from ad in _db.GSeatCosts
                                      join fd in _db.Employee on ad.CreatedBy equals fd.EmployeeId
                                      where ad.IsActive == true && ad.IsDeleted == false &&
                                                 ad.CompanyId == claims.companyId && ad.Update == false
                                      select new GetHelperSeatCost
                                      {
                                          GSeatCostId = ad.GSeatCostId,
                                          GlobalAmount = ad.GlobalAmount,
                                          Update = ad.Update,
                                          UpdateDate = ad.UpdatedOn == null ? ad.CreatedOn : ad.UpdatedOn,
                                          UpdateBy = ad.CreatedBy,
                                          UpdateByName = fd.DisplayName
                                      }).ToListAsync();
                    if (DataCost.Count > 0)
                    {
                        res.Data = DataCost;
                        res.Status = true;
                        res.Message = "Seat Cost List Found";
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "Seat Cost List Not Found";
                    }
                }
                else
                {
                    DataCost = (from ad in _db.GSeatCosts
                                join fd in _db.Employee on ad.CreatedBy equals fd.EmployeeId
                                where ad.IsActive == true && ad.IsDeleted == false &&
                                           ad.CompanyId == claims.companyId && ad.Update == false
                                select new GetHelperSeatCost
                                {
                                    GSeatCostId = ad.GSeatCostId,
                                    GlobalAmount = ad.GlobalAmount,
                                    Update = ad.Update,
                                    UpdateDate = ad.UpdatedOn == null ? ad.CreatedOn : ad.UpdatedOn,
                                    UpdateBy = ad.CreatedBy,
                                    UpdateByName = fd.DisplayName
                                }).ToList();

                    if (DataCost.Count > 0)
                    {
                        res.Data = DataCost;
                        res.Status = true;
                        res.Message = "Seat Cost List Found";
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "Seat Cost List Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get all Global Seat Cost Amount of projects

        #region Get BY ID

        /// <summary>
        /// Created By Shriya Malvi On 22-06-2022
        /// API >> GeT >> api/project/GetBySeat
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getseatcostid")]
        public async Task<ResponseBodyModel> GetSeatCostId(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var costdata = await _db.GSeatCosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.GSeatCostId == Id).Select(x => new GetHelperForSeatCostById
                {
                    GSeatCostId = x.GSeatCostId,
                    GlobalAmount = x.GlobalAmount,
                    Update = x.Update
                }).FirstOrDefaultAsync();
                if (costdata != null)
                {
                    res.Data = costdata;
                    res.Status = true;
                    res.Message = "List Found";
                }
                else
                {
                    res.Message = "list Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get BY ID

        #region HelperModel for Global seat Cost

        public class GetHelperSeatCost
        {
            public int GSeatCostId { get; set; }
            public double GlobalAmount { get; set; }
            public bool Update { get; set; }
            public DateTime? UpdateDate { get; set; }
            public int? UpdateBy { get; set; }
            public string UpdateByName { get; set; }
        }

        public class GetHelperForSeatCostById
        {
            public int GSeatCostId { get; set; }
            public double GlobalAmount { get; set; }
            public bool Update { get; set; }
        }

        #endregion HelperModel for Global seat Cost

        #endregion seat Cost API

        #region API To Convert Currency

        /// <summary>
        /// Created By Harshit Mitra on 24-06-2022
        /// API >> Get >> api/project/convertcurrency
        /// </summary->
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("convertcurrency")]
        public ResponseBodyModel CurrencyConversion(CurrencyConvertHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                HelperCurrency currency = new HelperCurrency();
                string urlPattern = "https://v6.exchangerate-api.com/v6/37adbbc7e62308915c7f51bb/latest/{0}";
                string url = string.Format(urlPattern, model.FromCurrency);
                using (var wc = new WebClient())
                {
                    var json = wc.DownloadString(url);

                    Newtonsoft.Json.Linq.JToken token = Newtonsoft.Json.Linq.JObject.Parse(json);
                    var exchangeRate = token.SelectToken("conversion_rates");
                    var toCurrencyRate = (decimal)exchangeRate.SelectToken(model.ToCurrency);

                    var exchangeAmount = model.Amount * toCurrencyRate;
                    var date = token.SelectToken("time_last_update_utc");

                    currency.ExchangeAmount = exchangeAmount;
                    currency.DateOfExchange = Convert.ToString(date);
                    currency.ExchangeRate = toCurrencyRate;

                    res.Message = "Currency Converted";
                    res.Status = true;
                    res.Data = currency;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public class CurrencyConvertHelper
        {
            public decimal Amount { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
        }

        public class HelperCurrency
        {
            public decimal ExchangeAmount { get; set; }
            public string DateOfExchange { get; set; }
            public decimal ExchangeRate { get; set; }
        }

        #endregion API To Convert Currency

        #region Projects
        #endregion Projects

        //admin
        #region GetProject

        [Route("GetProject")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetProject()
        {
            try
            {
                List<ProjectData> projectDataList = new List<ProjectData>();
                var finalData = (from ad in _db.Project
                                     //join td in db.Technology on ad.TechnologyId equals td.TechnologyId
                                 join sd in _db.Status on ad.StatusId equals sd.statusId
                                 join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                 join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                 join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                 orderby ad.ProjectId descending
                                 where ad.IsDeleted == false
                                 select new
                                 {
                                     ad.ProjectId,
                                     ad.ProjectName,
                                     ad.ActualStartDate,
                                     ad.ActualEndDate,
                                     // td.TechnologyType,
                                     sd.StatusVal,
                                     ptd.Project_Type,
                                     btd.Bill_Type,
                                     v = etd.FirstName + " " + etd.LastName,
                                     ad.StatusId,
                                     //ad.TechnologyId,
                                     ad.ProjectTypeId,
                                     ad.BillTypeId,
                                     ad.EmployeeId
                                 }).ToList();
                foreach (var item in finalData)
                {
                    ProjectData data = new ProjectData();

                    data.ProjectName = item.ProjectName;
                    data.ActualStartDate = item.ActualStartDate;
                    data.ActualEndDate = item.ActualEndDate;
                    //data.TechnologyType = item.TechnologyType;
                    data.StatusVal = item.StatusVal;
                    data.Project_Type = item.Project_Type;
                    data.Bill_Type = item.Bill_Type;
                    data.FullName = item.v;
                    data.ProjectId = item.ProjectId;
                    data.StatusId = item.StatusId;
                    // data.TechnologyId = item.TechnologyId;
                    data.ProjectTypeId = item.ProjectTypeId;
                    data.BillTypeId = item.BillTypeId;
                    data.EmployeeId = item.EmployeeId;
                    projectDataList.Add(data);
                }
                return Ok(projectDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetProject

        #region GetAllProjectBasedOnId

        //for other role
        [Route("GetAllProjectBasedOnId")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetAllProjectBasedOnId(int Id)
        {
            try
            {
                var checkRole = (from ad in _db.Employee where ad.EmployeeId == Id select ad).FirstOrDefault();
                if (checkRole == null)
                {
                    return Ok();
                }
                else
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    var finalData = (from ld in _db.Team
                                     join ad in _db.Project on ld.ProjectId equals ad.ProjectId
                                     //join td in db.Technology on ad.TechnologyId equals td.TechnologyId
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ld.TeamMemberId == Id && ld.IsDelete == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         //td.TechnologyType,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         //ad.TechnologyId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        ProjectData data = new ProjectData();

                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        // data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }

                    return Ok(projectDataList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetAllProjectBasedOnId

        #region GetProjectByFilter

        [Route("GetProjectByFilter")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult GetProjectByFilter(ProjectFilter project)
        {
            try
            {
                List<ProjectData> projectDataList = new List<ProjectData>();
                ProjectData data = new ProjectData();
                if (project.projectManager != 0)
                {
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                }
                if (project.projectStatus != 0)
                {
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.StatusId == project.projectStatus
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                }
                if (project.billType != 0)
                {
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.BillTypeId == project.billType
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                }
                if (project.projectType != 0)
                {
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.ProjectTypeId == project.projectType
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        // data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                }
                if (project.projectTechnology != 0)
                {
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        //data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                }

                //var finalList = projectDataList.Select(x => x.ProjectId).Distinct();

                //var finalList = projectDataList.Distinct(i => i.ProjectId).

                // projectDataList = projectDataList.DistinctBy(i => i.Name).DistinctBy(i => i.ProductId).ToList();

                //IEnumerable<ProjectData> distinctCars = projectDataList.DistinctBy(car => car.CarCode);

                //Notes.Select(x => x.Author).Distinct();

                // List<ProjectData> distinct = projectDataList.Distinct().ToList();
                return Ok(projectDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetProjectByFilter

        #region ProjectDetails

        [Route("ProjectDetails")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult ProjectDetails()
        {
            try
            {
                Base response = new Base();
                var totalProjects = _db.Project.Where(x => x.IsDeleted == false).Count();
                var completedProject = _db.Project.Where(x => x.StatusId == 5 && x.IsDeleted == false).Count();
                var activeProject = _db.Project.Where(x => x.StatusId == 4 && x.IsDeleted == false).Count();
                var totalProjectManagers = _db.Employee.Where(x => x.RoleId == 21 && x.IsDeleted == false).Count();
                var totalProjectCoordinator = _db.Employee.Where(x => x.RoleId == 19 && x.IsDeleted == false).Count();
                response.StatusReason = true;
                response.totalProjects = totalProjects;
                response.completedProjects = completedProject;
                response.activeProjects = activeProject;
                response.totalProjectManagers = totalProjectManagers;
                response.totalProjectCoordinator = totalProjectCoordinator;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion ProjectDetails

        #region GetTeamByProjectId

        [Route("GetTeamByProjectId")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetTeamByProjectId(int Id)
        {
            try
            {
                List<ProjectData> teamDataList = new List<ProjectData>();
                var teamData = (from ad in _db.Project
                                join bd in _db.Team on ad.ProjectId equals bd.ProjectId
                                join cd in _db.Employee on bd.TeamMemberId equals cd.EmployeeId
                                join td in _db.Role on cd.RoleId equals td.RoleId
                                where ad.ProjectId == Id
                                select new
                                {
                                    ad.ProjectId,
                                    bd.TeamId,
                                    ad.ProjectName,
                                    v = cd.FirstName + " " + cd.LastName,
                                    cd.PersonalEmail,
                                    td.RoleType,
                                    bd.IsActive,
                                    bd.IsDelete
                                }).ToList();
                foreach (var item in teamData)
                {
                    ProjectData data = new ProjectData();

                    data.ProjectId = item.ProjectId;
                    data.ProjectName = item.ProjectName;
                    data.FullName = item.v;
                    data.Email = item.PersonalEmail;
                    data.RoleType = item.RoleType;
                    data.TeamId = item.TeamId;
                    data.IsActive = item.IsActive;
                    data.IsDeleted = item.IsDelete;
                    teamDataList.Add(data);
                }
                return Ok(teamDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetTeamByProjectId

        //#region GetProjectByEmployeeId

        //[Route("GetProjectByEmployeeId")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetProjectByEmployeeId(int Id)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<ProjectData> teamDataList = new List<ProjectData>();
        //        var teamData = (from ad in _db.Project
        //                        join bd in _db.Team on ad.ProjectId equals bd.ProjectId
        //                        join cd in _db.Employee on ad.EmployeeId equals cd.EmployeeId
        //                        where bd.TeamMemberId == Id && bd.IsDelete == false
        //                        select new
        //                        {
        //                            ad.ProjectName,
        //                            v = cd.FirstName + " " + cd.LastName
        //                        }).ToList();
        //        foreach (var item in teamData)
        //        {
        //            ProjectData data = new ProjectData();

        //            data.ProjectName = item.ProjectName;
        //            data.FullName = item.v;
        //            teamDataList.Add(data);
        //        }
        //        if (teamDataList.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.employeeProject = teamDataList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion GetProjectByEmployeeId

        #region GetProjectName

        [Route("GetProjectName")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetProjectName(int Id)
        {
            try
            {
                Base response = new Base();
                var projectName = _db.Project.Where(x => x.ProjectId == Id && x.IsDeleted == false).FirstOrDefault();
                if (projectName != null)
                {
                    response.StatusReason = true;
                    response.Message = "Success";
                    response.projectAssociation = projectName;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Failed";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetProjectName

        //#region GetTaskByProjectId
        //[Route("GetTaskByProjectId")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetTaskByProjectId(int projectid)
        //{
        //    try
        //    {
        //        List<ProjectData> taskDataList = new List<ProjectData>();
        //        var teamData = (from ad in _db.Project
        //                        join bd in _db.Task on ad.ProjectId equals bd.ProjectId
        //                        join cd in _db.Employee on bd.AssignedToId equals cd.EmployeeId
        //                        join sd in _db.Employee on bd.AssignedById equals sd.EmployeeId
        //                        join td in _db.Status on bd.TaskStatusId equals td.statusId
        //                        where ad.ProjectId == projectid && bd.IsDeleted == false &&
        //                        ad.CompanyId == claims.companyid && ad.OrgId == Global.Gblemployeeid
        //                        select new
        //                        {
        //                            ad.ProjectId,
        //                            ad.ProjectName,
        //                            bd.Tasks,
        //                            bd.AssignedById,
        //                            bd.Description,
        //                            bd.CreatedOn,
        //                            td.StatusVal,
        //                            s = cd.FirstName + "" + cd.LastName,
        //                            v = sd.FirstName + "" + sd.LastName,
        //                        }).ToList();

        //        foreach (var item in teamData)
        //        {
        //            ProjectData data = new ProjectData();

        //            data.ProjectId = item.ProjectId;
        //            data.ProjectName = item.ProjectName;

        //            data.AssignedToName = item.s;
        //            data.AssignedByName = item.v;
        //            data.Description = item.Description;
        //            data.CreatedOn = item.CreatedOn;
        //            data.TaskStatus = item.StatusVal;
        //            data.Task = item.Tasks;
        //            taskDataList.Add(data);
        //        }
        //        return Ok(taskDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //#endregion

        #region GetProjectById

        [Route("GetProjectById")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetProjectById(int Id)
        {
            try
            {
                Base response = new Base();
                var projectData = _db.Project.Where(x => x.ProjectId == Id && x.IsDeleted == false).FirstOrDefault();
                if (projectData != null)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.projectAssociation = projectData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetProjectById

        #region ActiveStatus

        [Route("ActiveStatus")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult ActiveStatus(AssignProject AssignProject)
        {
            try
            {
                Base response = new Base();
                var ProjectUpdate = _db.AssignProjects.Where(x => x.ProjectId == AssignProject.ProjectId && x.EmployeeId == AssignProject.EmployeeId).FirstOrDefault();
                if (ProjectUpdate.IsActive == true)
                {
                    ProjectUpdate.IsDeleted = true;
                    ProjectUpdate.IsActive = false;
                    _db.SaveChanges();
                }
                else
                {
                    ProjectUpdate.IsDeleted = false;
                    ProjectUpdate.IsActive = true;
                    _db.SaveChanges();
                }

                response.StatusReason = true;
                //db.AssignProjects.update
                //db.SaveChanges();
                response.Message = "Status Updated";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion ActiveStatus

        #region AssignProject

        [Route("AssignProject")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AssignProject(AssignProject AssignProject)
        {
            try
            {
                Base response = new Base();
                var count = 0;
                //foreach (var emp in AssignProjectdata.Employee)
                //{
                //  if(AssignProject.EmployeeId == EmplyeeId && item == AssignProject.ProjectId)
                var checkEmp = _db.AssignProjects.Where(x => x.ProjectId == AssignProject.ProjectId && x.EmployeeId == AssignProject.EmployeeId).FirstOrDefault();
                if (checkEmp == null)
                {
                    AssignProject data = new AssignProject();
                    data.EmployeeId = AssignProject.EmployeeId;
                    data.ProjectId = AssignProject.ProjectId;
                    data.Status = AssignProject.Status;
                    data.ManagerId = AssignProject.ManagerId;
                    data.OccupyPercent = AssignProject.OccupyPercent;
                    data.IsActive = true;
                    data.IsDeleted = false;
                    _db.AssignProjects.Add(data);
                    _db.SaveChanges();
                    count++;
                }

                //  }
                response.StatusReason = true;
                if (count == 0)
                {
                    response.Message = "Project Already Exist";
                }
                else
                {
                    response.Message = "Project Saved Successfully";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion AssignProject

        #region AssignProjectdata

        public class AssignProjectdata
        {
            // public int AssignProjectId { get; set; }
            public int ProjectId { get; set; }

            public int EmployeeId { get; set; }

            // public int ManagerId { get; set; }
            //public string OccupyPercent { get; set; }
            // public bool IsActive { get; set; }
            // public bool IsDeleted { get; set; }
            public string Status { get; set; }
        }

        #endregion AssignProjectdata

        #region GetAllProjectById

        [Route("GetAllProjectById")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetAllProjectById(int EmployeeId)
        {
            try
            {
                Base response = new Base();

                List<ProData> ProDataList = new List<ProData>();

                var ProData = (from ad in _db.ProjectLists
                                   // join bd in db.Employee on ad.HR equals bd.EmployeeId
                                   //join cd in db.Employee on ad.EmployeeId equals cd.EmployeeId
                               join fd in _db.Employee on ad.ProjectManager equals fd.EmployeeId
                               join pd in _db.AssignProjects on ad.ID equals pd.ProjectId
                               where pd.EmployeeId == EmployeeId
                               //  join pt in db.Project on ad.ProjectName equals pt.ProjectId
                               select new
                               {
                                   // emp = cd.FirstName + " " + cd.LastName,
                                   manger = fd.FirstName + " " + fd.LastName,
                                   ad.ProjectName,
                                   ad.ID,
                                   pd.Status,
                                   pd.IsActive,
                                   pd.OccupyPercent,
                                   pd.IsDeleted
                               }).ToList();

                //var ProData = (from ad in db.AssignProjects
                //               join fd in db.Employee on ad.EmployeeId equals fd.EmployeeId
                //               join pd in db.ProjectLists on ad.ProjectId equals pd.ID
                //               where pd. == EmployeeId

                foreach (var item in ProData)
                {
                    ProData data = new ProData();

                    data.ProjectName = item.ProjectName;
                    data.ProjectManager = item.manger;
                    data.Id = item.ID;
                    data.Status = item.Status;
                    data.IsActive = item.IsActive;
                    data.IsDeleted = item.IsDeleted;
                    data.OccupyPercent = item.OccupyPercent;
                    ProDataList.Add(data);
                    //response.Message = "Project Saved Successfully";
                }
                return Ok(ProDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetAllProjectById

        #region GetAllEmployeeById

        [Route("GetAllEmployeeById")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetAllEmployeeById(int ProjectId)
        {
            try
            {
                Base response = new Base();

                List<ProData> ProDataList = new List<ProData>();

                var ProData = (from ad in _db.ProjectLists
                                   // join bd in db.Employee on ad.HR equals bd.EmployeeId
                                   //join cd in db.Employee on ad.EmployeeId equals cd.EmployeeId
                               join fd in _db.Employee on ad.ProjectManager equals fd.EmployeeId
                               join pd in _db.AssignProjects on ad.ID equals pd.ProjectId
                               where pd.EmployeeId == ProjectId
                               //  join pt in db.Project on ad.ProjectName equals pt.ProjectId
                               select new
                               {
                                   // emp = cd.FirstName + " " + cd.LastName,
                                   manger = fd.FirstName + " " + fd.LastName,
                                   ad.ProjectName,
                                   ad.ID,
                                   pd.IsActive,
                                   pd.IsDeleted
                               }).ToList();

                //var ProData = (from ad in db.AssignProjects
                //               join fd in db.Employee on ad.EmployeeId equals fd.EmployeeId
                //               join pd in db.ProjectLists on ad.ProjectId equals pd.ID
                //               where pd. == EmployeeId

                foreach (var item in ProData)
                {
                    ProData data = new ProData();

                    data.ProjectName = item.ProjectName;
                    data.ProjectManager = item.manger;
                    data.Id = item.ID;
                    data.IsActive = item.IsActive;
                    data.IsDeleted = item.IsDeleted;
                    ProDataList.Add(data);
                    //response.Message = "Project Saved Successfully";
                }
                return Ok(ProDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetAllEmployeeById

        #region API to search the project data which is availeble
        /// <summary>
        /// Created By Bhavendra Singh Jat on 30-9-2022
        /// API >> Get >> api/project/searchfromprojectdata
        /// </summary->
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("searchfromprojectdata")]
        //public async Task<ResponseBodyModel> GetSearchDataProject(string search)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity)
        //    try
        //    {
        //        var ProjectSearchData = await (from pd in _db.ProjectLists
        //                                       join emp in _db.Employee on pd.ProjectManager equals emp.EmployeeId
        //                                       where pd.CompanyId == claims.companyid && pd.IsActive && !pd.IsDeleted &&
        //                                       (pd.ProjectName.ToLower().Contains(search.ToLower()) || pd.CampanyName.ToLower().Contains(search.ToLower()) ||
        //                                       pd.Technology.ToLower().Contains(search.ToLower()) || pd.LeadType.ToLower().Contains(search.ToLower()))

        //                                       select new
        //                                       {
        //                                           ID = pd.ID,
        //                                           ProjectName = pd.ProjectName,
        //                                           Manger = emp.DisplayName,
        //                                           DisplayName = emp.DisplayName,
        //                                           ProjectManager = pd.ProjectManager,
        //                                           EmployeeId = emp.EmployeeId,
        //                                           ClientBillableAmount = pd.ClientBillableAmount,
        //                                           CampanyName = pd.CampanyName,
        //                                           Technology = pd.Technology,
        //                                           PaymentType = pd.PaymentType,
        //                                           IsActive = pd.IsActive,
        //                                           IsDeleted = pd.IsDeleted,
        //                                           CreatedBy = pd.CreatedBy,
        //                                           UpdatedBy = pd.UpdatedBy,
        //                                           LeadType = pd.LeadType,
        //                                       }
        //                                      ).ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        #endregion

        #region GetResourceByProject

        [Route("GetResourceByProject")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetResourceByProject()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                GetDataEmployeeModal getdata = new GetDataEmployeeModal();
                List<DataEmployee> employeeList = new List<DataEmployee>();
                var EmployeeData = await (from ad in _db.AssignProjects
                                          join bd in _db.Employee on ad.EmployeeId equals bd.EmployeeId
                                          //join cd in db.Role on bd.RoleId equals cd.RoleId
                                          where ad.IsActive == true
                                          select new
                                          {
                                              bd.EmployeeId,
                                              n = bd.FirstName + " " + bd.LastName,
                                              bd.PrimaryContact,
                                              bd.PersonalEmail,
                                              bd.GrossSalery,
                                              bd.FirstName,
                                              bd.LastName,
                                              ad.Status,
                                              ad.AssignProjectId,
                                              ad.OccupyPercent
                                          }).ToListAsync();
                var SumofBillableSalary = 0.0;
                var SumofNonBillableSalary = 0.0;
                var billAndNonBilEmpSalary = 0.0;
                foreach (var item in EmployeeData)
                {
                    DataEmployee data = new DataEmployee();
                    data.EmployeeId = item.EmployeeId;
                    data.FullName = item.n;
                    data.PrimaryContact = item.PrimaryContact;
                    data.Email = item.PersonalEmail;
                    data.Salary = item.GrossSalery;
                    data.Status = item.Status;
                    data.AssignProjectId = item.AssignProjectId;
                    data.FirstName = item.FirstName;
                    data.LastName = item.LastName;
                    int occupy = _db.AssignProjects.Where(x => x.EmployeeId == data.EmployeeId).Count();
                    int OccupyPercent = 100 / occupy;
                    data.OccupyPercent = OccupyPercent.ToString() + "%";
                    // this used to Add sum of the salary of Billable or Non-Billable employees
                    if (data.Status == "Billable")
                        data.SumofBillableSalary += Convert.ToDouble(item.GrossSalery);
                    if (data.Status == "Non-Billable")
                        data.SumofNonBillableSalary += Convert.ToDouble(item.GrossSalery);
                    billAndNonBilEmpSalary = SumofBillableSalary + SumofNonBillableSalary;
                    employeeList.Add(data);
                }
                if (employeeList.Count != 0)
                {
                    res.Status = true;
                    res.Message = "data found";
                    res.Data = employeeList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "data list not found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion GetResourceByProject

        #region UpdateProject

        [Route("UpdateProject")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateProject(Project updateProject)
        {
            try
            {
                Base response = new Base();
                var updateProjectData = _db.Project.Where(x => x.ProjectId == updateProject.ProjectId).FirstOrDefault();
                if (updateProjectData != null)
                {
                    updateProjectData.ProjectId = updateProject.ProjectId;
                    updateProjectData.ProjectName = updateProject.ProjectName;
                    updateProjectData.ProjectTypeId = updateProject.ProjectTypeId;
                    updateProjectData.BillTypeId = updateProject.BillTypeId;
                    updateProjectData.EstimatedDays = updateProject.EstimatedDays;
                    updateProjectData.ClientId = updateProject.ClientId;
                    updateProjectData.StatusId = updateProject.StatusId;
                    updateProjectData.ProjectOwner = updateProject.ProjectOwner;
                    updateProjectData.EmployeeId = updateProject.EmployeeId;
                    updateProjectData.ActualStartDate = updateProject.ActualStartDate;
                    updateProjectData.ActualEndDate = updateProject.ActualEndDate;
                    updateProjectData.CreatedOn = DateTime.Now;
                    updateProjectData.IsDeleted = false;
                    updateProjectData.IsActive = true;

                    _db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Project Updated Successfully";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion UpdateProject

        #region DeleteProject

        [Route("DeleteProject")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteProject(int projectid)
        {
            try
            {
                Base response = new Base();
                var projectData = _db.Project.Where(x => x.IsDeleted == false && x.ProjectId == projectid).FirstOrDefault();

                if (projectData != null)
                {
                    projectData.IsDeleted = true;
                    projectData.IsActive = false;
                    _db.Entry(projectData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Project Deleted Successfully";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion DeleteProject

        #region GetProjectList

        [Route("GetProjectList")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetProjectList()
        {
            try
            {
                Base response = new Base();
                //var projectdata = (from ad in db.Project select ad).ToList();
                var projectData = _db.Project.Where(x => x.IsDeleted == false).ToList();
                if (projectData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.projectData = projectData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetProjectList

        #region ProjectByFilter

        [Route("ProjectByFilter")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult ProjectByFilter(ProjectFilter project)
        {
            try
            {
                if (project.projectManager == 0 && project.projectStatus == 0 && project.billType == 0 && project.projectType == 0 && project.projectTechnology == 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0 && project.billType != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0 && project.billType != 0 && project.projectType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.billType != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0 && project.billType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.billType != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.billType != 0 && project.projectType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.StatusId == project.projectStatus && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.billType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0 && project.billType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.billType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.BillTypeId == project.billType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectType != 0 && project.projectStatus != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectType != 0 && project.billType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.billType != 0 && project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectStatus != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.StatusId == project.projectStatus && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.billType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.BillTypeId == project.billType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.EmployeeId == project.projectManager && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.billType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.StatusId == project.projectStatus && ad.BillTypeId == project.billType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.projectType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.StatusId == project.projectStatus && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.StatusId == project.projectStatus && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.billType != 0 && project.projectType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.BillTypeId == project.billType && ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.billType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.BillTypeId == project.billType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectType != 0 && project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where ad.ProjectTypeId == project.projectType && fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectManager != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.EmployeeId == project.projectManager && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.billType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.BillTypeId == project.billType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectStatus != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.StatusId == project.projectStatus && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectType != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     where ad.ProjectTypeId == project.projectType && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }
                else if (project.projectTechnology != 0)
                {
                    List<ProjectData> projectDataList = new List<ProjectData>();
                    ProjectData data = new ProjectData();
                    var finalData = (from ad in _db.Project
                                     join sd in _db.Status on ad.StatusId equals sd.statusId
                                     join ptd in _db.ProjectTypes on ad.ProjectTypeId equals ptd.ProjectTypeId
                                     join btd in _db.BillType on ad.BillTypeId equals btd.BillTypeId
                                     join etd in _db.Employee on ad.EmployeeId equals etd.EmployeeId
                                     join fd in _db.ProjectTechnology on ad.ProjectId equals fd.ProjectID
                                     where fd.TechnologyID == project.projectTechnology && ad.IsDeleted == false
                                     select new
                                     {
                                         ad.ProjectId,
                                         ad.ProjectName,
                                         ad.ActualStartDate,
                                         ad.ActualEndDate,
                                         sd.StatusVal,
                                         ptd.Project_Type,
                                         btd.Bill_Type,
                                         v = etd.FirstName + " " + etd.LastName,
                                         ad.StatusId,
                                         ad.ProjectTypeId,
                                         ad.BillTypeId,
                                         ad.EmployeeId
                                     }).ToList();
                    foreach (var item in finalData)
                    {
                        data.ProjectName = item.ProjectName;
                        data.ActualStartDate = item.ActualStartDate;
                        data.ActualEndDate = item.ActualEndDate;
                        // data.TechnologyType = item.TechnologyType;
                        data.StatusVal = item.StatusVal;
                        data.Project_Type = item.Project_Type;
                        data.Bill_Type = item.Bill_Type;
                        data.FullName = item.v;
                        data.ProjectId = item.ProjectId;
                        data.StatusId = item.StatusId;
                        //data.TechnologyId = item.TechnologyId;
                        data.ProjectTypeId = item.ProjectTypeId;
                        data.BillTypeId = item.BillTypeId;
                        data.EmployeeId = item.EmployeeId;
                        projectDataList.Add(data);
                    }
                    return Ok(projectDataList);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion ProjectByFilter

        #region

        #region

        [Route("GetProjectTechnology")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetProjectTechnology(int Id)
        {
            try
            {
                List<TechData> techDataList = new List<TechData>();
                var techData = (from ad in _db.ProjectTechnology
                                join bd in _db.Technology on ad.TechnologyID equals bd.TechnologyId
                                where ad.ProjectID == Id && ad.IsDeleted == false
                                select new
                                {
                                    ad.ProjectID,
                                    ad.ProjectTechId,
                                    ad.TechnologyID,
                                    bd.TechnologyType
                                }).ToList();
                foreach (var item in techData)
                {
                    TechData data = new TechData();
                    data.ProjectID = item.ProjectID;
                    data.ProjectTechId = item.ProjectTechId;
                    data.TechnologyID = item.TechnologyID;
                    data.TechnologyType = item.TechnologyType;
                    techDataList.Add(data);
                }
                return Ok(techDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("DeleteProjectTechnology")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteProjectTechnology(int projectId, int projectTechId)
        {
            try
            {
                Base response = new Base();
                var techData = (from ad in _db.ProjectTechnology where ad.ProjectID == projectId && ad.ProjectTechId == projectTechId select ad).FirstOrDefault();
                if (techData != null)
                {
                    techData.IsDeleted = true;
                    techData.IsActive = false;
                    _db.SaveChanges();

                    response.Message = "Data Deleted Successfully";
                    response.StatusReason = true;
                    return Ok(response);
                }
                else
                {
                    response.Message = "Data Not Found";
                    response.StatusReason = false;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("AddProjectTechnology")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddProjectTechnology(ProjectTechnology TechData)
        {
            try
            {
                Base response = new Base();

                foreach (var item in TechData.TechnologyId)
                {
                    ProjectTechnology data = new ProjectTechnology();
                    data.ProjectID = TechData.ProjectID;
                    data.TechnologyID = item;
                    data.IsActive = true;
                    data.IsDeleted = false;
                    _db.ProjectTechnology.Add(data);
                    _db.SaveChanges();
                }

                response.Message = "Data Added Successfully";
                response.StatusReason = true;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Category
        #region

        [Route("AddCategory")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddCategory(Category Category)
        {
            try
            {
                Base response = new Base();

                Category ProData = new Category();
                ProData.CategoryName = Category.CategoryName;
                ProData.UsertypeId = Category.UsertypeId;
                ProData.CategoryId = Category.CategoryId;
                ProData.CategoryTypeId = Category.CategoryTypeId;

                _db.Category.Add(ProData);
                _db.SaveChanges();
                response.StatusReason = true;
                response.Message = "Category Saved Successfully";

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("GetCategoryType")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetCategoryType()
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<DataCategory> CategoryList = new List<DataCategory>();
        //        var cattype = _db.CategoryType.ToList();
        //        var CategoryData = (from ad in _db.CategoryType
        //                                // join dc in db.CategoryType on ad.CategoryTypeId equals dc.CategoryTypeId
        //                                // join pd in db.CategoryType on ad.CategoryTypeId equals pd.CategoryTypeId
        //                                //join cd in db.Role on ad.RoleId equals cd.RoleId
        //                            select new
        //                            {
        //                                ad.CategoryTypeId,
        //                                ad.Category_Type
        //                            }).ToList();

        //        foreach (var item in CategoryData)
        //        {
        //            DataCategory data = new DataCategory();
        //            data.CategoryTypeId = item.CategoryTypeId;
        //            data.Category_Type = item.Category_Type;
        //            // data.Category_Type = item.Category_Type;
        //            // data.QuestionId = item.QuestionId;
        //            // data.UserId = item.UserId;
        //            // data.RoleType = item.RoleType;
        //            CategoryList.Add(data);
        //        }
        //        if (CategoryList.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.DataCategory = CategoryList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("GraphAverageScore")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GraphAverageScore(int empId)
        {
            try
            {
                Base response = new Base();
                DateTime todate = DateTime.Now, fromdate = DateTime.Now;
                var Feedbackbymonth = new List<KeyValuePair<int, int>>();
                for (int i = 1; i <= 12; i++)
                {
                    var currentDate = DateTime.Now;
                    fromdate = new DateTime(currentDate.Year, i, 1, 0, 0, 0); // new DateTime(currentDate.Year,i , 1, 0, 0, 0);
                    var endDate = fromdate.AddMonths(1).AddDays(-1);
                    todate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 12, 59, 59);
                    // var allTests = _service.GetParticipantByDate(fromdate, todate);
                    // Feedbackbymonth.Add(new KeyValuePair<int, int>(i, allTests.TotalCount));

                    int year = DateTime.Today.Year;
                    DateTime firstDay = new DateTime(year, 1, 1);
                    DateTime lastDay = new DateTime(year, 12, 31);

                    var MyFeedback = _db.Feedbacks.Where(x => x.ReceiverEmployeeId == empId).ToList();
                    int Calculate = 0;
                    Double CalculateAvg = 0;
                    if (MyFeedback.Count > 0)
                    {
                        foreach (var myFeedback in MyFeedback)
                        {
                            var MyScore = _db.FeedbackScore.Where(x => x.FeedbackId == myFeedback.FeedbackId).ToList();
                            Double innerCalculate = 0;
                            Double innerCalculateAvg = 0;
                            foreach (var myScore in MyScore)
                            {
                                innerCalculate += myScore.QuestionScore;
                            }
                            innerCalculateAvg = innerCalculate / MyScore.Count();
                            CalculateAvg += Math.Round(innerCalculateAvg);
                            if (Double.IsNaN(CalculateAvg) || Double.IsInfinity(CalculateAvg))
                            {
                                CalculateAvg = 0;
                            }
                            var feedback = (from ad in _db.Feedbacks where ad.FeedbackId == myFeedback.FeedbackId select ad).FirstOrDefault();
                            if (feedback != null)
                            {
                                feedback.AverageScore = Convert.ToInt32(CalculateAvg);
                                _db.SaveChanges();
                            }
                            //var Feedback = (from ad in db.Feedbacks
                            // where ad.ReceiverEmployeeId == empId &&
                            // ad.CreatedDate.Month == ad.CreatedDate.Month
                            // select new
                            // {
                            // ).ToList();

                            //var Feedback = from fb in db.Feedbacks
                            // group fb by fb.CreatedDate into fbGroup
                            // orderby fbGroup.Key descending
                            // select new
                            // {
                            // Key = fbGroup.Key,
                            // Feedback = fbGroup.OrderBy(x => x.CreatedDate)
                            // };

                            var q = from t in _db.Feedbacks
                                    where t.CreatedDate <= firstDay && t.CreatedDate >= lastDay
                                    group t by t.CreatedDate
                            into g
                                    select new
                                    {
                                        dateField = g.Key,
                                        countField = g.Count()
                                    };

                            var data = _db.Feedbacks.Where(x => x.CreatedDate <= firstDay && x.CreatedDate >= lastDay)
                            .GroupBy(s => new { month = s.CreatedDate.ToString(), date = s.CreatedDate })
                            .Select(x => new { count = x.Count(), month = x.Key.month, date = x.Key.date }).ToList();

                            foreach (var item in data)
                            {
                                GraphData con = new GraphData();
                                con.Month = item.date;
                                con.Count = item.count;
                            }

                            foreach (var item in q)
                            {
                                GraphData con = new GraphData();
                                con.Month = item.dateField;
                                con.Count = item.countField;
                            }
                        }
                        Calculate = Convert.ToInt32(Math.Round(CalculateAvg / MyFeedback.Count()));
                    }
                    else
                    {
                        Calculate = 0;
                    }
                }
                var b = new FeedbackServices();

                DateTime st = DateTime.Now;
                DateTime _stDate = new DateTime(st.Year, st.Month, 01, 0, 0, 0);
                DateTime startdate = _stDate.AddMonths(-12);

                var Feedbackbymonths = new List<KeyValuePair<DateTime, double>>();

                for (int i = 0; i < 12; i++)
                {
                    DateTime endDate = startdate.AddMonths((i * 1) + 1);
                    double r = b.GetAVGbyMonth(startdate.AddMonths(i * 1), endDate.AddDays(-1));
                    Feedbackbymonths.Add(new KeyValuePair<DateTime, double>(startdate.AddMonths(i * 1), r));
                }

                return Ok(Feedbackbymonths);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("GetAllEmployeeByTeamTypeId")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllEmployeeByTeamTypeId(int TeamLeadId)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<TeamType> teamTypelist = new List<TeamType>();
        //        List<EmployeeMiniModel> MiniModellst = new List<EmployeeMiniModel>();
        //        var TeamData = (from ad in _db.TeamType
        //                        join empbd in _db.Employee on ad.TeamTypeId equals empbd.TeamTypeId
        //                        where (ad.TeamLeadId == TeamLeadId)
        //                        orderby ad.TeamName ascending
        //                        select new TeamTypeVM
        //                        {
        //                            TeamLeadId = ad.TeamLeadId,
        //                            TeamTypeId = ad.TeamTypeId,
        //                            FullName = empbd.FirstName + " " + empbd.LastName,
        //                            EmployeeId = empbd.EmployeeId,
        //                            RoleId = empbd.RoleId,
        //                            TeamName = ad.TeamName,
        //                            TeamLeadName = ad.TeamLeadName,
        //                        }).ToList();
        //        if (TeamData.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";

        //            response.typeVMs = TeamData;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetCategoryName")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetCategoryName()
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<DataCategory> CategoryList = new List<DataCategory>();
        //        var cattype = _db.CategoryType.ToList();
        //        var CategoryData = (from ad in _db.Category
        //                            select new
        //                            {
        //                                ad.CategoryId,
        //                                ad.CategoryName
        //                            }).ToList();

        //        foreach (var item in CategoryData)
        //        {
        //            DataCategory data = new DataCategory();
        //            data.CategoryId = item.CategoryId;
        //            data.CategoryName = item.CategoryName;
        //            CategoryList.Add(data);
        //        }
        //        if (CategoryList.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.DataCategory = CategoryList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetAllCategory")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllCategory()
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<DataCategory> CategoryDatas = new List<DataCategory>();
        //        {
        //            var getQueryData = (from cat in _db.Category
        //                                join cd in _db.Role on cat.UsertypeId equals cd.RoleId
        //                                join ct in _db.CategoryType on cat.CategoryTypeId equals ct.CategoryTypeId
        //                                select new DataCategory
        //                                {
        //                                    CategoryId = cat.CategoryId,
        //                                    CategoryName = cat.CategoryName,
        //                                    CategoryTypeId = ct.CategoryTypeId,
        //                                    Category_Type = ct.Category_Type,
        //                                    RoleId = cd.RoleId,
        //                                    RoleType = cd.RoleType,
        //                                    CreatedDate = cat.CreatedDate,
        //                                    UpdatedDate = cat.UpdatedDate
        //                                }).ToList();
        //            foreach (var i in getQueryData)
        //            {
        //                DataCategory dataCategory = new DataCategory();
        //                List<Questions> QuestionMd = _db.Questions.Where(x => x.CategoryId == i.CategoryId).ToList();
        //                dataCategory.CategoryId = i.CategoryId;
        //                dataCategory.CategoryName = i.CategoryName;
        //                dataCategory.quenstions = QuestionMd;
        //                dataCategory.CategoryName = i.CategoryName;
        //                dataCategory.CategoryTypeId = i.CategoryTypeId;
        //                dataCategory.Category_Type = i.Category_Type;
        //                dataCategory.RoleId = i.RoleId;
        //                dataCategory.RoleType = i.RoleType;
        //                dataCategory.UpdatedDate = i.UpdatedDate;
        //                dataCategory.CreatedDate = i.CreatedDate;
        //                CategoryDatas.Add(dataCategory);
        //            }
        //        }

        //        if (CategoryDatas.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.DataCategory = CategoryDatas;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("UpdateCategory")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateCategory(Category category)
        {
            try
            {
                Base response = new Base();
                if (category != null)
                {
                    Category Categories = _db.Category.Where(x => x.CategoryId == category.CategoryId).FirstOrDefault();
                    if (Categories != null)
                    {
                        Categories.CategoryId = category.CategoryId;
                        Categories.CategoryName = category.CategoryName;
                        Categories.CategoryTypeId = category.CategoryTypeId;
                        Categories.UsertypeId = category.UsertypeId;

                        _db.Entry(Categories).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        response.StatusReason = true;
                        response.Message = "Record Updated Successfully";
                        return Ok(response);
                    }
                    else
                    {
                        response.StatusReason = false;
                        response.Message = "Record not found in database !!!";
                        return Ok(response);
                    }
                }
                else
                {
                    return Ok("Please select the record");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("TestCat")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult TestCat(CategoryFilterDTO CategoryFilterDTO)
        {
            var rol = _db.Role.Where(r => r.CategoryTypeId == CategoryFilterDTO.CategoryTypeId).FirstOrDefault();
            var catType = _db.CategoryType.Where(c => c.CategoryTypeId == CategoryFilterDTO.CategoryTypeId).FirstOrDefault();
            var cats = _db.Category.Where(c => c.CategoryTypeId == CategoryFilterDTO.CategoryTypeId).ToList();
            List<DataCategory> CategoryDatas = new List<DataCategory>();
            foreach (var c in cats)
            {
                var x = new DataCategory();
                x.CategoryId = c.CategoryId;
                x.CategoryName = c.CategoryName;
                x.Category_Type = catType.Category_Type;
                x.RoleId = rol.RoleId;
                x.CategoryTypeId = catType.CategoryTypeId;
                x.RoleType = rol.RoleType;

                var ques = _db.Questions.Where(q => q.CategoryId == c.CategoryId).ToList();
                x.UpdatedDate = ques.Max(m => m.UpdatedDate);
                CategoryDatas.Add(x);
            }

            return Ok(CategoryDatas);
        }

        #endregion

        //#region

        //[Route("GetCategoryFilter")]
        //[HttpPost]
        //[Authorize]
        //public IHttpActionResult GetCategoryFilter(CategoryFilterDTO CategoryFilterDTO)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<DataCategory> CategoryData = new List<DataCategory>();// HR
        //        List<DataCategory> getQueryData = new List<DataCategory>();// HR

        //        if (CategoryFilterDTO.RoleId != 0 && CategoryFilterDTO.CategoryTypeId != 0)
        //        {
        //            getQueryData = (from cat in _db.Category
        //                            join cd in _db.Role on cat.UsertypeId equals cd.RoleId
        //                            join ct in _db.CategoryType on cat.CategoryTypeId equals ct.CategoryTypeId
        //                            where cd.RoleId == CategoryFilterDTO.RoleId && cat.CategoryTypeId == CategoryFilterDTO.CategoryTypeId
        //                            select new DataCategory
        //                            {
        //                                CategoryId = cat.CategoryId,
        //                                CategoryName = cat.CategoryName,
        //                                CategoryTypeId = ct.CategoryTypeId,
        //                                Category_Type = ct.Category_Type,
        //                                RoleId = cd.RoleId,
        //                                RoleType = cd.RoleType
        //                            }).ToList();
        //        }

        //        if (CategoryFilterDTO.CategoryTypeId != 0 && CategoryFilterDTO.RoleId == 0)
        //        {
        //            getQueryData = (from cat in _db.Category
        //                            join cd in _db.Role on cat.UsertypeId equals cd.RoleId
        //                            join ct in _db.CategoryType on cat.CategoryTypeId equals ct.CategoryTypeId
        //                            where cat.CategoryTypeId == CategoryFilterDTO.CategoryTypeId
        //                            select new DataCategory
        //                            {
        //                                CategoryId = cat.CategoryId,
        //                                CategoryName = cat.CategoryName,
        //                                CategoryTypeId = ct.CategoryTypeId,
        //                                Category_Type = ct.Category_Type,
        //                                RoleId = cd.RoleId,
        //                                RoleType = cd.RoleType
        //                            }).ToList();
        //        }

        //        if (CategoryFilterDTO.CategoryTypeId == 0 && CategoryFilterDTO.RoleId != 0)
        //        {
        //            getQueryData = (from cat in _db.Category
        //                            join cd in _db.Role on cat.UsertypeId equals cd.RoleId
        //                            join ct in _db.CategoryType on cat.CategoryTypeId equals ct.CategoryTypeId
        //                            where cd.RoleId == CategoryFilterDTO.RoleId
        //                            select new DataCategory
        //                            {
        //                                CategoryId = cat.CategoryId,
        //                                CategoryName = cat.CategoryName,
        //                                CategoryTypeId = ct.CategoryTypeId,
        //                                Category_Type = ct.Category_Type,
        //                                RoleId = cd.RoleId,
        //                                RoleType = cd.RoleType
        //                            }).ToList();
        //        }

        //        foreach (var i in getQueryData)
        //        {
        //            DataCategory dataCategory = new DataCategory();
        //            List<Questions> QuestionMd = _db.Questions.Where(x => x.CategoryId == i.CategoryId).ToList();
        //            dataCategory.CategoryId = i.CategoryId;
        //            dataCategory.CategoryName = i.CategoryName;
        //            dataCategory.quenstions = QuestionMd;
        //            dataCategory.CategoryName = i.CategoryName;
        //            dataCategory.CategoryTypeId = i.CategoryTypeId;
        //            dataCategory.Category_Type = i.Category_Type;
        //            dataCategory.RoleId = i.RoleId;
        //            dataCategory.RoleType = i.RoleType;
        //            CategoryData.Add(dataCategory);
        //        }

        //        if (CategoryData.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.DataCategory = CategoryData;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetCount")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetCount()
        //{
        //    try
        //    {
        //        Base response = new Base();

        //        CountData countData = new CountData();

        //        var CategoryCount = (
        //                      from cat in _db.Category
        //                      select new
        //                      {
        //                          cat.CategoryId,
        //                      }).Count();

        //        countData.TotalCategory = CategoryCount;

        //        var FeedbackCount = (
        //                    from cat in _db.Feedbacks
        //                    select new
        //                    {
        //                        cat.FeedbackId,
        //                    }).Count();

        //        countData.TotalFeedbacks = FeedbackCount;

        //        var TeamsCount = (
        //                    from cat in _db.Team
        //                    where cat.IsDelete == false
        //                    select new
        //                    {
        //                        cat.TeamId,
        //                    }).Count();

        //        countData.TotalTeams = TeamsCount;

        //        response.StatusReason = true;
        //        response.Message = "Data Found";
        //        response.CountData = countData;

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetRoleType")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetRoleType(int CategoryTypeId)
        //{
        //    try
        //    {
        //        // var categoryId = 3;

        //        // selec rale, roleId from TableRoale where catId= categoryId;

        //        Base response = new Base();
        //        List<DataCategory> CategoryList = new List<DataCategory>();// HR
        //        var CategoryData = (from ad in _db.Role
        //                            orderby ad.RoleType
        //                            where ad.CategoryTypeId == CategoryTypeId
        //                            select new
        //                            {
        //                                ad.RoleId,
        //                                ad.RoleType,
        //                            }).ToList();

        //        foreach (var item in CategoryData)
        //        {
        //            DataCategory data = new DataCategory();
        //            data.RoleId = item.RoleId;
        //            data.RoleType = item.RoleType;

        //            CategoryList.Add(data);
        //        }
        //        if (CategoryList.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.DataCategory = CategoryList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetCategories")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetCategories(int roleId, int SelectedEmpId, int LoginEmpId)
        //{
        //    Base response = new Base();

        //    var feedback = _db.Feedbacks.Where(x => x.RatedByEmpId == LoginEmpId && x.ReceiverEmployeeId == SelectedEmpId &&
        //    x.CreatedDate.Month == DateTime.Now.Month && x.CreatedDate.Year == DateTime.Now.Year).FirstOrDefault();
        //    if (feedback != null)
        //    {
        //        response.StatusReason = false;
        //        response.Message = "You have already given feedback this month";
        //        return Ok(response);
        //    }

        //    List<DataCategory> CategoryList = categoryService.GetCategoryByRoleId(roleId);
        //    if (CategoryList.Count == 0)
        //    {
        //        response.StatusReason = false;
        //        response.Message = "Category not found";
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        response.DataCategory = CategoryList;
        //        response.StatusReason = true;
        //        response.Message = "Category found";
        //        return Ok(response);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetCategoriesByCategoryType")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetCategoriesByCategoryType(int CategoryTypeId)
        //{
        //    Base response = new Base();

        //    List<DataCategory> CategoryList = categoryService.GetCategoryByCtype(CategoryTypeId);
        //    response.DataCategory = CategoryList;
        //    return Ok(response);
        //}

        //#endregion

        #region

        [Route("DeleteCategory")]
        [HttpDelete]
        public IHttpActionResult DeleteCategory(int CategoryId)
        {
            Base response = new Base();
            if (CategoryId == 0)
            {
                return BadRequest();
            }
            questionService.RemoveQuestionCategoryId(CategoryId);
            var resCatogory = categoryService.RemoveCategoryById(CategoryId);

            if (resCatogory)
            {
                response.StatusReason = true;
                response.Message = "Category & questions Deleted Successfully";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!!";
            }
            return Ok(response);
        }

        #endregion

        #region

        [Route("AddFeedbacks")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddFeedbacks(FeedbackMaster Feedback)
        {
            try
            {
                Base response = new Base(); //var feedback = db.CategoryType.Where(x => x.CategoryTypeId == 2).ToList();
                if (Feedback.CategoryTypeId == 2)//EMPLOYEEiD=2
                {
                    int month = DateTime.Now.Month;
                    int Year = DateTime.Now.Year;
                    var feedback = _db.Feedbacks.Where(x => x.RatedByEmpId == Feedback.RatedByEmpId && x.ReceiverEmployeeId == Feedback.ReceiverEmployeeId && x.CreatedDate.Month == month && x.CreatedDate.Year == Year).FirstOrDefault();
                    if (feedback != null)
                    {
                        response.StatusReason = false;
                        response.Message = "You have already given feedback this month";
                        return Ok(response);
                    }
                }
                else
                {
                    Feedback.ReceiverEmployeeId = 0;
                }
                FeedbackMaster feedbackobj = new FeedbackMaster();
                feedbackobj.FeedbackId = Feedback.FeedbackId;
                feedbackobj.ReceiverEmployeeId = Feedback.ReceiverEmployeeId;
                feedbackobj.RatedByEmpId = Feedback.RatedByEmpId;
                feedbackobj.YourFeedback = Feedback.YourFeedback;
                feedbackobj.CategoryTypeId = Feedback.CategoryTypeId;
                feedbackobj.UpdatedDate = DateTime.Now;
                feedbackobj.CreatedDate = DateTime.Now;
                feedbackobj.RoleId = Feedback.RoleId;
                feedbackobj.TeamLeadId = Feedback.TeamLeadId;
                _db.Feedbacks.Add(feedbackobj);
                _db.SaveChanges(); foreach (var item1 in Feedback.FBScore)
                {
                    FeedbackScore feedbackscoreobj = new FeedbackScore();
                    feedbackscoreobj.FeedbackId = feedbackobj.FeedbackId;
                    feedbackscoreobj.CategoryId = item1.CategoryId;
                    feedbackscoreobj.QuestionId = item1.QuestionId;
                    feedbackscoreobj.QuestionScore = item1.QuestionScore;
                    _db.FeedbackScore.Add(feedbackscoreobj);
                    _db.SaveChanges();
                }
                response.StatusReason = true;
                response.Message = "FEEDBACK Saved Successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("GetAllFeedbackHistoryByEmployeeId")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllFeedbackHistoryByEmployeeId(int empId)
        //{
        //    try
        //    {
        //        Base response = new Base(); List<FeedbackData> feedbacklst = new List<FeedbackData>();
        //        List<DataCategory> dataCategorieslst;
        //        //List<DataCategory> dataCategorieslst = new List<DataCategory>();
        //        //List<FeedbackDTO> check = FeedbackServices.GetFeebackByEmpId(empId); //return Ok(check);
        //        //
        //        var FeedbackData = (from fd in _db.Feedbacks
        //                            join tt in _db.TeamType on fd.TeamLeadId equals tt.TeamLeadId
        //                            join cat in _db.CategoryType on fd.CategoryTypeId equals cat.CategoryTypeId
        //                            where fd.RatedByEmpId == empId
        //                            select new
        //                            {
        //                                fd.FeedbackId,
        //                                fd.AverageScore,
        //                                fd.UpdatedDate,
        //                                fd.YourFeedback,
        //                                fd.RoleId,
        //                                fd.ReceiverEmployeeId,
        //                                //RecieverEmployeeName = empbd.FirstName + " " + empbd.LastName,
        //                                fd.CategoryTypeId,
        //                                cat.Category_Type,
        //                                tt.TeamLeadId
        //                                //sat.TeamLeadName
        //                            }).ToList();
        //        if (FeedbackData.Count != 0)
        //        {
        //            foreach (var items in FeedbackData)
        //            {
        //                FeedbackData feedbackdataObj = new FeedbackData();
        //                dataCategorieslst = new List<DataCategory>();
        //                feedbackdataObj.FeedbackId = items.FeedbackId;
        //                feedbackdataObj.UpdatedDate = DateTime.Now;
        //                feedbackdataObj.YourFeedback = items.YourFeedback;
        //                feedbackdataObj.RoleId = items.RoleId;
        //                feedbackdataObj.ReceiverEmployeeId = items.ReceiverEmployeeId;
        //                //feedbackdataObj.ReceiverEmployeeName = items.RecieverEmployeeName;
        //                feedbackdataObj.CategoryTypeId = items.CategoryTypeId;
        //                feedbackdataObj.CategoryType = items.Category_Type;
        //                feedbackdataObj.TeamLeadId = items.TeamLeadId;
        //                //feedbackdataObj.TeamLeadName = items.TeamLeadName;
        //                if (feedbackdataObj.ReceiverEmployeeId != 0)
        //                {
        //                    var emp = _db.Employee.Where(x => x.EmployeeId == items.ReceiverEmployeeId).FirstOrDefault();
        //                    feedbackdataObj.ReceiverEmployeeName = emp.FirstName + " " + emp.LastName;
        //                }
        //                else
        //                {
        //                    feedbackdataObj.ReceiverEmployeeName = null;
        //                }
        //                var feedback = (from fd in _db.FeedbackScore
        //                                join cat in _db.Category on fd.CategoryId equals cat.CategoryId
        //                                where fd.FeedbackId == items.FeedbackId
        //                                select new
        //                                {
        //                                    CategoryId = fd.CategoryId,
        //                                    CategoryName = cat.CategoryName,
        //                                }).GroupBy(z => z.CategoryId).Select(g => g.FirstOrDefault()).ToList(); if (feedback.Count() != 0)
        //                {
        //                    var averageScore = 0;
        //                    foreach (var i in feedback)
        //                    {
        //                        List<QuestionScore> QScore = new List<QuestionScore>();
        //                        DataCategory DCategory = new DataCategory(); var feedData = (from qd in _db.Questions
        //                                                                                     join fd in _db.FeedbackScore on qd.CategoryId equals fd.CategoryId
        //                                                                                     where qd.CategoryId == i.CategoryId && fd.FeedbackId == items.FeedbackId && qd.QuestionId == fd.QuestionId
        //                                                                                     select new
        //                                                                                     {
        //                                                                                         qd.QuestionId,
        //                                                                                         qd.Question,
        //                                                                                         fd.QuestionScore
        //                                                                                     }).ToList();
        //                        var QuestionScoreTotal = 0;
        //                        foreach (var feedbackItrate in feedData)
        //                        {
        //                            QuestionScore QScoreObj = new QuestionScore();
        //                            QScoreObj.QuestionId = feedbackItrate.QuestionId;
        //                            QScoreObj.QuestionName = feedbackItrate.Question;
        //                            QScoreObj.Score = feedbackItrate.QuestionScore;
        //                            QuestionScoreTotal += feedbackItrate.QuestionScore;
        //                            QScore.Add(QScoreObj);
        //                        }
        //                        DCategory.CategoryAverageScore = QuestionScoreTotal / feedData.Count();
        //                        averageScore += DCategory.CategoryAverageScore;
        //                        DCategory.CategoryId = i.CategoryId;
        //                        DCategory.CategoryName = i.CategoryName;
        //                        DCategory.questionScores = QScore;
        //                        dataCategorieslst.Add(DCategory);
        //                    }
        //                    if (feedback.Count() > 0)
        //                    {
        //                        feedbackdataObj.AverageScore = averageScore / feedback.Count();
        //                    }
        //                    else
        //                    {
        //                        feedbackdataObj.AverageScore = averageScore;
        //                    }
        //                    feedbackdataObj.DataCategories = dataCategorieslst;
        //                    // feedbackdataObj.FBScore = feedbackScorelst;
        //                    feedbacklst.Add(feedbackdataObj);
        //                }
        //            }
        //        }
        //        if (feedbacklst.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found Successfully";
        //            response.feedbackDatas = feedbacklst;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [HttpPut]
        [Route("ChangePassword")]
        [Authorize]
        public IHttpActionResult ChangePassword(Employee Employee)// we are using contact table as a employee
        {
            Base response = new Base(); var EmployeeData = _db.Employee.Where(x => x.IsDeleted == false && x.EmployeeId == Employee.EmployeeId).FirstOrDefault();
            //EmployeeData.ConfirmationDate = Employee.JoiningDate != null ? Convert.ToDateTime(Employee.ConfirmationDate).AddDays(1) : DateTime.Now;
            EmployeeData.Password = Employee.Password;
            _db.SaveChanges(); var UserData = _db.User.Where(x => x.EmployeeId == Employee.EmployeeId).FirstOrDefault();
            if (UserData != null)
            {
                UserData.UserName = Employee.OfficeEmail;
                var Password = Employee.Password;
                var keynew = DataHelper.GeneratePasswords(10);
                var passw = DataHelper.EncodePassword(Password, keynew);
                UserData.Password = passw;
                UserData.HashCode = keynew;
                UserData.EmployeeId = EmployeeData.EmployeeId;
                UserData.DepartmentId = Employee.RoleId;
                UserData.IsDeleted = false;
                UserData.IsActive = true;
                UserData.CreatedOn = DateTime.Now;
                _db.SaveChanges();
            }
            else
            {
                User UserDataInsert = new User();
                UserDataInsert.UserName = Employee.OfficeEmail;
                var Password = Employee.Password;
                var keynew = DataHelper.GeneratePasswords(10);
                var passw = DataHelper.EncodePassword(Password, keynew);
                UserDataInsert.Password = passw;
                UserDataInsert.HashCode = keynew;
                UserDataInsert.EmployeeId = EmployeeData.EmployeeId;
                UserDataInsert.DepartmentId = Employee.RoleId;
                UserDataInsert.IsDeleted = false;
                UserDataInsert.IsActive = true;
                UserDataInsert.CreatedOn = DateTime.Now;
                _db.User.Add(UserDataInsert);
                _db.SaveChanges();
            }
            response.StatusReason = true;
            response.Message = "Data Updated Successfully";
            return Ok(response);
        }

        #endregion

        //#region

        //[Route("GetAllFeedbacks")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllFeedbacks()
        //{
        //    try
        //    {
        //        Base response = new Base();

        //        List<FeedbackData> feedbacklst = new List<FeedbackData>();
        //        List<DataCategory> dataCategorieslst;
        //        // List<DataCategory> dataCategorieslst = new List<DataCategory>();

        //        var FeedbackData = (from fd in _db.Feedbacks

        //                            join cat in _db.CategoryType on fd.CategoryTypeId equals cat.CategoryTypeId
        //                            join e in _db.Employee on fd.RatedByEmpId equals e.EmployeeId
        //                            select new
        //                            {
        //                                fd.FeedbackId,
        //                                fd.AverageScore,
        //                                fd.UpdatedDate,
        //                                fd.YourFeedback,
        //                                fd.RoleId,
        //                                fd.RatedByEmpId,
        //                                e.FirstName,
        //                                e.LastName,
        //                                fd.ReceiverEmployeeId,
        //                                fd.CategoryTypeId,
        //                                cat.Category_Type
        //                            }).ToList();
        //        foreach (var items in FeedbackData)
        //        {
        //            FeedbackData feedbackdataObj = new FeedbackData();
        //            dataCategorieslst = new List<DataCategory>();
        //            feedbackdataObj.FeedbackId = items.FeedbackId;
        //            feedbackdataObj.UpdatedDate = DateTime.Now;
        //            feedbackdataObj.YourFeedback = items.YourFeedback;
        //            feedbackdataObj.RoleId = items.RoleId;
        //            feedbackdataObj.ReceiverEmployeeId = items.ReceiverEmployeeId;
        //            feedbackdataObj.CategoryTypeId = items.CategoryTypeId;
        //            feedbackdataObj.CategoryType = items.Category_Type;
        //            feedbackdataObj.RatedByEmpName = items.FirstName + ' ' + items.LastName;
        //            if (feedbackdataObj.ReceiverEmployeeId != 0)
        //            {
        //                var emp = _db.Employee.Where(x => x.EmployeeId == items.ReceiverEmployeeId).FirstOrDefault();
        //                feedbackdataObj.ReceiverEmployeeName = emp.FirstName + " " + emp.LastName;
        //            }
        //            else
        //            {
        //                feedbackdataObj.ReceiverEmployeeName = null;
        //            }

        //            var feedback = (from fd in _db.FeedbackScore
        //                            join cat in _db.Category on fd.CategoryId equals cat.CategoryId
        //                            where fd.FeedbackId == items.FeedbackId
        //                            select new
        //                            {
        //                                CategoryId = fd.CategoryId,
        //                                CategoryName = cat.CategoryName,
        //                            }).GroupBy(z => z.CategoryId).Select(g => g.FirstOrDefault()).ToList();

        //            var averageScore = 0;
        //            foreach (var i in feedback)
        //            {
        //                List<QuestionScore> QScore = new List<QuestionScore>();
        //                DataCategory DCategory = new DataCategory();

        //                var feedData = (from qd in _db.Questions
        //                                join fd in _db.FeedbackScore on qd.CategoryId equals fd.CategoryId
        //                                where qd.CategoryId == i.CategoryId && fd.FeedbackId == items.FeedbackId && qd.QuestionId == fd.QuestionId
        //                                select new
        //                                {
        //                                    qd.QuestionId,
        //                                    qd.Question,
        //                                    fd.QuestionScore
        //                                }).ToList();
        //                var QuestionScoreTotal = 0;
        //                foreach (var feedbackItrate in feedData)
        //                {
        //                    QuestionScore QScoreObj = new QuestionScore();
        //                    QScoreObj.QuestionId = feedbackItrate.QuestionId;
        //                    QScoreObj.QuestionName = feedbackItrate.Question;
        //                    QScoreObj.Score = feedbackItrate.QuestionScore;
        //                    QuestionScoreTotal += feedbackItrate.QuestionScore;
        //                    QScore.Add(QScoreObj);
        //                }
        //                if (feedData.Count() > 0)
        //                {
        //                    DCategory.CategoryAverageScore = QuestionScoreTotal / feedData.Count();
        //                }
        //                else
        //                {
        //                    DCategory.CategoryAverageScore = QuestionScoreTotal;
        //                }
        //                averageScore += DCategory.CategoryAverageScore;
        //                DCategory.CategoryId = i.CategoryId;
        //                DCategory.CategoryName = i.CategoryName;
        //                DCategory.questionScores = QScore;
        //                dataCategorieslst.Add(DCategory);
        //            }
        //            if (feedback.Count() > 0)
        //            {
        //                feedbackdataObj.AverageScore = averageScore / feedback.Count();
        //            }
        //            else
        //            {
        //                feedbackdataObj.AverageScore = averageScore;
        //            }

        //            feedbackdataObj.DataCategories = dataCategorieslst;
        //            feedbacklst.Add(feedbackdataObj);
        //        }
        //        if (feedbacklst.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found Successfully";
        //            response.feedbackDatas = feedbacklst;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("MyTotalAvgScore")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult MyTotalAvgScore(int empId)
        {
            try
            {
                Base response = new Base();
                var res = feedbackServices.GetMyAvgScore(empId);

                response.StatusReason = true;
                response.Message = "Your Average Score";
                response.MyAvgScore = res;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("MyTotalCategoryScore")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult MyTotalCategoryScore(int empId)
        //{
        //    try
        //    {
        //        Base response = new Base();

        //        List<GetcatName> CategoryNameDataList = new List<GetcatName>();
        //        List<CategoryAvg> res = feedbackServices.GetCategoryScore(empId);

        //        var GetCatName = res.OrderBy(x => x.Avg).Take(5);
        //        foreach (var item in GetCatName)
        //        {
        //            GetcatName CatName = new GetcatName();

        //            var GetName = _db.Category.Where(x => x.CategoryId == item.CategoryId).FirstOrDefault();
        //            CatName.CategoryName = GetName.CategoryName;
        //            CatName.GetcatNameId = GetName.CategoryId;
        //            CategoryNameDataList.Add(CatName);
        //        }
        //        response.CategoryNameDataList = CategoryNameDataList;
        //        response.CategoryDataList = res;
        //        response.StatusReason = true;
        //        response.Message = "Your Category Score";
        //        response.MyCategoryScore = res;

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetAllFeedback")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllFeedback()
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        var res = feedbackServices.GetAllFeedback();
        //        if (res.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.Feedbacks = res;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("DeleteFeedback")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteFeedback(int FeedbackId)
        {
            try
            {
                Base response = new Base();
                if (FeedbackId == 0)
                {
                    return BadRequest("feedback id can not be zero");
                }
                FeedbackMaster feedback = _db.Feedbacks.Find(FeedbackId);
                if (feedback != null)
                {
                    _db.Feedbacks.Remove(feedback);
                    _db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Feedback Deleted Successfully";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("GetFeedbackFilter")]
        //[HttpPost]
        //[Authorize]
        //public IHttpActionResult GetFeedbackFilter(FeedbackFilter FeedbackFilter)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        var res = feedbackServices.FilterFeedback(FeedbackFilter);
        //        if (res.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.Feedbacks = res;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetSelectTeamRole")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetSelectTeamRole()
        //{
        //    try
        //    {
        //        Base response = new Base();

        //        var TeamData = (from ad in _db.Employee
        //                        orderby ad.FirstName
        //                        where (ad.RoleId == 31 || ad.RoleId == 21 || ad.RoleId == 130 || ad.RoleId == 136 || ad.RoleId == 13 || ad.RoleId == 144) && ad.IsDeleted == false

        //                        select new TeamData
        //                        {
        //                            EmployeeId = ad.EmployeeId,
        //                            FullName = ad.FirstName + " " + ad.LastName,
        //                        }).ToList();

        //        if (TeamData.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Record Found";
        //            response.TeamDatas = TeamData;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No Record Found!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetSelectEmployeeTeamRole")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetSelectEmployeeTeamRole(int empId)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<TeamData> TeamData = new List<TeamData>();
        //        var a = _db.Employee.Where(x => x.EmployeeId == empId).FirstOrDefault();
        //        if (a != null)
        //        {
        //            if (a.RoleId == 120)//(admin)
        //            {
        //                TeamData = (from ad in _db.Employee
        //                            orderby ad.FirstName
        //                            where (ad.RoleId == 31 || ad.RoleId == 21 || ad.RoleId == 130 || ad.RoleId == 136) && ad.IsDeleted == false

        //                            select new TeamData
        //                            {
        //                                EmployeeId = ad.EmployeeId,
        //                                FullName = ad.FirstName + " " + ad.LastName,
        //                            }).ToList();
        //            }
        //            else
        //            {
        //                TeamData = teamService.GetLeadsByEmp(empId);
        //            }
        //        }

        //        if (TeamData.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Record Found";
        //            response.TeamDatas = TeamData;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No Record Found!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetSelectAllTeamRole")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetSelectAllTeamRole()
        //{
        //    try
        //    {
        //        Base response = new Base();

        //        List<TeamData> teamDataList = new List<TeamData>();
        //        TeamData teamData;
        //        var TeamData = (from ad in _db.Employee

        //                        join cd in _db.Role on ad.RoleId equals cd.RoleId
        //                        orderby ad.FirstName
        //                        where ad.RoleId != 31 && ad.RoleId != 21 && ad.RoleId != 130 && ad.RoleId != 136 && ad.IsDeleted == false
        //                        select new
        //                        {
        //                            ad.EmployeeId,
        //                            FullName = ad.FirstName + " " + ad.LastName,
        //                            ad.RoleId,
        //                            cd.RoleType
        //                        }).ToList();

        //        foreach (var item in TeamData)
        //        {
        //            teamData = new TeamData();
        //            teamData.EmployeeId = item.EmployeeId;
        //            teamData.FullName = item.FullName;
        //            teamData.RoleId = item.RoleId;
        //            teamData.RoleType = item.RoleType;
        //            teamDataList.Add(teamData);
        //        }

        //        if (TeamData.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Record Found";
        //            response.TeamDatas = teamDataList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No Record Found!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        //#region

        //[Route("GetAllTeam")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllTeam()
        //{
        //    try
        //    {
        //        Base response = new Base();

        //        List<TeamType> teamTypelist = new List<TeamType>();
        //        var TeamData = (from ad in _db.TeamType
        //                        join empbd in _db.Employee on ad.TeamLeadId equals empbd.EmployeeId
        //                        select new
        //                        {
        //                            ad.TeamLeadId,
        //                            ad.TeamName,
        //                            ad.TeamTypeId,
        //                            ad.UpdatedDate,
        //                            FullName = empbd.FirstName + " " + empbd.LastName,
        //                        }).ToList();

        //        foreach (var i in TeamData)
        //        {
        //            var empData = _db.Employee.Where(x => x.TeamTypeId == i.TeamTypeId).ToList();

        //            TeamType teamTypeObj = new TeamType();

        //            teamTypeObj.TeamTypeId = i.TeamTypeId;
        //            teamTypeObj.TeamName = i.TeamName;
        //            teamTypeObj.TeamLeadName = i.FullName;
        //            teamTypeObj.TeamLeadId = i.TeamLeadId;
        //            teamTypeObj.UpdatedDate = i.UpdatedDate;
        //            List<EmployeeMiniModel> MiniModellst = new List<EmployeeMiniModel>();

        //            foreach (var item in empData)
        //            {
        //                EmployeeMiniModel employeeMiniModel = new EmployeeMiniModel();

        //                var roleData = _db.Role.Where(x => x.RoleId == item.RoleId).FirstOrDefault();
        //                employeeMiniModel.EmployeeId = item.EmployeeId;
        //                employeeMiniModel.FullName = item.FirstName + " " + item.LastName;
        //                employeeMiniModel.RoleId = item.RoleId;
        //                employeeMiniModel.RoleType = roleData.RoleType;

        //                MiniModellst.Add(employeeMiniModel);
        //            }
        //            // teamTypeObj.NumberOfCounts = MiniModellst.Count();
        //            teamTypeObj.TeamMemberArray = MiniModellst;
        //            teamTypelist.Add(teamTypeObj);
        //        }
        //        if (teamTypelist.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.TeamVM = teamTypelist;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("AddTeams")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddTeams(TeamType teamType)
        {
            try
            {
                Base response = new Base();

                var teams = _db.TeamType.Where(x => x.TeamLeadId == teamType.TeamLeadId).FirstOrDefault();

                if (teams == null)
                {
                    TeamType teamObj = new TeamType();
                    teamObj.TeamName = teamType.TeamName;
                    teamObj.TeamLeadId = teamType.TeamLeadId;
                    teamObj.TeamLeadName = null;
                    teamObj.UpdatedDate = DateTime.Now;
                    teamObj.CreatedDate = DateTime.Now;
                    teamObj.IsActive = true;
                    teamObj.IsDeleted = false;

                    _db.TeamType.Add(teamObj);
                    _db.SaveChanges();
                }

                var teamTypeData = _db.TeamType.Where(x => x.TeamLeadId == teamType.TeamLeadId).FirstOrDefault();

                var leadData = _db.Employee.Where(x => x.EmployeeId == teamType.TeamLeadId).FirstOrDefault();

                if (leadData != null)
                {
                    leadData.TeamTypeId = teamTypeData.TeamTypeId;
                    _db.Entry(leadData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                foreach (var item in teamType.TeamMemberArray)
                {
                    var empData = _db.Employee.Where(x => x.EmployeeId == item.EmployeeId).FirstOrDefault();

                    if (empData != null)
                    {
                        empData.TeamTypeId = teamTypeData.TeamTypeId;
                        _db.Entry(empData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                response.StatusReason = true;
                response.Message = "Team Saved Successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("GetTeamFilter")]
        //[HttpPost]
        //public IHttpActionResult GetTeamFilter(TeamFilterDTO TeamFilterDTO)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<TeamType> teamTypelist = new List<TeamType>();

        //        if (TeamFilterDTO.EmployeeId != 0 && TeamFilterDTO.CreatedDate != default(DateTime))
        //        {
        //            TeamFilterDTO.CreatedDate = TeamFilterDTO.CreatedDate.AddDays(1);
        //            var TeamData = (from ad in _db.TeamType
        //                            join empbd in _db.Employee on ad.TeamLeadId equals empbd.EmployeeId
        //                            where (TeamFilterDTO.EmployeeId == ad.TeamLeadId &&
        //                            TeamFilterDTO.CreatedDate.Day == ad.CreatedDate.Day &&
        //                            TeamFilterDTO.CreatedDate.Month == ad.CreatedDate.Month &&
        //                            TeamFilterDTO.CreatedDate.Year == ad.CreatedDate.Year) && ad.IsDeleted == false

        //                            select new
        //                            {
        //                                ad.TeamLeadId,
        //                                ad.TeamName,
        //                                ad.TeamTypeId,
        //                                ad.UpdatedDate,
        //                                FullName = empbd.FirstName + " " + empbd.LastName,
        //                            }).ToList();

        //            foreach (var i in TeamData)
        //            {
        //                var empData = _db.Employee.Where(x => x.TeamTypeId == i.TeamTypeId).ToList();

        //                TeamType teamTypeObj = new TeamType();

        //                teamTypeObj.TeamTypeId = i.TeamTypeId;
        //                teamTypeObj.TeamName = i.TeamName;
        //                teamTypeObj.TeamLeadName = i.FullName;
        //                teamTypeObj.TeamLeadId = i.TeamLeadId;
        //                teamTypeObj.UpdatedDate = i.UpdatedDate;
        //                List<EmployeeMiniModel> MiniModellst = new List<EmployeeMiniModel>();

        //                foreach (var item in empData)
        //                {
        //                    EmployeeMiniModel employeeMiniModel = new EmployeeMiniModel();

        //                    var roleData = _db.Role.Where(x => x.RoleId == item.RoleId).FirstOrDefault();
        //                    employeeMiniModel.EmployeeId = item.EmployeeId;
        //                    employeeMiniModel.FullName = item.FirstName + " " + item.LastName;
        //                    employeeMiniModel.RoleId = item.RoleId;
        //                    employeeMiniModel.RoleType = roleData.RoleType;

        //                    MiniModellst.Add(employeeMiniModel);
        //                }
        //                // teamTypeObj.NumberOfCounts = MiniModellst.Count();
        //                teamTypeObj.TeamMemberArray = MiniModellst;
        //                teamTypelist.Add(teamTypeObj);
        //            }
        //        }
        //        if (TeamFilterDTO.EmployeeId != 0 && TeamFilterDTO.CreatedDate == default(DateTime))
        //        {
        //            var TeamData = (from ad in _db.TeamType
        //                            join empbd in _db.Employee on ad.TeamLeadId equals empbd.EmployeeId
        //                            where (TeamFilterDTO.EmployeeId == ad.TeamLeadId)
        //                            select new
        //                            {
        //                                ad.TeamLeadId,
        //                                ad.TeamName,
        //                                ad.TeamTypeId,
        //                                ad.UpdatedDate,
        //                                FullName = empbd.FirstName + " " + empbd.LastName,
        //                            }).ToList();

        //            foreach (var i in TeamData)
        //            {
        //                var empData = _db.Employee.Where(x => x.TeamTypeId == i.TeamTypeId).ToList();

        //                TeamType teamTypeObj = new TeamType();

        //                teamTypeObj.TeamTypeId = i.TeamTypeId;
        //                teamTypeObj.TeamName = i.TeamName;
        //                teamTypeObj.TeamLeadName = i.FullName;
        //                teamTypeObj.TeamLeadId = i.TeamLeadId;
        //                teamTypeObj.UpdatedDate = i.UpdatedDate;
        //                List<EmployeeMiniModel> MiniModellst = new List<EmployeeMiniModel>();

        //                foreach (var item in empData)
        //                {
        //                    EmployeeMiniModel employeeMiniModel = new EmployeeMiniModel();

        //                    var roleData = _db.Role.Where(x => x.RoleId == item.RoleId).FirstOrDefault();
        //                    employeeMiniModel.EmployeeId = item.EmployeeId;
        //                    employeeMiniModel.FullName = item.FirstName + " " + item.LastName;
        //                    employeeMiniModel.RoleId = item.RoleId;
        //                    employeeMiniModel.RoleType = roleData.RoleType;

        //                    MiniModellst.Add(employeeMiniModel);
        //                }
        //                // teamTypeObj.NumberOfCounts = MiniModellst.Count();
        //                teamTypeObj.TeamMemberArray = MiniModellst;
        //                teamTypelist.Add(teamTypeObj);
        //            }
        //        }
        //        if (TeamFilterDTO.EmployeeId == 0 && TeamFilterDTO.CreatedDate != default(DateTime))
        //        {
        //            TeamFilterDTO.CreatedDate = TeamFilterDTO.CreatedDate.AddDays(1);
        //            var TeamData = (from ad in _db.TeamType
        //                            join empbd in _db.Employee on ad.TeamLeadId equals empbd.EmployeeId
        //                            where (TeamFilterDTO.CreatedDate.Day == ad.CreatedDate.Day &&
        //                            TeamFilterDTO.CreatedDate.Month == ad.CreatedDate.Month &&
        //                            TeamFilterDTO.CreatedDate.Year == ad.CreatedDate.Year) && ad.IsDeleted == false
        //                            select new
        //                            {
        //                                ad.TeamLeadId,
        //                                ad.TeamName,
        //                                ad.TeamTypeId,
        //                                ad.UpdatedDate,
        //                                FullName = empbd.FirstName + " " + empbd.LastName,
        //                            }).ToList();

        //            foreach (var i in TeamData)
        //            {
        //                var empData = _db.Employee.Where(x => x.TeamTypeId == i.TeamTypeId).ToList();

        //                TeamType teamTypeObj = new TeamType();

        //                teamTypeObj.TeamTypeId = i.TeamTypeId;
        //                teamTypeObj.TeamName = i.TeamName;
        //                teamTypeObj.TeamLeadName = i.FullName;
        //                teamTypeObj.TeamLeadId = i.TeamLeadId;
        //                teamTypeObj.UpdatedDate = i.UpdatedDate;
        //                List<EmployeeMiniModel> MiniModellst = new List<EmployeeMiniModel>();

        //                foreach (var item in empData)
        //                {
        //                    EmployeeMiniModel employeeMiniModel = new EmployeeMiniModel();

        //                    var roleData = _db.Role.Where(x => x.RoleId == item.RoleId).FirstOrDefault();
        //                    employeeMiniModel.EmployeeId = item.EmployeeId;
        //                    employeeMiniModel.FullName = item.FirstName + " " + item.LastName;
        //                    employeeMiniModel.RoleId = item.RoleId;
        //                    employeeMiniModel.RoleType = roleData.RoleType;

        //                    MiniModellst.Add(employeeMiniModel);
        //                }
        //                // teamTypeObj.NumberOfCounts = MiniModellst.Count();
        //                teamTypeObj.TeamMemberArray = MiniModellst;
        //                teamTypelist.Add(teamTypeObj);
        //            }
        //        }

        //        if (teamTypelist.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.TeamVM = teamTypelist;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("UpdateTeam")]
        [HttpPut]
        public IHttpActionResult UpdateTeam(TeamType teamType)
        {
            try
            {
                Base response = new Base();

                var teamData = _db.TeamType.Where(x => x.TeamTypeId == teamType.TeamTypeId).FirstOrDefault();

                if (teamData != null)
                {
                    teamData.TeamName = teamType.TeamName;
                    _db.Entry(teamData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    var empData = _db.Employee.Where(x => x.TeamTypeId == teamType.TeamTypeId).ToList();

                    //foreach (var i in empData)
                    //{
                    //    //i.TeamTypeId = null;
                    //    db.Entry(i).State = System.Data.Entity.EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                    foreach (var t in teamType.TeamMemberArray)
                    {
                        var empDetails = _db.Employee.Where(x => x.EmployeeId == t.EmployeeId).FirstOrDefault();

                        if (empDetails != null)
                        {
                            empDetails.TeamTypeId = teamType.TeamTypeId;
                            _db.Entry(empDetails).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                            response.StatusReason = true;
                            response.Message = "Team Updated Successfully";
                        }
                        else
                        {
                            response.StatusReason = false;
                            response.Message = "Records not found";
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("DeleteTeams")]
        [HttpDelete]
        public IHttpActionResult DeleteTeams(int TeamTypeId)
        {
            Base response = new Base();
            if (TeamTypeId != 0)
            {
                var empData = _db.Employee.Where(x => x.TeamTypeId == TeamTypeId).ToList();

                foreach (var item in empData)
                {
                    item.TeamTypeId = null;
                    _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            var teamTypeData = _db.TeamType.Where(x => x.TeamTypeId == TeamTypeId).FirstOrDefault();
            if (teamTypeData != null)
            {
                _db.TeamType.Remove(teamTypeData);
                _db.SaveChanges();
                response.StatusReason = true;
                response.Message = "Teams  Deleted Successfully";
            }
            else
            {
                response.StatusReason = false;
                response.Message = "No Record Found!!";
            }
            return Ok(response);
        }

        #endregion

        #region

        [Route("DeleteTeamMember")]
        [HttpDelete]
        public IHttpActionResult DeleteTeamMember(int EmpId)
        {
            try
            {
                Base response = new Base();
                if (EmpId != 0)
                {
                    var empData = _db.Employee.Where(x => x.EmployeeId == EmpId).FirstOrDefault();
                    if (empData != null)
                    {
                        empData.TeamTypeId = null;
                        _db.Entry(empData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        response.StatusReason = true;
                        response.Message = "Record Found!!";
                    }
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Record not Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("AddQuestion")]
        [HttpPost]
        public IHttpActionResult AddQuestion(CategoryModel categoryModel)
        {
            try
            {
                Base response = new Base();
                if (categoryModel.Questions.Count == 0)
                {
                    response.StatusReason = false;
                    response.Message = "List is empty";
                    return Ok(response);
                }
                //Category cat = new Category();
                //cat.CategoryName = categoryModel.CategoryName;
                //cat.CategoryTypeId = categoryModel.CategoryTypeId;
                //cat.UsertypeId = categoryModel.UsertypeId;
                //cat.CreatedDate = DateTime.Now;
                //cat.UpdatedDate = DateTime.Now;
                //var res = CategoryService.AddCategory(cat);

                foreach (var item in categoryModel.listUsertypeId)
                {
                    Category cat = new Category();
                    cat.CategoryName = categoryModel.CategoryName;
                    cat.CategoryTypeId = categoryModel.CategoryTypeId;
                    cat.UsertypeId = item;
                    cat.CreatedDate = DateTime.Now;
                    cat.UpdatedDate = DateTime.Now;
                    var res = categoryService.AddCategory(cat);
                    //db.Category.Add(cat);
                    //db.SaveChanges();

                    foreach (var item1 in categoryModel.Questions)
                    {
                        Questions ProData = new Questions();
                        ProData.Question = item1.Question;
                        ProData.UpdatedDate = DateTime.Now;
                        ProData.CreatedDate = DateTime.Now;

                        // ProData.CategoryName = categoryModel.CategoryName;
                        //ProData.Category_Type = categoryModel.Category_Type;

                        //add category
                        ProData.CategoryId = cat.CategoryId;
                        //add new question
                        _db.Questions.Add(ProData);
                        _db.SaveChanges();
                    }
                }

                response.StatusReason = true;
                response.Message = "Category added Successfully";

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //#region

        //[Route("GetAllQuestion")]
        //[HttpGet]
        //public IHttpActionResult GetAllQuestion()
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        List<DataQuestion> QuestionList = new List<DataQuestion>();
        //        var QuestionData = (from ad in _db.Questions

        //                            select new
        //                            {
        //                                ad.QuestionId,
        //                                ad.Question
        //                            }).ToList();

        //        foreach (var item in QuestionData)
        //        {
        //            DataQuestion data = new DataQuestion();
        //            data.Question = item.Question;
        //            data.QuestionId = item.QuestionId;
        //            QuestionList.Add(data);
        //        }
        //        if (QuestionList.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.DataQuestion = QuestionList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion

        #region

        [Route("UpdateQuestion")]
        [HttpPut]
        public IHttpActionResult UpdateQuestion(CategoryModel categoryModel)
        {
            try
            {
                Base response = new Base();

                if (categoryModel.Questions.Count == 0)
                {
                    response.StatusReason = false;
                    response.Message = "List is empty";
                    return Ok(response);
                }
                else
                {
                    foreach (var item in categoryModel.Questions)
                    {
                        Questions ques = _db.Questions.Where(x => x.QuestionId == item.QuestionId).FirstOrDefault();
                        if (ques != null)
                        {
                            ques.QuestionId = item.QuestionId;
                            ques.Question = item.Question;
                            ques.UpdatedDate = DateTime.Now;
                            ques.CategoryId = item.CategoryId;

                            _db.Entry(ques).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                            response.StatusReason = true;
                            response.Message = "Question Updated Successfully";
                        }
                        else
                        {
                            response.StatusReason = false;
                            response.Message = "Records not found";
                        }
                    }
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        #endregion

        #region

        [Route("AddQueOnExistingCategory")]
        [HttpPost]
        public IHttpActionResult AddQueOnExistingCategory(CategoryModel categoryModel)
        {
            try
            {
                Base response = new Base(); if (categoryModel.Questions.Count == 0)
                {
                    response.StatusReason = false;
                    response.Message = "List is empty";
                }
                else
                {
                    var checkCategory = _db.Category.Where(x => x.CategoryId == categoryModel.CategoryId).FirstOrDefault();
                    if (checkCategory != null)
                    {
                        foreach (var item in categoryModel.Questions)
                        {
                            Questions questions = new Questions();
                            questions.CategoryId = categoryModel.CategoryId;
                            questions.Question = item.Question;
                            questions.UpdatedDate = DateTime.Now;
                            questions.CreatedDate = DateTime.Now;
                            _db.Questions.Add(questions);
                            _db.SaveChanges();
                        }
                        response.StatusReason = true;
                        response.Message = "Questions Added Successfully";
                    }
                    else
                    {
                        response.StatusReason = false;
                        response.Message = "Oops! Category not Exist";
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        #endregion

        #region

        [Route("DeleteQuestions")]
        [HttpDelete]
        public IHttpActionResult DeleteQuestions(int QuestionId)
        {
            try
            {
                Base response = new Base();
                var questionDelete = _db.Questions.Where(x => x.QuestionId == QuestionId).FirstOrDefault();

                if (questionDelete != null)
                {
                    _db.Questions.Remove(questionDelete);
                    _db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Question Delete Successfully";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Please select question";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #endregion

        #region

        [Route("TotalManagerCoordinator")]
        [HttpGet]
        public IHttpActionResult TotalNumberOfManagerCoordinator()
        {
            try
            {
                Base response = new Base();
                var totalProjectManagers = _db.Employee.Where(x => x.RoleId == 1 && x.IsDeleted == false).Count();
                var totalProjectCoordinator = _db.Employee.Where(x => x.RoleId == 2 && x.IsDeleted == false).Count();
                response.totalProjectManagers = totalProjectManagers;
                response.totalProjectCoordinator = totalProjectCoordinator;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("CreateResource")]
        [HttpPost]
        public IHttpActionResult CreateResource(Resource createResource)
        {
            try
            {
                Base response = new Base();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    Resource resourceData = new Resource();
                    resourceData.Name = createResource.Name;
                    resourceData.Designation = createResource.Designation;
                    resourceData.ProjectManager = createResource.ProjectManager;
                    resourceData.TechLead = createResource.TechLead;
                    resourceData.Technology = createResource.Technology;
                    resourceData.Project = createResource.Project;
                    resourceData.BillType = createResource.BillType;
                    resourceData.CompanyName = createResource.CompanyName;
                    resourceData.CreatedDate = DateTime.Now;
                    resourceData.IsDelete = false;
                    resourceData.IsActive = true;

                    _db.Resource.Add(resourceData);
                    _db.SaveChanges();

                    //foreach (var item in resourceData.Technology)
                    //{
                    //    ResourceTechnology data = new ResourceTechnology();
                    //    data.ResourceID = resourceData.ResourceId;
                    //    data.Technology = item;
                    //    data.IsActive = true;
                    //    data.IsDeleted = false;
                    //    db.ResourceTechnology.Add(data);
                    //    db.SaveChanges();
                    //}

                    //foreach (var item in resourceData.Project)
                    //{
                    //    ResourceProject data = new ResourceProject();
                    //    data.ResourceID = resourceData.ResourceId;
                    //    data.Project = item;
                    //    data.IsActive = true;
                    //    data.IsDeleted = false;
                    //    db.ResourceProject.Add(data);
                    //    db.SaveChanges();
                    //}

                    //foreach (var item in resourceData.ProjectManager)
                    //{
                    //    ResourcePM data = new ResourcePM();
                    //    data.ResourceID = resourceData.ResourceId;
                    //    data.ProjectManager = item;
                    //    data.IsActive = true;
                    //    data.IsDeleted = false;
                    //    db.ResourcePM.Add(data);
                    //    db.SaveChanges();
                    //}

                    response.StatusReason = true;
                    response.Message = "Resource Saved Successfully";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateResource")]
        [HttpPut]
        public IHttpActionResult UpdateResource(Resource updateResource)
        {
            try
            {
                Base response = new Base();

                var resourceData = (from ad in _db.Resource where ad.ResourceId == updateResource.ResourceId select ad).FirstOrDefault();
                resourceData.Name = updateResource.Name;
                resourceData.Designation = updateResource.Designation;
                resourceData.ProjectManager = updateResource.ProjectManager;
                resourceData.TechLead = updateResource.TechLead;
                resourceData.Technology = updateResource.Technology;
                resourceData.Project = updateResource.Project;
                resourceData.BillType = updateResource.BillType;
                resourceData.CompanyName = updateResource.CompanyName;
                resourceData.IsDelete = false;
                resourceData.IsActive = true;
                _db.SaveChanges();

                response.StatusReason = true;
                response.Message = "Resource Updated";

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetResources")]
        [HttpGet]
        public IHttpActionResult GetResources()
        {
            try
            {
                List<resourcesData> resourcesDataList = new List<resourcesData>();
                var finalData = (from ad in _db.Resource
                                 join bd in _db.Employee on ad.ProjectManager equals bd.EmployeeId
                                 join cd in _db.Project on ad.Project equals cd.ProjectId
                                 join td in _db.Technology on ad.Technology equals td.TechnologyId
                                 join ld in _db.ResourceCompany on ad.CompanyName equals ld.CompanyId
                                 join rd in _db.Role on ad.Designation equals rd.RoleId
                                 select new
                                 {
                                     ad.ResourceId,
                                     ad.Name,
                                     rd.RoleType,
                                     v = bd.FirstName + " " + bd.LastName,
                                     ad.TechLead,
                                     td.TechnologyType,
                                     cd.ProjectName,
                                     ad.BillType,
                                     ld.CompanyName
                                 }).ToList();
                foreach (var item in finalData)
                {
                    resourcesData data = new resourcesData();
                    data.ResourceId = item.ResourceId;
                    data.Name = item.Name;
                    data.Designation = item.RoleType;
                    data.ProjectManager = item.v;
                    data.TechLead = item.TechLead;
                    data.TechnologyType = item.TechnologyType;
                    data.ProjectcName = item.ProjectName;
                    data.BillType = item.BillType;
                    data.CompanyName = item.CompanyName;

                    //var data1 = db.Employee.ToList();
                    //foreach (var item1 in data1)
                    //{
                    //    ResourcePM dt = new ResourcePM();
                    //    dt.ProjectManager = item1.BankName;
                    //    data.ResourcePM.Add(dt);
                    //}

                    //var data1 = db.Employee.ToList();
                    //foreach (var item1 in data1)
                    //{
                    //    ResourcePM dt = new ResourcePM();
                    //    dt.ProjectManager = item1.BankName;
                    //    data.ResourcePM.Add(dt);
                    //}

                    //var data1 = db.Employee.ToList();
                    //foreach (var item1 in data1)
                    //{
                    //    ResourcePM dt = new ResourcePM();
                    //    dt.ProjectManager = item1.BankName;
                    //    data.ResourcePM.Add(dt);
                    //}

                    resourcesDataList.Add(data);
                }
                return Ok(resourcesDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class resourcesData
        {
            public int ResourceId { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
            public string ProjectManager { get; set; }
            public string TechLead { get; set; }
            public string TechnologyType { get; set; }

            public string ProjectcName { get; set; }
            public string CompanyName { get; set; }
            public string BillType { get; set; }

            public List<ResourcePMData> ResourcePM { get; set; }
            public List<ResourceProjectData> ResourceProject { get; set; }
            public List<ResourceTechnologyData> ResourceTechnology { get; set; }
        }

        public class ResourcePMData
        {
            public string ProjectManager { get; set; }
        }

        public class ResourceProjectData
        {
            public string Project { get; set; }
        }

        public class ResourceTechnologyData
        {
            public string Technology { get; set; }
        }

        [Route("GetProjectByManager")]
        [HttpGet]
        public IHttpActionResult GetProjectByManager(int Id)
        {
            try
            {
                Base response = new Base();
                var projectData = (from ad in _db.Project where ad.EmployeeId == Id && ad.IsDeleted == false select ad).ToList();
                if (projectData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.projectData = projectData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Record Not Found";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetResourceCompany")]
        [HttpGet]
        public IHttpActionResult GetResourceCompany()
        {
            try
            {
                Base response = new Base();
                var resourceData = (from ad in _db.ResourceCompany select ad).ToList();
                if (resourceData.Count != 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.resourceData = resourceData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "Record Not Found";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("GetStatus")]
        [HttpGet]
        public IHttpActionResult GetStatus()
        {
            try
            {
                Base response = new Base();
                var statusData = _db.Status.Where(x => x.IsDeleted == false && x.StatusType == "Project").ToList();
                if (statusData.Count > 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.statusData = statusData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("GetProjectTypes")]
        [HttpGet]
        public IHttpActionResult GetProjectTypes()
        {
            try
            {
                Base response = new Base();
                var projectTypeData = _db.ProjectTypes.Where(x => x.IsDeleted == false).ToList();
                if (projectTypeData.Count > 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.projectTypeData = projectTypeData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region

        [Route("GetBillTypes")]
        [HttpGet]
        public IHttpActionResult GetBillTypes()
        {
            try
            {
                Base response = new Base();
                var billTypeData = _db.BillType.Where(x => x.IsDeleted == false).ToList();
                if (billTypeData.Count > 0)
                {
                    response.StatusReason = true;
                    response.Message = "Record Found";
                    response.billTypeData = billTypeData;
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Record Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #endregion

        #region Helper classes

        public class TechData
        {
            public int ProjectID { get; set; }
            public int ProjectTechId { get; set; }
            public int TechnologyID { get; set; }
            public string TechnologyType { get; set; }
        }

        public class ProjectData
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public DateTime? ActualStartDate { get; set; }
            public DateTime? ActualEndDate { get; set; }
            public string TechnologyType { get; set; }
            public string StatusVal { get; set; }
            public string Project_Type { get; set; }
            public string Bill_Type { get; set; }
            public int Companyid { get; set; }
            public int StatusId { get; set; }
            public int TechnologyId { get; set; }
            public int ProjectTypeId { get; set; }
            public int BillTypeId { get; set; }
            public double ProjectPrice { get; set; }
            public int EmployeeId { get; set; }
            public int TeamId { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsActive { get; set; }

            //Project Manager Name
            public string FullName { get; set; }

            //Class for team api
            public string Email { get; set; }

            public string RoleType { get; set; }
            //class for task api

            public string Task { get; set; }
            public string Description { get; set; }
            public string AssignedTo { get; set; }
            public string AssignedBy { get; set; }
            public string TaskStatus { get; set; }
            public DateTime? CreatedOn { get; set; }
            public string AssignedToName { get; set; }
            public string AssignedByName { get; set; }
        }
        public class DataQuestion
        {
            public int QuestionId { get; set; }
            public string Question { get; set; }
        }

        public class DataCategory
        {
            public int CategoryId { get; set; }

            public string CategoryName { get; set; }

            public string Category_Type { get; set; }
            public int CategoryTypeId { get; set; }
            public int RoleId { get; set; }
            public int UserId { get; set; }
            public int QuestionId { get; set; }
            public List<Questions> quenstions { get; set; }
            public List<QuestionScore> questionScores { get; set; }

            //public int TotalNoQuestion { get; set; }
            public string RoleType { get; set; }

            public DateTime CreatedDate { get; set; }

            public DateTime UpdatedDate { get; set; }
            public int CategoryAverageScore { get; set; }
        }

        public class TeamTypeVM
        {
            public int TeamTypeId { get; set; }
            public int TeamLeadId { get; set; }
            public string TeamName { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
            public List<EmployeeMiniModel> TeamMemberArray { get; set; }
            public string TeamLeadName { get; set; }
            public string FullName { get; set; }
            public int EmployeeId { get; set; }
            public int RoleId { get; set; }
        }

        public class FeedbackData
        {
            public int FeedbackId { get; set; }
            public int ReceiverEmployeeId { get; set; }
            public string ReceiverEmployeeName { get; set; }
            public DateTime UpdatedDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public int RatedByEmpId { get; set; }
            public string RatedByEmpName { get; set; }
            public string YourFeedback { get; set; }
            public int CategoryTypeId { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string CategoryTypeName { get; set; }
            public string CategoryType { get; set; }
            public List<FeedbackScoredata> FBScore { get; set; }
            public int AverageScore { get; set; }
            public List<DataCategory> DataCategories { get; set; }
            public string TeamLeadName { get; set; }
            public int TeamLeadId { get; set; }
        }

        public class FeedbackScoredata
        {
            public int FeedbackId { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int QuestionId { get; set; }
            public string Question { get; set; }
            public int QuestionScore { get; set; }
        }

        public class DataFeedback
        {
            public int FeedbackId { get; set; }
            public int CategoryId { get; set; }
            public int UserId { get; set; }
            public DateTime FeedbackDate { get; set; }
            public string FeedbackType { get; set; }
            public int AverageScore { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime FromDate { get; set; }
            public int EmployeeId { get; set; }
            public string RoleType { get; set; }
            public string Category_Type { get; set; }
            public List<Employee> Employees { get; set; }
            public string FullName { get; set; }
            public DateTime UpdatedDate { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public int RoleId { get; set; }
        }

        public class CountData
        {
            public int TotalFeedbacks { get; set; }
            public int TotalTeams { get; set; }
            public int TotalCategory { get; set; }
        }

        public class TeamData
        {
            public int EmployeeId { get; set; }
            public string FullName { get; set; }
            public int RoleId { get; set; }
            public string RoleType { get; set; }
        }

        public class GetDataEmployeeModal
        {
            public List<DataEmployee> EmployeeList { get; set; }
            public double BillAndNonBilEmpSalary { get; set; }
            public int ClientBillableAmount { get; set; }
            public string ClientBillableAmounts { get; set; }
            public double Diffrence { get; set; }
            public double YearlyBurn { get; set; }
            public double MontlyBurn { get; set; }
            public double Profit { get; set; }
            public double Loss { get; set; }
            public string MonthlyDiffer { get; set; }
            public string YearlyBurns { get; set; }
            public string MontlyBurns { get; set; }
            public string Profits { get; set; }
            public string Losses { get; set; }
            public string ProjectName { get; set; }
            public string CompanyName { get; set; }
            public string Technologies { get; set; }
            public bool ProjectStatus { get; set; }
            public double SumofBillableSalary { get; set; }
            public double SumofNonBillableSalary { get; set; }
            public string ProfitPer { get; set; }
            public string LossPer { get; set; }
            public int BillableResourceCount { get; set; }
            public int NonBillableResourceCount { get; set; }
            public string LeadType { get; set; }
            public string Currency { get; set; }
            public string EmployeesMonthlySalary { get; set; }
            public string MonthlyProjectExpense { get; set; }
        }

        public class DataEmployee
        {
            public int EmployeeId { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PrimaryContact { get; set; }
            public DateTime? CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public double Salary { get; set; }
            public string RoleType { get; set; }
            public string Status { get; set; }
            public string OccupyPercent { get; set; }
            public int AssignProjectId { get; set; }
            public string DesignationName { get; set; }
            public double SumofBillableSalary { get; set; }
            public double SumofNonBillableSalary { get; set; }
            public string MonthlySalary { get; set; }
            public string CurrentMonthExpense { get; set; }
            public bool IsProjectManager { get; set; }
            public string RoleName { get; set; }

        }

        public class ProData
        {
            public int Id { get; set; }
            public string ProjectName { get; set; }
            public string ProjectManager { get; set; }
            public string Status { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public string OccupyPercent { get; set; }
        }

        public class AssignProjectData
        {
            public int AssignProjectId { get; set; }
            public List<int> Project { get; set; }
            public int ProjectId { get; set; }
            public int EmployeeId { get; set; }
            public int ManagerId { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }

        #endregion
    }
}