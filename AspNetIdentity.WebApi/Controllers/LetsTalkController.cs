using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/main")]
    public class LetsTalkController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Post data in Group

        /// <summary>
        /// created by Mayank Prajapati on 18/7/2022
        /// Api >> Post >> api/main/groupuser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("grouppost")]
        [Authorize]
        public async Task<ResponseBodyModel> GroupPost(GroupUser model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Unvalid";
                    res.Status = false;
                }
                else
                {
                    GroupUser post = new GroupUser
                    {
                        UserInGroupId = model.UserInGroupId,
                        EmployeeId = model.EmployeeId,
                        GroupId = model.GroupId,
                        UserName = model.UserName,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.GPSUser.Add(post);
                    await _db.SaveChangesAsync();
                    res.Message = "Post Added";
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

        #endregion Post data in Group

        #region

        [Route("creategroup")]
        [HttpPost]
        public async Task<ResponseBodyModel> CreateGroup(Group model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var data = _db.Employee.Where(x => x.OrgId == 0).Select(x => x.EmployeeId).FirstOrDefault();
#pragma warning disable CS0472 // The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
                    if (data != null)
                    {
                        Group post = new Group();
                        post.Memberid = _db.Employee.Where(x => x.OrgId == 0).Select(x => x.EmployeeId).FirstOrDefault();

                        post.Description = model.Description;
                        post.GroupName = model.GroupName;
                        post.GroupImageURL = model.GroupImageURL;
                        post.Messagedesc = model.Messagedesc;
                        post.CreatedBy = claims.employeeId;
                        post.CompanyId = claims.companyId;
                        post.OrgId = claims.orgId;
                        post.CreatedOn = DateTime.Now;
                        post.IsActive = true;
                        post.IsDeleted = false;
                        _db.Groups.Add(post);
                        await _db.SaveChangesAsync();

                        res.Message = "Group Created Successfully";
                        res.Status = true;
                        res.Data = post;
                    }
#pragma warning restore CS0472 // The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
                }
                else
                {
                    res.Message = "Model is Unvalid";
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
    }
}