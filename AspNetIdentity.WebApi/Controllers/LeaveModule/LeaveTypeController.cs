using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Leave;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
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
using static AspNetIdentity.WebApi.Controllers.NewPayRoll.PayGroupAndSettings.PayGroupController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.LeaveModule
{
    /// <summary>
    /// Created By Harshit Mitra On 14-02-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/leavetype")]
    public class LeaveTypeController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


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




        #region API TO CREATE LEAVE TYPE
        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// Modified By Harshit Mitra On 14-02-20223
        /// API >> POST >> api/leavecentral/addleavetype
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addleavetype")]
        public async Task<IHttpActionResult> AddLeaveType(CreateLeaveTypeRequest model)
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



                //LeaveTypeModel obj = new LeaveTypeModel
                //{
                //    LeaveTypeName = model.LeaveTypeName,
                //    Description = model.Description,
                //    IsPaidLeave = model.IsPaidLeave,
                //    RestrictToG = model.RestrictToG,
                //    Gender = model.Gender,
                //    RestrictToS = model.RestrictToS,
                //    Status = model.Status,
                //    IsReasonRequired = model.IsReasonRequired,
                //    CreatedBy = tokenData.employeeId,
                //    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                //    CompanyId = tokenData.companyId,
                //};
                //_db.LeaveTypes.Add(obj);
                //await _db.SaveChangesAsync();

                res.Message = "Leave Type Created";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/addpaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class CreateLeaveTypeRequest
        {
            public string LeaveTypeName { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public bool IsPaidLeave { get; set; } = false;
            public bool RestrictToG { get; set; } = false;
            public string Gender { get; set; } = String.Empty;
            public bool RestrictToS { get; set; } = false;
            public string Status { get; set; } = String.Empty;
            public bool IsReasonRequired { get; set; } = false;
        }
        #endregion API To Add Leave Type

        #region API To Get All Leave Type

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Get >> api/leavecentral/getallleavetype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallleavetype")]
        public async Task<ResponseBodyModel> GetAllLeaveType(int page, int count)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                // LeaveHelper.CheckLeave(claims.companyid);
                var leaveType = await _db.LeaveTypes.Where(x => x.CompanyId == claims.companyId &&
                        x.IsActive == true && x.IsDeleted == false).ToListAsync();
                leaveType = leaveType.OrderBy(x => x.UpdatedOn.HasValue ? x.UpdatedOn : x.CreatedOn).ToList();
                leaveType
                    .ForEach(x =>
                    {
                        x.CreatedByName = _db.GetEmployeeNameById(x.CreatedBy);
                        x.UpdatedByName = x.UpdatedBy.HasValue ? _db.GetEmployeeNameById((int)x.UpdatedBy) : null;
                        x.DeletedByName = x.DeletedBy.HasValue ? _db.GetEmployeeNameById((int)x.DeletedBy) : null;
                    });

                if (leaveType.Count > 0)
                {
                    res.Message = "Leave Type Found";
                    res.Status = true;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveType.Count,
                        Counts = count,
                        List = leaveType.Skip((page - 1) * count).Take(count).ToList(),
                    };
                }
                else
                {
                    res.Message = "No Leave Type Found";
                    res.Status = false;
                    res.Data = new PaginationData
                    {
                        TotalData = leaveType.Count,
                        Counts = count,
                        List = leaveType.Skip((page - 1) * count).Take(count).ToList(),
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

        #endregion API To Get All Leave Type

        #region API To Get Leave Type By Id

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/getleavetypebyid
        /// </summary>
        /// <param name="leaveTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getleavetypebyid")]
        public async Task<ResponseBodyModel> GetLeaveType(int leaveTypeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveType = await _db.LeaveTypes.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                            x.IsDeleted == false && x.LeaveTypeId == leaveTypeId).FirstOrDefaultAsync();
                if (leaveType != null)
                {
                    res.Message = "Leave Type Found";
                    res.Status = true;
                    res.Data = leaveType;
                }
                else
                {
                    res.Message = "No Leave Type Found";
                    res.Status = false;
                    res.Data = leaveType;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Type By Id

        #region API To Edit Leave Type

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/editleavetype
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editleavetype")]
        public async Task<ResponseBodyModel> EditLeaveType(LeaveType model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    var leaveType = await _db.LeaveTypes.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                                x.IsDeleted == false && x.LeaveTypeId == model.LeaveTypeId).FirstOrDefaultAsync();
                    if (leaveType != null)
                    {
                        leaveType.LeaveTypeName = model.LeaveTypeName;
                        leaveType.Description = model.Description;
                        leaveType.IsPaidLeave = model.IsPaidLeave;
                        leaveType.RestrictToG = model.RestrictToS;
                        leaveType.Gender = model.Gender;
                        leaveType.RestrictToS = model.RestrictToS;
                        leaveType.Status = model.Status;
                        leaveType.IsReasonRequired = model.IsReasonRequired;
                        leaveType.IsDelatable = model.IsDelatable;
                        leaveType.UpdatedBy = claims.employeeId;
                        leaveType.UpdatedOn = DateTime.Now;
                        leaveType.IsDelatable = !(model.LeaveTypeName == "Paid Leave" || model.LeaveTypeName == "Sick Leave" || model.LeaveTypeName == "Un-Paid Leave");

                        _db.Entry(leaveType).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Leave Type Updated";
                        res.Status = true;
                        res.Data = leaveType;
                    }
                    else
                    {
                        res.Message = "No Leave Type Created";
                        res.Status = false;
                        res.Data = leaveType;
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

        #endregion API To Edit Leave Type

        #region API To Delete Leave Type

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Delete >> api/leavecentral/deleteleavetype
        /// </summary>
        /// <param name="leaveTypeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteleavetype")]
        public async Task<ResponseBodyModel> EditLeaveType(int leaveTypeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leavetype = await _db.LeaveTypes.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                            x.IsDeleted == false && x.LeaveTypeId == leaveTypeId).FirstOrDefaultAsync();
                if (leavetype != null)
                {
                    leavetype.IsActive = false;
                    leavetype.IsDeleted = true;
                    leavetype.DeletedBy = claims.employeeId;
                    leavetype.DeletedOn = DateTime.Now;

                    _db.Entry(leavetype).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Type Deleted";
                    res.Status = true;
                    res.Data = leavetype;
                }
                else
                {
                    res.Message = "No Leave Type Found";
                    res.Status = false;
                    res.Data = leavetype;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Delete Leave Type

    }
}
