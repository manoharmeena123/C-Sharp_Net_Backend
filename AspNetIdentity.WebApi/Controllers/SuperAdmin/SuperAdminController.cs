using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.SuperAdmin
{
    [Authorize]
    [RoutePrefix("api/masters")]
    public class SuperAdminController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        #region API TO GET YEAR LIST IN SUPER ADMIN DASHBOARD (MASTERS)
        /// <summary>
        /// Created By Harshit Mitra On 12-01-2023
        /// API >> GET >> api/masters/getyearlistinmasters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getyearlistinmasters")]
        public async Task<IHttpActionResult> GetYearListInMasters()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (tokenData.roleType != "SuperAdmin") return Unauthorized();

                var min = await _db.Company.MinAsync(x => x.CreatedOn.Year);
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                var companiesYears = Enumerable
                    .Range(min, today.Year - min + 1)
                    .Select(x => new
                    {
                        Year = x,
                    })
                    .ToList();

                if (companiesYears.Count > 0)
                {
                    res.Message = "Year List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = companiesYears
                        .OrderByDescending(x => x.Year)
                        .ToList();
                    return Ok(res);
                }
                res.Message = "There are No Year In The List";
                res.Status = false;
                res.StatusCode = HttpStatusCode.NoContent;
                res.Data = companiesYears;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/masters/getyearlistinmasters | " +
                    //"Pay Group Id : " + payGroupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET SUPER ADMIN DASHBIOARD (MASTERS)
        /// <summary>
        /// Created By Harshit Mitra On 12-01-2023
        /// API >> GET >> api/masters/getmasterdashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmasterdashboard")]
        public async Task<IHttpActionResult> GetMastersDashboard(int year, string searchString)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (tokenData.roleType != "SuperAdmin") return Unauthorized();

                var monthList = Enumerable
                    .Range(1, 12)
                    .Select(i => new
                    {
                        MonthId = i,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        LastDateOfMonth = new DateTime(year, i, DateTime.DaysInMonth(year, i)),
                    })
                    .OrderBy(x => x.MonthId)
                    .ToList();
                var employeeList = await _db.Employee
                    .Select(x => new { x.CreatedOn, x.CompanyId, x.OrgId })
                    .ToListAsync();
                var companyData = await (from c in _db.Company
                                         join o in _db.OrgMaster on c.CompanyId equals o.CompanyId
                                         where c.IsActive && !c.IsDeleted && o.IsActive && !o.IsDeleted
                                         select new
                                         {
                                             c.CompanyId,
                                             c.RegisterCompanyName,
                                             o.OrgId,
                                             o.OrgName,
                                             o.CreatedOn,
                                         })
                                         .ToListAsync();

                res.Message = "Dashboard";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    LineGraph = companyData
                        .Select(x => new
                        {
                            x.CompanyId,
                            x.RegisterCompanyName,
                        })
                        .Distinct()
                        .Select(x => new
                        {
                            Name = x.RegisterCompanyName,
                            Series = monthList
                                .Select(z => new
                                {
                                    Name = z.MonthName,
                                    Value = employeeList.LongCount(e => e.CompanyId == x.CompanyId && e.CreatedOn <= z.LastDateOfMonth),
                                })
                                .ToList(),
                        }).ToList(),
                    OrgListTable = (String.IsNullOrEmpty(searchString) || String.IsNullOrWhiteSpace(searchString)) ?
                        companyData
                            .Select(x => new
                            {
                                x.CompanyId,
                                x.RegisterCompanyName,
                                x.OrgId,
                                x.OrgName,
                                OrgCreateDate = x.CreatedOn,
                                EmployeeInOrg = employeeList.LongCount(z => z.OrgId == x.OrgId),
                                AdminInCompany = employeeList.LongCount(z => z.CompanyId == x.CompanyId && z.OrgId == 0),
                            })
                            .ToList() :
                        companyData
                            .Where(x => x.RegisterCompanyName.ToUpper().Contains(searchString.ToUpper()) || x.OrgName.ToUpper().Contains(searchString.ToUpper()))
                            .Select(x => new
                            {
                                x.CompanyId,
                                x.RegisterCompanyName,
                                x.OrgId,
                                x.OrgName,
                                OrgCreateDate = x.CreatedOn,
                                EmployeeInOrg = employeeList.LongCount(z => z.OrgId == x.OrgId),
                                AdminInCompany = employeeList.LongCount(z => z.CompanyId == x.CompanyId && z.OrgId == 0),
                            })
                            .ToList()
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/masters/getmasterdashboard | " +
                    "Year : " + year + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


    }
}
