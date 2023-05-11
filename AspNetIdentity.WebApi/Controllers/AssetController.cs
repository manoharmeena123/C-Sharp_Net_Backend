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
    //[EnableCors(origins: "*", headers: "*", methods: "*")]

    [Authorize]
    [RoutePrefix("api/assets")]
    public class AssetController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        //#region Get All the Assets

        ///// <summary>
        ///// API >> Get >> api/assets/getalltheassets
        ///// edited by shriya
        ///// </summary>
        ///// <returns></returns>
        //[Route("GetAlltheAssets")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetAlltheAssets()
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //AssetDataList res = new AssetDataList();
        //        List<AssetData> AssetDataList = new List<AssetData>();
        //        var finalData = await _db.AssetMaster.Where(x => x.IsDeleted == false).ToListAsync();

        //        foreach (var item in finalData)
        //        {
        //            AssetData data = new AssetData();
        //            data.AssetId = item.AssetId;
        //            data.AssetName = item.AssetName;
        //            data.Location = item.Location;
        //            data.ConditionName = item.ConditionName;
        //            data.Status = item.Status;
        //            data.AssignedToId = item.AssignedToId;
        //            data.AssignedTo = item.AssignedTo;
        //            data.CategoryId = item.CategoryId;
        //            data.CategoryName = _db.Category.Where(x => x.CategoryTypeId == item.CategoryId).Select(x => x.CategoryName).FirstOrDefault();
        //            data.SubCategoryId = _db.SubCategorysHindmt.Where(x => x.Categoryid == item.CategoryId).Select(x => x.SubCategoryId).FirstOrDefault();
        //            data.SubCategoryName = _db.SubCategorysHindmt.Where(s => s.Categoryid == item.CategoryId).Select(x => x.SubcategoryName).FirstOrDefault();
        //            data.ConditionId = item.ConditionId;
        //            data.ConditionName = item.ConditionName;
        //            data.BaseCategoryId = item.BaseCategoryId;
        //            data.BaseCategoryName = _db.BaseCategoryDb.Where(b => b.BaseCategoryId == item.BaseCategoryId).Select(b => b.BaseCategoryName).FirstOrDefault();
        //            data.NotAvailableReasonId = item.NotAvailableReasonId;
        //            data.ReasonName = _db.Reason.Where(r => r.ReasonId == item.NotAvailableReasonId).Select(r => r.ReasonName).FirstOrDefault();
        //            data.Status = item.Status;
        //            data.SubsubCategoryid = _db.SubsubCategorys.Where(c => c.Categoryid == item.CategoryId).Select(x => x.SubsubCategoryid).FirstOrDefault();
        //            data.SubsubCategoryName = _db.SubsubCategorys.Where(s => s.Categoryid == item.CategoryId).Select(x => x.SubcategoryName).FirstOrDefault();

        //            AssetDataList.Add(data);
        //        }
        //        //int pageSize = 10;

        //        //int pageNumber = page;
        //        //return Ok(AssetDataList.ToPagedList(pageNumber, pageSize));
        //        if (AssetDataList.Count != 0)
        //        {
        //            response.Message = "Asset data list found";
        //            response.Status = true;
        //            response.Data = AssetDataList;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.Status = false;
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

        //#endregion Get All the Assets

        //#region GetAllCentralAssets

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        //[Route("GetAllCentralAssets")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetAllCentralAssets()
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //AssetDataList res = new AssetDataList();
        //        List<AssetData> AssetDataList = new List<AssetData>();
        //        var finalData = await (from ad in _db.ItemMasterCentralDB
        //                               join ce in _db.AssetMaster on ad.CategoryName equals ce.AssetName
        //                               //ad.AssignedTo == Id
        //                               select new
        //                               {
        //                                   ad.Id,
        //                                   ad.SubCategoryId,
        //                                   ad.SubsubCategoryid,
        //                                   ad.Categoryid,
        //                                   ce.AssignedTo,
        //                                   ce.AssetName,
        //                                   ce.AssetId,
        //                                   ad.CategoryName,
        //                                   ce.ConditionId
        //                               }).ToListAsync();

        //        foreach (var item in finalData)
        //        {
        //            AssetData data = new AssetData();
        //            data.Id = item.Id;
        //            data.AssetId = item.AssetId;
        //            data.AssetName = item.AssetName;
        //            data.AssignedTo = item.AssignedTo;
        //            data.ConditionId = item.ConditionId;
        //            data.CategoryId = item.Categoryid;
        //            data.CategoryName = item.CategoryName;
        //            data.SubCategoryId = item.SubCategoryId;
        //            data.SubsubCategoryid = item.SubsubCategoryid;
        //            // data.AssetTitle = item.AssetTitle;
        //            // data.AssetType = item.AssetType;
        //            // data.AssignedBy = item.w;
        //            //  data.Assetstatus = item.StatusVal;
        //            //  data.AssetstatusId = item.AssetstatusId;
        //            //   data.AssetTypeId = item.AssetTypeId;
        //            //  data.AssetDescription = item.AssetDescription;
        //            //  data.Comment = item.Comment;
        //            AssetDataList.Add(data);
        //        }
        //        //int pageSize = 10;

        //        //int pageNumber = page;
        //        //return Ok(AssetDataList.ToPagedList(pageNumber, pageSize));
        //        if (AssetDataList.Count != 0)
        //        {
        //            response.Message = "Asset data list found";
        //            response.Status = true;
        //            response.Data = AssetDataList;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.Status = false;
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

        //#endregion GetAllCentralAssets

        #region GetMyAssets

        /// <summary>
        /// API >> Get >> api/assets/GetMyAssets
        /// Created by shriya Created on 20-04-2022
        /// </summary>
        /// <returns></returns>
        [Route("GetMyAssets")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetMyAssets()
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<AssignAssetsData> AssignAssetDataList = new List<AssignAssetsData>();
                //var empId = db.User.Where(x => x.EmployeeId == claims.employeeid).Select(x => x.EmployeeId).FirstOrDefault();
                var AssignData = await _db.AssignMasterData.Where(x => x.AssignedToId == claims.employeeId && x.IsDeleted == false && x.IsActive == true).ToListAsync();
                foreach (var assignItem in AssignData)
                {
                    AssignAssetsData assets = new AssignAssetsData();
                    assets.ItemId = assignItem.ItemId;
                    assets.ItemName = _db.ItemMaster.Where(x => x.ItemId == assets.ItemId).Select(x => x.ItemName).FirstOrDefault();
                    assets.AssignedToId = assignItem.AssignedToId;
                    assets.AssignedTo = assignItem.AssignedTo;
                    assets.Condition = assignItem.Condition;
                    assets.AssetsKey = assignItem.AssetsKey;
                    assets.WarehouseId = assignItem.WarehouseId;
                    assets.Comment = assignItem.Comment;
                    assets.CreateOn = assignItem.CreatedOn;
                    AssignAssetDataList.Add(assets);
                }
                if (AssignAssetDataList.Count > 0)
                {
                    response.Status = true;
                    response.Message = "get all asset of user";
                    response.Data = AssignAssetDataList;
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

        #endregion GetMyAssets

        //#region CreateAsset

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="asset"></param>
        ///// <returns></returns>
        //[Route("CreateAsset")]
        //[HttpPost]
        //public async Task<ResponseBodyModel> CreateAsset(AssetMaster asset)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        //Base response = new Base();
        //        AssetMaster AssetObj = new AssetMaster();

        //        AssetObj.AssetName = asset.AssetName;
        //        AssetObj.CategoryId = asset.CategoryId;
        //        AssetObj.AssignedTo = asset.AssignedTo;
        //        AssetObj.ConditionId = asset.ConditionId;
        //        AssetObj.SubCategoryId = asset.SubCategoryId;
        //        AssetObj.IconImageUrl = asset.IconImageUrl;
        //        AssetObj.ImageUrl = asset.ImageUrl;
        //        AssetObj.Location = asset.Location;
        //        AssetObj.CompanyId = claims.companyid;
        //        AssetObj.OrgId = claims.orgid;

        //        AssetObj.CreatedBy = claims.employeeid;
        //        AssetObj.UpdatedBy = claims.employeeid;
        //        AssetObj.CreatedOn = DateTime.Now;
        //        AssetObj.UpdatedOn = DateTime.Now;
        //        AssetObj.IsActive = true;
        //        AssetObj.IsDeleted = false;

        //        // Assetobj.Attributes["customerid"] = new EntityReference("account", new Guid("7E100331-7419-E911-A96A-000D3AF2924B"));
        //        _db.AssetMaster.Add(AssetObj);
        //        await _db.SaveChangesAsync();

        //        //AssetCategory category = new AssetCategory();
        //        //category.CategoryId = AssetObj.CategoryId;
        //        //db.AssetCategory.Add(category);
        //        //db.SaveChanges();

        //        //Condition condition = new Condition();
        //        //condition.ConditionId = AssetObj.ConditionId;
        //        //db.Condition.Add(condition);
        //        //db.SaveChanges();

        //        //var AssetObjM = (from ad in db.Employee where ad.EmployeeId == AssetObj.AssignedTo select ad).FirstOrDefault();

        //        //UserEmailDTOResponse responseMail = new UserEmailDTOResponse();
        //        //if (AssetObjM.MoreyeahsMailId != null)
        //        //{
        //        //    UserEmail MailModel = new UserEmail();
        //        //    MailModel.To = AssetObjM.MoreyeahsMailId;//"sachins@moreyeahs.co";
        //        //    //MailModel.FromMail = "";
        //        //    //MailModel.MailPassword = "";
        //        //    MailModel.Subject = "Asset"; //add subject here
        //        //    MailModel.Body = "Hello " + "<b>" + AssetObjM.FirstName + " " + AssetObjM.LastName + "</b>" + ",<br><br> " +
        //        //        "Please find the Asset assigned to you with below description." + "<br> " +
        //        //         //"This Asset assign to you.<br>" +
        //        //         "<b>Asset No:-</b>  " + AssetObj.AssetId + "<br> " +
        //        //         //"<b>Ticket Name:-</b>  " + AssetData. + "<br> " +
        //        //         "<b>Asset Name:-</b>  " + AssetObj.AssetName + "<br> " +
        //        //         "<b>For More Details View Portal:-</b>  <br>" +
        //        //         " https://uatmoreyeahsportal.moreyeahs.in/ " + "<br><br> " +// -(redirect to the Asset page)

        //        //        " Best Regards<br> " +
        //        //          "Team MoreYeahs";
        //        //    var EmailResponse = UserEmailHelper.SendEmail(MailModel);
        //        //}
        //        //else
        //        //{
        //        //    responseMail.Message = "Email doesn't exist";
        //        //    responseMail.Success = false;
        //        //}

        //        response.Status = true;
        //        response.Message = "Asset Created Successfully";
        //        response.Data = AssetObj;
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

        //#endregion CreateAsset

        //#region AssignAsset

        ///// <summary>
        ///// API >> Post >> api/assets/AssignAsset
        ///// Created by Shriya
        /////</summary>
        ///// <param name="asset"></param>
        ///// <returns></returns>
        //[Route("AssignAsset")]
        //[HttpPost]
        //public async Task<ResponseBodyModel> AssignAsset(AssignMasterData asset)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //Base response = new Base();
        //        AssignMasterData assetObj = new AssignMasterData();
        //        assetObj.ItemId = asset.ItemId;
        //        assetObj.ItemName = _db.ItemMaster.Where(x => x.ItemId == assetObj.ItemId).Select(x => x.ItemName).FirstOrDefault();
        //        assetObj.AssignedToId = asset.AssignedToId;
        //        assetObj.AssignedTo = _db.Employee.Where(a => a.EmployeeId == assetObj.AssignedToId).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
        //        assetObj.Condition = asset.Condition;
        //        assetObj.AssetsKey = asset.AssetsKey;
        //        assetObj.Comment = asset.Comment;
        //        assetObj.WarehouseId = asset.WarehouseId;
        //        assetObj.CompanyId = claims.companyid;
        //        assetObj.OrgId = claims.orgid;
        //        assetObj.CreatedBy = claims.employeeid;
        //        assetObj.CreatedOn = DateTime.Now;
        //        assetObj.IsActive = true;
        //        assetObj.IsDeleted = false;
        //        assetObj.AvailablityStatus = true;
        //        assetObj.IconId = asset.IconId;
        //        assetObj.IconUrl = asset.IconUrl;  // change by suraj Bundel
        //        _db.AssignMasterData.Add(assetObj); // add data in assign master tabel
        //        await _db.SaveChangesAsync();
        //        response.Status = true;
        //        response.Message = "Asset Assigned Successfully";
        //        response.Data = assetObj;

        //        var data = _db.ItemMaster.Where(x => x.ItemId == asset.ItemId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
        //        if (data != null)
        //        {
        //            data.ItemId = asset.ItemId;
        //            var exitingUnit = data.Units;
        //            data.Units = exitingUnit - 1;
        //            data.CompanyId = claims.companyid;
        //            data.OrgId = claims.orgid;
        //            data.UpdatedBy = claims.employeeid;
        //            data.UpdatedOn = DateTime.Now;
        //            data.IsActive = true;
        //            data.IsDeleted = false;
        //            _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
        //            _db.SaveChanges();
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

        //#endregion AssignAsset

        //#region CreateCategory

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="asset"></param>
        ///// <returns></returns>
        //[Route("CreateCategory")]
        //[HttpPost]
        //public async Task<ResponseBodyModel> CreateCategory(AssetCategory asset)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        //Base response = new Base();
        //        AssetCategory category = new AssetCategory();
        //        category.CategoryId = asset.CategoryId;
        //        category.CategoryTypeId = asset.CategoryTypeId;
        //        category.CategoryName = asset.CategoryName;
        //        category.IconImageUrl = asset.IconImageUrl;
        //        category.ImageUrl = asset.ImageUrl;

        //        category.CompanyId = claims.companyid;
        //        category.OrgId = claims.orgid;

        //        category.CreatedBy = claims.employeeid;
        //        category.UpdatedBy = claims.employeeid;
        //        category.CreatedOn = DateTime.Now;
        //        category.UpdatedOn = DateTime.Now;
        //        category.IsActive = true;
        //        category.IsDeleted = false;

        //        _db.AssetCategory.Add(category);
        //        await _db.SaveChangesAsync();

        //        response.Status = true;
        //        response.Message = "Category Created Successfully";
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

        //#endregion CreateCategory

        //#region GetAlltheReasons

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        //[Route("GetAlltheReasons")]
        //[HttpGet]
        //public IHttpActionResult GetAlltheReasons()
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

        //        ReasonsDataList res = new ReasonsDataList();
        //        List<ReasonData> ReasonDataList = new List<ReasonData>();
        //        var finalData = _db.Reason.Where(x => x.IsDeleted == false && x.CompanyId == compid && x.OrgId == orgid).ToList();

        //        foreach (var item in finalData)
        //        {
        //            ReasonData data = new ReasonData();
        //            data.ReasonId = item.ReasonId;
        //            data.ReasonName = item.ReasonName;
        //            ReasonDataList.Add(data);
        //        }
        //        //int pageSize = 10;

        //        //int pageNumber = page;
        //        //return Ok(AssetDataList.ToPagedList(pageNumber, pageSize));
        //        if (ReasonDataList.Count != 0)
        //        {
        //            res.Message = "Asset Reason list found";
        //            res.Status = "Ok";
        //            res.MyReasonsList = ReasonDataList;
        //        }
        //        else
        //        {
        //            res.Message = "Data Not Found";
        //            res.Status = "Error";
        //        }
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion GetAlltheReasons

        //#region GetReasonsById

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[Route("GetReasonsById")]
        //[HttpGet]
        //public IHttpActionResult GetReasonsById(int Id)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        ReasonsDataList res = new ReasonsDataList();
        //        List<ReasonData> ReasonDataList = new List<ReasonData>();

        //        var finalData = _db.Reason.Where(x => x.ReasonId == Id).ToList();
        //        foreach (var item in finalData)
        //        {
        //            ReasonData data = new ReasonData();
        //            data.ReasonId = item.ReasonId;
        //            data.ReasonName = item.ReasonName;
        //            ReasonDataList.Add(data);
        //        }
        //        if (ReasonDataList.Count != 0)
        //        {
        //            res.Message = "Reason list found";
        //            res.Status = "Ok";
        //            res.MyReasonsList = ReasonDataList;
        //        }
        //        else
        //        {
        //            res.Message = "Data Not Found";
        //            res.Status = "Error";
        //        }
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#endregion GetReasonsById

        #region GetAlltheConditions

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [Route("GetAlltheConditions")]
        [HttpGet]
        public IHttpActionResult GetAlltheConditions()
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                int userid = 0;
                int compid = 0;
                int orgid = 0;

                // Access claims

                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

                ConditionDataList res = new ConditionDataList();
                List<ConditionData> ConditionDataList = new List<ConditionData>();
                var finalData = _db.Condition.Where(x => x.IsDeleted == false && x.CompanyId == compid && x.OrgId == orgid).ToList();

                foreach (var item in finalData)
                {
                    ConditionData data = new ConditionData();
                    data.ConditionId = item.ConditionId;
                    data.ConditionName = item.ConditionName;
                    ConditionDataList.Add(data);
                }
                //int pageSize = 10;

                //int pageNumber = page;
                //return Ok(AssetDataList.ToPagedList(pageNumber, pageSize));
                if (ConditionDataList.Count != 0)
                {
                    res.Message = "Condition list found";
                    res.Status = "Ok";
                    res.MyConditionsList = ConditionDataList;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = "Error";
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion GetAlltheConditions

        //#region GetConditionsById

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[Route("GetConditionsById")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetConditionsById(int Id)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //Base response = new Base();
        //        //ConditionDataList res = new ConditionDataList();
        //        List<ConditionData> ConditionDataList = new List<ConditionData>();

        //        var finalData = await _db.Condition.Where(x => x.ConditionId == Id).ToListAsync();
        //        foreach (var item in finalData)
        //        {
        //            ConditionData data = new ConditionData();
        //            data.ConditionId = item.ConditionId;
        //            data.ConditionName = item.ConditionName;
        //            ConditionDataList.Add(data);
        //        }
        //        if (ConditionDataList.Count != 0)
        //        {
        //            response.Message = "Condition data list found";
        //            response.Status = true;
        //            response.Data = ConditionDataList;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.Status = false;
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

        //#endregion GetConditionsById

        //#region Update Asset Condition

        ///// <summary>
        ///// API >> Put >> api/assets/updateassetcondition
        /////Created  by Shriya
        ///// </summary>
        ///// <param name="asset"></param>
        ///// <returns></returns>
        //[Route("UpdateAssetCondition")]
        //[HttpPut]
        //public async Task<ResponseBodyModel> UpdateAssetCondition(AssetMaster asset)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //Base response = new Base();
        //        var assetData = _db.AssetMaster.Where(x => x.AssetId == asset.AssetId && x.IsDeleted == false).FirstOrDefault();

        //        //var assetData = (from ad in db.AssetMaster
        //        //                 join bd in db.Condition on ad.ConditionName equals bd.ConditionName
        //        //                 select new
        //        //                 {
        //        //                     bd.ConditionName,
        //        //                     ad.ConditionId
        //        //                 }).ToList();
        //        if (assetData != null)
        //        {
        //            assetData.ConditionId = asset.ConditionId;
        //            assetData.ConditionName = _db.Condition.Where(a => a.ConditionId == assetData.ConditionId).Select(x => x.ConditionName).FirstOrDefault();
        //            if (assetData.ConditionName == "Good") { assetData.Status = "AVAILABLE"; } else { assetData.Status = "NOT AVAILABLE"; }
        //            assetData.UpdatedBy = claims.employeeid;
        //            assetData.UpdatedOn = DateTime.Now;
        //            assetData.IsActive = true;
        //            assetData.IsDeleted = false;
        //            _db.Entry(assetData).State = System.Data.Entity.EntityState.Modified;
        //            await _db.SaveChangesAsync();
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

        //#endregion Update Asset Condition

        #region Update Asset Details (Assets recover)

        /// <summary>
        /// API >> Put >>  api/assets/updateassetdetails
        /// Created by shriya
        /// </summary>
        /// <param name = "asset" ></ param >
        /// < returns ></ returns >
        [Route("UpdateAssetDetails")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateAssetDetails(AssignMasterData asset)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {    //Base response = new Base();
                var assetAssD = _db.AssignMasterData.Where(x => x.ItemId == asset.ItemId && x.AssetsKey == asset.AssetsKey && x.IsDeleted == false).FirstOrDefault();
                if (assetAssD != null)
                {
                    assetAssD.AssetRecoveredById = asset.AssetRecoveredById;
                    assetAssD.AssetRecoveredBy = _db.Employee.Where(a => a.EmployeeId == assetAssD.AssetRecoveredById).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
                    assetAssD.Condition = asset.Condition;
                    assetAssD.CompanyId = claims.companyId;
                    assetAssD.OrgId = claims.orgId;
                    assetAssD.DeletedBy = claims.employeeId;
                    assetAssD.DeletedOn = DateTime.Now;
                    assetAssD.IsActive = false;
                    assetAssD.IsDeleted = true;
                    _db.Entry(assetAssD).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
                    await _db.SaveChangesAsync();
                    response.Status = true;
                    response.Message = "Assign assets recover succesfully";
                    response.Data = assetAssD;

                    if (asset.Condition == "Good")
                    {
                        var data = _db.ItemMaster.Where(x => x.ItemId == asset.ItemId && x.WarehouseId == asset.WarehouseId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
                        if (data != null)
                        {
                            data.ItemId = asset.ItemId;
                            var exitingUnit = data.Units;
                            data.Units = exitingUnit + 1;
                            data.CompanyId = claims.companyId;
                            data.OrgId = claims.orgId;
                            data.UpdatedBy = claims.employeeId;
                            data.UpdatedOn = DateTime.Now;
                            data.IsActive = true;
                            data.IsDeleted = false;
                            _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        FaultyAssetsData faultyAss = new FaultyAssetsData();
                        faultyAss.ItemId = asset.ItemId;
                        faultyAss.Assetskey = asset.AssetsKey;
                        faultyAss.Condition = asset.Condition;
                        faultyAss.Comments = asset.Comment;
                        faultyAss.AssetRecoveredById = asset.AssetRecoveredById;
                        faultyAss.AssetRecoveredBy = _db.Employee.Where(a => a.EmployeeId == faultyAss.AssetRecoveredById).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
                        faultyAss.WarehouseId = asset.WarehouseId;
                        faultyAss.CreatedBy = claims.employeeId;
                        faultyAss.CreatedOn = DateTime.Now;
                        faultyAss.IsActive = true;
                        faultyAss.IsDeleted = false;
                        faultyAss.CompanyId = claims.companyId;
                        faultyAss.OrgId = claims.orgId;
                        _db.FaultyAssetsData.Add(faultyAss); // add data in faulty assests ..
                        _db.SaveChanges();
                    }
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
                response.Data = null;
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        #endregion Update Asset Details (Assets recover)

        //#region DeleteAsset

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="AssetId"></param>
        ///// <returns></returns>
        ////Delete Asset Api//
        //[Route("DeleteAsset")]
        //[HttpDelete]
        //public async Task<ResponseBodyModel> DeleteAsset(int AssetId)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        //Base response = new Base();
        //        var AssetDelete = _db.AssetMaster.Where(x => x.AssetId == AssetId).FirstOrDefault();
        //        if (AssetDelete != null)
        //        {
        //            AssetDelete.IsDeleted = true;
        //            AssetDelete.IsActive = false;
        //            _db.Entry(AssetDelete).State = System.Data.Entity.EntityState.Modified;
        //            await _db.SaveChangesAsync();
        //            response.Status = true;
        //            response.Message = "Record Delete Successfully";
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            response.Message = "Please select the record";
        //        }
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Data = null;
        //        response.Message = ex.Message;
        //        response.Status = false;
        //        return response;
        //    }
        //}

        //#endregion DeleteAsset

        //#region GetAssestsByAssetId

        ///// <summary>
        ///// Api >> Get >> api/ticketmaster/getempbytcid
        ///// </summary>
        ///// <param name="AssetId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getempbyAssetid")]
        //public async Task<ResponseBodyModel> getempbyAssetid(int AssetId)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var Asset = await _db.AssetMaster.Where(x => x.IsDeleted == false && x.AssetId == AssetId).ToListAsync();
        //        if (Asset != null)

        //        {
        //            res.Message = "Asset Found";
        //            res.Status = true;
        //            res.Data = Asset;
        //        }
        //        else
        //        {
        //            res.Message = "Asset Not Found";
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

        //#endregion GetAssestsByAssetId

        //#region getAssetid

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getAssetid")]
        //public async Task<ResponseBodyModel> getAssetid(int Id)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        List<AssetData> assetDataList = new List<AssetData>();

        //        var finalData = await (from ad in _db.AssetMaster
        //                               join ed in _db.Employee on ad.AssignedToId equals ed.EmployeeId
        //                               where ad.AssetId == Id || ad.AssignedToId == Id && ad.IsDeleted == false
        //                               select new
        //                               {
        //                                   ad.AssetId,
        //                                   ad.AssetName,
        //                                   ed.EmployeeId,
        //                                   FullName = ed.FirstName + " " + ed.LastName,
        //                                   ad.AssignedToId,
        //                                   ad.AssignedTo,
        //                                   ad.ConditionId,
        //                                   ad.ConditionName,
        //                                   ed.PersonalEmail,
        //                                   ed.MobilePhone,
        //                               }).ToListAsync();

        //        foreach (var item in finalData)
        //        {
        //            AssetData data = new AssetData();
        //            data.AssetId = item.AssetId;
        //            data.AssetName = item.AssetName;
        //            data.AssignedTo = item.AssignedTo;
        //            data.Email = item.PersonalEmail;
        //            data.ConditionName = item.ConditionName;
        //            data.ConditionId = item.ConditionId;
        //            data.FullName = item.FullName;
        //            // data.MobilePhone = item.MobilePhone;
        //            assetDataList.Add(data);
        //        }

        //        if (assetDataList.Count != 0)
        //        {
        //            res.Message = "Data found ";
        //            res.Status = true;
        //            res.Data = finalData;
        //        }
        //        else
        //        {
        //            res.Message = "Data Not Found";
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

        //#endregion getAssetid

        //#region Mark AS Not Available

        ///// <summary>
        ///// API >> Put >> api/assets/markasnotavailable
        ///// Created by shriya
        ///// </summary>
        //[Route("MarkASNotAvailable")]
        //[HttpPut] //hold for some time
        //public async Task<ResponseBodyModel> MarkASNotAvailable(AssetMaster asset)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        var assetData = _db.AssetMaster.Where(x => x.AssetId == asset.AssetId && x.IsDeleted == false).FirstOrDefault();
        //        if (assetData != null)
        //        {
        //            assetData.ConditionId = asset.ConditionId;
        //            assetData.ConditionName = _db.Condition.Where(a => a.ConditionId == assetData.ConditionId).Select(x => x.ConditionName).FirstOrDefault();
        //            assetData.NotAvailableReasonId = asset.NotAvailableReasonId;
        //            assetData.Status = "NOT AVAILABLE";
        //            assetData.UpdatedBy = claims.employeeid;
        //            assetData.UpdatedOn = DateTime.Now;
        //            assetData.IsActive = true;
        //            assetData.IsDeleted = false;
        //            _db.Entry(assetData).State = System.Data.Entity.EntityState.Modified;
        //            await _db.SaveChangesAsync();
        //            response.Status = true;
        //            response.Message = "Details Updated Successfully!";
        //            response.Data = assetData;
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

        //#endregion Mark AS Not Available

        //#region GetAlltheAssignAssets

        ///// <summary>
        /////  API >> Get >> api/assets/getalltheassignassets
        ///// Created  by Shriya
        ///// Created on 11/04/22
        ///// </summary>
        //[Route("GetAlltheAssignAssets")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetAlltheAssignAssets()
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        List<AssignAssetsData> AssignAssetDataList = new List<AssignAssetsData>();
        //        //var assignAssets = db.AssignMasterData.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
        //        var assignAssets = await (from aa in _db.AssignMasterData
        //                                  join am in _db.ItemMaster on aa.ItemId equals am.ItemId
        //                                  where aa.IsDeleted == false && aa.IsActive == true
        //                                  select new
        //                                  {
        //                                      am.ItemId,
        //                                      am.ItemName,
        //                                      aa.AssignedToId,
        //                                      aa.AssignedTo,
        //                                      aa.Condition,
        //                                      aa.AssetsKey,
        //                                      aa.WarehouseId,
        //                                      aa.Comment
        //                                  }).ToListAsync();
        //        foreach (var assignItem in assignAssets)
        //        {
        //            AssignAssetsData assets = new AssignAssetsData();
        //            assets.ItemId = assignItem.ItemId;
        //            assets.ItemName = _db.ItemMaster.Where(x => x.ItemId == assets.ItemId).Select(x => x.ItemName).FirstOrDefault();
        //            assets.AssignedToId = assignItem.AssignedToId;
        //            assets.AssignedTo = assignItem.AssignedTo;
        //            assets.Condition = assignItem.Condition;
        //            assets.AssetsKey = assignItem.AssetsKey;
        //            assets.WarehouseId = assignItem.WarehouseId;
        //            assets.Comment = assignItem.Comment;
        //            AssignAssetDataList.Add(assets);
        //        }
        //        if (AssignAssetDataList.Count > 0)
        //        {
        //            response.Status = true;
        //            response.Message = "assets assign data list";
        //            response.Data = AssignAssetDataList;
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            response.Message = "data not found";
        //            response.Data = AssignAssetDataList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //        response.Status = false;
        //        return response;
        //    }
        //    return response;
        //}

        //#endregion GetAlltheAssignAssets

        #region GetAssignAssetByAssignToId

        /// <summary>
        ///  API >> Get >> api/assets/getdatabyassigntoid
        /// Created by Shriya
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetDataByAssignToId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetDataByAssignToId(int id)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                List<AssignAssetsData> AssignAssetDataList = new List<AssignAssetsData>();
                var AssignData = await _db.AssignMasterData.Where(x => x.AssignedToId == id && x.IsDeleted == false && x.IsActive == true).ToListAsync();
                foreach (var assignItem in AssignData)
                {
                    AssignAssetsData assets = new AssignAssetsData();
                    assets.ItemId = assignItem.ItemId;
                    assets.ItemName = _db.ItemMaster.Where(x => x.ItemId == assets.ItemId).Select(x => x.ItemName).FirstOrDefault();
                    assets.AssignedToId = assignItem.AssignedToId;
                    assets.AssignedTo = assignItem.AssignedTo;
                    assets.Condition = assignItem.Condition;
                    assets.AssetsKey = assignItem.AssetsKey;
                    assets.WarehouseId = assignItem.WarehouseId;
                    assets.Comment = assignItem.Comment;
                    AssignAssetDataList.Add(assets);
                }
                if (AssignAssetDataList.Count > 0)
                {
                    response.Status = true;
                    response.Message = "get all assets by assigntoId";
                    response.Data = AssignAssetDataList;
                }
                else
                {
                    response.Status = false;
                    response.Message = "data is not found";
                    response.Data = AssignAssetDataList;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        #endregion GetAssignAssetByAssignToId

        #region Assetmaster Get Form Item

        /// <summary>
        ///
        /// </summary>
        [Route("GetaAssetmasterFormItem")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetaAssetmasterFormItem()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var FinalData = await (from R in _db.ItemMaster
                                       join f in _db.ItemMasterWarehouses on R.WarehouseId equals f.WarehouseId
                                       //   where R.IsAsset != false && R.IsDeleted == false
                                       where R.IsDeleted == false // Change by Suraj Bundel on 01/07/2022
                                       select new
                                       {
                                           ItemId = R.ItemId,
                                           ItemName = R.ItemName,
                                           BaseCategoryId = R.BaseCategoryId,
                                           BaseCategoryName = R.BaseCategoryName,
                                           Categoryid = R.Categoryid,
                                           CategoryName = R.CategoryName,
                                           SubCategoryId = R.SubCategoryId,
                                           SubcategoryName = R.SubcategoryName,
                                           WarehouseId = R.WarehouseId,
                                           WarehouseName = R.WarehouseName,
                                           Condition = R.Condition,
                                           Itemcode = R.Itemcode,
                                           Units = R.Units,
                                           IsAsset = R.IsAsset,
                                           UniqueCode = R.UniqueCode,
                                           SerialNumber = R.SerialNumber,
                                           WarehouseAddress = f.WarehouseAddress
                                       }).ToListAsync();
                if (FinalData != null)
                {
                    res.Status = true;
                    res.Message = "Assetmaster Get Form Item data list Found";
                    res.Data = FinalData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "data not found";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
            return res;
        }

        #endregion Assetmaster Get Form Item

        //#region This is used for asstes information recovered

        ///// <summary>
        ///// API >> Get >> api/assets/getinforecoverable
        ///// Created by shriya Create on 26-05-2022
        ///// </summary>
        ///// <returns></returns>
        //[Route("getinforecoverable")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetInfoRecoverable()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var FinalData = await (from R in _db.ItemMaster
        //                               join C in _db.ItemMasterCategory on R.Categoryid equals C.Categoryid
        //                               join SC in _db.itemMasterSubCategory on R.SubCategoryId equals SC.SubCategoryId
        //                               join f in _db.AssignMasterData on R.ItemId equals f.ItemId
        //                               where f.IsActive == false && f.IsDeleted == true
        //                               select new
        //                               {
        //                                   asstesName = f.ItemName,
        //                                   categoryId = R.Categoryid,
        //                                   categoryName = C.CategoryName,
        //                                   subcategoryId = R.SubCategoryId,
        //                                   subCategory = SC.SubcategoryName,
        //                                   condition = f.Condition,
        //                                   employeeName = f.AssignedTo,
        //                                   recoverBy = f.AssetRecoveredBy,
        //                                   recoverDate = f.DeletedOn
        //                               }).ToListAsync();
        //        if (FinalData != null)
        //        {
        //            res.Status = true;
        //            res.Message = "Assets list";
        //            res.Data = FinalData;
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "list is not found because asset not recover";
        //            res.Data = FinalData;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }

        //    return res;
        //}

        //#endregion This is used for asstes information recovered

        #region GetAllFaultyAssets

        /// <summary>
        /// API >> Get >> api/assets/GetAllFaultyAssetsByWHId
        /// created by Shriya
        /// created on 15-04-2022
        ///get all fulty assets by warehouse id
        /// </summary>
        [Route("GetAllFaultyAssetsByWHId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllFaultyAssetsByWHId(int Id)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                List<FaultyAssetsDataModel> faultyAssetsDatalist = new List<FaultyAssetsDataModel>();
                var faultyAss = await _db.FaultyAssetsData.Where(x => x.WarehouseId == Id && x.IsDeleted == false && x.IsActive == true).ToListAsync();
                foreach (var faultyItem in faultyAss)
                {
                    FaultyAssetsDataModel fultyAssets = new FaultyAssetsDataModel();
                    fultyAssets.FaultyAssetId = faultyItem.FaultyAssetId;
                    fultyAssets.ItemId = faultyItem.ItemId;
                    fultyAssets.ItemName = _db.ItemMaster.Where(x => x.ItemId == fultyAssets.ItemId && x.WarehouseId == Id).Select(x => x.ItemName).FirstOrDefault();
                    fultyAssets.Assetskey = faultyItem.Assetskey;
                    fultyAssets.AssetRecoveredById = faultyItem.AssetRecoveredById;
                    fultyAssets.AssetRecoveredBy = faultyItem.AssetRecoveredBy;
                    fultyAssets.Comments = faultyItem.Comments;
                    fultyAssets.WarehouseId = faultyItem.WarehouseId;
                    fultyAssets.Condition = faultyItem.Condition;
                    faultyAssetsDatalist.Add(fultyAssets);
                }
                if (faultyAssetsDatalist.Count > 0)
                {
                    response.Status = true;
                    response.Message = "Faulty assets list found";
                    response.Data = faultyAssetsDatalist;
                }
                else
                {
                    response.Status = false;
                    response.Message = "Faulty assets list not found";
                    response.Data = faultyAssetsDatalist;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
                return response;
            }
            return response;
        }

        #endregion GetAllFaultyAssets










        #region This Api Is Used For Get All Api For Use In Assets Deashboard

        /// <summary>
        /// Created By Ankit Jain Date-27/05/2022
        /// Api >> Get >> api/assets/getallassetsdashboard
        /// </summary>
        /// <returns></returns>
        [Route("getallassetsdashboard")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllAssetsForDashboard()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            DashboardAsset response = new DashboardAsset();
            List<AssetBarModel> AssetBarChart = new List<AssetBarModel>();
            List<AssetPichart> AssetPiChart = new List<AssetPichart>();
            try
            {
                #region This Api Use Get All Assets

                var TotalAsstet = await _db.ItemMaster.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsAsset == true).ToListAsync();
                var Assets = 0;
                foreach (var item in TotalAsstet)
                {
                    Assets = Assets + item.Units;
                }
                response.GetAllAsset = Assets;

                #endregion This Api Use Get All Assets

                #region This Api Use Get All Assign Assets

                var AssignAsstet = _db.AssignMasterData.Where(x => x.IsDeleted == false && x.IsActive == true).Count();
                response.GetAllAssignAsset = AssignAsstet;

                #endregion This Api Use Get All Assign Assets

                #region This Api Use Get All Assets Not Available

                var FaultyAsstet = _db.FaultyAssetsData.Where(x => x.IsDeleted == false && x.IsActive == true).Count();
                var notAvilable = _db.AssignMasterData.Where(x => x.AvailablityStatus == false).Count();
                response.GetAllFaultyAssets = notAvilable;

                #endregion This Api Use Get All Assets Not Available

                #region This Api Use Get All Assets Available

                var AvailableAssets = Assets - (AssignAsstet + FaultyAsstet);
                response.AvailableAssets = AvailableAssets;

                #endregion This Api Use Get All Assets Available

                #region This Api Use For Bar Chart In Asset

                var Total = _db.ItemMaster.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToList().Distinct();

                var FaultyValue = _db.FaultyAssetsData.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
                foreach (var item in Total)
                {
                    List<Condition> ListCon = new List<Condition>();
                    AssetBarModel obj = new AssetBarModel
                    {
                        Name = Total.Where(x => x.IsActive == true && x.IsDeleted == false && x.ItemId == item.ItemId).Select(x => x.ItemName).FirstOrDefault(),
                        Series = new List<Condition>(),
                    };
                    Condition con1 = new Condition
                    {
                        Name = "Good",
                        Value = Total.Where(x => x.Condition == "Good" && x.ItemId == item.ItemId).Select(x => x.Units).FirstOrDefault(),
                    };
                    ListCon.Add(con1);
                    Condition con2 = new Condition
                    {
                        Name = "Damage",
                        Value = FaultyValue.Where(x => x.Condition == "Damage" && x.ItemId == item.ItemId).Select(x => x.Condition).ToList().Count,
                    };
                    ListCon.Add(con2);
                    obj.Series = ListCon;
                    AssetBarChart.Add(obj);
                }
                response.ChartAssets = AssetBarChart;

                #endregion This Api Use For Bar Chart In Asset

                #region This Api For Pi Chart in asset

                foreach (var item in Total)
                {
                    AssetPichart Pi = new AssetPichart
                    {
                        Name = Total.Where(x => x.IsActive == true && x.IsDeleted == false && x.ItemName == item.ItemName).Select(x => x.ItemName).FirstOrDefault(),
                        Value = Total.Where(x => x.ItemName == item.ItemName).Select(x => x.Units).FirstOrDefault(),
                    };
                    AssetPiChart.Add(Pi);
                }
                response.Series = AssetPiChart;

                #endregion This Api For Pi Chart in asset

                res.Message = "All Assets Available";
                res.Status = true;
                res.Data = response;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                return res;
            }
            return res;
        }

        //public class HelperForBarChart
        //{
        //    public int Item { get; set; }
        //}

        #endregion This Api Is Used For Get All Api For Use In Assets Deashboard

        #region Update Faulty Asset

        /// <summary>
        ///  API >> Put >> api/assets/UpdateFaultyAsset
        ///  Created by shriya
        ///  Crated on 15-04-2022
        ///  get fulty assets repair  then update unit of item
        /// </summary>
        [Route("UpdateFaultyAsset")]
        [HttpPut]
        public async Task<ResponseBodyModel> UpdateFaultyAsset(FaultyAssetsDataModel assets)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var faultyAsssetf = await _db.FaultyAssetsData.Where(x => x.FaultyAssetId == assets.FaultyAssetId && x.ItemId == assets.ItemId && x.WarehouseId == assets.WarehouseId && x.IsDeleted == false && x.IsActive == true).FirstOrDefaultAsync();
                if (faultyAsssetf != null)
                {
                    faultyAsssetf.Condition = assets.Condition;
                    faultyAsssetf.IsActive = false;
                    faultyAsssetf.IsDeleted = true;
                    faultyAsssetf.UpdatedBy = claims.userId;
                    faultyAsssetf.UpdatedOn = DateTime.Now;
                    _db.Entry(faultyAsssetf).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    var data = _db.ItemMaster.Where(x => x.ItemId == assets.ItemId && x.WarehouseId == assets.WarehouseId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
                    if (data != null)
                    {
                        data.ItemId = assets.ItemId;
                        var exitingUnit = data.Units;
                        data.Units = exitingUnit + 1;
                        data.CompanyId = claims.companyId;
                        data.OrgId = claims.orgId;
                        data.UpdatedBy = claims.userId;
                        data.UpdatedOn = DateTime.Now;
                        data.IsActive = true;
                        data.IsDeleted = false;
                        _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    response.Status = true;
                    response.Message = "faulty assets condition updated";
                    response.Data = faultyAsssetf;
                }
                else
                {
                    response.Status = false;
                    response.Message = "faulty assets condition not update";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        #endregion Update Faulty Asset

        //#region This Api Is USe For Add Asset In import

        ///// <summary>
        ///// Created By ankit Date- 21/06/2022
        ///// Api >> Post >> api/assets/addassetimport
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("addassetimport")]
        //public async Task<ResponseBodyModel> Addassetimport(List<AssetDataImport> item)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        if (item == null)
        //        {
        //            res.Message = "Error";
        //            res.Status = false;
        //            return res;
        //        }
        //        if (item.Count > 0)
        //        {
        //            foreach (var model in item)
        //            {
        //                AssetMaster data = new AssetMaster();
        //                data.AssetId = model.AssetId;
        //                data.ItemId = model.ItemId;
        //                data.AssetName = model.AssetName;
        //                data.Location = model.Location;
        //                data.ConditionName = model.ConditionName;
        //                data.Status = model.Status;
        //                data.AssignedToId = model.AssignedToId;
        //                data.AssignedTo = model.AssignedTo;
        //                data.CategoryId = model.CategoryId;
        //                data.CategoryName = _db.Category.Where(x => x.CategoryTypeId == model.CategoryId).Select(x => x.CategoryName).FirstOrDefault();
        //                data.SubCategoryId = _db.SubCategorysHindmt.Where(x => x.Categoryid == model.CategoryId).Select(x => x.SubCategoryId).FirstOrDefault();
        //                data.SubCategoryName = _db.SubCategorysHindmt.Where(s => s.Categoryid == model.CategoryId).Select(x => x.SubcategoryName).FirstOrDefault();
        //                data.ConditionId = model.ConditionId;
        //                data.ConditionName = model.ConditionName;
        //                data.BaseCategoryId = model.BaseCategoryId;
        //                data.BaseCategoryName = _db.BaseCategoryDb.Where(b => b.BaseCategoryId == model.BaseCategoryId).Select(b => b.BaseCategoryName).FirstOrDefault();
        //                data.SubSubCategoryId = _db.SubsubCategorys.Where(c => c.Categoryid == model.CategoryId).Select(x => x.SubsubCategoryid).FirstOrDefault();
        //                data.SubSubCategoryName = _db.SubsubCategorys.Where(s => s.Categoryid == model.CategoryId).Select(x => x.SubcategoryName).FirstOrDefault();
        //                data.NotAvailableReasonId = model.NotAvailableReasonId;
        //                data.Status = model.Status;
        //                _db.AssetMaster.Add(data);
        //                await _db.SaveChangesAsync();
        //                res.Message = "Asset Data Add Successfully";
        //                res.Status = true;
        //                res.Data = data;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion This Api Is USe For Add Asset In import

        #region mark as not available

        /// <summary>
        /// created by Suraj Bundel Created on 06/07/2022
        /// </summary>
        /// <param name="availablity"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateavailablitystatus")]
        public async Task<ResponseBodyModel> Updateavailablitystatus(AvailablityStatusmodel availablity)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var condition = await _db.AssignMasterData.Where(x => x.AssignId == availablity.AssignId && x.AvailablityStatus == true).FirstOrDefaultAsync();
                if (condition != null)
                {
                    //AvailablityStatusmodel Statusmodel = new AvailablityStatusmodel();
                    condition.AvailablityStatus = false;
                    condition.ReasonfornotAvailable = availablity.ReasonfornotAvailable;
                    condition.Note = availablity.Note;
                    condition.IsActive = false;
                    condition.IsDeleted = true;
                    condition.DeletedOn = DateTime.Now;
                    condition.DeletedBy = claims.employeeId;
                    _db.Entry(condition).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Availablity condition changed";
                    res.Status = true;
                    res.Data = null;
                }
                else
                {
                    res.Message = "No data found";
                    res.Status = false;
                    res.Data = null;
                }
            }
            catch (Exception)
            {
                res.Message = "unable to change availablity condition";
                res.Status = false;
                res.Data = null;
            }
            return res;
        }

        #endregion mark as not available

        #region update condition of assign assets to  assignee

        /// <summary>
        /// Created by Shriya Malvi On 06-07-2022
        /// API >> Put >>  api/assets/assignassetconup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("assignassetconups")]
        public async Task<ResponseBodyModel> UpdateConditionOfAssignAssets(AssignMasterData model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var assetAssD = _db.AssignMasterData.Where(x => x.AssignId == model.AssignId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                if (assetAssD != null)
                {
                    if (model.Condition == "Damage")
                    {
                        assetAssD.AssetRecoveredById = claims.employeeId;
                        assetAssD.AssetRecoveredBy = _db.Employee.Where(a => a.EmployeeId == assetAssD.AssetRecoveredById).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
                        assetAssD.Condition = model.Condition;
                        assetAssD.DeletedBy = claims.employeeId;
                        assetAssD.DeletedOn = DateTime.Now;
                        assetAssD.IsActive = false;
                        assetAssD.IsDeleted = true;
                        _db.Entry(assetAssD).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
                        await _db.SaveChangesAsync();

                        FaultyAssetsData faultyAss = new FaultyAssetsData();
                        faultyAss.ItemId = model.ItemId;
                        faultyAss.Assetskey = model.AssetsKey;
                        faultyAss.Condition = model.Condition;
                        faultyAss.Comments = model.Comment;
                        faultyAss.AssetRecoveredById = claims.employeeId;
                        faultyAss.AssetRecoveredBy = _db.Employee.Where(a => a.EmployeeId == faultyAss.AssetRecoveredById).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
                        faultyAss.WarehouseId = model.WarehouseId;
                        faultyAss.CreatedBy = claims.employeeId;
                        faultyAss.CreatedOn = DateTime.Now;
                        faultyAss.IsActive = true;
                        faultyAss.IsDeleted = false;
                        faultyAss.CompanyId = claims.companyId;
                        faultyAss.OrgId = claims.orgId;
                        _db.FaultyAssetsData.Add(faultyAss); // add data in faulty assests ..
                        _db.SaveChanges();

                        res.Data = assetAssD;
                        res.Message = "update assets condition and recover assets";
                        res.Status = true;
                    }
                    else
                    {
                        assetAssD.Condition = model.Condition;
                        assetAssD.UpdatedBy = claims.employeeId;
                        assetAssD.UpdatedOn = DateTime.Now;

                        _db.Entry(assetAssD).State = System.Data.Entity.EntityState.Modified; //data upadte in tabel
                        await _db.SaveChangesAsync();

                        res.Data = assetAssD;
                        res.Message = "Assets condition updated";
                        res.Status = true;
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

        #region helper class

        public class AvailablityStatusmodel
        {
            public long AssignId { get; set; }
            public bool AvailablityStatus { get; set; }
            public string ReasonfornotAvailable { get; set; }
            public string Note { get; set; }
        }

        public class ConUpAssAssets
        {
            public int AssignId { get; set; }
            public string Condition { get; set; }
            public string Note { get; set; }
        }

        public class DashboardAsset
        {
            public int GetAllAsset { get; set; }
            public int GetAllAssignAsset { get; set; }
            public int GetAllFaultyAssets { get; set; }
            public int AvailableAssets { get; set; }
            public List<AssetBarModel> ChartAssets { get; set; }
            public List<AssetPichart> Series { get; set; }
        }

        public class AssetBarModel
        {
            public string Name { get; set; }
            public List<Condition> Series { get; set; }
        }

        public class Condition
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class AssetPichart
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class AssetData
        {
            public long Id { get; set; }
            public long AssetId { get; set; }
            public string AssetName { get; set; }
            public int BaseCategoryId { get; set; }
            public string BaseCategoryName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int SubCategoryId { get; set; }
            public string SubCategoryName { get; set; }
            public int? SubsubCategoryid { get; set; }
            public string SubsubCategoryName { get; set; }
            public string FullName { get; set; }
            public int AssignedToId { get; set; }
            public string AssignedTo { get; set; }
            public int MobilePhone { get; set; }

            public string Location { get; set; }
            public int ConditionId { get; set; }
            public string ConditionName { get; set; }
            public string Status { get; set; }
            public int NotAvailableReasonId { get; set; }
            public string ReasonName { get; set; }
            public string Email { get; set; }
        }

        public class AssetDataImport
        {
            public long ItemId { get; set; }
            public long AssetId { get; set; }
            public string AssetName { get; set; }
            public int BaseCategoryId { get; set; }
            public int CategoryId { get; set; }
            public int AssignedToId { get; set; }
            public string AssignedTo { get; set; }
            public string Location { get; set; }
            public int ConditionId { get; set; }
            public string ConditionName { get; set; }
            public string Status { get; set; }
            public int NotAvailableReasonId { get; set; }
        }

        public class ReasonData
        {
            public long Id { get; set; }
            public long ReasonId { get; set; }
            public string ReasonName { get; set; }
        }

        public class ReasonsDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<ReasonData> MyReasonsList { get; set; }
        }

        public class ConditionData
        {
            public long Id { get; set; }
            public long ConditionId { get; set; }
            public string ConditionName { get; set; }
        }

        public class ConditionDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<ConditionData> MyConditionsList { get; set; }
        }

        public class SubData
        {
            public string SubCategoryName { get; set; }
            public long SubCategoryId { get; set; }
        }

        public class AssignAssetsData
        {
            public long ItemId { get; set; }
            public string ItemName { get; set; }
            public int WarehouseId { get; set; }
            public string Condition { get; set; }
            public int AssignedToId { get; set; }
            public string AssignedTo { get; set; }
            public string AssetsKey { get; set; }
            public int? AssetRecoveredById { get; set; }
            public string AssetRecoveredBy { get; set; }
            public string Comment { get; set; }
            public DateTime CreateOn { get; set; }
        }

        public class FaultyAssetsDataModel
        {
            public int FaultyAssetId { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int WarehouseId { get; set; }
            public string Assetskey { get; set; }
            public string Condition { get; set; }
            public string Comments { get; set; }
            public int? AssetRecoveredById { get; set; }
            public string AssetRecoveredBy { get; set; }
        }

        #endregion helper class
    }
}