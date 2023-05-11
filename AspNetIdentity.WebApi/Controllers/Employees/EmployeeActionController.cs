using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.EmployeeModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Employees
{
    [RoutePrefix("api/employeectioncontroller")]
    [Authorize]
    public class EmployeeActionController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region

        /// <summary>
        /// created by Mayank Prajapati on 08/08/2022
        /// Api >> Post >> api/employeectioncontroller/postresigon
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("postresigon")]
        public async Task<ResponseBodyModel> PostResigon(ResignationResigon model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                if (model == null)
                {
                    res.Message = "Resigon is Unvalid";
                    res.Status = false;
                }
                else
                {
                    ResignationResigon post = new ResignationResigon
                    {
                        Name = model.Name,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    db.ResignationResigons.Add(post);
                    await db.SaveChangesAsync();
                    res.Message = "Resigon Added";
                    res.Status = true;
                    res.Data = post;
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

        #region GetResigon

        [HttpGet]
        [Route("getresigon")]
        public async Task<ResponseBodyModel> GetResigon()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                check(claims.companyId);
                var ResigonData = await db.ResignationResigons.Where(x => x.IsDeleted == false)
                            .Select(x => new
                            {
                                x.Id,
                                x.Name,
                            }).ToListAsync();

                if (ResigonData.Count > 0)
                {
                    res.Message = "Resigon Add";
                    res.Status = true;
                    res.Data = ResigonData;
                }
                else
                {
                    res.Message = "Failed To Data";
                    res.Status = false;
                    res.Data = ResigonData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public void check(int companyId)
        {
            var checking = db.ResignationResigons.Where(x => x.CompanyId == companyId && x.IsActive == true && x.IsDeleted == false).ToList();
            if (checking.Count == 0)
            {
                ResignationResigon obj = new ResignationResigon
                {
                    Name = "Other",
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                };
                db.ResignationResigons.Add(obj);
                db.SaveChanges();
            }
        }

        #endregion

        #region Edit Resigon Data

        [HttpPut]
        [Route("putresigon")]
        public async Task<ResponseBodyModel> EditResigonData(ResignationResigon model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Add = await db.ResignationResigons.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (Add != null)
                {
                    Add.Name = model.Name;
                    Add.UpdatedBy = claims.employeeId;
                    Add.CompanyId = claims.companyId;
                    Add.OrgId = claims.employeeId;
                    Add.UpdatedOn = DateTime.Now;
                    Add.IsActive = true;
                    Add.IsDeleted = false;

                    db.Entry(Add).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Resigon UpDated";
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

        #endregion

        #region  Edit Resigon  Data

        [HttpDelete]
        [Route("deleteresigon")]
        public async Task<ResponseBodyModel> EditResigonData(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteData = await db.ResignationResigons.FirstOrDefaultAsync(x =>
                    x.Id == Id && x.IsDeleted == false && x.IsActive == true);
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;

                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Data Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Data Found!!";
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
    }
}