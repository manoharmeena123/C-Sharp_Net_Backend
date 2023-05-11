using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.MailTemplate;
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

namespace AspNetIdentity.WebApi.Controllers.MailTemplate
{
    /// <summary>
    /// Created By Ankit Jain on 16-02-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/mailtemplate")]
    public class HiringTemplateController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API FOR Add Template
        /// <summary>
        /// Created By ankit Jain On 30-01-2023
        /// API >> Post >> api/mailtemplate/addinterviewschedule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("addinterviewschedule")]
        [HttpPost]
        public async Task<IHttpActionResult> AddInterviewSchedule(AddScheduleInterview model)
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

                HiringTemplate obj = new HiringTemplate
                {
                    Templatetype = model.Templatetype,
                    IsCustmized = model.IsCustmized,
                    TemplateForCandidate = model.TemplateForCandidate,
                    TemplateForInterviewer = model.TemplateForInterviewer,
                    TemplateForRecruiter = model.TemplateForRecruiter,
                    CreatedBy = tokenData.employeeId,
                    CompanyId = tokenData.companyId,
                    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                };
                _db.HiringTemplates.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Created Successfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = obj;
            }
            catch (Exception ex)
            {

                logger.Error("API : api/mailtemplate/addinterviewschedule | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        public class AddScheduleInterview
        {
            public string CandidateName { get; set; } = string.Empty;
            public TemplateTypeConstants Templatetype { get; set; } = TemplateTypeConstants.InterviewSchedule;
            public bool IsCustmized { get; set; } = false;
            public string TemplateForInterviewer { get; set; } = string.Empty;
            public string TemplateForRecruiter { get; set; } = string.Empty;
            public string TemplateForCandidate { get; set; } = string.Empty;
        }
        #endregion

        #region This Api To Get All Mail Template
        /// <summary>
        /// Created By Ankit Jain on 30-01-2023
        /// API >>Get >> api/mailtemplate/getallmailtemplate
        /// </summary>
        [HttpGet]
        [Route("getallmailtemplate")]
        public async Task<IHttpActionResult> getallmailtemplate()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var templateData = await (from g in _db.HiringTemplates
                                          where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                          && g.IsActive && !g.IsDeleted
                                          select new
                                          {
                                              id = g.Id,
                                              TemplateType = g.Templatetype,
                                              TemplateName = g.Templatetype.ToString(),
                                              CustomizeName = g.IsCustmized == true ? "Custmized" : "Default",
                                          }).ToListAsync();
                if (templateData.Count > 0)
                {
                    res.Message = "Template List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = templateData;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/mailtemplate/getallmailtemplate | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        #endregion

        #region This Api To Get All Mail Template by Id
        /// <summary>
        /// Created By Ankit Jain on 30-01-2023
        /// API >>Get >> api/mailtemplate/getmailtemplatebyid
        /// </summary>
        /// <param name="templateid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getmailtemplatebyid")]
        public async Task<IHttpActionResult> GetMailTemplateById(Guid templateid)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var templateData = await (from g in _db.HiringTemplates
                                          where g.IsActive && !g.IsDeleted && g.CompanyId == tokenData.companyId
                                          && g.IsActive && !g.IsDeleted && g.Id == templateid
                                          select new
                                          {
                                              id = g.Id,
                                              Templatetype = g.Templatetype,
                                              TemplateType = g.Templatetype.ToString(),
                                              CustomizeName = g.IsCustmized,
                                              TemplateForCandidate = g.TemplateForCandidate,
                                              TemplateForInterviewer = g.TemplateForInterviewer,
                                              TemplateForRecruiter = g.TemplateForRecruiter,
                                          }).FirstOrDefaultAsync();
                if (templateData != null)
                {
                    res.Message = "Template Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = templateData;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/mailtemplate/getmailtemplatebyid | " +
                    "Id : " + templateid + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }

        #endregion

        #region API FOR Update Template
        /// <summary>
        /// Created By ankit Jain On 30-01-2023
        /// API >> Post >> api/mailtemplate/updateinterviewschedule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updateinterviewschedule")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateInterviewSchedule(UpdateScheduleInterview model)
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
                    var hiringTemplate = await _db.HiringTemplates.
                               FirstOrDefaultAsync(x => x.Id == model.Id
                                 && x.IsActive && !x.IsDeleted);
                    if (hiringTemplate != null)
                    {
                        hiringTemplate.Templatetype = model.Templatetype;
                        hiringTemplate.IsCustmized = model.IsCustmized;
                        hiringTemplate.TemplateForCandidate = model.TemplateForCandidate;
                        hiringTemplate.TemplateForInterviewer = model.TemplateForInterviewer;
                        hiringTemplate.TemplateForRecruiter = model.TemplateForRecruiter;
                        hiringTemplate.UpdatedBy = tokenData.employeeId;
                        hiringTemplate.CompanyId = tokenData.companyId;
                        hiringTemplate.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                        _db.Entry(hiringTemplate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Updated Successfully  !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = hiringTemplate;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Data Not Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = hiringTemplate;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/mailtemplate/updateinterviewschedule | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }

        public class UpdateScheduleInterview
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public TemplateTypeConstants Templatetype { get; set; } = TemplateTypeConstants.InterviewSchedule;
            public bool IsCustmized { get; set; } = false;
            public string TemplateForInterviewer { get; set; } = string.Empty;
            public string TemplateForRecruiter { get; set; } = string.Empty;
            public string TemplateForCandidate { get; set; } = string.Empty;
        }
        #endregion

        #region API TO DELETE Template
        /// <summary>
        /// Created By Ankit Jain On 02/02/2023
        /// API >> POST >> api/mailtemplate/deletetemplate
        /// </summary>
        /// <param name="templateId></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletetemplate")]
        public async Task<IHttpActionResult> DeleteTemplate(Guid templateId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var hiringTemplate = await _db.HiringTemplates
                          .FirstOrDefaultAsync(x => x.Id == templateId);
                if (hiringTemplate == null)
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    hiringTemplate.IsActive = false;
                    hiringTemplate.IsDeleted = true;
                    hiringTemplate.DeletedBy = tokenData.employeeId;
                    hiringTemplate.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(hiringTemplate).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Template Deleted";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = hiringTemplate;
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/mailtemplate/deletetemplate | " +
                    "Id : " + templateId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion
    }
}
