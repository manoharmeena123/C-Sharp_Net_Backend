using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/experience")]
    public class ExperienceController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region#Get Experience By Experience Id

        [Route("GetExperienceById")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetExperienceById(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                ExperienceData depData = new ExperienceData();
                var ExperienceData = db.Experience.Where(x => x.ExperienceId == Id && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefault();
                if (ExperienceData != null)
                {
                    depData.Status = "OK";
                    depData.Message = "Experience Found";
                    depData.Experience = ExperienceData;
                }
                else
                {
                    depData.Status = "Error";
                    depData.Message = "No Experience Found!!";
                    depData.Experience = null;
                }
                return Ok(depData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #region#Get Experience By EmployeeId

        [Route("GetExperienceByEmployeeId")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetExperienceByEmployeeId(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                ExperienceData depData = new ExperienceData();
                var ExperienceData = db.Experience.Where(x => x.EmployeeId == Id && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefault();
                if (ExperienceData != null)
                {
                    depData.Status = "OK";
                    depData.Message = "Experience Found";
                    depData.Experience = ExperienceData;
                }
                else
                {
                    depData.Status = "Error";
                    depData.Message = "No Experience Found!!";
                    depData.Experience = null;
                }
                return Ok(depData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #region#Get All Experience

        [Route("GetAllExperience")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetAllExperience()
        {
            try
            {
                ExperienceDataList dep = new ExperienceDataList();
                var ExperienceData = db.Experience.Where(x => x.IsDeleted == false).ToList();
                if (ExperienceData.Count != 0)
                {
                    dep.Status = "OK";
                    dep.Message = "Experience list Found";
                    dep.ExperienceList = ExperienceData;
                }
                else
                {
                    dep.Status = "Not OK";
                    dep.Message = "No Experience list Found";
                    dep.ExperienceList = null;
                }
                return Ok(dep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #region#Post Experience

        [Route("CreateExperience")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateExperience(Experience createExperience)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateDepData = db.Experience.Where(x => x.ExperienceId == createExperience.ExperienceId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefault();
                ExperienceData res = new ExperienceData();
                if (updateDepData == null)
                {
                    Experience newExperience = new Experience();
                    newExperience.EmployeeId = createExperience.EmployeeId;
                    newExperience.FromDate = createExperience.FromDate;
                    newExperience.ToDate = createExperience.ToDate;
                    newExperience.LastDrawnSalary = createExperience.LastDrawnSalary;
                    newExperience.CompanyName = createExperience.CompanyName;
                    newExperience.TotalYearOfExperience = createExperience.TotalYearOfExperience;
                    newExperience.CompanyId = claims.companyId;
                    newExperience.OrgId = claims.orgId;
                    db.Experience.Add(newExperience);
                    db.SaveChanges();

                    res.Status = "OK";
                    res.Message = "Experience added Successfully!";
                    res.Experience = newExperience;
                    return Ok(res);
                }
                else
                {
                    res.Status = "NOT OK";
                    res.Message = "Duplicate Experience!";
                    res.Experience = null;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #region#PUT Experience

        [Route("UpdateExperience")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateExperience(Experience updateDep)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var updateExpData = db.Experience.Where(x => x.ExperienceId == updateDep.ExperienceId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId && x.IsDeleted == false).FirstOrDefault();
                if (updateExpData != null)
                {
                    updateExpData.EmployeeId = updateDep.EmployeeId;
                    updateExpData.FromDate = updateDep.FromDate;
                    updateExpData.ToDate = updateDep.ToDate;
                    updateExpData.LastDrawnSalary = updateDep.LastDrawnSalary;
                    updateExpData.CompanyName = updateDep.CompanyName;
                    updateExpData.TotalYearOfExperience = updateDep.TotalYearOfExperience;

                    db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Experience Updated Successfully!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #region#Delete Experience

        [Route("DeleteExperience")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteExperience(int ExperienceId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var deleteData = db.Experience.Where(x => x.ExperienceId == ExperienceId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefault();
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Experience Deleted Successfully!";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Experience Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        public class ExperienceData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public Experience Experience { get; set; }
        }

        public class ExperienceDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<Experience> ExperienceList { get; set; }
        }
    }
}