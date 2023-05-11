using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.HiringQuestion;
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

namespace AspNetIdentity.WebApi.Controllers.HiringQuestion
{
    /// <summary>
    /// Created By Ankit Jain on 09-02-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/question")]
    public class HiringQuestionBankController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API FOR Add Question In Job
        /// <summary>
        /// Created By ankit Jain On 09-02-2023
        /// API >> Post >> api/question/addjobquestion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("addjobquestion")]
        [HttpPost]
        public async Task<IHttpActionResult> AddJobQuestion(AddJobQuestions model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var jobData = await _db.JobPosts.FirstOrDefaultAsync
                                (x => x.JobPostId == model.JobId && x.IsActive &&
                                !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (jobData != null)
                    {
                        HiringQuestionsBank obj = new HiringQuestionsBank
                        {
                            JobId = model.JobId,
                            QuesionTitle = model.QuesionTitle,
                            ShowQuestionOnPortal = model.ShowQuestionOnPortal,
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        };

                        _db.HiringQuestionsBanks.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = "Add Question In Job Added!";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Data Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = jobData;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/question/addjobquestion | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class AddJobQuestions
        {
            public int JobId { get; set; } = 0;
            public string QuesionTitle { get; set; } = string.Empty;
            public bool ShowQuestionOnPortal { get; set; } = false;
        }
        #endregion

        #region This Api To Get All Quesions In Job
        /// <summary>
        /// Created By Ankit Jain on 09-02-2023
        /// API >> Get >> api/question/getalljobquesions
        /// </summary>
        [HttpGet]
        [Route("getalljobquesions")]
        public async Task<IHttpActionResult> GetAllJobQuesions(int jobId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobData = await (from j in _db.JobPosts
                                     join hq in _db.HiringQuestionsBanks on
                                     j.JobPostId equals hq.JobId
                                     where hq.IsActive
                                     && !hq.IsDeleted && hq.CompanyId == tokenData.companyId
                                     && hq.JobId == jobId
                                     select new GetAllQuesions
                                     {
                                         QuesionsId = hq.QuesionsId,
                                         JobId = hq.JobId,
                                         QuesionTitle = hq.QuesionTitle,
                                         ShowQuestionOnPortal = hq.ShowQuestionOnPortal
                                     }).ToListAsync();
                if (jobData.Count > 0)
                {
                    res.Message = "Quesions List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = jobData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = jobData;
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/question/getalljobquesions | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        public class GetAllQuesions : AddJobQuestions
        {
            public Guid QuesionsId { get; set; } = Guid.NewGuid();
        }
        #endregion

        #region API FOR UPDATE Job Quesions 
        /// <summary>
        /// Created By ankit Jain On 09-02-2023
        /// API >> Post >> api/question/updatejobquesion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updatejobquesion")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateJobQuesions(GetAllQuesions model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var jobQuesion = await _db.HiringQuestionsBanks
                              .FirstOrDefaultAsync(x => x.QuesionsId
                               == model.QuesionsId && x.IsActive &&
                               !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (jobQuesion != null)
                    {
                        jobQuesion.JobId = model.JobId;
                        jobQuesion.QuesionTitle = model.QuesionTitle;
                        jobQuesion.ShowQuestionOnPortal = model.ShowQuestionOnPortal;
                        jobQuesion.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                        jobQuesion.UpdatedBy = tokenData.employeeId;

                        _db.Entry(jobQuesion).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Updated Successfully  !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = jobQuesion;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Data Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = jobQuesion;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/question/updatejobquesion | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }
        #endregion

        #region API TO DELETE Job Question
        /// <summary>
        /// Created By Ankit Jain On 02/02/2023
        /// API >> POST >> api/question/deletequesions
        /// </summary>
        /// <param name="quesionID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletequesions")]
        public async Task<IHttpActionResult> DeleteQuesions(Guid quesionID)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobQuestion = await _db.HiringQuestionsBanks
                          .FirstOrDefaultAsync(x => x.QuesionsId == quesionID
                           && x.IsActive && !x.IsDeleted);
                if (jobQuestion == null)
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    jobQuestion.IsActive = false;
                    jobQuestion.IsDeleted = true;
                    jobQuestion.DeletedBy = tokenData.employeeId;
                    jobQuestion.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(jobQuestion).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Job Questions Deleted";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = jobQuestion;
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/goalpermission/deletepermission | " +
                    "Quesion ID : " + quesionID + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region This Api To Get All Quesions In Job By Id
        /// <summary>
        /// Created By Ankit Jain on 09-02-2023
        /// API >> Get >> api/question/getjobquesionsbyid
        /// <param name="quesionID"></param>
        /// <returns></returns>
        ///  </summary>
        [HttpGet]
        [Route("getjobquesionsbyid")]
        public async Task<IHttpActionResult> GetjobQuesionsById(Guid quesionId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobData = await (from hq in _db.HiringQuestionsBanks
                                     where hq.IsActive && !hq.IsDeleted
                                     && hq.CompanyId == tokenData.companyId
                                     && hq.QuesionsId == quesionId
                                     select new GetAllQuesions
                                     {
                                         JobId = hq.JobId,
                                         QuesionTitle = hq.QuesionTitle,
                                         ShowQuestionOnPortal = hq.ShowQuestionOnPortal
                                     }).FirstOrDefaultAsync();
                if (jobData != null)
                {
                    res.Message = "Quesions Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = jobData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = jobData;
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/question/getjobquesionsbyid | " +
                    "QuesionID : " + quesionId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api To Get All Quesions And Ans In Job By Id
        /// <summary>
        /// Created By Ankit Jain on 09-02-2023
        /// API >> Get >> api/question/getansandquestionsbyid
        /// <param name="quesionID"></param>
        /// <returns></returns>
        ///  </summary>
        [HttpGet]
        [Route("getansandquestionsbyid")]
        public async Task<IHttpActionResult> GetAnsAndQuestionsById(int candidateId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobData = await (from hq in _db.HiringQuestionsBanks
                                     join h in _db.HiringQuesionsAndAnsBanks
                                     on hq.QuesionsId equals h.QuesionsId
                                     where h.IsActive && !h.IsDeleted
                                     && h.CompanyId == tokenData.companyId
                                     && h.CandidateId == candidateId
                                     select new AnsAndQuestionsHelper
                                     {
                                         QuesionsId = h.QuesionsId,
                                         QuesionTitle = hq.QuesionTitle,
                                         AnswerId = h.AnswerId,
                                         Answer = h.Answer
                                     }).ToListAsync();
                if (jobData.Count > 0)
                {
                    res.Message = "Get Quesions And Ans Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = jobData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = jobData;
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/question/getansandquestionsbyid | " +
                    "CandidateId : " + candidateId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        public class AnsAndQuestionsHelper
        {
            public Guid AnswerId { get; set; } = Guid.NewGuid();
            public Guid QuesionsId { get; set; } = Guid.NewGuid();
            public string Answer { get; set; } = string.Empty;
            public string QuesionTitle { get; set; } = string.Empty;
        }
        #endregion
    }
}
