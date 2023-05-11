using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.GiftsModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Gift
{
    [Authorize]
    [RoutePrefix("api/giftsmgt")]
    public class GiftsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Genrate Path For Gift Images

        /// <summary>
        /// Created by Shriya Malvi On 15-06-2022
        /// API >> Post >> api/giftsmgt/uploadmultipleimg
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadmultipleimg")]
        public async Task<UploadImageResponse> UploadMultipleImg()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/giftimages/" + claims.companyId + "/"), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;
                        string path = "uploadimage\\giftimages\\" + claims.companyId + "\\" + dates + '.' + Fileresult + extension;
                        //string path = "uploadimage\\userdocuments\\" + claims.employeeid + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = path;
                        result.Extension = extension;
                        result.ExtensionType = extemtionType;
                    }
                    else
                    {
                        result.Message = "You Pass 0 Content";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }

        #endregion Genrate Path For Gift Images

        #region Get All categories By base category Id

        /// <summary>
        /// /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/getcatebybaseid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcatebybaseid")]
        public async Task<ResponseBodyModel> GetCateByBaseId(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var category = await (from s in _db.GiftCategories
                                      where s.IsActive == true && s.IsDeleted == false
                                      && s.GBCategoryId == Id
                                      select new
                                      {
                                          s.GiftCategoryId,
                                          s.CategoryName,
                                      }).ToListAsync();

                if (category.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Category list Found";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Category list Not Found";
                    res.Data = category;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All categories By base category Id

        #region Get All  Base Base categories By category Id

        /// <summary>
        /// /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/getbbcategroiesbyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbbcategroiesbyid")]
        public async Task<ResponseBodyModel> GetBBCategroiesById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var category = await (from s in _db.GiftBaseBaseCategories
                                      where s.IsActive == true && s.IsDeleted == false
                                      && s.CategoryId == Id
                                      select new
                                      {
                                          s.GiftBBCategoryId,
                                          s.BBCategoryName,
                                      }).ToListAsync();

                if (category.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Category list Found";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Category list Not Found";
                    res.Data = category;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All  Base Base categories By category Id

        #region Gift Base Category  Api's

        #region Add Gift  Base Category

        /// <summary>
        /// Created by Shriya Malvi On 16-06-2022
        /// API >> Post >> api/giftsmgt/addgbasecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addgbasecategory")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddGBaseCategory(GiftBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    GiftBaseCategory category = new GiftBaseCategory();
                    category.BaseCategoryName = model.BaseCategoryName;
                    category.BaseCategoryCode = model.BaseCategoryCode;
                    category.Description = model.Description;
                    category.Image1 = model.Image1;
                    category.CompanyId = claims.companyId;
                    category.OrgId = claims.orgId;
                    category.CreatedBy = claims.employeeId;
                    category.CreatedOn = DateTime.Now;
                    category.IsActive = true;
                    category.IsDeleted = false;
                    _db.GiftBaseCategories.Add(category);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Category Added Succesfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Category Not Add";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Add Gift  Base Category

        #region Get All Base Category

        /// <summary>
        /// Created by Shriya Malvi On 16-06-2022
        /// API >> Get >> api/giftsmgt/getallbasecategory
        /// </summary>
        [Route("getallbasecategory")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllBaseCategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<GetHelperModel> categoryList = new List<GetHelperModel>();
                var employee = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
                var category = await _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                foreach (var item in category)
                {
                    GetHelperModel baseCate = new GetHelperModel();
                    baseCate.BaseCategoryId = item.GiftBCategoryId;
                    baseCate.BaseCategoryName = item.BaseCategoryName;
                    baseCate.BaseCategoryCode = item.BaseCategoryCode;
                    baseCate.Description = item.Description;
                    baseCate.Image1 = item.Image1;
                    baseCate.CreatedDate = item.CreatedOn;
                    baseCate.CreatedBy = item.CreatedBy;
                    baseCate.CreatedByName = employee.Where(a => a.EmployeeId == baseCate.CreatedBy).Select(a => a.DisplayName).FirstOrDefault();

                    categoryList.Add(baseCate);
                };
                if (categoryList.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Gift Base Category List Found ";
                    res.Data = categoryList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Base Category List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Base Category

        #region Get Base Category By Id

        /// <summary>
        /// Created by Shriya Malvi On 16-06-2022
        /// API >> Get >> api/giftsmgt/getbasecategorybyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbasecategorybyid")]
        public async Task<ResponseBodyModel> GetBaseCategoryById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var category = await _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftBCategoryId == Id).Select(x => new GetHelperModel
                {
                    BaseCategoryId = x.GiftBCategoryId,
                    BaseCategoryName = x.BaseCategoryName,
                    BaseCategoryCode = x.BaseCategoryCode,
                    Description = x.Description,
                    Image1 = x.Image1
                }).FirstOrDefaultAsync();

                if (category != null)
                {
                    res.Status = true;
                    res.Message = "Gift Base Category List Found ";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Base Category List Not Found ";
                    res.Data = category;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Base Category By Id

        #region Update Base Category by Id

        /// <summary>
        /// Created by Shriya Malvi On 16-06-2022
        /// API >> Put >> api/giftsmgt/updatebasecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updatebasecategory")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateBaseCategory(GiftBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftBCategoryId == model.GiftBCategoryId).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.BaseCategoryName = String.IsNullOrEmpty(model.BaseCategoryName) ? category.BaseCategoryName : model.BaseCategoryName;
                    category.BaseCategoryCode = String.IsNullOrEmpty(model.BaseCategoryCode) ? category.BaseCategoryCode : model.BaseCategoryCode;
                    category.Description = String.IsNullOrEmpty(model.Description) ? category.Description : model.Description;
                    category.Image1 = String.IsNullOrEmpty(model.Image1) ? category.Image1 : model.Image1;
                    //category.Image2 = String.IsNullOrEmpty(model.Image2) ? category.Image2 : model.Image2;
                    //category.Image3= String.IsNullOrEmpty(model.Image3) ? category.Image3 : model.Image3;
                    //category.Image4 = String.IsNullOrEmpty(model.Image4) ? category.Image4 : model.Image4;
                    //category.Image5= String.IsNullOrEmpty(model.Image5) ? category.Image5 : model.Image5;
                    category.UpdatedBy = claims.employeeId;
                    category.UpdatedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Base Category Updated  Successfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Base Category Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update Base Category by Id

        #region Delete Base  Category

        /// <summary>
        /// Created by Shriya Malvi On 16-06-2022
        /// API >> Delete >> api/giftsmgt/deletebasecategory
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletebasecategory")]
        public async Task<ResponseBodyModel> DeleteBaseCategory(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftBCategoryId == Id).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.IsActive = false;
                    category.IsDeleted = true;
                    category.DeletedBy = claims.employeeId;
                    category.DeletedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Base Category Deleted Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Base Category Not Delete ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Delete Base  Category

        #endregion Gift Base Category  Api's

        #region Gift Category Api's

        #region Add Gift Category

        /// <summary>
        /// Created by Shriya Malvi On 17-06-2022
        /// API >> Post >> api/giftsmgt/addgcategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addgcategory")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddGCategory(GiftCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var GBcategory = _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                if (model != null)
                {
                    GiftCategory category = new GiftCategory();
                    category.CategoryName = model.CategoryName;
                    category.GBCategoryId = model.GBCategoryId;
                    category.GBCategoryName = GBcategory.Where(x => x.GiftBCategoryId == category.GBCategoryId).Select(x => x.BaseCategoryName).FirstOrDefault();
                    category.Description = model.Description;
                    category.Image1 = model.Image1;
                    //category.Image2 = model.Image2;
                    //category.Image3 = model.Image3;
                    //category.Image4 = model.Image4;
                    //category.Image5 = model.Image5;
                    category.CompanyId = claims.companyId;
                    category.OrgId = claims.orgId;
                    category.CreatedBy = claims.employeeId;
                    category.CreatedOn = DateTime.Now;
                    category.IsActive = true;
                    category.IsDeleted = false;
                    _db.GiftCategories.Add(category);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Category Added Succesfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Category Not Add";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Add Gift Category

        #region Get All Category

        /// <summary>
        /// Created by Shriya Malvi On 17-06-2022
        /// API >> Get >> api/giftsmgt/getallcategory
        /// </summary>

        [Route("getallcategory")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllCategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<GetHelperModelForCate> categoryList = new List<GetHelperModelForCate>();
                var employee = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
                var category = await _db.GiftCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                foreach (var item in category)
                {
                    GetHelperModelForCate cateee = new GetHelperModelForCate();
                    cateee.CategoryId = item.GiftCategoryId;
                    cateee.CategoryName = item.CategoryName;
                    cateee.GBCategoryId = item.GBCategoryId;
                    cateee.GBCategoryName = item.GBCategoryName;
                    cateee.Description = item.Description;
                    cateee.Image1 = item.Image1;
                    cateee.CreatedDate = item.CreatedOn;
                    cateee.CreatedBy = item.CreatedBy;
                    cateee.CreatedByName = employee.Where(a => a.EmployeeId == cateee.CreatedBy).Select(a => a.DisplayName).FirstOrDefault();

                    categoryList.Add(cateee);
                }
                if (categoryList.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Gift Category List Found ";
                    res.Data = categoryList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift  Category List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Category

        #region Get Category By Id

        /// <summary>
        /// Created by Shriya Malvi On 16-06-2022
        /// API >> Get >> api/giftsmgt/getcategorybyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcategorybyid")]
        public async Task<ResponseBodyModel> GetCategoryById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var category = await _db.GiftCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftCategoryId == Id).Select(x => new GetHelperModelForCate
                {
                    CategoryId = x.GiftCategoryId,
                    CategoryName = x.CategoryName,
                    GBCategoryId = x.GBCategoryId,
                    GBCategoryName = x.GBCategoryName,
                    Description = x.Description,
                    Image1 = x.Image1
                }).FirstOrDefaultAsync();

                if (category != null)
                {
                    res.Status = true;
                    res.Message = "Gift  Category List Found ";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift  Category List Not Found ";
                    res.Data = category;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Category By Id

        #region Update Category by Id

        /// <summary>
        /// Created by Shriya Malvi On 17-06-2022
        /// API >> Put >> api/giftsmgt/updatecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updatecategory")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateCategory(GiftCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var GBcategory = _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                var category = await _db.GiftCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftCategoryId == model.GiftCategoryId).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.CategoryName = String.IsNullOrEmpty(model.CategoryName) ? category.CategoryName : model.CategoryName;
                    category.GBCategoryId = model.GBCategoryId == 0 ? category.GBCategoryId : model.GBCategoryId;
                    category.GBCategoryName = GBcategory.Where(x => x.GiftBCategoryId == category.GBCategoryId).Select(x => x.BaseCategoryName).FirstOrDefault();
                    category.Description = String.IsNullOrEmpty(model.Description) ? category.Description : model.Description;
                    category.Image1 = String.IsNullOrEmpty(model.Image1) ? category.Image1 : model.Image1;
                    //category.Image2 = String.IsNullOrEmpty(model.Image2) ? category.Image2 : model.Image2;
                    //category.Image3= String.IsNullOrEmpty(model.Image3) ? category.Image3 : model.Image3;
                    //category.Image4 = String.IsNullOrEmpty(model.Image4) ? category.Image4 : model.Image4;
                    //category.Image5= String.IsNullOrEmpty(model.Image5) ? category.Image5 : model.Image5;
                    category.UpdatedBy = claims.employeeId;
                    category.UpdatedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Category Updated  Successfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Category Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update Category by Id

        #region Delete Category

        /// <summary>
        /// Created by Shriya Malvi On 17-06-2022
        /// API >> Delete >> api/giftsmgt/deletecategory
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecategory")]
        public async Task<ResponseBodyModel> DeleteCategory(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftCategoryId == Id).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.IsActive = false;
                    category.IsDeleted = true;
                    category.DeletedBy = claims.employeeId;
                    category.DeletedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Category Deleted Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = " Category Not Delete ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Delete Category

        #endregion Gift Category Api's

        #region Gift BaseBase category  Api's

        #region Add  Gift BaseBase  Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Post >> api/giftsmgt/addbasebasecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addbasebasecategory")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddBaseBaseCategory(GiftBaseBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitingCate = _db.GiftCategories.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                var exitingBase = _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                if (model != null)
                {
                    GiftBaseBaseCategory category = new GiftBaseBaseCategory();
                    category.BBCategoryName = model.BBCategoryName;
                    category.BaseCategoryId = model.BaseCategoryId;
                    category.BaseCategoryName = exitingBase.Where(x => x.GiftBCategoryId == category.BaseCategoryId).Select(x => x.BaseCategoryName).FirstOrDefault();
                    category.CategoryId = model.CategoryId;
                    category.CategoryName = exitingCate.Where(x => x.GiftCategoryId == category.CategoryId).Select(x => x.CategoryName).FirstOrDefault();
                    category.Description = model.Description;
                    category.Image1 = model.Image1;
                    category.CompanyId = claims.companyId;
                    category.OrgId = claims.orgId;
                    category.CreatedBy = claims.employeeId;
                    category.CreatedOn = DateTime.Now;
                    category.IsActive = true;
                    category.IsDeleted = false;
                    _db.GiftBaseBaseCategories.Add(category);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "BaseBase Category Added Succesfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "BaseBase Category Not Add";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Add  Gift BaseBase  Category

        #region Get All BaseBase Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/getallbasebasecategory
        /// </summary>
        [Route("getallbasebasecategory")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllBaseBaseCategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
                List<GetHelperModalForBBCategory> bBCatList = new List<GetHelperModalForBBCategory>();
                var category = await _db.GiftBaseBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                foreach (var item in category)
                {
                    GetHelperModalForBBCategory bbcatee = new GetHelperModalForBBCategory();
                    bbcatee.BBCategoryId = item.GiftBBCategoryId;
                    bbcatee.BBCategoryName = item.BBCategoryName;
                    bbcatee.BaseCategoryId = item.BaseCategoryId;
                    bbcatee.BaseCategoryName = item.BaseCategoryName;
                    bbcatee.CategoryId = item.CategoryId;
                    bbcatee.CategoryName = item.CategoryName;
                    bbcatee.Description = item.Description;
                    bbcatee.Image1 = item.Image1;
                    bbcatee.CreatedDate = item.CreatedOn;
                    bbcatee.CreatedBy = item.CreatedBy;
                    bbcatee.CreatedByName = employee.Where(x => x.EmployeeId == bbcatee.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();

                    bBCatList.Add(bbcatee);
                }
                if (bBCatList.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Gift BaseBase Category List Found ";
                    res.Data = bBCatList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift BaseBase Category List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All BaseBase Category

        #region Get BaseBase Category By Id

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/getbbcategorybyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbbcategorybyid")]
        public async Task<ResponseBodyModel> GetBBCategoryById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var category = await _db.GiftBaseBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftBBCategoryId == Id).Select(x => new GetHelperModalForBBCategory
                {
                    BBCategoryId = x.GiftBBCategoryId,
                    BBCategoryName = x.BBCategoryName,
                    BaseCategoryId = x.BaseCategoryId,
                    BaseCategoryName = x.BaseCategoryName,
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    Image1 = x.Image1,
                }).FirstOrDefaultAsync();

                if (category != null)
                {
                    res.Status = true;
                    res.Message = "Gift BaseBase Category List Found ";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift BaseBase Category List Not Found ";
                    res.Data = category;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get BaseBase Category By Id

        #region Update BaseBase Category by Id

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Put >> api/giftsmgt/UpdateBBCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateBBCategory")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateBBCategory(GiftBaseBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var GBcategory = _db.GiftCategories.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                var exitingBase = _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                var category = await _db.GiftBaseBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftBBCategoryId == model.GiftBBCategoryId).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.BBCategoryName = String.IsNullOrEmpty(model.BBCategoryName) ? category.BBCategoryName : model.BBCategoryName;
                    category.BaseCategoryId = model.CategoryId == 0 ? category.BaseCategoryId : model.BaseCategoryId;
                    category.BaseCategoryName = exitingBase.Where(x => x.GiftBCategoryId == category.BaseCategoryId).Select(x => x.BaseCategoryName).FirstOrDefault();
                    category.CategoryId = model.CategoryId == 0 ? category.CategoryId : model.CategoryId;
                    category.CategoryName = GBcategory.Where(x => x.GiftCategoryId == category.CategoryId).Select(x => x.CategoryName).FirstOrDefault();
                    category.Description = String.IsNullOrEmpty(model.Description) ? category.Description : model.Description;
                    category.Image1 = String.IsNullOrEmpty(model.Image1) ? category.Image1 : model.Image1;

                    category.UpdatedBy = claims.employeeId;
                    category.UpdatedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = " BaseBase Category Updated  Successfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "BaseBase Category Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update BaseBase Category by Id

        #region Delete  BaseBase Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Delete >> api/giftsmgt/deletebbcategory
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletebbcategory")]
        public async Task<ResponseBodyModel> DeleteBBCategory(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftBaseBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftBBCategoryId == Id).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.IsActive = false;
                    category.IsDeleted = true;
                    category.DeletedBy = claims.employeeId;
                    category.DeletedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "BaseBase Category Deleted Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = "BaseBase Category Not Delete ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Delete  BaseBase Category

        #endregion Gift BaseBase category  Api's

        #region Gift Item Ctaegory Api's

        #region Add Gift  Item Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Post >> api/giftsmgt/addgitemcategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addgitemcategory")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddGItemCategory(GiftItemCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var exitingCount = _db.GiftItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).Count();
                    if (exitingCount < 10)
                    {
                        GiftItemCategory icategory = new GiftItemCategory();
                        icategory.GItemCategoryName = model.GItemCategoryName;
                        icategory.CategoryCods = model.CategoryCods;
                        icategory.CompanyId = claims.companyId;
                        icategory.OrgId = claims.orgId;
                        icategory.CreatedBy = claims.employeeId;
                        icategory.CreatedOn = DateTime.Now;
                        icategory.IsActive = true;
                        icategory.IsDeleted = false;

                        _db.GiftItemCategories.Add(icategory);
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Item Category Added Succesfully";
                        res.Data = icategory;
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "Can Not Add";
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Item Category Not Add";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Add Gift  Item Category

        #region Get All Item Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/getallitemcategory
        /// </summary>
        [Route("getallitemcategory")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllItemCategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();

                if (category.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Gift BaseBase Category List Found ";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift BaseBase Category List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Item Category

        #region Get Item Category By Id

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/itemcategorybyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("itemcategorybyid")]
        public async Task<ResponseBodyModel> ItemCategoryById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var category = await _db.GiftItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GItemCategoryId == Id).FirstOrDefaultAsync();
                if (category != null)
                {
                    res.Status = true;
                    res.Message = "Gift Item Category List Found ";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Item Category List Not Found ";
                    res.Data = category;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Item Category By Id

        #region Update Item Category by Id

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Put >> api/giftsmgt/updateitemcategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updateitemcategory")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateItemCategory(GiftItemCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GItemCategoryId == model.GItemCategoryId).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.GItemCategoryName = String.IsNullOrEmpty(model.GItemCategoryName) ? category.GItemCategoryName : model.GItemCategoryName;
                    category.CategoryCods = String.IsNullOrEmpty(model.CategoryCods) ? category.CategoryCods : model.CategoryCods;

                    category.UpdatedBy = claims.employeeId;
                    category.UpdatedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Base Category Updated  Successfully";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Base Category Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update Item Category by Id

        #region Delete Item  Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Delete >> api/giftsmgt/deleteitemcategory
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteitemcategory")]
        public async Task<ResponseBodyModel> DeleteItemCategory(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.GiftItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.GItemCategoryId == Id).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.IsActive = false;
                    category.IsDeleted = true;
                    category.DeletedBy = claims.employeeId;
                    category.DeletedOn = DateTime.Now;
                    _db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Item Category Deleted Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Item Category Not Delete ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Delete Item  Category

        #endregion Gift Item Ctaegory Api's

        #region Gift Items Api's

        #region Add Gift Item

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Post >> api/giftsmgt/addgiftitem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addgiftitem")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddGiftItem(GiftItem model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    GiftItem gitem = new GiftItem();
                    gitem.ItemName = model.ItemName;
                    gitem.GBCategoryId = model.GBCategoryId;
                    gitem.GCategoryId = model.GCategoryId;
                    gitem.GBBCategoryId = model.GBBCategoryId;
                    gitem.UnitOfStock = model.UnitOfStock;
                    gitem.MRP = model.MRP;
                    gitem.PurchasePrice = model.PurchasePrice;
                    gitem.ColorPicker = model.ColorPicker;
                    gitem.Image = model.Image;
                    gitem.CompanyId = claims.companyId;
                    gitem.OrgId = claims.orgId;
                    gitem.CreatedBy = claims.employeeId;
                    gitem.CreatedOn = DateTime.Now;
                    gitem.IsActive = true;
                    gitem.IsDeleted = false;
                    _db.GiftItems.Add(gitem);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Item Added Succesfully";
                    res.Data = gitem;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Item Not Add";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Add Gift Item

        #region Get All Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Get >> api/giftsmgt/getallgiftitem
        /// </summary>

        [Route("getallgiftitem")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllGiftItem()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<GiftItemModel> itemList = new List<GiftItemModel>();
                var employee = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
                var giftitem = await (from s in _db.GiftItems
                                      join gb in _db.GiftBaseCategories on s.GBCategoryId equals gb.GiftBCategoryId
                                      join gc in _db.GiftCategories on s.GCategoryId equals gc.GiftCategoryId
                                      join gbb in _db.GiftBaseBaseCategories on s.GBBCategoryId equals gbb.GiftBBCategoryId
                                      where s.IsActive == true && s.IsDeleted == false && s.CompanyId == claims.companyId
                                      select new
                                      {
                                          s.GiftItemId,
                                          s.ItemName,
                                          s.GBCategoryId,
                                          gb.BaseCategoryName,
                                          s.GCategoryId,
                                          gc.CategoryName,
                                          s.GBBCategoryId,
                                          gbb.BBCategoryName,
                                          s.UnitOfStock,
                                          s.MRP,
                                          s.PurchasePrice,
                                          s.CreatedBy,
                                          s.CreatedOn,
                                          s.UpdatedBy
                                      }).ToListAsync();

                foreach (var item in giftitem)
                {
                    GiftItemModel gift = new GiftItemModel();
                    gift.GiftItemId = item.GiftItemId;
                    gift.ItemName = item.ItemName;
                    gift.GBCategoryId = item.GBCategoryId;
                    gift.CreatedBy = item.CreatedBy;
                    gift.CreatedDate = item.CreatedOn;
                    gift.CreatedByName = employee.Where(a => a.EmployeeId == gift.CreatedBy).Select(a => a.DisplayName).FirstOrDefault();
                    gift.UnitOfStock = item.UnitOfStock;
                    gift.MRP = item.MRP;
                    gift.PurchasePrice = item.PurchasePrice;
                    gift.TotalPrice = gift.UnitOfStock * gift.PurchasePrice;
                    gift.AllTheCategory = item.BaseCategoryName + " " + item.CategoryName + " " + item.BBCategoryName;
                    itemList.Add(gift);
                }
                if (itemList.Count > 0)
                {
                    res.Status = true;
                    res.Message = " Gift Item List Found ";
                    res.Data = itemList;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift  Item List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Category

        #region Delete Category

        /// <summary>
        /// Created by Shriya Malvi On 20-06-2022
        /// API >> Delete >> api/giftsmgt/deleteitem
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteitem")]
        public async Task<ResponseBodyModel> DeleteItem(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var item = await _db.GiftItems.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftItemId == Id).FirstOrDefaultAsync();
                if (item != null)
                {
                    item.IsActive = false;
                    item.IsDeleted = true;
                    item.DeletedBy = claims.employeeId;
                    item.DeletedOn = DateTime.Now;
                    _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Item Deleted Successfully";
                }
                else
                {
                    res.Status = false;
                    res.Message = " Item Not Delete ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Delete Category

        #region Get Item Deatil  By Item Id

        /// <summary>
        /// Created by Shriya Malvi On 21-06-2022
        /// API >> Get >> api/giftsmgt/getitembyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getitembyid")]
        public async Task<ResponseBodyModel> GetItemById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var giftitem = await (from s in _db.GiftItems
                                      join gb in _db.GiftBaseCategories on s.GBCategoryId equals gb.GiftBCategoryId
                                      join gc in _db.GiftCategories on s.GCategoryId equals gc.GiftCategoryId
                                      join gbb in _db.GiftBaseBaseCategories on s.GBBCategoryId equals gbb.GiftBBCategoryId
                                      where s.IsActive == true && s.IsDeleted == false && s.CompanyId == claims.companyId && s.GiftItemId == Id
                                      select new GiftItemModel
                                      {
                                          GiftItemId = s.GiftItemId,
                                          ItemName = s.ItemName,
                                          GBCategoryId = s.GBCategoryId,
                                          GBCategoryName = gb.BaseCategoryName,
                                          GCategoryId = s.GCategoryId,
                                          CategoryName = gc.CategoryName,
                                          GBBCategoryId = s.GBBCategoryId,
                                          GBbCategoryName = gbb.BBCategoryName,
                                          UnitOfStock = s.UnitOfStock,
                                          MRP = s.MRP,
                                          PurchasePrice = s.PurchasePrice,
                                          CreatedBy = s.CreatedBy,
                                          CreatedDate = s.CreatedOn,
                                          Image = s.Image,
                                      }).FirstOrDefaultAsync();

                if (giftitem != null)
                {
                    res.Status = true;
                    res.Message = "Gift Item List Found ";
                    res.Data = giftitem;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Item  List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Item Deatil  By Item Id

        #region Update Item

        /// <summary>
        /// Created by Shriya Malvi On 21-06-2022
        /// API >> Put >> api/giftsmgt/updateitem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updateitem")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateItem(GiftItem model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var gitem = await _db.GiftItems.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftItemId == model.GiftItemId).FirstOrDefaultAsync();
                if (gitem != null)
                {
                    gitem.ItemName = string.IsNullOrEmpty(model.ItemName) ? gitem.ItemName : model.ItemName;
                    gitem.GBCategoryId = model.GBCategoryId == 0 ? gitem.GBCategoryId : model.GBCategoryId;
                    gitem.GCategoryId = model.GCategoryId == 0 ? gitem.GCategoryId : model.GCategoryId;
                    gitem.GBBCategoryId = model.GBBCategoryId == 0 ? gitem.GBBCategoryId : model.GBBCategoryId;
                    gitem.UnitOfStock = model.UnitOfStock;
                    gitem.MRP = model.MRP;
                    gitem.PurchasePrice = model.PurchasePrice;
                    gitem.ColorPicker = model.ColorPicker;
                    gitem.Image = string.IsNullOrEmpty(model.Image) ? gitem.Image : model.Image;
                    gitem.UpdatedBy = claims.employeeId;
                    gitem.UpdatedOn = DateTime.Now;
                    _db.Entry(gitem).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Item Updated  Successfully";
                    res.Data = gitem;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Item Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update Item

        #endregion Gift Items Api's

        #region Gift Assign  Api's

        #region Assign Gift To Emp

        /// <summary>
        /// Created by Shriya Malvi On 21-06-2022
        /// API >> Post >> api/giftsmgt/assigngiftstoemp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("assigngiftstoemp")]
        [HttpPost]
        public async Task<ResponseBodyModel> AssignGiftsToEmp(GiftAssign model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var employee = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
            try
            {
                if (model != null)
                {
                    var exitingitem = _db.GiftItems.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftItemId == model.GItemId).FirstOrDefault();
                    if (exitingitem != null)
                    {
                        exitingitem.UnitOfStock = exitingitem.UnitOfStock - 1;
                        _db.Entry(exitingitem).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        GiftAssign category = new GiftAssign();
                        category.EmployeeId = model.EmployeeId;
                        category.EmployeeName = employee.Where(x => x.EmployeeId == category.EmployeeId).Select(x => x.DisplayName).FirstOrDefault();
                        category.ItemCategory = model.ItemCategory;
                        category.GItemId = model.GItemId;
                        category.GItem = "Assigned";
                        category.CompanyId = claims.companyId;
                        category.OrgId = claims.orgId;
                        category.CreatedBy = claims.employeeId;
                        category.CreatedOn = DateTime.Now;
                        category.IsActive = true;
                        category.IsDeleted = false;
                        _db.GiftAssigns.Add(category);
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Gift Assign Succesfully";
                        res.Data = category;
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Not Assign";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Assign Gift To Emp

        #region Get All Assign Gifts

        /// <summary>
        /// Created by Shriya Malvi On 21-06-2022
        /// API >> Get >> api/giftsmgt/getallassigngifts
        /// </summary>
        [Route("getallassigngifts")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllAssignGifts()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                // List<GiftAssignHelper> categoryList = new List<GiftAssignHelper>();
                // = await _db.GiftBaseCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyid && x.OrgId == claims.orgid).ToListAsync();
                var category = await (from a in _db.GiftAssigns
                                      join s in _db.GiftItems on a.GItemId equals s.GiftItemId
                                      join ic in _db.GiftItemCategories on a.ItemCategory equals ic.GItemCategoryId
                                      join e in _db.Employee on a.CreatedBy equals e.EmployeeId
                                      where a.IsActive == true && a.IsDeleted == false && a.CompanyId == claims.companyId
                                      select new GiftAssignHelper
                                      {
                                          GiftAssignId = a.GiftAssignId,
                                          EmployeeId = a.EmployeeId,
                                          EmployeeName = a.EmployeeName,
                                          GItemId = a.GItemId,
                                          GItemName = s.ItemName,
                                          ItemCategory = a.ItemCategory,
                                          ItemCategoryName = ic.GItemCategoryName,
                                          AssignStatus = a.GItem,
                                          GiftImage = s.Image,
                                          CreatedBy = s.CreatedBy,
                                          CreatedByName = e.DisplayName,
                                          CreatedDate = s.CreatedOn
                                      }).ToListAsync();

                if (category.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Gift Assign List Found ";
                    res.Data = category;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Assign List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Assign Gifts

        #region Get Assign Gift By Assign ID

        /// <summary>
        /// Created by Shriya Malvi On 21-06-2022
        /// API >> Get >> api/giftsmgt/getassigngbyid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getassigngbyid")]
        public async Task<ResponseBodyModel> GetAssignGById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var AssignGift = await (from a in _db.GiftAssigns
                                        join s in _db.GiftItems on a.GItemId equals s.GiftItemId
                                        join ic in _db.GiftItemCategories on a.ItemCategory equals ic.GItemCategoryId
                                        join e in _db.Employee on a.CreatedBy equals e.EmployeeId
                                        where a.IsActive == true && a.IsDeleted == false && a.CompanyId == claims.companyId && a.GiftAssignId == Id
                                        select new GiftAssignHelper
                                        {
                                            GiftAssignId = a.GiftAssignId,
                                            EmployeeId = a.EmployeeId,
                                            EmployeeName = a.EmployeeName,
                                            GItemId = a.GItemId,
                                            GItemName = s.ItemName,
                                            ItemCategory = a.ItemCategory,
                                            ItemCategoryName = ic.GItemCategoryName,
                                            AssignStatus = a.GItem,
                                            GiftImage = s.Image,
                                            CreatedBy = s.CreatedBy,
                                            CreatedByName = e.DisplayName,
                                            CreatedDate = s.CreatedOn
                                        }).ToListAsync();

                if (AssignGift != null)
                {
                    res.Status = true;
                    res.Message = "Gift Assign List Found ";
                    res.Data = AssignGift;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Gift Assign List Not Found ";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Assign Gift By Assign ID

        #region Update By Assign Id

        /// <summary>
        /// Created by Shriya Malvi On 21-06-2022
        /// API >> Put >> api/giftsmgt/updateassigndetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updateassigndetail")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateAssignDetail(GiftAssign model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
                var gitem = await _db.GiftAssigns.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftAssignId == model.GiftAssignId).FirstOrDefaultAsync();
                if (gitem != null)
                {
                    if (gitem.GItemId != model.GItemId && model.GItemId != 0)
                    {
                        var exitingUnits = _db.GiftItems.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftItemId == model.GItemId).FirstOrDefault();
                        var exitingitem = _db.GiftItems.Where(x => x.IsActive == true && x.IsDeleted == false && x.GiftItemId == gitem.GItemId).FirstOrDefault();

                        exitingitem.UnitOfStock = exitingitem.UnitOfStock + 1;
                        _db.Entry(exitingitem).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        exitingUnits.UnitOfStock = exitingUnits.UnitOfStock - 1;
                        _db.Entry(exitingitem).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        gitem.EmployeeId = model.EmployeeId == 0 ? gitem.EmployeeId : model.EmployeeId;
                        gitem.EmployeeName = employee.Where(x => x.EmployeeId == gitem.EmployeeId).Select(x => x.DisplayName).FirstOrDefault();
                        gitem.GItemId = model.GItemId;
                        gitem.ItemCategory = model.ItemCategory == 0 ? gitem.ItemCategory : model.ItemCategory;
                        gitem.UpdatedBy = claims.employeeId;
                        gitem.UpdatedOn = DateTime.Now;
                        _db.Entry(gitem).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        res.Status = true;
                        res.Message = "Item Updated  Successfully";
                        res.Data = gitem;
                    }
                    else
                    {
                        gitem.EmployeeId = model.EmployeeId == 0 ? gitem.EmployeeId : model.EmployeeId;
                        gitem.EmployeeName = employee.Where(x => x.EmployeeId == gitem.EmployeeId).Select(x => x.DisplayName).FirstOrDefault();
                        gitem.GItemId = model.GItemId == 0 ? gitem.GItemId : model.GItemId;
                        gitem.ItemCategory = model.ItemCategory == 0 ? gitem.ItemCategory : model.ItemCategory;
                        gitem.UpdatedBy = claims.employeeId;
                        gitem.UpdatedOn = DateTime.Now;
                        _db.Entry(gitem).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        res.Status = true;
                        res.Message = "Item Updated  Successfully";
                        res.Data = gitem;
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Item Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Update By Assign Id

        #endregion Gift Assign  Api's

        #region Helper Modal Class

        public class GetHelperModel
        {
            public int BaseCategoryId { get; set; }
            public String BaseCategoryName { get; set; }
            public string BaseCategoryCode { get; set; }
            public string Description { get; set; }
            public string Image1 { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
        }

        public class GetHelperModelForCate
        {
            public int CategoryId { get; set; }
            public String CategoryName { get; set; }
            public int GBCategoryId { get; set; }
            public string GBCategoryName { get; set; }
            public string Description { get; set; }
            public string Image1 { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
        }

        public class GetHelperModalForBBCategory
        {
            public int BBCategoryId { get; set; }
            public String BBCategoryName { get; set; }
            public int BaseCategoryId { get; set; }
            public string BaseCategoryName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public string Image1 { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
        }

        public class GiftItemModel
        {
            public int GiftItemId { get; set; }
            public string ItemName { get; set; }
            public int GBCategoryId { get; set; }
            public string GBCategoryName { get; set; }
            public int GCategoryId { get; set; }
            public string CategoryName { get; set; }
            public int GBBCategoryId { get; set; }
            public string GBbCategoryName { get; set; }
            public int UnitOfStock { get; set; }
            public double MRP { get; set; }
            public double PurchasePrice { get; set; }
            public double TotalPrice { get; set; }
            public string Image { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
            public string AllTheCategory { get; set; }
        }

        public class GiftAssignHelper
        {
            public int GiftAssignId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int GItemId { get; set; }
            public string GItemName { get; set; }
            public string AssignStatus { get; set; }
            public int ItemCategory { get; set; }
            public string ItemCategoryName { get; set; }
            public string GiftImage { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
        }

        #endregion Helper Modal Class
    }
}