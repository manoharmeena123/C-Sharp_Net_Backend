//using AspNetIdentity.WebApi.Helper;
//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Model;
//using AspNetIdentity.WebApi.Models;
//using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web.Http;
//using static AspNetIdentity.WebApi.Model.EnumClass;

//namespace AspNetIdentity.WebApi.Controllers
//{
//    /// <summary>
//    /// Created By Harshit Mitra on 18-02-2022
//    /// </summary>
//    [Authorize]
//    [RoutePrefix("api/paygroups")]
//    public class PayGroupController : ApiController
//    {
//        private readonly ApplicationDbContext _db = new ApplicationDbContext();

//        #region Api To Add Pay Groups On Payroll

//        /// <summary>
//        /// API >> Post >> api/paygroups/addpaygroup
//        /// Created By Harshit Mitra on 18-02-2022
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addpaygroup")]
//        public async Task<ResponseBodyModel> AddPayGroup(PayGroup model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model is Unvalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    PayGroup obj = new PayGroup
//                    {
//                        PayGroupName = model.PayGroupName,
//                        Description = model.Description,
//                        CreatedBy = claims.employeeId,
//                        CreatedOn = DateTime.Now,
//                        IsActive = true,
//                        IsDeleted = false,
//                        CompanyId = claims.companyId,
//                        OrgId = claims.orgId,
//                    };
//                    _db.PayGroups.Add(obj);
//                    await _db.SaveChangesAsync();

//                    ///Example///
//                    var paySetup = Enum.GetValues(typeof(PayrollSetupConstants))
//                                .Cast<PayrollSetupConstants>()
//                                .Select(x => new
//                                {
//                                    Step = (int)x,
//                                    Title = Enum.GetName(typeof(PayrollSetupConstants), x),
//                                }).ToList();

//                    foreach (var item in paySetup)
//                    {
//                        PayRollSetup newObj = new PayRollSetup
//                        {
//                            PayGroupId = obj.PayGroupId,
//                            Title = item.Title,
//                            Step = item.Step,
//                            Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.PENDING),
//                            IsActive = true,
//                            IsDeleted = false,
//                            CreatedBy = claims.userId,
//                            CreatedOn = DateTime.Now,
//                            CompanyId = claims.companyId,
//                            OrgId = claims.orgId,
//                        };
//                        _db.PayRollSetups.Add(newObj);
//                        await _db.SaveChangesAsync();
//                    }

//                    res.Message = "Pay Group Added";
//                    res.Status = true;
//                    res.Data = obj;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Pay Groups On Payroll

//        #region Api To Get All Pay Groups

//        /// <summary>
//        /// API >> Get >> api/paygroups/getallpaygroup
//        /// Created By Harshit Mitra on 18-02-2022
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getallpaygroup")]
//        public async Task<ResponseBodyModel> GetAllPayGroups()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.Where(x => x.CompanyId == claims.companyId).ToListAsync();
//                if (payGroup.Count > 0)
//                {
//                    res.Message = "Get All Pay Groups";
//                    res.Status = true;
//                    res.Data = payGroup;
//                }
//                else
//                {
//                    res.Message = "List Is Empty";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Pay Groups

//        #region Api To Get All Active Pay Groups With Employee Count

//        /// <summary>
//        /// Created By Harshit Mitra on 18-02-2022
//        /// API >> Get >> api/paygroups/getallactivepaygroup
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getallactivepaygroup")]
//        public async Task<ResponseBodyModel> GetAllActivePayGroups()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.Where(x => x.CompanyId == claims.companyId)
//                    .Select(x => new ActivePayGroupModel
//                    {
//                        PayGroupId = x.PayGroupId,
//                        PayGroupName = x.PayGroupName,
//                        Description = x.Description,
//                        EmployeeCount = _db.Employee.Where(z => z.PayGroupId == x.PayGroupId).ToList().Count,
//                    }).ToListAsync();

//                if (payGroup.Count > 0)
//                {
//                    res.Message = "Get All Pay Groups";
//                    res.Status = true;
//                    res.Data = payGroup;
//                }
//                else
//                {
//                    res.Message = "List Is Empty";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Active Pay Groups With Employee Count

//        #region Api To Get Pay Groups By Id

//        /// <summary>
//        /// Created By Harshit Mitra on 19-02-2022
//        /// API >> Get >> api/paygroups/paygroupbyid
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("paygroupbyid")]
//        public async Task<ResponseBodyModel> GetPayGroupById(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    res.Message = "Pay Group Found";
//                    res.Status = true;
//                    res.Data = payGroup;
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = true;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pay Groups By Id

//        #region Api To Edit Pay Group

//        /// <summary>
//        /// Created By Harshit Mitra on 19-02-2022
//        /// API >> Put >> api/paygroups/editpaygroup
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("editpaygroup")]
//        public async Task<ResponseBodyModel> EditPayGroup(PayGroup model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
//                if (payGroup != null)
//                {
//                    payGroup.PayGroupName = model.PayGroupName;
//                    payGroup.Description = model.Description;
//                    payGroup.UpdatedBy = claims.employeeId;
//                    payGroup.UpdatedOn = DateTime.Now;
//                    _db.Entry(payGroup).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Pay Group Updated";
//                    res.Status = true;
//                    res.Data = payGroup;
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Edit Pay Group

//        #region Api To Delete Pay Group(Soft Delete)

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpPut]
//        public async Task<ResponseBodyModel> DeletePayGroup(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    payGroup.DeletedBy = claims.employeeId;
//                    payGroup.DeletedOn = DateTime.Now;
//                    payGroup.IsDeleted = true;
//                    payGroup.IsActive = false;
//                    _db.Entry(payGroup).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Pay Group Updated";
//                    res.Status = true;
//                    res.Data = payGroup;
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Delete Pay Group(Soft Delete)

//        #region Api To Get Pay Group Whose Structure And Setup Is Completed

//        /// <summary>
//        /// Created By Harshit Mitra on 17-06-2022
//        /// API >> Get >> api/paygroups/getcompletedpaygroup
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getcompletedpaygroup")]
//        public async Task<ResponseBodyModel> GetCmpletedPayGroup()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.Where(x =>
//                        x.CompanyId == claims.companyId && x.IsCompleted == true).ToListAsync();
//                foreach (var item in payGroup)
//                {
//                    var emp = _db.Employee.Where(x => x.PayGroupId == item.PayGroupId).ToList();
//                    item.EmployeeInPayGroup = emp.Count;
//                    var dep = emp.Select(z => z.DepartmentId).Distinct().ToList();
//                    item.DepartmentInPayGroup = _db.Department.Where(x => dep.Contains(x.DepartmentId)).ToList().Count;
//                    var totalEmp = _db.Employee.Where(x => x.IsActive == true && x.CompanyId == claims.companyId && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).ToList().Count;
//                    item.TotalEmployee = totalEmp;
//                    var totalDep = _db.Department.Where(x => x.IsActive == true && x.CompanyId == claims.companyId && x.DepartmentName != "Administrator").ToList().Count;
//                    item.TotalDepartment = totalDep;
//                }
//                if (payGroup.Count > 0)
//                {
//                    res.Message = "Get All Pay Groups";
//                    res.Status = true;
//                    res.Data = payGroup;
//                }
//                else
//                {
//                    res.Message = "List Is Empty";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pay Group Whose Structure And Setup Is Completed

//        #region Helper Model Class

//        /// <summary>
//        /// Created By Harshit Mitra on 20-02-2022
//        /// </summary>
//        public class ActivePayGroupModel
//        {
//            public int PayGroupId { get; set; }
//            public string PayGroupName { get; set; }
//            public string Description { get; set; }
//            public int EmployeeCount { get; set; }
//        }

//        #endregion Helper Model Class
//    }
//}