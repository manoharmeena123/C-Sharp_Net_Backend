using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.LeaveMasterModel;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.LeaveModule
{
    /// <summary>
    /// Created By Harshit Mitra On 22-07-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/leavemaster")]
    public class LeaveMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region API To Get Leave List of Logged-In Employee

        /// <summary>
        /// Created By Harshit Mita On 23-07-2022
        /// API >> Get >> api/leavemaster/getempleavelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getempleavelist")]
        public async Task<ResponseBodyModel> GetEmployeeLeaveList(bool forInternal = false, int empId = 0)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Employee emp = new Employee();
                if (forInternal)
                    emp = await _db.Employee.FirstOrDefaultAsync(x =>
                        x.EmployeeId == empId && x.CompanyId == claims.companyId);
                else
                    emp = await _db.Employee.FirstOrDefaultAsync(x =>
                           x.EmployeeId == claims.employeeId && x.CompanyId == claims.companyId);
                if (emp != null)
                {
                    if (emp.LeaveGroupId != 0)
                    {
                        var leaveComponents = await (from c in _db.LeaveComponents
                                                     join l in _db.LeaveTypes on c.LeaveTypeId equals l.LeaveTypeId
                                                     where c.IsCompleted && c.LeaveGroupId == emp.LeaveGroupId
                                                     select new
                                                     {
                                                         l.LeaveTypeId,
                                                         l.LeaveTypeName,
                                                         c.Quota,
                                                         c.QuotaCount,
                                                         l.IsReasonRequired,
                                                         l.RestrictToG,
                                                         l.RestrictToS,
                                                         l.Gender,
                                                         l.Status,
                                                         c.IsQuotaLimit,
                                                     }).ToListAsync();
                        {
                            if (leaveComponents.Any(x => x.RestrictToG))
                            {
                                var check = leaveComponents.Where(x => x.RestrictToG).ToList();
                                var removeLeave = check.Where(x => x.Gender != emp.Gender).Select(x => x.LeaveTypeId).ToList();
                                if (check.Count > 0)
                                    leaveComponents.RemoveAll(x => removeLeave.Contains(x.LeaveTypeId));
                            }
                            if (leaveComponents.Any(x => x.RestrictToS))
                            {
                                var check = leaveComponents.Where(x => x.RestrictToS).ToList();
                                var removeLeave = check.Where(x => x.Status != emp.MaritalStatus).Select(x => x.LeaveTypeId).ToList();
                                if (check.Count > 0)
                                    leaveComponents.RemoveAll(x => removeLeave.Contains(x.LeaveTypeId));
                            }
                        }
                        if (leaveComponents.Count > 0)
                        {
                            if (forInternal)
                            {
                                res.Message = "Your Leave Types";
                                res.Status = true;
                                res.Data = leaveComponents
                                    .Select(x => new ForInternalHelperClass
                                    {
                                        LeaveTypeId = x.LeaveTypeId,
                                        LeaveTypeName = x.LeaveTypeName,
                                        QuataCount = x.QuotaCount,
                                        Quota = x.Quota,
                                        IsQuataLimit = x.IsQuotaLimit,
                                        IsReasonRequired = x.IsReasonRequired,
                                    }).ToList();
                            }
                            else
                            {
                                res.Message = "Your Leave Types";
                                res.Status = true;
                                res.Data = leaveComponents;
                            }
                        }
                        else
                        {
                            res.Message = "No Leave Policy Assign";
                            res.Status = false;
                            res.Data = leaveComponents;
                        }
                    }
                    else
                    {
                        res.Message = "No Leave Policy Assign";
                        res.Status = false;
                        res.Data = new List<string>();
                    }
                }
                else
                {
                    res.Message = "Employee Not Found";
                    res.Status = false;
                    res.Data = new List<string>();
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        /// <summary>
        /// Created By Harshit Mitra On 23-07-2022
        /// </summary>
        public class ForInternalHelperClass
        {
            public int LeaveTypeId { get; set; }
            public string LeaveTypeName { get; set; }
            public string Quota { get; set; }
            public int QuataCount { get; set; }
            public bool IsQuataLimit { get; set; }
            public bool IsReasonRequired { get; set; }
        }

        #endregion API To Get Leave List of Logged-In Employee

        #region API To Get Leave List Graph Of Employee

        /// <summary>
        /// Created By Harshit Mita On 25-07-202leavetypebalance
        /// API >> Get >> api/leavemaster/getleavegraphofemployee
        /// </summary>
        [HttpGet]
        [Route("getleavegraphofemployee")]
        public async Task<ResponseBodyModel> GetLeaveGraphofEmployee()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await GetEmployeeLeaveList(true, claims.employeeId);
                if (data.Status)
                {
                    var leaveType = (List<ForInternalHelperClass>)data.Data;
                    var leaveTypeIds = leaveType.ConvertAll(x => x.LeaveTypeId);
                    var g = _db.GlobalLeave.Where(x => x.CompanyId == claims.companyId && x.CurrentActive).FirstOrDefault();
                    var leaveRequest = await _db.LeaveRequests.Where(x => leaveTypeIds.Contains(x.LeaveTypeId) &&
                                x.RequestedBy == claims.employeeId && x.CompanyId == claims.companyId && x.Status !=
                                LeaveStatusConstants.Rejected && x.Status != LeaveStatusConstants.Cancel && x.IsActive && !x.IsDeleted).ToListAsync();
                    if (leaveRequest.Count > 0)
                    {
                        res.Data = leaveType
                                .ConvertAll(x => new
                                {
                                    LeaveType = x.LeaveTypeName,
                                    IsUnlimited = !x.IsQuataLimit,
                                    Data = new List<object>()
                                    {
                                        new
                                        {
                                            Name = "Avaliable",
                                            Value = x.IsQuataLimit ? x.QuataCount - leaveRequest.Where(z => z.LeaveTypeId == x.LeaveTypeId).Select(z => z.LeaveDay).ToList().Sum() : 0,
                                        },
                                        new
                                        {
                                            Name = "Consumed",
                                            Value = leaveRequest.Where(z => z.LeaveTypeId == x.LeaveTypeId).Select(z => z.LeaveDay).ToList().Sum(),
                                        }
                                    },
                                }).ToList();
                    }
                    else
                    {
                        res.Data = leaveType
                                .ConvertAll(x => new
                                {
                                    LeaveType = x.LeaveTypeName,
                                    IsUnlimited = !x.IsQuataLimit,
                                    Data = new List<object>()
                                    {
                                        new
                                        {
                                            Name = "Avaliable",
                                            Value = x.QuataCount,
                                        },
                                        new
                                        {
                                            Name = "Consumed",
                                            Value = 0,
                                        }
                                    },
                                }).ToList();
                    }
                    res.Message = "Leave Graph";
                    res.Status = true;
                }
                else
                {
                    res.Message = "No Leave Policy Assign";
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

        #endregion API To Get Leave List Graph Of Employee

        #region API To Get Leave List Taken And Remaining Balance of Employee

        /// <summary>
        /// Created By Harshit Mita On 25-07-2022
        /// API >> Get >> api/leavemaster/leavetypebalance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leavetypebalance")]
        public async Task<ResponseBodyModel> LeaveTypeListAndBalance(bool isForCheking = false, int leaveTypeId = 0)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await GetEmployeeLeaveList(true, claims.employeeId);
                if (data.Status)
                {
                    var leaveType = (List<ForInternalHelperClass>)data.Data;
                    var leaveTypeIds = leaveType.ConvertAll(x => x.LeaveTypeId);
                    var leaveRequest = await _db.LeaveRequests.Where(x => leaveTypeIds.Contains(x.LeaveTypeId) &&
                                x.RequestedBy == claims.employeeId && x.CompanyId == claims.companyId && x.Status !=
                                LeaveStatusConstants.Rejected && x.IsActive && !x.IsDeleted).ToListAsync();
                    if (leaveRequest.Count > 0)
                    {
                        var response = leaveType
                                .ConvertAll(x => new
                                {
                                    x.LeaveTypeId,
                                    x.LeaveTypeName,
                                    RemaningQuata = x.IsQuataLimit ? (x.QuataCount - leaveRequest
                                            .Where(z => z.LeaveTypeId == x.LeaveTypeId && z.Status !=
                                LeaveStatusConstants.Rejected && z.Status != LeaveStatusConstants.Cancel).Select(z => z.LeaveDay).Sum()).ToString() : "Un-Limited",
                                    Remaning = x.IsQuataLimit ? (x.QuataCount - leaveRequest
                                            .Where(z => z.LeaveTypeId == x.LeaveTypeId &&
                                           z.Status != LeaveStatusConstants.Rejected && z.Status != LeaveStatusConstants.Cancel).Select(z => z.LeaveDay).Sum()) : 0.0,
                                    IsUnlimited = !x.IsQuataLimit,
                                    x.IsReasonRequired,
                                }).ToList();
                        res.Data = isForCheking
                            ? !response.Where(x => x.LeaveTypeId == leaveTypeId).Select(x => x.IsUnlimited).First() ?
                                    response.Where(x => x.LeaveTypeId == leaveTypeId).Select(x => x.Remaning).FirstOrDefault() : 100000
                            : (object)response;
                    }
                    else
                    {
                        var response = leaveType
                                .ConvertAll(x => new
                                {
                                    x.LeaveTypeId,
                                    x.LeaveTypeName,
                                    RemaningQuata = x.IsQuataLimit ? x.QuataCount.ToString() : "Un-Limited",
                                    Remaning = x.IsQuataLimit ? x.QuataCount : 0.0,
                                    IsUnlimited = !x.IsQuataLimit,
                                    x.IsReasonRequired,
                                }).ToList();
                        res.Data = isForCheking
                            ? !response.Where(x => x.LeaveTypeId == leaveTypeId).Select(x => x.IsUnlimited).First() ?
                                    response.Where(x => x.LeaveTypeId == leaveTypeId).Select(x => x.Remaning).FirstOrDefault() : 100000
                            : (object)response;
                    }
                    res.Message = "Leave Type Balance";
                    res.Status = true;
                }
                else
                {
                    res.Message = data.Message;
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

        #endregion API To Get Leave List Taken And Remaining Balance of Employee

        #region API To Apply For Leave

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// API >> Post >> api/leavemaster/applyleave
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("applyleave")]
        public async Task<ResponseBodyModel> ApplyLeaveRequest(LeaveApplyHelperModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = DateTime.Now.Date;
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    var check = await LeaveTypeListAndBalance(true, model.LeaveTypeId);
                    if (((double)check.Data >= model.LeaveDay))
                    {
                        var checkSameDay = await _db.LeaveRequests.Where(x => x.IsActive && !x.IsDeleted
                                && x.CompanyId == claims.companyId && x.RequestedBy == claims.employeeId
                                && DbFunctions.TruncateTime(x.ToDate) >= today && x.Status == LeaveStatusConstants.Pending).ToListAsync();
                        if (checkSameDay.Count > 0)
                        {
                            List<DateTime> dates = new List<DateTime>();
                            List<DayOfWeek> dayList = new List<DayOfWeek>();
                            List<DateTime> dateList = new List<DateTime>();
                            foreach (var item in checkSameDay)
                            {
                                _ = GetDatesBetween(item.FromDate, item.ToDate, dayList, dateList);
                                dates.AddRange(dateList);
                            }
                            if (dates.Contains(model.FromDate) || dates.Contains(model.ToDate))
                            {
                                res.Message = "Already On Leave That Day";
                                res.Status = false;
                                return res;
                            }
                        }
                        LeaveRequest obj = new LeaveRequest
                        {
                            LeaveTypeId = model.LeaveTypeId,
                            FromDate = model.FromDate,
                            ToDate = model.ToDate,
                            CaseA = model.CaseA,
                            CaseB = model.CaseB,
                            RequestDay = model.RequestDay,
                            LeaveDay = model.LeaveDay,
                            Details = model.Details,
                            ReportingManagerId = model.ReportingManagerId,
                            Documents = model.DocumentURL,
                            Status = LeaveStatusConstants.Pending,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            RequestedBy = claims.employeeId,
                            LeaveOnSameDay = (model.FromDate == model.ToDate),
                            CreatedOn = DateTime.Now
                        };
                        _db.LeaveRequests.Add(obj);
                        await _db.SaveChangesAsync();

                        LeaveRequestHistory his = new LeaveRequestHistory
                        {
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            LeaveRequestId = obj.LeaveRequestId,
                            IsActive = true,
                            IsDeleted = false,
                            Message = "Leave applied by " + _db.GetEmployeeNameById(claims.employeeId) +
                                " on " + DateTime.Today.ToLongDateString() + " for total " + model.LeaveDay,
                            Status = LeaveStatusConstants.Pending,
                        };
                        _db.LeaveReqHistories.Add(his);
                        await _db.SaveChangesAsync();

                        res.Message = "Leave Request Added";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "You Dont Have Leave Quata \nPlease Select Another Leave Or Unpaid Leave";
                        res.Status = false;
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

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// </summary>
        public class LeaveApplyHelperModel
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public LeaveCase? CaseA { get; set; }
            public LeaveCase? CaseB { get; set; }
            public int LeaveTypeId { get; set; }
            public double RequestDay { get; set; }
            public double LeaveDay { get; set; }
            public string Details { get; set; }
            public int ReportingManagerId { get; set; }
            public string DocumentURL { get; set; }
        }

        #endregion API To Apply For Leave

        #region API To Count Total Days While Taking Leave

        /// <summary>
        /// Created By Harshit Mitra On 25-07-2022
        /// API >> Get >> api/leavemaster/countleavedays
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("countleavedays")]
        public async Task<ResponseBodyModel> CountLeaveDays(DateTime fromDate, DateTime toDate, LeaveCase? caseA = null, LeaveCase? caseB = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                CheckLeaveDaysModel data = new CheckLeaveDaysModel();
                var checkEmployee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
                if (fromDate.Date == toDate.Date)
                {
                    var holidays = await _db.Holidays.Where(x => DbFunctions.TruncateTime(x.HolidayDate) == fromDate.Date && x.CompanyId == claims.companyId).ToListAsync();
                    var weekOff = await _db.WeekOffDaysCases.Include("Group").Where(x => x.Group.WeekOffId == (Guid)checkEmployee.WeekOffId).ToListAsync();
                    //if (fromDate.DayOfWeek == DayOfWeek.Sunday || fromDate.DayOfWeek == DayOfWeek.Saturday)
                    if (weekOff.Any(x => x.DayId == fromDate.DayOfWeek && x.CaseId == WeekOffCase.All_Week && x.CaseResponseId != WeekOffDayConstants.Not_Set))
                    {
                        data.RequestDay = 1;
                        data.LeaveDay = 0;
                        data.IsDual = false;
                        data.IsWorking = false;
                    }
                    else if (holidays.Count > 0)
                    {
                        data.RequestDay = 1;
                        data.LeaveDay = 0;
                        data.IsDual = false;
                        data.IsWorking = false;
                    }
                    else
                    {
                        data.RequestDay = 1;
                        data.LeaveDay = 1;
                        data.IsDual = false;
                        data.IsWorking = true;
                    }
                    if (caseA.HasValue)
                    {
                        if (data.LeaveDay != 0)
                        {
                            switch ((LeaveCase)caseA)
                            {
                                case LeaveCase.Second_Half:
                                    data.LeaveDay -= 0.5;
                                    break;

                                case LeaveCase.First_Half:
                                    data.LeaveDay -= 0.5;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    var holidays = await _db.Holidays.Where(x => DbFunctions.TruncateTime(x.HolidayDate) <= fromDate.Date &&
                            DbFunctions.TruncateTime(x.HolidayDate) >= toDate.Date && x.CompanyId == claims.companyId).ToListAsync();
                    var weekOff = await _db.WeekOffDaysCases.Include("Group").Where(x => x.Group.WeekOffId == (Guid)checkEmployee.WeekOffId).ToListAsync();
                    List<DateTime> list = new List<DateTime>();
                    double totalDays = (toDate - fromDate).Days + 1;
                    for (int i = 0; i < totalDays; i++)
                    {
                        list.Add(fromDate.AddDays(i).Date);
                    }
                    if (holidays.Count > 0)
                    {
                        var holidayDate = holidays.Select(x => x.HolidayDate.Date).ToList();
                        list.RemoveAll(x => holidayDate.Contains(x));
                    }
                    var removeweekoffs = weekOff.Where(x => x.CaseId == WeekOffCase.All_Week && x.CaseResponseId != WeekOffDayConstants.Not_Set).Select(x => x.DayId).ToList();
                    list.RemoveAll(x => removeweekoffs.Contains(x.DayOfWeek));

                    if (list.Count > 0)
                    {
                        data.RequestDay = totalDays;
                        data.LeaveDay = list.Count;
                        data.IsDual = true;
                        data.IsWorking = false;
                    }
                    else
                    {
                        data.RequestDay = totalDays;
                        data.LeaveDay = 0;
                        data.IsDual = false;
                        data.IsWorking = false;
                    }
                    if (caseB.HasValue && caseA.HasValue)
                    {
                        if ((LeaveCase)caseB == LeaveCase.Second_Half && (LeaveCase)caseA == LeaveCase.Second_Half)
                        {
                            data.RequestDay -= 0.5;
                            data.LeaveDay -= 0.5;
                        }
                        if ((LeaveCase)caseB == LeaveCase.First_Half && (LeaveCase)caseA == LeaveCase.First_Half)
                        {
                            data.RequestDay -= 0.5;
                            data.LeaveDay -= 0.5;
                        }
                        if ((LeaveCase)caseB == LeaveCase.First_Half && (LeaveCase)caseA == LeaveCase.Second_Half)
                        {
                            data.RequestDay -= 1;
                            data.LeaveDay -= 1;
                        }
                    }
                    if (caseA.HasValue && !caseB.HasValue)
                    {
                        if ((LeaveCase)caseA == LeaveCase.Second_Half)
                        {
                            data.RequestDay -= 0.5;
                            data.LeaveDay -= 0.5;
                        }
                    }
                    if (caseB.HasValue && !caseA.HasValue)
                    {
                        if ((LeaveCase)caseB == LeaveCase.First_Half)
                        {
                            data.RequestDay -= 0.5;
                            data.LeaveDay -= 0.5;
                        }
                    }
                }

                //data.LeaveDay = Math.Round(data.LeaveDay, 1, MidpointRounding.AwayFromZero);

                res.Message = "Date Check";
                res.Status = true;
                res.Data = data;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        /// <summary>
        /// Created By Harshit Mitra on 25-07-2022
        /// </summary>
        public class CheckLeaveDaysModel
        {
            public double RequestDay { get; set; }
            public double LeaveDay { get; set; }
            public bool IsDual { get; set; }
            public bool IsWorking { get; set; }
        }

        #endregion API To Count Total Days While Taking Leave

        #region API To Get Reporting Manager On Leave

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// API >> api/leavemaster/leavereportingmanagers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leavereportingmanagers")]
        public async Task<ResponseBodyModel> GetReportingManagerOnLeave()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var emp = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId && x.CompanyId == claims.companyId);
                if (emp != null)
                {
                    var managers = _db.Employee.Where(x => x.EmployeeId == emp.ReportingManager).Select(x => x.ReportingManager).ToList();

                    if (managers.Count > 0)
                    {
                        var managersList = managers
                                .Select(x => new
                                {
                                    ManagerId = emp.ReportingManager,
                                    ManagerName = _db.GetEmployeeNameById(emp.ReportingManager),
                                }).ToList();

                        res.Message = "Managers List";
                        res.Status = true;
                        res.Data = managersList;
                    }
                    else
                    {
                        res.Message = "No Managers Assign";
                        res.Status = false;
                        res.Data = managers;
                    }
                }
                else
                {
                    res.Message = "Employee Not Found";
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

        #endregion API To Get Reporting Manager On Leave

        #region API To Get Leave Status Type

        /// <summary>
        /// Created By Harshit Mitra on 03-08-2022
        /// API >> Get >> api/leavemaster/getstatustype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getstatustype")]
        public ResponseBodyModel GetStatusType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var status = Enum.GetValues(typeof(LeaveStatusConstants))
                                .Cast<LeaveStatusConstants>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Name = Enum.GetName(typeof(LeaveStatusConstants), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Status List";
                res.Status = true;
                res.Data = status;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Status Type

        #region API To Get Leave List Self Profile

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// Modified By Harshit Mitra On 04-08-2022
        /// API >> Get >> api/leavemaster/getselfleave
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getselfleave")]
        public async Task<ResponseBodyModel> GetSelfAppliedLeave(LeaveStatusConstants status, PagingRequest paging)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveData = await _db.LeaveRequests.Where(x => x.RequestedBy == claims.employeeId &&
                            x.IsDeleted == false && x.IsActive == true && x.Status == status).ToListAsync();
                if (leaveData.Count > 0)
                {
                    var leaveType = _db.LeaveTypes.Where(x => x.IsActive == true && x.CompanyId == claims.companyId && x.IsDeleted == false).ToList();

                    var leaves = leaveData
                            .Select(x => new
                            {
                                x.LeaveRequestId,
                                AppliedBy = _db.GetEmployeeNameById(x.RequestedBy),
                                x.FromDate,
                                x.ToDate,
                                x.CreatedOn,
                                From = x.FromDate.Date == x.ToDate ? "For " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " ") + " Only" :
                                        "Form " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " ") + " To " + Enum.GetName(typeof(LeaveCase), x.CaseB).Replace("_", " "),
                                x.Details,
                                x.LeaveDay,
                                StatusEnum = x.Status,
                                Status = Enum.GetName(typeof(LeaveStatusConstants), x.Status).Replace("_", " "),
                                x.LeaveTypeId,
                                LeaveType = leaveType.Where(z => z.LeaveTypeId == x.LeaveTypeId).Select(z => z.LeaveTypeName).FirstOrDefault(),
                                x.ReportingManagerId,
                                ReportingTo = _db.GetEmployeeNameById(x.ReportingManagerId),
                            }).ToList();

                    res.Message = "Leave Request List";
                    res.Status = true;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveData.Count,
                        Counts = paging.Count,
                        List = leaves.Skip((paging.Page - 1) * paging.Count).Take(paging.Count).ToList(),
                    };
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveData.Count,
                        Counts = paging.Count,
                        List = new List<int>(),
                    }; ;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave List Self Profile

        #region API To Get Leave List On Reporting Managers

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// Modified By Harshit Mitra On 04-08-2022
        /// API >> api/leavemaster/getleaveonmanager
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getleaveonmanager")]
        public async Task<ResponseBodyModel> GetLeaveListOnManager(LeaveStatusConstants status, PagingRequest paging)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveData = await _db.LeaveRequests.Where(x => x.IsDeleted == false && x.IsActive == true &&
                            x.ReportingManagerId == claims.employeeId && x.Status == status).ToListAsync();
                if (leaveData.Count > 0)
                {
                    var leaveType = _db.LeaveTypes.Where(x => x.IsActive == true && x.CompanyId == claims.companyId && x.IsDeleted == false).ToList();

                    var leaves = leaveData
                                .Select(x => new
                                {
                                    x.LeaveRequestId,
                                    AppliedBy = _db.GetEmployeeNameById(x.RequestedBy),
                                    x.FromDate,
                                    x.ToDate,
                                    x.CreatedOn,
                                    From = x.FromDate.Date == x.ToDate ? "For " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " ") + " Only" :
                                            "Form " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " ") + " To " + Enum.GetName(typeof(LeaveCase), x.CaseB).Replace("_", " "),
                                    x.Details,
                                    x.LeaveDay,
                                    StatusEnum = x.Status,
                                    Status = Enum.GetName(typeof(LeaveStatusConstants), x.Status).Replace("_", " "),
                                    x.LeaveTypeId,
                                    LeaveType = leaveType.Where(z => z.LeaveTypeId == x.LeaveTypeId).Select(z => z.LeaveTypeName).FirstOrDefault(),
                                    x.ReportingManagerId,
                                    ReportingTo = _db.GetEmployeeNameById(x.ReportingManagerId),
                                }).ToList();

                    res.Message = "Leave Request List";
                    res.Status = true;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveData.Count,
                        Counts = paging.Count,
                        List = leaves.Skip((paging.Page - 1) * paging.Count).Take(paging.Count).ToList(),
                    };
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveData.Count,
                        Counts = paging.Count,
                        List = new List<int>(),
                    };
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave List On Reporting Managers

        #region API To Get Leave List On HR

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// Modified By Harshit Mitra On 04-08-2022
        /// API >> Post >> api/leavemaster/getleaveonhr
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getleaveonhr")]
        public async Task<ResponseBodyModel> GetLeaveListOnHR(LeaveStatusConstants status, PagingRequest paging)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveData = await _db.LeaveRequests.Where(x => x.CompanyId == claims.companyId
                        && x.IsDeleted == false && x.IsActive == true && x.Status == status).ToListAsync();
                if (leaveData.Count > 0)
                {
                    var leaveType = _db.LeaveTypes.Where(x => x.IsActive == true && x.CompanyId == claims.companyId && x.IsDeleted == false).ToList();

                    var leaves = leaveData
                            .Select(x => new
                            {
                                x.LeaveRequestId,
                                AppliedBy = _db.GetEmployeeNameById(x.RequestedBy),
                                x.FromDate,
                                x.ToDate,
                                x.CreatedOn,
                                From = x.FromDate.Date == x.ToDate ? "For " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " ") + " Only" :
                                        "For " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " ") + " To " + Enum.GetName(typeof(LeaveCase), x.CaseA).Replace("_", " "),
                                x.Details,
                                x.LeaveDay,
                                StatusEnum = x.Status,
                                Status = Enum.GetName(typeof(LeaveStatusConstants), x.Status).Replace("_", " "),
                                x.LeaveTypeId,
                                LeaveType = leaveType.Where(z => z.LeaveTypeId == x.LeaveTypeId).Select(z => z.LeaveTypeName).FirstOrDefault(),
                                x.ReportingManagerId,
                                ReportingTo = _db.GetEmployeeNameById(x.ReportingManagerId),
                            }).ToList().OrderByDescending(x => x.CreatedOn).ToList();

                    res.Message = "Leave Request List";
                    res.Status = true;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveData.Count,
                        Counts = paging.Count,
                        List = leaves.Skip((paging.Page - 1) * paging.Count).Take(paging.Count).ToList(),
                    };
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveData.Count,
                        Counts = paging.Count,
                        List = new List<int>(),
                    }; ;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave List On HR

        #region API To Reject Leave

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// API >> api/leavemaster/rejectleave
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("rejectleave")]
        public async Task<ResponseBodyModel> RejectLeave(ApprovalLeaveClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId
                            && x.IsDeleted == false && x.IsActive == true && x.LeaveRequestId == model.LeaveRequestId);
                if (leaveRequest != null)
                {
                    leaveRequest.UpdatedOn = DateTime.Now;
                    leaveRequest.UpdatedBy = claims.employeeId;
                    leaveRequest.Status = LeaveStatusConstants.Rejected;
                    leaveRequest.RejectReason = model.RejectReason;

                    _db.Entry(leaveRequest).State = System.Data.Entity.EntityState.Modified;

                    LeaveRequestHistory his = new LeaveRequestHistory
                    {
                        CompanyId = claims.companyId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        LeaveRequestId = leaveRequest.LeaveRequestId,
                        IsActive = true,
                        IsDeleted = false,
                        Message = "Leave rejected by " + _db.GetEmployeeNameById(claims.employeeId) +
                            " on " + DateTime.Today.ToLongDateString(),
                    };
                    _db.LeaveReqHistories.Add(his);
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Rejected";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Leave Request Not Found";
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

        #endregion API To Reject Leave

        #region API To Approve Leave Partially

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// API >> api/leavemaster/approvepartiallyleave
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("approvepartiallyleave")]
        public async Task<ResponseBodyModel> ApproveLeavePartially(ApprovalLeaveClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId
                            && x.IsDeleted == false && x.IsActive == true && x.LeaveRequestId == model.LeaveRequestId);
                if (leaveRequest != null)
                {
                    leaveRequest.UpdatedOn = DateTime.Now;
                    leaveRequest.UpdatedBy = claims.employeeId;
                    leaveRequest.Status = LeaveStatusConstants.Partially_Approved;

                    _db.Entry(leaveRequest).State = System.Data.Entity.EntityState.Modified;

                    LeaveRequestHistory his = new LeaveRequestHistory
                    {
                        CompanyId = claims.companyId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        LeaveRequestId = leaveRequest.LeaveRequestId,
                        IsActive = true,
                        IsDeleted = false,
                        Message = "Leave partially approved by " + _db.GetEmployeeNameById(claims.employeeId) +
                            " on " + DateTime.Today.ToLongDateString(),
                    };
                    _db.LeaveReqHistories.Add(his);
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Approved";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Leave Request Not Found";
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

        #endregion API To Approve Leave Partially

        #region API To Approve Leave By HR

        /// <summary>
        /// Created By Harshit Mitra On 26-07-2022
        /// API >> api/leavemaster/approveleavehr
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("approveleavehr")]
        public async Task<ResponseBodyModel> ApprovePartially(ApprovalLeaveClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId
                            && x.IsDeleted == false && x.IsActive == true && x.LeaveRequestId == model.LeaveRequestId);
                if (leaveRequest != null)
                {
                    leaveRequest.UpdatedOn = DateTime.Now;
                    leaveRequest.UpdatedBy = claims.employeeId;
                    leaveRequest.Status = LeaveStatusConstants.Approved;

                    _db.Entry(leaveRequest).State = System.Data.Entity.EntityState.Modified;

                    LeaveRequestHistory his = new LeaveRequestHistory
                    {
                        CompanyId = claims.companyId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        LeaveRequestId = leaveRequest.LeaveRequestId,
                        IsActive = true,
                        IsDeleted = false,
                        Message = "Leave approved by " + _db.GetEmployeeNameById(claims.employeeId) +
                            " on " + DateTime.Today.ToLongDateString(),
                    };
                    _db.LeaveReqHistories.Add(his);
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Approved";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Leave Request Not Found";
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

        #endregion API To Approve Leave By HR

        #region API To Cancel Leave By User

        /// <summary>
        /// Created By Harshit Mitra On 27-07-2022
        /// API >> api/leavemaster/cancleleave
        /// </summary>
        [HttpPut]
        [Route("cancleleave")]
        public async Task<ResponseBodyModel> CancleLeave(ApprovalLeaveClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId
                            && x.IsDeleted == false && x.IsActive == true && x.LeaveRequestId == model.LeaveRequestId);
                if (leaveRequest != null)
                {
                    if (leaveRequest.Status != LeaveStatusConstants.Approved)
                    {
                        leaveRequest.UpdatedOn = DateTime.Now;
                        leaveRequest.UpdatedBy = claims.employeeId;
                        leaveRequest.Status = LeaveStatusConstants.Cancel;

                        _db.Entry(leaveRequest).State = System.Data.Entity.EntityState.Modified;

                        LeaveRequestHistory his = new LeaveRequestHistory
                        {
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            LeaveRequestId = leaveRequest.LeaveRequestId,
                            IsActive = true,
                            IsDeleted = false,
                            Message = "Leave cancle by " + _db.GetEmployeeNameById(claims.employeeId) +
                                " on " + DateTime.Today.ToLongDateString(),
                        };
                        _db.LeaveReqHistories.Add(his);
                        await _db.SaveChangesAsync();

                        res.Message = "Leave Cancled";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "You can't cancle your leave because its allready approved";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Leave Request Not Found";
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

        #endregion API To Cancel Leave By User

        #region API To Get Leave Dashboard Of User

        /// <summary>
        /// Created By Harshit Mitra On 28-07-2022
        /// API >> Get >> api/leavemaster/userleavedashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("userleavedashboard")]
        public async Task<ResponseBodyModel> GetUserLeaveDashboard()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var emp = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
                if (emp != null)
                {
                    if (emp.LeaveGroupId != 0)
                    {
                        var leaveRequest = await _db.LeaveRequests.Where(x => x.RequestedBy == emp.EmployeeId &&
                                x.Status != LeaveStatusConstants.Rejected && x.Status != LeaveStatusConstants.Cancel).ToListAsync();
                        if (leaveRequest.Count > 0)
                        {
                            var dateList = new List<DateTime>();
                            var dayOfWeeks = new List<DayOfWeek>();

                            leaveRequest.ForEach(x => GetDatesBetween(x.FromDate, x.ToDate, dayOfWeeks, dateList));

                            #region Region To Get Leave Week

                            dayOfWeeks.RemoveAll(x => x == DayOfWeek.Sunday || x == DayOfWeek.Saturday);
                            var weekData = Enum.GetValues(typeof(DayOfWeek))
                                    .Cast<DayOfWeek>()
                                    .Select(x => new
                                    {
                                        DayOfWeek = x,
                                        Name = Enum.GetName(typeof(DayOfWeek), x),
                                        Value = dayOfWeeks.Where(d => d == x).ToList().Count,
                                    }).OrderBy(x => ((int)x.DayOfWeek + 6) % 7)
                                    .Select(x => new
                                    {
                                        x.Name,
                                        x.Value,
                                    }).ToList();

                            #endregion Region To Get Leave Week

                            #region Region To Get Month Leave

                            var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                                         .TakeWhile(m => m != String.Empty)
                                         .Select((m, i) => new
                                         {
                                             Month = i + 1,
                                             MonthName = m
                                         }).ToList();
                            dateList.RemoveAll(x => x.DayOfWeek == DayOfWeek.Sunday || x.DayOfWeek == DayOfWeek.Saturday);
                            var monthData = months
                                        .Select(x => new
                                        {
                                            Name = x.MonthName,
                                            Value = dateList.Where(z => z.Month == x.Month).ToList().Count,
                                        }).ToList();

                            #endregion Region To Get Month Leave

                            res.Message = "User Dashboard Data";
                            res.Status = true;
                            res.Data = new
                            {
                                WeekGraph = weekData,
                                MonthGraph = monthData,
                            };
                        }
                        else
                        {
                            res.Message = "No Leave Taken";
                            res.Status = false;
                            res.Data = new
                            {
                                WeekGraph = new List<int>(),
                                MonthGraph = new List<int>(),
                            };
                        }
                    }
                    else
                    {
                        res.Message = "No Leave Policy Assign";
                        res.Status = false;
                        res.Data = new
                        {
                            WeekGraph = new List<int>(),
                            MonthGraph = new List<int>(),
                        };
                    }
                }
                else
                {
                    res.Message = "Employee Not Found";
                    res.Status = false;
                    res.Data = new
                    {
                        WeekGraph = new List<int>(),
                        MonthGraph = new List<int>(),
                    };
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public List<DayOfWeek> GetDatesBetween(DateTime startDate, DateTime endDate, List<DayOfWeek> dayList, List<DateTime> dateList)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dateList.Add(date);
                dayList.Add(date.DayOfWeek);
            }
            return dayList;
        }

        #endregion API To Get Leave Dashboard Of User

        #region API To Get Overall Leave Dashboard

        /// <summary>
        /// Created By Harshit Mitra On 01-08-2022
        /// API >> Get >> api/leavemaster/leavedashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leavedashboard")]
        public async Task<ResponseBodyModel> LeaveDashboard(int year, int? month = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.Where(x => claims.companyId == x.CompanyId).ToListAsync();

                #region Leave Line Graph

                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                         .TakeWhile(m => m != String.Empty)
                                         .Select((m, i) => new
                                         {
                                             Month = i + 1,
                                             MonthName = m
                                         }).ToList();
                var monthData = months
                            .Select(x => new
                            {
                                Name = x.MonthName,
                                Value = leaveRequest.Where(z => z.FromDate.Month == x.Month &&
                                         z.Status != LeaveStatusConstants.Rejected &&
                                         z.Status != LeaveStatusConstants.Cancel).ToList().Count,
                            }).ToList();
                var rejectLeave = months
                            .Select(x => new
                            {
                                Name = x.MonthName,
                                Value = leaveRequest.Where(z => z.FromDate.Month == x.Month &&
                                         (z.Status == LeaveStatusConstants.Rejected ||
                                         z.Status == LeaveStatusConstants.Cancel)).ToList().Count,
                            }).ToList();
                var employeeOnLeave = months
                            .Select(x => new
                            {
                                Name = x.MonthName,
                                Value = leaveRequest.Where(z => z.FromDate.Month == x.Month &&
                                         (z.Status != LeaveStatusConstants.Rejected &&
                                         z.Status != LeaveStatusConstants.Cancel) &&
                                         z.FromDate.Year == year)
                                        .Select(z => z.RequestedBy).Distinct().ToList().Count,
                            }).ToList();

                #endregion Leave Line Graph

                #region Leave Details Data

                var previousMonth = new List<LeaveRequest>();
                var currentMonth = new List<LeaveRequest>();
                if (month.HasValue)
                {
                    if (month != 1)
                    {
                        previousMonth = leaveRequest.Where(x => x.FromDate.Year == year && x.FromDate.Month == (month - 1)).ToList();
                    }
                }
                else
                {
                    if (month != 1)
                    {
                        previousMonth = leaveRequest.Where(x => x.FromDate.Year == (year - 1)).ToList();
                    }
                }
                if (month.HasValue)
                {
                    currentMonth = leaveRequest.Where(x => x.FromDate.Year == year && x.FromDate.Month == month).ToList();
                }
                else
                {
                    currentMonth = leaveRequest.Where(x => x.FromDate.Year == year).ToList();
                }

                var box = new List<LeaveBox>
                {
                    new LeaveBox
                    {
                        Name = "Requested",
                        New = currentMonth.Count,
                        Old = previousMonth.Count > 0 ? previousMonth.Count : 0,
                        Check = "",
                    },
                    new LeaveBox
                    {
                        Name = "Approved",
                        New = currentMonth.Where(x=> x.Status == LeaveStatusConstants.Approved).ToList().Count,
                        Old = previousMonth.Count > 0 ? previousMonth.Where(x=> x.Status == LeaveStatusConstants.Approved).ToList().Count : 0,
                        Check= "",
                    },
                    new LeaveBox
                    {
                        Name = "Rejected",
                        New = currentMonth.Where(x=> x.Status == LeaveStatusConstants.Rejected).ToList().Count,
                        Old = previousMonth.Count > 0 ? previousMonth.Where(x=> x.Status == LeaveStatusConstants.Rejected).ToList().Count : 0,
                        Check = "",
                    },
                    new LeaveBox
                    {
                        Name = "Cancel",
                        New = currentMonth.Where(x=> x.Status == LeaveStatusConstants.Cancel).ToList().Count,
                        Old = previousMonth.Count > 0 ? previousMonth.Where(x=> x.Status == LeaveStatusConstants.Cancel).ToList().Count : 0,
                        Check = "",
                    },
                    new LeaveBox
                    {
                        Name = "Partially Approved",
                        New = currentMonth.Where(x=> x.Status == LeaveStatusConstants.Partially_Approved).ToList().Count,
                        Old = previousMonth.Count > 0 ? previousMonth.Where(x=> x.Status == LeaveStatusConstants.Partially_Approved).ToList().Count : 0,
                        Check = "",
                    },
                    new LeaveBox
                    {
                        Name = "Pending",
                        New = currentMonth.Where(x=> x.Status == LeaveStatusConstants.Pending).ToList().Count,
                        Old = previousMonth.Count > 0 ? previousMonth.Where(x=> x.Status == LeaveStatusConstants.Pending).ToList().Count : 0,
                        Check = "",
                    }
                };

                box.ForEach(x => x.Check = CheckPosition(x.Old, x.New));

                #endregion Leave Details Data

                #region Pie Chart Data

                var leaveType = await _db.LeaveTypes.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();
                var pieChart = leaveType
                        .Select(x => new
                        {
                            Name = x.LeaveTypeName,
                            Value = leaveRequest.Where(z => z.LeaveTypeId == x.LeaveTypeId).ToList().Count,
                        }).ToList();
                List<LeaveRequest> monthLeaveRequest = new List<LeaveRequest>();
                if (month.HasValue)
                {
                    monthLeaveRequest = leaveRequest.Where(x => x.FromDate.Month == month && x.FromDate.Year == year).ToList();
                }
                else
                {
                    monthLeaveRequest = leaveRequest.Where(x => x.FromDate.Year == year).ToList();
                }
                var monthPieChart = leaveType
                        .Select(x => new
                        {
                            Name = x.LeaveTypeName,
                            Value = monthLeaveRequest.Where(z => z.LeaveTypeId == x.LeaveTypeId).ToList().Count,
                        }).ToList();
                Random rnd = new Random();
                var colorGorup = leaveType
                        .Select(x => new
                        {
                            Name = x.LeaveTypeName,
                            Value = String.Format("#{0:X6}", rnd.Next(0x1000000)),
                        }).ToList();

                #endregion Pie Chart Data

                res.Message = "leave Dashboard";
                res.Status = true;
                res.Data = new
                {
                    MonthGraph = new List<object>()
                    {
                        //new
                        //{
                        //    Name = "Reject Leave",
                        //    Series = rejectLeave,
                        //},
                        //new
                        //{
                        //    Name = "Approve Leave",
                        //    Series = monthData,
                        //},
                        new
                        {
                            Name = "Employee On Leve",
                            Series = employeeOnLeave,
                        },
                    },
                    BoxData = box,
                    ColurCode = colorGorup,
                    OverallPieChart = new
                    {
                        ColourCode = colorGorup,
                        PieChart = pieChart,
                    },
                    DynamicPieChart = new
                    {
                        IsMonthly = month.HasValue,
                        ColourCode = colorGorup,
                        PieChart = monthPieChart,
                    },
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public string CheckPosition(int oldD, int newD)
        {
            var response = "";
            if (oldD == newD)
                response = "EQUAL";
            if (oldD > newD)
                response = "MINUS";
            if (newD > oldD)
                response = "PLUS";
            return response;
        }

        public class LeaveBox
        {
            public string Name { get; set; }
            public int New { get; set; }
            public int Old { get; set; }
            public string Check { get; set; }
        }

        #endregion API To Get Overall Leave Dashboard

        #region API To Get Year List Of Leave Dashboard

        /// <summary>
        /// Created By Harshit Mitra On 01-08-2022
        /// API >> Get >> api/leavemaster/leavedashboardyearlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leavedashboardyearlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardYearList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.Where(x => claims.companyId == x.CompanyId).ToListAsync();
                var checkYear = leaveRequest
                        .Select(x => new
                        {
                            Name = x.FromDate.Year
                        }).Distinct().ToList();
                if (checkYear.Count > 0)
                {
                    res.Message = "Year List";
                    res.Status = true;
                    res.Data = checkYear;
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = checkYear;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Year List Of Leave Dashboard

        #region API To Get Month List Of Leave Dashboard

        /// <summary>
        /// Created By Harshit Mitra On 01-08-2022
        /// API >> Get >> api/leavemaster/leavedashboardmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leavedashboardmonthlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardMonthListAsync(int year)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.Where(x => claims.companyId == x.CompanyId && x.CreatedOn.Year == year)
                            .Select(x => x.CreatedOn.Month).Distinct().OrderBy(x => x).FirstOrDefaultAsync();
                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                        .TakeWhile(m => m != String.Empty)
                                        .Select((m, i) => new
                                        {
                                            Month = i + 1,
                                            MonthName = m
                                        }).ToList();
                months = months.Skip(leaveRequest - 1).ToList();
                if (months.Count > 0)
                {
                    res.Message = "Month List";
                    res.Status = true;
                    res.Data = months;
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = months;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Month List Of Leave Dashboard

        #region API To Get Leave Detailed Dashboard

        /// <summary>
        /// Created By Harshit Mitra On 02-08-2022
        /// API >> Get >> api/leavemaster/newleavedasboard
        /// </summary>
        [HttpGet]
        [Route("newleavedasboard")]
        public async Task<ResponseBodyModel> NewLeaveDashboard(int year, int? month = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveRequest = await _db.LeaveRequests.Where(x => claims.companyId == x.CompanyId && x.FromDate.Year == year).ToListAsync();
                var checkLR = leaveRequest;
                if (month.HasValue)
                    checkLR = leaveRequest.Where(x => x.FromDate.Month == month).ToList();

                #region Box Data

                var resposnseBox = new List<object>()
                {
                    new
                    {
                        Name = "Total Requested",
                        Value = checkLR.Count,
                        Inner = new List<object>()
                        {
                            new
                            {
                                Name = "Responded",
                                Value = checkLR.Where(x=> x.Status != LeaveStatusConstants.Pending).ToList().Count,
                            },
                            new
                            {
                                Name = "Not Responded",
                                Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Pending).ToList().Count,
                            },
                        },
                    },
                    new
                    {
                        Name = "Total Approvals",
                        Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Approved || x.Status == LeaveStatusConstants.Partially_Approved).ToList().Count,
                        Inner = new List<object>()
                        {
                            new
                            {
                                Name = "Approved",
                                Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Approved).ToList().Count,
                            },
                            new
                            {
                                Name = "Partially Approved",
                                Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Partially_Approved).ToList().Count,
                            },
                        },
                    },
                    new
                    {
                        Name = "Total Rejected",
                        Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Rejected || x.Status == LeaveStatusConstants.Cancel).ToList().Count,
                        Inner = new List<object>()
                        {
                            new
                            {
                                Name = "Rejected",
                                Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Rejected).ToList().Count,
                            },
                            new
                            {
                                Name = "Self Cancelled",
                                Value = checkLR.Where(x=> x.Status == LeaveStatusConstants.Cancel).ToList().Count,
                            },
                        },
                    },
                };

                #endregion Box Data

                #region Bar Chart Data

                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                         .TakeWhile(m => m != String.Empty)
                                         .Select((m, i) => new
                                         {
                                             Month = i + 1,
                                             MonthName = m
                                         }).ToList();
                var monthNames = months.ConvertAll(x => x.MonthName);

                var listDynamic = new List<dynamic>();
                var intList1 = new List<int>();
                var data1 = new
                {
                    label = "Requested",
                    stack = "1",
                    data = intList1,
                };
                var intList2 = new List<int>();
                var data2 = new
                {
                    label = "Approved",
                    stack = "2",
                    data = intList2,
                };
                var intList3 = new List<int>();
                var data3 = new
                {
                    label = "Rejected",
                    stack = "3",
                    data = intList3,
                };

                foreach (var item in months)
                {
                    intList1.Add(leaveRequest.Where(x => x.FromDate.Month == item.Month).ToList().Count);
                    intList2.Add(leaveRequest.Where(x => x.FromDate.Month == item.Month && (x.Status == LeaveStatusConstants.Approved
                                || x.Status == LeaveStatusConstants.Partially_Approved)).ToList().Count);
                    intList3.Add(leaveRequest.Where(x => x.FromDate.Month == item.Month && (x.Status == LeaveStatusConstants.Rejected
                                || x.Status == LeaveStatusConstants.Cancel)).ToList().Count);
                }
                listDynamic.Add(data1);
                listDynamic.Add(data2);
                listDynamic.Add(data3);

                #endregion Bar Chart Data

                #region Top Ten Leave Taken Employee

                var employeeLeaveList = (from l in checkLR
                                         join e in _db.Employee on l.RequestedBy equals e.EmployeeId
                                         where e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                         select new
                                         {
                                             e.EmployeeId,
                                             e.DisplayName,
                                             Count = checkLR.Where(x => x.RequestedBy == e.EmployeeId).Select(x => x.LeaveDay).ToList().Sum(),
                                         }).ToList().Distinct().OrderByDescending(x => x.Count).ToList().Take(10).ToList();

                #endregion Top Ten Leave Taken Employee

                #region Pie Chart Data

                var totalEmp = _db.Employee.Where(x => x.CompanyId == claims.companyId && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).ToList().Count;
                var empNotOnLeave = leaveRequest.Where(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today &&
                            x.CompanyId == claims.companyId && (x.Status == LeaveStatusConstants.Partially_Approved || x.Status == LeaveStatusConstants.Approved))
                            .ToList().Count;
                var pieChartData = new
                {
                    data = new List<object>()
                    {
                        new
                        {
                            Name = "Employee Not On Leave",
                            Value = totalEmp - empNotOnLeave,
                        },
                        new
                        {
                            Name = "On Leave Today",
                            Value = empNotOnLeave,
                        }
                    },
                };

                #endregion Pie Chart Data

                res.Message = "Leave Dashboard";
                res.Status = true;
                res.Data = new
                {
                    BoxData = resposnseBox,
                    TopEmployee = employeeLeaveList,
                    BarGraph = new
                    {
                        Label = monthNames,
                        BarData = listDynamic,
                    },
                    PieChartData = pieChartData,
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Detailed Dashboard

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra On 27-07-2022
        /// </summary>
        public class ApprovalLeaveClass
        {
            public int LeaveRequestId { get; set; }
            public string RejectReason { get; set; }
        }

        #endregion Helper Model Class
    }
}