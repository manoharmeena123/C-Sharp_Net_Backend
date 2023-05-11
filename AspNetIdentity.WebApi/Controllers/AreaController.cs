using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/Area")]
    public class AreaController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public object EntityArea { get; private set; }

        [Route("GetAreaById")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAreaById(int Id)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                int userid = 0;
                int compid = 0;
                int orgid = 0;

                // Access claims

                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

                //AreaData depData = new AreaData();
                var AreaData = await db.Area.Where(x => x.AreaId == Id).FirstOrDefaultAsync();
                if (AreaData != null)
                {
                    response.Status = true;
                    response.Message = "Area Found";
                    response.Data = AreaData;
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Area Found!!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        [Route("GetAllArea")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllArea()
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                int userid = 0;
                int compid = 0;
                int orgid = 0;

                // Access claims

                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

                //AreaDataList dep = new AreaDataList();
                var AreaData = await db.Area.Where(x => x.Deleted == false).ToListAsync();
                if (AreaData.Count != 0)
                {
                    response.Status = true;
                    response.Message = "Area list Found";
                    response.Data = AreaData;
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Area list Found";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        [Route("CreateArea")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> CreateArea(Area createArea)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                int userid = 0;
                int compid = 0;
                int orgid = 0;

                // Access claims

                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

                //Base response = new Base();
                var tAreaData = db.Area.Where(x => string.Equals(x.AreaName.Trim(), createArea.AreaName.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                //AreaData res = new AreaData();
                Area newArea = new Area();
                if (tAreaData == null)
                {
                    newArea.AreaName = createArea.AreaName;
                    newArea.CityId = createArea.CityId;
                    newArea.CompanyId = compid;
                    newArea.OrgId = orgid;
                    newArea.active = true;
                    newArea.Deleted = false;
                    newArea.CreatedOn = DateTime.Now;
                    newArea.UpdatedOn = DateTime.Now;
                    newArea.CreatedBy = claims.employeeId.ToString();
                    newArea.UpdateBy = claims.employeeId.ToString();
                    db.Area.Add(newArea);
                    await db.SaveChangesAsync();

                    response.Status = true;
                    response.Message = "Area added Successfully!";
                    response.Data = newArea;
                }
                else
                {
                    //res.Status = "";

                    //if (tAreaData.active == true)
                    //    res.Status = "Duplicate & not active";
                    //else
                    //    res.Status = "Duplicate";

                    if (tAreaData.active == false)
                    {
                        tAreaData.AreaName = createArea.AreaName;
                        tAreaData.CityId = createArea.CityId;
                        tAreaData.active = true;
                        tAreaData.Deleted = false;
                        db.Entry(tAreaData).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        response.Status = true;
                        response.Message = "Area added Successfully!";
                        response.Data = tAreaData;
                    }

                    response.Message = "Area already exists!";
                    response.Data = newArea;
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        [Route("UpdateArea")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> UpdateArea(Area updateDep)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateDepData = db.Area.Where(x => x.AreaId == updateDep.AreaId).FirstOrDefault();

                if (updateDepData != null)
                {
                    var checkarea = db.Area.Where(x => x.AreaName.ToLower().Trim() == updateDep.AreaName.ToLower().Trim()).FirstOrDefault();
                    if (checkarea != null)
                    {
                        response.Status = false;
                        response.Message = "Area Already Update !";
                    }

                    updateDepData.AreaName = updateDep.AreaName;
                    updateDepData.AreaId = updateDep.AreaId;
                    //updateDepData.AreaName = updateDep.AreaName;
                    updateDepData.UpdateBy = claims.employeeId.ToString();
                    updateDepData.UpdatedOn = DateTime.Now;
                    db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;

                    await db.SaveChangesAsync();

                    response.Status = true;
                    response.Message = "Area Updated Successfully!";
                }
                else
                {
                    response.Status = false;
                    response.Message = "Area Already Update !";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }

        [Route("DeleteArea")]
        [HttpDelete]
        [Authorize]
        public async Task<ResponseBodyModel> DeleteArea(int AreaId)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                //Base response = new Base();

                var deleteData = db.Area.Where(x => x.AreaId == AreaId).FirstOrDefault();
                if (deleteData != null)
                {
                    deleteData.Deleted = true;
                    deleteData.active = false;
                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    response.Status = true;
                    response.Message = "Area Deleted Successfully!";
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Area Found!!";
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        public class AreaData
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public Area Area { get; set; }
        }

        public class AreaDataList
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<Area> AreaList { get; set; }
        }
    }
}