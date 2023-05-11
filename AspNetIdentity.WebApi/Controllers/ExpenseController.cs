using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
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
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/expense")]
    public class ExpenseController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Get Expense Catyegory By Id // Commented Code

        /* Get Expense Catyegory By Id*/

        //[Route("GetExpenseCategoryId")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetExpenseCategoryId(int Id)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        //Base response = new Base();
        //        ExpenseCategoryData expensedata = new ExpenseCategoryData();
        //        var ExpensesData = db.ExpenseCategory.Where(x => x.CategoryId == Id).FirstOrDefault();
        //        if (ExpensesData != null)
        //        {
        //            expensedata.Status = "OK";
        //            expensedata.Message = "ExpensesCategory Found";
        //            expensedata.expense = ExpensesData;

        //        }
        //        else
        //        {
        //            expensedata.Status = "Error";
        //            expensedata.Message = "No ExpensesCategory Found!!";
        //            expensedata.expense = null;
        //        }
        //        return Ok(expensedata);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        #endregion Get Expense Catyegory By Id // Commented Code

        #region Get All Expense Category //Commented Code

        ///* Get All Expense Category */

        //[Route("" +
        //    "GetAllExpenseCategory")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetAllExpenseCategory()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        ExpenseCategoryDataList exc = new ExpenseCategoryDataList();
        //        var expenseData = db.ExpenseCategory.Where(x => x.IsDeleted == false).ToList();
        //        if (expenseData.Count != 0)
        //        {
        //            exc.Status = "OK";
        //            exc.Message = "ExpenseCatagory list Found";
        //            exc.expensecatlist = expenseData;
        //        }
        //        else
        //        {
        //            exc.Status = "Not OK";
        //            exc.Message = "No Department list Found";
        //            exc.expensecatlist = null;
        //        }
        //        return Ok(exc);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        #endregion Get All Expense Category //Commented Code

        #region Get all Expense Category list // by Employee

        /// <summary>
        /// Created by Suraj Bundel on 24/05/2022
        /// Update by Suraj Bundel on 02/06/2022
        /// API >> Get >> api/expense/getallexpenseenterylist
        /// </summary>
        /// use to get the Expense Entry List
        /// <returns></returns>

        [Route("getallexpenseenterylist")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> Getallexpenseenterylist(int? page = null, int? count = null)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var allCurrency = CurrencyHelper.GetCurrencyName().AsEnumerable();
                var brach = await (from EE in _db.ExpenseEntry
                                   join Emp in _db.Employee on EE.CreatedBy equals Emp.EmployeeId
                                   where EE.CreatedBy == claims.employeeId &&
                                    EE.IsActive == true && EE.IsDeleted == false && EE.CompanyId == claims.companyId && Emp.EmployeeId == claims.employeeId
                                   select new Getallexpensedata
                                   {
                                       ExpenseId = EE.ExpenseId,
                                       CategoryId = EE.CategoryId,
                                       IconImageUrl = EE.IconImageUrl,
                                       ImageUrl = EE.ImageUrl,
                                       ExpenseCategoryType = EE.ExpenseCategoryType,
                                       ExpenseTitle = EE.ExpenseTitle,
                                       ExpenseStatus = EE.ExpenseStatus,
                                       ExpenseDate = EE.ExpenseDate,
                                       ExpenseAmount = EE.ExpenseAmount,
                                       BillNumber = EE.BillNumber,
                                       MerchantName = EE.MerchantName,
                                       Comment = EE.Comment,
                                       FinalApproveAmount = EE.FinalApproveAmount,
                                       //CurrencyName = allCurrency.Where(x=> x.ISOCurrencySymbol == EE.ISOCurrencyCode).Select(x=> x.CurrencyEnglishName).FirstOrDefault().ToString(),
                                       CurrencyName = "",
                                       ISOCurrencyCode = EE.ISOCurrencyCode,

                                       //C.CurrencyId,
                                       DisplayName = Emp.DisplayName,
                                       ApproveRejectBy = EE.ApprovedRejectBy.HasValue ? _db.Employee.Where(x => x.EmployeeId == (int)EE.ApprovedRejectBy)
                                                                .Select(x => x.DisplayName).FirstOrDefault() : "--------",
                                       CreatedOn = EE.CreatedOn,
                                       UpdatedOn = EE.UpdatedOn,
                                   }).ToListAsync();
                var newBranch = brach.Select(x => new Getallexpensedata
                {
                    ExpenseId = x.ExpenseId,
                    CategoryId = x.CategoryId,
                    IconImageUrl = x.IconImageUrl,
                    ImageUrl = x.ImageUrl,
                    ExpenseCategoryType = x.ExpenseCategoryType,
                    ExpenseTitle = x.ExpenseTitle,
                    ExpenseStatus = x.ExpenseStatus,
                    ExpenseDate = x.ExpenseDate,
                    ExpenseAmount = x.ExpenseAmount,
                    BillNumber = x.BillNumber,
                    MerchantName = x.MerchantName,
                    Comment = x.Comment,
                    FinalApproveAmount = x.FinalApproveAmount,
                    CurrencyName = allCurrency.Where(q => x.ISOCurrencyCode == q.ISOCurrencySymbol).Select(q => q.CurrencyEnglishName).FirstOrDefault(),
                    ISOCurrencyCode = x.ISOCurrencyCode,

                    //C.CurrencyId,
                    DisplayName = x.DisplayName,
                    ApproveRejectBy = x.ApproveRejectBy,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                }).ToList()
                .OrderByDescending(x => x.UpdatedOn.HasValue ? x.UpdatedOn : x.CreatedOn)
                .ToList();

                if (newBranch.Count != 0)
                {
                    response.Status = true;
                    response.Message = "Expense list Found";
                    response.Data = page.HasValue && count.HasValue
                    ? new PaginationData
                    {
                        TotalData = newBranch.Count,
                        Counts = (int)count,
                        List = newBranch.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                    }
                        : (object)newBranch;
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Expense list Found";
                    response.Data = newBranch;
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

        #endregion Get all Expense Category list // by Employee

        #region Get All Expense Category // dropdown

        /// <summary>
        /// Created By Suraj Bundel on 23-05-2022
        /// API >> Get >> api/expense/Getallexpenseentrycategory
        /// Dropdown using Enum for expense type category
        /// </summary>
        /// <returns></returns>

        [Route("Getallexpenseentrycategory")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel Getallexpenseentrycategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var identity = User.Identity as ClaimsIdentity;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Expenseentry = Enum.GetValues(typeof(ExpenseTypeConstants))
                    .Cast<ExpenseTypeConstants>()
                    .Select(x => new TypeList
                    {
                        ExpenseTypeId = (int)x,
                        ExpenseType = Enum.GetName(typeof(ExpenseTypeConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Expense Category Exist";
                res.Status = true;
                res.Data = Expenseentry;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Expense Category // dropdown

        #region Add Expense Entry

        /// <summary>
        /// Created By Suraj Bundel On 23-05-2022
        /// API >> Post >> api/expense/createexpenseentry
        /// </summary>
        /// use to create the Expense in Expense entery List
        /// <returns></returns>
        [Route("createexpenseentry")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> CreateExpenseEntry(ExpenseEntry model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            Currency currencytype = new Currency();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    ExpenseEntry newexpenseentry = new ExpenseEntry
                    {
                        ExpenseCategoryType = model.ExpenseCategoryType,
                        ExpenseTitle = model.ExpenseTitle,
                        ExpenseDate = model.ExpenseDate,
                        ISOCurrencyCode = model.ISOCurrencyCode,
                        CategoryId = model.CategoryId,
                        ExpenseAmount = model.ExpenseAmount,
                        BillNumber = model.BillNumber,
                        MerchantName = model.MerchantName,
                        Comment = model.Comment,
                        CreatedBy = claims.employeeId,
                        //  UpdatedBy = claims.employeeid,
                        CreatedOn = DateTime.Now.Date,
                        //CreatedOn = DateTime.Now.AddDays(1),
                        // UpdatedOn = DateTime.Now.Date,
                        IsActive = model.IsActive = true,
                        IsDeleted = model.IsActive = false,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        ImageUrl = model.ImageUrl,
                        IconImageUrl = model.IconImageUrl,
                        AppliedBy = model.AppliedBy,
                        EmployeeId = claims.employeeId,

                        //Status = ExpenseCurrentStatus.Pending,
                        ExpenseStatus = "Pending",
                    };

                    //var val1 = Enum.GetName(typeof(ExpenseCurrentStatus), "Pending");

                    _db.ExpenseEntry.Add(newexpenseentry);
                    await _db.SaveChangesAsync();

                    res.Message = "Expense added successfully";
                    res.Status = true;
                    res.Data = newexpenseentry;
                }
                else
                {
                    res.Message = "Unable to added expense ";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        #endregion Add Expense Entry

        #region Api To Upload Expense Category File

        /// <summary>
        /// Created By Suraj Bundel On 23-05-2022
        /// API >> Post >> api/expense/uploadexpensecategoryfile
        /// </summary>
        /// use to post Document the Expense Category List
        /// <returns></returns>
        [HttpPost]
        [Route("uploadexpensentryfile")]
        [Authorize]
        public async Task<UploadImageResponse> uploadexpensentryfile()
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
                        if (extemtionType == "image" || extemtionType == "doc" || extemtionType == "application")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/expenseentry/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\expenseentry\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion Api To Upload Expense Category File

        #region Update Expense Entry

        /// <summary>
        /// Created by Suraj Bundel on 25/05/2022
        /// API >> Get >> api/expense/updateexpenseentrystatus
        /// </summary>
        /// use to update the Expense Status
        /// <param name="updateexc"></param>
        /// <returns></returns>

        [Route("updateexpenseentrystatus")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> updateexpenseentry(ExpenseEntry updateexc)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var updateexcData = await _db.ExpenseEntry.Where(x => x.ExpenseId == updateexc.ExpenseId/*&& x.IsDeleted == false*/).FirstOrDefaultAsync();
                if (updateexcData != null)
                {
                    updateexcData.ExpenseStatus = updateexc.ExpenseStatus;
                    updateexcData.ModeofPayment = updateexc.ModeofPayment;
                    updateexcData.FinalApproveAmount = updateexc.FinalApproveAmount;
                    updateexcData.Reason = updateexc.Reason;
                    updateexcData.UpdatedOn = DateTime.Now;
                    updateexcData.UpdatedBy = claims.employeeId;
                    updateexcData.ApprovedRejectBy = claims.employeeId;
                    updateexcData.IsActive = true;
                    updateexcData.IsDeleted = false;
                    _db.Entry(updateexcData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Updated Successfully!";
                }
                else
                {
                    res.Message = "Update request failed";
                    res.Status = false;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Update Expense Entry

        #region Delete ExpenseEntry by ID //Soft Delete //Commented Code

        ///// <summary>
        ///// Created by Suraj Bundel on 25/05/2022
        ///// API >> Get >> api/ExpenseEntry/deleteexpensecategory
        ///// </summary>
        ///// use to Delete the Expense  from Expense Category List
        ///// <param name="deleteexc"></param>
        ///// <returns></returns>
        //[Route("deleteexpenseentry")]
        //[HttpPut]
        //[Authorize]
        //public async Task<ResponseBodyModel> DeleteExpenseEntry(ExpenseEntry deleteexc)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var deleteexcData = await db.ExpenseEntry.Where(x => x.ExpenseId == deleteexc.ExpenseId && x.IsDeleted == false).FirstOrDefaultAsync();
        //        if (deleteexcData != null)
        //        {
        //            //db.ExpenseEntry.Remove(deleteexcData);
        //            //deleteexcData.DeletedOn = DateTime.Now;
        //            deleteexcData.DeletedBy = claims.employeeid;
        //            deleteexcData.Reason= deleteexc.Reason;
        //            deleteexcData.DeletedOn = DateTime.Now;
        //            deleteexcData.ExpenseStatus = "Rejected";
        //            deleteexcData.IsDeleted = true;
        //            deleteexcData.IsActive = false;
        //            db.Entry(deleteexcData).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            res.Status = true;
        //            res.Message = "ExpenseEntry Deleted Successfully!";
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "No ExpenseEntry Found!!";
        //        }
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        #endregion Delete ExpenseEntry by ID //Soft Delete //Commented Code

        #region Delete ExpenseEntry  //Soft Delete // not in use

        /// <summary>
        /// Created by Suraj Bundel on 25/05/2022
        /// API >> Get >> api/expense/deleteexpensecategory
        /// </summary>
        /// use to Delete the Expense  from Expense Category List
        /// <param name="deleteexc"></param>
        /// <returns></returns>
        [Route("expenseentryforreject")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> RejectExpenseEntry(ExpenseEntry deleteexc)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteexcData = await _db.ExpenseEntry.Where(x => x.ExpenseId == deleteexc.ExpenseId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (deleteexcData != null)
                {
                    //db.ExpenseEntry.Remove(deleteexcData);
                    //deleteexcData.DeletedOn = DateTime.Now;
                    deleteexcData.DeletedBy = claims.employeeId;
                    deleteexcData.Reason = deleteexc.Reason;
                    deleteexcData.DeletedOn = DateTime.Now;
                    deleteexcData.ExpenseStatus = "Rejected";
                    deleteexcData.IsActive = true;
                    _db.Entry(deleteexcData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Expense Entry Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No ExpenseEntry Found!!";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Delete ExpenseEntry  //Soft Delete // not in use

        #region Add Expense Category //Commented Code

        ///* Add Expense Category */

        //[Route("CreateExpenseCategory")]
        //[HttpPost]
        //[Authorize]
        //public IHttpActionResult CreateExpenseCategory(ExpenseCategory expenseCategory)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        ExpenseCategoryData res = new ExpenseCategoryData();
        //        ExpenseCategory newexpensecat = new ExpenseCategory();
        //        if (model.CategoryName.Trim() != "")
        //        {
        //            newexpensecat.CategoryName = model.CategoryName;
        //            newexpensecat.IconImageUrl = model.IconImageUrl;
        //            newexpensecat.CompanyId = compid;
        //            newexpensecat.OrgId = orgid;
        //            newexpensecat.ImageUrl = model.ImageUrl;
        //            newexpensecat.CreatedBy = userid;
        //            newexpensecat.UpdatedBy = userid;
        //            newexpensecat.CreatedOn = DateTime.Now;
        //            newexpensecat.IsActive = true;
        //            newexpensecat.IsDeleted = false;

        //            db.ExpenseCategory.Add(newexpensecat);
        //            db.SaveChanges();

        //            res.Status = "OK";
        //            res.Message = "ExpenseCategory added Successfully!";
        //            res.expense = newexpensecat;
        //            return Ok(res);
        //        }
        //        else
        //        {
        //            res.Status = "Error";
        //            res.Message = "ExpenseCategory is invalid!";
        //            res.expense = null;
        //            return Ok(res);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        #endregion Add Expense Category //Commented Code

        #region Update Expense Category //List

        ///* Update Expense Category */
        /// <summary>
        /// Created by Suraj Bundel on 27/05/2022
        /// API >> Get >> api/expense/updateexpenseentrylist
        /// </summary>
        /// use to Update the Expense Entry List
        /// <returns></returns>
        [Route("updateexpenseentrylist")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> UpdateExpenseEntrylist(ExpenseEntry updateexc)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateexcData = await _db.ExpenseEntry.Where(x => x.ExpenseId == updateexc.ExpenseId && x.IsDeleted == false && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (updateexcData != null)
                {
                    updateexcData.ExpenseStatus = "Pending";
                    updateexcData.ExpenseCategoryType = updateexc.ExpenseCategoryType;
                    updateexcData.ExpenseTitle = updateexc.ExpenseTitle;
                    updateexcData.ExpenseDate = updateexc.ExpenseDate;
                    updateexcData.ISOCurrencyCode = updateexc.ISOCurrencyCode;
                    updateexcData.ExpenseAmount = updateexc.ExpenseAmount;
                    updateexcData.BillNumber = updateexc.BillNumber;
                    updateexcData.MerchantName = updateexc.MerchantName;
                    updateexcData.ImageUrl = updateexc.ImageUrl;
                    updateexcData.Comment = updateexc.Comment;
                    updateexcData.UpdatedOn = DateTime.Now;
                    updateexcData.UpdatedBy = claims.employeeId;
                    updateexcData.IsActive = true;
                    updateexcData.IsDeleted = false;
                    _db.Entry(updateexcData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Updated Successfully!";
                }
                else
                {
                    res.Message = "Update request failed";
                    res.Status = false;
                };
            }
            catch (Exception)
            {
                res.Message = "Update request failed";
                res.Status = false;
            }
            return res;
        }

        #endregion Update Expense Category //List

        #region Commented Code

        ////public Task<ResponseBodyModel> UpdateExpenseCategorylist(ExpenseCategory updateexc)
        ////{
        ////        var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        ////    try
        ////    {
        ////        var identity = User.Identity as ClaimsIdentity;

        ////        Base response = new Base();
        ////        var updateexcData = db.ExpenseCategory.Where(x => x.CategoryId == updateexc.CategoryId && x.IsDeleted == false).FirstOrDefault();
        ////        if (updateexcData != null)
        ////        {
        ////            updateexcData.CategoryId = updateexc.CategoryId;
        ////            updateexcData.IconImageUrl = updateexc.IconImageUrl;
        ////            updateexcData.ImageUrl = updateexc.ImageUrl;
        ////            updateexcData.UpdatedBy = userid;
        ////            updateexcData.UpdatedOn = DateTime.Now;
        ////            updateexcData.IsActive = true;
        ////            updateexcData.IsDeleted = false;

        ////            db.SaveChanges();

        ////            response.StatusReason = true;
        ////            response.Message = "ExpenseCategory Updated Successfully!";
        ////        }
        ////        return System.Threading.Tasks.Task.FromResult(Ok(response));
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return System.Threading.Tasks.Task.FromResult(BadRequest(ex.Message));
        ////    }
        ////}

        #endregion Commented Code

        #region Delete ExpenseCategory by ID //Commented Code

        //    /* Delete ExpenseCategory */
        //    [Route("DeleteExpenseCategory")]
        //[HttpDelete]
        //[Authorize]
        //public IHttpActionResult DeleteExpenseCategory(int CategoryId)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        var deleteData = db.ExpenseCategory.Where(x => x.CategoryId == CategoryId).FirstOrDefault();
        //        if (deleteData != null)
        //        {
        //            deleteData.IsDeleted = true;
        //            deleteData.IsActive = false;
        //            db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            response.StatusReason = true;
        //            response.Message = "ExpenseCategory Deleted Successfully!";
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No ExpenseCategory Found!!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        #endregion Delete ExpenseCategory by ID //Commented Code

        #region Get all Expense Entry list // Request for Approval

        /// <summary>
        /// Created by Suraj Bundel on 27/05/2022
        /// Created by Suraj Bundel on 02/06/2022
        /// API >> Get >> api/expense/getallexpenseentryforapproval
        /// </summary>
        /// use to get all the expense entry for approval
        /// <returns></returns>

        [Route("getallexpenseentryforapproval")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetallExpenseEntryforApproval()
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var allCurrency = CurrencyHelper.GetCurrencyName().AsEnumerable();
                var brach = await (from EE in _db.ExpenseEntry
                                   join Emp in _db.Employee on EE.CreatedBy equals Emp.EmployeeId
                                   where EE.IsDeleted == false && EE.CompanyId == claims.companyId
                                   select new Getallexpensedata
                                   {
                                       ExpenseId = EE.ExpenseId,
                                       CategoryId = EE.CategoryId,
                                       IconImageUrl = EE.IconImageUrl,
                                       ImageUrl = EE.ImageUrl,
                                       ExpenseCategoryType = EE.ExpenseCategoryType,
                                       ExpenseTitle = EE.ExpenseTitle,
                                       ExpenseStatus = EE.ExpenseStatus,
                                       ExpenseDate = EE.ExpenseDate,
                                       ExpenseAmount = EE.ExpenseAmount,
                                       BillNumber = EE.BillNumber,
                                       MerchantName = EE.MerchantName,
                                       Comment = EE.Comment,
                                       FinalApproveAmount = EE.FinalApproveAmount,
                                       //CurrencyName = allCurrency.Where(x=> x.ISOCurrencySymbol == EE.ISOCurrencyCode).Select(x=> x.CurrencyEnglishName).FirstOrDefault().ToString(),
                                       CurrencyName = "",
                                       ISOCurrencyCode = EE.ISOCurrencyCode,

                                       //C.CurrencyId,
                                       DisplayName = Emp.DisplayName,
                                       ApproveRejectBy = EE.ApprovedRejectBy.HasValue ? _db.Employee.Where(x => x.EmployeeId == (int)EE.ApprovedRejectBy)
                                                                .Select(x => x.DisplayName).FirstOrDefault() : "--------",
                                       CreatedOn = EE.CreatedOn,
                                       UpdatedOn = EE.UpdatedOn,
                                   }).ToListAsync();
                var newBranch = brach.Select(x => new Getallexpensedata
                {
                    ExpenseId = x.ExpenseId,
                    CategoryId = x.CategoryId,
                    IconImageUrl = x.IconImageUrl,
                    ImageUrl = x.ImageUrl,
                    ExpenseCategoryType = x.ExpenseCategoryType,
                    ExpenseTitle = x.ExpenseTitle,
                    ExpenseStatus = x.ExpenseStatus,
                    ExpenseDate = x.ExpenseDate,
                    ExpenseAmount = x.ExpenseAmount,
                    BillNumber = x.BillNumber,
                    MerchantName = x.MerchantName,
                    Comment = x.Comment,
                    FinalApproveAmount = x.FinalApproveAmount,
                    CurrencyName = allCurrency.Where(q => x.ISOCurrencyCode == q.ISOCurrencySymbol).Select(q => q.CurrencyEnglishName).FirstOrDefault(),
                    ISOCurrencyCode = x.ISOCurrencyCode,

                    //C.CurrencyId,
                    DisplayName = x.DisplayName,
                    ApproveRejectBy = x.ApproveRejectBy,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                }).ToList()
                .OrderByDescending(x => x.UpdatedOn.HasValue ? x.UpdatedOn : x.CreatedOn)
                .ToList();

                if (newBranch.Count != 0)
                {
                    response.Status = true;
                    response.Message = "Expense list Found";
                    response.Data = newBranch;
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Expense list Found";
                    response.Data = newBranch;
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

        #endregion Get all Expense Entry list // Request for Approval

        #region This Api Used For Get Travel Expense By Id
        /// <summary>
        /// created by ankit jain Date - 14/12/2022
        /// Api >> Get >> api/expense/getexpenseentrybyid
        /// </summary>
        /// <returns></returns>
        [Route("getexpenseentrybyid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetExpenseEntryById(int expensebyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var travelExpensentrye = await _db.ExpenseEntry.FirstOrDefaultAsync(x =>
                 x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.ExpenseId == expensebyId);
                if (travelExpensentrye != null)
                {
                    res.Message = "View Expense Entry Found";
                    res.Status = true;
                    res.Data = travelExpensentrye;
                }
                else
                {
                    res.Message = "No TravelExpense Found";
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

        #endregion This Api Used For Get wfh Request By Id

        #region Mode of Payment // Dropdown

        /// <summary>
        /// Created By Suraj Bundel on 27-05-2022
        /// API >> Get >> api/expense/modeofpayments
        /// Dropdown using Enum payment Mode
        /// </summary>
        /// <returns></returns>

        [Route("modeofpayments")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel ModeofPayments()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var PaymentMode = Enum.GetValues(typeof(ModeofPaymentConstants))
                    .Cast<ModeofPaymentConstants>()
                    .Select(x => new ModeofpaymentsList
                    {
                        ModeofPayment = (int)x,
                        ModeofPaymentName = Enum.GetName(typeof(ModeofPaymentConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "Mode of payment Exist";
                res.Status = true;
                res.Data = PaymentMode;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Mode of Payment // Dropdown

        #region get update employees Details by id

        /// <summary>
        /// Created by Suraj Bundel on 26/05/2022
        /// API >> Get >> api/expense/getexpensedetailsbyId
        /// </summary>
        /// use to get the Expense details by Id
        /// <returns></returns>
        [Route("getexpensedetailsbyId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetExpenseDetailsbyId(int expenseId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getdata = await (from EE in _db.ExpenseEntry
                                     where EE.ExpenseId == expenseId && EE.EmployeeId == claims.employeeId
                                     select new GetExpenseDetailsbyIdHelperClasss
                                     {
                                         ExpenseId = EE.ExpenseId,
                                         ModeofPayment = EE.ModeofPayment,
                                         Reason = EE.Reason,
                                         FinalApproveAmount = EE.FinalApproveAmount,
                                         ExpenseStatus = EE.ExpenseStatus,

                                         ModeOfPaymentString = "",
                                     }).ToListAsync();
                foreach (var item in getdata)
                {
                    item.ModeOfPaymentString = Enum.GetName(typeof(ModeofPaymentConstants), item.ModeofPayment).Replace("_", " ");
                }
                if (getdata.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Expense list Found";
                    res.Data = getdata;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Expense list Found";
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

        #endregion get update employees Details by id

        #region This section use for Expense Travel  //

        // USER SIDE

        #region This is use for post expense travel

        /// <summary>
        /// API >> Post >> api/expense/addexpensetravel
        /// Create By shriya , Create On 27-05-2022
        /// Modification on 28-05-2022 by Shriya
        /// </summary>
        /// <param name="addexpense"></param>
        /// <returns></returns>
        [Route("addexpensetravel")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> AddTravelExpense(ExpenseTravelModal addexpense)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (addexpense != null)
                {
                    TravelExpense expense = new TravelExpense();
                    expense.EmployeeId = claims.employeeId;
                    expense.AppliedBy = claims.displayName;
                    expense.TravelFrom = addexpense.TravelFrom;
                    expense.TravelTo = addexpense.TravelTo;
                    expense.DepartureDate = addexpense.DepartureDate;
                    expense.ReturnDate = addexpense.ReturnDate;
                    expense.TravelerCount = addexpense.TravelerCount;
                    expense.TravelVia = addexpense.TravelVia;
                    expense.Comment = addexpense.Comment;
                    expense.ExpenseStatus = Enum.GetName(typeof(ExpenseTravalStatusConstants), ExpenseTravalStatusConstants.Pending).ToString();
                    expense.IsActive = true;
                    expense.IsDeleted = false;
                    expense.CreatedBy = claims.employeeId;
                    expense.CreatedOn = DateTime.Now;
                    expense.CompanyId = claims.companyId;
                    expense.OrgId = claims.orgId;
                    _db.TravelExpenses.Add(expense);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Traval expense added";
                    res.Data = expense;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data not recive";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is use for post expense travel

        #region This is used for get all trevel via

        /// <summary>
        /// API >> Get >> api/expense/gettraveltype
        /// Created by shriya ,Created on 27-05-2022
        /// </summary>
        /// <returns></returns>
        [Route("gettraveltype")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel GetTravelType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var identity = User.Identity as ClaimsIdentity;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var TravelWays = Enum.GetValues(typeof(TravelViaConstants))
                    .Cast<ExpenseTypeConstants>()
                    .Select(x => new TypeList
                    {
                        ExpenseTypeId = (int)x,
                        ExpenseType = Enum.GetName(typeof(TravelViaConstants), x).Replace("_", " ")
                    }).ToList();
                res.Message = "TravelType Exist";
                res.Status = true;
                res.Data = TravelWays;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get all trevel via

        #region This is used for upload expense travel

        /// <summary>
        /// API >> Post >> api/expense/uploadexpensetraveldocments
        /// Create by shriya ,Create on 27-05-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadexpensetraveldocments")]
        public async Task<UploadImageResponse> UploadExpenseTravelDocments()
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
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/expenseentry/expensetravel/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\expenseentry\\expensetravel\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This is used for upload expense travel

        #region This is used for get all travel expense of user

        /// <summary>
        /// API >> Get >> api/expense/getalltravelexpensebyuser
        /// Created by Shriya , Created on 28-05-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalltravelexpensebyuser")]
        public async Task<ResponseBodyModel> GetAlltravelExpenseByUser()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<ExpenseTravelModal> expenseTravellist = new List<ExpenseTravelModal>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var travelExpense = await _db.TravelExpenses.Where(ET => ET.EmployeeId == claims.employeeId
                                && ET.IsActive == true && ET.IsDeleted == false).ToListAsync();

                if (travelExpense.Count > 0)
                {
                    foreach (var item in travelExpense)
                    {
                        ExpenseTravelModal exp = new ExpenseTravelModal();
                        exp.ExpenseId = item.TravelExpenseId;
                        exp.EmployeeId = item.EmployeeId;
                        exp.AppliedBy = item.AppliedBy;
                        exp.TravelFrom = item.TravelFrom;
                        exp.TravelTo = item.TravelTo;
                        exp.DepartureDate = item.DepartureDate;
                        exp.ReturnDate = item.ReturnDate;
                        exp.TravelerCount = item.TravelerCount;
                        exp.TravelVia = item.TravelVia;
                        exp.TravelWays = Enum.GetName(typeof(TravelViaConstants), item.TravelVia).Replace("_", " ").ToString();
                        exp.UploadTravelExpense = item.UploadTravelExpenseDoc;
                        exp.Comment = item.Comment;
                        exp.ExpenseStatus = item.ExpenseStatus;
                        expenseTravellist.Add(exp);
                    }
                    res.Data = expenseTravellist.OrderByDescending(x => x.ExpenseId).ToList();
                    res.Status = true;
                    res.Message = "Travel expense list  found";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Travel expense list not found";
                    res.Data = expenseTravellist;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get all travel expense of user

        #region This is used for get all expense by expense id

        /// <summary>
        /// API >> Get >> api/expense/getexpensetravalbyexpenseid
        /// Created by Shriya , Created on 28-05-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getexpensetravalbyexpenseid")]
        public async Task<ResponseBodyModel> GetExpenseTravalByExpenseId(int expenseId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //List<ExpenseTravelModal> expenseTravellist = new List<ExpenseTravelModal>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var travelExpense = await _db.TravelExpenses.Where(x => x.TravelExpenseId == expenseId
                                 && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();

                if (travelExpense != null)
                {
                    ExpenseTravelModal exp = new ExpenseTravelModal();
                    exp.ExpenseId = travelExpense.TravelExpenseId;
                    exp.EmployeeId = travelExpense.EmployeeId;
                    exp.AppliedBy = travelExpense.AppliedBy;
                    exp.TravelFrom = travelExpense.TravelFrom;
                    exp.TravelTo = travelExpense.TravelTo;
                    exp.DepartureDate = travelExpense.DepartureDate;
                    exp.ReturnDate = travelExpense.ReturnDate;
                    exp.TravelerCount = travelExpense.TravelerCount;
                    //exp.ExpenseCategoryType = travelExpense.ExpenseCategoryType;
                    //exp.ExpenseCategory = Enum.GetName(typeof(ExpenseTypeEnum), travelExpense.ExpenseCategoryType).Replace("_", " ").ToString();
                    exp.TravelVia = travelExpense.TravelVia;
                    exp.TravelWays = Enum.GetName(typeof(TravelViaConstants), travelExpense.TravelVia).Replace("_", " ").ToString();
                    exp.UploadTravelExpense = travelExpense.UploadTravelExpenseDoc;
                    exp.Comment = travelExpense.Comment;
                    exp.ExpenseStatus = travelExpense.ExpenseStatus;

                    res.Data = exp;
                    res.Status = true;
                    res.Message = "Travel expense list  found";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Travel expense list not found";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get all expense by expense id

        #region This is used for update expense travel

        /// <summary>
        /// API >> Put >> api/expense/expensetravelupdate
        ///  Created by Shriya , Created on 28-05-2022
        ///  Modified by Shriya , Modified on 30-05-2022
        /// </summary>
        /// <param name="exptravel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("expensetravelupdate")]
        public async Task<ResponseBodyModel> ExpenseTravelUpdate(ExpenseTravelModal expenses)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exptraval = await _db.TravelExpenses.Where(x => x.TravelExpenseId == expenses.ExpenseId
                                 && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();

                if (exptraval != null)
                {
                    exptraval.EmployeeId = claims.employeeId;
                    exptraval.AppliedBy = claims.displayName;
                    //exptraval.ExpenseCategoryType = expenses.ExpenseCategoryType;
                    exptraval.TravelFrom = expenses.TravelFrom;
                    exptraval.TravelTo = expenses.TravelTo;
                    exptraval.DepartureDate = expenses.DepartureDate;
                    exptraval.ReturnDate = expenses.ReturnDate;
                    //exptraval.UploadTravelExpenseDoc = expenses.UploadTravelExpense;
                    exptraval.TravelerCount = expenses.TravelerCount;
                    exptraval.TravelVia = expenses.TravelVia;
                    exptraval.Comment = expenses.Comment;
                    exptraval.IsActive = true;
                    exptraval.IsDeleted = false;
                    exptraval.UpdatedBy = claims.employeeId;
                    exptraval.UpdatedOn = DateTime.Now;
                    _db.Entry(exptraval).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Traval expense updated";
                    res.Data = exptraval;
                }
                else
                {
                    res.Status = false;
                    res.Message = "traval expense not updated";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This is used for update expense travel

        // HR SIDE

        #region This is used for get all travel expense of user

        /// <summary>
        /// API >> Get >> api/expense/getalltravelexpense
        /// Created by Shriya , Created on 30-05-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalltravelexpense")]
        public async Task<ResponseBodyModel> GetAlltravelExpense()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<ExpenseTravelModal> expenseTravellist = new List<ExpenseTravelModal>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var travelExpense = await _db.TravelExpenses.Where(x =>
                x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();

                if (travelExpense.Count > 0)
                {
                    foreach (var item in travelExpense)
                    {
                        ExpenseTravelModal exp = new ExpenseTravelModal();
                        exp.ExpenseId = item.TravelExpenseId;
                        exp.EmployeeId = item.EmployeeId;
                        exp.AppliedBy = item.AppliedBy;
                        exp.TravelFrom = item.TravelFrom;
                        exp.TravelTo = item.TravelTo;
                        exp.DepartureDate = item.DepartureDate;
                        exp.ReturnDate = item.ReturnDate;
                        exp.TravelerCount = item.TravelerCount;
                        exp.TravelVia = item.TravelVia;
                        exp.TravelWays = Enum.GetName(typeof(TravelViaConstants), item.TravelVia).Replace("_", " ").ToString();
                        exp.UploadTravelExpense = item.UploadTravelExpenseDoc;
                        exp.Comment = item.Comment;
                        exp.ExpenseStatus = item.ExpenseStatus;
                        exp.CreateOn = item.CreatedOn;
                        expenseTravellist.Add(exp);
                    }
                    res.Data = expenseTravellist.OrderByDescending(x => x.CreateOn);
                    res.Status = true;
                    res.Message = "Travel expense list  found";
                }
                else
                {
                    res.Status = false;
                    res.Message = "Travel expense list not found";
                    res.Data = expenseTravellist;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get all travel expense of user

        #region Update Travel Expense 

        /// <summary>
        /// Created by ankit on 20/12/2022
        /// API >> Get >> api/expense/updatetravelexpensetatus
        /// </summary>
        /// use to update the Expense Status
        /// <param name="updateexc"></param>
        /// <returns></returns>
        [Route("updatetravelexpensetatus")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> UpdateTravelexpense(TervelExpenseHelper updateexc)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var updateexcData = await _db.TravelExpenses.Where(x => x.TravelExpenseId == updateexc.TravelExpenseId).FirstOrDefaultAsync();
                if (updateexcData != null)
                {
                    updateexcData.ExpenseStatus = updateexc.ExpenseStatus;
                    updateexcData.ModeofPayment = updateexc.ModeofPayment;
                    updateexcData.FinalApproveAmount = updateexc.FinalApproveAmount;
                    updateexcData.Reason = updateexc.Reason;
                    updateexcData.UpdatedOn = DateTime.Now;
                    updateexcData.UpdatedBy = claims.employeeId;
                    updateexcData.ApprovedRejectBy = claims.employeeId;
                    updateexcData.IsActive = true;
                    updateexcData.IsDeleted = false;
                    _db.Entry(updateexcData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Updated Successfully!";
                }
                else
                {
                    res.Message = "Update request failed";
                    res.Status = false;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class TervelExpenseHelper
        {
            public int TravelExpenseId { get; set; }
            public ModeofPaymentConstants ModeofPayment { get; set; }
            public string Reason { get; set; }
            public double FinalApproveAmount { get; set; }
            public string ExpenseStatus { get; set; }
        }
        #endregion Update Expense Entry

        #region This Api Used For Get Travel Expense By Id
        /// <summary>
        /// created by ankit jain Date - 14/12/2022
        /// Api >> Get >> api/expense/gettravelexpensebyid
        /// </summary>
        /// <returns></returns>
        [Route("gettravelexpensebyid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetTravelExpenseById(int expensebyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var travelExpense = await _db.TravelExpenses.FirstOrDefaultAsync(x =>
                 x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.TravelExpenseId == expensebyId);
                if (travelExpense != null)
                {
                    res.Message = "View TravelExpense Found";
                    res.Status = true;
                    res.Data = travelExpense;
                }
                else
                {
                    res.Message = "No TravelExpense Found";
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

        #endregion This Api Used For Get wfh Request By Id

        #region This is used for get all trevel via

        /// <summary>
        /// API >> Get >> api/expense/getexpensetravelexp
        /// Created by shriya ,Created on 30-05-2022
        /// </summary>
        /// <returns></returns>
        [Route("getexpensetravelexp")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel GetExpenseTravelExp()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var identity = User.Identity as ClaimsIdentity;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var TravelWays = Enum.GetValues(typeof(ExpenseTravalStatusConstants))
                    .Cast<ExpenseTypeConstants>()
                    .Select(x => new TypeList
                    {
                        ExpenseTypeId = (int)x,
                        ExpenseType = Enum.GetName(typeof(ExpenseTravalStatusConstants), x)
                    }).ToList();
                res.Message = "Travel Status List";
                res.Status = true;
                res.Data = TravelWays;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get all trevel via

        #region This is used for update status by HR epense treval

        /// <summary>
        /// API >> Put >> api/expense/updatestatusbyhr
        /// Create by shriya create on 30-05-2022
        /// </summary>
        /// <param name="travalupdate"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatestatusbyhr")]
        public async Task<ResponseBodyModel> UpdateStatusByHr(ExpTravalModal travalupdate)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var exptraval = await _db.TravelExpenses.Where(x => x.TravelExpenseId == travalupdate.ExpenseId
                                && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();
                // var expense =await db.ExpenseEntry.Where(x => x.ExpenseId== travalupdate.ExpenseId && x.IsActive == true && x.IsDeleted == true).FirstOrDefaultAsync();
                if (exptraval != null)
                {
                    exptraval.ExpenseStatus = Enum.GetName(typeof(ExpenseTravalStatusConstants), travalupdate.ExpenseStatus).ToString();
                    exptraval.ModeofPayment = travalupdate.ModeofPayment;
                    exptraval.Reason = travalupdate.Reason;
                    exptraval.TransactoionNo = travalupdate.TransactoionNo;
                    exptraval.UploadTravelExpenseDoc = travalupdate.UploadTravelExpenseDoc;
                    exptraval.TravelVia = travalupdate.TravelVia;
                    exptraval.FinalApproveAmount = travalupdate.FinalApproveAmount;
                    exptraval.UpdatedBy = claims.employeeId;
                    exptraval.UpdatedOn = DateTime.Now;

                    _db.Entry(exptraval).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Expense status updated";
                    res.Data = exptraval;
                }
                else
                {
                    res.Status = false;
                    res.Message = "expense status not updated because expense not found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This is used for update status by HR epense treval

        #region Get by Expense By Status

        /// <summary>
        /// Created by Suraj Bundel on 29/06/2022
        /// </summary>
        /// API >> Get >> api/expense/getallexpensebystatus
        /// <param name="Status"></param>
        /// <returns></returns>
        [Route("getallexpensebystatus")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllExpenseByStatus(string Status)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var ExpenseData = await (from EE in _db.ExpenseEntry
                                         join Emp in _db.Employee on EE.CreatedBy equals Emp.EmployeeId
                                         // join pj in db.Project on EE.ProjectId equals pj.ProjectId
                                         where EE.IsDeleted == false && EE.CompanyId == claims.companyId && EE.ExpenseStatus.Trim().ToUpper() == Status.Trim().ToUpper()
                                         select new Getallexpensedata
                                         {
                                             ExpenseId = EE.ExpenseId,
                                             CategoryId = EE.CategoryId,
                                             EmployeeId = EE.EmployeeId,
                                             DisplayName = Emp.DisplayName,
                                             //ProjectId = pj.ProjectId,
                                             //ProjectName = pj.ProjectName,
                                             ExpenseTitle = EE.ExpenseTitle,
                                             ExpenseDate = EE.ExpenseDate,
                                             ISOCurrencyCode = EE.ISOCurrencyCode,
                                             ExpenseAmount = EE.ExpenseAmount,
                                             MerchantName = EE.MerchantName,
                                             BillNumber = EE.BillNumber,
                                             Comment = EE.Comment,
                                             ImageUrl = EE.ImageUrl,
                                             IsApprove = EE.IsApprove,
                                             ExpenseStatus = EE.ExpenseStatus,
                                             Reason = EE.Reason,
                                             IconImageUrl = EE.IconImageUrl,
                                             ExpenseCategoryType = EE.ExpenseCategoryType,
                                             ModeofPayment = EE.ModeofPayment,
                                             FinalApproveAmount = EE.FinalApproveAmount,
                                             ApproveRejectBy = EE.ApprovedRejectBy.HasValue ? _db.Employee.Where(x => x.EmployeeId == (int)EE.ApprovedRejectBy)
                                                                                      .Select(x => x.DisplayName).FirstOrDefault() : "--------",
                                             TravelFrom = EE.TravelFrom,
                                             TravelTo = EE.TravelTo,
                                             DepartureDate = EE.DepartureDate,
                                             ReturnDate = EE.ReturnDate,
                                             TravelerCount = EE.TravelerCount,
                                             TravelVia = EE.TravelVia,
                                             CompanyId = EE.CompanyId,
                                             OrgId = EE.OrgId,
                                             CreatedBy = EE.CreatedBy,
                                             UpdatedBy = EE.UpdatedBy,
                                             DeletedBy = EE.DeletedBy,
                                             CreatedOn = EE.CreatedOn,
                                             UpdatedOn = EE.UpdatedOn,
                                             DeletedOn = EE.DeletedOn,
                                             IsActive = EE.IsActive,
                                             IsDeleted = EE.IsDeleted,
                                             ReasonOfRejection = EE.Reason,
                                         }).ToListAsync();
                //var ExpenseData = db.ExpenseEntry.Where(x => x.IsDeleted == false && x.ExpenseStatus.Trim().ToUpper() == Status.Trim().ToUpper()).ToList();
                //if (ExpenseData.Count != 0)
                if (ExpenseData.Count > 0)
                {
                    res.Message = "All " + Status + "Expense list Found";
                    res.Status = true;
                    res.Data = ExpenseData;
                }
                else
                {
                    res.Message = "No " + Status + "Expense Not list Found";
                    res.Status = false;
                    res.Data = ExpenseData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get by Expense By Status

        #region Delete ExpenseEntry by ID //Soft Delete //Commented Code

        ///// <summary>
        ///// Created by Suraj Bundel on 25/05/2022
        ///// API >> Get >> api/ExpenseEntry/deleteexpensecategory
        ///// </summary>
        ///// use to Delete the Expense  from Expense Category List
        ///// <param name="deleteexc"></param>
        ///// <returns></returns>
        //[Route("deleteexpenseentry")]
        //[HttpPut]
        //[Authorize]
        //public async Task<ResponseBodyModel> DeleteExpenseEntry(ExpenseEntry deleteexc)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var deleteexcData = await db.ExpenseEntry.Where(x => x.ExpenseId == deleteexc.ExpenseId && x.IsDeleted == false).FirstOrDefaultAsync();
        //        if (deleteexcData != null)
        //        {
        //            //db.ExpenseEntry.Remove(deleteexcData);
        //            //deleteexcData.DeletedOn = DateTime.Now;
        //            deleteexcData.DeletedBy = claims.employeeid;
        //            deleteexcData.Reason= deleteexc.Reason;
        //            deleteexcData.DeletedOn = DateTime.Now;
        //            deleteexcData.ExpenseStatus = "Rejected";
        //            deleteexcData.IsDeleted = true;
        //            deleteexcData.IsActive = false;
        //            db.Entry(deleteexcData).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            res.Status = true;
        //            res.Message = "ExpenseEntry Deleted Successfully!";
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "No ExpenseEntry Found!!";
        //        }
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        #endregion Delete ExpenseEntry by ID //Soft Delete //Commented Code

        public class ExpTravalModal
        {
            public int? ExpenseId { get; set; }
            public string TransactoionNo { get; set; }
            public ModeofPaymentConstants ModeofPayment { get; set; }
            public ExpenseTravalStatusConstants ExpenseStatus { get; set; }
            public string Reason { get; set; }
            public string UploadTravelExpenseDoc { get; set; }
            public TravelViaConstants TravelVia { get; set; }
            public double FinalApproveAmount { get; set; }
        }

        public class ExpenseTravelModal
        {
            public int? ExpenseId { get; set; }

            public int? EmployeeId { get; set; }
            public string AppliedBy { get; set; }
            public string TravelFrom { get; set; }
            public string TravelTo { get; set; }
            public DateTime? DepartureDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public int TravelerCount { get; set; }
            public TravelViaConstants TravelVia { get; set; }
            public string TravelWays { get; set; }
            public string UploadTravelExpense { get; set; }
            public string Comment { get; set; }
            public string ExpenseStatus { get; set; }
            public DateTime? CreateOn { get; set; }
        }

        #endregion This section use for Expense Travel  //

        #region Api's For Use Expenses Dashboard 

        #region Api's For Expense Dashboard
        /// <summary>
        /// Create By Ravi Vyas 25-08-2022
        /// API >> GET >> api/expense/expensedashboard
        /// </summary>
        [HttpGet]
        [Route("expensedashboard")]
        public async Task<ResponseBodyModel> GetDashboard(int? year, int? month = null, ExpenseTypeConstants? type = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            DashboardRes response = new DashboardRes();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            year = year ?? DateTime.Now.Year;        // Check Year
            try
            {
                var dashboardData = await _db.ExpenseEntry.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.CreatedOn.Year == year).ToListAsync();
                //if (dashboardData.Count > 0)
                //{
                dashboardData = month.HasValue ? dashboardData.Where(x => x.CreatedOn.Month == month).ToList() : dashboardData;       // Check Month

                var totalApproved = dashboardData.Where(x => x.ExpenseStatus == "Approved").Select(x => x.ExpenseAmount).Sum();
                totalApproved = Math.Round(totalApproved, 2);                                                               // Sum Of Approved Expenses
                var totalAmount = dashboardData.Select(x => x.ExpenseAmount).Sum();
                totalAmount = Math.Round(totalAmount, 2);
                var avgAmount = dashboardData.Count > 0 ? dashboardData.Select(x => x.ExpenseAmount).Average() : 0;                                                // Avg. Amount of Expenses
                avgAmount = Math.Round(avgAmount, 2);


                var totalApprovedExpense = dashboardData.Count(x => x.ExpenseStatus == "Approved");                 // Count Of Total Approved Expenses
                var totalRejected = dashboardData.Count(x => x.ExpenseStatus == "Rejected");                        // Count Of Total Rejected Expenses
                var totalPending = dashboardData.Count(x => x.ExpenseStatus == "Pending");

                response.ApprovedAmount = totalApproved;
                response.TotalAmount = totalAmount;
                response.AvgAmount = avgAmount;
                response.TotalApprovedExpense = totalApprovedExpense;
                response.TotalRejectExpense = totalRejected;
                response.TotalPendingExpense = totalPending;

                #region Bar Chart Data

                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                         .TakeWhile(m => m != String.Empty)
                                         .Select((m, i) => new
                                         {
                                             Month = i + 1,
                                             MonthName = m
                                         }).ToList();
                var monthNames = months.ConvertAll(x => x.MonthName);

                var listDynamic = new List<dynamic>();
                var intList1 = new List<double>();
                var data1 = new
                {
                    label = "Approved",
                    stack = "1",
                    data = intList1,
                };
                var intList2 = new List<double>();
                var data2 = new
                {
                    label = "Pending",
                    stack = "2",
                    data = intList2,
                };
                var intList3 = new List<double>();
                var data3 = new
                {
                    label = "Rejected",
                    stack = "3",
                    data = intList3,
                };

                foreach (var item in months)
                {
                    if (type == null)
                    {
                        intList1.Add(dashboardData.Where(x => x.CreatedOn.Month == item.Month && x.ExpenseStatus == "Pending" /*&& x.ExpenseCategoryType == type*/)
                        .Select(x => x.ExpenseAmount).ToList().Sum());
                        intList2.Add(dashboardData.Where(x => x.CreatedOn.Month == item.Month && x.ExpenseStatus == "Approved" /*&& x.ExpenseCategoryType == type*/)
                            .Select(x => x.ExpenseAmount).ToList().Sum());
                        intList3.Add(dashboardData.Where(x => x.CreatedOn.Month == item.Month && x.ExpenseStatus == "Rejected" /*&& x.ExpenseCategoryType == type*/)
                                  .Select(x => x.ExpenseAmount).ToList().Sum());
                    }
                    else
                    {
                        intList1.Add(dashboardData.Where(x => x.CreatedOn.Month == item.Month && x.ExpenseStatus == "Pending" && x.ExpenseCategoryType == type)
                       .Select(x => x.ExpenseAmount).ToList().Sum());
                        intList2.Add(dashboardData.Where(x => x.CreatedOn.Month == item.Month && x.ExpenseStatus == "Approved" && x.ExpenseCategoryType == type)
                            .Select(x => x.ExpenseAmount).ToList().Sum());
                        intList3.Add(dashboardData.Where(x => x.CreatedOn.Month == item.Month && x.ExpenseStatus == "Rejected" && x.ExpenseCategoryType == type)
                                  .Select(x => x.ExpenseAmount).ToList().Sum());
                    }

                }
                listDynamic.Add(data1);
                listDynamic.Add(data2);
                listDynamic.Add(data3);

                response.label = monthNames;
                response.graph = listDynamic;

                #endregion Bar Chart Data

                #region Api for CategoryType And Applied pie Chart.

                List<HelpForCiechart> PieChart = new List<HelpForCiechart>();
                var expenseTypeEnum = Enum.GetValues(typeof(ExpenseTypeConstants))
               .Cast<ExpenseTypeConstants>()
               .Select(x => new HelperModelForEnum
               {
                   TypeId = (int)x,
                   TypeName = Enum.GetName(typeof(ExpenseTypeConstants), x).Replace("_", " ")
               }).ToList();


                foreach (var item in expenseTypeEnum)
                {
                    HelpForCiechart obj = new HelpForCiechart
                    {

                        Name = item.TypeName,
                        Value = dashboardData.Count(x => x.IsActive && !x.IsDeleted && ((int)x.ExpenseCategoryType) == item.TypeId
                        && x.CompanyId == claims.companyId),
                    };
                    PieChart.Add(obj);
                    response.AppliedPieCharting = PieChart;
                }



                #endregion

                #region Api for CategoryType And Status Approved pie Chart.

                List<HelpForCiechart> PieChart2 = new List<HelpForCiechart>();
                var expenseTypeEnum2 = Enum.GetValues(typeof(ExpenseTypeConstants))
               .Cast<ExpenseTypeConstants>()
               .Select(x => new HelperModelForEnum
               {
                   TypeId = (int)x,
                   TypeName = Enum.GetName(typeof(ExpenseTypeConstants), x).Replace("_", " ")
               }).ToList();


                foreach (var item in expenseTypeEnum2)
                {
                    HelpForCiechart obj = new HelpForCiechart
                    {
                        Name = item.TypeName,
                        Value = dashboardData.Count(x => x.IsActive && !x.IsDeleted && ((int)x.ExpenseCategoryType) == item.TypeId &&
                        x.ExpenseStatus == "Approved" && x.CompanyId == claims.companyId),
                    };
                    PieChart2.Add(obj);
                    response.ApprovedPieCharting = PieChart2;
                }



                #endregion

                #region Api for CategoryType And Status Reject pie Chart.



                List<HelpForCiechart> PieChart3 = new List<HelpForCiechart>();
                var expenseTypeEnum3 = Enum.GetValues(typeof(ExpenseTypeConstants))
               .Cast<ExpenseTypeConstants>()
               .Select(x => new HelperModelForEnum
               {
                   TypeId = (int)x,
                   TypeName = Enum.GetName(typeof(ExpenseTypeConstants), x).Replace("_", " ")
               }).ToList();


                foreach (var item in expenseTypeEnum3)
                {
                    HelpForCiechart obj = new HelpForCiechart
                    {
                        Name = item.TypeName,
                        Value = dashboardData.Count(x => x.IsActive && !x.IsDeleted && ((int)x.ExpenseCategoryType) == item.TypeId &&
                        x.ExpenseStatus == "Rejected" && x.CompanyId == claims.companyId),
                    };
                    PieChart3.Add(obj);
                    response.RejectPieCharting = PieChart3;
                }



                #endregion

                #region Api For Applied Pending Rejectd Count Pie Chart

                List<HelpForCiechart> PieChart4 = new List<HelpForCiechart>();
                var data = dashboardData.ConvertAll(x => x.ExpenseStatus).Distinct();
                foreach (var item in data)
                {
                    HelpForCiechart obj = new HelpForCiechart
                    {
                        Name = item,
                        Value = dashboardData.Count(x => x.IsActive && !x.IsDeleted && x.ExpenseStatus == item && x.CompanyId == claims.companyId),
                    };
                    PieChart4.Add(obj);
                    response.AllStatusCount = PieChart4;
                }

                #endregion


                res.Message = "Data Found !";
                res.Status = true;
                res.Data = response;
                //}
                //else
                //{

                //    res.Message = "Data Not Found !";
                //    res.Status = false;
                //    res.Data = response;

                //}
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region Get All ExpensesType // dropdown

        /// <summary>
        /// Created By Ravi Vyas on 25-08-2022
        /// API >> Get >>api/expense/getcategory
        /// Dropdown using Enum for Exit employee Reason
        /// </summary>
        [Route("getcategory")]
        [HttpGet]
        public ResponseBodyModel ExpensesType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var expensesType = Enum.GetValues(typeof(ExpenseTypeConstants))
                    .Cast<ExpenseTypeConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(ExpenseTypeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Expenses Category List Found !";
                res.Status = true;
                res.Data = expensesType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region API To Get Year List Of Expense Dashboard

        /// <summary>
        /// Created By Ravi Vyas On 25-08-2022
        /// API >> Get >> api/expense/expensesdashboardyearlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("expensesdashboardyearlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardYearList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var expensesRequest = await _db.ExpenseEntry.Where(x => claims.companyId == x.CompanyId).ToListAsync();
                var checkYear = expensesRequest
                        .Select(x => new
                        {
                            Name = x.CreatedOn.Year
                        }).Distinct().ToList();
                if (checkYear.Count > 0)
                {
                    res.Message = "Year List";
                    res.Status = true;
                    res.Data = checkYear;
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = checkYear;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Year List Of Leave Dashboard

        #region API To Get Month List Of Expenses Dashboard

        /// <summary>
        /// Created By Ravi Vyas On 25-08-2022
        /// API >> Get >> api/expense/expensesdashboardmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("expensesdashboardmonthlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardMonthListAsync(int year)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var expensesRequest = await _db.ExpenseEntry.Where(x => claims.companyId == x.CompanyId && x.CreatedOn.Year == year)
                            .Select(x => x.CreatedOn.Month).Distinct().OrderBy(x => x).FirstOrDefaultAsync();
                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                        .TakeWhile(m => m != String.Empty)
                                        .Select((m, i) => new
                                        {
                                            Month = i + 1,
                                            MonthName = m
                                        }).ToList();
                months = months.Skip(expensesRequest - 1).ToList();
                if (months.Count > 0)
                {
                    res.Message = "Month List !";
                    res.Status = true;
                    res.Data = months;
                }
                else
                {
                    res.Message = "No Expenses Found !";
                    res.Status = false;
                    res.Data = months;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Month List Of Leave Dashboard

        #endregion

        #region Helper Model
        public class GetExpenseDetailsbyIdHelperClasss
        {
            public int ExpenseId { get; set; }
            public ModeofPaymentConstants ModeofPayment { get; set; }
            public string Reason { get; set; }
            public double FinalApproveAmount { get; set; }
            public string ExpenseStatus { get; set; }
            public string ModeOfPaymentString { get; set; }
        }

        public class ExpenseEntryData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public ExpenseCategory expense { get; set; }
        }

        public class ExpenseCategoryDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<ExpenseCategory> expensecatlist { get; set; }
        }

        public class TypeList
        {
            public int ExpenseTypeId { get; set; }
            public string ExpenseType { get; set; }
        }

        public class ModeofpaymentsList
        {
            public int ModeofPayment { get; set; }
            public string ModeofPaymentName { get; set; }
        }

        public class getenumname
        {
            public int ModeofpaymentEnumId { get; set; }
            public string ModeofpaymentEnumName { get; set; }
        }

        public class Getallexpensedata
        {
            public int ExpenseId { get; set; }

            public int CategoryId { get; set; }
            public string IconImageUrl { get; set; }
            public string ImageUrl { get; set; }
            public ExpenseTypeConstants ExpenseCategoryType { get; set; }
            public string ExpenseTitle { get; set; }
            public string ExpenseStatus { get; set; }
            public DateTime? ExpenseDate { get; set; }
            public double ExpenseAmount { get; set; }
            public string BillNumber { get; set; }
            public string MerchantName { get; set; }
            public string Comment { get; set; }
            public double FinalApproveAmount { get; set; }
            public string ISOCurrencyCode { get; set; }
            public string CurrencyName { get; set; }
            public int CurrencyId { get; set; }
            public string DisplayName { get; set; }
            public string ApproveRejectBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }

            /// <summary>
            /// Changed by Suraj Bundel  on 28/06/2022
            /// </summary>
            public int EmployeeId { get; set; }

            //public int ProjectId { get; set; }
            //public string ProjectName { get; set; }
            public bool IsApprove { get; set; }

            public string Reason { get; set; }
            public ModeofPaymentConstants ModeofPayment { get; set; }
            public string TravelFrom { get; set; }
            public string TravelTo { get; set; }
            public string ReasonOfRejection { get; set; }
            public DateTime? DepartureDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public int TravelerCount { get; set; }
            public TravelViaConstants TravelVia { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
            public int CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public int? DeletedBy { get; set; }
            public DateTime? DeletedOn { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }
        public class DashboardRes
        {
            public double ApprovedAmount { get; set; }
            public double TotalAmount { get; set; }
            public double AvgAmount { get; set; }
            public int TotalApprovedExpense { get; set; }
            public int TotalRejectExpense { get; set; }
            public int TotalPendingExpense { get; set; }
            public List<HelpForCiechart> ApprovedPieCharting { get; set; }
            public List<HelpForCiechart> RejectPieCharting { get; set; }
            public List<HelpForCiechart> AppliedPieCharting { get; set; }
            public List<HelpForCiechart> AllStatusCount { get; set; }
            public object graph { get; set; }
            public object label { get; set; }
            public List<ExpenseGraphModalForMonth> ExpenseGraphModalForMonth { get; set; }

        }
        public class HelpForCiechart
        {

            public string Name { get; set; }
            public int Value { get; set; }

        }
        public class HelpForBarChart
        {

            public string Name { get; set; }
            public double Value { get; set; }

        }

        public class ExpenseGraphModalForMonth
        {
            public string CategoryName { get; set; }
            public List<HelpForBarChart> Series { get; set; }
        }
        #endregion
    }
}

