using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.NewClientRequirement;
using AspNetIdentity.WebApi.Model.NewClientRequirement.FaultyLogs;
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
using System.Web.UI.WebControls;

namespace AspNetIdentity.WebApi.Controllers.ClientNewRequirement
{
    /// <summary>
    /// Created by Suraj Bundel on  24/02/2023 
    /// As per New Cllient Requirement
    /// </summary>
    [RoutePrefix("api/newclient")]
    [Authorize]
    public class NewClientController : ApiController
    {

        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Add client
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/newclient/addclient
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addclient")]
        public async Task<IHttpActionResult> Add(AddrequestModel Model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            // List<AssignWorktype> list = new List<AssignWorktype>();
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
                    var duplicate = _db.NewClientModels.Where(x => x.ClientName == Model.ClientName && x.ClientCode == Model.ClientCode && !x.IsDeleted && x.IsActive && x.CompanyId == tokendata.companyId).ToList();
                    if (duplicate.Count > 0)
                    {
                        res.Message = "Duplicate Client Code found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.Ambiguous;
                        return Ok(res);
                    }
                    else
                    {
                        NewClientModel obj = new NewClientModel()
                        {
                            ClientName = Model.ClientName,
                            Description = Model.Description,
                            ClientCode = Model.ClientCode,
                            //   Worktype = JsonConvert.SerializeObject(Model.Worktype),
                            OfficialEmail = Model.OfficialEmail,
                            MobileNumber = Model.MobileNumber,
                            CreatedBy = tokendata.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = tokendata.companyId,
                            OrgId = tokendata.orgId,
                        };
                        _db.NewClientModels.Add(obj);
                        await _db.SaveChangesAsync();

                        //foreach (var item in Model.Worktype)
                        //{
                        //    AssignWorktype awtobj = new AssignWorktype()
                        //    {
                        //        clientId = obj.ClientId,
                        //        worktypeid = item,
                        //        CreatedBy = tokendata.employeeId,
                        //        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                        //        IsActive = true,
                        //        IsDeleted = false,
                        //        CompanyId = tokendata.companyId,
                        //        OrgId = tokendata.orgId,
                        //    };
                        //    list.Add(awtobj);
                        //}
                        //_db.AssignWorktypes.AddRange(list);
                        //_db.SaveChanges();




                        res.Message = "Client added Successfully";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = obj;

                        NewClientHistory hisobj = new NewClientHistory()
                        {
                            ClientId = obj.ClientId,
                            ClientName = obj.ClientName,
                            Description = obj.Description,
                            //  Worktype = obj.Worktype,
                            ClientCode = obj.ClientCode,
                            OfficialEmail = obj.OfficialEmail,
                            MobileNumber = obj.MobileNumber,
                            CreatedBy = obj.CreatedBy,
                            CreatedOn = obj.CreatedOn,
                            IsActive = obj.IsActive,
                            IsDeleted = obj.IsDeleted,
                            CompanyId = obj.CompanyId,
                            OrgId = obj.OrgId,

                        };
                        _db.NewClientHistories.Add(hisobj);
                        await _db.SaveChangesAsync();

                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclient/addclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region get client
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => GET => api/newclient/getclient
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getclient")]
        public async Task<IHttpActionResult> Getall(int? page = null, int? count = null, string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                //var getwork = _db.TypeofWorks.Select(x => new
                //{
                //    workid = x.WorktypeId,
                //    WorktypeName = x.WorktypeName
                //}).ToList();

                var getall = await (from nt in _db.NewClientModels
                                    where nt.CompanyId == tokendata.companyId && !nt.IsDeleted && nt.IsActive
                                    select new
                                    {
                                        nt.ClientId,
                                        nt.ClientName,
                                        Description = nt.Description == "" ? null : nt.Description,
                                        //     nt.Worktype,
                                        nt.ClientCode,
                                        OfficialEmail = nt.OfficialEmail == "" ? null : nt.OfficialEmail,
                                        MobileNumber = nt.MobileNumber == "" ? null : nt.MobileNumber,
                                        nt.IsActive,
                                        nt.IsDeleted,
                                        nt.CompanyId,
                                        nt.OrgId,
                                        nt.IsDefaultCreated,
                                        CreatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == nt.CreatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                        UpdatedBy = _db.Employee.Where(y => y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId && y.EmployeeId == nt.UpdatedBy).Select(y => y.DisplayName).FirstOrDefault(),
                                        nt.DeletedBy,
                                        nt.CreatedOn,
                                        nt.UpdatedOn,
                                        nt.DeletedOn
                                    }).OrderByDescending(x => x.CreatedOn).ToListAsync();
                //var joindata = await (from c in _db.NewClientModels
                //                      join a in _db.AssignWorktypes on c.ClientId equals a.clientId
                //                      //    join w in _db.TypeofWorks on a.worktypeid equals w.WorktypeId
                //                      where c.CompanyId == tokendata.companyId && !c.IsDeleted && c.IsActive
                //                      select new
                //                      {
                //                          c.ClientId,
                //                          c.ClientName,
                //                          c.Description,
                //                          //w.WorktypeName,
                //                          c.OfficialEmail,
                //                          c.MobileNumber,
                //                          c.IsActive,
                //                          c.IsDeleted,
                //                          c.CompanyId,
                //                          c.OrgId,
                //                          c.IsDefaultCreated,
                //                          c.CreatedBy,
                //                          c.UpdatedBy,
                //                          c.DeletedBy,
                //                          c.CreatedOn,
                //                          c.UpdatedOn,
                //                          c.DeletedOn
                //                      }).ToListAsync();

                //var query = await (from c in _db.NewClientModels
                //                   join a in _db.AssignWorktypes on c.ClientId equals a.clientId into gj
                //                   from s in gj.DefaultIfEmpty()
                //                   select new
                //                   {
                //                     //  s.worktypeid,
                //                       c.ClientId,
                //                       c.ClientName,
                //                       c.Description,
                //                       c.OfficialEmail,
                //                       c.MobileNumber,
                //                       c.IsActive,
                //                       c.IsDeleted,
                //                       c.CompanyId,
                //                       c.OrgId,
                //                       c.IsDefaultCreated,
                //                       c.CreatedBy,
                //                       c.UpdatedBy,
                //                       c.DeletedBy,
                //                       c.CreatedOn,
                //                       c.UpdatedOn,
                //                       c.DeletedOn,
                //                  //     WorktypeName = _db.TypeofWorks.Where(z =>  (s.worktypeid) .Contains((z.WorktypeId))).Select(z => z.WorktypeName),
                //                   //    WorktypeName = _db.TypeofWorks.Where(z =>  (s.worktypeid == z.WorktypeId)).Select(z => z.WorktypeName),

                //                   }).ToListAsync();
                //var gete = query.Distinct().ToList();
                //res.Message = "Client List";
                //res.Status = true;
                // res.StatusCode = HttpStatusCode.OK;
                //res.Data= getall;
                //return Ok(res);
                //var getall = await (from nt in _db.NewClientModels
                //                    where nt.CompanyId == tokendata.companyId && !nt.IsDeleted && nt.IsActive
                //                    select new
                //                    {
                //                        nt.ClientId,
                //                        nt.ClientName,
                //                        nt.Description,
                //                        nt.Worktype,
                //                        nt.OfficialEmail,
                //                        nt.MobileNumber,
                //                        nt.IsActive,
                //                        nt.IsDeleted,
                //                        nt.CompanyId,
                //                        nt.OrgId,
                //                        nt.IsDefaultCreated,
                //                        nt.CreatedBy,
                //                        nt.UpdatedBy,
                //                        nt.DeletedBy,
                //                        nt.CreatedOn,
                //                        nt.UpdatedOn,
                //                        nt.DeletedOn
                //                    }).ToListAsync();

                //var desc = getall.Select(x => new
                //{
                //    x.ClientId,
                //    x.ClientName,
                //    x.Description,
                //    WorktypeId = JsonConvert.DeserializeObject<List<Guid>>(x.Worktype),
                //    x.OfficialEmail,
                //    x.MobileNumber,
                //    x.IsActive,
                //    x.IsDeleted,
                //    x.CompanyId,
                //    x.OrgId,
                //    x.IsDefaultCreated,
                //    x.CreatedBy,
                //    x.UpdatedBy,
                //    x.DeletedBy,
                //    x.CreatedOn,
                //    x.UpdatedOn,
                //    x.DeletedOn
                //}).Select(x => new
                //{
                //    x.ClientId,
                //    x.ClientName,
                //    x.Description,
                //    x.OfficialEmail,
                //    x.MobileNumber,
                //    x.IsActive,
                //    x.IsDeleted,
                //    x.CompanyId,
                //    x.OrgId,
                //    x.IsDefaultCreated,
                //    x.CreatedBy,
                //    x.UpdatedBy,
                //    x.DeletedBy,
                //    x.CreatedOn,
                //    x.UpdatedOn,
                //    x.DeletedOn,
                //    WorktypeName = _db.TypeofWorks.Where(z => x.WorktypeId.Contains(z.WorktypeId)).Select(z => z.WorktypeName),
                //})
                //    .ToList();
                if (getall.Count > 0)
                {
                    res.Message = "Client List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                    {
                        var text = textInfo.ToUpper(search);
                        res.Data = new
                        {
                            TotalData = getall.Count,
                            Counts = (int)count,
                            List = getall.Where(x => x.ClientName.ToUpper().Contains(text))
                                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = getall.Count,
                            Counts = (int)count,
                            List = getall.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
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
                    res.Message = "Client list not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getall;
                    return Ok(res);

                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclient/getclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region Update client
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/newclient/editclient
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editclient")]
        public async Task<IHttpActionResult> Update(UpdateClientRequest Model)
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
                    var edit = _db.NewClientModels.Where(x => x.IsActive && !x.IsDeleted && x.ClientId == Model.Id && x.CompanyId == tokendata.companyId).FirstOrDefault();
                    if (edit == null)
                    {
                        res.Message = "Client not found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = edit;
                        return Ok(res);
                    }
                    else
                    {
                        var duplicate = _db.NewClientModels.FirstOrDefault(x => x.ClientCode.ToUpper().Trim() == Model.ClientCode.ToUpper().Trim() && x.CompanyId == tokendata.companyId);
                        if (duplicate == null)
                        {
                            edit.ClientName = Model.ClientName;
                            edit.ClientCode = Model.ClientCode;
                            edit.Description = Model.Description;
                            edit.OfficialEmail = Model.OfficialEmail;
                            edit.MobileNumber = Model.MobileNumber;
                            edit.UpdatedBy = tokendata.employeeId;
                            edit.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                            _db.Entry(edit).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }

                        edit.ClientName = Model.ClientName;
                        edit.Description = Model.Description;
                        edit.OfficialEmail = Model.OfficialEmail;
                        edit.MobileNumber = Model.MobileNumber;
                        edit.UpdatedBy = tokendata.employeeId;
                        edit.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);

                        _db.Entry(edit).State = EntityState.Modified;
                        await _db.SaveChangesAsync();



                        res.Message = "Client data Updated Successfully";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = edit;

                        NewClientHistory hisobj = new NewClientHistory()
                        {
                            ClientId = edit.ClientId,
                            ClientName = edit.ClientName,
                            Description = edit.Description,
                            ClientCode = edit.ClientCode,
                            //   Worktype = edit.Worktype,
                            OfficialEmail = edit.OfficialEmail,
                            MobileNumber = edit.MobileNumber,
                            CreatedBy = tokendata.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                            IsActive = edit.IsActive,
                            IsDeleted = edit.IsDeleted,
                            CompanyId = edit.CompanyId,
                            OrgId = edit.OrgId,
                            UpdatedBy = edit.UpdatedBy,
                            UpdatedOn = edit.UpdatedOn,

                        };
                        _db.NewClientHistories.Add(hisobj);
                        await _db.SaveChangesAsync();
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclient/editclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region Delete client
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => POST => api/newclient/deleteclient
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deleteclient")]
        public async Task<IHttpActionResult> Delete(UpdateClientRequest Model)
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
                    return Ok(res);
                }
                else
                {
                    var edit = _db.NewClientModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId && x.ClientId == Model.Id).FirstOrDefault();
                    if (edit == null)
                    {
                        res.Message = "Client not found !";
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

                        res.Message = "Client deleted Successfully";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = edit;

                        NewClientHistory hisobj = new NewClientHistory()
                        {
                            ClientId = Model.Id,
                            ClientName = edit.ClientName,
                            Description = edit.Description,
                            //   Worktype = edit.Worktype,
                            OfficialEmail = edit.OfficialEmail,
                            MobileNumber = edit.MobileNumber,
                            CreatedBy = tokendata.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone),
                            IsActive = edit.IsActive,
                            IsDeleted = edit.IsDeleted,
                            CompanyId = edit.CompanyId,
                            OrgId = edit.OrgId,
                            UpdatedBy = edit.UpdatedBy,
                            UpdatedOn = edit.UpdatedOn,
                            DeletedOn = edit.DeletedOn,
                            DeletedBy = edit.DeletedBy,
                        };
                        _db.NewClientHistories.Add(hisobj);
                        await _db.SaveChangesAsync();

                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclient/deleteclient", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }
        #endregion

        #region get Client by Id
        /// <summary>
        /// Created by Suraj Bundel on  24/02/2023
        /// API => GET => api/typeofwork/getworktypebyid
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getclientbyid")]
        public async Task<IHttpActionResult> GetbyId(Guid id)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var getwork = _db.TypeofWorks.Select(x => new
                {
                    workid = x.WorktypeId,
                    WorktypeName = x.WorktypeName
                }).ToList();

                var getall = await (from nt in _db.NewClientModels
                                    where nt.CompanyId == tokendata.companyId && !nt.IsDeleted && nt.IsActive && nt.ClientId == id
                                    select new
                                    {
                                        nt.ClientId,
                                        nt.ClientName,
                                        nt.Description,
                                        nt.ClientCode,
                                        //     nt.Worktype,
                                        nt.OfficialEmail,
                                        nt.MobileNumber,
                                        nt.IsActive,
                                        nt.IsDeleted,
                                        nt.CompanyId,
                                        nt.OrgId,
                                        nt.IsDefaultCreated,
                                        nt.CreatedBy,
                                        nt.UpdatedBy,
                                        nt.DeletedBy,
                                        nt.CreatedOn,
                                        nt.UpdatedOn,
                                        nt.DeletedOn
                                    }).FirstOrDefaultAsync();

                if (getall == null)
                {
                    res.Message = "Work type List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getall;
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
                logger.Error("api/newclient/getclientbyid", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest(ex.InnerException.ToString());
            }
        }

        #endregion

        #region Add & Update Client By Excel upload

        /// <summary>
        /// Created By Suraj Bundel on 01-03-2023
        /// API >> POST >> api/newclient/clientimport
        /// Model used >>AddItemMasterImport
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("clientimport")]
        public async Task<IHttpActionResult> ClientImport(List<NewclientImportFaultyLog> models)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            List<NewclientImportFaultyLog> falultyImportItem = new List<NewclientImportFaultyLog>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            long successfullImported = 0;
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (models.Count <= 0)
                {
                    res.Message = "Excel Not Have Any Data";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = falultyImportItem;
                    return Ok(res);
                }
                else
                {
                    foreach (var item in models)
                    {

                        var check = await _db.NewClientModels.FirstOrDefaultAsync(x => x.CompanyId == tokendata.companyId && x.ClientCode.ToUpper().Trim() == item.ClientCode.ToUpper().Trim());
                        if (String.IsNullOrEmpty(item.ClientName) && String.IsNullOrWhiteSpace(item.ClientName))
                        {
                            item.FailReason = "Client name should not be empty !";
                            falultyImportItem.Add(item);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(item.ClientCode) && String.IsNullOrWhiteSpace(item.ClientCode))
                            {
                                item.FailReason = "Client Code should not be empty !";
                                falultyImportItem.Add(item);
                            }
                            else
                            {
                                if (item.ClientName == null)
                                {
                                    item.FailReason = "Client name should not be empty !";
                                    falultyImportItem.Add(item);
                                }

                                if (check == null)
                                {
                                    NewClientModel obj = new NewClientModel();
                                    obj.IsActive = true;
                                    obj.IsDeleted = false;
                                    obj.CompanyId = tokendata.companyId;
                                    obj.OrgId = tokendata.orgId;
                                    obj.CreatedBy = tokendata.employeeId;
                                    obj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
                                    _db.NewClientModels.Add(obj);
                                    await _db.SaveChangesAsync();
                                    check = obj;
                                }
                                else
                                {
                                    //   obj.CreatedOn = item.CreatedOn;
                                    check.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
                                    check.UpdatedBy = tokendata.employeeId;

                                }
                                check.ClientName = item.ClientName.Trim();
                                check.Description = item.Description;
                                check.OfficialEmail = item.OfficialEmail;
                                check.ClientCode = item.ClientCode.Trim();
                                check.MobileNumber = item.MobileNumber;
                                _db.Entry(check).State = EntityState.Modified;
                                await _db.SaveChangesAsync();

                                NewClientHistory historyobj = new NewClientHistory();
                                historyobj.ClientId = check.ClientId;
                                historyobj.ClientName = check.ClientName;
                                historyobj.Description = check.Description;
                                historyobj.OfficialEmail = check.OfficialEmail;
                                historyobj.ClientCode = check.ClientCode;
                                historyobj.MobileNumber = check.MobileNumber;
                                historyobj.CreatedBy = tokendata.employeeId;
                                historyobj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokendata.TimeZone);
                                historyobj.IsActive = check.IsActive;
                                historyobj.IsDeleted = check.IsDeleted;
                                historyobj.CompanyId = check.CompanyId;
                                historyobj.OrgId = check.OrgId;
                                historyobj.UpdatedBy = check.UpdatedBy;
                                historyobj.DeletedBy = check.DeletedBy;
                                historyobj.CreatedOn = check.CreatedOn;
                                historyobj.UpdatedOn = check.UpdatedOn;
                                historyobj.DeletedOn = check.DeletedOn;

                                _db.NewClientHistories.Add(historyobj);
                                await _db.SaveChangesAsync();
                                successfullImported += 1;
                            }
                        }
                    }
                    if (falultyImportItem.Count > 0)
                    {
                        NewclientImportFaultyLogGroup groupobj = new NewclientImportFaultyLogGroup()
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
                        _db.NewclientImportFaultyLogGroups.Add(groupobj);
                        await _db.SaveChangesAsync();

                        falultyImportItem.ForEach(x =>
                        {
                            x.Groups = groupobj;
                        });
                        _db.NewclientImportFaultyLogs.AddRange(falultyImportItem);
                        await _db.SaveChangesAsync();

                        if ((models.Count - falultyImportItem.Count) > 0)
                        {
                            res.Message = "Data Imported Succesfull Of " +
                            (models.Count - falultyImportItem.Count) + " Fields And " +
                            falultyImportItem.Count + " Feilds Are Not Imported";
                            res.Status = true;
                            res.Data = falultyImportItem;
                            res.StatusCode = HttpStatusCode.OK;
                        }
                        else
                        {
                            res.Message = "Few Fields Are Not Imported";
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
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/newclient/clientimport", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }

        }
        #endregion

        public class UpdateClientRequest : AddrequestModel
        {
            public Guid Id { get; set; }
        }

        public class AddrequestModel
        {
            public string ClientName { get; set; }
            public string Description { get; set; }
            // public List<Guid> Worktype { get; set; }
            public string OfficialEmail { get; set; }
            public string MobileNumber { get; set; }
            public string ClientCode { get; set; } = string.Empty;

        }
    }
}