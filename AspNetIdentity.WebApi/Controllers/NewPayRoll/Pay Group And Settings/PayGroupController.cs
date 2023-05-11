using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.PayGroupAndSettings
{
    /// <summary>
    /// Created By Harshit Mitra On 08/12/2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/paygroup")]
    public class PayGroupController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO ADD PAY GORUP 
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/paygroup/addpaygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpaygroup")]
        public async Task<IHttpActionResult> CreatePayGroup(CreatePayGroupRequest model)
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
                else
                {
                    PayGroup obj = new PayGroup
                    {
                        PayGroupName = model.PayGroupName,
                        Description = model.Description,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                        //CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                    };
                    var payRollSetup = Enum.GetValues(typeof(PayRollSetupConstants))
                                .Cast<PayRollSetupConstants>()
                                .OrderBy(x => x)
                                .Select(x => new PayGroupSetup
                                {
                                    PayGroupId = obj.PayGroupId,
                                    StepsInSettings = x,

                                }).ToList();

                    _db.PayGroups.Add(obj);
                    _db.PayGroupSetups.AddRange(payRollSetup);
                    await _db.SaveChangesAsync();

                    res.Message = "Pay Group Created";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = obj;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/addpaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO EDIT PAY GORUP 
        /// <summary>
        /// Created By Harshit Mitra On 09/12/2022
        /// API >> POST >> api/paygroup/editpaygroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editpaygroup")]
        public async Task<IHttpActionResult> EditPayGroup(EditPayGorupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var payGroup = await _db.PayGroups
                        .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                    if (payGroup == null)
                    {
                        res.Message = "Pay Group Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        payGroup.PayGroupName = model.PayGroupName;
                        payGroup.Description = model.Description;

                        payGroup.UpdatedBy = tokenData.employeeId;
                        payGroup.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                        _db.Entry(payGroup).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Pay Group Updated";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Accepted;
                        res.Data = payGroup;
                    }
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/editpaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO DELETE PAY GORUP 
        /// <summary>
        /// Created By Harshit Mitra On 09/12/2022
        /// API >> POST >> api/paygroup/deletepaygroup
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletepaygroup")]
        public async Task<IHttpActionResult> DeletePayGroup(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var payGroup = await _db.PayGroups
                        .FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
                    if (payGroup == null)
                    {
                        res.Message = "Pay Group Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        if (_db.Employee.Count(x => x.PayGroupId == payGroupId) > 0)
                        {
                            res.Message = "You Cannot Delete This Pay Group \nIts Have Employee In It.";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NotAcceptable;
                            return Ok(res);
                        }
                        payGroup.IsActive = false;
                        payGroup.IsDeleted = true;

                        payGroup.DeletedBy = tokenData.employeeId;
                        payGroup.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                        _db.Entry(payGroup).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Pay Group Deleted";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Accepted;
                    }
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/deletepaygroup | " +
                    "Pay Group Id : " + payGroupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PAY GROUP BY ID
        /// <summary>
        /// Created By Harshit Mitra On 09/12/2022
        /// API >> GET >> api/paygroup/getpaygroupbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaygroupbyid")]
        public async Task<IHttpActionResult> GetPayGroupById(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payGroup = await _db.PayGroups
                    .Where(x => x.PayGroupId == payGroupId)
                    .Select(z => new
                    {
                        z.PayGroupId,
                        z.PayGroupName,
                        z.Description,

                    }).FirstOrDefaultAsync();

                if (payGroup != null)
                {
                    res.Message = "Pay Group List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = payGroup;
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Pay Group Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = payGroup;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getpaygroupbyid | " +
                    "Pay Group Id : " + payGroupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PAY GROUP LIST WITH EMPLOYEE COUNT
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> GET >> api/paygroup/getbyemployeecount
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyemployeecount")]
        public async Task<IHttpActionResult> GetPayGroupWithEmployeeCount()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payGroupList = await _db.PayGroups
                    .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                    .OrderBy(x => x.CreatedOn)
                    .Select(x => new GetPayGroupWithEmployeeCountResponse
                    {
                        PayGroupId = x.PayGroupId,
                        PayGroupName = x.PayGroupName,
                        Description = x.Description,
                        EmployeeCount = _db.Employee.Count(z => z.PayGroupId == x.PayGroupId),

                    })
                    .ToListAsync();

                if (payGroupList.Count > 0)
                {
                    res.Message = "Pay Group List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = payGroupList;
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Pay Group Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = payGroupList;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getbyemployeecount | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PAY GROUP SETTING BY PAY GROUP ID
        /// <summary>
        /// Created By Harshit Mitra On 09/12/2022
        /// API >> GET >> api/paygroup/getpaygroupsettinginfobyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaygroupsettinginfobyid")]
        public async Task<IHttpActionResult> GetPayGroupSetting(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var payGroup = await _db.PayGroups
                    .Where(x => x.PayGroupId == payGroupId)
                    .Select(z => new
                    {
                        z.PayGroupId,
                        z.PayGroupName,
                        z.Description,
                        SetupList = _db.PayGroupSetups
                             .Where(x => x.PayGroupId == payGroupId)
                             .OrderBy(x => x.StepsInSettings)
                             .Select(x => new GetPayRollSetupResponse
                             {
                                 Step = x.StepsInSettings,
                                 Title = x.StepsInSettings.ToString().Replace("_", " "),
                                 Status = x.IsSetupComplete ? "COMPLETED" : "PENDING",
                                 IsCompleted = x.IsSetupComplete,
                                 IsPrevious = true,
                             })
                             .ToList(),
                    }).FirstOrDefaultAsync();

                if (payGroup != null)
                {
                    res.Message = "Pay Group List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new
                    {
                        payGroup.PayGroupId,
                        payGroup.PayGroupName,
                        payGroup.Description,
                        SetupList = payGroup.SetupList
                            .Select(x => new GetPayRollSetupResponse
                            {
                                Step = x.Step,
                                Title = x.Title,
                                Status = x.Status,
                                IsCompleted = x.IsCompleted,
                                IsPrevious = x.Step == PayRollSetupConstants.Company_Info ? true
                                    : payGroup.SetupList
                                        .Where(z => (int)z.Step == (((int)x.Step) - 1))
                                        .Select(z => z.IsCompleted)
                                        .FirstOrDefault(),
                            })
                            .ToList(),
                    };
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Pay Group Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = payGroup;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getpaygroupsettinginfobyid | " +
                    "Pay Group Id : " + payGroupId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region RESPONSE AND REQUEST 
        public class CreatePayGroupRequest
        {
            [Required]
            public string PayGroupName { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
        }

        public class EditPayGorupRequest : CreatePayGroupRequest
        {
            public Guid PayGroupId { get; set; }
        }
        public class GetPayGroupWithEmployeeCountResponse
        {
            public Guid PayGroupId { get; set; }
            public string PayGroupName { get; set; }
            public string Description { get; set; }
            public int EmployeeCount { get; set; }
        }
        public class GetPayRollSetupResponse
        {
            public PayRollSetupConstants Step { get; set; }
            public string Title { get; set; }
            public string Status { get; set; }
            public bool IsCompleted { get; set; }
            public bool IsPrevious { get; set; }
        }
        #endregion
    }
}
