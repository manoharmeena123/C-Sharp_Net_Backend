using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.NewDashboard;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);


        #region This Api Used to All the data get in Deshboard

        /// <summary>
        /// created by Ankit Jain 04/05/2022
        /// Modify By Shriya Malvi 28-06-2022
        /// api/Dashboard/GetAllMargeData
        /// </summary>Get- api/Dashboard/GetAllMargeData
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllMargeData")]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllMargeData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetEmpBdayandAnyModel response = new GetEmpBdayandAnyModel();
            List<EmployeeData1> employeeDataList = new List<EmployeeData1>();
            List<Endrosments> EndrosmentsDataList = new List<Endrosments>();
            List<WFHData1> WFHData = new List<WFHData1>();
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, claims.TimeZone);
                #region This Api Use Employe WeekData

                var currentDate = DateTime.Now.Date;
                List<DateTime> dateList = new List<DateTime>();

                for (int i = 1; i <= 7; i++)
                {
                    dateList.Add(currentDate.AddDays(i));
                }
                var employeeData = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeTypeId !=
                            EmployeeTypeConstants.Ex_Employee && x.CompanyId == claims.companyId).ToList();

                response.CurrentBirthdatList = employeeData.Where(x => x.DateOfBirth.Day == currentDate.Day
                && x.DateOfBirth.Month == currentDate.Month && !x.HideDOB)
                        .Select(x => new BdayEmployee
                        {
                            EmployeeId = x.EmployeeId,
                            EmployeeName = x.DisplayName,
                            DOB = x.DateOfBirth,
                            HideDOB = x.HideDOB,
                        }).ToList();

                var birthdayEmployee = new List<BdayEmployee>();
                for (int i = 0; i < 7; i++)
                {
                    var bdayEmp = employeeData.Where(x => x.DateOfBirth.Day == dateList[i].Day
                    && x.DateOfBirth.Month == dateList[i].Month && !x.HideDOB)
                            .Select(x => new BdayEmployee
                            {
                                EmployeeId = x.EmployeeId,
                                EmployeeName = x.DisplayName,
                                DOB = x.DateOfBirth,
                                HideDOB = x.HideDOB,
                            }).ToList();
                    birthdayEmployee.AddRange(bdayEmp);
                }
                response.BirthdatList = birthdayEmployee;

                var curentannyEmp = employeeData.Where(x => x.JoiningDate.Day == currentDate.Day && x.JoiningDate.Month == currentDate.Month)
                        .Select(x => new AnniEmployee
                        {
                            EmployeeId = x.EmployeeId,
                            EmployeeName = x.DisplayName,
                            JoiningDate = x.JoiningDate,
                        }).ToList();
                response.CurrentAnniversaryList = curentannyEmp;

                var annaversyEmployee = new List<AnniEmployee>();
                for (int i = 0; i < 7; i++)
                {
                    var annyEmp = employeeData.Where(x => x.JoiningDate.Day == dateList[i].Day && x.JoiningDate.Month == dateList[i].Month)
                          .Select(x => new AnniEmployee
                          {
                              EmployeeId = x.EmployeeId,
                              EmployeeName = x.DisplayName,
                              JoiningDate = x.JoiningDate,
                          }).ToList();
                    annaversyEmployee.AddRange(annyEmp);
                }
                response.AnniversaryList = annaversyEmployee;

                #endregion This Api Use Employe WeekData

                #region This Api use Holiday

                var emp = employeeData.FirstOrDefault(x => x.EmployeeId == claims.employeeId);
                var holidayDataList = await (from g in _db.HolidayGroups
                                             join hg in _db.HolidayInGroups on g.GroupId equals hg.GroupId
                                             join h in _db.HolidayModels on hg.HolidayId equals h.HolidayId
                                             where g.GroupId == emp.HolidayGroupId
                                             select h)
                                             .OrderBy(x => x.HolidayDate)
                                             .ToListAsync();
                var checkList = holidayDataList
                    .Where(x => x.HolidayDate.Date >= today.Date)
                    .OrderBy(x => x.HolidayDate)
                    .Select(x => new HolidayOnHomeResponse
                    {
                        HolidayName = x.HolidayName,
                        ImageUrl = x.ImageUrl,
                        TextColor = x.TextColor,
                        IsFloaterOptional = x.IsFloaterOptional,
                        MonthName = x.HolidayDate.ToString("MMMM"),
                        StartDate = x.HolidayDate.ToString("dd"),
                        DayName = x.HolidayDate.ToString("dddd"),
                    })
                    .Take(3)
                    .ToList();

                response.AllHolidays = checkList;

                #endregion This Api use Holiday

                #region this api use workFormHome

                var WorkFromHomData = (from ad in _db.Employee
                                       join lr in _db.WorkFromHomes on ad.EmployeeId equals lr.EmployeeId
                                       orderby lr.StartDate descending
                                       where lr.CompanyId == claims.companyId && lr.OrgId == claims.orgId && lr.StartDate <= DateTime.Today &&
                                                 lr.EndDate >= DateTime.Today && lr.WFHStatus == "Approved" && ad.IsDeleted == false
                                       select new
                                       {
                                           lr.EmployeeId,
                                           n = ad.DisplayName
                                       }).ToList();
                foreach (var item in WorkFromHomData)
                {
                    WFHData1 wfh = new WFHData1();
                    wfh.EmployeeId = item.EmployeeId;
                    wfh.FullName = item.n;
                    WFHData.Add(wfh);
                }
                response.WorkFromStatus = WFHData;

                #endregion this api use workFormHome

                #region For leave Status

                //var employeeData1 = (from ad in _db.Employee
                //                     join lr in _db.LeaveRequest on ad.EmployeeId equals lr.EmployeeId
                //                     orderby lr.Startdate descending
                //                     where lr.CompanyId == claims.companyid && lr.OrgId == claims.orgid && lr.Startdate <= DateTime.Today &&
                //                            lr.EndDate >= DateTime.Today && lr.LeaveStatus == "Approved" && ad.IsDeleted == false
                //                     select new
                //                     {
                //                         ad.EmployeeId,
                //                         n = ad.FirstName + " " + ad.LastName
                //                     }).ToList().Distinct().ToList();
                //foreach (var item in employeeData1)
                //{
                //    EmployeeData1 data = new EmployeeData1();
                //    data.EmployeeId = item.EmployeeId;
                //    data.FullName = item.n;
                //    employeeDataList.Add(data);
                //}
                var empOnLeave = await (from l in _db.LeaveRequests
                                        join e in _db.Employee on l.RequestedBy equals e.EmployeeId
                                        where l.IsActive && !l.IsDeleted && e.CompanyId == claims.companyId &&
                                        DbFunctions.TruncateTime(l.FromDate) <= DateTime.Today &&
                                        DbFunctions.TruncateTime(l.ToDate) >= DateTime.Today &&
                                        e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee && l.Status != LeaveStatusConstants.Cancel
                                        select new EmployeeData1
                                        {
                                            EmployeeId = e.EmployeeId,
                                            FullName = e.DisplayName,
                                        }).ToListAsync();

                response.EmployeesTodayOnLeave = empOnLeave;

                #endregion For leave Status

                #region Api Use Endrosment Badges

                var EndrosmentData = (from ad in _db.Endorsements
                                      join ed in _db.EmpBadges on ad.BadgeId equals ed.BadgeId
                                      join dp in _db.Employee on ad.EmployeeId equals dp.EmployeeId
                                      where ad.IsDeleted == false && ad.EmployeeId == claims.employeeId
                                      select new
                                      {
                                          ad.EmployeeId,
                                          ed.ImageUrl,
                                          ed.Title,
                                      }).ToList();

                foreach (var item in EndrosmentData)
                {
                    Endrosments data = new Endrosments();
                    data.EmployeeId = item.EmployeeId;
                    data.ImageUrl = item.ImageUrl;
                    data.Title = item.Title;

                    EndrosmentsDataList.Add(data);
                }
                response.EndrosmentsData = EndrosmentsDataList;

                #endregion Api Use Endrosment Badges

                res.Message = "List";
                res.Status = true;
                res.Data = response;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class HolidayOnHomeResponse
        {
            public string HolidayName { get; set; } = String.Empty;
            public string ImageUrl { get; set; } = String.Empty;
            public string TextColor { get; set; } = String.Empty;
            public bool IsFloaterOptional { get; set; } = false;
            public string MonthName { get; set; } = String.Empty;
            public string StartDate { get; set; } = String.Empty;
            public string DayName { get; set; } = String.Empty;
        }
        #endregion This Api Used to All the data get in Deshboard

        // api/Dashboard/checkunauthorized
        [Route("checkunauthorized")]
        public IHttpActionResult CheckUnAuthorized()
        {
            return Unauthorized();
        }





        #region Helper Region Model

        /// <summary>
        /// Created By ankit jain 04/05/2022
        /// </summary>
        public class GetEmpBdayandAnyModel
        {
            public List<BdayEmployee> CurrentBirthdatList { get; set; }
            public List<BdayEmployee> BirthdatList { get; set; }
            public List<AnniEmployee> CurrentAnniversaryList { get; set; }
            public List<AnniEmployee> AnniversaryList { get; set; }
            public List<WFHData1> WorkFromStatus { get; set; }
            public List<HolidayOnHomeResponse> AllHolidays { get; set; }
            public List<EmployeeData1> EmployeesTodayOnLeave { get; set; }
            public List<Endrosments> EndrosmentsData { get; set; }
        }

        public class BdayEmployee
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public DateTime? DOB { get; set; }
            public bool HideDOB { get; set; }
        }

        public class AnniEmployee
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public DateTimeOffset? JoiningDate { get; set; }
        }

        public class holidaylist
        {
            public long HolidayId { get; set; }
            public string HolidayName { get; set; }
            public string Description { get; set; }
            public bool IsFloaterOptional { get; set; }
            public string ImageUrl { get; set; }
            public DateTime HolidayDate { get; set; }
        }

        public class WorkFromHomeDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<WorkFromHome> WorkFromHomeList { get; set; }
            public List<WFHData1> WFHList { get; set; }
        }

        public class WFHData1
        {
            public int EmployeeId { get; set; }
            public string FullName { get; set; }
        }

        public class EmployeeData1
        {
            public int EmployeeId { get; set; }
            public string FullName { get; set; }
            public string RoleName { get; set; }
            public bool IsManager { get; set; }
        }

        public class Endrosments
        {
            public int EmployeeId { get; set; }
            public string ImageUrl { get; set; }
            public string Title { get; set; }
        }

        public class EmployeeList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<EmployeeData1> EmpList { get; set; }
        }

        #endregion Helper Region Model


        #region This Api's Use For About Us


        #region Api's For Add About Us Description
        /// <summary>
        /// API >> Post >> api/Dashboard/addabout
        /// Created By Ravi Vyas on 30-08-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("addabout")]
        public async Task<ResponseBodyModel> AddAbout(AboutUs model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var aboutData = _db.AboutUs.Where(x => x.AboutId == model.AboutId && x.IsActive == true && x.IsDeleted == false
                                                     && x.CompanyId == claims.companyId).FirstOrDefault();
                    {
                        if (aboutData == null)
                        {
                            AboutUs obj = new AboutUs
                            {
                                AboutDescription = model.AboutDescription,
                                IsDraft = model.IsDraft,
                                Status = model.IsDraft == true ? AboutUsStatusConstants.Draft : AboutUsStatusConstants.Publish,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                OrgId = claims.orgId,
                                CompanyId = claims.companyId,
                                CreatedBy = claims.employeeId,
                            };
                            _db.AboutUs.Add(obj);
                            await _db.SaveChangesAsync();

                            res.Message = "About Us Added";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            aboutData.AboutDescription = model.AboutDescription;
                            aboutData.IsDraft = model.IsDraft;
                            aboutData.Status = model.IsDraft == true ? AboutUsStatusConstants.Draft : AboutUsStatusConstants.Publish;
                            aboutData.IsActive = true;
                            aboutData.IsDeleted = false;
                            aboutData.OrgId = claims.orgId;
                            aboutData.CompanyId = claims.companyId;
                            aboutData.UpdatedOn = DateTime.Now;
                            aboutData.UpdatedBy = claims.employeeId;
                            _db.Entry(aboutData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                            res.Message = " Updated Succedfully !";
                            res.Status = true;
                            res.Data = aboutData;
                        }


                    }
                }
                else
                {
                    res.Message = "No Data ! ";
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

        #region Api's For Get About Us  Data
        /// <summary>
        /// API >> Post >> api/Dashboard/getdraftdata
        /// Created By Ravi Vyas on 30-08-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getdraftdata")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetAboutData()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AboutUsRes response = new AboutUsRes();


            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //var aboutDraft = _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                //               x.CompanyId == claims.companyid && x.Status == AboutUsStatusEnum.Draft && x.IsDraft == true).ToList().LastOrDefault();

                //response.DraftData = aboutDraft;

                //var publishData = _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                //               x.CompanyId == claims.companyid && x.Status == AboutUsStatusEnum.Publish && x.IsDraft == true).ToList().LastOrDefault();

                //response.PublishData = publishData;
                var aboutDraft = _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                              x.CompanyId == claims.companyId  /*&& x.Status == AboutUsStatusEnum.Draft*/ ).ToList().LastOrDefault();

                if (aboutDraft != null)
                {
                    res.Message = "About Us Data Found !";
                    res.Status = true;
                    res.Data = aboutDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = aboutDraft;
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

        #region Api's For Get About Us Publish Data
        /// <summary>
        /// Created By Ravi Vyas on 30-08-2022
        /// API >> GET >> api/Dashboard/getbulishdata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getbulishdata")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetAbout()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var aboutDraft = _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.Status == AboutUsStatusConstants.Publish).ToList().LastOrDefault();
                if (aboutDraft != null)
                {
                    res.Message = "About Us Data Found !";
                    res.Status = true;
                    res.Data = aboutDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = aboutDraft;
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

        #region Api's For Get About Us Draft Data By Id
        /// <summary>
        /// API >> Post >> api/Dashboard/getdraftdatabyid
        /// Created By Ravi Vyas on 30-08-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getdraftdatabyid")]
        public async Task<ResponseBodyModel> GetAboutDataById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var aboutDraft = await _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId /*&& x.Status == AboutUsStatusEnum.Draft*/ && x.AboutId == id).FirstOrDefaultAsync();

                if (aboutDraft != null)
                {
                    res.Message = "About Us Data Found !";
                    res.Status = true;
                    res.Data = aboutDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = aboutDraft;
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

        #region Api's For Delete About Us

        /// <summary>
        /// Created By Ravi Vyas on 01-09-2022
        /// API >> GET >> api/Dashboard/deleteabout
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deleteabout")]
        public async Task<ResponseBodyModel> DeleteAbout(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var aboutDelete = await _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.AboutId == id).FirstOrDefaultAsync();
                if (aboutDelete != null)
                {
                    aboutDelete.IsActive = false;
                    aboutDelete.IsDeleted = true;
                    aboutDelete.CompanyId = claims.companyId;
                    aboutDelete.DeletedOn = DateTime.Now;
                    aboutDelete.DeletedBy = claims.employeeId;

                    _db.Entry(aboutDelete).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Deleted Successfully  !";
                    res.Status = true;
                    res.Data = aboutDelete;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = aboutDelete;
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

        #region Api's For Get About Us Publish Data By Id
        /// <summary>
        /// API >> Post >> api/Dashboard/getpublishdatabyid?id
        /// Created By Ravi Vyas on 30-08-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getpublishdatabyid")]
        public async Task<ResponseBodyModel> GetAboutPublishDataById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var aboutDraft = await _db.AboutUs.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.Status == AboutUsStatusConstants.Publish && x.AboutId == id).FirstOrDefaultAsync();

                if (aboutDraft != null)
                {
                    res.Message = "About Us Data Found !";
                    res.Status = true;
                    res.Data = aboutDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = aboutDraft;
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

        #region This Api's Use For Services&Benfits


        #region Api's For Add Services&Benfits Description
        /// <summary>
        /// API >> Post >> api/Dashboard/addservicebenifit
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("addservicebenifit")]
        public async Task<ResponseBodyModel> AddService(Benefits model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var benifitData = _db.Benefits.Where(x => x.BenefitsId == model.BenefitsId && x.IsActive &&
                            !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                    {
                        if (benifitData == null)
                        {
                            Benefits obj = new Benefits
                            {
                                BenifitsServicesDescription = model.BenifitsServicesDescription,
                                IsDraft = model.IsDraft,
                                Status = model.IsDraft == true ? AboutUsStatusConstants.Draft : AboutUsStatusConstants.Publish,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                OrgId = claims.orgId,
                                CompanyId = claims.companyId,
                                CreatedBy = claims.employeeId,
                            };
                            _db.Benefits.Add(obj);
                            await _db.SaveChangesAsync();

                            res.Message = " Services & Benfits Added Successfully  !";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            benifitData.BenifitsServicesDescription = model.BenifitsServicesDescription;
                            benifitData.IsDraft = model.IsDraft;
                            benifitData.Status = model.IsDraft == true ? AboutUsStatusConstants.Draft : AboutUsStatusConstants.Publish;
                            benifitData.IsActive = true;
                            benifitData.IsDeleted = false;
                            benifitData.OrgId = claims.orgId;
                            benifitData.CompanyId = claims.companyId;
                            benifitData.UpdatedOn = DateTime.Now;
                            benifitData.UpdatedBy = claims.employeeId;
                            _db.Entry(benifitData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                            res.Message = " Updated Successfully  !";
                            res.Status = true;
                            res.Data = benifitData;
                        }
                    }
                }
                else
                {
                    res.Message = "No Data ! ";
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

        #region Api's For Get Services & Benfits Data
        /// <summary>
        /// API >> GET >> api/Dashboard/servicesbenfitdata
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("servicesbenfitdata")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetServicesBenfits()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var benifitData = _db.Benefits.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId  /*&& x.Status == AboutUsStatusEnum.Draft*/ ).ToList().LastOrDefault();

                if (benifitData != null)
                {
                    res.Message = "Services & Benfits Data Found !";
                    res.Status = true;
                    res.Data = benifitData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = benifitData;
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

        #region Api's For Get Services & Benfit  Data By Id
        /// <summary>
        /// API >> GET >> api/Dashboard/getdraftdatabyid
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getbenfitdatabyid")]
        public async Task<ResponseBodyModel> GetBenifitDataById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var benifitData = await _db.Benefits.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId /*&& x.Status == AboutUsStatusEnum.Draft*/ && x.BenefitsId == id).FirstOrDefaultAsync();

                if (benifitData != null)
                {
                    res.Message = "Services & Benfit Data Found Successfully  !";
                    res.Status = true;
                    res.Data = benifitData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = benifitData;
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

        #region Api's For Get Services & Benfit Publish Data
        /// <summary>
        /// Created By Ravi Vyas on 01-09-2022
        /// API >> GET >> api/Dashboard/getbenfitpulishdata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getbenfitpulishdata")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetBenfit()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var benifitData = _db.Benefits.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.Status == AboutUsStatusConstants.Publish).ToList().LastOrDefault();
                if (benifitData != null)
                {
                    res.Message = "Services & Benfit  Data Found !";
                    res.Status = true;
                    res.Data = benifitData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = benifitData;
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

        #region Api's For DeleteBenifit  

        /// <summary>
        /// Created By Ravi Vyas on 01-09-2022
        /// API >> DETELE >> api/Dashboard/deletebenifit
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deletebenifit")]
        public async Task<ResponseBodyModel> DeleteBenifit(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var benifitData = await _db.Benefits.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.BenefitsId == id).FirstOrDefaultAsync();
                if (benifitData != null)
                {
                    benifitData.IsActive = false;
                    benifitData.IsDeleted = true;
                    benifitData.CompanyId = claims.companyId;
                    benifitData.DeletedOn = DateTime.Now;
                    benifitData.DeletedBy = claims.employeeId;

                    _db.Entry(benifitData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Deleted Successfully  !";
                    res.Status = true;
                    res.Data = benifitData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = benifitData;
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

        #region This Api's Use For Wroking At Company


        #region Api's For Add Company Description
        /// <summary>
        /// API >> Post >> api/Dashboard/copamnyadd
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("copamnyadd")]
        public async Task<ResponseBodyModel> AddCompany(WorkingAt model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var companyData = _db.WorkingAt.Where(x => x.WorkingAtId == model.WorkingAtId && x.IsActive == true && x.IsDeleted == false
                                                     && x.CompanyId == claims.companyId).FirstOrDefault();
                    {
                        if (companyData == null)
                        {
                            WorkingAt obj = new WorkingAt
                            {
                                WorkingAtDescription = model.WorkingAtDescription,
                                IsDraft = model.IsDraft,
                                Status = model.IsDraft == true ? AboutUsStatusConstants.Draft : AboutUsStatusConstants.Publish,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                OrgId = claims.orgId,
                                CompanyId = claims.companyId,
                                CreatedBy = claims.employeeId,
                            };
                            _db.WorkingAt.Add(obj);
                            await _db.SaveChangesAsync();

                            res.Message = " Services & Benfits Added Successfully  !";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            companyData.WorkingAtDescription = model.WorkingAtDescription;
                            companyData.IsDraft = model.IsDraft;
                            companyData.Status = model.IsDraft == true ? AboutUsStatusConstants.Draft : AboutUsStatusConstants.Publish;
                            companyData.IsActive = true;
                            companyData.IsDeleted = false;
                            companyData.OrgId = claims.orgId;
                            companyData.CompanyId = claims.companyId;
                            companyData.UpdatedOn = DateTime.Now;
                            companyData.UpdatedBy = claims.employeeId;
                            _db.Entry(companyData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                            res.Message = " Updated Successfully  !";
                            res.Status = true;
                            res.Data = companyData;
                        }
                    }
                }
                else
                {
                    res.Message = "No Data ! ";
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

        #region Api's For Get Working At Company Data
        /// <summary>
        /// API >> GET >> api/Dashboard/getcomapnydata
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getcomapnydata")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetCompanyData()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //var companyData = _db.WorkingAt.Where(x => x.IsActive == true && x.IsDeleted == false &&
                //               x.CompanyId == claims.companyid  /*&& x.Status == AboutUsStatusEnum.Draft*/ ).ToList().LastOrDefault();

                var companyData = (from w in _db.WorkingAt
                                   join c in _db.Company on w.CompanyId equals c.CompanyId
                                   where w.IsActive == true && w.IsDeleted == false && w.CompanyId == claims.companyId
                                   select new
                                   {
                                       w.WorkingAtId,
                                       w.WorkingAtDescription,
                                       w.IsDraft,
                                       w.Status,
                                       w.CompanyId,
                                       w.OrgId,
                                       w.IsActive,
                                       w.IsDeleted,
                                       w.CreatedOn,
                                       c.RegisterCompanyName,
                                   }).ToList().LastOrDefault();
                if (companyData != null)
                {
                    res.Message = "Working At Company Data Found !";
                    res.Status = true;
                    res.Data = companyData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = companyData;
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

        #region Api's For Get Working At Company Data By Id
        /// <summary>
        /// API >> GET >> api/Dashboard/getcompanydatabyid
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getcompanydatabyid")]
        public async Task<ResponseBodyModel> GetCompanyDataById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var companyData = await _db.WorkingAt.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId /*&& x.Status == AboutUsStatusEnum.Draft*/ && x.WorkingAtId == id).FirstOrDefaultAsync();

                if (companyData != null)
                {
                    res.Message = " Working At Company Data Found !";
                    res.Status = true;
                    res.Data = companyData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = companyData;
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

        #region Api's For Get Working At Company Publish Data
        /// <summary>
        /// Created By Ravi Vyas on 01-09-2022
        /// API >> GET >> api/Dashboard/getcompanybulishdata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getcompanybulishdata")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetWoekingData()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //var companyData = _db.WorkingAt.Where(x => x.IsActive == true && x.IsDeleted == false &&
                //x.CompanyId == claims.companyid && x.Status == AboutUsStatusEnum.Publish).ToList().LastOrDefault();

                var companyData = (from w in _db.WorkingAt
                                   join c in _db.Company on w.CompanyId equals c.CompanyId
                                   where w.IsActive == true && w.IsDeleted == false && w.CompanyId == claims.companyId && w.Status == AboutUsStatusConstants.Publish
                                   select new
                                   {
                                       w.WorkingAtId,
                                       w.WorkingAtDescription,
                                       w.IsDraft,
                                       w.Status,
                                       w.CompanyId,
                                       w.OrgId,
                                       w.IsActive,
                                       w.IsDeleted,
                                       w.CreatedOn,
                                       c.RegisterCompanyName,
                                   }).ToList().LastOrDefault();
                if (companyData != null)
                {
                    res.Message = " Working Company Data Found !";
                    res.Status = true;
                    res.Data = companyData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = companyData;
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

        #region Api's For Delete Working At Company Data

        /// <summary>
        /// Created By Ravi Vyas on 01-09-2022
        /// API >> DETELE >> api/Dashboard/deletecompnaydata
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deletecompnaydata")]
        public async Task<ResponseBodyModel> DeleteCompany(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var companyData = await _db.WorkingAt.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.WorkingAtId == id).FirstOrDefaultAsync();
                if (companyData != null)
                {
                    companyData.IsActive = false;
                    companyData.IsDeleted = true;
                    companyData.CompanyId = claims.companyId;
                    companyData.DeletedOn = DateTime.Now;
                    companyData.DeletedBy = claims.employeeId;

                    _db.Entry(companyData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Deleted Successfully  !";
                    res.Status = true;
                    res.Data = companyData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = companyData;
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

        #region This Api's For Tools

        #region Api for Add Tools

        /// <summary>
        /// API >> Post >> api/Dashboard/addtool
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("addtool")]
        public async Task<ResponseBodyModel> AddTool(Tools model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var addTools = await _db.Tools.Where(x => x.Name == model.Name && x.CompanyId == claims.companyId
                                      && x.IsDeleted == false && x.IsActive == true && x.URL == model.URL).FirstOrDefaultAsync();
                    if (addTools == null)
                    {
                        Tools obj = new Tools
                        {
                            Name = model.Name,
                            URL = model.URL,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                        };
                        _db.Tools.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = "Tools Added Successfully  !";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Tools Already Exits !";
                        res.Status = false;
                        res.Data = null;
                    }


                }
                else
                {
                    res.Message = "No Data ! ";
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

        #region Api for get Tools

        /// <summary>
        /// API >> GET >> api/Dashboard/gettool
        /// Created By Ravi Vyas on 01-09-2022
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("gettool")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetTool()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var toolData = _db.Tools.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();


                if (toolData.Count > 0)
                {
                    res.Message = "Tools Found !";
                    res.Status = true;
                    res.Data = toolData;
                }
                else
                {
                    res.Message = "Tools Not Found !";
                    res.Status = true;
                    res.Data = toolData;
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

        #region Api for Delete Tools

        /// <summary>
        /// Created By Ravi Vyas on 01-09-2022
        /// API >> DETELE >> api/Dashboard/deletetooldata
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deletetooldata")]
        public async Task<ResponseBodyModel> DeleteTool(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var toolData = await _db.Tools.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.ToolId == id).FirstOrDefaultAsync();
                if (toolData != null)
                {
                    toolData.IsActive = false;
                    toolData.IsDeleted = true;
                    toolData.CompanyId = claims.companyId;
                    toolData.DeletedOn = DateTime.Now;
                    toolData.DeletedBy = claims.employeeId;

                    _db.Entry(toolData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Deleted Succedfully !";
                    res.Status = true;
                    res.Data = toolData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = toolData;
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

        #region Api for Update Tools

        /// <summary>
        /// Created By Ravi Vyas on 02-09-2022
        /// API >> PUT >> api/Dashboard/updatetooldata
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("updatetooldata")]
        public async Task<ResponseBodyModel> UpdateTool(Tools model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var toolData = await _db.Tools.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.ToolId == model.ToolId && ((x.Name.ToUpper().Trim() == model.Name.ToUpper().Trim()
                               || x.URL.ToUpper().Trim() == model.URL.ToUpper().Trim())
                               )).FirstOrDefaultAsync();
                if (toolData != null)
                {
                    toolData.Name = model.Name;
                    toolData.URL = model.URL;
                    toolData.IsActive = true;
                    toolData.IsDeleted = false;
                    toolData.CompanyId = claims.companyId;
                    toolData.UpdatedOn = DateTime.Now;
                    toolData.UpdatedBy = claims.employeeId;

                    _db.Entry(toolData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Update Successfully  !";
                    res.Status = true;
                    res.Data = toolData;
                }
                else
                {
                    res.Message = " Tools Already Exits  ! ";
                    res.Status = false;
                    res.Data = toolData;
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

        #region Api for get Tools by Id

        /// <summary>
        /// API >> GET >> api/Dashboard/gettoolbyid?id
        /// Created By Ravi Vyas on 02-09-2022
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("gettoolbyid")]
        public async Task<ResponseBodyModel> GetToolById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var toolData = await _db.Tools.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId
                                               && x.OrgId == claims.orgId && x.ToolId == id).ToListAsync();

                if (toolData.Count > 0)
                {
                    res.Message = "Tools Found !";
                    res.Status = true;
                    res.Data = toolData;
                }
                else
                {
                    res.Message = "Tools Not Found !";
                    res.Status = true;
                    res.Data = toolData;
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

        #region This Api's For QuickLinks

        #region Api for Add Links

        /// <summary>
        /// API >> Post >> api/Dashboard/addlink
        /// Created By Ravi Vyas on 02-09-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("addlink")]
        public async Task<ResponseBodyModel> AddTool(QuickLinks model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var addLink = _db.QuickLinks.Where(x => x.LinkName == model.LinkName && x.IsActive == true && x.IsDeleted == false
                    && x.CompanyId == claims.companyId && x.URL == model.URL).FirstOrDefault();
                    if (addLink == null)
                    {
                        QuickLinks obj = new QuickLinks
                        {
                            LinkName = model.LinkName,
                            URL = model.URL,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                        };
                        _db.QuickLinks.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = "Links Added Successfully  !";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Links Already Exits !";
                        res.Status = true;
                        res.Data = null;
                    }

                }
                else
                {
                    res.Message = "No Data ! ";
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

        #region Api for get Link

        /// <summary>
        /// API >> GET >> api/Dashboard/getlink
        /// Created By Ravi Vyas on 02-09-2022
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getlink")]
        public async Task<ResponseBodyModel> GetLink()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var linkData = await _db.QuickLinks.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId
                                               && x.OrgId == claims.orgId).ToListAsync();

                if (linkData.Count > 0)
                {
                    res.Message = "Links Found !";
                    res.Status = true;
                    res.Data = linkData;
                }
                else
                {
                    res.Message = "Link Not Found !";
                    res.Status = true;
                    res.Data = linkData;
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

        #region Api for Delete Link

        /// <summary>
        /// Created By Ravi Vyas on 02-09-2022
        /// API >> DETELE >> api/Dashboard/deletelinkdata?id
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("deletelinkdata")]
        public async Task<ResponseBodyModel> DeleteLink(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var linkData = await _db.QuickLinks.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.LinkId == id).FirstOrDefaultAsync();
                if (linkData != null)
                {
                    linkData.IsActive = false;
                    linkData.IsDeleted = true;
                    linkData.CompanyId = claims.companyId;
                    linkData.DeletedOn = DateTime.Now;
                    linkData.DeletedBy = claims.employeeId;

                    _db.Entry(linkData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Deleted Succedfully !";
                    res.Status = true;
                    res.Data = linkData;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = linkData;
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

        #region Api for Update Link

        /// <summary>
        /// Created By Ravi Vyas on 02-09-2022
        /// API >> PUT >> api/Dashboard/updatelinkdata
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("updatelinkdata")]
        public async Task<ResponseBodyModel> UpdateLink(QuickLinks model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var toolData = await _db.QuickLinks.Where(x => x.IsActive == true && x.IsDeleted == false &&
                               x.CompanyId == claims.companyId && x.LinkId == model.LinkId && ((x.LinkName.ToUpper().Trim() == model.LinkName.ToUpper().Trim()
                               || x.URL.ToUpper().Trim() == model.URL.ToUpper().Trim())
                               )).FirstOrDefaultAsync();
                if (toolData != null)
                {
                    toolData.LinkName = model.LinkName;
                    toolData.URL = model.URL;
                    toolData.IsActive = true;
                    toolData.IsDeleted = false;
                    toolData.CompanyId = claims.companyId;
                    toolData.UpdatedOn = DateTime.Now;
                    toolData.UpdatedBy = claims.employeeId;

                    _db.Entry(toolData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = " Update Successfully  !";
                    res.Status = true;
                    res.Data = toolData;
                }
                else
                {
                    res.Message = " Tools Already Exits  ! ";
                    res.Status = false;
                    res.Data = toolData;
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

        #region Api for get Link

        /// <summary>
        /// API >> GET >> api/Dashboard/getlinkbyid
        /// Created By Ravi Vyas on 02-09-2022
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getlinkbyid")]
        public async Task<ResponseBodyModel> GetLinkById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var linkData = await _db.QuickLinks.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId
                                               && x.OrgId == claims.orgId && x.LinkId == id).ToListAsync();

                if (linkData.Count > 0)
                {
                    res.Message = "Links Found !";
                    res.Status = true;
                    res.Data = linkData;
                }
                else
                {
                    res.Message = "Link Not Found !";
                    res.Status = true;
                    res.Data = linkData;
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

        #region Api's For  Company Polices

        #region This Api Use for Post Policy
        /// <summary>
        /// created by Mayank Prajapati on 05/09/2022
        /// Api >> Post >> api/Dashboard/addpolicy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpolicy")]
        [Authorize]
        public async Task<ResponseBodyModel> PostPolicy(CompanyPolicy model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Policy Not Found";
                    res.Status = false;
                }
                else
                {
                    CompanyPolicy obj = new CompanyPolicy
                    {
                        PolicyName = model.PolicyName,
                        PolicyGroupId = model.PolicyGroupId,
                        Link = model.Link,
                        PolicyDiscriyption = model.PolicyDiscriyption,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.CompanyPolicys.Add(obj);
                    await _db.SaveChangesAsync();
                    res.Message = "Policy Added Successfully  !";
                    res.Status = true;
                    res.Data = obj;

                    CompanyPolicyHistory cObj = new CompanyPolicyHistory
                    {
                        PolicyId = obj.PolicyId,
                        PolicyName = model.PolicyName,
                        PolicyGroupId = model.PolicyGroupId,
                        Link = model.Link,
                        PolicyDiscriyption = model.PolicyDiscriyption,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.CompanyPolicyHistorys.Add(cObj);
                    await _db.SaveChangesAsync();


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

        #region Api Use For Get All Policy
        /// <summary>
        /// created by Mayank Prajapati on 05/09/2022
        /// Api >> Post >> api/Dashboard/getallpolicy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallpolicy")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetPolicy()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = (from c in _db.CompanyPolicys
                               join ecp in _db.PolicyGroups on c.PolicyGroupId equals ecp.PolicyGroupId
                               where c.CompanyId == claims.companyId && c.IsActive == true && c.IsDeleted == false
                               //&& ecp.EmployeeId == claims.employeeid
                               select new CompanyPolicyRes
                               {
                                   PolicyId = c.PolicyId,
                                   PolicyDiscriyption = c.PolicyDiscriyption,
                                   PolicyGroupId = c.PolicyGroupId,
                                   PolicyName = c.PolicyName,
                                   Link = c.Link,
                                   PolicyGroupName = _db.PolicyGroups.Where(x => x.PolicyGroupId == c.PolicyGroupId).Select(x => x.PolicyGroupName).FirstOrDefault(),
                               }).ToList();

                if (getData.Count > 0)
                {
                    res.Message = "Policy Get !";
                    res.Status = true;
                    res.Data = getData;
                }
                else
                {
                    res.Message = "Policy Get Not Found";
                    res.Status = false;
                    res.Data = getData;
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

        #region Api for Get policy By id
        /// <summary>
        /// Created By Ravi Vyas on 15-09-2022 
        /// API>>GET>>api/Dashboard/getpolicybyid?id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpolicybyid")]

        public async Task<ResponseBodyModel> GetPolicyDataById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var poliyData = await (from p in _db.CompanyPolicys
                                       join pg in _db.PolicyGroups on p.PolicyGroupId equals pg.PolicyGroupId
                                       where p.PolicyId == id && p.IsActive == true && p.IsDeleted == false
                                       && p.CompanyId == claims.companyId
                                       select new
                                       {
                                           p.PolicyId,
                                           p.Link,
                                           p.PolicyName,
                                           p.PolicyDiscriyption,
                                           pg.PolicyGroupName,
                                           pg.PolicyGroupId,
                                       }).FirstOrDefaultAsync();

                if (poliyData != null)
                {
                    res.Message = "Policy Found !";
                    res.Status = true;
                    res.Data = poliyData;
                }
                else
                {
                    res.Message = "Policy Not Found !";
                    res.Status = false;
                    res.Data = poliyData;
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

        #region  Api for Update Policy Data
        /// <summary>
        /// Created By Mayank Prajapati On 05-09-2022
        /// Api >> Post >> api/Dashboard/updatepolicy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatepolicy")]
        public async Task<ResponseBodyModel> EditPolicy(CompanyPolicy model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var PolicyData = await _db.CompanyPolicys.FirstOrDefaultAsync(x => x.PolicyId == model.PolicyId && x.CompanyId == claims.companyId &&
                                                  x.IsActive == true && x.IsDeleted == false);
                if (PolicyData != null)
                {
                    PolicyData.PolicyName = model.PolicyName;
                    PolicyData.PolicyDiscriyption = model.PolicyDiscriyption;
                    PolicyData.Link = model.Link;
                    PolicyData.PolicyGroupId = model.PolicyGroupId;
                    PolicyData.UpdatedBy = claims.employeeId;
                    PolicyData.CompanyId = claims.companyId;
                    PolicyData.OrgId = claims.employeeId;
                    PolicyData.UpdatedOn = DateTime.Now;
                    PolicyData.IsActive = true;
                    PolicyData.IsDeleted = false;
                    _db.Entry(PolicyData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Policy Update Successfully  !";
                    res.Status = true;
                    res.Data = PolicyData;

                    CompanyPolicyHistory obj = new CompanyPolicyHistory
                    {
                        PolicyId = model.PolicyId,
                        PolicyName = model.PolicyName,
                        Link = model.Link,
                        PolicyDiscriyption = model.PolicyDiscriyption,
                        PolicyGroupId = model.PolicyGroupId,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = claims.employeeId,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.CompanyPolicyHistorys.Add(obj);
                    _db.SaveChanges();
                }
                else
                {
                    res.Message = " Failed To Update !";
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

        #region  Api for Delete Policy Data
        /// <summary>
        /// Created By Mayank Prajapati On 05-09-2022
        /// Api >> Post >> api/Dashboard/deletepolicydata?id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletepolicydata")]
        public async Task<ResponseBodyModel> DeletePolicyData(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var deletePoly = await _db.CompanyPolicys.FirstOrDefaultAsync(x => x.PolicyId == id && x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                if (deletePoly != null)
                {
                    deletePoly.IsDeleted = true;
                    deletePoly.IsActive = false;
                    deletePoly.DeletedBy = claims.employeeId;
                    deletePoly.DeletedOn = DateTime.Now;
                }
                _db.Entry(deletePoly).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();
                res.Status = true;
                res.Message = "Policy Deleted Successfully !";

                CompanyPolicyHistory obj = new CompanyPolicyHistory
                {
                    PolicyId = id,
                    PolicyGroupId = deletePoly.PolicyGroupId,
                    PolicyDiscriyption = deletePoly.PolicyDiscriyption,
                    PolicyName = deletePoly.PolicyName,
                    Link = deletePoly.Link,
                    IsDeleted = true,
                    IsActive = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = claims.employeeId,
                    CompanyId = claims.companyId,
                    OrgId = claims.orgId,
                    DeletedBy = claims.employeeId,
                    DeletedOn = DateTime.Now,

                };
                _db.CompanyPolicyHistorys.Add(obj);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region Api for add PolicyGroup 
        /// <summary>
        /// Created By Ravi Vyas on 12-09-2022
        /// API>>POST>>api/Dashboard/addpolicygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpolicygroup")]
        public async Task<ResponseBodyModel> AddGroup(AddEditGroupDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<EmployeePolicyGroup> list = new List<EmployeePolicyGroup>();
            try
            {
                if (model != null)
                {
                    PolicyGroup obj = new PolicyGroup
                    {
                        PolicyGroupName = model.PolicyGroupName,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.PolicyGroups.Add(obj);
                    await _db.SaveChangesAsync();
                    foreach (var item in model.EmployeePolicyGroup)
                    {
                        var member = _db.Employee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        if (member != null)
                        {
                            EmployeePolicyGroup newobj = new EmployeePolicyGroup
                            {
                                PolicyGroupId = obj.PolicyGroupId,
                                EmployeeId = member.EmployeeId,
                                DepartmentId = item.DepartmentId,
                                DesignationId = item.DesignationId,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = claims.userId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                            };
                            _db.EmployeePolicyGroups.Add(newobj);
                            await _db.SaveChangesAsync();
                            list.Add(newobj);
                        }
                    }
                    res.Message = "Policy Group Added Successfully !";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Model Is Empty  !";
                    res.Status = true;
                    res.Data = null;
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

        #region Api for Get PolicyGroup
        /// <summary>
        /// Create By Ravi Vyas on 10-09-2022
        /// API>>GET>>api/Dashboard/getpolicygroup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpolicygroup")]
        public async Task<ResponseBodyModel> GetPolicyGroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var groupData = await (from pg in _db.PolicyGroups
                                       where pg.IsActive == true && pg.IsDeleted == false && pg.CompanyId == claims.companyId
                                       select new PolicyGroupRes
                                       {
                                           PolicyGroupId = pg.PolicyGroupId,
                                           PolicyGroupName = pg.PolicyGroupName,
                                           CreatedOn = pg.CreatedOn,
                                           policyResponses = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == pg.PolicyGroupId).Distinct().Select(x => new PolicyResponse
                                           {
                                               EmployeeId = x.EmployeeId,
                                               PolicyGroupId = pg.PolicyGroupId,
                                               DepartmentName = _db.Department.Where(d => d.DepartmentId == x.DepartmentId).Select(d => d.DepartmentName).FirstOrDefault(),
                                               DesignationName = _db.Designation.Where(a => a.DesignationId == x.DesignationId).Select(a => a.DesignationName).FirstOrDefault(),
                                           }).Distinct().ToList(),
                                           Count = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == pg.PolicyGroupId).Count(),
                                       }).ToListAsync();
                if (groupData.Count > 0)
                {
                    res.Message = "Get Successfully !";
                    res.Status = true;
                    res.Data = new PaginationData1
                    {
                        TotalData = groupData.Count,
                        List = groupData.OrderByDescending(x => x.CreatedOn),
                    };
                }
                else
                {
                    res.Message = "Faild To Get Data !";
                    res.Status = false;
                    res.Data = groupData;
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

        #region Api To Edit PolicyGroup
        /// <summary>
        /// Created By Ravi Vyas on 12-09-2022
        /// API>>Put>>api/Dashboard/editpolicygroup
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editpolicygroup")]
        public async Task<ResponseBodyModel> EditTeam(AddEditGroupNewDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmployeePolicyGroup> list = new List<EmployeePolicyGroup>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var group = await _db.PolicyGroups.FirstOrDefaultAsync(x => x.PolicyGroupId == model.PolicyGroupId &&
                            x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true);

                if (group != null)
                {
                    group.PolicyGroupName = model.PolicyGroupName;
                    group.UpdatedOn = DateTime.Now;
                    group.UpdatedBy = claims.userId;
                    group.CompanyId = claims.companyId;
                    group.OrgId = claims.orgId;
                    _db.Entry(group).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var deleteMember = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == group.PolicyGroupId).ToList();

                    foreach (var item in deleteMember)
                        _db.Entry(item).State = EntityState.Deleted;
                    await _db.SaveChangesAsync();



                    var listObj = (from el in model.EmployeeId
                                   join em in _db.Employee on el equals em.EmployeeId
                                   select new EmployeePolicyGroup
                                   {
                                       EmployeeId = em.EmployeeId,
                                       DepartmentId = em.DepartmentId,
                                       DesignationId = em.DesignationId,
                                       CreatedBy = claims.userId,
                                       CompanyId = claims.companyId,
                                       OrgId = claims.orgId,
                                   }).ToList();
                    listObj.ForEach(x => x.PolicyGroupId = group.PolicyGroupId);
                    _db.EmployeePolicyGroups.AddRange(listObj);
                    await _db.SaveChangesAsync();



                    //foreach (var itemDepartment in model.DepartmentId)
                    //{
                    //    var departmentdata = _db.Department.Where(a => a.DepartmentId == itemDepartment).Select(a => a.DepartmentId).FirstOrDefault();
                    //    if (departmentdata == null)
                    //    {

                    //        foreach (var itemDesignation in model.DesignationId)
                    //        {
                    //            var designationdata = _db.Designation.Where(a => a.DesignationId == itemDesignation).Select(a => a.DesignationId).FirstOrDefault();
                    //            if (designationdata == null)
                    //            {


                    //                foreach (var item in model.EmployeeId)
                    //                {


                    //                    var member = _db.Employee.Where(x => x.EmployeeId == item).Select(x => x.EmployeeId).FirstOrDefault();
                    //#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    //                                        if (member != null)
                    //                                        {
                    //                                            var table = _db.EmployeePolicyGroups.Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims
                    //                                            .companyId && x.PolicyGroupId == model.PolicyGroupId && x.EmployeeId == member).FirstOrDefault();
                    //                                            if (table == null)
                    //                                            {
                    //                                                EmployeePolicyGroup groupobj = new EmployeePolicyGroup();
                    //                                                groupobj.PolicyGroupId = group.PolicyGroupId;
                    //                                                groupobj.EmployeeId = member;
                    //                                                //groupobj.DepartmentId = model.DepartmentId;
                    //                                                //groupobj.DesignationId = model.DesignationId;
                    //                                                groupobj.DepartmentId = departmentdata;
                    //                                                groupobj.DesignationId = designationdata;
                    //                                                groupobj.IsActive = true;
                    //                                                groupobj.IsDeleted = false;
                    //                                                groupobj.CreatedBy = claims.employeeId;
                    //                                                groupobj.CreatedOn = DateTime.Now;
                    //                                                groupobj.CompanyId = claims.companyId;
                    //                                                groupobj.OrgId = claims.orgId;
                    //                                                _db.EmployeePolicyGroups.Add(groupobj);
                    //                                                await _db.SaveChangesAsync();
                    //                                            }
                    //                                            else
                    //                                            {
                    //                                                table.PolicyGroupId = group.PolicyGroupId;
                    //                                                table.EmployeeId = member;
                    //                                                //table.DepartmentId = model.DepartmentId;
                    //                                                //table.DesignationId = model.DesignationId;
                    //                                                table.DepartmentId = departmentdata;
                    //                                                table.DesignationId = designationdata;
                    //                                                table.IsActive = true;
                    //                                                table.IsDeleted = false;
                    //                                                table.UpdatedBy = claims.employeeId;
                    //                                                table.UpdatedOn = DateTime.Now;
                    //                                                table.CompanyId = claims.companyId;
                    //                                                table.OrgId = claims.orgId;
                    //                                                _db.Entry(table).State = System.Data.Entity.EntityState.Modified;
                    //                                                await _db.SaveChangesAsync();
                    //                                            }

                    //                                            list.Add(table);
                    //                                        }
                    //#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    //}
                    //}
                    //}
                    //}
                    //}
                    res.Message = "Group Update Successfully !";
                    res.Status = true;
                    res.Data = group;

                    //    else
                    //{
                    //    res.Message = "Lead Not Found";
                    //    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Edit Team

        #region Api To Delete PolicyGroup
        /// <summary>
        /// Created By Ravi Vyas on 12-09-2022
        /// API >> Get >> api/Dashboard/deletepolicys?policyId
        /// </summary>
        /// <param name="teamid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletepolicys")]
        public async Task<ResponseBodyModel> DeleteTeam(int policyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var policyMaster = await _db.PolicyGroups.FirstOrDefaultAsync(x => x.PolicyGroupId == policyId && x.IsDeleted == false &&
                        x.CompanyId == claims.companyId && x.IsActive == true);
                if (policyMaster != null)
                {
                    policyMaster.IsActive = false;
                    policyMaster.IsDeleted = true;
                    policyMaster.DeletedBy = claims.userId;
                    policyMaster.DeletedOn = DateTime.Now;
                    _db.Entry(policyMaster).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var policDelete = await _db.CompanyPolicys.Where(x => x.PolicyGroupId == policyId && x.IsActive == true && x.IsDeleted == false
                    && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                    if (policDelete != null)
                    {
                        policDelete.IsActive = false;
                        policDelete.IsDeleted = true;
                        _db.Entry(policDelete).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var policyMember = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == policyId).ToList();
                    if (policyMember.Count > 0)
                    {
                        foreach (var item in policyMember)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;
                            item.DeletedBy = claims.userId;
                            item.DeletedOn = DateTime.Now;
                            _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Policy Deleted Successfully !";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Policy Not Found or Allready Deleted";
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

        #endregion Api To Delete PolicyGroup

        #region Api for Get PolicyGroup by Id
        /// <summary>
        /// Create By Ravi Vyas on 10-09-2022
        /// API>>GET>>api/Dashboard/getpolicygroupbyid?id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpolicygroupbyid")]
        public async Task<ResponseBodyModel> GetPolicyGroupById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var groupData = _db.PolicyGroups.Where(x => x.PolicyGroupId == id && x.IsActive == true &&
                x.IsDeleted == false && x.CompanyId == claims.companyId).FirstOrDefault();
                if (groupData != null)
                {
                    PolicyDTO obj = new PolicyDTO
                    {
                        PolicyGroupId = groupData.PolicyGroupId,
                        PolicyGroupName = groupData.PolicyGroupName,
                        DepartmentId = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == groupData.PolicyGroupId).Select(x => x.DepartmentId).ToList(),
                        DesignationId = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == groupData.PolicyGroupId).Select(x => x.DesignationId).ToList(),
                        ids = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == id).Select(x => x.EmployeeId).ToList(),
                        PolicyMemberList = _db.EmployeePolicyGroups.Where(x => x.PolicyGroupId == groupData.PolicyGroupId)
                                .Select(x => new PolicyResponse
                                {
                                    PolicyGroupId = x.PolicyGroupId,
                                    EmployeeId = x.EmployeeId,
                                    DepartmentName = _db.Department.Where(d => d.DepartmentId == x.DepartmentId).Select(d => d.DepartmentName).FirstOrDefault(),
                                    DesignationName = _db.Designation.Where(a => a.DesignationId == x.DesignationId).Select(a => a.DesignationName).FirstOrDefault(),
                                    DisplayName = _db.Employee.Where(e => e.EmployeeId == x.EmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                }).ToList(),
                    };
                    res.Message = "Get Successfully !";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Faild To Get Data !";
                    res.Status = false;
                    res.Data = null;
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

        #region Api for get policy By EmployeId
        /// <summary>
        /// Created By Ravi Vyas on 14-09-2022 
        /// API>>GET>>api/Dashboard/policybyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("policybyid")]
        public async Task<ResponseBodyModel> GetPolicyData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var policyData = await (from cp in _db.CompanyPolicys
                                        join pg in _db.EmployeePolicyGroups on cp.PolicyGroupId equals pg.PolicyGroupId
                                        join p in _db.PolicyGroups on cp.PolicyGroupId equals p.PolicyGroupId
                                        where cp.IsActive == true && cp.IsDeleted == false && cp.CompanyId == claims.companyId
                                        && pg.EmployeeId == claims.employeeId
                                        select new
                                        {
                                            pg.EmployeeId,
                                            cp.Link,
                                            cp.PolicyGroupId,
                                            p.PolicyGroupName,
                                            cp.PolicyName,
                                        }).ToListAsync();

                if (policyData.Count > 0)
                {
                    res.Message = "Policy Found !";
                    res.Status = true;
                    res.Data = policyData;
                }
                else
                {
                    res.Message = "Policy Not Found !";
                    res.Status = true;
                    res.Data = policyData;
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

        #region Api for Get Policy Content By Company And Group Id
        /// <summary>
        /// Created By Ravi Vyas on 14-09-2022
        /// API>>GET>>api/Dashboard/getpolicycontent
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpolicycontent")]
        public async Task<ResponseBodyModel> GetContent(int companyid, int groupid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getContent = await _db.CompanyPolicys.Where(x => x.IsActive == true && x.IsDeleted == false &&
                 x.CompanyId == companyid && x.PolicyGroupId == groupid).FirstOrDefaultAsync();
                if (getContent != null)
                {
                    res.Message = "Policy Content Found !";
                    res.Status = true;
                    res.Data = getContent;
                }
                else
                {
                    res.Message = "Policy Content Not Found !";
                    res.Status = false;
                    res.Data = getContent;
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

        #region Api for add PolicyGroup new  
        /// <summary>
        /// Created By Ravi Vyas on 12-09-2022
        /// API>>POST>>api/Dashboard/addpolicygroupnew
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpolicygroupnew")]
        public async Task<ResponseBodyModel> AddGroupNew(AddEditGroupNewDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<EmployeePolicyGroup> list = new List<EmployeePolicyGroup>();
            try
            {
                if (model != null && model.EmployeeId.Count > 0)
                {
                    PolicyGroup obj = new PolicyGroup
                    {
                        PolicyGroupName = model.PolicyGroupName,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.PolicyGroups.Add(obj);
                    await _db.SaveChangesAsync();


                    var listObj = (from el in model.EmployeeId
                                   join em in _db.Employee on el equals em.EmployeeId
                                   //join ds in _db.Department on em.DepartmentId equals ds.DepartmentId
                                   select new EmployeePolicyGroup
                                   {
                                       PolicyGroupId = obj.PolicyGroupId,
                                       EmployeeId = em.EmployeeId,
                                       DepartmentId = em.DepartmentId,
                                       DesignationId = em.DesignationId,
                                       CreatedBy = claims.userId,
                                       CompanyId = claims.companyId,
                                       OrgId = claims.orgId,
                                   }).ToList();
                    _db.EmployeePolicyGroups.AddRange(listObj);
                    await _db.SaveChangesAsync();
                    res.Message = "Policy Group Added Successfully !";
                    res.Status = true;
                    res.Data = obj;
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

        #region Api for get Employee for mention in Comments 
        /// <summary>
        /// API>>GET>>api/Dashboard/getallemployeelist
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeelist")]
        public async Task<IHttpActionResult> GetAllEmployeeList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted &&
                               x.CompanyId == tokenData.companyId)
                               .Select(x => new
                               {
                                   EmployeeId = x.EmployeeId,
                                   DisplayName = x.DisplayName
                               }).ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Data get succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/Dashboard/getallemployeelist", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Helper Model Class
        public class CompanyPolicyRes
        {
            public int PolicyId { get; set; }
            public int PolicyGroupId { get; set; }
            public string PolicyName { get; set; }
            public string Link { get; set; }
            public string PolicyDiscriyption { get; set; }
            public string PolicyGroupName { get; set; }
        }
        public class AddEditGroupDTO
        {
            public int PolicyGroupId { get; set; }
            public string PolicyGroupName { get; set; }
            public List<EmployeePolicyGroup> EmployeePolicyGroup { get; set; }
        }
        public class PolicyGroupRes
        {
            public int PolicyGroupId { get; set; }
            public string PolicyGroupName { get; set; }
            //public string DesignationName { get; set; }
            //public string DepartmentName { get; set; }
            //public int EmployeeId { get; set; }
            public DateTime CreatedOn { get; set; }
            public object policyResponses { get; set; }
            public int Count { get; set; }
        }
        public class PaginationData1
        {
            public int TotalData { get; set; }
            public object List { get; set; }

        }
        public class PolicyDTO
        {
            public int PolicyGroupId { get; set; }
            public string PolicyGroupName { get; set; }
            public List<int> DepartmentId { get; set; }
            public List<int> DesignationId { get; set; }
            public object ids { get; set; }
            public List<PolicyResponse> PolicyMemberList { get; set; }
        }

        public class PolicyResponse
        {
            public int PolicyGroupId { get; set; }
            public int EmployeeId { get; set; }
            //public int DepartmentId { get; set; }
            //public int DesignationId { get; set; }

            public string DepartmentName { get; set; }
            public string DesignationName { get; set; }
            public string DisplayName { get; set; }
        }

        public class AboutUsRes
        {
            public object DraftData { get; set; }
            public object PublishData { get; set; }
        }

        public class AddEditGroupNewDTO
        {
            public int PolicyGroupId { get; set; }
            public string PolicyGroupName { get; set; }
            //public int DepartmentId { get; set; }
            //public int DesignationId { get; set; }
            //public List<int> DepartmentId { get; set; }
            //public List<int> DesignationId { get; set; }
            public List<int> EmployeeId { get; set; }
        }

        //public class EmployeeRes
        //{
        //    public int EmployeeId { get; set; }
        //}


        #endregion


    }

}