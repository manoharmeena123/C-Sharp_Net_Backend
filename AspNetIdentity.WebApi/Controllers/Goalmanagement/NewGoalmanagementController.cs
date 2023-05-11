using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.GoalManagement;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;

namespace AspNetIdentity.WebApi.Controllers.Goalmanagement
{
    /// <summary>
    /// Created By Ankit Jain on 19-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newgoal")]
    public class NewGoalmanagementController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api To Get Goal Cycle
        /// <summary>
        /// Created By ankit Jain on 19-01-2023
        /// API >> Get >> api/newgoal/getgoalcycle
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getgoalcycle")]
        public IHttpActionResult GetAllCycle()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalCycle = Enum.GetValues(typeof(GoalCycleConstants))
                                .Cast<GoalCycleConstants>()
                                .Select(x => new
                                {
                                    GoalTypeId = (int)x,
                                    GoalTypeName = Enum.GetName(typeof(GoalCycleConstants), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Get Goal Cycle List Found !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = goalCycle;
            }
            catch (Exception ex)
            {
                logger.Error("API :api/newgoal/getgoalcycle | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        #endregion Api To Get GoalType

        #region Api To Get Goal Status
        /// <summary>
        /// Created By ankit Jain on 19-01-2023
        /// API >> Get >> api/newgoal/getgoalstatus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getgoalstatus")]
        public IHttpActionResult GetGoalStatus()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalCycle = Enum.GetValues(typeof(GoalStatusConstantsClass))
                                .Cast<GoalStatusConstantsClass>()
                                .Select(x => new GolaTypeList
                                {
                                    GoalStatusId = (int)x,
                                    GoalStatusName = Enum.GetName(typeof(GoalStatusConstantsClass), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Get Goal Cycle List Found !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = goalCycle;
            }
            catch (Exception ex)
            {
                logger.Error("API :api/newgoal/getgoalcycle | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        /// <summary>
        /// Created By Harshit Mitra on 02-05-2022
        /// </summary>
        public class GolaTypeList
        {
            public int GoalStatusId { get; set; }
            public string GoalStatusName { get; set; }
        }
        #endregion Api To Get GoalType

        #region Api To Get Goal Reviewer Type
        /// <summary>
        /// Created By ankit Jain on 19-01-2023
        /// API >> Get >> api/newgoal/getreviewertype
        /// <returns></returns>
        [HttpGet]
        [Route("getreviewertype")]
        public IHttpActionResult GetReviewerType(GoalTypeConstants type)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalCycle = Enum.GetValues(typeof(GoalReviewerType))
                                .Cast<GoalReviewerType>()
                                .Select(x => new
                                {
                                    GoalTypeId = (int)x,
                                    GoalTypeName = Enum.GetName(typeof(GoalReviewerType), x).Replace("_", " "),
                                    IsEnable = (type == GoalTypeConstants.Individual_Goal) ? true : !((x == GoalReviewerType.ReportingManager) || (x == GoalReviewerType.Both)),

                                }).ToList();

                res.Message = "Get Goal List Found !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = goalCycle;
            }
            catch (Exception ex)
            {
                logger.Error("API :api/newgoal/getgoalcycle | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        #endregion Api To Get GoalType

        #region This Api To Get All Employee List
        /// <summary>
        /// Created By Ankit Jain on 19-01-2023
        /// API >> api/newgoal/getallemployeelist
        /// </summary>
        [HttpGet]
        [Route("getallemployeelist")]
        public async Task<IHttpActionResult> GetEmployeeList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          where e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,
                                              isReportingManagerNotAssign = e.ReportingManager == 0
                                          }).ToListAsync();
                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = employeeList;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getallemployeelist | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion

        #region This Api To Get All Department List
        /// <summary>
        /// Created By Ankit Jain on 25-01-2023
        /// API >> api/newgoal/getalldepartmentlist
        /// </summary>
        [HttpGet]
        [Route("getalldepartmentlist")]
        public async Task<IHttpActionResult> GetAllDepartmentList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var departmentList = await (from e in _db.Department
                                            where e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId
                                            select new
                                            {
                                                e.DepartmentId,
                                                e.DepartmentName,
                                            }).ToListAsync();
                if (departmentList.Count > 0)
                {
                    res.Message = "Department List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = departmentList;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getalldepartmentlist | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion

        #region This Api To Get Employee By Department Id
        /// <summary>
        /// Created By Ankit Jain on 25-01-2023
        /// API >> api/newgoal/getemployeebydepartmentid
        /// </summary>
        [HttpGet]
        [Route("getemployeebydepartmentid")]
        public async Task<IHttpActionResult> GetEmployeeByDepartmentId(int departmentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          where e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId
                                          && e.DepartmentId == departmentId
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,
                                          }).ToListAsync();
                if (employeeList.Count > 0)
                {
                    res.Message = "Empoloyee List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = employeeList;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getemployeebydepartmentid | " +
                     "DepartmentId : " + departmentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion

        #region API TO CREATE Goal 
        /// <summary>
        /// Created By Ankit Jain On 25-01-2023
        /// API >> POST >> api/newgoal/addnewgoal
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addnewgoal")]
        public async Task<IHttpActionResult> AddNewGoal(AddGoalRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var createDate = TimeZoneConvert.ConvertTimeToSelectedZone
                        (DateTime.UtcNow, tokenData.TimeZone);
                    var employeeInGoal = await _db.Employee
                        .Where(x => model.EmployeeIds.Contains(x.EmployeeId))
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.ReportingManager,
                        })
                        .ToListAsync();

                    if (model.GoalType == GoalTypeConstants.Individual_Goal)
                    {
                        foreach (var emp in model.EmployeeIds)
                        {
                            GoalManagement obj = new GoalManagement
                            {
                                GoalType = GoalTypeConstants.Individual_Goal,
                                GoalOwnerId = emp,
                                GoalCycle = model.GoalCycle,
                                StartDate = model.StartDate,
                                EndDate = model.GoalCycle == GoalCycleConstants.Custom ?
                                    model.StartDate : model.StartDate.AddMonths((int)model.GoalCycle),
                                GoalReviewerType = model.GoalReviewerType,
                                GoalTitle = model.GoalTitle,
                                Description = model.Description,
                                GoalUrl = model.GoalUrl,
                                CompanyId = tokenData.companyId,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = createDate,
                            };
                            int reportingManager = employeeInGoal.Where(x => x.EmployeeId == emp)
                                .Select(x => x.ReportingManager).FirstOrDefault();
                            if (reportingManager == 0)
                            {
                                ReviewersInGoal revSelf = new ReviewersInGoal
                                {
                                    GoalId = obj.GoalId,
                                    ReviewerId = tokenData.employeeId,
                                    CompanyId = tokenData.companyId,
                                    CreatedBy = tokenData.employeeId,
                                    CreatedOn = createDate,
                                };
                                _db.ReviewersInGoals.Add(revSelf);
                                _db.GoalManagements.Add(obj);
                                await _db.SaveChangesAsync();
                                HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread
                                (obj, tokenData, GoalTypeConstants.Individual_Goal));
                            }
                            else
                            {
                                switch (model.GoalReviewerType)
                                {
                                    case GoalReviewerType.Self:
                                        ReviewersInGoal revSelf = new ReviewersInGoal
                                        {
                                            GoalId = obj.GoalId,
                                            ReviewerId = tokenData.employeeId,
                                            CompanyId = tokenData.companyId,
                                            CreatedBy = tokenData.employeeId,
                                            CreatedOn = createDate,
                                        };
                                        _db.ReviewersInGoals.Add(revSelf);
                                        break;
                                    case GoalReviewerType.ReportingManager:
                                        ReviewersInGoal revRep = new ReviewersInGoal
                                        {
                                            GoalId = obj.GoalId,
                                            ReviewerId = employeeInGoal.Where(x => x.EmployeeId == emp).Select(x => x.ReportingManager).FirstOrDefault(),
                                            CompanyId = tokenData.companyId,
                                            CreatedBy = tokenData.employeeId,
                                            CreatedOn = createDate,
                                        };
                                        _db.ReviewersInGoals.Add(revRep);
                                        break;
                                    case GoalReviewerType.Both:
                                        ReviewersInGoal revBoth1 = new ReviewersInGoal
                                        {
                                            GoalId = obj.GoalId,
                                            ReviewerId = tokenData.employeeId,
                                            CompanyId = tokenData.companyId,
                                            CreatedBy = tokenData.employeeId,
                                            CreatedOn = createDate,
                                        };
                                        ReviewersInGoal revBoth2 = new ReviewersInGoal
                                        {
                                            GoalId = obj.GoalId,
                                            ReviewerId = employeeInGoal.Where(x => x.EmployeeId == emp).Select(x => x.ReportingManager).FirstOrDefault(),
                                            CompanyId = tokenData.companyId,
                                            CreatedBy = tokenData.employeeId,
                                        };
                                        _db.ReviewersInGoals.Add(revBoth1);
                                        _db.ReviewersInGoals.Add(revBoth2);
                                        break;
                                    case GoalReviewerType.Custom:
                                        var addReviewer = model.ReviewerIds
                                            .Select(x => new ReviewersInGoal
                                            {
                                                GoalId = obj.GoalId,
                                                ReviewerId = x,
                                                CompanyId = tokenData.companyId,
                                                CreatedBy = tokenData.employeeId,
                                                CreatedOn = createDate,
                                            })
                                            .ToList();
                                        _db.ReviewersInGoals.AddRange(addReviewer);
                                        break;
                                    default:
                                        break;
                                }
                                _db.GoalManagements.Add(obj);
                                await _db.SaveChangesAsync();
                                HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread
                                (obj, tokenData, GoalTypeConstants.Individual_Goal));
                            }

                        }
                    }
                    else
                    {
                        GoalManagement obj = new GoalManagement
                        {
                            GoalType = model.GoalType,
                            GoalOwnerId = model.GoalOwnerId,
                            DepartmentId = model.DepartmentId,
                            GoalCycle = model.GoalCycle,
                            StartDate = model.StartDate,
                            EndDate = model.GoalCycle == GoalCycleConstants.Custom ?
                                    model.StartDate : model.StartDate.AddMonths((int)model.GoalCycle),
                            GoalReviewerType = model.GoalReviewerType,
                            GoalTitle = model.GoalTitle,
                            Description = model.Description,
                            GoalUrl = model.GoalUrl,
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = createDate,
                        };
                        switch (model.GoalReviewerType)
                        {
                            case GoalReviewerType.Self:
                                ReviewersInGoal revSelf = new ReviewersInGoal
                                {
                                    GoalId = obj.GoalId,
                                    ReviewerId = tokenData.employeeId,
                                    CompanyId = tokenData.companyId,
                                    CreatedBy = tokenData.employeeId,
                                    CreatedOn = createDate,
                                };
                                _db.ReviewersInGoals.Add(revSelf);
                                break;
                            case GoalReviewerType.Custom:
                                var addReviewer = model.ReviewerIds
                                    .Select(x => new ReviewersInGoal
                                    {
                                        GoalId = obj.GoalId,
                                        ReviewerId = x,
                                        CompanyId = tokenData.companyId,
                                        CreatedBy = tokenData.employeeId,
                                        CreatedOn = createDate,
                                    })
                                    .ToList();
                                _db.ReviewersInGoals.AddRange(addReviewer);
                                break;
                            default:
                                break;
                        }
                        _db.GoalManagements.Add(obj);
                        await _db.SaveChangesAsync();
                        HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread
                        (obj, tokenData, GoalTypeConstants.Departmental_Goal));
                    }

                    res.Message = "Goal Created!";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/addnewgoal | " +
                   "Model : " + JsonConvert.SerializeObject(model) + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }

        }

        public class AddGoalRequest
        {
            public GoalTypeConstants GoalType { get; set; } = GoalTypeConstants.Individual_Goal;
            public List<int> EmployeeIds { get; set; } = new List<int>();
            public int DepartmentId { get; set; } = 0;
            public GoalCycleConstants GoalCycle { get; set; } = GoalCycleConstants.Monthly;
            public DateTime StartDate { get; set; } = DateTime.UtcNow;
            public GoalReviewerType GoalReviewerType { get; set; } = GoalReviewerType.Self;
            public List<int> ReviewerIds { get; set; } = new List<int>();
            public int GoalOwnerId { get; set; } = 0;
            public string GoalTitle { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public string GoalUrl { get; set; } = string.Empty;
        }
        #endregion

        #region This Use For Mail Function
        public void SendMailInThread(GoalManagement obj, ClaimsHelperModel tokanData, GoalTypeConstants mailType)
        {
            switch (mailType)
            {
                case GoalTypeConstants.Individual_Goal:
                    Thread.Sleep(1000); // 1000 = 1 sec
                    _ = SendMailCreatedGoal(obj.GoalId, tokanData);
                    break;
                case GoalTypeConstants.Departmental_Goal:
                    Thread.Sleep(1000); // 1000 = 1 sec
                    _ = SendMailCreatedGoal(obj.GoalId, tokanData);
                    break;
            }

        }
        #endregion

        #region This Api Use To Send Mail To Created Goal 
        ///// <summary>
        ///// Create By Ankit Jain Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        [Route("getmail")]
        [HttpGet]
        public async Task SendMailCreatedGoal(Guid goalId, ClaimsHelperModel tokenData)
        {
            try
            {
                var goalData = (from g in _db.GoalManagements
                                join gd in _db.ReviewersInGoals on g.GoalId equals gd.GoalId
                                where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                && g.GoalId == goalId
                                select new
                                {
                                    g.CreatedBy,
                                    g.GoalOwnerId,
                                    gd.ReviewerId,
                                    g.Description,
                                    g.StartDate,
                                    g.GoalId,
                                    g.EndDate,
                                    g.GoalUrl,
                                }).FirstOrDefault();
                var createdby = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.CreatedBy);
                var goalOwner = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.GoalOwnerId);
                var reviewerby = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.ReviewerId);
                var st = (DateTimeOffset)goalData.StartDate;
                var startDate = st.ToString("d MMMM, yyyy");
                var URl = goalData.GoalUrl + goalData.GoalId;
                var et = (DateTimeOffset)goalData.EndDate;
                var endDate = et.ToString("d MMMM, yyyy");
                var companylist = _db.Company.Where(y => y.CompanyId == tokenData.companyId
                         && y.IsActive && !y.IsDeleted)
                       .Select(x => new
                       {
                           x.RegisterAddress,
                           x.RegisterCompanyName

                       }).FirstOrDefault();
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (tokenData.IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == tokenData.companyId)
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

                string htmlBody = NewGoalHelper.CreateGoals
                       .Replace("<|GOALASSIGNEENAME|>", goalOwner.DisplayName)
                       .Replace("<|CREATEDBY|>", createdby.DisplayName)
                       .Replace("<|DESCRIPTION|>", goalData.Description)
                       .Replace("<|STARTDATE|>", startDate)
                       .Replace("<|ENDDATE|>", endDate)
                       .Replace("<|IMAGE_PATH|>", "emossy.png")
                       .Replace("<|VIEWGOAL|>", URl)
                       .Replace("<|COMPANYNAME|>", companylist.RegisterCompanyName)
                       .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                    Subject = "Reminder",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { reviewerby.OfficeEmail, goalOwner.OfficeEmail, createdby.OfficeEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region This Api To Get All Goal  
        /// <summary>
        /// Created By Ankit Jain on 27-01-2023
        /// API >> api/newgoal/getallgoal
        /// </summary>
        [HttpGet]
        [Route("getallgoal")]
        public async Task<IHttpActionResult> GetAllGoal()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = _db.Employee.Where(x => x.CompanyId == tokenData.companyId)
                       .Select(x => new
                       {
                           x.EmployeeId,
                           x.DisplayName
                       }).ToList();
                var checkDepartment = _db.Department.Where(x => x.CompanyId == tokenData.companyId)
                       .Select(x => new
                       {
                           x.DepartmentId,
                           x.DepartmentName
                       }).ToList();
                var goalList = await (from g in _db.GoalManagements
                                      join e in _db.Employee on g.GoalOwnerId equals e.EmployeeId
                                      join d in _db.Department on g.DepartmentId equals d.DepartmentId into p
                                      from Result in p.DefaultIfEmpty()
                                      where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                      select new
                                      {
                                          g.GoalId,
                                          g.GoalOwnerId,
                                          g.GoalTitle,
                                          g.Description,
                                          g.GoalReviewerType,
                                          g.GoalType,
                                          g.StartDate,
                                          g.GoalCycle,
                                          g.EndDate,
                                          g.Status,
                                          e.EmployeeId,
                                          g.DepartmentId,
                                          g.CreatedOn,
                                          g.CreatedBy,
                                          g.GoalPercentage
                                      }).ToListAsync();

                #region Check Goal Pemmission 

                var permission = await _db.GoalsPermissions
                    .FirstOrDefaultAsync(x => x.EmployeeId == tokenData.employeeId);
                if (!NewGoalHelper.CheckGoalPermission(permission.Permission, GoalTypeConstants.Individual_Goal, NewGoalHelper.PermissionType.View))
                    goalList = goalList.Where(x => x.GoalType != GoalTypeConstants.Individual_Goal).ToList();
                if (!NewGoalHelper.CheckGoalPermission(permission.Permission, GoalTypeConstants.Departmental_Goal, NewGoalHelper.PermissionType.View))
                    goalList = goalList.Where(x => x.GoalType != GoalTypeConstants.Departmental_Goal).ToList();
                if (!NewGoalHelper.CheckGoalPermission(permission.Permission, GoalTypeConstants.Company_Goal, NewGoalHelper.PermissionType.View))
                    goalList = goalList.Where(x => x.GoalType != GoalTypeConstants.Company_Goal).ToList();

                #endregion

                var goalData = goalList
                        .Select(go => new GetGoalHelper
                        {
                            GoalId = go.GoalId,
                            GoalOwnerFor = go.GoalOwnerId != 0 ? checkData.Where(x => x.EmployeeId == go.GoalOwnerId)
                                                 .Select(x => x.DisplayName).FirstOrDefault() : null,
                            DepartmentName = go.DepartmentId != 0 ? checkDepartment.Where(x => x.DepartmentId == go.DepartmentId)
                                                   .Select(x => x.DepartmentName).FirstOrDefault() : null,
                            GoalCycleName = go.GoalCycle.ToString(),
                            GoalReviewerTypeName = go.GoalReviewerType.ToString(),
                            GoalStatus = go.Status.ToString(),
                            StartDate = go.StartDate,
                            EndDate = go.EndDate,
                            Description = go.Description,
                            GoalTypeName = go.GoalType.ToString(),
                            GoalTitle = go.GoalTitle,
                            CreatedOn = go.CreatedOn,
                            CreatedBy = checkData.Where(x => x.EmployeeId == go.CreatedBy).Select(x => x.DisplayName).FirstOrDefault(),
                            GoalPercentage = go.GoalPercentage,
                        }).OrderByDescending(go => go.CreatedOn).ToList();


                if (goalData.Count > 0)
                {
                    res.Message = "Goal List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = goalData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getallgoal | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class GetGoalHelper
        {
            public Guid GoalId { get; set; }
            public string GoalOwnerFor { get; set; }
            public string DepartmentName { get; set; }
            public string GoalTypeName { get; set; }
            public string GoalCycleName { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public string GoalReviewerTypeName { get; set; }
            public string GoalTitle { get; set; }
            public string Description { get; set; }
            public string GoalStatus { get; set; }
            public string CreatedBy { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public int GoalPercentage { get; set; } = 0;
        }
        #endregion

        #region This Api To Get Goal Assign By You 
        /// <summary>
        /// Created By Ankit Jain on 27-01-2023
        /// API >> api/newgoal/getgoalassignbyyou
        /// </summary>
        [HttpGet]
        [Route("getgoalassignbyyou")]
        public async Task<IHttpActionResult> GetGoalAssignByYou()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var checkData = _db.Employee.Where(x => x.CompanyId == tokenData.companyId)
                      .Select(x => new
                      {
                          x.EmployeeId,
                          x.DisplayName
                      }).ToList();
                var checkDepartment = _db.Department.Where(x => x.CompanyId == tokenData.companyId)
                       .Select(x => new
                       {
                           x.DepartmentId,
                           x.DepartmentName
                       }).ToList();
                var goalList = await (from g in _db.GoalManagements
                                      join re in _db.ReviewersInGoals on g.GoalId equals re.GoalId
                                      join e in _db.Employee on g.GoalOwnerId equals e.EmployeeId
                                      join d in _db.Department on g.DepartmentId equals d.DepartmentId into p
                                      from Result in p.DefaultIfEmpty()
                                      where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                       && g.CreatedBy == tokenData.employeeId && g.IsActive && !g.IsDeleted
                                      select new
                                      {
                                          g.GoalId,
                                          g.GoalOwnerId,
                                          g.GoalTitle,
                                          g.Description,
                                          g.GoalReviewerType,
                                          g.GoalType,
                                          g.StartDate,
                                          g.GoalCycle,
                                          g.EndDate,
                                          g.Status,
                                          e.EmployeeId,
                                          g.DepartmentId,
                                          g.CreatedOn,
                                          re.ReviewerId,
                                          g.GoalPercentage
                                      }).ToListAsync();
                var goalData = goalList
                        .Select(go => new GetGoal
                        {
                            GoalId = go.GoalId,
                            GoalAssignByName = go.GoalOwnerId != 0 ? checkData.Where(x => x.EmployeeId == go.GoalOwnerId)
                                                 .Select(x => x.DisplayName).FirstOrDefault() : null,
                            DepartmentName = go.DepartmentId != 0 ? checkDepartment.Where(x => x.DepartmentId == go.DepartmentId)
                                                   .Select(x => x.DepartmentName).FirstOrDefault() : null,
                            ReviewerName = go.ReviewerId != 0 ? checkData.Where(x => x.EmployeeId == go.ReviewerId)
                                                 .Select(x => x.DisplayName).FirstOrDefault() : null,
                            GoalCycleName = go.GoalCycle.ToString(),
                            GoalReviewerTypeName = go.GoalReviewerType.ToString(),
                            GoalStatus = go.Status.ToString(),
                            StartDate = go.StartDate,
                            EndDate = go.EndDate,
                            Description = go.Description,
                            GoalTypeName = go.GoalType.ToString(),
                            GoalTitle = go.GoalTitle,
                            CreatedOn = go.CreatedOn,
                            GoalPercentage = go.GoalPercentage,
                        }).OrderByDescending(go => go.CreatedOn).ToList();

                if (goalData.Count > 0)
                {
                    res.Message = "Goal List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = goalData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getgoalassignbyyou | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class GetGoal
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public string GoalAssignByName { get; set; } = string.Empty;
            public string GoalTypeName { get; set; } = string.Empty;
            public string GoalCycleName { get; set; } = string.Empty;
            public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;
            public DateTimeOffset EndDate { get; set; } = DateTimeOffset.Now;
            public string GoalTitle { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string GoalStatus { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public string GoalReviewerTypeName { get; set; } = string.Empty;
            public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
            public string ReviewerName { get; set; } = string.Empty;
            public int GoalPercentage { get; set; } = 0;
        }
        #endregion

        #region This Api To Get Goal Assign To You 
        /// <summary>
        /// Created By Ankit Jain on 27-01-2023
        /// API >> api/newgoal/getgoalassigntoyou
        /// </summary>
        [HttpGet]
        [Route("getgoalassigntoyou")]
        public async Task<IHttpActionResult> GetGoalAssigntoYou()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var checkData = _db.Employee.Where(x => x.CompanyId == tokenData.companyId)
                      .Select(x => new
                      {
                          x.EmployeeId,
                          x.DisplayName
                      }).ToList();
                var checkDepartment = _db.Department.Where(x => x.CompanyId == tokenData.companyId)
                       .Select(x => new
                       {
                           x.DepartmentId,
                           x.DepartmentName
                       }).ToList();
                var goalList = await (from g in _db.GoalManagements
                                      join re in _db.ReviewersInGoals on g.GoalId equals re.GoalId
                                      join e in _db.Employee on g.GoalOwnerId equals e.EmployeeId
                                      join d in _db.Department on g.DepartmentId equals d.DepartmentId into p
                                      from Result in p.DefaultIfEmpty()
                                      where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                      && g.GoalOwnerId == tokenData.employeeId && g.IsActive && !g.IsDeleted
                                      select new
                                      {
                                          g.GoalId,
                                          g.GoalOwnerId,
                                          g.GoalTitle,
                                          g.Description,
                                          g.GoalReviewerType,
                                          g.GoalType,
                                          g.StartDate,
                                          g.GoalCycle,
                                          g.EndDate,
                                          g.Status,
                                          e.EmployeeId,
                                          g.DepartmentId,
                                          g.CreatedBy,
                                          re.ReviewerId,
                                          g.CreatedOn,
                                          g.GoalPercentage
                                      }).ToListAsync();
                var goalData = goalList
                        .Select(go => new GetGoalData
                        {
                            GoalId = go.GoalId,
                            GoalAssignToName = go.GoalOwnerId != 0 ? checkData.Where(x => x.EmployeeId == go.CreatedBy)
                                                 .Select(x => x.DisplayName).FirstOrDefault() : null,
                            DepartmentName = go.DepartmentId != 0 ? checkDepartment.Where(x => x.DepartmentId == go.DepartmentId)
                                                   .Select(x => x.DepartmentName).FirstOrDefault() : null,
                            ReviewerName = go.ReviewerId != 0 ? checkData.Where(x => x.EmployeeId == go.ReviewerId)
                                                 .Select(x => x.DisplayName).FirstOrDefault() : null,
                            GoalCycleName = go.GoalCycle.ToString(),
                            GoalReviewerTypeName = go.GoalReviewerType.ToString(),
                            GoalStatus = go.Status.ToString(),
                            StartDate = go.StartDate,
                            EndDate = go.EndDate,
                            Description = go.Description,
                            GoalTypeName = go.GoalType.ToString(),
                            GoalTitle = go.GoalTitle,
                            CreatedOn = go.CreatedOn,
                            GoalPercentage = go.GoalPercentage,
                        }).OrderByDescending(go => go.CreatedOn).ToList();

                if (goalData.Count > 0)
                {
                    res.Message = "Goal List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = goalData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getgoalassignbyyou | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class GetGoalData
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public string GoalAssignToName { get; set; } = string.Empty;
            public string GoalTypeName { get; set; } = string.Empty;
            public string GoalCycleName { get; set; } = string.Empty;
            public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;
            public DateTimeOffset EndDate { get; set; } = DateTimeOffset.Now;
            public string GoalTitle { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string GoalStatus { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public string GoalReviewerTypeName { get; set; } = string.Empty;
            public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
            public string ReviewerName { get; set; } = string.Empty;
            public int GoalPercentage { get; set; } = 0;
        }
        #endregion

        #region API FOR UPDATE Goal Status
        /// <summary>
        /// Created By ankit Jain On 30-01-2023
        /// API >> Post >> api/newgoal/updategoalstatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updategoalstatus")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGoal(UpdateGoalStatusResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var getData = await _db.GoalManagements.FirstOrDefaultAsync
                                (x => x.GoalId == model.GoalId && x.IsActive &&
                                !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (getData != null)
                    {
                        getData.Status = model.Status;
                        getData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                        getData.UpdatedBy = tokenData.employeeId;

                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        _ = SendMailUpadeGoalStatus(getData.GoalId);

                        res.Message = "Updated Successfully  !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
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
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newgoal/updategoalstatus | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class UpdateGoalStatusResponse
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public GoalStatusConstantsClass Status { get; set; } = GoalStatusConstantsClass.Pending;
        }
        #endregion

        #region This Api Use To Send Mail To Change Goal Status
        ///// <summary>
        ///// Create By Ankit Jain Date-31-01-2023
        ///// </summary>
        ///// <returns></returns>
        public async Task SendMailUpadeGoalStatus(Guid goalId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalData = (from g in _db.GoalManagements
                                join gd in _db.ReviewersInGoals on g.GoalId equals gd.GoalId
                                where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                 && g.GoalId == goalId
                                select new
                                {
                                    g.CreatedBy,
                                    g.GoalOwnerId,
                                    gd.ReviewerId,
                                    g.Description,
                                    g.StartDate,
                                    g.EndDate,
                                    g.Status,
                                    g.GoalResponse
                                }).FirstOrDefault();
                var createdby = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.CreatedBy);
                var goalOwner = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.GoalOwnerId);
                var reviewerby = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.ReviewerId);
                var st = (DateTimeOffset)goalData.StartDate;
                var startDate = st.ToString("d MMMM, yyyy");
                var et = (DateTimeOffset)goalData.EndDate;
                var endDate = et.ToString("d MMMM, yyyy");
                var status = goalData.Status.ToString();
                var resStatus = goalData.GoalResponse.ToString();
                var companylist = _db.Company.Where(y => y.CompanyId == tokenData.companyId
                        && y.IsActive && !y.IsDeleted)
                      .Select(x => new
                      {
                          x.RegisterAddress,
                          x.RegisterCompanyName

                      }).FirstOrDefault();
                if (goalData.Status == GoalStatusConstantsClass.InProgress)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (tokenData.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == tokenData.companyId)
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
                    string htmlBody = NewGoalHelper.StatusInGoal
                       .Replace("<|DESCRIPTION|>", goalData.Description)
                       .Replace("<|GOALASSIGNEENAME|>", goalOwner.DisplayName)
                       .Replace("<|STARTDATE|>", startDate)
                       .Replace("<|ENDDATE|>", endDate)
                       .Replace("<|IMAGE_PATH|>", "emossy.png")
                       .Replace("<|STATUS|>", status)
                       .Replace("<|COMPANYNAME|>", companylist.RegisterCompanyName)
                       .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);

                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                        Subject = "Reminder",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { reviewerby.OfficeEmail, goalOwner.OfficeEmail, createdby.OfficeEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (goalData.Status == GoalStatusConstantsClass.InReview)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (tokenData.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == tokenData.companyId)
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
                    string htmlBody = NewGoalHelper.DocStatusInGoal
                       .Replace("<|DESCRIPTION|>", goalData.Description)
                       .Replace("<|STARTDATE|>", startDate)
                       .Replace("<|ENDDATE|>", endDate)
                       .Replace("<|IMAGE_PATH|>", "emossy.png")
                       .Replace("<|STATUS|>", status)
                       .Replace("<|COMPANYNAME|>", companylist.RegisterCompanyName)
                       .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);

                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                        Subject = "Reminder",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { reviewerby.OfficeEmail, goalOwner.OfficeEmail, createdby.OfficeEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (goalData.Status == GoalStatusConstantsClass.Completed)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (tokenData.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == tokenData.companyId)
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
                    string htmlBody = NewGoalHelper.ApprovedStatusInGoal
                       .Replace("<|DESCRIPTION|>", goalData.Description)
                       .Replace("<|STARTDATE|>", startDate)
                       .Replace("<|ENDDATE|>", endDate)
                       .Replace("<|IMAGE_PATH|>", "emossy.png")
                       .Replace("<|STATUS|>", resStatus)
                       .Replace("<|COMPANYNAME|>", companylist.RegisterCompanyName)
                       .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);

                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                        Subject = "Reminder",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { reviewerby.OfficeEmail, goalOwner.OfficeEmail, createdby.OfficeEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (goalData.Status == GoalStatusConstantsClass.Reject)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (tokenData.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == tokenData.companyId)
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
                    string htmlBody = NewGoalHelper.RejectedStatusInGoal
                       .Replace("<|DESCRIPTION|>", goalData.Description)
                       .Replace("<|STARTDATE|>", startDate)
                       .Replace("<|ENDDATE|>", endDate)
                       .Replace("<|IMAGE_PATH|>", "emossy.png")
                       .Replace("<|STATUS|>", resStatus)
                       .Replace("<|COMPANYNAME|>", companylist.RegisterCompanyName)
                       .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);

                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                        Subject = "Reminder",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { reviewerby.OfficeEmail, goalOwner.OfficeEmail, createdby.OfficeEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
            }

            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }

        }
        #endregion

        #region This api To Upload Goal Document 

        /// <summary>
        ///Created By Ankit On 30-01-2023
        /// </summary>Api>>Post>> api/newgoal/uploadgoaldocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadgoaldocuments")]
        public async Task<UploadGoalDoc> UploadGoalDocments()
        {
            UploadGoalDoc result = new UploadGoalDoc();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/goaldocuments/" + tokenData.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { tokenData.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + tokenData.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\goaldocuments\\" + tokenData.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successfuly";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = path;
                        result.Extension = extension;
                        result.ExtensionType = extemtionType;
                    }
                    else
                    {
                        result.Message = "You Pass 0 Content";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }


        /// <summary>
        /// Created By Ankit
        /// </summary>

        public class UploadGoalDoc
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }
        #endregion This api use for upload documents for Goal document

        #region API FOR Add Document Url In Goal 
        /// <summary>
        /// Created By ankit Jain On 30-01-2023
        /// API >> Post >> api/newgoal/adddocurl
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("adddocurl")]
        [HttpPost]
        public async Task<IHttpActionResult> AddDocUrl(AddDoc model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var getData = await _db.GoalManagements.FirstOrDefaultAsync
                                (x => x.GoalId == model.GoalId && x.IsActive &&
                                !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (getData != null)
                    {
                        GoalsDocument obj = new GoalsDocument
                        {
                            GoalId = model.GoalId,
                            DocumentTitle = model.DocumentTitle,
                            FileURL = model.FileURL,
                            ExtensionType = model.ExtensionType,
                            Description = model.Description,
                            GoalPercentage = model.GoalPercentage,
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            DocDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        };

                        _db.GoalsDocuments.Add(obj);
                        await _db.SaveChangesAsync();
                        getData.GoalPercentage = model.GoalPercentage;
                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Message = "Document Added!";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
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
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newgoal/adddocurl | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class AddDoc
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public string DocumentTitle { get; set; } = string.Empty;
            public string FileURL { get; set; } = string.Empty;
            public string ExtensionType { get; set; } = string.Empty;
            public int GoalPercentage { get; set; } = 0;
            public string Description { get; set; } = String.Empty;
        }
        #endregion

        #region This Api To Get Goal Document
        /// <summary>
        /// Created By Ankit Jain on 30-01-2023
        /// API >> api/newgoal/getgoaldocument
        /// </summary>
        [HttpGet]
        [Route("getgoaldocument")]
        public async Task<IHttpActionResult> GetGoalDocument(Guid goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalData = await (from g in _db.GoalManagements
                                      join gd in _db.GoalsDocuments on g.GoalId equals gd.GoalId
                                      where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                      && g.IsActive && !g.IsDeleted && g.GoalId == goalId
                                      select new GetDoc
                                      {
                                          GoalId = gd.GoalId,
                                          DocumentTitle = gd.DocumentTitle,
                                          FileURL = gd.FileURL,
                                          ExtensionType = gd.ExtensionType,
                                          GoalPercentage = gd.GoalPercentage,
                                          Description = gd.Description,
                                          DocDate = gd.DocDate,
                                      }).ToListAsync();
                if (goalData.Count > 0)
                {
                    res.Message = "Goal List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = goalData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getgoaldocument | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class GetDoc
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public string DocumentTitle { get; set; } = string.Empty;
            public string FileURL { get; set; } = string.Empty;
            public string ExtensionType { get; set; } = string.Empty;
            public int GoalPercentage { get; set; } = 0;
            public string Description { get; set; } = String.Empty;
            public DateTimeOffset DocDate { get; set; } = DateTimeOffset.UtcNow;
        }
        #endregion

        #region API FOR UPDATE Goal Document Status
        /// <summary>
        /// Created By ankit Jain On 30-01-2023
        /// API >> Post >> api/newgoal/updatedocumentstatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updatedocumentstatus")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateDocumentStatus(UpdateDocStatus model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var getData = await _db.GoalManagements.FirstOrDefaultAsync
                                (x => x.GoalId == model.GoalId && x.IsActive &&
                                !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (getData != null)
                    {
                        if (model.GoalPercentage == 100)
                        {
                            getData.Status = GoalStatusConstantsClass.InReview;
                            getData.GoalPercentage = model.GoalPercentage;
                            getData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getData.UpdatedBy = tokenData.employeeId;

                            _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            _ = SendMailUpadeGoalStatus(getData.GoalId);
                            res.Message = "Updated Successfully  !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                            res.Data = getData;
                            return Ok(res);
                        }
                        else
                        {
                            getData.Status = getData.Status;
                            getData.GoalPercentage = model.GoalPercentage;
                            getData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getData.UpdatedBy = tokenData.employeeId;

                            _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            _ = SendMailUpadeGoalStatus(getData.GoalId);
                            res.Message = "Updated Successfully  !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
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
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newgoal/updatedocumentstatus | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class UpdateDocStatus
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public int GoalPercentage { get; set; } = 0;
        }
        #endregion

        #region Api To Add Comment In Goal

        /// <summary>
        /// API >> Post >> api/newgoal/addcommentongoal
        /// Created By Ankit Jain on 09/02/2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcommentongoal")]
        public async Task<IHttpActionResult> AddCommentOnGoal(AddComments model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var goalData = await _db.GoalManagements.FirstOrDefaultAsync(x => x.GoalId == model.GoalId
                    && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (goalData != null)
                    {
                        GoalComment obj = new GoalComment
                        {
                            GoalId = goalData.GoalId,
                            Message = model.Message,
                            CommentBy = tokenData.displayName,
                            CommentOn = DateTime.Now.ToString("MMM dd yyyy,h:mm:ss"),
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        };

                        _db.GoalComments.Add(obj);
                        await _db.SaveChangesAsync();
                        _ = SendMailComments(obj.GoalId);
                        res.Message = "Comments Added!";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = obj;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Data Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = goalData;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newgoal/addcommentongoal | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class AddComments
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public string Message { get; set; } = string.Empty;
        }
        #endregion Api To Add Comment In Goal

        #region This Mail Add Comments
        /// <summary>
        ///   // Create By Ankit Jain Date - 09/02/2023
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>

        public async Task SendMailComments(Guid goalId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalData = (from g in _db.GoalManagements
                                join gc in _db.GoalComments on g.GoalId equals gc.GoalId
                                join gd in _db.ReviewersInGoals on g.GoalId equals gd.GoalId
                                where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                 && g.GoalId == goalId
                                select new
                                {
                                    g.CreatedBy,
                                    g.GoalOwnerId,
                                    gd.ReviewerId,
                                    gc.CreatedOn,
                                    g.Status,
                                    gc.Message,
                                    g.GoalResponse
                                }).FirstOrDefault();
                var createdby = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.CreatedBy);
                var goalOwner = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.GoalOwnerId);
                var reviewerby = _db.Employee.FirstOrDefault(x => x.EmployeeId == goalData.ReviewerId);
                var st = (DateTimeOffset)goalData.CreatedOn;
                var Date = st.ToString("d MMMM, yyyy");
                var status = goalData.Status.ToString();
                var resStatus = goalData.GoalResponse.ToString();
                var companylist = _db.Company.Where(y => y.CompanyId == tokenData.companyId
                        && y.IsActive && !y.IsDeleted)
                      .Select(x => new
                      {
                          x.RegisterAddress,
                          x.RegisterCompanyName

                      }).FirstOrDefault();
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (tokenData.IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == tokenData.companyId)
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
                string htmlBody = NewGoalHelper.CommentInGoal
                   .Replace("<|MESSAGE|>", goalData.Message)
                   .Replace("<|GOALASSIGNEENAME|>", goalOwner.DisplayName)
                   .Replace("<|DATE|>", Date)
                   .Replace("<|IMAGE_PATH|>", "emossy.png")
                   .Replace("<|STATUS|>", status)
                   .Replace("<|COMPANYNAME|>", companylist.RegisterCompanyName)
                   .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);

                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                    Subject = "Reminder",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { reviewerby.OfficeEmail, goalOwner.OfficeEmail, createdby.OfficeEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }

            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region This Api To Get Goal By ID
        /// <summary>
        /// Created By Ankit Jain on 02-01-2023
        /// API >> Get >> api/newgoal/getgoalbyid
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getgoalbyid")]
        public async Task<IHttpActionResult> GetGoalById(Guid goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetGoalHelperClass response = new GetGoalHelperClass();
            try
            {
                var checkData = _db.Employee.Where(x => x.CompanyId == tokenData.companyId)
                       .Select(x => new
                       {
                           x.EmployeeId,
                           x.DisplayName
                       }).ToList();
                var checkDepartment = _db.Department.Where(x => x.CompanyId == tokenData.companyId)
                       .Select(x => new
                       {
                           x.DepartmentId,
                           x.DepartmentName
                       }).ToList();
                var goalList = await (from g in _db.GoalManagements
                                      join e in _db.Employee on g.GoalOwnerId equals e.EmployeeId
                                      join d in _db.Department on g.DepartmentId equals d.DepartmentId into p
                                      from Result in p.DefaultIfEmpty()
                                      where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                      && g.IsActive && !g.IsDeleted && g.GoalId == goalId
                                      select new
                                      {
                                          g.GoalId,
                                          g.GoalOwnerId,
                                          g.GoalTitle,
                                          g.Description,
                                          g.GoalReviewerType,
                                          g.GoalType,
                                          g.StartDate,
                                          g.GoalCycle,
                                          g.EndDate,
                                          g.Status,
                                          e.EmployeeId,
                                          g.DepartmentId,
                                          g.CreatedOn,
                                          g.GoalPercentage
                                      }).ToListAsync();
                var goalData = goalList
                        .Select(go => new GetGoalHelperClass
                        {
                            GoalId = go.GoalId,
                            GoalOwnerFor = go.GoalOwnerId != 0 ? checkData.Where(x => x.EmployeeId == go.GoalOwnerId)
                                                 .Select(x => x.DisplayName).FirstOrDefault() : null,
                            DepartmentName = go.DepartmentId != 0 ? checkDepartment.Where(x => x.DepartmentId == go.DepartmentId)
                                                   .Select(x => x.DepartmentName).FirstOrDefault() : null,
                            GoalCycleName = go.GoalCycle.ToString(),
                            GoalReviewerTypeName = go.GoalReviewerType.ToString(),
                            GoalStatus = go.Status.ToString(),
                            StartDate = go.StartDate,
                            EndDate = go.EndDate,
                            Description = go.Description,
                            GoalTypeName = go.GoalType.ToString(),
                            GoalTitle = go.GoalTitle,
                            CreatedOn = go.CreatedOn,
                            GoalPercentages = go.GoalPercentage,
                        }).OrderByDescending(go => go.CreatedOn).ToList();

                var commentList = _db.GoalComments.Where(x => x.IsActive && !x.IsDeleted &&
                               x.GoalId == goalId).Select(x => new
                               {
                                   CreateOn = x.CreatedOn,
                                   x.CommentBy,
                                   x.CommentOn,
                                   x.Message,
                                   CommentType = x.CreatedBy != tokenData.employeeId ? "RECIVER" : "SENDER",
                               }).ToList().OrderBy(x => x.CreateOn).ToList();
                response.CommentList = commentList;
                if (goalData.Count > 0)
                {
                    res.Message = "Goal List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = response;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getgoalbyid | " +
                     "DepartmentId : " + goalId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class GetGoalHelperClass
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public string GoalOwnerFor { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public string GoalTypeName { get; set; } = string.Empty;
            public string GoalCycleName { get; set; } = string.Empty;
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public string GoalReviewerTypeName { get; set; } = string.Empty;
            public string GoalTitle { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string GoalStatus { get; set; } = string.Empty;
            public DateTimeOffset CreatedOn { get; set; }
            public int GoalPercentages { get; set; } = 0;
            public object CommentList { get; set; }
        }
        #endregion

        #region API FOR UPDATE Goal Response Status
        /// <summary>
        /// Created By ankit Jain On 30-01-2023
        /// API >> Post >> api/newgoal/updategoalresponse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updategoalresponse")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGoalResponse(UpdateResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var getData = await _db.GoalManagements.FirstOrDefaultAsync
                                (x => x.GoalId == model.GoalId && x.IsActive &&
                                !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (getData != null)
                    {
                        if (model.GoalResponse == GoalResponseConstantsClass.Approved)
                        {
                            getData.GoalResponse = model.GoalResponse;
                            getData.Status = GoalStatusConstantsClass.Completed;
                            getData.Reason = model.Reason;
                            getData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getData.UpdatedBy = tokenData.employeeId;

                            _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            _ = SendMailUpadeGoalStatus(getData.GoalId);
                            res.Message = "Updated Successfully  !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                            res.Data = getData;
                            return Ok(res);
                        }
                        else
                        {
                            getData.GoalResponse = model.GoalResponse;
                            getData.Status = GoalStatusConstantsClass.Reject;
                            getData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getData.UpdatedBy = tokenData.employeeId;

                            _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            _ = SendMailUpadeGoalStatus(getData.GoalId);
                            res.Message = "Updated Successfully  !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
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
            }
            catch (Exception ex)
            {

                logger.Error("API : api/newgoal/updategoalresponse | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class UpdateResponse
        {
            public Guid GoalId { get; set; } = Guid.NewGuid();
            public GoalResponseConstantsClass GoalResponse { get; set; } = GoalResponseConstantsClass.Approved;
            public string Reason { get; set; } = string.Empty;
        }
        #endregion

        #region This Api To Get Goal Status Assign To You By Id 
        /// <summary>
        /// Created By Ankit Jain on 06-02-2023
        /// API >> Get >> api/newgoal/getgoalstatusbyid
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getgoalstatusbyid")]
        public async Task<IHttpActionResult> GetGoalStatusById(Guid goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalList = await _db.GoalManagements.Where(x => x.CompanyId == tokenData.companyId
                            && x.GoalId == goalId && x.IsActive && !x.IsDeleted && x.GoalOwnerId == tokenData.employeeId)
                       .Select(x => new GetGoalStatusData
                       {
                           GoalId = x.GoalId,
                           Status = x.Status.ToString(),
                       }).ToListAsync();

                if (goalList.Count > 0)
                {
                    res.Message = "Goal List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = goalList;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newgoal/getgoalstatusbyid | " +
                    "GoalId : " + goalId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class GetGoalStatusData
        {
            public Guid GoalId { get; set; }
            public string Status { get; set; }

        }
        #endregion

        #region This Api Use To Get Percentage Range 
        /// <summary>
        /// Created By Ankit Jain on 06-02-2023
        /// API >> GET >> api/newgoal/getgoalrange
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getgoalrange")]
        public async Task<IHttpActionResult> GetGoalRange(Guid goalId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goal = await _db.GoalManagements
                    .FirstOrDefaultAsync(x => x.GoalId == goalId);
                if (goal == null)
                {
                    res.Message = "Goal Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = NewGoalHelper.GetRangeList(0, 5);
                    return Ok(res);
                }

                res.Message = "Goal Range List Found !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = NewGoalHelper.GetRangeList(goal.GoalPercentage, 5);
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API :  api/newgoal/getgoalrange | " +
                       "GoalId : " + goalId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }
        #endregion

        #region This Api use To Merging old Data 
        /// <summary>
        /// Created By Ankit Jain on 06-02-2023
        /// API >> Post >> api/newgoal/addolddata
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addolddata")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> addolddata()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var companyList = await _db.Company.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                foreach (var item in companyList)
                {
                    var goalData = await _db.Goals
                          .Where(x => x.IsActive && !x.IsDeleted
                          && x.CompanyId == item.CompanyId).ToListAsync();
                    var addData = goalData
                    .Select(x => new GoalManagement
                    {
                        GoalOwnerId = x.AssignById,
                        DepartmentId = 0,
                        GoalType = GoalTypeConstants.Individual_Goal,
                        GoalCycle = GoalCycleConstants.Custom,
                        StartDate = x.StartDate,
                        EndDate = x.ExpEndDate,
                        GoalReviewerType = GoalReviewerType.Self,
                        GoalTitle = x.GoalName,
                        Description = x.Description,
                        Status = GoalStatusConstantsClass.Pending,
                        GoalResponse = GoalResponseConstantsClass.Approved,
                        Reason = x.Reason,
                        GoalUrl = x.GoalDocuments,
                        GoalPercentage = x.GoalPercentage,
                    }).ToList();
                    _db.GoalManagements.AddRange(addData);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("API :  api/newgoal/addolddata | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion
    }
}
