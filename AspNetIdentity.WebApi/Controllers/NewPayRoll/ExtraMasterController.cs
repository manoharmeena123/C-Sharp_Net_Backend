using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.ExtraMasterModel;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Payroll
{
    /// <summary>
    /// Created By Harshit Mitra on 22-02-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/extramaster")]
    public class ExtraMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api's For Location Masters

        #region Api To Add Location

        /// <summary>
        /// APi >> Post >> api/extramaster/addlocation
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addlocation")]
        public async Task<ResponseBodyModel> AddLocation(Location model)
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
                    Location obj = new Location
                    {
                        LocationName = model.LocationName,
                        Address = model.Address,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                    };
                    _db.Locations.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Location Added";
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

        #endregion Api To Add Location

        #region Api to Get All Location List

        /// <summary>
        /// API >> Get >> api/extramaster/getalllocation
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalllocation")]
        public async Task<ResponseBodyModel> GetAllActiveLocation()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var location = await _db.Locations.Where(x => x.CompanyId == claims.companyId)
                    .Select(x => new
                    {
                        x.LocationId,
                        x.LocationName,
                    }).ToListAsync();
                if (location.Count > 0)
                {
                    res.Message = "Location List";
                    res.Status = true;
                    res.Data = location;
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

        #endregion Api to Get All Location List

        #region Api to Get Active Location List

        /// <summary>
        /// API >> Get >> api/extramaster/getactivelocation
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getactivelocation")]
        public async Task<ResponseBodyModel> GetActiveLocation()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var location = await _db.Locations.Where(x => x.IsActive == true && x.IsDeleted == false &&
                x.CompanyId == claims.companyId).Select(x => new
                {
                    x.LocationId,
                    x.LocationName,
                    x.Address,
                }).ToListAsync();
                if (location.Count > 0)
                {
                    res.Message = "Location List";
                    res.Status = true;
                    res.Data = location;
                }
                else
                {
                    res.Message = "List is Empty";
                    res.Status = false;
                    res.Data = location;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api to Get Active Location List

        #region Api To Get Location By Id

        /// <summary>
        /// API >> Get >> api/extramaster/getlocationbyid
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getlocationbyid")]
        public async Task<ResponseBodyModel> GetLocationById(int locationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var location = await _db.Locations.FirstOrDefaultAsync(x =>
                        x.LocationId == locationId && x.CompanyId == claims.companyId);
                if (location != null)
                {
                    res.Message = "Location List";
                    res.Status = true;
                    res.Data = location;
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

        #endregion Api To Get Location By Id

        #region Api To Edit Location

        /// <summary>
        /// APi >> Post >> api/extramaster/editlocation
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("editlocation")]
        public async Task<ResponseBodyModel> EditLocation(Location model)
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
                    var location = await _db.Locations.FirstOrDefaultAsync(x =>
                            x.LocationId == model.LocationId && x.CompanyId == claims.companyId);
                    if (location != null)
                    {
                        location.LocationName = model.LocationName;
                        location.Address = model.Address;
                        location.UpdatedBy = claims.employeeId;
                        location.UpdatedOn = DateTime.Now;

                        _db.Entry(location).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Location Edited";
                        res.Status = true;
                        res.Data = location;
                    }
                    else
                    {
                        res.Message = "Location Not Found";
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

        #endregion Api To Edit Location

        #endregion Api's For Location Masters

        #region Api's For Nature Of Business

        #region Add Nature Of Business

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Post >> api/extramaster/addnatureofbusiness
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addnatureofbusiness")]
        public async Task<ResponseBodyModel> CreateNatureOfBusinessAsync(NatureOfBusiness model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Message = string.Join(" | ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage));
                    res.Status = false;
                }
                else
                {
                    NatureOfBusiness obj = new NatureOfBusiness
                    {
                        NatureOfBusinessName = model.NatureOfBusinessName,
                        CreatedBy = 0,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now
                    };

                    _db.NatureOfBusinesses.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Nature Of Business Added";
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

        #endregion Add Nature Of Business

        #region Get All Nature Of Business

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Get >> api/extramaster/activenatureofbusiness
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("activenatureofbusiness")]
        public async Task<ResponseBodyModel> GetAllNatureOfBusiness()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var natureOfBusiness = await _db.NatureOfBusinesses.Where(x =>
                        x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToListAsync();
                if (natureOfBusiness.Count != 0)
                {
                    res.Message = "NatureOfBusiness list Found";
                    res.Status = true;
                    res.Data = natureOfBusiness;
                }
                else
                {
                    res.Message = "NoNatureOfBusiness list Found";
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

        #endregion Get All Nature Of Business

        #region Get NatureOfBusiness By Id

        /// <summary>
        /// Created By Priyanka Gayakwad
        /// Modify By Harshit Mitra on 27-04-2022
        /// API >> Get >> api/extramaster/natureodbusinessbyid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("natureodbusinessbyid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetNatureOfBusinessById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var natureOfBusiness = await _db.NatureOfBusinesses.FirstOrDefaultAsync(x =>
                        x.NatureOfBusinessId == id && x.IsDeleted == false && x.IsActive == true);
                if (natureOfBusiness != null)
                {
                    res.Message = "NatureOfBusiness Found";
                    res.Status = true;
                    res.Data = natureOfBusiness;
                }
                else
                {
                    res.Message = "Nature Of Business Not Found";
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

        #endregion Get NatureOfBusiness By Id

        #region Edit Nature Of Business

        /// <summary>
        /// Modify By Harshit Mitra On 27-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Put >> api/extramaster/editnatureofbusiness
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editnatureofbusiness")]
        public async Task<ResponseBodyModel> UpdateNatureOfBusiness(NatureOfBusiness model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var natureOfBusiness = await _db.NatureOfBusinesses.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.NatureOfBusinessId == model.NatureOfBusinessId)
                        .FirstOrDefaultAsync();
                if (natureOfBusiness != null)
                {
                    natureOfBusiness.NatureOfBusinessName = model.NatureOfBusinessName;
                    natureOfBusiness.UpdatedBy = 0;
                    natureOfBusiness.UpdatedOn = DateTime.Now;
                    natureOfBusiness.IsActive = true;
                    natureOfBusiness.IsDeleted = false;

                    _db.Entry(natureOfBusiness).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Nature Of Business Updated";
                    res.Status = true;
                    res.Data = natureOfBusiness;
                }
                else
                {
                    res.Message = "Nature Of Business Not Found";
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

        #endregion Edit Nature Of Business

        #region Delete Nature Of Business

        /// <summary>
        /// Created By Harshit Mitra On 27-04-2022
        /// API >> Put >> api/extramaster/deletenatureofbusiness
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletenatureofbusiness")]
        public async Task<ResponseBodyModel> DeleteNatureOfBusiness(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var natureOfBusiness = await _db.NatureOfBusinesses.FirstOrDefaultAsync(x =>
                        x.NatureOfBusinessId == id && x.IsDeleted == false && x.IsActive == true);
                if (natureOfBusiness != null)
                {
                    natureOfBusiness.IsActive = false;
                    natureOfBusiness.IsDeleted = true;

                    _db.Entry(natureOfBusiness).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Nature Of Business Updated";
                    res.Status = true;
                    res.Data = natureOfBusiness;
                }
                else
                {
                    res.Message = "Nature Of Business Not Found";
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

        #endregion Delete Nature Of Business

        #endregion Api's For Nature Of Business

        #region Api's For Type Of Business

        #region Get Type Of Business By Id

        /// <summary>
        /// Modify By Harshit Mitra On 28-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Get >> api/extramaster/natureodbusinessbyid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("gettypeofbusinessbyid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetTypeOfBusinessById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var typeOfBusiness = await _db.TypeOfBusinesses.FirstOrDefaultAsync(x => x.TypeOfBusinessId == id);
                if (typeOfBusiness != null)
                {
                    res.Message = "TypeOfBusiness Found";
                    res.Status = true;
                    res.Data = typeOfBusiness;
                }
                else
                {
                    res.Message = "No TypeOfBusiness Found!!";
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

        #endregion Get Type Of Business By Id

        #region Get All Active Type Of Business

        /// <summary>
        /// Modify By Harshit Mitra On 28-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Get >> api/extramaster/getallbusinesstype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallbusinesstype")]
        public async Task<ResponseBodyModel> GetAllTypeOfBusiness()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var TypeOfBusinessData = await _db.TypeOfBusinesses.Where(x =>
                        x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToListAsync();
                if (TypeOfBusinessData.Count != 0)
                {
                    res.Message = "TypeOfBusiness list Found";
                    res.Status = true;
                    res.Data = TypeOfBusinessData;
                }
                else
                {
                    res.Message = "No TypeOfBusiness list Found";
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

        #endregion Get All Active Type Of Business

        #region Add Type Of Business

        /// <summary>
        /// Modify By Harshit Mitra On 28-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Post >> api/extramaster/addtypeofbusiness
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addtypeofbusiness")]
        public async Task<ResponseBodyModel> CreateTypeOfBusiness(TypeOfBusiness model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            //TypeOfBusinessValidator validator = new TypeOfBusinessValidator();
            try
            {
                TypeOfBusiness obj = new TypeOfBusiness
                {
                    TypeOfBusinessName = model.TypeOfBusinessName,
                    CreatedBy = 0,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedOn = DateTime.Today,
                };
                _db.TypeOfBusinesses.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "TypeOfBusiness added Successfully!";
                res.Status = true;
                res.Data = obj;
                // }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Add Type Of Business

        #region Edit Type Of Business

        /// <summary>
        /// Modify By Harshit Mitra On 28-04-2022
        /// Created By Priyanka Gayakwad
        /// API >> Put >> api/extramaster/editypeofbusiness
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editypeofbusiness")]
        public async Task<ResponseBodyModel> UpdateTypeOfBusiness(TypeOfBusiness model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var typeOfBusiness = await _db.TypeOfBusinesses.FirstOrDefaultAsync(x =>
                        x.TypeOfBusinessId == model.TypeOfBusinessId && x.IsDeleted == false && x.IsActive == true);
                if (typeOfBusiness != null)
                {
                    typeOfBusiness.TypeOfBusinessName = model.TypeOfBusinessName;
                    typeOfBusiness.UpdatedBy = 0;
                    typeOfBusiness.IsActive = true;
                    typeOfBusiness.IsDeleted = false;

                    _db.Entry(typeOfBusiness).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "TypeOfBusiness Updated Successfully!";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Type Of Business Not Found";
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

        #endregion Edit Type Of Business

        #region Api To Delete Type Of Business

        /// <summary>
        /// Created By Harshit Mitra On 28-04-2022
        /// API >> Put >> api/extramaster/delettypeofbusiness
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("delettypeofbusiness")]
        public async Task<ResponseBodyModel> DeleteTypeOfBusiness(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var typeOfBusiness = await _db.TypeOfBusinesses.FirstOrDefaultAsync(x =>
                        x.TypeOfBusinessId == id && x.IsDeleted == false && x.IsActive == true);
                if (typeOfBusiness != null)
                {
                    typeOfBusiness.UpdatedBy = 0;
                    typeOfBusiness.UpdatedOn = DateTime.Today;
                    typeOfBusiness.IsActive = false;
                    typeOfBusiness.IsDeleted = true;

                    _db.Entry(typeOfBusiness).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "TypeOfBusiness Updated Successfully!";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Not Found";
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

        #endregion Api To Delete Type Of Business

        #endregion Api's For Type Of Business

        #region Api's For Sector Master

        #region Api To Get All Sector

        /// <summary>
        /// Created By Harshit Mitra on 28-04-2022
        /// API >> Get >> api/extramaster/getallsector
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallsector")]
        public async Task<ResponseBodyModel> GetAllSector()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var SectorDataList = await _db.Sectors.Where(x =>
                    x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.UpdatedOn == null ? x.CreatedOn : x.UpdatedOn).ToListAsync();
                if (SectorDataList != null)
                {
                    res.Status = true;
                    res.Message = "Sector list Found";
                    res.Data = SectorDataList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Sector list Found";
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

        #endregion Api To Get All Sector

        #region Api To Add Sector

        /// <summary>
        /// Created By Harshit Mitra on 28-04-2022
        /// API >> Get >> api/extramaster/createsector
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createsector")]
        public async Task<ResponseBodyModel> CreateSector(Sector model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                Sector obj = new Sector
                {
                    SectorName = model.SectorName,
                    CreatedBy = 0,
                    CreatedOn = DateTime.Today,
                    IsActive = true,
                    IsDeleted = false
                };

                _db.Sectors.Add(obj);
                await _db.SaveChangesAsync();

                res.Status = true;
                res.Message = "Sector added Successfully!";
                res.Data = obj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Sector

        #region Api To Edit Sector

        /// <summary>
        /// Created By Harshit Mitra on 28-04-2022
        /// API >> Get >> api/extramaster/editsector
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editsector")]
        public async Task<ResponseBodyModel> UpdateSector(Sector model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var updateDepData = await _db.Sectors.Where(x =>
                    x.SectorId == model.SectorId).FirstOrDefaultAsync();
                if (updateDepData != null)
                {
                    updateDepData.SectorName = model.SectorName;
                    updateDepData.UpdatedBy = 0;
                    updateDepData.UpdatedOn = DateTime.Now;
                    updateDepData.IsActive = true;
                    updateDepData.IsDeleted = false;

                    _db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Sector Updated Successfully!";
                }
                else
                {
                    res.Message = "Sector Not Found";
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

        #endregion Api To Edit Sector

        #region Api To Delete Sector

        /// <summary>
        /// Created By Harshit Mitra on 28-04-2022
        /// API >> Get >> api/extramaster/deletesector
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletesector")]
        public async Task<ResponseBodyModel> DeleteSector(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var updateDepData = await _db.Sectors.Where(x =>
                    x.SectorId == id).FirstOrDefaultAsync();
                if (updateDepData != null)
                {
                    updateDepData.UpdatedBy = 0;
                    updateDepData.UpdatedOn = DateTime.Now;
                    updateDepData.IsActive = false;
                    updateDepData.IsDeleted = true;

                    _db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Sector Updated Successfully!";
                }
                else
                {
                    res.Message = "Sector Not Found";
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

        #endregion Api To Delete Sector

        #endregion Api's For Sector Master

        #region Api To Get Blood Group

        /// <summary>
        /// Created By Harshit Mitra on 26-04-2022
        /// API >> Get >> api/extramaster/getbloodtype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbloodtype")]
        public ResponseBodyModel GetAllBloodType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var bloodGroup = Enum.GetValues(typeof(BloodGroupConstants))
                                .Cast<BloodGroupConstants>()
                                .Select(x => new BooodGroupList
                                {
                                    BloodGroupId = (int)x,
                                    BloodGroupType = Enum.GetName(typeof(BloodGroupConstants), x).Contains("_pos") ?
                                                     Enum.GetName(typeof(BloodGroupConstants), x).Replace("_pos", "+") :
                                                     Enum.GetName(typeof(BloodGroupConstants), x).Replace("_neg", "-"),
                                }).ToList();

                res.Message = "Blood Group List";
                res.Status = true;
                res.Data = bloodGroup;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Blood Group

        #region Api To Get Employee Type

        /// <summary>
        /// Created By Harshit Mitra on 26-04-2022
        /// API >> Get >> api/extramaster/getemployeetype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeetype")]
        public ResponseBodyModel GetAllEmployeeType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var empType = Enum.GetValues(typeof(EmployeeTypeConstants))
                                .Cast<EmployeeTypeConstants>()
                                .Select(x => new
                                {
                                    EmployeeTypeId = (int)x,
                                    EmployeeTypeName = Enum.GetName(typeof(EmployeeTypeConstants), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Employee Type List";
                res.Status = true;
                res.Data = empType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Employee Type

        #region Api To Get User Login Type

        /// <summary>
        /// Created By Harshit Mitra on 26-04-2022
        /// API >> Get >> api/extramaster/getuserlogintype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserlogintype")]
        public ResponseBodyModel GetUserLoginType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.orgId == 0)
                {
                    var empType = Enum.GetValues(typeof(LoginRolesConstants))
                                    .Cast<LoginRolesConstants>()
                                    .Where(x => x != LoginRolesConstants.SuperAdmin)
                                    .Select(x => new
                                    {
                                        EmployeeTypeId = (int)x,
                                        EmployeeTypeName = Enum.GetName(typeof(LoginRolesConstants), x).Replace("_", " "),
                                        IncludeOrg = (x != LoginRolesConstants.Administrator),
                                    }).ToList();
                    res.Message = "Employee Type List";
                    res.Status = true;
                    res.Data = empType;
                }
                else
                {
                    var empType = Enum.GetValues(typeof(LoginRolesConstants))
                                    .Cast<LoginRolesConstants>()
                                    .Where(x => x != LoginRolesConstants.SuperAdmin && x != LoginRolesConstants.Administrator)
                                    .Select(x => new
                                    {
                                        EmployeeTypeId = (int)x,
                                        EmployeeTypeName = Enum.GetName(typeof(LoginRolesConstants), x).Replace("_", " "),
                                        IncludeOrg = false,
                                    }).ToList();
                    res.Message = "Employee Type List";
                    res.Status = true;
                    res.Data = empType;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get User Login Type

        #region Extra Api's On PayRoll

        #region Api To Get Pay Frequency Cycle

        /// <summary>
        /// API >> Api >> api/extramaster/payfrequencycycle
        /// Created By Harshit Mitra on 23-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("payfrequencycycle")]
        public Task<ResponseBodyModel> ToGetPayFrequencyCycle()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetPayFrequencyCycle> list = new List<GetPayFrequencyCycle>();
            try
            {
                foreach (var item in Enum.GetValues(typeof(PayFrequency)))
                {
                    GetPayFrequencyCycle obj = new GetPayFrequencyCycle()
                    {
                        Title = item.ToString(),
                    };
                    list.Add(obj);
                }
                if (list.Count > 0)
                {
                    res.Message = "Pay Frequency List";
                    res.Status = true;
                    res.Data = list;
                }
                else
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Task.FromResult(res);
        }

        #endregion Api To Get Pay Frequency Cycle

        #region Api To Get Pay Cycle On Behaf Of Pay Frequency

        /// <summary>
        /// API >> Get >> api/extramaster/paycycle
        /// Created By Harshit Mitra on 24-02-2022
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("paycycle")]
        public Task<ResponseBodyModel> GetPayCycle(string title)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                switch (title)
                {
                    case "Monthly":
                        List<MonthList> monthList = new List<MonthList>();

                        string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                        var ordered = monthNames.Skip(DateTime.Today.Month - 1)
                                            .Concat(monthNames.Take(DateTime.Today.Month - 1))
                                            .Where(s => !String.IsNullOrEmpty(s))
                                            .ToList();
                        foreach (var month in ordered)
                        {
                            MonthList months = new MonthList
                            {
                                MonthName = month,
                            };
                            monthList.Add(months);
                        }
                        res.Data = monthList;
                        break;

                    case "Weekly":
                        List<WeeksList> weekList = new List<WeeksList>();
                        DateTime dt = new DateTime(DateTime.Now.Year, 12, 31);
                        var totalWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt,
                                        CalendarWeekRule.FirstFullWeek,
                                        DayOfWeek.Monday);
                        for (int i = 0; i < totalWeek; i++)
                        {
                            WeeksList weeks = new WeeksList
                            {
                                WeekName = i + 1 + " Week",
                            };
                            weekList.Add(weeks);
                        }
                        res.Data = weekList;
                        break;

                    default:
                        res.Data = null;
                        break;
                }
                res.Message = "Pay Cycle";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Task.FromResult(res);
        }

        #endregion Api To Get Pay Cycle On Behaf Of Pay Frequency

        #region Api To Get Date Cycle on Behalf Of Month

        /// <summary>
        /// API >> Get >> api/extramaster/paydatecycle
        /// Created By Harshit Mitra on 24-02-2022
        /// </summary>
        /// <param name="monthId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("paydatecycle")]
        public Task<ResponseBodyModel> GetDateCycle(string monthName)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var year = DateTime.Now.Year;
                List<DayList> list = new List<DayList>();

                var monthId = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(monthName) + 1;
                // Loop from the first day of the month until we hit the next month, moving forward a day at a time
                for (var date = new DateTime(year, monthId, 1); date.Month == monthId; date = date.AddDays(1))
                {
                    DayList obj = new DayList()
                    {
                        Day = date.ToString("dd MMM"),
                    };
                    list.Add(obj);
                }
                if (list.Count > 0)
                {
                    res.Message = "Add Days In Selected Month";
                    res.Status = true;
                    res.Data = list;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Task.FromResult(res);
        }

        #endregion Api To Get Date Cycle on Behalf Of Month

        #endregion Extra Api's On PayRoll



        #region REQUEST AND RESPONSE 

        public class CreateBusinessNatureRequest
        {
            [Required]
            public string Title { get; set; }
        }



        #endregion


        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 23-02-2022
        /// </summary>
        public class GetPayFrequencyCycle
        {
            public string Title { get; set; } = String.Empty;
        }

        /// <summary>
        /// Created By Harshit Mitra on 23-02-2022
        /// </summary>
        public class MonthList
        {
            public string MonthName { get; set; } = String.Empty;
        }

        /// <summary>
        /// Created By Harshit Mitra
        /// </summary>
        public class WeeksList
        {
            public string WeekName { get; set; } = String.Empty;
        }

        public class DayList
        {
            public string Day { get; set; } = String.Empty;
        }

        /// <summary>
        /// Created By Harshit Mitra on 25-04-2022
        /// </summary>
        public class BooodGroupList
        {
            public int BloodGroupId { get; set; }
            public string BloodGroupType { get; set; } = String.Empty;
        }

        #endregion Helper Model Class
    }
}