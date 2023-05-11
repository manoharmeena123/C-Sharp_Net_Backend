using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.SkillModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Skill
{
    [Authorize]
    [RoutePrefix("api/skill")]
    public class SkillMaterNewController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api's For Skills Masters

        #region Api To Add Skills

        /// <summary>
        /// API >> Post >> api/skill/addskill
        /// Updated By Chandra Prakash Rawat  on 07-06-2022
        ///Modify By Shriya Malvi On 04-08-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addskill")]
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
                    var check = await _db.Skills.AnyAsync(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                                x.IsDeleted == false && x.SkillName.ToLower().Trim() == model.SkillName.ToLower().Trim());
                    if (check)
                    {
                        res.Message = "Skill Already Exist";
                        res.Status = false;
                        return res;
                    }
                    SkillMaster obj = new SkillMaster
                    {
                        SkillName = model.SkillName,
                        Description = model.Description,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                        SkillRequestId = 0,
                    };
                    _db.Skills.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Skill Added";
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
        /// API >> Get >> api/skill/getallskill
        /// Updated By Chandra Prakash Rawat  on 07-06-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallskill")]
        public async Task<ResponseBodyModel> Getallskill()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skill = await _db.Skills.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.SkillId,
                            x.SkillName,
                            x.Description,
                        }).ToListAsync();
                if (skill.Count > 0)
                {
                    res.Message = "Skill List";
                    res.Status = true;
                    res.Data = skill.OrderByDescending(x => x.SkillId).ToList();
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

        #region Api To Get Skill By Id

        /// <summary>
        /// API >> Get >> api/skill/getskillbyid
        /// Updated By Chandra Prakash Rawat  on 07-06-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getskillbyid")]
        public async Task<ResponseBodyModel> Getskillbyid(int skillId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == skillId &&
                        x.IsActive == true && x.IsDeleted == false && claims.companyId == x.CompanyId);
                if (skill != null)
                {
                    res.Message = "Skill List";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "Skill List is Empty";
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
        /// API >> Put >> api/skill/editskills
        /// Updated By Chandra Prakash Rawat  on 07-06-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editskills")]
        public async Task<ResponseBodyModel> Editskills(SkillMaster model)
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
                    var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == model.SkillId &&
                            x.IsActive == true && x.IsDeleted == false && claims.companyId == x.CompanyId);
                    if (skill != null)
                    {
                        //var check = await _db.Skills.AnyAsync(x => x.CompanyId == claims.companyid && x.IsActive == true &&
                        //        x.IsDeleted == false && x.SkillName.ToLower().Trim() == model.SkillName.ToLower().Trim());
                        //if (check)
                        //{
                        //    res.Message = "Skill Already Exist";
                        //    res.Status = false;
                        //    return res;
                        //}
                        skill.SkillName = model.SkillName;
                        skill.Description = model.Description;
                        skill.UpdatedBy = claims.employeeId;
                        skill.UpdatedOn = DateTime.Now;

                        _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Skill Updated";
                        res.Status = true;
                        res.Data = skill;
                    }
                    else
                    {
                        res.Message = "Skill Not Found";
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

        #region Api To Delete Skill

        /// <summary>
        /// API >> Put >> api/skill/deleteskill
        /// Updated By Chandra Prakash Rawat  on 07-06-2022
        /// Modify By Shriya Malvi On 04-08-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("deleteskill")]
        public async Task<ResponseBodyModel> Editdeleteativekills(int skillId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == skillId
                            && x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false);
                if (skill != null)
                {
                    if (skill.SkillRequestId != 0)
                    {
                        var skillReqdata = _db.SkillRequests.Where(x => x.RequestId == skill.SkillRequestId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
                        if (skillReqdata != null)
                        {
                            skillReqdata.IsActive = false;
                            skillReqdata.IsDeleted = true;
                            skillReqdata.DeletedBy = claims.employeeId;
                            skillReqdata.DeletedOn = DateTime.Now;

                            _db.Entry(skillReqdata).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                    skill.IsActive = false;
                    skill.IsDeleted = true;
                    skill.DeletedBy = claims.employeeId;
                    skill.DeletedOn = DateTime.Now;

                    _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "skill Deleted";
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

        #endregion Api To Delete Skill

        #endregion Api's For Skills Masters

        #region Api's For SkillGroup Masters

        #region Api To Add SkillGroup

        /// <summary>
        /// APi >> Post >> api/skill/addskillgroup
        /// Updated By Chandra Prakash Rawat on 07-06-2022
        /// </summary>
        /// <returns></returns>
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
                    var check = await _db.SkillGroups.AnyAsync(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                                x.IsDeleted == false && x.SkillGroupName.ToLower().Trim() == model.SkillGroupName.ToLower().Trim());
                    if (check)
                    {
                        res.Message = "Skill Group Already Exist";
                        res.Status = false;
                        return res;
                    }
                    SkillGroup obj = new SkillGroup
                    {
                        SkillGroupName = model.SkillGroupName,
                        Description = model.Description,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                    };
                    _db.SkillGroups.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Skill Group Added";
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

        #region Api To Edit skillgroup

        /// <summary>
        /// API >> Put >> api/skill/editskilgroup
        /// Updated By Chandra Prakash Rawat on 07-06-2022
        /// </summary>
        /// <returns></returns>
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
                    var skillgroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == model.SkillGroupId
                                && x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId);
                    if (skillgroup != null)
                    {
                        var check = await _db.SkillGroups.AnyAsync(x => x.CompanyId == claims.companyId && x.IsActive == true &&
                                x.IsDeleted == false && x.SkillGroupName.ToLower().Trim() == model.SkillGroupName.ToLower().Trim());
                        if (check)
                        {
                            res.Message = "Skill Group Already Exist";
                            res.Status = false;
                            return res;
                        }
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

        #region Api To Delete Skillgroup

        /// <summary>
        /// APi >> Delete >> api/skills/deleteskilgroup
        /// Updated By Chandra Prakash Rawat on 07-06-2022
        /// </summary>
        /// <param name="skillGroupId"></param>
        /// <returns></returns>
        [Route("deleteskilgroup")]
        [HttpDelete]
        public async Task<ResponseBodyModel> RemoveSkilsGroup(int skillGroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skilsGroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == skillGroupId && x.IsDeleted == false
                            && x.CompanyId == claims.companyId && x.IsActive == true);
                if (skilsGroup != null)
                {
                    skilsGroup.DeletedOn = DateTime.Now;
                    skilsGroup.IsDeleted = true;
                    skilsGroup.IsActive = false;
                    _db.Entry(skilsGroup).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Skill Group Delete";
                    res.Status = true;
                    res.Data = skilsGroup;
                }
                else
                {
                    res.Message = "Skill Group Not Found";
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

        #endregion Api To Delete Skillgroup

        #endregion Api's For SkillGroup Masters

        #region Api To AddUserRatingSkill

        /// <summary>
        /// APi >> Post >> api/skills/adduserratingskill
        /// Created By Kapil Nema on 11-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("adduserratingskill")]
        public async Task<ResponseBodyModel> Adduserratingskill(AddUserRatingSkill model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
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
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    AddUserRatingSkill obj = new AddUserRatingSkill
                    {
                        UserId = userid,
                        SkillId = model.SkillId,
                        SkillName = model.SkillName,
                        Rating = model.Rating,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userid
                    };
                    _db.AddUserRatingSkill.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "addUserratingskill added";
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

        #endregion Api To AddUserRatingSkill

        #region Api To Updateskillrating

        /// <summary>
        /// API >> Put >> api/skill/updateskillrating
        /// Created By Kapil Nema on 11-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("updateskillrating")]
        public async Task<ResponseBodyModel> Updateskillrating(AddUserRatingSkill model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                int userid = 0;
                int compid = 0;
                int orgid = 0;
                //Access claims
#pragma warning disable CS0642 // Possible mistaken empty statement
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid")) ;
#pragma warning restore CS0642 // Possible mistaken empty statement
                userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                var skill = await _db.AddUserRatingSkill.FirstOrDefaultAsync(x => x.RatingSkillId == model.RatingSkillId);
                if (skill != null)
                {
                    skill.SkillName = model.SkillName;
                    skill.SkillId = model.SkillId;
                    skill.Rating = model.Rating;
                    skill.UpdatedBy = userid;
                    skill.UpdatedOn = DateTime.Now;

                    _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "skillgroup Edited";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "skillgroup Not Found";
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

        #endregion Api To Updateskillrating

        #region Api to Delete Rating skill List

        /// <summary>
        /// API >> DELETE >> api/skill/deleteSkils
        /// Updated By Chandra Prakash Rawat  on 07-06-2022
        /// </summary>
        /// <returns></returns>
        [Route("deleteSkils")]
        [HttpDelete]
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

        #region Api's For SkillInSkillsGroup

        #region Api to Add and Update SkillInGroup

        /// <summary>
        /// API >> POST >> api/skill/addSkillInSkillGroup
        /// Created By Chandra Prakash Rawat on 07-06-2022
        /// Modify By Harshit Mitra on 13-07-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addSkillInSkillGroup")]
        public async Task<ResponseBodyModel> AddSkilsIn(AddSkilsInSkillGroup model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    if (model.SkillsIds != null || model.SkillsIds.Count > 0)
                    {
                        var skillCheck = await _db.SkillIn.Where(x => x.SkillGroupId == model.SkillGroupId).ToListAsync();
                        if (skillCheck.Count > 0)
                        {
                            _db.SkillIn.RemoveRange(skillCheck);
                            await _db.SaveChangesAsync();
                            foreach (var Item in model.SkillsIds)
                            {
                                SkillsInSkillsGroup Obj = new SkillsInSkillsGroup
                                {
                                    SkillGroupId = model.SkillGroupId,
                                    SkillId = Item,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId
                                };
                                _db.SkillIn.Add(Obj);
                                await _db.SaveChangesAsync();
                            }
                            res.Message = "SkillIn Updated Successful";
                            res.Status = true;
                        }
                        else
                        {
                            foreach (var Item in model.SkillsIds)
                            {
                                SkillsInSkillsGroup Obj = new SkillsInSkillsGroup
                                {
                                    SkillGroupId = model.SkillGroupId,
                                    SkillId = Item,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId
                                };
                                _db.SkillIn.Add(Obj);
                                await _db.SaveChangesAsync();
                            }
                            res.Message = "SkillIn Added Successful";
                            res.Status = true;
                        }
                    }
                    else
                    {
                        res.Message = "You Have To add Atleats one skill";
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

        #endregion Api to Add and Update SkillInGroup

        #region This Api Use For Get By Id In SkillIn Skill Group

        /// <summary>
        /// Created By ankit Date - 13/07/2022
        /// Api >> Get >> api/skill/getallskillgroup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallskillgroup")]
        public async Task<ResponseBodyModel> GetSkillsAll(int skillGroupId = 0)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (skillGroupId == 0)
                {
                    var skillData = await _db.SkillGroups.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId)
                                .Select(x => new
                                {
                                    x.SkillGroupId,
                                    x.SkillGroupName,
                                    x.Description,
                                    SkillsList = (from s in _db.Skills
                                                  join sk in _db.SkillIn on s.SkillId equals sk.SkillId
                                                  where s.IsActive == true && s.IsDeleted == false && sk.SkillGroupId == x.SkillGroupId
                                                  select new
                                                  {
                                                      sk.SkillGroupId,
                                                      s.SkillId,
                                                      s.SkillName,
                                                  }).ToList(),
                                }).ToListAsync();
                    var data = skillData
                        .Select(x => new
                        {
                            x.SkillGroupId,
                            x.SkillGroupName,
                            x.Description,
                            SkillsId = x.SkillsList.Where(z => z.SkillGroupId == x.SkillGroupId).Select(z => z.SkillId).ToList(),
                            SkillsName = x.SkillsList.Where(z => z.SkillGroupId == x.SkillGroupId).Select(z => z.SkillName),
                        });
                    if (skillData.Count > 0)
                    {
                        res.Message = "Skill In Skill Group Found";
                        res.Status = true;
                        res.Data = data;
                    }
                    else
                    {
                        res.Message = "Data Not Found";
                        res.Status = false;
                        res.Data = data;
                    }
                }
                else
                {
                    var skillData = await _db.SkillGroups.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                x.CompanyId == claims.companyId && x.SkillGroupId == skillGroupId)
                                .Select(x => new
                                {
                                    x.SkillGroupId,
                                    x.SkillGroupName,
                                    SkillsList = (from s in _db.Skills
                                                  join sk in _db.SkillIn on s.SkillId equals sk.SkillId
                                                  where s.IsActive == true && s.IsDeleted == false && sk.SkillGroupId == x.SkillGroupId
                                                  select new
                                                  {
                                                      s.SkillId,
                                                      s.SkillName,
                                                  }).ToList(),
                                }).ToListAsync();
                    if (skillData.Count > 0)
                    {
                        res.Message = "Skills Data Found";
                        res.Status = true;
                        res.Data = skillData;
                    }
                    else
                    {
                        res.Message = "Data Not Found";
                        res.Status = false;
                        res.Data = skillData;
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

        #endregion This Api Use For Get By Id In SkillIn Skill Group

        #region Update SkillInSkillGroup

        /// <summary>
        /// API >> PUT >> api/skill/putSkillInSkillGroup
        /// Created By Chandra Prakash Rawat on 07-06-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("putSkillInSkillGroup")]
        public async Task<ResponseBodyModel> UpdateSkillIn(AddSkilsInSkillGroup model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var skillsInSkillsGroup = _db.SkillIn.Where(x => x.SkillGroupId == model.SkillGroupId).FirstOrDefault();

                foreach (var skillsId in model.SkillsIds)
                {
                    skillsInSkillsGroup.SkillId = skillsId;

                    _db.Entry(skillsInSkillsGroup).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Record Updated successful";
                    res.Data = skillsInSkillsGroup;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion Update SkillInSkillGroup

        #endregion Api's For SkillInSkillsGroup

        #region Api To Add Skills In Particular Skill Group

        [HttpPost]
        [Route("addskillsinskillgroup")]
        public async Task<ResponseBodyModel> addskillsinskillgroup(AddSkilsInSkillGroup model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AddSkilsInSkillGroupResponse response = new AddSkilsInSkillGroupResponse();
            List<AddSkilsInSkillGroupSkillResponse> skillList = new List<AddSkilsInSkillGroupSkillResponse>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    var skillGroup = await _db.SkillGroups.FirstOrDefaultAsync(x => x.SkillGroupId == model.SkillGroupId);
                    if (skillGroup != null)
                    {
                        if (model.SkillsIds.Count > 0)
                        {
                            foreach (var item in model.SkillsIds)
                            {
                                var skill = _db.Skills.FirstOrDefault(x => x.SkillId == item);
                                if (skill != null)
                                {
                                    skill.UpdatedBy = claims.userId;
                                    skill.UpdatedOn = DateTime.Now;
                                    _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                                    await _db.SaveChangesAsync();

                                    AddSkilsInSkillGroupSkillResponse obj = new AddSkilsInSkillGroupSkillResponse()
                                    {
                                        SkillId = skill.SkillId,
                                        SkillName = skill.SkillName
                                    };
                                    skillList.Add(obj);
                                }
                            }
                            response.SkillGroupId = skillGroup.SkillGroupId;
                            response.SkillName = skillGroup.SkillGroupName;
                            response.SkillsIds = skillList;

                            res.Message = "Skills Added In Skill Group";
                            res.Status = true;
                            res.Data = response;
                        }
                        else
                        {
                            res.Message = "Skill List Is Empty";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "Skill Group Not Found";
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

        #endregion Api To Add Skills In Particular Skill Group

        #region Api's For Skills Request

        /// <summary>
        /// created by Mayank Prajapati on 19/7/2022
        /// Api >> Post >>api/skill/skillrequestpost
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("skillrequestpost")]
        [Authorize]
        public async Task<ResponseBodyModel> RequstPost(SkillRequest Item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = _db.SkillRequests.Where(x => x.SkillName == Item.SkillName && x.IsActive == true && x.IsDeleted == false && claims.companyId == x.CompanyId).FirstOrDefault();

                if (data != null)
                {
                    res.Message = "Duplicate Data";
                    res.Status = false;
                }
                else
                {
                    SkillRequest post = new SkillRequest
                    {
                        SkillName = Item.SkillName,
                        // RequestId = Item.RequestId,
                        //SkillId = Item.SkillId,
                        Status = "Pending",
                        Description = Item.Description,
                        RequestById = claims.employeeId,
                        RequestedId = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault(),
                        ApprovedById = 0,
                        ApprovedBy = Item.ApprovedBy,
                        RequestOn = DateTime.Now,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.SkillRequests.Add(post);
                    await _db.SaveChangesAsync();
                    res.Message = "Post Added";
                    res.Status = true;
                    res.Data = post;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion Api's For Skills Request

        #region Get Skill Request

        /// <summary>
        /// Updated by Shubham Sharma on 24/07/2022
        /// Api >> Post >> api/skill/skillrequestget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("skillrequestget")]
        public async Task<ResponseBodyModel> GetSkillRequest()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            SkillRequestStatusModel req = new SkillRequestStatusModel();
            try
            {
                var skillRequestData = await _db.SkillRequests.Where(x => x.IsActive == true && x.IsDeleted == false && claims.companyId == x.CompanyId).ToListAsync();
                req.SkillRequestNotify = skillRequestData.ToList();
                req.ApproveRequest = skillRequestData.Where(x => x.Status == "Approved").ToList();
                req.RejectRequest = skillRequestData.Where(x => x.Status == "Rejected").ToList();
                req.PendingRequest = skillRequestData.Where(x => x.Status == "Pending").ToList();
                req.ApproveRejectList = skillRequestData.Where(x => x.Status == "Approved" || x.Status == "Rejected").ToList();
                if (skillRequestData.Count > 0)
                {
                    res.Message = "Get Data successfully";
                    res.Status = true;
                    res.Data = req;
                }
                else
                {
                    res.Message = "Failed To Post";
                    res.Status = false;
                    res.Data = skillRequestData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Skill Request

        #region Get Skill Request

        /// <summary>
                /// created by Mayank Prajapati on 19/7/2022
                /// Api >> Post >> api/skill/skillrequestget
                /// </summary>
                /// <param name="model"></param>
                /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("skillrequestgetalldata")]
        public async Task<ResponseBodyModel> GetSkillRequestAllData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var WallData = await _db.SkillRequests.Where(x => x.IsDeleted == false && claims.companyId == x.CompanyId).ToListAsync();
                if (WallData.Count > 0)
                {
                    res.Message = "Get Data";
                    res.Status = true;
                    res.Data = WallData.OrderByDescending(x => x.SkillId).ToList();
                }
                else
                {
                    res.Message = "Failed To Post";
                    res.Status = false;
                    res.Data = WallData;
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

        #region Get by Skill Requests By Status

        /// <summary>
        /// API >> Get >> api/skill/GetAllSkillRequestsByStatus
        /// Created By Shubham Sharma On 23-07-2022
        /// (Apply used  to claims here )
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        [Route("GetAllSkillRequestsByStatus")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllSkillRequestsByStatus(string Status)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var SkillRequestData = await _db.SkillRequests.Where(x => x.IsDeleted == false && x.Status.Trim().ToUpper() == Status.Trim().ToUpper() && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                if (SkillRequestData.Count != 0)
                {
                    res.Status = true;
                    res.Message = "All  Skill Request list Found";
                    res.Data = SkillRequestData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No  Skill Request list Found";
                    res.Data = SkillRequestData;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get by Skill Requests By Status

        #region Edit SkillRequest Data

        /// <summary>
        /// created by Mayank Prajapati on 19/7/2022
        /// Api >> Post >> api/skill/skillrequestput
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("skillrequestput")]
        public async Task<ResponseBodyModel> PutSkillRequest(SkillRequest Model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Add = await _db.SkillRequests.FirstOrDefaultAsync(x => x.RequestId == Model.RequestId);
                if (Add != null)
                {
                    Add.SkillName = Model.SkillName;
                    Add.RequestId = Model.RequestId;
                    Add.Status = Model.Status;
                    Add.Description = Model.Description;
                    Add.RequestedId = Model.RequestedId;
                    Add.ApprovedBy = Model.ApprovedBy;
                    Add.RequestOn = DateTime.Now;
                    Add.ApprovedOn = DateTime.Now;
                    Add.UpdatedBy = claims.employeeId;
                    Add.CompanyId = claims.companyId;
                    Add.OrgId = claims.employeeId;
                    Add.UpdatedOn = DateTime.Now;
                    Add.IsActive = true;
                    Add.IsDeleted = false;

                    _db.Entry(Add).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Data UpDated";
                    res.Status = true;
                    res.Data = Add;
                }
                else
                {
                    res.Message = " Failed To Update";
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

        #endregion Edit SkillRequest Data

        #region Delete Skill data

        /// <summary>
        /// created by Mayank Prajapati on 19/7/2022
        /// Api >> Post >> api/skill/skillrequestdelete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("skillrequestdelete")]
        public async Task<ResponseBodyModel> RemoveSkillRequest(int RequestId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Data = await _db.SkillRequests.FirstOrDefaultAsync(x =>
                    x.RequestId == RequestId && x.IsDeleted == false && x.IsActive == true);
                if (Data != null)
                {
                    var skillData = _db.Skills.Where(x => x.SkillRequestId == Data.RequestId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
                    if (skillData != null)
                    {
                        skillData.IsDeleted = true;
                        skillData.IsActive = false;
                        skillData.DeletedBy = claims.employeeId;
                        skillData.DeletedOn = DateTime.Now;
                        _db.Entry(skillData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    Data.IsDeleted = true;
                    Data.IsActive = false;
                    Data.DeletedBy = claims.employeeId;
                    Data.DeletedOn = DateTime.Now;

                    _db.Entry(Data).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Data Deleted Successfully!";
                    res.Data = Data;
                }
                else
                {
                    res.Status = false;
                    res.Message = " Data Not Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Delete Skill data

        #region Approve Reject Skill Request API

        /// <summary>
        /// Created by Shubham Sharma On 24/07/2022
        /// Modify By Shriya Malvi On 04-08-2022
        /// </summary>
        /// <param name="updateskillRequest"></param>
        /// <returns></returns>
        [Route("skillrequeststatusupdate")]
        [HttpPut]
        public async Task<ResponseBodyModel> SkillRequestStatusUpdate(SkillRequest updateskillRequest)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateData = await _db.SkillRequests.Where(x => x.RequestId == updateskillRequest.RequestId && x.IsDeleted == false && claims.companyId == x.CompanyId).FirstOrDefaultAsync();
                if (updateData != null)
                {
                    if (updateskillRequest.Status == "Rejected")
                    {
                        var skill = await _db.Skills.FirstOrDefaultAsync(x => x.SkillId == updateData.ApprovedSkillId);
                        if (skill != null)
                        {
                            skill.IsActive = false;
                            skill.IsDeleted = true;
                            skill.DeletedBy = claims.employeeId;
                            skill.DeletedOn = DateTime.Now;

                            _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        updateData.IsActive = false;
                        updateData.IsDeleted = true;
                        updateData.DeletedBy = claims.employeeId;
                        updateData.DeletedOn = DateTime.Now;

                        _db.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Skills Request Updated";
                        res.Status = true;
                    }
                }
                else
                {
                    res.Message = "Skill Request Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Approve Reject Skill Request API

        #region Skill APPROVE

        /// <summary>
        /// Modify by Shriya Malvi On 04-08-2022
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SkillApprovePost")]
        [Authorize]
        public async Task<ResponseBodyModel> SkillApprovePost(SkillRequest Item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = _db.SkillRequests.Where(x => x.RequestId == Item.RequestId).FirstOrDefault();
                if (Item.Status == "Approved")
                {
                    var Data = _db.Skills.Where(x => x.SkillName == Item.SkillName && x.CompanyId ==
                                claims.companyId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                    if (Data != null)
                    {
                        res.Message = "Duplicate Data";
                        res.Status = false;
                    }
                    else
                    {
                        SkillMaster post = new SkillMaster
                        {
                            ApprovedId = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault(),
                            SkillName = data.SkillName,
                            SkillId = _db.Skills.Where(x => x.SkillId == Item.SkillId).Select(x => x.SkillId).FirstOrDefault(),
                            Description = data.Description,
                            CreatedBy = claims.employeeId,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            SkillRequestId = data.RequestId,
                        };

                        _db.Skills.Add(post);
                        await _db.SaveChangesAsync();

                        data.ApprovedById = claims.employeeId;
                        data.ApprovedBy = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault();
                        data.ApprovedSkillId = post.SkillId;
                        data.Status = "Approved";
                        _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Skill Approved Succesfully";
                        res.Status = true;
                        res.Data = data;
                    }
                }
                else
                {
                    data.ApprovedSkillId = 0;
                    data.Status = "Rejected";
                    _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Skill Request Rejected";
                    res.Status = true;
                    res.Data = data;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion Skill APPROVE

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

        public class SkillRequestStatusModel
        {
            public List<SkillRequest> SkillRequestNotify { get; set; }
            public List<SkillRequest> PendingRequest { get; set; }
            public List<SkillRequest> ApproveRequest { get; set; }
            public List<SkillRequest> RejectRequest { get; set; }
            public List<SkillRequest> ApproveRejectList { get; set; }
            public int PendingCount { get; set; }
        }

        #endregion Helper Model Classes
    }
}