using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
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
using static AspNetIdentity.WebApi.Controllers.TimeSheet.TaskCreationController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 06-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskapprovel")]
    public class TaskApprovelController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API FOR SEND APPROVEL TO PM
        /// <summary>
        /// Created by Ravi Vyas 06-12-2022
        /// API>>POST>>api/taskapprovel/sendforapprovel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("sendforapprovel")]
        [HttpPost]
        public async Task<IHttpActionResult> SendForApprovel(List<dynamic> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var taskSave = model.
                               Select(x => new TaskApprovel
                               {
                                   TaskId = x.taskId,
                                   ProjectId = x.projectId,
                                   ProjectName = x.projectName,
                                   TaskName = x.taskName,
                                   TotalWorkingTime = x.totalWorkingTimeInProject,
                                   ProjectManagerId = x.projectManagerId,
                                   TaskRequest = TaskRequestConstants.Pending,
                                   StartDate = x.startDate,
                                   EstimateTime = x.estimateTimeLong,
                                   CompanyId = tokenData.companyId,
                                   OrgId = tokenData.orgId,
                                   DataJson = JsonConvert.SerializeObject(x.dates),
                                   CreatedBy = tokenData.employeeId,
                                   CreatedOn = DateTimeOffset.Now,
                                   IsSFA = true,
                               })
                               .ToList();

                foreach (var task in taskSave)
                {
                    var check = _db.TaskApprovels.Where(a => a.TaskId == task.TaskId).FirstOrDefault();
                    if (check == null)
                    {
                        TaskApprovel obj = new TaskApprovel()
                        {
                            TaskId = task.TaskId,
                            ProjectId = task.ProjectId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                            CompanyId = tokenData.companyId,
                            IsSFA = true,
                        };

                        _db.TaskApprovels.Add(obj);
                        await _db.SaveChangesAsync();
                        check = obj;
                    }
                    else
                    {
                        check.UpdatedBy = tokenData.employeeId;
                        check.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                    }
                    check.ProjectName = task.ProjectName;
                    check.TaskName = task.TaskName;
                    check.TotalWorkingTime = task.TotalWorkingTime;
                    check.ProjectManagerId = task.ProjectManagerId;
                    check.TaskRequest = TaskRequestConstants.Pending;
                    check.StartDate = task.StartDate;
                    check.EstimateTime = task.EstimateTime;
                    check.CompanyId = tokenData.companyId;
                    check.OrgId = tokenData.orgId;
                    check.DataJson = task.DataJson;
                    check.IsRe_Evaluate = false;
                    check.IsSFA = true;

                    _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                res.Message = "Send Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = taskSave;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/taskapprovel/sendforapprovel", ex.Message, model);
                return BadRequest("Failed");
            }

        }

        #endregion

        #region API FOR GET SENDAR EMPLOYEE INFORMATION TO PM
        /// <summary>
        /// Created By Ravi Vyas on 07-12-2022
        /// API>>GET>>api/taskapprovel/getsendardata?page?count?ProjectId
        /// </summary>
        /// <returns></returns>       
        [Route("getsendardata")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSendarName(int ProjectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.TaskApprovels.
                                     Where(x => x.ProjectId == ProjectId && x.IsActive &&
                                    !x.IsDeleted && x.IsApproved == false &&
                                     x.IsRe_Evaluate == false && x.CompanyId == tokenData.companyId).
                                     Select(x => new GetApprovelRequestData
                                     {
                                         TaskApprovelId = x.TaskApprovelId,
                                         TaskId = x.TaskId,
                                         CreatedBy = x.CreatedBy,
                                         TaskTiltle = x.TaskName,
                                         SendarName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy && e.IsActive && !e.IsDeleted).Select(e => e.DisplayName).FirstOrDefault(),
                                         TotalWorkingTimeInProject = x.TotalWorkingTime,
                                         CreateOn = x.CreatedOn,
                                         TaskStartDate = _db.TaskModels.Where(t => t.TaskId == x.TaskId).Select(t => t.StartDate.Value).FirstOrDefault(),
                                         SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).ToList(),
                                         SpentTime1 = "",
                                         EstimateTimeLong = _db.TaskModels.Where(e => e.TaskId == x.TaskId).Select(e => e.EstimateTime).ToList().Sum(),
                                         EstimateTime = "",
                                     })
                                     .OrderByDescending(x => x.CreateOn)
                                     .ToListAsync();
                if (getData.Count > 0)
                {
                    getData.ForEach(x =>
                    {
                        x.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                        (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60));
                        x.EstimateTime = string.Format("{00:00}:{01:00}", (int)x.EstimateTimeLong / 60, x.EstimateTimeLong % 60);
                    });

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

                logger.Error("api/taskapprovel/getsendardata", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API FOR GET APPROVEL REQUEST BY APPROVEL ID TO PM
        /// <summary>
        /// Created By Ravi Vyas on 12-07-2022
        /// API>>GET>>api/taskapprovel/getapprovelrequsetbypm?TaskApprovelId
        /// </summary>
        /// <returns></returns>
        [Route("getapprovelrequsetbypm")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTaskDetail(Guid TaskApprovelId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                #region Comment Code 


                //var getData = (from t in _db.TaskModels
                //               join tl in _db.TaskLogs on t.TaskId equals tl.TaskId
                //               where t.IsActive && !t.IsDeleted && t.AssignEmployeeId == tokenData.employeeId
                //               select new DayEntry2
                //               {
                //                     task_id = t.TaskId,
                //                     project_id = t.ProjectId,
                //                     user_id = t.AssignEmployeeId,
                //                     task_name = t.TaskTitle,
                //                     project_name = _db.ProjectLists.Where(p=>p.ID == t.ProjectId).Select(p=>p.ProjectName).FirstOrDefault(),
                //                     SpentTime = tl.SpentTime,
                //                     updated_at = tl.DueDate

                //               }).OrderBy(x=>x.updated_at).ToList(); 

                //var getData = (from t in _db.TaskModels
                //               join ta in _db.TaskApprovels on t.ProjectId equals ta.ProjectId
                //               where t.IsActive && !t.IsDeleted && ta.ProjectManagerId == tokenData.employeeId
                //               select new RootResponse
                //               {
                //                   ProjectId = t.ProjectId,
                //                   ProjectName = _db.ProjectLists.Where(x => x.ID == t.ProjectId).Select(x => x.ProjectName).FirstOrDefault(),
                //                   TaskId = t.TaskId,
                //                   TaskName = t.TaskTitle,
                //                   Dates = _db.TaskApprovels.Where(c => c.TaskId == t.TaskId && c.ProjectManagerId == tokenData.employeeId).Select(c => new DateResponse
                //                   {

                //                       DateNew = c.DueDate,
                //                       SpentTime = c.SpentTime,
                //                       IsApproved = c.IsApproved,
                //                   }).OrderBy(x => x.DateNew).ToList(),
                //               }).ToList().Distinct();
                //rootObj.AddRange(getData);

                //foreach (var data in getData)
                //{
                //    if (data.Dates.Count > 0)
                //    {
                //        rootObj.AddRange(getData);
                //    }

                //}



                //var getData = _db.TaskModels.Where(x=>x.IsActive && !x.IsDeleted).ToList();
                //foreach (var item in getData)
                //{



                //    var d = _db.TaskApprovels.Where(p => p.TaskId == item.TaskId && p.ProjectManagerId == tokenData.employeeId).Select(p => new RootResponse
                //    {
                //        ProjectId = item.ProjectId,
                //        ProjectName = _db.ProjectLists.Where(x => x.ID == item.ProjectId).Select(x => x.ProjectName).FirstOrDefault(),
                //        TaskId = item.TaskId,
                //        TaskName = item.TaskTitle,
                //        Dates = _db.TaskApprovels.Where(c => c.TaskId == item.TaskId).Select(c => new DateResponse
                //        {

                //            DateNew = c.DueDate,
                //            SpentTime = c.SpentTime
                //        }).OrderBy(x => x.DateNew).ToList(),
                //    }).ToList();

                //    rootObj.AddRange(d);
                //}

                #endregion

                var getData = await _db.TaskApprovels.
                                    Where(x => x.IsApproved == false && x.IsActive && !x.IsDeleted &&
                                    x.TaskApprovelId == TaskApprovelId && x.CompanyId == tokenData.companyId).
                                    Select(x => new
                                    {
                                        TaskId = x.TaskId,
                                        TaskName = x.TaskName,
                                        ProjectId = x.ProjectId.Value,
                                        ProjectManagerId = x.ProjectManagerId,
                                        TotalWorkingTimeInProject = x.TotalWorkingTime,
                                        ListData = x.DataJson,
                                        EstimateTime = x.EstimateTime,
                                        StartDate = x.StartDate,
                                        SpentTime = _db.TaskLogs.Where(y => y.TaskId == x.TaskId).Select(y => y.SpentTime).ToList(),
                                        SpentTime1 = ""
                                    }).ToListAsync();
                var data = getData.
                           Select(x => new JsonToClassResponseModel
                           {
                               TaskId = x.TaskId,
                               TaskName = x.TaskName,
                               ProjectId = x.ProjectId,
                               ProjectManagerId = x.ProjectManagerId,
                               TotalWorkingTimeInProject = x.TotalWorkingTimeInProject,
                               StartDate = x.StartDate,
                               EstimateTime = x.EstimateTime,
                               SpentTime1 = string.Format("{00:00}:{01:00}", (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                             (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60)),
                               jsonToClassDatas = JsonConvert.DeserializeObject<List<JsonToClassData>>(x.ListData),
                           }).ToList();
                res.Message = "Send Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = data;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/taskapprovel/getapprovelrequsetbypm", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API FOR UPDATE APROVED TASK BY PM
        /// <summary>
        /// API>>PUT>>api/taskapprovel/approvedrequest
        /// Created By Ravi Vyas on 12-07-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("approvedrequest")]
        [HttpPost]
        public async Task<IHttpActionResult> ApprovedRequet(ApprovedRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                foreach (var item in model.TaskApprovelId)
                {
                    var getData = _db.TaskApprovels.Where(x => x.TaskApprovelId == item && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();
                    var statusData = _db.TaskModels.Where(x => x.TaskId == getData.TaskId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();
                    if (getData != null)
                    {
                        getData.IsApproved = true;
                        getData.UpdatedOn = DateTimeOffset.Now;
                        getData.TaskRequest = TaskRequestConstants.Approved;
                        getData.IsRe_Evaluate = false;
                        getData.UpdatedBy = tokenData.employeeId;
                        _db.Entry(getData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "No Record Found!!";
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                    if (statusData != null)
                    {
                        statusData.Status = TaskStatusConstants.Closed;
                        statusData.Percentage = 100;
                        statusData.DeletedBy = tokenData.employeeId;
                        statusData.DeletedOn = DateTime.Now;
                        _db.Entry(statusData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "No Record Found!!";
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                    //HostingEnvironment.QueueBackgroundWorkItem(ct => UpdateTaskBackgroundCheck(statusData));
                }
                res.Message = "Update Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/taskapprovel/approvedrequest", ex.Message, model);
                return BadRequest("Failed");
            }
        }


        #endregion

        #region API FOR GET ALL  APPROVEL TASK EMPLOYEE INFORMATION TO PM
        /// <summary>
        /// Created By Ravi Vyas on 08-12-2022
        /// API>>GET>>api/taskapprovel/getallapprovelsendardata?page?count?ProjectId
        /// </summary>
        /// <returns></returns>       
        [Route("getallapprovelsendardata")]
        [HttpGet]
        public async Task<IHttpActionResult> GetApprovelSendarName(int ProjectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.TaskApprovels.
                                     Where(x => x.ProjectId == ProjectId &&
                                     x.IsActive && !x.IsDeleted &&
                                     x.IsApproved == true && x.CompanyId == tokenData.companyId).
                                     Select(x => new GetApprovelRequestData
                                     {
                                         TaskId = x.TaskId,
                                         TaskApprovelId = x.TaskApprovelId,
                                         CreatedBy = x.CreatedBy,
                                         TaskTiltle = x.TaskName,
                                         SendarName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy && e.IsActive && !e.IsDeleted).Select(e => e.DisplayName).FirstOrDefault(),
                                         TotalWorkingTimeInProject = x.TotalWorkingTime,
                                         IsApproved = x.IsApproved,
                                         UpdatedDate = x.UpdatedOn.Value,
                                         EstimateTimeLong = x.EstimateTime,
                                         EstimateTime = "",
                                         TaskStartDate = _db.TaskModels.Where(t => t.TaskId == x.TaskId).Select(t => t.StartDate.Value).FirstOrDefault(),
                                         CreateOn = x.UpdatedOn.Value,
                                         SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).ToList(),
                                         SpentTime1 = "",
                                     })
                                     .OrderByDescending(x => x.UpdatedDate)
                                     .ToListAsync();

                if (getData.Count > 0)
                {
                    getData.ForEach(a =>
                    {
                        a.EstimateTime = string.Format("{00:00}:{01:00}", (int)a.EstimateTimeLong / 60, a.EstimateTimeLong % 60);
                        a.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(a.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                        (int)(a.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60));

                    });

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

                logger.Error("api/taskapprovel/getallapprovelsendardata", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API FOR GET All APPROVEL REQUEST BY APPROVEL ID TO PM
        /// <summary>
        /// Created By Ravi Vyas on 12-07-2022
        /// API>>GET>>api/taskapprovel/getallapprovelrequsetbypm?TaskApprovelId
        /// </summary>
        /// <returns></returns>
        [Route("getallapprovelrequsetbypm")]
        [HttpGet]
        public async Task<IHttpActionResult> GetApprovedTaskDetail(Guid TaskApprovelId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var getData = await _db.TaskApprovels.
                                    Where(x => x.IsApproved == true && x.IsActive && !x.IsDeleted &&
                                    x.TaskApprovelId == TaskApprovelId && x.CompanyId == tokenData.companyId).
                                    Select(x => new
                                    {
                                        TaskId = x.TaskId,
                                        TaskName = x.TaskName,
                                        ProjectId = x.ProjectId.Value,
                                        ProjectManagerId = x.ProjectManagerId,
                                        TotalWorkingTimeInProject = x.TotalWorkingTime,
                                        ListData = x.DataJson,
                                        IsApproved = x.IsApproved,
                                        SpentTime = _db.TaskLogs.Where(y => y.TaskId == x.TaskId).Select(y => y.SpentTime).ToList(),
                                        SpentTime1 = ""
                                    }).ToListAsync();
                var data = getData.
                           Select(x => new JsonToClassResponseModel
                           {
                               TaskId = x.TaskId,
                               TaskName = x.TaskName,
                               ProjectId = x.ProjectId,
                               ProjectManagerId = x.ProjectManagerId,
                               TotalWorkingTimeInProject = x.TotalWorkingTimeInProject,
                               IsApproved = x.IsApproved,
                               jsonToClassDatas = JsonConvert.DeserializeObject<List<JsonToClassData>>(x.ListData),
                               SpentTime1 = string.Format("{00:00}:{01:00}", (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60), (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60)),

                           })
                           .ToList();

                res.Message = "Send Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = data;
                return Ok(res);

            }
            catch (Exception ex)
            {

                logger.Error("api/taskapprovel/getapprovelrequsetbypm", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API FOR RE_EVALUATE 
        /// <summary>
        /// Creted By Ravi Vyas on 09-12-2022
        /// API>>PUT>> api/taskapprovel/taskreevaluete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns> 
        [Route("taskreevaluete")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateReevaluate(ReEvaluteRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reEvalute = _db.TaskApprovels.
                                 Where(x => x.TaskApprovelId == model.TaskApprovelId &&
                                 x.IsActive && !x.IsDeleted &&
                                 x.CompanyId == tokenData.companyId).
                                 FirstOrDefault();
                if (reEvalute != null)
                {
                    reEvalute.IsApproved = false;
                    reEvalute.ReEvaluteDiscription = model.ReEvalutedDiscription;
                    reEvalute.TaskRequest = TaskRequestConstants.Pending;
                    reEvalute.IsRe_Evaluate = true;
                    reEvalute.IsSFA = false;
                    reEvalute.UpdatedOn = DateTimeOffset.Now;
                    reEvalute.UpdatedBy = tokenData.employeeId;
                    _db.Entry(reEvalute).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Message = "Succesfully Update !";
                    res.Data = reEvalute;
                    return Ok(res);
                }
                else
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Message = "Data not Found !";
                    res.Data = reEvalute;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/taskapprovel/taskreevaluete", ex.Message, model);
                return BadRequest("Failed");
            }
        }



        public class ReEvaluteRequest
        {
            public Guid TaskApprovelId { get; set; }
            public string ReEvalutedDiscription { get; set; }
        }


        #endregion

        #region API FOR GET ALL  RE_EVALUATE TASK INFORMATION TO PM
        /// <summary>
        /// Created By Shagun Moyade on 29-12-2022
        /// API>>GET>>api/taskapprovel/getallreevaluatedata?page?count?ProjectId
        /// </summary>
        /// <returns></returns>       
        [Route("getallreevaluatedata")]
        [HttpGet]
        public async Task<IHttpActionResult> GetReevaluateData(int ProjectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.TaskApprovels.
                                     Where(x => x.ProjectId == ProjectId && x.IsActive && !x.IsDeleted &&
                                     x.IsRe_Evaluate == true && x.CompanyId == tokenData.companyId).
                                     Select(x => new GetApprovelRequestData
                                     {
                                         TaskApprovelId = x.TaskApprovelId,
                                         CreatedBy = x.CreatedBy,
                                         TaskTiltle = x.TaskName,
                                         SendarName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy && e.IsActive && !e.IsDeleted).Select(e => e.DisplayName).FirstOrDefault(),
                                         TotalWorkingTimeInProject = x.TotalWorkingTime,
                                         IsApproved = x.IsApproved,
                                         RE_EVALUATE = x.IsRe_Evaluate,
                                         UpdatedDate = x.UpdatedOn.Value,
                                         TaskStartDate = _db.TaskModels.Where(t => t.TaskId == x.TaskId).Select(t => t.StartDate.Value).FirstOrDefault(),
                                         EstimateTimeLong = x.EstimateTime,
                                         EstimateTime = "",
                                         CreateOn = x.CreatedOn
                                     })
                                     .OrderByDescending(x => x.UpdatedDate)
                                     .ToListAsync();
                if (getData.Count > 0)
                {
                    getData.ForEach(a =>
                    {
                        a.EstimateTime = string.Format("{00:00}:{01:00}", (int)a.EstimateTimeLong / 60, a.EstimateTimeLong % 60);
                    });

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

                logger.Error("api/taskapprovel/getallreevaluatedata", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api for Task Approvel Filter
        /// <summary>
        /// API>POST>>api/taskapprovel/approvelfillter
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("approvelfillter")]
        public async Task<IHttpActionResult> GetDataBy(int ProjectId, DateTime? dateValue, int page = 1, int count = 10)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset checkDate = new DateTimeOffset();
                if (dateValue.HasValue)
                    checkDate = TimeZoneConvert.ConvertTimeToSelectedZone(dateValue.Value, tokenData.TimeZone);
                var getAll = await _db.TaskApprovels.
                                   Where(t => t.IsActive && !t.IsDeleted && t.CompanyId == tokenData.companyId && t.ProjectId == ProjectId && t.UpdatedOn.HasValue).
                                   Where(x => dateValue.HasValue ? (x.IsApproved && x.UpdatedOn.Value >= checkDate.Date) : x.IsActive).
                                   Select(t => new GetApprovelRequestData
                                   {
                                       TaskApprovelId = t.TaskApprovelId,
                                       CreatedBy = t.CreatedBy,
                                       TaskTiltle = t.TaskName,
                                       SendarName = _db.Employee.Where(e => e.EmployeeId == t.CreatedBy && e.IsActive && !e.IsDeleted).Select(e => e.DisplayName).FirstOrDefault(),
                                       TotalWorkingTimeInProject = t.TotalWorkingTime,
                                       IsApproved = t.IsApproved,
                                       UpdatedDate = t.UpdatedOn.Value,
                                       EstimateTimeLong = t.EstimateTime,
                                       EstimateTime = "",
                                       TaskStartDate = _db.TaskModels.Where(x => x.TaskId == t.TaskId).Select(x => x.StartDate.Value).FirstOrDefault(),
                                   })
                                  .OrderByDescending(x => x.UpdatedDate)
                                  .Skip((page - 1) * count).Take(count)
                                  .ToListAsync();

                if (getAll.Count == 0)
                {
                    res.Message = "Task list Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new
                    {
                        Page = page,
                        Count = count,
                        TaskApprovalList = getAll,
                    };
                    return Ok(res);
                }

                getAll.ForEach(t =>
                {
                    t.EstimateTime = string.Format("{00:00}:{01:00}", (int)t.EstimateTimeLong / 60, t.EstimateTimeLong % 60);
                });

                res.Message = "Task list Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Found;
                res.Data = new
                {
                    Page = page,
                    Count = count,
                    TaskApprovalList = getAll,
                };
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/taskapprovel/approvelfillter", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class TaskFillterResponseModel : TaskResponseModel
        {
            public int ProjectId { get; set; }
            public TaskTypeConstants TaskTypeId { get; set; }
            public TaskStatusConstants StatusId { get; set; }
            public int CreatedBy { get; set; } = 0;
            public int? UpdatedBy { get; set; } = null;

        }


        #endregion

        #region Helper Model Class

        public class GetApprovelRequestData
        {
            public Guid TaskApprovelId { get; set; } = Guid.Empty;
            public int CreatedBy { get; set; } = 0;
            public string SendarName { get; set; }
            public string TotalWorkingTimeInProject { get; set; }
            public string TaskTiltle { get; set; }
            public bool IsApproved { get; set; }
            public DateTimeOffset UpdatedDate { get; set; }
            public DateTimeOffset CreateOn { get; set; }
            public bool RE_EVALUATE { get; set; }
            public Guid TaskId { get; set; }
            public DateTimeOffset TaskStartDate { get; set; }
            public long EstimateTimeLong { get; set; }
            public string EstimateTime { get; set; }
            public List<TimeSpan> SpentTime { get; set; }
            public string SpentTime1 { get; set; }

        }
        public class ApprovelResponseClass
        {
            public DateTimeOffset TaskStartDate { get; set; }
            public DateTimeOffset TaskEndDate { get; set; }
            public int RequestEmployeeId { get; set; }
        }
        public class ApprovelId
        {
            public int ProjectId { get; set; }
            public Guid TaskId { get; set; } = Guid.Empty;
        }
        public class JsonToClassData
        {
            public string day { get; set; }
            public DateTime dateNew { get; set; }
            public string spentTime { get; set; }
            public bool isCurrent { get; set; }
            public bool isApproved { get; set; }
            public string ViewSpentTime { get; set; }
        }

        public class JsonToClassResponseModel
        {
            public Guid TaskId { get; set; } = Guid.Empty;
            public string ProjectName { get; set; }
            public int ProjectId { get; set; } = 0;
            public string TaskName { get; set; }
            public string TotalWorkingTimeInProject { get; set; }
            public string SpentTime1 { get; set; }
            public int ProjectManagerId { get; set; }
            public bool IsApproved { get; set; }
            public DateTimeOffset StartDate { get; set; }
            //public TimeSpan EstimateTime { get; set; }
            public double EstimateTime { get; set; }
            public List<JsonToClassData> jsonToClassDatas { get; set; }
        }
        public class ApprovedRequest
        {
            public List<Guid> TaskApprovelId { get; set; }

        }
        #endregion

    }
}
