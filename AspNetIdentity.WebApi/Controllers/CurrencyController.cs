using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/currency")]
    public class CurrencyController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region #Get Currency By Id API

        [Route("GetCurrencyById")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetCurrencyById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                //Base response = new Base();

                var CurrencyData = await db.Currency.FirstOrDefaultAsync(x => x.CurrencyId == Id && x.IsDeleted == false);
                if (CurrencyData != null)
                {
                    res.Status = true;
                    res.Message = "Currency Found";
                    res.Data = CurrencyData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Currency Found!!";
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

        #endregion #Get Currency By Id API

        #region #Get All Currency API

        [Route("GetAllCurrency")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllCurrency()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var CurrencyData = await db.Currency.Where(x => x.CurrencySymbol != "" && x.IsActive == true && x.IsDeleted == false).ToListAsync();

                if (CurrencyData.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Currency list Found";
                    res.Data = CurrencyData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Currency list Found";
                    res.Data = CurrencyData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion #Get All Currency API

        #region #Create Currency (POST) API

        [Route("CreateCurrency")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> CreateCurrency(Currency model)
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
                var CurrencyData = db.Currency.FirstOrDefault(x => x.CurrencySymbol == model.CurrencySymbol || x.CurrencyName == model.CurrencyName || x.ISOCurrencyCode == model.ISOCurrencyCode && x.IsActive == true && x.IsDeleted == false);
                //var currencySymbol = db.Currency.Where(x => x.CurrencySymbol == model.CurrencySymbol && x.IsActive == true && x.IsDeleted == false).ToList();
                //var currencyName = db.Currency.Where(x => x.CurrencyName == model.CurrencyName && x.IsActive == true && x.IsDeleted == false).ToList();
                //var isoCurrencyCode = db.Currency.Where(x => x.ISOCurrencyCode == model.ISOCurrencyCode && x.IsActive == true && x.IsDeleted == false).ToList();

                if (CurrencyData != null)
                {
                    var resMessage = CurrencyData.CurrencySymbol == model.CurrencySymbol ? " symbol " : (CurrencyData.CurrencyName == model.CurrencyName ? " name " : " ISO code ");
                    res.Message = "Currency" + resMessage + "exist";
                    res.Status = false;
                }
                //if (currencySymbol != null )
                //{
                //    res.Message = "Currency Symbol already exist";
                //    res.Status = false;
                //}
                //else if ( currencyName != null )
                //{
                //    res.Message = "Currency Name already exist";
                //    res.Status = false;
                //}
                //else if (isoCurrencyCode != null)
                //{
                //    res.Message = "Iso Currency Code already exist";
                //    res.Status = false;
                //}
                else
                {
                    Currency obj = new Currency
                    {
                        CurrencyId = model.CurrencyId,
                        CurrencyName = model.CurrencyName,
                        ISOCurrencyCode = model.ISOCurrencyCode,
                        CurrencySymbol = model.CurrencySymbol,
                        TransactionCurrencyId = model.TransactionCurrencyId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userid
                    };
                    db.Currency.Add(obj);
                    await db.SaveChangesAsync();

                    res.Message = "Currency Added Successfuly";
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

        #endregion #Create Currency (POST) API

        #region #Update Currency (PUT) API

        [Route("UpdateCurrency")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult UpdateCurrency(Currency updateDep)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var updateDepData = db.Currency.Where(x => x.CurrencyId == updateDep.CurrencyId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                if (updateDepData != null)
                {
                    var CurrencyData = db.Currency.FirstOrDefault(x => x.CurrencySymbol == updateDep.CurrencySymbol || x.CurrencyName == updateDep.CurrencyName ||
                                    x.ISOCurrencyCode == updateDep.ISOCurrencyCode && x.IsActive == true && x.IsDeleted == false && x.CurrencyId != updateDep.CurrencyId);

                    if (CurrencyData != null)
                    {
                        var resMessage = CurrencyData.CurrencySymbol == updateDep.CurrencySymbol ? " symbol " : (CurrencyData.CurrencyName == updateDep.CurrencyName ? " name " : " ISO code ");
                        response.Message = "Currency" + resMessage + "exist";
                        response.Status = false;
                    }
                    else
                    {
                        updateDepData.CurrencyName = updateDep.CurrencyName;
                        updateDepData.CurrencySymbol = updateDep.CurrencySymbol;
                        updateDepData.TransactionCurrencyId = updateDep.TransactionCurrencyId;

                        db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        response.Status = true;
                        response.Message = "Currency Updated Successfully!";
                    }
                }
                else
                {
                    response.Message = "Currency Not Found";
                    response.Status = false;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion #Update Currency (PUT) API

        [Route("DeleteCurrency")]
        [HttpDelete]
        public async Task<ResponseBodyModel> RemoveCurrency(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteData = await db.Currency.FirstOrDefaultAsync(x => x.CurrencyId == Id && x.IsDeleted == false);

                if (deleteData != null)
                {
                    //   deleteData.DeletedBy = claims.employeeid;
                    deleteData.DeletedOn = DateTime.Now;
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Currency deleted successfully";
                    res.Status = true;
                    res.Data = deleteData;
                }
                else
                {
                    res.Message = "Currency Not Found";
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

        public class CurrencyData
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public Currency Currency { get; set; }
        }

        public class CurrencyDataList
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<Currency> CurrencyList { get; set; }
        }



        #region Api for Get Country Api
        /// <summary>
        /// API>>api/currency/getallcountrydata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcountrydata")]
        public ResponseBodyModel GetAllCountry()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                string baseURL = "https://restcountries.com/v2/all?fields=name";
                string response = RestClientHelper.HitTheRestClient(baseURL);
                var myDeserializedClass = JsonConvert.DeserializeObject<List<GetCurrencyByCountryNameResponse>>(response);
                var curencyListWithCountryName = myDeserializedClass;
                Console.WriteLine(myDeserializedClass);
                res.Message = "Succesfully Get ";
                res.Status = true;
                res.Data = myDeserializedClass;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }



        #endregion

        #region Api for Get Currency by Country Name
        /// <summary>
        ///  API>>api/currency/getalldatabycountryname
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldatabycountryname")]
        public ResponseBodyModel GetAllCurrencyByCountryName(string name = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                string baseURL = "https://restcountries.com/v2/name/" + name + "?fullText=true&fields=callingCodes,currencies,flags";
                string response = RestClientHelper.HitTheRestClient(baseURL);
                var myDeserializedClass = JsonConvert.DeserializeObject<List<RootResponseByCountryName>>(response);
                var curencyListWithCountryName = myDeserializedClass;
                Console.WriteLine(myDeserializedClass);



                res.Message = "Succesfully Get ";
                res.Status = true;
                res.Data = myDeserializedClass;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }



        #endregion

        #region APi for Get TimeZone Behalf Of Country Name
        /// <summary>
        /// API>>api/currency/getalltimezonebycountryname?name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getalltimezonebycountryname")]
        public ResponseBodyModel GetAllTimeZoneByCountryName(string name = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                string baseURL = "https://restcountries.com/v2/name/" + name + "?fullText=true&fields=timezones";
                string response = RestClientHelper.HitTheRestClient(baseURL);
                var myDeserializedClass = JsonConvert.DeserializeObject<List<TimeZoneResponse>>(response);
                var curencyListWithCountryName = myDeserializedClass;
                Console.WriteLine(myDeserializedClass);



                res.Message = "Succesfully Get Time Zone ";
                res.Status = true;
                res.Data = myDeserializedClass;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region Api for Get Mulltiple Currency by Country Name
        /// <summary>
        ///  API>>api/currency/getallmulltiplecurrenciesbycountryname
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallmulltiplecurrenciesbycountryname")]
        public ResponseBodyModel GetAllMulltipleCurrencyByCountryName(string name = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                string baseURL = "https://restcountries.com/v2/name/" + name + "?fullText=true&fields=currencies";
                string response = RestClientHelper.HitTheRestClient(baseURL);
                var myDeserializedClass = JsonConvert.DeserializeObject<List<MulltipleCurrenciesResponse>>(response);
                var curencyListWithCountryName = myDeserializedClass;
                Console.WriteLine(myDeserializedClass);



                res.Message = "Succesfully Get ";
                res.Status = true;
                res.Data = myDeserializedClass;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }



        #endregion

        #region Helper Model For Country And Currency 
        public class CurrencyResponse
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Symbol { get; set; }
        }



        public class GetCurrencyByCountryNameResponse
        {
            public string Name { get; set; }
        }
        public class FlagsDTO
        {
            public string Svg { get; set; }

        }

        public class RootResponseByCountryName
        {
            public List<string> CallingCodes { get; set; }
            public FlagsDTO Flags { get; set; }
            public List<CurrencyResponse> Currencies { get; set; }

        }

        public class TimeZoneResponse
        {
            public List<string> timezones { get; set; }
        }

        public class MulltipleCurrenciesResponse
        {
            public List<CurrencyResponse> currencies { get; set; }
        }
        #endregion



    }
}