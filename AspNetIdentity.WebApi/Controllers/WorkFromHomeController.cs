using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/WorkFromHome")]
    public class WorkFromHomeController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        #region-WFH Request APIs for HR Admin

        #region This Api is used to Craete Wfh Request

        /// <summary>
        /// Created By Ankit Date - 23/05/2022
        /// </summary>Route >> Post >> api/WorkFromHome/CreateWFHRequest
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreateWFHRequest")]
        [HttpPost]
        public async Task<ResponseBodyModel> CreateWFHRequest(AddWfhRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    if (model.NotifyEmployees.Count > 0)
                    {
                        WorkFromHome obj = new WorkFromHome
                        {
                            EmployeeId = claims.employeeId,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            NumberOfDays = model.NumberOfDays,
                            Comment = model.Comment,
                            AppliedBy = claims.displayName,
                            WFHStatus = "Pending",
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                        };
                        db.WorkFromHomes.Add(obj);
                        await db.SaveChangesAsync();
                        obj.NotifyEmployees = new List<WfhNofifyByEmployee>();

                        foreach (var item in model.NotifyEmployees)
                        {
                            WfhNofifyByEmployee obj2 = new WfhNofifyByEmployee
                            {
                                WFHId = obj.WFHId,
                                EmployeeId = item,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                            };
                            db.WfhNotify.Add(obj2);
                            await db.SaveChangesAsync();
                            obj.NotifyEmployees.Add(obj2);
                        }
                        res.Message = "Sucess";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "You Didnt Select Any Employee To Notify";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Invalid Model";
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

        #endregion This Api is used to Craete Wfh Request

        #region This Api Use for get All request by notify Wfh

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Get >> api/WorkFromHome/GetAllWFHRequestsNotify
        /// </summary>
        /// <returns></returns>
        [Route("GetAllWFHRequestsNotify")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllWFHRequests()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var notifyRequest = await db.WfhNotify.Where(x => x.EmployeeId == claims.employeeId).ToListAsync();
                if (notifyRequest.Count > 0)
                {
                    var checkIds = notifyRequest.Select(x => x.WFHId).Distinct().ToList();
                    var wfhRequest = await db.WorkFromHomes.Where(x => checkIds.Contains(x.WFHId)).Select(x => new
                    {
                        x.WFHId,
                        x.StartDate,
                        x.EndDate,
                        x.NumberOfDays,
                        x.WFHStatus,
                        x.Comment,
                        x.CreatedOn,
                        x.AppliedBy,
                    }).ToListAsync();
                    if (wfhRequest.Count > 0)
                    {
                        res.Message = "WFH Request Found";
                        res.Status = true;
                        res.Data = wfhRequest.OrderByDescending(x => x.WFHId).ToList();
                    }
                    else
                    {
                        res.Message = "No WFH Request Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "No WFH Request Found";
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

        #endregion This Api Use for get All request by notify Wfh

        #region This Api Is used To Get All Wfh Request By Employee ID

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Get >> api/WorkFromHome/GetAllWFHRequest
        /// </summary>
        /// <returns></returns>
        [Route("GetAllWFHRequest")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllWFHRequestsByEmployeeId()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var WFHRequestData = await db.WorkFromHomes.Where(x => x.EmployeeId == claims.employeeId && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .Select(x => new
                        {
                            x.WFHId,
                            x.StartDate,
                            x.EndDate,
                            x.Comment,
                            x.AppliedBy,
                            x.WFHStatus,
                            x.CreatedOn,
                            x.NumberOfDays
                        }).ToListAsync();
                if (WFHRequestData.Count != 0)
                {
                    res.Message = "WFH Request list Found";
                    res.Status = true;
                    res.Data = WFHRequestData.OrderByDescending(x => x.WFHId).ToList();
                }
                else
                {
                    res.Status = false;
                    res.Message = "No WFH Request list Found";
                    res.Data = WFHRequestData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Is used To Get All Wfh Request By Employee ID

        #region This Api use For Update Approved Wfh Status

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Put >> api/WorkFromHome/WFHRequestStatusUpdate
        /// </summary>
        /// <returns></returns>
        [Route("WFHRequestStatusUpdate")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> WFHRequestStatusUpdate(WFHApproved model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateDepData = await db.WorkFromHomes.Where(x => x.WFHId == model.WFHId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (updateDepData != null)
                {
                    updateDepData.WFHStatus = model.WFHStatus;
                    db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Status = true;
                    res.Message = "WFH Request Status Updated Successfully!";
                    res.Data = updateDepData;
                }
                else
                {
                    res.Status = true;
                    res.Message = "WFH Request Status Not Updated Successfully!";
                    res.Data = updateDepData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api use For Update Approved Wfh Status

        #region This Api Is used to Employee Today on wfh

        /// <summary>
        /// Created By Ankit date- 23/05/2022
        /// API >> Get >> api/WorkFromHome/EmployeesTodayOnWorkFromHome
        /// </summary>
        /// <returns></returns>
        [Route("EmployeesTodayOnWorkFromHome")]
        [HttpGet]
        public async Task<ResponseBodyModel> EmployeesTodayOnWorkFromHome()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<WFHData1> employeeDataList = new List<WFHData1>();
            try
            {
                var employeeData = await (from ad in db.Employee
                                          join lr in db.WorkFromHomes on ad.EmployeeId equals lr.EmployeeId
                                          orderby lr.StartDate descending
                                          where ad.CompanyId == claims.companyId && ad.OrgId == claims.orgId && lr.StartDate <= DateTime.Today && lr.EndDate >= DateTime.Today && lr.WFHStatus == "Approved" && ad.IsDeleted == false
                                          select new
                                          {
                                              ad.EmployeeId,
                                              n = ad.FirstName + " " + ad.LastName
                                          }).ToListAsync();
                foreach (var item in employeeData)
                {
                    WFHData1 data = new WFHData1();
                    data.EmployeeId = item.EmployeeId;
                    data.FullName = item.n;
                    employeeDataList.Add(data);
                }
                if (employeeDataList.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Data Found";
                    res.Data = employeeDataList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No employee on leave today.";
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

        #endregion This Api Is used to Employee Today on wfh

        #region This Api Used For Get wfh Request By Id

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Get >> api/WorkFromHome/GetWFHRequestById
        /// </summary>
        /// <returns></returns>
        [Route("GetWFHRequestById")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetWFHRequestById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var WfhRequest = await db.WorkFromHomes.FirstOrDefaultAsync(x => x.WFHId == Id && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId);
                if (WfhRequest != null)
                {
                    res.Message = "WFH Request Found";
                    res.Status = true;
                    res.Data = WfhRequest;
                }
                else
                {
                    res.Message = "No WFH Request Found";
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

        #endregion This Api Used For Get wfh Request By Id

        #region This Api used For Upadte Wfh Request

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Put >> api/WorkFromHome/UpdateWFHRequest
        /// </summary>
        /// <returns></returns>
        [Route("UpdateWFHRequest")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> UpdateWFHRequest(UpdateWfhRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateDepData = await db.WorkFromHomes.Where(x => x.WFHId == model.WFHId && x.IsDeleted == false && x.IsApproved != true).FirstOrDefaultAsync();
                if (updateDepData != null)
                {
                    updateDepData.AppliedBy = model.AppliedBy;
                    updateDepData.Comment = model.Comment;
                    updateDepData.StartDate = model.StartDate;
                    updateDepData.EndDate = model.EndDate;
                    updateDepData.NumberOfDays = model.NumberOfDays;
                    updateDepData.WFHStatus = model.WFHStatus;
                    updateDepData.IsDeleted = false;
                    db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    res.Status = true;
                    res.Data = updateDepData;
                    res.Message = "WorkFromHomeRequest Updated Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No WFH Request Found!!";
                    res.Data = updateDepData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api used For Upadte Wfh Request

        #region This Api Used For Delete Wfh Request

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Delete >> api/WorkFromHome/DeleteWFHRequest
        /// </summary>
        /// <returns></returns>
        [Route("DeleteWFHRequest")]
        [HttpDelete]
        [Authorize]
        public async Task<ResponseBodyModel> DeleteWFHRequest(int WFHId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var itemToRemove = await db.WorkFromHomes.Where(x => x.WFHId == WFHId && x.IsActive == true && x.IsDeleted == false)
                          .FirstOrDefaultAsync();
                if (itemToRemove != null)
                {
                    itemToRemove.IsDeleted = true;
                    itemToRemove.IsActive = false;
                    itemToRemove.DeletedOn = DateTime.Now;
                    db.Entry(itemToRemove).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    res.Status = true;
                    res.Message = "Wfh Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No WFH Request Found!!";
                    res.Data = itemToRemove;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Used For Delete Wfh Request

        #region This api is used to Get by Leave Requests By Status

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Get >> api/WorkFromHome/GetAllWorkFromByStatus
        /// </summary>
        /// <returns></returns>
        [Route("GetAllWorkFromByStatus")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllWorkFromByStatus(string Status)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                WorkFromHomeDataList dep = new WorkFromHomeDataList();
                var WorkFromHomData = await db.WorkFromHomes.Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId && x.WFHStatus.Trim().ToUpper() == Status.Trim().ToUpper()).ToListAsync();
                if (WorkFromHomData.Count != 0)
                {
                    res.Message = "All " + Status + "Work From list Found";
                    res.Status = true;
                    res.Data = WorkFromHomData;
                }
                else
                {
                    res.Message = "No " + Status + "WorkFrom list not Found";

                    res.Status = false;
                    res.Data = WorkFromHomData;
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This api is used to Get by Leave Requests By Status

        #region This api Used for Get by WFH Requests By Status

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Get >> api/WorkFromHome/GetAllWFHRequestsByStatus
        /// </summary>
        /// <returns></returns>
        [Route("GetAllWFHRequestsByStatus")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllWFHRequestsByStatus(string Status)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var WFHRequestData = await db.WorkFromHomes.Where(x => x.IsDeleted == false && x.WFHStatus.Trim().ToUpper() == Status.Trim().ToUpper()).ToListAsync();
                if (WFHRequestData.Count != 0)
                {
                    res.Message = "All " + Status + " WFH Request list Found";
                    res.Status = true;
                    res.Data = WFHRequestData;
                }
                else
                {
                    res.Message = "No " + Status + " WFH Request list Found";
                    res.Status = false;
                    res.Data = WFHRequestData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This api Used for Get by WFH Requests By Status

        #region This Api used For Wfh Request Status

        /// <summary>
        /// created by ankit jain Date - 24/05/2022
        /// Api >> Get >> api/WorkFromHome/WFHRequestsStatus
        /// </summary>
        /// <returns></returns>
        [Route("WFHRequestsStatus")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllWFHRequestsStatus(string Status)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var WFHRequestData = await db.WorkFromHomes.Where(x => x.IsDeleted == false && x.WFHStatus == "Approved").ToListAsync();
                if (WFHRequestData.Count != 0)
                {
                    res.Message = "All " + Status + " WFH Request list Found";
                    res.Status = true;
                    res.Data = WFHRequestData;
                }
                else
                {
                    res.Message = "No " + Status + " WFH Request list Found";
                    res.Status = false;
                    res.Data = WFHRequestData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api used For Wfh Request Status

        #region This api's is used for Get by All work from Home Request behalf on company and org

        /// <summary>
        /// API >> Get  >> api/WorkFromHome/getwfhallrequests
        /// Create by Shriya Malvi On 28-06-2022
        /// </summary>
        /// <returns></returns>
        [Route("getwfhallrequests")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetWFHAllRequests()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var workfromhome = await db.WorkFromHomes.Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();

                if (workfromhome.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Work From Home Request list Found";
                    res.Data = workfromhome;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Work From Home Request list not Found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This api's is used for Get by All work from Home Request behalf on company and org

        #endregion

        #region Helper Model

        public class WorkFromHomeData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public WorkFromHome WorkFromHomeRequest { get; set; }
        }

        public class WorkFromHomeDataList
        {
            public WorkFromHome AddWfhRequest { get; set; }
            public List<WfhNofifyByEmployee> WFHList { get; set; }
        }

        public class WFHApproved
        {
            public string WFHStatus { get; set; }
            public int WFHId { get; set; }
        }

        public class WFHData1
        {
            public string FullName { get; set; }
            public int EmployeeId { get; set; }
        }

        /// <summary>
        /// Created By Ankit Date- 24/05/2022
        /// </summary>
        public class AddWfhRequest
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string NumberOfDays { get; set; }
            public string Comment { get; set; }
            public string AppliedBy { get; set; }
            public string WFHStatus { get; set; }
            public List<int> NotifyEmployees { get; set; }
        }

        /// <summary>
        /// Created By Ankit Date- 24/05/2022
        /// </summary>
        public class UpdateWfhRequest
        {
            public int WFHId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string NumberOfDays { get; set; }
            public string Comment { get; set; }
            public string AppliedBy { get; set; }
            public string WFHStatus { get; set; }
            // public List<int> NotifyEmployees { get; set; }
        }

        #endregion
    }
}