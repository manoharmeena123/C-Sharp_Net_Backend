using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Performence;
using AspNetIdentity.WebApi.Model.Reviews;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;
using Logger = NLog.Logger;
using LogManager = NLog.LogManager;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Reviews")]
    public class GroupReController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Enum Classes Review Model

        #region This Api Use To Get Frequrnce Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Get >> api/Reviews/getfrequrnce
        ///// </summary>api/Funtion/getjobfuntiondepartment
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getfrequrnce")]
        [HttpGet]
        public IHttpActionResult GetFrequrnce()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewGroup = Enum.GetValues(typeof(FrequenceConstants))
                    .Cast<FrequenceConstants>()
                    .Select(x => new GrouphelperModel
                    {
                        ReviewGroupTypeId = (int)x,
                        ReviewGroupType = Enum.GetName(typeof(FrequenceConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Review Group Exist";
                res.Status = true;
                res.Data = reviewGroup;
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/getjobfuntiondepartment", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GrouphelperModel
        {
            public int ReviewGroupTypeId { get; set; }
            public string ReviewGroupType { get; set; }
        }
        #endregion

        #region This Api use To Get Revoewcycle Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Get >> api/Reviews/getrevoewcycleofgroup
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getrevoewcycleofgroup")]
        [HttpGet]
        public IHttpActionResult GetReviewCycle()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcyclegroup = Enum.GetValues(typeof(FrequenceConstants))
                    .Cast<FrequenceConstants>()
                    .Select(x => new ReviewGrouphelperModel
                    {
                        ReviewCycleGroupTypeId = (int)x,
                        ReviewCycleGroupType = Enum.GetName(typeof(FrequenceConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Review Cycle Group Exist";
                res.Status = true;
                res.Data = reviewcyclegroup;
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getrevoewcycleofgroup", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class ReviewGrouphelperModel
        {
            public int ReviewCycleGroupTypeId { get; set; }
            public string ReviewCycleGroupType { get; set; }
        }
        #endregion

        #region This Api Use To Get Employee Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Get >> api/Reviews/getallemployee
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getallemployee")]
        [HttpGet]
        public IHttpActionResult GetReviewEmployee()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employegroup = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                    .Select(x => new EmployeeReviewGrouphelperModel
                    {
                        EmployeeTypeId = x.EmployeeId,
                        EmployeeGroupType = x.DisplayName
                    }).ToList();
                res.Message = "Employee Group Exist";
                res.Status = true;
                res.Data = employegroup;
            }
            catch (Exception ex)
            {
                logger.Error(" api/Reviews/getallemployee", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class EmployeeReviewGrouphelperModel
        {
            public int EmployeeTypeId { get; set; }
            public string EmployeeGroupType { get; set; }
        }
        #endregion

        #endregion

        #region This Api Use  Review Group

        #region This Api Is use To Add Review Group
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Post >> api/Reviews/addreviewgroup
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addreviewgroup")]
        public async Task<IHttpActionResult> AddReviewGroup(ReviewsGroup model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    ReviewsGroup postdata = new ReviewsGroup
                    {
                        EmployeeId = model.EmployeeId,
                        ReviewCycle = model.ReviewCycle,
                        FrequenceReview = model.FrequenceReview,
                        ReviewGroupName = model.ReviewGroupName,
                        Description = model.Description,
                        ManageGroup = _db.Employee.Where(x => x.EmployeeId == model.EmployeeId)
                        .Select(x => x.DisplayName).FirstOrDefault(),
                        StartDate = DateTime.Now,
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.ReviewsGroups.Add(postdata);
                    await _db.SaveChangesAsync();
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = postdata;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addreviewgroup", ex.Message, model);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region This Api Use Get Review Group 
        /// <summary>
        /// API >> Get >>api/Reviews/getallreviewdata
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallreviewdata")]
        public async Task<IHttpActionResult> GetAllReviewData()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewgroup = await _db.ReviewsGroups.Where(x => x.IsActive && !x.IsDeleted
                      && x.CompanyId == tokenData.companyId)
                    .Select(x => new GetReviewsHelperModel
                    {
                        ReviewGroupId = x.ReviewGroupId,
                        ReviewGroupName = x.ReviewGroupName,
                        EmployeeId = x.EmployeeId,
                        Description = x.Description,
                        ReviewCycle = x.ReviewCycle.ToString().Replace("_", " "),
                        FrequenceReview = x.FrequenceReview.ToString().Replace("_", " "),
                        StartDate = x.StartDate,
                        ManageGroup = x.ManageGroup,
                        Count = _db.ReviewsGroups.Where(y => y.EmployeeId == x.EmployeeId
                        && y.ReviewGroupId == x.ReviewGroupId).Count(),
                    }).ToListAsync();
                if (reviewgroup != null)
                {
                    res.Message = "Review Group Successfully Get !";
                    res.Status = true;
                    res.Data = reviewgroup;
                }
                else
                {
                    res.Message = "No Review Group Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getallreviewdata", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetReviewsHelperModel
        {
            public Guid ReviewGroupId { get; set; }
            public string ReviewGroupName { get; set; }
            public int? EmployeeId { get; set; }
            public string Description { get; set; }
            public string ReviewCycle { get; set; }
            public string FrequenceReview { get; set; }
            public DateTime? StartDate { get; set; }
            public string ManageGroup { get; set; }
            public int Count { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get All Review Group  Data 
        /// <summary>
        /// API >> Get >>api/Reviews/getallreiviewdata
        ///  Created by  Mayank Prajapati On 13/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallreiviewdata")]
        public async Task<IHttpActionResult> GetAllReiviewData(Guid reviewGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewdata = await (from e in _db.Employee
                                        join add in _db.AddMultipleEmployees on e.EmployeeId equals add.EmployeeId
                                        join ad in _db.ReviewsGroups on add.ReviewGroupId equals ad.ReviewGroupId
                                        where !add.IsDeleted && add.IsActive
                                        && ad.ReviewGroupId == reviewGroupId
                                        select new GetEmployee1Data()
                                        {
                                            EmployeeId = e.EmployeeId,
                                            Employee = e.DisplayName,
                                            Department = e.DepartmentName,
                                            Location = e.LocalAddress,
                                            JoiningDate = e.JoiningDate,
                                            ReportingManager = _db.Employee.Where(x => x.EmployeeId == x.EmployeeId)
                                            .Select(x => x.DisplayName).FirstOrDefault(),
                                        }).ToListAsync();
                if (reviewdata != null)
                {
                    res.Message = " Successfully Get Review Group !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = reviewdata;
                }
                else
                {
                    res.Message = "Sprint Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = reviewdata;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getallreiviewdata", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetEmployee1Data
        {
            public int EmployeeId { get; set; }
            public string Employee { get; set; }
            public string ReportingManager { get; set; }
            public string Department { get; set; }
            public string Location { get; set; }
            public string BusinessUnit { get; set; }
            public string PayGrade { get; set; }
            public DateTimeOffset JoiningDate { get; set; }
            public string CurrentReviewGroup { get; set; }
        }
        #endregion

        #region This Api Use Get Review Group By Id
        /// <summary>
        /// API >> Get >>api/Reviews/getReviewgroupbyid
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getReviewgroupbyid")]
        public async Task<IHttpActionResult> GetReviewGroupById(Guid reviewgroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewgroup = await _db.ReviewsGroups.Where(x => x.IsActive && !x.IsDeleted
                      && x.ReviewGroupId == reviewgroupId && x.CompanyId == tokenData.companyId)
                    .Select(x => new GetReviewsGroupHelperModel
                    {
                        ReviewGroupName = x.ReviewGroupName,
                        EmployeeId = x.EmployeeId,
                        Description = x.Description,
                        ReviewCycle = x.ReviewCycle.ToString().Replace("_", " "),
                        FrequenceReview = x.FrequenceReview.ToString().Replace("_", " "),
                        StartDate = x.StartDate,
                        ManageGroup = x.ManageGroup,
                        Count = _db.ReviewsGroups.Where(y => y.EmployeeId == x.EmployeeId).Count(),
                    }
                      ).ToListAsync();
                if (reviewgroup != null)
                {
                    res.Message = " Successfully Get Reviews Groups !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = reviewgroup;
                }
                else
                {
                    res.Message = "Review Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getReviewgroupbyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetReviewsGroupHelperModel
        {
            public Guid ReviewGroupId { get; set; }
            public string ReviewGroupName { get; set; }
            public int? EmployeeId { get; set; }
            public string Description { get; set; }
            public string ReviewCycle { get; set; }
            public string FrequenceReview { get; set; }
            public DateTime? StartDate { get; set; }
            public string ManageGroup { get; set; }
            public int Count { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Update Review Group
        /// <summary>
        /// API >> Put >>api/Reviews/updatereviewgroup
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updatereviewgroup")]
        public async Task<IHttpActionResult> UpdateReviewGroup(ReviewsGroup model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewgroup = await _db.ReviewsGroups.FirstOrDefaultAsync(x => x.ReviewGroupId == model.ReviewGroupId);
                if (reviewgroup != null)
                {
                    reviewgroup.ReviewGroupName = model.ReviewGroupName;
                    reviewgroup.Description = model.Description;
                    reviewgroup.FrequenceReview = model.FrequenceReview;
                    reviewgroup.ManageGroup = model.ManageGroup;
                    reviewgroup.UpdatedBy = tokenData.employeeId;
                    reviewgroup.UpdatedOn = DateTime.Now;

                    _db.Entry(reviewgroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Updated Successfully  !";
                    res.Status = true;
                    res.Data = reviewgroup;
                }
                else
                {
                    res.Message = "Review Group Not Update";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updatereviewgroup", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Deleted Review Group
        /// <summary>
        /// API >> Delete >>api/Reviews/reviewgroupdelete
        ///  Created by Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("reviewgroupdelete")]
        public async Task<IHttpActionResult> ReviewGroupDelete(Guid reviewgroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewgroup = await _db.ReviewsGroups.FirstOrDefaultAsync(x =>
                    x.IsActive && !x.IsDeleted && x.ReviewGroupId == reviewgroupId);
                if (reviewgroup != null)
                {
                    reviewgroup.IsDeleted = true;
                    reviewgroup.IsActive = false;
                    reviewgroup.DeletedBy = tokenData.employeeId;
                    reviewgroup.DeletedOn = DateTime.Now;

                    _db.Entry(reviewgroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully !";
                    res.Status = true;
                    res.Data = reviewgroup;

                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/reviewgroupdelete", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region This Api Use Review Cycle 

        #region This Api Is use To Add ReviewCycle Group
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Post >> api/Reviews/addreviewcyclegroup
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addreviewcyclegroup")]
        public async Task<IHttpActionResult> AddReviewCycleGroup(ReviewCycleGroup model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewgroup = await _db.ReviewsGroups.Where(x => x.IsActive && !x.IsDeleted &&
                    x.CompanyId == tokenData.companyId && x.ReviewGroupId == model.ReviewGroupId).FirstOrDefaultAsync();
                if (reviewgroup != null)
                {
                    var employeedetail = _db.EmployeeReviews.Where(x => x.IsActive && !x.IsDeleted
                       && x.ReviewEmployeeId == model.ReviewEmployeeId).FirstOrDefault();
                    if (employeedetail != null)
                    {
                        ReviewCycleGroup cyclepostdata = new ReviewCycleGroup
                        {
                            ReviewGroupId = reviewgroup.ReviewGroupId,
                            ReviewEmployeeId = employeedetail.ReviewEmployeeId,
                            ReviewCycleName = model.ReviewCycleName,
                            Description = model.Description,
                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            CreatedBy = tokenData.employeeId,
                            CompanyId = tokenData.companyId,
                            OrgId = tokenData.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false
                        };
                        _db.ReviewCycleGroups.Add(cyclepostdata);
                        await _db.SaveChangesAsync();

                        res.Message = "Created Successfully  !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = cyclepostdata;
                    }
                    else
                    {
                        res.Message = "ReviewEmployee not found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "ReviewGroup not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addreviewcyclegroup", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Get Review Cycle Group all Data
        /// <summary>
        /// API >> Get >>api/Reviews/getallReviewcycle
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallReviewcycle")]
        public async Task<IHttpActionResult> GetAllReviewCycle()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcycleGroupd = await _db.ReviewCycleGroups.Where(x => x.IsActive && !x.IsDeleted
                 && x.CompanyId == tokenData.companyId).ToListAsync();
                if (reviewcycleGroupd != null)
                {
                    res.Message = "Get Successfully  !";
                    res.Status = true;
                    res.Data = reviewcycleGroupd;
                }
                else
                {
                    res.Message = "Review Cycle Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion Get all Group Detail

        #region This Api Use Get Review Cycle Group By Id
        /// <summary>
        /// API >> Get >>api/Reviews/getReviewcyclebyid?reviewgroupId
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getReviewcyclebyid")]
        public async Task<IHttpActionResult> GetReviewCycleById(Guid reviewgroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcycleGroup = await _db.ReviewCycleGroups.Where(x => x.IsActive && !x.IsDeleted
               && x.ReviewGroupId == reviewgroupId).FirstOrDefaultAsync();
                if (reviewcycleGroup != null)
                {
                    res.Message = "Review Cyele Successfully Get !";
                    res.Status = true;
                    res.Data = reviewcycleGroup;
                }
                else
                {
                    res.Message = "Review Cycle Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getReviewcyclebyid?reviewgroupId", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion Get all Group Detail

        #region This Api Use Remove Review Cycle Group By Id
        /// <summary>
        /// API >> Remove >>api/Reviews/romvereviewcyclebyid?reviewgroupId
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("romvereviewcyclebyid")]
        public async Task<IHttpActionResult> RemoveReviewCycleById(Guid reviewgroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcycleGroup = await _db.ReviewCycleGroups.Where(x => x.IsActive && !x.IsDeleted
                     && x.ReviewGroupId == reviewgroupId).FirstOrDefaultAsync();
                if (reviewcycleGroup != null)
                {
                    reviewcycleGroup.IsDeleted = true;
                    reviewcycleGroup.IsActive = false;
                    reviewcycleGroup.DeletedBy = tokenData.employeeId;
                    reviewcycleGroup.DeletedOn = DateTime.Now;

                    _db.Entry(reviewcycleGroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = reviewcycleGroup;
                }
                else
                {
                    res.Message = "Review Cycle Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/romvereviewcyclebyid?reviewgroupId", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion Get all Group Detail

        #region This Api Is use To Update ReviewCycle Group
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Put >> api/Reviews/updatereviewcyclegroup
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPut]
        [Route("updatereviewcyclegroup")]
        public async Task<IHttpActionResult> UpdateReviewCycleGroup(ReviewCycleGroup model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcycle = await _db.ReviewCycleGroups.Where(x => x.IsActive && !x.IsDeleted
                && x.ReviewEmployeeId == model.ReviewEmployeeId && x.CompanyId == tokenData.companyId).FirstOrDefaultAsync();
                if (reviewcycle != null)
                {
                    reviewcycle.ReviewGroupId = model.ReviewGroupId;
                    reviewcycle.ReviewEmployeeId = model.ReviewEmployeeId;
                    reviewcycle.ReviewCycleName = model.ReviewCycleName;
                    reviewcycle.Description = model.Description;
                    reviewcycle.StartTime = model.StartTime;
                    reviewcycle.EndTime = model.EndTime;
                    reviewcycle.UpdatedBy = tokenData.employeeId;
                    reviewcycle.UpdatedOn = DateTime.Now;

                    _db.Entry(reviewcycle).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Updated Successfully  !";
                    res.Status = true;
                    res.Data = reviewcycle;
                }
                else
                {
                    res.Message = "Review Cycle Group not found";
                    res.Status = false;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region This Api Use Core Competency 

        #region This Api Is use To Add Core Competency
        ///// <summary>
        ///// created by  Mayank Prajapati On 3/11/2022
        ///// Api >> Post >> api/Reviews/addreviewcompetency
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addreviewcompetency")]
        public async Task<IHttpActionResult> AddReviewCompetency(CoreCompetency model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BehaviourTypeResponse response = new BehaviourTypeResponse();
            List<Behaviour> BehaviourList = new List<Behaviour>();
            try
            {
                if (model != null)
                {
                    CoreCompetency postdata = new CoreCompetency
                    {
                        CoreCompetencyName = model.CoreCompetencyName,
                        CompetencyTypeId = model.CompetencyTypeId,
                        Description = model.Description,
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.CoreCompetencys.Add(postdata);
                    await _db.SaveChangesAsync();
                    foreach (var item in model.Behaviours)
                    {
                        if (item != null)
                        {
                            Behaviour BehaType = new Behaviour
                            {
                                CoreCompetencyId = postdata.CoreCompetencyId,
                                Behaveour = item.Behaveour,
                                Action = item.Action,
                                UseInRating = item.UseInRating,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                            };
                            _db.Behaviours.Add(BehaType);
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Core Competency Not Add";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addreviewcompetency", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BehaviourTypeResponse
        {
            public CoreCompetency CoreCompetency { get; set; }
            public List<Behaviour> Behaviours { get; set; }
        }
        #endregion

        #region This Api Use Get All Core Competency
        /// <summary>
        /// API >> Get >>api/Reviews/getcorecompetency
        ///  Created by  Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcorecompetency")]
        public async Task<IHttpActionResult> GetCoreCompetency()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcompetency = await _db.CoreCompetencys.Where(x => x.IsActive && !x.IsDeleted).OrderByDescending(x => x.CreatedOn)
                    .Select(x => new HelperCorecompetencyModuel
                    {
                        CoreCompetencyId = x.CoreCompetencyId,
                        CoreCompetencyName = x.CoreCompetencyName,
                        CompetencyTypeId = (int)x.CompetencyTypeId,
                        CompetencyTypeName = x.CompetencyTypeId.ToString().Replace("_", " "),
                        Description = x.Description,
                        Count = _db.Behaviours.Where(y => y.CoreCompetencyId == x.CoreCompetencyId).Count(),
                        behaviours = _db.Behaviours.Where(y => y.CoreCompetencyId == x.CoreCompetencyId).Select(y => new BehaviaororeResponse
                        {
                            behavioursId = y.BehaviourId,
                            useInRating = y.UseInRating,
                            Behaveour = y.Behaveour
                        }).ToList(),
                    }).ToListAsync();
                if (reviewcompetency != null)
                {
                    res.Message = " Core Competency Successfully Get !";
                    res.Status = true;
                    res.Data = reviewcompetency;
                }
                else
                {
                    res.Message = "Core Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getcorecompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class HelperCorecompetencyModuel
        {
            public Guid CoreCompetencyId { get; set; }
            public string CoreCompetencyName { get; set; }
            public int CompetencyTypeId { get; set; }
            public string Description { get; set; }
            public int Count { get; set; }
            public string CompetencyTypeName { get; set; }
            public List<BehaviaororeResponse> behaviours { get; set; }
        }
        public class BehaviaororeResponse
        {
            public Guid behavioursId { get; set; }
            public bool useInRating { get; set; }
            public string Behaveour { get; set; }
        }
        #endregion

        #region This Api Use Core Competency Get By Id
        // <summary>
        // API >> Get >>api/Reviews/getcorecompetencybyid
        // Created by Mayank Prajapati On 3/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getcorecompetencybyid")]
        public async Task<IHttpActionResult> GetCoreCompetencyById(Guid CoreCompetencyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var corecdata = _db.CoreCompetencys.Where(x => x.IsActive && !x.IsDeleted && x.CoreCompetencyId == CoreCompetencyId)
                  .Select(x => new HelperModuel
                  {
                      CoreCompetencyId = x.CoreCompetencyId,
                      CoreCompetencyName = x.CoreCompetencyName,
                      Description = x.Description,
                      CompetencyTypeId = (int)x.CompetencyTypeId,
                      CompetencyTypeName = x.CoreCompetencyName,
                      behaviour = _db.Behaviours.Where(y => y.CoreCompetencyId == x.CoreCompetencyId).Select(y => new BehaviaorResponse
                      {
                          behavioursId = y.BehaviourId,
                          useInRating = y.UseInRating,
                          Behaveour = y.Behaveour
                      }).ToList(),
                  }).FirstOrDefault();
                if (corecdata != null)
                {
                    res.Message = " Successfully Get Core Competencys !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = corecdata;
                }
                else
                {
                    res.Message = "Core Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getcorecompetencybyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class HelperModuel
        {
            public Guid CoreCompetencyId { get; set; }
            public string CoreCompetencyName { get; set; }
            public int CompetencyTypeId { get; set; }
            public string Description { get; set; }
            public string CompetencyTypeName { get; set; }
            public List<BehaviaorResponse> behaviour { get; set; }
        }
        public class BehaviaorResponse
        {
            public Guid behavioursId { get; set; }
            public bool useInRating { get; set; }
            public string Behaveour { get; set; }
        }
        #endregion

        #region This Api Use Get Enum Competency Type
        /// <summary>
        /// API >> Get >>api/Reviews/getcompetencytype
        ///  Created by  Mayank Prajapati On 09/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompetencytype")]
        public async Task<ResponseBodyModel> GetCompetencyType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var CompetencyData = Enum.GetValues(typeof(CompetencyTypeConstants))
                    .Cast<CompetencyTypeConstants>()
                    .Select(x => new CompetencyTypeModel
                    {
                        CompetencyTypeId = (int)x,
                        CompetencyType = Enum.GetName(typeof(CompetencyTypeConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Competency Data exist";
                res.Status = true;
                res.Data = CompetencyData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class CompetencyTypeModel
        {
            public int CompetencyTypeId { get; set; }
            public string CompetencyType { get; set; }
        }
        #endregion

        #region This Api Use Update Core Competency
        /// <summary>
        /// API >> Put >>api/Reviews/updatecorecompetency
        ///  Created by  Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        ///
        [HttpPost]
        [Route("updatecorecompetency")]
        public async Task<IHttpActionResult> UpdateCoreCompetency(BehaviourupTypeResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BehaviourupTypeResponse response = new BehaviourupTypeResponse();
            List<BehaviourData> BehaviourList = new List<BehaviourData>();
            try
            {
                var corecompetency = await _db.CoreCompetencys.FirstOrDefaultAsync(x => x.CoreCompetencyId == model.CoreCompetencyId);
                if (corecompetency != null)
                {
                    corecompetency.CoreCompetencyName = model.CoreCompetencyName;
                    corecompetency.Description = model.Description;
                    corecompetency.CompetencyTypeId = model.CompetencyTypeId;
                    corecompetency.UpdatedBy = tokenData.employeeId;
                    corecompetency.UpdatedOn = DateTime.Now;
                    corecompetency.IsActive = true;
                    corecompetency.IsDeleted = false;
                    _db.Entry(corecompetency).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    foreach (var item in model.Behaviours)
                    {
                        var behavioures = _db.Behaviours.Where(x => x.BehaviourId == item.BehavioursId).FirstOrDefault();
                        {
                            behavioures.Behaveour = item.Behaveour;
                            behavioures.UseInRating = item.UseInRating;
                            behavioures.Action = item.Action;
                            behavioures.UpdatedBy = tokenData.employeeId;
                            behavioures.UpdatedOn = DateTime.Now;
                            behavioures.IsActive = true;
                            behavioures.IsDeleted = false;

                            _db.Entry(behavioures).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Update Successfully  !";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Core Competency Not Updated";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updatecorecompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BehaviourupTypeResponse
        {
            public Guid CoreCompetencyId { get; set; }

            public string CoreCompetencyName { get; set; }
            public string Description { get; set; }
            public CompetencyTypeConstants CompetencyTypeId { get; set; }

            public List<BehaviourData> Behaviours { get; set; }
        }
        public class BehaviourData
        {
            public Guid BehavioursId { get; set; }
            public string Behaveour { get; set; }
            public bool UseInRating { get; set; }
            public bool Action { get; set; }
        }
        #endregion

        #region This Api Use Deleted Core Competency
        /// <summary>
        /// API >> Delete >>api/Reviews/removecorecompetency
        ///  Created by Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>

        [HttpDelete]
        [Route("removecorecompetency")]
        public async Task<IHttpActionResult> CoreCompetencyDelete(Guid CoreCompetencyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var corecompetency = await _db.CoreCompetencys.FirstOrDefaultAsync(x =>
                    x.CoreCompetencyId == CoreCompetencyId && x.IsActive && !x.IsDeleted);
                if (corecompetency != null)
                {
                    corecompetency.IsDeleted = true;
                    corecompetency.IsActive = false;
                    corecompetency.DeletedBy = tokenData.employeeId;
                    corecompetency.DeletedOn = DateTime.Now;

                    _db.Entry(corecompetency).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = corecompetency;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/removecorecompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region This Api Use Common Success Competency 

        #region This Api Is use To Add Common Success Competency
        ///// <summary>
        ///// created by  Mayank Prajapati On 8/11/2022
        ///// Api >> Post >> api/Reviews/addcommonsuccesscompetency
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addcommonsuccesscompetency")]
        public async Task<IHttpActionResult> AddCommonSuccessCompetency(CommonSuccessCompetency model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BeheviourTypeResponse response = new BeheviourTypeResponse();
            List<Behaviour> Behaviourlist = new List<Behaviour>();
            try
            {
                if (model != null)
                {
                    CommonSuccessCompetency Post = new CommonSuccessCompetency();
                    Post.CommonSuccessCompetencyName = model.CommonSuccessCompetencyName;
                    Post.Description = model.Description;
                    Post.CompetencyTypeId = model.CompetencyTypeId;
                    var CompetencyTypeName = Enum.GetName(typeof(CompetencyTypeConstants), model.CompetencyTypeId);
                    Post.CreatedBy = tokenData.employeeId;
                    Post.CompanyId = tokenData.companyId;
                    Post.OrgId = tokenData.employeeId;
                    Post.CreatedOn = DateTime.Now;
                    Post.IsActive = true;
                    Post.IsDeleted = false;
                    _db.CommonSuccessCompetencys.Add(Post);
                    await _db.SaveChangesAsync();
                    response.CommonSuccessCompetency = Post;

                    foreach (var item in model.Behaviours)
                    {
                        if (item != null)
                        {
                            Behaviour BehaType = new Behaviour
                            {
                                CommonSuccessCompetencyId = Post.CommonSuccessCompetencyId,
                                Behaveour = item.Behaveour,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                            };
                            _db.Behaviours.Add(BehaType);
                            await _db.SaveChangesAsync();
                            Behaviourlist.Add(BehaType);
                        }
                    }
                    response.Behavioures = Behaviourlist;

                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Common Success Competency Not Add";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addcommonsuccesscompetency", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BeheviourTypeResponse
        {
            public CommonSuccessCompetency CommonSuccessCompetency { get; set; }

            public List<Behaviour> Behavioures { get; set; }
        }
        #endregion

        #region This Api Use Get All Common Success Competency
        /// <summary>
        /// API >> Get >>api/Reviews/getcommonsuccesscompetency
        ///  Created by  Mayank Prajapati On 8/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcommonsuccesscompetency")]
        public async Task<IHttpActionResult> GetCommonSuccessCompetency()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcompetency = await _db.CommonSuccessCompetencys.Where(x => x.IsActive &&
                   !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();
                if (reviewcompetency != null)
                {
                    res.Message = "Common Success Succesfully Get !";
                    res.Status = true;
                    res.Data = reviewcompetency;
                }
                else
                {
                    res.Message = "Common Success Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getcommonsuccesscompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Get Common Competency By Id
        /// <summary>
        /// API >> Get >>api/Reviews/getcommoncompetencybyid
        /// Created by  Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcommoncompetencybyid")]
        public async Task<IHttpActionResult> GetCommonCompetencyById(Guid CommonSuccessCompetencyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var commonsuccessdata = await _db.CommonSuccessCompetencys.Where(x => x.IsActive && !x.IsDeleted && x.CommonSuccessCompetencyId == CommonSuccessCompetencyId)
               .Select(x => new CommonHelperModuel
               {
                   CommonSuccessCompetencyName = x.CommonSuccessCompetencyName,
                   Description = x.Description,
                   CompetencyTypeId = (int)x.CompetencyTypeId,
                   behaviours = _db.Behaviours.Where(y => y.CommonSuccessCompetencyId == x.CommonSuccessCompetencyId).Select(y => new BehaviaortResponse
                   {
                       behavioursId = y.BehaviourId,
                       useInRating = y.UseInRating,
                       behaveour = y.Behaveour
                   }).ToList(),
               }).FirstOrDefaultAsync();

                if (commonsuccessdata != null)
                {
                    res.Message = " Succesfully Get Common Success !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = commonsuccessdata;
                }
                else
                {
                    res.Message = "Common Success Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getcommoncompetencybyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class CommonHelperModuel
        {
            public string CommonSuccessCompetencyName { get; set; }
            public int CompetencyTypeId { get; set; }
            public string Description { get; set; }
            public string CompetencyTypeName { get; set; }
            public List<BehaviaortResponse> behaviours { get; set; }
        }
        public class BehaviaortResponse
        {
            public Guid behavioursId { get; set; }
            public bool useInRating { get; set; }
            public string behaveour { get; set; }
        }
        #endregion

        #region This Api Use Get Common Enum Competency Type
        /// <summary>
        /// API >> Get >>api/Reviews/getcommoncompetencytype
        ///  Created by  Mayank Prajapati On 11/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcommoncompetencytype")]
        public async Task<ResponseBodyModel> GetCommonCompetencyType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var CommonCompetencyData = Enum.GetValues(typeof(CompetencyTypeConstants))
                    .Cast<CompetencyTypeConstants>()
                    .Select(x => new CommonTypeModel
                    {
                        CommonCompetencyTypeId = (int)x,
                        CommonCompetencyType = Enum.GetName(typeof(CompetencyTypeConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Common Competency Data exist";
                res.Status = true;
                res.Data = CommonCompetencyData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class CommonTypeModel
        {
            public int CommonCompetencyTypeId { get; set; }
            public string CommonCompetencyType { get; set; }
        }

        #endregion

        #region This Api Use Update Common Success Competency
        /// <summary>
        /// API >> Put >>api/Reviews/updatecommonsuccesscompetency
        ///  Created by  Mayank Prajapati On 8/11/2022
        /// </summary>
        /// <returns></returns>
        ///
        [HttpPut]
        [Route("updatecommonsuccesscompetency")]
        public async Task<IHttpActionResult> UpdateCommonSuccessCompetency(CommonSuccessCompetency model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BehaviourCommonTypeResponse response = new BehaviourCommonTypeResponse();
            List<Behaviour> BehaviourList = new List<Behaviour>();
            try
            {
                var commonsuccesscompetency = await _db.CommonSuccessCompetencys.FirstOrDefaultAsync(x => x.CommonSuccessCompetencyId == model.CommonSuccessCompetencyId);
                if (commonsuccesscompetency != null)
                {
                    commonsuccesscompetency.CommonSuccessCompetencyName = model.CommonSuccessCompetencyName;
                    commonsuccesscompetency.Description = model.Description;
                    commonsuccesscompetency.CompetencyTypeId = model.CompetencyTypeId;
                    commonsuccesscompetency.UpdatedBy = tokenData.employeeId;
                    commonsuccesscompetency.UpdatedOn = DateTime.Now;
                    commonsuccesscompetency.IsActive = true;
                    commonsuccesscompetency.IsDeleted = false;
                    _db.Entry(commonsuccesscompetency).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    response.CommonSuccessCompetency = commonsuccesscompetency;

                    foreach (var item in model.Behaviours)
                    {
                        var behavioures = _db.Behaviours.Where(x => x.CommonSuccessCompetencyId == item.CommonSuccessCompetencyId).FirstOrDefault();
                        {
                            behavioures.CommonSuccessCompetencyId = item.CommonSuccessCompetencyId;
                            behavioures.Behaveour = item.Behaveour;
                            behavioures.UseInRating = item.UseInRating;
                            behavioures.Action = item.Action;
                            behavioures.UpdatedBy = tokenData.employeeId;
                            behavioures.UpdatedOn = DateTime.Now;
                            behavioures.IsActive = true;
                            behavioures.IsDeleted = false;

                            _db.Entry(behavioures).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                            response.Behaviours = BehaviourList;
                        }
                        BehaviourList.Add(behavioures);
                    }
                    res.Message = " Successfully Get Common Success Competency !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Common Success Competency Not Updated";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updatecommonsuccesscompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BehaviourCommonTypeResponse
        {
            public CommonSuccessCompetency CommonSuccessCompetency { get; set; }

            public List<Behaviour> Behaviours { get; set; }
        }
        #endregion

        #region This Api Use Deleted Common Success Competency
        /// <summary>
        /// API >> Delete >>api/Reviews/commonsuccesscompetencydelete
        ///  Created by Mayank Prajapati On 8/11/2022
        /// </summary>
        /// <returns></returns>

        [HttpDelete]
        [Route("commonsuccesscompetencydelete")]
        public async Task<IHttpActionResult> CommonSuccessCompetencyDelete(Guid CommonSuccessCompetencyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var commonsuccesscompetency = await _db.CommonSuccessCompetencys.FirstOrDefaultAsync(x =>
                    x.CommonSuccessCompetencyId == CommonSuccessCompetencyId && x.IsActive && !x.IsDeleted);
                if (commonsuccesscompetency != null)
                {
                    commonsuccesscompetency.IsDeleted = true;
                    commonsuccesscompetency.IsActive = false;
                    commonsuccesscompetency.DeletedBy = tokenData.employeeId;
                    commonsuccesscompetency.DeletedOn = DateTime.Now;

                    _db.Entry(commonsuccesscompetency).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = commonsuccesscompetency;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Common Success Competency Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/commonsuccesscompetencydelete", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region This Api Use  Job Specific Competency

        #region This Api Is use To Add Job Specific Competency
        ///// <summary>
        ///// created by  Mayank Prajapati On 8/11/2022
        ///// Api >> Post >> api/Reviews/addjobspecificcompetency
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addjobspecificcompetency")]
        public async Task<IHttpActionResult> AddJobSpecificCompetency(JobSpecificCompetency model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BehiviourTypeResponse response = new BehiviourTypeResponse();
            List<Behaviour> BehaviourList = new List<Behaviour>();
            try
            {
                if (model != null)
                {
                    JobSpecificCompetency Post = new JobSpecificCompetency
                    {
                        JobSpecificCompetencyName = model.JobSpecificCompetencyName,
                        Description = model.Description,
                        CompetencyTypeId = model.CompetencyTypeId,
                        CompetencyTypeName = Enum.GetName(typeof(CompetencyTypeConstants), model.CompetencyTypeId),
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.JobSpecificCompetencys.Add(Post);
                    await _db.SaveChangesAsync();
                    response.JobSpecificCompetency = Post;

                    foreach (var item in model.Behaviours)
                    {
                        if (item != null)
                        {
                            Behaviour BehaType = new Behaviour
                            {
                                JobSpecificCompetencyId = Post.JobSpecificCompetencyId,
                                Behaveour = item.Behaveour,
                                UseInRating = item.UseInRating,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                            };
                            _db.Behaviours.Add(BehaType);
                            await _db.SaveChangesAsync();
                            BehaviourList.Add(BehaType);
                        }
                    }
                    response.Behavioures = BehaviourList;
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Job Specific Competency Not Add";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addjobspecificcompetency", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BehiviourTypeResponse
        {
            public JobSpecificCompetency JobSpecificCompetency { get; set; }
            public List<Behaviour> Behavioures { get; set; }
        }

        #endregion

        #region This Api Use Get All Job Specific Competency
        /// <summary>
        /// API >> Get >>api/Reviews/getaddjobspecificcompetency
        ///  Created by  Mayank Prajapati On 8/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getaddjobspecificcompetency")]
        public async Task<IHttpActionResult> GetJobSpecificCompetency()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var JobSpecificcompetency = await _db.JobSpecificCompetencys.Where(x => x.IsActive &&
                   !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();
                if (JobSpecificcompetency != null)
                {
                    res.Message = "Job Specific Competency Succesfully Get !";
                    res.Status = true;
                    res.Data = JobSpecificcompetency;
                }
                else
                {
                    res.Message = "Job Specific Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getaddjobspecificcompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Get job Competency By Id
        /// <summary>
        /// API >> Get >>api/Reviews/getjobcompetencybyid
        ///  Created by  Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobcompetencybyid")]
        public async Task<IHttpActionResult> GetJobCompetencyById(Guid JobSpecificCompetencyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobdata = _db.JobSpecificCompetencys.Where(x => x.IsActive && !x.IsDeleted && x.JobSpecificCompetencyId == JobSpecificCompetencyId)
              .Select(x => new JobHelperModuel
              {
                  JobSpecificCompetencyId = x.JobSpecificCompetencyId,
                  JobSpecificCompetencyName = x.JobSpecificCompetencyName,
                  Description = x.Description,
                  CompetencyTypeId = (int)x.CompetencyTypeId,
                  behaviours = _db.Behaviours.Where(y => y.JobSpecificCompetencyId == x.JobSpecificCompetencyId).Select(y => new BehaviaorJobResponse
                  {
                      behavioursId = y.BehaviourId,
                      useInRating = y.UseInRating,
                      behaveour = y.Behaveour
                  }).ToList(),
              }).FirstOrDefault();
                if (jobdata != null)
                {
                    res.Message = " Job Specific Competency Successfully Get  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = jobdata;
                }
                else
                {
                    res.Message = "Job Specific Competency Data Not Found";
                    res.Status = false;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getjobcompetencybyid", ex.Message);
                return Ok(res);
            }
        }
        public class JobHelperModuel
        {
            public Guid JobSpecificCompetencyId { get; set; } = Guid.Empty;
            public string JobSpecificCompetencyName { get; set; }
            public int CompetencyTypeId { get; set; }
            public string Description { get; set; }
            public string CompetencyTypeName { get; set; }
            public List<BehaviaorJobResponse> behaviours { get; set; }
        }
        public class BehaviaorJobResponse
        {
            public Guid behavioursId { get; set; }
            public bool useInRating { get; set; }
            public string behaveour { get; set; }

        }
        #endregion

        #region This Api Use Get Enum Competency Type
        /// <summary>
        /// API >> Get >>api/Reviews/getjobcompetencytype
        ///  Created by  Mayank Prajapati On 09/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobcompetencytype")]
        public async Task<IHttpActionResult> GetJobCompetencyType()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var JobCompetencyData = Enum.GetValues(typeof(CompetencyTypeConstants))
                    .Cast<CompetencyTypeConstants>()
                    .Select(x => new JobTypeModel
                    {
                        JobCompetencyTypeId = (int)x,
                        JobCompetencyType = Enum.GetName(typeof(CompetencyTypeConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = " Job Competency Data exist";
                res.Status = true;
                res.Data = JobCompetencyData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        public class JobTypeModel
        {
            public int JobCompetencyTypeId { get; set; }
            public string JobCompetencyType { get; set; }
        }
        #endregion

        #region Api for Get Compentic by department
        /// <summary>
        /// created by Mayank Prajapati on 22/11/2022
        /// Api >> Post >>api/Reviews/getallcompentencies
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcompentencies")]
        public async Task<IHttpActionResult> GetAllCompentencies()
        {
            JobFunctionHelper AllCompentencies = new JobFunctionHelper();
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<JobFunctionData> jodata = new List<JobFunctionData>();
                var jobfunctiondata = await _db.JobFuntions.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefaultAsync();
                {
                    JobFunctionData obj = new JobFunctionData();
                    obj.CompentenciesName = _db.JobFunctionCompencies.Where(y => y.CompentenciesId == jobfunctiondata.CompetenciesId).Select(y => y.CompentenciesName).FirstOrDefault();
                    obj.BehaviourCount = _db.Behaviours.Where(z => z.IsActive && !z.IsDeleted && z.CompanyId == tokenData.companyId).ToList().Count;
                    obj.Weight = _db.WeightAges.Where(f => f.IsActive && !f.IsDeleted && f.EmployeeId == tokenData.employeeId).Select(f => f.Weightage1).FirstOrDefault();
                    obj.CompentenciesType = Enum.GetName(typeof(CompetencyTypeConstants), jobfunctiondata.CompetencyTypeId);
                    obj.Description = jobfunctiondata.Description;
                    jodata.Add(obj);
                }
                AllCompentencies.JobFunctionDatas = jodata;

                res.Message = "Data Found !";
                res.Status = true;
                res.Data = AllCompentencies;
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getallcompentencies", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class JobFunctionHelper
        {
            public List<JobFunctionData> JobFunctionDatas { get; set; }
            public List<jobfunctioncompenciesdata> JobFunctionCompenciesDatas { get; set; }
            public List<departmentjobfuntiondata> DepartmentJobFuntionData { get; set; }
        }
        public class JobFunctionData
        {
            public string Description { get; set; }
            public int BehaviourCount { get; set; }
            public int Weight { get; set; }
            public string CompentenciesName { get; set; }
            public string CompentenciesType { get; set; }
        }
        public class jobfunctioncompenciesdata
        {
            public string Description { get; set; }
            public int BehaviourCount { get; set; }
            public int Weight { get; set; }
            public string CompentenciesName { get; set; }
            public string CompentenciesType { get; set; }
        }
        public class departmentjobfuntiondata
        {

            public string Description { get; set; }
            public int BehaviourCount { get; set; }
            public int Weight { get; set; }
            public string CompentenciesName { get; set; }
            public string CompentenciesType { get; set; }
        }
        #endregion

        #region This Api Use Update Job Specific Competency
        /// <summary>
        /// API >> Put >>api/Reviews/updatejobspecificcompetency
        ///  Created by  Mayank Prajapati On 8/11/2022
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("updatejobspecificcompetency")]
        public async Task<IHttpActionResult> UpdateJobSpecificCompetency(BehaviourJobTypeResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BehaviourJobTypeResponse response = new BehaviourJobTypeResponse();
            BehaviourDataModel BehaviourList = new BehaviourDataModel();
            try
            {
                var jobspecificcompetency = await _db.JobSpecificCompetencys.FirstOrDefaultAsync(x => x.JobSpecificCompetencyId == model.JobSpecificCompetencyId);
                if (jobspecificcompetency != null)
                {
                    jobspecificcompetency.JobSpecificCompetencyName = model.JobSpecificCompetencyName;
                    jobspecificcompetency.Description = model.Description;
                    jobspecificcompetency.CompetencyTypeName = Enum.GetName(typeof(CompetencyTypeConstants), model.CompetencyTypeId).Replace("_", " ");
                    jobspecificcompetency.UpdatedBy = tokenData.employeeId;
                    jobspecificcompetency.UpdatedOn = DateTime.Now;
                    jobspecificcompetency.IsActive = true;
                    jobspecificcompetency.IsDeleted = false;
                    _db.Entry(jobspecificcompetency).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    foreach (var item in model.Behaviours)
                    {
                        var behavioures = _db.Behaviours.Where(x => x.BehaviourId == item.behavioursId).FirstOrDefault();
                        {
                            behavioures.Behaveour = item.behaveour;
                            behavioures.UseInRating = item.useInRating;
                            behavioures.UpdatedBy = tokenData.employeeId;
                            behavioures.UpdatedOn = DateTime.Now;
                            behavioures.IsActive = true;
                            behavioures.IsDeleted = false;

                            _db.Entry(behavioures).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Update Successfully  !";
                    res.Status = true;
                    res.Data = null;
                }
                else
                {
                    res.Message = "Job Specific Competency Not Updated";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updatejobspecificcompetency", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BehaviourJobTypeResponse
        {
            public Guid JobSpecificCompetencyId { get; set; }
            public string JobSpecificCompetencyName { get; set; }
            public string Description { get; set; }
            public int CompetencyTypeId { get; set; }
            public List<BehaviourDataModel> Behaviours { get; set; }
        }
        public class BehaviourDataModel
        {
            public Guid behavioursId { get; set; }
            public string behaveour { get; set; }
            public bool useInRating { get; set; }
        }
        #endregion

        #region This Api Use Deleted Job Specific Competency
        /// <summary>
        /// API >> Delete >>api/Reviews/jobspecificcompetencydelete
        ///  Created by Mayank Prajapati On 8/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("jobspecificcompetencydelete")]
        public async Task<IHttpActionResult> JobSpecificCompetencyDelete(Guid JobSpecificCompetencyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobspecificcompetency = await _db.JobSpecificCompetencys.FirstOrDefaultAsync(x =>
                    x.JobSpecificCompetencyId == JobSpecificCompetencyId && x.IsActive && !x.IsDeleted);
                if (jobspecificcompetency != null)
                {
                    jobspecificcompetency.IsDeleted = true;
                    jobspecificcompetency.IsActive = false;
                    jobspecificcompetency.DeletedBy = tokenData.employeeId;
                    jobspecificcompetency.DeletedOn = DateTime.Now;

                    _db.Entry(jobspecificcompetency).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = jobspecificcompetency;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Job Specific Competency Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/jobspecificcompetencydelete", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region This Api Use Review Core Value

        #region This Api Is use To Add Core Value
        ///// <summary>
        ///// created by  Mayank Prajapati On 16/11/2022
        ///// Api >> Post >>api/Reviews/addreviewcorevalue
        ///// </summary>    
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addreviewcorevalue")]
        public async Task<IHttpActionResult> AddReviewCoreValue(ReviewCoreValue model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            CoreValueTypeResponse response = new CoreValueTypeResponse();
            List<Behaviour> BeheviourList = new List<Behaviour>();
            try
            {
                if (model != null)
                {
                    ReviewCoreValue Post = new ReviewCoreValue
                    {
                        ReviewCoreValueName = model.ReviewCoreValueName,
                        Description = model.Description,
                        Behaviours = model.Behaviours,
                        Badge = model.Badge,
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.ReviewCoreValues.Add(Post);
                    await _db.SaveChangesAsync();
                    foreach (var item in model.Behaviours)
                    {
                        if (item != null)
                        {
                            Behaviour BehaType = new Behaviour
                            {
                                ReviewCoreValueId = Post.ReviewCoreValueId,
                                Behaveour = item.Behaveour,
                                IsActive = true,
                                IsDeleted = false,
                                UseInRating = item.UseInRating,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                            };
                            _db.Behaviours.Add(BehaType);
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Review Core Value Added";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addreviewcorevalue", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class CoreValueTypeResponse
        {
            public ReviewCoreValue ReviewCoreValue { get; set; }
            public List<Behaviour> Behaviours { get; set; }
        }
        #endregion

        #region This Api Use Get All Review Core Value
        /// <summary>
        /// API >> Get >>api/Reviews/getreviewcorevalue
        ///  Created by  Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getreviewcorevalue")]
        public async Task<IHttpActionResult> GetReviewCoreValue()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var corevaluedata = await _db.ReviewCoreValues.Where(x => x.IsActive && !x.IsDeleted).OrderByDescending(x => x.CreatedOn)
                    .Select(x => new HelperCoreValueModuel
                    {
                        ReviewCoreValueId = x.ReviewCoreValueId,
                        ReviewCoreValueName = x.ReviewCoreValueName,
                        Description = x.Description,
                        Count = _db.Behaviours.Where(y => y.ReviewCoreValueId == x.ReviewCoreValueId).Count(),
                        behaviour = _db.Behaviours.Where(y => y.ReviewCoreValueId == x.ReviewCoreValueId).Select(y => new BehaviaorCoreValueResponse
                        {
                            BehavioursId = y.BehaviourId,
                            useInRating = y.UseInRating,
                            Behaveour = y.Behaveour
                        }).ToList(),
                    }).ToListAsync();
                if (corevaluedata != null)
                {
                    res.Message = "CoreValue Successfully Get !";
                    res.Status = true;
                    res.Data = corevaluedata;
                }
                else
                {
                    res.Message = "Core Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getreviewcorevalue", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class HelperCoreValueModuel
        {
            public Guid ReviewCoreValueId { get; set; } = Guid.NewGuid();
            public string ReviewCoreValueName { get; set; }
            public string Description { get; set; }
            public int Count { get; set; }
            public List<BehaviaorCoreValueResponse> behaviour { get; set; }
        }
        public class BehaviaorCoreValueResponse
        {
            public Guid BehavioursId { get; set; } = Guid.NewGuid();
            public bool useInRating { get; set; }
            public string Behaveour { get; set; }
            public string Action { get; set; }
        }
        #endregion

        #region This Api Use Get All Review Core Value Id & Name
        /// <summary>
        /// API >> Get >>api/Reviews/getreviewcorevalueid
        ///  Created by  Mayank Prajapati On 24/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getreviewcorevalueid")]
        public async Task<IHttpActionResult> GetReviewCoreValueId()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var corevaluedata = await _db.ReviewCoreValues.Where(x => x.IsActive && !x.IsDeleted)
                    .Select(x => new HelperCoreValueIdModuel
                    {
                        ReviewCoreValueId = x.ReviewCoreValueId,
                        ReviewCoreValueName = x.ReviewCoreValueName,

                    }).ToListAsync();
                if (corevaluedata != null)
                {
                    res.Message = " Succesfully Get Review CoreValue Id !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = corevaluedata;
                }
                else
                {
                    res.Message = "Core Value Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        public class HelperCoreValueIdModuel
        {
            public Guid ReviewCoreValueId { get; set; } = Guid.NewGuid();
            public string ReviewCoreValueName { get; set; }
        }
        #endregion

        #region This Api Use Core Value Get By Id
        // <summary>
        // API >> Get >>api/Reviews/getcorevaluesbyid
        //  Created by  Mayank Prajapati On 3/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getcorevaluesbyid")]
        public async Task<IHttpActionResult> GetCoreValuesById(Guid ReviewCoreValueId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var corecdata = _db.ReviewCoreValues.Where(x => x.IsActive && !x.IsDeleted && x.ReviewCoreValueId == ReviewCoreValueId)
                  .Select(x => new HelperCoreModuel
                  {
                      ReviewCoreValueId = x.ReviewCoreValueId,
                      ReviewCoreValueName = x.ReviewCoreValueName,
                      Badge = x.Badge,
                      Description = x.Description,
                      behaviour = _db.Behaviours.Where(y => y.ReviewCoreValueId == x.ReviewCoreValueId).Select(y => new BehaviaorCoreResponse
                      {
                          BehavioursId = y.BehaviourId,
                          useInRating = y.UseInRating,
                          Behaveour = y.Behaveour
                      }).ToList(),
                  }).FirstOrDefault();
                if (corecdata != null)
                {
                    res.Message = " Successfully Review CoreValue !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = corecdata;
                }
                else
                {
                    res.Message = "Core Competency Data Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getcorevaluesbyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class HelperCoreModuel
        {
            public Guid ReviewCoreValueId { get; set; } = Guid.NewGuid();
            public string ReviewCoreValueName { get; set; }
            public string Badge { get; set; }
            public string Description { get; set; }

            public List<BehaviaorCoreResponse> behaviour { get; set; }
        }
        public class BehaviaorCoreResponse
        {
            public Guid BehavioursId { get; set; } = Guid.NewGuid();
            public bool useInRating { get; set; }
            public string Behaveour { get; set; }
            public string Action { get; set; }

        }
        #endregion

        #region This Api Use Update Review core value
        /// <summary>
        /// API >> Put >>api/Reviews/updatereviewcorevalue
        ///  Created by  Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updatereviewcorevalue")]
        public async Task<IHttpActionResult> UpdateReviewcorevalue(BehaviourCorevTypeResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            BehaviourCorevTypeResponse response = new BehaviourCorevTypeResponse();
            BehaviourDataHelper BehaviourList = new BehaviourDataHelper();
            try
            {
                var reviewcorevalue = await _db.ReviewCoreValues.FirstOrDefaultAsync(x => x.ReviewCoreValueId == model.ReviewCoreValueId);
                if (reviewcorevalue != null)
                {
                    reviewcorevalue.ReviewCoreValueName = model.ReviewCoreValueName;
                    reviewcorevalue.Description = model.Description;
                    reviewcorevalue.UpdatedBy = tokenData.employeeId;
                    reviewcorevalue.Badge = model.Badge;
                    reviewcorevalue.UpdatedOn = DateTime.Now;
                    _db.Entry(reviewcorevalue).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    foreach (var item in model.Behaviours)
                    {
                        var behavioures = _db.Behaviours.Where(x => x.BehaviourId == item.BehavioursId).FirstOrDefault();
                        {
                            behavioures.Behaveour = item.Behaveour;
                            behavioures.UseInRating = item.UseInRating;
                            behavioures.Action = item.Action;
                            behavioures.UpdatedBy = tokenData.employeeId;
                            behavioures.UpdatedOn = DateTime.Now;

                            _db.Entry(behavioures).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Updated Successfully  !";
                    res.Status = true;
                    res.Data = response;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Core Value Not Updated";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updatereviewcorevalue", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class BehaviourCorevTypeResponse
        {
            public Guid ReviewCoreValueId { get; set; }
            public string ReviewCoreValueName { get; set; }
            public string Description { get; set; }
            public string Badge { get; set; }

            public List<BehaviourDataHelper> Behaviours { get; set; }
        }
        public class BehaviourDataHelper
        {
            public Guid BehavioursId { get; set; }
            public string Behaveour { get; set; }
            public bool UseInRating { get; set; }
            public bool Action { get; set; }
        }
        #endregion

        #region This Api Use Deleted Review Competency
        // <summary>
        // API >> Delete >>api/Reviews/reviewcorevaluedelete
        //  Created by Mayank Prajapati On 3/11/2022
        // </summary>
        // <returns></returns>
        [HttpDelete]
        [Route("reviewcorevaluedelete")]
        public async Task<IHttpActionResult> ReviewCoreValueDelete(Guid reviewcorevalueId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewcorevalue = await _db.ReviewCoreValues.FirstOrDefaultAsync(x =>
                    x.ReviewCoreValueId == reviewcorevalueId && x.IsActive && !x.IsDeleted);
                if (reviewcorevalue != null)
                {
                    reviewcorevalue.IsDeleted = true;
                    reviewcorevalue.IsActive = false;
                    reviewcorevalue.DeletedBy = tokenData.employeeId;
                    reviewcorevalue.DeletedOn = DateTime.Now;

                    _db.Entry(reviewcorevalue).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Message = "Deleted Successfully  !";
                    res.Status = true;

                    return Ok(res);
                }
                else
                {
                    res.Status = false;
                    res.Message = "Review Core Value Not Found!!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/reviewcorevaluedelete", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region Api To Upload Core value Badge
        /// <summary>
        /// Created By Mayank Prajapati Date - 11/04/2022
        /// API >> Post >> api/Reviews/uploadbadgescorevalue
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadbadgescorevalue")]
        public async Task<UploadImageResponse> UploadBadgesCoreValue()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/reviewbadges/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\reviewbadges\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successful";
                            result.Status = true;
                            result.URL = FileUrl;
                            result.Path = path;
                            result.Extension = extension;
                            result.ExtensionType = extemtionType;
                        }
                        else
                        {
                            result.Message = "Only Select Image Format";
                            result.Status = false;
                        }
                    }
                    else
                    {
                        result.Message = "You Pass 0 Content";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }
        #endregion Api To Update Employee Profile Image

        #endregion

        #region This Api Use Review Employee Group

        #region This Api Use To Add Review Employee Group

        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >>Add >> api/Reviews/addreviewemployeegroup
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addreviewemployeegroup")]
        public async Task<IHttpActionResult> AddReviewEmployeeGroup(EmployeeReviewHelperGroup model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            EmployeeReviewHelperGroup response = new EmployeeReviewHelperGroup();
            List<EmplHelperModel> employee = new List<EmplHelperModel>();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var reviewemployeedata = await _db.ReviewsGroups.Where(x => x.CompanyId == tokenData.companyId
                    && x.ReviewGroupId == model.ReviewGroupId).FirstOrDefaultAsync();
                    if (reviewemployeedata != null)
                    {
                        EmployeeReview reviewpostdata = new EmployeeReview
                        {
                            ReviewGroupId = reviewemployeedata.ReviewGroupId,
                            ReportingManager = _db.Employee.Where(x => x.CompanyId == tokenData.companyId).Select(x => x.DisplayName).FirstOrDefault(),
                            CreatedBy = tokenData.employeeId,
                            CompanyId = tokenData.companyId,
                            OrgId = tokenData.orgId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false
                        };
                        _db.EmployeeReviews.Add(reviewpostdata);
                        await _db.SaveChangesAsync();

                        foreach (var item in model.EmplHelperModels)
                        {
                            AddMultipleEmployee empdata = new AddMultipleEmployee();
                            {
                                empdata.ReviewGroupId = reviewpostdata.ReviewGroupId;
                                empdata.EmployeeId = item.EmployeeId;
                                empdata.EmployeeName = _db.Employee.Where(z => z.EmployeeId == item.EmployeeId).Select(z => z.DisplayName).FirstOrDefault();
                                empdata.OrgId = tokenData.orgId;
                                empdata.CompanyId = tokenData.employeeId;
                                empdata.IsActive = true;
                                empdata.IsDeleted = false;
                                empdata.CreatedOn = DateTime.Now;
                                empdata.IsActive = true;
                                empdata.IsDeleted = false;
                            };
                            _db.AddMultipleEmployees.Add(empdata);
                            await _db.SaveChangesAsync();

                            res.Message = "Created Successfully  !";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                            res.Data = reviewpostdata;
                        }
                    }
                    else
                    {
                        res.Message = "Review Group Not Found";
                        res.Status = false;
                        res.Data = response;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addreviewemployeegroup", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class EmployeeReviewHelperGroup
        {

            public Guid ReviewGroupId { get; set; } = Guid.Empty;
            public List<EmplHelperModel> EmplHelperModels { get; set; }
        }

        public class EmplHelperModel
        {
            public int EmployeeId { get; set; }
        }
        #endregion

        #region This Api use to Review Employee Group Updated
        /// <summary>
        /// API >> Put >api/Reviews/updateemployeegroup
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>

        [HttpPut]
        [Route("updateemployeegroup")]
        public async Task<IHttpActionResult> UpdateEmployeeGroup(EmployeeReview model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Employee = await _db.EmployeeReviews.FirstOrDefaultAsync(x => x.ReviewEmployeeId == model.ReviewEmployeeId);
                if (Employee != null)
                {
                    Employee.EmployeeName = model.EmployeeName;
                    Employee.EmployeeNumber = model.EmployeeNumber;
                    Employee.ReportingManager = model.ReportingManager;
                    Employee.Department = model.Department;
                    Employee.Location = model.Location;
                    Employee.BusinessUnit = model.BusinessUnit;
                    Employee.PayGrade = model.PayGrade;
                    Employee.JoiningDate = model.JoiningDate;
                    Employee.UpdatedBy = tokenData.employeeId;
                    Employee.UpdatedOn = DateTime.Now;

                    _db.Entry(Employee).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Data Successfully Get !";
                    res.Status = true;
                    res.Data = Employee;
                }
                else
                {
                    res.Message = "Group Not Update";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updateemployeegroup", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Delete Review Employee Data 
        /// <summary>
        /// API >> Delete >>api/Reviews/deletereviewemployee
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("deletereviewemployee")]
        public async Task<IHttpActionResult> DeleteEmployee(ReviewEmployeeHelper model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                foreach (var item in model.RemoveEmployee)
                {
                    var reviewemployee = await _db.AddMultipleEmployees.Where(x =>
                    x.EmployeeId == item.EmployeeId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (reviewemployee != null)
                    {
                        reviewemployee.IsDeleted = true;
                        reviewemployee.IsActive = false;
                        reviewemployee.DeletedOn = DateTime.Now;
                        reviewemployee.DeletedBy = tokenData.employeeId;
                        _db.Entry(reviewemployee).State = EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Deleted Successfully  !";
                        res.Status = true;
                        res.Data = reviewemployee;
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "Review Employee Data Not Found!!";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/deletereviewemployee", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class ReviewEmployeeHelper
        {
            public List<EmplHelperModel> RemoveEmployee { get; set; }
        }
        #endregion

        #region This Api Use To Get Employee Review By id
        /// <summary>
        /// API >> Get >>api/Reviews/getbyemployeeid
        ///  Created by Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyemployeeid")]
        public async Task<IHttpActionResult> GetByEmployeeId(Guid ReviewGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.EmployeeReviews.Where(x => x.IsActive == true &&
                 x.IsDeleted == false && x.ReviewGroupId == ReviewGroupId).FirstOrDefaultAsync();
                if (employee != null)
                {
                    res.Message = "Review Employee List";
                    res.Status = true;
                    res.Data = employee;
                }
                else
                {
                    res.Message = "Remove Employee not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion Get all Group Detail

        #region This Api Use Get all Group Detail
        /// <summary>
        /// API >> Get >>api/Reviews/getallgroupdetails
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallgroupdetails")]
        public async Task<IHttpActionResult> GetAllGroupDetails()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var GroupDetail = await _db.ReviewsGroups.Where(x => x.IsActive == true &&
                x.IsDeleted == false && x.CompanyId == tokenData.companyId).
                    Select(x => new GroupDetail
                    {
                        GroupId = x.ReviewGroupId,
                        GroupName = x.ReviewGroupName
                    }).ToListAsync();

                res.Status = true;
                res.Message = "Review Group found";
                res.Data = GroupDetail;
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getallgroupdetails", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GroupDetail
        {
            public Guid GroupId { get; set; }
            public string GroupName { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get All Employee Data Count
        /// <summary>
        /// API >> Get >>api/Reviews/getallemployeecount
        ///  Created by Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeecount")]
        public async Task<IHttpActionResult> GetallEmployeeCount()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();
                if (employee.Count > 0)
                {
                    res.Message = "Employee Successfully Get !";
                    res.Status = true;
                    res.Data = employee.Count;
                }
                else
                {
                    res.Message = "Employee not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion Get all Group Detail

        #region This To Get All Review Employee
        /// <summary>
        /// API >> Get >>api/Reviews/getemployeedata
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeedata")]
        public async Task<IHttpActionResult> GetEmployeeData()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewemployee = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted
                    && x.CompanyId == tokenData.companyId).ToListAsync();
                if (reviewemployee != null)
                {
                    res.Message = "EmployeeData Successfully Get !";
                    res.Status = true;
                    res.Data = reviewemployee;
                }
                else
                {
                    res.Message = "Data Not Get";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getemployeedata", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region This Api Use Rating Scales

        #region This Api Is use To Add Rating Scales
        ///// <summary>
        ///// created by  Mayank Prajapati On 05/11/2022
        ///// Api >> Get >> api/Reviews/addratingscales
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addratingscales")]
        public async Task<IHttpActionResult> AddRatingScales(RatingScorEResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    RatingScales postdata = new RatingScales
                    {
                        RatingScalesName = model.RatingScalesName,
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.RatingScalies.Add(postdata);
                    await _db.SaveChangesAsync();

                    foreach (var item in model.RatingScoreScaleResponse)
                    {
                        RatingScore obj = new RatingScore();
                        obj.RatingScalesId = postdata.RatingScalesId;
                        obj.RatingScaleScore = item.RatingScaleScore;
                        obj.RatingLable = item.RatingLable;
                        obj.Description = item.Description;
                        _db.RatingScores.Add(obj);
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = postdata;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addratingscales", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class RatingScorEResponse
        {
            public string RatingScalesName { get; set; }
            public List<RatingScoreScaleResponse> RatingScoreScaleResponse { get; set; }
        }
        public class RatingScoreScaleResponse
        {
            public Guid RatingscalescoreId { get; set; }
            public int RatingScaleScore { get; set; }
            public string RatingLable { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region This Api Use Get All Rating Scales
        /// <summary>
        /// API >> Get >>api/Reviews/getallratingscales
        ///  Created by  Mayank Prajapati On 05/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallratingscales")]
        public async Task<IHttpActionResult> GetAllRatingScales()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ScoreData = _db.RatingScalies.Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                    .Select(x => new HelperScalesgModuel
                    {
                        RatingScalesId = x.RatingScalesId,
                        RatingScalesName = x.RatingScalesName,
                        Count = _db.RatingScores.Where(y => y.RatingScalesId == x.RatingScalesId).Count(),

                        RatingScore = _db.RatingScores.Where(y => y.RatingScalesId == x.RatingScalesId).Select(y => new ScoreResponse
                        {
                            RatingscalescoreId = y.RatingscalescoreId,
                            RatingScaleScore = y.RatingScaleScore,
                            Description = y.Description,
                            RatingLable = y.RatingLable
                        }).ToList().OrderBy(y => y.RatingScaleScore).ToList(),
                    }).ToList();
                if (ScoreData != null)
                {
                    res.Message = "Rating Scale Successfully Get !";
                    res.Status = true;
                    res.Data = ScoreData;
                }
                else
                {
                    res.Message = "Rating Scales Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getallratingscales", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class HelperScalesgModuel
        {
            public Guid RatingScalesId { get; set; }
            public string RatingScalesName { get; set; }
            public int Count { get; set; }
            public List<ScoreResponse> RatingScore { get; set; }
        }
        public class ScoreResponse
        {
            public Guid RatingscalescoreId { get; set; }
            public int RatingScaleScore { get; set; }
            public string RatingLable { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region This Api Use Get Rating Scales By Id
        // <summary>
        // API >> Get >>api/Reviews/getratingscalesbyid
        //  Created by  Mayank Prajapati On 05/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getratingscalesbyid")]
        public async Task<IHttpActionResult> GetRatingScalesById(Guid RatingScalesId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ratingscoredata = _db.RatingScalies.Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted && x.RatingScalesId == RatingScalesId)
                    .Select(x => new HelperRatingScalesgModuel
                    {
                        RatingScalesId = x.RatingScalesId,
                        RatingScalesName = x.RatingScalesName,
                        Count = _db.RatingScores.Where(y => y.RatingScalesId == x.RatingScalesId).Count(),
                        RatingRsScore = _db.RatingScores.Where(z => z.RatingScalesId == x.RatingScalesId).Select(z => new RatingScoreResponse
                        {
                            RatingscalescoreId = z.RatingscalescoreId,
                            RatingscaleId = (Guid)z.RatingScalesId,
                            RatingScaleScore = z.RatingScaleScore,
                            Description = z.Description,
                            RatingLable = z.RatingLable
                        }).ToList().OrderByDescending(y => y.RatingScaleScore).ToList(),
                    }).FirstOrDefault();
                if (ratingscoredata != null)
                {
                    res.Message = " Successfully Get Rating Scales !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = ratingscoredata;
                }
                else
                {
                    res.Message = "Rating Scales Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getratingscalesbyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class HelperRatingScalesgModuel
        {
            public Guid RatingScalesId { get; set; }
            public string RatingScalesName { get; set; }
            public int Count { get; set; }
            public List<RatingScoreResponse> RatingRsScore { get; set; }
        }
        public class RatingScoreResponse
        {
            public Guid RatingscalescoreId { get; set; }
            public Guid RatingscaleId { get; set; }
            public int RatingScaleScore { get; set; }
            public string RatingLable { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region This Api Use Update Rating Scales
        /// <summary>
        /// API >> Get >>api/Reviews/updateratingscales
        ///  Created by  Mayank Prajapati On 05/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updateratingscales")]
        public async Task<IHttpActionResult> UpdateRatingScales(RatingScorEResponseData model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            RatingScorEResponseData response = new RatingScorEResponseData();
            RatingScoreScaleResponse RatingList = new RatingScoreScaleResponse();
            try
            {
                var Update = await _db.RatingScalies.FirstOrDefaultAsync(x => x.RatingScalesId == model.RatingScalesId);
                if (Update != null)
                {
                    Update.RatingScalesName = model.RatingScalesName;
                    Update.UpdatedBy = tokenData.employeeId;
                    Update.UpdatedOn = DateTime.Now;
                    _db.Entry(Update).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    foreach (var item in model.RatingScoreScaleResponseData)
                    {
                        var Rating = _db.RatingScores.Where(x => x.RatingscalescoreId == item.RatingscalescoreId).FirstOrDefault();
                        {
                            Rating.RatingScalesId = Update.RatingScalesId;
                            Rating.RatingScaleScore = item.RatingScaleScore;
                            Rating.RatingLable = item.RatingLable;
                            Rating.Description = item.Description;
                            Rating.UpdatedBy = tokenData.employeeId;
                            Rating.UpdatedOn = DateTime.Now;
                            _db.Entry(Rating).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Data = response;
                    res.Status = true;
                    res.Message = " Updated Rating Scales";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/updateratingscales", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class RatingScorEResponseData
        {
            public Guid RatingScalesId { get; set; }
            public string RatingScalesName { get; set; }
            public List<RatingScoreScaleResponseData> RatingScoreScaleResponseData { get; set; }
        }
        public class RatingScoreScaleResponseData
        {
            public Guid RatingscalescoreId { get; set; }
            public int RatingScaleScore { get; set; }
            public string RatingLable { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region This Api Use Rating Scales Delete
        /// <summary>
        /// API >> Delete >>api/Reviews/ratingscalesdeleted
        ///  Created by  Mayank Prajapati On 05/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ratingscalesdeleted")]
        public async Task<IHttpActionResult> RatingScalesDeleted(GetResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var question = await _db.RatingScalies.FirstOrDefaultAsync(x =>
                     x.IsActive && !x.IsDeleted && x.RatingScalesId == model.RatingScalesId);
                if (question != null)
                {
                    question.IsDeleted = true;
                    question.IsActive = false;

                    _db.Entry(question).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Rating Scales Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Rating Scales Not Found!!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/ratingscalesdeleted", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion)
        public class GetResponse
        {
            public Guid RatingScalesId { get; set; }
        }
        #endregion  This Api Use Rating Scales

        #region This Api Use Reviews Question

        #region This Api Is use To Add Review Question
        ///// <summary>
        ///// created by  Mayank Prajapati On 05/11/2022
        ///// Api >> post >> api/Reviews/addreviewquestion
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addreviewquestion")]
        public async Task<IHttpActionResult> AddReviewQuestion(ReviewHelperClass model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            OptionTypeResponse response = new OptionTypeResponse();
            List<OptionSelect> ResType = new List<OptionSelect>();
            try
            {
                if (model != null)
                {
                    ReviewQuestion postdata = new ReviewQuestion();
                    postdata.ReviewGroupId = model.ReviewGroupId;
                    postdata.ReviewEmployeeId = model.ReviewEmployeeId;
                    postdata.ReviewQuestionName = model.ReviewQuestionName;
                    postdata.Reviews = model.Reviews;
                    postdata.OptionType = model.OptionType;
                    postdata.ReviewsName = Enum.GetName(typeof(ReviewsTypeConstants), model.Reviews);
                    postdata.OptionTypeName = Enum.GetName(typeof(OptionTypeConstants), model.OptionType);
                    postdata.CreatedBy = tokenData.employeeId;
                    postdata.CompanyId = tokenData.companyId;
                    postdata.OrgId = tokenData.orgId;
                    postdata.CreatedOn = DateTime.Now;
                    postdata.IsActive = true;
                    postdata.IsDeleted = false;
                    _db.ReviewQuestions.Add(postdata);
                    await _db.SaveChangesAsync();

                    foreach (var item in model.OptionData)
                    {
                        if (item != null)
                        {
                            OptionSelect RespType = new OptionSelect();
                            RespType.ReviewQuestionId = postdata.ReviewQuestionId;
                            RespType.OptionId = item.OptionId;
                            RespType.Option = item.Option;
                            RespType.IsActive = true;
                            RespType.IsDeleted = false;
                            RespType.CreatedBy = tokenData.employeeId;
                            RespType.CreatedOn = DateTime.Now;
                            RespType.CompanyId = tokenData.companyId;
                            RespType.OrgId = tokenData.orgId;
                            _db.OptionSelects.Add(RespType);
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Question Not Add";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addreviewquestion", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class ReviewHelperClass
        {
            public Guid ReviewGroupId { get; set; }
            public Guid ReviewEmployeeId { get; set; }
            public string ReviewQuestionName { get; set; }
            public OptionTypeConstants OptionType { get; set; }
            public string OptionTypeName { get; set; }
            public ReviewsTypeConstants Reviews { get; set; }
            public string ReviewsName { get; set; }
            public List<OptionSelectData> OptionData { get; set; }
        }
        public class OptionSelectData
        {
            public int OptionId { get; set; }
            public string Option { get; set; }
        }
        #endregion

        #region This Api Use Get All Review Question
        // <summary>
        // API >> Get >>api/Reviews/getallreviewquestion
        //  Created by  Mayank Prajapati On 08/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getallreviewquestion")]
        public async Task<ResponseBodyModel> GetAllReviewQuestion()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewquestion = await _db.ReviewQuestions.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                var review = reviewquestion
                    .Select(x => new ReviewQucstionHelperClass
                    {
                        ReviewQuestionId = x.ReviewQuestionId,
                        ReviewQuestionName = x.ReviewQuestionName,
                        OptionTypeName = x.OptionTypeName,
                        ReviewsName = x.ReviewsName,
                        OptionseletData = _db.OptionSelects.Where(y => y.ReviewQuestionId == x.ReviewQuestionId).Select(y => new OptionSelectHelperData
                        {
                            OptionId = y.OptionId,
                            Option = y.Option,
                        }).ToList(),
                    }).ToList();
                if (review != null)
                {
                    res.Message = "Get Reviews Quetion";
                    res.Status = true;
                    res.Data = review;
                }
                else
                {
                    res.Message = "Get Question Data Not Found";
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
        public class ReviewQucstionHelperClass
        {
            public Guid ReviewQuestionId { get; set; }
            public string ReviewQuestionName { get; set; }
            public string OptionTypeName { get; set; }
            public string ReviewsName { get; set; }
            public List<OptionSelectHelperData> OptionseletData { get; set; }
        }
        public class OptionSelectHelperData
        {
            public int OptionId { get; set; }
            public string Option { get; set; }
        }
        #endregion

        #region This Api Use Get All Review Question By Id
        // <summary>
        // API >> Get >>api/Reviews/getallreviewsquestionbyid
        //  Created by  Mayank Prajapati On 08/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getallreviewsquestionbyid")]
        public async Task<ResponseBodyModel> GetAllReviewQuestionById(Guid reviewQuestionId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var reviewoquestion = await _db.ReviewQuestions.Where(x => x.IsActive && !x.IsDeleted && x.ReviewQuestionId == reviewQuestionId)
                 .Select(x => new ReviewQuestionDataHelperClass
                 {
                     ReviewQuestionId = x.ReviewQuestionId,
                     ReviewQuestionName = x.ReviewQuestionName,
                     OptionTypeName = x.OptionTypeName,
                     OptionType = x.OptionType,
                     Reviews = x.Reviews,
                     ReviewsName = x.ReviewsName,
                     OptionData = _db.OptionSelects.Where(y => y.ReviewQuestionId == x.ReviewQuestionId).Select(y => new OptionSelectHelperModelData
                     {
                         OptionId = y.OptionId,
                         Option = y.Option,

                     }).ToList(),
                 }).FirstOrDefaultAsync();
                if (reviewoquestion != null)
                {
                    res.Message = "Get Review Question Data";
                    res.Status = true;
                    res.Data = reviewoquestion;
                }
                else
                {
                    res.Message = " Review Question Data Not Found";
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
        public class ReviewQuestionDataHelperClass
        {
            public Guid ReviewQuestionId { get; set; }
            public string ReviewQuestionName { get; set; }
            public string OptionTypeName { get; set; }
            public OptionTypeConstants OptionType { get; set; }
            public ReviewsTypeConstants Reviews { get; set; }
            public string ReviewsName { get; set; }
            public List<OptionSelectHelperModelData> OptionData { get; set; }
        }
        public class OptionSelectHelperModelData
        {
            public int OptionId { get; set; }
            public string Option { get; set; }
        }
        #endregion

        #region This Api Use Get Option Type
        // <summary>
        // API >> Get >>api/Reviews/getoption
        //  Created by  Mayank Prajapati On 07/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getoption")]
        public async Task<ResponseBodyModel> GetOption()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var OptionData = Enum.GetValues(typeof(OptionTypeConstants))
                    .Cast<OptionTypeConstants>()
                    .Select(x => new OptionTypeModel
                    {
                        OptionTypeId = (int)x,
                        OptionType = Enum.GetName(typeof(OptionTypeConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Option Data exist";
                res.Status = true;
                res.Data = OptionData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class OptionTypeModel
        {
            public int OptionTypeId { get; set; }
            public string OptionType { get; set; }
        }
        #endregion

        #region This Api Use Get Reviews Type
        // <summary>
        // API >> Get >>api/Reviews/getreviewstype
        //  Created by  Mayank Prajapati On 08/11/2022
        // </summary>
        // <returns></returns>
        [HttpGet]
        [Route("getreviewstype")]
        public async Task<ResponseBodyModel> GetReviewsType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ReviewsData = Enum.GetValues(typeof(ReviewsTypeConstants))
                    .Cast<ReviewsTypeConstants>()
                    .Select(x => new ReviewsTypeModel
                    {
                        ReviewsTypeId = (int)x,
                        ReviewsType = Enum.GetName(typeof(ReviewsTypeConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Option Data exist";
                res.Status = true;
                res.Data = ReviewsData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class ReviewsTypeModel
        {
            public int ReviewsTypeId { get; set; }
            public string ReviewsType { get; set; }
        }
        #endregion

        #region This Api Use Update Review Question
        // <summary>
        // API >> Get >>api/Reviews/updatereviewQuestion
        //  Created by  Mayank Prajapati On 05/11/2022
        // </summary>
        // <returns></returns>
        //
        [HttpPut]
        [Route("updatereviewQuestion")]
        public async Task<ResponseBodyModel> UpdateReviewQuestion(ReviewQuestionData model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            ReviewQuestionData response = new ReviewQuestionData();
            List<OptionSelectData> BehaviourList = new List<OptionSelectData>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var question = await _db.ReviewQuestions.FirstOrDefaultAsync(x => x.ReviewQuestionId == model.ReviewQuestionId);
                if (question != null)
                {
                    question.ReviewQuestionName = model.ReviewQuestionName;
                    question.ReviewsName = Enum.GetName(typeof(ReviewsTypeConstants), model.Reviews);
                    question.Reviews = model.Reviews;
                    question.OptionType = model.OptionType;
                    question.OptionTypeName = Enum.GetName(typeof(OptionTypeConstants), model.OptionType);
                    question.UpdatedBy = claims.employeeId;
                    question.UpdatedOn = DateTime.Now;
                    question.IsActive = true;
                    question.IsDeleted = false;
                    _db.Entry(question).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    foreach (var item in model.optionData)
                    {
                        var option = _db.OptionSelects.Where(x => x.OptionId == item.OptionId).FirstOrDefault();
                        {
                            option.Option = item.Option;
                            option.UpdatedBy = claims.employeeId;
                            option.UpdatedOn = DateTime.Now;
                            option.IsActive = true;
                            option.IsDeleted = false;

                            _db.Entry(option).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Data = response;
                    res.Status = true;
                    res.Message = "Review Question Updated";
                }
                else
                {
                    res.Message = "Review QuestionNot Update";
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
        public class ReviewQuestionData
        {
            public Guid ReviewQuestionId { get; set; }
            public string ReviewQuestionName { get; set; }
            public OptionTypeConstants OptionType { get; set; }
            public ReviewsTypeConstants Reviews { get; set; }
            public string ReviewsName { get; set; }
            public List<OptionSelectHelper> optionData { get; set; }
        }
        public class OptionSelectHelper
        {
            public Guid ReviewQuestionId { get; set; }
            public int OptionId { get; set; }
            public string Option { get; set; }
        }
        #endregion

        #region This Api Use Review Question Delete
        // <summary>
        // API >> Delete >>api/Reviews/reviewemployeedeleted
        //  Created by  Mayank Prajapati On 05/11/2022
        // </summary>
        // <returns></returns>
        [HttpDelete]
        [Route("reviewemployeedeleted")]
        public async Task<ResponseBodyModel> ReviewQuestionDeleted(Guid ReviewQuestionId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var question = await _db.ReviewQuestions.FirstOrDefaultAsync(x =>
                    x.ReviewQuestionId == ReviewQuestionId);
                if (question != null)
                {
                    question.IsActive = false;
                    question.IsDeleted = true;
                    question.DeletedOn = DateTime.Now;
                    question.DeletedBy = claims.companyId;
                    var ticEmp = _db.OptionSelects.Where(x => x.ReviewQuestionId == question.ReviewQuestionId).ToList();
                    foreach (var item in ticEmp)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;
                        item.DeletedBy = claims.employeeId;
                        item.DeletedOn = DateTime.Now;

                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    _db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Employee Data Not Found!!";
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
        public class OptionTypeResponse
        {
            public ReviewQuestion ReviewQuestion { get; set; }
            public List<OptionSelect> OptionSelects { get; set; }
        }
        #endregion

        #region This Api Use Employee FeedBack Request

        #region This Api Use Get Feed Back Created Employee Data
        /// <summary>
        /// API >> Get >>api/Reviews/getfeedbackfrom
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getfeedbackfrom")]
        public async Task<IHttpActionResult> GetFeedBAckFrom()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbdata = await (from rfd in _db.RequestFeedbacks
                                       join fbf in _db.FeedBackFroms on rfd.RequestFeedbackId equals fbf.RequestFeedbackId
                                       where (rfd.IsActive && !rfd.IsDeleted && fbf.RequestFeedbackFrom == tokenData.employeeId && fbf.IsFeedbackGiven == true)
                                       select new GetFeedBAckHelperModel
                                       {
                                           RequestFeedbackId = rfd.RequestFeedbackId,
                                           EmployeeName = _db.Employee.Where(v => v.EmployeeId == rfd.RequestFeedbackFor).Select(v => v.DisplayName).FirstOrDefault(),
                                           EmployeeNameFrom = _db.Employee.Where(v => v.EmployeeId == fbf.RequestFeedbackFrom).Select(v => v.DisplayName).FirstOrDefault(),
                                           RequestBy = _db.Employee.Where(z => z.EmployeeId == rfd.CreatedBy).Select(z => z.DisplayName).FirstOrDefault(),
                                           FeebBackMessage = fbf.FeebBackMessage,
                                           CreateDate = DateTime.Now
                                       }).ToListAsync();
                if (feedbdata != null)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = feedbdata;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = feedbdata;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getfeedbackfrom", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetFeedBAckHelperModel
        {
            public int RequestFeedbackId { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeNameFrom { get; set; }
            public string FeebBackMessage { get; set; }
            public string RequestBy { get; set; }
            public DateTime CreateDate { get; set; }
        }
        #endregion Get all Group Detail

        #region Api To Add Requested User Feedback
        ///// <summary>
        ///// Created By Mayank Prajapati on 22/11/2022
        ///// API >> Post >> api/Reviews/addfeedbackRequest
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("addfeedbackRequest")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddFeedbackRequest(FeedBackRequestRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    RequestFeedback feedbackobj = new RequestFeedback();
                    feedbackobj.RequestFeedbackFor = model.RequestFeedBackFor;
                    feedbackobj.RequestMessage = model.RequestMessage;
                    feedbackobj.CompanyId = tokenData.companyId;
                    feedbackobj.OrgId = tokenData.orgId;
                    feedbackobj.CreatedBy = tokenData.employeeId;

                    _db.RequestFeedbacks.Add(feedbackobj);
                    await _db.SaveChangesAsync();

                    foreach (var item in model.FeedbackFrom)
                    {
                        FeedBackResponse FBdata = new FeedBackResponse
                        {
                            RequestFeedbackId = feedbackobj.RequestFeedbackId,
                            RequestFeedbackFrom = item,
                            IsFeedbackGiven = false,
                        };
                        _db.FeedBackFroms.Add(FBdata);
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    res.Message = "Data not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addfeedbackRequest", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class FeedBackRequestRequest
        {
            public int RequestFeedBackFor { get; set; }
            public string RequestMessage { get; set; }
            public List<int> FeedbackFrom { get; set; }
        }
        #endregion

        #region Api To Add  User Feedback Responce
        ///// <summary>
        ///// Created By Mayank Prajapati on 22/11/2022
        ///// API >> Post >> api/Reviews/updatefeedback
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("updatefeedback")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> UpdateFeedbackRequest(FeedBackHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedback = await _db.RequestFeedbacks.Where(x => x.RequestFeedbackId == model.RequestFeedbackId).FirstOrDefaultAsync();
                if (feedback != null)
                {
                    feedback.RequestFeedbackFor = feedback.RequestFeedbackFor;
                    feedback.RequestMessage = feedback.RequestMessage;
                    feedback.UpdatedBy = claims.employeeId;
                    feedback.UpdatedOn = DateTime.Now;
                    feedback.IsDeleted = false;
                    feedback.IsActive = true;

                    _db.Entry(feedback).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    var feedbackfrom = await _db.FeedBackFroms.Where(x => x.RequestFeedbackId == feedback.RequestFeedbackId).FirstOrDefaultAsync();
                    if (feedbackfrom != null)
                    {
                        feedbackfrom.RequestFeedbackId = feedback.RequestFeedbackId;
                        feedbackfrom.RequestFeedbackFrom = feedbackfrom.RequestFeedbackFrom;
                        feedbackfrom.FeebBackMessage = model.FeebBackMessage;
                        feedbackfrom.IsFeedbackGiven = true;
                        feedbackfrom.UpdatedBy = claims.employeeId;
                        feedbackfrom.UpdatedOn = DateTime.Now;
                        feedbackfrom.IsDeleted = false;
                        feedbackfrom.IsActive = true;

                        _db.Entry(feedbackfrom).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "FeedBack Request Added";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Data not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        public class FeedBackHelper
        {
            public int RequestFeedbackId { get; set; }
            public string FeebBackMessage { get; set; }
        }
        #endregion

        #region This Api Use Get  Responce Feed BAck Data 
        /// <summary>
        /// API >> Get >>api/Reviews/getalldataresponcefeedback
        ///  Created by  Mayank Prajapati On 23/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldataresponcefeedback")]
        public async Task<IHttpActionResult> GetAllDataResponceFeedBack()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var Claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var requestfeedback = await (from rf in _db.RequestFeedbacks
                                             join fr in _db.FeedBackFroms on rf.RequestFeedbackId equals fr.RequestFeedbackId
                                             where !rf.IsDeleted && rf.IsActive && rf.CompanyId == Claims.companyId
                                             select new FeedBackResponceModel()
                                             {
                                                 RequestFeedbackFor = _db.Employee.Where(x => x.EmployeeId == rf.RequestFeedbackFor).Select(x => x.DisplayName).FirstOrDefault(),
                                                 RequestMessage = rf.RequestMessage,
                                                 CreatedDate = DateTime.Now,
                                                 RequestFeedbackId = fr.RequestFeedbackId,
                                                 RequestFeedbackFrom = _db.Employee.Where(x => x.EmployeeId == fr.RequestFeedbackFrom).Select(x => x.DisplayName).FirstOrDefault(),
                                                 FeebBackMessage = fr.FeebBackMessage
                                             }).ToListAsync();
                if (requestfeedback != null)
                {
                    res.Message = "FeedBack Request Successfully Get !";
                    res.Status = true;
                    res.Data = requestfeedback;
                }
                else
                {
                    res.Message = "FeedBack Request Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getalldataresponcefeedback", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class FeedBackResponceModel
        {
            public string RequestFeedbackFor { get; set; }
            public string RequestMessage { get; set; }
            public DateTime CreatedDate { get; set; }
            public int RequestFeedbackId { get; set; }
            public string RequestFeedbackFrom { get; set; }
            public string FeebBackMessage { get; set; }
        }
        #endregion

        #region This Api Use Get  Requested Feed BAck Data 
        /// <summary>
        /// API >> Get >>api/Reviews/getalldatafeedbackrequest
        ///  Created by  Mayank Prajapati On 23/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldatafeedbackrequest")]
        public async Task<IHttpActionResult> GetAllDataFeedBackRequest()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var Claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var requestfeedback = await (from rf in _db.RequestFeedbacks
                                             join fr in _db.FeedBackFroms on rf.RequestFeedbackId equals fr.RequestFeedbackId
                                             where !rf.IsDeleted && rf.IsActive && rf.CompanyId == Claims.companyId
                                                && fr.RequestFeedbackFrom == Claims.employeeId
                                             select new FeedBackRequestRequestHelper
                                             {
                                                 RequestFeedBack = _db.Employee.Where(x => x.EmployeeId == rf.RequestFeedbackFor).Select(x => x.DisplayName).FirstOrDefault(),
                                                 RequestMessage = rf.RequestMessage,
                                                 CreatedDate = DateTime.Now,
                                                 RequestedBy = _db.Employee.Where(x => x.EmployeeId == rf.CreatedBy).Select(x => x.DisplayName).FirstOrDefault(),
                                                 FeedbackFrom = _db.Employee.Where(x => x.EmployeeId == fr.RequestFeedbackFrom).Select(x => x.DisplayName).FirstOrDefault(),
                                             }).ToListAsync();
                if (requestfeedback != null)
                {
                    res.Message = "Get FeedBack Request Found";
                    res.Status = true;
                    res.Data = requestfeedback;
                }
                else
                {
                    res.Message = "FeedBack Request Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        public class FeedBackRequestRequestHelper
        {
            public string RequestFeedBack { get; set; }
            public string RequestMessage { get; set; }
            public DateTime CreatedDate { get; set; }
            public string RequestedBy { get; set; }
            public string FeedbackFrom { get; set; }
        }
        #endregion

        #region This Api Use Get requested Employee Id Data 
        /// <summary>
        /// API >> Get >>api/Reviews/getemployeealldata
        ///  Created by  Mayank Prajapati On 23/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeealldata")]
        public async Task<IHttpActionResult> GetEmployeeallData()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getemplydata = await _db.Employee
                    .Where(x => x.EmployeeId == x.EmployeeId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .ToListAsync();
                if (getemplydata != null)
                {
                    res.Message = "Employee Data Found";
                    res.Status = true;
                    res.Data = getemplydata;
                }
                else
                {
                    res.Message = " Data not found ";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/Reviews/getemployeealldata", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Get All FeedBack Count 
        /// <summary>
        /// API >> Get >>api/Reviews/getallfeedbackcount
        ///  Created by Mayank Prajapati On 28/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallfeedbackcount")]
        public async Task<IHttpActionResult> GetallFeedBackCount()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var FeedBack = await _db.RequestFeedbacks.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (FeedBack.Count > 0)
                {
                    res.Message = "FeedBack List";
                    res.Status = true;
                    res.Data = FeedBack.Count;
                }
                else
                {
                    res.Message = "FeedBacke not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getallfeedbackcount", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion Get all Group Detail

        #region This Api Use Get Feed Back From Employee
        /// <summary>
        /// API >> Get >>api/Reviews/getfeedbackfromemployee
        ///  Created by Mayank Prajapati On 14/10/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getfeedbackfromemployee")]
        public async Task<IHttpActionResult> GetFeedBAckFromEmployee()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var fbdata = await (from rfd in _db.RequestFeedbacks
                                    join fbf in _db.FeedBackFroms on rfd.RequestFeedbackId equals fbf.RequestFeedbackId
                                    where (rfd.IsActive && !rfd.IsDeleted && fbf.RequestFeedbackFrom == tokenData.employeeId && fbf.IsFeedbackGiven == false)
                                    select new GetFeedBAckFromEmployeemodel
                                    {
                                        RequestFeedbackId = rfd.RequestFeedbackId,
                                        EmployeeName = _db.Employee.Where(v => v.EmployeeId == rfd.RequestFeedbackFor).Select(v => v.DisplayName).FirstOrDefault(),
                                        EmployeeNameFrom = _db.Employee.Where(v => v.EmployeeId == fbf.RequestFeedbackFrom).Select(v => v.DisplayName).FirstOrDefault(),
                                        Count = _db.RequestFeedbacks.Where(y => y.RequestFeedbackId == fbf.RequestFeedbackFrom).Count(),
                                        RequestBy = _db.Employee.Where(z => z.EmployeeId == rfd.CreatedBy).Select(z => z.DisplayName).FirstOrDefault(),
                                        RequestMessage = rfd.RequestMessage,
                                        CreateDate = DateTime.Now
                                    }).ToListAsync();
                if (fbdata != null)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = fbdata;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getfeedbackfromemployee", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetFeedBAckFromEmployeemodel
        {
            public int RequestFeedbackId { get; set; }
            public string EmployeeName { get; set; }
            public bool IsFeedbackGiven { get; set; }
            public int Count { get; set; }
            public string EmployeeNameFrom { get; set; }
            public string RequestMessage { get; set; }
            public string RequestBy { get; set; }
            public DateTime CreateDate { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get Feed Back Requested FeedBack Id
        /// <summary>
        /// API >> Get >>api/Reviews/getrequestedfeedbackid
        ///  Created by  Mayank Prajapati On 14/10/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getrequestedfeedbackid")]
        public async Task<IHttpActionResult> GetRequestedFeedBackId(int requestfeedbackid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var fbdata = await (from rfd in _db.RequestFeedbacks
                                    join fbf in _db.FeedBackFroms on rfd.RequestFeedbackId equals fbf.RequestFeedbackId
                                    where (rfd.IsActive && !rfd.IsDeleted && fbf.RequestFeedbackId == requestfeedbackid && fbf.RequestFeedbackFrom == claims.employeeId)
                                    select new RequestedFeedBackIdModel
                                    {
                                        EmployeeName = _db.Employee.Where(v => v.EmployeeId == rfd.RequestFeedbackFor).Select(v => v.DisplayName).FirstOrDefault(),
                                        EmployeeNameFrom = _db.Employee.Where(v => v.EmployeeId == fbf.RequestFeedbackFrom).Select(v => v.DisplayName).FirstOrDefault(),
                                        RequestBy = _db.Employee.Where(z => z.EmployeeId == rfd.CreatedBy).Select(z => z.DisplayName).FirstOrDefault(),
                                        RequestMessage = rfd.RequestMessage,
                                        CreateDate = DateTime.Now
                                    }).ToListAsync();
                if (fbdata != null)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = fbdata;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = fbdata;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getrequestedfeedbackid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class RequestedFeedBackIdModel
        {
            public string EmployeeName { get; set; }
            public string EmployeeNameFrom { get; set; }
            public string RequestMessage { get; set; }
            public string RequestBy { get; set; }
            public DateTime CreateDate { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get All FeedBack Count 
        /// <summary>
        /// API >> Get >> api/Reviews/getrequestfeedbackcount
        ///  Created by Mayank Prajapati On 28/10/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getrequestfeedbackcount")]
        public async Task<IHttpActionResult> GetaRequestFeedBackCount()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var FeedBack = _db.FeedBackFroms.Where(x => x.IsActive && !x.IsDeleted && x.RequestFeedbackFrom == claims.employeeId && x.IsFeedbackGiven == false).ToList().Count();

                if (FeedBack != null)
                {
                    res.Message = "FeedBack Count";
                    res.Status = true;
                    res.Data = new
                    {
                        Count = FeedBack,
                    };
                }
                else
                {
                    res.Message = "FeedBack Count not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/getrequestfeedbackcount", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class RequestcountHelper
        {
            public int Count { get; set; }
        }
        #endregion Get all Group Detail

        #endregion
    }
}
