using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.EmossyWallModel;
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

namespace AspNetIdentity.WebApi.Controllers.EmossyWall
{
    /// <summary>
    /// Created By Harshit Mitra On 02-03-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/wallaction")]
    public class EmossyWallActionsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO UPDATE LIKE STATUS   
        /// <summary>
        /// Created By Harshit Mitra On 09-03-2023
        /// API >> POST >> api/wallaction/sendlikereaction
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("sendlikereaction")]
        public async Task<IHttpActionResult> SendLikeReaction(Guid wallPostId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var likeReaction = await _db.PostLikes
                    .FirstOrDefaultAsync(x => x.WallPostId == wallPostId &&
                        x.EmployeeId == tokenData.employeeId);
                if (likeReaction == null)
                {
                    PostLike obj = new PostLike
                    {
                        EmployeeId = tokenData.employeeId,
                        LikeStatus = true,
                        WallPostId = wallPostId,
                    };
                    _db.PostLikes.Add(obj);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    likeReaction.LikeStatus = !likeReaction.LikeStatus;
                    _db.Entry(likeReaction).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Message = "Status Updated";

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wallaction/sendlikereaction | " +
                    "Wall Post Id : " + wallPostId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD COMMENTS
        /// <summary>
        /// Created By Harshit Mitra On 09-03-2023
        /// API >> POST >> api/wallaction/createcomments
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("createcomments")]
        public async Task<IHttpActionResult> CreateComments(AddCommentRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (String.IsNullOrEmpty(model.Comment))
                {
                    res.Message = "Input Valid Comment";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }

                PostComment obj = new PostComment
                {
                    WallPostId = model.WallPostId,
                    Comment = model.Comment,
                    EmployeeId = tokenData.employeeId,
                    CommentOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                };
                _db.PostComments.Add(obj);
                await _db.SaveChangesAsync();

                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Message = "Status Updated";
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wallaction/sendlikereaction | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AddCommentRequest
        {
            public Guid WallPostId { get; set; } = Guid.Empty;
            public string Comment { get; set; } = String.Empty;
        }
        #endregion

        #region API TO GET COMMENT BY POST ID
        /// <summary>
        /// Created By Harshit Mitra On 09-03-2023
        /// API >> POST >> api/wallaction/getpostcomment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpostcomment")]
        public async Task<IHttpActionResult> GetPostComment(Guid wallPostId, int take = 5)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var comments = await (from c in _db.PostComments
                                      join e in _db.Employee on c.EmployeeId equals e.EmployeeId
                                      where c.WallPostId == wallPostId
                                      select new
                                      {
                                          c.Id,
                                          c.EmployeeId,
                                          e.DisplayName,
                                          e.ProfileImageUrl,
                                          c.Comment,
                                          c.CommentOn,
                                      })
                                      .OrderByDescending(x => x.CommentOn)
                                      .Take(take)
                                      .ToListAsync();
                if (comments.Count == 0)
                {
                    res.Message = "No Comment Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = comments;
                    return Ok(res);
                }

                res.Message = "Comment Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = comments
                        .Select(x => new
                        {
                            x.Id,
                            x.DisplayName,
                            x.ProfileImageUrl,
                            x.Comment,
                            x.CommentOn,
                            DeleteAccess = (tokenData.IsAdminInCompany) || (tokenData.employeeId == x.EmployeeId),
                            Interval = TimeIntervalHelper.GetIntervalOffSet(x.CommentOn, today),
                        })
                        .ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wallaction/sendlikereaction | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Wall Post Id : " + wallPostId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO DELETE COMMENT BY ID
        /// <summary>
        /// Created By Harshit Mitra On 09-03-2023
        /// API >> POST >> api/wallaction/deletecomment
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("deletecomment")]
        public async Task<IHttpActionResult> DeleteComment(Guid id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var comments = await _db.PostComments.FirstOrDefaultAsync(x => x.Id == id);
                if (comments == null)
                {
                    res.Message = "Comment Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                _db.PostComments.Remove(comments);
                await _db.SaveChangesAsync();

                res.Message = "Comment Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wallaction/sendlikereaction | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Id : " + id + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL USER LIKES OF POST
        /// <summary>
        /// Created By Harshit Mitra On 09-03-2023
        /// API >> GET >> api/wallaction/getpostlikes
        /// </summary>
        /// <param name="wallPostId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpostlikes")]
        public async Task<IHttpActionResult> GetPostLikes(Guid wallPostId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var likes = await (from l in _db.PostLikes
                                   join e in _db.Employee on l.EmployeeId equals e.EmployeeId
                                   where l.WallPostId == wallPostId && l.LikeStatus
                                   select new
                                   {
                                       e.DisplayName,
                                       ProfileImageUrl = e.ProfileImageUrl ?? String.Empty,
                                   })
                                   .ToListAsync();
                if (likes.Count == 0)
                {
                    res.Message = "No Comment Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = likes;
                    return Ok(res);
                }

                res.Message = "Comment Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = likes;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wallaction/getpostlikes | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Wall Post Id : " + wallPostId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion
    }
}
