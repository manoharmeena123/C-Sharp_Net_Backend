using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/urlcategory")]
    public class URLCategoryController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //        #region #Get by URL Category Id API

        //        [Route("GetURLCategoryById")]
        //        [HttpGet]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> GetCategoryURLById(int Id)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            try
        //            {
        //                //Base response = new Base();

        //                var urldata = db.CategoryURLs.Where(x => x.CategoryURLId == Id && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
        //                if (urldata != null)
        //                {
        //                    res.Status = true;
        //                    res.Message = "URL Found";
        //                    res.Data = urldata;
        //                }
        //                else
        //                {
        //                    res.Status = false;
        //                    res.Message = "No URL Found!!";
        //                    res.Data = null;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        #endregion #Get by URL Category Id API

        //        #region #Get All URL Category API

        //        [Route("GetAllURLCategory")]
        //        [HttpGet]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> GetAllCategoryURL()
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            try
        //            {
        //                //Base response = new Base();

        //                //var urldata = db.CategoryURLs.Where(x => x.IsDeleted == false).ToList();
        //                var urldata = (from C in db.CategoryURLs
        //                               join D in db.Department on C.DepartmentId equals D.DepartmentId
        //                               where C.IsDeleted == false
        //                               select new URlDto
        //                               {
        //                                   CategoryURLId = C.CategoryURLId,
        //                                   CategoryName = C.CategoryName,
        //                                   CategoryUrl = C.CategoryUrl,
        //                                   DepartmentId = D.DepartmentId,
        //                                   DepartmentName = D.DepartmentName,
        //                                   IsProductive = C.IsProductive,
        //                                   TrackingCategoryId = C.TrackingCategoryId,
        //                                   TrackingCategoryName = C.TrackingCategoryName,
        //                                   IsNonProductive = C.IsNonProductive,
        //                                   IsBlock = C.IsBlock,
        //                                   IsActive = true,
        //                                   IsDeleted = false,
        //                               }).ToList();

        //                if (urldata != null)
        //                {
        //                    res.Status = true;
        //                    res.Message = "URL Found";
        //                    res.Data = urldata;
        //                }
        //                else
        //                {
        //                    res.Status = false;
        //                    res.Message = "No URL Found!!";
        //                    res.Data = null;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        #endregion #Get All URL Category API

        //        #region #Post URL Category API

        //        [Route("CreateURLCategory")]
        //        [HttpPost]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> CreateCategoryURL(CategoryURL createcategory)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            try
        //            {
        //                //Base response = new Base();

        //                var urldata = db.CategoryURLs.Where(x => x.TrackingCategoryId == createcategory.TrackingCategoryId && x.IsDeleted == false).ToList();
        //#pragma warning disable CS0472 // The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
        //                if (urldata.Count != null)
        //                {
        //                    CategoryURL newcategory = new CategoryURL();

        //                    newcategory.CategoryName = createcategory.CategoryName;
        //                    newcategory.CategoryUrl = createcategory.CategoryUrl;
        //                    newcategory.DepartmentName = createcategory.DepartmentName;
        //                    var url = (from x in db.Department where x.DepartmentName.Trim().ToUpper() == createcategory.DepartmentName.Trim().ToUpper() select x).SingleOrDefault();
        //                    int DepartmentId = 0;
        //                    if (url != null)
        //                    {
        //                        DepartmentId = url.DepartmentId;
        //                    }
        //                    else
        //                    {
        //                    }

        //                    newcategory.DepartmentId = createcategory.DepartmentId;
        //                    newcategory.IsProductive = createcategory.IsProductive;
        //                    newcategory.TrackingCategoryId = createcategory.TrackingCategoryId;
        //                    newcategory.TrackingCategoryName = createcategory.TrackingCategoryName;
        //                    newcategory.IsNonProductive = createcategory.IsNonProductive;
        //                    newcategory.IsBlock = createcategory.IsBlock;
        //                    newcategory.CreatedOn = DateTime.Now;
        //                    newcategory.IsActive = true;
        //                    newcategory.IsDeleted = false;

        //                    db.CategoryURLs.Add(newcategory);
        //                    db.SaveChanges();

        //                    res.Status = true;
        //                    res.Message = "URL added Successfully!";
        //                    res.Data = newcategory;
        //                }
        //                else
        //                {
        //                    res.Status = false;
        //                    res.Message = "URL already added!";
        //                    res.Data = null;
        //                }
        //#pragma warning restore CS0472 // The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
        //            }
        //            catch (Exception ex)
        //            {
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        #endregion #Post URL Category API

        #region #Post URL Category API

        [Route("CreateTrackingURL")]
        [HttpPost]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> CreateTrackingId(URLTracking createcategory)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                //Base response = new Base();

                URLTracking category = new URLTracking();

                category.TrackingCategoryId = createcategory.TrackingCategoryId;
                category.CategoryName = createcategory.CategoryName;
                category.CreatedOn = DateTime.Now;
                category.IsActive = true;
                category.IsDeleted = false;

                db.URLTrackings.Add(category);
                db.SaveChanges();

                CategoryURL categories = new CategoryURL();

                categories.TrackingCategoryId = category.TrackingCategoryId;
                categories.CategoryName = createcategory.CategoryName;
                categories.CreatedOn = DateTime.Now;
                categories.CreatedBy = createcategory.CreatedBy;
                categories.IsActive = true;
                categories.IsDeleted = false;

                db.CategoryURLs.Add(categories);
                db.SaveChanges();
                res.Status = true;
                res.Message = "URL added Successfully!";
                res.Data = category;
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
            return res;
        }

        #endregion #Post URL Category API

        #region #Get All Tracking URL API

        [Route("GetAllTrackingURL")]
        [HttpGet]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetAllTrackingURL()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                //Base response = new Base();

                var urldata = db.URLTrackings.Where(x => x.IsDeleted == false).ToList();
                if (urldata != null)
                {
                    res.Status = true;
                    res.Message = "URL Found";
                    res.Data = urldata;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No URL Found!!";
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

        #endregion #Get All Tracking URL API

        //        #region #PUT URL Category API

        //        [Route("UpdateURLCategory")]
        //        [HttpPut]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> UpdateUrl(CategoryURL createcategory)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            try
        //            {
        //                var updateDepData = db.CategoryURLs.Where(x => x.CategoryURLId == createcategory.CategoryURLId && x.IsDeleted == false).FirstOrDefault();
        //                if (updateDepData != null)
        //                {
        //                    updateDepData.CategoryURLId = createcategory.CategoryURLId;
        //                    updateDepData.TrackingCategoryId = createcategory.TrackingCategoryId;
        //                    var cd = db.URLTrackings.Where(x => x.TrackingCategoryId == createcategory.TrackingCategoryId).FirstOrDefault();
        //                    if (cd != null)
        //                    {
        //                        updateDepData.TrackingCategoryName = cd.CategoryName;
        //                    }

        //                    //updateDepData.CategoryName = createcategory.CategoryName;
        //                    updateDepData.CategoryUrl = createcategory.CategoryUrl;
        //                    if (createcategory.DepartmentId > 0)
        //                    {
        //                        var depart = db.Department.Where(x => x.DepartmentId == createcategory.DepartmentId).FirstOrDefault();
        //                        updateDepData.DepartmentId = depart.DepartmentId;
        //                        updateDepData.DepartmentName = depart.DepartmentName;
        //                    }
        //                    updateDepData.IsProductive = createcategory.IsProductive;
        //                    updateDepData.IsNonProductive = createcategory.IsNonProductive;
        //                    updateDepData.IsBlock = createcategory.IsBlock;
        //                    updateDepData.IsActive = true;
        //                    updateDepData.IsDeleted = false;

        //                    db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
        //                    db.SaveChanges();

        //                    res.Status = true;
        //                    res.Message = "URL Updated Successfully!";
        //                    res.Data = updateDepData;
        //                }
        //                else
        //                {
        //                    res.Message = "No Attendance Available";
        //                    res.Status = false;
        //                    res.Data = null;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                res.Data = null;
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        #endregion #PUT URL Category API

        //        #region #Delete URL Category API

        //        [Route("DeleteURLCategory")]
        //        [HttpDelete]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> DeleteCategoryURL(int CategoryURLId)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            ResponseBodyModel response = new ResponseBodyModel();
        //            try
        //            {
        //                var deleteData = db.CategoryURLs.Where(x => x.CategoryURLId == CategoryURLId).FirstOrDefault();
        //                if (deleteData != null)
        //                {
        //                    deleteData.IsDeleted = true;
        //                    deleteData.IsActive = false;
        //                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
        //                    db.SaveChanges();
        //                    response.Status = true;
        //                    response.Message = "CategoryURL Deleted Successfully!";
        //                }
        //                else
        //                {
        //                    response.Status = false;
        //                    response.Message = "No URL Found!!";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                response.Data = null;
        //                response.Message = ex.Message;
        //                response.Status = false;
        //                return response;
        //            }
        //            return response;
        //        }

        //        #endregion #Delete URL Category API

        #region PUT URL Tracking API

        [Route("UpdateTrackingURL")]
        [HttpPut]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> UpdateTrackingURL(CategoryURL createcategory)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var updateDepData = db.URLTrackings.Where(x => x.TrackingCategoryId == createcategory.TrackingCategoryId && x.IsDeleted == false).FirstOrDefault();
                if (updateDepData != null)
                {
                    updateDepData.CategoryName = createcategory.CategoryName;

                    updateDepData.IsActive = true;
                    updateDepData.IsDeleted = false;

                    db.SaveChanges();

                    response.Status = true;
                    response.Message = "URL Updated Successfully!";
                    response.Data = updateDepData;
                }
                else
                {
                    response.Status = false;
                    response.Message = "URL Updated faild!";
                    response.Data = null;
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

        #endregion PUT URL Tracking API

        #region #Delete TrackingURL API

        [Route("DeleteTrackingURL")]
        [HttpDelete]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteTrackingURLL(int TrackingCategoryId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var deleteData = db.URLTrackings.Where(x => x.TrackingCategoryId == TrackingCategoryId).FirstOrDefault();
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.Status = true;
                    response.Message = "Deleted Successfully!";
                }
                else
                {
                    response.Status = false;
                    response.Message = "No URL Found!!";
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

        #endregion #Delete TrackingURL API

        public class URLData
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public CategoryURL Category { get; set; }
        }

        public class URLDatas
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public URLTracking Categorylist { get; set; }
        }

        public class URLDataList
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<CategoryURL> CategoryList { get; set; }
        }

        public class URLDataLists
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<URLTracking> CategoryLists { get; set; }
        }

        public class URLDRes
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<URlDto> URlDto { get; set; }
        }

        public class URlDto
        {
            public int CategoryURLId { get; set; }
            public string CategoryName { get; set; }

            public string CategoryUrl { get; set; }
            public int DepartmentId { get; set; }

            public string DepartmentName { get; set; }

            //public object Department { get; internal set; }
            public bool IsProductive { get; set; }

            public int TrackingCategoryId { get; set; }
            public string TrackingCategoryName { get; set; }
            public bool IsNonProductive { get; set; }
            public bool IsBlock { get; set; }

            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}