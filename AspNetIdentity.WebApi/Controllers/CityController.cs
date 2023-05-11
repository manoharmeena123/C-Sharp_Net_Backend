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
    [RoutePrefix("api/city")]
    public class CityController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Get City By Id

        [Route("getcitybyid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetCityById(int cityId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var CityData = await _db.City.FirstOrDefaultAsync
                        (x => x.CityId == cityId);
                if (CityData != null)
                {
                    res.Status = true;
                    res.Message = "City Found";
                    res.Data = CityData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No City Found!!";
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

        #endregion Api To Get City By Id

        #region Api To Get All City

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/city/getallcity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcity")]
        public async Task<ResponseBodyModel> GetAllCity()
        {
            ResponseBodyModel dep = new ResponseBodyModel();
            try
            {
                var CityData = await (from c in _db.City
                                      join s in _db.State on c.StateId equals s.StateId
                                      join x in _db.Country on s.CountryId equals x.CountryId
                                      where x.IsDeleted == false && x.IsActive == true
                                      && c.IsDeleted == false && c.IsActive == true
                                      && s.IsDeleted == false && s.IsActive == true
                                      select new
                                      {
                                          c.CityId,
                                          c.CityName,
                                          s.StateId,
                                          s.StateName,
                                          x.CountryId,
                                          x.CountryName,
                                          c.CreatedOn,
                                          c.UpdatedOn,
                                      }).ToListAsync();
                if (CityData.Count != 0)
                {
                    CityData = CityData.OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToList();
                    dep.Status = true;
                    dep.Message = "City list Found";
                    dep.Data = CityData;
                }
                else
                {
                    dep.Status = false;
                    dep.Message = "No City list Found";
                    dep.Data = CityData;
                }
            }
            catch (Exception ex)
            {
                dep.Message = ex.Message;
                dep.Status = false;
            }
            return dep;
        }

        #endregion Api To Get All City

        #region Api To Create City

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/city/createcity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcity")]
        public async Task<ResponseBodyModel> CreateCity(City model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var checkCity = await _db.City.FirstOrDefaultAsync(x =>
                        x.CityName.Trim().ToUpper() == model.CityName.Trim().ToUpper() &&
                        x.StateId == model.StateId);
                if (checkCity == null)
                {
                    City obj = new City
                    {
                        StateId = model.StateId,
                        CityName = model.CityName,
                        CreatedBy = 0,
                        CreatedOn = DateTime.Today,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.City.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "City Added";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    if (checkCity.IsDeleted == false && checkCity.IsActive == true)
                    {
                        res.Message = "City Already Exist";
                        res.Status = false;
                    }
                    else
                    {
                        checkCity.IsDeleted = false;
                        checkCity.IsActive = true;
                        checkCity.UpdatedOn = DateTime.Now;
                        checkCity.UpdatedBy = 0;
                        _db.Entry(checkCity).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "State added Successfully!";
                        res.Status = true;
                        res.Data = checkCity;
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

        #endregion Api To Create City

        #region Api To Edit City

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/city/editcity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editcity")]
        public async Task<ResponseBodyModel> UpdateCity(City model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var city = await _db.City.FirstOrDefaultAsync(x => x.IsDeleted == false && x.IsActive == true && x.CityId == model.CityId);
                if (city != null)
                {
                    city.StateId = model.StateId;
                    city.CityName = model.CityName;
                    city.UpdatedOn = DateTime.Today;
                    city.UpdatedBy = 0;

                    _db.Entry(city).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "City Updated";
                    res.Status = true;
                    res.Data = city;
                }
                else
                {
                    res.Message = "City Not Found";
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

        #endregion Api To Edit City

        #region Api To Delete City

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/city/deletecity
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletecity")]
        public async Task<ResponseBodyModel> DeleteCity(int cityId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteData = await _db.City.FirstOrDefaultAsync(x =>
                    x.CityId == cityId && x.IsDeleted == false && x.IsActive == true);
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;

                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "City Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No City Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Delete City

        #region Api To Get All City By State Id

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/city/getallcitybystateid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcitybystateid")]
        public async Task<ResponseBodyModel> GetAllCityByStateId(int stateId)
        {
            ResponseBodyModel dep = new ResponseBodyModel();
            try
            {
                var CityData = await (from c in _db.City
                                      where c.IsDeleted == false && c.IsActive == true
                                      && c.StateId == stateId
                                      select new
                                      {
                                          c.CityId,
                                          c.CityName,
                                      }).ToListAsync();
                if (CityData.Count != 0)
                {
                    dep.Status = true;
                    dep.Message = "City list Found";
                    dep.Data = CityData;
                }
                else
                {
                    dep.Status = false;
                    dep.Message = "No City list Found";
                    dep.Data = CityData;
                }
            }
            catch (Exception ex)
            {
                dep.Message = ex.Message;
                dep.Status = false;
            }
            return dep;
        }

        #endregion Api To Get All City By State Id
    }
}