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
    [RoutePrefix("api/region")]
    [Authorize]
    public class RegionNewController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("GetByRegionId")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetRegionById(int Id)
        {
            try
            {
                Base response = new Base();
                RegionData depData = new RegionData();
                var RegionData = db.Region.Where(x => x.RegionId == Id).FirstOrDefault();
                if (RegionData != null)
                {
                    depData.Status = "OK";
                    depData.Message = "Region Found";
                    depData.Region = RegionData;
                }
                else
                {
                    depData.Status = "Error";
                    depData.Message = "No Region Found!!";
                    depData.Region = null;
                }
                return Ok(depData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetAllRegion")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetAllRegion()
        {
            try
            {
                RegionDataList dep = new RegionDataList();
                var RegionData = db.Region.Where(x => x.IsDeleted == false).ToList();
                if (RegionData.Count != 0)
                {
                    dep.Status = "OK";
                    dep.Message = "Region list Found";
                    dep.RegionList = RegionData;
                }
                else
                {
                    dep.Status = "Not OK";
                    dep.Message = "No Region list Found";
                    dep.RegionList = null;
                }
                return Ok(dep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("CreateRegion")]
        [HttpPost]
        public IHttpActionResult CreateRegion(Region createRegion)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                RegionData res = new RegionData();
                Region newRegion = new Region();
                newRegion.RegionName = createRegion.RegionName;
                newRegion.Description = createRegion.Description;
                newRegion.CompanyId = claims.companyId;
                newRegion.OrgId = claims.orgId;
                newRegion.IsActive = true;
                newRegion.IsDeleted = false;

                db.Region.Add(newRegion);
                db.SaveChanges();

                res.Status = "OK";
                res.Message = "Region added Successfully!";
                res.Region = newRegion;
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateRegion")]
        [HttpPut]
        public IHttpActionResult UpdateRegion(Region updateDep)
        {
            try
            {
                Base response = new Base();
                var updateDepData = db.Region.Where(x => x.RegionId == updateDep.RegionId && x.IsDeleted == false).FirstOrDefault();
                if (updateDepData != null)
                {
                    updateDepData.RegionName = updateDep.RegionName;
                    updateDepData.Description = updateDep.Description;
                    updateDepData.IsActive = true;
                    updateDepData.IsDeleted = false;

                    db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Region Updated Successfully!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteRegion")]
        [HttpDelete]
        public IHttpActionResult DeleteRegion(int RegionId)
        {
            try
            {
                Base response = new Base();
                var deleteData = db.Region.Where(x => x.RegionId == RegionId).FirstOrDefault();
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Region Deleted Successfully!";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No Region Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class RegionData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public Region Region { get; set; }
        }

        public class RegionDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<Region> RegionList { get; set; }
        }
    }
}