using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs;
using AspNetIdentity.WebApi.Model.NewClientRequirement.TypeofWork;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls.Expressions;

namespace AspNetIdentity.WebApi.Controllers.ClientNewRequirement
{
    /// <summary>
    /// Created by Suraj Bundel on  24/02/2023 
    /// As per New Cllient Requirement
    /// </summary>
    [RoutePrefix("api/typeofwork")]
    [Authorize]
    public class TypeofWorkController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Add Work Type
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/typeofwork/addworktype
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addworktype")]
        public async Task<IHttpActionResult> Add(TypeofWorkRequestModel Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                if (Model == null)
                {
                    res.Message = "Model is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    //res.Data= tokendata;
                    return Ok(res);
                }
                else
                {

                    var duplicate = _db.TypeofWorks.Where(x => x.WorktypeName == Model.WorktypeName && !x.IsDeleted && x.IsActive && x.CompanyId == tokendata.companyId).ToList();
                    if (duplicate.Count > 0)
                    {
                        res.Message = "Duplicate Work Type found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.Ambiguous;
                        return Ok(res);
                    }
                    else
                    {
                        TypeofWork obj = new TypeofWork()
                        {
                            WorktypeName = Model.WorktypeName,
                            Description = Model.Description,
                            //DocumentName = Model.DocumentName,
                            //WorkTypeCode = Model.code,
                            CreatedBy = tokendata.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = tokendata.companyId,
                            OrgId = tokendata.orgId,
                        };
                        _db.TypeofWorks.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Work Type added Successfully";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = obj;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {


                logger.Error("api/typeofwork/addclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region get Work Type
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => GET => api/typeofwork/getworktype?page=&Count=&search&orderby
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getworktype")]
        public async Task<IHttpActionResult> Getall(int? page = null, int? count = null, string search = null, int? orderby = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var data = await _db.TypeofWorks.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId)
                    .Select(x => new
                    {
                        x.WorktypeId,
                        x.WorktypeName,
                        CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == x.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                        x.CreatedOn,
                        UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == x.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                        x.UpdatedOn,
                        Description = x.Description == "" ? null : x.Description,
                    }).OrderByDescending(x => x.CreatedOn).ToListAsync();

                var getall = data;
                switch (orderby)
                {
                    case 0:
                        getall = data;
                        break;
                    case 1:
                        getall = data.OrderBy(x => x.WorktypeName).ToList();
                        break;
                    case 2:
                        getall = data.OrderByDescending(x => x.WorktypeName).ToList();
                        break;
                    case 3:
                        getall = data.OrderBy(x => x.CreatedOn).ToList();
                        break;
                    case 4:
                        getall = data.OrderByDescending(x => x.CreatedOn).ToList();
                        break;
                }
                if (getall.Count > 0)
                {
                    res.Message = "Work type List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new
                        {
                            TotalData = getall.Count,
                            Counts = (int)count,
                            List = getall.Where(x => x.WorktypeName.ToUpper().Contains(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).OrderBy(x => x.WorktypeName).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = getall.Count,
                            Counts = (int)count,
                            List = getall.Skip(((int)page - 1) * (int)count).Take((int)count).OrderBy(x => x.WorktypeName).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = getall;
                    }
                    return Ok(res);
                }
                else
                {
                    res.Message = "Work type list not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getall;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {
                logger.Error("api/typeofwork/getclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Update Work Type
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/typeofwork/editworktype
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editworktype")]
        public async Task<IHttpActionResult> Update(UpdateTypeofWorkRequest Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                if (Model == null)
                {
                    res.Message = "Model is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    //res.Data= tokendata;
                    return Ok(res);
                }
                else
                {
                    var edit = _db.TypeofWorks.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.WorktypeId == Model.Id);
                    if (edit == null)
                    {
                        res.Message = "Work type not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = edit;
                        return Ok(res);
                    }
                    else
                    {
                        var check = await _db.TypeofWorks.FirstOrDefaultAsync(x => x.WorktypeName.ToUpper().Trim() == Model.WorktypeName.ToUpper().Trim() && x.CompanyId == tokendata.companyId);
                        if (check == null)
                        {
                            //    if (edit.WorktypeName == Model.WorktypeName)
                            //{
                            //    res.Message = "Duplicte Work type !";
                            //    res.Status = false;
                            //    res.StatusCode = HttpStatusCode.NoContent;
                            //    res.Data = edit;
                            //    return Ok(res); 
                            //}
                            edit.WorktypeName = Model.WorktypeName;
                            edit.Description = Model.Description;
                            edit.UpdatedBy = tokendata.employeeId;
                            edit.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                            _db.Entry(edit).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            edit.Description = Model.Description;
                            edit.UpdatedBy = tokendata.employeeId;
                            edit.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                            _db.Entry(edit).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }

                        res.Message = "Work Type data Updated Successfully";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = edit;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/typeofwork/editworktype", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region Delete Work Type
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/typeofwork/deleteworktype
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deleteworktype")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                //if (Model == null)
                //{
                //    res.Message = "Model is Empty";
                //    res.Status = false;
                //    res.StatusCode = HttpStatusCode.NoContent;
                //    //res.Data= tokendata;
                //    return Ok(res);
                //}
                //else
                //if
                //{
                var edit = _db.TypeofWorks.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.WorktypeId == id).FirstOrDefault();
                if (edit == null)
                {
                    res.Message = "Work type not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = edit;
                    return Ok(res);
                }
                else
                {
                    edit.IsActive = false;
                    edit.IsDeleted = true;
                    edit.DeletedBy = tokendata.employeeId;
                    edit.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                    _db.Entry(edit).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Work Type added Successfully";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = edit;
                    return Ok(res);
                }
                //}
            }
            catch (Exception ex)
            {
                logger.Error("api/typeofwork/deleteworktype", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region get Work Type by Id
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => GET => api/typeofwork/getworktypebyid?id=
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getworktypebyid")]
        public async Task<IHttpActionResult> GetbyId(Guid id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var getbyid = await _db.TypeofWorks.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.WorktypeId == id).FirstOrDefaultAsync();
                if (getbyid == null)
                {
                    res.Message = "Work type list not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getbyid;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Work type List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getbyid;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/typeofwork/getworktypebyid", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region Add Type of Work By Excel upload

        /// <summary>
        /// Created By Suraj Bundel on 27-02-2023
        /// API >> POST >> api/typeofwork/typeofworkeimport
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("typeofworkimport")]
        public async Task<IHttpActionResult> TypeofworkExcelImport(List<TypeofWorkImportFaultyLog> models)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            List<TypeofWorkImportFaultyLog> falultyImportItem = new List<TypeofWorkImportFaultyLog>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            long successfullImported = 0;
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (models.Count <= 0)
                {
                    res.Message = "Excel Not Have Any Data";
                    res.Status = false;
                    res.Data = falultyImportItem;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                else
                {
                    foreach (var item in models)
                    {
                        var checkworktypename = _db.TypeofWorks.Where(x => x.WorktypeName.Trim().ToUpper() == item.WorktypeName.Trim().ToUpper() && !x.IsDeleted && x.IsActive && x.CompanyId == tokendata.companyId).FirstOrDefault();
                        if (checkworktypename != null)
                        {
                            res.Message = "Duplicate Work type exist !";
                            res.Status = false;
                            res.Data = falultyImportItem;
                            res.StatusCode = HttpStatusCode.NoContent;
                            item.FailReason = "Employee Not Found Or Wrong Employee Offical E-Mail Inputed";
                            item.FaultyId = Guid.NewGuid();
                            falultyImportItem.Add(item);
                            continue;
                        }
                        else
                        {
                            TypeofWork obj = new TypeofWork();

                            obj.WorktypeName = item.WorktypeName.Trim();
                            obj.CreatedBy = tokendata.employeeId;
                            //  obj.WorkTypeCode = item.Code;
                            obj.Description = item.Description;
                            obj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
                            obj.IsActive = true;
                            obj.IsDeleted = false;
                            obj.CompanyId = tokendata.companyId;
                            obj.OrgId = tokendata.orgId;

                            _db.TypeofWorks.Add(obj);
                            await _db.SaveChangesAsync();

                            TypeofWorkHistory hisobj = new TypeofWorkHistory()
                            {
                                WorktypeId = obj.WorktypeId,
                                WorktypeName = obj.WorktypeName,
                                Description = obj.Description,
                                // WorkTypeCode = obj.WorkTypeCode,
                                IsActive = obj.IsActive,
                                IsDeleted = obj.IsDeleted,
                                CompanyId = obj.CompanyId,
                                OrgId = obj.OrgId,
                                IsDefaultCreated = obj.IsDefaultCreated,
                                CreatedBy = obj.CreatedBy,
                                UpdatedBy = obj.UpdatedBy,
                                DeletedBy = obj.DeletedBy,
                                CreatedOn = obj.CreatedOn,
                                UpdatedOn = obj.UpdatedOn,
                                DeletedOn = obj.DeletedOn,
                            };
                            _db.TypeofWorkHistories.Add(hisobj);
                            await _db.SaveChangesAsync();
                            successfullImported += 1;
                        }
                    }
                }

                if (falultyImportItem.Count > 0)
                {

                    TypeofWorkImportFaultyLogsGroup groupobj = new TypeofWorkImportFaultyLogsGroup
                    {
                        GroupId = Guid.NewGuid(),
                        TotalImported = models.Count,
                        SuccessFullImported = successfullImported,
                        UnSuccessFullImported = falultyImportItem.Count,
                        CreatedBy = tokendata.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = tokendata.companyId,
                        OrgId = tokendata.orgId,
                    };
                    _db.TypeofWorkImportFaultyLogsGroups.Add(groupobj);
                    await _db.SaveChangesAsync();

                    falultyImportItem.ForEach(x =>
                            {
                                x.Groups = groupobj;
                            });
                    _db.TypeofWorkImportFaultyLogs.AddRange(falultyImportItem);
                    await _db.SaveChangesAsync();

                    if ((models.Count - falultyImportItem.Count) > 0)
                    {
                        res.Message = "Type of work Imported Succesfull Of " +
                        (models.Count - falultyImportItem.Count) + " Fields And " +
                        falultyImportItem.Count + " Fields Are Not Imported";
                        res.Status = true;
                        res.Data = falultyImportItem;
                        res.StatusCode = HttpStatusCode.OK;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "All Fields Are Not Imported";
                        res.StatusCode = HttpStatusCode.OK;
                        res.Status = true;
                        res.Data = falultyImportItem;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "Data Added Successfully Of All Fields";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = falultyImportItem;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {
                logger.Error("api/typeofwork/typeofworkeimport", ex.Message);
                res.StatusCode = HttpStatusCode.BadRequest;
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        #endregion Add & Update Assets By Excel upload

        #region Add type of work to all company
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/typeofwork/addworktypecompany
        /// This is use to add type of work in all previously created company
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addworktypecompany")]
        public async Task<IHttpActionResult> AddCompany()
        {
            ResponseStatusCode res = new ResponseStatusCode();

            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
                {
                    var companylist = await _db.Company.Where(x => x.IsActive && !x.IsDeleted)/*.Select(X => X.CompanyId)*/.ToListAsync();
                    foreach (var CompanyItem in companylist)
                    {
                        var rest = AddDefaultWorkType(CompanyItem.CompanyId);
                    }
                    res.Message = "Data added to company";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/typeofwork/addclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region Helper Methoud
        public async Task AddDefaultWorkType(int CompanyId)
        {
            ApplicationDbContext _db = new ApplicationDbContext();
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var getstatic = Worktype.workobj();

                foreach (var item in getstatic)
                {
                    var duplication = _db.TypeofWorks.FirstOrDefault(x => x.WorktypeName == item.Worktype && x.CompanyId == CompanyId);
                    if (duplication == null)
                    {
                        TypeofWork obj = new TypeofWork()
                        {
                            WorktypeName = item.Worktype,
                            Description = item.Description,
                            //WorkTypeCode = item.WorkTypeCode,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = CompanyId,
                            IsDefaultCreated = true,
                            CreatedBy = 0,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                        };
                        _db.TypeofWorks.Add(obj);
                        _db.SaveChanges();
                        res.Message = "Default fields added successfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        res.Message = "failed to add Default fields !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
            }
        }
        #endregion

        #region Static WorkType List
        public static class Worktype
        {
            public static List<worktypeModel> workobj()
            {
                List<worktypeModel> workList = new List<worktypeModel>();
                worktypeModel worktype1 = new worktypeModel();
                worktype1.Worktype = "Work types";
                worktype1.Description = "Default Fields";

                worktypeModel worktype2 = new worktypeModel();
                worktype2.Worktype = "Income tax";
                worktype2.Description = "Default Fields code";

                worktypeModel worktype3 = new worktypeModel();
                worktype3.Worktype = "GST";
                worktype3.Description = "Default Fields code";

                worktypeModel worktype4 = new worktypeModel();
                worktype4.Worktype = "Audit";
                worktype4.Description = "Default Fields code";

                worktypeModel worktype5 = new worktypeModel();
                worktype5.Worktype = "RERA";
                worktype5.Description = "Default Fields code";

                worktypeModel worktype6 = new worktypeModel();
                worktype6.Worktype = "Company Laws";
                worktype6.Description = "Default Fields code";

                worktypeModel worktype7 = new worktypeModel();
                worktype7.Worktype = "Project Finance";
                worktype7.Description = "Default Fields code";

                worktypeModel worktype8 = new worktypeModel();
                worktype8.Worktype = "Other";
                worktype8.Description = "Default Fields code";

                workList.Add(worktype1);
                workList.Add(worktype2);
                workList.Add(worktype3);
                workList.Add(worktype4);
                workList.Add(worktype5);
                workList.Add(worktype6);
                workList.Add(worktype7);
                workList.Add(worktype8);

                return workList;
            }
        }
        #endregion

        public class worktypeModel
        {
            public string Id { get; set; }
            public string Worktype { get; set; }
            public string Description { get; set; }
            //public string WorkTypeCode { get; set; }
        }
        public class UpdateTypeofWorkRequest : TypeofWorkRequestModel
        {
            public Guid Id { get; set; } = Guid.Empty;
        }
        public class TypeofWorkRequestModel
        {
            public string WorktypeName { get; set; }
            public string Description { get; set; }
            //  public string code { get; set; }

        }

    }
}
