using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Mayank Prajapati On 02-09-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/news")]
    public class NewsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        #region This Api Use for Post News
        /// <summary>
        /// created by Mayank Prajapati on 1/9/2022
        /// Api >> Post >> api/news/newspost
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("newspost")]
        [Authorize]
        public async Task<ResponseBodyModel> PostNews(NewsEntity model)
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
                    NewsEntity obj = new NewsEntity
                    {
                        Title = model.Title,
                        Image = model.Image,
                        Description = model.Description,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        NewsType = model.NewsType,
                    };
                    _db.NewsEntities.Add(obj);
                    await _db.SaveChangesAsync();
                    res.Message = "News Added Succesfully ! ";
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
        #endregion

        #region Api for  Get Top 3 News
        /// <summary>
        /// API > GET > api/news/gettopnews
        /// Created By Mayank Prajapti on 02-09-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gettopnews")]
        public async Task<ResponseBodyModel> GetTopNews()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var m = indianTime.Month;
            var y = indianTime.Year;
            var d = indianTime.Day;
            try
            {
                var newsData = await _db.NewsEntities.Where(x => x.IsDeleted == false && x.CompanyId == claims.companyId
                                       && x.IsActive == true).Take(3).OrderByDescending(x => x.NewsId).ToListAsync();
                if (newsData.Count > 0)
                {
                    res.Message = "News Get !";
                    res.Status = true;
                    res.Data = newsData;
                }
                else
                {
                    res.Message = "Failed To Get News !";
                    res.Status = false;
                    res.Data = newsData;
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

        #region  Api for Update News Data
        /// <summary>
        /// Created By Mayank Prajapati On 02-09-2022
        /// Api >> Update >> api/news/updatenews
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatenews")]

        public async Task<ResponseBodyModel> EditNewsData(NewsEntity model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateData = await _db.NewsEntities.FirstOrDefaultAsync(x => x.NewsId == model.NewsId && x.CompanyId == claims.companyId &&
                                                  x.IsActive == true && x.IsDeleted == false);
                if (updateData != null)
                {
                    updateData.NewsId = model.NewsId;
                    updateData.Title = model.Title;
                    updateData.Description = model.Description;
                    updateData.Image = model.Image;
                    updateData.UpdatedBy = claims.employeeId;
                    updateData.CompanyId = claims.companyId;
                    updateData.OrgId = claims.employeeId;
                    updateData.UpdatedOn = DateTime.Now;
                    updateData.IsActive = true;
                    updateData.IsDeleted = false;
                    updateData.NewsType = model.NewsType;
                    _db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "News Update Succesfully !";
                    res.Status = true;
                    res.Data = updateData;
                }
                else
                {
                    res.Message = " Failed To Update !";
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

        #region  Api for Delete News Data 
        /// <summary>
        /// Created By Mayank Prajapati On 02-09-2022
        /// Api >> Delete >> api/news/deletenews
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletenews")]
        public async Task<ResponseBodyModel> DeleteNewsData(Guid id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var deleteData = await _db.NewsEntities.FirstOrDefaultAsync(x => x.NewsId == id && x.IsDeleted == false
                                                        && x.IsActive == true && x.CompanyId == claims.companyId);
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    deleteData.DeletedBy = claims.employeeId;
                    deleteData.DeletedOn = DateTime.Now;

                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "News Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "News Data not Found!!";
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

        #region  Upload Image Api 
        /// <summary>
        /// Created By Mayank Prajapati On 02-09-2022
        /// API>>POST>>api/news/uploadimage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadimage")]
        public async Task<UploadImageResponse> UploadImage()
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
                        if (extemtionType == "image" || extemtionType == "application")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/News/" + claims.companyId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }

                            string path = "uploadimage\\News\\" + claims.companyId + "\\" + dates + '.' + Fileresult + extension;


                            //string path = "uploadimage\\holidayimages\\" + claims.employeeid + "\\" + dates + '.' + Fileresult + extension;

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
                            result.Message = "Only Select Image Format";
                            result.Status = false;
                        }
                    }
                    else
                    {
                        result.Message = "No content Passed ";
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

        #endregion Api To Use Delete 

        #region Api for Get News Of Curent Date
        /// <summary>
        /// Created By Mayank Prajapati On 02-09-2022
        /// API>>GET>>api/news/getdatabydate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdatabydate")]

        public async Task<ResponseBodyModel> GetDataByDate()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            var m = indianTime.Month;
            var y = indianTime.Year;
            var d = indianTime.Day;


            var currentdata = await _db.NewsEntities.Where(x => x.IsActive == true && x.IsDeleted == false && x.CreatedOn.Day == indianTime.Day &&
                                                  x.CompanyId == claims.companyId).ToListAsync();
            if (currentdata.Count > 0)
            {
                res.Message = "News Data Get !";
                res.Status = true;
                res.Data = currentdata;
            }
            else
            {
                res.Message = "Failed To Get News !";
                res.Status = false;
                res.Data = currentdata;
            }

            return res;
        }
        #endregion

        #region Api for  Get All News
        /// <summary>
        /// API > GET > api/news/getallnews
        /// Created By Mayank Prajapti on 02-09-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallnews")]
        public async Task<ResponseBodyModel> GetNews(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var newsData = await _db.NewsEntities
                    .Where(x => !x.IsDeleted && x.CompanyId == claims.companyId
                        && x.NewsType == Core.Enum.NewsEnumType.Publish && x.IsActive)
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();
                if (newsData.Count > 0)
                {
                    res.Message = "Get All News list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {

                        res.Data = new PaginationData
                        {
                            TotalData = newsData.Count,
                            Counts = (int)count,
                            List = newsData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = newsData;
                    }
                }
                else
                {
                    res.Message = "Failed To Get News !";
                    res.Status = false;
                    res.Data = newsData;
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

        #region Api for  Get  News By News Id
        /// <summary>
        /// API > GET > api/news/getnewsbyid
        /// Created By Mayank Prajapti on 02-09-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewsbyid")]
        public async Task<ResponseBodyModel> GetNewsById(Guid newsId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var newsData = await _db.NewsEntities.Where(x => x.IsDeleted == false && x.CompanyId == claims.companyId
                                       && x.IsActive == true && x.NewsId == newsId).FirstOrDefaultAsync();
                if (newsData != null)
                {
                    res.Message = "Found";
                    res.Status = true;
                    res.Data = newsData;
                }
                else
                {
                    res.Message = "Failed To Get News !";
                    res.Status = false;
                    res.Data = newsData;
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

        #region helper Model
        public class UploadImogeResponse
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }
        #endregion

        #region  Api for Update News Data
        /// <summary>
        /// Created By Mayank Prajapati On 02-09-2022
        /// Api >> Update >> api/news/GetAlldatanews
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAlldatanews")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> UpdateNewsDataAll()
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var updateData = await _db.CompanyNews.Where(x =>
                                                  x.IsActive == true && x.IsDeleted == false).ToListAsync();
                foreach (var item in updateData)
                {
                    NewsEntity newsEntity = new NewsEntity();
                    newsEntity.Title = item.News;
                    newsEntity.Description = item.NewsHeading;
                    newsEntity.Image = item.Image;
                    newsEntity.CreatedOn = DateTime.Now;

                    _db.NewsEntities.Add(newsEntity);
                    await _db.SaveChangesAsync();

                    res.Message = "News Update Succesfully !";
                    res.Status = true;
                    res.Data = updateData;
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

        #region Get All Draft Nesw
        /// <summary>
        /// Created By Harshit Mitra On 18-04-2023
        /// API >> GET >> api/news/getnewsdrafted
        /// </summary>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewsdrafted")]
        public async Task<ResponseBodyModel> GetNewsDrafted(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var newsData = await _db.NewsEntities
                    .Where(x => !x.IsDeleted && x.CompanyId == claims.companyId
                        && x.NewsType == Core.Enum.NewsEnumType.Drafted && x.IsActive)
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();
                if (newsData.Count > 0)
                {
                    res.Message = "Get All News list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {

                        res.Data = new PaginationData
                        {
                            TotalData = newsData.Count,
                            Counts = (int)count,
                            List = newsData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = newsData;
                    }
                }
                else
                {
                    res.Message = "Failed To Get News !";
                    res.Status = false;
                    res.Data = newsData;
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

    }

}
