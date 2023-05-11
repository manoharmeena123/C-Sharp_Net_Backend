using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.NewClientRequirement.Clienttask;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.ClientNewRequirement
{
    [RoutePrefix("api/clientapproval")]
    [Authorize]
    public class NewClientApprovalController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        #region Send for Approval Task
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/sendforapproval
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("sendforapproval")]
        [HttpPost]
        public async Task<IHttpActionResult> SendForApproval(List<ClientTaskApproval> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taskdata = model.Select(x => new ClientTaskApproval
                {
                    ClientTaskId = x.ClientTaskId,
                    ClientId = x.ClientId,
                    ClientCode = x.ClientCode,
                    WorkTypeId = x.WorkTypeId,
                    TaskName = x.TaskName,
                    ProjectManagerId = x.ProjectManagerId,

                }).ToList();

                foreach (var item in taskdata)
                {
                    var check = _db.ClientTaskApprovals.Where(a => a.ClientTaskId == item.ClientTaskId).FirstOrDefault();
                    if (check == null)
                    {
                        ClientTaskApproval obj = new ClientTaskApproval()
                        {
                            ClientTaskId = item.ClientTaskId,
                            ClientId = item.ClientId,
                            ClientCode = item.ClientCode,
                            CreatedBy = tokendata.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                            CompanyId = tokendata.companyId,
                            TaskRequest = ClientTaskRequestConstants.Pending,
                            OrgId = tokendata.orgId,
                            IsSFA = false,
                            WorkTypeId = item.WorkTypeId,
                        };
                        _db.ClientTaskApprovals.Add(obj);
                        _db.SaveChanges();
                        check = obj;
                    }
                    else
                    {
                        check.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                        check.UpdatedBy = tokendata.employeeId;
                    }

                    check.ReEvaluteDiscription = item.ReEvaluteDiscription;
                    check.TaskName = item.TaskName;
                    check.IsSFA = true;
                    check.IsApproved = false;
                    check.IsRe_Evaluate = false;
                    check.TaskName = item.TaskName;
                    //   check.TotalWorkingTime = item.TotalWorkingTime;
                    // check.ProjectManagerId = item.ProjectManagerId;
                    check.TaskRequest = ClientTaskRequestConstants.Pending;
                    check.CompanyId = tokendata.companyId;
                    check.OrgId = tokendata.orgId;
                    check.IsRe_Evaluate = false;
                    check.IsSFA = true;

                    _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                res.Message = "Send Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = taskdata;
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/sendforapproval", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api for type of Task 
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API>POST>>api/clientapproval/gettaskstatus
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gettaskstatus")]
        public async Task<IHttpActionResult> GetDataByTaskStatus(PaginationRequest Model)//(int ProjectId, DateTime? dateValue, int page = 1, int count = 10)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Manager = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.ReportingManager == tokendata.employeeId).FirstOrDefault();

                var task = await (from ap in _db.ClientTaskApprovals
                                  join t in _db.clientTaskModels on ap.ClientTaskId equals t.ClientTaskId
                                  join c in _db.NewClientModels on t.ClientId equals c.ClientId
                                  join e in _db.Employee on t.CreatedBy equals e.EmployeeId
                                  where t.IsActive && !t.IsDeleted && t.CompanyId == tokendata.companyId// && e.ReportingManager == tokenData.employeeId
                                  select new Taskstatusresponse
                                  {
                                      ReportingManager = e.ReportingManager,
                                      ClientTaskId = ap.ClientTaskId,
                                      ClientId = ap.ClientId,
                                      ClientCode = ap.ClientCode,
                                      Taskdate = null,
                                      Taskdatedata = t.TaskDate,
                                      WorkTypeId = ap.WorkTypeId,
                                      Description = t.Description,
                                      worktypename = _db.TypeofWorks.Where(x => x.WorktypeId == t.WorkTypeId).Select(x => x.WorktypeName).FirstOrDefault(),
                                      ClientName = c.ClientName,
                                      CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == t.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                      UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == t.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                      CreatedById = t.CreatedBy,
                                      UpdatedById = t.UpdatedBy,
                                      CreatedOn = t.CreatedOn,
                                      UpdatedOn = t.UpdatedOn,
                                      WorkingHours = TimeSpan.Zero,
                                      Starttime = null,
                                      Endtime = null,
                                      TaskStarttime = t.StartTime,
                                      TaskEndtime = t.EndTime,
                                      TaskRequest = ap.TaskRequest.ToString(),
                                      reEvalutedDiscription = ap.ReEvaluteDiscription,


                                  }).OrderByDescending(x => x.CreatedOn).ToListAsync();

                if (tokendata.IsAdminInCompany)
                {
                    if (task.Count > 0)
                    {
                        task.ForEach(x =>
                        {
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                            x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                        });
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                        {
                            var text = textInfo.ToUpper(Model.Pendingsearch);
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                                       .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                            };
                        }
                        else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                        {
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),

                            };
                        }
                        else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                        {
                            var text = textInfo.ToUpper(Model.Approvesearch);
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString()).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Approved.ToString())
                                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                            };
                        }
                        else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                        {
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString()).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString())
                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),

                            };
                        }
                        else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                        {
                            var text = textInfo.ToUpper(Model.Revaluationsearch);
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString()).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString())
                                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                            };
                        }
                        else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                        {
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString()).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString())
                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),

                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = new
                        {
                            Pendingtaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                            Approvetaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString()).Count(),
                            Revaltaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString()).Count(),
                        };
                    }
                }

                else if (Manager != null)
                {

                    if (task.Count > 0)
                    {
                        task.ForEach(x =>
                        {
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                            x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                        });
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                        {
                            var text = textInfo.ToUpper(Model.Pendingsearch);
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId)
                                                       .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                            };
                        }
                        else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                        {
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId)
                                .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),

                            };
                        }
                        else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                        {
                            var text = textInfo.ToUpper(Model.Approvesearch);
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId)
                                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                            };
                        }
                        else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                        {
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId)
                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),

                            };
                        }
                        else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                        {
                            var text = textInfo.ToUpper(Model.Revaluationsearch);
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId)
                                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                            };
                        }
                        else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                        {
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId)
                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),

                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = new
                        {
                            Pendingtaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                            Approvetaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                            Revaltaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                        };
                    }
                }

                else
                {

                    if (task.Count > 0)
                    {
                        task.ForEach(x =>
                        {
                            x.Taskdate = x.Taskdatedata.ToString("MMM dd yyyy");
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                            x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                        });
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                        {
                            var text = textInfo.ToUpper(Model.Pendingsearch);
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId)
                                                       .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                            };
                        }
                        else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                        {
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId)
                                .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),

                            };
                        }
                        else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                        {
                            var text = textInfo.ToUpper(Model.Approvesearch);
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId)
                                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                            };
                        }
                        else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                        {
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId)
                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),

                            };
                        }
                        else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                        {
                            var text = textInfo.ToUpper(Model.Revaluationsearch);
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId)
                                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                            };
                        }
                        else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                        {
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId)
                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),

                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = new
                        {
                            Pendingtaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                            Approvetaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                            Revaltaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                        };
                    }

                }

                //DateTimeOffset checkDate = new DateTimeOffset();
                //if (dateValue.HasValue)
                //    checkDate = TimeZoneConvert.ConvertTimeToSelectedZone(dateValue.Value, tokenData.TimeZone);
                //var getAll = await _db.TaskApprovels
                //    .Where(t => t.IsActive && !t.IsDeleted && t.CompanyId == tokenData.companyId && t.ProjectId == ProjectId && t.UpdatedOn.HasValue)
                //    .Where(x => dateValue.HasValue ? (x.IsApproved && x.UpdatedOn.Value >= checkDate.Date) : x.IsActive)
                //    .Select(t => new GetApprovelRequestData
                //    {
                //        TaskApprovelId = t.TaskApprovelId,
                //        CreatedBy = t.CreatedBy,
                //        TaskTiltle = t.TaskName,
                //        SendarName = _db.Employee.Where(e => e.EmployeeId == t.CreatedBy && e.IsActive && !e.IsDeleted).Select(e => e.DisplayName).FirstOrDefault(),
                //        TotalWorkingTimeInProject = t.TotalWorkingTime,
                //        IsApproved = t.IsApproved,
                //        UpdatedDate = t.UpdatedOn.Value,
                //        EstimateTimeLong = t.EstimateTime,
                //        EstimateTime = "",
                //        TaskStartDate = _db.TaskModels.Where(x => x.TaskId == t.TaskId).Select(x => x.StartDate.Value).FirstOrDefault(),
                //    })
                //    .OrderByDescending(x => x.UpdatedDate)
                //    .Skip((page - 1) * count).Take(count)
                //    .ToListAsync();

                //if (getAll.Count == 0)
                //{
                //    res.Message = "Task list Not Found";
                //    res.Status = false;
                //    res.StatusCode = HttpStatusCode.NoContent;
                //    res.Data = new
                //    {
                //        Page = page,
                //        Count = count,
                //        TaskApprovalList = getAll,
                //    };
                //    return Ok(res);
                //}

                //getAll.ForEach(t =>
                //{
                //    t.EstimateTime = string.Format("{00:00}:{01:00}", (int)t.EstimateTimeLong / 60, t.EstimateTimeLong % 60);
                //});

                //res.Message = "Task list Found";
                //res.Status = true;
                //res.StatusCode = HttpStatusCode.Found;
                //res.Data = new
                //{
                //    Page = page,
                //    Count = count,
                //    TaskApprovalList = getAll,
                //};
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/taskapprovel/gettaskstatus", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class PaginationRequest
        {
            public int? Pendingpage { get; set; } = 0;
            public int? Pendingcount { get; set; } = 0;
            public string Pendingsearch { get; set; } = string.Empty;

            public int? Approvepage { get; set; } = 0;
            public int? Approvecount { get; set; } = 0;
            public string Approvesearch { get; set; } = string.Empty;

            public int? Revaluationpage { get; set; } = 0;
            public int? Revaluationcount { get; set; } = 0;
            public string Revaluationsearch { get; set; } = string.Empty;
        }

        public class TaskFilterResponseModel
        {
            public int ProjectId { get; set; }
            public TaskTypeConstants TaskTypeId { get; set; }
            public TaskStatusConstants StatusId { get; set; }
            public int CreatedBy { get; set; } = 0;
            public int? UpdatedBy { get; set; } = null;

        }


        #endregion

        #region  Approval Task
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/approvetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("approvetask")]
        [HttpPost]
        public async Task<IHttpActionResult> ApproveTask(ClienttaskIdRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                // List<ClientTaskApproval> list = new List<ClientTaskApproval>();
                if (model == null)
                {
                    res.Message = "Model is Empty !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    foreach (var item in model.ClienttaskId)
                    {
                        var getdata = _db.ClientTaskApprovals.FirstOrDefault(s => s.ClientTaskId == item && s.IsActive && !s.IsDeleted && s.CompanyId == tokendata.companyId && !s.IsApproved);
                        if (getdata == null)
                        {
                            res.Message = "No task Available for Approval !";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                            // res.Data = list;
                        }
                        else
                        {

                            getdata.TaskRequest = ClientTaskRequestConstants.Approved;
                            getdata.IsApproved = true;
                            getdata.IsRe_Evaluate = false;
                            getdata.ProjectManagerId = tokendata.employeeId;
                            getdata.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getdata.UpdatedBy = tokendata.employeeId;
                            //  list.Add(getdata);
                        }
                        _db.Entry(getdata).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Task Approved !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        // res.Data = list;
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/approvetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Get Send For Approval Task  //  not in use
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/getsendforapproval
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("getsendforapproval")]
        [HttpPut]
        public async Task<IHttpActionResult> GetSendForApproveTask()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<ClientTaskApproval> list = new List<ClientTaskApproval>();

                var getdata = await (from t in _db.clientTaskModels
                                     join a in _db.ClientTaskApprovals on t.ClientTaskId equals a.ClientTaskId
                                     where t.IsActive && !t.IsDeleted && t.CompanyId == tokendata.companyId && a.IsSFA
                                     select new
                                     {
                                         t.ClientTaskId,
                                         t.ClientId,
                                         t.ClientCode,
                                         t.TaskDate,
                                         t.StartTime,
                                         t.EndTime,
                                         t.WorkTypeId,
                                         t.Description,
                                     })
                              .ToListAsync();
                if (getdata.Count > 0)
                {
                    res.Message = "Task Available for Approval !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getdata;
                }
                else
                {
                    res.Message = "No Task Available for Approval !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getdata;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/sendforapproval", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Get Approval Task   // not im use
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/getapprovetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("getapprovetask")]
        [HttpPut]
        public async Task<IHttpActionResult> GetApproveTask()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<ClientTaskApproval> list = new List<ClientTaskApproval>();

                var getdata = await (from t in _db.clientTaskModels
                                     join a in _db.ClientTaskApprovals on t.ClientTaskId equals a.ClientTaskId
                                     where t.IsActive && !t.IsDeleted && t.CompanyId == tokendata.companyId && a.IsSFA && a.IsApproved
                                     select new
                                     {
                                         t.ClientTaskId,
                                         t.ClientId,
                                         t.ClientCode,
                                         t.TaskDate,
                                         t.StartTime,
                                         t.EndTime,
                                         t.WorkTypeId,
                                         t.Description,
                                     }).ToListAsync();
                if (getdata.Count > 0)
                {
                    res.Message = "Task Available for Approval !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getdata;
                }
                else
                {
                    res.Message = "No Task Available for Approval !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getdata;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/getapprovetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Get Revaluation Task // not in use
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/getrevaluationtask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("getrevaluationtask")]
        [HttpPost]
        public async Task<IHttpActionResult> GetRevaluationTask(PaginationtaskIdRequest Model)//(int? page = null, int? count = null, string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<ClientTaskApproval> list = new List<ClientTaskApproval>();

                var task = await (from ap in _db.ClientTaskApprovals
                                  join t in _db.clientTaskModels on ap.ClientTaskId equals t.ClientTaskId
                                  join c in _db.NewClientModels on t.ClientId equals c.ClientId
                                  join e in _db.Employee on t.CreatedBy equals e.EmployeeId
                                  where t.IsActive && !t.IsDeleted && t.CompanyId == tokendata.companyId && ap.TaskRequest == ClientTaskRequestConstants.Revaluation// && e.ReportingManager == tokenData.employeeId
                                  select new Taskstatusresponse
                                  {


                                      ClientTaskId = ap.ClientTaskId,
                                      ClientId = ap.ClientId,
                                      ClientCode = ap.ClientCode,
                                      Taskdate = null,
                                      Taskdatedata = t.TaskDate,
                                      WorkTypeId = ap.WorkTypeId,
                                      Description = t.Description,
                                      worktypename = _db.TypeofWorks.Where(x => x.WorktypeId == t.WorkTypeId).Select(x => x.WorktypeName).FirstOrDefault(),
                                      ClientName = c.ClientName,
                                      CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == t.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                      UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == t.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                      CreatedById = t.CreatedBy,
                                      UpdatedById = t.UpdatedBy,
                                      CreatedOn = t.CreatedOn,
                                      UpdatedOn = t.UpdatedOn,
                                      WorkingHours = TimeSpan.Zero,
                                      Starttime = null,
                                      Endtime = null,
                                      TaskStarttime = t.StartTime,
                                      TaskEndtime = t.EndTime,
                                      TaskRequest = ap.TaskRequest.ToString(),

                                  }).ToListAsync();
                if (task.Count > 0)
                {
                    task.ForEach(x =>
                    {
                        x.Taskdate = x.Taskdatedata.ToString("MMM dd yyyy");
                        x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                        x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                        x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                    });
                    res.Message = "Task list Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                    {
                        var text = textInfo.ToUpper(Model.search);
                        res.Data = new
                        {
                            Data = task.Count(),
                            Count = (int)Model.count,
                            List = task.Where(x => x.ClientName.ToUpper().Contains(text))
                                                   .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                        };
                    }
                    else if (Model.page.HasValue && Model.count.HasValue)
                    {
                        res.Data = new
                        {
                            Data = task.Count(),
                            Count = (int)Model.count,
                            List = task//.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                            .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                            //List = task.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),

                        };
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/getrevaluationtask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region  Re-evalulation Task
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/revaluatetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("revaluatetask")]
        [HttpPost]
        public async Task<IHttpActionResult> RevaluateTask(ClienttaskIdRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //    List<ClientTaskApproval> list = new List<ClientTaskApproval>();
                if (model == null)
                {
                    res.Message = "Model is Empty !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    foreach (var item in model.ClienttaskId)
                    {
                        var getdata = _db.ClientTaskApprovals.FirstOrDefault(s => s.ClientTaskId == item && s.IsActive && !s.IsDeleted && s.CompanyId == tokendata.companyId && !s.IsRe_Evaluate);
                        if (getdata == null)
                        {
                            res.Message = "No task Available for Re-evalulation !";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                            // res.Data = list;
                        }
                        else
                        {
                            getdata.ReEvaluteDiscription = model.reEvalutedDiscription == null ? getdata.ReEvaluteDiscription : model.reEvalutedDiscription;
                            getdata.TaskRequest = ClientTaskRequestConstants.Revaluation;
                            getdata.IsRe_Evaluate = true;
                            getdata.IsApproved = false;
                            getdata.ProjectManagerId = tokendata.employeeId;
                            getdata.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getdata.UpdatedBy = tokendata.employeeId;
                            // list.Add(getdata);
                        }
                        _db.Entry(getdata).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Task Available for Approval !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        //  res.Data = list;
                    }
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/revaluatetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region  send Re-evalulation Task to pending task
        /// <summary>
        /// Created by Suraj Bundel on 14/03/2023
        /// API => Post => api/clientapproval/revaluatetopending
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("revaluatetopending")]
        [HttpPost]
        public async Task<IHttpActionResult> Revaluatetopending(ClienttaskIdRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Empty !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    foreach (var item in model.ClienttaskId)
                    {
                        var getdata = _db.ClientTaskApprovals.FirstOrDefault(s => s.ClientTaskId == item && s.IsActive && !s.IsDeleted && s.CompanyId == tokendata.companyId);
                        if (getdata == null)
                        {
                            res.Message = "No task Available for Re-evalulation !";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                            // res.Data = list;
                        }
                        else
                        {
                            getdata.TaskRequest = ClientTaskRequestConstants.Pending;
                            getdata.IsRe_Evaluate = false;
                            getdata.IsApproved = false;
                            getdata.IsSFA = false;
                            getdata.ProjectManagerId = tokendata.employeeId;
                            getdata.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                            getdata.UpdatedBy = tokendata.employeeId;
                            // list.Add(getdata);
                            _db.Entry(getdata).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }

                        res.Message = "Task Available for send to Approval !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                    }
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/revaluatetopending", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion 

        #region  Re-evalulation Task Edit
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Put => api/clientapproval/editrevaluatetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("editrevaluatetask")]
        [HttpPost]
        public async Task<IHttpActionResult> EditRevaluateTask(EditRevalTaskRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //    List<ClientTaskApproval> list = new List<ClientTaskApproval>();
                if (model == null)
                {
                    res.Message = "Model is Empty !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {

                    var getdata = _db.clientTaskModels.FirstOrDefault(s => s.ClientTaskId == model.ClientTaskId && s.IsActive && !s.IsDeleted && s.CompanyId == tokendata.companyId);
                    if (getdata == null)
                    {
                        res.Message = "No task Available for Re-evalulation !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        // res.Data = list;
                    }
                    else
                    {
                        var sendtask = _db.ClientTaskApprovals.FirstOrDefault(s => s.ClientTaskId == model.ClientTaskId && s.IsActive && !s.IsDeleted && s.CompanyId == tokendata.companyId);
                        sendtask.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                        sendtask.UpdatedBy = tokendata.employeeId;
                        //sendtask.TaskRequest = ClientTaskRequestConstants.Pending;
                        //sendtask.IsRe_Evaluate = false;
                        //sendtask.IsApproved = false;
                        getdata.TaskDate = model.Taskdate;
                        getdata.StartTime = model.StartTime;
                        getdata.EndTime = model.EndTime;
                        getdata.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                        getdata.UpdatedBy = tokendata.employeeId;
                        // list.Add(getdata);
                        _db.Entry(sendtask).State = System.Data.Entity.EntityState.Modified;
                    }
                    _db.Entry(getdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Task Available for Approval !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    //  res.Data = list;
                }

                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/revaluatetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion 

        #region Get All Re-Evaluate Task Information to PM
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API>>GET>>api/clientapproval/getallreevaluatetask?page?count?ProjectId
        /// </summary>
        /// <returns></returns>       
        [Route("getallreevaluatetask")]
        [HttpGet]
        public async Task<IHttpActionResult> GetReevaluateData(Guid ClientId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.ClientTaskApprovals.Where(x => x.ClientId == ClientId && x.IsActive && !x.IsDeleted &&
                       x.IsRe_Evaluate && x.CompanyId == tokenData.companyId).
                    Select(x => new
                    {
                        TaskApprovelId = x.ClientTaskApprovalId,
                        CreatedBy = x.CreatedBy,
                        TaskTiltle = x.TaskName,
                        SenderName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy && e.IsActive && !e.IsDeleted).Select(e => e.DisplayName).FirstOrDefault(),
                        //  TotalWorkingTimeInProject = x.TotalWorkingTime,
                        IsApproved = x.IsApproved,
                        RE_EVALUATE = x.IsRe_Evaluate,
                        UpdatedDate = x.UpdatedOn.Value,
                        //  TaskStartDate = _db.TaskModels.Where(t => t.TaskId == x.TaskId).Select(t => t.StartDate.Value).FirstOrDefault(),
                        //  EstimateTimeLong = x.EstimateTime,
                        //  EstimateTime = "",
                    }).OrderByDescending(x => x.UpdatedDate).ToListAsync();


                if (getData.Count > 0)
                {
                    //getData.ForEach(a => {
                    //    a.EstimateTime = string.Format("{00:00}:{01:00}", (int)a.EstimateTimeLong / 60, a.EstimateTimeLong % 60);
                    //});


                    res.Message = "Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
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
                    res.Message = "No Data Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/getallreevaluatetask", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Get Task Status

        /// <summary>
        /// Create by Suraj Bundel On 13-03-2023
        /// API >>  Post >> api/clientapproval/gettaskstatusenum
        /// </summary>
        /// <returns></returns>
        [Route("gettaskstatusenum")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllAssetsCondition()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var Tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var condition = Enum.GetValues(typeof(ClientTaskRequestConstants))
                    .Cast<ClientTaskRequestConstants>()
                    .Select(x => new HelperFortaskstatus
                    {
                        Id = (int)x,
                        Name = Enum.GetName(typeof(ClientTaskRequestConstants), x)
                    }).ToList();
                res.Message = "Task Status !";
                res.Status = true;
                res.Data = condition;
                res.StatusCode = HttpStatusCode.OK;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/clientapproval/gettaskstatusenum", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }

        }

        #endregion Used for Assets Condition Api

        //#region Get Task filter SP
        //[Route("taskfilterSP")]
        //[HttpGet]
        //public async Task<List<ClientTaskModelRequest>> TaskFilterSP(FilterSpRequest filterSP)
        //{
        //    List<ClientTaskModelRequest> responseList = new List<ClientTaskModelRequest>();
        //    try
        //    {
        //        var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        //        using (var con = new SqlConnection(_connectionString.ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("SP_ClientTaskFilter", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@companyId", SqlDbType.Int).Value = filterSP.companyId; 
        //            cmd.Parameters.Add("@clientId", SqlDbType.NVarChar).Value = Model.ClientId;
        //            cmd.Parameters.Add("@employeeId", SqlDbType.NVarChar).Value = Model.Employeid;
        //            cmd.Parameters.Add("@startdate", SqlDbType.Date).Value = Model.Startdate;
        //            cmd.Parameters.Add("@enddate", SqlDbType.Date).Value = Model.Enddate;
        //            cmd.Parameters.Add("@taskrequest", SqlDbType.Int).Value = Model.TaskStatus;
        //            cmd.Parameters.Add("@isadmin", SqlDbType.Binary).Value = tokendata.IsAdminInCompany;
        //            cmd.Parameters.Add("@isPM", SqlDbType.Binary).Value = RP;
        //            cmd.Parameters.Add("@isUser", SqlDbType.Binary).Value = tokendata.employeeId;

        //            con.Open();
        //            SqlDataReader rdr = await cmd.ExecuteReaderAsync();
        //            while (rdr.Read())
        //            {
        //                var entTaskId = Guid.Parse(rdr[1].ToString());
        //                responseList.Add(new ClientTaskModelRequest
        //                {
        //                    ClientTaskId = Guid.Parse(rdr["ClientTaskId"].ToString()),
        //                    ClientId = Guid.Parse(rdr["ClientId"].ToString()),
        //                    ClientCode = (rdr["ClientCode"]).ToString(),
        //                    TaskdateData = (DateTime)rdr["TaskDate"],
        //                    Taskdate = ((DateTime)rdr["TaskDate"]).ToString(),
        //                    WorkTypeId = Guid.Parse(rdr["WorkTypeId"].ToString()),
        //                    Description = (rdr["Description"]).ToString(),
        //                    worktypename = (rdr["WorkTypeName"]).ToString(),
        //                    ClientName = (rdr["ClientName"]).ToString(),
        //                    CreatedBy = (rdr["CreateByName"]).ToString(),
        //                    UpdatedBy = (rdr["UpdateByName"]).ToString(),
        //                    TaskRequest = (rdr["TaskRequest"]).ToString(),
        //                    TaskRequestid = Convert.ToInt32(rdr[12]),
        //                    IsSFA = Convert.ToBoolean(rdr["IsSFA"]),
        //                    CreatedOn = (DateTime)rdr["CreatedOn"],
        //                    UpdatedOn = (DateTime)rdr["UpdatedOn"],
        //                    Starttime = (rdr["StartTime"]).ToString(),
        //                    Endtime = (rdr["EndTime"]).ToString(),
        //                    IsApproved = Convert.ToBoolean(rdr["IsApproved"]),
        //                    TaskStarttime = ((DateTimeOffset)rdr["StartTime"]),
        //                    TaskEndtime = ((DateTimeOffset)rdr["EndTime"])
        //                });
        //            }
        //            con.Close();
        //            return responseList;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return responseList;
        //    }
        //}
        //#endregion

        #region USE SP // not in use
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API => Post => api/clientapproval/hittasksp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("hittasksp")]
        [HttpPost]
        public async Task<IHttpActionResult> HitTaskSP(PaginationtaskIdRequest Model)//(int? page = null, int? count = null, string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {



                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/clientapproval/getrevaluationtask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        public class FilterSpRequest
        {
            public int @companyId { get; set; } = 0;
            public string @clientId { get; set; }
            public string @EmployeIds { get; set; }
            public DateTimeOffset @Startdate { get; set; }
            public DateTimeOffset @Enddate { get; set; }
            public bool @RP { get; set; }
            public bool @IsAdminInCompany { get; set; }
            public bool @TaskStatus { get; set; }
        }
        #endregion

        #region Api for type of Task filter
        /// <summary>
        /// Created by Suraj Bundel on 09/03/2023
        /// API>POST>>api/clientapproval/gettaskstatus
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gettaskstatusfilter")]
        public async Task<IHttpActionResult> GetDataByTaskStatus(filterPaginationRequest Model)//(int ProjectId, DateTime? dateValue, int page = 1, int count = 10)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Manager = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.ReportingManager == tokendata.employeeId).FirstOrDefault();

                var task = await (from ap in _db.ClientTaskApprovals
                                  join t in _db.clientTaskModels on ap.ClientTaskId equals t.ClientTaskId
                                  join c in _db.NewClientModels on t.ClientId equals c.ClientId
                                  join e in _db.Employee on t.CreatedBy equals e.EmployeeId
                                  where t.IsActive && !t.IsDeleted && t.CompanyId == tokendata.companyId// && e.ReportingManager == tokenData.employeeId
                                  select new Taskstatusresponse
                                  {
                                      ReportingManager = e.ReportingManager,
                                      ClientTaskId = ap.ClientTaskId,
                                      ClientId = ap.ClientId,
                                      ClientCode = ap.ClientCode,
                                      Taskdate = null,
                                      Taskdatedata = t.TaskDate,
                                      WorkTypeId = ap.WorkTypeId,
                                      Description = t.Description,
                                      worktypename = _db.TypeofWorks.Where(x => x.WorktypeId == t.WorkTypeId).Select(x => x.WorktypeName).FirstOrDefault(),
                                      ClientName = c.ClientName,
                                      CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == t.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                      UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == t.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                      CreatedById = t.CreatedBy,
                                      UpdatedById = t.UpdatedBy,
                                      CreatedOn = t.CreatedOn,
                                      UpdatedOn = t.UpdatedOn,
                                      WorkingHours = TimeSpan.Zero,
                                      Starttime = null,
                                      Endtime = null,
                                      TaskStarttime = t.StartTime,
                                      TaskEndtime = t.EndTime,
                                      TaskRequest = ap.TaskRequest.ToString(),
                                      reEvalutedDiscription = ap.ReEvaluteDiscription,


                                  }).OrderByDescending(x => x.CreatedOn).ToListAsync();

                if (tokendata.IsAdminInCompany)
                {
                    if (task.Count > 0)
                    {
                        task.ForEach(x =>
                        {
                            x.Taskdate = x.Taskdatedata.ToString("MMM dd yyyy");
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                            x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                        });
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                        {
                            var text = textInfo.ToUpper(Model.Pendingsearch);
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                                       .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                            };
                        }
                        else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                        {
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),

                            };
                        }
                        else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                        {
                            var text = textInfo.ToUpper(Model.Approvesearch);
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString()).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Approved.ToString())
                                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                            };
                        }
                        else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                        {
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString()).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString())
                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),

                            };
                        }
                        else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                        {
                            var text = textInfo.ToUpper(Model.Revaluationsearch);
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString()).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString())
                                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                            };
                        }
                        else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                        {
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString()).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString())
                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),

                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = new
                        {
                            Pendingtaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                            Approvetaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString()).Count(),
                            Revaltaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString()).Count(),
                        };
                    }
                }

                else if (Manager != null)
                {

                    if (task.Count > 0)
                    {
                        task.ForEach(x =>
                        {
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                            x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                        });
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                        {
                            var text = textInfo.ToUpper(Model.Pendingsearch);
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId)
                                                       .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                            };
                        }
                        else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                        {
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId)
                                .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),

                            };
                        }
                        else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                        {
                            var text = textInfo.ToUpper(Model.Approvesearch);
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId)
                                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                            };
                        }
                        else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                        {
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId)
                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),

                            };
                        }
                        else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                        {
                            var text = textInfo.ToUpper(Model.Revaluationsearch);
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId)
                                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                            };
                        }
                        else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                        {
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId)
                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),

                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = new
                        {
                            Pendingtaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                            Approvetaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                            Revaltaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.ReportingManager == tokendata.employeeId).Count(),
                        };
                    }
                }
                else
                {

                    if (task.Count > 0)
                    {
                        task.ForEach(x =>
                        {
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
                            x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
                        });
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                        {
                            var text = textInfo.ToUpper(Model.Pendingsearch);
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId)
                                                       .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                            };
                        }
                        else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                        {
                            res.Data = new
                            {
                                TotalPendingData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                PendingDataCount = (int)Model.Pendingcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId)
                                .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),

                            };
                        }
                        else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                        {
                            var text = textInfo.ToUpper(Model.Approvesearch);
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId)
                                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                            };
                        }
                        else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                        {
                            res.Data = new
                            {
                                TotalApproveData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                ApproveDataCount = (int)Model.Approvecount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId)
                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),

                            };
                        }
                        else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                        {
                            var text = textInfo.ToUpper(Model.Revaluationsearch);
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId)
                                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                            };
                        }
                        else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                        {
                            res.Data = new
                            {
                                TotalRevalData = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                                RevalDataCount = (int)Model.Revaluationcount,
                                List = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId)
                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),

                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = new
                        {
                            Pendingtaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                            Approvetaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Approved.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                            Revaltaskdata = task.Where(x => x.TaskRequest == ClientTaskRequestConstants.Revaluation.ToString() && x.CreatedById == tokendata.employeeId).Count(),
                        };
                    }

                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/taskapprovel/gettaskstatus", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class filterPaginationRequest
        {
            public int? Pendingpage { get; set; } = 0;
            public int? Pendingcount { get; set; } = 0;
            public string Pendingsearch { get; set; } = string.Empty;

            public int? Approvepage { get; set; } = 0;
            public int? Approvecount { get; set; } = 0;
            public string Approvesearch { get; set; } = string.Empty;

            public int? Revaluationpage { get; set; } = 0;
            public int? Revaluationcount { get; set; } = 0;
            public string Revaluationsearch { get; set; } = string.Empty;
        }




        #endregion


        public class GetdataOrderbystatus
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class ClienttaskIdRequest
        {
            public List<Guid> ClienttaskId { get; set; }
            public string reEvalutedDiscription { get; set; }
        }
        public class HelperFortaskstatus
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class PaginationtaskIdRequest
        {
            public int? page { get; set; }
            public int? count { get; set; }
            public string search { get; set; }
        }
        public class EditRevalTaskRequest
        {
            public Guid ClientTaskId { get; set; }
            public DateTimeOffset Taskdate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string UpdatedById { get; set; }
            public string UpdatedOn { get; set; }
        }
        public class Taskstatusresponse
        {
            public Guid ClientTaskId { get; set; }
            public Guid ClientId { get; set; }
            public string ClientCode { get; set; }
            public string Taskdate { get; set; }
            public DateTimeOffset Taskdatedata { get; set; }
            public Guid WorkTypeId { get; set; }
            public string Description { get; set; }
            public string worktypename { get; set; }
            public string ClientName { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
            public int CreatedById { get; set; }
            public int? UpdatedById { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
            public bool IsDefaultCreated { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset? UpdatedOn { get; set; }
            public TimeSpan WorkingHours { get; set; }
            public string Starttime { get; set; }
            public string Endtime { get; set; }
            public TimeSpan TaskStarttime { get; set; }
            public TimeSpan TaskEndtime { get; set; }
            public string TaskRequest { get; set; }
            public int? ReportingManager { get; set; }
            public string reEvalutedDiscription { get; set; }

        }
    }
}
