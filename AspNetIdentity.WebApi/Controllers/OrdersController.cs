using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/purcess")]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region Api is Use For Get All The Item Amount in OdersController

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// Api >> Get >>  api/purcess/gettotalorderamount
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("gettotalorderamount")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetTotalOrderAmount()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DashboardOrderExpense response = new DashboardOrderExpense();
            ResponseBodyModel Res = new ResponseBodyModel();
            try
            {
                #region Get Total Compuer Amount

                var TotalComputer = await db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Computer"
                && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToListAsync();
                if (TotalComputer.Count > 0)
                {
                    var Total = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Computer"
                    && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList().Sum();
                    response.GetAllComputerAmount = Total;
                }

                #endregion Get Total Compuer Amount

                #region Get Total Laptop Amount

                var TotalLaptop = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Laptop"
                && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList();
                if (TotalLaptop.Count > 0)
                {
                    var Total = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Laptop"
                    && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList().Sum();
                    response.GetAllLaptopAmount = Total;
                }

                #endregion Get Total Laptop Amount

                #region Get Total Accessories Amount

                var TotalAccessories = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Accessories"
                && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList();
                if (TotalAccessories.Count > 0)
                {
                    var Total = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Accessories"
                    && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList().Sum();
                    response.GetAllAccessoriesAmount = Total;
                }

                #endregion Get Total Accessories Amount

                #region Get Total Others Amount

                var TotalOthers = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Others"
                && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList();
                if (TotalOthers.Count > 0)
                {
                    var Total = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Category == "Others"

                    && x.Status != "Rejected" && x.Status != "Pending").Select(x => x.Amount).ToList().Sum();
                    response.GetAllOthersAmount = Total;
                }

                #endregion Get Total Others Amount

                Res.Message = "All Order Available";
                Res.Status = true;
                Res.Data = response;
            }
            catch (Exception ex)
            {
                Res.Message = ex.Message;
                Res.Status = false;
            }
            return Res;
        }

        #endregion Api is Use For Get All The Item Amount in OdersController

        #region This Api Is Used To Add Items in OrderController

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// Api >> post >> api/purcess/Createorder
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        [Route("Createorder")]
        [HttpPost]
        public async Task<ResponseBodyModel> Createorder(Purches Item)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Purches OrderObj = new Purches();
                OrderObj.OrderId = Item.OrderId;
                OrderObj.OrderByName = claims.displayName;
                OrderObj.ItemId = Item.ItemId;
                OrderObj.ItemName = Item.ItemName;
                OrderObj.From = Item.From;
                OrderObj.PurchesDocUrl = Item.PurchesDocUrl;
                OrderObj.CategoryID = Item.CategoryID;
                OrderObj.Category = Enum.GetName(typeof(PurchesItem), OrderObj.CategoryID).ToString();
                OrderObj.OrderDate = Item.OrderDate;
                OrderObj.PaidByEnum = Item.PaidByEnum;
                OrderObj.PaidBy = Enum.GetName(typeof(PaidByName), OrderObj.PaidByEnum).ToString();
                OrderObj.Status = "Pending";
                OrderObj.Amount = Item.Amount;
                OrderObj.Units = Item.Units;
                OrderObj.CompanyId = claims.companyId;
                OrderObj.OrgId = claims.orgId;
                OrderObj.CreatedBy = claims.employeeId;
                OrderObj.CreatedOn = DateTime.Now;
                OrderObj.IsActive = true;
                OrderObj.IsDeleted = false;
                db.Purches.Add(OrderObj);
                await db.SaveChangesAsync();
                response.Status = true;
                response.Message = "Created Successfully";
                response.Data = OrderObj;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        #endregion This Api Is Used To Add Items in OrderController

        #region This Api Use For Get All Order List In CreatedBy

        /// <summary>
        /// Created By Ankit jain Date - 31/05/2022
        /// Api >> Get >> api/purcess/getallorderlist
        /// </summary>
        /// <param name="Itemid"></param>
        /// <returns></returns>
        [Route("getallorderlist")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllOrderList()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var HistoryData = await db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.CreatedBy == claims.employeeId)
                                      .Select(x => new
                                      {
                                          x.OrderId,
                                          x.ItemId,
                                          x.ItemName,
                                          x.OrderByName,
                                          x.From,
                                          x.OrderDate,
                                          x.Status,
                                          x.Amount,
                                          x.PaidBy,
                                          x.Category,
                                          x.Units,
                                          x.PurchesDocUrl,
                                          x.Reason
                                      }).ToListAsync();
                if (HistoryData != null)
                {
                    res.Message = "Item History List Found";
                    res.Status = true;
                    res.Data = HistoryData;
                }
                else
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                    res.Data = HistoryData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use For Get All Order List In CreatedBy

        #region This Api Use For Get All Order List In Admin

        /// <summary>
        /// Created By Ankit jain Date - 31/05/2022
        /// Api >> Get >> api/purcess/getallorder
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("getallorder")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllOrderhistory()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var HistoryData = await db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false)
                                   .Select(x => new
                                   {
                                       x.OrderId,
                                       x.ItemId,
                                       x.ItemName,
                                       x.OrderByName,
                                       x.From,
                                       x.OrderDate,
                                       x.Status,
                                       x.Amount,
                                       x.PaidBy,
                                       x.Category,
                                       x.Units,
                                       x.PurchesDocUrl,
                                       x.Reason
                                   }).ToListAsync();
                if (HistoryData != null)
                {
                    res.Message = "Item History List Found";
                    res.Status = true;
                    res.Data = HistoryData;
                }
                else
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                    res.Data = HistoryData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use For Get All Order List In Admin

        #region Api is Use For Get All The Item Amount in OdersController By Status

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// Api >> Get >>  api/purcess/gettotalorderamountbystatus
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("gettotalorderamountbystatus")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetTotalOrderAmountStatus()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DashboardOrderinvoice response = new DashboardOrderinvoice();
            ResponseBodyModel Res = new ResponseBodyModel();
            try
            {
                #region Get Total Pending

                var TotalPending = await db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Status == "Pending").CountAsync();
                response.GetAllPending = TotalPending;

                #endregion Get Total Pending

                #region Get Total Approved

                var TotalApproved = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Status == "Approved").Count();
                response.GetAllApprove = TotalApproved;

                #endregion Get Total Approved

                #region Get Total Reject

                var TotalReject = db.Purches.Where(x => x.IsActive == true && x.IsDeleted == false && x.Status == "Rejected").Count();
                response.GetAllReject = TotalReject;

                #endregion Get Total Reject

                Res.Message = "All Order Available";
                Res.Status = true;
                Res.Data = response;
            }
            catch (Exception ex)
            {
                Res.Message = ex.Message;
                Res.Status = false;
            }
            return Res;
        }

        #endregion Api is Use For Get All The Item Amount in OdersController By Status

        #region Api To Get PaidType

        /// <summary>
        /// Created By Ankit Jain on 31-05-2022
        /// API >> Get >> api/purcess/getpaidtype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaidtype")]
        public ResponseBodyModel GetPaidType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var PaidData = Enum.GetValues(typeof(PaidByName))
                                .Cast<PaidByName>()
                                .Select(x => new PaidModel
                                {
                                    PaidById = (int)x,
                                    PaidByName = Enum.GetName(typeof(PaidByName), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Paid List";
                res.Status = true;
                res.Data = PaidData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get PaidType

        #region Api To Get CategoryType

        /// <summary>
        /// Created By Ankit Jain on 31-05-2022
        /// API >> Get >> api/purcess/getcategorytype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcategorytype")]
        public ResponseBodyModel GetCategoryType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var PaidData = Enum.GetValues(typeof(PurchesItem))
                                .Cast<PurchesItem>()
                                .Select(x => new CategoryModel
                                {
                                    CategoryId = (int)x,
                                    CategoryName = Enum.GetName(typeof(PurchesItem), x).Replace("_", " "),
                                }).ToList();

                res.Message = "Paid List";
                res.Status = true;
                res.Data = PaidData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get CategoryType

        #region This Api Use For Upload Documents For Purches Document

        /// <summary>
        ///Created By Ankit On 01-06-2022
        /// </summary>Api>> Post>> api/purcess/uploadpurchesdocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadpurchesdocuments")]
        public async Task<UploadPurchesItem> UploadInvoiceDocments()
        {
            UploadPurchesItem result = new UploadPurchesItem();
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
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/purchesdoc/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\purchesdoc\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This Api Use For Upload Documents For Purches Document

        #region This Api used For Upadte Purches Orders Request

        /// <summary>
        /// Created by Ankit Date -30/05/2022
        /// Api >> Put >> api/purcess/updatestatus
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("updatestatus")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateRequests(Updatestatus model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await db.Purches.Where(x => x.OrderId == model.OrderId && x.IsDeleted == false && x.IsActive == true).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.OrderId = model.OrderId;
                    data.Status = model.Status;
                    data.Reason = model.Reason;
                    data.CompanyId = claims.companyId;
                    data.OrgId = claims.orgId;
                    data.UpdatedBy = claims.userId;
                    data.UpdatedOn = DateTime.Now;
                    data.IsActive = true;
                    data.IsDeleted = false;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    res.Status = true;
                    res.Message = "Order Request Updated";
                    res.Data = data;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Order Request Found!!";
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

        #endregion This Api used For Upadte Purches Orders Request

        #region This Api is Used To Delete Purchess Order

        /// <summary>
        /// Created By Ankit date - 04/04/2022
        /// Api >> Delete >> api/purcess/deletepurches
        /// </summary>
        /// <param name="DeleteItem"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletepurches")]
        public async Task<ResponseBodyModel> DeleteItem(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var Item = await db.Purches.FirstOrDefaultAsync(x => x.OrderId == Id &&
                               x.IsDeleted == false && x.IsActive == true);
                if (Item != null)
                {
                    Item.IsActive = false;
                    Item.IsDeleted = true;
                    Item.DeletedOn = DateTime.Now;
                    Item.DeletedBy = claims.companyId;

                    db.Entry(Item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Item Deleted";
                    res.Status = true;
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

        #endregion This Api is Used To Delete Purchess Order

        #region Helper Model

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// </summary>
        public class DashboardOrderExpense
        {
            public int GetAllComputerAmount { get; set; }
            public int GetAllLaptopAmount { get; set; }
            public int GetAllAccessoriesAmount { get; set; }
            public int GetAllOthersAmount { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// </summary>
        public class PaidModel
        {
            public int PaidById { get; set; }
            public string PaidByName { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// </summary>
        public class CategoryModel
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain Date - 31/05/2022
        /// </summary>
        public class DashboardOrderinvoice
        {
            public int GetAllPending { get; set; }
            public int GetAllApprove { get; set; }
            public int GetAllReject { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain Date-30/05/2022
        /// </summary>
        public class UploadPurchesItem
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        public class Updatestatus
        {
            public int OrderId { get; set; }
            public string Status { get; set; }
            public string Reason { get; set; }
        }

        #endregion Helper Model
    }
}