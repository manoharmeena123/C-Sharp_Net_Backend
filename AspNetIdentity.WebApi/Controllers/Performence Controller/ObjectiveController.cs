using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Performence;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Performence_Controller
{
    [Authorize]
    [RoutePrefix("api/Objective")]
    public class ObjectiveController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region This Api Is use To Add Objactive
        ///// <summary>
        ///// created by  Mayank Prajapati On 26/11/2022
        ///// Api >> Post >> api/Reviews/addobjectives
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("addobjectives")]
        public async Task<IHttpActionResult> AddObjectives(ObjectiveModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<int> ids = new List<int>();
            try
            {
                if (model == null)
                {
                    res.Message = "Objective Not Added";
                    res.Status = false;
                }
                else
                {
                    var reviewgroupdata = await _db.ObjectiveModels.Where(x => x.IsActive && !x.IsDeleted &&
                    x.CompanyId == tokenData.companyId).ToListAsync();
                    if (model != null)
                    {
                        ObjectiveModel obj = new ObjectiveModel
                        {
                            ObjectiveName = model.ObjectiveName,
                            Objectivetype = Enum.GetName(typeof(ObjectiveTypeConstants), model.ObjectiveTypeEnumId),
                            Owner = model.Owner,
                            IncludeInReview = model.IncludeInReview,
                            StartValue = model.StartValue,
                            TargetValue = model.TargetValue,
                            WhoCanSeeName = Enum.GetName(typeof(WhoCanSeeConstants), model.WhoCanSeeEnumId),
                            ProgressName = Enum.GetName(typeof(ProgressConstants), model.ProgressEnumId),
                            TagsName = Enum.GetName(typeof(TagsConstants), model.TagsEnumId),
                            CompanyId = tokenData.companyId,
                            OrgId = tokenData.orgId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false
                        };
                        _db.ObjectiveModels.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Objactive Added";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Objective Not Added";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/addobjectives", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Get All Objective 
        /// <summary>
        /// API >> Get >>api/Objective/getallobjactive
        ///  Created by  Mayank Prajapati On 26/11/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallobjactive")]
        public async Task<IHttpActionResult> GetAllObjactive()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Objectivedata = await _db.ObjectiveModels.Where(x => x.IsActive && !x.IsDeleted
                      && x.CompanyId == tokenData.companyId)
                    .Select(x => new GetObjactiveHelperModel
                    {
                        ObjectiveId = x.ObjectiveId,
                        ObjectiveName = x.ObjectiveName,
                        Owner = x.Owner,
                        StartValue = x.StartValue,
                        WhoCanSeeName = x.WhoCanSeeName.ToString().Replace("_", " "),
                        Objectivetype = x.Objectivetype.ToString().Replace("_", " "),
                        ProgressName = x.ProgressName.ToString().Replace("_", " "),
                        TagsName = x.TagsName.ToString().Replace("_", " "),
                        TargetValue = x.TargetValue,
                    }
                      ).ToListAsync();
                if (Objectivedata != null)
                {
                    res.Message = "Review Group Found";
                    res.Status = true;
                    res.Data = Objectivedata;
                }
                else
                {
                    res.Message = "Review Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/getallobjactive", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetObjactiveHelperModel
        {
            public Guid ObjectiveId { get; set; }
            public string ObjectiveName { get; set; }
            public ObjectiveTypeConstants ObjectiveTypeEnumId { get; set; }
            public string Objectivetype { get; set; }
            public string Owner { get; set; }
            public WhoCanSeeConstants WhoCanSeeEnumId { get; set; }
            public string WhoCanSeeName { get; set; }
            public string IncludeInReview { get; set; }
            public ProgressConstants ProgressEnumId { get; set; }
            public string ProgressName { get; set; }
            public TagsConstants TagsEnumId { get; set; }
            public string TagsName { get; set; }
            public int StartValue { get; set; }
            public int TargetValue { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get All Objective By Id
        /// <summary>
        /// API >> Get >>api/Objective/getaobjactivebyid
        ///  Created by  Mayank Prajapati On 26/11/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getaobjactivebyid")]
        public async Task<IHttpActionResult> GetObjactiveById(Guid ObjectiveId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Objectivedata = await _db.ObjectiveModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                    .Select(x => new GetObjactiveByIdHelperModel
                    {
                        ObjectiveId = x.ObjectiveId,
                        ObjectiveName = x.ObjectiveName,
                        Owner = x.Owner,
                        IncludeInReview = x.IncludeInReview,
                        StartValue = x.StartValue,
                        WhoCanSeeName = x.WhoCanSeeName.ToString().Replace("_", " "),
                        Objectivetype = x.Objectivetype.ToString().Replace("_", " "),
                        ProgressName = x.ProgressName.ToString().Replace("_", " "),
                        TagsName = x.TagsName.ToString().Replace("_", " "),
                        TargetValue = x.TargetValue,
                    }).ToListAsync();
                if (Objectivedata != null)
                {
                    res.Message = "Review Group Found";
                    res.Status = true;
                    res.Data = Objectivedata;
                }
                else
                {
                    res.Message = "Review Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/getaobjactivebyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetObjactiveByIdHelperModel
        {
            public Guid ObjectiveId { get; set; }
            public string ObjectiveName { get; set; }
            public ObjectiveTypeConstants ObjectiveTypeEnumId { get; set; }
            public string Objectivetype { get; set; }
            public string Owner { get; set; }
            public WhoCanSeeConstants WhoCanSeeEnumId { get; set; }
            public string WhoCanSeeName { get; set; }
            public string IncludeInReview { get; set; }
            public ProgressConstants ProgressEnumId { get; set; }
            public string ProgressName { get; set; }
            public TagsConstants TagsEnumId { get; set; }
            public string TagsName { get; set; }
            public int StartValue { get; set; }
            public int TargetValue { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Update objective   
        /// <summary>
        /// API >> Put >>api/Objective/updateobjective
        ///  Created by  Mayank Prajapati On 26/11/2022
        /// </summary>
        /// <returns></returns>
        ///
        [HttpPut]
        [Route("updateobjactive")]
        public async Task<IHttpActionResult> UpdateObjective(ObjectiveModel model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var objdata = await _db.ObjectiveModels.FirstOrDefaultAsync(x => x.ObjectiveId == model.ObjectiveId);
                if (objdata != null)
                {
                    objdata.ObjectiveName = model.ObjectiveName;
                    objdata.WhoCanSeeName = model.WhoCanSeeName;
                    objdata.Objectivetype = model.Objectivetype;
                    objdata.ProgressName = model.ProgressName;
                    objdata.TagsName = model.TagsName;
                    objdata.Owner = model.Owner;
                    objdata.StartValue = model.StartValue;
                    objdata.TargetValue = model.TargetValue;
                    objdata.UpdatedBy = tokenData.employeeId;
                    objdata.UpdatedOn = DateTime.Now;

                    _db.Entry(objdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Message = "Objective  Updated";
                    res.Status = true;
                    res.Data = objdata;
                }
                else
                {
                    res.Message = "Objective Not Update";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/updateobjective", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Deleted Objactive
        /// <summary>
        /// API >> Delete >>api/Objective/deleteObjective
        ///  Created by Mayank Prajapati On 26/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteObjective")]
        public async Task<IHttpActionResult> DeleteObjactive(Guid objactiveid)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var objactivedata = await _db.ObjectiveModels.FirstOrDefaultAsync(x =>
                    x.IsActive && !x.IsDeleted && x.ObjectiveId == objactiveid);
                if (objactivedata != null)
                {
                    objactivedata.IsDeleted = true;
                    objactivedata.IsActive = false;
                    objactivedata.DeletedBy = tokenData.employeeId;
                    objactivedata.DeletedOn = DateTime.Now;

                    _db.Entry(objactivedata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Objective Deleted Successfully!";
                    res.Data = objactivedata;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/deleteObjective", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use To Get Objective Type Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Get >> api/Objective/getobjectivetyperenum
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getobjectivetyperenum")]
        [HttpGet]
        public IHttpActionResult GetObjectivetypeEnum()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var reviewGroup = Enum.GetValues(typeof(ObjectiveTypeConstants))
                    .Cast<ObjectiveTypeConstants>()
                    .Select(x => new ObjectivetypeModel
                    {
                        ObjectivetypeTypeId = (int)x,
                        Objectivetype = Enum.GetName(typeof(ObjectiveTypeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Review Group Exist";
                res.Status = true;
                res.Data = reviewGroup;
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/getobjectivetyperenum", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class ObjectivetypeModel
        {
            public int ObjectivetypeTypeId { get; set; }
            public string Objectivetype { get; set; }
        }
        #endregion

        #region This Api Use To Get Who Can See Name Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Get >> api/Objective/getwhocanseename
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getwhocanseename")]
        [HttpGet]
        public IHttpActionResult GetWhoCanSeeName()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var reviewGroup = Enum.GetValues(typeof(WhoCanSeeConstants))
                    .Cast<WhoCanSeeConstants>()
                    .Select(x => new WhoCanSeeNameModel
                    {
                        WhoCanSeeEnumId = (int)x,
                        WhoCanSeeName = Enum.GetName(typeof(WhoCanSeeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Review Group Exist";
                res.Status = true;
                res.Data = reviewGroup;
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/getwhocanseename", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class WhoCanSeeNameModel
        {
            public int WhoCanSeeEnumId { get; set; }
            public string WhoCanSeeName { get; set; }
        }
        #endregion

        #region This Api Use To Get Progress Name Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 13/10/2022
        ///// Api >> Get >> api/Objective/getwhocanseename
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getwhocanseename")]
        [HttpGet]
        public IHttpActionResult GetProgressName()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var reviewGroup = Enum.GetValues(typeof(ProgressConstants))
                    .Cast<ProgressConstants>()
                    .Select(x => new ProgressNameModel
                    {
                        ProgressEnumId = (int)x,
                        ProgressName = Enum.GetName(typeof(ProgressConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Review Group Exist";
                res.Status = true;
                res.Data = reviewGroup;
            }
            catch (Exception ex)
            {
                logger.Error("api/Objective/getwhocanseename", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class ProgressNameModel
        {
            public int ProgressEnumId { get; set; }
            public string ProgressName { get; set; }
        }
        #endregion

    }
}
