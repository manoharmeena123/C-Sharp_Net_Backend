using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Modify BY Harshit Mitra on 27-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/country")]
    public class CountryController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Get County By Id

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/country/getcountrybyid
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcountrybyid")]
        public async Task<ResponseBodyModel> GetCountryById(int countryId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var country = await _db.Country.FirstOrDefaultAsync(x =>
                        x.CountryId == countryId && x.IsDeleted == false && x.IsActive);
                if (country != null)
                {
                    res.Message = "Country Found";
                    res.Status = true;
                    res.Data = country;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Country Found!!";
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

        #endregion Api To Get County By Id

        #region Api To Get All Country

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Get >> api/country/getallcountry
        /// API >>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcountry")]
        public async Task<ResponseBodyModel> GetAllCountry()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var countryList = await _db.Country.Where(x =>
                        x.IsDeleted == false && x.IsActive == true)
                        .ToListAsync();
                if (countryList.Count > 0)
                {
                    countryList = countryList.OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToList();
                    res.Status = true;
                    res.Message = "Country list Found";
                    res.Data = countryList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Country list Found";
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

        #endregion Api To Get All Country

        #region Api To Add Country

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Post >> api/country/createcountry
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcountry")]
        public async Task<ResponseBodyModel> CreateCountry(Country model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                    return res;
                }
                var checkCountry = await _db.Country.FirstOrDefaultAsync(x =>
                        x.CountryName.Trim().ToUpper() == model.CountryName.Trim().ToUpper());
                if (checkCountry == null)
                {
                    Country obj = new Country
                    {
                        CountryName = model.CountryName.Trim(),
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now
                    };
                    _db.Country.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Country added Successfully!";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    if (checkCountry.IsDeleted == false && checkCountry.IsActive == true)
                    {
                        res.Message = "Country already exists!";
                        res.Status = false;
                    }
                    else
                    {
                        checkCountry.UpdatedOn = DateTime.Now;
                        checkCountry.UpdatedBy = 0;
                        checkCountry.IsActive = true;
                        checkCountry.IsDeleted = false;
                        _db.Entry(checkCountry).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Country added Successfully!";
                        res.Status = true;
                        res.Data = checkCountry;
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

        #endregion Api To Add Country

        #region Api To Edit Country

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/country/editcountry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editcountry")]
        public async Task<ResponseBodyModel> UpdateCountry(Country model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var updateDepData = await _db.Country.FirstOrDefaultAsync(x =>
                        x.CountryId == model.CountryId);
                if (updateDepData != null)
                {
                    var checkCountry = _db.Country.ToList()
                            .Select(x => x.CountryName.Trim().ToUpper())
                            .Contains(model.CountryName.Trim().ToUpper());
                    if (checkCountry)
                    {
                        res.Status = false;
                        res.Message = "Country Already Exist ";
                    }
                    else
                    {
                        updateDepData.CountryName = model.CountryName;
                        updateDepData.UpdatedOn = DateTime.Now;
                        _db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Country Updated Successfully!";
                    }
                }
                else
                {
                    res.Message = "Country Not Found";
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

        #endregion Api To Edit Country

        #region Api To Delete Country

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/country/deletecountry
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletecountry")]
        public async Task<ResponseBodyModel> DeleteCountry(int countryId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteData = await _db.Country.FirstOrDefaultAsync(x =>
                        x.CountryId == countryId && x.IsDeleted == false && x.IsActive == true);
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    deleteData.DeletedOn = DateTime.Today;
                    deleteData.DeletedBy = 0;
                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Country Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Country Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Delete Country
    }
}