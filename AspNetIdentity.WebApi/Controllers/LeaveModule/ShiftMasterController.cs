using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.LeaveModule
{
    /// <summary>
    /// Created By Harshit Mitra On 28-07-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/shiftmaster")]
    public class ShiftMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region This Is Use To Shift And ShiftTime

        #region API To Get Shift Timing On Add Shift
        /// <summary>
        /// Created By Harshit Mitra On 29-09-2022
        /// API >> Get >> api/shiftmaster/getshifttimingbeforeadd
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getshifttimingbeforeadd")]
        public ResponseBodyModel GetShiftTiming()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                AddShift obj = new AddShift();
                obj.ShiftTiming = Enum.GetValues(typeof(DayOfWeek))
                                .Cast<DayOfWeek>()
                                .Select(x => new AddShiftTiming
                                {
                                    WeekDay = x,
                                    WeekName = x.ToString(),
                                    StartTime = new TimeSpan(09, 0, 0),
                                    EndTime = new TimeSpan(18, 0, 0),
                                    BreakTime = 0

                                })
                                .OrderBy(x => x.WeekDay == DayOfWeek.Sunday).ThenBy(x => x.WeekDay)
                                .ToList();

                res.Message = "New Data";
                res.Status = true;
                res.Data = obj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region API To Add Shift
        /// <summary>
        /// Created By Harshit Mitra On 30-09-2022
        /// API >> api/shiftmaster/addnewshift
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addnewshift")]
        public async Task<ResponseBodyModel> AddNewShift(AddShift model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                ShiftGroup obj = new ShiftGroup
                {
                    ShiftGoupId = Guid.NewGuid(),
                    ShiftName = model.ShiftName,
                    ShiftCode = model.ShiftCode,
                    Description = model.Description,
                    IsFlexible = model.IsFlexible,
                    IsTimingDifferent = model.IsTimingDifferent,
                    IsDurationDifferent = model.IsDurationDifferent,
                    CompanyId = claims.companyId,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,

                };
                _db.ShiftGroups.Add(obj);
                await _db.SaveChangesAsync();
                foreach (var timimgmodel in model.ShiftTiming)
                {
                    ShiftTiming timeObj = new ShiftTiming
                    {
                        ShiftTimingId = Guid.NewGuid(),
                        ShiftGroup = obj,
                        WeekDay = timimgmodel.WeekDay,
                        WeekName = timimgmodel.WeekName,
                        StartTime = timimgmodel.StartTime,
                        EndTime = timimgmodel.EndTime,
                        BreakTime = timimgmodel.BreakTime,
                        CompanyId = claims.companyId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.ShiftTimings.Add(timeObj);
                    await _db.SaveChangesAsync();
                }
                res.Message = "Shift Save";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class AddShift
        {
            public string ShiftName { get; set; }
            public string ShiftCode { get; set; }
            public string Description { get; set; }
            public bool IsFlexible { get; set; }
            public bool IsTimingDifferent { get; set; }
            public bool IsDurationDifferent { get; set; }
            public List<AddShiftTiming> ShiftTiming { get; set; }
        }
        public class AddShiftTiming
        {
            public DayOfWeek WeekDay { get; set; }
            public string WeekName { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public int BreakTime { get; set; }
        }

        #endregion API To Add Shift

        #region Get All Shift Group Timings
        /// <summary>
        /// Created By Harshit Mitra On 30-09-2022
        /// API >> Get >> api/shiftmaster/getshiftgoruptimimg
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getshiftgoruptimimg")]
        public async Task<ResponseBodyModel> GetShiftGroupTimimg()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftGroups = await _db.ShiftGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted)
                            .Select(x => new ShiftGroupResponse
                            {
                                ShiftGoupId = x.ShiftGoupId,
                                ShiftName = x.ShiftName,
                                ShiftCode = x.ShiftCode,
                                Description = x.Description,
                                IsFlexible = x.IsFlexible,
                                IsTimingDifferent = x.IsTimingDifferent,
                                IsDurationDifferent = x.IsDurationDifferent,
                                ShiftTiming = _db.ShiftTimings.Where(z => z.ShiftGroup == x)
                                        .Select(z => new AddShiftTiming
                                        {
                                            WeekDay = z.WeekDay,
                                            WeekName = z.WeekName,
                                            StartTime = z.StartTime,
                                            EndTime = z.EndTime,
                                            BreakTime = z.BreakTime,
                                        })
                                        .OrderBy(z => z.WeekDay == DayOfWeek.Sunday).ThenBy(z => z.WeekDay)
                                        .ToList(),

                            }).ToListAsync();
                if (shiftGroups.Count > 0)
                {
                    res.Message = "Shift Groups";
                    res.Status = true;
                    res.Data = shiftGroups;
                }
                else
                {
                    res.Message = "Shift Groups Not Found";
                    res.Status = false;
                    res.Data = shiftGroups;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class ShiftGroupResponse : AddShift
        {
            public Guid ShiftGoupId { get; set; }
        }
        #endregion

        #region This Api use to Update Shift
        /// <summary>
        /// Created By Ankit jain On 30-09-2022
        /// API >> Put >> api/shiftmaster/updatenewshift
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updatenewshift")]
        public async Task<ResponseBodyModel> UpdateNewShift(UpdateShift model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftGroups = await _db.ShiftGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive
                && !x.IsDeleted && x.ShiftGoupId == model.ShiftGoupId).FirstOrDefaultAsync();
                if (shiftGroups != null)
                {
                    shiftGroups.ShiftName = model.ShiftName;
                    shiftGroups.ShiftCode = model.ShiftCode;
                    shiftGroups.Description = model.Description;
                    shiftGroups.IsFlexible = model.IsFlexible;
                    shiftGroups.IsTimingDifferent = model.IsTimingDifferent;
                    shiftGroups.IsDurationDifferent = model.IsDurationDifferent;
                    shiftGroups.CompanyId = claims.companyId;
                    shiftGroups.UpdatedOn = DateTime.Now;
                    shiftGroups.IsActive = true;
                    shiftGroups.IsDeleted = false;

                }
                _db.Entry(shiftGroups).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                foreach (var item in model.ShiftTiming)
                {
                    var shifttime = _db.ShiftTimings.Include("ShiftGroup").FirstOrDefault(x =>
                            x.WeekDay == item.WeekDay && x.ShiftGroup.ShiftGoupId == shiftGroups.ShiftGoupId);
                    if (shifttime != null)
                    {

                        shifttime.WeekName = item.WeekName;
                        shifttime.StartTime = item.StartTime;
                        shifttime.EndTime = item.EndTime;
                        shifttime.BreakTime = item.BreakTime;
                        shifttime.CompanyId = claims.companyId;
                        shifttime.UpdatedOn = DateTime.Now;
                        shifttime.UpdatedBy = claims.employeeId;
                        _db.Entry(shifttime).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                    }
                }
                res.Message = "Shift Updated";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class UpdateShift : Shift
        {
            public Guid ShiftGoupId { get; set; }
        }

        public class Shift
        {
            public string ShiftName { get; set; }
            public string ShiftCode { get; set; }
            public string Description { get; set; }
            public bool IsFlexible { get; set; }
            public bool IsTimingDifferent { get; set; }
            public bool IsDurationDifferent { get; set; }
            public List<UpdateShiftTiming> ShiftTiming { get; set; }
        }
        public class UpdateShiftTiming
        {
            public DayOfWeek WeekDay { get; set; }
            public string WeekName { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public int BreakTime { get; set; }
        }
        #endregion

        #region This Api Use To delete shift Group

        /// <summary>
        /// Created By Ankit Jain On 30-09-2022
        /// API >> api/shiftmaster/deleteshift
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteshift")]
        public async Task<ResponseBodyModel> DeleteShifts(Guid shiftgroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftGroup = await _db.ShiftGroups.FirstOrDefaultAsync(x =>
                    x.ShiftGoupId == shiftgroupId && !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (shiftGroup != null)
                {
                    shiftGroup.IsDeleted = true;
                    shiftGroup.IsActive = false;

                    _db.Entry(shiftGroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Shift Group Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Data Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }


        #endregion

        #region This Api Use To delete shift Time

        /// <summary>
        /// Created By Ankit Jain On 30-09-2022
        /// API >> api/shiftmaster/deleteshifttime
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteshifttime")]
        public async Task<ResponseBodyModel> DeleteShiftTime(Guid shifttimingId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftTime = await _db.ShiftTimings.FirstOrDefaultAsync(x =>
                    x.ShiftTimingId == shifttimingId && x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                if (shiftTime != null)
                {
                    shiftTime.IsDeleted = true;
                    shiftTime.IsActive = false;

                    _db.Entry(shiftTime).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Shift Time Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Data Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class DeleteTime
        {
            public Guid ShiftTimingId { get; set; }
        }
        #endregion

        #region Get All Shift Group By Id
        /// <summary>
        /// Created By Ankit Jain On 30-09-2022
        /// API >> Get >> api/shiftmaster/getshiftgoruptimimgById
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getshiftgoruptimimgById")]
        public async Task<ResponseBodyModel> GetShiftGroupTimimgById(Guid shiftgoupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftGroups = await _db.ShiftGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.ShiftGoupId == shiftgoupId)
                            .Select(x => new ShiftGroupResponse
                            {
                                ShiftGoupId = x.ShiftGoupId,
                                ShiftName = x.ShiftName,
                                ShiftCode = x.ShiftCode,
                                Description = x.Description,
                                IsFlexible = x.IsFlexible,
                                IsTimingDifferent = x.IsTimingDifferent,
                                IsDurationDifferent = x.IsDurationDifferent,
                                ShiftTiming = _db.ShiftTimings.Where(z => z.ShiftGroup == x)
                                        .Select(z => new AddShiftTiming
                                        {
                                            WeekDay = z.WeekDay,
                                            WeekName = z.WeekName,
                                            StartTime = z.StartTime,
                                            EndTime = z.EndTime,
                                            BreakTime = z.BreakTime,
                                        })
                                        .OrderBy(z => z.WeekDay == DayOfWeek.Sunday).ThenBy(z => z.WeekDay)
                                        .ToList(),

                            }).ToListAsync();
                if (shiftGroups.Count > 0)
                {
                    res.Message = "Shift Groups";
                    res.Status = true;
                    res.Data = shiftGroups;
                }
                else
                {
                    res.Message = "Shift Groups Not Found";
                    res.Status = false;
                    res.Data = shiftGroups;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class ShiftGroupResponses
        {
            public Guid ShiftGoupId { get; set; }
        }
        #endregion

        #region Get All Shift Group
        /// <summary>
        /// Created By Suraj Bundel On 08-11-2022
        /// API >> Get >> api/shiftmaster/getshiftgroup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getshiftgroup")]
        public async Task<ResponseBodyModel> GetShiftGroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftGroups = await _db.ShiftGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted)
                            .Select(x => new ShiftGroupResponse
                            {
                                ShiftGoupId = x.ShiftGoupId,
                                ShiftName = x.ShiftName,
                                ShiftCode = x.ShiftCode,
                                Description = x.Description,
                                IsFlexible = x.IsFlexible,
                                IsTimingDifferent = x.IsTimingDifferent,
                                IsDurationDifferent = x.IsDurationDifferent
                            }).ToListAsync();
                if (shiftGroups.Count > 0)
                {
                    res.Message = "Shift Groups";
                    res.Status = true;
                    res.Data = shiftGroups;
                }
                else
                {
                    res.Message = "Shift Groups Not Found";
                    res.Status = false;
                    res.Data = shiftGroups;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #endregion

        #region API For Get Type
        /// <summary>
        /// Create By Ankit Jain 30-09-2022
        /// api/shiftmaster/getweekday
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getweekday")]
        public ResponseBodyModel GetWeekDay()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weekday = Enum.GetValues(typeof(WeekOffDayConstants))
                              .Cast<WeekOffDayConstants>()
                              .Select(x => new
                              {
                                  Id = (int)x,
                                  Type = Enum.GetName(typeof(WeekOffDayConstants), x).Replace("_", " "),
                              }).ToList();

                res.Message = "Get Week Day";
                res.Status = true;
                res.Data = weekday;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API For Get Type

        #region Region For Week Off APIs

        #region API To Add Week Off Days
        /// <summary>
        /// Created By Harshit Mitra On 07-10-2022
        /// API >> Post >> api/shiftmaster/addweekoffs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addweekoffs")]
        public async Task<ResponseBodyModel> AddWeekOff(AddWeekOffRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                WeekOffDaysGroup obj = new WeekOffDaysGroup
                {
                    WeekOffId = Guid.NewGuid(),
                    WeekOffName = model.WeekOffName,
                    Description = model.Description,

                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = claims.employeeId,
                    CreatedOn = DateTime.Now,
                    CompanyId = claims.companyId,
                };
                _db.WeekOffDays.Add(obj);
                await _db.SaveChangesAsync();
                foreach (var item in model.CaseList)
                {
                    if (item.CaseResponseId != WeekOffDayConstants.Not_Set)
                    {
                        WeekOffDaysCases caseObj = new WeekOffDaysCases
                        {
                            WeekOffCaseId = Guid.NewGuid(),
                            Group = obj,
                            DayId = item.DayId,
                            CaseId = item.CaseId,
                            CaseResponseId = item.CaseResponseId,

                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            CompanyId = claims.companyId,
                        };
                        _db.WeekOffDaysCases.Add(caseObj);
                        await _db.SaveChangesAsync();
                    }
                }
                res.Message = "Week Off Save";
                res.Status = true;
                res.Data = model;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class AddWeekOffRequest
        {
            public string WeekOffName { get; set; }
            public string Description { get; set; }
            public List<AddWeekOffCaseRequest> CaseList { get; set; }
        }
        public class AddWeekOffCaseRequest
        {
            public DayOfWeek DayId { get; set; }
            public WeekOffCase CaseId { get; set; }
            public WeekOffDayConstants CaseResponseId { get; set; }
        }
        #endregion

        #region API To Get All Week Off Day
        /// <summary>
        /// Created By Harshit Mitra On 07-10-2022
        /// API >> Get >> api/shiftmaster/getallweekoffs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallweekoffs")]
        public async Task<ResponseBodyModel> GetAllWeekOff()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weekOff = await _db.WeekOffDays.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId)
                    .Select(x => new
                    {
                        x.WeekOffId,
                        x.WeekOffName,
                        x.Description,
                        Case = _db.WeekOffDaysCases.Where(z => z.Group == x)
                                .Select(z => new
                                {
                                    DayId = z.DayId,
                                    DayName = z.DayId.ToString(),
                                    CaseId = z.CaseId,
                                    CaseName = z.CaseId.ToString().Replace("_", " "),
                                    CaseResponseId = z.CaseResponseId,
                                    CaseResponseName = z.CaseResponseId.ToString().Replace("_", " "),

                                }).ToList(),

                    }).ToListAsync();
                if (weekOff.Count > 0)
                {
                    res.Message = "Week Off List";
                    res.Status = true;
                    res.Data = weekOff;
                }
                else
                {
                    res.Message = "No Week Off Found";
                    res.Status = false;
                    res.Data = weekOff;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region API To Get All Week Off Day By Id
        /// <summary>
        /// Created By Harshit Mitra On 10-10-2022
        /// API >> Get >> api/shiftmaster/getweekoffdetailbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getweekoffdetailbyid")]
        public async Task<ResponseBodyModel> GetWeekOffDetailsById(Guid weekOfId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weekOff = await _db.WeekOffDays.Where(x => x.WeekOffId == weekOfId && !x.IsDeleted && x.IsActive)
                        .Select(x => new
                        {
                            WeekOfId = x.WeekOffId,
                            WeekOffName = x.WeekOffName,
                            Description = x.Description,
                            CaseList = _db.WeekOffDaysCases.Where(z => z.Group == x)
                                .Select(z => new
                                {
                                    DayId = z.DayId,
                                    DayName = z.DayId.ToString(),
                                    CaseId = z.CaseId,
                                    CaseName = z.CaseId.ToString().Replace("_", " "),
                                    CaseResponseId = z.CaseResponseId,
                                    CaseResponseName = z.CaseResponseId.ToString().Replace("_", " "),

                                }).ToList(),
                        }).FirstOrDefaultAsync();
                if (weekOff != null)
                {
                    res.Message = "Week Off Found";
                    res.Status = true;
                    res.Data = weekOff;
                }
                else
                {
                    res.Message = "Week Off Not Found";
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
        #endregion

        #region API To Update All Week Off Day
        /// <summary>
        /// Created By Harshit Mitra On 10-10-2022
        /// API >> Get >> api/shiftmaster/updateweekoff
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updateweekoff")]
        public async Task<ResponseBodyModel> UpdateWeekOff(UpdateWeekOffRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weekOffGroup = await _db.WeekOffDays.Where(x => x.WeekOffId == model.WeekOffId && !x.IsDeleted && x.IsActive).FirstOrDefaultAsync();
                if (weekOffGroup != null)
                {
                    weekOffGroup.WeekOffName = model.WeekOffName;
                    weekOffGroup.Description = model.Description;
                    weekOffGroup.UpdatedOn = DateTime.Now;
                    weekOffGroup.UpdatedBy = claims.employeeId;
                    _db.Entry(weekOffGroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    if (model.CaseList.Count > 0)
                    {
                        var removeData = await _db.WeekOffDaysCases.Include("Group").Where(x => x.Group.WeekOffId == weekOffGroup.WeekOffId).ToListAsync();
                        _db.WeekOffDaysCases.RemoveRange(removeData);
                        await _db.SaveChangesAsync();

                        List<WeekOffDaysCases> getData = new List<WeekOffDaysCases>();
                        foreach (var item in model.CaseList)
                        {
                            WeekOffDaysCases caseObj = new WeekOffDaysCases
                            {
                                WeekOffCaseId = Guid.NewGuid(),
                                Group = weekOffGroup,
                                DayId = item.DayId,
                                CaseId = item.CaseId,
                                CaseResponseId = item.CaseResponseId,

                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                            };
                            _db.WeekOffDaysCases.Add(caseObj);
                            await _db.SaveChangesAsync();
                            getData.Add(caseObj);
                        }
                        res.Message = "Week Off Updated";
                        res.Status = true;
                        res.Data = getData
                            .Select(z => new
                            {
                                DayId = z.DayId,
                                DayName = z.DayId.ToString(),
                                CaseId = z.CaseId,
                                CaseName = z.CaseId.ToString().Replace("_", " "),
                                CaseResponseId = z.CaseResponseId,
                                CaseResponseName = z.CaseResponseId.ToString().Replace("_", " "),

                            }).ToList();
                    }
                }
                else
                {
                    res.Message = "Week Of Not Found";
                    res.Status = false;
                }
                res.Message = "Week Of Updated";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class UpdateWeekOffRequest : AddWeekOffRequest
        {
            public Guid WeekOffId { get; set; }
        }
        #endregion

        #region API To Delete All Week Off Day
        /// <summary>
        /// Created By Harshit Mitra On 10-10-2022
        /// API >> Put >> api/shiftmaster/deleteweekoff
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteweekoff")]
        public async Task<ResponseBodyModel> DeleteWeekOff(Guid weekOffId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weekOffGroup = await _db.WeekOffDays.Where(x => x.WeekOffId == weekOffId && !x.IsDeleted && x.IsActive).FirstOrDefaultAsync();
                if (weekOffGroup != null)
                {
                    weekOffGroup.DeletedOn = DateTime.Now;
                    weekOffGroup.DeletedBy = claims.employeeId;
                    weekOffGroup.IsActive = false;
                    weekOffGroup.IsDeleted = true;

                    _db.Entry(weekOffGroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Week Off Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Week Of Not Found";
                    res.Status = false;
                }
                res.Message = "Week Of Updated";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #endregion

        #region API To Add default Week Off Days
        /// <summary>
        /// Created By Harshit Mitra On 07-10-2022
        /// API >> Post >> api/shiftmaster/addweekoffs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adddefaultsweekoffs")]
        //public async Task<ResponseBodyModel> AddDefaultWeekOff(AddDefaultWeekOffRequest model)
        public async Task<ResponseBodyModel> AddDefaultWeekOff(int companyId, Company comp = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (comp != null)
                    companyId = comp.CompanyId;
                WeekOffDaysGroup obj = new WeekOffDaysGroup
                {
                    WeekOffId = Guid.NewGuid(),
                    WeekOffName = "Default Week Offs",
                    Description = "Default WeekOffs",

                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    CompanyId = companyId,
                };
                _db.WeekOffDays.Add(obj);
                await _db.SaveChangesAsync();

                var weekOffAdd = Enum.GetValues(typeof(DayOfWeek))
                            .Cast<DayOfWeek>()
                            .Where(x => x == DayOfWeek.Sunday || x == DayOfWeek.Saturday)
                            .Select(x => new WeekOffDaysCases
                            {
                                WeekOffCaseId = Guid.NewGuid(),
                                Group = obj,
                                DayId = x,
                                CaseId = WeekOffCase.All_Week,
                                CaseResponseId = WeekOffDayConstants.Full_Day_Weekly_Off,

                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = 0,
                                CreatedOn = DateTime.Now,
                                CompanyId = companyId,

                            }).ToList();
                _db.WeekOffDaysCases.AddRange(weekOffAdd);
                await _db.SaveChangesAsync();

                if (comp != null)
                {
                    comp.DefaultWeekOff = obj.WeekOffId;
                    _db.Entry(comp).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }

                res.Message = "Week Off Save";
                res.Status = true;
                res.Data = weekOffAdd;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region API To Get All Week Off Groups
        /// <summary>
        /// Created By Suraj Bundel On 08-11-2022
        /// API >> Get >> api/shiftmaster/getallweekoffsgroup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallweekoffsgroup")]
        public async Task<ResponseBodyModel> GetAllWeekOffGroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weekOff = await _db.WeekOffDays.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId)
                    .Select(x => new
                    {
                        x.WeekOffId,
                        x.WeekOffName,
                        x.Description,
                        //Case = _db.WeekOffDaysCases.Where(z => z.Group == x)
                        //        .Select(z => new
                        //        {
                        //            DayId = z.DayId,
                        //            DayName = z.DayId.ToString(),
                        //            CaseId = z.CaseId,
                        //            CaseName = z.CaseId.ToString().Replace("_", " "),
                        //            CaseResponseId = z.CaseResponseId,
                        //            CaseResponseName = z.CaseResponseId.ToString().Replace("_", " "),
                        //        }).ToList(),

                    }).ToListAsync();
                if (weekOff.Count > 0)
                {
                    res.Message = "Week Off List";
                    res.Status = true;
                    res.Data = weekOff;
                }
                else
                {
                    res.Message = "No Week Off Found";
                    res.Status = false;
                    res.Data = weekOff;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

    }
}