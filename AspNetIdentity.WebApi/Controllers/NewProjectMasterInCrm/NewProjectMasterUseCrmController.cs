using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.ProjectMaster;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.NewProjectMasterInCrm
{
    [RoutePrefix("api/openproject")]
    public class NewProjectMasterUseCrmController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region This api use Open Api in project master

        #region This Api use For Added Project In Open Api

        /// <summary>
        /// Create By Ankit Jain Date 16-12-2022
        /// </summary>route api/openproject/addopenProject
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addopenProject")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> AddOpenProjectData(AddOpenProject model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var projectdata = _db.NewProjectMasters.Where(x => x.ProjectName == model.ProjectName).FirstOrDefault();
                if (projectdata == null)
                {
                    NewProjectMaster PreData = new NewProjectMaster();
                    PreData.ProjectName = model.ProjectName;
                    PreData.ProjectId = model.ProjectId;
                    PreData.ProjectManagerId = model.ProjectManagerId;
                    PreData.LeadTypeId = model.LeadTypeId;
                    PreData.CampanyName = model.CampanyName;
                    PreData.TechnologyId = model.TechnologyId;
                    PreData.TechnologyName = model.TechnologyName;
                    PreData.PaymentType = model.PaymentType;
                    if ("Recurring" == model.LeadTypeName)
                    {
                        PreData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                    }
                    else
                    {
                        PreData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                        PreData.StartDate = model.StartDate;
                        PreData.EndDate = model.EndDate;
                    }

                    PreData.Others = model.Others;
                    PreData.FromCurrency = model.FromCurrency;
                    PreData.ToCurrency = model.ToCurrency;
                    PreData.ClientAmount = model.ClientAmount;
                    PreData.ExchangeRate = model.ExchangeRate;
                    PreData.ExchangeDate = DateTime.Now;
                    PreData.LeadTypeName = model.LeadTypeName;
                    PreData.ClientConvertedAmt = model.ClientConvertedAmt;
                    PreData.ProjectDiscription = model.ProjectDiscription;
                    PreData.IsDeleted = false;
                    PreData.IsActive = true;
                    PreData.CreatedBy = 0;
                    PreData.CreatedOn = DateTime.Now;
                    PreData.CompanyId = model.CompanyId;
                    PreData.OrgId = 0;
                    _db.NewProjectMasters.Add(PreData);
                    await _db.SaveChangesAsync();

                    res.Message = "Project Added Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = PreData;
                }
                else
                {
                    projectdata.ProjectId = model.ProjectId;
                    projectdata.ProjectName = model.ProjectName;
                    projectdata.ProjectManagerId = model.ProjectManagerId;
                    projectdata.LeadTypeId = model.LeadTypeId;
                    projectdata.CampanyName = model.CampanyName;
                    projectdata.TechnologyId = model.TechnologyId;
                    projectdata.TechnologyName = model.TechnologyName;
                    projectdata.PaymentType = model.PaymentType;
                    if ("Recurring" == model.LeadTypeName)
                    {
                        projectdata.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                    }
                    else
                    {
                        projectdata.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                        projectdata.StartDate = model.StartDate;
                        projectdata.EndDate = model.EndDate;
                    }

                    projectdata.Others = model.Others;
                    projectdata.FromCurrency = model.FromCurrency;
                    projectdata.ToCurrency = model.ToCurrency;
                    projectdata.ClientAmount = model.ClientAmount;
                    projectdata.ExchangeRate = model.ExchangeRate;
                    projectdata.ExchangeDate = DateTime.Now;
                    projectdata.LeadTypeName = model.LeadTypeName;
                    projectdata.ClientConvertedAmt = model.ClientConvertedAmt;
                    projectdata.ProjectDiscription = model.ProjectDiscription;
                    projectdata.IsDeleted = false;
                    projectdata.IsActive = true;
                    projectdata.CreatedBy = 0;
                    projectdata.CreatedOn = DateTime.Now;
                    projectdata.CompanyId = model.CompanyId;
                    projectdata.OrgId = 0;
                    _db.Entry(projectdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Message = "Project Updated Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = projectdata;

                }
            }
            catch (Exception ex)
            {
                logger.Error("api/openproject/addopenProject", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class AddOpenProject
        {
            public Guid ProjectId { get; set; }
            public string ProjectName { get; set; }
            public Guid ProjectManagerId { get; set; }
            public string CampanyName { get; set; }
            public Guid TechnologyId { get; set; }
            public string TechnologyName { get; set; }
            public string PaymentType { get; set; }
            public string ProjectDiscription { get; set; }
            public Guid LeadTypeId { get; set; }
            public string LeadTypeName { get; set; }
            public string Others { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
            public double ClientAmount { get; set; }
            public double ClientConvertedAmt { get; set; }
            public decimal ExchangeRate { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int CompanyId { get; set; }
        }
        #endregion This Api use For Added Project Open Api

        #region This API is used  update project open Project api

        /// <summary>
        /// API >> Put >> api/openproject/UpdateOpenProjectList
        /// Created by Ankit jain , Create on 22-12-2022
        /// </summary>
        /// <param name="updateProjectList"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateOpenProjectList")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateOpenProjectList(AddOpenProject model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var updateProjectListData = _db.NewProjectMasters.Where(x => x.ProjectId == model.ProjectId).FirstOrDefault();
                if (updateProjectListData != null)
                {
                    updateProjectListData.ID = updateProjectListData.ID;
                    updateProjectListData.ProjectId = model.ProjectId;
                    updateProjectListData.ProjectName = model.ProjectName;
                    updateProjectListData.ProjectManagerId = model.ProjectManagerId;
                    updateProjectListData.LeadTypeId = model.LeadTypeId;
                    updateProjectListData.CampanyName = model.CampanyName;
                    updateProjectListData.TechnologyId = model.TechnologyId;
                    updateProjectListData.TechnologyName = model.TechnologyName;
                    updateProjectListData.PaymentType = model.PaymentType;
                    if ("Recurring" == model.LeadTypeName)
                    {
                        if (updateProjectListData.ClientBillableAmount != model.ClientConvertedAmt)
                        {
                            updateProjectListData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                            updateProjectListData.LeadTypeName = "Recurring";
                        }
                    }
                    else
                    {
                        if (updateProjectListData.ClientBillableAmount != model.ClientConvertedAmt)
                        {
                            updateProjectListData.ClientBillableAmount = Convert.ToInt32(model.ClientConvertedAmt * 12);
                            updateProjectListData.StartDate = model.StartDate;
                            updateProjectListData.EndDate = model.EndDate;
                            updateProjectListData.LeadTypeName = "Fixed";
                        }
                    }

                    updateProjectListData.FromCurrency = model.FromCurrency;
                    updateProjectListData.ToCurrency = model.ToCurrency;
                    updateProjectListData.ClientAmount = model.ClientAmount;
                    updateProjectListData.ExchangeRate = model.ExchangeRate;
                    updateProjectListData.ClientConvertedAmt = model.ClientConvertedAmt;
                    updateProjectListData.IsDeleted = false;
                    updateProjectListData.IsActive = true;
                    updateProjectListData.UpdatedOn = DateTime.Now;
                    updateProjectListData.ProjectDiscription = model.ProjectDiscription;
                    updateProjectListData.UpdatedBy = 0;
                    _db.Entry(updateProjectListData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Project Updated Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = updateProjectListData;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/project/UpdateOpenProjectList", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion This API is used  update project

        #region Get all Project List Open Api

        /// <summary>
        /// API >> Get >> api/openproject/getallopenprojectlist
        ///  Created by Ankit jain on 16-12-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallopenprojectlist")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetOpenProjectList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var projectlist = await _db.NewProjectMasters.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                if (projectlist.Count > 0)
                {
                    res.Message = "Get All Project List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = projectlist;
                }
                else
                {
                    res.Message = "Not Found Project List";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = projectlist;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/project/UpdateOpenProjectList", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion Get all Project List

        #region Get all Project List Open Api

        /// <summary>
        /// API >> Get >> api/openproject/getopenprojectbyid
        ///  Created by Ankit jain on 27-12-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getopenprojectbyid")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetOpenProjecbyid(Guid Id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var projectlist = await _db.NewProjectMasters.Where(x => x.ID == Id && x.IsActive
                && !x.IsDeleted).FirstOrDefaultAsync();
                if (projectlist != null)
                {
                    res.Message = "Get Project Data";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = projectlist;
                }
                else
                {
                    res.Message = "Not Found Project Data";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = projectlist;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/openproject/getopenprojectbyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion Get all Project List

        #region API FOR DELETE Api In open Project
        /// <summary>
        /// Created By ankit Jain On 26-12-2022
        /// API >> Delete >> api/openproject/removeProjectdata
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("removeProjectdata")]
        [HttpDelete]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RemoveProject(Guid projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectData = _db.NewProjectMasters.Where(x => x.ProjectId == projectId
                              && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (projectData != null)
                {
                    projectData.IsActive = false;
                    projectData.IsDeleted = true;
                    projectData.UpdatedBy = tokenData.employeeId;
                    projectData.UpdatedOn = DateTime.Now;
                    _db.Entry(projectData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Succesfully  !";
                    res.Status = true;
                    res.Data = projectData;
                    return Ok(res);
                }
                else
                {
                    res.Message = " Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = projectData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {


                logger.Error("api/project/removeProjectdata", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #endregion
    }
}
