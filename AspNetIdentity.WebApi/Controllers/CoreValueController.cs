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
    [Authorize]
    [RoutePrefix("api/CoreValue")]
    public class CoreValueController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api For Add CoreValue

        /// <summary>
        /// api/CoreValue/AddCoreValues
        /// Created On 14-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddCoreValues")]
        public async Task<ResponseBodyModel> AddCoreValues(AddCoreValue model)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            if (model != null)
            {
                var identity = User.Identity as ClaimsIdentity;
                int userid = 0, compid = 0, orgid = 0;

                // Access claims
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

                CoreValue obj = new CoreValue
                {
                    CoreValueName = model.CoreValueName,
                    Description = model.Description,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userid,
                    CompanyId = compid,
                    OrgId = orgid,
                    CreatedOn = DateTime.Now,
                };
                _db.CoreValues.Add(obj);
                await _db.SaveChangesAsync();

                if (model.Behaviours.Count > 0)
                {
                    foreach (var item in model.Behaviours)
                    {
                        CoreValueBehaviour beh = new CoreValueBehaviour
                        {
                            CoreValueId = obj.CoreValueId,
                            BehavioursName = item.BehavioursName,
                            UseInRating = item.UseInRating,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userid,
                            CompanyId = compid,
                            OrgId = orgid,
                            CreatedOn = DateTime.Now,
                        };
                        _db.CoreValueBehaviours.Add(beh);
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

        #endregion Api For Add CoreValue

        #region Api For Get All Corevalue

        /// <summary>
        /// api/compentency/getallcorevalue
        /// Created On 15-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcorevalue")]
        public async Task<ResponseBodyModel> GetAllCorevalue()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetCoreValueList> list = new List<GetCoreValueList>();
            try
            {
                var corevalue = await _db.CoreValues.Where(s => s.IsActive == true && s.IsDeleted == false).ToListAsync();
                if (corevalue.Count > 0)
                {
                    foreach (var item in corevalue)
                    {
                        GetCoreValueList obj = new GetCoreValueList
                        {
                            CoreValueId = item.CoreValueId,
                            CoreValueName = item.CoreValueName,
                            Description = item.Description,
                            TotalBehavior = _db.CoreValueBehaviours.Where(x => x.CoreValueId == item.CoreValueId).Count(),
                        };
                        list.Add(obj);
                    }
                }
                if (list.Count > 0)
                {
                    res.Message = "CoreValue List";
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

        #endregion Api For Get All Corevalue

        #region Api For Get by Id

        /// <summary>
        /// api/corevalue/getbyId
        /// Created On 15-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyid")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> getbyid(int CorevalueId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var corevalue = _db.CoreValues.Where(s => s.IsActive == true && s.IsDeleted == false && s.CoreValueId == CorevalueId).FirstOrDefault();
                if (corevalue != null)
                {
                    var list = _db.CoreValueBehaviours.Where(x => x.CoreValueId == corevalue.CoreValueId).ToList();
                    if (list.Count > 0)
                    {
                        res.Message = "CoreValue List";
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
                else
                {
                    res.Message = "CorevalueId not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = "CorevalueId not found";
                res.Status = false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return res;
        }

        #endregion Api For Get by Id

        #region Api For Delete CoreValue

        /// <summary>
        /// api/compentency/deletecorevalue
        /// Created On 7-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecorevalue")]
        public async Task<ResponseBodyModel> DeleteCoreValue(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            CoreValue data = new CoreValue();
            if (Id > 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    var corevalue = await db.CoreValues.Where(s => s.CoreValueId == Id && s.IsActive == true && s.IsDeleted == false).FirstOrDefaultAsync();
                    if (corevalue != null)
                    {
                        corevalue.IsActive = false;
                        corevalue.IsDeleted = true;
                        db.Entry(corevalue).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        res.Message = "successfully Deleted";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "CoreValue Not Found";
                        res.Status = false;
                    }
                }
            }
            else
            {
                res.Message = "You Are unable to Delete";
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Delete CoreValue

        #region Api For update CoreValue

        /// <summary>
        /// Created on 3/15/2022
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>

        [HttpPut]
        [Route("updatecorevalue")]
        public async Task<ResponseBodyModel> UpdateCoreValues(CoreValue core)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                using (var _db = new ApplicationDbContext())
                    if (core != null)
                    {
                        var obj = await _db.CoreValues.FirstOrDefaultAsync(x => x.CoreValueId == core.CoreValueId);
                        if (obj != null)
                        {
                            obj.CoreValueId = core.CoreValueId;
                            obj.CoreValueName = core.CoreValueName;
                            obj.Description = core.Description;
                            _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            res.Message = "CoreValue Updated";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            res.Message = "CoreValue Not Found";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "No Data";
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

        #endregion Api For update CoreValue

        public class AddCoreValue
        {
            public string CoreValueName { get; set; }
            public string Description { get; set; }
            public List<CoreValueBehaviour> Behaviours { get; set; }
        }

        public class GetCoreValueList
        {
            public int CoreValueId { get; set; }
            public string CoreValueName { get; set; }
            public string Description { get; set; }
            public int TotalBehavior { get; set; }
        }
    }
}