using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/ItemMasterCategories")]
    public class ItemMasterCategoriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        #region This Api IS Used for Get Base Category

        /// <summary>
        /// Created by Nayan Pancholi on 06-04-2022
        /// Api >> Get >> api/ItemMasterCategories/GetItemMasterBaseCategory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItemMasterBaseCategory")]
        public async Task<ResponseBodyModel> GetItemMasterBaseCategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var basecategory = await db.ItemMasterBaseCategory.Where(x => x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                if (basecategory != null)
                {
                    res.Message = "BaseCategory found successfully";
                    res.Status = true;
                    res.Data = basecategory;
                }
                else
                {
                    res.Message = "BaseCategory Not Found";
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

        #endregion This Api IS Used for Get Base Category

        #region This Api is Used for Add Base Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Modified by Suraj Bundel on 01/07/2022
        /// Api >> Post >> api/ItemMasterCategories/AddItemMasterBaseCategory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddItemMasterBaseCategory")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> AddItemMasterBaseCategory(ItemMasterBaseCategory item)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (item != null)
                {
                    item.CreatedOn = DateTime.Now;
                    item.UpdatedOn = DateTime.Now;
                    item.IsActive = true;
                    item.IsDeleted = false;
                    item.CompanyId = claims.companyId;
                    item.OrgId = claims.orgId;
                    item.CreatedBy = claims.employeeId;
                    db.ItemMasterBaseCategory.Add(item);
                    int id = db.SaveChanges();

                    res.Message = "BaseCategory created successfully";
                    res.Status = true;
                    res.Data = item;
                }
                else
                {
                    res.Message = "BaseCategory Not Added";
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

        #endregion This Api is Used for Add Base Category

        #region This Api is Used for Edit Base Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> put >> api/ItemMasterCategories/EditItemMasterBasecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPut]
        [Route("EditItemMasterBasecategory")]
        public async Task<ResponseBodyModel> EditItemMasterBasecategory(ItemMasterBaseCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var Basecat = await db.ItemMasterBaseCategory.FirstOrDefaultAsync(x => x.BaseCategoryId == model.BaseCategoryId);
                if (Basecat != null)
                {
                    Basecat.BaseCategoryName = model.BaseCategoryName;
                    Basecat.BaseCategoryDescription = model.BaseCategoryDescription;
                    Basecat.BaseCategoryCode = model.BaseCategoryCode;
                    Basecat.UpdatedOn = DateTime.Now;

                    db.Entry(Basecat).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "BaseCategory Updated";
                    res.Status = true;
                    res.Data = Basecat;
                }
                else
                {
                    res.Message = "BaseCategory Not Found";
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

        #endregion This Api is Used for Edit Base Category

        #region This Api Is Used for Delete Base Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Delete >> api/ItemMasterCategories/DeleteItemMasterBaseCategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DeleteItemMasterBaseCategory")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteItemMasterBaseCategory(int id)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                ItemMasterBaseCategory category = db.ItemMasterBaseCategory.Where(x => x.BaseCategoryId == id && x.IsDeleted == false).FirstOrDefault();
                if (category != null)
                {
                    category.IsDeleted = true;
                    category.IsActive = false;
                    category.UpdatedOn = DateTime.Now;
                    db.ItemMasterBaseCategory.Attach(category);
                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Message = "BaseCategory Deleted successfully";
                    res.Status = true;
                    res.Data = category;
                }
                else
                {
                    res.Message = "BaseCategory Not Found";
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

        #endregion This Api Is Used for Delete Base Category

        #region This Api Is Used for Get Base Category By ID

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Get >> api/ItemMasterCategories/GetBaseCategoryByID
        /// </summary>
        /// <param name="BaseCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBaseCategoryByID")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetBaseCategoryByID(int BaseCategoryId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                List<ItemMasterBaseCategory> basecategoryList = new List<ItemMasterBaseCategory>();
                var BaseCategoryData = (from ad in db.ItemMasterBaseCategory
                                        where ad.BaseCategoryId == BaseCategoryId
                                        where ad.IsActive == true
                                        select new
                                        {
                                            ad.BaseCategoryId,
                                            ad.BaseCategoryName,
                                            ad.BaseCategoryDescription,
                                            ad.BaseCategoryCode,
                                        }).ToList();
                foreach (var item in BaseCategoryData)
                {
                    ItemMasterBaseCategory data = new ItemMasterBaseCategory();
                    data.BaseCategoryId = item.BaseCategoryId;
                    data.BaseCategoryName = item.BaseCategoryName;
                    data.BaseCategoryCode = item.BaseCategoryCode;
                    data.BaseCategoryDescription = item.BaseCategoryDescription;
                    basecategoryList.Add(data);
                    res.Message = "BaseCategory Found Successfully";
                    res.Status = true;
                    res.Data = BaseCategoryData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Is Used for Get Base Category By ID

        #region This Api Is Used to Add Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> post >> api/ItemMasterCategories/AddItemMasterCategory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("AddItemMasterCategory")]
        [HttpPost]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> AddItemMasterCategory(ItemMasterCategory item)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                if (item != null)
                {
                    db.AddItemMasterCategory(item);
                    res.Message = "Category added successfully";
                    res.Status = true;
                    res.Data = item;
                }
                else
                {
                    res.Message = "Category Not Found";
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

        #endregion This Api Is Used to Add Category

        #region This Api Is Used to Get Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Get >> api/ItemMasterCategories/GetItemMasterCategory
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItemMasterCategory")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetItemMasterCategory()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                //var category = db.ItemMasterCategory.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
                var category = (from ad in db.ItemMasterCategory
                                join cb in db.ItemMasterBaseCategory on ad.BaseCategoryId equals cb.BaseCategoryId
                                where ad.IsActive == true && ad.IsDeleted == false
                                select new GetCategoryListDTO
                                {
                                    Categoryid = ad.Categoryid,
                                    CategoryName = ad.CategoryName,
                                    BaseCategoryId = cb.BaseCategoryId,
                                    BaseCategoryName = cb.BaseCategoryName,
                                    CategoryDescription = ad.CategoryDescription,
                                    CategoryCode = ad.CategoryCode,
                                }).ToList();

                if (category != null)
                {
                    res.Message = "Category found successfully";
                    res.Status = true;
                    res.Data = category;
                }
                else
                {
                    res.Message = "Category Not Found";
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

        #endregion This Api Is Used to Get Category

        #region This Api is Used for Delete Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Delete >> api/ItemMasterCategories/DeleteCategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteCategory")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteCategory(int id)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                ItemMasterCategory category = db.ItemMasterCategory.Where(x => x.Categoryid == id && x.IsDeleted == false).FirstOrDefault();
                if (category != null)
                {
                    category.IsDeleted = true;
                    category.IsActive = false;
                    db.ItemMasterCategory.Attach(category);

                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    res.Status = true;
                    res.Message = "Category Deleted Successfully!";
                    res.Data = category;
                }
                else
                {
                    res.Message = "Category Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                //   //logger.Error("Error in del Category " + ex.Message);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return res;
        }

        #endregion This Api is Used for Delete Category

        #region This Api Is Used to Update Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Put >> api/ItemMasterCategories/EditItemMastercategory
        /// </summary>
        /// <param name="createcategory"></param>
        /// <returns></returns>
        [Route("EditItemMastercategory")]
        [HttpPut]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> EditItemMastercategory(ItemMasterCategory createcategory)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var updateDepData = db.ItemMasterCategory.Where(x => x.Categoryid == createcategory.Categoryid && x.IsDeleted == false).FirstOrDefault();
                if (updateDepData != null)
                {
                    updateDepData.Categoryid = createcategory.Categoryid;
                    updateDepData.BaseCategoryId = createcategory.BaseCategoryId;
                    var cd = db.ItemMasterBaseCategory.Where(x => x.BaseCategoryId == createcategory.BaseCategoryId).FirstOrDefault();
                    if (cd != null)
                    {
                        updateDepData.BaseCategoryId = cd.BaseCategoryId;
                    }

                    //updateDepData.CategoryName = createcategory.CategoryName;
                    updateDepData.CategoryName = createcategory.CategoryName;
                    updateDepData.CategoryDescription = createcategory.CategoryDescription;
                    updateDepData.CategoryCode = createcategory.CategoryCode;
                    updateDepData.IsActive = true;
                    updateDepData.IsDeleted = false;

                    db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Status = true;
                    res.Message = " Updated Successfully!";
                    res.Data = updateDepData;
                }
                else
                {
                    res.Message = "Not Upadated";
                    res.Status = false;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Is Used to Update Category

        #region This Api Is Used to Add Sub Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Post >> api/ItemMasterCategories/AddItemMasterSubCategory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("AddItemMasterSubCategory")]
        [HttpPost]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> AddItemMasterSubCategory(ItemMasterSubCategory item)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                if (item != null)
                {
                    db.AddItemMasterSubCategory(item);
                    res.Message = "SubCategory added successfully";
                    res.Status = true;
                    res.Data = item;
                }
                else
                {
                    res.Message = "SubCategory Not Found";
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

        #endregion This Api Is Used to Add Sub Category

        #region This Api is Used to Get SubCategory

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Get >> api/ItemMasterCategories/GetItemMasterSubCategory
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItemMasterSubCategory")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetItemMasterSubCategory()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var category = (from ad in db.itemMasterSubCategory
                                join cb in db.ItemMasterCategory on ad.Categoryid equals cb.Categoryid
                                where ad.IsActive == true && ad.IsDeleted == false
                                select new GetSubCategoryListDTO
                                {
                                    SubCategoryId = ad.SubCategoryId,
                                    SubcategoryName = ad.SubcategoryName,
                                    SubCategoryDescription = ad.SubCategoryDescription,
                                    SubCategoryCode = ad.SubCategoryCode,
                                    Categoryid = cb.Categoryid,
                                    CategoryName = cb.CategoryName,
                                }).ToList();

                if (category != null)
                {
                    res.Message = "SubCategory found successfully";
                    res.Status = true;
                    res.Data = category;
                }
                else
                {
                    res.Message = "SubCategory Not Found";
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

        #endregion This Api is Used to Get SubCategory

        #region This Api Is Used for Delete Sub Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Delete >> api/ItemMasterCategories/DeleteSubCategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteSubCategory")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteSubCategory(int id)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                ItemMasterSubCategory category = db.itemMasterSubCategory.Where(x => x.SubCategoryId == id && x.IsDeleted == false).FirstOrDefault();
                if (category != null)
                {
                    category.IsDeleted = true;
                    category.IsActive = false;
                    db.itemMasterSubCategory.Attach(category);

                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    res.Status = true;
                    res.Message = "SubCategory Deleted Successfully!";
                    res.Data = category;
                }
                else
                {
                    res.Message = "SubCategory Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return res;
        }

        #endregion This Api Is Used for Delete Sub Category

        #region This Api Is used to Update Sub Category

        /// <summary>
        /// Created by Nayan Pancholi on 6-4-2022
        /// Api >> Put >> api/ItemMasterCategories/EditItemMasterSubcategory
        /// </summary>
        /// <param name="createcategory"></param>
        /// <returns></returns>
        [Route("EditItemMasterSubcategory")]
        [HttpPut]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> EditItemMasterSubcategory(ItemMasterSubCategory createcategory)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var updateDepData = db.itemMasterSubCategory.Where(x => x.SubCategoryId == createcategory.SubCategoryId && x.IsDeleted == false).FirstOrDefault();
                if (updateDepData != null)
                {
                    updateDepData.SubCategoryId = createcategory.SubCategoryId;
                    updateDepData.Categoryid = createcategory.Categoryid;
                    var cd = db.ItemMasterCategory.Where(x => x.Categoryid == createcategory.Categoryid).FirstOrDefault();
                    if (cd != null)
                    {
                        updateDepData.Categoryid = cd.Categoryid;
                    }

                    updateDepData.SubcategoryName = createcategory.SubcategoryName;
                    updateDepData.SubCategoryDescription = createcategory.SubCategoryDescription;
                    updateDepData.SubCategoryCode = createcategory.SubCategoryCode;
                    updateDepData.IsActive = true;
                    updateDepData.IsDeleted = false;

                    db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Status = true;
                    res.Message = "Updated Successfully!";
                    res.Data = updateDepData;
                }
                else
                {
                    res.Message = "Data Not Updated";
                    res.Status = false;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Is used to Update Sub Category

        public class GetCategoryListDTO
        {
            public int Categoryid { get; set; }
            public string CategoryName { get; set; }
            public string CategoryDescription { get; set; }
            public string CategoryCode { get; set; }
            public int BaseCategoryId { get; set; }
            public string BaseCategoryName { get; set; }
        }

        public class GetSubCategoryListDTO
        {
            public int SubCategoryId { get; set; }
            public string SubcategoryName { get; set; }
            public string SubCategoryDescription { get; set; }
            public string SubCategoryCode { get; set; }
            public int Categoryid { get; set; }
            public string CategoryName { get; set; }
        }
    }
}