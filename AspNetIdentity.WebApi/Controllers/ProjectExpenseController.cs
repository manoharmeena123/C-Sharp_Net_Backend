using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controller
{
    [Authorize]
    [RoutePrefix("api/projectexpense")]
    public class ProjectExpenseController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        #region Create Expense

        /// <summary>
        /// Create by Shriya Malvi On 19-07-2022
        /// Modify By ankit on 05-08-2022
        /// API >> Post >> api/projectexpense/projectexpensecreate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("projectexpensecreate")]
        public async Task<ResponseBodyModel> ProjectExpenseCreate(CreateExpenseHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = db.ProjectExpenseMasters.Where(x => x.ExpenseName == model.ExpenseName).FirstOrDefault();
                if (data != null)
                {
                    res.Message = "Duplicate Data";
                    res.Status = false;
                    res.Data = data;
                }
                else if (model != null)
                {
                    ProjectExpenseMaster pem = new ProjectExpenseMaster();
                    pem.ProjectId = model.ProjectId;
                    pem.ExpenseDate = DateTime.Now;
                    pem.ExpenseName = model.ExpenseName;
                    pem.ExpenseDate = model.ExpenseDate;
                    pem.ExpenseInvoice = model.ExpenseInvoice;
                    pem.IsActive = true;
                    pem.IsDeleted = false;
                    pem.CreatedBy = claims.employeeId;
                    pem.CreatedOn = DateTime.Now;
                    pem.CompanyId = claims.companyId;
                    pem.OrgId = claims.orgId;
                    db.ProjectExpenseMasters.Add(pem);
                    await db.SaveChangesAsync();

                    foreach (var item in model.CostAmtList)
                    {
                        ProjectExpCostAmt CostAmt = new ProjectExpCostAmt();
                        CostAmt.Amount = item.Amount;
                        CostAmt.CostName = item.CostName;
                        CostAmt.ExpenseId = pem.ProjectExpId;
                        CostAmt.ProjectId = pem.ProjectId;
                        CostAmt.ExpenseDate = pem.ExpenseDate;
                        CostAmt.IsDeleted = false;
                        CostAmt.IsActive = true;
                        CostAmt.CreatedBy = claims.employeeId;
                        CostAmt.CreatedOn = DateTime.Now;
                        CostAmt.CompanyId = claims.companyId;
                        CostAmt.OrgId = claims.orgId;

                        db.ProjectExpCostAmts.Add(CostAmt);
                        db.SaveChanges();
                    }
                    res.Status = true;
                    res.Message = "Expense Created";
                    res.Data = pem;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Expense Not Create";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Create Expense

        //#region    Get ALL expense
        //public async Task<ResponseBodyModel> GetAllProjectExpense()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    var employee = db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.employeeid).ToList();
        //    try
        //    {
        //        if (claims.roletype == "Administrator")
        //        {
        //            var ProjectExpense = (from proexp in db.ProjectExpenseMasters
        //                                  join ca in db.ProjectExpCostAmts on proexp.ProjectExpId equals ca.ExpenseId
        //                                  join pro in db.ProjectLists on proexp.ProjectId equals pro.ID
        //                                  where proexp.IsActive == true && proexp.IsDeleted == false &&
        //                                  proexp.CompanyId == claims.companyid
        //                                  select new GetHelperForExpense
        //                                  {
        //                                      ProjectId = proexp.ProjectId,
        //                                      ProjectExpenseId = proexp.ProjectExpId,
        //                                      ProjectName = pro.ProjectName,
        //                                      ExpenseDate = Convert.ToDateTime(proexp.ExpenseDate),
        //                                      ExpenseInvoice = proexp.ExpenseInvoice,
        //                                      CreatedBy = proexp.CreatedBy,
        //                                      CreatedByName = employee.Where(x => x.EmployeeId == proexp.CreatedBy).Select(x => x.DisplayName).FirstOrDefault(),
        //                                      UpdateBy = proexp.UpdatedBy,
        //                                      UpdateByName = employee.Where(x => x.EmployeeId == proexp.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault()
        //                                  }).ToList();
        //        }
        //        else
        //        {
        //            var ProjectExpense = (from proexp in db.ProjectExpenseMasters
        //                                  join fd in db.ProjectExpCostAmts on proexp.ProjectExpId equals fd.ExpenseId
        //                                  join pro in db.ProjectLists on proexp.ProjectId equals pro.ID
        //                                  where proexp.IsActive == true && proexp.IsDeleted == false &&
        //                            proexp.CompanyId == claims.companyid && (proexp.OrgId == claims.orgid || proexp.OrgId == 0)
        //                                  select new GetHelperForExpense
        //                                  {
        //                                      ProjectId = proexp.ProjectId,
        //                                      ProjectExpenseId = proexp.ProjectExpId,
        //                                      ProjectName = pro.ProjectName,
        //                                      ExpenseDate = Convert.ToDateTime(proexp.ExpenseDate),
        //                                      ExpenseInvoice = proexp.ExpenseInvoice,
        //                                      CreatedBy = proexp.CreatedBy,
        //                                      CreatedByName = employee.Where(x => x.EmployeeId == proexp.CreatedBy).Select(x => x.DisplayName).FirstOrDefault(),
        //                                      UpdateBy = proexp.UpdatedBy,
        //                                      UpdateByName = employee.Where(x => x.EmployeeId == proexp.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault(),

        //                                  }).ToList();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Status = false;
        //        res.Message = ex.Message;

        //    }
        //    return res;
        //}
        //#endregion

        #region Get ALL expense

        /// <summary>
        /// Create by Shriya Malvi On 19-07-2022
        /// API >> Get >>api/projectexpense/getallprojectexpense
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallprojectexpense")]
        public async Task<ResponseBodyModel> GetAllProjectExpense()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var employee = await db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();
            var total_count = db.Employee.Where(x => x.IsDeleted == false && x.IsActive == true).Count();
            res.Data = employee;

            try
            {
                var ProjectExpense = (from proexp in db.ProjectExpenseMasters
                                          //  join ca in db.ProjectExpCostAmts on proexp.ProjectExpId equals ca.ExpenseId
                                      join pro in db.ProjectLists on proexp.ProjectId equals pro.ID
                                      join empc in db.Employee on proexp.CreatedBy equals empc.EmployeeId
                                      //join empu in db.Employee on proexp.UpdatedBy equals empu.EmployeeId
                                      where proexp.IsActive == true && proexp.IsDeleted == false &&
                                      proexp.CompanyId == claims.companyId
                                      select new GetHelperForExpense
                                      {
                                          ProjectId = proexp.ProjectId,
                                          ProjectExpenseId = proexp.ProjectExpId,
                                          ProjectName = pro.ProjectName,
                                          ExpenseDate = proexp.ExpenseDate,
                                          ExpenseInvoice = proexp.ExpenseInvoice,
                                          CreatedBy = proexp.CreatedBy,
                                          CreatedByName = empc.DisplayName,
                                          CreateOn = proexp.CreatedOn,
                                          UpdateBy = proexp.UpdatedBy,
                                          UpdateByName = db.Employee.Where(x => x.EmployeeId == proexp.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault(),
                                          ExpenseName = proexp.ExpenseName,
                                          TotalAmount = db.ProjectExpCostAmts.Where(x => x.ExpenseId == proexp.ProjectExpId).Select(x => x.Amount).Sum(),
                                      }).ToList().OrderByDescending(x => x.ProjectId).ToList();
                //var expid = ProjectExpense.Select(x => x.ProjectExpenseId).Distinct().ToList();
                //var total = ProjectExpense.Sum(x => x.Amount);

                if (ProjectExpense.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Project expense List Found";
                    res.Data = ProjectExpense;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Project expense List  Found";
                    res.Data = ProjectExpense;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get ALL expense

        #region Delete Expense

        /// <summary>
        /// Create By Shriya Malvi On 20-07-2022
        /// API >> Deleted >> api/projectexpense/expensedelete
        /// </summary>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("expensedelete")]
        public async Task<ResponseBodyModel> DeleteExpense(int ExpenseId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectExpense = db.ProjectExpenseMasters.Where(x => x.IsActive == true && x.IsDeleted == false && x.ProjectExpId == ExpenseId).FirstOrDefault();
                if (projectExpense != null)
                {
                    projectExpense.IsDeleted = true;
                    projectExpense.IsActive = false;
                    projectExpense.DeletedOn = DateTime.Now;
                    projectExpense.DeletedBy = claims.employeeId;

                    db.Entry(projectExpense).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    var costAmt = db.ProjectExpCostAmts.Where(x => x.IsActive == true && x.IsDeleted == false && x.ExpenseId == ExpenseId).ToList();
                    if (costAmt != null)
                    {
                        foreach (var item in costAmt)
                        {
                            item.IsDeleted = true;
                            item.IsActive = false;
                            item.DeletedOn = DateTime.Now;
                            item.DeletedBy = claims.employeeId;

                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }

                    res.Status = true;
                    res.Message = "Project Expense Delete";
                    res.Data = projectExpense;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Project Expesne Not Delete";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Delete Expense

        #region Get By ExpenseId

        /// <summary>
        /// Create By Shriya Malvi On 20-07-2022
        /// API >> Get >> api/projectexpense/getbyprojectexpenseid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyprojectexpenseid")]
        public async Task<ResponseBodyModel> GetByProjectExpenseId(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<CostAmtList> CostList = new List<CostAmtList>();
            try
            {
                var expense = await db.ProjectExpenseMasters.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId && x.ProjectExpId == Id).FirstOrDefaultAsync();
                if (expense != null)
                {
                    CreateExpenseHelper expensedata = new CreateExpenseHelper();
                    expensedata.ExpenseId = expense.ProjectExpId;
                    expensedata.ProjectId = expense.ProjectId;
                    expensedata.ExpenseInvoice = expense.ExpenseInvoice;
                    expensedata.ExpenseDate = expense.ExpenseDate;
                    expensedata.ExpenseName = expense.ExpenseName;
                    var data = db.ProjectExpCostAmts.Where(x => x.IsActive == true && x.IsDeleted == false && x.ExpenseId == expense.ProjectExpId).ToList();
                    foreach (var item in data)
                    {
                        CostAmtList obj = new CostAmtList()
                        {
                            CostName = item.CostName,
                            Amount = item.Amount,
                        };
                        CostList.Add(obj);
                    }
                    expensedata.CostAmtList = CostList;

                    res.Data = expensedata;
                    res.Message = " Project Expense Found";
                    res.Status = true;
                }
                else
                {
                    res.Message = " Project Expense Not Found";
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

        #endregion Get By ExpenseId

        #region Update Project Expense

        /// <summary>
        /// Create By Shriya Malvi On 20-07-2022
        /// API >> Put >> api/projectexpense/expenseupdate
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("expenseupdate")]
        public async Task<ResponseBodyModel> ExpenseUpdate(CreateExpenseHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var expense = await db.ProjectExpenseMasters.Where(x => x.IsActive == true && x.IsDeleted == false && x.ProjectExpId == model.ExpenseId).FirstOrDefaultAsync();
                if (expense != null)
                {
                    expense.ExpenseName = model.ExpenseName;
                    expense.ExpenseInvoice = model.ExpenseInvoice;
                    expense.ExpenseDate = model.ExpenseDate;
                    expense.ProjectId = model.ProjectId;
                    expense.UpdatedBy = claims.employeeId;
                    expense.UpdatedOn = DateTime.Now;
                    var data = db.ProjectExpCostAmts.Where(x => x.IsActive == true && x.IsDeleted == false && x.ExpenseId == expense.ProjectExpId).ToList();
                    if (data.Count > 0)
                    {
                        db.ProjectExpCostAmts.RemoveRange(data);
                        db.SaveChanges();
                    }
                    foreach (var item in model.CostAmtList)
                    {
                        ProjectExpCostAmt CostAmt = new ProjectExpCostAmt();
                        CostAmt.Amount = item.Amount;
                        CostAmt.CostName = item.CostName;
                        CostAmt.ExpenseId = expense.ProjectExpId;
                        CostAmt.ProjectId = expense.ProjectId;
                        CostAmt.ExpenseDate = expense.ExpenseDate;
                        CostAmt.IsDeleted = false;
                        CostAmt.IsActive = true;
                        CostAmt.CreatedBy = claims.employeeId;
                        CostAmt.CreatedOn = DateTime.Now;
                        CostAmt.CompanyId = claims.companyId;
                        CostAmt.OrgId = claims.orgId;

                        db.ProjectExpCostAmts.Add(CostAmt);
                        db.SaveChanges();
                    }
                    db.Entry(expense).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Status = true;
                    res.Message = "Project Expense Updated";
                    res.Data = expense;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Project Expense Not Update";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Update Project Expense

        #region Get expense List Behalf of Expense Date

        /// <summary>
        /// CreatE By Shriya
        /// API >> Get >> api/projectexpense/getexpensesbydate
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getexpensesbydate")]
        public async Task<ResponseBodyModel> GetExpensesByDate(DateTime Date)
        {
            //  DateTime date = Convert.ToDateTime(Date);
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var expense = await (from proexp in db.ProjectExpenseMasters
                                         //  join ca in db.ProjectExpCostAmts on proexp.ProjectExpId equals ca.ExpenseId
                                     join pro in db.ProjectLists on proexp.ProjectId equals pro.ID
                                     join empc in db.Employee on proexp.CreatedBy equals empc.EmployeeId
                                     //join empu in db.Employee on proexp.UpdatedBy equals empu.EmployeeId
                                     where proexp.IsActive == true && proexp.IsDeleted == false && proexp.ExpenseDate.Month == Date.Month
                                     && proexp.ExpenseDate.Year == Date.Year && proexp.CompanyId == claims.companyId
                                     select new GetHelperForExpense
                                     {
                                         ProjectId = proexp.ProjectId,
                                         ProjectExpenseId = proexp.ProjectExpId,
                                         ProjectName = pro.ProjectName,
                                         ExpenseDate = proexp.ExpenseDate,
                                         ExpenseInvoice = proexp.ExpenseInvoice,
                                         CreatedBy = proexp.CreatedBy,
                                         CreatedByName = empc.DisplayName,
                                         CreateOn = proexp.CreatedOn,
                                         UpdateBy = proexp.UpdatedBy,
                                         UpdateByName = db.Employee.Where(x => x.EmployeeId == proexp.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault(),
                                         ExpenseName = proexp.ExpenseName,
                                         TotalAmount = db.ProjectExpCostAmts.Where(x => x.ExpenseId == proexp.ProjectExpId).Select(x => x.Amount).Sum(),
                                     }).ToListAsync();
                if (expense.Count > 0)
                {
                    res.Data = expense;
                    res.Status = true;
                    res.Message = "Project Expense List Of Selected Date";
                }
                else
                {
                    res.Data = expense;
                    res.Status = false;
                    res.Message = "Not Expense Found On This Date";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get expense List Behalf of Expense Date

        #region Get All projects For DropDown

        /// <summary>
        /// Create by Shriya Malvi On 19-07-202
        /// API >> Get  >> api/projectexpense/getallprojectsdd
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallprojectsdd")]
        public async Task<ResponseBodyModel> GetAllProjectsDD()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var DataProject = await (from ad in db.ProjectLists
                                         join fd in db.Employee on ad.ProjectManager equals fd.EmployeeId
                                         where ad.IsActive == true && ad.IsDeleted == false &&
                                         ad.CompanyId == claims.companyId
                                         select new GetProjectHelperClass
                                         {
                                             ID = ad.ID,
                                             ProjectName = ad.ProjectName,
                                         }).ToListAsync();
                if (DataProject.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Project List Found for DropDwon";
                    res.Data = DataProject;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Project List Not  Found for DropDwon";
                    res.Data = DataProject;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get All projects For DropDown

        #region upload expensee

        /// <summary>
        /// Created By Shriya Malvi On 19-07-2022
        /// API >> Post >> api/projectexpense/uploadprojectexpenseinvoice
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadprojectexpenseinvoice")]
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
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/ProjectExpense/ExpenseInvoice/" + claims.companyId + "/"), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\ProjectExpense\\ExpenseInvoice\\" + claims.companyId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion upload expensee

        #region Get Expense List Behalf On Selected Expense Date And Particular project Id

        /// <summary>
        /// Create By Shriya Malvi On 21-07-2022
        /// API >> Get >> api/projectexpense/getexpensehistorybyprojectid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getexpensehistorybyprojectid")]
        public async Task<ResponseBodyModel> GetExpenseHistoryByProjectId(DateTime Date, int ProjectId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<GetHelperForExpense> ListExpense = new List<GetHelperForExpense>();
            try
            {
                var ProjectExpense = await db.ProjectExpenseMasters.Where(x => x.IsActive == true && x.IsDeleted == false && x.ProjectId == ProjectId && x.CompanyId == claims.companyId
                && x.ExpenseDate.Month == Date.Month && x.ExpenseDate.Year == Date.Year).ToListAsync();
                var ExpenseId = ProjectExpense.Select(x => x.ProjectExpId).Distinct().ToList();
                foreach (var item in ExpenseId)
                {
                    var exp = ProjectExpense.Where(x => x.ProjectExpId == item).First();
                    GetHelperForExpense obj = new GetHelperForExpense()
                    {
                        ProjectId = exp.ProjectId,
                        ProjectExpenseId = exp.ProjectExpId,
                        ProjectName = db.ProjectLists.Where(x => x.ID == ProjectId && x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).Select(x => x.ProjectName).First(),
                        ExpenseDate = exp.ExpenseDate,
                        ExpenseInvoice = exp.ExpenseInvoice,
                        CreatedBy = exp.CreatedBy,
                        CreatedByName = db.Employee.Where(x => x.EmployeeId == exp.CreatedBy).Select(x => x.DisplayName).FirstOrDefault(),
                        CreateOn = exp.CreatedOn,
                        UpdateBy = exp.UpdatedBy,
                        UpdateByName = db.Employee.Where(x => x.EmployeeId == exp.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault(),
                        ExpenseName = exp.ExpenseName,
                        TotalAmount = db.ProjectExpCostAmts.Where(x => x.ExpenseId == exp.ProjectExpId && x.CompanyId == claims.companyId).Select(x => x.Amount).Sum(),
                    };
                    ListExpense.Add(obj);
                }
                if (ListExpense.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Expense List Behalf on Selected Date and Project Id ";
                    res.Data = ListExpense;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Expense List not found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get Expense List Behalf On Selected Expense Date And Particular project Id

        #region History Behalf on Project Id

        /// <summary>
        /// Create By Shriya Malvi On 21-07-2022
        /// API >> Get >> api/projectexpense/histroybehalfonprojectid
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("histroybehalfonprojectid")]
        public async Task<ResponseBodyModel> HistroyBehalfOnProjectId(int ProjectId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<GetHelperForExpense> ListExpense = new List<GetHelperForExpense>();
            try
            {
                var ProjectExpense = await db.ProjectExpenseMasters.Where(x => x.IsActive == true && x.IsDeleted == false && x.ProjectId == ProjectId && x.CompanyId == claims.companyId).ToListAsync();
                var ExpenseId = ProjectExpense.Select(x => x.ProjectExpId).Distinct().ToList();
                foreach (var item in ExpenseId)
                {
                    var exp = ProjectExpense.Where(x => x.ProjectExpId == item).First();
                    GetHelperForExpense obj = new GetHelperForExpense();

                    obj.ProjectId = exp.ProjectId;
                    obj.ProjectExpenseId = exp.ProjectExpId;
                    obj.ProjectName = db.ProjectLists.Where(x => x.ID == ProjectId && x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).Select(x => x.ProjectName).First();
                    obj.ExpenseDate = exp.ExpenseDate;
                    obj.ExpenseInvoice = exp.ExpenseInvoice;
                    obj.CreatedBy = exp.CreatedBy;
                    obj.CreatedByName = db.Employee.Where(x => x.EmployeeId == exp.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                    obj.CreateOn = exp.CreatedOn;
                    obj.UpdateBy = exp.UpdatedBy;
                    obj.UpdateByName = db.Employee.Where(x => x.EmployeeId == exp.UpdatedBy).Select(x => x.DisplayName).FirstOrDefault();
                    obj.ExpenseName = exp.ExpenseName;
                    obj.TotalAmount = db.ProjectExpCostAmts.Where(x => x.ExpenseId == exp.ProjectExpId && x.CompanyId == claims.companyId).Select(x => x.Amount).Sum();

                    ListExpense.Add(obj);
                }
                if (ListExpense.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Project Expense List";
                    res.Data = ListExpense;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Project Expense List Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion History Behalf on Project Id

        #region Helper Classes For Project

        public class GetProjectHelperClass
        {
            public int ID { get; set; }
            public string ProjectName { get; set; }
        }

        public class GetHelperForExpense
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int ProjectExpenseId { get; set; }
            public DateTime CreateOn { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
            public int? UpdateBy { get; set; }
            public string UpdateByName { get; set; }
            public double? TotalAmount { get; set; }
            public DateTime ExpenseDate { get; set; }

            // public List<CostAmtList> CostAmtList { get; set; }
            public string ExpenseInvoice { get; set; }

            public string ExpenseName { get; set; }
            //public string CostName { get; set; }
            //public double Amount { get; set; }
        }

        public class CreateExpenseHelper
        {
            public int? ExpenseId { get; set; }
            public int ProjectId { get; set; }
            public DateTime ExpenseDate { get; set; }
            public List<CostAmtList> CostAmtList { get; set; }
            public string ExpenseInvoice { get; set; }
            public string ExpenseName { get; set; }

            [NotMapped]
            public bool IsRequired { get; set; }
        }

        public class CostAmtList
        {
            public string CostName { get; set; }
            public double Amount { get; set; }
        }

        #endregion Helper Classes For Project
    }
}