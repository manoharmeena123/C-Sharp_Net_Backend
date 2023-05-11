using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.AssetsModel;
using AspNetIdentity.WebApi.Models;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;
using static AspNetIdentity.WebApi.Model.EnumClass;
using TextInfo = System.Globalization.TextInfo;

namespace AspNetIdentity.WebApi.Controllers.Asset
{
    [Authorize]
    [RoutePrefix("api/assetsnew")]
    public class AssetsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Add Assets Base Category

        /// <summary>
        /// Created by Shriya Malvi On 09-07-2022
        /// API >> Post >> api/assetsnew/addbasecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addbasecategory")]
        public async Task<ResponseBodyModel> AddABCategory(AssetsBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var basecategory = await _db.AssetsBaseCategories.FirstOrDefaultAsync(x =>
                            x.AssetsBCategoryName.Trim().ToLower() == model.AssetsBCategoryName.Trim().ToLower() &&
                            x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                    if (basecategory == null)
                    {
                        AssetsBaseCategory assets = new AssetsBaseCategory();
                        assets.AssetsBCategoryName = model.AssetsBCategoryName.Trim();
                        assets.Description = model.Description.Trim();
                        assets.AssetsType = model.AssetsType;
                        assets.CompanyId = claims.companyId;
                        assets.OrgId = claims.orgId;
                        assets.CreatedBy = claims.employeeId;
                        assets.CreatedOn = DateTime.Now;
                        assets.IsActive = true;
                        assets.IsDeleted = false;
                        _db.AssetsBaseCategories.Add(assets);
                        await _db.SaveChangesAsync();
                        res.Data = assets;
                        res.Message = "Assets Base Category Created";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "Assets Base Category already Exist";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Assets Base Category Not Create";
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

        #endregion Add Aseets Base Category

        #region Get All Assets Base Category

        /// <summary>
        /// Created by Shriya Malvi On 09-07-2022
        /// Modified by Suraj Bundel 12-08-2022
        /// Modified by Ravi Vyas On 26-08-2022 (Add Serach)
        /// API >> Get  >> api/assetsnew/getbasecategory
        /// </summary>
        [HttpGet]
        [Route("getbasecategory")]
        public async Task<ResponseBodyModel> GetBaseCategory(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<HelperForBaseCategory> BaseCatList = new List<HelperForBaseCategory>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var icondata = _db.AssetIcons.ToList();
                if (icondata.Count == 0)
                {

                    var data = AssetIconController.AddAssetIconList();
                    res.Message = "Asset Icon list Added Successfully";
                    res.Status = true;
                }
                var category = await _db.AssetsBaseCategories.Where(x => !x.IsDeleted &&
                        x.IsActive && x.CompanyId == claims.companyId)
                        .Select(item => new HelperForBaseCategory
                        {
                            AssetsBCategoryId = item.AssetsBCategoryId,
                            AssetsBCategoryName = item.AssetsBCategoryName,
                            AssetsType = item.AssetsType,
                            Description = item.Description,
                            AssetsTypeName = item.AssetsType.ToString(),
                            Count = _db.AssetsItemMasters.Count(x => x.AssetsBaseCategoryId ==
                                item.AssetsBCategoryId && x.IsActive && !x.IsDeleted),

                        }).ToListAsync();

                if (category.Count > 0)
                {
                    res.Message = "Base Category list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = BaseCatList.Count,
                            Counts = (int)count,
                            List = category.Where(x => x.AssetsBCategoryName.ToUpper().Contains(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = BaseCatList.Count,
                            Counts = (int)count,
                            List = category.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = category;
                    }
                }
                else
                {
                    res.Message = "Base Category list Not Found";
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

        public class HelperForBaseCategory
        {
            public int AssetsBCategoryId { get; set; }

            public string AssetsBCategoryName { get; set; }
            public string Description { get; set; }
            public AssetsItemType AssetsType { get; set; }

            public string AssetsTypeName { get; set; }
            public int Count { get; set; }
        }

        public class HelperForBaseCategory1
        {
            public int AssetsBCategoryId { get; set; }

            public string AssetsBCategoryName { get; set; }
            public string Description { get; set; }
            // public int AssetsType { get; set; }

            public string AssetsTypeName { get; set; }
        }

        #endregion Get All Assets Base Category

        #region Get Assets Base Category By ID

        /// <summary>
        /// Created by Shriya Malvi On 09-07-2022
        /// API >>Get  >> api/assetsnew/getbasecategorybyid
        /// </summary>
        /// <param name="BaseCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbasecategorybyid")]
        public async Task<ResponseBodyModel> GetBaseCategoryById(int BaseCategoryId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.AssetsBaseCategories.Where(x => !x.IsDeleted && x.IsActive
                               && x.CompanyId == claims.companyId && x.AssetsBCategoryId == BaseCategoryId).ToListAsync();
                if (category.Count > 0)
                {
                    res.Data = category;
                    res.Message = "Assets Base Category List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets Base Category List Not Found";
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

        #endregion Get Assets Base Category By ID

        #region Update Assets Base Category

        /// <summary>
        /// Created by Shriya Malvi On 09-07-2022
        /// API >> Put>> api/assetsnew/updatebasecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatebasecategory")]
        public async Task<ResponseBodyModel> UpdateBaseCategory(AssetsBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetscate = _db.AssetsBaseCategories.Where(x => x.AssetsBCategoryId == model.AssetsBCategoryId && x.IsActive && !x.IsDeleted
                && x.CompanyId == claims.companyId).FirstOrDefault();
                if (assetscate != null)
                {
                    assetscate.AssetsType = model.AssetsType;
                    assetscate.AssetsBCategoryName = model.AssetsBCategoryName;
                    assetscate.Description = model.Description;
                    assetscate.UpdatedBy = claims.employeeId;
                    assetscate.UpdatedOn = DateTime.Now;
                    _db.Entry(assetscate).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Data = assetscate;
                    res.Message = "Assets Base Category Updated";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets Base Category Not Update";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Update Assets Base Category

        #region Delete Assets Base Category

        /// <summary>
        /// Created by Shriya Malvi On 09-07-2022
        /// API >> Delete >> api/assetsnew/deletebasecategory
        /// </summary>
        /// <param name="Id"></param>
        [HttpDelete]
        [Route("deletebasecategory")]
        public async Task<ResponseBodyModel> DeleteBaseCategory(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetsBasecate = _db.AssetsBaseCategories.Where(x => x.AssetsBCategoryId == Id && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                if (assetsBasecate != null)
                {
                    if (_db.AssetsItemMasters.Count(x => x.AssetsBaseCategoryId == Id && x.IsActive && !x.IsDeleted) > 0)
                    {
                        res.Message = "Item Are Exist In This Base Category \nYou Are Not Able to Delete This Base Category";
                        res.Status = false;
                    }
                    else
                    {
                        assetsBasecate.IsDeleted = true;
                        assetsBasecate.IsActive = false;
                        assetsBasecate.DeletedBy = claims.employeeId;
                        assetsBasecate.DeletedOn = DateTime.Now;

                        var assetCate = _db.AssetsCategories.Where(x => x.IsActive && !x.IsDeleted && x.AssetsBCategoryId == assetsBasecate.AssetsBCategoryId).Select(x => x.AssetsBCategoryId).ToList();
                        foreach (var item in assetCate)
                        {
                            var deleteCate = _db.AssetsCategories.Where(x => x.IsActive && !x.IsDeleted && x.AssetsBCategoryId == item).FirstOrDefault();
                            deleteCate.IsDeleted = true;
                            deleteCate.IsActive = false;
                            deleteCate.DeletedBy = claims.employeeId;
                            deleteCate.DeletedOn = DateTime.Now;
                            _db.Entry(deleteCate).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                        _db.Entry(assetsBasecate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Data = assetsBasecate;
                        res.Message = "Assets Base Category Deleted";
                        res.Status = true;
                    }
                }
                else
                {
                    res.Message = "Assets Base Category Not Delete";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Delete Assets Base Category

        #region Add Assets Category

        /// <summary>
        /// Create by Shriya Malvi On 11-07-2022
        /// API >>  Post >> api/assetsnew/addcategory
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addcategory")]
        public async Task<ResponseBodyModel> AddCategory(AssetsCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var category = await _db.AssetsCategories.FirstOrDefaultAsync(x =>
                  x.AssetsCategoryName.Trim().ToUpper() == model.AssetsCategoryName.Trim().ToUpper() && x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                    if (category == null)
                    {
                        AssetsCategory assets = new AssetsCategory();
                        assets.AssetsCategoryName = model.AssetsCategoryName.Trim();
                        assets.AssetsBCategoryId = model.AssetsBCategoryId;
                        assets.Description = model.Description;
                        if (model.IsAssetsIcon)
                        {
                            assets.IsAssetsIcon = model.IsAssetsIcon;
                            assets.ColorCode = model.ColorCode;
                            assets.AssetsCategoryIconId = model.AssetsCategoryIconId;
                        }
                        else
                        {
                            assets.IsAssetsIcon = model.IsAssetsIcon;
                            assets.AssetsCategoryIconId = 0;
                            assets.AssetIconImgUrl = model.AssetIconImgUrl;
                        }

                        assets.CompanyId = claims.companyId;
                        assets.OrgId = claims.orgId;
                        assets.CreatedBy = claims.employeeId;
                        assets.CreatedOn = DateTime.Now;
                        assets.IsActive = true;
                        assets.IsDeleted = false;
                        _db.AssetsCategories.Add(assets);
                        await _db.SaveChangesAsync();

                        res.Data = assets;
                        res.Message = "Assets Category Created";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "AssetsCategory already exist";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "AssetsCategory Not Create";
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

        #endregion Add Aseets Category

        #region Get Assets Category

        /// <summary>
        /// Create by Shriya Malvi On 11-07-2022
        /// API >> Get >> api/assetsnew/getcategory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcategory")]
        public async Task<ResponseBodyModel> GetCategory(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var category = await (from ac in _db.AssetsCategories
                                      join bc in _db.AssetsBaseCategories on ac.AssetsBCategoryId equals bc.AssetsBCategoryId
                                      where ac.CompanyId == claims.companyId && ac.IsActive && !ac.IsDeleted
                                      select new getallCategoryModel
                                      {
                                          AssetsCategoryId = ac.AssetsCategoryId,
                                          AssetsBCategoryId = ac.AssetsBCategoryId,
                                          AssetsBCategoryName = bc.AssetsBCategoryName,
                                          AssetsCategoryName = ac.AssetsCategoryName,
                                          AssetsIconId = ac.AssetsCategoryIconId,
                                          ColorCode = ac.ColorCode,
                                          Description = ac.Description,
                                          AssetIconImgUrl = ac.AssetIconImgUrl,
                                          IsAssetsIcon = ac.IsAssetsIcon,
                                          Count = _db.AssetsItemMasters.Count(x => x.AssetsCategoryId
                                                == ac.AssetsCategoryId && x.IsActive && !x.IsDeleted),
                                      }).ToListAsync();
                if (category.Count > 0)
                {
                    res.Message = " Category list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {

                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = category.Count,
                            Counts = (int)count,
                            List = category.Where(x => x.AssetsBCategoryName.ToUpper().StartsWith(text) || x.AssetsCategoryName.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {

                        res.Data = new
                        {
                            TotalData = category.Count,
                            Counts = (int)count,
                            List = category.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = category;
                    }
                }
                else
                {
                    res.Message = "Category list Not Found";
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

        #endregion Get Assets Category

        #region Get Assets Category By ID

        /// <summary>
        /// Create by Shriya Malvi On 11-07-2022
        /// API >> Get >> api/assetsnew/getcategorybyid
        /// </summary>
        /// <param name="CategoryId"></param>
        [HttpGet]
        [Route("getcategorybyid")]
        public async Task<ResponseBodyModel> GetCategoryById(int CategoryId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await _db.AssetsCategories.Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId && x.AssetsCategoryId == CategoryId).FirstOrDefaultAsync();

                if (category != null)
                {
                    CategoryModel cate = new CategoryModel
                    {
                        AssetsCategoryId = category.AssetsCategoryId,
                        AssetsBCategoryId = category.AssetsBCategoryId,
                        AssetsCategoryName = category.AssetsCategoryName,
                        Description = category.Description
                    };
                    cate.AssetsBCategoryName = _db.AssetsBaseCategories.Where(x => x.AssetsBCategoryId == cate.AssetsBCategoryId)
                            .Select(x => x.AssetsBCategoryName).FirstOrDefault();
                    cate.AssetsIconId = category.AssetsCategoryIconId;
                    cate.ColorCode = category.ColorCode;

                    res.Data = cate;
                    res.Message = "Assets Category List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets Category List Not Found";
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

        #endregion Get Assets Category By ID

        #region Update Assets Category

        /// <summary>
        /// Created by Shriya Malvi On 09-07-2022
        /// API >> Put>> api/assetsnew/updatecategory
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("updatecategory")]
        public async Task<ResponseBodyModel> UpdateCategory(AssetsCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetscate = _db.AssetsCategories.Where(x => x.AssetsCategoryId == model.AssetsCategoryId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                if (assetscate != null)
                {
                    assetscate.AssetsCategoryName = model.AssetsCategoryName;
                    assetscate.Description = model.Description;
                    assetscate.UpdatedBy = claims.employeeId;
                    assetscate.UpdatedOn = DateTime.Now;

                    assetscate.AssetsBCategoryId = model.AssetsBCategoryId;
                    if (model.IsAssetsIcon)
                    {
                        assetscate.IsAssetsIcon = model.IsAssetsIcon;
                        assetscate.ColorCode = model.ColorCode;
                        assetscate.AssetsCategoryIconId = model.AssetsCategoryIconId;
                        assetscate.AssetIconImgUrl = null;
                    }
                    else
                    {
                        assetscate.ColorCode = model.ColorCode;
                        assetscate.IsAssetsIcon = model.IsAssetsIcon;
                        assetscate.AssetsCategoryIconId = 0;
                        assetscate.AssetIconImgUrl = model.AssetIconImgUrl;
                    }

                    _db.Entry(assetscate).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Data = assetscate;
                    res.Message = "Assets  Category Updated";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets  Category Not Update";
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

        #endregion Update Assets Base Category

        #region Delete Assets  Category

        /// <summary>
        /// Create by Shriya Malvi On 11-07-2022
        /// API >> Delete >> api/assetsnew/deletecategory
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
                var assetscate = _db.AssetsCategories.Where(x => x.AssetsCategoryId == Id && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                if (assetscate != null)
                {
                    if (_db.AssetsItemMasters.Count(x => x.AssetsCategoryId == assetscate.AssetsCategoryId && x.IsActive && !x.IsDeleted) > 0)
                    {
                        res.Message = "Item Are Exist In This Category \nYou Are Not Able to Delete This Category";
                        res.Status = false;
                    }
                    else
                    {
                        assetscate.IsDeleted = true;
                        assetscate.IsActive = false;
                        assetscate.DeletedBy = claims.employeeId;
                        assetscate.DeletedOn = DateTime.Now;
                        _db.Entry(assetscate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Data = assetscate;
                        res.Message = "Assets Category Deleted";
                        res.Status = true;
                    }
                }
                else
                {
                    res.Message = "Assets Category Not Deleted";
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

        #endregion Delete Assets  Category

        #region Get All category By base category id

        /// <summary>
        /// Created By Shriya Malvi On 13-07-2022
        /// API >> Put >> api/assetsnew/getallcategoryBybaseid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcategoryBybaseid")]
        public async Task<ResponseBodyModel> GetAllCategoryByBaseid(int BaseId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var AssetsCategory = await (from s in _db.AssetsCategories
                                            where s.IsActive && !s.IsDeleted && s.CompanyId == claims.companyId
                                            && s.AssetsBCategoryId == BaseId
                                            select new
                                            {
                                                s.AssetsCategoryId,
                                                s.AssetsCategoryName,
                                            }).ToListAsync();

                if (AssetsCategory.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Category list Found";
                    res.Data = AssetsCategory;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Category list Not Found";
                    res.Data = AssetsCategory;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All category By base category

        #region Get All Warehouse List

        /// <summary>
        /// Create by Suraj Bundel On 12-07-2022
        /// API >>  GET >> api/assetsnew/getallwarehouselist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallwarehouselist")]
        public async Task<ResponseBodyModel> getallwarehouse(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var warehouse = await (from wh in _db.AssetsWarehouses
                                       where !wh.IsDeleted && wh.CompanyId == claims.companyId && wh.IsActive
                                       select new GetAllWarehouseModule
                                       {
                                           WarehouseId = wh.WarehouseId,
                                           WarehouseName = wh.WarehouseName,
                                           WarehouseAddress = wh.WarehouseAddress,
                                           WarehouseDescription = wh.WarehouseDescription,

                                           TotalItems = _db.AssetsItemMasters.Where(x => x.WareHouseId == wh.WarehouseId && x.IsActive && !x.IsDeleted).Count(),
                                       }).ToListAsync();
                if (warehouse.Count > 0)
                {
                    res.Message = "Warehouse list Found";
                    res.Status = true;

                    if (page.HasValue && count.HasValue && search != null)
                    {

                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = warehouse.Count,
                            Counts = (int)count,
                            List = warehouse.Where(x => x.WarehouseName.ToUpper().StartsWith(text) || x.WarehouseAddress.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = warehouse.Count,
                            Counts = (int)count,
                            List = warehouse.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                        };
                    }
                    else
                    {
                        res.Data = warehouse;
                    }
                }
                else
                {
                    res.Message = "Warehouse List Not Found";
                    res.Status = false;
                    res.Data = warehouse;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Warehouse List

        #region Get Assets Warehouse require parms

        /// <summary>
        /// Create by Suraj Bundel On 12-07-2022
        /// API >> Get >> api/assetsnew/getallwarehouse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallwarehouse")]
        public async Task<ResponseBodyModel> getwarehousebyid()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var category = await (from aw in _db.AssetsWarehouses
                                          // join bc in db.AssetsBaseCategories on ac.CompanyId equals bc.CompanyId
                                      where aw.CompanyId == claims.companyId && aw.IsActive && !aw.IsDeleted
                                      select new
                                      {
                                          aw.WarehouseName,
                                          aw.WarehouseAddress,
                                          aw.WarehouseDescription,
                                          aw.WarehouseId,
                                      }).ToListAsync();
                if (category != null)
                {
                    res.Data = category;
                    res.Message = "Warehouse List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Warehouse List Not Found";
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

        #endregion Get Assets Warehouse require parms

        #region Add Assets Warehouse

        /// <summary>
        /// Create by Suraj Bundel On 12-07-2022
        /// API >>  Post >> api/assetsnew/addwarehouse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addwarehouse")]
        public async Task<ResponseBodyModel> AddWarehouse(AssetsWarehouse model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var warehouse = await _db.AssetsWarehouses.FirstOrDefaultAsync(x =>
                 x.WarehouseName.Trim().ToUpper() == model.WarehouseName.Trim().ToUpper() && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                    if (warehouse == null)
                    {
                        AssetsWarehouse assets = new AssetsWarehouse();

                        assets.WarehouseName = model.WarehouseName;
                        assets.WarehouseAddress = model.WarehouseAddress;
                        assets.WarehouseDescription = model.WarehouseDescription;
                        assets.CompanyId = claims.companyId;
                        assets.OrgId = claims.orgId;
                        assets.CreatedBy = claims.employeeId;
                        assets.CreatedOn = DateTime.Now;
                        assets.IsActive = true;
                        assets.IsDeleted = false;
                        _db.AssetsWarehouses.Add(assets);
                        await _db.SaveChangesAsync();

                        res.Data = assets;
                        res.Message = "WareHouse Created ";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "WareHouse already exist";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "WareHouse Not Create";
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

        #endregion Add Assets Warehouse

        #region Update Assets Warehouse

        /// <summary>
        /// Create by Suraj Bundel On 12-07-2022
        /// API >> PUT >> api/assetsnew/updateassetwarehouse
        /// </summary>
        /// <param name="assetsItem"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateassetwarehouse")]
        public async Task<ResponseBodyModel> UpdateAssetWarehouse(AssetsWarehouse assetswarehouse)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assets = await _db.AssetsWarehouses.Where(x => x.WarehouseId == assetswarehouse.WarehouseId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (assetswarehouse != null)
                {
                    assets.WarehouseName = assetswarehouse.WarehouseName;
                    assets.WarehouseAddress = assetswarehouse.WarehouseAddress;
                    assets.WarehouseDescription = assetswarehouse.WarehouseDescription;
                    assets.CompanyId = claims.companyId;
                    assets.OrgId = claims.orgId;
                    assets.UpdatedBy = claims.employeeId;
                    assets.UpdatedOn = DateTime.Now;
                    assets.IsActive = true;
                    assets.IsDeleted = false;
                    _db.Entry(assets).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = "Update successfully";
                    res.Status = true;
                    res.Data = assets;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Update Assets Warehouse

        #region Delete Assets  Warehouse

        /// <summary>
        /// Create by Suraj Bundel On 12-07-2022
        /// API >>  PUT >> api/assetsnew/deletewarehouse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deletewarehouse")]
        public async Task<ResponseBodyModel> DeleteWarehouse(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assets = await _db.AssetsWarehouses.Where(x => x.WarehouseId == id && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (assets != null)
                {
                    if (_db.AssetsItemMasters.Count(x => x.WareHouseId == assets.WarehouseId && x.IsActive && !x.IsDeleted) > 0)
                    {
                        res.Message = "Item Are Exist In This Warehouse \nYou Are Not Able to Delete This Warehouse";
                        res.Status = false;
                    }
                    else
                    {

                        assets.DeletedBy = claims.employeeId;
                        assets.DeletedOn = DateTime.Now;
                        assets.IsActive = false;
                        assets.IsDeleted = true;
                        _db.Entry(assets).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        res.Data = assets;
                        res.Status = true;
                        res.Message = "Warehouse Deleted";
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Warehouse Not Delete";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Delete Assets  Warehouse

        #region Get All Assets List

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// Modified By shriya Malvi on 16-08-2022
        /// API >> GET >> api/assetsnew/getallasset
        /// </summary>
        [HttpGet]
        [Route("getallasset")]
        public async Task<ResponseBodyModel> GetAllAssetList(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var assets = await (from ass in _db.AssetsItemMasters
                                    join it in _db.AssetsCategories on ass.AssetsCategoryId equals it.AssetsCategoryId
                                    where !ass.IsDeleted && ass.IsActive && ass.CompanyId == claims.companyId &&
                                    ass.AssetStatus == AssetStatusConstants.Available
                                    && (ass.AssetCondition == AssetConditionConstants.Good || ass.AssetCondition == AssetConditionConstants.Fair)
                                    select new GetHelperGetAllAassets
                                    {
                                        ItemId = ass.ItemId,
                                        ItemName = ass.ItemName,
                                        AssetsBaseCategoryId = ass.AssetsBaseCategoryId,
                                        AssetsBaseCategoryName = ass.AssetsBaseCategoryName,
                                        AssetsCategoryId = ass.AssetsCategoryId,
                                        AssetsCategoryName = ass.AssetsCategoryName,
                                        WareHouseId = ass.WareHouseId,
                                        WareHouseName = ass.WareHouseName,
                                        SerialNo = ass.SerialNo,
                                        ItemCode = ass.ItemCode,
                                        AssetCondition = ass.AssetCondition.ToString(),
                                        AssetStatus = ass.AssetStatus.ToString(),
                                        Location = ass.Location,
                                        ColorCode = it.ColorCode,
                                        AssetsIconId = it.AssetsCategoryIconId,
                                        IsAssetsIcon = it.IsAssetsIcon,
                                        AssetIconImgUrl = it.AssetIconImgUrl,
                                        Assetstype = ass.AssetsType,
                                        PurchaseDate = ass.PurchaseDate,
                                        AssignToId = ass.AssignToId,
                                        AssignedToName = ass.AssignedToName,
                                        Price = ass.Price,
                                        WarentyExpDate = ass.WarentyExpDate,
                                        LicenseExpiryDate = ass.LicenseExpiryDate,
                                        LicApplicableCount = ass.LicApplicableCount,
                                        AssetsTypeName = ass.AssetsType.ToString(),
                                        AssetsDescription = ass.AssetsDescription,
                                        Comment = ass.Comment,
                                        InvoiceNo = ass.InvoiceNo,
                                        LicenseKey = ass.LicenseKey,
                                        LicenseStartDate = ass.LicenseStartDate,
                                        AssignDate = ass.AssignDate,
                                        RecoverDate = ass.RecoverDate,
                                        Compliance = ass.Compliance,
                                        Officeemail = ass.AssignToId == 0 ? "" : _db.Employee.Where(x => x.EmployeeId == ass.AssignToId).Select(x => x.OfficeEmail).FirstOrDefault(),

                                    }).ToListAsync();

                if (assets.Count > 0)
                {
                    res.Message = "Assets list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = assets.Count,
                            Counts = (int)count,
                            List = assets.Where(x => x.AssetsBaseCategoryName.ToUpper().StartsWith(text) || (x.AssetsCategoryName.ToUpper().StartsWith(text) &&
                                    x.WareHouseName.ToUpper().StartsWith(text))).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = assets.Count,
                            Counts = (int)count,
                            List = assets.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = assets;
                    }
                }
                else
                {
                    res.Message = "Assets List Not Found";
                    res.Status = false;
                    res.Data = assets;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Assets List

        #region Get Assets By ID //based on item ID

        /// <summary>
        /// Created By Suraj Bundel on 07-13-2022
        /// Modified by Shriya Malvi on 02-08-2022 && 18-08-2022
        /// API >> GET >> api/assetsnew/getassetbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getassetbyid")]
        public async Task<ResponseBodyModel> GetAssetListById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<HelperForGetAsstsById> assetlist = new List<HelperForGetAsstsById>();
                var assets = await (from im in _db.AssetsItemMasters
                                    where im.ItemId == id && !im.IsDeleted && im.IsActive && im.CompanyId == claims.companyId
                                    select new /*HelperForGetAsstsById*/
                                    {
                                        im.ItemId,
                                        im.ItemName,
                                        im.AssetsBaseCategoryId,
                                        im.AssetsCategoryId,
                                        im.AssetsBaseCategoryName,
                                        im.AssetsCategoryName,
                                        im.WareHouseId,
                                        im.WareHouseName,
                                        im.ItemCode,
                                        im.SerialNo,
                                        im.Location,
                                        im.PurchaseDate,
                                        im.AssignedToName,
                                        im.AssetCondition,
                                        im.AssetStatus,
                                        im.AssetsDescription,
                                        im.AssignDate,
                                        im.RecoverDate,
                                        im.Comment,
                                        im.InvoiceUrl,
                                        im.AvailablityStatus,
                                        im.CompanyId,
                                        im.OrgId,
                                        im.CreatedBy,
                                        im.UpdatedBy,
                                        im.CreatedOn,
                                        im.UpdatedOn,
                                        im.AssignToId,
                                        Invoicepic = im.UpImg1 + "," + im.UpImg2 + "," + im.UpImg3 + "," + im.UpImg4 + "," + im.UpImg5,
                                        im.UpImg1,
                                        im.UpImg2,
                                        im.UpImg3,
                                        im.UpImg4,
                                        im.UpImg5,
                                        im.UpImg6,
                                        im.UpImg7,
                                        im.UpImg8,
                                        im.UpImg9,
                                        im.UpImg10,
                                        im.Price,
                                        im.InvoiceNo,
                                        im.WarentyExpDate,
                                        im.IsCompliance,
                                        im.Compliance,
                                        im.AssetsType,  //added on 06-08-2022 by Shriya Malvi
                                        im.LicenseKey,
                                        im.LicenseStartDate,
                                        im.LicenseExpiryDate,
                                        im.LicApplicableCount
                                    }).ToListAsync();

                HelperForGetAsstsById assetobj = new HelperForGetAsstsById();

                foreach (var item in assets)
                {
                    var upimg = item.Invoicepic.Split(',').ToList();
                    upimg.RemoveAll(x => x == "");
                    assetobj.uploadimgs = upimg;
                    assetobj.ItemId = item.ItemId;
                    assetobj.ItemName = item.ItemName;
                    assetobj.AssetsBaseCategoryId = item.AssetsBaseCategoryId;
                    assetobj.AssetsBaseCategoryName = item.AssetsBaseCategoryName;
                    assetobj.AssetsCategoryId = item.AssetsCategoryId;
                    assetobj.AssetsCategoryName = item.AssetsCategoryName;
                    assetobj.wareHouseId = item.WareHouseId;
                    assetobj.WareHouseName = item.WareHouseName;
                    assetobj.ItemCode = item.ItemCode;
                    assetobj.SerialNo = item.SerialNo;
                    assetobj.Location = item.Location;
                    assetobj.PurchaseDate = item.PurchaseDate;
                    assetobj.AssignedToName = item.AssignedToName;
                    assetobj.AssetCondition = (int)Enum.Parse(typeof(AssetConditionConstants), item.AssetCondition.ToString());
                    assetobj.AssetStatus = item.AssetStatus.ToString();
                    assetobj.AssetsDescription = item.AssetsDescription;
                    assetobj.AssignDate = item.AssignDate;
                    assetobj.RecoverDate = item.RecoverDate;
                    assetobj.Comment = item.Comment;
                    assetobj.InvoiceUrl = item.InvoiceUrl;
                    assetobj.AvailablityStatus = item.AvailablityStatus;
                    assetobj.CompanyId = item.CompanyId;
                    assetobj.OrgId = item.OrgId;
                    assetobj.CreatedBy = item.CreatedBy;
                    assetobj.UpdatedBy = item.UpdatedBy;
                    assetobj.CreatedOn = item.CreatedOn;
                    assetobj.UpdatedOn = item.UpdatedOn;
                    assetobj.InvoiceNo = item.InvoiceNo;
                    assetobj.Price = item.Price;
                    assetobj.WarentyExpDate = item.WarentyExpDate;
                    if (item.IsCompliance)
                    {
                        assetobj.IsCompliance = "true";
                        if (item.Compliance != null)
                        {
                            var Compliance = item.Compliance.Split(',');
                            List<int> ListCompli = new List<int>();
                            List<string> ListStage = new List<string>();
                            foreach (var tech in Compliance)
                            {
                                //var i = Convert.ToInt32(tech);
                                var checkComplian = _db.Compliances.Where(x => x.ComplianceName == tech).Select(x => x.CompanyId).FirstOrDefault();
                                var techno = _db.Compliances.Where(x => x.ComplianceId == checkComplian).Select(x => x.ComplianceName).FirstOrDefault();
                                if (techno != null)
                                {
                                    ListStage.Add(techno);
                                    ListCompli.Add(checkComplian);
                                }
                            }
                            //ProjectListObj.TechnologyId = item.TechnologyId;
                            var comp = String.Join(",", ListStage);

                            assetobj.Compliance = ListStage;
                            assetobj.Complie = ListCompli;
                        }
                        else
                        {
                            assetobj.Compliance = null;
                        }
                    }
                    else
                    {
                        assetobj.IsCompliance = "false";
                    }
                    assetobj.UpImg1 = item.UpImg1;
                    assetobj.UpImg2 = item.UpImg2;
                    assetobj.UpImg3 = item.UpImg3;
                    assetobj.UpImg4 = item.UpImg4;
                    assetobj.UpImg5 = item.UpImg5;
                    assetobj.UpImg6 = item.UpImg6;
                    assetobj.UpImg7 = item.UpImg7;
                    assetobj.UpImg8 = item.UpImg8;
                    assetobj.UpImg9 = item.UpImg9;
                    assetobj.UpImg10 = item.UpImg10;
                    // assetlist.Add(assetobj);
                    assetobj.AssetsType = item.AssetsType;
                    assetobj.LicenseKey = item.LicenseKey;
                    assetobj.LicenseStartDate = item.LicenseStartDate;
                    assetobj.LicenseExpiryDate = item.LicenseExpiryDate;
                    assetobj.LicApplicableCount = item.LicApplicableCount;
                    assetobj.AssCondition = item.AssetCondition.ToString();
                    assetobj.AssType = Enum.GetName(typeof(AssetsItemType), item.AssetsType);
                    assetobj.AssignToId = item.AssignToId;
                }

                //var assets = await db.AssetsItemMasters.Where(x => x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyid && x.ItemId == id).FirstOrDefaultAsync();

                if (assets.Count > 0)
                {
                    res.Data = assetobj;
                    res.Message = "Assets List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets List Not Found";
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

        public class HelperForGetAsstsById
        {
            public object uploadimgs { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int AssetsBaseCategoryId { get; set; }
            public int AssetsCategoryId { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public int wareHouseId { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public string AssignedToName { get; set; }
            public int AssetCondition { get; set; }
            public string AssetStatus { get; set; }
            public string AssetsDescription { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? RecoverDate { get; set; }
            public string Comment { get; set; }
            public string InvoiceUrl { get; set; }
            public bool AvailablityStatus { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
            public int CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public DateTime? CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }

            public string InvoiceNo { get; set; }
            public double Price { get; set; }

            public DateTime? WarentyExpDate { get; set; }
            public List<string> Compliance { get; set; }
            public List<int> Complie { get; set; }
            public string IsCompliance { get; set; }
            public string UpImg1 { get; set; }     //add multiple  image while create item
            public string UpImg2 { get; set; }
            public string UpImg3 { get; set; }
            public string UpImg4 { get; set; }
            public string UpImg5 { get; set; }
            public string UpImg6 { get; set; }
            public string UpImg7 { get; set; }
            public string UpImg8 { get; set; }
            public string UpImg9 { get; set; }
            public string UpImg10 { get; set; }

            public string LicenseKey { get; set; } //added for digital asset type
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int? LicApplicableCount { get; set; }
            public AssetsItemType AssetsType { get; set; }

            public string AssCondition { get; set; }
            public string AssType { get; set; }
            public int AssignToId { get; set; }
        }

        #endregion Get Assets By ID //based on item ID

        #region Get Assets By ID //based on warehouse

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// modified by Shriya Malvi 22-08-2022
        /// Updated By Ravi Vyas on 23-08-2022
        /// API >> GET >> api/assetsnew/getassetbywarehouse
        /// </summary>
        [HttpGet]
        [Route("getassetbywarehouse")]
        public async Task<ResponseBodyModel> GetAssetByWarehouse(int id, int? page = null, int? count = null, int? page1 = null, int? count1 = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AssetsTypeRes response = new AssetsTypeRes();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var physicalAssetsInWarehouse = await (from im in _db.AssetsItemMasters
                                                       join ac in _db.AssetsCategories on im.AssetsCategoryId equals ac.AssetsCategoryId
                                                       where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive && im.WareHouseId == id
                                                       && im.AssetStatus == AssetStatusConstants.Assigned
                                                       && im.AssetsType == AssetsItemType.Physical && (im.AssetCondition == AssetConditionConstants.Good ||
                                                       im.AssetCondition == AssetConditionConstants.Fair)
                                                       select new GetAssetByWarehouseModel
                                                       {
                                                           ItemId = im.ItemId,
                                                           ItemName = im.ItemName,
                                                           AssetsBaseCategoryId = im.AssetsBaseCategoryId,
                                                           AssetsBaseCategoryName = im.AssetsBaseCategoryName,
                                                           AssetsCategoryId = im.AssetsCategoryId,
                                                           AssetsCategoryName = im.AssetsCategoryName,
                                                           WareHouseId = im.WareHouseId,
                                                           WareHouseName = im.WareHouseName,
                                                           ItemCode = im.ItemCode,
                                                           SerialNo = im.SerialNo,
                                                           Location = im.Location,
                                                           PurchaseDate = im.PurchaseDate,
                                                           AssignToId = im.AssignToId,
                                                           AssignedToName = im.AssignedToName,
                                                           RecoverById = im.RecoverById,
                                                           AssetCondition = im.AssetCondition.ToString(),
                                                           AssetStatus = im.AssetStatus.ToString(),
                                                           AssetsDescription = im.AssetsDescription,
                                                           AvailablityStatus = im.AvailablityStatus,
                                                           Comment = im.Comment,
                                                           totalcount = _db.AssetsItemMasters.Count(x => x.WareHouseId == im.WareHouseId && x.CompanyId == claims.companyId),
                                                           ColorCode = ac.ColorCode,
                                                           AssetsIconId = ac.AssetsCategoryIconId,
                                                           IsAssetsIcon = ac.IsAssetsIcon,
                                                           AssetIconImgUrl = ac.AssetIconImgUrl
                                                       }).ToListAsync();

                response.PhysicalAssets = physicalAssetsInWarehouse;

                var DigitalAssets = await (from im in _db.AssetsItemMasters
                                           join ac in _db.AssetsCategories on im.AssetsCategoryId equals ac.AssetsCategoryId
                                           where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive && im.WareHouseId == id
                                           && im.AssetsType == AssetsItemType.Digital && im.AssetStatus == AssetStatusConstants.Assigned
                                           select new GetAssetByWarehouseModel
                                           {
                                               ItemId = im.ItemId,
                                               ItemName = im.ItemName,
                                               AssetsBaseCategoryId = im.AssetsBaseCategoryId,
                                               AssetsBaseCategoryName = im.AssetsBaseCategoryName,
                                               AssetsCategoryId = im.AssetsCategoryId,
                                               AssetsCategoryName = im.AssetsCategoryName,
                                               WareHouseId = im.WareHouseId,
                                               WareHouseName = im.WareHouseName,
                                               ItemCode = im.ItemCode,
                                               SerialNo = im.SerialNo,
                                               Location = im.Location,
                                               PurchaseDate = im.PurchaseDate,
                                               AssignToId = im.AssignToId,
                                               AssignedToName = im.AssignedToName,
                                               RecoverById = im.RecoverById,
                                               AssetCondition = im.AssetCondition.ToString(),
                                               AssetStatus = im.AssetStatus.ToString(),
                                               AssetsDescription = im.AssetsDescription,
                                               AvailablityStatus = im.AvailablityStatus,
                                               Comment = im.Comment,
                                               totalcount = _db.AssetsItemMasters.Count(x => x.WareHouseId == im.WareHouseId && x.CompanyId == claims.companyId),
                                               ColorCode = ac.ColorCode,
                                               AssetsIconId = ac.AssetsCategoryIconId,
                                               IsAssetsIcon = ac.IsAssetsIcon,
                                               AssetIconImgUrl = ac.AssetIconImgUrl
                                           }).ToListAsync();

                response.DigitalAssets = DigitalAssets;

                if (physicalAssetsInWarehouse.Count > 0)
                {
                    res.Message = "Assets List  Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = physicalAssetsInWarehouse.Count,
                            Counts = (int)count,
                            List = physicalAssetsInWarehouse.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = response;
                    }
                }

                if (DigitalAssets.Count > 0)
                {
                    res.Message = "Assets List  Found";
                    res.Status = true;
                    if (page1.HasValue && count1.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = DigitalAssets.Count,
                            Counts = (int)count1,
                            List = DigitalAssets.Skip(((int)page1 - 1) * (int)count1).Take((int)count1).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = response;
                    }
                }

                if (DigitalAssets.Count == 0 && physicalAssetsInWarehouse.Count == 0)
                {
                    res.Message = "Assets List Not Found";
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
        public class AssetsTypeRes
        {
            public object PhysicalAssets { get; set; }
            public object DigitalAssets { get; set; }
        }
        #endregion Get Assets By ID //based on warehouse

        #region Get Assets By ID //based on warehouse

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// modified by Shriya Malvi 22-08-2022
        /// Updated By Ravi Vyas on 23-08-2022
        /// API >> GET >> api/assetsnew/getavaliableassetbywarehouse
        /// </summary>
        [HttpGet]
        [Route("getavaliableassetbywarehouse")]
        public async Task<ResponseBodyModel> GetAvaliableAssetByWarehouse(int id, int? page = null, int? count = null, int? page1 = null, int? count1 = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AssetsTypeRes response = new AssetsTypeRes();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var physicalAssetsInWarehouse = await (from im in _db.AssetsItemMasters
                                                       join ac in _db.AssetsCategories on im.AssetsCategoryId equals ac.AssetsCategoryId
                                                       join em in _db.Employee on im.RecoverById equals em.EmployeeId into q
                                                       from result in q.DefaultIfEmpty()
                                                       where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive && im.WareHouseId == id
                                                       && im.AssetsType == AssetsItemType.Physical && im.AssetStatus == AssetStatusConstants.Available
                                                       && (im.AssetCondition != AssetConditionConstants.Damage || im.AssetCondition != AssetConditionConstants.UnderRepair)
                                                       select new GetAssetByWarehouseModel
                                                       {
                                                           ItemId = im.ItemId,
                                                           ItemName = im.ItemName,
                                                           AssetsBaseCategoryId = im.AssetsBaseCategoryId,
                                                           AssetsBaseCategoryName = im.AssetsBaseCategoryName,
                                                           AssetsCategoryId = im.AssetsCategoryId,
                                                           AssetsCategoryName = im.AssetsCategoryName,
                                                           WareHouseId = im.WareHouseId,
                                                           WareHouseName = im.WareHouseName,
                                                           ItemCode = im.ItemCode,
                                                           SerialNo = im.SerialNo,
                                                           Location = im.Location,
                                                           PurchaseDate = im.PurchaseDate,
                                                           AssignToId = im.AssignToId,
                                                           AssignedToName = im.AssignedToName,
                                                           RecoverById = im.RecoverById,
                                                           RecoverByName = result.DisplayName,
                                                           AssetCondition = im.AssetCondition.ToString(),
                                                           AssetStatus = im.AssetStatus.ToString(),
                                                           AssetsDescription = im.AssetsDescription,
                                                           AvailablityStatus = im.AvailablityStatus,
                                                           Comment = im.Comment,
                                                           totalcount = _db.AssetsItemMasters
                                                                .Count(x => x.WareHouseId == im.WareHouseId && x.CompanyId == claims.companyId),
                                                           ColorCode = ac.ColorCode,
                                                           AssetsIconId = ac.AssetsCategoryIconId,
                                                           IsAssetsIcon = ac.IsAssetsIcon,
                                                           AssetIconImgUrl = ac.AssetIconImgUrl
                                                       }).ToListAsync();

                response.PhysicalAssets = physicalAssetsInWarehouse;

                var DigitalAssets = await (from im in _db.AssetsItemMasters
                                           join ac in _db.AssetsCategories on im.AssetsCategoryId equals ac.AssetsCategoryId
                                           join em in _db.Employee on im.RecoverById equals em.EmployeeId into q
                                           from result in q.DefaultIfEmpty()
                                           where !im.IsDeleted && im.CompanyId == claims.companyId && im.IsActive && im.WareHouseId == id
                                           && im.AssetsType == AssetsItemType.Digital && im.AssetStatus == AssetStatusConstants.Available
                                           && (im.AssetCondition != AssetConditionConstants.Damage || im.AssetCondition != AssetConditionConstants.UnderRepair)
                                           select new GetAssetByWarehouseModel
                                           {
                                               ItemId = im.ItemId,
                                               ItemName = im.ItemName,
                                               AssetsBaseCategoryId = im.AssetsBaseCategoryId,
                                               AssetsBaseCategoryName = im.AssetsBaseCategoryName,
                                               AssetsCategoryId = im.AssetsCategoryId,
                                               AssetsCategoryName = im.AssetsCategoryName,
                                               WareHouseId = im.WareHouseId,
                                               WareHouseName = im.WareHouseName,
                                               ItemCode = im.ItemCode,
                                               SerialNo = im.SerialNo,
                                               Location = im.Location,
                                               PurchaseDate = im.PurchaseDate,
                                               AssignToId = im.AssignToId,
                                               AssignedToName = im.AssignedToName,
                                               RecoverById = im.RecoverById,
                                               RecoverByName = result.DisplayName,
                                               AssetCondition = im.AssetCondition.ToString(),
                                               AssetStatus = im.AssetStatus.ToString(),
                                               AssetsDescription = im.AssetsDescription,
                                               AvailablityStatus = im.AvailablityStatus,
                                               Comment = im.Comment,
                                               totalcount = _db.AssetsItemMasters.Count(x => x.WareHouseId == im.WareHouseId && x.CompanyId == claims.companyId),
                                               ColorCode = ac.ColorCode,
                                               AssetsIconId = ac.AssetsCategoryIconId,
                                               IsAssetsIcon = ac.IsAssetsIcon,
                                               AssetIconImgUrl = ac.AssetIconImgUrl,

                                           }).ToListAsync();

                response.DigitalAssets = DigitalAssets;

                if (physicalAssetsInWarehouse.Count > 0)
                {
                    res.Message = "Assets List  Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = physicalAssetsInWarehouse.Count,
                            Counts = (int)count,
                            List = physicalAssetsInWarehouse.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = response;
                    }
                }

                if (DigitalAssets.Count > 0)
                {
                    res.Message = "Assets List  Found";
                    res.Status = true;
                    if (page1.HasValue && count1.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = DigitalAssets.Count,
                            Counts = (int)count1,
                            List = DigitalAssets.Skip(((int)page1 - 1) * (int)count1).Take((int)count1).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = response;
                    }
                }

                if (DigitalAssets.Count == 0 && physicalAssetsInWarehouse.Count == 0)
                {
                    res.Message = "Assets List Not Found";
                    res.Status = false;
                    res.Data = response;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion Get Assets By ID //based on warehouse

        #region Add Assets Items

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// Modified by Shriya Malvi on 02-08-2022
        /// API >> POST >> api/assetsnew/addassetitem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addassetitem")]
        public async Task<ResponseBodyModel> AddAssetItem(AddAssetsItemMasterHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                if (model != null)
                {
                    var asset = await _db.AssetsItemMasters.FirstOrDefaultAsync(x =>
                                x.SerialNo.Trim().ToUpper() == model.SerialNo.Trim().ToUpper() &&
                                x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                    if (asset == null)
                    {
                        AssetsItemMaster obj = new AssetsItemMaster();
                        obj.ItemName = model.ItemName.Trim();
                        obj.AssetsBaseCategoryId = model.AssetsBaseCategoryId;
                        obj.AssetsBaseCategoryName = _db.AssetsBaseCategories.Where(x => x.AssetsBCategoryId == model.AssetsBaseCategoryId).Select(x => x.AssetsBCategoryName).FirstOrDefault();
                        obj.AssetsCategoryId = model.AssetsCategoryId;
                        obj.AssetsCategoryName = _db.AssetsCategories.Where(x => x.AssetsCategoryId == model.AssetsCategoryId).Select(x => x.AssetsCategoryName).FirstOrDefault();
                        obj.WareHouseId = model.WareHouseId;
                        obj.WareHouseName = _db.AssetsWarehouses.Where(x => x.WarehouseId == model.WareHouseId).Select(x => x.WarehouseName).FirstOrDefault();
                        obj.ItemCode = model.ItemCode;
                        obj.SerialNo = model.SerialNo;
                        obj.Location = model.Location.Trim();
                        obj.InvoiceUrl = model.InvoiceUrl;
                        obj.PurchaseDate = model.PurchaseDate;
                        obj.RecoverById = 0;
                        obj.AssetCondition = model.AssetCondition;
                        obj.AssetsDescription = model.AssetsDescription;
                        obj.ReasonNotAvailable = model.ReasonNotAvailable;
                        obj.AvailablityStatus = true;
                        obj.CompanyId = claims.companyId;
                        obj.OrgId = claims.orgId;
                        obj.CreatedBy = claims.employeeId;
                        obj.CreatedOn = DateTime.Now;
                        obj.IsActive = true;
                        obj.IsDeleted = false;
                        obj.IsRefurbish = false;
                        obj.RefurbishCount = 0;
                        // modify on 29-07-2022
                        obj.Price = model.Price;
                        obj.InvoiceNo = model.InvoiceNo;
                        if (model.IsCompliance)
                        {
                            obj.Compliance = model.Compliance.Count > 0 ?
                                    string.Join(",", model.Compliance) : null;
                            obj.IsCompliance = true;
                        }
                        else
                        {
                            obj.IsCompliance = false;
                        }
                        obj.UpImg1 = model.UpImg1;
                        obj.UpImg2 = model.UpImg2;
                        obj.UpImg3 = model.UpImg3;
                        obj.UpImg4 = model.UpImg4;
                        obj.UpImg5 = model.UpImg5;
                        obj.UpImg6 = model.UpImg6;
                        obj.UpImg7 = model.UpImg7;
                        obj.UpImg8 = model.UpImg8;
                        obj.UpImg9 = model.UpImg9;
                        obj.UpImg10 = model.UpImg10;
                        obj.LicApplicableCount = 0;
                        obj.AssetsType = model.AssetsType;
                        if (model.AssignToId != 0)
                        {
                            obj.AssignToId = model.AssignToId;
                            obj.AssignedToName = _db.Employee.Where(x => x.EmployeeId == model.AssignToId).Select(x => x.DisplayName).FirstOrDefault();
                            obj.AssetStatus = AssetStatusConstants.Assigned;
                            obj.Recovered = false;
                            obj.Assigned = true;
                            obj.AssignDate = DateTime.UtcNow;
                        }
                        else
                        {
                            obj.AssignToId = 0;
                            obj.AssetStatus = AssetStatusConstants.Available;
                            obj.Recovered = false;
                            obj.Assigned = false;
                        }
                        if (AssetsItemType.Physical == model.AssetsType)
                        {
                            obj.LicApplicableCount = 0;
                            obj.AssetsType = model.AssetsType;
                            obj.WarentyExpDate = model.WarentyExpDate;
                        }
                        else
                        {
                            obj.LicenseKey = model.LicenseKey;
                            obj.LicenseStartDate = model.LicenseStartDate;
                            obj.LicenseExpiryDate = model.LicenseExpiryDate;
                            obj.LicApplicableCount = model.LicApplicableCount;
                            obj.AssetsType = model.AssetsType;
                        }
                        _db.AssetsItemMasters.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = "Asset Added Successfully";
                        res.Status = true;
                        res.Data = obj;

                        var history = await _db.AssetsItemMasters.Where(x => x.ItemId == obj.ItemId).FirstOrDefaultAsync();
                        AssetsHistory historyobj = new AssetsHistory();


                        historyobj.ItemId = history.ItemId;
                        historyobj.ItemName = history.ItemName;
                        historyobj.AssetsBaseCategoryId = history.AssetsBaseCategoryId;
                        historyobj.AssetsCategoryId = history.AssetsCategoryId;
                        historyobj.WareHouseId = history.WareHouseId;
                        historyobj.ItemCode = history.ItemCode;
                        historyobj.SerialNo = history.SerialNo;
                        historyobj.Location = history.Location;
                        historyobj.InvoiceUrl = history.InvoiceUrl;
                        historyobj.PurchaseDate = history.PurchaseDate;
                        historyobj.AssignToId = history.AssignToId;
                        historyobj.AssignById = history.AssignById;
                        historyobj.RecoverById = history.RecoverById;
                        //  historyobj.AssignById = history.AssignById;
                        historyobj.AssetCondition = history.AssetCondition;
                        historyobj.AssetStatus = history.AssetStatus;
                        historyobj.AssetsDescription = history.AssetsDescription;
                        historyobj.ReasonNotAvailable = history.ReasonNotAvailable;
                        historyobj.AvailablityStatus = history.AvailablityStatus;
                        historyobj.Recovered = history.Recovered;
                        historyobj.Assigned = history.Assigned;
                        historyobj.AssignDate = history.AssignDate;
                        historyobj.RecoverDate = history.RecoverDate;
                        historyobj.Comment = history.Comment;
                        historyobj.IsRefurbish = history.IsRefurbish;
                        historyobj.RefurbishCount = history.RefurbishCount;
                        historyobj.InvoiceNo = history.InvoiceNo;
                        historyobj.Price = history.Price;
                        historyobj.WarentyExpDate = history.WarentyExpDate;
                        historyobj.Compliance = history.Compliance;
                        historyobj.IsCompliance = history.IsCompliance;
                        historyobj.LicenseKey = history.LicenseKey;
                        historyobj.LicenseStartDate = history.LicenseStartDate;
                        historyobj.LicenseExpiryDate = history.LicenseExpiryDate;
                        historyobj.LicApplicableCount = history.LicApplicableCount;
                        historyobj.AssetsType = history.AssetsType;
                        historyobj.UpImg1 = history.UpImg1;
                        historyobj.UpImg2 = history.UpImg2;
                        historyobj.UpImg3 = history.UpImg3;
                        historyobj.UpImg4 = history.UpImg4;
                        historyobj.UpImg5 = history.UpImg5;
                        historyobj.UpImg6 = history.UpImg6;
                        historyobj.UpImg7 = history.UpImg7;
                        historyobj.UpImg8 = history.UpImg8;
                        historyobj.UpImg9 = history.UpImg9;
                        historyobj.UpImg10 = history.UpImg10;
                        historyobj.IsActive = history.IsActive;
                        historyobj.IsDeleted = history.IsDeleted;
                        historyobj.CompanyId = history.CompanyId;
                        historyobj.OrgId = history.OrgId;
                        historyobj.CreatedBy = history.CreatedBy;
                        historyobj.UpdatedBy = history.UpdatedBy;
                        historyobj.DeletedBy = history.DeletedBy;
                        historyobj.CreatedOn = history.CreatedOn;
                        historyobj.UpdatedOn = history.UpdatedOn;
                        historyobj.DeletedOn = history.DeletedOn;
                        historyobj.AssignToName = history.AssignedToName;

                        _db.AssetsHistories.Add(historyobj);
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        res.Message = "Asset Unable to Update Serial No. Already Exist";
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

        #endregion Add Assets Items

        #region Update Assets Items

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// Modified by Shriya Malvi on 02-08-2022
        /// API >> PUT >> api/assetsnew/updateassetitem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateassetitem")]
        public async Task<ResponseBodyModel> UpdateAssetItem(AddAssetsItemMasterHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assets = await _db.AssetsItemMasters.Where(x => x.ItemId == model.ItemId && x.IsActive
                    && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (model != null)
                {
                    assets.ItemName = model.ItemName;
                    assets.Location = model.Location;
                    assets.AssetCondition = model.AssetCondition;
                    assets.AssetStatus = assets.AssetCondition == AssetConditionConstants.Damage ? AssetStatusConstants.Damage :
                            assets.AssetCondition == AssetConditionConstants.UnderRepair ? AssetStatusConstants.UnderRepair :
                             assets.AssetCondition == AssetConditionConstants.Good ? AssetStatusConstants.Available : assets.AssetCondition == AssetConditionConstants.Fair ? AssetStatusConstants.Available :
                            assets.AssetStatus;
                    assets.AssetsDescription = model.AssetsDescription;
                    assets.AssetsBaseCategoryId = model.AssetsBaseCategoryId;
                    assets.AssetsBaseCategoryName = _db.AssetsBaseCategories.Where(x => x.AssetsBCategoryId == model.AssetsBaseCategoryId).Select(x => x.AssetsBCategoryName).FirstOrDefault();
                    assets.AssetsCategoryId = model.AssetsCategoryId;
                    assets.AssetsCategoryName = _db.AssetsCategories.Where(x => x.AssetsCategoryId == model.AssetsCategoryId).Select(x => x.AssetsCategoryName).FirstOrDefault();
                    assets.WareHouseId = model.WareHouseId;
                    assets.WareHouseName = _db.AssetsWarehouses.Where(x => x.WarehouseId == model.WareHouseId).Select(x => x.WarehouseName).FirstOrDefault();
                    assets.ItemCode = model.ItemCode;
                    assets.SerialNo = model.SerialNo;
                    assets.Location = model.Location;
                    assets.InvoiceUrl = model.InvoiceUrl;
                    assets.PurchaseDate = model.PurchaseDate;
                    assets.AssetsDescription = model.AssetsDescription;
                    assets.AssignToId = (model.AssetCondition == AssetConditionConstants.UnderRepair || model.AssetCondition == AssetConditionConstants.Damage) ? 0 : model.AssignToId;
                    assets.UpImg1 = String.IsNullOrEmpty(model.UpImg1) ? assets.UpImg1 : model.UpImg1;
                    assets.UpImg2 = String.IsNullOrEmpty(model.UpImg2) ? assets.UpImg2 : model.UpImg2;
                    assets.UpImg3 = String.IsNullOrEmpty(model.UpImg3) ? assets.UpImg3 : model.UpImg3;
                    assets.UpImg4 = String.IsNullOrEmpty(model.UpImg4) ? assets.UpImg4 : model.UpImg4;
                    assets.UpImg5 = String.IsNullOrEmpty(model.UpImg5) ? assets.UpImg5 : model.UpImg5;
                    assets.UpImg6 = String.IsNullOrEmpty(model.UpImg6) ? assets.UpImg6 : model.UpImg6;
                    assets.UpImg7 = String.IsNullOrEmpty(model.UpImg7) ? assets.UpImg7 : model.UpImg7;
                    assets.UpImg8 = String.IsNullOrEmpty(model.UpImg8) ? assets.UpImg8 : model.UpImg8;
                    assets.UpImg9 = String.IsNullOrEmpty(model.UpImg9) ? assets.UpImg9 : model.UpImg9;
                    assets.UpImg10 = String.IsNullOrEmpty(model.UpImg10) ? assets.UpImg10 : model.UpImg10;
                    assets.UpdatedBy = claims.employeeId;
                    assets.UpdatedOn = DateTime.Now;
                    // assets.AssignedToName = _db.Employee.Where(x => x.EmployeeId == assets.AssignToId).FirstOrDefault();
                    //modify on 29-07-2022
                    assets.Price = model.Price;
                    assets.WarentyExpDate = model.WarentyExpDate;
                    assets.InvoiceNo = model.InvoiceNo;
                    assets.AssignedToName = _db.Employee.Where(x => x.EmployeeId == assets.AssignToId).Select(x => x.DisplayName).FirstOrDefault();
                    assets.AssetStatus = assets.AssignToId != 0 ? AssetStatusConstants.Assigned : assets.AssetStatus;
                    if (model.IsCompliance)
                    {
                        if (model.Compliance != null)
                        {
                            assets.Compliance = model.Compliance.Count > 0 ?
                                string.Join(",", model.Compliance)
                                : null;
                            assets.IsCompliance = true;
                        }
                        else
                        {
                            assets.Compliance = null;
                        }

                    }
                    else
                    {
                        assets.IsCompliance = false;
                    }
                    if (AssetsItemType.Physical == model.AssetsType)
                    {
                        assets.LicApplicableCount = 0;
                        assets.AssetsType = model.AssetsType;
                        assets.WarentyExpDate = model.WarentyExpDate;
                    }
                    else
                    {
                        assets.LicenseKey = model.LicenseKey;
                        assets.LicenseStartDate = model.LicenseStartDate;
                        assets.LicenseExpiryDate = model.LicenseExpiryDate;
                        assets.LicApplicableCount = model.LicApplicableCount;
                        assets.AssetsType = model.AssetsType;
                    }
                    _db.Entry(assets).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update successfully";
                    res.Status = true;
                    res.Data = assets;


                    //var updatehistory = _db.AssetsItemMasters.Where(x => x.ItemId == model.ItemId);
                    var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == model.ItemId);

                    AssetsHistory history = new AssetsHistory();
                    {
                        history.ItemId = updatehistory.ItemId;
                        history.ItemName = updatehistory.ItemName;
                        history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                        history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                        history.WareHouseId = updatehistory.WareHouseId;
                        history.ItemCode = updatehistory.ItemCode;
                        history.SerialNo = updatehistory.SerialNo;
                        history.Location = updatehistory.Location;
                        history.InvoiceUrl = updatehistory.InvoiceUrl;
                        history.PurchaseDate = updatehistory.PurchaseDate;
                        history.AssignToId = updatehistory.AssignToId;
                        history.AssignToName = updatehistory.AssignedToName;
                        history.AssignById = updatehistory.AssignById;
                        history.RecoverById = updatehistory.RecoverById;
                        history.AssetCondition = updatehistory.AssetCondition;
                        history.AssetStatus = updatehistory.AssetStatus;
                        history.AssetsDescription = updatehistory.AssetsDescription;
                        history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                        history.AvailablityStatus = updatehistory.AvailablityStatus;
                        history.Recovered = updatehistory.Recovered;
                        history.Assigned = updatehistory.Assigned;
                        history.AssignDate = updatehistory.AssignDate;
                        history.RecoverDate = updatehistory.RecoverDate;
                        history.Comment = updatehistory.Comment;
                        history.IsRefurbish = updatehistory.IsRefurbish;
                        history.RefurbishCount = updatehistory.RefurbishCount;
                        history.InvoiceNo = updatehistory.InvoiceNo;
                        history.Price = updatehistory.Price;
                        history.WarentyExpDate = updatehistory.WarentyExpDate;
                        history.Compliance = updatehistory.Compliance;
                        history.IsCompliance = updatehistory.IsCompliance;
                        history.LicenseKey = updatehistory.LicenseKey;
                        history.LicenseStartDate = updatehistory.LicenseStartDate;
                        history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                        history.LicApplicableCount = updatehistory.LicApplicableCount;
                        history.AssetsType = updatehistory.AssetsType;
                        history.UpImg1 = updatehistory.UpImg1;
                        history.UpImg2 = updatehistory.UpImg2;
                        history.UpImg3 = updatehistory.UpImg3;
                        history.UpImg4 = updatehistory.UpImg4;
                        history.UpImg5 = updatehistory.UpImg5;
                        history.UpImg6 = updatehistory.UpImg6;
                        history.UpImg7 = updatehistory.UpImg7;
                        history.UpImg8 = updatehistory.UpImg8;
                        history.UpImg9 = updatehistory.UpImg9;
                        history.UpImg10 = updatehistory.UpImg10;
                        history.AssignImage1 = updatehistory.AssignImage1;
                        history.AssignImage2 = updatehistory.AssignImage2;
                        history.AssignImage3 = updatehistory.AssignImage3;
                        history.AssignImage4 = updatehistory.AssignImage4;
                        history.AssignImage5 = updatehistory.AssignImage5;
                        history.RecoverImage1 = updatehistory.RecoverImage1;
                        history.RecoverImage2 = updatehistory.RecoverImage2;
                        history.RecoverImage3 = updatehistory.RecoverImage3;
                        history.RecoverImage4 = updatehistory.RecoverImage4;
                        history.RecoverImage5 = updatehistory.RecoverImage5;
                        history.IsActive = updatehistory.IsActive;
                        history.IsDeleted = updatehistory.IsDeleted;
                        history.CompanyId = updatehistory.CompanyId;
                        history.OrgId = updatehistory.OrgId;
                        history.CreatedBy = updatehistory.CreatedBy;
                        history.UpdatedBy = updatehistory.UpdatedBy;
                        history.DeletedBy = updatehistory.DeletedBy;
                        history.CreatedOn = updatehistory.CreatedOn;
                        history.UpdatedOn = updatehistory.UpdatedOn;
                        history.DeletedOn = updatehistory.DeletedOn;
                        _db.AssetsHistories.Add(history);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    res.Message = "Asset Not Found";
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
        #endregion Update Assets Items

        #region Delete Assets Base Category

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// API >> Delete >> api/assetsnew/deleteasset
        /// </summary>
        /// <param name="assetsItem"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteasset")]
        public async Task<ResponseBodyModel> deleteasset(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assets = await _db.AssetsItemMasters.Where(x => x.ItemId == id && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (assets != null)
                {
                    assets.CompanyId = claims.employeeId;
                    assets.OrgId = claims.orgId;
                    assets.DeletedBy = claims.employeeId;
                    assets.DeletedOn = DateTime.Now;
                    assets.IsActive = false;
                    assets.IsDeleted = true;
                    _db.Entry(assets).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Delete Assets Base Category

        #region Add asset invoice

        /// <summary>
        /// Created By Suraj Bundel On 09-07-2022
        /// API >> Post >> api/assetsnew/uploadassetinvoice
        /// </summary>
        /// use to post Document the client List
        /// <returns></returns>
        [HttpPost]
        [Route("uploadassetinvoice")]
        [Authorize]
        public async Task<UploadImageResponse> UploadAssetInvoice()
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
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Assetitem/AssetInvoice/" + claims.companyId), dates + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }

                            string path = "uploadimage\\Assetitem\\AssetInvoice\\" + claims.companyId + "\\" + dates + Fileresult + extension;

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

        #endregion Add asset invoice

        #region Assign Assets

        /// <summary>
        /// Created By shriya Malvi On 12-07-2022
        /// API >>  Put  >>  api/assetsnew/assignasset
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("assignasset")]
        public async Task<ResponseBodyModel> AssignAssets(HelperForAssign model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetsAssign = _db.AssetsItemMasters.Where(x => x.ItemId == model.ItemId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                if (assetsAssign != null)
                {
                    var employee = await _db.Employee.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                            x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee && x.EmployeeId == model.AssignToId);
                    if (employee != null)
                    {
                        if (model.ConditionId == AssetConditionConstants.Damage)
                        {
                            assetsAssign.AssignById = claims.employeeId;
                            assetsAssign.AssignToId = 0;
                            //var condition = Convert.ToInt32(Addasset.AssetCondition).ToString();
                            //model.AssetCondition = Enum.GetName(typeof(AssetConditionEnum), condtion).ToString();
                            //  model.AssetCondition = (AssetConditionEnum)Enum.Parse(typeof(AssetConditionEnum), condition);
                            assetsAssign.AssetCondition = AssetConditionConstants.Damage;
                            // assetsAssign.AssignedToName = _db.Employee.Where(x => x.EmployeeId == model.AssignToId).Select(x => x.DisplayName).FirstOrDefault();
                            assetsAssign.Assigned = false;
                            assetsAssign.Recovered = false;
                            assetsAssign.AssignDate = DateTime.Now;
                            assetsAssign.Comment = model.Comment;
                            assetsAssign.AssetStatus = Model.EnumClass.AssetStatusConstants.Damage;
                            assetsAssign.AssignImage1 = model.AssignImage1;
                            assetsAssign.AssignImage2 = model.AssignImage2;
                            assetsAssign.AssignImage3 = model.AssignImage3;
                            assetsAssign.AssignImage4 = model.AssignImage4;
                            assetsAssign.AssignImage5 = model.AssignImage5;
                            //   assetsAssign.AssetStatus = AssetStatusEnum.Assigned;

                            _db.Entry(assetsAssign).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }

                        else if (model.ConditionId == AssetConditionConstants.UnderRepair)
                        {
                            assetsAssign.AssignById = claims.employeeId;
                            assetsAssign.AssignToId = 0;
                            //var condition = Convert.ToInt32(Addasset.AssetCondition).ToString();
                            //model.AssetCondition = Enum.GetName(typeof(AssetConditionEnum), condtion).ToString();
                            //  model.AssetCondition = (AssetConditionEnum)Enum.Parse(typeof(AssetConditionEnum), condition);
                            assetsAssign.AssetCondition = AssetConditionConstants.UnderRepair;
                            // assetsAssign.AssignedToName = _db.Employee.Where(x => x.EmployeeId == model.AssignToId).Select(x => x.DisplayName).FirstOrDefault();
                            assetsAssign.Assigned = true;
                            assetsAssign.Recovered = false;
                            assetsAssign.AssignDate = DateTime.Now;
                            assetsAssign.Comment = model.Comment;
                            assetsAssign.AssetStatus = Model.EnumClass.AssetStatusConstants.UnderRepair;
                            assetsAssign.AssignImage1 = model.AssignImage1;
                            assetsAssign.AssignImage2 = model.AssignImage2;
                            assetsAssign.AssignImage3 = model.AssignImage3;
                            assetsAssign.AssignImage4 = model.AssignImage4;
                            assetsAssign.AssignImage5 = model.AssignImage5;
                            //   assetsAssign.AssetStatus = AssetStatusEnum.Assigned;

                            _db.Entry(assetsAssign).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            assetsAssign.AssignById = claims.employeeId;
                            assetsAssign.AssignToId = model.AssignToId;
                            //var condition = Convert.ToInt32(Addasset.AssetCondition).ToString();
                            //model.AssetCondition = Enum.GetName(typeof(AssetConditionEnum), condtion).ToString();
                            //  model.AssetCondition = (AssetConditionEnum)Enum.Parse(typeof(AssetConditionEnum), condition);
                            assetsAssign.AssetCondition = model.ConditionId;
                            assetsAssign.AssignedToName = _db.Employee.Where(x => x.EmployeeId == model.AssignToId).Select(x => x.DisplayName).FirstOrDefault();
                            assetsAssign.Assigned = true;
                            assetsAssign.Recovered = false;
                            assetsAssign.AssignDate = DateTime.Now;
                            assetsAssign.Comment = model.Comment;
                            assetsAssign.AssetStatus = Model.EnumClass.AssetStatusConstants.Assigned;
                            //   assetsAssign.AssetStatus = AssetStatusEnum.Assigned;
                            assetsAssign.AssignImage1 = model.AssignImage1;
                            assetsAssign.AssignImage2 = model.AssignImage2;
                            assetsAssign.AssignImage3 = model.AssignImage3;
                            assetsAssign.AssignImage4 = model.AssignImage4;
                            assetsAssign.AssignImage5 = model.AssignImage5;
                            _db.Entry(assetsAssign).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }


                        //AsstesAssignHistory history1 = new AsstesAssignHistory();
                        //history1.AssignToId = model.AssignToId;
                        //history1.ItemId = model.ItemId;
                        //history1.AssetCondition = assetsAssign.AssetCondition.ToString();
                        //history1.RecoverBy = 0;
                        //history1.AssignImage1 = model.AssignImage1;
                        //history1.AssignImage2 = model.AssignImage2;
                        //history1.AssignImage3 = model.AssignImage3;
                        //history1.AssignImage4 = model.AssignImage4;
                        //history1.AssignImage5 = model.AssignImage5;
                        //history1.CompanyId = claims.companyid;
                        //history1.OrgId = claims.orgid;
                        //history1.CreatedBy = claims.employeeid;
                        //history1.CreatedOn = DateTime.Now;
                        //history1.IsActive = true;
                        //history1.IsDeleted = false;


                        res.Data = assetsAssign;
                        //res.Message = "Asset Assign Succesfully";
                        res.Message = assetsAssign.AssetStatus == AssetStatusConstants.UnderRepair ? "Asset On Repair" : "Asset Assign Succesfully";
                        res.Status = true;
                        var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == model.ItemId);

                        AssetsHistory history = new AssetsHistory();
                        {
                            history.RecoverImage1 = updatehistory.RecoverImage1;
                            history.RecoverImage2 = updatehistory.RecoverImage2;
                            history.RecoverImage3 = updatehistory.RecoverImage3;
                            history.RecoverImage4 = updatehistory.RecoverImage4;
                            history.RecoverImage5 = updatehistory.RecoverImage5;
                            history.AssignImage1 = updatehistory.AssignImage1;
                            history.AssignImage2 = updatehistory.AssignImage2;
                            history.AssignImage3 = updatehistory.AssignImage3;
                            history.AssignImage4 = updatehistory.AssignImage4;
                            history.AssignImage5 = updatehistory.AssignImage5;
                            history.AssignToName = updatehistory.AssignedToName;
                            history.ItemId = updatehistory.ItemId;
                            history.ItemName = updatehistory.ItemName;
                            history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                            history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                            history.WareHouseId = updatehistory.WareHouseId;
                            history.ItemCode = updatehistory.ItemCode;
                            history.SerialNo = updatehistory.SerialNo;
                            history.Location = updatehistory.Location;
                            history.InvoiceUrl = updatehistory.InvoiceUrl;
                            history.PurchaseDate = updatehistory.PurchaseDate;
                            history.AssignById = updatehistory.AssignById;
                            history.AssignToId = updatehistory.AssignToId;
                            history.RecoverById = updatehistory.RecoverById;
                            history.AssetCondition = updatehistory.AssetCondition;
                            history.AssetStatus = updatehistory.AssetStatus;
                            history.AssetsDescription = updatehistory.AssetsDescription;
                            history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                            history.AvailablityStatus = updatehistory.AvailablityStatus;
                            history.Recovered = updatehistory.Recovered;
                            history.Assigned = updatehistory.Assigned;
                            history.AssignDate = updatehistory.AssignDate;
                            history.RecoverDate = updatehistory.RecoverDate;
                            history.Comment = updatehistory.Comment;
                            history.IsRefurbish = updatehistory.IsRefurbish;
                            history.RefurbishCount = updatehistory.RefurbishCount;
                            history.InvoiceNo = updatehistory.InvoiceNo;
                            history.Price = updatehistory.Price;
                            history.WarentyExpDate = updatehistory.WarentyExpDate;
                            history.Compliance = updatehistory.Compliance;
                            history.IsCompliance = updatehistory.IsCompliance;
                            history.LicenseKey = updatehistory.LicenseKey;
                            history.LicenseStartDate = updatehistory.LicenseStartDate;
                            history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                            history.LicApplicableCount = updatehistory.LicApplicableCount;
                            history.AssetsType = updatehistory.AssetsType;
                            history.UpImg1 = updatehistory.UpImg1;
                            history.UpImg2 = updatehistory.UpImg2;
                            history.UpImg3 = updatehistory.UpImg3;
                            history.UpImg4 = updatehistory.UpImg4;
                            history.UpImg5 = updatehistory.UpImg5;
                            history.UpImg6 = updatehistory.UpImg6;
                            history.UpImg7 = updatehistory.UpImg7;
                            history.UpImg8 = updatehistory.UpImg8;
                            history.UpImg9 = updatehistory.UpImg9;
                            history.UpImg10 = updatehistory.UpImg10;
                            history.IsActive = updatehistory.IsActive;
                            history.IsDeleted = updatehistory.IsDeleted;
                            history.CompanyId = updatehistory.CompanyId;
                            history.OrgId = updatehistory.OrgId;
                            history.CreatedBy = updatehistory.CreatedBy;
                            history.UpdatedBy = updatehistory.UpdatedBy;
                            history.DeletedBy = updatehistory.DeletedBy;
                            history.CreatedOn = updatehistory.CreatedOn;
                            history.UpdatedOn = updatehistory.UpdatedOn;
                            history.DeletedOn = updatehistory.DeletedOn;
                            _db.AssetsHistories.Add(history);
                            await _db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        res.Message = "Employee Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Asset Not Found";
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

        #endregion Assign Assets

        #region GetAlltheAssignAssets

        /// <summary>
        /// Created By shriya Malvi On 12-07-2022
        /// Modify By Ravi Vyas on 26-08-2022 ( Add Total Asset Amount And Diffrentiate Physical And Digital Asset List )
        /// API >>  Get  >>  api/assetsnew/getalltheassignassets
        /// </summary>
        [HttpGet]
        [Route("getalltheassignassets")]
        public async Task<ResponseBodyModel> GetAllTheAssignAssets(int? page = null, int? count = null, string search = null)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            ResponseBodyModel res = new ResponseBodyModel();
            List<Assetlist> list = new List<Assetlist>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetdata = await (from ass in _db.AssetsItemMasters
                                       join it in _db.AssetsCategories on ass.AssetsCategoryId equals it.AssetsCategoryId
                                       join em in _db.Employee on ass.AssignToId equals em.EmployeeId
                                       join dp in _db.Department on em.DepartmentId equals dp.DepartmentId
                                       //   where em.CompanyId == claims.companyid && ass.Assigned && ass.AssetStatus == AssetStatusEnum.Assigned && ass.AssetCondition != AssetConditionEnum.Damage
                                       where em.CompanyId == claims.companyId /*&& ass.Assigned*/ && ass.AssetStatus == AssetStatusConstants.Assigned && (ass.AssetCondition != AssetConditionConstants.Damage || ass.AssetCondition != AssetConditionConstants.UnderRepair) && ass.IsActive && !ass.IsDeleted
                                       select new
                                       {
                                           ass.ItemId,
                                           ass.ItemName,
                                           ass.SerialNo,
                                           ass.ItemCode,
                                           em.DisplayName,
                                           ass.AssignToId,
                                           ass.AssignedToName,
                                           dp.DepartmentName,
                                           ass.AssetCondition,
                                           ass.WareHouseId,
                                           ass.AssetsCategoryId,
                                           ass.AssignDate,
                                           assign = ass.AssignDate ?? ass.CreatedOn,
                                           it.ColorCode,
                                           it.AssetsCategoryIconId,
                                           ass.Price,
                                           it.IsAssetsIcon,
                                           it.AssetIconImgUrl,
                                           ass.AssetsType
                                       }).ToListAsync();

                var employeeIds = assetdata.ConvertAll(x => new DistingByAssest
                {
                    AssignedToId = x.AssignToId,
                    DisplayName = x.DisplayName,
                    DepartmentName = x.DepartmentName,
                    ItemId = x.ItemId,
                    Condition = x.AssetCondition.ToString(),
                    WarehouseId = x.WareHouseId,
                    IconId = x.AssetsCategoryIconId,
                    ColorCode = x.ColorCode
                });
                foreach (int item in employeeIds.Select(x => x.AssignedToId).Distinct().ToList())
                {
                    var data = employeeIds.Find(x => x.AssignedToId == item);
                    Assetlist obj = new Assetlist
                    {
                        EmployeeId = data.AssignedToId,
                        EmployeeName = data.DisplayName,
                        Department = data.DepartmentName,
                        TotalAmount = assetdata.Where(x => x.AssignToId == data.AssignedToId).Select(x => x.Price).Sum(),

                        PhysicalAssesdataList = assetdata.Where(x => x.AssignToId == data.AssignedToId && x.AssetsType == AssetsItemType.Physical)
                                .Select(x => new Assetdatamodel
                                {
                                    EmployeeName = x.DisplayName,
                                    ItemId = x.ItemId,
                                    ItemName = x.ItemName,
                                    Model = x.ItemCode,
                                    Serialnumber = x.SerialNo,
                                    Condition = x.AssetCondition.ToString(),
                                    Price = x.Price,
                                    WarehouseId = x.WareHouseId,
                                    AssignId = x.AssignToId,
                                    IconId = x.AssetsCategoryIconId,
                                    ColorCode = x.ColorCode,
                                    AssetIconImgUrl = x.AssetIconImgUrl,
                                    AssetType = x.AssetsType,
                                    IsAssetsIcon = x.IsAssetsIcon
                                }).ToList(),
                        TotalPhysicalAssetsAmount = assetdata.Where(x => x.AssignToId == data.AssignedToId && x.AssetsType == AssetsItemType.Physical)
                        .Select(x => x.Price).Sum(),

                        DigitalAssesdataList = assetdata.Where(x => x.AssignToId == data.AssignedToId && x.AssetsType == AssetsItemType.Digital)
                                .Select(x => new Assetdatamodel
                                {
                                    EmployeeName = x.DisplayName,
                                    ItemId = x.ItemId,
                                    ItemName = x.ItemName,
                                    Model = x.ItemCode,
                                    Serialnumber = x.SerialNo,
                                    Condition = x.AssetCondition.ToString(),
                                    Price = x.Price,
                                    WarehouseId = x.WareHouseId,
                                    AssignId = x.AssignToId,
                                    IconId = x.AssetsCategoryIconId,
                                    ColorCode = x.ColorCode,
                                    AssetIconImgUrl = x.AssetIconImgUrl,
                                    AssetType = x.AssetsType,
                                    IsAssetsIcon = x.IsAssetsIcon
                                }).ToList(),
                        TotalDigitalAssetsAmount = assetdata.Where(x => x.AssignToId == data.AssignedToId && x.AssetsType == AssetsItemType.Digital)
                        .Select(x => x.Price).Sum(),
                    };
                    list.Add(obj);
                }


                //        res.Message = "Assets List Found";
                //        res.Status = true;
                //        //res.Data = damageassets;
                //        if (page.HasValue && count.HasValue && search != null)
                //        {
                //            var text = textInfo.ToUpper(search);
                //            res.Data = new PaginationData
                //            {
                //                TotalData = damageassets.Count,
                //                Counts = (int)count,
                //                List = damageassets.Where(x => x.ItemName.ToUpper().StartsWith(text) || x.AssignedToName.ToUpper().StartsWith(text))
                //                       .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                //            };
                //        }
                //        else if (page.HasValue && count.HasValue)
                //        {
                //            //if (page.HasValue && count.HasValue && !String.IsNullOrEmpty(search))
                //            //{
                //            res.Data = new PaginationData
                //            {
                //                TotalData = damageassets.Count,
                //                Counts = (int)count,
                //                //List = category.Where(x => x.AssetsBCategoryName.Contains(search)).ToList(),
                //                List = damageassets.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                //            };
                //        }
                //        else
                //        {
                //            res.Data = damageassets;
                //        }
                //    }
                //    else
                //    {
                //        res.Message = "Assets List Not Found";
                //        res.Status = false;
                //        res.Data = damageassets;
                //    }
                //}


                if (list.Count != 0)
                {
                    res.Message = "Department list Found";
                    res.Status = true;
                    //    if (/*(page.HasValue && count.HasValue)||*/(search!=null))
                    if (page.HasValue && count.HasValue)
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = list.Count,
                            Counts = (int)count,
                            List = list.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            //List = assetdata.Where(x => x.ItemName.ToUpper().StartsWith(text) || x.AssignedToName.ToUpper().StartsWith(text) ||x.ItemCode.ToUpper().StartsWith(text) || x.SerialNo.ToUpper().StartsWith(text) || x.DepartmentName.ToUpper().StartsWith(text))
                            //       .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = list;
                    }
                }
                else
                {
                    res.Message = "No Item Is Assigned";
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

        #endregion GetAlltheAssignAssets

        #region Update Asset Details (Assets recover)

        /// <summary>
        /// Created by Shriya Malvi On 13-07-2022
        /// API >> Put >>  api/assetsnew/recoverassignasset
        /// </summary>
        /// <param name="asset"></ param >
        [HttpPut]
        [Route("recoverassignasset")]
        public async Task<ResponseBodyModel> RecoverAssignAsset(HelperForRecoverAsset asset)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var assetAssD = _db.AssetsItemMasters.Where(x => x.ItemId == asset.ItemId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                if (assetAssD != null)
                {
                    if (asset.ConditionId == AssetConditionConstants.Good || asset.ConditionId == AssetConditionConstants.Fair)
                    {

                        assetAssD.RecoverById = claims.employeeId;
                        assetAssD.AssetCondition = asset.ConditionId;
                        assetAssD.AssetStatus = AssetStatusConstants.Available;
                        assetAssD.RecoverDate = DateTime.Now;
                        assetAssD.Recovered = true;
                        assetAssD.Assigned = false;
                        assetAssD.RecoverImage1 = asset.RecoverImage1;
                        assetAssD.RecoverImage2 = asset.RecoverImage2;
                        assetAssD.RecoverImage3 = asset.RecoverImage3;
                        assetAssD.RecoverImage4 = asset.RecoverImage4;
                        assetAssD.RecoverImage5 = asset.RecoverImage5;

                        //var history = _db.AsstesAssignHistories.Where(x => x.AssignToId == assetAssD.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                        //if (history != null)
                        //{
                        //    history.RecoverBy = claims.employeeid;
                        //    history.AssetCondition = Enum.GetName(typeof(AssetConditionEnum), asset.ConditionId);
                        //    history.IsDeleted = true;
                        //    history.IsActive = false;
                        //    history.DeletedOn = DateTime.Now;
                        //    history.DeletedBy = claims.employeeid;
                        //    history.RecoverImage1 = asset.RecoverImage1;
                        //    history.RecoverImage2 = asset.RecoverImage2;
                        //    history.RecoverImage3 = asset.RecoverImage3;
                        //    history.RecoverImage4 = asset.RecoverImage4;
                        //    history.RecoverImage5 = asset.RecoverImage5;
                        //    _db.Entry(history).State = System.Data.Entity.EntityState.Modified;
                        //    await _db.SaveChangesAsync();
                        //}
                    }
                    else if (assetAssD.AssetCondition == AssetConditionConstants.Damage)
                    {
                        assetAssD.RecoverById = claims.employeeId;
                        assetAssD.AssetCondition = asset.ConditionId;
                        assetAssD.AssetStatus = AssetStatusConstants.Damage;
                        assetAssD.Recovered = true;
                        assetAssD.Assigned = false;
                        assetAssD.Assigned = false;
                        assetAssD.AvailablityStatus = false;
                        assetAssD.RecoverImage1 = asset.RecoverImage1;
                        assetAssD.RecoverImage2 = asset.RecoverImage2;
                        assetAssD.RecoverImage3 = asset.RecoverImage3;
                        assetAssD.RecoverImage4 = asset.RecoverImage4;
                        assetAssD.RecoverImage5 = asset.RecoverImage5;

                        //var history = _db.AsstesAssignHistories.Where(x => x.AssignToId == assetAssD.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                        //if (history != null)
                        //{
                        //    history.RecoverBy = claims.employeeid;
                        //    history.AssetCondition = Enum.GetName(typeof(AssetConditionEnum), asset.ConditionId);
                        //    history.IsDeleted = true;
                        //    history.IsActive = false;
                        //    history.DeletedOn = DateTime.Now;
                        //    history.DeletedBy = claims.employeeid;
                        //    history.RecoverImage1 = asset.RecoverImage1;
                        //    history.RecoverImage2 = asset.RecoverImage2;
                        //    history.RecoverImage3 = asset.RecoverImage3;
                        //    history.RecoverImage4 = asset.RecoverImage4;
                        //    history.RecoverImage5 = asset.RecoverImage5;
                        //    _db.Entry(history).State = System.Data.Entity.EntityState.Modified;
                        //    await _db.SaveChangesAsync();
                        //}
                    }
                    else
                    {
                        assetAssD.RecoverById = claims.employeeId;
                        assetAssD.AssetCondition = asset.ConditionId;
                        assetAssD.AssetStatus = AssetStatusConstants.UnderRepair;
                        assetAssD.Recovered = true;
                        assetAssD.Assigned = false;
                        assetAssD.Assigned = false;
                        assetAssD.AvailablityStatus = false;
                        assetAssD.RecoverImage1 = asset.RecoverImage1;
                        assetAssD.RecoverImage2 = asset.RecoverImage2;
                        assetAssD.RecoverImage3 = asset.RecoverImage3;
                        assetAssD.RecoverImage4 = asset.RecoverImage4;
                        assetAssD.RecoverImage5 = asset.RecoverImage5;

                        //var history = _db.AsstesAssignHistories.Where(x => x.AssignToId == assetAssD.AssignToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                        //if (history != null)
                        //{
                        //    history.RecoverBy = claims.employeeid;
                        //    history.AssetCondition = Enum.GetName(typeof(AssetConditionEnum), asset.ConditionId);
                        //    history.IsDeleted = true;
                        //    history.IsActive = false;
                        //    history.DeletedOn = DateTime.Now;
                        //    history.DeletedBy = claims.employeeid;
                        //    history.RecoverImage1 = asset.RecoverImage1;
                        //    history.RecoverImage2 = asset.RecoverImage2;
                        //    history.RecoverImage3 = asset.RecoverImage3;
                        //    history.RecoverImage4 = asset.RecoverImage4;
                        //    history.RecoverImage5 = asset.RecoverImage5;
                        //    _db.Entry(history).State = System.Data.Entity.EntityState.Modified;
                        //    await _db.SaveChangesAsync();
                        //}
                    }
                    var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == asset.ItemId);

                    AssetsHistory history = new AssetsHistory();
                    {
                        history.RecoverImage1 = updatehistory.RecoverImage1;
                        history.RecoverImage2 = updatehistory.RecoverImage2;
                        history.RecoverImage3 = updatehistory.RecoverImage3;
                        history.RecoverImage4 = updatehistory.RecoverImage4;
                        history.RecoverImage5 = updatehistory.RecoverImage5;
                        history.ItemId = updatehistory.ItemId;
                        history.ItemName = updatehistory.ItemName;
                        history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                        history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                        history.WareHouseId = updatehistory.WareHouseId;
                        history.ItemCode = updatehistory.ItemCode;
                        history.SerialNo = updatehistory.SerialNo;
                        history.Location = updatehistory.Location;
                        history.InvoiceUrl = updatehistory.InvoiceUrl;
                        history.PurchaseDate = updatehistory.PurchaseDate;
                        history.AssignToId = updatehistory.AssignToId;
                        history.AssignById = updatehistory.AssignById;
                        history.RecoverById = updatehistory.RecoverById;
                        history.AssetCondition = updatehistory.AssetCondition;
                        history.AssetStatus = updatehistory.AssetStatus;
                        history.AssetsDescription = updatehistory.AssetsDescription;
                        history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                        history.AvailablityStatus = updatehistory.AvailablityStatus;
                        history.Recovered = updatehistory.Recovered;
                        history.Assigned = updatehistory.Assigned;
                        history.AssignDate = updatehistory.AssignDate;
                        history.RecoverDate = updatehistory.RecoverDate;
                        history.Comment = updatehistory.Comment;
                        history.IsRefurbish = updatehistory.IsRefurbish;
                        history.RefurbishCount = updatehistory.RefurbishCount;
                        history.InvoiceNo = updatehistory.InvoiceNo;
                        history.Price = updatehistory.Price;
                        history.WarentyExpDate = updatehistory.WarentyExpDate;
                        history.Compliance = updatehistory.Compliance;
                        history.IsCompliance = updatehistory.IsCompliance;
                        history.LicenseKey = updatehistory.LicenseKey;
                        history.LicenseStartDate = updatehistory.LicenseStartDate;
                        history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                        history.LicApplicableCount = updatehistory.LicApplicableCount;
                        history.AssetsType = updatehistory.AssetsType;
                        history.UpImg1 = updatehistory.UpImg1;
                        history.UpImg2 = updatehistory.UpImg2;
                        history.UpImg3 = updatehistory.UpImg3;
                        history.UpImg4 = updatehistory.UpImg4;
                        history.UpImg5 = updatehistory.UpImg5;
                        history.UpImg6 = updatehistory.UpImg6;
                        history.UpImg7 = updatehistory.UpImg7;
                        history.UpImg8 = updatehistory.UpImg8;
                        history.UpImg9 = updatehistory.UpImg9;
                        history.UpImg10 = updatehistory.UpImg10;
                        history.IsActive = updatehistory.IsActive;
                        history.IsDeleted = updatehistory.IsDeleted;
                        history.CompanyId = updatehistory.CompanyId;
                        history.OrgId = updatehistory.OrgId;
                        history.CreatedBy = updatehistory.CreatedBy;
                        history.UpdatedBy = updatehistory.UpdatedBy;
                        history.DeletedBy = updatehistory.DeletedBy;
                        history.CreatedOn = updatehistory.CreatedOn;
                        history.UpdatedOn = updatehistory.UpdatedOn;
                        history.DeletedOn = updatehistory.DeletedOn;
                        _db.AssetsHistories.Add(history);
                        await _db.SaveChangesAsync();
                    }
                    assetAssD.AssignToId = 0;
                    assetAssD.AssignedToName = null;
                    _db.Entry(assetAssD).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    response.Status = true;
                    response.Message = "Assign assets recover succesfully";
                    response.Data = assetAssD;
                }
                else
                {
                    response.Status = false;
                    response.Message = "Assign assets not recover";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        #endregion Update Asset Details (Assets recover)

        #region update condition of assign assets to  assignee

        /// <summary>
        /// Created By shriya Malvi On 12-07-2022
        /// API >> Put >>  api/assetsnew/assignassetconup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("assignassetconup")]
        public async Task<ResponseBodyModel> UpdateConditionOfAssignAssets(HelperForUpdateCondition model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ConditionUpdate = _db.AssetsItemMasters.Where(x => x.ItemId == model.ItemId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefault();
                if (ConditionUpdate != null)
                {
                    if ((AssetConditionConstants.Damage) == model.ConditionId)
                    {
                        ConditionUpdate.AssignToId = 0;
                        ConditionUpdate.AssignedToName = null;
                        ConditionUpdate.AssetCondition = AssetConditionConstants.Damage;
                        ConditionUpdate.RecoverById = claims.employeeId;
                        ConditionUpdate.RecoverDate = DateTime.Now;
                        ConditionUpdate.Recovered = true;
                        ConditionUpdate.Assigned = false;
                        ConditionUpdate.AvailablityStatus = false;
                        ConditionUpdate.AssetStatus = AssetStatusConstants.Damage;
                        ConditionUpdate.UpdatedBy = claims.employeeId;
                        ConditionUpdate.UpdatedOn = DateTime.Now;
                        ConditionUpdate.RecoverImage1 = model.RecoverImage1;
                        ConditionUpdate.RecoverImage2 = model.RecoverImage2;
                        ConditionUpdate.RecoverImage3 = model.RecoverImage3;
                        ConditionUpdate.RecoverImage4 = model.RecoverImage4;
                        ConditionUpdate.RecoverImage5 = model.RecoverImage5;
                        _db.Entry(ConditionUpdate).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
                        await _db.SaveChangesAsync();

                        res.Data = ConditionUpdate;
                        res.Message = "update assets condition and recover assets";
                        res.Status = true;
                    }
                    else if ((AssetConditionConstants.UnderRepair) == model.ConditionId)
                    {
                        ConditionUpdate.AssignToId = 0;
                        ConditionUpdate.AssignedToName = null;
                        ConditionUpdate.AssetCondition = AssetConditionConstants.UnderRepair;
                        ConditionUpdate.RecoverById = claims.employeeId;
                        ConditionUpdate.RecoverDate = DateTime.Now;
                        ConditionUpdate.Recovered = true;
                        ConditionUpdate.Assigned = false;
                        ConditionUpdate.AvailablityStatus = false;
                        ConditionUpdate.AssetStatus = AssetStatusConstants.UnderRepair;
                        ConditionUpdate.UpdatedBy = claims.employeeId;
                        ConditionUpdate.UpdatedOn = DateTime.Now;
                        ConditionUpdate.RecoverImage1 = model.RecoverImage1;
                        ConditionUpdate.RecoverImage2 = model.RecoverImage2;
                        ConditionUpdate.RecoverImage3 = model.RecoverImage3;
                        ConditionUpdate.RecoverImage4 = model.RecoverImage4;
                        ConditionUpdate.RecoverImage5 = model.RecoverImage5;

                        _db.Entry(ConditionUpdate).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
                        await _db.SaveChangesAsync();

                        res.Data = ConditionUpdate;
                        res.Message = "update assets condition and recover assets";
                        res.Status = true;
                    }
                    else
                    {
                        ConditionUpdate.AssetCondition = model.ConditionId;
                        ConditionUpdate.UpdatedBy = claims.employeeId;
                        ConditionUpdate.UpdatedOn = DateTime.Now;

                        _db.Entry(ConditionUpdate).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
                        await _db.SaveChangesAsync();

                        res.Data = ConditionUpdate;
                        res.Message = "Assets condition updated";
                        res.Status = true;
                    }
                    var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == model.ItemId);

                    AssetsHistory history = new AssetsHistory();
                    {
                        history.AssignImage1 = updatehistory.AssignImage1;
                        history.AssignImage2 = updatehistory.AssignImage2;
                        history.AssignImage3 = updatehistory.AssignImage3;
                        history.AssignImage4 = updatehistory.AssignImage4;
                        history.AssignImage5 = updatehistory.AssignImage5;
                        history.RecoverImage1 = updatehistory.RecoverImage1;
                        history.RecoverImage2 = updatehistory.RecoverImage2;
                        history.RecoverImage3 = updatehistory.RecoverImage3;
                        history.RecoverImage4 = updatehistory.RecoverImage4;
                        history.RecoverImage5 = updatehistory.RecoverImage5;
                        history.ItemId = updatehistory.ItemId;
                        history.ItemName = updatehistory.ItemName;
                        history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                        history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                        history.WareHouseId = updatehistory.WareHouseId;
                        history.ItemCode = updatehistory.ItemCode;
                        history.SerialNo = updatehistory.SerialNo;
                        history.Location = updatehistory.Location;
                        history.InvoiceUrl = updatehistory.InvoiceUrl;
                        history.PurchaseDate = updatehistory.PurchaseDate;
                        history.AssignToId = updatehistory.AssignToId;
                        history.AssignById = updatehistory.AssignById;
                        history.RecoverById = updatehistory.RecoverById;
                        history.AssetCondition = updatehistory.AssetCondition;
                        history.AssetStatus = updatehistory.AssetStatus;
                        history.AssetsDescription = updatehistory.AssetsDescription;
                        history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                        history.AvailablityStatus = updatehistory.AvailablityStatus;
                        history.Recovered = updatehistory.Recovered;
                        history.Assigned = updatehistory.Assigned;
                        history.AssignDate = updatehistory.AssignDate;
                        history.RecoverDate = updatehistory.RecoverDate;
                        history.Comment = updatehistory.Comment;
                        history.IsRefurbish = updatehistory.IsRefurbish;
                        history.RefurbishCount = updatehistory.RefurbishCount;
                        history.InvoiceNo = updatehistory.InvoiceNo;
                        history.Price = updatehistory.Price;
                        history.WarentyExpDate = updatehistory.WarentyExpDate;
                        history.Compliance = updatehistory.Compliance;
                        history.IsCompliance = updatehistory.IsCompliance;
                        history.LicenseKey = updatehistory.LicenseKey;
                        history.LicenseStartDate = updatehistory.LicenseStartDate;
                        history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                        history.LicApplicableCount = updatehistory.LicApplicableCount;
                        history.AssetsType = updatehistory.AssetsType;
                        history.UpImg1 = updatehistory.UpImg1;
                        history.UpImg2 = updatehistory.UpImg2;
                        history.UpImg3 = updatehistory.UpImg3;
                        history.UpImg4 = updatehistory.UpImg4;
                        history.UpImg5 = updatehistory.UpImg5;
                        history.UpImg6 = updatehistory.UpImg6;
                        history.UpImg7 = updatehistory.UpImg7;
                        history.UpImg8 = updatehistory.UpImg8;
                        history.UpImg9 = updatehistory.UpImg9;
                        history.UpImg10 = updatehistory.UpImg10;
                        history.IsActive = updatehistory.IsActive;
                        history.IsDeleted = updatehistory.IsDeleted;
                        history.CompanyId = updatehistory.CompanyId;
                        history.OrgId = updatehistory.OrgId;
                        history.CreatedBy = updatehistory.CreatedBy;
                        history.UpdatedBy = updatehistory.UpdatedBy;
                        history.DeletedBy = updatehistory.DeletedBy;
                        history.CreatedOn = updatehistory.CreatedOn;
                        history.UpdatedOn = updatehistory.UpdatedOn;
                        history.DeletedOn = updatehistory.DeletedOn;
                        _db.AssetsHistories.Add(history);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "Asstes not found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion update condition of assign assets to  assignee

        #region Mark AS Not Available

        /// <summary>
        /// API >> Put >> api/assetsnew/markasnotavailable
        /// Created by Suraj Bundel on 12/07/2022
        /// </summary>
        [Route("markasnotavailable")]
        [HttpPut]
        public async Task<ResponseBodyModel> MarkAsNotAvailable(HelperMarkasnotAvailable asset)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var assetData = _db.AssetsItemMasters.Where(x => x.AvailablityStatus && !x.IsDeleted && x.IsActive && x.ItemId == asset.ItemId && x.CompanyId == claims.companyId).FirstOrDefault();

                if (assetData != null)
                {
                    //assetData.AssetCondition = asset.AssetCondition;
                    assetData.ReasonNotAvailable = asset.ReasonNotAvailable;
                    //assetData.AssetStatus = Model.EnumClass.AssetStatusEnum.Damage;
                    //assetData.AssetStatus = AssetStatusEnum.Notavailable;
                    assetData.UpdatedBy = claims.employeeId;
                    assetData.UpdatedOn = DateTime.Now;
                    assetData.AvailablityStatus = false;
                    assetData.IsActive = false;
                    assetData.IsDeleted = true;
                    _db.Entry(assetData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    response.Status = true;
                    response.Message = "Details Updated Successfully!";
                    response.Data = assetData;
                }
                var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == asset.ItemId);

                AssetsHistory history = new AssetsHistory();
                {
                    history.RecoverImage1 = updatehistory.RecoverImage1;
                    history.RecoverImage2 = updatehistory.RecoverImage2;
                    history.RecoverImage3 = updatehistory.RecoverImage3;
                    history.RecoverImage4 = updatehistory.RecoverImage4;
                    history.RecoverImage5 = updatehistory.RecoverImage5;
                    history.AssignImage1 = updatehistory.AssignImage1;
                    history.AssignImage2 = updatehistory.AssignImage2;
                    history.AssignImage3 = updatehistory.AssignImage3;
                    history.AssignImage4 = updatehistory.AssignImage4;
                    history.AssignImage5 = updatehistory.AssignImage5;
                    history.ItemId = updatehistory.ItemId;
                    history.ItemName = updatehistory.ItemName;
                    history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                    history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                    history.WareHouseId = updatehistory.WareHouseId;
                    history.ItemCode = updatehistory.ItemCode;
                    history.SerialNo = updatehistory.SerialNo;
                    history.Location = updatehistory.Location;
                    history.InvoiceUrl = updatehistory.InvoiceUrl;
                    history.PurchaseDate = updatehistory.PurchaseDate;
                    history.AssignToId = updatehistory.AssignToId;
                    history.AssignById = updatehistory.AssignById;
                    history.RecoverById = updatehistory.RecoverById;
                    history.AssetCondition = updatehistory.AssetCondition;
                    history.AssetStatus = updatehistory.AssetStatus;
                    history.AssetsDescription = updatehistory.AssetsDescription;
                    history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                    history.AvailablityStatus = updatehistory.AvailablityStatus;
                    history.Recovered = updatehistory.Recovered;
                    history.Assigned = updatehistory.Assigned;
                    history.AssignDate = updatehistory.AssignDate;
                    history.RecoverDate = updatehistory.RecoverDate;
                    history.Comment = updatehistory.Comment;
                    history.IsRefurbish = updatehistory.IsRefurbish;
                    history.RefurbishCount = updatehistory.RefurbishCount;
                    history.InvoiceNo = updatehistory.InvoiceNo;
                    history.Price = updatehistory.Price;
                    history.WarentyExpDate = updatehistory.WarentyExpDate;
                    history.Compliance = updatehistory.Compliance;
                    history.IsCompliance = updatehistory.IsCompliance;
                    history.LicenseKey = updatehistory.LicenseKey;
                    history.LicenseStartDate = updatehistory.LicenseStartDate;
                    history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                    history.LicApplicableCount = updatehistory.LicApplicableCount;
                    history.AssetsType = updatehistory.AssetsType;
                    history.UpImg1 = updatehistory.UpImg1;
                    history.UpImg2 = updatehistory.UpImg2;
                    history.UpImg3 = updatehistory.UpImg3;
                    history.UpImg4 = updatehistory.UpImg4;
                    history.UpImg5 = updatehistory.UpImg5;
                    history.UpImg6 = updatehistory.UpImg6;
                    history.UpImg7 = updatehistory.UpImg7;
                    history.UpImg8 = updatehistory.UpImg8;
                    history.UpImg9 = updatehistory.UpImg9;
                    history.UpImg10 = updatehistory.UpImg10;
                    history.IsActive = updatehistory.IsActive;
                    history.IsDeleted = updatehistory.IsDeleted;
                    history.CompanyId = updatehistory.CompanyId;
                    history.OrgId = updatehistory.OrgId;
                    history.CreatedBy = updatehistory.CreatedBy;
                    history.UpdatedBy = updatehistory.UpdatedBy;
                    history.DeletedBy = updatehistory.DeletedBy;
                    history.CreatedOn = updatehistory.CreatedOn;
                    history.UpdatedOn = updatehistory.UpdatedOn;
                    history.DeletedOn = updatehistory.DeletedOn;
                    _db.AssetsHistories.Add(history);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        #endregion Mark AS Not Available

        #region Get Unavailable Assets List

        /// <summary>
        /// Create by Suraj Bundel On 11-07-2022
        /// API >> Get >> api/assetsnew/getmarkasunavail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmarkasunavail")]
        public async Task<ResponseBodyModel> GetMarkasUnavail()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var LostAssetList = await _db.AssetsItemMasters.Where(x => x.CompanyId == claims.companyId && !x.AvailablityStatus && !x.IsActive && x.IsDeleted && x.AssetStatus == AssetStatusConstants.Damage).ToListAsync();
                if (LostAssetList.Count > 0)
                {
                    res.Data = LostAssetList;
                    res.Message = "Lost Asset List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Not Lose Any Assets";
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

        #endregion Get Unavailable Assets List

        #region Get All Damge Assets

        /// <summary>
        /// Create  By Shriya Malvi On 13-07-2022
        /// API >> Get >> api/assetsnew/getalldamageassets
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <returns></returns>
        [Route("getalldamageassets")]
        public async Task<ResponseBodyModel> GetAllDamgeAssets(int WareHouseId, string search = null, int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var damageassets = await (from ai in _db.AssetsItemMasters
                                          join bc in _db.AssetsBaseCategories on ai.AssetsBaseCategoryId equals bc.AssetsBCategoryId
                                          join ct in _db.AssetsCategories on ai.AssetsCategoryId equals ct.AssetsCategoryId
                                          join wh in _db.AssetsWarehouses on ai.WareHouseId equals wh.WarehouseId
                                          where ai.CompanyId == claims.companyId && ai.WareHouseId == WareHouseId
                                          && ai.IsActive && !ai.IsDeleted && ai.AssetCondition ==
                                          AssetConditionConstants.Damage && ai.AssetStatus == AssetStatusConstants.Damage
                                          select new
                                          {
                                              ItemId = ai.ItemId,
                                              ItemName = ai.ItemName,
                                              ItemCode = ai.ItemCode,
                                              ai.SerialNo,
                                              AssignToId = ai.AssignToId,
                                              AssignedToName = ai.AssignedToName,
                                              RecoverById = ai.RecoverById,
                                              AssetCondition = ai.AssetCondition.ToString().Replace("_", " "),
                                              AssetStatus = ai.AssetStatus.ToString(),
                                              BaseCategoryName = bc.AssetsBCategoryName,
                                              CategoryName = ct.AssetsCategoryName,
                                              wh.WarehouseId,
                                              wh.WarehouseName,
                                              ai.Location,
                                              ai.PurchaseDate
                                              //RecoveredbyName = e.DisplayName,
                                          }
                                    ).ToListAsync();
                if (damageassets.Count > 0)
                {
                    //damageassets.WareHouseId = Model.WareHouseId;
                    //damageassets.AssetCondition = (AssetConditionEnum.Damage).ToString();
                    //damageassets.AssetStatus = (AssetStatusEnum.Unavailable).ToString();
                    //damageassets.AssignToId=Model.AssignToId;


                    res.Message = "Assets List Found";
                    res.Status = true;
                    //res.Data = damageassets;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = damageassets.Count,
                            Counts = (int)count,
                            List = damageassets.Where(x => x.ItemName.ToUpper().StartsWith(text) || x.AssignedToName.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        //if (page.HasValue && count.HasValue && !String.IsNullOrEmpty(search))
                        //{
                        res.Data = new PaginationData
                        {
                            TotalData = damageassets.Count,
                            Counts = (int)count,
                            List = damageassets.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                        };
                    }
                    else
                    {
                        res.Data = damageassets;
                    }
                }
                else
                {
                    res.Message = "Assets List Not Found";
                    res.Status = false;
                    res.Data = damageassets;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All Damge Assets

        #region Update Damage Asset Condition

        /// <summary>
        /// API >> Put >> api/assetsnew/updatedamageassetcondition
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatedamageassetcondition")]
        public async Task<ResponseBodyModel> Updateassetcondition(HelperDamageAssetCondtion model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updatecon = await _db.AssetsItemMasters.Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId && x.ItemId == model.ItemId).FirstOrDefaultAsync();

                if (updatecon != null)
                {
                    if (updatecon.AssetCondition == AssetConditionConstants.Damage)
                    {
                        updatecon.AssetCondition = (AssetConditionConstants)Enum.Parse(typeof(AssetConditionConstants), model.ConditionId.ToString());

                        if (updatecon.AssetCondition == AssetConditionConstants.Damage)
                        {
                            updatecon.AssetStatus = Model.EnumClass.AssetStatusConstants.Damage;
                        }
                        else if (updatecon.AssetCondition == AssetConditionConstants.UnderRepair)
                        {
                            updatecon.AssetStatus = Model.EnumClass.AssetStatusConstants.UnderRepair;
                        }
                        else
                        {
                            updatecon.AssetStatus = Model.EnumClass.AssetStatusConstants.Available;
                        }
                        _db.Entry(updatecon).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Data = updatecon;
                        res.Status = true;
                        res.Message = "Update Condition";
                    }
                    if (updatecon.AssetCondition == AssetConditionConstants.UnderRepair)
                    {
                        updatecon.AssetCondition = (AssetConditionConstants)Enum.Parse(typeof(AssetConditionConstants), model.ConditionId.ToString());

                        if (updatecon.AssetCondition == AssetConditionConstants.Damage)
                        {
                            updatecon.AssetStatus = Model.EnumClass.AssetStatusConstants.Damage;
                        }
                        else if (updatecon.AssetCondition == AssetConditionConstants.UnderRepair)
                        {
                            updatecon.AssetStatus = Model.EnumClass.AssetStatusConstants.UnderRepair;
                        }
                        else
                        {
                            updatecon.AssetStatus = Model.EnumClass.AssetStatusConstants.Available;
                        }
                        _db.Entry(updatecon).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Data = updatecon;
                        res.Status = true;
                        res.Message = "Update Condition";
                    }
                    var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == model.ItemId);

                    AssetsHistory history = new AssetsHistory();
                    {
                        history.RecoverImage1 = updatehistory.RecoverImage1;
                        history.RecoverImage2 = updatehistory.RecoverImage2;
                        history.RecoverImage3 = updatehistory.RecoverImage3;
                        history.RecoverImage4 = updatehistory.RecoverImage4;
                        history.RecoverImage5 = updatehistory.RecoverImage5;
                        history.AssignImage1 = updatehistory.AssignImage1;
                        history.AssignImage2 = updatehistory.AssignImage2;
                        history.AssignImage3 = updatehistory.AssignImage3;
                        history.AssignImage4 = updatehistory.AssignImage4;
                        history.AssignImage5 = updatehistory.AssignImage5;
                        history.ItemId = updatehistory.ItemId;
                        history.ItemName = updatehistory.ItemName;
                        history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                        history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                        history.WareHouseId = updatehistory.WareHouseId;
                        history.ItemCode = updatehistory.ItemCode;
                        history.SerialNo = updatehistory.SerialNo;
                        history.Location = updatehistory.Location;
                        history.InvoiceUrl = updatehistory.InvoiceUrl;
                        history.PurchaseDate = updatehistory.PurchaseDate;
                        history.AssignToId = updatehistory.AssignToId;
                        history.AssignById = updatehistory.AssignById;
                        history.RecoverById = updatehistory.RecoverById;
                        history.AssetCondition = updatehistory.AssetCondition;
                        history.AssetStatus = updatehistory.AssetStatus;
                        history.AssetsDescription = updatehistory.AssetsDescription;
                        history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                        history.AvailablityStatus = updatehistory.AvailablityStatus;
                        history.Recovered = updatehistory.Recovered;
                        history.Assigned = updatehistory.Assigned;
                        history.AssignDate = updatehistory.AssignDate;
                        history.RecoverDate = updatehistory.RecoverDate;
                        history.Comment = updatehistory.Comment;
                        history.IsRefurbish = updatehistory.IsRefurbish;
                        history.RefurbishCount = updatehistory.RefurbishCount;
                        history.InvoiceNo = updatehistory.InvoiceNo;
                        history.Price = updatehistory.Price;
                        history.WarentyExpDate = updatehistory.WarentyExpDate;
                        history.Compliance = updatehistory.Compliance;
                        history.IsCompliance = updatehistory.IsCompliance;
                        history.LicenseKey = updatehistory.LicenseKey;
                        history.LicenseStartDate = updatehistory.LicenseStartDate;
                        history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                        history.LicApplicableCount = updatehistory.LicApplicableCount;
                        history.AssetsType = updatehistory.AssetsType;
                        history.UpImg1 = updatehistory.UpImg1;
                        history.UpImg2 = updatehistory.UpImg2;
                        history.UpImg3 = updatehistory.UpImg3;
                        history.UpImg4 = updatehistory.UpImg4;
                        history.UpImg5 = updatehistory.UpImg5;
                        history.UpImg6 = updatehistory.UpImg6;
                        history.UpImg7 = updatehistory.UpImg7;
                        history.UpImg8 = updatehistory.UpImg8;
                        history.UpImg9 = updatehistory.UpImg9;
                        history.UpImg10 = updatehistory.UpImg10;
                        history.IsActive = updatehistory.IsActive;
                        history.IsDeleted = updatehistory.IsDeleted;
                        history.CompanyId = updatehistory.CompanyId;
                        history.OrgId = updatehistory.OrgId;
                        history.CreatedBy = updatehistory.CreatedBy;
                        history.UpdatedBy = updatehistory.UpdatedBy;
                        history.DeletedBy = updatehistory.DeletedBy;
                        history.CreatedOn = updatehistory.CreatedOn;
                        history.UpdatedOn = updatehistory.UpdatedOn;
                        history.DeletedOn = updatehistory.DeletedOn;
                        _db.AssetsHistories.Add(history);
                        await _db.SaveChangesAsync();
                    }

                }
                else
                {
                    res.Data = updatecon;
                    res.Status = false;
                    res.Message = " Condition not  update";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Update Damage Asset Condition

        #region Used for Assets Condition Api

        /// <summary>
        /// Create by Shriya Malvi On 11-07-2022
        /// API >>  Post >> api/assetsnew/getallassetscondition
        /// </summary>
        /// <returns></returns>
        [Route("getallassetscondition")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel GetAllAssetsCondition()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var condition = Enum.GetValues(typeof(AssetConditionConstants))
                    .Cast<AssetConditionConstants>()
                    .Select(x => new HelperForAssetsDd
                    {
                        Id = (int)x,
                        Name = Enum.GetName(typeof(AssetConditionConstants), x)
                    }).ToList();
                res.Message = "Assets Condition";
                res.Status = true;
                res.Data = condition;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Used for Assets Condition Api

        #region Used for Assets Status Api

        /// <summary>
        /// Create by Shriya Malvi On 11-07-2022
        /// API >>  GET  >> api/assetsnew/getallnotavailassetstatus
        /// </summary>
        /// <returns></returns>
        [Route("getallnotavailassetstatus")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel GetAllNotAvailableAssetStatus()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var status = Enum.GetValues(typeof(AssetsReasonForMarkNotAvailConstants))
                    .Cast<AssetsReasonForMarkNotAvailConstants>()
                    .Select(x => new HelperForAssetsDd
                    {
                        Id = (int)x,
                        Name = Enum.GetName(typeof(AssetsReasonForMarkNotAvailConstants), x)
                    }).ToList();
                res.Message = "Assets Status";
                res.Status = true;
                res.Data = status;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Used for Assets Status Api

        #region Get all Asset Status

        /// <summary>
        /// Create by Suraj Bundel On 12-07-2022
        /// API >>  Get >> api/assetsnew/getallassetstatus
        /// </summary>
        /// <returns></returns>
        [Route("getallassetstatus")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel GetAllAssetStatus()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var availstatus = Enum.GetValues(typeof(AssetStatusConstants))
                    .Cast<AssetStatusConstants>()
                    .Select(x => new HelperForAssetsDd
                    {
                        Id = (int)x,
                        Name = Enum.GetName(typeof(AssetStatusConstants), x)
                    }).ToList();
                res.Message = "Assets Status";
                res.Status = true;
                res.Data = availstatus;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get all Asset Status

        #region Get My Assets

        /// <summary>
        /// Created by Shriya Malvi On 15-07-2022
        /// Modified By Shriya Malvi On 16-08-2022
        /// API >> Get >> api/assetsnew/GetMyAssets
        /// </summary>
        [HttpGet]
        [Route("getmyassets")]
        public async Task<ResponseBodyModel> GetMyAssets(int? page = null, int? count = null)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<AssignAssetsData> AssignAssetDataList = new List<AssignAssetsData>();
                var AssignData = await (from ass in _db.AssetsItemMasters
                                        join it in _db.AssetsCategories on ass.AssetsCategoryId equals it.AssetsCategoryId
                                        where ass.AssignToId == claims.employeeId && ass.Assigned && ass.IsActive && !ass.IsDeleted &&
                                                ass.AssetStatus == Model.EnumClass.AssetStatusConstants.Assigned && ass.AssetCondition != AssetConditionConstants.Damage && ass.AssetCondition != AssetConditionConstants.UnderRepair
                                        //  ass.AssetStatus == AssetStatusEnum.Unavailable && ass.AssetCondition != AssetConditionEnum.Damage && !ass.IsDeleted
                                        select new
                                        {
                                            ass.ItemId,
                                            ass.ItemName,
                                            ass.AssetsBaseCategoryId,
                                            ass.AssetsBaseCategoryName,
                                            ass.AssetsCategoryId,
                                            ass.AssetsCategoryName,
                                            ass.WareHouseId,
                                            ass.WareHouseName,
                                            ass.SerialNo,
                                            ass.ItemCode,
                                            ass.AssetCondition,
                                            ass.AssetStatus,
                                            ass.Location,
                                            ass.AssignToId,
                                            ass.AssignedToName,
                                            ass.Comment,
                                            it.ColorCode,
                                            it.AssetsCategoryIconId,
                                            ass.AssignDate,
                                            it.IsAssetsIcon,
                                            it.AssetIconImgUrl,

                                        }).ToListAsync();
                foreach (var assignItem in AssignData)
                {
                    AssignAssetsData assets = new AssignAssetsData
                    {
                        ItemId = assignItem.ItemId,
                        ItemName = assignItem.ItemName,
                        AssignedToId = assignItem.AssignToId,
                        AssignedTo = assignItem.AssignedToName,
                        Condition = assignItem.AssetCondition.ToString().Replace("_", " "),
                        ItemCode = assignItem.ItemCode,
                        WarehouseId = assignItem.WareHouseId,
                        Comment = assignItem.Comment,
                        AssignDate = assignItem.AssignDate,
                        ColorCode = assignItem.ColorCode,
                        AssetsIconId = assignItem.AssetsCategoryIconId,
                        IsAssetsIcon = assignItem.IsAssetsIcon,
                        AssetIconImgUrl = assignItem.AssetIconImgUrl
                    };
                    AssignAssetDataList.Add(assets);
                }
                if (AssignAssetDataList.Count != 0)
                {
                    response.Message = "Department list Found";
                    response.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        response.Data = new PaginationData
                        {
                            TotalData = AssignAssetDataList.Count,
                            Counts = (int)count,
                            List = AssignAssetDataList.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        response.Data = AssignAssetDataList;
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "data is not found";
                    response.Data = AssignAssetDataList;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
        }

        #endregion Get My Assets

        #region Get All Lost Assets List

        /// <summary>
        /// Created By Suraj Bundel on 15-07-2022
        /// API >> GET >> api/assetsnew/getalllostasset
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalllostasset")]
        public async Task<ResponseBodyModel> GetAllLostList(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var assets = await _db.AssetsItemMasters.Where(x => x.IsDeleted && !x.IsActive && x.CompanyId == claims.companyId && x.AssetStatus == AssetStatusConstants.Damage && !x.AvailablityStatus).ToListAsync();

                if (assets.Count != 0)
                {
                    res.Message = "Assets List Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = assets.Count,
                            Counts = (int)count,
                            List = assets.Where(x => x.ItemName.ToUpper().StartsWith(text) || x.AssignedToName.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        //if (page.HasValue && count.HasValue && !String.IsNullOrEmpty(search))
                        //{
                        res.Data = new
                        {
                            TotalData = assets.Count,
                            Counts = (int)count,
                            //List = category.Where(x => x.AssetsBCategoryName.Contains(search)).ToList(),
                            List = assets.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                        };
                    }
                    else
                    {
                        res.Data = assets;
                    }
                }
                else
                {
                    res.Message = "Assets List Not Found";
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

        #endregion Get All Lost Assets List

        #region This is used for asstes information recovered

        /// <summary>
        /// Created by Shriya Malvi  On 15-07-2022
        /// API >> Get >> api/assetsnew/getinforecoverable
        /// </summary>
        [HttpGet]
        [Route("getinforecoverable")]
        public async Task<ResponseBodyModel> GetInfoRecoverable(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var FinalData = await (from aim in _db.AssetsItemMasters
                                       join abc in _db.AssetsBaseCategories on aim.AssetsBaseCategoryId equals abc.AssetsBCategoryId
                                       join ac in _db.AssetsCategories on aim.AssetsCategoryId equals ac.AssetsCategoryId
                                       join ah in _db.AsstesAssignHistories on aim.ItemId equals ah.ItemId
                                       join empR in _db.Employee on ah.RecoverBy equals empR.EmployeeId
                                       join empA in _db.Employee on ah.AssignToId equals empA.EmployeeId
                                       where !ah.IsActive && ah.IsDeleted && aim.CompanyId == claims.companyId
                                       && abc.CompanyId == claims.companyId && ac.CompanyId == claims.companyId
                                       && ah.CompanyId == claims.companyId
                                       select new
                                       {
                                           itemId = aim.ItemId,
                                           asstesName = aim.ItemName,
                                           categoryId = aim.AssetsCategoryId,
                                           categoryName = ac.AssetsCategoryName,
                                           basecategoryId = aim.AssetsBaseCategoryId,
                                           basecategoryName = abc.AssetsBCategoryName,
                                           condition = aim.AssetCondition.ToString(),
                                           employeeName = empA.DisplayName,
                                           recoverBy = empR.DisplayName,
                                           recoverDate = ah.DeletedOn,
                                       }).ToListAsync();

                if (FinalData.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Assets list";
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = FinalData.Count,
                            Counts = (int)count,
                            List = FinalData.Where(x => x.categoryName.ToUpper().StartsWith(text) || x.basecategoryName.ToUpper().StartsWith(text) || x.asstesName.ToUpper().StartsWith(text)
                                    || x.condition.ToUpper().StartsWith(text) || x.employeeName.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = FinalData.Count,
                            Counts = (int)count,
                            List = FinalData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                        };
                    }
                    else
                    {
                        res.Data = FinalData;
                    }
                }
                else
                {
                    res.Message = " Asset Not Found";
                    res.Status = false;
                    res.Data = FinalData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion This is used for asstes information recovered

        #region API for Upload multiple for upload invoice

        /// <summary>
        ///  29/11/21
        ///  API >> POST >> api/assetsnew/uploadmultiple
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadmultiple")]
        public async Task<UploadImageResponse> UploadMultipleFileAll()
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
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"').Replace(" ", "");

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Assetitem/AssetInvoice/" + claims.companyId), dates + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string temp = "uploadimage\\Assetitem\\AssetInvoice\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = temp;
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

        //public async Task<HttpResponseMessageMultiple> UploadMultipleFileAll()
        //{
        //    // logger.Info("Start UploadFileAll");
        //    HttpResponseMessageMultiple result = new HttpResponseMessageMultiple();
        //    List<PathLists> list = new List<PathLists>();
        //    try
        //    {
        //        var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //        //var identity = User.Identity as ClaimsIdentity;
        //        List<string> path = new List<string>();
        //        // int userid = 0;
        //        // Access claims

        //        //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        //    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        var data = Request.Content.IsMimeMultipartContent();
        //        if (Request.Content.IsMimeMultipartContent())
        //        {
        //            //fileList f = new fileList();
        //            var provider = new MultipartMemoryStreamProvider();
        //            await Request.Content.ReadAsMultipartAsync(provider);
        //            var content = provider.Contents.Count;

        //            for (int i = 0; i < content; i++)
        //            {
        //                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
        //                var filefromreq = provider.Contents[i];
        //                Stream _id = filefromreq.ReadAsStreamAsync().Result;
        //                StreamReader reader = new StreamReader(_id);
        //                string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

        //                string extension = Path.GetExtension(filename);
        //                string Fileresult = filename.Substring(0, filename.Length - extension.Length);

        //                byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
        //                //f.byteArray = buffer;
        //                string mime = filefromreq.Headers.ContentType.ToString();
        //                Stream stream = new MemoryStream(buffer);
        //                var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Assetitem/AssetInvoice/" + claims.companyid), dates + filename);
        //                string DirectoryURL = (FileUrl.Split(new string[] { claims.companyid + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyid;

        //                //for create new Folder
        //                DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
        //                if (!objDirectory.Exists)
        //                {
        //                    Directory.CreateDirectory(DirectoryURL);
        //                }
        //                ////////////// old Code 12-07-2021
        //                //string path = "UploadImages\\" + compid + "\\" + filename;

        //                var temp = "uploadimage\\Assetitem\\AssetInvoice\\" + claims.companyid + "\\" + dates + Fileresult + extension;

        //                ////////////// old Code 12-07-2021

        //                File.WriteAllBytes(FileUrl, buffer.ToArray());
        //                PathLists obj = new PathLists
        //                {
        //                    Pathurl = temp,
        //                };
        //                list.Add(obj);
        //                path.Add(temp);
        //                var listdata = String.Join(",", list);
        //            }

        //            result.Message = "Successful";
        //            result.Success = true;
        //            result.Paths = list;
        //            result.PathArray = path;
        //            //  result.URL = FileUrl;
        //            //result.extension = extension;
        //        }
        //        else
        //        {
        //            result.Message = "Error";
        //            result.Success = false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //logger.Error("Error In UploadFileAll : " + ex.Message);
        //        result.Message = "BackHandError : " + ex.Message;
        //        result.Success = false;
        //    }
        //    //    logger.Info("End in UploadFileAll");
        //    return result;
        //}
        ///// <summary>
        ///// Model Class
        ///// </summary>

        #endregion API for Upload multiple for upload invoice

        #region API for Upload time assignImages

        /// <summary>
        ///  29/11/21
        ///  API >> POST >> api/assetsnew/assignassetsimages
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("assignassetsimages")]
        public async Task<HttpResponseMessageMultiple> AssignAssetsImages()
        {
            // logger.Info("Start UploadFileAll");
            HttpResponseMessageMultiple result = new HttpResponseMessageMultiple();
            List<PathLists> list = new List<PathLists>();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
                //var identity = User.Identity as ClaimsIdentity;
                List<string> path = new List<string>();
                // int userid = 0;
                // Access claims

                //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                //    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    var content = provider.Contents.Count;

                    for (int i = 0; i < content; i++)
                    {
                        var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                        var filefromreq = provider.Contents[i];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        ////////////// Add By Mohit 12-07-2021
                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);

                        ////////////// Add By Mohit 12-07-2021
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Assetitem/AssetAssignImg/" + claims.companyId), dates + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        var temp = "uploadimage\\Assetitem\\AssetAssignImg\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        ////////////// old Code 12-07-2021

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        PathLists obj = new PathLists
                        {
                            Pathurl = temp,
                        };
                        list.Add(obj);
                        path.Add(temp);
                        var listdata = String.Join(",", list);
                    }

                    result.Message = "Successful";
                    result.Success = true;
                    result.Paths = list;
                    result.PathArray = path;
                    //  result.URL = FileUrl;
                    //result.extension = extension;
                }
                else
                {
                    result.Message = "Error";
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error In UploadFileAll : " + ex.Message);
                result.Message = "BackHandError : " + ex.Message;
                result.Success = false;
            }
            //    logger.Info("End in UploadFileAll");
            return result;
        }

        #endregion API for Upload time assignImages

        #region API for Upload time recoverImages

        /// <summary>
        ///  29/11/21
        ///  API >> POST >> api/assetsnew/recoverassetsimages
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("recoverassetsimages")]
        public async Task<HttpResponseMessageMultiple> RecoverAssetsImages()
        {
            // logger.Info("Start UploadFileAll");
            HttpResponseMessageMultiple result = new HttpResponseMessageMultiple();
            List<PathLists> list = new List<PathLists>();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
                //var identity = User.Identity as ClaimsIdentity;
                List<string> path = new List<string>();
                // int userid = 0;
                // Access claims

                //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                //    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    var content = provider.Contents.Count;

                    for (int i = 0; i < content; i++)
                    {
                        var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                        var filefromreq = provider.Contents[i];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        ////////////// Add By Mohit 12-07-2021
                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);

                        ////////////// Add By Mohit 12-07-2021
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Assetitem/AssetRecoverImg/" + claims.companyId), dates + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        var temp = "uploadimage\\Assetitem\\AssetRecoverImg\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        ////////////// old Code 12-07-2021

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        PathLists obj = new PathLists
                        {
                            Pathurl = temp,
                        };
                        list.Add(obj);
                        path.Add(temp);
                        var listdata = String.Join(",", list);
                    }

                    result.Message = "Successful";
                    result.Success = true;
                    result.Paths = list;
                    result.PathArray = path;
                    //  result.URL = FileUrl;
                    //result.extension = extension;
                }
                else
                {
                    result.Message = "Error";
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error In UploadFileAll : " + ex.Message);
                result.Message = "BackHandError : " + ex.Message;
                result.Success = false;
            }
            //    logger.Info("End in UploadFileAll");
            return result;
        }

        #endregion API for Upload time recoverImages

        #region Get item history by Id

        /// <summary>
        ///  Created by Suraj Bundel on 19/07/22
        ///  API >> POST >> api/assetsnew/itemhistorybyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("itemhistorybyid")]
        public async Task<ResponseBodyModel> ItemHistoryById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<HelperForAssetsHistory> assignassetLH = new List<HelperForAssetsHistory>();
            List<string> assignImg = new List<string>();
            List<string> recoverImg = new List<string>();
            try
            {
                var history = await (from ah in _db.AsstesAssignHistories
                                     join e in _db.Employee on ah.AssignToId equals e.EmployeeId
                                     join aim in _db.AssetsItemMasters on ah.ItemId equals aim.ItemId
                                     join it in _db.AssetsCategories on aim.AssetsCategoryId equals it.AssetsCategoryId
                                     //   join icon in db.AssetIcons on it.AssetsCategoryIconId equals icon.AssetIconId
                                     where ah.ItemId == id && ah.IsActive && !ah.IsDeleted

                                     select new
                                     {
                                         ah.AssignHistroyId,
                                         ah.ItemId,
                                         ah.AssignToId,
                                         ah.RecoverBy,
                                         ah.AssetCondition,
                                         ah.CompanyId,
                                         ah.OrgId,
                                         ah.CreatedBy,
                                         ah.UpdatedBy,
                                         ah.DeletedBy,
                                         ah.CreatedOn,
                                         ah.UpdatedOn,
                                         ah.DeletedOn,
                                         ah.IsActive,
                                         ah.IsDeleted,
                                         e.DisplayName,
                                         aim.ItemName,
                                         //icon.AssetIconUrl,
                                         assignImg = ah.AssignImage1 + "," + ah.AssignImage2 + "," + ah.AssignImage3 + "," + ah.AssignImage4 + "," + ah.AssignImage5,
                                         recoverimage = ah.RecoverImage1 + "," + ah.RecoverImage2 + "," + ah.RecoverImage3 + "," + ah.RecoverImage4 + "," + ah.RecoverImage5,
                                     }).ToListAsync();

                foreach (var item in history)
                {
                    HelperForAssetsHistory assets = new HelperForAssetsHistory();
                    var aimg = item.assignImg.Split(',').ToList();
                    aimg.RemoveAll(x => x == "");
                    var rimg = item.recoverimage.Split(',').ToList();
                    rimg.RemoveAll(x => x == "");

                    assets.Recoverimgs = rimg;
                    assets.Assignimgs = aimg;
                    assets.AssetCondition = item.AssetCondition;
                    assets.AssignHistroyId = item.AssignHistroyId;
                    assets.ItemId = item.ItemId;
                    assets.AssignToId = item.AssignToId;
                    assets.RecoverBy = item.RecoverBy;
                    assets.AssetCondition = item.AssetCondition;
                    assets.CompanyId = item.CompanyId;
                    assets.OrgId = item.OrgId;
                    assets.CreatedBy = item.CreatedBy;
                    assets.UpdatedBy = item.UpdatedBy;
                    assets.DeletedBy = item.DeletedBy;
                    assets.CreatedOn = item.CreatedOn;
                    assets.UpdatedOn = item.UpdatedOn;
                    assets.DeletedOn = item.DeletedOn;
                    assets.IsActive = item.IsActive;
                    assets.IsDeleted = item.IsDeleted;
                    assets.DisplayName = item.DisplayName;
                    assets.ItemName = item.ItemName;
                    // assets.AssetIconUrl = item.AssetIconUrl;
                    assignassetLH.Add(assets);
                }

                res.Message = "Success";
                res.Status = true;
                res.Data = assignassetLH;
            }
            catch
            {
                res.Message = "failed";
                res.Status = false;
            }
            return res;
        }

        private class HelperForAssetsHistory
        {
            public object Assignimgs { get; set; }
            public object Recoverimgs { get; set; }
            public int ItemId { get; set; }
            public int AssignHistroyId { get; set; }
            public int AssignToId { get; set; }
            public int? RecoverBy { get; set; }
            public string AssetCondition { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
            public int CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public int? DeletedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }
            public DateTime? DeletedOn { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public string DisplayName { get; set; }
            public string ItemName { get; set; }
            public string AssetIconUrl { get; set; }
        }

        #endregion Get item history by Id

        #region Get All Asset Category // dropdown

        /// <summary>
        /// Created By Shriya Malvi on 23-05-2022
        /// API >> Get >> api/assetsnew/getallassetsitemtype
        /// Dropdown using Enum for expense type category
        /// </summary>
        [HttpGet]
        [Route("getallassetsitemtype")]
        public ResponseBodyModel GetAllAssetsItemType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetsItemType = Enum.GetValues(typeof(AssetsItemType))
                    .Cast<AssetsItemType>()
                    .Select(x => new AssetTypeList
                    {
                        AssetsTypeId = (int)x,
                        AssetsType = Enum.GetName(typeof(AssetsItemType), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Assets Item Type";
                res.Status = true;
                res.Data = assetsItemType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class AssetTypeList
        {
            public int AssetsTypeId { get; set; }
            public string AssetsType { get; set; }
        }

        #endregion Get All Expense Category // dropdown

        #region Api To Get All Category Behalf on assets type

        /// <summary>
        /// Created By Shriya Malvi On 05-08-2022
        /// API >> Get >> api/assetsnew/getassetscatebehalfontype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getassetscatebehalfontype")]
        public async Task<ResponseBodyModel> GetAssetsCateBehalfOnType(AssetsItemType assetsTypeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var AssetsCategory = await (from s in _db.AssetsBaseCategories
                                            where s.IsActive && !s.IsDeleted &&
                                            s.AssetsType == assetsTypeId && s.CompanyId == claims.companyId
                                            select new
                                            {
                                                s.AssetsBCategoryId,
                                                s.AssetsBCategoryName,
                                            }).ToListAsync();

                if (AssetsCategory.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Base Category List Found Behalf On Assets type";
                    res.Data = AssetsCategory;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Base Category List Not Found";
                    res.Data = AssetsCategory;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Category Behalf on assets type

        #region API for Upload multiple for Create Assets Icon Url

        /// <summary>
        ///  Created  By Shriya Malvi On 16-08-2022
        ///  API >> POST >> api/assetsnew/createassetsiconurl
        /// </summary>
        [HttpPost]
        [Route("createassetsiconurl")]
        public async Task<UploadImageResponse> CreateAssetsIconUrl()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
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
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Assetitem/AssetsIconUrl/" + claims.companyId), dates + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }

                        string temp = "uploadimage\\Assetitem\\AssetsIconUrl\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = temp;
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

        #endregion API for Upload multiple for Create Assets Icon Url

        #region Get All Assets List by Assets Type

        /// <summary>
        /// Created By Shriya Malvi on 17-08-2022
        /// API >> GET >> api/assetsnew/getallassetsbyassetstype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallassetsbyassetstype")]
        public async Task<ResponseBodyModel> GetAllAssetsByAssetsType(AssetsItemType AssetsTypeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<GetHelperGetAllAassets> AssignAssetDataList = new List<GetHelperGetAllAassets>();
            try
            {
                var assetsList = await (from ass in _db.AssetsItemMasters
                                        join it in _db.AssetsCategories on ass.AssetsCategoryId equals it.AssetsCategoryId
                                        where !ass.IsDeleted && ass.IsActive && ass.CompanyId == claims.companyId && ass.AssetStatus == Model.EnumClass.AssetStatusConstants.Available && !ass.Assigned && ass.AssetsType == AssetsTypeId &&
                                        (ass.AssetCondition == AssetConditionConstants.Good || ass.AssetCondition == AssetConditionConstants.Fair)
                                        //&& it.EmployeeIdifAssigned != 0
                                        select new GetHelperGetAllAassets
                                        {
                                            ItemId = ass.ItemId,
                                            ItemName = ass.ItemName,
                                            AssetsBaseCategoryId = ass.AssetsBaseCategoryId,
                                            AssetsBaseCategoryName = ass.AssetsBaseCategoryName,
                                            AssetsCategoryId = ass.AssetsCategoryId,
                                            AssetsCategoryName = ass.AssetsCategoryName,
                                            WareHouseId = ass.WareHouseId,
                                            WareHouseName = ass.WareHouseName,
                                            SerialNo = ass.SerialNo,
                                            ItemCode = ass.ItemCode,
                                            AssetCondition = ass.AssetCondition.ToString(),
                                            AssetStatus = ass.AssetStatus.ToString(),
                                            Location = ass.Location,
                                            ColorCode = it.ColorCode,
                                            AssetsIconId = it.AssetsCategoryIconId,
                                            IsAssetsIcon = it.IsAssetsIcon,
                                            AssetIconImgUrl = it.AssetIconImgUrl
                                        }).ToListAsync();

                if (assetsList.Count > 0)
                {
                    res.Data = assetsList;
                    res.Message = "Assets List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets List Not Found";
                    res.Status = false;
                    res.Data = assetsList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Assets List by Assets Type

        #region All assets Recoverable Detail

        /// <summary>
        /// Created by Shriya Malvi On 17-08-2022
        ///Api>>Get  >> api/assetsnew/allrecoverassetsdetail
        /// </summary>
        [HttpGet]
        [Route("allrecoverassetsdetail")]
        public async Task<ResponseBodyModel> AllRecoverAssetsDetail()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var recoverAssets = await (from ac in _db.AssetsItemMasters
                                           join bc in _db.AsstesAssignHistories on ac.ItemId equals bc.ItemId
                                           where ac.CompanyId == claims.companyId && ac.IsActive && !ac.IsDeleted && bc.RecoverBy > 0
                                           select new
                                           {
                                               ac.ItemId,
                                               ac.ItemName,
                                               bc.AssignToId,
                                               bc.CreatedOn,
                                               bc.RecoverBy,
                                               bc.DeletedOn,
                                           }).ToListAsync();
                if (recoverAssets.Count > 0)
                {
                    res.Data = recoverAssets;
                    res.Status = true;
                    res.Message = "Recover Assets List Found";
                }
                else
                {
                    res.Data = recoverAssets;
                    res.Status = false;
                    res.Message = "Recover Assets List Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion All assets Recoverable Detail

        #region Get All PurchceDetail With In Dates //fillter purchase betwen 2 dates

        /// <summary>
        /// Created By Shriya Malvi on 17-08-2022
        /// API >> Get >> api/assetsnew/getdatabydate
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [HttpGet]
        [Route("getdatabydate")]
        public async Task<ResponseBodyModel> GetAllPurchceDetailWithInDates(DateTime? start, DateTime? end, string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                var getLicenseExpiry = getData.Where(x => x.AssetsType == AssetsItemType.Digital &&
                        x.LicenseExpiryDate.Value.Date >= start.Value.Date && x.LicenseExpiryDate.Value.Date <= end).ToList();

                if (getData.Count > 0)
                {
                    res.Message = "Purches Assets List Found";
                    res.Status = true;
                    res.Data = getLicenseExpiry;
                }
                else
                {
                    res.Message = " Assets List Found";
                    res.Status = false;
                    res.Data = getData;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All PurchceDetail With In Dates //fillter purchase betwen 2 dates

        #region Get assets report behalf on assets warenty exppairy date

        /// <summary>
        ///  Created By Shriya Malvi on 18-08-2022
        /// API >> Get >> api/assetsnew/getassetsexpiredlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getassetsexpiredlist")]
        public async Task<ResponseBodyModel> GetAssetsExpiredList(int type)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dataexp = await _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                DateTime TodayDate = DateTime.Now.Date;
                DateTime AfterTenDaysDate = DateTime.Today.AddDays(10);
                DateTime lastTenDayDate = DateTime.Today.AddDays(-10);
                if (type == 1)
                {
                    // exp till now
                    var ExpList = dataexp.Where(x => x.LicenseExpiryDate <= TodayDate).Select(x => new HelperForExpiryData
                    {
                        AssetsId = x.ItemId,
                        AssetsName = x.ItemName,
                        ItemCode = x.ItemCode,
                        SerialNo = x.SerialNo,
                        Location = x.Location,
                        InvoiceNo = x.InvoiceNo,
                        Price = x.Price,
                        WarentyExpDate = x.WarentyExpDate,
                        LicenseKey = x.LicenseKey,
                        LicenseStartDate = x.LicenseStartDate,
                        LicenseExpiryDate = x.LicenseExpiryDate,
                        AssetsType = x.AssetsType
                    }).OrderByDescending(x => x.LicenseExpiryDate).ToList();

                    res.Message = "Total expire list Till Now";
                    res.Status = true;
                    res.Data = ExpList;
                }
                else if (type == 3)
                {
                    var ExpSoon = dataexp.Where(x => x.LicenseExpiryDate <= AfterTenDaysDate &&
                    x.LicenseExpiryDate >= TodayDate).Select(x => new HelperForExpiryData
                    {
                        AssetsId = x.ItemId,
                        AssetsName = x.ItemName,
                        ItemCode = x.ItemCode,
                        SerialNo = x.SerialNo,
                        Location = x.Location,
                        InvoiceNo = x.InvoiceNo,
                        Price = x.Price,
                        WarentyExpDate = x.WarentyExpDate,
                        LicenseKey = x.LicenseKey,
                        LicenseStartDate = x.LicenseStartDate,
                        LicenseExpiryDate = x.LicenseExpiryDate,
                        AssetsType = x.AssetsType
                    }).ToList();

                    res.Message = " license expire in next ten days ";
                    res.Status = true;
                    res.Data = ExpSoon;
                }
                else if (type == 2)
                {
                    var ExpList = dataexp.Where(x => x.WarentyExpDate >= TodayDate).Select(x => new HelperForExpiryData
                    {
                        AssetsId = x.ItemId,
                        AssetsName = x.ItemName,
                        ItemCode = x.ItemCode,
                        SerialNo = x.SerialNo,
                        Location = x.Location,
                        InvoiceNo = x.InvoiceNo,
                        Price = x.Price,
                        WarentyExpDate = x.WarentyExpDate,
                        LicenseKey = x.LicenseKey,
                        LicenseStartDate = x.LicenseStartDate,
                        LicenseExpiryDate = x.LicenseExpiryDate,
                        AssetsType = x.AssetsType
                    }).ToList();

                    res.Message = "Total expire assets";
                    res.Status = true;
                    res.Data = ExpList;
                }
                else
                {
                    res.Message = "list not found";
                    res.Status = true;
                    res.Data = new List<int>();
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get assets report behalf on assets warenty exppairy date

        #region get assets expiry dashboard

        /// <summary>
        /// Created By Shriya Malvi On 18-08-2022
        /// API >> Get >> api/assetsnew/getassetsexpirydashboard
        /// Its Used for license expired in last 30 days and license expire in nexy 30 days
        /// </summary>
        [HttpGet]
        [Route("getassetsexpirydashboard")]
        public async Task<ResponseBodyModel> GetAssetsExpiryDashboard()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DashboardExpiryAssets ExpAssetsData = new DashboardExpiryAssets();
            DateTime? TodayDate = DateTime.Now.Date;
            DateTime AfterTenDaysDate = DateTime.Today.AddDays(10);
            DateTime lastTenDaysDate = DateTime.Today.AddDays(-10);
            List<AssetPichart> AssetPiChart = new List<AssetPichart>();
            List<AssetsLineChart> AssetsLineChart = new List<AssetsLineChart>();
            List<AssetPichart> AssetLine = new List<AssetPichart>();
            try
            {
                var dataexp = await _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted /*&& x.CompanyId == claims.companyId*/).ToListAsync();

                ExpAssetsData.TotalLicenseExpireCount = dataexp.Count(x => x.LicenseExpiryDate <= TodayDate);
                ExpAssetsData.NextTenDaysExpireCount = dataexp.Count(x => x.LicenseExpiryDate <= AfterTenDaysDate && x.LicenseExpiryDate >= TodayDate);
                ExpAssetsData.TotalAssetsExpireCount = dataexp.Where(x => x.WarentyExpDate.HasValue).Count(x => x.WarentyExpDate <= TodayDate);

                #region Pie Chart Data

                var assetCategoryIds = await _db.AssetsBaseCategories.Where(x => x.CompanyId == claims.companyId && !x.IsDeleted && x.IsActive).ToListAsync();

                ExpAssetsData.Series = new List<AssetPichart>
                {
                    new AssetPichart
                    {
                        Name = "Physical",
                        Value = dataexp.Count(x=> x.AssetsType == AssetsItemType.Physical && x.WarentyExpDate < DateTime.Today) ,
                       //Value = 0,
                    },
                    new AssetPichart
                    {
                        Name = "Digital",
                        Value = dataexp.Count(x=> x.AssetsType == AssetsItemType.Digital && x.LicenseExpiryDate< DateTime.Today),
                        //Value = 0,
                    }
                };

                #endregion Pie Chart Data

                #region Line Chart Data

                var physical = new List<dynamic>();
                var digital = new List<dynamic>();
                for (int i = 1; i <= 12; i++)
                {
                    AssetPichart line1 = new AssetPichart
                    {
                        Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        Value = dataexp.Count(x => x.AssetsType == AssetsItemType.Physical /*&& ((DateTime)x.WarentyExpDate).Month < i*/),
                    };
                    physical.Add(line1);
                    AssetPichart line2 = new AssetPichart
                    {
                        Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        Value = dataexp.Count(x => x.AssetsType == AssetsItemType.Digital /*&& ((DateTime)x.LicenseExpiryDate).Month < i*/),
                    };
                    digital.Add(line2);
                }
                ExpAssetsData.Multi = new List<dynamic>
                {
                    new
                    {
                        Name = "Physical Warrenty Expire Assets",
                        Series = physical,
                    },
                    new
                    {
                        Name = "Digital License Expire Asset",
                        Series = digital,
                    }
                };

                #endregion

                res.Message = "Expiry List found";
                res.Status = true;
                res.Data = ExpAssetsData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion get assets expiry dashboard

        #region fillter  for License expire between 2 dates

        /// <summary>
        /// Created By Shriya Malvi on 22-08-2022
        /// API >> Get >> api/assetsnew/getlicenseexpirebtwdates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [Route("getlicenseexpirebtwdates")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetLicenseExpireBtwDates(DateTime? start, DateTime? end)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                var getLicenseExpiry = getData.Where(x => x.AssetsType == AssetsItemType.Digital &&
                ((DateTime)x.LicenseExpiryDate).Date >= start.Value.Date && x.LicenseExpiryDate.Value.Date <= end).Select(x => new HelperForExpiryData
                {
                    AssetsId = x.ItemId,
                    AssetsName = x.ItemName,
                    ItemCode = x.ItemCode,
                    SerialNo = x.SerialNo,
                    Location = x.Location,
                    InvoiceNo = x.InvoiceNo,
                    Price = x.Price,
                    WarentyExpDate = x.WarentyExpDate,
                    LicenseKey = x.LicenseKey,
                    LicenseStartDate = x.LicenseStartDate,
                    LicenseExpiryDate = x.LicenseExpiryDate,
                    AssetsType = x.AssetsType
                }).OrderByDescending(x => x.LicenseExpiryDate).ToList();

                if (getLicenseExpiry.Count > 0)
                {
                    res.Message = "License Expire Assets List Found";
                    res.Status = true;
                    res.Data = getLicenseExpiry;
                }
                else
                {
                    res.Message = "License Expire Assets Not Found";
                    res.Status = false;
                    res.Data = getLicenseExpiry;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion fillter  for License expire between 2 dates

        #region Renew Assets Api

        /// <summary>
        /// Create by Harshit Mitra on 23-08-2022
        /// API >> Put >> api/assetsnew/renewasset
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("renewasset")]
        public async Task<ResponseBodyModel> RenewAsset(HelperForRenewAssets model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    var asset = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == model.ItemId &&
                            x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                    if (asset != null)
                    {
                        if (model.AssetsType == AssetsItemType.Physical)
                        {
                            if (asset.WarentyExpDate < DateTime.Now)
                            {
                                asset.WarentyExpDate = model.WarentyExpDate;
                                asset.UpdatedBy = claims.employeeId;
                                asset.UpdatedOn = DateTime.Now;

                                _db.Entry(asset).State = System.Data.Entity.EntityState.Modified;
                                await _db.SaveChangesAsync();
                                res.Message = "Warrenty Extended";
                                res.Status = true;
                            }
                            else
                            {
                                res.Message = "You Cann't Extend Warrenty Date After Warrenty Expiry";
                                res.Status = false;
                            }
                        }
                        else if (model.AssetsType == AssetsItemType.Digital)
                        {
                            asset.LicenseStartDate = model.LicenseStartDate;
                            asset.LicenseExpiryDate = model.LicenseExpiryDate;
                            asset.LicenseKey = model.LicenseKey;
                            asset.UpdatedBy = claims.employeeId;
                            asset.UpdatedOn = DateTime.Now;

                            _db.Entry(asset).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                            res.Message = "License Renewed";
                            res.Status = true;
                        }
                        else
                        {
                            res.Message = "Wrong Asset Type Send";
                            res.Status = false;
                        }
                        var updatehistory = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.ItemId == model.ItemId);

                        AssetsHistory history = new AssetsHistory();
                        {
                            history.RecoverImage1 = updatehistory.RecoverImage1;
                            history.RecoverImage2 = updatehistory.RecoverImage2;
                            history.RecoverImage3 = updatehistory.RecoverImage3;
                            history.RecoverImage4 = updatehistory.RecoverImage4;
                            history.RecoverImage5 = updatehistory.RecoverImage5;
                            history.AssignImage1 = updatehistory.AssignImage1;
                            history.AssignImage2 = updatehistory.AssignImage2;
                            history.AssignImage3 = updatehistory.AssignImage3;
                            history.AssignImage4 = updatehistory.AssignImage4;
                            history.AssignImage5 = updatehistory.AssignImage5;
                            history.ItemId = updatehistory.ItemId;
                            history.ItemName = updatehistory.ItemName;
                            history.AssetsBaseCategoryId = updatehistory.AssetsBaseCategoryId;
                            history.AssetsCategoryId = updatehistory.AssetsCategoryId;
                            history.WareHouseId = updatehistory.WareHouseId;
                            history.ItemCode = updatehistory.ItemCode;
                            history.SerialNo = updatehistory.SerialNo;
                            history.Location = updatehistory.Location;
                            history.InvoiceUrl = updatehistory.InvoiceUrl;
                            history.PurchaseDate = updatehistory.PurchaseDate;
                            history.AssignToId = updatehistory.AssignToId;
                            history.AssignById = updatehistory.AssignById;
                            history.RecoverById = updatehistory.RecoverById;
                            history.AssetCondition = updatehistory.AssetCondition;
                            history.AssetStatus = updatehistory.AssetStatus;
                            history.AssetsDescription = updatehistory.AssetsDescription;
                            history.ReasonNotAvailable = updatehistory.ReasonNotAvailable;
                            history.AvailablityStatus = updatehistory.AvailablityStatus;
                            history.Recovered = updatehistory.Recovered;
                            history.Assigned = updatehistory.Assigned;
                            history.AssignDate = updatehistory.AssignDate;
                            history.RecoverDate = updatehistory.RecoverDate;
                            history.Comment = updatehistory.Comment;
                            history.IsRefurbish = updatehistory.IsRefurbish;
                            history.RefurbishCount = updatehistory.RefurbishCount;
                            history.InvoiceNo = updatehistory.InvoiceNo;
                            history.Price = updatehistory.Price;
                            history.WarentyExpDate = updatehistory.WarentyExpDate;
                            history.Compliance = updatehistory.Compliance;
                            history.IsCompliance = updatehistory.IsCompliance;
                            history.LicenseKey = updatehistory.LicenseKey;
                            history.LicenseStartDate = updatehistory.LicenseStartDate;
                            history.LicenseExpiryDate = updatehistory.LicenseExpiryDate;
                            history.LicApplicableCount = updatehistory.LicApplicableCount;
                            history.AssetsType = updatehistory.AssetsType;
                            history.UpImg1 = updatehistory.UpImg1;
                            history.UpImg2 = updatehistory.UpImg2;
                            history.UpImg3 = updatehistory.UpImg3;
                            history.UpImg4 = updatehistory.UpImg4;
                            history.UpImg5 = updatehistory.UpImg5;
                            history.UpImg6 = updatehistory.UpImg6;
                            history.UpImg7 = updatehistory.UpImg7;
                            history.UpImg8 = updatehistory.UpImg8;
                            history.UpImg9 = updatehistory.UpImg9;
                            history.UpImg10 = updatehistory.UpImg10;
                            history.IsActive = updatehistory.IsActive;
                            history.IsDeleted = updatehistory.IsDeleted;
                            history.CompanyId = updatehistory.CompanyId;
                            history.OrgId = updatehistory.OrgId;
                            history.CreatedBy = updatehistory.CreatedBy;
                            history.UpdatedBy = updatehistory.UpdatedBy;
                            history.DeletedBy = updatehistory.DeletedBy;
                            history.CreatedOn = updatehistory.CreatedOn;
                            history.UpdatedOn = updatehistory.UpdatedOn;
                            history.DeletedOn = updatehistory.DeletedOn;
                            _db.AssetsHistories.Add(history);
                            await _db.SaveChangesAsync();
                        }

                    }
                    else
                    {
                        res.Message = "Asset Not Found";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public class HelperForRenewAssets
        {
            public int ItemId { get; set; }
            public AssetsItemType AssetsType { get; set; }

            //public DateTime RenewalDate { get; set; }
            public DateTime? LicenseStartDate { get; set; }

            public DateTime? LicenseExpiryDate { get; set; }
            public string LicenseKey { get; set; }
            public DateTime? WarentyExpDate { get; set; }
        }

        #endregion Renew Assets Api

        #region Helper  classes
        public class AddAssetsItemMasterHelper
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int AssetsBaseCategoryId { get; set; }
            public int AssetsCategoryId { get; set; }
            public int WareHouseId { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public string InvoiceUrl { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public AssetConditionConstants AssetCondition { get; set; }
            public string AssetsDescription { get; set; }
            public string ReasonNotAvailable { get; set; }
            public string UpImg1 { get; set; }     //add multiple  image while create item
            public string UpImg2 { get; set; }
            public string UpImg3 { get; set; }
            public string UpImg4 { get; set; }
            public string UpImg5 { get; set; }
            public string UpImg6 { get; set; }
            public string UpImg7 { get; set; }
            public string UpImg8 { get; set; }
            public string UpImg9 { get; set; }
            public string UpImg10 { get; set; }
            public string InvoiceNo { get; set; }
            public double Price { get; set; }
            public DateTime? WarentyExpDate { get; set; }
            public List<int> Compliance { get; set; }

            //public bool IsCompliance { get; set; }
            public bool IsCompliance { get; set; }

            public string LicenseKey { get; set; }
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int LicApplicableCount { get; set; }
            public AssetsItemType AssetsType { get; set; }

            public int AssignToId { get; set; }
        }

        // Create by Shriya Malvi on 11-07-2022
        public class HelperForAssetsDd
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // Create by Shriya Malvi on 12-07-2022
        public class HelperForAssign
        {

            public int ItemId { get; set; }
            public int AssignToId { get; set; }
            public int AssignById { get; set; }
            public AssetConditionConstants ConditionId { get; set; }
            public string Comment { get; set; }
            public string AssignImage1 { get; set; }
            public string AssignImage2 { get; set; }
            public string AssignImage3 { get; set; }
            public string AssignImage4 { get; set; }
            public string AssignImage5 { get; set; }
        }

        public class Addassetmodel
        {
            public int BasecategoryId { get; set; }
            public int CategoryId { get; set; }
            public int WarehouseId { get; set; }
        }

        public class CategoryModel
        {
            public int AssetsCategoryId { get; set; }
            public int AssetsBCategoryId { get; set; }
            public string AssetsBCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public int AssetsIconId { get; set; }
            public string ColorCode { get; set; }

            //   public string AssetsCategoryIconName { get; set; }
            public string Description { get; set; }
        }

        public class getallCategoryModel
        {
            public int AssetsCategoryId { get; set; }
            public int AssetsBCategoryId { get; set; }
            public string AssetsBCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public int AssetsIconId { get; set; }
            public string AssetsCategoryIconName { get; set; }
            public string ColorCode { get; set; }
            public string AssetIconImgUrl { get; set; }
            public string Description { get; set; }
            public bool IsAssetsIcon { get; set; }
            public int Count { get; set; }
        }

        public class DistingByAssest
        {
            public int AssignedToId { get; set; }
            public string DisplayName { get; set; }
            public string DepartmentName { get; set; }
            public int ItemId { get; set; }
            public string AssetsKey { get; set; }
            public string Condition { get; set; }
            public int WarehouseId { get; set; }
            public long AssignId { get; set; }
            public int IconId { get; set; }
            public string IconUrl { get; set; }
            public string ColorCode { get; set; }
        }

        public class Assetdatamodel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string Model { get; set; }
            public string Serialnumber { get; set; }
            public string AssetsKey { get; set; }
            public string Condition { get; set; }
            public int WarehouseId { get; set; }
            public string EmployeeName { get; set; }
            public long AssignId { get; set; }
            public string IconUrl { get; set; }
            public string ColorCode { get; set; }
            public int IconId { get; set; }
            public double Price { get; set; }
            public bool IsAssetsIcon { get; set; }
            public string AssetIconImgUrl { get; set; }
            public AssetsItemType AssetType { get; set; }
        }

        public class Assetlist
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Department { get; set; }
            public double TotalAmount { get; set; }
            public double TotalDigitalAssetsAmount { get; set; }
            public double TotalPhysicalAssetsAmount { get; set; }

            public List<Assetdatamodel> PhysicalAssesdataList { get; set; }
            public List<Assetdatamodel> DigitalAssesdataList { get; set; }
        }

        public class HelperForRecoverAsset
        {
            public int ItemId { get; set; }
            public string SerialNo { get; set; }
            public AssetConditionConstants ConditionId { get; set; }
            public string RecoverImage1 { get; set; }
            public string RecoverImage2 { get; set; }
            public string RecoverImage3 { get; set; }
            public string RecoverImage4 { get; set; }
            public string RecoverImage5 { get; set; }
        }

        public class HelperForUpdateCondition
        {
            public int ItemId { get; set; }
            public AssetConditionConstants ConditionId { get; set; }
            public int AssignToId { get; set; }
            public string RecoverImage1 { get; set; }
            public string RecoverImage2 { get; set; }
            public string RecoverImage3 { get; set; }
            public string RecoverImage4 { get; set; }
            public string RecoverImage5 { get; set; }
        }

        public class HelperMarkasnotAvailable
        {
            public int ItemId { get; set; }
            public string ReasonNotAvailable { get; set; }
            //  public string AssetStatus { get; set; }
        }

        public class HelperDamageAssetCondtion
        {
            public int ItemId { get; set; }
            public int ConditionId { get; set; }
        }

        // Create by Suraj Bundel on 14-07-2022
        public class Helperdamageassets
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string ItemCode { get; set; }
            public int AssignToId { get; set; }
            public string AssignedToName { get; set; }
            public string AssetStatus { get; set; }

            public int RecoverById { get; set; }
            public string AssetCondition { get; set; }
        }

        public class UnderRepairassets
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int AssignToId { get; set; }
            public string AssignedToName { get; set; }
            public string AssetStatus { get; set; }
            public int RecoverById { get; set; }
            public string AssetCondition { get; set; }
        }

        public class AssignAssetsData
        {
            public long ItemId { get; set; }
            public string ItemName { get; set; }
            public int WarehouseId { get; set; }
            public string Condition { get; set; }
            public int AssignedToId { get; set; }
            public string AssignedTo { get; set; }
            public string ItemCode { get; set; }
            public int? AssetRecoveredById { get; set; }
            public string AssetRecoveredBy { get; set; }
            public string Comment { get; set; }
            public DateTime? AssignDate { get; set; }

            public string ColorCode { get; set; }
            public int AssetsIconId { get; set; }
            public bool IsAssetsIcon { get; set; }
            public string AssetIconImgUrl { get; set; }
        }

        public class GetAssetByWarehouseModel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int AssetsBaseCategoryId { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public int AssetsCategoryId { get; set; }
            public string AssetsCategoryName { get; set; }
            public int WareHouseId { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public int AssignToId { get; set; }
            public string AssignedToName { get; set; }
            public int RecoverById { get; set; }
            public string RecoverByName { get; set; }
            public string AssetCondition { get; set; }
            public string AssetStatus { get; set; }
            public string AssetsDescription { get; set; }
            public bool AvailablityStatus { get; set; }
            public string Comment { get; set; }
            public string iconurl { get; set; }
            public int totalcount { get; set; }
            public int? AssetsIconId { get; set; }
            public string ColorCode { get; set; }
            public bool IsAssetsIcon { get; set; }
            public string AssetIconImgUrl { get; set; }
        }

        public class HelperForPieChart
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
        }

        //public class DashboardAsset
        //{
        //    public int GetAllAsset { get; set; }
        //    public int GetAllAssignAsset { get; set; }
        //    public int GetAllFaultyAssets { get; set; }
        //    public double AvailableAssets { get; set; }
        //    public int TotalPhysicalAssets { get; set; }
        //    public int TotalDamageAssets { get; set; }
        //    public int TotalPhysicalAssigned { get; set; }
        //    public int DamageAssets { get; set; }
        //    public double AllAssetAmount { get; set; }
        //    public int TotalDigitalAssets { get; set; }
        //    public int TotalPhysicalAvailableAssets { get; set; }
        //    public int TotalDigitalAvailableAssets { get; set; }
        //    public int TotalDigitalAssigned { get; set; }
        //    public List<AssetBarModel> ChartAssets { get; set; }
        //    public List<AssetPichart> Series { get; set; }
        //}

        //public class AssetBarModel
        //{
        //    public string Name { get; set; }
        //    public List<Condition> Series { get; set; }
        //}

        public class Condition
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }




        public class GetAllWarehouseModule
        {
            public int WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public string WarehouseAddress { get; set; }
            public string WarehouseDescription { get; set; }
            public int TotalItems { get; set; } = 0;
        }

        public class HttpResponseMessageMultiple
        {
            public string Message { get; set; } = String.Empty;
            public bool Success { get; set; } = false;
            public List<PathLists> Paths { get; set; } = new List<PathLists>();
            public List<string> PathArray { get; set; } = new List<string>();
            public List<string> ExtensionList { get; set; } = new List<string>();
        }

        public class PathLists
        {
            public string Pathurl { get; set; }
        }

        public class HelperGetAssetListById
        {
            public int MyProperty { get; set; }
        }

        public class GetHelperGetAllAassets
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int AssetsBaseCategoryId { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public int AssetsCategoryId { get; set; }
            public string AssetsCategoryName { get; set; }
            public int WareHouseId { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public string AssetCondition { get; set; }
            public string AssetStatus { get; set; }
            public string ColorCode { get; set; }
            public int AssetsIconId { get; set; }
            public bool IsAssetsIcon { get; set; }
            public string AssetIconImgUrl { get; set; }
            public AssetsItemType Assetstype { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public int AssignToId { get; set; }
            public string AssignedToName { get; set; }
            public double Price { get; set; }
            public DateTime? WarentyExpDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int? LicApplicableCount { get; set; }
            public string AssetsTypeName { get; set; }
            public string Comment { get; set; }
            public string AssetsDescription { get; set; }
            public string InvoiceNo { get; set; }
            public string LicenseKey { get; set; }
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? RecoverDate { get; set; }
            public string Compliance { get; set; }
            public string Officeemail { get; set; }
        }

        public class DashboardExpiryAssets
        {
            public int TotalLicenseExpireCount { get; set; }
            public int TotalAssetsExpireCount { get; set; }
            public int NextTenDaysExpireCount { get; set; }
            public object Series { get; set; }
            public object Multi { get; set; }
        }

        public class AssetsLineChart
        {
            public string Name { get; set; }
            public List<AssetPichart> Series { get; set; }
        }

        public class HelperForExpiryData
        {
            public int AssetsId { get; set; }
            public string AssetsName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public string InvoiceNo { get; set; }
            public double Price { get; set; }
            public DateTime? WarentyExpDate { get; set; }
            public string Compliance { get; set; }
            public bool IsCompliance { get; set; }
            public string LicenseKey { get; set; }
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int? LicApplicableCount { get; set; }
            public AssetsItemType AssetsType { get; set; }
        }

        #endregion Helper  classes

        #region get list of assets

        /// <summary>
        ///  Created by Suraj Bundel on 22/08/2022
        ///  API >> POST >> api/assetsnew/getithraproval
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("getemployeeasssetbyname")]
        public async Task<ResponseBodyModel> GetEmployeeAssetbyId(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var approval = await (from am in _db.AssetsItemMasters
                                      join e in _db.Employee on am.AssignToId equals e.EmployeeId
                                      join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                      where (am.IsActive && !am.IsDeleted && e.CompanyId == claims.companyId &&
                                      e.DisplayName.StartsWith(name) || am.ItemName.StartsWith(name) || d.DepartmentName.StartsWith(name))
                                      select new
                                      {
                                          am.ItemId,
                                          am.ItemName,
                                          am.SerialNo,
                                          am.ItemCode,
                                          e.DisplayName,
                                          am.AssignToId,
                                          am.AssignedToName,
                                          d.DepartmentName,
                                          am.AssetCondition,
                                          am.WareHouseId,
                                          am.AssetsCategoryId,
                                          am.AssignDate,
                                      }).ToListAsync();

                if (approval.Count > 0)
                {
                    res.Message = "List Found";
                    res.Status = true;
                    res.Data = approval;
                }
                else
                {
                    res.Message = "No List Found";
                    res.Status = false;
                    res.Data = approval;
                }
            }
            catch
            {
                res.Message = "failed";
                res.Status = false;
            }
            return res;
        }

        #endregion get list of assets

        #region All Search Api's


        #region API to searh data from the assigned assets 
        /// <summary>
        /// API > GET > api/assetsnew/searchassetsfromassignedassets
        /// Created By Bhavendra Singh Jat 28-08-2022
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("searchassetsfromassignedassets")]

        public async Task<ResponseBodyModel> GetAllAssignedAssetsSearch(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<Assetlist> list = new List<Assetlist>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var assignedAssetsSearchData = await (from aim in _db.AssetsItemMasters
                                                      join ac in _db.AssetsCategories on aim.AssetsCategoryId equals ac.AssetsCategoryId
                                                      join empdata in _db.Employee on aim.AssignToId equals empdata.EmployeeId
                                                      join dp in _db.Department on empdata.DepartmentId equals dp.DepartmentId
                                                      where !aim.IsDeleted && aim.IsActive && aim.CompanyId == claims.companyId && aim.AssetStatus == AssetStatusConstants.Assigned &&
                                                      (aim.AssetCondition == AssetConditionConstants.Good || aim.AssetCondition == AssetConditionConstants.Fair)
                                                      && (aim.ItemName.ToLower().Contains(search.ToLower()) || aim.AssignedToName.ToLower().Contains(search.ToLower()) || aim.ItemName.ToLower().Contains(search.ToLower()) ||
                                                      aim.ItemCode.ToLower().Contains(search.ToLower())
                                                      )
                                                      select new
                                                      {
                                                          aim.ItemId,
                                                          aim.ItemName,
                                                          aim.SerialNo,
                                                          aim.ItemCode,
                                                          empdata.DisplayName,
                                                          aim.AssignToId,
                                                          aim.AssignedToName,
                                                          Department = dp.DepartmentName,
                                                          aim.AssetCondition,
                                                          aim.WareHouseId,
                                                          aim.AssetsCategoryId,
                                                          aim.AssignDate,
                                                          assign = aim.AssignDate ?? aim.CreatedOn,
                                                          ac.ColorCode,
                                                          ac.AssetsCategoryIconId,
                                                          aim.Price,
                                                          ac.IsAssetsIcon,
                                                          ac.AssetIconImgUrl,
                                                          aim.AssetsType
                                                      }).ToListAsync();
                var employeeIds = assignedAssetsSearchData.ConvertAll(x => new DistingByAssest
                {
                    AssignedToId = x.AssignToId,
                    DisplayName = x.DisplayName,
                    DepartmentName = x.Department,
                    ItemId = x.ItemId,
                    Condition = x.AssetCondition.ToString(),
                    WarehouseId = x.WareHouseId,
                    IconId = x.AssetsCategoryIconId,
                    ColorCode = x.ColorCode
                });
                foreach (int item in employeeIds.Select(x => x.AssignedToId).Distinct().ToList())
                {
                    var data = employeeIds.Find(x => x.AssignedToId == item);
                    Assetlist obj = new Assetlist
                    {
                        EmployeeId = data.AssignedToId,
                        EmployeeName = data.DisplayName,
                        Department = data.DepartmentName,
                        TotalAmount = assignedAssetsSearchData.Where(x => x.AssignToId == data.AssignedToId).Select(x => x.Price).Sum(),

                        PhysicalAssesdataList = assignedAssetsSearchData.Where(x => x.AssignToId == data.AssignedToId && x.AssetsType == AssetsItemType.Physical)
                                .Select(x => new Assetdatamodel
                                {
                                    EmployeeName = x.DisplayName,
                                    ItemId = x.ItemId,
                                    ItemName = x.ItemName,
                                    Model = x.ItemCode,
                                    Serialnumber = x.SerialNo,
                                    Condition = x.AssetCondition.ToString(),
                                    Price = x.Price,
                                    WarehouseId = x.WareHouseId,
                                    AssignId = x.AssignToId,
                                    IconId = x.AssetsCategoryIconId,
                                    ColorCode = x.ColorCode,
                                    AssetIconImgUrl = x.AssetIconImgUrl,
                                    AssetType = x.AssetsType,
                                    IsAssetsIcon = x.IsAssetsIcon
                                }).ToList(),


                        DigitalAssesdataList = assignedAssetsSearchData.Where(x => x.AssignToId == data.AssignedToId && x.AssetsType == AssetsItemType.Digital)
                                .Select(x => new Assetdatamodel
                                {
                                    EmployeeName = x.DisplayName,
                                    ItemId = x.ItemId,
                                    ItemName = x.ItemName,
                                    Model = x.ItemCode,
                                    Serialnumber = x.SerialNo,
                                    Condition = x.AssetCondition.ToString(),
                                    Price = x.Price,
                                    WarehouseId = x.WareHouseId,
                                    AssignId = x.AssignToId,
                                    IconId = x.AssetsCategoryIconId,
                                    ColorCode = x.ColorCode,
                                    AssetIconImgUrl = x.AssetIconImgUrl,
                                    AssetType = x.AssetsType,
                                    IsAssetsIcon = x.IsAssetsIcon
                                }).ToList(),
                    };
                    list.Add(obj);
                }

                if (list.Count != 0)
                {
                    res.Message = "Department list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = list.Count,
                            Counts = (int)count,
                            List = list.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = list;
                    }
                }
                else
                {
                    res.Message = "No Item Is Assigned";
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

        #endregion 

        #region API for search from the all assetes data 
        /// <summary>
        /// API > GET > api/assetsnew/searchassetsdatafromallassetslist
        /// Created By Bhavendra Singh Jat 28-08-2022
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("searchassetsdatafromallassetslist")]
        public async Task<ResponseBodyModel> GetAllAssetsSearch(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var assetDataSearch = await (from a in _db.AssetsItemMasters
                                             join b in _db.AssetsBaseCategories on a.AssetsBaseCategoryId equals b.AssetsBCategoryId
                                             join c in _db.AssetsCategories on a.AssetsCategoryId equals c.AssetsCategoryId
                                             join w in _db.AssetsWarehouses on a.WareHouseId equals w.WarehouseId
                                             where !a.IsDeleted && a.IsActive && a.CompanyId == claims.companyId
                                             && (a.ItemName.ToLower().Contains(search.ToLower()) || a.ItemCode.ToLower().Contains(search.ToLower()) || a.SerialNo.ToLower().Contains(search.ToLower()) ||
                                             a.Location.ToLower().Contains(search.ToLower()) || a.AssignedToName.ToLower().Contains(search.ToLower())
                                             )
                                             select new
                                             {
                                                 ItemId = a.ItemId,
                                                 ItemName = a.ItemName,
                                                 AssetsBaseCategoryId = b.AssetsBCategoryId,
                                                 AssetsBaseCategoryName = b.AssetsBCategoryName,
                                                 AssetsCategoryId = c.AssetsCategoryId,
                                                 AssetsCategoryName = c.AssetsCategoryName,
                                                 ItemCode = a.ItemCode,
                                                 SerialNo = a.SerialNo,
                                                 Location = a.Location,
                                                 PurchaseDate = a.PurchaseDate,
                                                 AssignedToName = a.AssignedToName,
                                                 AssignToId = a.AssignToId,
                                                 AssetStatus = a.AssetStatus,
                                                 AssetConditionEnum = a.AssetCondition,
                                                 AssetCondition = a.AssetCondition.ToString(),
                                                 AssignDate = a.AssignDate,
                                                 RecoverDate = a.RecoverDate,
                                                 Price = a.Price,
                                                 AssetsType = a.AssetsType,
                                                 AssetsTypeName = a.AssetsType.ToString(),
                                                 WareHouseName = w.WarehouseName,
                                                 WarehouseId = w.WarehouseId,
                                                 AssetStatusName = a.AssetStatus.ToString(),
                                                 a.AssetsDescription,
                                                 a.Comment,
                                                 a.InvoiceNo,
                                                 a.WarentyExpDate,
                                                 a.LicenseKey,
                                                 a.LicenseStartDate,
                                                 a.LicenseExpiryDate,
                                                 a.LicApplicableCount,
                                                 a.Compliance,
                                                 Officeemail = a.AssignToId == 0 ? "" : _db.Employee.Where(x => x.EmployeeId == a.AssignToId).Select(x => x.OfficeEmail).FirstOrDefault(),


                                             }).ToListAsync();
                if (assetDataSearch.Count > 0)
                {
                    res.Message = "Search results found";
                    //res.Data = assetDataSearch;
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToTitleCase(search);
                        res.Data = new PaginationData
                        {
                            TotalData = assetDataSearch.Count,
                            Counts = (int)count,
                            List = assetDataSearch.Where(aim => aim.ItemName.ToLower().Contains(search.ToLower()) || aim.ItemCode.ToLower().Contains(search.ToLower()) || aim.SerialNo.ToLower().Contains(search.ToLower()) ||
                                             aim.Location.ToLower().Contains(search.ToLower()) || aim.AssignedToName.ToLower().Contains(search.ToLower())).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue && search == null)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = assetDataSearch.Count,
                            Counts = (int)count,
                            List = assetDataSearch.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (!page.HasValue && !count.HasValue && search != null)
                    {
                        res.Data = assetDataSearch.Where(aim => aim.ItemName.ToLower().Contains(search.ToLower()) || aim.ItemCode.ToLower().Contains(search.ToLower()) || aim.SerialNo.ToLower().Contains(search.ToLower()) ||
                                             aim.Location.ToLower().Contains(search.ToLower()) || aim.AssignedToName.ToLower().Contains(search.ToLower())).ToList();
                    }
                    else
                    {
                        res.Data = assetDataSearch;
                    }
                }
                else
                {
                    res.Message = "No search Results found";
                    res.Data = assetDataSearch;
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

        #region API for search anything from all availebele assets data
        /// <summary>
        /// API > GET > api/assetsnew/searchassetsdatafromallavailebleassetslist
        /// Created By Bhavendra Singh Jat 28-08-2022
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("searchassetsdatafromallavailebleassetslist")]
        public async Task<ResponseBodyModel> GetAllAvailebleAssetsSearch(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var assetDataSearch = await (from aim in _db.AssetsItemMasters
                                             join ac in _db.AssetsCategories on aim.AssetsCategoryId equals ac.AssetsCategoryId
                                             where !aim.IsDeleted && aim.IsActive && aim.CompanyId == claims.companyId && aim.AssetStatus == AssetStatusConstants.Available &&
                                             (aim.AssetCondition == AssetConditionConstants.Good || aim.AssetCondition == AssetConditionConstants.Fair)
                                             && (aim.ItemName.ToLower().Contains(search.ToLower()) || aim.ItemCode.ToLower().Contains(search.ToLower()) || aim.WareHouseName.ToLower().Contains(search.ToLower()) ||
                                             aim.SerialNo.ToLower().Contains(search.ToLower()) || aim.AssetsBaseCategoryName.ToLower().Contains(search.ToLower()) || aim.AssetsCategoryName.ToLower().Contains(search.ToLower()) ||
                                             aim.AssetCondition.ToString().ToLower().Contains(search.ToLower()) || aim.AssetStatus.ToString().ToLower().Contains(search.ToLower()) || aim.Location.ToLower().Contains(search.ToLower()) ||
                                             aim.AssignedToName.ToLower().Contains(search.ToLower()) || aim.AssetsType.ToString().ToLower().Contains(search.ToLower())
                                             )
                                             select new
                                             {
                                                 aim.ItemId,
                                                 aim.ItemName,
                                                 aim.AssetsBaseCategoryId,
                                                 aim.AssetsBaseCategoryName,
                                                 aim.AssetsCategoryId,
                                                 aim.AssetsCategoryName,
                                                 aim.WareHouseId,
                                                 aim.WareHouseName,
                                                 aim.SerialNo,
                                                 aim.ItemCode,
                                                 AssetCondition = aim.AssetCondition.ToString(),
                                                 AssetStatus = aim.AssetStatus.ToString(),
                                                 aim.Location,
                                                 aim.PurchaseDate,
                                                 aim.AssignToId,
                                                 aim.AssignedToName,
                                                 aim.Price,
                                                 aim.WarentyExpDate,
                                                 aim.LicenseExpiryDate,
                                                 aim.LicApplicableCount,
                                                 AssetsType = aim.AssetsType.ToString(),
                                                 ac.ColorCode,
                                                 assetsIconId = ac.AssetsCategoryIconId,
                                                 ac.IsAssetsIcon,
                                                 ac.AssetIconImgUrl,
                                                 aim.AssetsDescription,
                                                 aim.Comment,
                                                 aim.InvoiceNo,
                                                 aim.LicenseKey,
                                                 aim.LicenseStartDate,
                                                 aim.AssignDate,
                                                 aim.RecoverDate,
                                                 aim.Compliance,
                                                 officeemail = aim.AssignToId == 0 ? "" : _db.Employee.Where(x => x.EmployeeId == aim.AssignToId).Select(x => x.OfficeEmail).FirstOrDefault(),

                                             }).ToListAsync();
                if (assetDataSearch.Count > 0)
                {
                    res.Message = "Search results found";
                    //res.Data = assetDataSearch;
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToTitleCase(search);
                        res.Data = new PaginationData
                        {
                            TotalData = assetDataSearch.Count,
                            Counts = (int)count,
                            List = assetDataSearch.Where(aim => aim.ItemName.ToLower().Contains(search.ToLower()) || aim.ItemCode.ToLower().Contains(search.ToLower()) || aim.WareHouseName.ToLower().Contains(search.ToLower()) ||
                                             aim.SerialNo.ToLower().Contains(search.ToLower()) || aim.AssetsBaseCategoryName.ToLower().Contains(search.ToLower()) || aim.AssetsCategoryName.ToLower().Contains(search.ToLower()) ||
                                             aim.AssetCondition.ToString().ToLower().Contains(search.ToLower()) || aim.AssetStatus.ToString().ToLower().Contains(search.ToLower()) || aim.Location.ToLower().Contains(search.ToLower()) ||
                                             aim.AssignedToName.ToLower().Contains(search.ToLower()) || aim.AssetsType.ToString().ToLower().Contains(search.ToLower())).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = assetDataSearch.Count,
                            Counts = (int)count,
                            List = assetDataSearch.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                }
                else
                {
                    res.Message = "No search Results found";
                    res.Data = assetDataSearch;
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

        #region Api for Search BaseCategory Item
        /// <summary>
        /// API > GET > api/assetsnew/serachbasecategory
        /// Created By Ravi VYas 24-08-2022
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [Route("serachbasecategory")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetSerach(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var categorySearch = await (from abc in _db.AssetsBaseCategories
                                            where !abc.IsDeleted && abc.IsActive && abc.CompanyId == claims.companyId && abc.AssetsBCategoryName.Contains(name)
                                            select new HelperForBaseCategory1
                                            {
                                                AssetsBCategoryId = abc.AssetsBCategoryId,
                                                AssetsBCategoryName = abc.AssetsBCategoryName,
                                                Description = abc.Description,
                                                //AssetsType =abc.AssetsType,
                                                AssetsTypeName = abc.AssetsType.ToString(),
                                            }).ToListAsync();

                if (categorySearch.Count > 0)
                {
                    res.Message = "BaseCategory Data Found !";
                    res.Status = true;
                    res.Data = categorySearch;
                }
                else
                {
                    res.Message = "BaseCategory Data Not Found !";
                    res.Status = false;
                    res.Data = categorySearch;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api for Serach BaseCategory Item

        #region Api for Serach Category Item

        /// <summary>
        /// API > GET > api/assetsnew/serachcategoryitem
        ///  /// Created By Ravi VYas 24-08-2022
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [Route("serachcategoryitem")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetCateSerach(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var categorySearch = await (from ac in _db.AssetsCategories
                                            join bc in _db.AssetsBaseCategories on ac.AssetsBCategoryId equals bc.AssetsBCategoryId
                                            where ac.CompanyId == claims.companyId && ac.IsActive && !ac.IsDeleted && ac.AssetsCategoryName.Contains(name) || bc.AssetsBCategoryName.Contains(name)
                                            select new getallCategoryModel
                                            {
                                                AssetsCategoryId = ac.AssetsCategoryId,
                                                AssetsBCategoryId = ac.AssetsBCategoryId,
                                                AssetsBCategoryName = bc.AssetsBCategoryName,
                                                AssetsCategoryName = ac.AssetsCategoryName,
                                                AssetsIconId = ac.AssetsCategoryIconId,
                                                ColorCode = ac.ColorCode,
                                                Description = ac.Description,
                                                AssetIconImgUrl = ac.AssetIconImgUrl,
                                                IsAssetsIcon = ac.IsAssetsIcon
                                            }).ToListAsync();

                if (categorySearch.Count > 0)
                {
                    res.Message = "Category Data Found !";
                    res.Status = true;
                    res.Data = categorySearch;
                }
                else
                {
                    res.Message = "Category Data Not Found !";
                    res.Status = false;
                    res.Data = categorySearch;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api for Serach Category Item

        #region Api's For  Warehouse Search

        /// <summary>
        /// Create by Ravi vyas On 24-08-2022
        /// API >>  GET >> api/assetsnew/searchwarehouselist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("searchwarehouselist")]
        public async Task<ResponseBodyModel> getallwarehouse(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var warehouse = await (
                                       from wh in _db.AssetsWarehouses
                                       where wh.IsDeleted == false && wh.CompanyId == claims.companyId && wh.IsActive && (wh.WarehouseAddress.Contains(name) || wh.WarehouseName.Contains(name))
                                       select new GetAllWarehouseModule
                                       {
                                           WarehouseId = wh.WarehouseId,
                                           WarehouseName = wh.WarehouseName,
                                           WarehouseAddress = wh.WarehouseAddress,
                                           WarehouseDescription = wh.WarehouseDescription,
                                           TotalItems = _db.AssetsItemMasters.Where(x => x.WareHouseId == wh.WarehouseId).Count(),
                                       }).ToListAsync();
                if (warehouse.Count != 0)
                {
                    res.Message = "Warehouse list Found";
                    res.Status = true;
                    res.Data = warehouse;
                }
                else
                {
                    res.Message = "Warehouse List Not Found";
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

        #endregion Api's For  Warehouse Search

        #region Api for Search Assets in warehouse

        /// <summary>
        /// Created By Ravi Vyas on 24-08-2022
        /// API >> GET >> api/assetsnew/searchassetbywarehouse
        /// </summary>
        /// <param name="serialno"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("searchassetbywarehouse")]
        public async Task<ResponseBodyModel> SerachAssetByWarehouse(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AssetsTypeRes response = new AssetsTypeRes();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var physicalAssetsInWarehouse = await (from im in _db.AssetsItemMasters
                                                       join ac in _db.AssetsCategories on im.AssetsCategoryId equals ac.AssetsCategoryId
                                                       //join ai in db.AssetIcons on ac.AssetsCategoryIconId equals ai.AssetIconId
                                                       where im.IsDeleted == false && im.CompanyId == claims.companyId && im.IsActive
                                                       && im.AssetsType == AssetsItemType.Physical && (im.ItemName.Contains(name) || im.AssetsBaseCategoryName.Contains(name) || im.AssetsCategoryName.Contains(name) || im.SerialNo.Contains(name))
                                                       select new GetAssetByWarehouseModel
                                                       {
                                                           ItemId = im.ItemId,
                                                           ItemName = im.ItemName,
                                                           AssetsBaseCategoryId = im.AssetsBaseCategoryId,
                                                           AssetsBaseCategoryName = im.AssetsBaseCategoryName,
                                                           AssetsCategoryId = im.AssetsCategoryId,
                                                           AssetsCategoryName = im.AssetsCategoryName,
                                                           WareHouseId = im.WareHouseId,
                                                           WareHouseName = im.WareHouseName,
                                                           ItemCode = im.ItemCode,
                                                           SerialNo = im.SerialNo,
                                                           Location = im.Location,
                                                           PurchaseDate = im.PurchaseDate,
                                                           AssignToId = im.AssignToId,
                                                           AssignedToName = im.AssignedToName,
                                                           RecoverById = im.RecoverById,
                                                           AssetCondition = im.AssetCondition.ToString(),
                                                           AssetStatus = im.AssetStatus.ToString(),
                                                           AssetsDescription = im.AssetsDescription,
                                                           AvailablityStatus = im.AvailablityStatus,
                                                           Comment = im.Comment,
                                                           //    iconurl = ai.AssetIconUrl,
                                                           totalcount = _db.AssetsItemMasters.Where(x => x.WareHouseId == im.WareHouseId && x.CompanyId == claims.companyId).Count(),
                                                           ColorCode = ac.ColorCode,
                                                           AssetsIconId = ac.AssetsCategoryIconId,
                                                           IsAssetsIcon = ac.IsAssetsIcon,
                                                           AssetIconImgUrl = ac.AssetIconImgUrl
                                                       }).ToListAsync();

                response.PhysicalAssets = physicalAssetsInWarehouse;

                var DigitalAssets = await (from im in _db.AssetsItemMasters
                                           join ac in _db.AssetsCategories on im.AssetsCategoryId equals ac.AssetsCategoryId
                                           //join ai in db.AssetIcons on ac.AssetsCategoryIconId equals ai.AssetIconId
                                           where im.IsDeleted == false && im.CompanyId == claims.companyId && im.IsActive
                                           && im.AssetsType == AssetsItemType.Digital && (im.ItemName.Contains(name) || im.AssetsBaseCategoryName.Contains(name) || im.AssetsCategoryName.Contains(name) || im.SerialNo.Contains(name))
                                           select new GetAssetByWarehouseModel
                                           {
                                               ItemId = im.ItemId,
                                               ItemName = im.ItemName,
                                               AssetsBaseCategoryId = im.AssetsBaseCategoryId,
                                               AssetsBaseCategoryName = im.AssetsBaseCategoryName,
                                               AssetsCategoryId = im.AssetsCategoryId,
                                               AssetsCategoryName = im.AssetsCategoryName,
                                               WareHouseId = im.WareHouseId,
                                               WareHouseName = im.WareHouseName,
                                               ItemCode = im.ItemCode,
                                               SerialNo = im.SerialNo,
                                               Location = im.Location,
                                               PurchaseDate = im.PurchaseDate,
                                               AssignToId = im.AssignToId,
                                               AssignedToName = im.AssignedToName,
                                               RecoverById = im.RecoverById,
                                               AssetCondition = im.AssetCondition.ToString(),
                                               AssetStatus = im.AssetStatus.ToString(),
                                               AssetsDescription = im.AssetsDescription,
                                               AvailablityStatus = im.AvailablityStatus,
                                               Comment = im.Comment,
                                               //    iconurl = ai.AssetIconUrl,
                                               totalcount = _db.AssetsItemMasters.Where(x => x.WareHouseId == im.WareHouseId && x.CompanyId == claims.companyId).Count(),
                                               ColorCode = ac.ColorCode,
                                               AssetsIconId = ac.AssetsCategoryIconId,
                                               IsAssetsIcon = ac.IsAssetsIcon,
                                               AssetIconImgUrl = ac.AssetIconImgUrl
                                           }).ToListAsync();

                response.DigitalAssets = DigitalAssets;

                res.Message = "Succesfully Get !";
                res.Status = true;
                res.Data = response;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api for Search Assets in warehouse

        #region Search All Damge Assets

        /// <summary>
        /// Create  By Ravi Vyas On 24-08-2022
        /// API >> Get >>api/assetsnew/searchalldamageassets
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("searchalldamageassets")]
        public async Task<ResponseBodyModel> SerachAllDamgeAssets(int WareHouseId, string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //var damageassets = await db.AssetsItemMasters.Where(x => x.CompanyId == claims.companyid && x.WareHouseId == Model.WareHouseId && x.AssetCondition == (AssetConditionEnum.Damage).ToString() && x.AssetStatus == (AssetStatusEnum.Unavailable).ToString()).FirstOrDefaultAsync();
                var damageassets = await (from ai in _db.AssetsItemMasters
                                              //join e in _db.Employee on ai.RecoverById equals e.EmployeeId
                                          where ai.CompanyId == claims.companyId && ai.WareHouseId == WareHouseId && ai.AssetCondition == AssetConditionConstants.Damage &&
                                          /*ai.AssetStatus == (AssetStatusEnum.Unavailable).ToString() && */ai.ItemName.Contains(name)
                                          select new Helperdamageassets
                                          {
                                              ItemId = ai.ItemId,
                                              ItemName = ai.ItemName,
                                              AssignToId = ai.AssignToId,
                                              AssignedToName = ai.AssignedToName,
                                              RecoverById = ai.RecoverById,
                                              AssetCondition = ai.AssetCondition.ToString(),
                                              AssetStatus = ai.AssetStatus.ToString(),
                                              //  RecoveredbyName = e.DisplayName,
                                          }
                                    ).ToListAsync();

                if (damageassets.Count > 0)
                {
                    res.Data = damageassets;
                    res.Message = "Assets Category List Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Assets Category List Not Found";
                    res.Status = false;
                    res.Data = damageassets;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Search All Damge Assets

        #region Serach All Lost Assets List

        /// <summary>
        /// Created By Ravi Vyas on 24-08-2022
        /// API >> GET >> api/assetsnew/serachalllostasset
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("serachalllostasset")]
        public async Task<ResponseBodyModel> SearchGetAllLostList(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assets = await _db.AssetsItemMasters.Where(x => x.IsDeleted && !x.IsActive &&
                x.CompanyId == claims.companyId && x.AssetStatus == AssetStatusConstants.Damage && !x.AvailablityStatus && x.ItemName.Contains(name)).ToListAsync();

                if (assets.Count != 0)
                {
                    res.Message = "Assets List Found !";
                    res.Status = true;
                    res.Data = assets;
                }
                else
                {
                    res.Message = "Assets List Not Found !";
                    res.Status = false;
                    res.Data = assets;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Serach All Lost Assets List

        #region Api's For Serach asstes information recovered

        /// <summary>
        /// API >> Get >> api/assetsnew/serachinforecoverable
        /// Created by Ravi Vyas  On 27-08-2022
        /// </summary>
        [HttpGet]
        [Route("serachinforecoverable")]
        public async Task<ResponseBodyModel> SerachGetInfoRecoverable(string name)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var FinalData = await (from aim in _db.AssetsItemMasters
                                       join abc in _db.AssetsBaseCategories on aim.AssetsBaseCategoryId equals abc.AssetsBCategoryId
                                       join ac in _db.AssetsCategories on aim.AssetsCategoryId equals ac.AssetsCategoryId
                                       join ah in _db.AsstesAssignHistories on aim.ItemId equals ah.ItemId
                                       join empR in _db.Employee on ah.RecoverBy equals empR.EmployeeId
                                       join empA in _db.Employee on ah.AssignToId equals empA.EmployeeId
                                       where !ah.IsActive && ah.IsDeleted && (ac.AssetsCategoryName.Contains(name) || abc.AssetsBCategoryName.Contains(name) || empA.DisplayName.Contains(name))
                                       select new
                                       {
                                           itemId = aim.ItemId,
                                           asstesName = aim.ItemName,
                                           categoryId = aim.AssetsCategoryId,
                                           categoryName = ac.AssetsCategoryName,
                                           basecategoryId = aim.AssetsBaseCategoryId,
                                           basecategoryName = abc.AssetsBCategoryName,
                                           condition = ah.AssetCondition,
                                           employeeName = empA.DisplayName,
                                           recoverBy = empR.DisplayName,
                                           recoverDate = ah.DeletedOn,
                                       }).ToListAsync();

                if (FinalData.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Assets list";
                    res.Data = FinalData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "list is not found because asset not recover";
                    res.Data = FinalData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion Api's For Serach asstes information recovered

        #endregion All Search Api's

        #region Add & Update Assets By Excel upload

        /// <summary>
        /// Created By Suraj Bundel on 07-11-2022
        /// Modified By Suraj Bundel on 08-24-2022
        /// API >> POST >> api/assetsnew/additemexcelimport
        /// Model used >>AddItemMasterImport
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("additemexcelimport")]
        public async Task<ResponseBodyModel> UpdateItemExcelImport(List<AssetImportFaultyLogs> models)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<AssetImportFaultyLogs> falultyImportItem = new List<AssetImportFaultyLogs>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            long successfullImported = 0;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (models.Count <= 0)
                {
                    res.Message = "Excel Not Have Any Data";
                    res.Status = false;
                    res.Data = falultyImportItem;
                }
                else
                {
                    var BaseCategoryNameobj = _db.AssetsBaseCategories.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToList();
                    var CategoryNameobj = _db.AssetsCategories.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToList();
                    var warehouseobj = _db.AssetsWarehouses.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToList();
                    var Employeeobj = _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).ToList();
                    foreach (var item in models)
                    {
                        var checkBaseCategory = BaseCategoryNameobj.FirstOrDefault(x => x.AssetsBCategoryName.Trim().ToUpper() ==
                            item.AssetsBaseCategoryName.Trim().ToUpper() && x.AssetsType.ToString() == item.AssetType.ToString());
                        if (checkBaseCategory == null)
                        {
                            item.FailReason = "Base Category Not Found Or Wrong Base Category Name Inputed Please Check AssetsType !";
                            item.FaultyId = Guid.NewGuid();
                            falultyImportItem.Add(item);
                        }
                        else
                        {
                            var checkcategory = CategoryNameobj.FirstOrDefault(x => x.AssetsCategoryName.Trim().ToUpper() == item.AssetsCategoryName.Trim().ToUpper());
                            if (checkcategory == null)
                            {
                                item.FailReason = "Category Not Found Or Wrong Category Name Inputed";
                                item.FaultyId = Guid.NewGuid();
                                falultyImportItem.Add(item);
                            }
                            else
                            {
                                var checkwarehouse = warehouseobj.FirstOrDefault(x => x.WarehouseName.Trim().ToUpper() == item.WareHouseName.Trim().ToUpper());
                                if (checkwarehouse == null)
                                {
                                    item.FailReason = "Warehouse Not Found Or Wrong Warehouse Name Inputed";
                                    item.FaultyId = Guid.NewGuid();
                                    falultyImportItem.Add(item);
                                }
                                else
                                {
                                    AspNetIdentity.WebApi.Model.Employee CheckEmployee = null;
                                    if (!String.IsNullOrEmpty(item.Officeemail) && !String.IsNullOrWhiteSpace(item.Officeemail))
                                    {
                                        CheckEmployee = Employeeobj.FirstOrDefault(x => x.OfficeEmail.Trim().ToUpper() == item.Officeemail.Trim().ToUpper());
                                        if (CheckEmployee == null)
                                        {
                                            item.FailReason = "Employee Not Found Or Wrong Employee Offical E-Mail Inputed";
                                            item.FaultyId = Guid.NewGuid();
                                            falultyImportItem.Add(item);
                                            continue;
                                        }
                                    }

                                    AssetsItemMaster addassets = new AssetsItemMaster();
                                    var check = _db.AssetsItemMasters.FirstOrDefault(x => x.CompanyId == claims.companyId && x.SerialNo == item.SerialNo);
                                    if (check != null)
                                        addassets = check;

                                    var text = textInfo.ToTitleCase(item.AssetType.Trim());
                                    if (text == "Physical")
                                    {
                                        addassets.AssetsType = AssetsItemType.Physical;
                                    }
                                    else if (text == "Digital")
                                    {
                                        addassets.AssetsType = AssetsItemType.Digital;
                                    }
                                    else
                                    {
                                        item.FailReason = "Asset Item Type Not Found Or Wrong Item Type Inputed";
                                        falultyImportItem.Add(item);
                                        continue;
                                    }

                                    var condition = textInfo.ToTitleCase(item.AssetCondition.ToString().Trim());
                                    if (condition == "Good")
                                    {
                                        addassets.AssetCondition = AssetConditionConstants.Good;
                                        addassets.AssetStatus = AssetStatusConstants.Available;
                                    }
                                    else if (condition == "Fair")
                                    {
                                        addassets.AssetCondition = AssetConditionConstants.Fair;
                                        addassets.AssetStatus = AssetStatusConstants.Available;
                                    }
                                    else if (condition == "Poor" || condition == "Damage" || condition == "Damaged")
                                    {
                                        addassets.AssetCondition = AssetConditionConstants.Damage;
                                        addassets.AssetStatus = AssetStatusConstants.Damage;
                                    }
                                    else if (condition == "UnderRepair" || condition == "Under Repair" || condition == "Underrepair" || condition == "Under repair")
                                    {
                                        addassets.AssetCondition = AssetConditionConstants.UnderRepair;
                                        addassets.AssetStatus = AssetStatusConstants.UnderRepair;
                                    }
                                    else
                                    {
                                        item.FailReason = "Asset Item Condition Not Found Or Wrong Item Condition Inputed";
                                        item.FaultyId = Guid.NewGuid();
                                        falultyImportItem.Add(item);
                                        continue;
                                    }
                                    addassets.ItemName = item.ItemName.Trim();
                                    addassets.AssetsBaseCategoryId = checkBaseCategory.AssetsBCategoryId;
                                    addassets.AssetsBaseCategoryName = item.AssetsBaseCategoryName.Trim();
                                    addassets.AssetsCategoryId = checkcategory.AssetsCategoryId;
                                    addassets.AssetsCategoryName = item.AssetsCategoryName.Trim();
                                    addassets.WareHouseId = checkwarehouse.WarehouseId;
                                    addassets.WareHouseName = item.WareHouseName.Trim();
                                    addassets.ItemCode = item.ItemCode.Trim();
                                    addassets.SerialNo = item.SerialNo.Trim();
                                    addassets.PurchaseDate = item.PurchaseDate;
                                    addassets.AssetsDescription = item.AssetsDescription;
                                    addassets.Location = item.Location;
                                    addassets.AvailablityStatus = true;
                                    addassets.CompanyId = claims.companyId;
                                    addassets.OrgId = claims.orgId;
                                    addassets.CreatedBy = claims.employeeId;
                                    addassets.CreatedOn = DateTime.Now;
                                    addassets.IsActive = true;
                                    addassets.IsDeleted = false;
                                    addassets.Comment = item.Comment;
                                    addassets.InvoiceNo = item.InvoiceNo.Trim();
                                    addassets.Price = item.Price;
                                    addassets.WarentyExpDate = item.WarentyExpDate;
                                    addassets.Recovered = false;
                                    addassets.Assigned = false;
                                    addassets.IsRefurbish = false;
                                    addassets.RefurbishCount = 0;
                                    addassets.Compliance = item.Compliance;
                                    if (addassets.AssetsType == AssetsItemType.Digital)
                                    {
                                        addassets.LicenseKey = item.LicenseKey.Trim();
                                        addassets.LicenseStartDate = item.LicenseStartDate;
                                        addassets.LicenseExpiryDate = item.LicenseExpiryDate;
                                        addassets.LicApplicableCount = item.LicApplicableCount;
                                    }
                                    addassets.IsCompliance = !String.IsNullOrEmpty(item.Compliance) ? true : false;
                                    if (check == null)
                                    {
                                        _db.AssetsItemMasters.Add(addassets);
                                        await _db.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _db.Entry(addassets).State = System.Data.Entity.EntityState.Modified;
                                        await _db.SaveChangesAsync();
                                    }

                                    if (
                                        (addassets.AssetStatus != AssetStatusConstants.Damage || addassets.AssetCondition != AssetConditionConstants.Damage)
                                        &&
                                        (addassets.AssetStatus != AssetStatusConstants.UnderRepair || addassets.AssetCondition != AssetConditionConstants.UnderRepair)
                                       )
                                    {
                                        if (!String.IsNullOrEmpty(item.Officeemail) && !String.IsNullOrWhiteSpace(item.Officeemail))
                                        {
                                            if (CheckEmployee != null)
                                            {
                                                addassets.AssignToId = CheckEmployee.EmployeeId;
                                                addassets.AssignedToName = CheckEmployee.DisplayName;
                                                addassets.Assigned = true;
                                                addassets.Recovered = false;
                                                addassets.AssignDate = item.AssignDate;
                                                addassets.Comment = item.Comment;
                                                addassets.AssetStatus = AssetStatusConstants.Assigned;
                                                addassets.AvailablityStatus = false;

                                                _db.Entry(addassets).State = System.Data.Entity.EntityState.Modified;
                                                await _db.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        addassets.AssignToId = 0;
                                        addassets.AssignedToName = null;
                                        addassets.Assigned = true;
                                        addassets.Recovered = false;
                                        addassets.AssignDate = null;
                                        addassets.Comment = null;
                                        addassets.AvailablityStatus = true;

                                        _db.Entry(addassets).State = System.Data.Entity.EntityState.Modified;
                                        await _db.SaveChangesAsync();
                                    }

                                    AssetsHistory historyobj = new AssetsHistory();

                                    historyobj.ItemId = addassets.ItemId;
                                    historyobj.ItemName = addassets.ItemName;
                                    historyobj.AssetsBaseCategoryId = addassets.AssetsBaseCategoryId;
                                    historyobj.AssetsCategoryId = addassets.AssetsCategoryId;
                                    historyobj.WareHouseId = addassets.WareHouseId;
                                    historyobj.ItemCode = addassets.ItemCode;
                                    historyobj.SerialNo = addassets.SerialNo;
                                    historyobj.Location = addassets.Location;
                                    historyobj.InvoiceUrl = addassets.InvoiceUrl;
                                    historyobj.PurchaseDate = addassets.PurchaseDate;
                                    historyobj.AssignToId = addassets.AssignToId;
                                    historyobj.RecoverById = addassets.RecoverById;
                                    historyobj.AssetCondition = addassets.AssetCondition;
                                    historyobj.AssetStatus = addassets.AssetStatus;
                                    historyobj.AssetsDescription = addassets.AssetsDescription;
                                    historyobj.ReasonNotAvailable = addassets.ReasonNotAvailable;
                                    historyobj.AvailablityStatus = addassets.AvailablityStatus;
                                    historyobj.Recovered = addassets.Recovered;
                                    historyobj.Assigned = addassets.Assigned;
                                    historyobj.AssignDate = addassets.AssignDate;
                                    historyobj.RecoverDate = addassets.RecoverDate;
                                    historyobj.Comment = addassets.Comment;
                                    historyobj.IsRefurbish = addassets.IsRefurbish;
                                    historyobj.RefurbishCount = addassets.RefurbishCount;
                                    historyobj.InvoiceNo = addassets.InvoiceNo;
                                    historyobj.Price = addassets.Price;
                                    historyobj.WarentyExpDate = addassets.WarentyExpDate;
                                    historyobj.Compliance = addassets.Compliance;
                                    historyobj.IsCompliance = addassets.IsCompliance;
                                    historyobj.LicenseKey = addassets.LicenseKey;
                                    historyobj.LicenseStartDate = addassets.LicenseStartDate;
                                    historyobj.LicenseExpiryDate = addassets.LicenseExpiryDate;
                                    historyobj.LicApplicableCount = addassets.LicApplicableCount;
                                    historyobj.AssetsType = addassets.AssetsType;
                                    historyobj.UpImg1 = addassets.UpImg1;
                                    historyobj.UpImg2 = addassets.UpImg2;
                                    historyobj.UpImg3 = addassets.UpImg3;
                                    historyobj.UpImg4 = addassets.UpImg4;
                                    historyobj.UpImg5 = addassets.UpImg5;
                                    historyobj.UpImg6 = addassets.UpImg6;
                                    historyobj.UpImg7 = addassets.UpImg7;
                                    historyobj.UpImg8 = addassets.UpImg8;
                                    historyobj.UpImg9 = addassets.UpImg9;
                                    historyobj.UpImg10 = addassets.UpImg10;
                                    historyobj.IsActive = addassets.IsActive;
                                    historyobj.IsDeleted = addassets.IsDeleted;
                                    historyobj.CompanyId = addassets.CompanyId;
                                    historyobj.OrgId = addassets.OrgId;
                                    historyobj.CreatedBy = addassets.CreatedBy;
                                    historyobj.UpdatedBy = addassets.UpdatedBy;
                                    historyobj.DeletedBy = addassets.DeletedBy;
                                    historyobj.CreatedOn = addassets.CreatedOn;
                                    historyobj.UpdatedOn = addassets.UpdatedOn;
                                    historyobj.DeletedOn = addassets.DeletedOn;

                                    _db.AssetsHistories.Add(historyobj);
                                    await _db.SaveChangesAsync();
                                    successfullImported += 1;
                                }
                            }
                        }
                    }
                    if (falultyImportItem.Count > 0)
                    {
                        AssetImportFaultyLogsGoups groupObj = new AssetImportFaultyLogsGoups
                        {
                            GroupId = Guid.NewGuid(),
                            TotalImported = models.Count,
                            SuccessFullImported = successfullImported,
                            UnSuccessFullImported = falultyImportItem.Count,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                        };
                        _db.AssetImportFaultieGroups.Add(groupObj);
                        await _db.SaveChangesAsync();

                        falultyImportItem.ForEach(x =>
                        {
                            x.FaultyId = Guid.NewGuid();
                            x.Groups = groupObj;
                        });
                        _db.AssetImportFaultieLogs.AddRange(falultyImportItem);
                        await _db.SaveChangesAsync();

                        if ((models.Count - falultyImportItem.Count) > 0)
                        {
                            res.Message = "Asset Imported Succesfull Of " +
                            (models.Count - falultyImportItem.Count) + " Fields And " +
                            falultyImportItem.Count + " Feilds Are Not Imported";
                            res.Status = true;
                            res.Data = falultyImportItem;
                        }
                        else
                        {
                            res.Message = "All Fields Are Not Imported";
                            res.Status = true;
                            res.Data = falultyImportItem;
                        }
                    }
                    else
                    {
                        res.Message = "Data Added Successfully Of All Fields";
                        res.Status = true;
                        res.Data = falultyImportItem;
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
        #endregion Add & Update Assets By Excel upload

        #region Assets Category Filter
        /// <summary>
        /// Created By Suraj Bundel on 02/09/2022
        /// API=> GET=> api/assetsnew/categoryfilter
        /// </summary>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <param name="search"></param>
        /// <param name="BasecategoryId"></param>
        /// <param name="CategoryId"></param>
        /// <param name="Assettype"></param>
        [HttpGet]
        [Route("categoryfilter")]
        public async Task<ResponseBodyModel> CategoryFilter(int? page = null, int? count = null,
            string search = null, int? BasecategoryId = null, int? CategoryId = null, AssetsItemType? Assettype = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var AssetsCategoryobj = await (from ass in _db.AssetsItemMasters
                                               join it in _db.AssetsCategories on ass.AssetsCategoryId equals it.AssetsCategoryId
                                               where !ass.IsDeleted && ass.IsActive && ass.CompanyId == claims.companyId && it.CompanyId == claims.companyId &&
                                                ass.AssetStatus == Model.EnumClass.AssetStatusConstants.Available &&
                                                (ass.AssetCondition == AssetConditionConstants.Good || ass.AssetCondition == AssetConditionConstants.Fair)
                                               select new GetHelperGetAllAassets
                                               {
                                                   ItemId = ass.ItemId,
                                                   ItemName = ass.ItemName,
                                                   AssetsBaseCategoryId = ass.AssetsBaseCategoryId,
                                                   AssetsBaseCategoryName = ass.AssetsBaseCategoryName,
                                                   AssetsCategoryId = ass.AssetsCategoryId,
                                                   AssetsCategoryName = ass.AssetsCategoryName,
                                                   WareHouseId = ass.WareHouseId,
                                                   WareHouseName = ass.WareHouseName,
                                                   SerialNo = ass.SerialNo,
                                                   ItemCode = ass.ItemCode,
                                                   AssetCondition = ass.AssetCondition.ToString(),
                                                   AssetStatus = ass.AssetStatus.ToString(),
                                                   Location = ass.Location,
                                                   ColorCode = it.ColorCode,
                                                   AssetsIconId = it.AssetsCategoryIconId,
                                                   IsAssetsIcon = it.IsAssetsIcon,
                                                   AssetIconImgUrl = it.AssetIconImgUrl,
                                                   Assetstype = ass.AssetsType,
                                                   AssetsTypeName = ass.AssetsType.ToString(),
                                               }).ToListAsync();
                if (CategoryId.HasValue)
                {
                    AssetsCategoryobj = AssetsCategoryobj.Where(x => x.AssetsCategoryId == (int)CategoryId).ToList();
                }
                if (BasecategoryId.HasValue)
                {
                    AssetsCategoryobj = AssetsCategoryobj.Where(x => x.AssetsBaseCategoryId == (int)BasecategoryId).ToList();
                }
                if (Assettype.HasValue)
                {
                    AssetsCategoryobj = AssetsCategoryobj.Where(x => x.Assetstype == Assettype).ToList();
                }
                if (AssetsCategoryobj.Count > 0)
                {
                    res.Message = "Assets list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToTitleCase(search);
                        res.Data = new PaginationData
                        {
                            TotalData = AssetsCategoryobj.Count,
                            Counts = (int)count,
                            List = AssetsCategoryobj.Where(x => x.AssetsBaseCategoryName.ToUpper().StartsWith(text) || x.AssetsCategoryName.ToUpper().StartsWith(text) &&
                            x.WareHouseName.ToUpper().StartsWith(text)).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = AssetsCategoryobj.Count,
                            Counts = (int)count,
                            List = AssetsCategoryobj.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = AssetsCategoryobj;
                    }
                }
                else
                {
                    res.Data = new
                    {
                        TotalData = AssetsCategoryobj.Count,
                        Counts = (int)count,
                        List = AssetsCategoryobj.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                    };
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion

        #region Getall global assets
        /// <summary>
        ///  Created by Suraj Bundel on 1/09/2022
        ///  API >> POST >> api/assetsnew/getallglobalassets
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("getallglobalassets")]
        public async Task<ResponseBodyModel> GetAllCompanyStatus(GlobalAssetsFilterModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var exempobj = await (from a in _db.AssetsItemMasters
                                      join e in _db.AssetsCategories on a.AssetsCategoryId equals e.AssetsCategoryId
                                      where (a.IsActive && !a.IsDeleted && a.CompanyId == claims.companyId)
                                      select new GetAllAssetsModel
                                      {
                                          ItemId = a.ItemId,
                                          ItemName = a.ItemName,
                                          AssetsIcon = e.AssetIconImgUrl,
                                          AssetsBaseCategoryName = a.AssetsBaseCategoryName,
                                          AssetsCategoryName = a.AssetsCategoryName,
                                          WareHouseName = a.WareHouseName,
                                          ItemCode = a.ItemCode,
                                          SerialNo = a.SerialNo,
                                          Location = a.Location,
                                          PurchaseDate = a.PurchaseDate,
                                          AssignedToName = a.AssignedToName,
                                          AssetConditionEnum = a.AssetCondition,
                                          AssetCondition = a.AssetCondition.ToString(),
                                          AssetStatusEnum = a.AssetStatus,
                                          AssetStatus = a.AssetStatus.ToString(),
                                          AvailablityStatus = a.AvailablityStatus,
                                          AssignDate = a.AssignDate,
                                          RecoverDate = a.RecoverDate,
                                          InvoiceNo = a.InvoiceNo,
                                          Price = a.Price,
                                          WarentyExpDate = a.WarentyExpDate,
                                          Compliance = a.Compliance,
                                          IsCompliance = a.IsCompliance,
                                          LicenseKey = a.LicenseKey,
                                          LicenseStartDate = a.LicenseStartDate,
                                          LicenseExpiryDate = a.LicenseExpiryDate,
                                          LicApplicableCount = a.LicApplicableCount,
                                          AssetsType = a.AssetsType,
                                          AssetsTypeName = a.AssetsType.ToString(),
                                          Recovered = a.Recovered,
                                          Assigned = a.Assigned,
                                      }).ToListAsync();

                if (exempobj.Count > 0)
                {
                    var predicate = PredicateBuilder.New<GetAllAssetsModel>(x => x.IsActive && !x.IsDelete);
                    if (model.FilterEnum.Count > 0)
                    {
                        exempobj = (exempobj.Where(x => model.FilterEnum.Contains(x.AssetStatusEnum))).ToList();
                    }
                    if (model.Type.Count > 0)
                    {
                        exempobj = (exempobj.Where(x => model.Type.Contains(x.AssetsType))).ToList();
                    }
                }
                if (exempobj.Count > 0)
                {
                    res.Message = "Assets list Found";
                    res.Status = true;
                    if (model.Page.HasValue && model.Count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = exempobj.Count,
                            Counts = (int)model.Count,
                            List = exempobj.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = exempobj;
                    }
                }
                else
                {
                    res.Data = new
                    {
                        TotalData = exempobj.Count,
                        Counts = (int)model.Count,
                        List = exempobj.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),
                    };
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class GlobalAssetsFilterModel
        {
            public int? Page { get; set; }
            public int? Count { get; set; }
            public List<AssetStatusConstants> FilterEnum { get; set; } = new List<AssetStatusConstants>();
            public List<AssetsItemType> Type { get; set; } = new List<AssetsItemType>();

        }



        #region Get All  Global // dropdown

        /// <summary>
        /// Created By Suraj Bundel on 23-05-2022
        /// API >> Get >> api/assetsnew/getallglobaldropdownfilter
        /// Dropdown using Enum for expense type category
        /// </summary>
        /// <returns></returns>

        [Route("getallglobaldropdownfilter")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel Getallexpenseentrycategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var listdata = Enum.GetValues(typeof(AssetStatusConstants))
                    .Cast<AssetStatusConstants>()
                    .Select(x => new AssetTypeListModel
                    {
                        AssetGlobalTypeId = (int)x,
                        AssetGlobalType = Enum.GetName(typeof(AssetStatusConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Asset condition list exist";
                res.Status = true;
                res.Data = listdata;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class AssetTypeListModel
        {
            public int AssetGlobalTypeId { get; set; }
            public string AssetGlobalType { get; set; }
        }

        #endregion Get all global assets// dropdown

        public class GetAllAssetsModel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public string AssignedToName { get; set; }
            public AssetConditionConstants AssetConditionEnum { get; set; }
            public string AssetCondition { get; set; }
            public AssetStatusConstants AssetStatusEnum { get; set; }
            public string AssetStatus { get; set; }
            public bool AvailablityStatus { get; set; }
            public bool Recovered { get; set; }
            public bool Assigned { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? RecoverDate { get; set; }
            public string InvoiceNo { get; set; }
            public double Price { get; set; }
            public DateTime? WarentyExpDate { get; set; }
            public string Compliance { get; set; }
            public bool IsCompliance { get; set; }
            public string LicenseKey { get; set; }
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int? LicApplicableCount { get; set; }
            public AssetsItemType AssetsType { get; set; }
            public string AssetsTypeName { get; set; }
            public string AssetsIcon { get; set; }
            public string Status { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }

            //  public List<int> Assets { get; set; }
        }
        #endregion

        #region Getall global assets export

        /// <summary>
        ///  Created by Suraj Bundel on 03/09/2022
        ///  API >> POST >> api/assetsnew/getallglobalassetsexport
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("getallglobalassetsexport")]
        public async Task<ResponseBodyModel> GetAllCompanyStatusExport()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var exempobj = await (from a in _db.AssetsItemMasters
                                      where (a.IsActive && !a.IsDeleted && a.CompanyId == claims.companyId)
                                      select new GetAllAssetsExportModel
                                      {
                                          ItemId = a.ItemId,
                                          ItemName = a.ItemName,
                                          AssetsBaseCategoryName = a.AssetsBaseCategoryName,
                                          AssetsCategoryName = a.AssetsCategoryName,
                                          WareHouseName = a.WareHouseName,
                                          ItemCode = a.ItemCode,
                                          SerialNo = a.SerialNo,
                                          Location = a.Location,
                                          PurchaseDate = a.PurchaseDate,
                                          AssignedToName = a.AssignedToName,
                                          AssetCondition = a.AssetCondition.ToString(),
                                          AssetStatus = a.AssetStatus.ToString(),
                                          AvailablityStatus = a.AvailablityStatus,
                                          AssignDate = a.AssignDate,
                                          RecoverDate = a.RecoverDate,
                                          InvoiceNo = a.InvoiceNo,
                                          Price = a.Price,
                                          WarentyExpDate = a.WarentyExpDate,
                                          Compliance = a.Compliance,
                                          IsCompliance = a.IsCompliance,
                                          LicenseKey = a.LicenseKey,
                                          LicenseStartDate = a.LicenseStartDate,
                                          LicenseExpiryDate = a.LicenseExpiryDate,
                                          LicApplicableCount = a.LicApplicableCount,
                                          AssetsType = a.AssetsType,
                                          AssetsTypeName = a.AssetsType.ToString(),

                                      }).ToListAsync();
                res.Message = "Data Download";
                res.Status = true;
                res.Data = exempobj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class GlobalAssetsFilterExportModel
        {
            public bool Assignassets { get; set; }
            public bool NotAvailable { get; set; }
            public bool DamageAssets { get; set; }
            public bool RepairAssets { get; set; }

        }



        public class GetAllAssetsExportModel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public string AssignedToName { get; set; }
            public string AssetCondition { get; set; }
            public string AssetStatus { get; set; }
            public bool AvailablityStatus { get; set; }
            public bool Recovered { get; set; }
            public bool Assigned { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? RecoverDate { get; set; }
            public string InvoiceNo { get; set; }
            public double Price { get; set; }
            public DateTime? WarentyExpDate { get; set; }
            public string Compliance { get; set; }
            public bool IsCompliance { get; set; }
            public string LicenseKey { get; set; }
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int? LicApplicableCount { get; set; }
            public AssetsItemType AssetsType { get; set; }
            public string AssetsTypeName { get; set; }

        }
        #endregion

        #region Get All Underrepair Assets

        /// <summary>
        /// Create  By Suraj Bundel On 07/09/2022
        /// API >> Get >> api/assetsnew/getallunderrepairassets
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <returns></returns>
      //  [HttpGet]
        [Route("getallunderrepairassets")]
        public async Task<ResponseBodyModel> GetAllUnderrepairAssets(int WareHouseId, string search = null, int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var underrepairassets = await (from ai in _db.AssetsItemMasters
                                               join bc in _db.AssetsBaseCategories on ai.AssetsBaseCategoryId equals bc.AssetsBCategoryId
                                               join ct in _db.AssetsCategories on ai.AssetsCategoryId equals ct.AssetsCategoryId
                                               join wh in _db.AssetsWarehouses on ai.WareHouseId equals wh.WarehouseId
                                               where ai.CompanyId == claims.companyId && ai.WareHouseId == WareHouseId &&
                                               ai.AssetCondition == AssetConditionConstants.UnderRepair && ai.AssetStatus == AssetStatusConstants.UnderRepair
                                               && ai.IsActive && !ai.IsDeleted
                                               select new
                                               {
                                                   ItemId = ai.ItemId,
                                                   ItemName = ai.ItemName,
                                                   ItemCode = ai.ItemCode,
                                                   ai.SerialNo,
                                                   AssignToId = ai.AssignToId,
                                                   AssignedToName = ai.AssignedToName,
                                                   RecoverById = ai.RecoverById,
                                                   AssetCondition = ai.AssetCondition.ToString().Replace("_", " "),
                                                   AssetStatus = ai.AssetStatus.ToString(),
                                                   BaseCategoryName = bc.AssetsBCategoryName,
                                                   CategoryName = ct.AssetsCategoryName,
                                                   wh.WarehouseId,
                                                   wh.WarehouseName,
                                                   ai.Location,
                                                   ai.PurchaseDate,
                                                   //RecoveredbyName = _db.GetEmployeeNameById(ai.RecoverById),

                                               }).ToListAsync();
                if (underrepairassets.Count > 0)
                {
                    res.Message = "Assets List Found";
                    res.Status = true;
                    //res.Data = damageassets;
                    if (page.HasValue && count.HasValue && search != null)
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new PaginationData
                        {
                            TotalData = underrepairassets.Count,
                            Counts = (int)count,
                            List = underrepairassets.Where(x => x.ItemName.ToUpper().StartsWith(text) || x.AssignedToName.ToUpper().StartsWith(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        //if (page.HasValue && count.HasValue && !String.IsNullOrEmpty(search))
                        //{
                        res.Data = new PaginationData
                        {
                            TotalData = underrepairassets.Count,
                            Counts = (int)count,
                            //List = category.Where(x => x.AssetsBCategoryName.Contains(search)).ToList(),
                            List = underrepairassets.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),

                        };
                    }
                    else
                    {
                        res.Data = underrepairassets;
                    }
                }
                else
                {
                    res.Message = "Assets List Not Found";
                    res.Status = false;
                    res.Data = underrepairassets;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion Get All Damge Assets

        #region Get Asset Expiry Notification

        /// <summary>
        /// Create  By Suraj Bundel On 07/09/2022
        /// API >> Get >> api/assetsnew/getexpiryassetnotification
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getexpiryassetnotification")]
        public async Task<ResponseBodyModel> GetAssetsExpiryNotification(AssetesNotificationModel Model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                // int test = 0;
                var employeelist = _db.User.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.LoginId == LoginRolesConstants.IT).ToList();
                var recoverydate = _db.AssetsItemMasters.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.LicenseExpiryDate != null).ToList();
                foreach (var item in recoverydate)
                {
                    if (item.LicenseExpiryDate > DateTime.Now)
                    {
                        int obj = (item.LicenseExpiryDate - DateTime.Now).Value.Days;
                        //   test = obj;

                        if (obj < 10)
                        {
                            var notify = _db.AssetsItemMasters.Where(x => x.ItemId == item.ItemId && x.LicenseExpiryDate == item.LicenseExpiryDate && x.CompanyId == claims.companyId).FirstOrDefault();
                            if (notify != null)
                            {
                                Notification AssetNotice = new Notification();


                                AssetNotice.Title = item.ItemName + " asset is Expiry on " + item.LicenseExpiryDate;
                                AssetNotice.Message = item.ItemName + " asset is Expiry on " + item.LicenseExpiryDate;
                                AssetNotice.CreateDate = DateTime.Now;
                                AssetNotice.IsActive = true;
                                AssetNotice.IsDeleted = false;
                                foreach (var employeedata in employeelist)
                                {

                                    //  AssetNotice.EmployeeId = employeelist.ForEach(x => x.LoginType == LoginRolesEnum.IT).Select(x => x.EmployeeId);
                                    //Select(x => x.EmployeeId);x
                                    AssetNotice.EmployeeId = employeedata.EmployeeId;
                                }

                                AssetNotice.ForPC = true;
                                AssetNotice.CompanyId = claims.companyId;
                                var FcmData = _db.FireBases.FirstOrDefault(f => f.CompanyId == item.CompanyId && f.EmployeeId == item.AssignToId);
                                NotificationController _noti = new NotificationController();
                                if (FcmData != null)
                                {
                                    FireBase FirebaseNotice = new FireBase();
                                    FirebaseNotice.PCFCMToken = Model.PCFCMToken;
                                    FirebaseNotice.EmployeeId = item.AssignToId;
                                    FirebaseNotice.CompanyId = claims.companyId;
                                    FirebaseNotice.FCMToken = Model.FCMToken;
                                    _ = await _noti.AddNotification(AssetNotice);

                                    _ = _noti.SendPcNotification(AssetNotice.Title, AssetNotice.Message, FirebaseNotice.PCFCMToken);
                                }
                                res.Message = "Candidate is Hired";
                                res.Status = true;
                                res.Data = AssetNotice;
                            }
                        }
                    }
                    else
                    {
                        res.Message = "Model Is Null";
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

        public class AssetesNotificationModel
        {
            public string FCMToken { get; set; }
            public string PCFCMToken { get; set; }
        }


        public class Helpermodel
        {
            public int Time { get; set; }
        }
        #endregion

        #region Change asset warehouse
        /// <summary>
        /// Created by Suraj Bundel on  13/09/2022
        /// API >> Get >> api/assetsnew/changewarehouse
        /// </summary>
        /// <param name="warehouseid"></param>
        /// <returns></returns>

        [HttpPut]
        [Route("changewarehouse")]
        public async Task<ResponseBodyModel> ChangeWarehouse(int Warehouseid, int Itemid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Change = await _db.AssetsItemMasters.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted /*&& x.WareHouseId == Warehouseid*/ && x.ItemId == Itemid).FirstOrDefaultAsync();
                if (Change != null)
                {
                    Change.WareHouseId = Warehouseid;
                    Change.WareHouseName = _db.AssetsWarehouses.Where(x => x.WarehouseId == Warehouseid).Select(x => x.WarehouseName).FirstOrDefault();
                    _db.Entry(Change).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Message = "Warehouse Update Successfully";
                    res.Status = true;
                    res.Data = Change;
                }
                else
                {
                    res.Message = "Failed to Update Warehouse";
                    res.Status = false;
                    res.Data = Change;
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

        #region Change asset warehouse
        /// <summary>
        /// Created by Suraj Bundel on  13/09/2022
        /// API >> Get >> api/assetsnew/getwarehousebyitemid
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("getwarehousebyitemid")]
        public async Task<ResponseBodyModel> GetWareHouseIdByItemId(int itemId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var item = await _db.AssetsItemMasters.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.ItemId == itemId);
                if (item != null)
                {
                    var warehouse = await _db.AssetsWarehouses.FirstOrDefaultAsync(x => x.WarehouseId == item.WareHouseId && x.IsActive && !x.IsDeleted);
                    if (warehouse != null)
                    {
                        res.Message = "WareHouse Id Found";
                        res.Status = true;
                        res.Data = new
                        {
                            warehouse.WarehouseId,
                            warehouse.WarehouseName,
                        };
                    }
                    else
                    {
                        res.Message = "WareHouse Id not Found";
                        res.Status = true;
                    }
                }
                else
                {
                    res.Message = "Item Not Found";
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

        #region MyRegion Get assets on behalf of orgid

        /// <summary>
        ///  Created by Suraj Bundel on 17/09/2022
        ///  API >> POST >> api/assetsnew/getassethistory
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("getassethistory")]
        public async Task<ResponseBodyModel> GetAssetHistory(int Itemid)
        //   public async Task<ResponseBodyModel> GetAllCompanyStatusOrg(int Itemid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                // var history = await _db.AssetsHistories.Where(x => x.ItemId == Itemid).Select(x => new HistoryAssetshelper() ).ToListAsync();

                var history = await (from ah in _db.AssetsHistories
                                     join ai in _db.AssetsItemMasters on ah.ItemId equals ai.ItemId
                                     join c in _db.AssetsCategories on ai.AssetsCategoryId equals c.AssetsCategoryId
                                     where ah.CompanyId == claims.companyId && ai.CompanyId == claims.companyId && ai.ItemId == Itemid
                                     select new HistoryAssetshelper
                                     {
                                         ItemId = ah.ItemId,
                                         ItemName = ah.ItemName,
                                         AssetsBaseCategoryName = _db.AssetsBaseCategories.Where(x => x.AssetsBCategoryId == ah.AssetsBaseCategoryId).Select(x => x.AssetsBCategoryName).FirstOrDefault(),
                                         AssetsCategoryName = _db.AssetsCategories.Where(x => x.AssetsCategoryId == ah.AssetsCategoryId).Select(x => x.AssetsCategoryName).FirstOrDefault(),
                                         WareHouseName = _db.AssetsWarehouses.Where(x => x.WarehouseId == ah.WareHouseId).Select(x => x.WarehouseName).FirstOrDefault(),
                                         ItemCode = ah.ItemCode,
                                         SerialNo = ah.SerialNo,
                                         Location = ah.Location,
                                         PurchaseDate = ah.PurchaseDate,
                                         AssignedToName = _db.Employee.Where(x => x.EmployeeId == ah.AssignToId).Select(x => x.DisplayName).FirstOrDefault(),
                                         AssetCondition = ah.AssetCondition.ToString(),
                                         AssetStatus = ah.AssetStatus.ToString(),
                                         AvailablityStatus = ah.AvailablityStatus,
                                         AssignDate = ah.AssignDate,
                                         RecoverDate = ah.RecoverDate,
                                         InvoiceNo = ah.InvoiceNo,
                                         Price = ah.Price,
                                         WarentyExpDate = ah.WarentyExpDate,
                                         Compliance = ah.Compliance,
                                         LicenseKey = ah.LicenseKey,
                                         LicenseStartDate = ah.LicenseStartDate,
                                         LicenseExpiryDate = ah.LicenseExpiryDate,
                                         LicApplicableCount = ah.LicApplicableCount,
                                         AssetsType = ah.AssetsType.ToString(),
                                         AssetsIcon = c.AssetIconImgUrl,
                                         RecoveredBy = _db.Employee.Where(x => x.EmployeeId == ah.RecoverById).Select(x => x.DisplayName).FirstOrDefault(),
                                         AssignedBy = _db.Employee.Where(x => x.EmployeeId == ah.AssignById).Select(x => x.DisplayName).FirstOrDefault(),
                                         // AssignDate = ah.AssignDate,
                                     }).ToListAsync();
                if (Itemid > 0)
                {

                    res.Message = "History found";
                    res.Status = true;
                    res.Data = history;
                }
                else
                {
                    res.Message = "History not Found";
                    res.Status = false;
                    res.Data = history;
                }
                //if (history.Count > 0)
                //{
                //    res.Message = "Assets list Found";
                //    res.Status = true;
                //    if (model.Page.HasValue && model.Count.HasValue)
                //    {
                //        res.Data = new
                //        {
                //            TotalData = history.Count,
                //            Counts = (int)model.Count,
                //            List = history.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),
                //        };
                //    }
                //    else
                //    {
                //        res.Data = history;
                //    }
                //}
                //else
                //{
                //    res.Data = new
                //    {
                //        TotalData = history.Count,
                //        Counts = (int)model.Count,
                //        List = history.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),
                //    };
                //}
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class HistoryAssetsModel
        {
            public int? Page { get; set; }
            public int? Count { get; set; }
            public int Itemid { get; set; }
        }
        public class HistoryAssetshelper
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public string AssignedToName { get; set; }

            public string AssetCondition { get; set; }

            public string AssetStatus { get; set; }
            public bool AvailablityStatus { get; set; }
            public string RecoveredBy { get; set; }
            public string AssignedBy { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? RecoverDate { get; set; }
            public string InvoiceNo { get; set; }
            public double Price { get; set; }
            public DateTime? WarentyExpDate { get; set; }
            public string Compliance { get; set; }
            public bool IsCompliance { get; set; }
            public string LicenseKey { get; set; }
            public DateTime? LicenseStartDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public int? LicApplicableCount { get; set; }
            public string AssetsType { get; set; }
            // public string AssetsTypeName { get; set; }
            public string AssetsIcon { get; set; }
            public string Status { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }
            public int OrgID { get; set; }
        }

        #endregion

        //#region MyRegion Get assets on behalf of orgid

        ///// <summary>
        /////  Created by Suraj Bundel on 1/09/2022
        /////  API >> POST >> api/assetsnew/allglobalassetsorg
        ///// </summary>
        ///// <returns></returns>

        //[HttpPost]
        //[Route("allglobalassetsorg")]
        //public async Task<ResponseBodyModel> GetAllCompanyStatusOrg(GlobalAssetsFilterModelOrg model)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //        var exempobj = await (from a in _db.AssetsItemMasters
        //                              join e in _db.AssetsCategories on a.AssetsCategoryId equals e.AssetsCategoryId
        //                              where (a.IsActive && !a.IsDeleted && a.CompanyId == claims.companyid && claims.orgid == 0)
        //                              select new GetAllAssetsModelOrg
        //                              {
        //                                  ItemId = a.ItemId,
        //                                  ItemName = a.ItemName,
        //                                  AssetsIcon = e.AssetIconImgUrl,
        //                                  AssetsBaseCategoryName = a.AssetsBaseCategoryName,
        //                                  AssetsCategoryName = a.AssetsCategoryName,
        //                                  WareHouseName = a.WareHouseName,
        //                                  ItemCode = a.ItemCode,
        //                                  SerialNo = a.SerialNo,
        //                                  Location = a.Location,
        //                                  PurchaseDate = a.PurchaseDate,
        //                                  AssignedToName = a.AssignedToName,
        //                                  AssetConditionEnum = a.AssetCondition,
        //                                  AssetCondition = a.AssetCondition.ToString(),
        //                                  AssetStatusEnum = a.AssetStatus,
        //                                  AssetStatus = a.AssetStatus.ToString(),
        //                                  AvailablityStatus = a.AvailablityStatus,
        //                                  AssignDate = a.AssignDate,
        //                                  RecoverDate = a.RecoverDate,
        //                                  InvoiceNo = a.InvoiceNo,
        //                                  Price = a.Price,
        //                                  WarentyExpDate = a.WarentyExpDate,
        //                                  Compliance = a.Compliance,
        //                                  IsCompliance = a.IsCompliance,
        //                                  LicenseKey = a.LicenseKey,
        //                                  LicenseStartDate = a.LicenseStartDate,
        //                                  LicenseExpiryDate = a.LicenseExpiryDate,
        //                                  LicApplicableCount = a.LicApplicableCount,
        //                                  AssetsType = a.AssetsType,
        //                                  AssetsTypeName = a.AssetsType.ToString(),
        //                                  Recovered = a.Recovered,
        //                                  Assigned = a.Assigned,
        //                                  OrgID = a.OrgId,
        //                              }).ToListAsync();

        //        if (exempobj.Count > 0)
        //        {
        //            var predicate = PredicateBuilder.New<GetAllAssetsModel>(x => x.IsActive && !x.IsDelete);

        //            foreach (var item in model.FilterEnum)
        //            {
        //                if (model.FilterEnum.Count > 0)
        //                {
        //                    exempobj = (exempobj.Where(x => model.FilterEnum.Contains(x.OrgID))).ToList();
        //                }
        //            }
        //        }
        //        if (exempobj.Count > 0)
        //        {
        //            res.Message = "Assets list Found";
        //            res.Status = true;
        //            if (model.Page.HasValue && model.Count.HasValue)
        //            {
        //                res.Data = new
        //                {
        //                    TotalData = exempobj.Count,
        //                    Counts = (int)model.Count,
        //                    List = exempobj.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),
        //                };
        //            }
        //            else
        //            {
        //                res.Data = exempobj;
        //            }
        //        }
        //        else
        //        {
        //            res.Data = new
        //            {
        //                TotalData = exempobj.Count,
        //                Counts = (int)model.Count,
        //                List = exempobj.Skip(((int)model.Page - 1) * (int)model.Count).Take((int)model.Count).ToList(),
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //public class GlobalAssetsFilterModelOrg
        //{
        //    public int? Page { get; set; }
        //    public int? Count { get; set; }
        //    public List<int> FilterEnum { get; set; }
        //}
        //public class GetAllAssetsModelOrg
        //{
        //    public int ItemId { get; set; }
        //    public string ItemName { get; set; }
        //    public string AssetsBaseCategoryName { get; set; }
        //    public string AssetsCategoryName { get; set; }
        //    public string WareHouseName { get; set; }
        //    public string ItemCode { get; set; }
        //    public string SerialNo { get; set; }
        //    public string Location { get; set; }
        //    public DateTime? PurchaseDate { get; set; }
        //    public string AssignedToName { get; set; }
        //    public AssetConditionEnum AssetConditionEnum { get; set; }
        //    public string AssetCondition { get; set; }
        //    public AssetStatusEnum AssetStatusEnum { get; set; }
        //    public string AssetStatus { get; set; }
        //    public bool AvailablityStatus { get; set; }
        //    public bool Recovered { get; set; }
        //    public bool Assigned { get; set; }
        //    public DateTime? AssignDate { get; set; }
        //    public DateTime? RecoverDate { get; set; }
        //    public string InvoiceNo { get; set; }
        //    public double Price { get; set; }
        //    public DateTime? WarentyExpDate { get; set; }
        //    public string Compliance { get; set; }
        //    public bool IsCompliance { get; set; }
        //    public string LicenseKey { get; set; }
        //    public DateTime? LicenseStartDate { get; set; }
        //    public DateTime? LicenseExpiryDate { get; set; }
        //    public int? LicApplicableCount { get; set; }
        //    public AssetsItemType AssetsType { get; set; }
        //    public string AssetsTypeName { get; set; }
        //    public string AssetsIcon { get; set; }
        //    public string Status { get; set; }
        //    public bool IsActive { get; set; }
        //    public bool IsDelete { get; set; }
        //    public int OrgID { get; set; }
        //}
        //#endregion

        #region API To Get Faulty Import Data In 
        /// <summary>
        /// Created By Harshit Mitra On 19-09-2022
        /// API >> Get >> api/assetsnew/getfaultyimport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getfaultyimport")]
        public async Task<ResponseBodyModel> GetFultyImport()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faultyReport = await _db.AssetImportFaultieGroups.Where(x => x.CompanyId ==
                        claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                if (faultyReport.Count > 0)
                {
                    res.Message = "Faulty Asset Reports";
                    res.Status = true;
                    res.Data = faultyReport;
                }
                else
                {
                    res.Message = "No Faulty Assets Imported";
                    res.Status = false;
                    res.Data = faultyReport;
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

        #region API To Get Faulty Import Data Logs
        /// <summary>
        /// Created By Harshit Mitra On 19-09-2022
        /// API >> Get >> api/assetsnew/getfaultyimportlog
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getfaultyimportlog")]
        public async Task<ResponseBodyModel> GetFultyImportLogs(Guid groupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faultyReportLog = await _db.AssetImportFaultieLogs.Include("Groups")
                        .Where(x => x.Groups.GroupId == groupId).ToListAsync();
                if (faultyReportLog.Count > 0)
                {
                    res.Message = "Faulty Asset Reports";
                    res.Status = true;
                    res.Data = faultyReportLog;
                }
                else
                {
                    res.Message = "No Faulty Assets Imported";
                    res.Status = false;
                    res.Data = faultyReportLog;
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

        #region Report Helper Class

        public class ReportHelper
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string AssetsBaseCategoryName { get; set; }
            public string AssetsCategoryName { get; set; }
            public string WareHouseName { get; set; }
            public string ItemCode { get; set; }
            public string SerialNo { get; set; }
            public string Location { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public string AssignedToName { get; set; }
            public AssetConditionConstants AssetCondition { get; set; }
            public string AssetConditionName { get; set; }
            public int AssignToId { get; set; }
            public AssetStatusConstants AssetStatus { get; set; }
            public string AssetStatusName { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? RecoverDate { get; set; }
            public double Price { get; set; }
            public AssetsItemType AssetsType { get; set; }
            public string AssetsTypeName { get; set; }
            public bool IsActive { get; set; }
            public int AssetsBaseCategoryId { get; set; }
            public int AssetsCategoryId { get; set; }
            //public int AssetType { get; set; }
            public bool IsDelete { get; set; }
            public string Descripton { get; set; }

            public int WarehouseId { get; set; }
        }
        public class ReportResponse
        {
            //public List<int> AssetsTypeId { get; set; }
            public List<AssetsItemType> AssetsType { get; set; } = new List<AssetsItemType>();
            public List<int> BaseCatrgoryId { get; set; } = new List<int>();
            public List<int> CategoryId { get; set; } = new List<int>();
            //public List<string> Condiaton { get; set; }
            public List<AssetConditionConstants> AssetConditionEnum { get; set; } = new List<AssetConditionConstants>();
            //public List<string> Status { get; set; }
            public List<AssetStatusConstants> AssetStatusEnum { get; set; } = new List<AssetStatusConstants>();
            public List<int> WarehouseId { get; set; } = new List<int>();
        }
        public class CategoryResponseDTO
        {
            public List<int> CategoryId { get; set; }
        }
        #endregion

        #region Get Reason for Asssets Status  // dropdown
        /// <summary>
        /// Created By Ravi Vyas on 19-09-2022
        /// API >> Get >>api/assetsnew/assetatatusenum
        /// Dropdown using Enum for Assets Status
        /// </summary>
        [Route("assetatatusenum")]
        [HttpGet]
        public ResponseBodyModel AssetStatusData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var assetsItemType = Enum.GetValues(typeof(AssetStatusConstants))
                    .Cast<AssetStatusConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(AssetStatusConstants), x).Replace("_", " ")
                    }).ToList();



                res.Message = "Reason list for Resignation";
                res.Status = true;
                res.Data = assetsItemType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }



        #endregion Get Reason for resignation // dropdown

        #region Api for Get Category Data by Id
        /// <summary>
        /// Created By Ravi Vyas on 19-09-2022
        /// API>>POST>>api/assetsnew/getcategories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getcategories")]
        public async Task<ResponseBodyModel> GetDataByIds(CategoryResponseDTO model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var categoryData = await (from c in _db.AssetsCategories
                                          join b in _db.AssetsBaseCategories on c.AssetsBCategoryId equals b.AssetsBCategoryId
                                          where model.CategoryId.Contains(c.AssetsBCategoryId) && c.IsActive && !c.IsDeleted && c.CompanyId == claims.companyId
                                          select new
                                          {
                                              c.AssetsCategoryId,
                                              c.AssetsCategoryName,
                                              b.AssetsBCategoryId,
                                              b.AssetsBCategoryName,
                                          }).ToListAsync();
                if (categoryData.Count > 0)
                {
                    res.Message = "Data Found !";
                    res.Status = true;
                    res.Data = categoryData;
                }
                else
                {
                    res.Message = "Data Found !";
                    res.Status = false;
                    res.Data = categoryData;
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

        #region Api for Assets Report List
        /// <summary>
        /// Created By Ravi Vyas on 19-09-2022 
        /// API>>POST>>api/assetsnew/assetsreport
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("assetsreport")]
        public async Task<ResponseBodyModel> GetDataBy(ReportResponse model, int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await (from a in _db.AssetsItemMasters
                                  join b in _db.AssetsBaseCategories on a.AssetsBaseCategoryId equals b.AssetsBCategoryId
                                  join c in _db.AssetsCategories on a.AssetsCategoryId equals c.AssetsCategoryId
                                  join w in _db.AssetsWarehouses on a.WareHouseId equals w.WarehouseId
                                  where a.IsActive && !a.IsDeleted && a.CompanyId == claims.companyId
                                  select new
                                  {
                                      ItemId = a.ItemId,
                                      ItemName = a.ItemName,
                                      AssetsBaseCategoryId = b.AssetsBCategoryId,
                                      AssetsBaseCategoryName = b.AssetsBCategoryName,
                                      AssetsCategoryId = c.AssetsCategoryId,
                                      AssetsCategoryName = c.AssetsCategoryName,
                                      ItemCode = a.ItemCode,
                                      SerialNo = a.SerialNo,
                                      Location = a.Location,
                                      PurchaseDate = a.PurchaseDate,
                                      AssignedToName = a.AssignedToName,
                                      AssignToId = a.AssignToId,
                                      AssetStatus = a.AssetStatus,
                                      AssetConditionEnum = a.AssetCondition,
                                      AssetCondition = a.AssetCondition.ToString(),
                                      AssignDate = a.AssignDate,
                                      RecoverDate = a.RecoverDate,
                                      Price = a.Price,
                                      AssetsType = a.AssetsType,
                                      AssetsTypeName = a.AssetsType.ToString(),
                                      WareHouseName = w.WarehouseName,
                                      WarehouseId = w.WarehouseId,
                                      AssetStatusName = a.AssetStatus.ToString(),
                                      a.AssetsDescription,
                                      a.Comment,
                                      a.InvoiceNo,
                                      a.WarentyExpDate,
                                      a.LicenseKey,
                                      a.LicenseStartDate,
                                      a.LicenseExpiryDate,
                                      a.LicApplicableCount,
                                      a.Compliance,
                                      Officeemail = a.AssignToId == 0 ? "" : _db.Employee.Where(x => x.EmployeeId == a.AssignToId).Select(x => x.OfficeEmail).FirstOrDefault(),

                                  }).ToListAsync();
                if (data.Count > 0)
                {
                    var predicate = PredicateBuilder.New<ReportHelper>(x => x.IsActive && !x.IsDelete);
                    if (model.AssetsType.Count > 0)
                    {
                        data = (data.Where(x => model.AssetsType.Contains(x.AssetsType))).ToList();
                    }
                    if (model.BaseCatrgoryId.Count > 0)
                    {
                        data = (data.Where(x => model.BaseCatrgoryId.Contains(x.AssetsBaseCategoryId))).ToList();
                    }
                    if (model.CategoryId.Count > 0)
                    {
                        data = (data.Where(x => model.CategoryId.Contains(x.AssetsCategoryId))).ToList();
                    }
                    if (model.AssetConditionEnum.Count > 0)
                    {
                        data = (data.Where(x => model.AssetConditionEnum.Contains(x.AssetConditionEnum))).ToList();
                    }
                    if (model.AssetStatusEnum.Count > 0)
                    {
                        data = (data.Where(x => model.AssetStatusEnum.Contains(x.AssetStatus))).ToList();
                    }
                    if (model.WarehouseId.Count > 0)
                    {
                        data = (data.Where(x => model.WarehouseId.Contains(x.WarehouseId))).ToList();
                    }
                }
                if (data.Count > 0)
                {
                    res.Message = "Assets list Found";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {

                        res.Data = new PaginationData
                        {
                            TotalData = data.Count,
                            Counts = (int)count,
                            List = data.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = data;
                    }
                }
                else
                {
                    res.Message = "Assets list Not Found";
                    res.Status = true;
                    res.Data = data;
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

        #region This Api Is Used For Get All Api For Use In Assets Dashboard

        /// <summary>
        /// Created By Shriya Malvi On 15-07-2022
        /// Api >> Get >> api/assetsnew/getallassetsdashboard
        /// </summary>
        [HttpGet]
        [Route("getallassetsdashboard")]
        public async Task<ResponseBodyModel> GetAllAssetsForDashboard()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            DashboardAsset response = new DashboardAsset();
            List<AssetBarModel> AssetBarChart = new List<AssetBarModel>();
            List<AssetPichart> AssetPiChart = new List<AssetPichart>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var assets = await _db.AssetsItemMasters.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();

            #region Upper Box Data

            // Top Of Box
            response.TotalPhysicalAssets = assets.Count(x => x.AssetsType == AssetsItemType.Physical);
            response.TotalPhysicalAssigned = assets.Count(x => x.AssetsType == AssetsItemType.Physical && x.AssetStatus == AssetStatusConstants.Assigned);
            response.TotalPhyAssignedAmount = assets.Where(x => x.AssetsType == AssetsItemType.Physical && x.AssetStatus == AssetStatusConstants.Assigned).Sum(x => x.Price);

            response.TotalPhysicalAvailable = assets.Count(x => x.AssetsType == AssetsItemType.Physical && x.AssetStatus == AssetStatusConstants.Available);
            response.TotalPhyAvaliableAmount = assets.Where(x => x.AssetsType == AssetsItemType.Physical && x.AssetStatus == AssetStatusConstants.Available).Sum(x => x.Price);

            response.TotalPhysicalUnderRepair = assets.Count(x => x.AssetsType == AssetsItemType.Physical && x.AssetStatus == AssetStatusConstants.UnderRepair);
            response.TotalPhyUnderRepairAmount = assets.Where(x => x.AssetsType == AssetsItemType.Physical && x.AssetStatus == AssetStatusConstants.UnderRepair).Sum(x => x.Price);

            // Lower Box
            response.TotalDamageAsset = assets.Count(x => x.AssetStatus == AssetStatusConstants.Damage);
            response.TotalDamageAssetAmount = assets.Where(x => x.AssetStatus == AssetStatusConstants.Damage).Sum(x => x.Price);

            response.TotalDigitalAssets = assets.Count(x => x.AssetsType == AssetsItemType.Digital);
            response.TotalDigitalAssigned = assets.Count(x => x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Assigned);
            response.TotalDigitalAssignedAmount = assets.Where(x => x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Assigned).Sum(x => x.Price);

            response.TotalDigitalAvailable = assets.Count(x => x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Available);
            response.TotalDigitalAvailableAmount = assets.Where(x => x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Available).Sum(x => x.Price);

            #endregion

            #region Lower Box Data

            response.TotalAssetAmount = assets.Sum(x => x.Price);
            response.UnusedAssetAmount = assets.Where(x => x.AssetStatus == AssetStatusConstants.Available).Sum(x => x.Price);
            response.PhysicalAssetAmount = assets.Where(x => x.AssetsType == AssetsItemType.Physical).Sum(x => x.Price);
            response.DigitalAssetAmount = assets.Where(x => x.AssetsType == AssetsItemType.Digital).Sum(x => x.Price);

            #endregion

            #region Total Asset Line Chart

            var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                     .TakeWhile(m => m != String.Empty)
                                     .Select((m, i) => new
                                     {
                                         Month = i + 1,
                                         MonthName = m
                                     }).ToList();

            var monthNames = months.ConvertAll(x => x.MonthName);

            var listDynamic = new List<dynamic>();
            var intList1 = new List<int>();
            var data1 = new
            {
                label = "Good and Fair",
                data = intList1,
            };
            var intList2 = new List<int>();
            var data2 = new
            {
                label = "Damage",
                data = intList2,
            };

            foreach (var data in months)
            {
                intList1.Add(assets.Where(x => x.CreatedOn.Month == data.Month && x.AssetCondition == AssetConditionConstants.Good && x.AssetCondition == AssetConditionConstants.UnderRepair).ToList().Count);
                intList2.Add(assets.Where(x => x.CreatedOn.Month == data.Month && x.AssetCondition == AssetConditionConstants.Damage).ToList().Count);
            }
            listDynamic.Add(data1);
            listDynamic.Add(data2);
            response.TotalAssetLineChart = new
            {
                Label = monthNames,
                LineData = listDynamic,
            };

            #endregion

            #region This Api Use For Bar Chart In Asset

            var Total = _db.AssetsCategories.Where(x => !x.IsDeleted &&
                    x.IsActive && x.CompanyId == claims.companyId).ToList().Distinct();

            foreach (var item in Total)
            {
                List<Condition> ListCon = new List<Condition>();
                AssetBarModel obj = new AssetBarModel
                {
                    Name = Total.Where(x => x.AssetsCategoryId == item.AssetsCategoryId).Select(x => x.AssetsCategoryName).FirstOrDefault(),
                    Series = new List<Condition>(),
                };

                Condition con1 = new Condition
                {
                    Name = "Good and Fair",
                    Value = assets.Count(x => x.AssetCondition != AssetConditionConstants.Damage && x.AssetsCategoryId == item.AssetsCategoryId)
                };
                ListCon.Add(con1);
                Condition con2 = new Condition
                {
                    Name = "Damage",
                    Value = assets.Count(x => x.AssetCondition == AssetConditionConstants.Damage && x.AssetsCategoryId == item.AssetsCategoryId)
                };
                ListCon.Add(con2);
                obj.Series = ListCon;
                AssetBarChart.Add(obj);
            }
            response.ChartAssets = AssetBarChart;

            #endregion This Api Use For Bar Chart In Asset

            #region Api for Chart 

            response.Seriess = new List<AssetPichart>
                {
                    new AssetPichart
                    {
                        Name = "Physical Assets",
                        Value = assets.Count(x=> x.AssetsType == AssetsItemType.Physical),
                    },
                    new AssetPichart
                    {
                        Name = "Digital Assets",
                        Value = assets.Count(x=> x.AssetsType == AssetsItemType.Digital ),
                    },
                    new AssetPichart
                    {
                         Name = "Damage Assets",
                         Value = assets.Count(x=> x.AssetStatus == AssetStatusConstants.Damage ),
                    },
                    new AssetPichart
                    {
                         Name = "Assets Physical UnderRepair",
                         Value = assets.Count(x=> x.AssetStatus == AssetStatusConstants.UnderRepair && x.AssetsType == AssetsItemType.Physical),
                    }
                };

            #endregion

            #region Api for Chart Digital Assest Available And Assinged

            response.AssetsSeriess = new List<AssetPichart>
                {
                    new AssetPichart
                    {
                        Name = "Digital Assest Available",
                        Value = assets.Count(x=> x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Available),
                    },
                    new AssetPichart
                    {
                        Name = "Digital Assets Assinged",
                        Value = assets.Count(x=> x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Assigned),
                    },

                };

            #endregion

            #region Line Chart Data Digital And Assigned Assets

            var listDynamicData = new List<dynamic>();
            var intListData1 = new List<int>();
            var dataNew1 = new
            {
                label = "Digital Assest Available",
                data = intListData1,
            };
            var intListData2 = new List<int>();
            var dataNew2 = new
            {
                label = "Digital Assets Assinged",
                data = intListData2,
            };

            foreach (var data in months)
            {
                intListData1.Add(assets.Where(x => x.CreatedOn.Month == data.Month && x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Available).ToList().Count);
                intListData2.Add(assets.Where(x => x.CreatedOn.Month == data.Month && x.AssetsType == AssetsItemType.Digital && x.AssetStatus == AssetStatusConstants.Assigned).ToList().Count);
            }
            listDynamicData.Add(dataNew1);
            listDynamicData.Add(dataNew2);
            response.LineChartAssets = new
            {
                Label = monthNames,
                LineData = listDynamicData,
            };

            #endregion Line Chart Data Digital And Assigned Assets

            #region Total Asset Amount On Behalf Of Month Add

            var listDynamicData2 = new List<dynamic>();
            var intListData3 = new List<double>();
            var dataNew3 = new
            {
                label = "Physical Assest Amount",
                data = intListData3,
            };
            var intListData4 = new List<double>();
            var dataNew4 = new
            {
                label = "Digital Assets Amount",
                data = intListData4,
            };

            foreach (var data in months)
            {
                intListData3.Add(assets.Where(x => x.CreatedOn.Month == data.Month && x.AssetsType == AssetsItemType.Physical).Sum(x => x.Price));
                intListData4.Add(assets.Where(x => x.CreatedOn.Month == data.Month && x.AssetsType == AssetsItemType.Digital).Sum(x => x.Price));
            }
            listDynamicData2.Add(dataNew3);
            listDynamicData2.Add(dataNew4);
            response.LineChartAmountGraph = new
            {
                Label = monthNames,
                LineData = listDynamicData2,
            };

            #endregion

            res.Message = "All Assets Available";
            res.Status = true;
            res.Data = response;
            return res;
        }
        public class DashboardAsset
        {
            public int TotalPhysicalAssets { get; set; }
            public int TotalPhysicalAssigned { get; set; }
            public double TotalPhyAssignedAmount { get; set; }
            public int TotalPhysicalAvailable { get; set; }
            public double TotalPhyAvaliableAmount { get; set; }
            public int TotalPhysicalUnderRepair { get; set; }
            public double TotalPhyUnderRepairAmount { get; set; }
            public int TotalDamageAsset { get; set; }
            public double TotalDamageAssetAmount { get; set; }
            public int TotalDigitalAssets { get; set; }
            public int TotalDigitalAssigned { get; set; }
            public double TotalDigitalAssignedAmount { get; set; }
            public int TotalDigitalAvailable { get; set; }
            public double TotalDigitalAvailableAmount { get; set; }
            public double TotalAssetAmount { get; set; }
            public double UnusedAssetAmount { get; set; }
            public double PhysicalAssetAmount { get; set; }
            public double DigitalAssetAmount { get; set; }
            public object TotalAssetLineChart { get; set; }
            public object ChartAssets { get; set; }
            public object Seriess { get; set; }
            public object AssetsSeriess { get; set; }
            public object LineChartAssets { get; set; }
            public object LineChartAmountGraph { get; set; }
        }
        public class AssetBarModel
        {
            public string Name { get; set; }
            public List<Condition> Series { get; set; }
        }
        public class AssetPichart
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        #endregion This Api Is Used For Get All Api For Use In Assets Dashboard

    }
}