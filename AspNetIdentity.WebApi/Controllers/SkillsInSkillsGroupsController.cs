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
    /// Created By Ankit
    /// </summary>
    [Authorize]
    [RoutePrefix("api/skillsIn")]
    public class SkillsInSkillsGroupsController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        #region Api's For SkillsInSkillsGroup

        #region Api To Add SkillsInSkillsGroup

        /// <summary>
        /// Created By Ankit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addskillsIN")]
        public async Task<ResponseBodyModel> addskills(AddUpdateSkilinSkillGroupDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<SkillsInSkillsGroup> list = new List<SkillsInSkillsGroup>();
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
                    var skillGroup = db.SkillGroups.FirstOrDefault(x => x.SkillGroupId == model.SkilGroupId);
                    if (skillGroup != null)
                    {
                        var skillcheck = db.SkillIn.ToList().Select(x => x.SkillGroupId).Distinct().ToList();
                        if (!skillcheck.Contains(model.SkilGroupId))
                        {
                            var skillList = await db.Skills.Select(x => x.SkillId).ToListAsync();
                            if (model.SkillIds != null && model.SkillIds.Count > 0)
                            {
                                foreach (var item in model.SkillIds)
                                {
                                    if (skillList.Contains(item))
                                    {
                                        SkillsInSkillsGroup obj = new SkillsInSkillsGroup
                                        {
                                            SkillGroupId = model.SkilGroupId,
                                            SkillId = item,
                                            IsActive = true,
                                            IsDeleted = false,
                                            CreatedBy = claims.userId,
                                            CreatedOn = DateTime.Now,
                                        };
                                        db.SkillIn.Add(obj);
                                        db.SaveChanges();

                                        list.Add(obj);
                                    }
                                }
                                res.Message = "Skills Added";
                                res.Status = true;
                                res.Data = list;
                            }
                            else
                            {
                                res.Message = "You Didnt Selected Any Skill";
                                res.Status = true;
                                res.Data = list;
                            }
                        }
                        else
                        {
                            res.Message = "Skill Group Not Found";
                            res.Data = list;
                            res.Status = true;
                        }
                    }
                    else
                    {
                        res.Message = "Skill Already Added";
                        res.Status = true;
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

        #endregion Api To Add SkillsInSkillsGroup

        #region Api to Get All SkillsInSkillsGroupall List

        /// <summary>
        /// API >> Get >> api/SkillsInSkillsGroupallskill
        /// Created By Ankit
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallSkillsInSkills")]
        public async Task<ResponseBodyModel> getSkillsInSkills()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skillIN = await db.SkillIn.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => new
                {
                    x.Id,
                    x.SkillId,
                    x.SkillGroupName,
                    x.SkillGroupId,
                }).ToListAsync();
                if (skillIN.Count > 0)
                {
                    res.Message = "SkillsInSkills List";
                    res.Status = true;
                    res.Data = skillIN.OrderByDescending(x => x.Id).ToList();
                }
                else
                {
                    res.Message = "SkillsInSkills List is Empty";
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

        #endregion Api to Get All SkillsInSkillsGroupall List

        #region Api To Get SkillsInSkills By Id

        /// <summary>
        /// API >> Get >> api/getSkillsInSkillsbyid
        /// Created By ankit
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getSkillsInSkillsbyid")]
        public async Task<ResponseBodyModel> getSkillsInSkillsbyid(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skill = await db.SkillIn.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true && x.IsDeleted == false);

                if (skill != null)
                {
                    res.Message = "SkillsInSkill List";
                    res.Status = true;
                    res.Data = skill;
                }
                else
                {
                    res.Message = "SkillsInSkill List is Empty";
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

        #endregion Api To Get SkillsInSkills By Id

        #region Api To Edit SkillsInSkill

        /// <summary>
        /// APi >> Post >> api/skillsIn/editSkillsInSkill
        /// Created By Ankit
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editSkillsInSkill")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> EditSkillsInSkill(AddUpdateSkilinSkillGroupDTO model)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
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
                    if (model.SkillIds.Count == 0)
                    {
                        var remove = db.SkillIn.Where(x => x.SkillGroupId == model.SkilGroupId).ToList();
                        foreach (var item in remove)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var allSkillList = db.SkillIn.Where(x => x.SkillGroupId == model.SkilGroupId).ToList();
                        if (allSkillList.Count == 0)
                        {
                            foreach (var item in model.SkillIds)
                            {
                                SkillsInSkillsGroup obj = new SkillsInSkillsGroup
                                {
                                    SkillGroupId = model.SkilGroupId,
                                    SkillId = item,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.userId,
                                    CreatedOn = DateTime.Now,
                                };
                                db.SkillIn.Add(obj);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var allskillids = allSkillList.Select(x => x.SkillId).ToList();
                            foreach (var item in model.SkillIds)
                            {
                                if (!allskillids.Contains(item))
                                {
                                    SkillsInSkillsGroup obj = new SkillsInSkillsGroup
                                    {
                                        SkillGroupId = model.SkilGroupId,
                                        SkillId = item,
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreatedBy = claims.userId,
                                        CreatedOn = DateTime.Now,
                                    };
                                    db.SkillIn.Add(obj);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    allskillids.Remove(item);
                                }
                            }
                            foreach (var item in allskillids)
                            {
                                var removeskill = allSkillList.Where(x => x.SkillId == item).FirstOrDefault();
                                db.Entry(removeskill).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                    }
                    res.Message = "Updated";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Edit SkillsInSkill

        [Route("DeleteSkillsInSkill")]
        [HttpDelete]
        [Authorize]
        public async Task<ResponseBodyModel> RemoveSkillsInSkill(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var skilsGroup = await db.SkillIn.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                if (skilsGroup != null)
                {
                    //skilsGroup.DeletedBy = claims.employeeid;
                    skilsGroup.DeletedOn = DateTime.Now;
                    skilsGroup.IsDeleted = true;
                    skilsGroup.IsActive = false;
                    db.Entry(skilsGroup).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "SkillsInSkill Deleted";
                    res.Status = true;
                    res.Data = skilsGroup;
                }
                else
                {
                    res.Message = "SkillsInSkill Not Found";
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

        #endregion Api's For SkillsInSkillsGroup

        public class AddSkilsInSkillGroupSkillResponse
        {
            public int SkillId { get; set; }
            public string SkillName { get; set; }
            public string SkilsInSkilsGroup { get; set; }
        }

        public class AddUpdateSkilinSkillGroupDTO
        {
            public int SkilGroupId { get; set; }
            public List<int> SkillIds { get; set; }
        }
    }
}