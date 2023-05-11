using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/ItemMasterAddItem")]
    public class ItemMasterAddItemController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        #region This Api Is Used To Get Category By Base Category Id

        /// <summary>
        /// Created By Nayan Pancholi Date - 04/04/2022
        /// Api >> Get >>  api/ItemMasterAddItem/GetCategoryByBasecateId
        /// </summary>
        /// <param name="basecategoryId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("GetCategoryByBasecateId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetCategoryByBasecateId(int basecategoryId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel resDto = new ResponseBodyModel();
            try
            {
                var data = await db.ItemMasterCategory.Where(x => x.BaseCategoryId == basecategoryId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
                if (data.Count > 0)
                {
                    resDto.Message = "Data Found Successful";
                    resDto.Status = true;
                    resDto.Data = data;
                }
                else
                {
                    resDto.Message = "Data does not exist";
                    resDto.Status = false;
                }

                return resDto;
            }
            catch (Exception ex)
            {
                resDto.Message = ex.Message;
                resDto.Status = false;
                return resDto;
            }
        }

        #endregion This Api Is Used To Get Category By Base Category Id

        #region Api for get Sub Category by category id

        /// <summary>
        /// Created By Nayan Pancholi Date - 04/04/2022
        /// Api >> Get >>  api/ItemMasterAddItem/GetSubCateogrybyCatId
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        [Route("GetSubCateogrybyCatId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetSubCateogrybyCatId(int catId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel objRes = new ResponseBodyModel();
            try
            {
                var data = await db.itemMasterSubCategory.Where(x => x.IsActive == true && x.IsDeleted == false && x.Categoryid == catId).ToListAsync();
                if (data.Count > 0)
                {
                    objRes.Message = "Data Found Successful";
                    objRes.Status = true;
                    objRes.Data = data;
                }
                else
                {
                    objRes.Message = "Data don't exist";
                    objRes.Status = false;
                }
                return objRes;
            }
            catch (Exception e)
            {
                objRes.Message = e.Message;
                objRes.Status = false;
                return objRes;
            }
        }

        #endregion Api for get Sub Category by category id

        #region This Api Is Used To Add Items in ItemMaster

        /// <summary>
        /// Created By Ankit Jain Date - 30/05/2022
        /// Api >> post >> api/ItemMasterAddItem/CreateItem
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        [Route("CreateItem")]
        [HttpPost]
        public async Task<ResponseBodyModel> CreateItem(ItemMaster Item)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                ItemMaster ItemObj = new ItemMaster();
                ItemObj.ItemId = Item.ItemId;
                ItemObj.ItemName = Item.ItemName;
                ItemObj.Categoryid = Item.Categoryid;
                ItemObj.BaseCategoryId = Item.BaseCategoryId;
                ItemObj.SubCategoryId = Item.SubCategoryId;
                ItemObj.WarehouseId = Item.WarehouseId;
                ItemObj.Units = Item.Units;
                ItemObj.Condition = Item.Condition;
                ItemObj.InvoiceUrl = Item.InvoiceUrl;
                ItemObj.CompanyId = claims.companyId;
                ItemObj.OrgId = claims.orgId;
                ItemObj.CreatedBy = claims.employeeId;
                ItemObj.CreatedOn = DateTime.Now;
                ItemObj.IsActive = true;
                ItemObj.IsDeleted = false;
                ItemObj.IsAsset = Item.IsAsset;
                db.ItemMaster.Add(ItemObj);
                await db.SaveChangesAsync();

                ItemHistory HistoryObj = new ItemHistory();
                HistoryObj.ItemId = ItemObj.ItemId;
                HistoryObj.ItemName = Item.ItemName;
                HistoryObj.Categoryid = Item.Categoryid;
                HistoryObj.BaseCategoryId = Item.BaseCategoryId;
                HistoryObj.SubCategoryId = Item.SubCategoryId;
                HistoryObj.WarehouseId = Item.WarehouseId;
                HistoryObj.Units = Item.Units;
                HistoryObj.Condition = Item.Condition;
                HistoryObj.InvoiceUrl = Item.InvoiceUrl;
                HistoryObj.CompanyId = claims.companyId;
                HistoryObj.OrgId = claims.orgId;
                HistoryObj.CreatedBy = claims.employeeId;
                HistoryObj.CreatedOn = DateTime.Now;
                HistoryObj.IsActive = true;
                HistoryObj.IsDeleted = false;
                HistoryObj.IsAsset = Item.IsAsset;
                db.ItemHistoryes.Add(HistoryObj);
                await db.SaveChangesAsync();
                response.Status = true;
                response.Message = "Created Successfully";
                response.Data = ItemObj;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        #endregion This Api Is Used To Add Items in ItemMaster

        #region This Api Is Use To Edit Isasset True

        /// <summary>
        /// Created By Nayan Pancholi Date - 04/04/2022
        /// Api >> Put >>  api/ItemMasterAddItem/EditIsAsset
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EditIsAsset")]
        [HttpPut]
        public async Task<ResponseBodyModel> EditLocation(ItemMaster model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    var Item = await db.ItemMaster.FirstOrDefaultAsync(x => x.ItemId == model.ItemId);
                    if (Item != null)
                    {
                        Item.WarehouseId = model.WarehouseId;
                        Item.IsAsset = true;
                        Item.UpdatedBy = claims.userId;
                        Item.UpdatedOn = DateTime.Now;
                        db.Entry(Item).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();

                        res.Message = "Asset Edited";
                        res.Status = true;
                        res.Data = Item;
                    }
                    else
                    {
                        res.Message = "Asset" +
                            " Not Found";
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

        #endregion This Api Is Use To Edit Isasset True

        #region This Api Is Used for get Items by WarehouseID

        /// <summary>
        /// Created By Nayan Pancholi Date - 04/04/2022
        /// Api >> Get >>  api/ItemMasterAddItem/GetItemsByWareId
        /// </summary>
        /// <param name="WareId"></param>
        /// <returns></returns>
        [Route("GetItemsByWareId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetItemsByWareId(int WareId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel objRes = new ResponseBodyModel();
            try
            {
                var data = await db.ItemMaster.Where(x => x.IsActive == true && x.IsDeleted == false && x.WarehouseId == WareId).ToListAsync();
                if (data.Count > 0)
                {
                    objRes.Message = "Data Found Successful";
                    objRes.Status = true;
                    objRes.Data = data;
                }
                else
                {
                    objRes.Message = "Data don't exist";
                    objRes.Status = false;
                }
                return objRes;
            }
            catch (Exception e)
            {
                objRes.Message = e.Message;
                objRes.Status = false;
                return objRes;
            }
        }

        #endregion This Api Is Used for get Items by WarehouseID

        #region This Api is Used To Add Units In Item Master

        /// <summary>
        /// Created by Ankit Date -30/05/2022
        /// Api >> Post >> api/ItemMasterAddItem/AddUnits
        /// </summary>
        /// <param name="WareId"></param>
        /// <returns></returns>
        [Route("AddUnits")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddUnitsData(ItemMaster Item)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var data = await db.ItemMaster.Where(x => x.ItemId == Item.ItemId && x.IsDeleted == false && x.IsActive == true).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.ItemId = Item.ItemId;
                    var exitingUnit = data.Units;
                    var addUnit = Item.Units;
                    data.Units = addUnit + exitingUnit;
                    data.CompanyId = claims.companyId;
                    data.OrgId = claims.orgId;
                    data.UpdatedBy = claims.userId;
                    data.UpdatedOn = DateTime.Now;
                    data.IsActive = true;
                    data.IsDeleted = false;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    ItemHistory HistoryObj = new ItemHistory();
                    HistoryObj.ItemId = data.ItemId;
                    HistoryObj.Units = Item.Units;
                    HistoryObj.InvoiceUrl = Item.InvoiceUrl;
                    HistoryObj.ItemName = data.ItemName;
                    HistoryObj.Categoryid = data.Categoryid;
                    HistoryObj.BaseCategoryId = data.BaseCategoryId;
                    HistoryObj.SubCategoryId = data.SubCategoryId;
                    HistoryObj.WarehouseId = data.WarehouseId;
                    HistoryObj.CompanyId = claims.companyId;
                    HistoryObj.OrgId = claims.orgId;
                    HistoryObj.CreatedBy = claims.userId;
                    HistoryObj.CreatedOn = DateTime.Now;
                    HistoryObj.IsActive = true;
                    HistoryObj.IsDeleted = false;
                    HistoryObj.IsAsset = data.IsAsset;
                    db.ItemHistoryes.Add(HistoryObj);
                    db.SaveChanges();

                    response.Status = true;
                    response.Message = "Units Update Successfully";
                    response.Data = data;
                }
                else
                {
                    response.Status = false;
                    response.Message = "Units Not Updated";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        #endregion This Api is Used To Add Units In Item Master

        #region This Api is Used To Delete Item

        /// <summary>
        /// Created By Ankit date - 04/04/2022
        /// Api >> Delete >> api/ItemMasterAddItem/deleteItem
        /// </summary>
        /// <param name="DeleteItem"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteItem")]
        public async Task<ResponseBodyModel> DeleteItem(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var Item = await db.ItemMaster.FirstOrDefaultAsync(x => x.ItemId == Id &&
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

        #endregion This Api is Used To Delete Item

        #region This Api Use For Upload Documents For Invoice Document

        /// <summary>
        ///Created By Ankit On 30-05-2022
        /// </summary>Api>>Post>> api/ItemMasterAddItem/uploadInvoicedocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadInvoicedocuments")]
        public async Task<UploadImageResponseItem> UploadInvoiceDocments()
        {
            UploadImageResponseItem result = new UploadImageResponseItem();
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
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/invoicedocinitem/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\invoicedocinitem\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This Api Use For Upload Documents For Invoice Document

        #region This Api Use For Get All Item History

        /// <summary>
        /// Created By Ankit jain Date - 30/05/2022
        /// Api >> Get >>  api/ItemMasterAddItem/getitemhistory
        /// </summary>
        /// <param name="Itemid"></param>
        /// <returns></returns>
        [Route("getitemhistory")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetItemHistory(int Itemid)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var HistoryData = await db.ItemHistoryes.Where(x => x.ItemId == Itemid)
                                      .Select(x => new
                                      {
                                          x.ItemId,
                                          x.Units,
                                          x.InvoiceUrl,
                                          x.ItemName,
                                          x.CreatedOn
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

        #endregion This Api Use For Get All Item History

        #region Add Assets By Excel

        /// <summary>
        /// Created By Suraj Bundel on 25-06-2022
        /// API >> api/ItemMasterAddItem/addassetsbyimport
        /// Model used >>AddItemMasterImport
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("additembyimport")]
        public async Task<ResponseBodyModel> AddassetExcel(List<AddItemMasterImport> models)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<AddItemMasterImport> falultyImportItem = new List<AddItemMasterImport>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var BaseCategoryNameobj = db.ItemMasterBaseCategory.ToList();
            var CategoryNameobj = db.ItemMasterCategory.ToList();
            var SubCategoryNameobj = db.itemMasterSubCategory.ToList();
            //var BaseCategoryNameobj = db.ItemMasterBaseCategory.Where(x => x.BaseCategoryName == models.Select(x.BaseCategoryName)).ToString();
            try
            {
                if (models.Count == 0)
                {
                    res.Message = "Error";
                    res.Status = false;
                }
                else
                {
                    var assetlist = await db.ItemMaster.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                    foreach (var item in models)
                    {
                        ItemMaster addassets = new ItemMaster();
                        int checkBaseCategory = 0, checkcategory = 0, checkSubCategory = 0;

                        checkBaseCategory = BaseCategoryNameobj.Where(x => x.BaseCategoryName == item.BaseCategoryName).Select(x => x.BaseCategoryId).FirstOrDefault();
                        checkcategory = CategoryNameobj.Where(x => x.CategoryName == item.CategoryName).Select(x => x.Categoryid).FirstOrDefault();
                        checkSubCategory = SubCategoryNameobj.Where(x => x.SubcategoryName == item.SubcategoryName).Select(x => x.SubCategoryId).FirstOrDefault();
                        if (checkBaseCategory != 0 && checkcategory != 0 && checkSubCategory != 0)
                        {
                            addassets.ItemName = item.ItemName;
                            addassets.BaseCategoryId = checkBaseCategory;
                            addassets.BaseCategoryName = item.BaseCategoryName;
                            addassets.Categoryid = checkcategory;
                            addassets.CategoryName = item.CategoryName;
                            addassets.SubCategoryId = item.SubCategoryId;
                            addassets.SubcategoryName = item.SubcategoryName;
                            addassets.WarehouseId = checkSubCategory;
                            addassets.WarehouseName = item.WarehouseName;
                            addassets.Condition = item.Condition;
                            addassets.Itemcode = item.Itemcode;
                            //addassets.IsAsset = item.IsAsset;
                            addassets.Units = item.Units;
                            addassets.UniqueCode = item.UniqueCode;
                            addassets.SerialNumber = item.SerialNumber;
                            //addassets.InvoiceUrl = item.InvoiceUrl;
                            addassets.PurchasedOn = item.PurchasedOn;
                            addassets.AssetDescription = item.AssetDescription;
                            addassets.AssetLocation = item.AssetLocation;
                            addassets.AssetStatus = item.AssetStatus;
                            addassets.AssetType = item.AssetType;
                            addassets.ReasonifNotAvailable = item.ReasonifNotAvailable;
                            addassets.EmployeeIdifAssigned = item.EmployeeIdifAssigned;
                            addassets.EmployeeNameifAssigned = item.EmployeeNameifAssigned;
                            addassets.EmployeeDepartmentifAssigned = item.EmployeeDepartmentifAssigned;
                            addassets.DateofAssetAssignment = item.DateofAssetAssignment;
                            addassets.IsActive = true;
                            addassets.IsDeleted = false;
                            addassets.CompanyId = claims.companyId;
                            addassets.OrgId = claims.orgId;
                            addassets.CreatedBy = claims.employeeId;
                            addassets.CreatedOn = DateTime.Now;
                            db.ItemMaster.Add(addassets);
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            falultyImportItem.Add(item);
                        }
                    }
                    if (falultyImportItem.Count > 0)
                    {
                        res.Message = ("Failed to add data of " + falultyImportItem.Count() + " Lines");
                        res.Status = false;
                        res.Data = falultyImportItem;
                    }
                    else
                    {
                        res.Message = "Data added successfully";
                        res.Status = true;
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

        #endregion Add Assets By Excel

        #region upload in excel //  row wise // bulk data//not in use

        /// <summary>
        /// Created By Suraj Bundel on 27-06-2022
        /// API >> Get >> api/ItemMasterAddItem/uploadexcelfilebulkdata
        /// </summary>
        /// <returns></returns>

        [Route("uploadexcelfilebulkdata")]
        [HttpPost]
        public string uploadfile()
        {
            try
            {
                #region Variable Declaration

                HttpResponseMessage ResponseMessage = null;
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;

                #endregion Variable Declaration

                #region Save Student Detail From Excel

                ResponseBodyModel response = new ResponseBodyModel();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        Inputfile = httpRequest.Files[0];
                        FileStream = Inputfile.InputStream;

                        if (Inputfile != null && FileStream != null)
                        {
                            if (Inputfile.FileName.EndsWith(".xls"))
                                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                            else if (Inputfile.FileName.EndsWith(".xlsx"))
                                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                            else
                            {
                                response.Message = "The file format is not supported.";
                                response.Status = false;
                            }
                            dsexcelRecords = reader.AsDataSet();
                            reader.Close();

                            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtRecords = dsexcelRecords.Tables[0];
                                for (int i = 2; i < dtRecords.Rows.Count; i++)
                                {
                                    ItemMaster obj = new ItemMaster
                                    {
                                        ItemId = Convert.ToInt32(dtRecords.Rows[i][0]),
                                        ItemName = Convert.ToString(dtRecords.Rows[i][1]),
                                        BaseCategoryId = Convert.ToInt32(dtRecords.Rows[i][2]),
                                        BaseCategoryName = Convert.ToString(dtRecords.Rows[i][3]),
                                        Categoryid = Convert.ToInt32(dtRecords.Rows[i][4]),
                                        CategoryName = Convert.ToString(dtRecords.Rows[i][5]),
                                        SubCategoryId = Convert.ToInt32(dtRecords.Rows[i][6]),
                                        SubcategoryName = Convert.ToString(dtRecords.Rows[i][7]),
                                        WarehouseId = Convert.ToInt32(dtRecords.Rows[i][8]),
                                        WarehouseName = Convert.ToString(dtRecords.Rows[i][9]),
                                        Condition = Convert.ToString(dtRecords.Rows[i][10]),
                                        Itemcode = Convert.ToString(dtRecords.Rows[i][11]),
                                        IsAsset = Convert.ToBoolean(dtRecords.Rows[i][12]),
                                        Units = Convert.ToInt32(dtRecords.Rows[i][13]),
                                        UniqueCode = Convert.ToInt32(dtRecords.Rows[i][14]),
                                        SerialNumber = Convert.ToString((int)dtRecords.Rows[i][15]),
                                        PurchasedOn = Convert.ToDateTime(dtRecords.Rows[i][16]),
                                        AssetDescription = Convert.ToString(dtRecords.Rows[i][17]),
                                        AssetLocation = Convert.ToString(dtRecords.Rows[i][18]),
                                        AssetStatus = Convert.ToString(dtRecords.Rows[i][19]),
                                        AssetType = Convert.ToString(dtRecords.Rows[i][20]),
                                        ReasonifNotAvailable = Convert.ToString(dtRecords.Rows[i][21]),
                                        EmployeeIdifAssigned = Convert.ToInt32(dtRecords.Rows[i][22]),
                                        EmployeeNameifAssigned = Convert.ToString(dtRecords.Rows[i][23]),
                                        EmployeeDepartmentifAssigned = Convert.ToString(dtRecords.Rows[i][24]),
                                        DateofAssetAssignment = Convert.ToDateTime(dtRecords.Rows[i][25]),
                                    };
                                }
                                int output = db.SaveChanges();
                                if (output > 0)
                                {
                                    response.Message = "The Excel file has been successfully uploaded.";
                                    response.Status = true;
                                }
                                else
                                {
                                    response.Message = "Something Went Wrong!, The Excel file uploaded has fiald.";
                                    response.Status = false;
                                }
                            }
                            else
                            {
                                response.Message = "Selected file is empty.";
                                response.Status = false;
                            }
                        }
                        else
                        {
                            response.Message = "Invalid File.";
                            response.Status = false;
                        }
                    }
                    else
                    {
                        ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
                        response.Status = false;
                    }
                }
                return response.Message;

                #endregion Save Student Detail From Excel
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion upload in excel //  row wise // bulk data//not in use

        #region Get All Asset Icons // dropdown

        /// <summary>
        /// Created By Suraj Bundel on 04-07-2022
        /// API >> Get >> api/ItemMasterAddItem/getasseticonlist
        /// </summary>
        /// <returns></returns>

        [Route("getasseticonlist")]
        [HttpGet]
        public ResponseBodyModel GetassetIconlist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<IconHelperModelClass> list = new List<IconHelperModelClass>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                for (int i = 1; i < 10; i++)
                {
                    IconHelperModelClass obj = new IconHelperModelClass
                    {
                        IconId = i,
                        IconURL = "uploadimage/AssetIcons/" + i + ".svg",
                    };
                    list.Add(obj);
                }
                res.Message = "Icon List";
                res.Status = true;
                res.Data = list;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class IconHelperModelClass
        {
            public int IconId { get; set; }
            public string IconURL { get; set; }
        }

        private static Dictionary<int, string> Iconlist;

        public static void asseticonlist()
        {
            foreach (var icon in Iconlist)
            {
                Iconlist = new Dictionary<int, string>();
                {
                    Iconlist.Add(1, "uploadimage/AssetIcons/1.svg");
                    Iconlist.Add(2, "uploadimage/AssetIcons/2.svg");
                    Iconlist.Add(3, "uploadimage/AssetIcons/3.svg");
                    Iconlist.Add(4, "uploadimage/AssetIcons/4.svg");
                    Iconlist.Add(5, "uploadimage/AssetIcons/5.svg");
                    Iconlist.Add(6, "uploadimage/AssetIcons/6.svg");
                    Iconlist.Add(7, "uploadimage/AssetIcons/7.svg");
                    Iconlist.Add(8, "uploadimage/AssetIcons/8.svg");
                    Iconlist.Add(9, "uploadimage/AssetIcons/9.svg");
                }
            }
        }

        #endregion Get All Asset Icons // dropdown

        #region GetAssignList

        /// <summary>
        /// Created By Suraj Bundel on 04/07/2022
        /// Get >> API >> api/ItemMasterAddItem/assignasset
        /// </summary>
        /// <returns></returns>
        [Route("assignasset")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAssignList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<Assetlist> list = new List<Assetlist>();
            List<AddItemMasterImport> ItemMasterList = new List<AddItemMasterImport>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetdata = await (from ass in db.AssignMasterData
                                       join it in db.ItemMaster on ass.ItemId equals it.ItemId
                                       join em in db.Employee on ass.AssignedToId equals em.EmployeeId
                                       join dp in db.Department on em.DepartmentId equals dp.DepartmentId
                                       where em.CompanyId == claims.companyId && ass.IsDeleted == false && ass.IsActive == true
                                       //&& it.EmployeeIdifAssigned != 0
                                       select new
                                       {
                                           it.ItemId,
                                           it.ItemName,
                                           it.SerialNumber,
                                           it.Itemcode,
                                           em.DisplayName,
                                           it.EmployeeIdifAssigned,
                                           dp.DepartmentName,
                                           ass.AssignedToId,
                                           ass.AssetsKey,
                                           ass.Condition,
                                           ass.WarehouseId,
                                           ass.AssignId,
                                           ass.IconId,
                                           ass.IconUrl
                                       }).ToListAsync();

                var employeeIds = assetdata.Select(x => new DistingByAssest
                {
                    //x.EmployeeIdifAssigned,
                    AssignedToId = x.AssignedToId,
                    DisplayName = x.DisplayName,
                    DepartmentName = x.DepartmentName,
                    ItemId = x.ItemId,
                    AssetsKey = x.AssetsKey,
                    Condition = x.Condition,
                    WarehouseId = x.WarehouseId,
                    AssignId = x.AssignId,
                    IconUrl = "uploadimage/AssetIcons/" + x.IconId + ".svg",
                }).ToList();
                var empId = employeeIds.Select(x => x.AssignedToId).Distinct().ToList();

                foreach (var item in empId)
                {
                    var data = employeeIds.Where(x => x.AssignedToId == item).FirstOrDefault();
                    Assetlist obj = new Assetlist
                    {
                        EmployeeId = data.AssignedToId,
                        EmployeeName = data.DisplayName,
                        Department = data.DepartmentName,
                        AssesdataList = assetdata.Where(x => x.AssignedToId == data.AssignedToId /*item.EmployeeIdifAssigned*/)
                                .Select(x => new Assetdatamodel
                                {
                                    EmployeeName = x.DisplayName,
                                    ItemId = x.ItemId,
                                    ItemName = x.ItemName,
                                    Model = x.Itemcode,
                                    Serialnumber = x.SerialNumber,
                                    AssetsKey = x.AssetsKey,
                                    Condition = x.Condition,
                                    WarehouseId = x.WarehouseId,
                                    AssignId = x.AssignId,
                                    IconUrl = "uploadimage/AssetIcons/" + x.IconId + ".svg",
                                }).ToList(),
                    };
                    list.Add(obj);
                }

                if (list.Count > 0)
                {
                    res.Message = "Assign asset list found";
                    res.Status = true;
                    res.Data = list;
                }
                else
                {
                    res.Message = "No Item Is Assigned";
                    res.Status = false;
                    res.Data = list;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return res;
        }

        public class AssetsDataModelClass
        {
            public int ItemId { get; set; }
            public int ItemName { get; set; }
            public string SerialNumber { get; set; }
            public string Itemcode { get; set; }
            public string DisplayName { get; set; }
            public int EmployeeIdifAssigned { get; set; }
            public string DepartmentName { get; set; }
            public int AssignedToId { get; set; }
            public string AssetsKey { get; set; }
            public string Condition { get; set; }
            public int WarehouseId { get; set; }
            public long AssignId { get; set; }
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
        }

        public class Assetlist
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Department { get; set; }
            public List<Assetdatamodel> AssesdataList { get; set; }
        }

        #endregion GetAssignList

        //#region  Update Asset Condition
        ///// <summary>
        ///// API >> Put >> api/assets/updateassetcondition
        /////Created  by Shriya
        ///// </summary>
        ///// <param name="asset"></param>
        ///// <returns></returns>
        //[Route("UpdateAssetCondition")]
        //[HttpPut]
        //public async Task<ResponseBodyModel> UpdateAssetCondition(FaultyAssetsData faultasset)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //Base response = new Base();
        //        //var assetData = db.AssetMaster.Where(x => x.AssetId == asset.AssetId && x.IsDeleted == false).FirstOrDefault();
        //        var assetData = db.faultyAssetsData.Where(x => x.FaultyAssetId == faultasset.FaultyAssetId && x.IsDeleted == false).FirstOrDefault();

        //        //var assetData = (from ad in db.AssetMaster
        //        //                 join bd in db.Condition on ad.ConditionName equals bd.ConditionName
        //        //                 select new
        //        //                 {
        //        //                     bd.ConditionName,
        //        //                     ad.ConditionId
        //        //                 }).ToList();
        //        if (assetData != null)
        //        {
        //            // assetData.ConditionId = asset.ConditionId;

        //            //assetData.Condition = db.Condition.Where(a => a.ConditionName == assetData.ConditionId).Select(x => x.ConditionName).FirstOrDefault();
        //            //if (assetData.ConditionName == "Good") { assetData.Status = "AVAILABLE"; } else { assetData.Status = "NOT AVAILABLE"; }
        //            assetData.UpdatedBy = claims.employeeid;
        //            assetData.UpdatedOn = DateTime.Now;
        //            assetData.IsActive = true;
        //            assetData.IsDeleted = false;
        //            db.Entry(assetData).State = System.Data.Entity.EntityState.Modified;
        //            await db.SaveChangesAsync();
        //            response.Status = true;
        //            response.Message = "Condition Updated Successfully!";
        //            response.Data = assetData;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Data = null;
        //        response.Message = ex.Message;
        //        response.Status = false;
        //    }
        //    return response;
        //}
        //#endregion

        //#region Update Asset Condition (Assets recover)
        ///// <summary>
        ///// API >> Put >>  api/assets/updateassetdetails
        ///// Created by Suraj Bundel on 06/07/2022
        ///// </summary>
        ///// <param name = "asset" ></ param >
        ///// < returns ></ returns >
        //[Route("updatefaultyassetdetails")]
        //[HttpPut]
        //public async Task<ResponseBodyModel> UpdateAssetDetails(FaultyAssetsData faultyasset)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        var assetAssD = db.faultyAssetsData.Where(x => x.ItemId == faultyasset.ItemId /*&& x.AssetsKey == asset.AssetsKey*/ && x.IsDeleted == false).FirstOrDefault();
        //        if (assetAssD != null)
        //        {
        //            assetAssD.AssetRecoveredById = faultyasset.AssetRecoveredById;
        //            assetAssD.AssetRecoveredBy = db.Employee.Where(a => a.EmployeeId == assetAssD.AssetRecoveredById).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
        //            assetAssD.CompanyId = claims.companyid;
        //            assetAssD.OrgId = claims.orgid;
        //            assetAssD.DeletedBy = claims.employeeid;
        //            assetAssD.DeletedOn = DateTime.Now;
        //            assetAssD.IsActive = true;
        //            assetAssD.IsDeleted = false;
        //            db.Entry(assetAssD).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
        //            await db.SaveChangesAsync();
        //            response.Status = true;
        //            response.Message = "Assign assets recover succesfully";

        //            response.Data = assetAssD;

        //            if (faultyasset.Condition == "Good")
        //            {
        //                var data = db.ItemMaster.Where(x => x.ItemId == faultyasset.ItemId && x.WarehouseId == faultyasset.WarehouseId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
        //                if (data != null)
        //                {
        //                    data.ItemId = faultyasset.ItemId;
        //                    var exitingUnit = data.Units;
        //                    data.Units = exitingUnit + 1;
        //                    data.CompanyId = claims.companyid;
        //                    data.OrgId = claims.orgid;
        //                    data.UpdatedBy = claims.employeeid;
        //                    data.UpdatedOn = DateTime.Now;
        //                    data.IsActive = true;
        //                    data.IsDeleted = false;

        //                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
        //                    db.SaveChanges();
        //                }
        //            }
        //            else
        //            {
        //                FaultyAssetsData faultyAss = new FaultyAssetsData();
        //                faultyAss.ItemId = faultyasset.ItemId;
        //                faultyAss.Assetskey = faultyasset.Assetskey;
        //                faultyAss.Condition = faultyasset.Condition;
        //                faultyAss.Comments = faultyasset.Comments;
        //                faultyAss.AssetRecoveredById = faultyasset.AssetRecoveredById;
        //                faultyAss.AssetRecoveredBy = db.Employee.Where(a => a.EmployeeId == faultyAss.AssetRecoveredById).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
        //                faultyAss.WarehouseId = faultyasset.WarehouseId;
        //                faultyAss.CreatedBy = claims.employeeid;
        //                faultyAss.CreatedOn = DateTime.Now;
        //                faultyAss.IsActive = true;
        //                faultyAss.IsDeleted = false;
        //                db.faultyAssetsData.Add(faultyAss); // add data in faulty assests ..
        //                db.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            //response.Message = "Faulty assets update succesfully";
        //            response.Message = "Faulty assets recovery failed";
        //            response.Data = null;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        response.Data = null;
        //        response.Message = ex.Message;
        //        response.Status = false;
        //        return response;
        //    }
        //    return response;
        //}
        //#endregion

        #region Helper  AddAssetImport

        /// <summary>
        /// Created by Suraj Bundel on 24-06-2022
        /// </summary>
        public class AddItemMasterImport
        {
            public int ItemId { get; set; }  // primary key
            public string ItemName { get; set; }        // Asset Name
            public int BaseCategoryId { get; set; }
            public string BaseCategoryName { get; set; }
            public int Categoryid { get; set; }
            public string CategoryName { get; set; }        // category name
            public int SubCategoryId { get; set; }
            public string SubcategoryName { get; set; }
            public int WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public string Condition { get; set; }
            public string Itemcode { get; set; }
            public bool IsAsset { get; set; }
            public int Units { get; set; }
            public int UniqueCode { get; set; }
            public string SerialNumber { get; set; }
            public string InvoiceUrl { get; set; }
            public DateTime? PurchasedOn { get; set; }
            //------------------------------------------

            public string AssetDescription { get; set; }
            public string AssetLocation { get; set; }
            public string AssetStatus { get; set; }
            public string AssetType { get; set; }
            public string ReasonifNotAvailable { get; set; }
            public int EmployeeIdifAssigned { get; set; }                      //Employee Id -> assigned to
            public string EmployeeDepartmentifAssigned { get; set; }
            public string EmployeeNameifAssigned { get; set; }
            public DateTime? DateofAssetAssignment { get; set; }
        }

        #endregion Helper  AddAssetImport

        #region Helper Model

        /// <summary>
        /// Created By Ankit Jain Date-30/05/2022
        /// </summary>
        public class UploadImageResponseItem
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        #endregion Helper Model
    }
}