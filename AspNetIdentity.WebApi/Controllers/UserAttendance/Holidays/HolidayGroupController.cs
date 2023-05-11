using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.UserAttendance.Holidays
{
    [Authorize]
    [RoutePrefix("api/holidaygroup")]
    public class HolidayGroupController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CREATE HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> POST >> api/holidaygroup/createholidaygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createholidaygroup")]
        public async Task<IHttpActionResult> CreateHolidayGroup(CreateHolidayGroupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var checkDuplicateName = await _db.HolidayGroups.AnyAsync(x => x.Title.ToUpper() == model.Title.ToUpper());
                if (checkDuplicateName)
                {
                    res.Message = "Holiday Group with same exist !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.Ambiguous;
                    return Ok(res);
                }
                HolidayGroup obj = new HolidayGroup
                {
                    Title = model.Title,
                    Description = model.Description,

                    CompanyId = tokenData.companyId,
                    CreatedBy = tokenData.employeeId,
                    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                };
                _db.HolidayGroups.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Holiday Group Created";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaygroup/createholidaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class CreateHolidayGroupRequest
        {
            public string Title { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
        }
        #endregion

        #region API TO UPDATE HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> POST >> api/holidaygroup/updateholidaygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateholidaygroup")]
        public async Task<IHttpActionResult> UpdateHolidayGroup(UpdateHolidayGroupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var group = await _db.HolidayGroups
                    .FirstOrDefaultAsync(x => x.GroupId == model.GroupId);
                if (group == null)
                {
                    res.Message = "Group Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                group.Title = model.Title;
                group.Description = model.Description;

                group.UpdatedBy = tokenData.employeeId;
                group.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Holiday Group Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = group;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaygroup/updateholidaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class UpdateHolidayGroupRequest : CreateHolidayGroupRequest
        {
            public Guid GroupId { get; set; } = Guid.Empty;
        }
        #endregion

        #region DELETE HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> POST >> api/holidaygroup/deleteholidaygroup
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteholidaygroup")]
        public async Task<IHttpActionResult> DeleteHolidayGroup(Guid groupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var group = await _db.HolidayGroups
                    .FirstOrDefaultAsync(x => x.GroupId == groupId);
                if (group == null)
                {
                    res.Message = "Group Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                group.IsActive = false;
                group.IsDeleted = true;

                group.DeletedBy = tokenData.employeeId;
                group.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                var emp = await _db.Employee.Where(x => x.HolidayGroupId == group.GroupId).ToListAsync();
                foreach (var item in emp)
                {
                    item.HolidayGroupId = Guid.Empty;
                    _db.Entry(item).State = EntityState.Modified;
                }

                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Holiday Group Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaygroup/deleteholidaygroup | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Group Id : " + groupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET HOLIDAY GROUP BY ID
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> GET >> api/holidaygroup/getholidaygroupbyid
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getholidaygroupbyid")]
        public async Task<IHttpActionResult> GetHolidayGroupById(Guid groupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var group = await _db.HolidayGroups
                    .FirstOrDefaultAsync(x => x.GroupId == groupId && x.IsActive && !x.IsDeleted);
                if (group == null)
                {
                    res.Message = "Group Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                res.Message = "Holiday Group Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = group;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaygroup/getholidaygroupbyid | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Group Id : " + groupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL HOLIDAY GROUP WITH DETAILS 
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> GET >> api/holidaygroup/getallholidaygroup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallholidaygroup")]
        public async Task<IHttpActionResult> GetAllHolidayGroup()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var groupData = await (from g in _db.HolidayGroups
                                       join ce in _db.Employee on g.CreatedBy equals ce.EmployeeId into ceEmpty
                                       from c in ceEmpty.DefaultIfEmpty()
                                       join ue in _db.Employee on g.UpdatedBy equals ue.EmployeeId into upEmpty
                                       from up in upEmpty.DefaultIfEmpty()
                                       where g.CompanyId == tokenData.companyId && g.IsActive && !g.IsDeleted
                                       select new
                                       {
                                           g.GroupId,
                                           g.Title,
                                           g.Description,
                                           CreateBy = g.IsDefaultCreated ? "Default Created" : c.DisplayName,
                                           CreatedOn = g.CreatedOn,
                                           UpdateBy = up != null ? up.DisplayName : String.Empty,
                                           UpdatedOn = up != null ? up.UpdatedOn.Value : DateTimeOffset.UtcNow,
                                       })
                                       .ToListAsync();
                if (groupData.Count == 0)
                {
                    res.Message = "Group Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                res.Message = "Holiday Group Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = groupData
                    .OrderByDescending(x => x.CreatedOn)
                    .Select(x => new
                    {
                        x.GroupId,
                        x.Title,
                        x.Description,
                        x.CreateBy,
                        CreatedOn = x.CreatedOn.Date.ToLongDateString(),
                        x.UpdateBy,
                        UpdatedOn = String.IsNullOrEmpty(x.UpdateBy) ? String.Empty : x.UpdatedOn.Date.ToLongDateString(),
                        EmployeeInGroup = groupData.LongCount(z => z.GroupId == x.GroupId) - 1,
                    })
                    .Distinct()
                    .ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidaygroup/getallholidaygroup | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    //"Group Id : " + groupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


    }
}
