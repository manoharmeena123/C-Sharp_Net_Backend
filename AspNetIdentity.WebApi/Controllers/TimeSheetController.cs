//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Model;
//using AspNetIdentity.WebApi.Models;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace AspNetIdentity.WebApi.Controllers
//{
//    [RoutePrefix("api/timesheet")]
//    public class TimeSheetController : ApiController
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();

//        [HttpPost]
//        [Route("addnewtimesheet")]
//        public async Task<ResponseBodyModel> AddTimeSheet(NewTimeSheet model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            NewTimeSheet obj = new NewTimeSheet();
//            try
//            {
//                var addtime = db.NewTimeSheets.Where(x => x.NewTimeSheetId == model.NewTimeSheetId).FirstOrDefault();
//                if (addtime == null)
//                {
//                    obj.NewTimeSheetId = model.NewTimeSheetId;
//                    obj.ProjectId = model.ProjectId;
//                    obj.DepartmentId = model.DepartmentId;
//                    obj.ProjectName = model.ProjectName;
//                    obj.Department = model.Department;
//                    obj.Time = model.Time;
//                    obj.Note = model.Note;
//                    obj.Date = model.Date;
//                    obj.IsActive = true;
//                    obj.IsDeleted = false;
//                    obj.TotalTime = model.TotalTime;
//                    obj.IsTimeStart = model.IsTimeStart;
//                    db.NewTimeSheets.Add(obj);
//                    db.SaveChanges();

//                    res.Message = "Added Succesfully";
//                    res.Status = true;
//                    res.Data = obj;
//                }
//                else
//                {
//                    res.Message = "Added faild";
//                    res.Status = false;
//                    res.Data = null;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        [HttpGet]
//        [Route("getnewtimesheet")]
//        public async Task<ResponseBodyModel> Gettime()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();

//            try
//            {
//                var gettime = db.NewTimeSheets.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
//                if (gettime != null)
//                {
//                    res.Message = "Get Succesfully";
//                    res.Status = true;
//                    res.Data = gettime;
//                }
//                else
//                {
//                    res.Message = "Added faild";
//                    res.Status = false;
//                    res.Data = null;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        [HttpPost]
//        [Route("addday")]

//        public async Task<ResponseBodyModel> AddDay(NewTimeSheet model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            NewTimeSheet obj = new NewTimeSheet();
//            try
//            {
//                var getday = db.NewTimeSheets.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
//                if (getday != null)
//                {
//                    obj.Time = model.Time;
//                    obj.Date = model.Date;
//                    obj.day = (model.Time.ToString("dddd"));

//                    res.Message = "add day Succesfully";
//                    res.Status = true;
//                    res.Data = getday;
//                }
//                else
//                {
//                    res.Message = "not added faild";
//                    res.Status = false;
//                    res.Data = null;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }
//    }
//}