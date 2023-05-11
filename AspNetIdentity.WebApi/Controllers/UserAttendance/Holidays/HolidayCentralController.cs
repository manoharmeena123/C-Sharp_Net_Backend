using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.HolidayModel;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Models;
using LinqKit;
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
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.UserAttendance.Holidays
{
    [Authorize]
    [RoutePrefix("api/holidaycentral")]
    public class HolidayCentralController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO GET EMPLOYEE LIST FOR FILTER SELECT 
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/getemployeelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeelist")]
        public async Task<IHttpActionResult> GetEmployeeList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var empList = await (from e in _db.Employee
                                     where e.CompanyId == tokenData.companyId &&
                                        e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                     select new
                                     {
                                         EmployeeId = e.EmployeeId,
                                         DisplayName = e.DisplayName,
                                     })
                                     .OrderBy(x => x.DisplayName)
                                     .ToListAsync();
                if (empList.Count == 0)
                {
                    res.Message = "All Employee Assign To Holiday Groups";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = empList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/getemployeelist | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ORG LIST FOR FILTER
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/getorglist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getorglist")]
        public async Task<IHttpActionResult> GetOrgList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<GetOrgListResponse> list = new List<GetOrgListResponse>();
                list = await (from x in _db.OrgMaster
                              where x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted
                              select new GetOrgListResponse
                              {
                                  OrgId = x.OrgId,
                                  OrgName = x.OrgName,
                              })
                              .OrderBy(x => x.OrgName)
                              .ToListAsync();
                list.Add(new GetOrgListResponse());
                if (list.Count == 0)
                {
                    res.Message = "Org Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                res.Message = "Org List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = list;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/getorglist | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class GetOrgListResponse
        {
            public int OrgId { get; set; } = 0;
            public string OrgName { get; set; } = "Company Admin";
        }
        #endregion

        #region API TO GET ALL EMPLOYEE LIST IN HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/getemployeelistholidaygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getemployeelistholidaygroup")]
        public async Task<IHttpActionResult> GetEmployeeListHolidayGroup(EmployeeListInGroupFilterRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assignEmployee = await (from e in _db.Employee
                                            join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                            join eo in _db.OrgMaster on e.OrgId equals eo.OrgId into orgEmpty
                                            from o in orgEmpty.DefaultIfEmpty()
                                            join eh in _db.HolidayGroups on e.HolidayGroupId equals eh.GroupId into holEmpty
                                            from h in holEmpty.DefaultIfEmpty()
                                            where e.CompanyId == tokenData.companyId &&
                                               e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                            select new DepartmentAndGroupClassResponse
                                            {
                                                EmployeeId = e.EmployeeId,
                                                OfficeEmail = e.OfficeEmail,
                                                DisplayName = e.DisplayName,
                                                OrgId = o != null ? o.OrgId : 0,
                                                OrgName = o != null ? o.OrgName : "Admin In Company",
                                                DepartmentId = d.DepartmentId,
                                                DepartmentName = d.DepartmentName,
                                                GroupId = h != null ? h.GroupId : Guid.Empty,
                                                Title = h != null ? h.Title : "Unassigned",
                                            })
                                            .ToListAsync();

                #region Predicate Builder Checks

                var predicate = PredicateBuilder.New<DepartmentAndGroupClassResponse>(x => x.EmployeeId != 0);

                //if (model.EmployeeId.Count != 0)
                //    predicate.And(x => model.EmployeeId.Contains(x.EmployeeId));

                //if (model.OrgId.Count != 0)
                //    predicate.And(x => model.OrgId.Contains(x.OrgId));

                //if (model.DepartmentId.Count != 0)
                //    predicate.And(x => model.DepartmentId.Contains(x.DepartmentId));

                //if (model.GroupId.Count != 0)
                //    predicate.And(x => model.GroupId.Contains(x.GroupId));

                if (model.SearchString.Length >= 3 && String.IsNullOrEmpty(model.SearchString))
                {
                    var searchString = model.SearchString.Trim().ToUpper();
                    predicate
                        .And(x =>
                                x.DisplayName.ToUpper().Contains(searchString)
                                || x.OfficeEmail.ToUpper().Contains(searchString)
                                || x.OrgName.ToUpper().Contains(searchString)
                                || x.DepartmentName.ToUpper().Contains(searchString)
                                || x.Title.ToUpper().Contains(searchString)
                        );
                }

                assignEmployee = assignEmployee.Where(predicate.Compile()).ToList();

                #endregion

                HolidayGroupSwitch(model.Filter, model.DescOrder, ref assignEmployee);

                if (assignEmployee.Count == 0)
                {
                    res.Message = "There Are No Employee";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = assignEmployee;
                }
                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = assignEmployee;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/getemployeelistholidaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class EmployeeListResponse
        {
            public int EmployeeId { get; set; } = 0;
            public string OfficeEmail { get; set; } = String.Empty;
            public string DisplayName { get; set; } = String.Empty;
            public int OrgId { get; set; } = 0;
            public string OrgName { get; set; } = String.Empty;
            //public int DepartmentId { get; set; } = 0;
            public string DepartmentName { get; set; } = String.Empty;
        }
        public class DepartmentAndGroupClassResponse : EmployeeListResponse
        {
            public int DepartmentId { get; set; } = 0;
            public Guid GroupId { get; set; } = Guid.Empty;
            public string Title { get; set; } = String.Empty;
        }
        public class EmployeeListInGroupFilterRequest : EmplListFilterRequest
        {
            //public List<int> EmployeeId { get; set; } = new List<int>();
            //public List<Guid> GroupId { get; set; } = new List<Guid>();
            //public List<int> DepartmentId { get; set; } = new List<int>();
            public EmployeeTableFilterScope Filter { get; set; } = EmployeeTableFilterScope.NoFilter;
            public bool DescOrder { get; set; } = false;
        }
        public enum EmployeeTableFilterScope
        {
            NoFilter = 0,
            EmployeeName = 1,
            OrgName = 2,
            DepartmentName = 3,
            GroupName = 4,
        }
        public void HolidayGroupSwitch(EmployeeTableFilterScope obj, bool order, ref List<DepartmentAndGroupClassResponse> list)
        {
            if (!order)
            {
                switch (obj)
                {
                    case EmployeeTableFilterScope.EmployeeName:
                        list = list.OrderBy(x => x.DisplayName).ToList();
                        break;
                    case EmployeeTableFilterScope.OrgName:
                        list = list.OrderBy(x => x.OrgName).ToList();
                        break;
                    case EmployeeTableFilterScope.DepartmentName:
                        list = list.OrderBy(x => x.DepartmentName).ToList();
                        break;
                    case EmployeeTableFilterScope.GroupName:
                        list = list.OrderBy(x => x.Title).ToList();
                        break;
                    default:
                        list = list.OrderBy(x => x.DisplayName).ThenBy(x => x.OrgName).ThenBy(x => x.DepartmentName).ThenBy(x => x.Title).ToList();
                        break;
                }
            }
            else
            {
                switch (obj)
                {
                    case EmployeeTableFilterScope.EmployeeName:
                        list = list.OrderByDescending(x => x.DisplayName).ToList();
                        break;
                    case EmployeeTableFilterScope.OrgName:
                        list = list.OrderByDescending(x => x.OrgName).ToList();
                        break;
                    case EmployeeTableFilterScope.DepartmentName:
                        list = list.OrderByDescending(x => x.DepartmentName).ToList();
                        break;
                    case EmployeeTableFilterScope.GroupName:
                        list = list.OrderByDescending(x => x.Title).ToList();
                        break;
                    default:
                        list = list.OrderBy(x => x.DisplayName).ThenBy(x => x.OrgName).ThenBy(x => x.DepartmentName).ThenBy(x => x.Title).ToList();
                        break;
                }
            }
        }
        #endregion

        // ----- Holidays Part In Holiday Groups ----- //

        #region API TO ASSIGN HOLIDAY IN HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/assignholidayinholidaygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("assignholidayinholidaygroup")]
        public async Task<IHttpActionResult> AssignHolidayInHolidayGroup(AssignHolidayInHoidayGroupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var checkHolidayGroup = await _db.HolidayInGroups
                    .Where(x => x.GroupId == model.GroupId)
                    .ToListAsync();

                if (checkHolidayGroup.Count != 0)
                    _db.HolidayInGroups.RemoveRange(checkHolidayGroup);

                var setData = model.HolidayList
                        .Where(x => x.IsChecked)
                        .Select(x => new HolidayInGroup
                        {
                            GroupId = model.GroupId,
                            HolidayId = x.HolidayId,

                        }).ToList();
                _db.HolidayInGroups.AddRange(setData);
                await _db.SaveChangesAsync();
                res.Message = "Holiday In Groups Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/assignholidayinholidaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AssignHolidayInHoidayGroupRequest
        {
            public Guid GroupId { get; set; } = Guid.Empty;
            public List<InnerAssignHolidayClass> HolidayList { get; set; } = new List<InnerAssignHolidayClass>();
        }
        #endregion

        #region API TO GET HOLIDAY IN HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/getassignholidayingroup
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getassignholidayingroup")]
        public async Task<IHttpActionResult> GetAssignHolidayInGroup(Guid groupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var holidayList = await _db.HolidayModels
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                    .ToListAsync();
                var checkHolidayGroup = await _db.HolidayInGroups
                    .Where(x => x.GroupId == groupId)
                    .Select(x => x.HolidayId)
                    .ToListAsync();

                var response = holidayList
                    .Select(x => new InnerAssignHolidayClass
                    {
                        IsChecked = checkHolidayGroup.Contains(x.HolidayId),
                        HolidayId = x.HolidayId,
                        HolidayName = x.HolidayName,
                        IsFloaterOptional = x.IsFloaterOptional,
                        ImageUrl = x.ImageUrl,
                        HolidayDate = x.HolidayDate,
                    })
                    .OrderByDescending(x => x.IsChecked)
                    .ThenBy(x => x.HolidayDate)
                    .ToList();

                res.Message = "Holiday In Groups";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/getassignholidayingroup | " +
                    "Group Id : " + groupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class InnerAssignHolidayClass
        {
            public bool IsChecked { get; set; } = false;
            public Guid HolidayId { get; set; } = Guid.Empty;
            public string HolidayName { get; set; } = String.Empty;
            public bool IsFloaterOptional { get; set; } = false;
            public string ImageUrl { get; set; } = String.Empty;
            public DateTimeOffset HolidayDate { get; set; } = DateTimeOffset.UtcNow;
        }
        #endregion

        // ----- Employee Part In Holiday Groups ----- //

        #region API TO ASSIGN EMPLOYEE IN HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/assignemployeeingroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("assignemployeeingroup")]
        public async Task<IHttpActionResult> AssignEmployeeInGroup(AssignEmployeeInHoidayGroupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var modelEmployeeIds = model.EmployeeList.Where(x => x.IsChecked).Select(x => x.EmployeeId).ToList();
                var allEmployees = await _db.Employee
                    .Where(x => x.HolidayGroupId == model.GroupId || modelEmployeeIds.Contains(x.EmployeeId))
                    .ToListAsync();
                foreach (var item in allEmployees)
                {
                    item.HolidayGroupId = model.EmployeeList.Any(x => x.EmployeeId == item.EmployeeId && x.IsChecked) ? model.GroupId : Guid.Empty;
                    _db.Entry(item).State = EntityState.Modified;
                }
                await _db.SaveChangesAsync();
                res.Message = "Employees Updated In Group";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/assignemployeeingroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AssignEmployeeInHoidayGroupRequest
        {
            public Guid GroupId { get; set; } = Guid.Empty;
            public List<IsCheckClass> EmployeeList { get; set; } = new List<IsCheckClass>();
        }
        #endregion

        #region API TO GET EMPLOYEES IN HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 07-02-2023
        /// API >> GET >> api/holidaycentral/getassignemployeeingroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getassignemployeeingroup")]
        public async Task<IHttpActionResult> GetAssignEmployeeInGroup(GroupIdRequestFilter model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assignEmployee = await (from e in _db.Employee
                                            join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                            join eo in _db.OrgMaster on e.OrgId equals eo.OrgId into orgEmpty
                                            from o in orgEmpty.DefaultIfEmpty()
                                            where e.CompanyId == tokenData.companyId &&
                                               e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                               (e.HolidayGroupId == model.GroupId || e.HolidayGroupId == Guid.Empty)
                                            select new IsCheckClass
                                            {
                                                IsChecked = (e.HolidayGroupId == model.GroupId),
                                                EmployeeId = e.EmployeeId,
                                                OfficeEmail = e.OfficeEmail,
                                                DisplayName = e.DisplayName,
                                                OrgId = o != null ? o.OrgId : 0,
                                                OrgName = o != null ? o.OrgName : "Admin In Company",
                                                //DepartmentId = d.DepartmentId,
                                                DepartmentName = d.DepartmentName,
                                            })
                                            .OrderByDescending(x => x.IsChecked)
                                            .ThenBy(x => x.DisplayName)
                                            .ThenBy(x => x.DepartmentName)
                                            .ToListAsync();
                #region Filters Checks

                var predicate = PredicateBuilder.New<IsCheckClass>(x => x.EmployeeId != 0);

                if (model.OrgId.Count != 0)
                    predicate.And(x => model.OrgId.Contains(x.OrgId));

                if (model.SearchString.Length >= 3 && !String.IsNullOrEmpty(model.SearchString))
                {
                    var searchString = model.SearchString.Trim().ToUpper();
                    predicate
                        .And(x =>
                                x.DisplayName.ToUpper().Contains(searchString)
                                || x.OrgName.ToUpper().Contains(searchString)
                                || x.DepartmentName.ToUpper().Contains(searchString)
                                || x.OfficeEmail.ToUpper().Contains(searchString)
                                || x.DisplayName.ToUpper().Contains(searchString)
                        );
                }

                assignEmployee = assignEmployee.Where(predicate.Compile()).ToList();

                #endregion

                if (assignEmployee.Count == 0)
                {
                    res.Message = "There Are No Employee";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = assignEmployee;
                }
                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = assignEmployee;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaycentral/getassignemployeeingroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    //"Group Id : " + groupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class IsCheckClass : EmployeeListResponse
        {
            public bool IsChecked { get; set; } = false;
        }
        public class EmplListFilterRequest
        {
            public string SearchString { get; set; } = String.Empty;
            public List<int> OrgId { get; set; } = new List<int>();
        }
        public class GroupIdRequestFilter : EmplListFilterRequest
        {
            public Guid GroupId { get; set; } = Guid.Empty;
        }
        #endregion



        #region API TO CONVERT ALL HOLIDAYS
        /// <summary>
        /// api/holidaycentral/convertholidays
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("convertholidays")]
        public async Task ConvertHolidays()
        {
            var setData = await (from c in _db.Company
                                 join h in _db.Holidays on c.CompanyId equals h.CompanyId
                                 where h.IsActive && !h.IsDeleted
                                 select new
                                 {
                                     c.CompanyDefaultTimeZone,
                                     c.CompanyId,
                                     h.CreatedOn,
                                     h.CreatedBy,
                                     h.HolidayDate,
                                     h.ImageUrl,
                                     h.HolidayName,
                                     h.Description,
                                     h.IsFloaterOptional,
                                     h.TextColor,
                                 })
                                  .ToListAsync();
            var holidayInsert = setData
                .Select(model => new HolidayModel
                {
                    HolidayName = model.HolidayName,
                    Description = model.Description,
                    IsFloaterOptional = model.IsFloaterOptional,
                    ImageUrl = model.ImageUrl,
                    TextColor = model.TextColor,
                    HolidayDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.HolidayDate.Date, model.CompanyDefaultTimeZone),

                    CompanyId = model.CompanyId,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(model.CreatedOn.Date, model.CompanyDefaultTimeZone),
                })
                .ToList();
            _db.HolidayModels.AddRange(holidayInsert);
            await _db.SaveChangesAsync();
        }
        #endregion

        #region API TO CREATE DEFAULT HOLIDAY IF NOT EXIST IN ALL COMPANIES
        /// <summary>
        /// api/holidaycentral/createdefaultholiday
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("createdefaultholiday")]
        public async Task CreateDefaultHoliday()
        {
            var companies = await (from c in _db.Company
                                   join h in _db.HolidayGroups.Where(x => x.IsDefaultCreated).AsEnumerable()
                                        on c.CompanyId equals h.CompanyId into holEmpty
                                   from eh in holEmpty.DefaultIfEmpty()
                                   select new
                                   {
                                       c.CompanyId,
                                       c.CreatedOn,
                                       IsNotPresent = (eh == null),

                                   })
                                   .Where(x => x.IsNotPresent)
                                   .ToListAsync();
            foreach (var item in companies)
            {
                HolidayGroup obj = new HolidayGroup
                {
                    Title = "Default Holiday Group",
                    Description = "This Is Default Holiday Group For All Users",
                    CompanyId = item.CompanyId,
                    IsDefaultCreated = true,
                    CreatedOn = item.CreatedOn,
                };
                var holidays = await _db.HolidayModels
                    .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == item.CompanyId)
                    .ToListAsync();
                var addHolidayInGroup = holidays
                    .Select(x => new HolidayInGroup
                    {
                        GroupId = obj.GroupId,
                        HolidayId = x.HolidayId,
                    }).ToList();
                var emp = await _db.Employee.Where(x => x.CompanyId == item.CompanyId).ToListAsync();
                foreach (var e in emp)
                {
                    e.HolidayGroupId = obj.GroupId;
                    _db.Entry(e).State = EntityState.Modified;
                }

                _db.HolidayGroups.Add(obj);
                _db.HolidayInGroups.AddRange(addHolidayInGroup);
                await _db.SaveChangesAsync();
            }
        }
        #endregion



    }
}
