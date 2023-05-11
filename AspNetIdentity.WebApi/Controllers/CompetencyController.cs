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
    /// <summary>
    /// Created by Harshit Mitra on 21-03-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/competency")]
    public class CompetencyController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Competency Type Masters Api's

        #region Api To Add Competency Type

        /// <summary>
        /// API >> Post >> api/competency/addcompetencytype
        /// Created By Harshit Mitra on 21-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcompetencytype")]
        public async Task<ResponseBodyModel> AddCompetencyType(CompetencyType model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    CompetencyType obj = new CompetencyType
                    {
                        CompetencyTypeName = model.CompetencyTypeName,
                        Description = model.Description,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                    };
                    _db.CompetencyTypes.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Competency Type Added";
                    res.Status = true;
                    res.Data = obj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Competency Type

        #region Api To Get All Active Competency Type

        /// <summary>
        /// API >> Get >> api/competency/getactcompetencytype
        /// Created By Harshit Mitra on 21-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getactcompetencytype")]
        public async Task<ResponseBodyModel> GetAllActiveCompentencyType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var compentencyTypeList = await _db.CompetencyTypes.
                    Where(x => x.IsActive == true && x.IsDeleted == false).ToListAsync();
                if (compentencyTypeList.Count > 0)
                {
                    res.Message = "Compentency List";
                    res.Status = true;
                    res.Data = compentencyTypeList;
                }
                else
                {
                    res.Message = "List is Empty";
                    res.Status = false;
                    res.Data = compentencyTypeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Active Competency Type

        #region Api To Edit Competency Type

        /// <summary>
        /// API >> Put >> api/competency/editcompetencytype
        /// Created By Harshit Mitra on 21-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editcompetencytype")]
        public async Task<ResponseBodyModel> EditCompetencyType(CompetencyType model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var compentencyType = await _db.CompetencyTypes.FirstOrDefaultAsync(x => x.IsActive == true && x.IsDeleted == false &&
                        x.CompetencyTypeId == model.CompetencyTypeId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (compentencyType != null)
                {
                    compentencyType.CompetencyTypeName = model.CompetencyTypeName;
                    compentencyType.Description = model.Description;
                    compentencyType.UpdatedBy = claims.userId;
                    compentencyType.UpdatedOn = DateTime.Now;

                    _db.Entry(compentencyType).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Compentency Updated";
                    res.Status = true;
                    res.Data = compentencyType;
                }
                else
                {
                    res.Message = "Compentency Type Not Found";
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

        #endregion Api To Edit Competency Type

        #region Api To Delete Competency Type

        /// <summary>
        /// API >> Put >> api/competency/deletecompetencytype
        /// Created By Harshit Mitra on 21-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletecompetencytype")]
        public async Task<ResponseBodyModel> DeleteCompetencyType(int compentencyId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var compentencyType = await _db.CompetencyTypes.FirstOrDefaultAsync(x => x.IsActive == true && x.IsDeleted == false &&
                        x.CompetencyTypeId == compentencyId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (compentencyType != null)
                {
                    compentencyType.IsActive = false;
                    compentencyType.IsDeleted = true;
                    compentencyType.DeletedBy = claims.userId;
                    compentencyType.DeletedOn = DateTime.Now;

                    _db.Entry(compentencyType).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Compentency Deleted";
                    res.Status = true;
                    res.Data = compentencyType;
                }
                else
                {
                    res.Message = "Compentency Type Not Found";
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

        #endregion Api To Delete Competency Type

        #endregion Competency Type Masters Api's

        #region Competency Masters Api's

        #region Api For Add Compentency

        /// <summary>
        /// api/competency/addcompetency
        /// Created On 7-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addcompetency")]
        public async Task<ResponseBodyModel> AddCompetency(AddGetCompentancy model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            if (model != null)
            {
                Competency obj = new Competency
                {
                    CompetencyName = model.CompetencyName,
                    Description = model.Description,
                    CompetencyTypeId = model.CompetencyTypeId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = claims.userId,
                    CompanyId = claims.companyId,
                    OrgId = claims.orgId,
                    CreatedOn = DateTime.Now,
                };
                _db.Competencies.Add(obj);
                await _db.SaveChangesAsync();

                if (model.Behaviours.Count > 0)
                {
                    foreach (var item in model.Behaviours)
                    {
                        CompentencyBehaviours beh = new CompentencyBehaviours
                        {
                            CompetencyId = obj.CompetencyId,
                            BehavioursName = item.BehavioursName,
                            UseInRating = item.UseInRating,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = claims.userId,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedOn = DateTime.Now,
                        };
                        _db.CompentencyBehaviours.Add(beh);
                        await _db.SaveChangesAsync();
                    }
                }

                res.Message = "Successfull Registered";
                res.Status = true;
            }
            else
            {
                res.Message = "Model in Invalid";
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Add Compentency

        #region Api For Get All Compentency

        /// <summary>
        /// api/competency/getallcompetency
        /// Created On 7-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcompetency")]
        public async Task<ResponseBodyModel> GetAllCompetency()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetCompendencyList> list = new List<GetCompendencyList>();
            try
            {
                var compentency = await _db.Competencies.Where(s => s.IsActive == true && s.CompanyId == claims.companyId
                        && s.IsDeleted == false && s.OrgId == claims.orgId).ToListAsync();
                if (compentency.Count > 0)
                {
                    foreach (var item in compentency)
                    {
                        GetCompendencyList obj = new GetCompendencyList
                        {
                            CompentencyId = item.CompetencyId,
                            CompetencyName = item.CompetencyName,
                            Description = item.Description,
                            TotalBehavior = _db.CompentencyBehaviours.Where(x => x.CompetencyId == item.CompetencyId).Count(),
                        };
                        list.Add(obj);
                    }
                }
                if (list.Count > 0)
                {
                    res.Message = "Compentency List";
                    res.Status = true;
                    res.Data = list;
                }
                else
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                    res.Data = list;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Get All Compentency

        #region Api For Delete Competency

        /// <summary>
        /// api/competency/deletecompetency
        /// Created On 7-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecompetency")]
        public async Task<ResponseBodyModel> DeleteCompetency(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            Competency data = new Competency();
            if (Id > 0)
            {
                var competency = await _db.Competencies.Where(s => s.CompetencyId == Id && s.IsActive == true && s.IsDeleted == false).FirstOrDefaultAsync();
                if (competency != null)
                {
                    competency.IsActive = false;
                    competency.IsDeleted = true;
                    competency.DeletedOn = DateTime.Now;
                    competency.DeletedBy = claims.userId;
                    _db.Entry(competency).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Message = "successfully Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Compentency Not Found";
                    res.Status = false;
                }
            }
            else
            {
                res.Message = "You Are unable to Delete";
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Delete Competency

        #region Api For Get Competency By Id

        /// <summary>
        /// api/competency/getcompetecybyid
        /// Created On 7-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompetecybyid")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> getbyid(int CompetencyId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            AddGetCompentancy response = new AddGetCompentancy();
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var compentency = _db.Competencies.Where(s => s.IsActive == true && s.IsDeleted == false && s.CompetencyId == CompetencyId).FirstOrDefault();
                if (compentency != null)
                {
                    var list = _db.CompentencyBehaviours.Where(x => x.CompetencyId == compentency.CompetencyId).ToList();

                    response.CompetencyId = compentency.CompetencyId;
                    response.CompetencyName = compentency.CompetencyName;
                    response.CompetencyTypeId = compentency.CompetencyTypeId;
                    response.Description = compentency.Description;
                    response.Behaviours = list;

                    res.Message = "Compentency Found";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "CompentencyId not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = "CompentencyId not found";
                res.Status = false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return res;
        }

        #endregion Api For Get Competency By Id

        #endregion Competency Masters Api's

        #region Helper Model Classes

        /// <summary>
        /// Created By Sneha Patidar
        /// </summary>
        public class GetCompentencyTypeList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Created By Sneha Patidar
        /// </summary>
        public class AddGetCompentancy
        {
            public int CompetencyId { get; set; }
            public int CompetencyTypeId { get; set; }
            public string CompetencyName { get; set; }
            public string Description { get; set; }
            public List<CompentencyBehaviours> Behaviours { get; set; }
        }

        /// <summary>
        /// Created By Sneha Patidar
        /// </summary>
        public class GetCompendencyList
        {
            public int CompentencyId { get; set; }
            public string CompetencyName { get; set; }
            public string Description { get; set; }
            public int TotalBehavior { get; set; }
        }

        #endregion Helper Model Classes
    }
}