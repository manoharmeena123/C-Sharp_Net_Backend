using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Modify By Harshit Mitra On 27-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/state")]
    public class StateController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Get State By Id

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/state/getstatebyid
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getstatebyid")]
        public async Task<ResponseBodyModel> GetStateById(int stateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var StateData = await _db.State.FirstOrDefaultAsync(x =>
                        x.IsActive == true && x.IsDeleted == false && x.StateId == stateId);
                if (StateData != null)
                {
                    res.Status = true;
                    res.Message = "State Found";
                    res.Data = StateData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No State Found!!";
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get State By Id

        #region Api To Get All State

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/state/getallstates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallstates")]
        public async Task<ResponseBodyModel> GetAllState()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var StateData = await (from c in _db.Country
                                       join s in _db.State on c.CountryId equals s.CountryId
                                       where s.IsActive == true && s.IsDeleted == false
                                       && c.IsActive == true && c.IsDeleted == false
                                       select new
                                       {
                                           s.StateId,
                                           s.StateName,
                                           c.CountryId,
                                           c.CountryName,
                                           s.CreatedOn,
                                           s.UpdatedOn
                                       }).ToListAsync();

                if (StateData.Count > 0)
                {
                    StateData = StateData.OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToList();
                    res.Status = true;
                    res.Message = "State list Found";
                    res.Data = StateData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No State list Found";
                    res.Data = StateData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All State

        #region Api To Add State

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/state/createstate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createstate")]
        public async Task<ResponseBodyModel> CreateState(State model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var checkstate = await _db.State.FirstOrDefaultAsync(x =>
                    x.StateName.Trim().ToUpper() == model.StateName.Trim().ToUpper());
                if (checkstate == null)
                {
                    var country = await _db.Country.FirstOrDefaultAsync(
                            x => x.CountryId == model.CountryId);
                    if (country == null)
                    {
                        res.Message = "Country Not Found";
                        res.Status = false;
                    }
                    else
                    {
                        State obj = new State
                        {
                            StateName = model.StateName,
                            CountryId = model.CountryId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = 0,
                        };

                        _db.State.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "State added Successfully!";
                        res.Status = true;
                        res.Data = obj;
                    }
                }
                else
                {
                    if (checkstate.IsDeleted == false && checkstate.IsActive == true)
                    {
                        res.Message = "State already exists!";
                        res.Status = false;
                    }
                    else
                    {
                        checkstate.IsDeleted = false;
                        checkstate.IsActive = true;
                        checkstate.UpdatedOn = DateTime.Now;
                        checkstate.UpdatedBy = 0;
                        _db.Entry(checkstate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "State added Successfully!";
                        res.Status = true;
                        res.Data = checkstate;
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

        #endregion Api To Add State

        #region Api To Update State

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/state/updatestate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatestate")]
        public async Task<ResponseBodyModel> UpdateState(State model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var StateData = await _db.State.FirstOrDefaultAsync(x =>
                        x.IsActive == true && x.IsDeleted == false && x.StateId == model.StateId);
                if (StateData != null)
                {
                    StateData.CountryId = model.CountryId;
                    StateData.StateName = model.StateName;

                    _db.Entry(StateData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "State Updated Successfully!";
                    res.Data = StateData;
                }
                else
                {
                    res.Message = "State Not Found";
                    res.Status = false;
                    res.Data = StateData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Update State

        #region Api To Delete State

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/state/deletestate
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletestate")]
        public async Task<ResponseBodyModel> DeleteState(int stateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var state = await _db.State.FirstOrDefaultAsync(x =>
                        x.StateId == stateId && x.IsDeleted == false && x.IsActive == true);
                if (state != null)
                {
                    state.IsDeleted = true;
                    state.IsActive = false;

                    _db.Entry(state).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "State Deleted Successfully!";
                    res.Data = state;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No State Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Delete State

        #region Api To Get All State By Country Id

        /// <summary>
        /// Created By Harshit Mitra On 01-05-2022
        /// API >> Put >> api/state/getallstatesbycountry
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallstatesbycountry")]
        public async Task<ResponseBodyModel> GetAllStateByCountryId(int countryId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var StateData = await (from s in _db.State
                                       where s.IsActive && !s.IsDeleted
                                       && s.CountryId == countryId
                                       select new
                                       {
                                           s.StateId,
                                           s.StateName,

                                       }).ToListAsync();

                if (StateData.Count != 0)
                {
                    res.Status = true;
                    res.Message = "State list Found";
                    res.Data = StateData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No State list Found";
                    res.Data = StateData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All State By Country

    }
}