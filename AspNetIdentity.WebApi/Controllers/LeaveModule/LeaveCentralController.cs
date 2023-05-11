using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Leave;
using AspNetIdentity.WebApi.Model.LeaveComponent;
using AspNetIdentity.WebApi.Models;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.LeaveModule
{
    /// <summary>
    /// Created By Harshit Mitra On 14-07-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/leavecentral")]
    public class LeaveCentralController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region All Leave Group APIs

        #region API To Add Leave Group

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/addleavegroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addleavegroup")]
        public async Task<ResponseBodyModel> AddLeaveGroup(LeaveGroup model)
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
                    if (_db.LeaveGroups.Any(x => x.GroupName.ToUpper().Trim() == model.GroupName.ToUpper().Trim() &&
                        x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted))
                    {
                        res.Message = "Leave Group Allready Exist !";
                        res.Status = false;
                    }
                    else
                    {
                        var globalLeaveSetting = await _db.GlobalLeave.FirstOrDefaultAsync(x =>
                                    x.CompanyId == claims.companyId && x.CurrentActive);
                        if (globalLeaveSetting != null)
                        {
                            LeaveGroup obj = new LeaveGroup
                            {
                                GroupName = model.GroupName,
                                Description = model.Description,
                                IsDateSet = false,
                                LeavePolicyStartDate = globalLeaveSetting.StartMonthYear,
                                StartMonth = globalLeaveSetting.StartMonthYear.ToString("MMM yyyy"),
                                LeavePolicyEndingDate = globalLeaveSetting.EndMonthYear,
                                EndMonth = globalLeaveSetting.EndMonthYear.ToString("MMM yyyy"),
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                CompanyId = claims.companyId,
                            };
                            _db.LeaveGroups.Add(obj);
                            await _db.SaveChangesAsync();

                            res.Message = "Leave Group Added";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            res.Message = "You Have To Add Global Leave Time Duration First For All Leave Group";
                            res.Status = false;
                        }
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

        #endregion API To Add Leave Group

        #region API To Get Leave Group List

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Get >> api/leavecentral/getallleavegroup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallleavegroup")]
        public async Task<ResponseBodyModel> GetAllLeaveGroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveGroup = await _db.LeaveGroups.Where(x => x.CompanyId == claims.companyId &&
                            x.IsActive == true && x.IsDeleted == false).ToListAsync();
                leaveGroup
                    .ForEach(x =>
                    {
                        x.CreatedByName = _db.GetEmployeeNameById(x.CreatedBy);
                        x.UpdatedByName = x.UpdatedBy.HasValue ? _db.GetEmployeeNameById((int)x.UpdatedBy) : null;
                        x.DeletedByName = x.DeletedBy.HasValue ? _db.GetEmployeeNameById((int)x.DeletedBy) : null;
                        x.EmployeeCount = _db.Employee.Where(z => z.LeaveGroupId == x.LeaveGroupId).ToList().Count;
                    });

                if (leaveGroup.Count > 0)
                {
                    res.Message = "Leave Group found";
                    res.Status = true;
                    res.Data = leaveGroup;
                }
                else
                {
                    res.Message = "No Leave Group Created";
                    res.Status = false;
                    res.Data = leaveGroup;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Group List

        #region API To Get Leave Group By Id

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/getleavegroupbyid
        /// </summary>
        /// <param name="leaveGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getleavegroupbyid")]
        public async Task<ResponseBodyModel> GetLeaveGroup(int leaveGroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveGroup = await _db.LeaveGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                            x.IsDeleted == false && x.LeaveGroupId == leaveGroupId).FirstOrDefaultAsync();
                if (leaveGroup != null)
                {
                    res.Message = "Leave Group Found";
                    res.Status = true;
                    res.Data = leaveGroup;
                }
                else
                {
                    res.Message = "No Leave Group Found";
                    res.Status = false;
                    res.Data = leaveGroup;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Group By Id

        #region API To Edit Leave Group

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/editleavegroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editleavegroup")]
        public async Task<ResponseBodyModel> EditLeaveGroup(LeaveGroup model)
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
                    var leaveGroup = await _db.LeaveGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                                x.IsDeleted == false && x.LeaveGroupId == model.LeaveGroupId).FirstOrDefaultAsync();
                    if (leaveGroup != null)
                    {
                        leaveGroup.GroupName = model.GroupName;
                        leaveGroup.Description = model.Description;
                        leaveGroup.UpdatedBy = claims.employeeId;
                        leaveGroup.UpdatedOn = DateTime.Now;

                        _db.Entry(leaveGroup).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Leave Group Updated";
                        res.Status = true;
                        res.Data = leaveGroup;
                    }
                    else
                    {
                        res.Message = "No Leave Group Created";
                        res.Status = false;
                        res.Data = leaveGroup;
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

        #endregion API To Edit Leave Group

        #region API To Delete Leave Group

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/deleteleavegroup
        /// </summary>
        /// <param name="leaveGroupId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteleavegroup")]
        public async Task<ResponseBodyModel> EditLeaveGroup(int leaveGroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveGroup = await _db.LeaveGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                            x.IsDeleted == false && x.LeaveGroupId == leaveGroupId).FirstOrDefaultAsync();
                if (leaveGroup != null)
                {
                    leaveGroup.IsActive = false;
                    leaveGroup.IsDeleted = true;
                    leaveGroup.DeletedBy = claims.employeeId;
                    leaveGroup.DeletedOn = DateTime.Now;

                    _db.Entry(leaveGroup).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var emplist = await _db.Employee.Where(x => x.LeaveGroupId == leaveGroup.LeaveGroupId).ToListAsync();
                    foreach (var emp in emplist)
                    {
                        emp.LeaveGroupId = 0;
                        _db.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "Leave Group Deleted";
                    res.Status = true;
                    res.Data = leaveGroup;
                }
                else
                {
                    res.Message = "No Leave Group Found";
                    res.Status = false;
                    res.Data = leaveGroup;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Delete Leave Group

        #endregion

        #region ALl Leave Type API's

        #region API To Add Leave Type

        /// <summary>
        /// Created By Harshit Mitra On 14-07-2022
        /// API >> Post >> api/leavecentral/addleavetype
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addleavetype")]
        public async Task<ResponseBodyModel> AddLeaveType(LeaveType model)
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
                    LeaveType obj = new LeaveType
                    {
                        LeaveTypeName = model.LeaveTypeName,
                        Description = model.Description,
                        IsPaidLeave = model.IsPaidLeave,
                        RestrictToG = model.RestrictToG,
                        Gender = model.Gender,
                        RestrictToS = model.RestrictToS,
                        Status = model.Status,
                        IsReasonRequired = model.IsReasonRequired,
                        IsDelatable = true,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                    };
                    _db.LeaveTypes.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Type Created";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
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

        #endregion ALl Leave Type API's

        #region API To Get Leave Component On Select Leave Group

        /// <summary>
        /// Created By Harshit Mitra On 15-07-2022
        /// API >> Get >> api/leavecentral/getleavecomponent
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getleavecomponent")]
        public async Task<ResponseBodyModel> GetLeaveComponent(int leaveGroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var leaveComponents = new List<LeaveComponent>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                LeaveHelper.CheckLeave(claims.companyId);
                var checkComponents = await _db.LeaveComponents.Where(x => x.CompanyId == claims.companyId && x.LeaveGroupId == leaveGroupId).ToListAsync();
                if (checkComponents.Count > 0)
                {
                    var check = checkComponents.Select(x => x.LeaveTypeId).ToList();
                    var leaveType = await _db.LeaveTypes.Where(x => x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
                    leaveComponents = leaveType
                                .Select(x => new LeaveComponent
                                {
                                    ComponentId = 0,
                                    LeaveTypeId = x.LeaveTypeId,
                                    LeaveTypeName = x.LeaveTypeName,
                                    Quota = "Not Set",
                                    IsCompleted = false,
                                    Description = x.Description,
                                    IsCheck = check.Contains(x.LeaveTypeId),
                                    IsDefault = (x.LeaveTypeName == "Paid Leave" || x.LeaveTypeName == "Sick Leave" || x.LeaveTypeName == "Un-Paid Leave")
                                }).ToList();
                }
                else
                {
                    var leaveType = await _db.LeaveTypes.Where(x => x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
                    leaveComponents = leaveType
                                .Select(x => new LeaveComponent
                                {
                                    ComponentId = 0,
                                    LeaveTypeId = x.LeaveTypeId,
                                    LeaveTypeName = x.LeaveTypeName,
                                    Quota = "Not Set",
                                    IsCompleted = false,
                                    Description = x.Description,
                                    IsCheck = (x.LeaveTypeName == "Paid Leave" || x.LeaveTypeName == "Sick Leave" || x.LeaveTypeName == "Un-Paid Leave"),
                                    IsDefault = (x.LeaveTypeName == "Paid Leave" || x.LeaveTypeName == "Sick Leave" || x.LeaveTypeName == "Un-Paid Leave")
                                }).ToList();
                }
                res.Message = "Leave Components";
                res.Status = true;
                res.Data = leaveComponents;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Component On Select Leave Group

        #region API To Add And Update Leave Components In Leave Group

        /// <summary>
        /// Created By Harshit Mitra on 19-07-2022
        /// API >> Post >> api/leavecentral/addupdateleavecomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdateleavecomponent")]
        public async Task<ResponseBodyModel> AddUpdateLeaveComponent(AddUpdateLeaveComponentsModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model.ComponentList != null)
                {
                    if (model.ComponentList.Count > 0)
                    {
                        model.ComponentList = model.ComponentList.Where(x => x.IsCheck == true).ToList();
                        var leaveGroup = await _db.LeaveGroups.FirstOrDefaultAsync(x => x.LeaveGroupId == model.LeaveGroupId &&
                                    x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false);
                        var checkComponents = await _db.LeaveComponents.Where(x => x.CompanyId == claims.companyId && x.LeaveGroupId == leaveGroup.LeaveGroupId).ToListAsync();
                        if (checkComponents.Count > 0)
                        {
                            var checkIds = checkComponents.Select(x => x.LeaveTypeId).ToList();
                            var newComponent = model.ComponentList.Where(x => x.IsCheck == true).Select(x => x.LeaveTypeId).ToList();
                            var removeData = checkIds.Where(x => !newComponent.Contains(x)).ToList();
                            var addingData = newComponent.Where(x => !checkIds.Contains(x)).ToList();
                            var addComponentList = model.ComponentList.Where(x => addingData.Contains(x.LeaveTypeId) && x.IsCheck == true).ToList();
                            var removeComponentList = checkComponents.Where(x => removeData.Contains(x.LeaveTypeId)).ToList();
                            foreach (var component in removeComponentList)
                            {
                                _db.Entry(component).State = EntityState.Deleted;
                                await _db.SaveChangesAsync();
                            }
                            foreach (var item in addComponentList)
                            {
                                LeaveComponent obj = new LeaveComponent
                                {
                                    LeaveGroupId = leaveGroup.LeaveGroupId,
                                    LeaveTypeId = item.LeaveTypeId,
                                    LeaveTypeName = item.LeaveTypeName,
                                    Quota = "Not Set",
                                    IsCompleted = false,

                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    IsActive = true,
                                    IsDeleted = false,
                                };
                                _db.LeaveComponents.Add(obj);
                                await _db.SaveChangesAsync();
                            }
                            res.Message = "leave Component Updated";
                            res.Status = true;
                            res.Data = model;
                        }
                        else
                        {
                            foreach (var item in model.ComponentList)
                            {
                                LeaveComponent obj = new LeaveComponent
                                {
                                    LeaveGroupId = leaveGroup.LeaveGroupId,
                                    LeaveTypeId = item.LeaveTypeId,
                                    LeaveTypeName = item.LeaveTypeName,
                                    Quota = "Not Set",
                                    IsCompleted = false,

                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    IsActive = true,
                                    IsDeleted = false,
                                };
                                _db.LeaveComponents.Add(obj);
                                await _db.SaveChangesAsync();
                            }
                            res.Message = "leave Component Added";
                            res.Status = true;
                            res.Data = model;
                        }
                    }
                    else
                    {
                        res.Message = "No Leave Type Selected";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Model Is Invalid";
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

        /// <summary>
        /// Created By Harshit Mitra on 19-07-2022
        /// </summary>
        public class AddUpdateLeaveComponentsModel
        {
            public int LeaveGroupId { get; set; }
            public List<LeaveComponent> ComponentList { get; set; }
        }

        #endregion API To Add And Update Leave Components In Leave Group

        #region API To Delete Leave Component From Leave Group

        /// <summary>
        /// Created By Harshit Mitra on 19-07-2022
        /// API >> Post >> api/leavecentral/deletecomponent
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecomponent")]
        public async Task<ResponseBodyModel> DeleteComponent(int componentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var component = await _db.LeaveComponents.FirstOrDefaultAsync(x => x.ComponentId == componentId && x.CompanyId == claims.companyId);
                if (component != null)
                {
                    _db.Entry(component).State = EntityState.Deleted;
                    await _db.SaveChangesAsync();

                    res.Message = "Component Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Component Not Found";
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

        #endregion API To Delete Leave Component From Leave Group

        #region API To Get Leave Component On Leave Group Page

        /// <summary>
        /// Created By Harshit Mitra on 20-07-2022
        /// Modified By Harshit Mitra on 04-08-2022
        /// API >> Post >> api/leavecentral/getleavecomponentbyleavegroup
        /// </summary>
        /// <param name="leaveGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getleavecomponentbyleavegroup")]
        public async Task<ResponseBodyModel> GetLeaveComponentByLeaveGroup(int leaveGroupId, int page, int count)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leaveComponent = await _db.LeaveComponents.Where(x => x.CompanyId == claims.companyId &&
                            x.LeaveGroupId == leaveGroupId && x.IsActive && !x.IsDeleted).ToListAsync();
                if (leaveComponent.Count > 0)
                {
                    leaveComponent.ForEach(x => x.IsDelatable = !(x.LeaveTypeName == "Paid Leave" || x.LeaveTypeName == "Sick Leave" || x.LeaveTypeName == "Un-Paid Leave"));
                    res.Message = "Component List";
                    res.Status = true;
                    res.Data = new
                    {
                        IsAllComponentCompleted = leaveComponent.All(x => x.IsCompleted),
                        LeaveComponent = new PaginationData
                        {
                            TotalData = leaveComponent.Count,
                            Counts = count,
                            List = leaveComponent.Skip((page - 1) * count).Take(count).ToList(),
                        },
                    };
                }
                else
                {
                    res.Message = "No Component Added";
                    res.Status = false;
                    res.Data = leaveComponent;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Leave Component On Leave Group Page

        #region API To Get Component Data By Id

        /// <summary>
        /// Created By Harshit Mitra On 21-07-2022
        /// API >> Put >> api/leavecentral/getcomponentbyid
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcomponentbyid")]
        public async Task<ResponseBodyModel> GetComponentById(int componentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var component = await _db.LeaveComponents.FirstOrDefaultAsync(x => x.ComponentId == componentId && x.CompanyId == claims.companyId);
                if (component != null)
                {
                    res.Message = "Component Found";
                    res.Status = false;
                    res.Data = component;
                }
                else
                {
                    res.Message = "Component Not Found";
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

        #endregion API To Get Component Data By Id

        #region API To Update Leave Component Setting

        /// <summary>
        /// Created By Harshit Mitra On 21-07-2022
        /// API >> Put >> api/leavecentral/updateleavecomponentsetting
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updateleavecomponentsetting")]
        public async Task<ResponseBodyModel> UpdateLeaveComponentSetting(LeaveComponent model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var leavecomponent = await _db.LeaveComponents.FirstOrDefaultAsync(x =>
                        x.IsDeleted == false && x.IsActive == true && x.ComponentId == model.ComponentId);
                if (leavecomponent != null)
                {
                    leavecomponent.IsQuotaLimit = model.IsQuotaLimit;
                    leavecomponent.QuotaCount = !model.IsQuotaLimit ? 0 : model.QuotaCount;
                    leavecomponent.Quota = !model.IsQuotaLimit ? "Un-Limited" : model.QuotaCount == 1 ? model.QuotaCount.ToString() + " day" : model.QuotaCount.ToString() + " days";
                    leavecomponent.IsCompleted = true;

                    leavecomponent.UpdatedOn = DateTime.Now;
                    leavecomponent.UpdatedBy = claims.companyId;

                    _db.Entry(leavecomponent).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Component Setting Saved";
                    res.Status = true;
                    res.Data = leavecomponent;
                }
                else
                {
                    res.Message = "Leave Component Not Found";
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

        #endregion API To Update Leave Component Setting

        #region API To Get Employee List on Leave Group Assignment

        /// <summary>
        /// Created By Harshit Mitra On 21-07-2022
        /// Modified By Harshit Mitra On 04-08-2022
        /// API >> Get >> api/leavecentral/employeelistfilteronleave
        /// </summary>
        /// <param name="leaveGroupId"></param>
        /// <param name="orgId"></param>
        /// <param name="departmentId"></param>
        /// <param name="designationId"></param>
        /// <param name="lowerValue"></param>
        /// <param name="range"></param>
        /// <param name="higherValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("employeelistfilteronleave")]
        public async Task<ResponseBodyModel> GetEmployeeListOnLeave
            (int? page = null,
            int? count = null,
            int? leaveGroupId = null,
            string search = null,
            int? orgId = null, int? departmentId = null,
            int? designationId = null, int? lowerValue = null,
            RangedEnumOnLeave? range = null, int? higherValue = null,
            string gender = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (leaveGroupId.HasValue)
                {
                    var leaveComponent = await _db.LeaveComponents.Where(x => x.CompanyId == claims.companyId &&
                            x.LeaveGroupId == leaveGroupId && x.IsActive && !x.IsDeleted).ToListAsync();
                    if (leaveComponent.Any(x => !x.IsCompleted))
                    {
                        res.Message = "Component Quta in not set";
                        res.Status = false;
                        res.Data = new List<int>();
                        return res;
                    }
                }
                var empList = await (from e in _db.Employee
                                     join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                     join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                     join o in _db.OrgMaster on e.OrgId equals o.OrgId into r
                                     from empty in r.DefaultIfEmpty()
                                     join l in _db.LeaveGroups on e.LeaveGroupId equals l.LeaveGroupId into q
                                     from result in q.DefaultIfEmpty()
                                     where e.IsActive && e.CompanyId == claims.companyId &&
                                     !e.IsDeleted && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                     select new EmpListOnLeave
                                     {
                                         EmployeeId = e.EmployeeId,
                                         DisplayName = e.DisplayName,
                                         OfficeEmail = e.OfficeEmail,
                                         DepartmentId = d.DepartmentId,
                                         DepartmentName = d.DepartmentName,
                                         DesignationId = ds.DesignationId,
                                         DesignationName = ds.DesignationName,
                                         OrgId = empty == null ? 0 : empty.OrgId,
                                         OrgName = empty.OrgName ?? "Company Head",
                                         JoiningDate = e.JoiningDate,
                                         LeaveGroupStatus = e.LeaveGroupId != 0 ? "Assigned" : "Un-Assigned",
                                         LeaveGroupName = result.GroupName ?? "Un-Assigned",
                                         LeaveGroupId = e.LeaveGroupId,
                                         Gender = e.Gender,
                                         CreateOn = e.CreatedOn,
                                         UpdateOn = e.UpdatedOn,
                                         //IsCheck = leaveGroupId.HasValue && e.LeaveGroupId == (int)leaveGroupId,
                                     }).ToListAsync();

                empList.ForEach(x => x.MonthTillJoining = ((DateTime.Now.Year - x.JoiningDate.Year) * 12) + DateTime.Now.Month - x.JoiningDate.Month);

                var predicate = PredicateBuilder.New<EmpListOnLeave>(x => x.EmployeeId != 0);
                if (leaveGroupId.HasValue)
                {
                    empList.ForEach(x => x.IsCheck = leaveGroupId.HasValue ? x.LeaveGroupId == (int)leaveGroupId : false);
                    predicate.And(x => x.LeaveGroupId == (int)leaveGroupId || x.LeaveGroupId == 0);
                }
                if (orgId.HasValue)
                    predicate.And(x => x.OrgId == (int)orgId);
                if (departmentId.HasValue)
                    predicate.And(x => x.DepartmentId == (int)departmentId);
                if (designationId.HasValue)
                    predicate.And(x => x.DesignationId == (int)designationId);
                if (!String.IsNullOrEmpty(gender))
                    predicate.And(x => x.Gender.ToUpper().Trim() == gender.ToUpper().Trim());
                if (range.HasValue)
                {
                    switch ((RangedEnumOnLeave)range)
                    {
                        case RangedEnumOnLeave.IsGreterThan:
                            if (lowerValue.HasValue)
                                predicate.And(x => x.MonthTillJoining >= (int)lowerValue);
                            break;

                        case RangedEnumOnLeave.IsLessThan:
                            if (higherValue.HasValue)
                                predicate.And(x => x.MonthTillJoining <= (int)higherValue);
                            break;

                        case RangedEnumOnLeave.IsBetween:
                            if (lowerValue.HasValue && higherValue.HasValue)
                                predicate.And(x => x.MonthTillJoining >= (int)lowerValue && x.MonthTillJoining <= (int)higherValue);
                            break;
                    }
                }

                var list = empList.Where(predicate.Compile()).ToList();

                if (empList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    if (leaveGroupId.HasValue)
                    {
                        empList = empList.OrderBy(x => x.IsCheck).ToList();
                        list = list.OrderByDescending(x => x.IsCheck).ToList();
                        if (page.HasValue && count.HasValue && !String.IsNullOrEmpty(search))
                        {
                            res.Data = new
                            {
                                EmployeeIds = empList.Where(x => x.LeaveGroupId == (int)leaveGroupId).Select(x => x.EmployeeId).ToList(),
                                List = new PaginationData
                                {
                                    TotalData = list.Count,
                                    Counts = (int)count,
                                    List = list.Where(x => x.OfficeEmail.Contains(search) || x.DisplayName.Contains(search))
                                    .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                                },
                            };
                        }
                        else if (page.HasValue && count.HasValue && String.IsNullOrEmpty(search))
                        {
                            res.Data = new
                            {
                                EmployeeIds = empList.Where(x => x.LeaveGroupId == (int)leaveGroupId).Select(x => x.EmployeeId).ToList(),
                                List = new PaginationData
                                {
                                    TotalData = list.Count,
                                    Counts = (int)count,
                                    List = list.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                                },
                            };
                        }
                        else if (!page.HasValue && !count.HasValue && !String.IsNullOrEmpty(search))
                        {
                            res.Data = new
                            {
                                EmployeeIds = empList.Where(x => x.LeaveGroupId == (int)leaveGroupId).Select(x => x.EmployeeId).ToList(),
                                List = new PaginationData
                                {
                                    TotalData = list.Count,
                                    Counts = (int)count,
                                    List = list.Where(x => x.OfficeEmail.Contains(search) || x.DisplayName.Contains(search)).ToList(),
                                },
                            };
                        }
                        else
                        {
                            res.Data = new
                            {
                                EmployeeIds = empList.Where(x => x.LeaveGroupId == (int)leaveGroupId).Select(x => x.EmployeeId).ToList(),
                                List = list,
                            };
                        }
                    }
                    else
                    {
                        res.Data = new
                        {
                            EmployeeIds = new List<int>(),
                            List = new PaginationData
                            {
                                TotalData = list.Count,
                                Counts = (int)count,
                                List = list.Where(x => x.LeaveGroupId != 0).OrderBy(x => x.LeaveGroupName).ThenBy(x => x.DisplayName)
                                      .Concat(list.Where(x => x.LeaveGroupId == 0).OrderBy(x => x.DisplayName))
                                      .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            },
                        };
                    }
                }
                else
                {
                    res.Message = "Emploee List Is Empty";
                    res.Status = false;
                    res.Data = empList;
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
        /// Created By Harshit Mitra on 21-07-2022
        /// </summary>
        public class EmpListOnLeave
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public int DesignationId { get; set; }
            public string DesignationName { get; set; }
            public string OfficeEmail { get; set; }
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTimeOffset JoiningDate { get; set; }
            public int MonthTillJoining { get; set; }
            public string LeaveGroupStatus { get; set; }
            public int LeaveGroupId { get; set; }
            public string LeaveGroupName { get; set; }
            public bool IsCheck { get; set; }
            public string Gender { get; set; }
            public DateTimeOffset CreateOn { get; set; }
            public DateTimeOffset? UpdateOn { get; set; }
        }

        #endregion API To Get Employee List on Leave Group Assignment

        #region API To Add Update Employees Leave Group

        /// <summary>
        /// Created By Harshit Mitra On 22-07-2022
        /// API >> Get >> api/leavecentral/addupdateemployeeleavegroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdateemployeeleavegroup")]
        public async Task<ResponseBodyModel> AddUpdateEmployeeLeaveGroup(AddUpdateEmpLeaveGroupHelperModel model)
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
                    var leaveGroup = await _db.LeaveGroups.FirstOrDefaultAsync(x => x.LeaveGroupId == model.LeaveGroupId
                                && x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false);
                    if (leaveGroup != null)
                    {
                        if (model.EmployeeIds != null)
                        {
                            if (model.EmployeeIds.Count > 0)
                            {
                                var emplist = await _db.Employee.Where(x => x.LeaveGroupId == model.LeaveGroupId ||
                                            model.EmployeeIds.Contains(x.EmployeeId)).ToListAsync();
                                foreach (var item in emplist)
                                {
                                    item.UpdatedBy = claims.employeeId;
                                    item.UpdatedOn = DateTime.Now;

                                    item.LeaveGroupId = model.EmployeeIds.Contains(item.EmployeeId) ? leaveGroup.LeaveGroupId : 0;

                                    _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                    await _db.SaveChangesAsync();
                                }
                                res.Message = "Leave Group Assign";
                                res.Status = true;
                            }
                            else
                            {
                                res.Message = "No Employee Select";
                                res.Status = false;
                            }
                        }
                        else
                        {
                            res.Message = "No Employee Select";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "Leave Group Not Found";
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
        /// Created By Harshit Mitra on 22-07-2022
        /// </summary>
        public class AddUpdateEmpLeaveGroupHelperModel
        {
            public int LeaveGroupId { get; set; }
            public List<int> EmployeeIds { get; set; }
        }

        #endregion API To Add Update Employees Leave Group

        #region API To Remove Employee From Leave Group

        /// <summary>
        /// Created By Harshit Mitra On 22-07-2022
        /// API >> Get >> api/leavecentral/removeleavegroup
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("removeleavegroup")]
        public async Task<ResponseBodyModel> RemoveLeaveGroup(int employeeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                if (employee != null)
                {
                    employee.LeaveGroupId = 0;
                    employee.UpdatedBy = claims.employeeId;
                    employee.UpdatedOn = DateTime.Now;

                    _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Leave Group Un-Assigned";
                    res.Status = true;
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

        #endregion API To Remove Employee From Leave Group

        #region API To Get Leave Year Duration Period

        /// <summary>
        /// Created By Harshit Mitra On 23-07-2022
        /// API >> Get >> api/leavecentral/leaveyearduration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leaveyearduration")]
        public ResponseBodyModel GetLeaveYearDuration()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                string[] durations = { "3 Months", "6 Months", "12 Months", "18 Months", "24 Months" };
                res.Message = "Leave Durations";
                res.Status = true;
                res.Data = new List<DurationHelperModel>()
                {
                    new DurationHelperModel(){  Name="3 Months"},
                    new DurationHelperModel(){  Name="6 Months"},
                    new DurationHelperModel(){  Name="12 Months"},
                    new DurationHelperModel(){  Name="18 Months"},
                    new DurationHelperModel(){  Name="24 Months"}
                };
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
        public class DurationHelperModel
        {
            public string Name { get; set; }
        }

        #endregion API To Get Leave Year Duration Period

        #region API To Set Global Leave Time Duration

        /// <summary>
        /// Created By Harshit Mitra On 23-07-2022
        /// API >> Get >> api/leavecentral/setleaveglobalduration
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setleaveglobalduration")]
        public async Task<ResponseBodyModel> SetLeaveGlobalDuration(SetLeaveDurationModel model)
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
                    var add = Convert.ToInt32(model.DurationProperty.Replace(" Months", "").Trim());
                    var leaveGlobal = await _db.GlobalLeave.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                    if (leaveGlobal.Count == 0)
                    {
                        GlobalLeaveYearHistory obj = new GlobalLeaveYearHistory
                        {
                            StartMonthYear = model.DateProperty.Date,
                            Duration = model.DurationProperty,
                            EndMonthYear = model.DateProperty.AddMonths(add),
                            CompanyId = claims.companyId,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            CurrentActive = true,
                        };
                        _db.GlobalLeave.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Date Updated";
                        res.Status = true;
                    }
                    else
                    {
                        //leaveGlobal.ForEach(x => x.CurrentActive = false);
                        //foreach (var item in leaveGlobal)
                        //{
                        //    _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        //    await _db.SaveChangesAsync();
                        //}
                        //GlobalLeaveYearHistory obj = new GlobalLeaveYearHistory
                        //{
                        //    StartMonthYear = model.DateProperty.Date,
                        //    Duration = model.DurationProperty,
                        //    EndMonthYear = model.DateProperty.AddMonths(add),
                        //    CompanyId = claims.companyid,
                        //    IsActive = true,
                        //    IsDeleted = false,
                        //    CreatedBy = claims.employeeid,
                        //    CreatedOn = DateTime.Now,
                        //    CurrentActive = true,
                        //};
                        //_db.GlobalLeave.Add(obj);
                        //await _db.SaveChangesAsync();

                        //res.Message = "Date Updated";
                        //res.Status = true;

                        res.Message = "Global Leave Date Duration Allready Set";
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
        /// Created By Harshit Mitra on 23-07-2022
        /// </summary>
        public class SetLeaveDurationModel
        {
            public DateTime DateProperty { get; set; }
            public string DurationProperty { get; set; }
        }

        #endregion API To Set Global Leave Time Duration

        #region API To Check Global Date Save OR Not

        /// <summary>
        /// Created By Harshit Mitra On 23-07-2022
        /// API >> Get >> api/leavecentral/checkglobaldate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("checkglobaldate")]
        public async Task<ResponseBodyModel> CheckGlobalDateSaveOrNot()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var check = await _db.GlobalLeave.AnyAsync(x => x.CompanyId == claims.companyId && x.CurrentActive == true);
                res.Message = "Check";
                res.Status = check;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Check Global Date Save OR Not
    }
}