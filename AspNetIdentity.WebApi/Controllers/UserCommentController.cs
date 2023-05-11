using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/main")]
    public class UserCommentController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region add comment on particular post

        /// <summary>
        /// created by Shubham Sharma on 9/7/2022
        /// Api >> Post >> api/main/PostComment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostComment")]
        public async Task<ResponseBodyModel> CreateComment(UserComment model)
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
                    UserComment comment = new UserComment();

                    comment.PostId = model.PostId;
                    comment.Comment = model.Comment;
                    comment.EmpId = model.EmpId;
                    comment.CompanyId = claims.companyId;
                    comment.OrgId = claims.orgId;
                    comment.CreatedOn = DateTime.Now;
                    comment.CreatedBy = claims.employeeId;
                    comment.IsActive = true;
                    comment.IsDeleted = false;
                    _db.UserComments.Add(comment);
                    await _db.SaveChangesAsync();

                    res.Message = "Comment Post";
                    res.Status = true;
                    res.Data = comment;
                }
            }
            catch (Exception)
            {
                res.Message = "Model Is Invalid";
                res.Status = false;
            }
            return res;
        }

        #endregion add comment on particular post

        #region get comment on particular post

        [HttpGet]
        [Route("getcomment")]
        public async Task<ResponseBodyModel> GetComment()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var CommentData = await _db.UserComments.Where(x => x.IsDeleted == false).ToListAsync();
                if (CommentData.Count > 0)
                {
                    res.Message = "Recived the comment";
                    res.Status = true;
                    res.Data = CommentData;
                }
                else
                {
                    res.Message = "Failed To Recive comment";
                    res.Status = false;
                    res.Data = CommentData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion get comment on particular post
    }
}