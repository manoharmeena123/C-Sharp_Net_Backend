using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.ToDoList_Module;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.ToDoList
{
    /// <summary>
    /// Created By Suraj Bundel on 05-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/todolist")]
    public class ToDoListController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        #region API TO CREATE TASK 
        /// <summary>
        /// Created By Suraj Bundel On 05-01-2023
        /// API >> POST >> api/todolist/createtask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(res);
                }
                else
                {
                    if (model.id == Guid.Empty)
                    {
                        if (model.StartDate >= DateTime.Today)
                        {

                            ToDoListModel obj = new ToDoListModel
                            {
                                Title = model.Title,
                                Description = model.Description,
                                StartDate = model.StartDate,
                                EndDate = model.EndDate,
                                IsComplete = model.IsComplete,
                                Image1 = model.Image1,
                                Image2 = model.Image2,
                                Image3 = model.Image3,
                                Image4 = model.Image4,
                                Image5 = model.Image5,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),

                            };
                            _db.ToDoListModels.Add(obj);
                            await _db.SaveChangesAsync();
                            res.Message = "Created Succesfully !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                            res.Data = obj;
                            return Ok(res);
                        }
                        res.Message = "Please select today or upcoming dates !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    else if (model.id != Guid.Empty)
                    {
                        {
                            var updatetask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.TodolistId == model.id).FirstOrDefaultAsync();

                            if (updatetask == null)
                            {

                                res.Message = "Task not Found !";
                                res.Status = false;
                                res.StatusCode = HttpStatusCode.NotFound;
                                res.Data = updatetask;
                                return Ok(res);
                            }
                            else
                            {
                                updatetask.Title = model.Title;
                                updatetask.Description = model.Description;
                                updatetask.StartDate = model.StartDate;
                                updatetask.EndDate = model.EndDate;
                                updatetask.IsComplete = model.IsComplete;
                                updatetask.Image1 = model.Image1;
                                updatetask.Image2 = model.Image2;
                                updatetask.Image3 = model.Image3;
                                updatetask.Image4 = model.Image4;
                                updatetask.Image5 = model.Image5;
                                updatetask.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                                updatetask.UpdatedBy = tokenData.employeeId;
                                _db.Entry(updatetask).State = EntityState.Modified;
                                await _db.SaveChangesAsync();

                                res.Message = "Update Succesfully !";
                                res.Status = true;
                                res.StatusCode = HttpStatusCode.Accepted;
                                res.Data = updatetask;
                                return Ok(res);
                            }
                        }
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/createtask", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region Get date upcomming  and previous dates
        /// <summary>
        /// Created By Suraj Bundel On 05-01-2023
        /// API >> POST >> api/todolist/getdate?nextday&previousday&newdate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getdate")]
        public async Task<IHttpActionResult> GetDate(getdate model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset date = DateTimeOffset.UtcNow.Date;

                DateTimeOffset data = (DateTimeOffset)model.getvalue.Date;
                if (data != null)
                {
                    if (model.nextday)
                    {
                        var next = data.AddDays(1);
                        res.Message = "Next Date !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = next.ToString("yyyy-MM-dd");
                        return Ok(res);
                    }
                    else if (model.previousday)
                    {
                        var next = data.AddDays(-1);
                        res.Message = "Previous Date !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = next.ToString("yyyy-MM-dd");
                        return Ok(res);
                    }
                    else
                    {
                        date = data.AddDays(0);
                        res.Message = "Created Succesfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = date.ToString("yyyy-MM-dd");
                        return Ok(res);
                    }

                }

                else
                {
                    if (model.nextday)
                    {
                        var next = date.AddDays(1);
                        res.Message = "Next Date !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = next.ToString("yyyy-MM-dd");

                        return Ok(res);
                    }
                    else if (model.previousday)
                    {
                        var next = date.AddDays(-1);
                        res.Message = "Previous Date !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = next.ToString("yyyy-MM-dd");
                        return Ok(res);
                    }
                    else
                    {
                        date = date.AddDays(0);
                        res.Message = "Created Succesfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = date.ToString("yyyy-MM-dd");
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/createtask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        public class getdate
        {
            public DateTimeOffset getvalue { get; set; } = DateTimeOffset.UtcNow.Date;
            public bool nextday { get; set; } = false;
            public bool previousday { get; set; } = false;


        }
        #endregion

        #region API for Upload multiple for to do list image

        /// <summary>
        ///  Created by Suraj Bundel on 05/01/23
        ///  API >> POST >> api/todolist/multipleuploads
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("multipleuploads")]
        public async Task<UploadImageResponse> UploadMultipleFileAll()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
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
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"').Replace(" ", "");

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Todolist/Taskimage/" + claims.companyId), dates + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string temp = "uploadimage\\Todolist\\Taskimage\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = temp;
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
        #endregion API for Upload multiple for upload invoice

        #region API FOR GET ALL TASK

        /// <summary>
        /// Created By Suraj Bundel on 05-01-2023
        /// API>>GET>> api/todolist/gettodaytask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettodaytask")]
        public async Task<IHttpActionResult> GetAllTask(DateTime Listdate)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetTaskDTO obj = new GetTaskDTO();
            try
            {
                if (Listdate == null)
                {
                    res.Message = "enter proper dates";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = obj;
                    return Ok(res);
                }
                else
                {
                    var IncompletedTask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.StartDate == Listdate && x.CreatedBy == tokenData.employeeId && !x.IsComplete)
                                               .Select(x => new TaskResponseModel
                                               {
                                                   id = x.TodolistId,
                                                   Title = x.Title,
                                                   Description = x.Description,
                                                   StartDate = x.StartDate,
                                                   EndDate = x.EndDate,
                                                   IsComplete = x.IsComplete,
                                                   Image1 = x.Image1,
                                                   Image2 = x.Image2,
                                                   Image3 = x.Image3,
                                                   Image4 = x.Image4,
                                                   Image5 = x.Image5,
                                                   Createdon = x.CreatedOn,
                                                   CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                               }).OrderByDescending(x => x.Createdon).ToListAsync();

                    obj.InCompletedtask = IncompletedTask;

                    var completedTask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.StartDate == Listdate && x.CreatedBy == tokenData.employeeId && x.IsComplete)
                                               .Select(x => new TaskResponseModel
                                               {
                                                   id = x.TodolistId,
                                                   Title = x.Title,
                                                   Description = x.Description,
                                                   StartDate = x.StartDate,
                                                   EndDate = x.EndDate,
                                                   IsComplete = x.IsComplete,
                                                   Image1 = x.Image1,
                                                   Image2 = x.Image2,
                                                   Image3 = x.Image3,
                                                   Image4 = x.Image4,
                                                   Image5 = x.Image5,
                                                   Createdon = x.CreatedOn,
                                                   CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                               }).OrderByDescending(x => x.Createdon).ToListAsync();
                    obj.Completedtask = completedTask;

                    res.Message = " To Do List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = obj;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                logger.Error("api/todolist/gettodaytask", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }

        }
        public class GetTaskDTO
        {
            public List<TaskResponseModel> Completedtask { get; set; }
            public List<TaskResponseModel> InCompletedtask { get; set; }
        }

        public class TaskResponseModel
        {
            public Guid id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public bool IsComplete { get; set; }
            public string Image1 { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string Image4 { get; set; }
            public string Image5 { get; set; }
            public string CreatedByName { get; set; }
            public DateTimeOffset Createdon { get; set; }

        }
        #endregion

        #region API TO delete TASK 
        /// <summary>
        /// Created By Suraj Bundel On 05-01-2023
        /// API >> POST >> api/todolist/deletetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletetask")]
        public async Task<IHttpActionResult> DeleteTask(UpdateRequest Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(res);
                }
                else
                {
                    var updatetask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.TodolistId == Model.id).FirstOrDefaultAsync();

                    if (updatetask == null)
                    {
                        res.Message = "Task not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = updatetask;
                        return Ok(res);
                    }
                    else
                    {
                        updatetask.IsActive = false;
                        updatetask.IsDeleted = true;
                        updatetask.DeletedBy = tokenData.employeeId;
                        updatetask.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                        _db.Entry(updatetask).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Delete Succesfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Accepted;
                        res.Data = updatetask;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/updatetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region API TO Complete Task
        /// <summary>
        /// Created By Suraj Bundel On 05-01-2023
        /// API >> POST >> api/todolist/completetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("completetask")]
        public async Task<IHttpActionResult> CompleteTask(UpdateRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(res);
                }
                else
                {
                    var updatetask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.TodolistId == model.id).FirstOrDefaultAsync();

                    if (updatetask == null)
                    {

                        res.Message = "Task not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = updatetask;
                        return Ok(res);
                    }
                    else
                    {
                        if (updatetask.IsComplete)
                        {
                            updatetask.IsComplete = false;
                            updatetask.UpdatedOn = DateTimeOffset.Now;
                            updatetask.UpdatedBy = tokenData.employeeId;
                            _db.Entry(updatetask).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            updatetask.IsComplete = true;
                            updatetask.UpdatedOn = DateTimeOffset.Now;
                            updatetask.UpdatedBy = tokenData.employeeId;
                            _db.Entry(updatetask).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }


                        res.Message = "Update Succesfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = updatetask;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/todolist/updatetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }




        #endregion

        #region API FOR GET TASK By id

        /// <summary>
        /// Created By Suraj Bundel on 05-01-2023
        /// API>>GET>> api/todolist/gettaskbyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettaskbyid")]
        public async Task<IHttpActionResult> GetTaskbyid(Guid id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (id == Guid.Empty)
                {
                    res.Message = "Task not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    var IncompletedTask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == tokenData.employeeId && x.TodolistId == id)
                                               .Select(x => new TaskResponseModel
                                               {
                                                   id = x.TodolistId,
                                                   Title = x.Title,
                                                   Description = x.Description,
                                                   StartDate = x.StartDate,
                                                   EndDate = x.EndDate,
                                                   IsComplete = x.IsComplete,
                                                   Image1 = x.Image1,
                                                   Image2 = x.Image2,
                                                   Image3 = x.Image3,
                                                   Image4 = x.Image4,
                                                   Image5 = x.Image5,
                                                   CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                               }).OrderByDescending(x => x.StartDate).ToListAsync();

                    res.Message = " To Do List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = IncompletedTask;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                logger.Error("api/todolist/gettodaytask", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region API FOR GET Export  Task

        /// <summary>
        /// Created By Suraj Bundel on 06-01-2023
        /// API>>GET>> api/todolist/gettaskexport
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettaskexport")]
        public async Task<IHttpActionResult> GetTaskExport(bool forweek = false, bool today = false)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DateTimeOffset startingDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

            try
            {
                if (forweek)
                {
                    var todaydate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now);
                    CreateRequest reqdata = new CreateRequest();

                    var dateList = string
                        .Join("|", Enumerable
                        .Range(0, 7)
                        .Select(i =>
                        (DateTimeOffset.UtcNow).AddDays(i)))
                        .Split('|')
                        .Select(x => DateTimeOffset.Parse(x))
                        .ToList();

                    var startdate = dateList[0].Date;
                    var enddate = dateList[6].Date;

                    DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever


                    while (startingDate.DayOfWeek != weekStart)
                        startingDate = startdate.AddDays(-1);

                    var TaskData = await _db.ToDoListModels
                        .Where(x => x.IsActive && !x.IsDeleted && x.CreatedBy == tokenData.employeeId &&
                        x.EndDate >= startdate.Date && x.StartDate <= enddate.Date && x.CompanyId == tokenData.companyId).ToListAsync();

                    List<ExportRequest> req = new List<ExportRequest>();

                    foreach (var datename in dateList)
                    {
                        var datalist = TaskData.Where(z => z.StartDate.Date == datename.Date)
                           .Select(x => new ExportRequest
                           {
                               TaskDate = datename.Date.ToString("dd-MM-yyyy"),
                               id = x.TodolistId,
                               Title = x.Title,
                               Description = x.Description,
                               IsComplete = x.IsComplete,
                               StartDate = x.StartDate.Date.ToString("dd-MM-yyyy"),
                               EndDate = x.EndDate.Date.ToString("dd-MM-yyyy"),
                           }).ToList();
                        if (datalist.Count == 0)
                        {
                            var objdata = new ExportRequest
                            {
                                TaskDate = datename.Date.ToString("dd-MM-yyyy"),
                                id = Guid.Empty,
                                Title = null,
                                Description = null,
                                IsComplete = false,
                                StartDate = null,
                                EndDate = null,
                            };
                            datalist.Add(objdata);
                        }
                        req.AddRange(datalist);
                    }
                    if (req == null)
                    {
                        res.Message = "List not found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = req;
                        return Ok(res);
                    }
                    else
                    {

                        res.Message = " To Do List Found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = req;
                        return Ok(res);
                    }
                }
                else if (today)
                {

                    DayOfWeek startingday = startingDate.DayOfWeek;

                    var todaydate = DateTimeOffset.UtcNow.Date.ToString("dd-MM-yyyy");

                    var tasklist = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == tokenData.employeeId && x.EndDate >= startingDate.Date && x.StartDate <= startingDate.Date)
                                                   .Select(x => new TaskResponseModel
                                                   {
                                                       id = x.TodolistId,
                                                       Title = x.Title,
                                                       Description = x.Description,
                                                       StartDate = x.StartDate,
                                                       EndDate = x.EndDate,
                                                       IsComplete = x.IsComplete,
                                                   }).ToListAsync();
                    //}).OrderByDescending(x => x.StartDate).ToListAsync();
                    if (tasklist.Count == 0)
                    {
                        var objtasklist = _db.ToDoListModels.Where(x => x.CreatedBy == tokenData.employeeId && x.IsActive && !x.IsDeleted).Select(x => new ExportRequest
                        {
                            TaskDate = todaydate,
                            id = Guid.Empty,
                            Title = null,
                            Description = null,
                            IsComplete = false,
                            StartDate = null,
                            EndDate = null,
                        }).Distinct().ToList();


                        res.Message = "No Task ";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = objtasklist;
                        return Ok(res);
                    }
                    else
                    {

                        res.Message = " To Do List Found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = tasklist;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = " Please select Current Day or Current week";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.OK;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                logger.Error("api/todolist/gettaskexport", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }

        }
        #endregion

        #region get current , upcoming and Previous weeks with data
        /// <summary>
        /// Created By Suraj Bundel on 06-01-2023
        /// API>>GET>> api/todolist/getweekdate
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        [Route("getweekdate")]
        [HttpGet]
        public async Task<IHttpActionResult> GetWeekDay(DateTimeOffset? dateValue)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now);
                if (!dateValue.HasValue)
                    dateValue = today;
                var dateList = string
                    .Join("|", Enumerable
                    .Range(0, 7)
                    .Select(i =>
                        ((DateTimeOffset)dateValue).AddDays(i)))
                    .Split('|')
                    .Select(x => DateTimeOffset.Parse(x))
                    .ToList();

                Root rootObj = new Root();

                var startdate = dateList[0];
                var enddate = dateList[6];

                var taskModel = await _db.ToDoListModels
                            .Where(x => x.IsActive && !x.IsDeleted && x.CreatedBy == tokenData.employeeId &&
                            x.EndDate >= startdate.Date && x.StartDate <= enddate.Date && x.CompanyId == tokenData.companyId)
                            .ToListAsync();
                var resule = dateList
                                    .Select(x => new
                                    {
                                        Date = x.Date == today.Date ? "Today" : x.ToString("dd-MM-yyyy"),
                                        ToDoList = taskModel.Where(z => z.StartDate.Date == x.Date)
                                                .Select(z => new
                                                {
                                                    Id = z.TodolistId,
                                                    Title = z.Title,
                                                    Description = z.Description,
                                                    IsComplete = z.IsComplete,
                                                }
                                            ).OrderBy(z => z.IsComplete).ToList(),
                                    })
                                    .ToList();
                rootObj.startdate = startdate.Date.ToString("dd-MM-yyyy");
                rootObj.StartDateValue = startdate;
                rootObj.enddate = enddate.Date.ToString("dd-MM-yyyy");
                rootObj.EndDateValue = enddate;
                rootObj.WeekDataList = resule;
                rootObj.Previousdate = startdate.Date.AddDays(-7);
                rootObj.Nextdate = enddate.Date.AddDays(1);

                return Ok(rootObj);
            }

            catch (Exception ex)
            {

                logger.Error("api/todolist/updatetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());

            }
        }
        public class Root
        {
            public string startdate { get; set; }
            public DateTimeOffset StartDateValue { get; set; }
            public string enddate { get; set; }
            public DateTimeOffset EndDateValue { get; set; }
            public DateTimeOffset Previousdate { get; set; }
            public DateTimeOffset Nextdate { get; set; }
            public object WeekDataList { get; set; }
        }


        #endregion

        #region API TO start Task
        /// <summary>
        /// Created By Suraj Bundel On 05-01-2023
        /// API >> POST >> api/todolist/starttask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("starttask")]
        public async Task<IHttpActionResult> StartTask(CreateRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(res);
                }
                else
                {
                    if (model.id == Guid.Empty)
                    {
                        if (model.StartDate >= DateTime.Today)
                        {
                            ToDoListModel obj = new ToDoListModel
                            {
                                Title = model.Title,
                                StartDate = model.StartDate,
                                EndDate = model.EndDate,
                                IsComplete = false,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),

                            };
                            _db.ToDoListModels.Add(obj);
                            await _db.SaveChangesAsync();
                            var CreatedOntest = obj.StartDate.ToString("dd-MMM-yyyy");
                            res.Message = "Created Succesfully !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                            res.Data = obj;
                            return Ok(res);
                        }
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/createtask", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region Api To Get Employee Directory Filter (For Admin) loginrole are not set

        /// <summary>
        /// Created By Suraj Bundel on 10-01-2023
        /// API >> api/todolist/todoemplist
        /// </summary>

        [HttpGet]
        [Route("todoemplist")]
        public async Task<IHttpActionResult> GetEmployeeToDoDirectoryFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var orgList = await _db.OrgMaster.Where(x => !x.IsActive && x.IsDeleted &&
                            x.CompanyId == tokenData.companyId).Select(x => x.OrgId).ToListAsync();
                EmployeeListRequest obj = new EmployeeListRequest();

                var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                           x.CompanyId == tokenData.companyId && x.RoleName == "Global").FirstOrDefault();

                if (roledata != null)
                //if (tokenData.orgId == 0 && tokenData.roleType == "Administrator")
                {
                    var employeeList = (from e in _db.Employee
                                        join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                        join td in _db.ToDoListModels on e.EmployeeId equals td.CreatedBy
                                       into q
                                        from result in q.DefaultIfEmpty()
                                        where e.IsActive && !e.IsDeleted &&
                                        e.CompanyId == tokenData.companyId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                        && !orgList.Contains(result.OrgId)
                                        select new EmployeeListRequest
                                        {
                                            EmployeeId = e.EmployeeId,
                                            DisplayName = e.DisplayName,
                                            MobilePhone = e.MobilePhone,
                                            DepartmentName = d.DepartmentName,
                                            OfficeEmail = e.OfficeEmail,
                                            todolist = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == e.EmployeeId).Count(),
                                            todolistCompleted = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == e.EmployeeId && x.IsComplete).Count(),
                                            todolistInCompleted = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == e.EmployeeId && !x.IsComplete).Count(),
                                        }).Distinct().ToList();

                    var Data = employeeList;
                    if (employeeList.Count > 0)
                    {
                        res.Message = "Employee List Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = Data;

                    }
                    else
                    {
                        res.Message = "Employee List not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = Data;
                    }

                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/createtask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region API FOR GET TASK By id (For Admin)
        /// <summary>
        /// Created By Suraj Bundel on 05-01-2023
        /// API>>GET>> api/todolist/getemployeetaskbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeetaskbyid")]
        public async Task<IHttpActionResult> GetEmployeeTaskbyid(int id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (id == 0)
                {
                    res.Message = "enter proper dates";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {

                    var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                           x.CompanyId == tokenData.companyId && x.RoleName == "Global").FirstOrDefault();
                    if (tokenData.roleType == null)
                    {
                        res.Message = " You don't have permission !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        return Ok(res);
                    }
                    else
                    {
                        var IncompletedTask = await (from e in _db.Employee
                                                     join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                                     join td in _db.ToDoListModels on e.EmployeeId equals td.CreatedBy into q
                                                     where e.IsActive && !e.IsDeleted &&
                                                     e.CompanyId == tokenData.companyId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                                     && e.EmployeeId == id
                                                     select new EmployeeListRequest
                                                     {
                                                         EmployeeId = e.EmployeeId,
                                                         DisplayName = e.DisplayName,
                                                         MobilePhone = e.MobilePhone,
                                                         DepartmentName = d.DepartmentName,
                                                         OfficeEmail = e.OfficeEmail,
                                                         todolist = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == e.EmployeeId).Count(),
                                                         todolistCompleted = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == e.EmployeeId && x.IsComplete).Count(),
                                                         todolistInCompleted = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == e.EmployeeId && !x.IsComplete).Count(),
                                                     }).Distinct().ToListAsync();

                        res.Message = " To Do List Found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = IncompletedTask;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                logger.Error("api/todolist/gettodaytask", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region Get All ToDoTask Status // dropdown

        /// <summary>
        /// Created By Suraj Bundel on 11-05-2023
        /// API >> Get >> api/todolist/gettaskstatus
        /// Dropdown using Enum for expense type category
        /// </summary>
        [HttpGet]
        [Route("gettaskstatus")]
        public ResponseBodyModel GetTaskStatus()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetsItemType = Enum.GetValues(typeof(Todotaskstatus))
                    .Cast<Todotaskstatus>()
                    .Select(x => new TodoTaskList
                    {
                        ToDoTaskStatusId = (int)x,
                        ToDoTaskStatusType = Enum.GetName(typeof(Todotaskstatus), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Assets Item Type";
                res.Status = true;
                res.Data = assetsItemType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class TodoTaskList
        {
            public int ToDoTaskStatusId { get; set; }
            public string ToDoTaskStatusType { get; set; }
        }

        #endregion Get All Expense Category // dropdown

        #region API FOR GET Task Data By id (For Admin)
        /// <summary>
        /// Created By Suraj Bundel on 05-01-2023
        /// API>>GET>> api/todolist/getemployedatabyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployedatabyid")]
        public async Task<IHttpActionResult> GetEmployeeTaskdatabyid(int id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (id == 0)
                {
                    res.Message = "enter proper dates";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                          x.CompanyId == tokenData.companyId && x.RoleName == "Global").FirstOrDefault();
                    if (tokenData.roleType == null)
                    {
                        res.Message = " You don't have permission !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        return Ok(res);
                    }
                    else
                    {
                        var IncompletedTask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == id)
                                               .Select(x => new TaskResponseModel
                                               {
                                                   id = x.TodolistId,
                                                   Title = x.Title,
                                                   Description = x.Description,
                                                   StartDate = x.StartDate,
                                                   EndDate = x.EndDate,
                                                   IsComplete = x.IsComplete,
                                                   Image1 = x.Image1,
                                                   Image2 = x.Image2,
                                                   Image3 = x.Image3,
                                                   Image4 = x.Image4,
                                                   Image5 = x.Image5,
                                                   CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                               }).OrderByDescending(x => x.StartDate).ToListAsync();

                        res.Message = " To Do List Found !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = IncompletedTask;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/gettodaytask", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region API FOR GET Task Data By id (For Admin)
        /// <summary>
        /// Created By Suraj Bundel on 09-01-2023
        /// API>>GET>> api/todolist/getemptaskdetail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemptaskdetail")]
        public async Task<IHttpActionResult> FilterEmployeeTaskdata(int employeeid, int status)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (employeeid == 0)
                {
                    res.Message = "enter proper dates";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                          x.CompanyId == tokenData.companyId && x.RoleName == "Global").FirstOrDefault();
                    if (tokenData.roleType == null)
                    {
                        res.Message = " You don't have permission !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        return Ok(res);
                    }
                    else
                    {
                        var IncompletedTask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == employeeid)
                                               .Select(x => new TaskResponseModel
                                               {
                                                   id = x.TodolistId,
                                                   Title = x.Title,
                                                   Description = x.Description,
                                                   StartDate = x.StartDate,
                                                   EndDate = x.EndDate,
                                                   IsComplete = x.IsComplete,
                                                   Image1 = x.Image1,
                                                   Image2 = x.Image2,
                                                   Image3 = x.Image3,
                                                   Image4 = x.Image4,
                                                   Image5 = x.Image5,
                                                   CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                               }).OrderByDescending(x => x.StartDate).ToListAsync();

                        if (status == 1)
                        {
                            var taskstatus = IncompletedTask.Where(x => !x.IsComplete);
                            res.Message = " To Do List Found !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.OK;
                            res.Data = taskstatus;
                            return Ok(res);
                        }
                        else if (status == 2)
                        {
                            var taskstatus = IncompletedTask.Where(x => x.IsComplete);
                            res.Message = " To Do List Found !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.OK;
                            res.Data = taskstatus;
                            return Ok(res);
                        }
                        else
                        {
                            res.Message = " List Not Found !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.NoContent;
                            res.Data = IncompletedTask;
                            return Ok(res);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/todolist/gettodaytask", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region API to search employee on behalf of details saved in employees profile
        /// <summary>
        /// Created By Suraj Bundel on 13-01-2023
        /// API >> api/employeenew/employeetasksearch?search
        /// </summary>
        [HttpGet]
        [Route("employeetasksearch")]
        public async Task<IHttpActionResult> GetEmployeetaskSearch(string search)
        {
            ResponseStatusCode res = new ResponseStatusCode();


            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<EmployeeListRequest> employeeList = new List<EmployeeListRequest>();
                //if (tokenData.orgId == 0)
                //{
                if (string.IsNullOrEmpty(search))
                {
                    res.Message = "No Search Result Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = employeeList;
                }
                // }
                else
                {

                    employeeList = await (from emp in _db.Employee
                                          join dep in _db.Department on emp.DepartmentId equals dep.DepartmentId
                                          where emp.IsActive && !emp.IsDeleted && emp.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                          emp.CompanyId == tokenData.companyId
                                          && emp.FirstName.ToLower().Contains(search.ToLower()) ||
                                          emp.LastName.ToLower().Contains(search.ToLower())

                                          select new EmployeeListRequest
                                          {
                                              EmployeeId = emp.EmployeeId,
                                              DisplayName = emp.DisplayName,
                                              MobilePhone = emp.MobilePhone,
                                              DepartmentName = dep.DepartmentName,
                                              OfficeEmail = emp.OfficeEmail,
                                              todolist = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == emp.EmployeeId).Count(),
                                              todolistCompleted = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == emp.EmployeeId && x.IsComplete).Count(),
                                              todolistInCompleted = _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == emp.EmployeeId && !x.IsComplete).Count(),

                                          }).Distinct().ToListAsync();
                    if (employeeList.Count > 0)
                    {
                        res.Status = true;
                        res.Message = "Employee List Found in Search Results";
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = employeeList;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Status = false;
                        res.Message = "No Search Result Found";
                        res.Data = employeeList;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }

        #endregion

        #region API FOR GET ALL INCOMPLETE TASK

        /// <summary>
        /// Created By Suraj Bundel on 31-01-2023
        /// API>>GET>> api/todolist/gettodaycompletedtask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettodaycompletedtask")]
        public async Task<IHttpActionResult> GetAllCompletedTask()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var completedTask = await _db.ToDoListModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.CreatedBy == tokenData.employeeId && !x.IsComplete)
                                           .Select(x => new
                                           {
                                               id = x.TodolistId,
                                               Title = x.Title,
                                               Description = x.Description,
                                               StartDate = x.StartDate,
                                               EndDate = x.EndDate,
                                               IsComplete = x.IsComplete,
                                               Image1 = x.Image1,
                                               Image2 = x.Image2,
                                               Image3 = x.Image3,
                                               Image4 = x.Image4,
                                               Image5 = x.Image5,
                                               Createdon = x.CreatedOn,
                                               CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                           }).OrderByDescending(x => x.StartDate).ToListAsync();

                if (completedTask.Count == 0)
                {
                    res.Message = "No Task Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = completedTask;
                    return Ok(res);
                }
                else
                {
                    res.Message = " To Do List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = completedTask;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                logger.Error("api/todolist/gettodaytask", ex.Message);
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region REQUEST AND RESPONSE
        public class EmployeeListRequest
        {

            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public string MobilePhone { get; set; }
            public string DepartmentName { get; set; }
            public string OfficeEmail { get; set; }
            public int todolist { get; set; }
            public int todolistCompleted { get; set; }
            public int todolistInCompleted { get; set; }


        }

        public class ExportRequest
        {
            public Guid id { get; set; } = Guid.Empty;
            public string Title { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public string StartDate { get; set; } = String.Empty;
            public string EndDate { get; set; } = String.Empty;
            public object TaskDate { get; set; }
            public bool IsComplete { get; set; } = false;
        }
        public class UpdateRequest
        {
            public Guid id { get; set; } = Guid.Empty;
        }
        public class CreateRequest : UpdateRequest
        {
            public string Title { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public DateTime StartDate { get; set; } = DateTime.UtcNow;
            public DateTime EndDate { get; set; } = DateTime.UtcNow;
            public bool IsComplete { get; set; } = false;
            public string Image1 { get; set; } = String.Empty;
            public string Image2 { get; set; } = String.Empty;
            public string Image3 { get; set; } = String.Empty;
            public string Image4 { get; set; } = String.Empty;
            public string Image5 { get; set; } = String.Empty;
        }
        #endregion
    }
}