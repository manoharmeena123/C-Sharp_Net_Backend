using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.EmployeeModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Employees
{
    [Authorize]
    [RoutePrefix("api/employeeexits")]
    public class EmployeeExitsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api For Exit Employeee

        /// <summary>
        /// Create by Ravi Vyas on 09-08-2022
        /// API >> Get >>api/employeeexits/exitemployee
        /// </summary>
        /// <param ></param>
        [HttpPost]
        [Route("exitemployee")]
        public async Task<ResponseBodyModel> EmployeeExit(EmployeeExits model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Empty !";
                    res.Status = false;
                }
                else
                {
                    var empName = await _db.Employee.Where(x => x.EmployeeId == claims.employeeId && x.IsActive &&
                            !x.IsDeleted && x.CompanyId == claims.companyId).Select(x => x.DisplayName).FirstOrDefaultAsync();
                    if (model.IsDiscussion)
                    {
                        EmployeeExits obj = new EmployeeExits()
                        {
                            DiscussionSummary = model.DiscussionSummary,
                            ReasonId = model.ReasonId,
                            Reason = Enum.GetName(typeof(ExitEmpReasonConstants), model.ReasonId),
                            EmployeeId = claims.employeeId,
                            EmployeeName = empName,
                            IsDiscussion = model.IsDiscussion,
                            ExitType = ExitInitingType.Employee_Want_To_Resign,
                            CompanyId = claims.companyId,
                            TerminateDate = DateTime.Now,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            Status = ExitStatusConstants.Pending,
                            IsActive = true,
                            IsDeleted = false,
                            InProgress = true,
                        };
                        _db.EmployeeExits.Add(obj);
                        _db.SaveChanges();
                        res.Message = "Employee Exits Succesfully !";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        EmployeeExits obj = new EmployeeExits()
                        {
                            Comment = model.Comment,
                            ReasonId = model.ReasonId,
                            Reason = Enum.GetName(typeof(ExitEmpReasonConstants), model.ReasonId),
                            EmployeeId = claims.employeeId,
                            EmployeeName = empName,
                            CompanyId = claims.companyId,
                            ExitType = ExitInitingType.Employee_Want_To_Resign,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                            TerminateDate = DateTime.Now,
                            CreatedOn = DateTime.Now,
                            Status = ExitStatusConstants.Pending,
                            IsActive = true,
                            IsDeleted = false,
                            InProgress = true,
                        };
                        _db.EmployeeExits.Add(obj);
                        _db.SaveChanges();
                        res.Message = "Employee Exits Succesfully !";
                        res.Status = true;
                        res.Data = obj;
                    }
                    EmployeeExitsHistory histobj = new EmployeeExitsHistory()
                    {
                        DiscussionSummary = model.DiscussionSummary,
                        ReasonId = model.ReasonId,
                        Reason = Enum.GetName(typeof(ExitEmpReasonConstants), model.ReasonId),
                        EmployeeId = claims.employeeId,
                        EmployeeName = empName,
                        IsDiscussion = model.IsDiscussion,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        Status = ExitStatusConstants.Pending,
                        IsActive = true,
                        IsDeleted = false,
                        Comment = model.Comment,
                        TerminateDate = DateTime.Now,
                        InProgress = true,
                    };
                    _db.EmployeeExitsHistorys.Add(histobj);
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

        #endregion Api For Exit Employeee

        #region Api For get exit data by id

        /// <summary>
        ///  Created by Suraj Bundel on 19/07/22
        ///  API >> POST >> api/employeeexits/getexitemployeebyid
        /// </summary>
        [HttpGet]
        [Route("getexitemployeebyid")]
        public async Task<ResponseBodyModel> GetExitEmployeebyId(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getdata = await (from ee in _db.EmployeeExits
                                     where !ee.IsDeleted && ee.IsActive && ee.CompanyId == claims.companyId && ee.EmployeeExitId == id
                                     select new
                                     {
                                         ee.EmployeeExitId,
                                         ee.EmployeeId,
                                         ee.EmployeeName,
                                         ee.IsDiscussion,
                                         ee.DiscussionSummary,
                                         ee.Reason,
                                         ee.Comment,
                                         ee.TerminateDate,
                                         ee.LastWorkingDate,
                                         ee.InProgress,
                                     }).FirstOrDefaultAsync();
                if (claims.roleType == "HR")
                {
                    if (getdata != null)
                    {
                        res.Message = "Success";
                        res.Status = true;
                        res.Data = getdata;
                    }
                    else
                    {
                        res.Message = "No list found";
                        res.Status = false;
                        res.Data = getdata;
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

        #endregion Api For get exit data by id

        #region Get Reason for resignation // dropdown

        /// <summary>
        /// Created By Shriya Malvi on 09-08-2022
        /// API >> Get >>api/employeeexits/exitempreason
        /// Dropdown using Enum for Exit employee Reason
        /// </summary>
        [Route("exitempreason")]
        [HttpGet]
        public ResponseBodyModel ExitEmpReason()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var assetsItemType = Enum.GetValues(typeof(ExitEmpReasonConstants))
                    .Cast<ExitEmpReasonConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(ExitEmpReasonConstants), x).Replace("_", " ")
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

        #region Exit Employee Type

        /// <summary>
        /// Created By Shriya Malvi on 09-08-2022
        /// API >> Get >> api/employeeexits/exitemptype
        /// Dropdown using Enum for Exit employee Reason
        /// </summary>
        /// <returns></returns>

        [Route("exitemptype")]
        [HttpGet]
        public ResponseBodyModel ExitEmpType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var identity = User.Identity as ClaimsIdentity;

            try
            {
                var assetsItemType = Enum.GetValues(typeof(ExitInitingType))
                    .Cast<ExitInitingType>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(ExitInitingType), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Resignation Type List";
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

        #endregion Exit Employee Type

        #region Exit Employee Process By HR

        /// <summary>
        /// Created By Ravi Vyas On 09-08-2022
        /// API >>  Post >>api/employeeexits/ExitEmpProcessByHR
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("exitempprocessbyhr")]
        public async Task<ResponseBodyModel> ExitEmpProcessByHR(EmployeeExits model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    EmployeeExits ExitEmp = new EmployeeExits
                    {
                        EmployeeId = model.EmployeeId,
                        EmployeeName = _db.GetEmployeeNameById(model.EmployeeId),
                        IsDiscussion = model.IsDiscussion,
                        DiscussionSummary = model.DiscussionSummary,
                        ReasonId = model.ReasonId,
                        Reason = Enum.GetName(typeof(ExitEmpReasonConstants), model.ReasonId),
                        ExitType = model.ExitType,
                        Status = ExitStatusConstants.Approved,
                        Comment = model.Comment,
                        TerminateDate = model.TerminateDate,
                        LastWorkingDate = model.LastWorkingDate,
                        IsReHired = model.IsReHired,
                        ExitFile = model.ExitFile,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        HRApproved = true,
                        InProgress = true,
                        HRApprovebyId = claims.employeeId,
                        TerminationDocument = model.TerminationDocument,
                        HRApprovebyName = _db.GetEmployeeNameById(claims.employeeId),
                    };
                    _db.EmployeeExits.Add(ExitEmp);
                    await _db.SaveChangesAsync();

                    EmployeeExitsHistory exobj = new EmployeeExitsHistory
                    {
                        EmployeeId = model.EmployeeId,
                        EmployeeName = _db.GetEmployeeNameById(model.EmployeeId),
                        IsDiscussion = model.IsDiscussion,
                        DiscussionSummary = model.DiscussionSummary,
                        ReasonId = model.ReasonId,
                        Reason = Enum.GetName(typeof(ExitEmpReasonConstants), model.ReasonId),
                        ExitType = model.ExitType,
                        Status = ExitStatusConstants.Approved,
                        Comment = model.Comment,
                        TerminateDate = model.TerminateDate,
                        LastWorkingDate = model.LastWorkingDate,
                        IsReHired = model.IsReHired,
                        ExitFile = model.ExitFile,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        HRApproved = true,
                        InProgress = true,
                        HRApprovebyId = claims.employeeId,
                        TerminationDocument = model.TerminationDocument,
                        HRApprovebyName = _db.GetEmployeeNameById(claims.employeeId),
                    };
                    _db.EmployeeExitsHistorys.Add(exobj);
                    _db.SaveChanges();

                    res.Message = "Employee initating Resign process is complete";
                    res.Status = true;
                    res.Data = ExitEmp;
                }
                else
                {
                    res.Message = "Employee initating Resign process is not complete";
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

        #endregion Exit Employee Process By HR

        #region Add Resignation Attachment  File

        /// <summary>
        /// Created By Ravi Vyas On 09-08-2022
        /// API >> Post >> api/employeeexits/UploadResignationAttachment
        /// </summary>
        [HttpPost]
        [Route("uploadresignationattachment")]
        public async Task<UploadImageResponse> UploadResignationAttachment()
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

                        string extemtionType = Helper.MimeType.GetContentType(filename).Split('/').First();
                        if (extemtionType == "image" || extemtionType == "application")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/EmployeeExits/ResignationAttachment/" + claims.companyId), dates + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }

                            string path = "uploadimage\\EmployeeExits\\ResignationAttachment\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successful";
                            result.Status = true;
                            result.URL = FileUrl;
                            result.Path = path;
                            result.Extension = extension;
                            result.ExtensionType = extemtionType;

                            EmployeeExits ex = new EmployeeExits();
                            ex.ExitFile = result.Path;
                            ex.CreatedOn = DateTime.Now;
                            _db.EmployeeExits.Add(ex);
                            _db.SaveChanges();
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

        #endregion Add Resignation Attachment  File

        #region Get Employee Exits Data

        /// <summary>
        /// Create By Ravi Vyas 10-08-2022
        /// API >> Get >> api/employeeexits/exitdata
        /// </summary>
        [HttpGet]
        [Route("exitdata")]
        public async Task<ResponseBodyModel> GetData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "HR")
                {
                    var data = await (from em in _db.Employee
                                      join dp in _db.Designation on em.DesignationId equals dp.DesignationId
                                      join ex in _db.EmployeeExits on em.EmployeeId equals ex.EmployeeId
                                      where ex.CompanyId == claims.companyId && ex.IsActive && !ex.IsDeleted
                                      select new ExiteData
                                      {
                                          Settelment = ex.Settelment,
                                          Summary = ex.DiscussionSummary,
                                          HRApproved = ex.HRApproved,
                                          ITApproved = ex.ITApproved,
                                          Finalsettelment = ex.Finalsettelment,
                                          EmployeeExitId = ex.EmployeeExitId,
                                          DisplayName = ex.EmployeeName,
                                          EmployeeId = ex.EmployeeId,
                                          ExitType = ex.ExitType.ToString().Replace("_", " "),
                                          LocalAddress = em.LocalAddress,
                                          Comment = ex.Comment,
                                          TerminateDate = ex.CreatedOn,
                                          Status = ex.Status.ToString(),
                                          DesignatioName = dp.DesignationName,
                                          InProgress = ex.InProgress,
                                          ExitStatus = ex.Status == ExitStatusConstants.Exit ? "Exployee Exit" : ex.Status == ExitStatusConstants.Retain ? "Employee Retain" : "In Progress",
                                      }).ToListAsync();

                    if (data.Count > 0)
                    {
                        res.Message = "Data Found !";
                        res.Status = true;
                        res.Data = data;
                    }
                    else
                    {
                        res.Message = "Data Not Found !";
                        res.Status = false;
                        res.Data = data;
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

        #endregion Get Employee Exits Data

        #region Update Status By HR

        /// <summary>
        /// Created By Ravi Vyas 17-08-2022
        /// API >> PUT >> api/employeeexits/updateexitdata
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("updateexitdata")]
        public async Task<ResponseBodyModel> UpdateData(EmployeeExits model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await _db.EmployeeExits.Where(x => x.EmployeeId == model.EmployeeId && x.IsActive &&
                        !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (claims.roleType == "HR")
                {
                    if (!model.IsAcceptRetainig)
                    {
                        if (data != null)
                        {
                            data.TerminateDate = model.TerminateDate;
                            data.LastWorkingDate = model.LastWorkingDate;
                            data.IsReHired = model.IsReHired;
                            data.Comment = model.Comment;
                            data.ExitFile = model.ExitFile;
                            data.UpdatedOn = DateTime.Now;
                            data.Status = ExitStatusConstants.Approved;
                            data.IsReHired = model.IsReHired;
                            data.HRApproved = true;
                            data.InProgress = true;
                            data.IsDiscussion = false;

                            _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();

                            var pay = _db.Employee.Where(y => y.EmployeeId == model.EmployeeId && y.IsActive &&
                            !y.IsDeleted && y.CompanyId == claims.companyId).FirstOrDefault();

                            if (pay != null)
                            {
                                pay.PayGroupId = Guid.Empty;
                                pay.LeaveGroupId = 0;
                                _db.Entry(pay).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            }

                            res.Message = "Succesfully Update ";
                            res.Status = true;
                            res.Data = data;
                        }
                        else
                        {
                            res.Message = " Update Failed ! ";
                            res.Status = false;
                            res.Data = data;
                        }
                    }
                    else
                    {
                        if (data != null)
                        {
                            data.DiscussionSummary = model.DiscussionSummary;
                            data.UpdatedOn = DateTime.Now;
                            data.Status = ExitStatusConstants.Retain;
                            data.HRApproved = true;
                            data.InProgress = true;
                            data.IsDiscussion = true;
                            _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                            res.Message = "Succesfully Update ";
                            res.Status = true;
                            res.Data = data;
                        }
                        else
                        {
                            res.Message = " Update Failed ! ";
                            res.Status = false;
                            res.Data = data;
                        }
                    }
                    var empobj = await _db.EmployeeExitsHistorys.Where(x => x.EmployeeId == model.EmployeeId && x.IsActive &&
                            !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();

                    empobj.DiscussionSummary = model.DiscussionSummary;
                    empobj.Status = ExitStatusConstants.Retain;
                    empobj.HRApproved = model.HRApproved;
                    empobj.InProgress = model.InProgress;
                    empobj.IsDiscussion = model.IsDiscussion;
                    empobj.TerminateDate = model.TerminateDate;
                    empobj.LastWorkingDate = model.LastWorkingDate;
                    empobj.ExitFile = model.ExitFile;
                    empobj.IsReHired = model.IsReHired;
                    empobj.Comment = model.Comment;
                    empobj.UpdatedOn = DateTime.Now;
                    empobj.Status = model.IsAcceptRetainig ? ExitStatusConstants.Approved : ExitStatusConstants.Retain;
                    _db.Entry(empobj).State = System.Data.Entity.EntityState.Modified;
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

        #endregion Update Status By HR

        #region Get Exit Employee Assets

        /// <summary>
        /// Created By Suraj Bundel On 12-07-2022
        /// API >> Put >> api/employeeexits/getexitempasset
        /// </summary>
        [HttpGet]
        [Route("getexitempasset")]
        public async Task<ResponseBodyModel> GetAllExitEmployeeAssets(int Employeeid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<ExitEmpAssetlist> list = new List<ExitEmpAssetlist>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assetdata = await (from ass in _db.AssetsItemMasters
                                       join it in _db.AssetsCategories on ass.AssetsCategoryId equals it.AssetsCategoryId
                                       join em in _db.Employee on ass.AssignToId equals em.EmployeeId
                                       join dp in _db.Department on em.DepartmentId equals dp.DepartmentId
                                       join ex in _db.EmployeeExits on ass.AssignToId equals ex.EmployeeId
                                       where ex.CompanyId == claims.companyId && ass.AssetCondition != AssetConditionConstants.Damage &&
                                       em.EmployeeId == Employeeid && ass.IsActive && !ass.IsDeleted
                                       select new
                                       {
                                           ex.InProgress,
                                           em.EmployeeId,
                                           ex.EmployeeExitId,
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
                                           ass.RecoverById,
                                           ass.Recovered,
                                           assign = ass.AssignDate ?? ass.CreatedOn,
                                       }).ToListAsync();

                var employeeIds = assetdata.ConvertAll(x => new ExitEmpDistingByAssest
                {
                    AssignedToId = x.AssignToId,
                    DisplayName = x.DisplayName,
                    DepartmentName = x.DepartmentName,
                    ItemId = x.ItemId,
                    Condition = x.AssetCondition.ToString(),
                    WarehouseId = x.WareHouseId,
                    Recovered = x.Recovered,
                });
                foreach (var item in employeeIds.Select(x => x.AssignedToId).Distinct().ToList())
                {
                    var data = employeeIds.Find(x => x.AssignedToId == item);
                    ExitEmpAssetlist obj = new ExitEmpAssetlist
                    {
                        EmployeeId = data.AssignedToId,
                        EmployeeName = data.DisplayName,
                        Department = data.DepartmentName,
                        IsAllAssetsRecovered = assetdata.All(x => x.Recovered),
                        AssesdataList = assetdata.Where(x => x.AssignToId == data.AssignedToId)
                                .Select(x => new ExitEmpAssetdatamodel
                                {
                                    EmployeeName = x.DisplayName,
                                    ItemId = x.ItemId,
                                    ItemName = x.ItemName,
                                    Model = x.ItemCode,
                                    Serialnumber = x.SerialNo,
                                    Condition = x.AssetCondition.ToString(),
                                    WarehouseId = x.WareHouseId,
                                    AssignId = x.AssignToId,
                                    Recovered = x.Recovered,
                                }).ToList(),
                    };
                    list.Add(obj);
                }

                var rest = _db.AssetsItemMasters.Where(x => x.AssignToId == Employeeid &&
                        x.IsActive && !x.IsDeleted).Select(x => x.Recovered).ToList();
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
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Exit Employee Assets

        #region Exit Employee Dashboard API

        /// <summary>
        ///  Created by Suraj Bundel on 16/08/22
        ///  Modified by Ravi Vyas on 18/08/22
        ///  API >> POST >> api/employeeexits/exitempdashboard
        /// </summary>
        [HttpGet]
        [Route("exitempdashboard")]
        public async Task<ResponseBodyModel> ExitEmployeeDashboard()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                #region This Api Use To Dashboard

                var emexit = await _db.EmployeeExits.Where(x => x.EmployeeId == claims.employeeId && x.IsDeleted == false && x.IsActive).ToListAsync();
                List<ExitEmployeeGraphModalForMonth> exempData = new List<ExitEmployeeGraphModalForMonth>();
                List<ExitGraph> Graph = new List<ExitGraph>();
                for (int j = 1; j <= 12; j++)
                {
                    ExitEmployeeGraphModalForMonth exemp = new ExitEmployeeGraphModalForMonth();
                    //exemp.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)j);
                    exemp.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(j);
                    exemp.value = emexit.Where(x => x.Status == ExitStatusConstants.Pending).ToList().Count;
                    exempData.Add(exemp);
                }
                //response.Graph = exempData;

                #endregion This Api Use To Dashboard

                res.Message = "Success";
                res.Status = true;
                res.Data = exempData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Exit Employee Dashboard API

        #region Update Asset Details (Assets recover) by IT

        /// <summary>
        /// Created by Suraj Bundel On 16-08-2022
        /// API >> Put >> api/employeeexits/itapproveexitemp
        /// </summary>
        /// <param name="model"></ param >
        [HttpPut]
        [Route("itapproveexitemp")]
        public async Task<ResponseBodyModel> ITApproveExitEmployee(ITApproveExitEmployeeModel model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                var assetAssD = await _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted && x.AssignToId == model.EmployeeId && x.CompanyId == claims.companyId).ToListAsync();
                if (assetAssD.Count > 0)
                {
                    foreach (var item in assetAssD)
                    {
                        var exemp = _db.EmployeeExits.Where(x => x.EmployeeId == item.AssignToId && !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId).FirstOrDefault();
                        if (model.EmployeeId != 0)
                        {
                            exemp.ITApproved = true;
                            exemp.ITApprovebyId = claims.employeeId;
                            exemp.ITApprovebyName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault();
                            _db.Entry(exemp).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();

                            var emphist = _db.EmployeeExitsHistorys.FirstOrDefault(x => x.EmployeeId == item.AssignToId && !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                            if (model.EmployeeId != 0)
                            {
                                emphist.ITApproved = true;
                                emphist.ITApprovebyId = claims.employeeId;
                                emphist.ITApprovebyName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault();
                                _db.Entry(emphist).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            }
                            else
                            {
                                response.Message = "Employee History not managed";
                                response.Status = false;
                            }
                            if (item.Recovered)
                            {
                                response.Message = "Resignation approve by IT";
                                response.Status = true;
                            }
                            else
                            {
                                response.Message = "Resignation Reject by IT";
                                response.Status = false;
                            }
                        }
                        else
                        {
                            response.Message = "Employee Id not found.";
                            response.Status = false;
                        }
                    }
                }
                else
                {
                    var exemp = _db.EmployeeExits.Where(x => x.EmployeeId == model.EmployeeId && !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId).FirstOrDefault();
                    if (model.EmployeeId != 0)
                    {
                        exemp.ITApproved = true;
                        exemp.ITApprovebyId = claims.employeeId;
                        exemp.ITApprovebyName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault();
                        exemp.ITComments = model.Comments;
                        _db.Entry(exemp).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        var emphist = _db.EmployeeExitsHistorys.FirstOrDefault(x => x.EmployeeId == model.EmployeeId && !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                        if (model.EmployeeId != 0)
                        {
                            emphist.ITApproved = true;
                            emphist.ITApprovebyId = claims.employeeId;
                            emphist.ITApprovebyName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault();
                            emphist.ITComments = model.Comments;
                            _db.Entry(emphist).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();

                            response.Message = "Resignation approve by IT";
                            response.Status = true;
                        }
                        else
                        {
                            response.Message = "Resignation Reject by IT";
                            response.Status = false;
                            response.Data = exemp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        #endregion Update Asset Details (Assets recover) by IT

        #region EX Employee status change

        /// <summary>
        ///  Created by Ravi Vyas on 19/07/22
        ///  API >> GET >> api/employeeexits/getallexemployee
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("getallexemployee")]
        public async Task<ResponseBodyModel> GetAllExEmployee()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exEmployee = await (from e in _db.Employee
                                        join ex in _db.EmployeeExits on e.EmployeeId equals ex.EmployeeId
                                        join d in _db.Designation on e.DesignationId equals d.DesignationId
                                        /*where ex.IsActive == true && ex.IsDeleted == false && ex.CompanyId == claims.companyid && ex.Status == ExitStatusEnum.Exit&& e.EmployeeTypeId == 2*/
                                        select new ExEmployeeRes
                                        {
                                            EmployeeId = e.EmployeeId,
                                            EmployeeName = e.DisplayName,
                                            joiningDate = e.JoiningDate,
                                            TerminateDate = ex.TerminateDate,
                                            ExitType = ex.ExitType.ToString(),
                                            Gender = e.Gender,
                                            DesignatioName = d.DesignationName,
                                            DepartmentName = _db.Department.Where(x => x.DepartmentId == e.DepartmentId).Select(x => x.DepartmentName).FirstOrDefault(),
                                            DiscussionSummary = ex.DiscussionSummary,
                                            Reason = ex.Reason,
                                            Comment = ex.Comment,
                                            Status = ex.Status,
                                            LastWorkingDate = ex.LastWorkingDate,
                                        }).ToListAsync();
                ExEmployeeRes obj = new ExEmployeeRes();
                foreach (var item in exEmployee)
                {
                    obj.EmployeeName = item.EmployeeName;
                    obj.joiningDate = item.joiningDate;
                    obj.TerminateDate = item.TerminateDate;
                    obj.ExitType = item.ExitType.ToString();
                    obj.Gender = item.Gender;
                    obj.DesignatioName = item.DesignatioName;
                    obj.DepartmentName = item.DepartmentName;
                    obj.DiscussionSummary = item.DiscussionSummary;
                    obj.Reason = item.Reason;
                    obj.Comment = item.Comment;
                    obj.Status = item.Status;
                    obj.LastWorkingDate = item.LastWorkingDate;
                    obj.serveperioud = GetMonthDifference(item.joiningDate, item.LastWorkingDate.Value) / 12;
                }

                if (exEmployee.Count > 0)
                {
                    res.Message = "Success";
                    res.Status = true;
                    res.Data = obj;
                }

                res.Message = "No list found";
                res.Status = false;
                res.Data = obj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion EX Employee status change

        #region get Approved list// For IT

        /// <summary>
        /// Created by Suraj Bundel on 16/08/2022
        /// API >> Get >> api/employeeexits/getexitempApprovelist
        /// </summary>
        [HttpGet]
        [Route("getexitempapprovelist")]
        public async Task<ResponseBodyModel> GetExitEmployeeApprovelist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exempobj = await (from ex in _db.EmployeeExits
                                      join e in _db.Employee on ex.EmployeeId equals e.EmployeeId
                                      where ex.IsActive && !ex.IsDeleted && ex.CompanyId == claims.companyId &&
                                      ex.Status == ExitStatusConstants.Approved && ex.HRApproved && !ex.ITApproved
                                      select new GetExitEmployeeApproveModel
                                      {
                                          EmployeeExitId = ex.EmployeeExitId,
                                          EmployeeId = ex.EmployeeId,
                                          EmployeeName = ex.EmployeeName,
                                          IsDiscussion = ex.IsDiscussion,
                                          DiscussionSummary = ex.DiscussionSummary,
                                          ReasonId = ex.ReasonId,
                                          Reason = ex.Reason,
                                          Comment = ex.Comment,
                                          ExitType = ex.ExitType,
                                          Status = ex.Status,
                                          TerminateDate = ex.TerminateDate,
                                          LastWorkingDate = ex.LastWorkingDate,
                                          IsReHired = ex.IsReHired,
                                          IsAcceptRetainig = ex.IsAcceptRetainig,
                                          DesignatioName = _db.Designation.Where(x => x.DesignationId == e.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                          InProgress = ex.InProgress,
                                      }).ToListAsync();

                if (exempobj.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = exempobj;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = exempobj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.Data = null;
                return res;
            }
            return res;
        }

        #endregion get Approved list// For IT

        #region get Approved list// For HR

        /// <summary>
        /// Created by Suraj Bundel on 16/08/2022
        /// API >> Get >> api/employeeexits/gethrexitempApprovelist
        /// </summary>
        /// <returns></returns>

        [Route("gethrexitempApprovelist")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetHRExitEmployeeApprovelist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "HR")
                {
                    var exempobj = await (from ex in _db.EmployeeExits
                                          join e in _db.Employee on ex.EmployeeId equals e.EmployeeId
                                          where (ex.IsActive && !ex.IsDeleted && ex.CompanyId == claims.companyId &&
                                          ex.Status == ExitStatusConstants.Approved && ex.HRApproved && ex.ITApproved)
                                          select new GetExitEmployeeApproveModel
                                          {
                                              EmployeeExitId = ex.EmployeeExitId,
                                              EmployeeId = ex.EmployeeId,
                                              EmployeeName = ex.EmployeeName,
                                              IsDiscussion = ex.IsDiscussion,
                                              DiscussionSummary = ex.DiscussionSummary,
                                              ReasonId = ex.ReasonId,
                                              Reason = ex.Reason,
                                              Comment = ex.Comment,
                                              ExitType = ex.ExitType,
                                              Status = ex.Status,
                                              TerminateDate = ex.TerminateDate,
                                              LastWorkingDate = ex.LastWorkingDate,
                                              IsReHired = ex.IsReHired,
                                              IsAcceptRetainig = ex.IsAcceptRetainig,
                                              DesignatioName = _db.Designation.Where(x => x.DesignationId == e.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                              InProgress = ex.InProgress,
                                          }).ToListAsync();
                    if (exempobj.Count != 0)
                    {
                        res.Status = true;
                        res.Message = "Employee list Found";
                        res.Data = exempobj;
                    }
                    else
                    {
                        res.Message = "No Employee list Found";
                        res.Status = false;
                        res.Data = exempobj;
                    }
                }
                else
                {
                    res.Message = "Only For IT Department";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.Data = null;
                return res;
            }
            return res;
        }

        #endregion get Approved list// For HR

        #region Final Settelment amount // Working On it // commented

        /// <summary>
        ///  Created by Suraj Bundel on 19/07/22
        ///  API >> POST >> api/employeeexits/finalsettelment
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("finalsettelment")]
        public async Task<ResponseBodyModel> FinalSettelment()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getdata = await _db.EmployeeExits.Where(x => x.IsActive && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();

                var exempobj = await _db.EmployeeExits.Where(x => x.IsActive && x.IsDeleted == false && x.CompanyId == claims.companyId && x.Status == ExitStatusConstants.Approved).ToListAsync();

                res.Message = "Success";
                res.Status = true;
                res.Data = getdata;

                if (exempobj.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = exempobj;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = exempobj;
                }
            }
            catch
            {
                res.Message = "failed";
                res.Status = false;
            }
            return res;
        }

        #endregion Final Settelment amount // Working On it // commented

        #region get list of exit employee for HR & IT Approve

        /// <summary>
        ///  Created by Suraj Bundel on 18/08/2022
        ///  API >> GET >> api/employeeexits/getithraproval
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("getithraproval")]
        public async Task<ResponseBodyModel> GetITHRApproval()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "IT")
                {
                    var approval = await _db.EmployeeExits.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.ITApproved && x.HRApproved).ToListAsync();
                    if (approval.Count > 0)
                    {
                        res.Message = "Success";
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
                else
                {
                    res.Message = "Only For IT & HR Department";
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

        #endregion get list of exit employee for HR & IT Approve

        #region get settelment // For HR

        /// <summary>
        /// Created by Suraj Bundel on 16/08/2022
        /// API >> Get >> api/employeeexits/getexitempsettelment
        /// </summary>
        [HttpGet]
        [Route("getexitempsettelment")]
        public async Task<ResponseBodyModel> GetExitEmployeeSettelment(int Employeeid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exempobj = await (from ex in _db.EmployeeExits
                                      join e in _db.Employee on ex.EmployeeId equals e.EmployeeId
                                      where ex.IsActive && !ex.IsDeleted && ex.CompanyId == claims.companyId && ex.Status == ExitStatusConstants.Approved && ex.EmployeeId == Employeeid
                                      select new GetExitEmployeeSettelmentModel
                                      {
                                          GoodAssets = _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted && (x.AssetCondition == AssetConditionConstants.Good || x.AssetCondition == AssetConditionConstants.Fair) && x.CompanyId == claims.companyId && x.AssignToId == Employeeid).Count(),
                                          DamageAssets = _db.AssetsItemMasters.Where(x => x.IsActive && !x.IsDeleted && x.AssetCondition == AssetConditionConstants.Damage && x.CompanyId == claims.companyId && x.AssignToId == Employeeid).Count(),
                                          EmployeeExitId = ex.EmployeeExitId,/*,*/
                                          EmployeeId = ex.EmployeeId,
                                          EmployeeName = ex.EmployeeName,
                                          IsDiscussion = ex.IsDiscussion,
                                          DiscussionSummary = ex.DiscussionSummary,
                                          ReasonId = ex.ReasonId,
                                          Reason = ex.Reason,
                                          Comment = ex.Comment,
                                          ExitType = ex.ExitType,
                                          Status = ex.Status,
                                          Joingdate = e.JoiningDate,
                                          LastWorkingDate = ex.LastWorkingDate,
                                          IsReHired = ex.IsReHired,
                                          IsAcceptRetainig = ex.IsAcceptRetainig,
                                          TerminateDate = ex.TerminateDate,
                                          DesignatioName = _db.Designation.Where(x => x.DesignationId == e.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                          InProgress = ex.InProgress,
                                          //WorkingDay =Enumerable.Range(0, 1 + (ex.LastWorkingDate - e.JoiningDate).Days).Select(offset => (e.JoiningDate).AddDays(offset)).Count()
                                          //WorkingDay = ForEach(x => GetDatesBetween(e.JoiningDate, ex.LastWorkingDate, dayOfWeeks, dateList)),
                                          WorkingDay = 0,
                                          //WorkingDay = GetDatesBetweenCount(e.JoiningDate, ex.LastWorkingDate),
                                          //WorkingDay = (ex.LastWorkingDate.Value.Day - e.JoiningDate.value.Day),
                                          //String workingDays = (ex.LastWorkingDate - e.JoiningDate).,
                                          //WorkingDay = DbFunctions.DiffDays(e.JoiningDate, ex.LastWorkingDate)
                                          //WorkingDay = SqlFunctions.DateDiff("Day", e.JoiningDate, ex.LastWorkingDate),
                                          //WorkingDay = (ex.LastWorkingDate - e.JoiningDate).to,
                                      }).FirstOrDefaultAsync();

                if (exempobj != null)
                {
                    //exempobj.ForEach(x => x.WorkingDay = );
                    exempobj.WorkingDay = GetDatesBetweenCount(exempobj.TerminateDate, exempobj.LastWorkingDate);
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = exempobj;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = exempobj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.Data = null;
            }
            return res;
        }

        public int GetDatesBetweenCount(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var daysCount = 0;
            for (DateTimeOffset date = (DateTimeOffset)startDate; date <= (DateTimeOffset)endDate; date = date.AddDays(1))
                daysCount++;
            return daysCount;
        }

        #endregion get settelment // For HR

        #region Addleave and netpay // For HR

        /// <summary>
        /// Created by Ravi Vyas on 16/08/2022
        /// API >> Get >> api/employeeexits/addleavenetpay
        /// </summary>
        [HttpPut]
        [Route("addleavenetpay")]
        public async Task<ResponseBodyModel> AddLeaveNetPay(GetExitEmployeeSettelmentModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var data = _db.EmployeeExits.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId &&
                            x.Status == ExitStatusConstants.Approved && x.EmployeeId == model.EmployeeId).FirstOrDefault();
                    if (data != null)
                    {
                        data.LeaveTaken = model.leaveTaken;
                        data.Netpay = model.Netpay;
                        data.Settelment = true;
                        _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        var histdata = _db.EmployeeExitsHistorys.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId &&
                                x.EmployeeId == model.EmployeeId).FirstOrDefault();
                        histdata.LeaveTaken = data.LeaveTaken;
                        histdata.Netpay = data.Netpay;
                        histdata.Settelment = data.Settelment;
                        _db.Entry(histdata).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        res.Status = true;
                        res.Message = "Employee list Found";
                        res.Data = data;
                    }
                    else
                    {
                        res.Message = "No Employee list Found";
                        res.Status = false;
                        res.Data = data;
                    }
                }
                else
                {
                    res.Message = "Model is null";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.Data = null;
                return res;
            }
            return res;
        }

        #endregion Addleave and netpay // For HR

        #region get Approved list// For HR

        /// <summary>
        /// Created by Suraj Bundel on 16/08/2022
        /// API >> Get >> api/employeeexits/getfinalsettelment
        /// </summary>
        [HttpGet]
        [Route("getfinalsettelment")]
        public async Task<ResponseBodyModel> GetFinalSettelment(int Employeeid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exempobj = await (from ex in _db.EmployeeExits
                                      join e in _db.Employee on ex.EmployeeId equals e.EmployeeId
                                      where (ex.IsActive && !ex.IsDeleted && ex.CompanyId == claims.companyId &&
                                      ex.Status == ExitStatusConstants.Approved && ex.EmployeeId == Employeeid)
                                      select new GetFinalSettelmentModel
                                      {
                                          EmployeeId = ex.EmployeeId,
                                          EmployeeName = ex.EmployeeName,
                                          DepartmentName = _db.Department.Where(x => x.DepartmentId == e.DepartmentId).Select(x => x.DepartmentName).FirstOrDefault(),
                                          DesignatioName = _db.Designation.Where(x => x.DesignationId == e.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                          Gender = e.Gender,
                                          joiningDate = e.JoiningDate,
                                          LastWorkingDate = ex.LastWorkingDate,
                                          Reason = ex.Reason,
                                          DiscussionSummary = ex.DiscussionSummary,
                                          Comment = ex.Comment,
                                          ExitType = ex.ExitType.ToString().Replace("_", " "),
                                          Status = ex.Status,
                                      }).ToListAsync();

                GetFinalSettelmentModel objh = new GetFinalSettelmentModel();
                foreach (var item in exempobj)
                {
                    objh.EmployeeId = item.EmployeeId;
                    objh.EmployeeName = item.EmployeeName;
                    objh.DepartmentName = item.DepartmentName;
                    objh.DesignatioName = item.DesignatioName;
                    objh.Gender = item.Gender;
                    objh.joiningDate = item.joiningDate;
                    objh.LastWorkingDate = item.LastWorkingDate;
                    objh.Reason = item.Reason;
                    objh.DiscussionSummary = item.DiscussionSummary;
                    objh.Comment = item.Comment;
                    objh.ExitType = item.ExitType.Replace("_", " ");
                    objh.Status = item.Status;
                    objh.serveperioud = GetMonthDifference(item.joiningDate.Value, item.LastWorkingDate.Value) / 12;
                }
                if (exempobj.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = objh;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = objh;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion get Approved list// For HR

        #region get Approved list Ex// For HR

        /// <summary>
        /// Created by Ravi Vyas on 16/08/2022
        /// API >> Get >> api/employeeexits/getfinalsettelment
        /// </summary>
        /// <returns></returns>

        [Route("getexfinalsettelment")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetExEmployee()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exempobj = await (from ex in _db.EmployeeExits
                                      join e in _db.Employee on ex.EmployeeId equals e.EmployeeId
                                      where (ex.IsActive && ex.IsDeleted == false)
                                      select new GetFinalSettelmentModel
                                      {
                                          EmployeeId = ex.EmployeeId,
                                          EmployeeName = ex.EmployeeName,
                                          DepartmentName = _db.Department.Where(x => x.DepartmentId == e.DepartmentId).Select(x => x.DepartmentName).FirstOrDefault(),
                                          DesignatioName = _db.Designation.Where(x => x.DesignationId == e.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),
                                          Gender = e.Gender,
                                          joiningDate = e.JoiningDate,
                                          LastWorkingDate = ex.LastWorkingDate,
                                          Reason = ex.Reason,
                                          DiscussionSummary = ex.DiscussionSummary,
                                          Comment = ex.Comment,
                                          ExitType = ex.ExitType.ToString().Replace("_", " "),
                                          //Status = ex.Status.ToString().Replace("_", " "),
                                      }).ToListAsync();

                GetFinalSettelmentModel objh = new GetFinalSettelmentModel();
                foreach (var item in exempobj)
                {
                    objh.EmployeeId = item.EmployeeId;
                    objh.EmployeeName = item.EmployeeName;
                    objh.DepartmentName = item.DepartmentName;
                    objh.DesignatioName = item.DesignatioName;
                    objh.Gender = item.Gender;
                    objh.joiningDate = item.joiningDate;
                    objh.LastWorkingDate = item.LastWorkingDate;
                    objh.Reason = item.Reason;
                    objh.DiscussionSummary = item.DiscussionSummary;
                    objh.Comment = item.Comment;
                    objh.ExitType = item.ExitType.ToString().Replace("_", " ");
                    objh.Status = item.Status;
                    objh.serveperioud = GetMonthDifference(item.joiningDate.Value, item.LastWorkingDate.Value) / 12;
                }
                if (exempobj.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = objh;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = objh;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion get Approved list Ex// For HR

        #region Update final settelment // for HR

        /// <summary>
        /// Created by SurajBundel On 19-08-2022
        /// API >> Put>> api/employeeexits/updatefinalsettelment
        /// </summary>
        /// <param name="EmployeeId"></param>
        [HttpPut]
        [Route("updatefinalsettelment")]
        public async Task<ResponseBodyModel> UpdateFinalSettelment(int EmployeeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "HR")
                {
                    var finalData = _db.EmployeeExits.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
                    if (finalData != null)
                    {
                        var empexit = _db.Employee.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
                        if (finalData != null)
                        {
                            finalData.UpdatedBy = claims.employeeId;
                            finalData.Status = ExitStatusConstants.Exit;
                            finalData.UpdatedOn = DateTime.Now;
                            finalData.InProgress = false;
                            finalData.Finalsettelment = true;
                            empexit.EmployeeTypeId = EmployeeTypeConstants.Ex_Employee;

                            _db.Entry(empexit).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                            _db.Entry(finalData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            res.Message = "Employee Removed Successfully";
                            res.Status = true;
                            res.Data = finalData;
                        }
                        else
                        {
                            res.Message = "Failed to Removed Employee";
                            res.Status = false;
                        }
                        var histData = _db.EmployeeExitsHistorys.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
                        if (histData != null)
                        {
                            histData.UpdatedBy = claims.employeeId;
                            histData.Status = ExitStatusConstants.Exit;
                            histData.UpdatedOn = DateTime.Now;
                            histData.InProgress = false;
                            _db.Entry(histData).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            res.Message = "Failed to Removed Employee";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "Only for HR";
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

        public class tokendispose
        {
            public string Token { get; set; }
        }

        #endregion Update final settelment // for HR

        #region Api For All Information Count

        /// <summary>
        /// Created By Ravi Vyas on  18-08-2022
        /// API >> Get >> api/employeeexits/allinfo
        /// </summary>
        [HttpGet]
        [Route("allinfo")]
        public async Task<ResponseBodyModel> GetAllData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            ExitEmployeeResignation response = new ExitEmployeeResignation();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var allResignedEmployees = await _db.EmployeeExits.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (allResignedEmployees.Count > 0)
                {
                    response.AllData = allResignedEmployees.Count;

                    var employeeResignation = allResignedEmployees.Where(x => x.ExitType == ExitInitingType.Employee_Want_To_Resign).ToList();
                    var terminate = allResignedEmployees.Where(x => x.ExitType == ExitInitingType.Company_Decideds_To_Terminate).ToList();
                    var pendingResignation = allResignedEmployees.Where(x => x.Status == ExitStatusConstants.Pending).ToList();
                    var exit = allResignedEmployees.Where(x => x.Status == ExitStatusConstants.Exit).ToList();

                    response.EmployeeResignation = employeeResignation.Count;
                    response.HrTerminate = terminate.Count;
                    response.PendingResignation = pendingResignation.Count;
                    response.ExitEmployee = exit.Count;

                    #region This Api Use To Resign Dashboard

                    var emexit = await _db.EmployeeExits.Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId).ToListAsync();
                    List<ExitEmployeeGraphModalForMonth> exempData = new List<ExitEmployeeGraphModalForMonth>();
                    for (int j = 1; j <= 12; j++)
                    {
                        ExitEmployeeGraphModalForMonth exemp = new ExitEmployeeGraphModalForMonth
                        {
                            name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(j),
                            value = emexit.Where(x => x.Status == ExitStatusConstants.Approved && x.CreatedOn.Month == j).ToList().Count
                        };
                        exempData.Add(exemp);
                    }
                    response.ExitEmployeeGraphModalForMonth = exempData;

                    #endregion This Api Use To Resign Dashboard

                    #region This Api Use To Retain Dashboard

                    List<ExitEmployeeGraphModalForMonth> exempData1 = new List<ExitEmployeeGraphModalForMonth>();
                    for (int j = 1; j <= 12; j++)
                    {
                        ExitEmployeeGraphModalForMonth exemp1 = new ExitEmployeeGraphModalForMonth
                        {
                            name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(j),
                            value = allResignedEmployees.Where(x => x.Status == ExitStatusConstants.Retain && x.CreatedOn.Month == j).ToList().Count
                        };
                        exempData1.Add(exemp1);
                    }

                    response.RetainEmployeeGraphModalForMonth = exempData1;

                    #endregion This Api Use To Retain Dashboard

                    res.Message = "Data Found !";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    response.ExitEmployeeGraphModalForMonth = new List<ExitEmployeeGraphModalForMonth>();
                    response.RetainEmployeeGraphModalForMonth = new List<ExitEmployeeGraphModalForMonth>();
                    res.Message = "Data Not Found !";
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

        #endregion Api For All Information Count

        #region Api For Get All Retain Employee

        /// <summary>
        /// Created By Ravi Vyas on 18-08-2022
        /// API >> Get >> api/employeeexits/getretaindata
        /// </summary>
        /// <returns></returns>
        [Route("getretaindata")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllRetain()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "HR")
                {
                    var retainEmployee = await _db.EmployeeExits.Where(x => x.IsActive && x.IsDeleted == false && x.CompanyId == claims.companyId && x.Status == ExitStatusConstants.Retain).ToListAsync();
                    if (retainEmployee.Count > 0)
                    {
                        res.Message = "Data Found !";
                        res.Status = true;
                        res.Data = retainEmployee;
                    }
                    else
                    {
                        res.Message = "Data Not Found !";
                        res.Status = false;
                        res.Data = retainEmployee;
                    }
                }
                else
                {
                    res.Message = " Sorry ! Access Denied !";
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

        #endregion Api For Get All Retain Employee

        #region get Ex-Employee list//

        /// <summary>
        /// Created by Suraj Bundel on 29/08/2022
        /// API >> Get >> api/employeeexits/getexemployee
        /// </summary>
        [HttpGet]
        [Route("getexemployee")]
        public async Task<ResponseBodyModel> GetExEmployeelist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exempobj = await (from ex in _db.EmployeeExits
                                      join e in _db.Employee on ex.EmployeeId equals e.EmployeeId
                                      join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                      join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                      where ex.IsActive && !ex.IsDeleted && ex.CompanyId == claims.companyId
                                      && e.EmployeeTypeId == EmployeeTypeConstants.Ex_Employee
                                      select new GetExEmployeeListModel
                                      {
                                          EmployeeExitId = ex.EmployeeExitId,
                                          EmployeeId = ex.EmployeeId,
                                          EmployeeName = ex.EmployeeName,
                                          Department = d.DepartmentName,
                                          DesignatioName = ds.DesignationName,
                                          DepartmentName = e.DepartmentName,
                                          Joingdate = e.JoiningDate,
                                          LastWorkingDate = (DateTime)ex.LastWorkingDate,
                                          TerminateDate = ex.TerminateDate,
                                          WorkingYears = 0.0,
                                          ExitType = ex.ExitType == ExitInitingType.Employee_Want_To_Resign ?
                                                "Self Resign" : "Terminated",
                                      }).ToListAsync();

                if (exempobj.Count > 0)
                {
                    exempobj.ForEach(x => x.WorkingYears = Math.Round((double)
                                    (((x.LastWorkingDate.Year - x.Joingdate.Year) * 12)
                                    + x.LastWorkingDate.Month - x.Joingdate.Month) / 12, 2));
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = exempobj;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = exempobj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.Data = null;
            }
            return res;
        }

        #endregion get Ex-Employee list//

        #region GetMonthDifference

        public double GetMonthDifference(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            double monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        #endregion GetMonthDifference

        #region API To Get Year List Of Exit Dashboard

        /// <summary>
        /// Created By Harshit Mitra On 30-08-2022
        /// API >> Get >> api/employeeexits/exitdashboardyearlist
        /// </summary>
        [HttpGet]
        [Route("exitdashboardyearlist")]
        public async Task<ResponseBodyModel> GetExitDashboardYearList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitRequest = await _db.EmployeeExits.Where(x => claims.companyId == x.CompanyId).ToListAsync();
                var checkYear = exitRequest
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
                    res.Message = "No Exit Request Found";
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

        #endregion API To Get Year List Of Exit Dashboard

        #region API To Get Month List Of Exit Dashboard

        /// <summary>
        /// Created By Harshit Mitra On 01-08-2022
        /// API >> Get >> api/employeeexits/exitdashboardmonthlist
        /// </summary>
        /// <param name="year"></param>
        [HttpGet]
        [Route("exitdashboardmonthlist")]
        public async Task<ResponseBodyModel> GetExitDashboardMonthList(int year)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitRequest = await _db.EmployeeExits.Where(x => claims.companyId == x.CompanyId && x.CreatedOn.Year == year)
                            .Select(x => x.CreatedOn.Month).Distinct().OrderBy(x => x).FirstOrDefaultAsync();
                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                        .TakeWhile(m => m != String.Empty)
                                        .Select((m, i) => new
                                        {
                                            Month = i + 1,
                                            MonthName = m
                                        }).ToList();
                months = months.Skip(exitRequest - 1).ToList();
                if (months.Count > 0)
                {
                    res.Message = "Month List";
                    res.Status = true;
                    res.Data = months;
                }
                else
                {
                    res.Message = "No Exit Request Found";
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

        #endregion API To Get Month List Of Exit Dashboard

        #region Employee Exit Dashboard

        /// <summary>
        /// Created By Harshit Mitra on 30-08-2022
        /// API >> Get >> api/employeeexits/exitdashboard
        /// </summary>
        [HttpGet]
        [Route("exitdashboard")]
        public async Task<ResponseBodyModel> ExitDashboard(int year, int? month = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var exitRequests = await _db.EmployeeExits.Where(x => x.CompanyId == claims.companyId &&
                        x.IsActive && !x.IsDeleted && x.CreatedOn.Year == year).ToListAsync();
                var checkER = month.HasValue ? exitRequests.Where(x => x.CreatedOn.Month == month).ToList() : exitRequests;

                #region Box Data

                var resposnseBox = new List<object>()
                {
                    new
                    {
                        Name = "Total Requested",
                        Value = checkER.Count,
                        Inner = new List<object>()
                        {
                            new
                            {
                                Name = "In Progress",
                                Value = checkER.Count(x=> x.Status != ExitStatusConstants.Exit && x.Status != ExitStatusConstants.Retain),
                            },
                            new
                            {
                                Name = "Retain",
                                Value = checkER.Count(x=> x.Status == ExitStatusConstants.Retain),
                            },
                            new
                            {
                                Name = "Exit",
                                Value = checkER.Count(x=> x.Status == ExitStatusConstants.Exit),
                            },
                        },
                    },
                };

                #endregion Box Data

                #region Exit Reason Pie Graph

                var exitResons = Enum.GetValues(typeof(ExitEmpReasonConstants))
                                .Cast<ExitEmpReasonConstants>()
                                .Select(x => new
                                {
                                    TypeId = (int)x,
                                    TypeName = Enum.GetName(typeof(ExitEmpReasonConstants), x).Replace("_", " ")
                                }).OrderBy(x => x.TypeName == "Other").ToList();

                var exitResonPieChart = new List<object>();
                foreach (var emp in exitResons)
                {
                    var innerPieData = new
                    {
                        Name = emp.TypeName,
                        Value = checkER.Count(x => x.ReasonId == emp.TypeId),
                    };
                    exitResonPieChart.Add(innerPieData);
                }

                #endregion Exit Reason Pie Graph

                #region Department Dynamic Bar Graph

                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                         .TakeWhile(m => m != string.Empty)
                                         .Select((m, i) => new
                                         {
                                             Month = i + 1,
                                             MonthName = m
                                         }).ToList();
                var monthNamesLable = months.ConvertAll(x => x.MonthName);

                var empDepartent = (from c in checkER
                                    join e in _db.Employee on c.EmployeeId equals e.EmployeeId
                                    join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                    select new
                                    {
                                        c.EmployeeId,
                                        e.DisplayName,
                                        d.DepartmentId,
                                        d.DepartmentName,
                                        c.CreatedOn,
                                        e.JoiningDate,
                                        c.LastWorkingDate,
                                        e.EmployeeTypeId,
                                    }).ToList();

                var departmentList = empDepartent
                        .Select(x => new
                        {
                            x.DepartmentId,
                            x.DepartmentName,
                        }).Distinct().ToList();
                var depatmentNameLable = empDepartent.Select(x => x.DepartmentName).Distinct().ToList();

                List<dynamic> lableList = new List<dynamic>();
                if (month.HasValue)
                {
                    BarGraphLabelModel obj = new BarGraphLabelModel
                    {
                        Label = "Employee Count In Department",
                        Stack = 1,
                        Data = new object(),
                    };
                    List<int> data = new List<int>();
                    foreach (var dep in departmentList)
                    {
                        data.Add(empDepartent.Count(x => x.DepartmentId == dep.DepartmentId));
                    }
                    obj.Data = data;
                    lableList.Add(obj);
                }
                else
                {
                    foreach (var dep in departmentList)
                    {
                        BarGraphLabelModel obj = new BarGraphLabelModel
                        {
                            Label = dep.DepartmentName,
                            DepartmentId = dep.DepartmentId,
                            Stack = 1,
                            Data = new object(),
                        };
                        lableList.Add(obj);
                    }
                    foreach (var item in lableList)
                    {
                        List<int> data = new List<int>();
                        foreach (var mnt in months)
                        {
                            data.Add(empDepartent.Count(x => x.DepartmentId == item.DepartmentId && x.CreatedOn.Month == mnt.Month));
                        }
                        item.Data = data;
                    }
                }

                #endregion Department Dynamic Bar Graph

                #region Get Resource Lost Count Top 10

                var employeeResourceCount = empDepartent
                            .Where(x => x.EmployeeTypeId == EmployeeTypeConstants.Ex_Employee)
                            .Select(x => new
                            {
                                x.DisplayName,
                                WorkingYears = Math.Round((double)(((((DateTime)x.LastWorkingDate).Year - x.JoiningDate.Year) * 12) +
                                                ((DateTime)x.LastWorkingDate).Month - x.JoiningDate.Month) / 12, 2),
                            })
                            .OrderByDescending(x => x.WorkingYears)
                            .Take(10)
                            .ToList();

                #endregion Get Resource Lost Count Top 10

                res.Message = "Exit Dashboard";
                res.Status = true;
                res.Data = new
                {
                    BoxData = resposnseBox,
                    ExitResonPieChart = exitResonPieChart,
                    BarGraph = month.HasValue ?
                    new
                    {
                        Label = depatmentNameLable,
                        BarData = lableList.ConvertAll(x => new
                        {
                            x.Label,
                            x.Stack,
                            x.Data,
                        }).ToList(),
                    }
                    :
                    new
                    {
                        Label = monthNamesLable,
                        BarData = lableList.ConvertAll(x => new
                        {
                            x.Label,
                            x.Stack,
                            x.Data,
                        }).ToList(),
                    },
                    ResourceLost = employeeResourceCount,
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class BarGraphLabelModel
        {
            public string Label { get; set; }
            public int DepartmentId { get; set; }
            public int Stack { get; set; }
            public object Data { get; set; }
        }

        #endregion Employee Exit Dashboard

        #region API To Upload Exit Decoument

        /// <summary>
        /// Created By Harshit Mitra On 01-09-2022
        /// API >> Post >> api/employeeexits/uploadexitdocument
        /// </summary>
        [HttpPost]
        [Route("uploadexitdocument")]
        public async Task<UploadImageResponse> UploadEmployeeDocments()
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
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"').Replace(" ", "");

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/exitfiles/" + claims.companyId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                            Directory.CreateDirectory(DirectoryURL);

                        string path = "uploadimage\\exitfiles\\" + claims.companyId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion API To Upload Exit Decoument

        #region Api to get list of resigning employees
        /// <summary>
        /// API => Get =>api/employeeexits/getresignemployee
        /// </summary>
        /// <returns></returns>
        [Route("getresignemployee")]
        [HttpGet]
        public async Task<IHttpActionResult> GetResignEmployeeList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                //      x.CompanyId == tokendata.companyId && x.RoleName == "Global").FirstOrDefault();
                //if (tokendata.roleType == null)
                //{
                //    res.Message = " You don't have permission !";
                //    res.Status = false;
                //    res.StatusCode = HttpStatusCode.NoContent;
                //    return Ok(res);
                //}
                //else
                //{
                var data = await (from emp in _db.Employee
                                  join des in _db.Designation on emp.DesignationId equals des.DesignationId
                                  join dep in _db.Department on emp.DepartmentId equals dep.DepartmentId
                                  join org in _db.OrgMaster on emp.OrgId equals org.OrgId into re
                                  from result in re.DefaultIfEmpty()
                                  join ex in _db.EmployeeExits on emp.EmployeeId equals ex.EmployeeId
                                  where ex.CompanyId == tokendata.companyId && ex.IsActive && !ex.IsDeleted
                                  //&& emp.EmployeeTypeId==EmployeeTypeConstants.Ex_Employee
                                  select new
                                  {
                                      EmployeeId = emp.EmployeeId,
                                      DisplayName = emp.DisplayName,
                                      MobilePhone = emp.MobilePhone,
                                      DepartmentId = dep.DepartmentId,
                                      DepartmentName = dep.DepartmentName,
                                      DesignationName = des.DesignationName,
                                      DesignationId = des.DesignationId,
                                      OfficeEmail = emp.OfficeEmail,
                                      OrganizationName = result.OrgName ?? "Company Head",
                                      EmployeeExitId = ex.EmployeeExitId,
                                      ExitType = ex.ExitType.ToString().Replace("_", " "),
                                      TerminateDate = ex.CreatedOn,
                                      Status = ex.Status.ToString(),
                                      ExitStatus = ex.Status == ExitStatusConstants.Exit ? "Exployee Exit" : ex.Status == ExitStatusConstants.Retain ? "Employee Retain" : "In Progress",
                                      HRUpdate = emp.EmployeeTypeId == EmployeeTypeConstants.Ex_Employee ? emp.UpdatedOn : null,

                                  }).ToListAsync();

                if (data.Count == 0)
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = data;
                }
                else
                {
                    res.Message = "Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = data;
                }

                // }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return Ok(res);

        }
        #endregion

        #region Helper Model

        /// <summary>
        /// Create by Shriya Malvi On 09-08-2022
        /// </summary>
        ///

        public class HelperModelForEnum
        {
            public int TypeId { get; set; }
            public string TypeName { get; set; }
        }

        public class ITApproveExitEmployeeModel
        {
            public int? EmployeeId { get; set; }
            public string Comments { get; set; }
        }

        public class GetFinalSettelmentModel
        {
            public string DesignatioName { get; set; }
            public string DepartmentName { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Gender { get; set; }
            public string DiscussionSummary { get; set; }
            public string Reason { get; set; }
            public string Comment { get; set; }
            public string ExitType { get; set; }
            public ExitStatusConstants Status { get; set; }
            public DateTimeOffset? joiningDate { get; set; }
            public DateTimeOffset? LastWorkingDate { get; set; }
            public double? serveperioud { get; set; }
        }

        /// <summary>
        /// Create by Ravi Vyas On 17-08-2022
        /// </summary>
        public class ExiteData
        {
            public int EmployeeId { get; set; }
            public int EmployeeExitId { get; set; }
            public string DisplayName { get; set; }
            public string ExitType { get; set; }
            public string DesignatioName { get; set; }
            public string Status { get; set; }
            public string LocalAddress { get; set; }
            public string Comment { get; set; }
            public DateTime? TerminateDate { get; set; }
            public bool InProgress { get; set; }
            public bool HRApproved { get; set; }
            public bool ITApproved { get; set; }
            public bool Settelment { get; set; }
            public bool Finalsettelment { get; set; }
            public string ExitStatus { get; set; }
            public string Summary { get; set; }

            //public DateTime? LastWorkingDate { get; set; }
        }

        public class ExitEmployeeGraphModalForMonth
        {
            public string name { get; set; }
            public int value { get; set; }
        }

        public class ExitGraph
        {
            public string Name { get; set; }
            public List<ExitEmployeeGraphModalForMonth> Series { get; set; }
        }

        public class GetExitEmployeeApproveModel
        {
            public string DesignatioName { get; set; }
            public int EmployeeExitId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public bool IsDiscussion { get; set; }
            public string DiscussionSummary { get; set; }
            public int ReasonId { get; set; }
            public string Reason { get; set; }
            public string Comment { get; set; }
            public ExitInitingType ExitType { get; set; }
            public ExitStatusConstants Status { get; set; }
            public DateTime? TerminateDate { get; set; }
            public DateTime? LastWorkingDate { get; set; }
            public bool IsReHired { get; set; }
            public bool IsAcceptRetainig { get; set; }
            public bool InProgress { get; set; }
        }

        public class ExitEmpDistingByAssest
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
            public bool Recovered { get; set; }
        }

        public class ExitEmpAssetdatamodel
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
            public bool Recovered { get; set; }
        }

        public class ExitEmpAssetlist
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Department { get; set; }
            public bool IsAllAssetsRecovered { get; set; }
            public List<ExitEmpAssetdatamodel> AssesdataList { get; set; }
        }

        public class GetExitEmployeeSettelmentModel
        {
            public string DesignatioName { get; set; }
            public int EmployeeExitId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public bool IsDiscussion { get; set; }
            public string DiscussionSummary { get; set; }
            public int ReasonId { get; set; }
            public string Reason { get; set; }
            public string Comment { get; set; }
            public ExitInitingType ExitType { get; set; }
            public ExitStatusConstants Status { get; set; }
            public DateTimeOffset Joingdate { get; set; }
            public DateTimeOffset? TerminateDate { get; set; }
            public DateTimeOffset? LastWorkingDate { get; set; }
            public bool IsReHired { get; set; }
            public bool IsAcceptRetainig { get; set; }
            public bool InProgress { get; set; }
            public int GoodAssets { get; set; }
            public int DamageAssets { get; set; }
            public double? WorkingDay { get; set; }
            public int? leaveTaken { get; set; }
            public double? Netpay { get; set; }
        }

        public class ExitEmployeeResignation
        {
            public int AllData { get; set; }
            public int EmployeeResignation { get; set; }
            public int HrTerminate { get; set; }
            public int PendingResignation { get; set; }
            public int ExitEmployee { get; set; }
            public List<ExitEmployeeGraphModalForMonth> ExitEmployeeGraphModalForMonth { get; set; }
            public List<ExitEmployeeGraphModalForMonth> RetainEmployeeGraphModalForMonth { get; set; }
            //public List<ExitGraph> ExitGraph { get; set; }
        }

        public class ExEmployeeRes
        {
            public string DesignatioName { get; set; }
            public string DepartmentName { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Gender { get; set; }
            public string DiscussionSummary { get; set; }
            public string Reason { get; set; }
            public string Comment { get; set; }
            public string ExitType { get; set; }
            public ExitStatusConstants Status { get; set; }
            public DateTimeOffset joiningDate { get; set; }
            public DateTimeOffset? LastWorkingDate { get; set; }
            public DateTimeOffset? TerminateDate { get; set; }
            public double? serveperioud { get; set; }
        }

        public class GetExEmployeeListModel
        {
            public string DesignatioName { get; set; }
            public string DepartmentName { get; set; }
            public int EmployeeExitId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public bool IsDiscussion { get; set; }
            public string DiscussionSummary { get; set; }
            public int ReasonId { get; set; }
            public string Reason { get; set; }
            public string Comment { get; set; }
            public string Department { get; set; }
            public string ExitType { get; set; }
            public ExitStatusConstants Status { get; set; }
            public DateTimeOffset Joingdate { get; set; }
            public DateTimeOffset? TerminateDate { get; set; }
            public DateTimeOffset LastWorkingDate { get; set; }
            public bool IsReHired { get; set; }
            public bool IsAcceptRetainig { get; set; }
            public bool InProgress { get; set; }
            public int GoodAssets { get; set; }
            public int DamageAssets { get; set; }
            public double? WorkingYears { get; set; }
            public int? LeaveTaken { get; set; }
            public double? Netpay { get; set; }
        }

        #endregion Helper Model
    }
}