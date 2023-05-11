using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
//using IronXL;

namespace AspNetIdentity.WebApi.Controllers
{
    //[Authorize]
    [RoutePrefix("api/Attendance")]
    public class AttendanceController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        #region #GetAll Attendance  API

        [Route("GetAllAttendance")]
        [HttpGet]
        public async Task<AttListDto> GetAttendance()
        {
            AttListDto res = new AttListDto();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                // var attendance = db.Attendances.Where(x => x.IsActive == true && x.IsDeleted == false &&x.CompanyId == claims.companyid && x.OrgId == claims.orgid).ToList();

                var attendance = await (from A in db.Attendances
                                        join E in db.Employee on A.EmployeeId equals E.EmployeeId
                                        where A.IsActive == true && A.IsDeleted == false && A.CompanyId == claims.companyId && A.OrgId == claims.orgId
                                        select new AttendanceDTO
                                        {
                                            Id = A.Id,
                                            EmployeeId = A.EmployeeId,
                                            AppliedBy = E.FirstName + " " + E.LastName,
                                            Date = A.Date,
                                            ClockIn = A.ClockIn,
                                            ClockOut = A.ClockOut,
                                            TotalMinute = A.TotalMinute,
                                            TotalHours = A.TotalHours,
                                            Comment = A.Comment,
                                            IsClock = A.IsClock,
                                            CreatedDate = A.CreatedDate,
                                            ModifiedDate = A.ModifiedDate,
                                            IsActive = A.IsActive,
                                            IsDeleted = A.IsDeleted,
                                            CompanyId = A.CompanyId,
                                            OrgId = A.OrgId
                                        }).ToListAsync();



                //Create new Excel WorkBook document. 
                //The default file format is XLSX, but we can override that for legacy support
                //WorkBook xlsWorkbook = WorkBook.Create(ExcelFileFormat.XLS);
                //xlsWorkbook.Metadata.Author = "IronXL";

                ////Add a blank WorkSheet
                //WorkSheet xlsSheet = xlsWorkbook.CreateWorkSheet("new_sheet");
                ////Add data and styles to the new worksheet

                //xlsSheet["A1"].Value = "Hello World";
                //xlsSheet["A2"].Style.BottomBorder.SetColor("#ff6600");
                //xlsSheet["A2"].Style.BottomBorder.Type = IronXL.Styles.BorderType.Double;

                //Save the excel file
                //xlsWorkbook.SaveAs("NewExcelFile.xls");

                if (attendance.Count != 0)
                {
                    res.Message = "Successfull";
                    res.Status = true;
                    res.AttList = attendance;
                }
                else
                {
                    res.Message = "No Attendance Available";
                    res.Status = false;
                    res.AttList = null;
                }
            }
            catch (Exception ex)
            {
                res.AttList = null;
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion #GetAll Attendance  API

        #region #Get All Attendance By Employee Id API

        [Route("GetAllAttendanceByEmployeeId")]
        [HttpGet]
        public async Task<AttListDto> GetAllAttendanceByEmployeeId(int EmpId)
        {
            AttListDto res = new AttListDto();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                // var attendance = db.Attendances.Where(x => x.IsActive == true && x.IsDeleted == false &&x.CompanyId == claims.companyid && x.OrgId == claims.orgid).ToList();

                var attendance = await (from A in db.Attendances
                                        join E in db.Employee on A.EmployeeId equals E.EmployeeId
                                        where E.EmployeeId == EmpId && A.IsActive == true && A.IsDeleted == false && A.CompanyId == claims.companyId && A.OrgId == claims.orgId
                                        select new AttendanceDTO
                                        {
                                            Id = A.Id,
                                            EmployeeId = A.EmployeeId,
                                            AppliedBy = E.FirstName + " " + E.LastName,
                                            Date = A.Date,
                                            ClockIn = A.ClockIn,
                                            ClockOut = A.ClockOut,
                                            TotalMinute = A.TotalMinute,
                                            TotalHours = A.TotalHours,
                                            Comment = A.Comment,
                                            IsClock = A.IsClock,
                                            CreatedDate = A.CreatedDate,
                                            ModifiedDate = A.ModifiedDate,
                                            IsActive = A.IsActive,
                                            IsDeleted = A.IsDeleted,
                                            CompanyId = A.CompanyId,
                                            OrgId = A.OrgId
                                        }).ToListAsync();

                if (attendance.Count != 0)
                {
                    res.Message = "Successfull";
                    res.Status = true;
                    res.AttList = attendance;
                }
                else
                {
                    res.Message = "No Attendance Available";
                    res.Status = false;
                    res.AttList = null;
                }
            }
            catch (Exception ex)
            {
                res.AttList = null;
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion #Get All Attendance By Employee Id API

        #region#Get Today's Attendance Of Logged in Employee

        [Route("GetMyTodaysAttendance")]
        [HttpGet]
        public async Task<AttendanceReq> GetMyTodaysAttendance(int empId)
        {
            AttendanceReq res = new AttendanceReq();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var m = indianTime.Month;
                var y = indianTime.Year;
                var d = indianTime.Day;

                var attendance = await db.Attendances.Where(x => x.Date.Value.Day == d && x.Date.Value.Month == m && x.Date.Value.Year == y &&
                        x.EmployeeId == empId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefaultAsync();

                if (attendance != null)
                {
                    res.Message = "Successfull";
                    res.Status = true;
                    res.Attendance = attendance;
                }
                else
                {
                    res.Message = "No Attendance Available";
                    res.Status = false;
                    res.Attendance = null;
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Attendance = null;
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
        }

        #endregion

        #region #Get All Attendance By Id API

        [Route("GetAllAttendanceById")]
        [HttpGet]
        public async Task<AttendanceDto> GetAttendanceById(int empId)
        {
            AttendanceDto res = new AttendanceDto();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var attendance = await db.Attendances.Where(x => x.EmployeeId == empId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();

                if (attendance.Count > 0)
                {
                    res.Message = "Successfull";
                    res.Status = true;
                    res.AttendanceList = attendance;
                }
                else
                {
                    res.Message = "No Attendance Available";
                    res.Status = false;
                    res.AttendanceList = null;
                }
                return res;
            }
            catch (Exception ex)
            {
                res.AttendanceList = null;
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
        }

        #endregion

        #region #Add Attendance API

        [Route("AddAttendance")]
        [HttpPost]
        public async Task<AttendanceReq> AddAttendance(Attendance Attendance)
        {
            AttendanceReq res = new AttendanceReq();
            Attendance Obj = new Attendance();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (Attendance == null)
                {
                    throw new ArgumentNullException("Add Attendance");
                }
                else
                {
                    var m = indianTime.Month;
                    var y = indianTime.Year;
                    var d = indianTime.Day;

                    var attendance = await db.Attendances.Where(x => x.EmployeeId == Attendance.EmployeeId && x.Date.Value.Day == indianTime.Day &&
                            x.Date.Value.Month == m && x.Date.Value.Year == y && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefaultAsync();
                    if (attendance == null)
                    {
                        Obj.EmployeeId = Attendance.EmployeeId;
                        Obj.AppliedBy = Attendance.AppliedBy;
                        Obj.ClockIn = Attendance.ClockIn;
                        Obj.ClockOut = Attendance.ClockOut;
                        Obj.TotalMinute = Attendance.TotalMinute;
                        Obj.TotalHours = Attendance.TotalHours;
                        Obj.Date = DateTime.Now;
                        Obj.Comment = Attendance.Comment;
                        Obj.IsClock = true;
                        Obj.CreatedDate = DateTime.Now;
                        Obj.ModifiedDate = DateTime.Now;
                        Obj.CompanyId = claims.companyId;
                        Obj.OrgId = claims.orgId;
                        Obj.IsActive = true;
                        Obj.IsDeleted = false;
                        db.Attendances.Add(Obj);
                        db.SaveChanges();
                        res.Message = "Successful Added!!";
                        res.Status = true;
                        //return Attendance;
                        res.Attendance = Obj;
                    }
                    else
                    {
                        res.Message = "Your Attendance Already Added";
                        res.Status = false;
                        res.Attendance = null;
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.Attendance = null;
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
        }

        #endregion

        #region #Update Clockout Time API

        [Route("UpdateClockOut")]
        [HttpPut]
        public async Task<AttendanceReq> UpdateClock(Attendance At)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            AttendanceReq res = new AttendanceReq();
            try
            {
                var m = indianTime.Month;
                var y = indianTime.Year;
                var d = indianTime.Day;

                var Clock = await db.Attendances.Where(x => x.EmployeeId == At.EmployeeId && x.Date.Value.Day == indianTime.Day &&
                        x.Date.Value.Month == m && x.Date.Value.Year == y && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefaultAsync();
                if (Clock != null)
                {
                    Clock.EmployeeId = At.EmployeeId;
                    Clock.ClockOut = At.ClockOut;
                    Clock.Date = DateTime.Now;
                    Clock.IsClock = false;
                    Clock.TotalHours = At.TotalHours;
                    Clock.TotalMinute = At.TotalMinute;
                    Clock.AppliedBy = At.AppliedBy;
                    db.Entry(Clock).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Message = "Successful Update";
                    res.Status = true;
                    res.Attendance = Clock;
                }
                else
                {
                    res.Message = "Updateb Faild !!";
                    res.Status = false;
                    res.Attendance = null;
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Attendance = null;
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
        }

        #endregion

        #region#Get TodayAttendance Api

        [Route("GetAllAttendanceByDate")]
        [HttpGet]
        public async Task<AttendanceRes> GetByDate()
        {
            AttendanceRes res = new AttendanceRes();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var m = indianTime.Month;
                var y = indianTime.Year;
                var d = indianTime.Day;

                var TodayData = await db.Attendances.Where(x => x.Date.Value.Day == indianTime.Day && x.Date.Value.Month == m &&
                        x.Date.Value.Year == y && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                if (TodayData.Count != 0)
                {
                    res.Message = "Successful";
                    res.Status = true;
                    res.Attendance = TodayData;
                }
                else
                {
                    res.Message = "No Attendance Available";
                    res.Status = false;
                    res.Attendance = null;
                }

                return res;
            }
            catch (Exception ex)
            {
                res.Attendance = null;
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
        }

        #endregion

    }

    public class AttendanceRes
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public List<Attendance> Attendance { get; set; }
    }

    public class AttendanceReq
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public Attendance Attendance { get; set; }
    }

    public class AttendanceDto
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public List<Attendance> AttendanceList { get; set; }
    }

    public class AttendanceDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string AppliedBy { get; set; }
        public DateTime? Date { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinute { get; set; }
        public string Comment { get; set; }
        public bool IsClock { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }

    public class AttListDto
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public List<AttendanceDTO> AttList { get; set; }
    }
}