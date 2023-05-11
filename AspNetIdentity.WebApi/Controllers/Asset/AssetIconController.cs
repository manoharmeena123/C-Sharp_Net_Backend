using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.AssetsModel;
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

namespace AspNetIdentity.WebApi.Controllers.Asset
{
    [RoutePrefix("api/asseticon")]
    public class AssetIconController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        #region Get AssetIconslist Working on it

        /// <summary>
        /// Created by Suraj Bundel on 09/07/2022
        /// GET >> API >> api/asseticon/asseticonslist
        /// </summary>
        /// <returns></returns>
        [Route("asseticonslist")]
        [HttpGet]
        public async Task<ResponseBodyModel> AssetIconslist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var geticon = await db.AssetIcons.ToListAsync();
                if (geticon.Count != 0)
                {
                    res.Message = "Success";
                    res.Status = true;
                    res.Data = geticon;
                }
            }
            catch
            {
                res.Message = "failed";
                res.Status = false;
            }
            return res;
        }

        #endregion Get AssetIconslist Working on it

        #region Get Add AssetIcon Working on it

        /// <summary>
        /// Created by Suraj Bundel on 09/07/2022
        /// GET >> API >> api/asseticon/addasseticon
        /// </summary>
        /// <returns></returns>
        [Route("addasseticon")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddAssetIcons(AssetIcon assetIcon)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var addicon = await db.AssetIcons.Where(x => x.CompanyId == 0 || x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (addicon == null)
                {
                    AssetIcon asset = new AssetIcon();
                    asset.AssetIconUrl = assetIcon.AssetIconUrl;
                    db.AssetIcons.Add(asset);
                    db.SaveChanges();

                    res.Message = "Success";
                    res.Status = true;
                    res.Data = asset;
                }
            }
            catch
            {
                res.Message = "failed";
                res.Status = false;
            }
            return res;
        }

        #endregion Get Add AssetIcon Working on it

        #region Api To add asset icons

        /// <summary>
        /// Created By Suraj Bundel On 09-07-2022
        /// API >> Post >> api/asseticon/uploadasseticons
        /// </summary>
        /// use to post Document the client List
        /// <returns></returns>
        [HttpPost]
        [Route("uploadasseticons")]
        [Authorize]
        public async Task<UploadImageResponse> UploadAssetIcons()
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
                        //if (extemtionType == "image" || extemtionType=="Document"||extemtionType== "application")
                        if (extemtionType == "image" || extemtionType == "application")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/AssetIcons/" + claims.companyId), filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }

                            //string path = "uploadimage\\AssetIcons" + claims.employeeid + "\\" + dates + '.' + Fileresult + extension;
                            string path = "uploadimage\\AssetIcons\\" + claims.companyId + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successful";
                            result.Status = true;
                            result.URL = FileUrl;
                            result.Path = path;
                            result.Extension = extension;
                            result.ExtensionType = extemtionType;

                            //var addicon = await db.AssetIcons.FirstOrDefaultAsync();
                            AssetIcon asset = new AssetIcon();
                            asset.AssetIconUrl = result.Path;
                            asset.OrgId = 0;
                            asset.CreatedOn = DateTime.Now;
                            asset.IsActive = true;
                            asset.IsDeleted = false;
                            asset.CompanyId = claims.companyId;
                            asset.AssetIconId = claims.companyId;
                            asset.CreatedBy = claims.employeeId;
                            //db.Entry(asset).State = System.Data.Entity.EntityState.Modified; //data update in tabel
                            //await db.SaveChangesAsync();
                            db.AssetIcons.Add(asset);
                            db.SaveChanges();
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

        #endregion Api To add asset icons

        #region Add Compliance Category

        /// <summary>
        /// Created by Shriya Malvi On 29-07-2022
        /// API >> Post >> api/asseticon/addcompliance
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcompliance")]
        public async Task<ResponseBodyModel> AddCompliance(Compliance model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    //var basecategory = await db.AssetsBaseCategories.Where(x => x.CompanyId == claims.companyid).ToListAsync();
                    var basecategory = db.Compliances.Where(x => x.ComplianceName.Trim().ToUpper() == model.ComplianceName.Trim().ToUpper() && x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                    if (basecategory == null)
                    {
                        Compliance assets = new Compliance();
                        assets.ComplianceName = model.ComplianceName.Trim();
                        //  assets.Description = model.Description.Trim();
                        assets.CompanyId = claims.companyId;
                        assets.OrgId = claims.orgId;
                        assets.CreatedBy = claims.employeeId;
                        assets.CreatedOn = DateTime.Now;
                        assets.IsActive = true;
                        assets.IsDeleted = false;
                        db.Compliances.Add(assets);
                        await db.SaveChangesAsync();
                        res.Data = assets;
                        res.Message = "Compliance Created";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "Compliance already Exist";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Compliance Not Create";
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

        #endregion Add Compliance Category

        #region Get Compliance Category

        /// <summary>
        /// Created by Shriya Malvi On 29-07-2022
        /// API >> Get  >> api/asseticon/getcompliance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompliance")]
        public async Task<ResponseBodyModel> GetCompliance()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await db.Compliances.Where(x => x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId).ToListAsync();

                if (category.Count > 0)
                {
                    res.Data = category;
                    res.Message = "Compliance List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Compliance List Not Found";
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

        #endregion Get Compliance Category

        //#region Add Basic Icon
        ///// <summary>
        ///// Created by Suraj Bundel on 12/08/2022
        ///// AddAssetIconList => add Icon to database
        ///// API => Post => api/asseticon/addbasicicon
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("addbasicicon")]
        //public async Task<ResponseBodyModel> AddBasicIcon()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var icondata =await db.AssetIcons.ToListAsync();
        //        if (icondata.Count == 0)
        //        {
        //            var data = AddAssetIconList();
        //            res.Message = "Asset Icon list Added Successfully";
        //            res.Status = true;
        //            res.Data = data;
        //        }
        //        else
        //        {
        //            res.Message = "Asset Icon list already exist";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        #region Icon List helpermethoud

        /// <summary>
        /// Created by Suraj Bundel on 12/08/2022
        /// </summary>
        /// <returns></returns>
        public class AssetIconmodel
        {
            public int AssetIconId { get; set; }
            public int CompanyId { get; set; }
            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public string AssetIconUrl { get; set; }
        }

        #endregion Icon List helpermethoud

        #region Add Icon List

        /// <summary>
        /// Created by Suraj Bundel on  03-08-2022
        /// Method to add Icon to database
        /// </summary>
        /// <returns></returns>
        public static object AddAssetIconList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            ApplicationDbContext db = new ApplicationDbContext();
            List<AssetIconmodel> getalliconList = GetAssetIconModule();

            for (int a = 0; a < getalliconList.Count; a++)
            {
                AssetIcon obj = new AssetIcon();
                obj.AssetIconUrl = getalliconList[a].AssetIconUrl;
                obj.CompanyId = 0;
                obj.CreatedOn = DateTime.Now;
                obj.IsActive = true;
                obj.IsDeleted = false;
                obj.CreatedBy = 0;
                db.AssetIcons.Add(obj);
                db.SaveChanges();
                res.Message = "Assets Icons added";
                res.Status = true;
                res.Data = getalliconList;
            }
            return res;
        }

        #endregion Add Icon List

        #region Icon list

        /// <summary>
        /// Created by Suraj Bundel on 12/08/2022
        /// </summary>
        /// <returns></returns>
        public static List<AssetIconmodel> GetAssetIconModule()
        {
            List<AssetIconmodel> Iconlist = new List<AssetIconmodel>();

            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\1.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\2.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\3.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\4.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\5.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\6.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\7.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\8.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\9.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\10.svg",
            });
            Iconlist.Add(new AssetIconmodel
            {
                AssetIconUrl = "uploadimage\\AssetIcons\\0\\11.svg",
            });
            return Iconlist;
        }

        #endregion Icon list
    }
}