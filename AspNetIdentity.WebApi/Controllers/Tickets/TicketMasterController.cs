using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.TicketsModel;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.Asset.AssetsController;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Tickets
{
    /// <summary>
    /// Created By Harshit Mitra on 22-03-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/ticketmaster")]
    public class TicketMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        #region Api To Add Ticket Category With Authorize

        /// <summary>
        /// Api >> Post >> api/ticketmaster/createticketcategory
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createticketcategory")]
        public async Task<object> CreateTicketCategory(TicketCategory model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            AddTicketCategoryResponse response = new AddTicketCategoryResponse();
            List<TicketCategoryEmployee> emplList = new List<TicketCategoryEmployee>();
            List<TicketCategoryPriority> prioList = new List<TicketCategoryPriority>();
            try
            {
                if (model != null)
                {
                    if (!model.Priorities.Any(x => x.IsRequired))
                    {
                        res.Message = "Any One Priority Is Required";
                        res.Status = false;
                        res.Data = null;
                        return res;
                    }
                    TicketCategory obj = new TicketCategory
                    {
                        CategoryName = model.CategoryName,
                        Description = model.Description,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                    };
                    _db.TicketCategories.Add(obj);
                    await _db.SaveChangesAsync();
                    response.TicketCategory = obj;

                    if (model.Employees != null && model.Employees.Count > 0)
                    {
                        foreach (var item in model.Employees)
                        {
                            var i = Convert.ToInt32(item);
                            var emp = _db.Employee.FirstOrDefault(x => x.EmployeeId == i);
                            TicketCategoryEmployee emobj = new TicketCategoryEmployee
                            {
                                EmployeeId = emp.EmployeeId,
                                EmployeeName = emp.DisplayName,
                                TicketCategoryId = obj.TicketCategoryId,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                            };
                            _db.TicketCategoryEmployees.Add(emobj);
                            await _db.SaveChangesAsync();
                            emplList.Add(emobj);
                        }
                        response.Employees = emplList;
                    }
                    if (model.Priorities != null && model.Priorities.Count > 0)
                    {
                        foreach (var item in model.Priorities)
                        {
                            if (item.IsRequired)
                            {
                                TicketCategoryPriority probj = new TicketCategoryPriority
                                {
                                    PriorityType = item.PriorityType,
                                    PriorityName = Enum.GetName(typeof(PriorityTypeEnum), item.PriorityType).Replace("_", " "),
                                    TicketCategoryId = obj.TicketCategoryId,
                                    PriorityDescription = item.PriorityDescription,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                };
                                _db.TicPriorities.Add(probj);
                                await _db.SaveChangesAsync();
                                prioList.Add(probj);
                            }
                        }
                        response.Priorities = prioList;
                    }
                    res.Message = "Category Added";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "No Data";
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

        #endregion Api To Add Ticket Category

        #region Api To Add Ticket Category Without Authorize

        /// <summary>
        /// Api >> Post >> api/ticketmaster/createticketcategorys
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createticketcategorys")]
        [AllowAnonymous]
        public async Task<object> CreateTicketCategorys(TicketCategory model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            AddTicketCategoryResponse response = new AddTicketCategoryResponse();
            List<TicketCategoryEmployee> emplList = new List<TicketCategoryEmployee>();
            List<TicketCategoryPriority> prioList = new List<TicketCategoryPriority>();
            try
            {
                if (model != null)
                {
                    TicketCategory obj = new TicketCategory
                    {
                        CategoryName = model.CategoryName,
                        Description = model.Description,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                    };
                    _db.TicketCategories.Add(obj);
                    await _db.SaveChangesAsync();
                    response.TicketCategory = obj;

                    if (model.Employees != null && model.Employees.Count > 0)
                    {
                        foreach (var item in model.Employees)
                        {
                            var i = Convert.ToInt32(item);
                            var emp = _db.Employee.FirstOrDefault(x => x.EmployeeId == i);
                            TicketCategoryEmployee emobj = new TicketCategoryEmployee
                            {
                                EmployeeId = emp.EmployeeId,
                                EmployeeName = emp.DisplayName,
                                TicketCategoryId = obj.TicketCategoryId,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                            };
                            _db.TicketCategoryEmployees.Add(emobj);
                            await _db.SaveChangesAsync();
                            emplList.Add(emobj);
                        }
                        response.Employees = emplList;
                    }
                    if (model.Priorities != null && model.Priorities.Count > 0)
                    {
                        foreach (var item in model.Priorities)
                        {
                            if (item.IsRequired)
                            {
                                TicketCategoryPriority probj = new TicketCategoryPriority
                                {
                                    PriorityType = item.PriorityType,
                                    PriorityName = Enum.GetName(typeof(PriorityTypeEnum), item.PriorityType).Replace("_", " "),
                                    TicketCategoryId = obj.TicketCategoryId,
                                    PriorityDescription = item.PriorityDescription,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                };
                                _db.TicPriorities.Add(probj);
                                await _db.SaveChangesAsync();
                                prioList.Add(probj);
                            }
                        }
                        response.Priorities = prioList;
                    }
                    res.Message = "Category Added";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "No Data";
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

        #endregion Api To Add Ticket Category

        #region Get Ticket Category By Id With Authorize

        /// <summary>
        /// Api >> Get >> api/ticketmaster/getticketcategorybyid
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="ticketCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getticketcategorybyid")]
        public async Task<ResponseBodyModel> GetAllTicketPriorty(int ticketCategoryId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            TicketHelper obj1 = new TicketHelper();
            try
            {
                var ticketCategory = await _db.TicketCategories.FirstOrDefaultAsync(x => x.TicketCategoryId == ticketCategoryId &&
                             !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (ticketCategory != null)
                {
                    var name = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == ticketCategory.TicketCategoryId).Select(x => x.EmployeeId).ToList();
                    List<TicketCategoryPriority> tP = new List<TicketCategoryPriority>();
                    var priorities = _db.TicPriorities.Where(x => x.TicketCategoryId == ticketCategory.TicketCategoryId).ToList();
                    for (int i = 1; i < 6; i++)
                    {
                        var data = priorities.Where(x => x.PriorityType == i)
                                .Select(x => new TicketCategoryPriority
                                {
                                    PriorityType = x.PriorityType,
                                    IsRequired = true,
                                    PriorityDescription = x.PriorityDescription,
                                }).FirstOrDefault();
                        if (data == null)
                        {
                            TicketCategoryPriority obj = new TicketCategoryPriority
                            {
                                PriorityType = i,
                                IsRequired = false,
                                PriorityDescription = "",
                            };
                            tP.Add(obj);
                        }
                        else
                        {
                            data.IsRequired = true;
                            tP.Add(data);
                        }
                    }
                    obj1.TicketResponseModel = ticketCategory;
                    obj1.Employees = name;
                    obj1.Priorities = tP;

                    ticketCategory.Priorities = tP;
                    res.Message = "Ticket Category Found";
                    res.Status = true;
                    res.Data = obj1;
                }
                else
                {
                    res.Message = "Ticket Category Not Found";
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

        #endregion Get Ticket Category By Id

        #region Api To Edit Ticket Category

        /// <summary>
        /// Api >> Put >> api/ticketmaster/editticketcategory
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editticketcategory")]
        public async Task<ResponseBodyModel> EditTicket(TicketCategory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AddTicketCategoryResponse response = new AddTicketCategoryResponse();
            List<TicketCategoryEmployee> emplList = new List<TicketCategoryEmployee>();
            List<TicketCategoryPriority> prioList = new List<TicketCategoryPriority>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ticketCategory = await _db.TicketCategories.FirstOrDefaultAsync(x => x.TicketCategoryId == model.TicketCategoryId &&
                             !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (ticketCategory != null)
                {
                    if (!model.Priorities.Any(x => x.IsRequired))
                    {
                        res.Message = "Any One Priority Is Required";
                        res.Status = false;
                        res.Data = null;
                        return res;
                    }
                    ticketCategory.CategoryName = model.CategoryName;
                    ticketCategory.Description = model.Description;
                    ticketCategory.UpdatedBy = claims.employeeId;
                    ticketCategory.UpdatedOn = DateTime.Now;

                    _db.Entry(ticketCategory).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    response.TicketCategory = ticketCategory;

                    if (model.Employees != null && model.Employees.Count > 0)
                    {
                        var tickEmployee = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == ticketCategory.TicketCategoryId).ToList();
                        _db.TicketCategoryEmployees.RemoveRange(tickEmployee);
                        await _db.SaveChangesAsync();
                        foreach (var item in model.Employees)
                        {
                            var i = Convert.ToInt32(item);
                            var emp = _db.Employee.FirstOrDefault(x => x.EmployeeId == i);
                            TicketCategoryEmployee emobj = new TicketCategoryEmployee
                            {
                                EmployeeId = emp.EmployeeId,
                                EmployeeName = emp.DisplayName,
                                TicketCategoryId = ticketCategory.TicketCategoryId,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                            };
                            _db.TicketCategoryEmployees.Add(emobj);
                            await _db.SaveChangesAsync();
                            emplList.Add(emobj);
                        }
                        response.Employees = emplList;
                    }
                    if (model.Priorities != null && model.Priorities.Count > 0)
                    {
                        var tickPriorities = _db.TicPriorities.Where(x => x.TicketCategoryId == ticketCategory.TicketCategoryId).ToList();
                        _db.TicPriorities.RemoveRange(tickPriorities);
                        await _db.SaveChangesAsync();
                        foreach (var item in model.Priorities)
                        {
                            if (item.IsRequired)
                            {
                                TicketCategoryPriority probj = new TicketCategoryPriority
                                {
                                    PriorityType = item.PriorityType,
                                    PriorityName = Enum.GetName(typeof(PriorityTypeEnum), item.PriorityType).Replace("_", " "),
                                    TicketCategoryId = ticketCategory.TicketCategoryId,
                                    PriorityDescription = item.PriorityDescription,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                };
                                _db.TicPriorities.Add(probj);
                                await _db.SaveChangesAsync();
                                prioList.Add(probj);
                            }
                        }
                        response.Priorities = prioList;
                    }
                    res.Message = "Category Updated";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Ticket Category Not Found";
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

        #endregion Api To Edit Ticket Category

        #region Api For Get All TicketCategory

        /// <summary>
        /// api/ticketmaster/getallticketcategory
        /// Created On 16-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallticketcategory")]
        public async Task<ResponseBodyModel> GetAllTicketCategory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetTicketCategoryResponseModel> list = new List<GetTicketCategoryResponseModel>();
            TicketCategoryCount objData = new TicketCategoryCount();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ticketcategory = await _db.TicketCategories.Where(s => s.IsActive && !s.IsDeleted && s.CompanyId == claims.companyId).ToListAsync();
                if (ticketcategory.Count > 0)
                {
                    list = ticketcategory
                        .OrderByDescending(x => x.CreatedOn)
                        .Select(item => new GetTicketCategoryResponseModel
                        {
                            TicketCategoryId = item.TicketCategoryId,
                            TicketCategoryTitle = item.CategoryName,
                            TicketCategoryDetails = item.Description,
                            HelpingTeam = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == item.TicketCategoryId).Select(x => x.EmployeeName).ToList(),
                            IsMore = false
                        })
                        .ToList();
                    //foreach (var item in ticketcategory)
                    //{
                    //    GetTicketCategoryResponseModel obj = new GetTicketCategoryResponseModel
                    //    {
                    //        TicketCategoryId = item.TicketCategoryId,
                    //        TicketCategoryTitle = item.CategoryName,
                    //        TicketCategoryDetails = item.Description,
                    //        HelpingTeam = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == item.TicketCategoryId).Select(x => x.EmployeeName).ToList(),
                    //        IsMore = false
                    //    };
                    //    list.Add(obj);
                    //}

                    objData.GetTicketList = list;
                    objData.Count = list.Count();

                    res.Message = "Ticket Category List";
                    res.Status = true;
                    res.Data = objData;
                }
                else
                {
                    res.Message = "List Is Empty";
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

        #endregion Api For Get All TicketCategory

        #region Api For Get All TicketCategory 2

        /// <summary>
        /// api/ticketmaster/getallticketcategory2
        /// Created On 16-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallticketcategory2")]
        public async Task<ResponseBodyModel> GetAllTicketCategory2()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetTicketCategoryResponseModel2> list = new List<GetTicketCategoryResponseModel2>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ticketcategory = await _db.TicketCategories.Where(s => s.IsActive == true && s.IsDeleted == false && s.CompanyId == claims.companyId).ToListAsync();
                if (ticketcategory.Count > 0)
                {
                    foreach (var item in ticketcategory)
                    {
                        GetTicketCategoryResponseModel2 obj = new GetTicketCategoryResponseModel2
                        {
                            TicketCategoryId = item.TicketCategoryId,
                            TicketCategoryTitle = item.CategoryName,
                            TicketCategoryDetails = item.Description,
                            HelpingTeam = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == item.TicketCategoryId)
                                    .Select(x => new EmployeeHelpingTeam
                                    {
                                        EmployeeName = x.EmployeeName.Trim(),
                                    }).ToList(),
                        };
                        list.Add(obj);
                    }
                }
                if (list.Count > 0)
                {
                    res.Message = "Ticket Category List";
                    res.Status = true;
                    res.Data = list;
                }
                else
                {
                    res.Message = "List Is Empty";
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

        #endregion Api For Get All TicketCategory 2

        #region Api To Delete Ticket Category

        /// <summary>
        /// API >> Put >> api/ticketmaster/deleteticketcategory
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="ticketCategoryId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deleteticketcategory")]
        public async Task<ResponseBodyModel> DeleteTicketCategory(int ticketCategoryId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticketCategory = await _db.TicketCategories.FirstOrDefaultAsync(x => x.TicketCategoryId == ticketCategoryId &&
                                !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId);
                if (ticketCategory != null)
                {
                    ticketCategory.IsActive = false;
                    ticketCategory.IsDeleted = true;
                    ticketCategory.DeletedOn = DateTime.Now;
                    ticketCategory.DeletedBy = claims.companyId;

                    var ticEmp = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == ticketCategory.TicketCategoryId).ToList();
                    foreach (var item in ticEmp)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;
                        item.DeletedBy = claims.employeeId;
                        item.DeletedOn = DateTime.Now;

                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }

                    _db.Entry(ticketCategory).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Ticket Category Not Found";
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

        #endregion Api To Delete Ticket Category

        #region Api To Get Ticket Category List

        /// <summary>
        /// API >> Get >> api/ticketmaster/ticketcategorylist
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ticketcategorylist")]
        public async Task<ResponseBodyModel> GetTicketCategoryList()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticketCategeryList = await _db.TicketCategories.Where(x => !x.IsDeleted &&
                        x.IsActive && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.TicketCategoryId,
                            x.CategoryName,
                            x.Description,
                        }).ToListAsync();
                if (ticketCategeryList.Count > 0)
                {
                    res.Message = "Ticket Category List";
                    res.Status = true;
                    res.Data = ticketCategeryList;
                }
                else
                {
                    res.Message = "Ticket Category List is Empty";
                    res.Status = false;
                    res.Data = ticketCategeryList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Ticket Category List

        #region Api To Get Ticket Priority List By Ticket Category Id

        /// <summary>
        /// API >> Get >> api/ticketmaster/gettprioritybytCategory
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="ticketCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettprioritybytCategory")]
        public async Task<ResponseBodyModel> GetTicketPriorityByTicketCategory(int ticketCategoryId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var proirityList = await _db.TicPriorities.Where(x => x.IsActive && !x.IsDeleted &&
                           x.CompanyId == claims.companyId && x.TicketCategoryId == ticketCategoryId)
                            .Select(x => new
                            {
                                x.TicPriorityId,
                                x.PriorityName,
                                x.PriorityDescription,
                            }).ToListAsync();
                if (proirityList.Count > 0)
                {
                    res.Message = "Ticket Priority List";
                    res.Status = true;
                    res.Data = proirityList;
                }
                else
                {
                    res.Message = "Priority List Is Empty";
                    res.Status = false;
                    res.Data = proirityList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Ticket Priority List By Ticket Category Id

        #region Api To Add Ticket By User

        /// <summary>
        /// API >> Put >> api/ticketmaster/adduserticket
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adduserticket")]
        public async Task<ResponseBodyModel> AddUserTickets(AddTicket model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Imvalid";
                    res.Status = false;
                }
                else
                {
                    var ticketCategory = await _db.TicketCategories.FirstOrDefaultAsync(x => x.TicketCategoryId == model.TicketCategoryId && x.IsActive && !x.IsDeleted);
                    if (ticketCategory != null)
                    {
                        if (!String.IsNullOrEmpty(model.Title) && !String.IsNullOrEmpty(model.Title))
                        {
                            Ticket obj = new Ticket();
                            obj.TicketCategoryId = ticketCategory.TicketCategoryId;
                            obj.CategoryName = ticketCategory.CategoryName;
                            obj.Title = model.Title;
                            obj.AssignedToId = 0;
                            obj.TicketStatus = (int)TicketStatus.Pending;
                            if (model.TicketPriorityId != 0)
                            {
                                var ticketPriority = _db.TicPriorities.FirstOrDefault(x => x.TicketCategoryId == ticketCategory.TicketCategoryId && x.TicPriorityId == model.TicketPriorityId);
                                if (ticketPriority != null)
                                {
                                    obj.TicketPriorityId = ticketPriority.TicPriorityId;
                                    obj.PriorityType = ticketPriority.PriorityType;
                                    obj.PriorityName = ticketPriority.PriorityName;
                                }
                                else
                                {
                                    res.Message = "Ticket Proiority Not Valid";
                                    res.Status = false;
                                    return res;
                                }
                            }
                            else
                            {
                                res.Message = "Ticket Proiority Not Valid";
                                res.Status = false;
                                return res;
                            }
                            obj.IsActive = true;
                            obj.IsDeleted = false;
                            obj.CreatedBy = claims.employeeId;
                            obj.CreatedOn = DateTime.Now;
                            obj.CompanyId = claims.companyId;
                            obj.OrgId = claims.orgId;

                            _db.Tickets.Add(obj);
                            await _db.SaveChangesAsync();

                            TicketComment comObj = new TicketComment();
                            comObj.TicketId = obj.TicketId;
                            comObj.CommentOn = DateTime.Now.ToShortDateString();
                            comObj.CommentBy = claims.displayName;
                            comObj.Message = model.Message;
                            comObj.Image1 = model.Image1;
                            comObj.Image2 = model.Image2;
                            comObj.Image3 = model.Image3;
                            comObj.Image4 = model.Image4;
                            comObj.Image5 = model.Image5;
                            comObj.IsActive = true;
                            comObj.IsDeleted = false;
                            comObj.CreatedBy = claims.employeeId;
                            comObj.CreatedOn = DateTime.Now;
                            comObj.CompanyId = claims.companyId;
                            comObj.OrgId = claims.orgId;

                            _db.TicketComments.Add(comObj);
                            await _db.SaveChangesAsync();

                            HostingEnvironment.QueueBackgroundWorkItem(ct => BackGroundTask(obj, claims));

                            res.Message = "Ticket Created";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            res.Message = "Title And Details Are Required";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "Ticket Category Not Found";
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
        public void BackGroundTask(Ticket obj, ClaimsHelperModel claims)
        {
            Thread.Sleep(1000); // 1000 = 1 sec
            SendMail(obj, claims);
        }
        #endregion Api To Add Ticket By User

        #region Api To Get Ticket Detail Page

        /// <summary>
        /// API >> Get >> api/ticketmaster/ticketdetails
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ticketdetails")]
        [Authorize]
        public async Task<ResponseBodyModel> GetTicketDetailsById(int ticketId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            UserBlock EmployeeBlock = new UserBlock();
            GetTicketDetailResponse response = new GetTicketDetailResponse();
            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId && x.CompanyId == claims.companyId
                            && !x.IsDeleted && x.IsActive == true);
                if (ticket != null)
                {
                    response.TicketId = ticket.TicketId;
                    response.TicketPriorityId = ticket.TicketPriorityId;
                    response.PriorityType = ticket.PriorityType;
                    response.PriorityName = ticket.PriorityName;
                    response.Title = ticket.Title;
                    response.TicketStatus = ticket.TicketStatus;
                    response.StatusName = Enum.GetName(typeof(TicketStatus), ticket.TicketStatus);
                    response.TicketCategoryId = ticket.TicketCategoryId;
                    response.CategoryName = ticket.CategoryName;
                    response.AssignedToId = ticket.AssignedToId;
                    response.AssignedToName = ticket.AssignedToId == 0 ? "Not Assigned Yet!" : ticket.AssignedToName;
                    var employee = _db.Employee.FirstOrDefault(x => x.EmployeeId == ticket.CreatedBy);
                    EmployeeBlock.EmployeeId = employee.EmployeeId;
                    EmployeeBlock.EmployeeName = employee.FirstName + " " + employee.LastName;
                    EmployeeBlock.DesignationId = employee.DesignationId;
                    EmployeeBlock.Designation = _db.Designation.Where(x => x.DesignationId == EmployeeBlock.DesignationId).Select(x => x.DesignationName).FirstOrDefault();
                    EmployeeBlock.ContactNumber = employee.WorkPhone;
                    EmployeeBlock.Location = employee.LocalAddress;
                    response.EmployeeBlock = EmployeeBlock;

                    var commentList = _db.TicketComments.Where(x => x.IsActive && !x.IsDeleted &&
                                x.TicketId == ticket.TicketId).Select(x => new
                                {
                                    CreateOn = x.CreatedOn,
                                    x.CommentBy,
                                    x.CommentOn,
                                    x.Message,
                                    CommentType = x.CreatedBy != claims.employeeId ? "RECIVER" : "SENDER",
                                    HasImage = (!String.IsNullOrEmpty(x.Image1) ||
                                            !String.IsNullOrEmpty(x.Image2) ||
                                            !String.IsNullOrEmpty(x.Image3) ||
                                            !String.IsNullOrEmpty(x.Image4) ||
                                            !String.IsNullOrEmpty(x.Image5)),
                                    ImageArray = new List<string>
                                    { x.Image1, x.Image2, x.Image3, x.Image4, x.Image5 },

                                }).ToList().OrderBy(x => x.CreateOn).ToList();
                    response.CommentList = commentList;

                    res.Message = "Ticket Details";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Ticket Not Found";
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

        #endregion Api To Get Ticket Detail Page

        #region Api To Get Ticket List Created By User (My Tickets)

        /// <summary>
        /// API >> Get >> api/ticketmaster/mytickets
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mytickets")]
        public async Task<ResponseBodyModel> GetMyTickets()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetMyTicketPage response = new GetMyTicketPage
            {
                OpenTicket = new List<OpenTicketMyTicket>(),
                CloseTicket = new List<CloseTicketMyTicket>(),
            };
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticketList = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted &&
                            x.CreatedBy == claims.employeeId /*&& x.CompanyId == claims.companyid && x.OrgId == x.OrgId*/).ToListAsync();
                if (ticketList.Count > 0)
                {
                    var openTicket = ticketList.Where(x => x.TicketStatus != (int)TicketStatus.Completed)
                        .Select(x => new OpenTicketMyTicket
                        {
                            TicketId = x.TicketId,
                            TicketNumber = "#" + x.TicketId,
                            PriorityType = x.PriorityType,
                            PriorityName = x.PriorityName,
                            Title = x.Title,
                            CreatedOn = x.CreatedOn,
                            CategoryName = x.CategoryName,
                            AssignedTo = x.AssignedToId == 0 ? "Not Assigned Yet!" : x.AssignedToName,
                            TicketStatus = Enum.GetName(typeof(TicketStatus), x.TicketStatus),
                            LastUpdated = x.UpdatedOn.HasValue ? (DateTime)x.UpdatedOn : x.CreatedOn,
                        }).ToList();

                    var closeTicket = ticketList.Where(x => x.TicketStatus == (int)TicketStatus.Completed)
                        .Select(x => new CloseTicketMyTicket
                        {
                            TicketId = x.TicketId,
                            TicketNumber = "#" + x.TicketId,
                            Title = x.Title,
                            CreatedOn = x.CreatedOn,
                            CategoryName = x.CategoryName,
                            ClosedBy = x.AssignedToName,
                            ClosedOn = x.TicketClosedOn,
                        }).ToList();

                    response.OpenTicket = openTicket;
                    response.CloseTicket = closeTicket;

                    res.Message = "Ticket List";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Ticket List Is Empty";
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

        #endregion Api To Get Ticket List Created By User (My Tickets)

        #region Api To Get openTicket (Following Ticket)

        /// <summary>
        /// API >> Get >> api/ticketmaster/openticket
        /// Created By Ravi Vyas Add Peginatton on 04-08-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("openticket")]
        public async Task<ResponseBodyModel> GetTicketOnFolloings(int Count, int Page)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var ticketCategory = await _db.TicketCategoryEmployees.Where(x => x.EmployeeId == claims.employeeId && x.IsActive && !x.IsDeleted).Select(x => x.TicketCategoryId).ToListAsync();
                if (ticketCategory.Count > 0)
                {
                    var ticketList = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && ticketCategory.Contains(x.TicketCategoryId)
                               /* x.TicketCategoryId == ticketCategory */&& x.CompanyId == claims.companyId && x.OrgId == x.OrgId).ToListAsync();
                    if (ticketList.Count > 0)
                    {
                        var openTicket = ticketList.Where(x => x.TicketStatus == ((int)TicketStatus.Pending) || x.TicketStatus == ((int)TicketStatus.Progress))
                            .Select(x => new OpenTicketMyTicket
                            {
                                TicketId = x.TicketId,
                                TicketNumber = "#" + x.TicketId,
                                PriorityType = x.PriorityType,
                                PriorityName = x.PriorityName,
                                Title = x.Title,
                                CreatedOn = x.CreatedOn,
                                CategoryName = x.CategoryName,
                                AssignedTo = x.AssignedToId == 0 ? "Not Assigned Yet!" : x.AssignedToName,
                                TicketStatus = Enum.GetName(typeof(TicketStatus), x.TicketStatus),
                                LastUpdated = x.UpdatedOn.HasValue ? (DateTime)x.UpdatedOn : x.CreatedOn,
                            }).ToList();

                        res.Message = "Ticket List";
                        res.Status = true;
                        res.Data = new PaginationData
                        {
                            TotalData = openTicket.Count,
                            Counts = Count,
                            List = openTicket.Skip((Page - 1) * Count).Take(Count).ToList(),
                        };
                    }
                    else
                    {
                        res.Message = "Ticket List Is Empty";
                        res.Status = false;
                        res.Data = null;
                    }
                }
                else
                {
                    res.Message = "Ticket Category Not Found";
                    res.Status = false;
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

        #endregion Api To Get openTicket (Following Ticket)

        #region Api To Get Ticket On Assigne Side (Following Ticket)

        /// <summary>
        /// API >> Get >> api/ticketmaster/followingticket
        /// Created By Harshit Mitra on 01-04-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("followingticket")]
        public async Task<ResponseBodyModel> GetTicketOnFolloings()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetMyTicketPage response = new GetMyTicketPage
            {
                OpenTicket = new List<OpenTicketMyTicket>(),
                CloseTicket = new List<CloseTicketMyTicket>(),
            };
            try
            {
                var ticketCategory = _db.TicketCategoryEmployees.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.TicketCategoryId).FirstOrDefault();
                if (ticketCategory != 0)
                {
                    var ticketList = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted &&
                                x.TicketCategoryId == ticketCategory && x.CompanyId == claims.companyId && x.OrgId == x.OrgId).ToListAsync();
                    if (ticketList.Count > 0)
                    {
                        var openTicket = ticketList
                            .Where(x => x.TicketStatus != (int)TicketStatus.Completed)
                            .Select(x => new OpenTicketMyTicket
                            {
                                TicketId = x.TicketId,
                                TicketNumber = "#" + x.TicketId,
                                PriorityType = x.PriorityType,
                                PriorityName = x.PriorityName,
                                Title = x.Title,
                                CreatedOn = x.CreatedOn,
                                CategoryName = x.CategoryName,
                                AssignedTo = x.AssignedToId == 0 ? "Not Assigned Yet!" : x.AssignedToName,
                                TicketStatus = Enum.GetName(typeof(TicketStatus), x.TicketStatus),
                                LastUpdated = x.UpdatedOn.HasValue ? (DateTime)x.UpdatedOn : x.CreatedOn,
                            })
                            .ToList();

                        var closeTicket = ticketList
                            .Where(x => x.TicketStatus == (int)TicketStatus.Completed)
                            .Select(x => new CloseTicketMyTicket
                            {
                                TicketId = x.TicketId,
                                TicketNumber = "#" + x.TicketId,
                                Title = x.Title,
                                CreatedOn = x.CreatedOn,
                                CategoryName = x.CategoryName,
                                ClosedBy = x.AssignedToName,
                                ClosedOn = x.TicketClosedOn,
                            })
                            .ToList();

                        response.OpenTicket = openTicket;
                        response.CloseTicket = closeTicket;

                        res.Message = "Ticket List";
                        res.Status = true;
                        res.Data = response;
                    }
                    else
                    {
                        res.Message = "Ticket List Is Empty";
                        res.Status = false;
                        res.Data = response;
                    }
                }
                else
                {
                    res.Message = "Ticket Category Not Found";
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

        #endregion Api To Get Ticket On Assigne Side (Following Ticket)

        #region Api To Start Working on Ticket

        /// <summary>
        /// API >> Get >> api/ticketmaster/changeticketstatus
        /// Created By Harshit Mitra on 24-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("startticket")]
        public async Task<ResponseBodyModel> StartTicket(int ticketId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId && x.IsActive &&
                                 !x.IsDeleted && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (ticket != null)
                {
                    ticket.TicketStatus = (int)TicketStatus.Progress;
                    ticket.AssignedToId = claims.employeeId;
                    ticket.AssignedToName = claims.displayName;
                    ticket.UpdatedBy = claims.employeeId;
                    ticket.UpdatedOn = DateTime.Now;

                    _db.Entry(ticket).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Ticket Assign To " + claims.displayName;
                    res.Status = true;
                    res.Data = ticket;
                }
                else
                {
                    res.Message = "Ticket Not Found";
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

        #endregion Api To Start Working on Ticket

        #region Api Tp Get Close Ticket

        /// <summary>
        /// API >> Get >> api/ticketmaster/closeTicket
        /// Created By Ravi Vyas on 04-08-2022
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("closeTicket")]
        public async Task<ResponseBodyModel> GetCloseTicket(int Count, int Page)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var ticketCategory = _db.TicketCategoryEmployees.Where(x => x.EmployeeId == claims.employeeId && x.IsActive && !x.IsDeleted).Select(x => x.TicketCategoryId).ToList();
                if (ticketCategory.Count > 0)
                {
                    var ticketList = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && ticketCategory.Contains(x.TicketCategoryId)
                              /* x.TicketCategoryId == ticketCategory*/ && x.CompanyId == claims.companyId && x.OrgId == x.OrgId).ToListAsync();

                    if (ticketList.Count > 0)
                    {
                        var closeTicket = ticketList.Where(x => x.TicketStatus == (int)TicketStatus.Completed)
                           .Select(x => new CloseTicketMyTicket
                           {
                               TicketId = x.TicketId,
                               TicketNumber = "#" + x.TicketId,
                               Title = x.Title,
                               CreatedOn = x.CreatedOn,
                               CategoryName = x.CategoryName,
                               ClosedBy = x.AssignedToName,
                               ClosedOn = x.TicketClosedOn,
                           }).ToList().OrderByDescending(x => x.ClosedOn).ToList();

                        res.Message = "Ticket List";
                        res.Status = true;
                        res.Data = new PaginationData
                        {
                            TotalData = closeTicket.Count,
                            Counts = Count,
                            List = closeTicket.Skip((Page - 1) * Count).Take(Count).ToList(),
                        };
                    }
                    else
                    {
                        res.Message = "Ticket List Is Empty";
                        res.Status = false;
                        res.Data = new PaginationData
                        {
                            TotalData = 0,
                            Counts = Count,
                            List = new List<int>(),
                        };
                    }
                }
                else
                {
                    res.Message = "Ticket Category Not Found";
                    res.Status = false;
                    res.Data = new PaginationData
                    {
                        TotalData = 0,
                        Counts = Count,
                        List = new List<int>(),
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

        #endregion Api Tp Get Close Ticket

        #region Api To End Ticket

        /// <summary>
        /// API >> Get >> api/ticketmaster/closeticket
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("closeticket")]
        public async Task<ResponseBodyModel> CloseTicket(int ticketId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId && x.IsActive &&
                                 !x.IsDeleted && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (ticket != null)
                {
                    ticket.TicketStatus = (int)TicketStatus.Completed;
                    ticket.TicketClosedOn = DateTime.Now;
                    ticket.UpdatedBy = claims.userId;
                    ticket.UpdatedOn = DateTime.Now;
                    _db.Entry(ticket).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Ticket Closed By " + claims.displayName;
                    res.Status = true;
                    res.Data = ticket;
                }
                else
                {
                    res.Message = "Ticket Not Found";
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

        #endregion Api To End Ticket

        #region Api To Add Chat In Ticket

        /// <summary>
        /// API >> Get >> api/ticketmaster/addcommentonticket
        /// Created By Harshit Mitra on 24-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcommentonticket")]
        public async Task<ResponseBodyModel> AddCommentOnTicket(TicketComment model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.TicketId == model.TicketId
                    && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                    if (ticket != null)
                    {
                        TicketComment obj = new TicketComment();
                        obj.TicketId = ticket.TicketId;
                        obj.Message = model.Message;
                        obj.Image1 = model.Image1;
                        obj.Image2 = model.Image2;
                        obj.Image3 = model.Image3;
                        obj.Image4 = model.Image4;
                        obj.Image5 = model.Image5;
                        obj.CommentBy = claims.displayName;
                        obj.CommentOn = DateTime.Now.ToString("MMM dd yyyy,h:mm:ss");
                        obj.IsActive = true;
                        obj.IsDeleted = false;
                        obj.CreatedBy = claims.employeeId;
                        obj.CreatedOn = DateTime.Now;
                        obj.CompanyId = claims.companyId;
                        obj.OrgId = claims.orgId;

                        _db.TicketComments.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Comment Saved";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Ticket Not Found";
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

        #endregion Api To Add Chat In Ticket

        #region Api To Edit Ticket

        /// <summary>
        /// Api >> Get >> api/ticketmaster/editticket
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editticket")]
        public async Task<ResponseBodyModel> Editticket(Ticket model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.TicketId == model.TicketId);
                if (ticket != null)
                {
                    var ticPriority = _db.TicPriorities.FirstOrDefault(x => x.TicPriorityId == model.TicketPriorityId);
                    if (ticPriority != null)
                    {
                        ticket.AssignedToId = model.AssignedToId;
                        ticket.AssignedToName = _db.Employee.Where(x => x.EmployeeId == model.AssignedToId).Select(x => x.DisplayName).FirstOrDefault();
                        ticket.TicketCategoryId = model.TicketCategoryId;
                        ticket.CategoryName = _db.TicketCategories.Where(x => x.TicketCategoryId == model.TicketCategoryId).Select(x => x.CategoryName).FirstOrDefault();
                        ticket.TicketStatus = model.TicketStatus;
                        ticket.PriorityType = model.PriorityType;
                        var check = Enum.GetName(typeof(TicketStatus), model.TicketStatus);
                        if (check == TicketStatus.Completed.ToString())
                            ticket.TicketClosedOn = DateTime.Now;
                        ticket.TicketPriorityId = ticPriority.TicPriorityId;
                        ticket.PriorityName = ticPriority.PriorityName;
                        ticket.PriorityType = ticPriority.PriorityType;
                        ticket.UpdatedOn = DateTime.Now;
                        ticket.UpdatedBy = claims.employeeId;

                        _db.Entry(ticket).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Ticket  Updated";
                        res.Status = true;
                        res.Data = ticket;

                        HostingEnvironment.QueueBackgroundWorkItem(ct => UpdateTicketBackgroundCheck(ticket, claims.companyName, claims));

                        //if (ticket.AssignedToId != 0 && ticket.TicketStatus != (int)TicketStatus.Completed)
                        //    SendAssignMail(ticket);

                        //if (ticket.TicketStatus != 1)
                        //{
                        //    SendTicketStatusMailToEmployee(ticket, claims.companyName, ticket.AssignedToId != 0 ? 1 : 2);
                        //}
                    }
                    else
                    {
                        res.Message = "Priority Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Ticket  Not Found";
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
        public void UpdateTicketBackgroundCheck(Ticket ticket, string companyName, ClaimsHelperModel claims)
        {
            Thread.Sleep(1000); // 1000 = 1 sec;
            {
                if (ticket.AssignedToId != 0 && ticket.TicketStatus != (int)TicketStatus.Completed)
                    SendAssignMail(ticket, claims);
                if (ticket.TicketStatus != 1)
                {
                    SendTicketStatusMailToEmployee(ticket, companyName, ticket.AssignedToId != 0 ? 1 : 2, claims);
                }
            }
        }
        #endregion Api To Edit Ticket

        #region Get Ticket Category Employee By Id

        /// <summary>
        /// Api >> Get >> api/ticketmaster/getempbytcid
        /// </summary>
        /// <param name="ticketCategoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getempbytcid")]
        public async Task<ResponseBodyModel> getempbytcid(int ticketCategoryId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticketCategory = await _db.TicketCategoryEmployees.Where(x => !x.IsDeleted && x.IsActive &&
                        x.TicketCategoryId == ticketCategoryId && x.CompanyId == claims.companyId).ToListAsync();

                if (ticketCategory != null)
                {
                    res.Message = "Ticket Category Employees Found";
                    res.Status = true;
                    res.Data = ticketCategory;
                }
                else
                {
                    res.Message = "Ticket Category Employees Not Found";
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

        #endregion Get Ticket Category Employee By Id

        #region This Api Use For Get The EmployeeTicket List

        /// <summary>
        /// Api >> Get >> api/ticketmaster/getemplist
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getemplist")]
        public async Task<ResponseBodyModel> GetEmployeeListOnTicketCategory()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ticketCategoryEmp = await _db.TicketCategoryEmployees.Where(x => !x.IsDeleted && x.IsActive == true
                && x.CompanyId == claims.companyId /*&& x.OrgId == claims.orgid*/).Select(x => x.EmployeeId).Distinct().ToListAsync();
                if (ticketCategoryEmp.Count > 0)
                {
                    var emplList = _db.Employee.Where(x => !ticketCategoryEmp.Contains(x.EmployeeId) && x.IsActive &&
                         !x.IsDeleted && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName
                        }).ToList();
                    if (emplList.Count > 0)
                    {
                        res.Message = "Ticket Category Employees List Found";
                        res.Status = true;
                        res.Data = emplList;
                    }
                    else
                    {
                        res.Message = "Ticket Category Employees Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    var emplList = _db.Employee.Where(x => x.IsActive &&
                         !x.IsDeleted && x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName
                        }).ToList();
                    if (emplList.Count > 0)
                    {
                        res.Message = "Ticket Category Employees List Found";
                        res.Status = true;
                        res.Data = emplList;
                    }
                    else
                    {
                        res.Message = "Ticket Category Employees Not Found";
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

        #endregion This Api Use For Get The EmployeeTicket List

        #region This Api use By Ticket Master Dashboard

        /// <summary>
        /// API >> Get >> api/ticketmaster/geticketcount
        /// Created by shriya
        /// Created on 25-04-2022
        /// Some Changes by Ankit
        /// </summary>
        [HttpGet]
        [Route("geticketcount")]
        public async Task<ResponseBodyModel> GetTicketCount()
        {
            TicketCountRes response = new TicketCountRes();
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<TicketGraphModalForMonth> tickGraph = new List<TicketGraphModalForMonth>();

            try
            {
                DataHelperResponse dateHelperDTO = new DataHelperResponse();
                DateTime currentDate = DateTime.Now.Date;
                dateHelperDTO.startDate = currentDate.AddDays(-30).Date;
                dateHelperDTO.endDate = currentDate;
                var date = DateTime.Now.Date;
                //var currentYear = DateTime.Now.Year;

                //var totalTicket = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId &&
                //                x.CreatedOn.Year == currentYear).ToListAsync();
                var totalTicket = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                /*&& x.CreatedOn.Year == currentYear*/).ToListAsync();

                var ticketCategory = (from TC in _db.TicketCategories
                                      where TC.CompanyId == claims.companyId
                                      select new
                                      {
                                          TC.TicketCategoryId,
                                          TC.CategoryName,
                                          TC.IsActive,
                                          TC.IsDeleted,
                                      }).ToList();

                if (totalTicket.Count > 0)
                {
                    #region genral count base's on status and time

                    //today create ticket
                    var todayCount = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date).Count();

                    response.TodayTicketCount = todayCount;

                    var TicketClosedOn = date.Date;
                    var closeCountToday = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketStatus == 3 && x.TicketClosedOn.HasValue).ToList();
                    var countCloseTicket = 0;
                    foreach (var item in closeCountToday)
                    {
                        var checkDate = (DateTime)item.TicketClosedOn;
                        checkDate = checkDate.Date;
                        if (checkDate == DateTime.Now.Date)
                            countCloseTicket++;
                    }

                    response.CloseTicketCountToday = countCloseTicket;

                    //all open tickets
                    var openCount = totalTicket.Where(x => x.IsActive && !x.IsDeleted && (x.TicketStatus == 1 || x.TicketStatus == 2)).Count();

                    response.OpenTicketCount = openCount;

                    //all ticket close ticket with in last 30 day
                    var lastThirtyDaysT = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketStatus == 3 && (x.TicketClosedOn >= dateHelperDTO.startDate && x.TicketClosedOn <= dateHelperDTO.endDate)).Count();

                    response.lastThirtyDaysCloseTickets = lastThirtyDaysT;

                    // its for graph of monthwise how many ticket open in every month
                    for (int i = 1; i <= 12; i++)
                    {
                        TicketGraphModalForMonth Tg = new TicketGraphModalForMonth();
                        Tg.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
                        //Tg.value = totalTicket.Where(x => x.CreatedOn.Year == currentYear &&
                        //  x.CreatedOn.Month == i).ToList().Count;
                        Tg.value = totalTicket
                            /*.Where(x => x.CreatedOn.Year == currentYear &&x.CreatedOn.Month == i)*/.ToList().Count;
                        tickGraph.Add(Tg);
                    }
                    List<TicketGraph> listGraph = new List<TicketGraph>();
                    TicketGraph ticObj = new TicketGraph
                    {
                        Name = "Ticket Count",
                        Series = tickGraph,
                    };
                    listGraph.Add(ticObj);
                    response.LineChart = listGraph;

                    #endregion genral count base's on status and time

                    #region Counts Bases on Category and Priority

                    List<TicketCategoryModal> TicketCate = new List<TicketCategoryModal>();
                    var ticketCategoryIds = totalTicket.Where(x => x.IsActive == true).Select(x => x.TicketCategoryId).Distinct().ToList();
                    foreach (var item in ticketCategoryIds)
                    {
                        TicketCategoryModal obj = new TicketCategoryModal();
                        obj.IsDeletedCategory = ticketCategory.Where(x => x.TicketCategoryId == item).Select(x => x.IsDeleted).FirstOrDefault();
                        obj.Category = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item).Select(x => x.CategoryName).FirstOrDefault();
                        obj.ToDay = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date && x.TicketCategoryId == item).Count();
                        obj.Last30Days = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item && (x.CreatedOn >= dateHelperDTO.startDate && x.CreatedOn <= dateHelperDTO.endDate)).Count();
                        List<TicketGraphModal> Tgm = new List<TicketGraphModal>();
                        List<TicketColourModal> Cgm = new List<TicketColourModal>();
                        var ticketPriorityes = totalTicket.Where(x => x.TicketCategoryId == item).Select(x => x.TicketPriorityId).Distinct().ToList();
                        foreach (var prio in ticketPriorityes)
                        {
                            TicketGraphModal graphModel = new TicketGraphModal
                            {
                                name = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).Select(x => x.PriorityName).FirstOrDefault(),
                                value = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).ToList().Count,
                            };

                            Tgm.Add(graphModel);
                        }
                        foreach (var point in Tgm)
                        {
                            TicketColourModal colourModal = new TicketColourModal();
                            switch (point.name)
                            {
                                case "Low":
                                    colourModal.value = "#F6358A";
                                    colourModal.name = point.name;
                                    break;

                                case "Medium":
                                    colourModal.value = "#02cccd";
                                    colourModal.name = point.name;
                                    break;

                                case "High":
                                    colourModal.value = "#a5a5a5";
                                    colourModal.name = point.name;
                                    break;

                                case "Urgent":
                                    colourModal.value = "#ffbc58";
                                    colourModal.name = point.name;
                                    break;

                                default:
                                    colourModal.value = "#A74AC7";
                                    colourModal.name = point.name;
                                    break;
                            }
                            Cgm.Add(colourModal);
                        }
                        obj.ColorGroup = Cgm;
                        obj.TicketGraphs = Tgm;
                        TicketCate.Add(obj);
                    }

                    response.Ticketcat = TicketCate.OrderBy(x => x.IsDeletedCategory).ToList();

                    #endregion Counts Bases on Category and Priority

                    res.Status = true;
                    res.Message = "get data behalf tickets";
                    res.Data = response;
                }
                else
                {
                    res.Status = false;
                    res.Message = "data not found";
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

        #endregion This Api use By Ticket Master Dashboard

        #region Helper Model Class

        public class TicketCategoryCount
        {
            public object GetTicketList { get; set; }
            public int Count { get; set; }
        }
        public class TicketHelper
        {

            public object TicketResponseModel { get; set; }
            public List<int> Employees { get; set; }
            public List<TicketCategoryPriority> Priorities { get; set; }

        }


        public class AddTicketCategoryResponse
        {
            public TicketCategory TicketCategory { get; set; }
            public List<TicketCategoryEmployee> Employees { get; set; }
            public List<TicketCategoryPriority> Priorities { get; set; }
        }

        public class GetTicketCategoryResponseModel
        {
            public int TicketCategoryId { get; set; }
            public string TicketCategoryTitle { get; set; }
            public string TicketCategoryDetails { get; set; }
            public List<string> HelpingTeam { get; set; }
            public bool IsMore { get; set; }
        }

        public class GetTicketCategoryResponseModel2
        {
            public int TicketCategoryId { get; set; }
            public string TicketCategoryTitle { get; set; }
            public string TicketCategoryDetails { get; set; }
            public List<EmployeeHelpingTeam> HelpingTeam { get; set; }
        }

        public class EmployeeHelpingTeam
        {
            public string EmployeeName { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 23-03-2022
        /// </summary>
        public class AddTicket
        {
            public int TicketCategoryId { get; set; }
            public int TicketPriorityId { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string Image1 { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string Image4 { get; set; }
            public string Image5 { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 23-03-2022
        /// </summary>
        public class GetTicketDetailResponse
        {
            public int TicketId { get; set; }
            public int TicketPriorityId { get; set; }
            public int PriorityType { get; set; }
            public string PriorityName { get; set; }
            public string Title { get; set; }
            public int TicketStatus { get; set; }
            public string StatusName { get; set; }
            public int TicketCategoryId { get; set; }
            public string CategoryName { get; set; }
            public int AssignedToId { get; set; }
            public string AssignedToName { get; set; }
            public UserBlock EmployeeBlock { get; set; }
            public object CommentList { get; set; }
        }

        public class UserBlock
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Designation { get; set; }
            public string ContactNumber { get; set; }
            public string Location { get; set; }
            public int DesignationId { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 23-03-2022
        /// </summary>
        public class GetCommentListOnTicketDetail
        {
            public DateTime CreateOn { get; set; }
            public string CommentBy { get; set; }
            public string CommentOn { get; set; }
            public string Message { get; set; }
            public string CommentType { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        public class GetMyTicketPage
        {
            public List<OpenTicketMyTicket> OpenTicket { get; set; }
            public List<CloseTicketMyTicket> CloseTicket { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        public class OpenTicketMyTicket
        {
            public int TicketId { get; set; }
            public string TicketNumber { get; set; }
            public int PriorityType { get; set; }
            public string PriorityName { get; set; }
            public string Title { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CategoryName { get; set; }
            public string AssignedTo { get; set; }
            public string TicketStatus { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 22-03-2022
        /// </summary>
        public class CloseTicketMyTicket
        {
            public int TicketId { get; set; }
            public string TicketNumber { get; set; }
            public string Title { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CategoryName { get; set; }
            public string ClosedBy { get; set; }
            public DateTime? ClosedOn { get; set; }
        }



        public class TicketCountRes
        {
            public int TodayTicketCount { get; set; }
            public int CloseTicketCountToday { get; set; }
            public int OpenTicketCount { get; set; }
            public int lastThirtyDaysCloseTickets { get; set; }
            public List<TicketGraph> LineChart { get; set; }
            public List<TicketCategoryModal> Ticketcat { get; set; }
        }

        public class TicketCategoryModal
        {
            public bool IsDeletedCategory { get; set; }
            public string Category { get; set; }
            public int ToDay { get; set; }
            public int Last30Days { get; set; }
            public List<TicketGraphModal> TicketGraphs { get; set; }
            public List<TicketColourModal> ColorGroup { get; set; }
        }


        public class TicketGraph
        {
            public string Name { get; set; }
            public List<TicketGraphModalForMonth> Series { get; set; }
        }

        public class TicketGraphModalForMonth
        {
            public string name { get; set; }
            public int value { get; set; }
        }

        public class TicketGraphModal
        {
            public string name { get; set; }
            public int value { get; set; }
        }

        public class TicketColourModal
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        #endregion Helper Model Class

        #region This Api Use To Send Ticket Mail
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public async Task SendMail(Ticket model, ClaimsHelperModel claims)
        {
            try
            {
                var assignEmployeeName = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == model.TicketCategoryId && x.IsActive && !x.IsDeleted).Select(x => x.EmployeeId).ToList();
                //var adminMail = _db.User.Where(x => x.CompanyId == 0 && x.OrgId == 0 && x.IsActive && !x.IsDeleted).ToList();
                foreach (var employee in assignEmployeeName)
                {
                    var employeeDemo = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).FirstOrDefault();
                    var employeeData = _db.Employee.Where(x => x.EmployeeId == employee).FirstOrDefault();
                    var createEmployee = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (employeeData != null)
                    {
                        string body = "<div style='background: #FFFFFFF; color: #333333; padding: 30px; font-family: arial,sans-serif; font-size: 15px;text-align: center; align-item: center; '>";
                        //body += "<h3 style='background-color: rgb(241, 89, 34);'>Hii </h3>";
                        body += "<h3>Ticket Created</h3>";
                        body += "Hello, " + employeeData.DisplayName + ',';
                        body += "<p> A ticket has been Created ! </p>";
                        //       body += "<br />";
                        body += "<hr />";
                        body += "Ticket Number : " + model.TicketId;
                        body += "<br />";
                        body += "<br />";
                        body += "Ticket Title : " + model.Title;
                        body += "<br />";
                        body += "<br />";
                        body += "Ticket Category : " + model.CategoryName;
                        body += "<br />";
                        body += "<br />";
                        body += "Ticket Created By : " + createEmployee.DisplayName;
                        body += "<br />";
                        body += "<br />";
                        //body += "Welcome to the team " + employee.CompanyName + ',';
                        body += "<p> We are glad to receive your acceptance. </p>";
                        body += "<hr />";
                        body += "Thanks";
                        body += "<br />";
                        body += "</div>";

                        SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                        if (claims.IsSmtpProvided)
                        {
                            smtpsettings = _db.CompanySmtpMailModels
                                .Where(x => x.CompanyId == claims.companyId)
                                .Select(x => new SmtpSendMailRequest
                                {
                                    From = x.From,
                                    SmtpServer = x.SmtpServer,
                                    MailUser = x.MailUser,
                                    MailPassword = x.Password,
                                    Port = x.Port,
                                    ConectionType = x.ConnectType,
                                })
                                .FirstOrDefault();
                        }
                        SendMailModelRequest sendMailObject = new SendMailModelRequest()
                        {
                            IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                            Subject = "Ticket Created in Your Category",
                            MailBody = body,
                            MailTo = new List<string>() { employeeData.OfficeEmail },
                            SmtpSettings = smtpsettings,
                        };
                        await SmtpMailHelper.SendMailAsync(sendMailObject);
                    }

                    if (createEmployee != null)
                    {
                        string body = "<div style='background: #FFFFFFF; color: #333333; padding: 30px; font-family: arial,sans-serif; font-size: 15px;text-align: center; align-item: center; '>";
                        //body += "<h3 style='background-color: rgb(241, 89, 34);'>Hii </h3>";
                        body += "<h3>Ticket Created</h3>";
                        body += "Hello, " + employeeDemo.DisplayName + ',';
                        body += "<p> A ticket has been Created ! </p>";
                        //       body += "<br />";
                        body += "<hr />";
                        body += "Ticket Number : " + model.TicketId;
                        body += "<br />";
                        body += "<br />";
                        body += "Ticket Title : " + model.Title;
                        body += "<br />";
                        body += "<br />";
                        body += "Ticket Category : " + model.CategoryName;
                        body += "<br />";
                        body += "<br />";
                        body += "Ticket Created By : " + createEmployee.DisplayName;
                        body += "<br />";
                        body += "<br />";
                        //body += "Welcome to the team " + employee.CompanyName + ',';
                        body += "<p> We are glad to assist you. . </p>";
                        body += "<hr />";
                        body += "Thanks";
                        body += "<br />";
                        body += "</div>";

                        // Your email address
                        SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                        if (claims.IsSmtpProvided)
                        {
                            smtpsettings = _db.CompanySmtpMailModels
                                .Where(x => x.CompanyId == claims.companyId)
                                .Select(x => new SmtpSendMailRequest
                                {
                                    From = x.From,
                                    SmtpServer = x.SmtpServer,
                                    MailUser = x.MailUser,
                                    MailPassword = x.Password,
                                    Port = x.Port,
                                    ConectionType = x.ConnectType,
                                })
                                .FirstOrDefault();

                        }
                        SendMailModelRequest sendMailObject = new SendMailModelRequest()
                        {
                            IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                            Subject = "Ticket Created in Your Category",
                            MailBody = body,
                            MailTo = new List<string>() { createEmployee.OfficeEmail },
                            SmtpSettings = smtpsettings,
                        };
                        await SmtpMailHelper.SendMailAsync(sendMailObject);
                    }
                }
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region Helper model for Combine
        public class TicketRefHelper
        {
            public List<GetTicketdataResponseModel> TicketCategory { get; set; }
            //public List<proirityListhelper> proirityList { get; set; }
            public dynamic TicketResponse { get; set; }
        }

        public class GetTicketdataResponseModel
        {
            public int TicketCategoryId { get; set; }
            public string TicketCategoryTitle { get; set; }
            public string TicketCategoryDetails { get; set; }
            public List<string> HelpingTeam { get; set; }
            public bool IsMore { get; set; }
            public List<Prioritylisthelper> Prioritylist { get; set; }
        }

        public class Prioritylisthelper
        {
            public int TicPriorityId { get; set; }
            public string PriorityName { get; set; }
            public string PriorityDescription { get; set; }

        }

        #endregion

        #region Api For Get All TicketCategory Reference Combine api

        /// <summary>
        /// API=> Post=> api/ticketmaster/getticketref
        /// Created On 16-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getticketref")]
        public async Task<ResponseBodyModel> GetAllTicketCategoryref(AddTicket modeldata)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetTicketdataResponseModel> list = new List<GetTicketdataResponseModel>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TicketRefHelper datalist = new TicketRefHelper();
            try
            {
                var ticketcategory = await _db.TicketCategories.Where(s => s.IsActive && !s.IsDeleted && s.CompanyId == claims.companyId).ToListAsync();
                if (ticketcategory.Count > 0)
                {
                    foreach (var item in ticketcategory)
                    {
                        GetTicketdataResponseModel obj = new GetTicketdataResponseModel
                        {
                            TicketCategoryId = item.TicketCategoryId,
                            TicketCategoryTitle = item.CategoryName,
                            TicketCategoryDetails = item.Description,
                            HelpingTeam = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == item.TicketCategoryId && x.CompanyId == claims.companyId).Select(x => x.EmployeeName).ToList(),
                            IsMore = false,
                            Prioritylist = _db.TicPriorities.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item.TicketCategoryId && x.CompanyId == claims.companyId)
                            .Select(x => new Prioritylisthelper
                            {
                                TicPriorityId = x.TicPriorityId,
                                PriorityName = x.PriorityName,
                                PriorityDescription = x.PriorityDescription,
                            }).ToList(),
                        };
                        list.Add(obj);
                    }
                }
                datalist.TicketCategory = list;
                //}
                TicketMasterController test = new TicketMasterController();
                var ticketdata = await test.AddUserTickets(modeldata);
                var response = (Ticket)ticketdata.Data;
                datalist.TicketResponse = response;

                if (list.Count > 0)
                {
                    res.Message = "Ticket Category List";
                    res.Status = true;
                    res.Data = datalist;
                }
                else
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                    res.Data = datalist;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion Api For Get All TicketCategory

        #region This Api Use To Send Ticket Mail For Assigend Employee
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public async Task SendAssignMail(Ticket model, ClaimsHelperModel claims)
        {
            try
            {
                var employeeData = _db.Employee.Where(x => x.EmployeeId == model.AssignedToId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();

                string body = "<div style='background: #FFFFFFF; color: #333333; padding: 30px; font-family: arial,sans-serif; font-size: 15px;text-align: center; align-item: center; '>";
                body += "<h3>Ticket Assigned</h3>";
                body += "Hello " + employeeData.DisplayName + ',';
                body += "<p> A ticket has been assigned to you ! </p>";
                body += "<br />";
                body += "<hr />";
                body += "Ticket Number : " + model.TicketId;
                body += "<br />";
                body += "<br />";
                body += "Ticket Title : " + model.Title;
                body += "<br />";
                body += "<br />";
                body += "Ticket Category : " + model.CategoryName;
                body += "<br />";
                body += "<br />";
                body += "Ticket Created By : " + createEmployee.DisplayName;
                body += "<br />";
                body += "<br />";
                body += "<hr />";
                body += "Thanks,";
                body += "<br />";
                body += "</div>";

                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (claims.IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == claims.companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefault();
                }
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "You Have a New Ticket",
                    MailBody = body,
                    MailTo = new List<string>() { employeeData.OfficeEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }

        }
        #endregion

        #region This Api Use To Send Ticket Mail For Update Employee
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public async Task SendTicketStatusMailToEmployee(Ticket model, string companyName, int type, ClaimsHelperModel claims)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                var check = Enum.GetName(typeof(TicketStatus), model.TicketStatus);
                var employeeData = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                //if (employeeData != null)
                //{
                string body = "<div style='background: #FFFFFFF; color: #333333; padding: 30px; font-family: arial,sans-serif; font-size: 15px;text-align: center; align-item: center; '>";
                //body += "<h3 style='background-color: rgb(241, 89, 34);'>Ticket Updated </h3>";
                body += "<h3>Ticket Updated </h3>";
                body += "Hii " + employeeData.DisplayName + ',';
                body += (type == 1 ? "<p> A ticket has been updated ! </p>" : "<p> A ticket category has been updated ! </p>");
                body += "<br />";
                body += "<p> Ticket Details ! </p>";
                // body += "<br />";
                body += "<hr />";
                body += "Ticket Number : " + model.TicketId;
                body += "<br />";
                body += "<br />";
                body += "Ticket Title : " + model.Title;
                body += "<br />";
                body += "<br />";
                body += "Ticket Status : " + check;
                body += "<br />";
                body += "<br />";
                body += "Ticket Category : " + model.CategoryName;
                body += "<br />";
                body += "<br />";
                body += "Assignee : " + (type == 1 ? model.AssignedToName : "Waiting for assignee");
                body += "<br />";
                body += "<br />";
                body += "Ticket Created By : " + employeeData.DisplayName;
                body += "<br />";
                body += "<br />";
                body += "<hr />";
                body += "<br />";
                body += "<br />";
                body += "<br />";
                body += "<br />";
                body += "<br />";
                body += "Regards,";
                body += "<br />";
                body += companyName;
                body += "<br />";
                body += "</div>";

                // Your email address
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (claims.IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == claims.companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefault();
                }
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "You Have a New Ticket",
                    MailBody = body,
                    MailTo = new List<string>() { employeeData.OfficeEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region Api for Get All Employee Where Login Type HR or IT
        /// <summary>
        /// API>>GET>>api/ticketmaster/getallheitemployee
        /// </summary>
        /// <returns></returns>

        [Route("getallheitemployee")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetHRorIT()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var getEmployee = await (from e in _db.Employee
                                         join u in _db.User on e.EmployeeId equals u.EmployeeId
                                         where e.IsActive && !e.IsDeleted && (u.LoginId == LoginRolesConstants.HR || u.LoginId == LoginRolesConstants.IT)
                                         && e.CompanyId == claims.companyId && e.OrgId == claims.orgId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                         select new
                                         {
                                             e.DisplayName,
                                             e.EmployeeId
                                         }).ToListAsync();

                if (getEmployee.Count > 0)
                {
                    res.Message = "Employee Get Succesfully !";
                    res.Status = true;
                    res.Data = getEmployee;

                }
                else
                {
                    res.Message = "Employee Not Found !";
                    res.Status = false;
                    res.Data = getEmployee;
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

        #region API TO UPLOAD TICKET IMAGE MULTIPLE
        /// <summary>
        /// api/ticketmaster/uploadticketmultiple
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadticketmultiple")]
        public async Task<HttpResponseMessageMultiple> UploadImage()
        {
            HttpResponseMessageMultiple result = new HttpResponseMessageMultiple();
            List<PathLists> list = new List<PathLists>();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
                List<string> path = new List<string>();
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
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
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/TicketIages/" + claims.companyId), dates + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        var temp = "uploadimage\\TicketIages\\" + claims.companyId + "\\" + dates + Fileresult + extension;

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
                }
                else
                {
                    result.Message = "Error";
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }

        #endregion

        #region API TO GET MONTH LIST 
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> GET >> api/ticketmaster/getmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmonthlist")]
        public IHttpActionResult GetMonthList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var response = Enumerable.Range(1, 12)
                    .Select(i => new
                    {
                        MonthId = i,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    })
                    .ToList();
                res.Message = "Month List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/getallcomponent | " + "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region This Api use By Ticket Master Dashboard

        /// <summary>
        /// API >> Get >> api/ticketmaster/geticketcountlist
        /// Created by Suraj Bundel
        /// Created on 05-01-2023
        /// </summary>
        [HttpGet]
        [Route("geticketcountlist")]
        public async Task<ResponseBodyModel> GetTicketCountonmonth(int? month, int? year)
        {
            TicketCountRes response = new TicketCountRes();
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<TicketGraphModalForMonth> tickGraph = new List<TicketGraphModalForMonth>();

            try
            {
                DataHelperResponse dateHelperDTO = new DataHelperResponse();
                DateTime currentDate = DateTime.Now.Date;
                dateHelperDTO.startDate = currentDate.AddDays(-30).Date;
                dateHelperDTO.endDate = currentDate;
                var date = DateTime.Now.Date;
                var currentYear = DateTime.Now.Year;

                //var totalTicket = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId &&
                //                x.CreatedOn.Year == currentYear).ToListAsync();
                if (year.HasValue)
                {
                    var totalTicket = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                    && x.CreatedOn.Year == year).ToListAsync();

                    var ticketCategory = (from TC in _db.TicketCategories
                                          where TC.CompanyId == claims.companyId
                                          select new
                                          {
                                              TC.TicketCategoryId,
                                              TC.CategoryName,
                                              TC.IsActive,
                                              TC.IsDeleted,
                                          }).ToList();

                    if (totalTicket.Count > 0)
                    {
                        #region genral count base's on status and time

                        //today create ticket
                        var todayCount = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date).Count();

                        response.TodayTicketCount = todayCount;

                        var TicketClosedOn = date.Date;
                        var closeCountToday = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketStatus == 3 && x.TicketClosedOn.HasValue).ToList();
                        var countCloseTicket = 0;
                        foreach (var item in closeCountToday)
                        {
                            var checkDate = (DateTime)item.TicketClosedOn;
                            checkDate = checkDate.Date;
                            if (checkDate == DateTime.Now.Date)
                                countCloseTicket++;
                        }

                        response.CloseTicketCountToday = countCloseTicket;

                        //all open tickets
                        var openCount = totalTicket.Where(x => x.IsActive && !x.IsDeleted && (x.TicketStatus == 1 || x.TicketStatus == 2)).Count();

                        response.OpenTicketCount = openCount;

                        //all ticket close ticket with in last 30 day
                        var lastThirtyDaysT = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketStatus == 3 && (x.TicketClosedOn >= dateHelperDTO.startDate && x.TicketClosedOn <= dateHelperDTO.endDate)).Count();

                        response.lastThirtyDaysCloseTickets = lastThirtyDaysT;

                        if (month.HasValue)
                        {
                            for (int i = 1; i <= 12; i++)
                            {
                                TicketGraphModalForMonth Tg = new TicketGraphModalForMonth();
                                Tg.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName((int)month);
                                Tg.value = totalTicket.Where(x => x.CreatedOn.Year == year && x.CreatedOn.Month == month).ToList().Count;
                                Tg.value = totalTicket.Where(x => x.CreatedOn.Year == year && x.CreatedOn.Month == month).ToList().Count;
                                tickGraph.Add(Tg);
                            }
                            List<TicketGraph> listGraph = new List<TicketGraph>();
                            TicketGraph ticObj = new TicketGraph
                            {
                                Name = "Ticket Count",
                                Series = tickGraph,
                            };
                            listGraph.Add(ticObj);
                            response.LineChart = listGraph;

                            #endregion genral count base's on status and time

                            #region Counts Bases on Category and Priority

                            List<TicketCategoryModal> TicketCate = new List<TicketCategoryModal>();
                            var ticketCategoryIds = totalTicket.Where(x => x.IsActive == true).Select(x => x.TicketCategoryId).Distinct().ToList();
                            foreach (var item in ticketCategoryIds)
                            {
                                TicketCategoryModal obj = new TicketCategoryModal();
                                obj.IsDeletedCategory = ticketCategory.Where(x => x.TicketCategoryId == item).Select(x => x.IsDeleted).FirstOrDefault();
                                obj.Category = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item).Select(x => x.CategoryName).FirstOrDefault();
                                obj.ToDay = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date && x.TicketCategoryId == item).Count();
                                obj.Last30Days = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item && (x.CreatedOn >= dateHelperDTO.startDate && x.CreatedOn <= dateHelperDTO.endDate)).Count();
                                List<TicketGraphModal> Tgm = new List<TicketGraphModal>();
                                List<TicketColourModal> Cgm = new List<TicketColourModal>();
                                var ticketPriorityes = totalTicket.Where(x => x.TicketCategoryId == item).Select(x => x.TicketPriorityId).Distinct().ToList();
                                foreach (var prio in ticketPriorityes)
                                {
                                    TicketGraphModal graphModel = new TicketGraphModal
                                    {
                                        name = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).Select(x => x.PriorityName).FirstOrDefault(),
                                        value = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).ToList().Count,
                                    };

                                    Tgm.Add(graphModel);
                                }
                                foreach (var point in Tgm)
                                {
                                    TicketColourModal colourModal = new TicketColourModal();
                                    switch (point.name)
                                    {
                                        case "Low":
                                            colourModal.value = "#F6358A";
                                            colourModal.name = point.name;
                                            break;

                                        case "Medium":
                                            colourModal.value = "#02cccd";
                                            colourModal.name = point.name;
                                            break;

                                        case "High":
                                            colourModal.value = "#a5a5a5";
                                            colourModal.name = point.name;
                                            break;

                                        case "Urgent":
                                            colourModal.value = "#ffbc58";
                                            colourModal.name = point.name;
                                            break;

                                        default:
                                            colourModal.value = "#A74AC7";
                                            colourModal.name = point.name;
                                            break;
                                    }
                                    Cgm.Add(colourModal);
                                }
                                obj.ColorGroup = Cgm;
                                obj.TicketGraphs = Tgm;
                                TicketCate.Add(obj);
                            }

                            response.Ticketcat = TicketCate.OrderBy(x => x.IsDeletedCategory).ToList();

                            #endregion Counts Bases on Category and Priority

                            res.Status = true;
                            res.Message = "get data behalf tickets";
                            res.Data = response;
                        }
                        else
                        {

                            for (int i = 1; i <= 12; i++)
                            {
                                TicketGraphModalForMonth Tg = new TicketGraphModalForMonth();
                                Tg.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
                                //Tg.value = totalTicket.Where(x => x.CreatedOn.Year == currentYear &&
                                //  x.CreatedOn.Month == i).ToList().Count;
                                Tg.value = totalTicket
                                    /*.Where(x => x.CreatedOn.Year == currentYear &&x.CreatedOn.Month == i)*/.ToList().Count;
                                tickGraph.Add(Tg);
                            }
                            List<TicketGraph> listGraph = new List<TicketGraph>();
                            TicketGraph ticObj = new TicketGraph
                            {
                                Name = "Ticket Count",
                                Series = tickGraph,
                            };
                            listGraph.Add(ticObj);
                            response.LineChart = listGraph;

                            #region Counts Bases on Category and Priority

                            List<TicketCategoryModal> TicketCate = new List<TicketCategoryModal>();
                            var ticketCategoryIds = totalTicket.Where(x => x.IsActive == true).Select(x => x.TicketCategoryId).Distinct().ToList();
                            foreach (var item in ticketCategoryIds)
                            {
                                TicketCategoryModal obj = new TicketCategoryModal();
                                obj.IsDeletedCategory = ticketCategory.Where(x => x.TicketCategoryId == item).Select(x => x.IsDeleted).FirstOrDefault();
                                obj.Category = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item).Select(x => x.CategoryName).FirstOrDefault();
                                obj.ToDay = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date && x.TicketCategoryId == item).Count();
                                obj.Last30Days = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item && (x.CreatedOn >= dateHelperDTO.startDate && x.CreatedOn <= dateHelperDTO.endDate)).Count();
                                List<TicketGraphModal> Tgm = new List<TicketGraphModal>();
                                List<TicketColourModal> Cgm = new List<TicketColourModal>();
                                var ticketPriorityes = totalTicket.Where(x => x.TicketCategoryId == item).Select(x => x.TicketPriorityId).Distinct().ToList();
                                foreach (var prio in ticketPriorityes)
                                {
                                    TicketGraphModal graphModel = new TicketGraphModal
                                    {
                                        name = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).Select(x => x.PriorityName).FirstOrDefault(),
                                        value = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).ToList().Count,
                                    };

                                    Tgm.Add(graphModel);
                                }
                                foreach (var point in Tgm)
                                {
                                    TicketColourModal colourModal = new TicketColourModal();
                                    switch (point.name)
                                    {
                                        case "Low":
                                            colourModal.value = "#F6358A";
                                            colourModal.name = point.name;
                                            break;

                                        case "Medium":
                                            colourModal.value = "#02cccd";
                                            colourModal.name = point.name;
                                            break;

                                        case "High":
                                            colourModal.value = "#a5a5a5";
                                            colourModal.name = point.name;
                                            break;

                                        case "Urgent":
                                            colourModal.value = "#ffbc58";
                                            colourModal.name = point.name;
                                            break;

                                        default:
                                            colourModal.value = "#A74AC7";
                                            colourModal.name = point.name;
                                            break;
                                    }
                                    Cgm.Add(colourModal);
                                }
                                obj.ColorGroup = Cgm;
                                obj.TicketGraphs = Tgm;
                                TicketCate.Add(obj);
                            }

                            response.Ticketcat = TicketCate.OrderBy(x => x.IsDeletedCategory).ToList();

                            #endregion Counts Bases on Category and Priority

                            res.Status = true;
                            res.Message = "get data behalf tickets";
                            res.Data = response;

                        }


                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "data not found";
                        res.Data = response;
                    }
                }

                else
                {
                    var totalTicket = await _db.Tickets.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                       ).ToListAsync();
                    var ticketCategory = (from TC in _db.TicketCategories
                                          where TC.CompanyId == claims.companyId
                                          select new
                                          {
                                              TC.TicketCategoryId,
                                              TC.CategoryName,
                                              TC.IsActive,
                                              TC.IsDeleted,
                                          }).ToList();

                    if (totalTicket.Count > 0)
                    {
                        #region genral count base's on status and time

                        //today create ticket
                        var todayCount = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date).Count();

                        response.TodayTicketCount = todayCount;

                        var TicketClosedOn = date.Date;
                        var closeCountToday = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketStatus == 3 && x.TicketClosedOn.HasValue).ToList();
                        var countCloseTicket = 0;
                        foreach (var item in closeCountToday)
                        {
                            var checkDate = (DateTime)item.TicketClosedOn;
                            checkDate = checkDate.Date;
                            if (checkDate == DateTime.Now.Date)
                                countCloseTicket++;
                        }

                        response.CloseTicketCountToday = countCloseTicket;

                        //all open tickets
                        var openCount = totalTicket.Where(x => x.IsActive && !x.IsDeleted && (x.TicketStatus == 1 || x.TicketStatus == 2)).Count();

                        response.OpenTicketCount = openCount;

                        //all ticket close ticket with in last 30 day
                        var lastThirtyDaysT = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketStatus == 3 && (x.TicketClosedOn >= dateHelperDTO.startDate && x.TicketClosedOn <= dateHelperDTO.endDate)).Count();

                        response.lastThirtyDaysCloseTickets = lastThirtyDaysT;

                        // its for graph of monthwise how many ticket open in every month
                        if (month.HasValue)
                        {
                            for (int i = 1; i <= 12; i++)
                            {
                                TicketGraphModalForMonth Tg = new TicketGraphModalForMonth();
                                Tg.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName((int)month);
                                Tg.value = totalTicket.Where(x => /*x.CreatedOn.Year == currentYear && */ x.CreatedOn.Month == month).ToList().Count;
                                Tg.value = totalTicket.Where(x => /*x.CreatedOn.Year == currentYear &&*/x.CreatedOn.Month == month).ToList().Count;
                                tickGraph.Add(Tg);
                            }
                            List<TicketGraph> listGraph = new List<TicketGraph>();
                            TicketGraph ticObj = new TicketGraph
                            {
                                Name = "Ticket Count",
                                Series = tickGraph,
                            };
                            listGraph.Add(ticObj);
                            response.LineChart = listGraph;

                            #endregion genral count base's on status and time

                            #region Counts Bases on Category and Priority

                            List<TicketCategoryModal> TicketCate = new List<TicketCategoryModal>();
                            var ticketCategoryIds = totalTicket.Where(x => x.IsActive == true).Select(x => x.TicketCategoryId).Distinct().ToList();
                            foreach (var item in ticketCategoryIds)
                            {
                                TicketCategoryModal obj = new TicketCategoryModal();
                                obj.IsDeletedCategory = ticketCategory.Where(x => x.TicketCategoryId == item).Select(x => x.IsDeleted).FirstOrDefault();
                                obj.Category = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item).Select(x => x.CategoryName).FirstOrDefault();
                                obj.ToDay = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date && x.TicketCategoryId == item).Count();
                                obj.Last30Days = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item && (x.CreatedOn >= dateHelperDTO.startDate && x.CreatedOn <= dateHelperDTO.endDate)).Count();
                                List<TicketGraphModal> Tgm = new List<TicketGraphModal>();
                                List<TicketColourModal> Cgm = new List<TicketColourModal>();
                                var ticketPriorityes = totalTicket.Where(x => x.TicketCategoryId == item).Select(x => x.TicketPriorityId).Distinct().ToList();
                                foreach (var prio in ticketPriorityes)
                                {
                                    TicketGraphModal graphModel = new TicketGraphModal
                                    {
                                        name = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).Select(x => x.PriorityName).FirstOrDefault(),
                                        value = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).ToList().Count,
                                    };

                                    Tgm.Add(graphModel);
                                }
                                foreach (var point in Tgm)
                                {
                                    TicketColourModal colourModal = new TicketColourModal();
                                    switch (point.name)
                                    {
                                        case "Low":
                                            colourModal.value = "#F6358A";
                                            colourModal.name = point.name;
                                            break;

                                        case "Medium":
                                            colourModal.value = "#02cccd";
                                            colourModal.name = point.name;
                                            break;

                                        case "High":
                                            colourModal.value = "#a5a5a5";
                                            colourModal.name = point.name;
                                            break;

                                        case "Urgent":
                                            colourModal.value = "#ffbc58";
                                            colourModal.name = point.name;
                                            break;

                                        default:
                                            colourModal.value = "#A74AC7";
                                            colourModal.name = point.name;
                                            break;
                                    }
                                    Cgm.Add(colourModal);
                                }
                                obj.ColorGroup = Cgm;
                                obj.TicketGraphs = Tgm;
                                TicketCate.Add(obj);
                            }

                            response.Ticketcat = TicketCate.OrderBy(x => x.IsDeletedCategory).ToList();

                            #endregion Counts Bases on Category and Priority

                            res.Status = true;
                            res.Message = "get data behalf tickets";
                            res.Data = response;
                        }
                        else
                        {

                            for (int i = 1; i <= 12; i++)
                            {
                                TicketGraphModalForMonth Tg = new TicketGraphModalForMonth();
                                Tg.name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
                                //Tg.value = totalTicket.Where(x => x.CreatedOn.Year == currentYear &&
                                //  x.CreatedOn.Month == i).ToList().Count;
                                Tg.value = totalTicket
                                    /*.Where(x => x.CreatedOn.Year == currentYear &&x.CreatedOn.Month == i)*/.ToList().Count;
                                tickGraph.Add(Tg);
                            }
                            List<TicketGraph> listGraph = new List<TicketGraph>();
                            TicketGraph ticObj = new TicketGraph
                            {
                                Name = "Ticket Count",
                                Series = tickGraph,
                            };
                            listGraph.Add(ticObj);
                            response.LineChart = listGraph;


                            #region Counts Bases on Category and Priority

                            List<TicketCategoryModal> TicketCate = new List<TicketCategoryModal>();
                            var ticketCategoryIds = totalTicket.Where(x => x.IsActive == true).Select(x => x.TicketCategoryId).Distinct().ToList();
                            foreach (var item in ticketCategoryIds)
                            {
                                TicketCategoryModal obj = new TicketCategoryModal();
                                obj.IsDeletedCategory = ticketCategory.Where(x => x.TicketCategoryId == item).Select(x => x.IsDeleted).FirstOrDefault();
                                obj.Category = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item).Select(x => x.CategoryName).FirstOrDefault();
                                obj.ToDay = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.CreatedOn.Date == date && x.TicketCategoryId == item).Count();
                                obj.Last30Days = totalTicket.Where(x => x.IsActive && !x.IsDeleted && x.TicketCategoryId == item && (x.CreatedOn >= dateHelperDTO.startDate && x.CreatedOn <= dateHelperDTO.endDate)).Count();
                                List<TicketGraphModal> Tgm = new List<TicketGraphModal>();
                                List<TicketColourModal> Cgm = new List<TicketColourModal>();
                                var ticketPriorityes = totalTicket.Where(x => x.TicketCategoryId == item).Select(x => x.TicketPriorityId).Distinct().ToList();
                                foreach (var prio in ticketPriorityes)
                                {
                                    TicketGraphModal graphModel = new TicketGraphModal
                                    {
                                        name = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).Select(x => x.PriorityName).FirstOrDefault(),
                                        value = totalTicket.Where(x => x.TicketPriorityId == prio && x.TicketCategoryId == item).ToList().Count,
                                    };

                                    Tgm.Add(graphModel);
                                }
                                foreach (var point in Tgm)
                                {
                                    TicketColourModal colourModal = new TicketColourModal();
                                    switch (point.name)
                                    {
                                        case "Low":
                                            colourModal.value = "#F6358A";
                                            colourModal.name = point.name;
                                            break;

                                        case "Medium":
                                            colourModal.value = "#02cccd";
                                            colourModal.name = point.name;
                                            break;

                                        case "High":
                                            colourModal.value = "#a5a5a5";
                                            colourModal.name = point.name;
                                            break;

                                        case "Urgent":
                                            colourModal.value = "#ffbc58";
                                            colourModal.name = point.name;
                                            break;

                                        default:
                                            colourModal.value = "#A74AC7";
                                            colourModal.name = point.name;
                                            break;
                                    }
                                    Cgm.Add(colourModal);
                                }
                                obj.ColorGroup = Cgm;
                                obj.TicketGraphs = Tgm;
                                TicketCate.Add(obj);
                            }

                            response.Ticketcat = TicketCate.OrderBy(x => x.IsDeletedCategory).ToList();

                            #endregion Counts Bases on Category and Priority

                            res.Status = true;
                            res.Message = "get data behalf tickets";
                            res.Data = response;
                        }
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "data not found";
                        res.Data = response;
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

        #endregion This Api use By Ticket Master Dashboard

    }
}