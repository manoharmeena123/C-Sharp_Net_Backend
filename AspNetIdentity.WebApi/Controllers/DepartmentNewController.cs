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
using System.Web.Routing;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Modify Department Controller By Harshit Mitra on 22-02-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/departmentnew")]
    public class DepartmentNewController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Add Department

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// API >> Post >> api/departmentnew/createdepartment
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("createdepartment")]
        public async Task<ResponseBodyModel> CreateDepartment(Department model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = _db.Department.Where(x =>
                        x.DepartmentName.ToUpper().Trim() == model.DepartmentName.ToUpper().Trim() &&
                        x.CompanyId == claims.companyId).FirstOrDefault();
                if (department == null)
                {
                    Department obj = new Department
                    {
                        DepartmentName = model.DepartmentName.Trim(),
                        Description = model.Description,
                        UsedForLogin = model.UsedForLogin,

                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                        OrgId = 0,
                        IsActive = true,
                        IsDeleted = false
                    };

                    _db.Department.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Department Added!";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    if (department.IsActive && !department.IsDeleted)
                    {
                        res.Message = "Department Allready Exist!";
                        res.Status = false;
                    }
                    else
                    {
                        department.IsActive = true;
                        department.IsDeleted = false;
                        department.UpdatedOn = DateTime.Now;
                        department.UpdatedBy = claims.userId;

                        _db.Entry(department).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "Department Allready Exist!";
                    res.Status = true;
                    res.Data = department;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Department

        #region Api To Get Department By Id

        /// <summary>
        /// API >> Get >> api/
        ///
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("getdepartmentid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetDepartmentId(int departmentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId);
                if (department != null)
                {
                    res.Message = "Department Found";
                    res.Status = true;
                    res.Data = department;
                }
                else
                {
                    res.Message = "No Department Found!!";
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

        #endregion Api To Get Department By Id

        #region API to get all department including department for admin
        /// <summary>
        /// created by Bhavendra Singh Jat on 1/10/2022
        /// Api >> Post >> api/departmentnew/alldepartmentlist
        /// </summary>
        [HttpGet]
        [Route("alldepartmentlist")]
        public async Task<ResponseBodyModel> GetAllDepartment()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await _db.Department.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId)
                    .Select(x => new
                    {
                        x.DepartmentId,
                        x.DepartmentName,
                    }).ToListAsync();
                if (department.Count > 0)
                {
                    res.Data = department;
                    res.Status = true;
                    res.Message = "Department list found";
                }
                else
                {
                    res.Data = department;
                    res.Status = false;
                    res.Message = "Department list not found";
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

        #region API To Get All Active Department List

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// Modify By Harshit Mitra on 05-08-2022
        /// API >> Get >> api/departmentnew/getallactivedeparmentlist
        /// </summary>
        [HttpGet]
        [Route("getallactivedeparmentlist")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ResponseBodyModel> GetAllDepartmentList(int? page = null, int? count = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = _db.Department.Where(x => !x.IsDeleted && x.IsActive &&
                        x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.DepartmentId,
                            x.DepartmentName,
                            x.Description,
                            x.UsedForLogin,
                            TotalEmployee = _db.Employee.Where(z => z.DepartmentId == x.DepartmentId).ToList().Count,
                        }).ToList();
                if (claims.orgId != 0)
                    department = department.Where(x => x.DepartmentName != "Administrator").ToList();

                if (department.Count != 0)
                {
                    res.Message = "Department list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = department.Count,
                            Counts = (int)count,
                            List = department.Skip(((int)page - 1) * (int)count).Take((int)count).ToList().OrderByDescending(x => x.DepartmentId).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = department;
                    }
                }
                else
                {
                    res.Message = "No Department list Found";
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

        #endregion API To Get All Active Department List

        #region Edit Department

        /// <summary>
        /// API >> Put >> api/departmentnew/editdepartment
        /// Modify By Harshit Mitra on 05-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("editdepartment")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateDepartment(Department model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId &&
                        !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (department != null)
                {
                    var check = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentName == model.DepartmentName && x.CompanyId == claims.companyId);
                    if (check != null)
                    {
                        // department.Description = model.Description;
                        department.UsedForLogin = model.UsedForLogin;
                        department.IsDeleted = false;
                        department.IsActive = true;
                        department.UpdatedOn = DateTime.Now;
                        department.UpdatedBy = claims.userId;
                        _db.Entry(department).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Message = "Departmenat Already But update other";
                        res.Status = false;
                        res.Data = department;
                        return res;
                    }
                    department.DepartmentName = model.DepartmentName;
                    department.Description = model.Description;
                    department.UsedForLogin = model.UsedForLogin;
                    department.UpdatedOn = DateTime.Now;
                    department.UpdatedBy = claims.userId;
                    department.IsActive = true;
                    department.IsDeleted = false;

                    _db.Entry(department).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Department Updated";
                    res.Status = true;
                    res.Data = department;
                }
                else
                {
                    res.Message = "Departmenat Already Found";
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

        #endregion Edit Department

        #region Api To Delete Department

        /// <summary>
        /// API >> Delete >> api/departmentnew/deletedepartment
        /// Modify By Harshit Mitra on 05-04-2022
        /// </summary>
        /// <param name="departmentId"></param>
        [HttpPost]
        [Route("deletedepartment")]
        public async Task<ResponseBodyModel> DeleteDepartment(int departmentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentId == departmentId &&
                        !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (department != null)
                {
                    var checkEmployee = await _db.Employee.CountAsync(x => x.DepartmentId == department.DepartmentId &&
                                x.IsActive && !x.IsDeleted && x.EmployeeTypeId == EnumClass.EmployeeTypeConstants.Ex_Employee);
                    if (checkEmployee > 0)
                    {
                        res.Message = "There are Employee In this Department You Cannot Delete this Departent";
                        res.Status = false;
                    }
                    else
                    {
                        department.DeletedBy = claims.employeeId;
                        department.DeletedOn = DateTime.Now;
                        department.IsActive = false;
                        department.IsDeleted = true;

                        _db.Entry(department).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Department Deleted Successfully!";
                        res.Status = true;
                        res.Data = department;
                    }
                }
                else
                {
                    res.Message = "Department Not Found or Allready Deleted";
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

        #endregion Api To Delete Department

        #region This Api Use for Add Department Data

        /// <summary>
        /// created by Mayank Prajapati on 11/7/2022
        /// Api >> Post >> api/departmentnew/departmentpost
        /// </summary>
        [HttpPost]
        [Route("departmentpost")]
        public async Task<ResponseBodyModel> DepartmentPost(List<Department> Item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (Item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    return res;
                }
                else if (Item.Count > 0)
                {
                    var departmentList = await _db.Department.Where(x => x.IsActive &&
                            !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                    foreach (var model in Item)
                    {
                        var data = departmentList.FirstOrDefault(x => x.DepartmentName == model.DepartmentName);
                        if (data == null)
                        {
                            Department post = new Department
                            {
                                DepartmentName = model.DepartmentName.Trim(),
                                Description = model.Description,
                                CompanyId = claims.companyId,
                                OrgId = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false
                            };
                            _db.Department.Add(post);
                            await _db.SaveChangesAsync();
                            res.Message = "Data Added";
                            res.Status = true;
                            departmentList.Add(post);
                        }
                        else if (data.DepartmentName == model.DepartmentName)
                        {
                            res.Message = "Duplicated Data";
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

        #endregion This Api Use for Add Department Data

        #region Edit Department Data

        /// <summary>
        /// created by Mayank Prajapati on 11/7/2022
        /// Api >> Post >> api/departmentnew/departmentput
        /// </summary>
        /// <param name="model"></param>
        ///
        [HttpPut]
        [Route("departmentput")]
        public async Task<ResponseBodyModel> PutDepartment(Department model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Add = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId);
                if (Add != null)
                {
                    Add.DepartmentId = model.DepartmentId;
                    Add.DepartmentName = model.DepartmentName;
                    Add.Description = model.Description;
                    Add.UpdatedBy = claims.employeeId;
                    Add.CompanyId = claims.companyId;
                    Add.OrgId = claims.employeeId;
                    Add.UpdatedOn = DateTime.Now;
                    Add.IsActive = true;
                    Add.IsDeleted = false;

                    _db.Entry(Add).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Data UpDated";
                    res.Status = true;
                    res.Data = Add;
                }
                else
                {
                    res.Message = " Failed To Update";
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

        #endregion Edit Department Data


    }
}