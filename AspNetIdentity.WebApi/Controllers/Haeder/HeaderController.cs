using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Header;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/navigation")]
    public class HeaderController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region This Api Use First Navigation

        #region This Api Use for Add Header
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Post >> api/navigation/addnavigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addnavigation")]
        public async Task<ResponseBodyModel> AddNavigation(HomeHeader model)
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
                    HomeHeader post = new HomeHeader
                    {
                        NavigationName = model.NavigationName,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.HomeHeaders.Add(post);
                    await _db.SaveChangesAsync();
                    res.Message = "Added Navigation Data";
                    res.Status = true;
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
        #endregion

        #region This Api Use for Get All Home Header
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Get >> api/navigation/gatallnavigation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gatallnavigation")]
        public async Task<ResponseBodyModel> GetByNavigation()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var headerData = await _db.HomeHeaders.Where(x => x.IsActive && !x.IsDeleted &&
                    x.CompanyId == claims.companyId).ToListAsync();
                if (headerData.Count > 0)
                {
                    res.Message = "Get Navigation Data";
                    res.Status = true;
                    res.Data = headerData;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = headerData;
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

        #region This api use For home header Update
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Put >> api/navigation/updatenavigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatenavigation")]
        public async Task<ResponseBodyModel> UpdateNavigationData(HomeHeader model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var UpdateFirst = await _db.HomeHeaders.FirstOrDefaultAsync(x => x.NavigaionId == model.NavigaionId);
                if (UpdateFirst != null)
                {
                    UpdateFirst.NavigationName = model.NavigationName;
                    UpdateFirst.UpdatedBy = claims.employeeId;
                    UpdateFirst.UpdatedOn = DateTime.Now;

                    _db.Entry(UpdateFirst).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Sucessfully";
                    res.Status = true;
                    res.Data = UpdateFirst;
                }
                else
                {
                    res.Message = "Failed To Update";
                    res.Status = false;
                    res.Data = UpdateFirst;
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

        #region This Api Use for Get Data By Id Header
        ///// <summary>
        ///// created by Ankit jain Date - 20-10-2022
        ///// Api >> Get >> api/navigation/gatnavigationbyid
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("gatnavigationbyid")]
        public async Task<ResponseBodyModel> GetNavigationById(int navigationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var headerData = await _db.HomeHeaders.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                    x.CompanyId == claims.companyId && x.NavigaionId == navigationId);
                if (headerData != null)
                {
                    res.Message = "Get Navigation Data";
                    res.Status = true;
                    res.Data = headerData;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = true;
                    res.Data = headerData;
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

        #endregion

        #region This Api Use To SecondHeading

        #region This Api Use for Add Second Heading
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Post >> api/navigation/addheading
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addheading")]
        [Authorize]
        public async Task<ResponseBodyModel> AddHeading(SecondHeader model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var homeheader = await _db.HomeHeaders.Where(x => x.NavigaionId == model.NavigationId && x.IsActive
                    && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (homeheader != null)
                {
                    var secondheader = _db.SecondHeaders.Where(x => x.HeaderName == model.HeaderName && x.IsActive
                    && !x.IsDeleted && x.CompanyId == claims.companyId && x.NavigationId == homeheader.NavigaionId).FirstOrDefault();
                    if (secondheader == null)
                    {
                        SecondHeader post = new SecondHeader
                        {
                            HeaderName = model.HeaderName,
                            NavigationId = homeheader.NavigaionId,
                            CreatedBy = claims.employeeId,
                            CompanyId = claims.companyId,
                            OrgId = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false
                        };
                        _db.SecondHeaders.Add(post);
                        await _db.SaveChangesAsync();

                        res.Message = "Added Second Heading";
                        res.Status = true;
                        res.Data = post;
                    }
                    else
                    {
                        res.Message = "Header Already Exist";
                        res.Status = false;
                        res.Data = null;
                    }
                }
                else
                {
                    res.Message = "Not Founfd Second Heading";
                    res.Status = false;
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
        #endregion

        #region This api use For Update Second header 
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Put >> api/navigation/updateheading
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateheading")]
        public async Task<ResponseBodyModel> UpdateHeading(SecondHeader model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var secondheading = await _db.SecondHeaders.FirstOrDefaultAsync(x => x.SecondHeaderId ==
                    model.SecondHeaderId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (secondheading != null)
                {
                    secondheading.HeaderName = model.HeaderName;
                    secondheading.NavigationId = model.NavigationId;
                    secondheading.UpdatedBy = claims.employeeId;
                    secondheading.UpdatedOn = DateTime.Now;

                    _db.Entry(secondheading).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Second Heading";
                    res.Status = true;
                    res.Data = secondheading;
                }
                else
                {
                    res.Message = "data Not Update";
                    res.Status = false;
                    res.Data = secondheading;
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

        #region This Api Use for Get All Second Heading
        ///// <summary>
        ///// created by Ankit jain Date - 20-10-2022
        ///// Api >> Get >> api/navigation/getallsecondheading
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("getallsecondheading")]
        public async Task<ResponseBodyModel> GetAllHeader()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var secondheading = await (from h in _db.HomeHeaders
                                           join s in _db.SecondHeaders on h.NavigaionId equals s.NavigationId
                                           where h.CompanyId == claims.companyId && h.IsActive && !h.IsDeleted
                                           && s.CompanyId == claims.companyId && s.IsActive && !s.IsDeleted
                                           select new
                                           {
                                               s.SecondHeaderId,
                                               s.NavigationId,
                                               h.NavigationName,
                                               s.HeaderName,
                                           }).ToListAsync();

                //var secondheading = await _db.SecondHeaders.Where(x => x.IsActive && 
                //    !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (secondheading != null)
                {
                    res.Message = "Heading Data Found";
                    res.Status = true;
                    res.Data = secondheading;
                }
                else
                {
                    res.Message = "Data Not Found ";
                    res.Status = false;
                    res.Data = secondheading;
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

        #region This Api Use for Get Header Data By Id
        ///// <summary>
        ///// created by Ankit jain Date - 20-10-2022
        ///// Api >> Get >> api/navigation/getheadingbyid
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("getheadingbyid")]
        public async Task<ResponseBodyModel> GetHeaderSecond(int navigationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var SecondData = await _db.SecondHeaders.Where(x => x.NavigationId == navigationId && x.CompanyId == claims.companyId &&
                x.IsActive && !x.IsDeleted).ToListAsync();
                if (SecondData.Count > 0)
                {
                    res.Message = "Get Heading Data";
                    res.Status = true;
                    res.Data = SecondData;
                }
                else
                {
                    res.Message = "Data Not Found ";
                    res.Status = false;
                    res.Data = SecondData;
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

        #endregion

        #region This Api Use To Thired Heading

        #region This Api Use for Add Thired Header
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Post >> api/navigation/addthiredheader
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addthiredheader")]
        [Authorize]
        public async Task<ResponseBodyModel> AddThirdHeader(ThirdHeader model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var hederData = await _db.HomeHeaders.Where(x => x.NavigaionId == model.NavigationId &&
                x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (hederData != null)
                {
                    var secondheader = await _db.SecondHeaders.Where(x => x.SecondHeaderId == model.SecondHeaderId &&
                    x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                    if (secondheader != null)
                    {
                        ThirdHeader obj = new ThirdHeader
                        {
                            ContentName = model.ContentName,
                            RichText = model.RichText,
                            SecondHeaderId = model.SecondHeaderId,
                            NavigationId = model.NavigationId,
                            CreatedBy = claims.employeeId,
                            CompanyId = claims.companyId,
                            OrgId = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            PolicyGroupId = model.PolicyGroupId,
                            URL = model.URL,

                        };
                        _db.ThirdHeaders.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = "Added third header";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Header not found";
                        res.Status = true;
                        res.Data = secondheader;
                    }
                }
                else
                {
                    res.Message = "Header not found";
                    res.Status = true;
                    res.Data = hederData;
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

        #region This api use For Update Thired Header
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Post >> api/navigation/updatethirdheader
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatethirdheader")]
        public async Task<ResponseBodyModel> UpdateThirdHeader(ThirdHeader model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var thiredheader = await _db.ThirdHeaders.FirstOrDefaultAsync(x => x.ThirdHeaderId == model.ThirdHeaderId && x.IsActive && !x.IsDeleted);
                if (thiredheader != null)
                {
                    thiredheader.NavigationId = model.NavigationId;
                    thiredheader.SecondHeaderId = model.SecondHeaderId;
                    thiredheader.RichText = model.RichText;
                    thiredheader.ContentName = model.ContentName;
                    thiredheader.UpdatedBy = claims.employeeId;
                    thiredheader.UpdatedOn = DateTime.Now;

                    _db.Entry(thiredheader).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Sucessfully";
                    res.Status = true;
                    res.Data = thiredheader;
                }
                else
                {
                    res.Message = "Not Update Data";
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
        #endregion

        #region  This api use For Get All Thired Header Data
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Get >> api/navigation/getallthiredheader
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallthiredheader")]
        public async Task<ResponseBodyModel> GetAllThiredHeader()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var thiredheader = await (from h in _db.HomeHeaders
                                          join s in _db.SecondHeaders on h.NavigaionId equals s.NavigationId
                                          join t in _db.ThirdHeaders on s.SecondHeaderId equals t.SecondHeaderId
                                          where h.CompanyId == claims.companyId && h.IsActive && !h.IsDeleted
                                          && s.CompanyId == claims.companyId && s.IsActive && !s.IsDeleted
                                             && t.CompanyId == claims.companyId && t.IsActive && !t.IsDeleted
                                          select new
                                          {
                                              h.NavigationName,
                                              h.NavigaionId,
                                              s.HeaderName,
                                              s.SecondHeaderId,
                                              t.ContentName,
                                              t.ThirdHeaderId,
                                              t.RichText
                                          }).ToListAsync();
                //var thiredheader = await _db.ThirdHeaders.Where(x => x.IsActive &&
                //    !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (thiredheader.Count > 0)
                {
                    res.Message = "Get All Thired Header Data";
                    res.Status = true;
                    res.Data = thiredheader;
                }
                else
                {
                    res.Message = "Not found data";
                    res.Status = false;
                    res.Data = thiredheader;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion  This api used Third header Update

        #region  This api use For Get All Thired Header Data by id
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Get >> api/navigation/getallthiredheaderbyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallthiredheaderbyid")]
        public async Task<ResponseBodyModel> GetAllThiredHeaderById(int? thirdheaderId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var thiredheader = await (from h in _db.HomeHeaders
                                          join s in _db.SecondHeaders on h.NavigaionId equals s.NavigationId
                                          join t in _db.ThirdHeaders on s.SecondHeaderId equals t.SecondHeaderId
                                          where h.CompanyId == claims.companyId && h.IsActive && !h.IsDeleted
                                          && s.CompanyId == claims.companyId && s.IsActive && !s.IsDeleted
                                             && t.CompanyId == claims.companyId && t.IsActive && !t.IsDeleted && t.ThirdHeaderId == thirdheaderId
                                          select new
                                          {
                                              h.NavigationName,
                                              h.NavigaionId,
                                              s.HeaderName,
                                              s.SecondHeaderId,
                                              t.ContentName,
                                              t.ThirdHeaderId,
                                              t.RichText,
                                              h.NavigationNameId,
                                              t.PolicyGroupId,
                                              t.URL
                                          }).FirstOrDefaultAsync();
                //var thiredheader = await _db.ThirdHeaders.Where(x => x.IsActive &&
                //    !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (thiredheader != null)
                {
                    res.Message = "Get All Thired Header Data";
                    res.Status = true;
                    res.Data = thiredheader;
                }
                else
                {
                    res.Message = "Not found data";
                    res.Status = false;
                    res.Data = thiredheader;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion  This api used Third header Update

        #region This Api Use For Get Thired Header Data By Id
        ///// <summary>
        ///// created by Ankit jain Date - 20-10-2022
        ///// Api >> Get >> api/navigation/getthiredheadingbyid
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("getthiredheadingbyid")]
        public async Task<ResponseBodyModel> GetThiredHeadind(int navigationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var thiredheader = await _db.ThirdHeaders.Where(x => x.NavigationId == navigationId && x.CompanyId == claims.companyId
                && x.IsActive && !x.IsDeleted).ToListAsync();
                if (thiredheader.Count > 0)
                {
                    res.Message = "Get all Thired Heading data";
                    res.Status = true;
                    res.Data = thiredheader;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = thiredheader;
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

        #region This api use For Update Thired Header
        /// <summary>
        /// created by Ankit jain Date - 20-10-2022
        /// Api >> Post >> api/navigation/deletethirdheader
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletethirdheader")]
        public async Task<ResponseBodyModel> DeleteThirdHeader(int ThirdHeaderId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var thiredheader = await _db.ThirdHeaders.FirstOrDefaultAsync(x => x.ThirdHeaderId == ThirdHeaderId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (thiredheader != null)
                {
                    thiredheader.IsActive = false;
                    thiredheader.IsDeleted = true;
                    thiredheader.DeletedBy = claims.employeeId;
                    thiredheader.DeletedOn = DateTime.Now;

                    _db.Entry(thiredheader).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Delete Sucessfully";
                    res.Status = true;
                    res.Data = thiredheader;
                }
                else
                {
                    res.Message = "Data Not Deleted";
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
        #endregion


        #endregion

        #region This Api Use To Get All Data
        ///// <summary>
        ///// created by Ankit jain Date - 20-10-2022
        ///// Api >> Get >> api/navigation/getalldatabyid
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("getalldatabyid")]
        public async Task<ResponseBodyModel> GetDataById(int navigationid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.SecondHeaders.Where(x => x.NavigationId == navigationid &&
                    x.IsActive && !x.IsDeleted)
                    .Select(x => new HeadaersResponse
                    {
                        SecondHeaderId = x.SecondHeaderId,
                        HeaderName = x.HeaderName,
                        Name = _db.ThirdHeaders.Where(n => n.SecondHeaderId == x.SecondHeaderId &&
                        n.IsActive && !n.IsDeleted)
                        .Select(n => new
                        {
                            n.ContentName,
                            n.ThirdHeaderId,
                            n.RichText,
                            n.URL,
                        }).ToList(),
                    }).ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Data Get Succefully";
                    res.Status = true;
                    res.Data = getData;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = getData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class HeadaersResponse
        {
            public int SecondHeaderId { get; set; }
            public string HeaderName { get; set; }
            public object Name { get; set; }
        }
        #endregion

        #region Function To Create Default Navigation When Company Create
        // api/navigation/addingnavigationforall
        [Route("addingnavigationforall")]
        public void CreateNavigationFirstTime()
        {
            var companies = _db.Company.Select(x => x.CompanyId).Distinct().ToList();
            var navigation = _db.HomeHeaders.Select(x => x.CompanyId).Distinct().ToList();
            companies.RemoveAll(x => navigation.Contains(x));
            foreach (var companyId in companies)
            {
                var navigationTop = Enum.GetValues(typeof(NavigationConstants))
                                    .Cast<NavigationConstants>()
                                    .Select(x => new HomeHeader
                                    {
                                        NavigationName = x.ToString().Replace("_", " "),
                                        CreatedBy = 0,
                                        CreatedOn = DateTime.Now,
                                        CompanyId = companyId,
                                        OrgId = 0,
                                        IsActive = true,
                                        IsDeleted = false,

                                    }).ToList();
                _db.HomeHeaders.AddRange(navigationTop);
                _db.SaveChanges();
            }
        }
        #region Function To Create Default Navigation When Company Create
        // api/navigation/addingnavigation
        [Route("addingnavigation")]

        #endregion
        #endregion

        #region Delete Navigation For All Companies
        /// <summary>
        /// POST => api/navigation/removenavigationforallcompanies
        /// Created By Suraj Bundel on 03/11/2022
        /// </summary>
        [HttpPost]
        [Route("removenavigationforallcompanies")]
        public async void RemoveNavigationForAllCompanies()
        {
            var companiesList = await (from h in _db.HomeHeaders
                                       join c in _db.Company on h.CompanyId equals c.CompanyId
                                       where h.IsActive && !h.IsDeleted && c.IsActive && !c.IsDeleted
                                       select h)
                                       .ToListAsync();

            foreach (var companies in companiesList)
            {
                _db.Entry(companies).State = EntityState.Deleted;
                _db.SaveChanges();
            }
        }
        #endregion

        #region create navigation for all companies
        /// api/navigation/createnavigationforallcompanies
        [HttpPost]
        [Route("createnavigationforallcompanies")]
        public async void createnavigationforallcompanies()
        {
            var companiesdata = await _db.Company.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.CompanyId).ToListAsync();
            foreach (var companies in companiesdata)
            {
                var navigationtop = Enum.GetValues(typeof(NavigationConstants))
                                .Cast<NavigationConstants>()
                                .Select(x => new HomeHeader
                                {
                                    NavigationName = x.ToString().Replace("_", " "),
                                    NavigationNameId = x,
                                    CreatedBy = 0,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = companies,
                                    OrgId = 0,
                                    IsActive = true,
                                    IsDeleted = false,
                                }).ToList();
                _db.HomeHeaders.AddRange(navigationtop);
                _db.SaveChanges();
            }
        }
        #endregion

    }
}