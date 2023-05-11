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
    // <summary>
    /// Created By Kapil Nema  on 08-03-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/skills")]
    public class SkillsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api's For Skills Masters

        #region Api To Add Skills

        /// <summary>
        /// APi >> Post >> api/extramaster/addlocation
        /// Created By Kapil Nema on 08-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addskills")]
        public async Task<ResponseBodyModel> AddSkills(SkillMaster model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    SkillMaster obj = new SkillMaster
                    {
                        SkillName = model.SkillName,
                        Description = model.Description,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                    };
                    _db.Skills.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "skill Added";
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

        #endregion Api To Add Skills

        #region Api to Get All Skill List

        /// <summary>
        /// API >> Get >> api/getallskill
        /// Created By Kapil Nema on 08-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallskill")]
        public async Task<ResponseBodyModel> getallskill()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skill = await _db.Skills.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => new
                {
                    x.SkillId,
                    x.SkillName,
                    x.Description,
                }).ToListAsync();
                if (skill.Count > 0)
                {
                    res.Message = "skillroup List";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "List is Empty";
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

        #endregion Api to Get All Skill List

        #region Api to Get Active skill List

        /// <summary>
        /// API >> Get >> api/getactiveSkill
        /// Created By Kapil Nema on 08-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getactiveskill")]
        public async Task<ResponseBodyModel> getactiveskill()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skill = await _db.Skills.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => new
                {
                    x.SkillId,
                    x.SkillName,
                    x.Description,
                }).ToListAsync();
                if (skill.Count > 0)
                {
                    res.Message = "Skill List";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "List is Empty";
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

        #endregion Api to Get Active skill List

        #region Api To Get Skill By Id

        /// <summary>
        /// API >> Get >> api/getlocationbyid
        /// Created By Kapil Nema on 08-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getskillbyid")]
        public async Task<ResponseBodyModel> getskillbyid(int skillid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == skillid && x.IsActive == true && x.IsDeleted == false);

                if (skill != null)
                {
                    res.Message = "skill List";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "skill List is Empty";
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

        #endregion Api To Get Skill By Id

        #region Api To Edit skill

        /// <summary>
        /// APi >> Post >> api/skill
        /// Created By Kapil Nema on 08-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editskills")]
        public async Task<ResponseBodyModel> editskills(SkillMaster model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == model.SkillId && x.IsActive == true && x.IsDeleted == false);
                    if (skill != null)
                    {
                        skill.SkillName = model.SkillName;
                        //skill.SkillGroupId = model.SkillGroupId;
                        skill.Description = model.Description;
                        skill.UpdatedBy = claims.employeeId;
                        skill.UpdatedOn = DateTime.Now;

                        _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "skill Edited";
                        res.Status = true;
                        res.Data = skill;
                    }
                    else
                    {
                        res.Message = "skill Not Found";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Edit skill

        #region Api To isactivedelete skill

        /// <summary>
        /// APi >> Post >> api/skills
        /// Created By Kapil Nema on 08-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editdeleteativekills")]
        public async Task<ResponseBodyModel> editdeleteativekills(int skillId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == skillId && x.IsActive == true && x.IsDeleted == false);
                if (skill != null)
                {
                    skill.IsActive = false;
                    skill.IsDeleted = true;
                    skill.UpdatedOn = DateTime.Now;
                    skill.DeletedOn = DateTime.Now;

                    _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "skill Edited";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "skill Not Found";
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

        #endregion Api To isactivedelete skill

        #endregion Api's For Skills Masters

        #region Api's For SkillGroup Masters

        #region Api To Add SkillGroup

        /// <summary>
        /// APi >> Post >> api/extramaster/addlocation
        /// Created By Kapil Nema on 07-03-2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("addskillgroup")]
        public async Task<ResponseBodyModel> Addskillgroup(SkillGroup model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    SkillGroup obj = new SkillGroup
                    {
                        SkillGroupName = model.SkillGroupName,
                        Description = model.Description,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.SkillGroups.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "skillgroup Added";
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

        #endregion Api To Add SkillGroup

        #region Api to Get All SkillGroup List

        /// <summary>
        /// API >> Get >> api/getallskillgroup
        /// Created By Kapil Nema on 07-03-2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("getallskillgroup")]
        public async Task<ResponseBodyModel> GetAllActivesskilgroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skillgroup = await _db.SkillGroups.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => new GetSkillsList
                {
                    SkillGroupId = x.SkillGroupId,
                    SkillGroupName = x.SkillGroupName,
                    Description = x.Description,
                    SkilsInSkilsGroup = _db.SkillIn.Where(z => z.SkillGroupId == x.SkillGroupId).Select(z => new SkilsList
                    {
                        SkillsIds = z.SkillId,
                        SkillName = _db.Skills.Where(p => p.SkillId == z.SkillId).Select(p => p.SkillName).FirstOrDefault(),
                    }).ToList(),
                }).ToListAsync();
                if (skillgroup.Count > 0)
                {
                    res.Message = "skillgroup List";
                    res.Status = true;
                    res.Data = skillgroup.OrderByDescending(x => x.SkillGroupId).ToList();
                }
                else
                {
                    res.Message = "List is Empty";
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

        #endregion Api to Get All SkillGroup List

        #region Api to Get Active skillgroup List

        /// <summary>
        /// API >> Get >> api/getactiveSkillGroup
        /// Created By Kapil Nema on 07-03-2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("getactivedeleteskillgroup")]
        public async Task<ResponseBodyModel> GetActiveSkillgroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skillgroup = await _db.SkillGroups.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => new
                {
                    x.SkillGroupId,
                    x.SkillGroupName,
                    x.Description,
                }).ToListAsync();
                if (skillgroup.Count > 0)
                {
                    res.Message = "SkillGroup List";
                    res.Status = true;
                    res.Data = skillgroup;
                }
                else
                {
                    res.Message = "List is Empty";
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

        #endregion Api to Get Active skillgroup List

        #region Api To Get SkillGroup By Id

        /// <summary>
        /// API >> Get >> api/getlocationbyid
        /// Created By Kapil Nema on 07-03-2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("getskillgroupbyid")]
        public async Task<ResponseBodyModel> GetSkillGroupById(int skillgroupid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skillgroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == skillgroupid);
                if (skillgroup != null)
                {
                    res.Message = "skillgroup List";
                    res.Status = true;
                    res.Data = skillgroup;
                }
                else
                {
                    res.Message = "skillgroup List is Empty";
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

        #endregion Api To Get SkillGroup By Id

        #region Api To Edit skillgroup

        /// <summary>
        /// APi >> Post >> api/skillgroup
        /// Created By Kapil Nema on 07-03-2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("editskilgroup")]
        public async Task<ResponseBodyModel> EditSkillGroup(SkillGroup model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    var skillgroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == model.SkillGroupId);
                    if (skillgroup != null)
                    {
                        skillgroup.SkillGroupName = model.SkillGroupName;
                        skillgroup.Description = model.Description;
                        skillgroup.UpdatedBy = claims.employeeId;
                        skillgroup.UpdatedOn = DateTime.Now;

                        _db.Entry(skillgroup).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "skillgroup Edited";
                        res.Status = true;
                        res.Data = skillgroup;
                    }
                    else
                    {
                        res.Message = "skillgroup Not Found";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Edit skillgroup

        #endregion Api's For SkillGroup Masters

        /// <summary>
        ///
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("Deleteskilgroup")]
        [HttpDelete]
        [Authorize]
        public async Task<ResponseBodyModel> RemoveSkilsGroup(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skilsGroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == Id && x.IsDeleted == false);
                if (skilsGroup != null)
                {
                    //skilsGroup.DeletedBy = claims.employeeid;
                    skilsGroup.DeletedOn = DateTime.Now;
                    skilsGroup.IsDeleted = true;
                    skilsGroup.IsActive = false;
                    _db.Entry(skilsGroup).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "skils Group Delete";
                    res.Status = true;
                    res.Data = skilsGroup;
                }
                else
                {
                    res.Message = "skils Group Not Found";
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

        //#region Api To AddUserRatingSkill

        ///// <summary>
        ///// APi >> Post >> api/extramaster/addlocation
        ///// Created By Kapil Nema on 11-03-2022
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[Route("adduserratingskill")]
        //public async Task<ResponseBodyModel> adduserratingskill(AddUserRatingSkill model)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;
        //        // Access claims
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //        if (model == null)
        //        {
        //            res.Message = "Model is Invalid";
        //            res.Status = false;
        //        }
        //        else
        //        {
        //            AddUserRatingSkill obj = new AddUserRatingSkill
        //            {
        //                UserId = userid,
        //                SkillId = model.SkillId,
        //                SkillName = model.SkillName,
        //                Rating = model.Rating,
        //                IsActive = true,
        //                IsDeleted = false,
        //                CreatedOn = DateTime.Now,
        //                CreatedBy = userid
        //            };
        //            _db.AddUserRatingSkill.Add(obj);
        //            await _db.SaveChangesAsync();

        //            res.Message = "addUserratingskill added";
        //            res.Status = true;
        //            res.Data = obj;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Api To AddUserRatingSkill        //#region Api To AddUserRatingSkill

        ///// <summary>
        ///// APi >> Post >> api/extramaster/addlocation
        ///// Created By Kapil Nema on 11-03-2022
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[Route("adduserratingskill")]
        //public async Task<ResponseBodyModel> adduserratingskill(AddUserRatingSkill model)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;
        //        // Access claims
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //        if (model == null)
        //        {
        //            res.Message = "Model is Invalid";
        //            res.Status = false;
        //        }
        //        else
        //        {
        //            AddUserRatingSkill obj = new AddUserRatingSkill
        //            {
        //                UserId = userid,
        //                SkillId = model.SkillId,
        //                SkillName = model.SkillName,
        //                Rating = model.Rating,
        //                IsActive = true,
        //                IsDeleted = false,
        //                CreatedOn = DateTime.Now,
        //                CreatedBy = userid
        //            };
        //            _db.AddUserRatingSkill.Add(obj);
        //            await _db.SaveChangesAsync();

        //            res.Message = "addUserratingskill added";
        //            res.Status = true;
        //            res.Data = obj;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Api To AddUserRatingSkill

        //#region Api To updateskillrating

        ///// <summary>
        ///// APi >> Post >> api/skill
        ///// Created By Kapil Nema on 11-03-2022
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPut]
        //[Route("updateskillrating")]
        //public async Task<ResponseBodyModel> updateskillrating(AddUserRatingSkill model)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;
        //        //Access claims
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid")) ;
        //        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //        if (model == null)
        //        {
        //            res.Message = "Model is Invalid";
        //            res.Status = false;
        //        }
        //        var skill = await _db.AddUserRatingSkill.FirstOrDefaultAsync(x => x.RatingSkillId == model.RatingSkillId);
        //        if (skill != null)
        //        {
        //            skill.SkillName = model.SkillName;
        //            skill.SkillId = model.SkillId;
        //            skill.Rating = model.Rating;
        //            skill.UpdatedBy = userid;
        //            skill.UpdatedOn = DateTime.Now;

        //            _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
        //            await _db.SaveChangesAsync();

        //            res.Message = "skillgroup Edited";
        //            res.Status = true;
        //            res.Data = skill;
        //        }
        //        else
        //        {
        //            res.Message = "skillgroup Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Api To updateskillrating

        #region Api to Delete Rating skill List

        /// <summary>
        /// API >> Get >> api/getactiveSkillGroup
        /// Created By Kapil Nema on 11-03-2022
        /// </summary>
        /// <returns></returns>
        [Route("DeleteSkils")]
        [HttpDelete]
        [Authorize]
        public async Task<ResponseBodyModel> RemoveSkils(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skilsGroup = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == Id && x.IsDeleted == false);
                if (skilsGroup != null)
                {
                    //skilsGroup.DeletedBy = claims.employeeid;
                    skilsGroup.DeletedOn = DateTime.Now;
                    skilsGroup.IsDeleted = true;
                    skilsGroup.IsActive = false;
                    _db.Entry(skilsGroup).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "skils Delete";
                    res.Status = true;
                    res.Data = skilsGroup;
                }
                else
                {
                    res.Message = "skils  Not Found";
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

        #endregion Api to Delete Rating skill List

        //#region Api To Add Skills In Particular Skill Group

        //[HttpPost]
        //[Route("addskillsinskillgroup")]
        //public async Task<ResponseBodyModel> addskillsinskillgroup(AddSkilsInSkillGroup model)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    AddSkilsInSkillGroupResponse response = new AddSkilsInSkillGroupResponse();
        //    List<AddSkilsInSkillGroupSkillResponse> skillList = new List<AddSkilsInSkillGroupSkillResponse>();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        if (model == null)
        //        {
        //            res.Message = "Model Is Invalid";
        //            res.Status = false;
        //        }
        //        else
        //        {
        //            var skillGroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == model.SkillGroupId);
        //            if (skillGroup != null)
        //            {
        //                if (model.SkillsIds.Count > 0)
        //                {
        //                    foreach (var item in model.SkillsIds)
        //                    {
        //                        var skill = _db.Skills.FirstOrDefault(x => x.SkillId == item);
        //                        if (skill != null)
        //                        {
        //                            skill.SkillGroupId = skillGroup.SkillGroupId;
        //                            skill.UpdatedBy = claims.userid;
        //                            skill.UpdatedOn = DateTime.Now;
        //                            _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
        //                            await _db.SaveChangesAsync();

        //                            AddSkilsInSkillGroupSkillResponse obj = new AddSkilsInSkillGroupSkillResponse()
        //                            {
        //                                SkillId = skill.SkillId,
        //                                SkillName = skill.SkillName
        //                            };
        //                            skillList.Add(obj);
        //                        }
        //                    }
        //                    response.SkillGroupId = skillGroup.SkillGroupId;
        //                    response.SkillName = skillGroup.SkillGroupName;
        //                    response.SkillsIds = skillList;

        //                    res.Message = "Skills Added In Skill Group";
        //                    res.Status = true;
        //                    res.Data = response;
        //                }
        //                else
        //                {
        //                    res.Message = "Skill List Is Empty";
        //                    res.Status = false;
        //                }
        //            }
        //            else
        //            {
        //                res.Message = "Skill Group Not Found";
        //                res.Status = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        #region Helper Model Classes

        public class AddSkilsInSkillGroup
        {
            public int SkillGroupId { get; set; }
            public List<int> SkillsIds { get; set; }
        }

        public class AddSkilsInSkillGroupResponse
        {
            public int SkillGroupId { get; set; }
            public string SkillName { get; set; }
            public List<AddSkilsInSkillGroupSkillResponse> SkillsIds { get; set; }
            public string SkilsInSkilsGroup { get; set; }
        }

        public class AddSkilsInSkillGroupSkillResponse
        {
            public int SkillId { get; set; }
            public string SkillName { get; set; }
            public string SkilsInSkilsGroup { get; set; }
        }

        public class SkilsList
        {
            public int SkillsIds { get; set; }
            public string SkillName { get; set; }
        }

        public class GetSkillsList
        {
            public int SkillGroupId { get; set; }
            public string SkillGroupName { get; set; }
            public string Description { get; set; }
            public List<SkilsList> SkilsInSkilsGroup { get; set; }
        }

        #endregion Helper Model Classes
    }
}