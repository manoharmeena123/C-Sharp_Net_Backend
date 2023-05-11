using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.NewClientRequirement.Clienttask;
using AspNetIdentity.WebApi.Models;
using DocumentFormat.OpenXml.Drawing;
using LinqKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.ClientNewRequirement.NewClientController;

namespace AspNetIdentity.WebApi.Controllers.ClientNewRequirement
{
    [RoutePrefix("api/newclienttask")]
    [Authorize]
    public class NewClientTaskController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        #region Add Client Task

        /// <summary>
        /// API >> Post >> api/newclienttask/addclienttask
        /// Craeted by Suraj Bundel on 02-03-2023
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addclienttask")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateClientTask(List<AddclientTaskRequest> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            List<clientTaskModel> list = new List<clientTaskModel>();
            List<clientTaskModelHistory> history = new List<clientTaskModelHistory>();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                int faltyAdding = 0;
                if (model == null)
                {
                    res.Message = "Model is empty !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    model = model.Where(x => x.ClientId != Guid.Empty).ToList();
                    if (model.Count == 0)
                    {
                        res.Message = "Model is empty !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        return Ok(res);
                    }
                    var clientIds = model.Select(x => x.ClientId).Distinct().ToList();
                    var checkClientTimeRange = await _db.clientTaskModels
                        .Where(x => x.CompanyId == tokendata.companyId && x.IsActive &&
                            !x.IsDeleted && clientIds.Contains(x.ClientId))
                        .ToListAsync();
                    foreach (var item in model)
                    {
                        if (checkClientTimeRange
                            .Any(x => x.TaskDate.Date == item.TaskDate.Date && x.StartTime == item.StartTime && x.EndTime == item.EndTime))
                        //(x.StartTime != item.StartTime && x.EndTime > item.StartTime) || 
                        //(x.StartTime < item.EndTime && x.EndTime > item.EndTime)))
                        {

                            faltyAdding++;
                            continue;
                        }
                        else
                        {
                            if (item.StartTime == item.EndTime)
                            {
                                faltyAdding++;
                                continue;
                            }

                            if (item.StartTime > item.EndTime)
                            {
                                faltyAdding++;
                                continue;
                            }

                            clientTaskModel obj = new clientTaskModel()
                            {
                                ClientId = item.ClientId,
                                ClientCode = _db.NewClientModels.Where(x => x.ClientId == item.ClientId && !x.IsDeleted && x.IsActive && x.CompanyId == tokendata.companyId).Select(x => x.ClientCode).FirstOrDefault(),

                                StartTime = item.StartTime,
                                EndTime = item.EndTime,
                                WorkTypeId = item.WorkTypeId,
                                Description = item.Description,
                                TaskDate = TimeZoneConvert.ConvertTimeToSelectedZone(item.TaskDate, tokendata.TimeZone),
                                IsActive = true,
                                IsDeleted = false,
                                CompanyId = tokendata.companyId,
                                OrgId = tokendata.orgId,
                                CreatedBy = tokendata.employeeId,
                                CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                            };

                            clientTaskModelHistory hisobj = new clientTaskModelHistory()
                            {
                                ClientTaskId = obj.ClientTaskId,
                                ClientId = obj.ClientId,
                                TaskDate = obj.TaskDate,
                                StartTime = obj.StartTime,
                                EndTime = obj.EndTime,
                                WorkTypeId = obj.WorkTypeId,
                                Description = obj.Description,
                                CreatedBy = tokendata.employeeId,
                                CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                                IsActive = obj.IsActive,
                                IsDeleted = obj.IsDeleted,
                                CompanyId = obj.CompanyId,
                                OrgId = obj.OrgId,
                                UpdatedBy = obj.UpdatedBy,
                                UpdatedOn = obj.UpdatedOn,
                                DeletedOn = obj.DeletedOn,
                                DeletedBy = obj.DeletedBy,
                            };
                            //list.Add(obj);
                            //history.Add(hisobj);
                            _db.clientTaskModelHistorys.Add(hisobj);

                            _db.clientTaskModels.Add(obj);
                            await _db.SaveChangesAsync();
                            checkClientTimeRange.Add(obj);
                            res.Message = "Task Saved Successfully";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.OK;
                        }
                    }
                }
                if (faltyAdding == 0)
                {
                    res.Message = "All Task are Saved";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    res.Message = faltyAdding + " Task are not saved";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.Accepted;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/newclienttask/addclienttask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.Message);
            }
        }
        public class AddclientTaskRequest
        {
            //  public Guid ClientTaskId { get; set; }
            public Guid ClientId { get; set; } = Guid.Empty;
            public DateTime TaskDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public Guid WorkTypeId { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region get client task
        /// <summary>
        /// Created by Suraj Bundel on  03/03/2023
        /// API => GET => api/newclienttask/getclienttask
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getclienttask")]
        public async Task<IHttpActionResult> Getall(int? page = null, int? count = null, string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                //var getemployeelist = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId).ToList();
                //var data = getemployeelist.Where(x => x.ReportingManager == tokendata.employeeId).FirstOrDefault();

                var getall = await (from c in _db.clientTaskModels
                                    join e in _db.Employee on c.CreatedBy equals e.EmployeeId
                                    join a in _db.ClientTaskApprovals on c.ClientTaskId equals a.ClientTaskId into gj
                                    from sub in gj.DefaultIfEmpty()
                                    where c.IsActive && !c.IsDeleted && c.CompanyId == tokendata.companyId //&& sub.IsActive && sub.IsDeleted==false && sub.CompanyId == tokendata.companyId
                                    select new ClientTaskModelRequest
                                    {
                                        ClientTaskId = c.ClientTaskId,
                                        WorkTypeId = c.WorkTypeId,
                                        Worktypename = _db.TypeofWorks.Where(y => y.WorktypeId == c.WorkTypeId).Select(y => y.WorktypeName).FirstOrDefault(),
                                        ClientId = c.ClientId,
                                        ClientName = _db.NewClientModels.Where(y => y.ClientId == c.ClientId).Select(y => y.ClientName).FirstOrDefault(),
                                        IsSFA = sub == null ? false : sub.IsSFA,
                                        Description = c.Description,
                                        ClientCode = c.ClientCode,
                                        Taskdate = null,
                                        TaskdateData = c.TaskDate,
                                        TaskStarttime = c.StartTime,
                                        TaskRequest = sub == null ? ClientTaskRequestConstants.New.ToString().Replace("_", " ") : sub.TaskRequest.ToString().Replace("_", " "),
                                        TaskEndtime = c.EndTime,
                                       // Starttime = TimeSpan.Zero.ToString(@"hh\:mm\tt"),
                                       // Endtime = TimeSpan.Zero.ToString(@"hh\:mm\tt"),
                                        WorkingHours = TimeSpan.Zero,
                                        IsActive = c.IsActive,
                                        IsDeleted = c.IsDeleted,
                                        CompanyId = c.CompanyId,
                                        OrgId = c.OrgId,
                                        IsDefaultCreated = c.IsDefaultCreated,
                                        CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == c.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                        UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == c.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                        CreatedOn = c.CreatedOn,
                                        UpdatedOn = c.UpdatedOn,
                                        CreatedById = c.CreatedBy,
                                        UpdatedById = c.UpdatedBy,
                                        ReportingManager = e.ReportingManager,// _db.Employee.Where(x => c.CreatedBy == x.EmployeeId).Select(x => x.ReportingManager).FirstOrDefault(),
                                        IsApproved = sub == null ? false : sub.IsApproved,
                                    }).OrderByDescending(x => x.CreatedOn).ToListAsync();


                if (getall.Count > 0)
                {
                    getall.ForEach(x =>
                    {
                        x.Taskdate = x.TaskdateData.ToString("MMM dd yyyy");
                        x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                        TimeSpan timespan2 = x.TaskStarttime;
                        DateTime time1 = DateTime.Today.Add(timespan2);
                        string displayTime2 = time1.ToString("hh:mm tt");
                        x.Starttime = displayTime2;
                        TimeSpan timespan = x.TaskEndtime;
                        DateTime time = DateTime.Today.Add(timespan);
                        string displayTime = time.ToString("hh:mm tt");
                        x.Endtime = displayTime;
                    });
                    if (tokendata.IsAdminInCompany)
                    {
                        res.Message = "Client List";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                        {
                            var text = textInfo.ToUpper(search);
                            res.Data = new
                            {
                                TotalData = getall.Count,
                                Counts = (int)count,
                                List = getall.Where(x => x.ClientName.ToUpper().Contains(text))
                                                       .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else if (page.HasValue && count.HasValue)
                        {
                            res.Data = new
                            {
                                TotalData = getall.Count,
                                Counts = (int)count,
                                List = getall.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else
                        {
                            res.Data = getall;
                        }
                        return Ok(res);
                    }
                    else if (await _db.Employee.AnyAsync(x => x.ReportingManager == tokendata.employeeId && x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId &&
                                 x.EmployeeTypeId != WebApi.Model.EnumClass.EmployeeTypeConstants.Ex_Employee))
                    {
                        res.Message = "Client List";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                        {
                            var text = textInfo.ToUpper(search);
                            res.Data = new
                            {
                                TotalData = getall.Where(x => x.ReportingManager == tokendata.employeeId).Count(),
                                Counts = (int)count,
                                List = getall.Where(x => x.ReportingManager == tokendata.employeeId && x.ClientName.ToUpper().Contains(text))
                                                       .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else if (page.HasValue && count.HasValue)
                        {
                            res.Data = new
                            {
                                TotalData = getall.Where(x => x.ReportingManager == tokendata.employeeId).Count(),
                                Counts = (int)count,
                                List = getall.Where(x => x.ReportingManager == tokendata.employeeId).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else
                        {
                            res.Data = getall.Where(x => x.ReportingManager == tokendata.employeeId).ToList();
                        }
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Client List";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                        {
                            var text = textInfo.ToUpper(search);
                            res.Data = new
                            {
                                TotalData = getall.Where(x => x.CreatedById == tokendata.employeeId).Count(),
                                Counts = (int)count,
                                List = getall.Where(x => x.CreatedById == tokendata.employeeId && x.ClientName.ToUpper().Contains(text))
                                                       .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else if (page.HasValue && count.HasValue)
                        {
                            res.Data = new
                            {
                                TotalData = getall.Where(x => x.CreatedById == tokendata.employeeId).Count(),
                                Counts = (int)count,
                                List = getall.Where(x => x.CreatedById == tokendata.employeeId).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else
                        {
                            res.Data = getall.Where(x => x.CreatedById == tokendata.employeeId).ToList();
                        }
                        return Ok(res);

                    }
                }
                else
                {
                    res.Message = "Client list not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getall;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclienttask/getclienttask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Update client task
        /// <summary>
        /// Created by Suraj Bundel on  03/03/2023
        /// API => POST => api/newclienttask/editclienttask
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editclienttask")]
        public async Task<IHttpActionResult> Update(updateclientTaskRequest Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                if (Model == null)
                {
                    res.Message = "Model is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    //res.Data= tokendata;
                    return Ok(res);
                }
                else
                {
                    var edit = _db.clientTaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.ClientTaskId == Model.Id).FirstOrDefault();
                    if (edit == null)
                    {
                        res.Message = "Client not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = edit;
                        return Ok(res);
                    }
                    else
                    {
                        if (Model.StartTime == Model.EndTime)
                        {
                            res.Message = "Start time and End time should not be same";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.Ambiguous;
                            return Ok(res);
                        }
                        //var duplicate = _db.NewClientModels.Where(x => /*x.ClientCode == Model.ClientCode && x.ClientId == Model.Id && */!x.IsDeleted && x.IsActive && x.CompanyId == tokendata.companyId).ToList();
                        //if (duplicate.Count > 0)
                        //{
                        //    res.Message = "Duplicate Client Code found";
                        //    res.Status = false;
                        //    res.StatusCode = HttpStatusCode.Ambiguous;
                        //    return Ok(res);
                        //}
                        else
                        {


                            edit.ClientTaskId = Model.Id;
                            edit.ClientId = Model.ClientId;
                            edit.ClientCode = _db.NewClientModels.Where(x => x.ClientId == Model.ClientId && !x.IsDeleted && x.IsActive && x.CompanyId == tokendata.companyId).Select(x => x.ClientCode).FirstOrDefault();
                            edit.TaskDate = Model.TaskDate;
                            edit.StartTime = Model.StartTime;
                            edit.EndTime = Model.EndTime;
                            edit.WorkTypeId = Model.WorkTypeId;
                            edit.Description = Model.Description;
                            edit.UpdatedBy = tokendata.employeeId;
                            edit.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                            _db.Entry(edit).State = EntityState.Modified;
                            await _db.SaveChangesAsync();

                            res.Message = "Client data Updated Successfully";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.OK;
                            res.Data = edit;

                            clientTaskModelHistory hisobj = new clientTaskModelHistory()
                            {
                                ClientTaskId = edit.ClientTaskId,
                                ClientId = edit.ClientId,
                                TaskDate = edit.TaskDate,
                                StartTime = edit.StartTime,
                                EndTime = edit.EndTime,
                                WorkTypeId = edit.WorkTypeId,
                                Description = edit.Description,
                                CreatedBy = tokendata.employeeId,
                                CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                                IsActive = edit.IsActive,
                                IsDeleted = edit.IsDeleted,
                                CompanyId = edit.CompanyId,
                                OrgId = edit.OrgId,
                                UpdatedBy = edit.UpdatedBy,
                                UpdatedOn = edit.UpdatedOn,
                                DeletedOn = edit.DeletedOn,
                                DeletedBy = edit.DeletedBy,
                            };
                            _db.clientTaskModelHistorys.Add(hisobj);
                            await _db.SaveChangesAsync();

                            return Ok(res);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclienttask/editclienttask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region Delete client
        /// <summary>
        /// Created by Suraj Bundel on  03/03/2023
        /// API => POST => api/newclienttask/deleteclienttask
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deleteclienttask")]
        public async Task<IHttpActionResult> Delete(UpdateClientRequest Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                if (Model == null)
                {
                    res.Message = "Model is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    var edit = _db.clientTaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.ClientTaskId == Model.Id).FirstOrDefault();
                    if (edit == null)
                    {
                        res.Message = "Client task not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = edit;
                        return Ok(res);
                    }
                    else
                    {
                        edit.IsActive = false;
                        edit.IsDeleted = true;
                        edit.DeletedBy = tokendata.employeeId;
                        edit.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                        _db.Entry(edit).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Client task Deleted Successfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = edit;

                        clientTaskModelHistory hisobj = new clientTaskModelHistory()
                        {
                            ClientTaskId = edit.ClientTaskId,
                            ClientId = edit.ClientId,
                            TaskDate = edit.TaskDate,
                            StartTime = edit.StartTime,
                            EndTime = edit.EndTime,
                            WorkTypeId = edit.WorkTypeId,
                            Description = edit.Description,
                            CreatedBy = tokendata.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                            IsActive = edit.IsActive,
                            IsDeleted = edit.IsDeleted,
                            CompanyId = edit.CompanyId,
                            OrgId = edit.OrgId,
                            UpdatedBy = edit.UpdatedBy,
                            UpdatedOn = edit.UpdatedOn,
                            DeletedOn = edit.DeletedOn,
                            DeletedBy = edit.DeletedBy,
                        };
                        _db.clientTaskModelHistorys.Add(hisobj);
                        await _db.SaveChangesAsync();

                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclienttask/deleteclienttask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region get Client task by Id
        /// <summary>
        /// Created by Suraj Bundel on  23/02/2023
        /// API => GET => api/newclienttask/getclienttaskbyid
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getclienttaskbyid")]
        public async Task<IHttpActionResult> GetbyId(Guid id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var workinghour = _db.clientTaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.ClientTaskId == id).FirstOrDefault();
                if (workinghour != null)
                {
                    var hours = workinghour.EndTime.Subtract(workinghour.StartTime);
                    TimeSpan timespan2 = workinghour.StartTime;
                    DateTime time1 = DateTime.Today.Add(timespan2);
                    string displayTime = time1.ToString("hh:mm tt");
                    TimeSpan timespan = workinghour.EndTime;
                    DateTime time = DateTime.Today.Add(timespan);
                    string displayTime1 = time1.ToString("hh:mm tt");

                    var getall = await (from nt in _db.clientTaskModels
                                        join at in _db.ClientTaskApprovals on nt.ClientTaskId equals at.ClientTaskId into gj
                                        from sub in gj.DefaultIfEmpty()
                                        where nt.CompanyId == tokendata.companyId && !nt.IsDeleted && nt.IsActive && nt.ClientTaskId == id
                                        select new
                                        {
                                            nt.ClientTaskId,
                                            nt.WorkTypeId,
                                            worktypename = _db.TypeofWorks.Where(x => x.WorktypeId == nt.WorkTypeId).Select(x => x.WorktypeName).FirstOrDefault(),
                                            nt.ClientId,
                                            ClientName = _db.NewClientModels.Where(x => x.ClientId == nt.ClientId).Select(x => x.ClientName).FirstOrDefault(),
                                            nt.Description,
                                            nt.ClientCode,
                                            nt.TaskDate,
                                            //  StartTime = nt.StartTime.ToString("h m tt"),
                                            // EndTime = nt.EndTime.ToString("h m tt"),
                                            StartTime = displayTime,
                                            EndTime = displayTime1,
                                            nt.IsActive,
                                            nt.IsDeleted,
                                            nt.CompanyId,
                                            nt.OrgId,
                                            nt.IsDefaultCreated,
                                            CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == nt.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                            UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == nt.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                            TaskRequest = sub == null ? ClientTaskRequestConstants.New.ToString().Replace("_", " ") : sub.TaskRequest.ToString().Replace("_", " "),
                                            nt.CreatedOn,
                                            nt.UpdatedOn,
                                            //   wh = TimeSpan.Zero,
                                            WorkingHour = hours
                                        }).FirstOrDefaultAsync();

                    if (getall != null)
                    {
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        res.Data = getall;

                        if (getall == null)
                        {
                            res.Message = "Work type list not found !";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                            res.Data = getall;
                        }
                        else
                        {
                            res.Message = "Work type List";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.OK;
                            res.Data = getall;
                        }
                    }
                }
                else
                {
                    res.Message = "working Hours are null";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = workinghour;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/newclienttask/getclienttaskbyid", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }

        #endregion

        #region get total working hours  task filter
        /// <summary>
        /// Created by Suraj Bundel on 06/03/2023
        /// API => Post => api/newclienttask/taskfilter
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [Route("taskfilter")]
        [HttpPost]
        public async Task<IHttpActionResult> GetWorkingHours(GetWorkingHoursRequest Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {

                TimeSpan TotalHours = TimeSpan.Zero;

                var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                List<ClientTaskModelRequest> list = new List<ClientTaskModelRequest>();

                if (Model == null)
                {
                    res.Message = "Model is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    #region get all data

                    //var getemployeelist = await _db.Employee
                    //    .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId &&
                    //        x.EmployeeTypeId != WebApi.Model.EnumClass.EmployeeTypeConstants.Ex_Employee)
                    //    .ToListAsync();

                    //var cherkRp = getemployeelist.Where(x => x.ReportingManager == tokenData.employeeId).FirstOrDefault();

                    var gatData = await (from c in _db.clientTaskModels
                                         join a in _db.ClientTaskApprovals on c.ClientTaskId equals a.ClientTaskId into gj
                                         from sub in gj.DefaultIfEmpty()
                                         where c.IsActive && !c.IsDeleted && c.CompanyId == tokenData.companyId
                                         select new ClientTaskModelRequest
                                         {
                                             ClientTaskId = c.ClientTaskId,
                                             WorkTypeId = c.WorkTypeId,
                                             Worktypename = _db.TypeofWorks.Where(y => y.WorktypeId == c.WorkTypeId).Select(y => y.WorktypeName).FirstOrDefault(),
                                             ClientId = c.ClientId,
                                             ClientName = _db.NewClientModels.Where(y => y.ClientId == c.ClientId).Select(y => y.ClientName).FirstOrDefault(),
                                             IsSFA = sub == null ? false : sub.IsSFA,
                                             Description = c.Description,
                                             ClientCode = c.ClientCode,
                                             Taskdate = null,
                                             TaskdateData = c.TaskDate,
                                             TaskStarttime = c.StartTime,
                                             TaskRequest = sub == null ? ClientTaskRequestConstants.New.ToString().Replace("_", " ") : sub.TaskRequest.ToString().Replace("_", " "),
                                             TaskEndtime = c.EndTime,
                                            // Starttime = TimeSpan.Zero.ToString(@"hh\:mm"),
                                            // Endtime = TimeSpan.Zero.ToString(@"hh\:mm"),
                                             WorkingHours = TimeSpan.Zero,
                                             IsActive = c.IsActive,
                                             IsDeleted = c.IsDeleted,
                                             CompanyId = c.CompanyId,
                                             OrgId = c.OrgId,
                                             IsDefaultCreated = c.IsDefaultCreated,
                                             CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokenData.companyId && y.EmployeeId == c.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                             UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokenData.companyId && y.EmployeeId == c.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                             CreatedOn = c.CreatedOn,
                                             UpdatedOn = c.UpdatedOn,
                                             CreatedById = c.CreatedBy,
                                             UpdatedById = c.UpdatedBy,
                                             TaskRequestid = sub == null ? (int)ClientTaskRequestConstants.New : (int)sub.TaskRequest,
                                             ReportingManager = _db.Employee.Where(x => c.CreatedBy == x.EmployeeId).Select(x => x.ReportingManager).FirstOrDefault(),
                                             TotalWorkingHours = "",
                                             // e.ReportingManager == 0 ? 0 : e.ReportingManager,
                                         }).ToListAsync();
                    var getall = gatData
                         .Select(go => new ClientTaskModelRequest
                         {
                             ClientTaskId = go.ClientTaskId,
                             ClientName = go.ClientName,
                             IsSFA = go.IsSFA,
                             ClientCode = go.ClientCode,
                             CompanyId = go.CompanyId,
                             ClientId = go.ClientId,
                             CreatedBy = go.CreatedBy,
                             CreatedOn = go.CreatedOn,
                             UpdatedOn = go.UpdatedOn,
                             CreatedById = go.CreatedById,
                             Description = go.Description,
                             Endtime = go.Endtime,
                             TotalWorkingHours = String.Format("{00:00}:{00:01}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                             ReportingManager = go.ReportingManager,
                             TaskRequestid = go.TaskRequestid,
                             UpdatedById = go.UpdatedById,
                             UpdatedBy = go.UpdatedBy,
                             IsDefaultCreated = go.IsDefaultCreated,
                             OrgId = go.OrgId,
                             IsDeleted = go.IsDeleted,
                             IsActive = go.IsActive,
                             WorkingHours = go.WorkingHours,
                             Starttime = go.Starttime,
                             TaskEndtime = go.TaskEndtime,
                             TaskRequest = go.TaskRequest,
                             TaskStarttime = go.TaskStarttime,
                             TaskdateData = go.TaskdateData,
                             Taskdate = go.Taskdate,
                             Worktypename = go.Worktypename,
                         }).OrderBy(go => go.ClientName).ToList();
                    if (getall.Count > 0)
                    {
                        getall.ForEach(x =>
                        {
                            x.Taskdate = x.TaskdateData.ToString("MMM dd yyyy");
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            TimeSpan timespan2 = x.TaskStarttime;
                            DateTime time1 = DateTime.Today.Add(timespan2);
                            string displayTime2 = time1.ToString("hh:mm tt");
                            x.Starttime = displayTime2;
                            TimeSpan timespan = x.TaskEndtime;
                            DateTime time = DateTime.Today.Add(timespan);
                            string displayTime = time.ToString("hh:mm tt");
                            x.Endtime = displayTime;
                            //x.Starttime = String.Format("{00:00}:{00:01}", x.TaskStarttime.Minutes / 60, x.TaskStarttime.Minutes % 60);
                            //x.Endtime = String.Format("{00:00}:{00:01}",x.TaskEndtime.Minutes / 60, x.TaskEndtime.Minutes % 60);

                        });
                        res.Message = "Client List";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = new
                        {
                            getall,
                        };
                        #endregion

                        #region
                        var data = getall;
                        switch (Model.DescOrder)
                        {
                            case true:
                                switch (Model.Filter)
                                {
                                    case FilterSortConstants.CreatedOn:
                                        data = getall.OrderBy(x => x.CreatedOn).ToList();
                                        break;
                                    case FilterSortConstants.Name:
                                        data = getall.OrderBy(x => x.ClientName).ToList();
                                        break;
                                    case FilterSortConstants.Date:
                                        data = getall.OrderBy(x => x.TaskdateData.Date).ToList();
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            default:
                                switch (Model.Filter)
                                {
                                    case FilterSortConstants.CreatedOn:
                                        data = getall.OrderByDescending(x => x.CreatedOn).ToList();
                                        break;
                                    case FilterSortConstants.Name:
                                        data = getall.OrderByDescending(x => x.ClientName).ToList();
                                        break;
                                    case FilterSortConstants.Date:
                                        data = getall.OrderByDescending(x => x.TaskdateData.Date).ToList();
                                        break;
                                    default:
                                        break;
                                }
                                break;
                        }
                        #endregion

                        #region predicate builder

                        if (data.Count > 0)
                        {
                            var predicate = PredicateBuilder.New<ClientTaskModelRequest>(x => x.IsActive && !x.IsDeleted);

                            if (Model.Employeid.Count > 0)
                            {
                                data = (data.Where(x => Model.Employeid.Contains(x.CreatedById))).ToList();
                            }
                            if (Model.ClientId.Count > 0)
                            {
                                data = (data.Where(x => Model.ClientId.Contains(x.ClientId))).ToList();
                            }
                            if (Model.Startdate != null)
                            {
                                //var startday = Model.Startdate.Value.AddDays(1);
                                //data = (data.Where(x => x.TaskdateData >= startday)).ToList();
                                data = (data.Where(x => x.TaskdateData >= Model.Startdate)).ToList();
                            }
                            if (Model.Enddate != null)
                            {
                                //var Endday = Model.Enddate.Value.AddDays(1);
                                //data = (data.Where(x => x.TaskdateData <= Endday)).ToList();
                                data = (data.Where(x => x.TaskdateData <= Model.Enddate)).ToList();
                            }
                            if (Model.dateId == 1)
                            {
                                var previousweek = DateTime.UtcNow.AddDays(-7);
                                data = (data.Where(x => x.TaskdateData >= previousweek)).ToList();
                            }
                            if (Model.dateId == 2)
                            {
                                var previousMonth = DateTime.UtcNow.AddMonths(-1);
                                data = (data.Where(x => x.TaskdateData >= previousMonth)).ToList();
                            }
                            if (Model.TaskStatus == (int)ClientTaskRequestConstants.Approved)
                            {
                                data = (data.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)).ToList();
                            }
                            if (Model.TaskStatus == (int)ClientTaskRequestConstants.Pending)
                            {
                                data = (data.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)).ToList();
                            }
                            if (Model.TaskStatus == (int)ClientTaskRequestConstants.Revaluation)
                            {
                                data = (data.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)).ToList();
                            }
                            if (Model.TaskStatus == (int)ClientTaskRequestConstants.New)
                            {
                                data = (data.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.New)).ToList();
                            }



                            if (tokenData.IsAdminInCompany)
                            {
                                res.Message = "Task list Found";
                                res.Status = true;
                                res.StatusCode = HttpStatusCode.Found;
                                foreach (var item in data)
                                {
                                    TotalHours += item.WorkingHours;

                                }
                                //  var time = Math.Round(TotalHours.TotalHours, 2);
                                var totalMinutes = TotalHours.TotalMinutes;
                                var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));


                                if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                                {
                                    var text = textInfo.ToUpper(Model.search);
                                    res.Data = new
                                    {
                                        Data = data./*Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).*/Count(),
                                        Count = (int)Model.count,
                                        List = data.Where(x => x.ClientName.ToUpper().Contains(text) //&& x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()
                                        ).Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                        //TottalHours  = String.Format("{00:00}:{01:00}", TotalHours.TotalMinutes / 60, TotalHours.TotalMinutes % 60),
                                        totalTimeString

                                    };
                                }
                                else if (Model.page > 0 && Model.count > 0)
                                {
                                    res.Data = new
                                    {
                                        Data = data/*.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())*/.Count(),
                                        Count = (int)Model.count,
                                        List = data//.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                                     .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                                else
                                {
                                    res.Data = new
                                    {
                                        List = data,
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                            }
                            else if (await _db.Employee.AnyAsync(x => x.ReportingManager == tokenData.employeeId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId &&
                            x.EmployeeTypeId != WebApi.Model.EnumClass.EmployeeTypeConstants.Ex_Employee))
                            {


                                res.Message = "Task list Found";
                                res.Status = true;
                                res.StatusCode = HttpStatusCode.Found;

                                var datalist = data.Where(x => x.ReportingManager == tokenData.employeeId).ToList();

                                foreach (var item in datalist)
                                {
                                    TotalHours += item.WorkingHours;
                                }

                                var totalMinutes = TotalHours.TotalMinutes;
                                var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));

                                if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                                {
                                    var text = textInfo.ToUpper(Model.search);

                                    res.Data = new
                                    {
                                        Data = data.Where(x => x.ReportingManager == tokenData.employeeId).Count(),
                                        Count = (int)Model.count,
                                        List = data.Where(x => x.ClientName.ToUpper().Contains(text) && x.ReportingManager == tokenData.employeeId //&& x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()
                                        ).Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                                else if (Model.page > 0 && Model.count > 0)
                                {
                                    res.Data = new
                                    {
                                        Data = data.Where(x => x.ReportingManager == tokenData.employeeId).Count(),
                                        Count = (int)Model.count,
                                        List = data.Where(x => x.ReportingManager == tokenData.employeeId)
                                        .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                                else
                                {
                                    res.Data = new
                                    {
                                        List = data.Where(x => x.ReportingManager == tokenData.employeeId),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                            }
                            else
                            {
                                res.Message = "Task list Found";
                                res.Status = true;
                                res.StatusCode = HttpStatusCode.Found;

                                var datalist = data.Where(x => x.CreatedById == tokenData.employeeId).ToList();
                                foreach (var item in datalist)
                                {
                                    TotalHours += item.WorkingHours;
                                }

                                var totalMinutes = TotalHours.TotalMinutes;
                                var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));

                                if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                                {
                                    var text = textInfo.ToUpper(Model.search);
                                    res.Data = new
                                    {
                                        Data = data.Where(x => x.CreatedById == tokenData.employeeId).Count(),
                                        PendingDataCount = (int)Model.count,
                                        List = data.Where(x => x.ClientName.ToUpper().Contains(text) && x.CreatedById == tokenData.employeeId
                                        ).Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                                else if (Model.page > 0 && Model.count > 0)
                                {
                                    res.Data = new
                                    {
                                        Data = data.Where(x => x.CreatedById == tokenData.employeeId).Count(),
                                        Count = (int)Model.count,
                                        List = data.Where(x => x.CreatedById == tokenData.employeeId)
                                        .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };

                                }
                                else
                                {
                                    res.Data = new
                                    {
                                        List = data.Where(x => x.CreatedById == tokenData.employeeId),
                                        //TottalHours = String.Format("{00:00}:{01:00}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                                        totalTimeString
                                    };
                                }
                            }
                        }
                        else
                        {
                            res.Message = "Task list not Found";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                            res.Data = new
                            {
                                List = data,
                            };
                        }
                    }
                    else
                    {
                        res.Message = "Task list not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = getall;
                    }

                    return Ok(res);
                    #endregion
                }
            }

            catch (Exception ex)
            {
                logger.Error("api/newclienttask/taskfilter", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Get Employee list data
        /// <summary>
        /// Created By  Suraj Bundel on 07/03/2023
        /// API => Get => api/newclienttask/employeelist
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [Route("employeelist")]
        [HttpGet]
        public async Task<IHttpActionResult> GetClientemployeeRequest()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                #region get all data
                var getemployeelist = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId).ToListAsync();

                var data = getemployeelist.Where(x => x.ReportingManager == tokendata.employeeId).ToList();
                if (tokendata.IsAdminInCompany)
                {
                    if (getemployeelist.Count > 0)
                    {
                        res.Message = "Employee list found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = getemployeelist;
                    }
                    else
                    {
                        res.Message = "Employee list not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = getemployeelist;
                    }
                }
                else if (data.Count > 0)
                {
                    if (data.Count > 0)
                    {
                        res.Message = "Employee list found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = data;
                    }
                    else
                    {
                        res.Message = "Employee list not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = data;
                    }
                }
                else
                {
                    var getlist = getemployeelist.Where(x => x.EmployeeId == tokendata.employeeId).ToList();
                    if (getlist.Count > 0)
                    {
                        res.Message = "Employee list found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = getlist;
                    }
                    else
                    {
                        res.Message = "Employee list not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = getlist;
                    }
                }
                return Ok(res);
            }
            #endregion

            catch (Exception ex)
            {
                logger.Error("api/newclienttask/employeelist", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }

        #endregion

        //#region comment it not in use
        //#region GetEmployee report data
        ///// <summary>
        ///// Created By  Suraj Bundel on 07/03/2023
        ///// API => Post => api/newclienttask/employereportfilter
        ///// </summary>
        ///// <param name="Model"></param>
        ///// <returns></returns>
        //[Route("employereportfilter")]
        //[HttpPost]
        //public async Task<IHttpActionResult> GetClientemployeeRequest(ClientemployeeRequest Model)
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    try
        //    {
        //        var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

        //        List<clientTaskModelRequest> list = new List<clientTaskModelRequest>();

        //        if (Model == null)
        //        {
        //            res.Message = "Model is Empty";
        //            res.Status = false;
        //            res.StatusCode = HttpStatusCode.NoContent;
        //            return Ok(res);
        //        }
        //        else
        //        {
        //            #region get all data

        //            var getall = await _db.clientTaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId)
        //                .Select(x => new clientTaskModelRequest
        //                {
        //                    ClientTaskId = x.ClientTaskId,
        //                    worktypename = _db.TypeofWorks.Where(y => y.WorktypeId == x.WorkTypeId).Select(y => y.WorktypeName).FirstOrDefault(),
        //                    ClientId = x.ClientId,
        //                    ClientName = _db.NewClientModels.Where(y => y.ClientId == x.ClientId).Select(y => y.ClientName).FirstOrDefault(),
        //                    Description = x.Description,
        //                    ClientCode = x.ClientCode,
        //                    Taskdate = x.Taskdate,
        //                    TaskStarttime = x.StartTime,
        //                    TaskEndtime = x.EndTime,
        //                    Starttime = null,
        //                    Endtime = null,
        //                    WorkingHours = TimeSpan.Zero,
        //                    IsActive = x.IsActive,
        //                    IsDeleted = x.IsDeleted,
        //                    CompanyId = x.CompanyId,
        //                    OrgId = x.OrgId,
        //                    IsDefaultCreated = x.IsDefaultCreated,
        //                    CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == x.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
        //                    UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == x.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
        //                    CreatedOn = x.CreatedOn,
        //                    UpdatedOn = x.UpdatedOn,
        //                    CreatedById = x.CreatedBy,
        //                    UpdatedById = x.UpdatedBy,
        //                }).OrderBy(x => x.CreatedOn).ToListAsync();

        //            if (getall.Count > 0)
        //            {
        //                getall.ForEach(x =>
        //                {
        //                    x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
        //                    x.Starttime = x.TaskStarttime.ToString("hh:mm:ss tt");
        //                    x.Endtime = x.TaskEndtime.ToString("hh:mm:ss tt");
        //                });
        //                res.Message = "Client List";
        //                res.Status = true;
        //                res.StatusCode = HttpStatusCode.OK;
        //                res.Data = getall;
        //            }
        //            else
        //            {
        //                res.Message = "Client list not found !";
        //                res.Status = false;
        //                res.StatusCode = HttpStatusCode.NoContent;
        //                res.Data = getall;
        //            }
        //            #endregion

        //            if (tokendata.IsAdminInCompany == true)
        //            {
        //                if (Model.Employeid.Count > 0 || Model.ClientId.Count > 0)
        //                {
        //                    if (Model.Startdate == null && Model.Enddate == null)
        //                    {
        //                        res.Message = "Select both startdate and enddate ";
        //                        res.Status = false;
        //                        res.StatusCode = HttpStatusCode.NoContent;
        //                        //res.Data = clientdata;
        //                    }
        //                    else
        //                    {
        //                        var TaskStartDate = Model.Startdate;
        //                        var TaskEndDate = Model.Enddate; // take today date
        //                        if (Model.ClientId.Count > 0)
        //                        {
        //                            foreach (var item in Model.ClientId)
        //                            {
        //                                if (item == Guid.Empty)
        //                                {
        //                                    var DateRangedata = getall.Where(x => Model.ClientId.Contains(x.ClientId) && Model.Employeid.Contains(x.CreatedById) && x.Taskdate >= TaskStartDate && x.Taskdate <= TaskEndDate);
        //                                    if (DateRangedata == null)
        //                                    {
        //                                        res.Message = "Client list not found !";
        //                                        res.Status = false;
        //                                        res.StatusCode = HttpStatusCode.NoContent;
        //                                        res.Data = DateRangedata;
        //                                    }
        //                                    else
        //                                    {
        //                                        res.Message = "Client list found !";
        //                                        res.Status = true;
        //                                        res.StatusCode = HttpStatusCode.OK;
        //                                        res.Data = DateRangedata;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    var DaterangeClientdata = getall.Where(x => x.ClientId == item && x.Taskdate >= TaskStartDate && x.Taskdate <= TaskEndDate);
        //                                    if (DaterangeClientdata == null)
        //                                    {
        //                                        res.Message = "Client list not found !";
        //                                        res.Status = false;
        //                                        res.StatusCode = HttpStatusCode.NoContent;
        //                                        res.Data = getall;
        //                                    }
        //                                    else
        //                                    {
        //                                        res.Message = "Client list found !";
        //                                        res.Status = true;
        //                                        res.StatusCode = HttpStatusCode.OK;
        //                                        res.Data = DaterangeClientdata;
        //                                    }
        //                                    list.AddRange(DaterangeClientdata);
        //                                }
        //                            }
        //                            return Ok(list);
        //                        }
        //                        else
        //                        {
        //                            res.Message = "Client list not found !";
        //                            res.Status = false;
        //                            res.StatusCode = HttpStatusCode.NoContent;
        //                            // res.Data = Alldata;
        //                        }
        //                        #endregion


        //                    }
        //                    switch (Model.dateId)
        //                    {
        //                        case 0:
        //                            break;
        //                    }
        //                    var clientdata = getall.Where(x => Model.ClientId.Contains(x.ClientId) && Model.Employeid.Contains(x.CreatedById)).ToList();
        //                    if (clientdata.Count > 0)
        //                    {
        //                        res.Message = "Client list found !";
        //                        res.Status = true;
        //                        res.StatusCode = HttpStatusCode.OK;
        //                        res.Data = clientdata;
        //                    }
        //                    else
        //                    {
        //                        res.Message = "Client list not found !";
        //                        res.Status = false;
        //                        res.StatusCode = HttpStatusCode.NoContent;
        //                        res.Data = clientdata;
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                if (Model.Employeid.Count > 0)
        //                {
        //                    var clientdata = getall.Where(x => Model.ClientId.Contains(x.ClientId) && x.CreatedById == tokendata.employeeId).ToList();
        //                    if (clientdata.Count > 0)
        //                    {
        //                        res.Message = "Client list found !";
        //                        res.Status = true;
        //                        res.StatusCode = HttpStatusCode.OK;
        //                        res.Data = clientdata;
        //                    }
        //                    else
        //                    {
        //                        res.Message = "Client list not found !";
        //                        res.Status = false;
        //                        res.StatusCode = HttpStatusCode.NoContent;
        //                        res.Data = clientdata;
        //                    }
        //                }
        //                //res.Message = "Client list found !";
        //                //res.Status = true;
        //                //res.StatusCode = HttpStatusCode.OK;
        //                //res.Data = getall;
        //            }


        //            //switch (Model.dateId)
        //            //{
        //            //    #region For All Data

        //            //    case 0:

        //            //        if (Model.ClientId.Count > 0)
        //            //        {
        //            //            foreach (var item in Model.ClientId)
        //            //            { 
        //            //                if (item == Guid.Empty)
        //            //                {
        //            //                    res.Message = "Client list not found !";
        //            //                    res.Status = true;
        //            //                    res.StatusCode = HttpStatusCode.OK;
        //            //                    res.Data = getall;
        //            //                }
        //            //                else
        //            //                {
        //            //                    var Alldata = getall.Where(x => x.ClientId == item).ToList();
        //            //                    if (Alldata.Count == 0)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = Alldata;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = Alldata;
        //            //                    }
        //            //                    list.AddRange(Alldata);
        //            //                }
        //            //            }
        //            //            return Ok(list);
        //            //        }
        //            //        else
        //            //        {
        //            //            res.Message = "Client list not found !";
        //            //            res.Status = false;
        //            //            res.StatusCode = HttpStatusCode.NoContent;
        //            //            // res.Data = Alldata;
        //            //        }
        //            //        break;
        //            //    #endregion

        //            //    #region For Weekly Data
        //            //    case 1:
        //            //        var week = DateTime.Now.AddDays(-7); 
        //            //        if (Model.ClientId.Count > 0)
        //            //        {
        //            //            foreach (var item in Model.ClientId)
        //            //            {
        //            //                if (item == Guid.Empty)
        //            //                {
        //            //                    var weekdata = getall.Where(x => x.Taskdate >= week).ToList();
        //            //                    if (weekdata.Count < 0)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = weekdata;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = weekdata;
        //            //                    }
        //            //                }
        //            //                else
        //            //                {
        //            //                    var WeekClientdata = getall.Where(x => x.ClientId == item && x.CreatedOn >= week).ToList();
        //            //                    if (WeekClientdata.Count < 0)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = getall;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = WeekClientdata;
        //            //                    }
        //            //                    list.AddRange(WeekClientdata);
        //            //                }
        //            //            }
        //            //            return Ok(list);
        //            //        }
        //            //        else
        //            //        {
        //            //            res.Message = "Client list not found !";
        //            //            res.Status = false;
        //            //            res.StatusCode = HttpStatusCode.NoContent;
        //            //            // res.Data = Alldata;
        //            //        }
        //            //        break;
        //            //    #endregion

        //            //    #region For Monthly Data
        //            //    case 2:
        //            //        var Month = DateTime.Now.AddMonths(-1); 
        //            //        if (Model.ClientId.Count > 0)
        //            //        {
        //            //            foreach (var item in Model.ClientId)
        //            //            { 
        //            //                if (item == Guid.Empty)
        //            //                {
        //            //                    var Monthdata = getall.Where(x => x.CreatedOn >= Month);
        //            //                    if (Monthdata == null)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = Monthdata;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = Monthdata;
        //            //                    }
        //            //                }
        //            //                else
        //            //                {
        //            //                    var MonthClientdata = getall.Where(x => x.ClientId == item && x.CreatedOn >= Month);
        //            //                    if (MonthClientdata == null)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = getall;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = MonthClientdata;
        //            //                    }
        //            //                    list.AddRange(MonthClientdata);
        //            //                }
        //            //            }
        //            //            return Ok(list);
        //            //        }
        //            //        else
        //            //        {
        //            //            res.Message = "Client list not found !";
        //            //            res.Status = false;
        //            //            res.StatusCode = HttpStatusCode.NoContent;
        //            //            // res.Data = Alldata;
        //            //        }
        //            //        break;
        //            //    #endregion

        //            //    #region For Date range Data
        //            //    case 3:
        //            //        var TaskStartDate = Model.Startdate;
        //            //        var TaskEndDate = Model.Enddate;
        //            //        if (Model.ClientId.Count > 0)
        //            //        {
        //            //            foreach (var item in Model.ClientId)
        //            //            { 
        //            //                if (item == Guid.Empty)
        //            //                {
        //            //                    var DateRangedata = getall.Where(x => x.Taskdate >= TaskStartDate && x.Taskdate <= TaskEndDate);
        //            //                    if (DateRangedata == null)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = DateRangedata;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = DateRangedata;
        //            //                    }
        //            //                }
        //            //                else
        //            //                {
        //            //                    var DaterangeClientdata = getall.Where(x => x.ClientId == item && x.Taskdate >= TaskStartDate && x.Taskdate <= TaskEndDate);
        //            //                    if (DaterangeClientdata == null)
        //            //                    {
        //            //                        res.Message = "Client list not found !";
        //            //                        res.Status = false;
        //            //                        res.StatusCode = HttpStatusCode.NoContent;
        //            //                        res.Data = getall;
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        res.Message = "Client list found !";
        //            //                        res.Status = true;
        //            //                        res.StatusCode = HttpStatusCode.OK;
        //            //                        res.Data = DaterangeClientdata;
        //            //                    }
        //            //                    list.AddRange(DaterangeClientdata);
        //            //                }
        //            //            }
        //            //            return Ok(list);
        //            //        }
        //            //        else
        //            //        {
        //            //            res.Message = "Client list not found !";
        //            //            res.Status = false;
        //            //            res.StatusCode = HttpStatusCode.NoContent;
        //            //            // res.Data = Alldata;
        //            //        }
        //            //        break;
        //            //        #endregion
        //            //}

        //            return Ok(res);
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        logger.Error("api/newclienttask/employereportfilter", ex.Message);
        //        res.Message = ex.Message;
        //        res.Status = false;
        //        res.StatusCode = HttpStatusCode.BadRequest;
        //        return BadRequest("Failed");
        //    }
        //}

        //#endregion

        #region Date Filter Enum

        /// <summary>
        /// Create by Suraj Bundel On 06-03-2023
        /// API >>  Get >> api/newclienttask/datefilterenum
        /// </summary>
        /// <returns></returns>
        [Route("datefilterenum")]
        [HttpGet]
        public ResponseBodyModel GetDateFilterEnum()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokedata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var condition = Enum.GetValues(typeof(TimeFilterConstant))
                    .Cast<TimeFilterConstant>()
                    .Select(x => new DateFilterConstant
                    {
                        Id = (int)x,
                        Name = Enum.GetName(typeof(TimeFilterConstant), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Assets Condition";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = condition;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = HttpStatusCode.NoContent;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region get total working hours  task filter for PM
        /// <summary>
        /// Created by Suraj Bundel on 06/03/2023
        /// API => Post => api/newclienttask/taskfilterforpm
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [Route("taskfilterforpm")]
        [HttpPost]
        public async Task<IHttpActionResult> GetWorkingHoursforpm(GetWorkingHoursRequest Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                TimeSpan TotalHours = TimeSpan.Zero;

                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                List<ClientTaskModelRequest> list = new List<ClientTaskModelRequest>();

                if (Model == null)
                {
                    res.Message = "Model is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    #region get all data

                    var getemployeelist = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId).ToListAsync();

                    var data = getemployeelist.Where(x => x.ReportingManager == tokendata.employeeId).FirstOrDefault();

                    var getalldata = await (from c in _db.clientTaskModels
                                            join e in _db.Employee on c.CreatedBy equals e.EmployeeId
                                            join a in _db.ClientTaskApprovals on c.ClientTaskId equals a.ClientTaskId into gj
                                            from sub in gj.DefaultIfEmpty()
                                            where c.IsActive && !c.IsDeleted && c.CompanyId == tokendata.companyId
                                            select new ClientTaskModelRequest
                                            {
                                                ClientTaskId = c.ClientTaskId,
                                                WorkTypeId = c.WorkTypeId,
                                                Worktypename = _db.TypeofWorks.Where(y => y.WorktypeId == c.WorkTypeId).Select(y => y.WorktypeName).FirstOrDefault(),
                                                ClientId = c.ClientId,
                                                ClientName = _db.NewClientModels.Where(y => y.ClientId == c.ClientId).Select(y => y.ClientName).FirstOrDefault(),
                                                IsSFA = sub == null ? false : sub.IsSFA,
                                                Description = c.Description,
                                                ClientCode = c.ClientCode,
                                                Taskdate = null,
                                                TaskdateData = c.TaskDate,
                                                TaskStarttime = c.StartTime,
                                                TaskRequest = sub == null ? ClientTaskRequestConstants.New.ToString().Replace("_", " ") : sub.TaskRequest.ToString().Replace("_", " "),
                                                TaskEndtime = c.EndTime,
                                                //Starttime = TimeSpan.Zero.ToString(@"hh\:mm\tt"),
                                                //Endtime = TimeSpan.Zero.ToString(@"hh\:mm\tt"),
                                                WorkingHours = TimeSpan.Zero,
                                                IsActive = c.IsActive,
                                                IsDeleted = c.IsDeleted,
                                                CompanyId = c.CompanyId,
                                                OrgId = c.OrgId,
                                                IsDefaultCreated = c.IsDefaultCreated,
                                                CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == c.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                                UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == c.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                                CreatedOn = c.CreatedOn,
                                                UpdatedOn = c.UpdatedOn,
                                                CreatedById = c.CreatedBy,
                                                UpdatedById = c.UpdatedBy,
                                                TaskRequestid = sub == null ? (int)ClientTaskRequestConstants.New : (int)sub.TaskRequest,
                                                ReportingManager = e.ReportingManager == 0 ? 0 : e.ReportingManager,
                                                TotalWorkingHours = "",
                                            }).ToListAsync();

                    var getdata = getalldata
                        .Select(go => new ClientTaskModelRequest
                        {
                            ClientTaskId = go.ClientTaskId,
                            ClientName = go.ClientName,
                            IsSFA = go.IsSFA,
                            ClientCode = go.ClientCode,
                            CompanyId = go.CompanyId,
                            ClientId = go.ClientId,
                            CreatedBy = go.CreatedBy,
                            CreatedOn = go.CreatedOn,
                            UpdatedOn = go.UpdatedOn,
                            CreatedById = go.CreatedById,
                            Description = go.Description,
                            Endtime = go.Endtime,
                            TotalWorkingHours = String.Format("{00:00}:{00:01}", TotalHours.Minutes / 60, TotalHours.Minutes % 60),
                            ReportingManager = go.ReportingManager,
                            TaskRequestid = go.TaskRequestid,
                            UpdatedById = go.UpdatedById,
                            UpdatedBy = go.UpdatedBy,
                            IsDefaultCreated = go.IsDefaultCreated,
                            OrgId = go.OrgId,
                            IsDeleted = go.IsDeleted,
                            IsActive = go.IsActive,
                            WorkingHours = go.WorkingHours,
                            Starttime = go.Starttime,
                            TaskEndtime = go.TaskEndtime,
                            TaskRequest = go.TaskRequest,
                            TaskStarttime = go.TaskStarttime,
                            TaskdateData = go.TaskdateData,
                            Taskdate = go.Taskdate,
                            Worktypename = go.Worktypename,
                        }).OrderBy(go => go.ClientName).ToList();
                    
                    if (getdata.Count > 0)
                    {
                        
                        getdata.ForEach(x =>
                        {

                            x.Taskdate = x.TaskdateData.ToString("MMM dd yyyy");
                            x.WorkingHours = x.TaskEndtime.Subtract(x.TaskStarttime);
                            TimeSpan timespan2 = x.TaskStarttime;
                            DateTime time1 = DateTime.Today.Add(timespan2);
                            string displayTime2 = time1.ToString("hh:mm tt");
                            x.Starttime = displayTime2;
                            TimeSpan timespan = x.TaskEndtime;
                            DateTime time = DateTime.Today.Add(timespan);
                            string displayTime = time.ToString("hh:mm tt");
                            x.Endtime = displayTime;
                        });
                        res.Message = "Client List";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = getdata;
                        #endregion

                        var getall = getdata.OrderBy(x => x.ClientName).ToList();
                        switch (Model.DescOrder)
                        {
                            case true:
                                switch (Model.Filter)
                                {
                                    case FilterSortConstants.CreatedOn:
                                        getall = getall.OrderByDescending(x => x.CreatedOn).ToList();
                                        break;
                                    case FilterSortConstants.Name:
                                        getall = getall.OrderByDescending(x => x.ClientName).ToList();
                                        break;
                                    //case FilterSortConstants.Date:
                                    //    getall = getdata.OrderByDescending(x => x.Taskdate.Date).ToList();
                                    //    break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                switch (Model.Filter)
                                {
                                    case FilterSortConstants.CreatedOn:
                                        getall = getall.OrderBy(x => x.CreatedOn).ToList();
                                        break;
                                    case FilterSortConstants.Name:
                                        getall = getall.OrderBy(x => x.ClientName).ToList();
                                        break;
                                    //case FilterSortConstants.Date:
                                    //    getall = getdata.OrderBy(x => x.Taskdate.Date).ToList();
                                    //    break;
                                    default:
                                        break;
                                }
                                break;
                        }

                        #region predicate builder

                        if (getall.Count > 0)
                        {
                            var predicate = PredicateBuilder.New<ClientTaskModelRequest>(x => x.IsActive && !x.IsDeleted);
                            if (tokendata.IsAdminInCompany)
                            {
                                if (Model.Employeid.Count > 0)
                                {
                                    getall = (getall.Where(x => Model.Employeid.Contains(x.CreatedById))).ToList();
                                }
                                if (Model.ClientId.Count > 0)
                                {
                                    getall = (getall.Where(x => Model.ClientId.Contains(x.ClientId))).ToList();
                                }
                                if (Model.Startdate != null)
                                {
                                    getall = (getall.Where(x => x.TaskdateData >= Model.Startdate)).ToList();
                                }
                                if (Model.Enddate != null)
                                {
                                    getall = (getall.Where(x => x.TaskdateData <= Model.Enddate)).ToList();
                                }
                                if (Model.dateId == 1)
                                {
                                    var previousweek = DateTime.UtcNow.AddDays(-7);
                                    getall = (getall.Where(x => x.TaskdateData >= previousweek)).ToList();
                                }
                                if (Model.dateId == 2)
                                {
                                    var previousMonth = DateTime.UtcNow.AddMonths(-1);
                                    getall = (getall.Where(x => x.TaskdateData >= previousMonth)).ToList();
                                }
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Approved)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Pending)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Revaluation)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.New)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.New)).ToList();
                                //}

                                if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                                {
                                    var text = textInfo.ToUpper(Model.Pendingsearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }

                                    //  var time = Math.Round(TotalHours.TotalHours, 2);
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));

                                    res.Data = new
                                    {
                                        TotalPendingData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending).Count(),
                                        PendingDataCount = (int)Model.Pendingcount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)
                                                               .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }

                                    //  var time = Math.Round(TotalHours.TotalHours, 2);
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalPendingData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending).Count(),
                                        PendingDataCount = (int)Model.Pendingcount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)
                                        .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                                {
                                    var text = textInfo.ToUpper(Model.Approvesearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)).ToList();

                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalApproveData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved).Count(),
                                        ApproveDataCount = (int)Model.Approvecount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)
                                                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalApproveData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved).Count(),
                                        ApproveDataCount = (int)Model.Approvecount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)
                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                                {

                                    var text = textInfo.ToUpper(Model.Revaluationsearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalRevalData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation).Count(),
                                        RevalDataCount = (int)Model.Revaluationcount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)
                                                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalRevalData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation).Count(),
                                        RevalDataCount = (int)Model.Revaluationcount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)
                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                                        totalTimeString

                                    };
                                }


                                else
                                {
                                    res.Message = "Task list Not Found";
                                    res.Status = false;
                                    res.StatusCode = HttpStatusCode.NoContent;
                                    res.Data = new
                                    {
                                        Pendingtaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending).Count(),
                                        Approvetaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved).Count(),
                                        Revaltaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation).Count(),
                                    };
                                }



                                //res.Message = "Task list Found";
                                //res.Status = true;
                                //res.StatusCode = HttpStatusCode.Found;
                                //if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                                //{
                                //    var text = textInfo.ToUpper(Model.search);
                                //    res.Data = new
                                //    {
                                //        TotalPendingData = getall.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                                //        PendingDataCount = (int)Model.count,
                                //        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                //                               .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                //    };
                                //}
                                //else if (Model.page > 0 && Model.count > 0)
                                //{
                                //    res.Data = new
                                //    {
                                //        TotalPendingData = getall.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()).Count(),
                                //        PendingDataCount = (int)Model.count,
                                //        List = getall.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                //        .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),

                                //    };
                                //}
                            }

                            else if (data != null)
                            {
                                if (Model.Employeid.Count > 0)
                                {
                                    getall = (getall.Where(x => Model.Employeid.Contains(x.CreatedById) && x.ReportingManager == tokendata.employeeId)).ToList();
                                }

                                if (Model.ClientId.Count > 0)
                                {
                                    getall = (getall.Where(x => Model.ClientId.Contains(x.ClientId) && x.ReportingManager == tokendata.employeeId)).ToList();
                                }

                                if (Model.Startdate != null)
                                {
                                    getall = (getall.Where(x => x.TaskdateData >= Model.Startdate && x.ReportingManager == tokendata.employeeId)).ToList();
                                }

                                if (Model.Enddate != null)
                                {
                                    getall = (getall.Where(x => x.TaskdateData <= Model.Enddate && x.ReportingManager == tokendata.employeeId)).ToList();
                                }

                                if (Model.dateId == 1)
                                {
                                    var previousweek = DateTime.UtcNow.AddDays(-7);
                                    getall = (getall.Where(x => x.TaskdateData >= previousweek && x.ReportingManager == tokendata.employeeId)).ToList();
                                }

                                if (Model.dateId == 2)
                                {
                                    var previousMonth = DateTime.UtcNow.AddMonths(-1);
                                    getall = (getall.Where(x => x.TaskdateData >= previousMonth && x.ReportingManager == tokendata.employeeId)).ToList();
                                }

                                if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                                {
                                    var text = textInfo.ToUpper(Model.Pendingsearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;

                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalPendingData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId).Count(),
                                        PendingDataCount = (int)Model.Pendingcount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId)
                                                               .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalPendingData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId).Count(),
                                        PendingDataCount = (int)Model.Pendingcount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId)
                                        .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                                {
                                    var text = textInfo.ToUpper(Model.Approvesearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalApproveData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId).Count(),
                                        ApproveDataCount = (int)Model.Approvecount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId)
                                                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;

                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));

                                    res.Data = new
                                    {
                                        TotalApproveData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId).Count(),
                                        ApproveDataCount = (int)Model.Approvecount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId)
                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                                {
                                    var text = textInfo.ToUpper(Model.Revaluationsearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;

                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));

                                    res.Data = new
                                    {
                                        TotalRevalData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId).Count(),
                                        RevalDataCount = (int)Model.Revaluationcount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId)
                                                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;

                                    res.StatusCode = HttpStatusCode.OK;

                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalRevalData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId).Count(),
                                        RevalDataCount = (int)Model.Revaluationcount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId)
                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else
                                {
                                    res.Message = "Task list Not Found";
                                    res.Status = false;
                                    res.StatusCode = HttpStatusCode.NoContent;
                                    res.Data = new
                                    {
                                        Pendingtaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.ReportingManager == tokendata.employeeId).Count(),
                                        Approvetaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.ReportingManager == tokendata.employeeId).Count(),
                                        Revaltaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.ReportingManager == tokendata.employeeId).Count(),
                                    };
                                }

                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.New)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.New)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Approved)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Pending)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Revaluation)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation)).ToList();
                                //}
                                //res.Message = "Task list Found";
                                //res.Status = true;
                                //res.StatusCode = HttpStatusCode.Found;
                                //if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                                //{
                                //    var text = textInfo.ToUpper(Model.search);
                                //    res.Data = new
                                //    {
                                //        TotalPendingData = getall.Count(),
                                //        PendingDataCount = (int)Model.count,
                                //        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) //&& x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()
                                //        ).Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                //    };
                                //}
                                //else if (Model.page > 0 && Model.count > 0)
                                //{
                                //    res.Data = new
                                //    {
                                //        Data = getall.Count(),
                                //        Count = (int)Model.count,
                                //        List = getall//.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                //        .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                //    };
                                //}
                                //else
                                //{
                                //    res.Message = "Task list Not Found";
                                //    res.Status = false;
                                //    res.StatusCode = HttpStatusCode.NoContent;
                                //    res.Data = new
                                //    {
                                //        task = getall.Count(),
                                //    };
                                //}
                            }
                            else
                            {
                                if (Model.Employeid.Count > 0)
                                {
                                    getall = (getall.Where(x => x.CreatedById == tokendata.employeeId)).ToList();
                                }
                                if (Model.ClientId.Count > 0)
                                {
                                    getall = (getall.Where(x => Model.ClientId.Contains(x.ClientId) && x.CreatedById == tokendata.employeeId)).ToList();
                                }
                                if (Model.Startdate != null)
                                {
                                    getall = (getall.Where(x => x.TaskdateData >= Model.Startdate && x.CreatedById == tokendata.employeeId)).ToList();
                                }
                                if (Model.Enddate != null)
                                {
                                    getall = (getall.Where(x => x.TaskdateData <= Model.Enddate && x.CreatedById == tokendata.employeeId)).ToList();
                                }
                                if (Model.dateId == 1)
                                {
                                    var previousweek = DateTime.UtcNow.AddDays(-7);
                                    getall = (getall.Where(x => x.TaskdateData >= previousweek && x.CreatedById == tokendata.employeeId)).ToList();
                                }
                                if (Model.dateId == 2)
                                {
                                    var previousMonth = DateTime.UtcNow.AddMonths(-1);
                                    getall = (getall.Where(x => x.TaskdateData >= previousMonth && x.CreatedById == tokendata.employeeId)).ToList();
                                }
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.New)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.New && x.CreatedById == tokendata.employeeId)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Approved)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Pending)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId)).ToList();
                                //}
                                //if (Model.TaskStatus == (int)ClientTaskRequestConstants.Revaluation)
                                //{
                                //    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId)).ToList();
                                //}
                                //res.Message = "Task list Found";
                                //res.Status = true;
                                //res.StatusCode = HttpStatusCode.Found;
                                //if (Model.page.HasValue && Model.count.HasValue && !string.IsNullOrEmpty(Model.search))
                                //{
                                //    var text = textInfo.ToUpper(Model.search);
                                //    res.Data = new
                                //    {
                                //        TotalPendingData = getall.Count(),
                                //        PendingDataCount = (int)Model.count,
                                //        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) //&& x.TaskRequest == ClientTaskRequestConstants.Pending.ToString()
                                //        ).Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),
                                //    };
                                //}
                                //else if (Model.page > 0 && Model.count > 0)
                                //{
                                //    res.Data = new
                                //    {
                                //        Data = getall.Count(),
                                //        Count = (int)Model.count,
                                //        List = getall//.Where(x => x.TaskRequest == ClientTaskRequestConstants.Pending.ToString())
                                //        .Skip(((int)Model.page - 1) * (int)Model.count).Take((int)Model.count).ToList(),

                                //    };

                                //}
                                //else
                                //{
                                //    res.Message = "Task list Not Found";
                                //    res.Status = false;
                                //    res.StatusCode = HttpStatusCode.NoContent;
                                //    res.Data = new
                                //    {
                                //        task = getall.Count(),
                                //    };
                                //}
                                if (Model.Pendingpage.HasValue && Model.Pendingcount.HasValue && !string.IsNullOrEmpty(Model.Pendingsearch))
                                {
                                    var text = textInfo.ToUpper(Model.Pendingsearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;

                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalPendingData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId).Count(),
                                        PendingDataCount = (int)Model.Pendingcount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId)
                                                               .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else if (Model.Pendingpage > 0 && Model.Pendingcount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalPendingData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId).Count(),
                                        PendingDataCount = (int)Model.Pendingcount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId)
                                        .Skip(((int)Model.Pendingpage - 1) * (int)Model.Pendingcount).Take((int)Model.Pendingcount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Approvepage.HasValue && Model.Approvecount.HasValue && !string.IsNullOrEmpty(Model.Approvesearch))
                                {
                                    var text = textInfo.ToUpper(Model.Approvesearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalApproveData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId).Count(),
                                        ApproveDataCount = (int)Model.Approvecount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId)
                                                               .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Approvepage > 0 && Model.Approvecount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalApproveData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId).Count(),
                                        ApproveDataCount = (int)Model.Approvecount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId)
                                       .Skip(((int)Model.Approvepage - 1) * (int)Model.Approvecount).Take((int)Model.Approvecount).ToList(),
                                        totalTimeString

                                    };
                                }

                                else if (Model.Revaluationpage.HasValue && Model.Revaluationcount.HasValue && !string.IsNullOrEmpty(Model.Revaluationsearch))
                                {
                                    var text = textInfo.ToUpper(Model.Revaluationsearch);
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalRevalData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId).Count(),
                                        RevalDataCount = (int)Model.Revaluationcount,
                                        List = getall.Where(x => x.ClientName.ToUpper().Contains(text) && x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId)
                                                               .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else if (Model.Revaluationpage > 0 && Model.Revaluationcount > 0)
                                {
                                    res.Message = "Task list  Found";
                                    res.Status = true;
                                    res.StatusCode = HttpStatusCode.OK;
                                    getall = (getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId)).ToList();
                                    foreach (var item in getall)
                                    {
                                        TotalHours += item.WorkingHours;

                                    }
                                    var totalMinutes = TotalHours.TotalMinutes;
                                    var totalTimeString = String.Format("{00:00}:{01:00}", (totalMinutes / 60), (totalMinutes % 60));
                                    res.Data = new
                                    {
                                        TotalRevalData = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId).Count(),
                                        RevalDataCount = (int)Model.Revaluationcount,
                                        List = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId)
                                       .Skip(((int)Model.Revaluationpage - 1) * (int)Model.Revaluationcount).Take((int)Model.Revaluationcount).ToList(),
                                        totalTimeString
                                    };
                                }

                                else
                                {
                                    res.Message = "Task list Not Found";
                                    res.Status = false;
                                    res.StatusCode = HttpStatusCode.NoContent;
                                    res.Data = new
                                    {
                                        Pendingtaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Pending && x.CreatedById == tokendata.employeeId).Count(),
                                        Approvetaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Approved && x.CreatedById == tokendata.employeeId).Count(),
                                        Revaltaskdata = getall.Where(x => x.TaskRequestid == (int)ClientTaskRequestConstants.Revaluation && x.CreatedById == tokendata.employeeId).Count(),
                                    };
                                }

                            }
                        }
                        else
                        {
                            res.Message = "Task list Not Found";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                            res.Data = getall;
                        }
                    }
                    return Ok(res);
                }
            }
            #endregion

            catch (Exception ex)
            {
                logger.Error("api/newclienttask/taskfilterforpm", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest("Failed");
            }
        }
        #endregion

        //#region Get data orderby 

        ///// <summary>
        ///// Create by Suraj Bundel On 20-03-2023
        ///// API >>  Post >> api/newclienttask/gettaskstatusenum
        ///// </summary>
        ///// <returns></returns>
        //[Route("getdataorderby")]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetDataOrderBy()
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var condition = Enum.GetValues(typeof(OrderbyRequestConstants))
        //            .Cast<OrderbyRequestConstants>()
        //            .Select(x => new GetdataOrderbystatus
        //            {
        //                Id = (int)x,
        //                Name = Enum.GetName(typeof(OrderbyRequestConstants), x)
        //            }).ToList();
        //        res.Message = "Task Order !";
        //        res.Status = true;
        //        res.Data = condition;
        //        res.StatusCode = HttpStatusCode.OK;
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {

        //        logger.Error("api/newclienttask/getdataorderby", ex.Message);
        //        res.Message = ex.Message;
        //        res.Status = false;
        //        res.StatusCode = HttpStatusCode.BadRequest;
        //        return BadRequest("Failed");
        //    }

        //}

        //#endregion Used for Assets Condition Api

        public class ClientemployeeRequest
        {
            public List<Guid> ClientId { get; set; }
            public List<int> Employeid { get; set; }
            public DateTimeOffset? Startdate { get; set; }
            public DateTimeOffset? Enddate { get; set; }
            public int dateId { get; set; }
        }

        public class DateFilterConstant
        {
            public int Id { get; set; }
            public string Name { get; set; }

        }

        public enum TimeFilterConstant
        {
            All = 0,
            Previous_Weekly = 1,
            Previous_Month = 2,
            Date_range = 3,
        }
        public class ClientTaskModelRequest
        {

            public Guid ClientTaskId { get; set; }
            public Guid ClientId { get; set; }
            public string ClientCode { get; set; }
            public string Taskdate { get; set; }
            public DateTimeOffset TaskdateData { get; set; }
            public Guid WorkTypeId { get; set; }
            public string Description { get; set; }
            public string Worktypename { get; set; }
            public string ClientName { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
            public string TaskRequest { get; set; }
            public int TaskRequestid { get; set; }
            public int CreatedById { get; set; }
            public int? UpdatedById { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsSFA { get; set; }
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
            public int? ReportingManager { get; set; }
            public bool IsApproved { get; set; }
            public string TotalWorkingHours { get; set; }

        }

        public class GetWorkingHoursRequest
        {
            public List<Guid> ClientId { get; set; }// = new List<Guid> { Guid.Empty };
            public DateTimeOffset? Startdate { get; set; }
            public DateTimeOffset? Enddate { get; set; }
            public List<int> Employeid { get; set; } = new List<int>();
            public int dateId { get; set; }
            public int TaskStatus { get; set; }
            public int? page { get; set; } = 0;
            public int? count { get; set; } = 0;
            public string search { get; set; } = string.Empty;

            public int? Pendingpage { get; set; } = 0;
            public int? Pendingcount { get; set; } = 0;
            public string Pendingsearch { get; set; } = string.Empty;

            public int? Approvepage { get; set; } = 0;
            public int? Approvecount { get; set; } = 0;
            public string Approvesearch { get; set; } = string.Empty;

            public int? Revaluationpage { get; set; } = 0;
            public int? Revaluationcount { get; set; } = 0;
            public string Revaluationsearch { get; set; } = string.Empty;

            public FilterSortConstants Filter { get; set; } = FilterSortConstants.CreatedOn;
            public bool DescOrder { get; set; } = false;

        }

        public class updateclientTaskRequest : AddclientTaskRequest
        {
            public Guid Id { get; set; }
        }

        #region Add & Update Client task By Excel upload // remove from frontend

        ///// <summary>
        ///// Created By Suraj Bundel on 03-03-2023
        ///// API >> POST >> api/newclienttask/clienttaskimport
        ///// </summary>
        ///// <param name="models"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("clienttaskimport")]
        //public async Task<IHttpActionResult> ClientImport(List<clientTaskImportFaultyLog> models)
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    List<clientTaskImportFaultyLog> falultyImportItem = new List<clientTaskImportFaultyLog>();
        //    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //    long successfullImported = 0;
        //    var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        if (models.Count <= 0)
        //        {
        //            res.Message = "Excel Not Have Any Data";
        //            res.Status = false;
        //            res.StatusCode = HttpStatusCode.NoContent;
        //            res.Data = falultyImportItem;
        //            return Ok(res);
        //        }
        //        else
        //        {
        //            var clientname = _db.NewClientModels.Where(x => x.IsActive/*ClientId== models[0].ClientId */).Select(x => x.ClientName).ToList();
        //            foreach (var item in models)
        //            {

        //                var check = await _db.NewClientModels.FirstOrDefaultAsync(x => x.CompanyId == tokendata.companyId && x.ClientCode.ToUpper().Trim() == item.ClientCode.ToUpper().Trim());

        //                if (String.IsNullOrEmpty(item.ClientName) && String.IsNullOrWhiteSpace(item.ClientName))
        //                {
        //                    item.FailReason = "Client name should not be empty !";
        //                    falultyImportItem.Add(item);
        //                }
        //                else
        //                {
        //                    if (String.IsNullOrEmpty(item.ClientCode) && String.IsNullOrWhiteSpace(item.ClientCode))
        //                    {
        //                        item.FailReason = "Client Code should not be empty !";
        //                        falultyImportItem.Add(item);
        //                    }
        //                    else
        //                    {
        //                        if (item.ClientName == null)
        //                        {
        //                            item.FailReason = "Client name should not be empty !";
        //                            falultyImportItem.Add(item);
        //                        }

        //                        if (check == null)
        //                        {
        //                            NewClientModel obj = new NewClientModel();
        //                            obj.IsActive = true;
        //                            obj.IsDeleted = false;
        //                            obj.CompanyId = tokendata.companyId;
        //                            obj.OrgId = tokendata.orgId;
        //                            obj.CreatedBy = tokendata.employeeId;
        //                            obj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
        //                            _db.NewClientModels.Add(obj);
        //                            await _db.SaveChangesAsync();
        //                            check = obj;
        //                        }
        //                        else
        //                        {
        //                            //   obj.CreatedOn = item.CreatedOn;
        //                            check.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
        //                            check.UpdatedBy = tokendata.employeeId;

        //                        }
        //                        check.ClientName = item.ClientName.Trim();
        //                        check.Description = item.Description;
        //                        check.OfficialEmail = item.OfficialEmail;
        //                        check.ClientCode = item.ClientCode.Trim();
        //                        check.MobileNumber = item.MobileNumber;
        //                        _db.Entry(check).State = EntityState.Modified;
        //                        await _db.SaveChangesAsync();

        //                        NewClientHistory historyobj = new NewClientHistory();
        //                        historyobj.ClientId = check.ClientId;
        //                        historyobj.ClientName = check.ClientName;
        //                        historyobj.Description = check.Description;
        //                        historyobj.OfficialEmail = check.OfficialEmail;
        //                        historyobj.ClientCode = check.ClientCode;
        //                        historyobj.MobileNumber = check.MobileNumber;
        //                        historyobj.CreatedBy = tokendata.employeeId;
        //                        historyobj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
        //                        historyobj.IsActive = check.IsActive;
        //                        historyobj.IsDeleted = check.IsDeleted;
        //                        historyobj.CompanyId = check.CompanyId;
        //                        historyobj.OrgId = check.OrgId;
        //                        historyobj.UpdatedBy = check.UpdatedBy;
        //                        historyobj.DeletedBy = check.DeletedBy;
        //                        historyobj.CreatedOn = check.CreatedOn;
        //                        historyobj.UpdatedOn = check.UpdatedOn;
        //                        historyobj.DeletedOn = check.DeletedOn;

        //                        _db.NewClientHistories.Add(historyobj);
        //                        await _db.SaveChangesAsync();
        //                        successfullImported += 1;
        //                    }
        //                }
        //            }
        //            if (falultyImportItem.Count > 0)
        //            {
        //                NewclientImportFaultyLogGroup groupobj = new NewclientImportFaultyLogGroup()
        //                {
        //                    GroupId = Guid.NewGuid(),
        //                    TotalImported = models.Count,
        //                    SuccessFullImported = successfullImported,
        //                    UnSuccessFullImported = falultyImportItem.Count,
        //                    CreatedBy = tokendata.employeeId,
        //                    CreatedOn = DateTime.Now,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    CompanyId = tokendata.companyId,
        //                    OrgId = tokendata.orgId,
        //                };
        //                _db.NewclientImportFaultyLogGroups.Add(groupobj);
        //                await _db.SaveChangesAsync();

        //                falultyImportItem.ForEach(x =>
        //                {
        //                    x.Groups = groupobj;
        //                });
        //                _db.NewclientImportFaultyLogs.AddRange(falultyImportItem);
        //                await _db.SaveChangesAsync();

        //                if ((models.Count - falultyImportItem.Count) > 0)
        //                {
        //                    res.Message = "Data Imported Succesfull Of " +
        //                    (models.Count - falultyImportItem.Count) + " Fields And " +
        //                    falultyImportItem.Count + " Feilds Are Not Imported";
        //                    res.Status = true;
        //                    res.Data = falultyImportItem;
        //                    res.StatusCode = HttpStatusCode.OK;
        //                }
        //                else
        //                {
        //                    res.Message = "All Fields Are Not Imported";
        //                    res.Status = true;
        //                    res.Data = falultyImportItem;
        //                }
        //            }
        //            else
        //            {
        //                res.Message = "Data Added Successfully Of All Fields";
        //                res.Status = true;
        //                res.Data = falultyImportItem;
        //            }
        //            return Ok(res);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("api/newclienttask/clienttaskimport", ex.Message);
        //        res.Message = ex.Message;
        //        res.Status = false;
        //        return BadRequest("Failed");
        //    }

        //}
        #endregion

        #region getall Methoud
        //public async Task getalltask(tokendata tokendata)
        //{
        //    var getall = await (from c in _db.clientTaskModels
        //                        join e in _db.Employee on c.CreatedBy equals e.EmployeeId
        //                        join a in _db.ClientTaskApprovals on c.ClientTaskId equals a.ClientTaskId into gj
        //                        from sub in gj.DefaultIfEmpty()
        //                        where c.IsActive && !c.IsDeleted && c.CompanyId == tokendata.companyId //&& sub.IsActive && sub.IsDeleted==false && sub.CompanyId == tokendata.companyId
        //                        select new clientTaskModelRequest
        //                        {
        //                            ClientTaskId = c.ClientTaskId,
        //                            WorkTypeId = c.WorkTypeId,
        //                            worktypename = _db.TypeofWorks.Where(y => y.WorktypeId == c.WorkTypeId).Select(y => y.WorktypeName).FirstOrDefault(),
        //                            ClientId = c.ClientId,
        //                            ClientName = _db.NewClientModels.Where(y => y.ClientId == c.ClientId).Select(y => y.ClientName).FirstOrDefault(),
        //                            IsSFA = sub == null ? false : sub.IsSFA,
        //                            Description = c.Description,
        //                            ClientCode = c.ClientCode,
        //                            Taskdate = c.Taskdate,
        //                            TaskStarttime = c.StartTime,
        //                            TaskRequest = sub == null ? ClientTaskRequestConstants.New.ToString().Replace("_", " ") : sub.TaskRequest.ToString().Replace("_", " "),
        //                            TaskEndtime = c.EndTime,
        //                            Starttime = null,
        //                            Endtime = null,
        //                            WorkingHours = TimeSpan.Zero,
        //                            IsActive = c.IsActive,
        //                            IsDeleted = c.IsDeleted,
        //                            CompanyId = c.CompanyId,
        //                            OrgId = c.OrgId,
        //                            IsDefaultCreated = c.IsDefaultCreated,
        //                            CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == c.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
        //                            UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == c.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
        //                            CreatedOn = c.CreatedOn,
        //                            UpdatedOn = c.UpdatedOn,
        //                            CreatedById = c.CreatedBy,
        //                            UpdatedById = c.UpdatedBy,
        //                            ReportingManager = e.ReportingManager,// _db.Employee.Where(x => c.CreatedBy == x.EmployeeId).Select(x => x.ReportingManager).FirstOrDefault(),
        //                                IsApproved = sub == null ? false : sub.IsApproved,
        //                        }).OrderByDescending(x => x.CreatedOn).ToListAsync();
        //}
        #endregion

        //#region check Methoud
        //public static checkdata(DateTimeOffset StartTime, DateTimeOffset EndTime)
        //{
        //    for (DateTimeOffset i = DateTimeOffset.MinValue; i < EndTime; i.AddMinutes(+1))
        //    {

        //    }
        //    while (true)
        //    {

        //    }


        //}
        //#endregion


    }
}