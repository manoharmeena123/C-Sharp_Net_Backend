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
    /// <summary>
    /// Created By Harshit Mitra on 05-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/designationnew")]
    public class DesignationController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Add Designation

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// Api >> Post >> api/designationnew/createdesignation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("createdesignation")]
        [HttpPost]
        public async Task<ResponseBodyModel> CreateDesignation(Designation model)
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
                    var designation = await _db.Designation.FirstOrDefaultAsync(x =>
                        x.DesignationName.Trim().ToUpper() == model.DesignationName.Trim().ToUpper() &&
                        x.CompanyId == claims.companyId);
                    if (designation == null)
                    {
                        Designation obj = new Designation();
                        obj.DesignationName = model.DesignationName;
                        obj.Description = model.Description;
                        obj.DepartmentId = model.DepartmentId;
                        obj.CreatedBy = claims.userId;
                        obj.CreatedOn = DateTime.Now;
                        obj.IsActive = true;
                        obj.IsDeleted = false;
                        obj.CompanyId = claims.companyId;
                        obj.OrgId = 0;

                        _db.Designation.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Designation Added !";
                        res.Data = obj;
                    }
                    else
                    {
                        designation.IsActive = true;
                        designation.IsDeleted = false;

                        _db.Entry(designation).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Department Updated";
                        res.Status = true;
                        res.Data = designation;
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

        #endregion Api To Add Designation

        #region API To Get All Active Designation

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// Modify By Harshit Mitra on 05-08-2022
        /// Api >>  Get >> api/designationnew/getallactivedesignation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallactivedesignation")]
        public async Task<ResponseBodyModel> GetAllDesignation()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var designationList = await _db.Designation.Where(x => x.IsDeleted == false && x.IsActive == true &&
                        x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.DesignationId,
                            x.DesignationName,
                            x.Description,
                            x.DepartmentId,
                            DepartmentName = _db.Department.Where(y => y.DepartmentId == x.DepartmentId)
                                             .Select(y => y.DepartmentName).FirstOrDefault(),
                            TotalEmployee = _db.Employee.Where(z => z.DesignationId == x.DesignationId).ToList().Count,
                        }).ToListAsync();

                if (designationList.Count > 0)
                {
                    res.Message = "Designation list Found";
                    res.Status = true;
                    res.Data = designationList.OrderByDescending(x => x.DepartmentId).ToList();
                }
                else
                {
                    res.Status = false;
                    res.Message = "Designation List Is Empty";
                    res.Data = designationList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get All Active Designation

        #region Api To Get Designation By Id

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// API >> Get >> api/designationnew/getdesignationbyid
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdesignationbyid")]
        public async Task<ResponseBodyModel> GetDesignationId(int designationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var DesignationData = await _db.Designation.FirstOrDefaultAsync(x => x.DesignationId == designationId &&
                        x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                if (DesignationData != null)
                {
                    res.Status = true;
                    res.Message = "Designation Found";
                    res.Data = DesignationData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Designation Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Designation By Id

        #region Api To Edit Designation

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// API >> Update >> api/designationnew/editdesignation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editdesignation")]
        public async Task<ResponseBodyModel> UpdateDesignation(Designation model)
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
                    var designation = await _db.Designation.FirstOrDefaultAsync(x => x.DesignationId == model.DesignationId &&
                            x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                    if (designation != null)
                    {
                        var check = await _db.Designation.FirstOrDefaultAsync(x => x.DesignationName == model.DesignationName);
                        if (check != null)
                        {
                            //check.DesignationName = model.DesignationName;
                            check.Description = model.Description;
                            check.DepartmentId = model.DepartmentId;
                            check.UpdatedOn = DateTime.Now;
                            check.UpdatedBy = claims.employeeId;
                            _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                            res.Message = "Designation Already But Update other";
                            res.Status = true;
                            res.Data = check;
                        }
                        designation.DesignationName = model.DesignationName;
                        designation.Description = model.Description;
                        designation.DepartmentId = model.DepartmentId;
                        designation.UpdatedOn = DateTime.Now;
                        designation.UpdatedBy = claims.userId;

                        _db.Entry(designation).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Designation Updated Successfully!";
                        res.Data = designation;
                    }

                    else
                    {
                        res.Status = false;
                        res.Message = "Designation Already Updated !";
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

        #endregion Api To Edit Designation

        #region Api To Delete Designation

        /// <summary>
        /// Modify By Harshit Mitra on 05-04-2022
        /// API >> Delete >> api/designationnew/deletedesignation
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletedesignation")]
        public async Task<ResponseBodyModel> DeleteDesignation(int designationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var designation = await _db.Designation.FirstOrDefaultAsync(x => x.DesignationId ==
                        designationId && !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (designation != null)
                {
                    var checkEmployee = await _db.Employee.CountAsync(x => x.DesignationId == designation.DesignationId &&
                                x.IsActive && !x.IsDeleted && x.EmployeeTypeId == EnumClass.EmployeeTypeConstants.Ex_Employee);
                    if (checkEmployee > 0)
                    {
                        res.Message = "There are Employee In this Desingation You Cannot Delete this Desingation";
                        res.Status = false;
                    }
                    else
                    {
                        designation.DeletedBy = claims.employeeId;
                        designation.DeletedOn = DateTime.Now;
                        designation.IsActive = false;
                        designation.IsDeleted = true;

                        _db.Entry(designation).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Designation Deleted !";
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Designation Not Found Or Its Allready Delated !!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Delete Designation

        #region Api To Get Designation On Behalf of Department By Id

        /// <summary>
        /// Created By Harshit Mitra on 08-04-2022
        /// API >> api/designationnew/designationbydepartment
        /// </summary>
        /// <param name="departmentId"></param>
        [HttpGet]
        [Route("designationbydepartment")]
        public async Task<ResponseBodyModel> GetDesignationByDepartment(int departmentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var designationList = await _db.Designation.Where(x => x.DepartmentId == departmentId &&
                        !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.DesignationId,
                            x.DesignationName,
                        }).ToListAsync();
                if (designationList.Count > 0)
                {
                    res.Message = "Designation List";
                    res.Status = true;
                    res.Data = designationList;
                }
                else
                {
                    res.Message = "No Department Found";
                    res.Status = false;
                    res.Data = designationList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Designation On Behalf of Department By Id

        #region Api To Get Designation On Behalf of Department By Model Class

        /// <summary>
        /// Created By Harshit Mitra on 08-04-2022
        /// API >> api/designationnew/designationbydepartmentId
        /// </summary>
        /// <param name="departmentId"></param>
        [HttpPost]
        [Route("designationbydepartmentId")]
        public async Task<ResponseBodyModel> GetDesignationByDepartment(GetDesignationByDepartmentrequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var designationList = await _db.Designation.Where(x => model.departmentId.Contains(x.DepartmentId) &&
                        !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.DesignationId,
                            x.DesignationName,
                        }).ToListAsync();
                if (designationList.Count > 0)
                {
                    res.Message = "Designation List";
                    res.Status = true;
                    res.Data = designationList;
                }
                else
                {
                    res.Message = "No Department Found";
                    res.Status = false;
                    res.Data = designationList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class GetDesignationByDepartmentrequest
        {
            public List<int> departmentId { get; set; }
        }

        #endregion Api To Get Designation On Behalf of Department

        #region Api To Get Designation By Department List

        /// <summary>
        ///  Created By Harshit Mitra On 28-04-2022
        /// API >> Get >> api/designationnew/getdepartmentbydesignationlist
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getdepartmentbydesignationlist")]
        public async Task<ResponseBodyModel> GetDesignationByDepartmentList(DepartmentListParameter model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var designationList = await _db.Designation.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.DepartmentId,
                            x.DesignationId,
                            x.DesignationName,
                        }).ToListAsync();
                if (model.DepartmentId.Count > 0)
                {
                    designationList = designationList.Where(x => model.DepartmentId.Contains(x.DepartmentId)).ToList();
                }
                else
                {
                    designationList = designationList.ToList();
                }
                if (designationList.Count > 0)
                {
                    res.Message = "Designation List";
                    res.Status = true;
                    res.Data = designationList;
                }
                else
                {
                    res.Message = "No Department Found";
                    res.Status = false;
                    res.Data = designationList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Designation By Department List

        #region This Api Use for Add Designation Data

        /// <summary>
        /// created by Mayank Prajapati on 11/7/2022
        /// Api >> Post >> api/designationnew/designationpost
        /// </summary>
        [HttpPost]
        [Route("designationpost")]
        public async Task<ResponseBodyModel> DesignationPost(List<Designation> Item)
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
                    var allDepartment = await _db.Department.Where(x => x.CompanyId ==
                            claims.companyId && !x.IsDeleted && x.IsActive).ToListAsync();
                    if (allDepartment.Count > 0)
                    {
                        var allDesignation = await _db.Designation.Where(x => x.IsActive &&
                                !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                        foreach (var model in Item)
                        {
                            var designationName = allDesignation.FirstOrDefault(x => x.DesignationName == model.DesignationName);
                            var checkDepartment = allDepartment.FirstOrDefault(x => x.DepartmentName == model.DepartmentName);
                            if (designationName == null && checkDepartment != null)
                            {
                                Designation post = new Designation
                                {
                                    DesignationName = model.DesignationName,
                                    DepartmentId = checkDepartment.DepartmentId,
                                    Description = model.Description,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    IsActive = true,
                                    IsDeleted = false
                                };

                                _db.Designation.Add(post);
                                await _db.SaveChangesAsync();
                                res.Message = "Data Added";
                                res.Status = true;
                                allDesignation.Add(post);
                            }
                            else
                            {
                                res.Message = "Data Added";
                            }
                        }
                    }
                    else
                    {
                        res.Message = "There Are No Department";
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

        #endregion This Api Use for Add Designation Data

        #region Edit Department Data

        /// <summary>
        /// created by Mayank Prajapati on 11/7/2022
        /// Api >> Post >> api/designationnew/designationput
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("designationput")]
        public async Task<ResponseBodyModel> PutDepartment(Designation Model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Add = await _db.Designation.FirstOrDefaultAsync(x => x.DesignationId == Model.DesignationId);
                if (Add != null)
                {
                    Add.DepartmentId = Model.DepartmentId;
                    Add.DesignationName = Model.DesignationName;
                    Add.Description = Model.Description;
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

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra On 28-04-2022
        /// </summary>
        public class DepartmentListParameter
        {
            public List<int> DepartmentId { get; set; }
        }

        #endregion Helper Model Class
    }
}