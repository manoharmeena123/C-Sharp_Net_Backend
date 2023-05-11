using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.AttendanceModel;
using AspNetIdentity.WebApi.Model.EmployeeModel;
using AspNetIdentity.WebApi.Models;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Excel
{
    /// <summary>
    /// Created By Suraj Bundel On 20-09-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/excelimport")]
    public class ExcelImportController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        //#region Api To Add Employee By Excel

        ///// <summary>
        ///// Created By Suraj Bundel on 20-09-2022
        ///// API >> api/excelimport/addemployeebyexcelimport
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("addemployeebyexcelimport")]
        //public async Task<ResponseBodyModel> AddEmployeeByExcel(List<EmployeeImportFaultyLogs> item)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    List<EmployeeImportFaultyLogs> falultyImportItem = new List<EmployeeImportFaultyLogs>();
        //    long successfullImported = 0;
        //    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //    try
        //    {
        //        if (item == null)
        //        {
        //            res.Message = "Error";
        //            res.Status = false;
        //            res.Data = falultyImportItem;
        //            return res;
        //        }
        //        if (item.Count > 0)
        //        {
        //            var shiftdata = _db.ShiftGroups.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
        //         && x.ShiftName == "Default Shift").FirstOrDefault();
        //            var OrgList = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
        //            var departmentList = await _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
        //            var desingationList = await _db.Designation.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
        //            foreach (var model in item)
        //            {
        //                if (String.IsNullOrEmpty(model.OfficeEmail))

        //                {
        //                    model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
        //                    falultyImportItem.Add(model);
        //                }
        //                else
        //                {
        //                    if (String.IsNullOrWhiteSpace(model.OfficeEmail))
        //                    {
        //                        model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
        //                        falultyImportItem.Add(model);
        //                    }
        //                    else
        //                    {
        //                        //var department = departmentList.Where(x=>x.DepartmentName.Trim())
        //                        var checkEmp = _db.Employee.Select(x => x.OfficeEmail.Trim().ToUpper())
        //                .Contains(model.OfficeEmail.Trim().ToUpper());
        //                        if (checkEmp)
        //                        {
        //                            res.Message = "Import Done";
        //                            res.Status = true;
        //                        }
        //                        else
        //                        {
        //                            var firstName = textInfo.ToTitleCase(model.FirstName.Trim());
        //                            var middleName = String.IsNullOrEmpty(textInfo.ToTitleCase(model.MiddleName)) ? "" : model.MiddleName;
        //                            var lastName = textInfo.ToTitleCase(model.LastName.Trim());

        //                            var Password = model.Password.Trim();
        //                            var hashKey = DataHelper.GeneratePasswords(10);
        //                            var encPassword = DataHelper.EncodePassword(Password, hashKey);
        //                            byte Levels = 4;

        //                            var split = model.OfficeEmail.Split('@');
        //                            model.OfficeEmail = split[0] + "@" + split[1].ToLower();

        //                            var departmentId = departmentList.Where(x => x.DepartmentName.Trim() == model.DepartmentName.Trim()).Select(x => x.DepartmentId).FirstOrDefault();
        //                            if (departmentId == 0)
        //                            {
        //                                model.FailReason = "Department Name Not Found Or Wrong Department Name Inputed";
        //                                falultyImportItem.Add(model);
        //                            }
        //                            else
        //                            {
        //                                var designationId = desingationList.Where(x => x.DepartmentId == departmentId && x.DesignationName.Trim() == model.DesignationName.Trim()).Select(x => x.DesignationId).FirstOrDefault();
        //                                if (designationId == 0)
        //                                {
        //                                    model.FailReason = "Designation Name Not Found Or Wrong Designation Name Inputed";
        //                                    falultyImportItem.Add(model);
        //                                }
        //                                else
        //                                {
        //                                    if (claims.orgId == 0)
        //                                        if (!String.IsNullOrEmpty(model.OrganizationName) && !String.IsNullOrWhiteSpace(model.OrganizationName))
        //                                            claims.orgId = OrgList.Where(x => x.OrgName == model.OrganizationName).Select(x => x.OrgId).FirstOrDefault();
        //                                    if (claims.orgId == 0)
        //                                    {
        //                                        model.FailReason = "Organization Name Not Found Or Wrong Organization Name Inputed";
        //                                        falultyImportItem.Add(model);
        //                                    }
        //                                    else
        //                                    {
        //                                        if (claims.orgId != 0 && designationId != 0 && departmentId != 0)
        //                                        {
        //                                            var user = new ApplicationUser()
        //                                            {
        //                                                FirstName = firstName,
        //                                                LastName = lastName,
        //                                                PhoneNumber = model.MobilePhone,
        //                                                Level = Levels,
        //                                                JoinDate = DateTime.Now,
        //                                                EmailConfirmed = true,
        //                                                Email = model.OfficeEmail,
        //                                                PasswordHash = hashKey,
        //                                                UserName = model.OfficeEmail,
        //                                                CompanyId = claims.companyId,
        //                                            };
        //                                            IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
        //                                            try
        //                                            {
        //                                                if (result.Succeeded)
        //                                                {
        //                                                    Employee empObj = new Employee();
        //                                                    empObj.FirstName = firstName;
        //                                                    empObj.MiddleName = middleName;
        //                                                    empObj.LastName = lastName;
        //                                                    empObj.DisplayName = String.IsNullOrEmpty(middleName) ? firstName + " " + lastName :
        //                                                        firstName + " " + middleName + " " + lastName;
        //                                                    empObj.FatherName = model.FatherName;
        //                                                    empObj.MotherName = model.MotherName;
        //                                                    empObj.Gender = textInfo.ToTitleCase(model.Gender);
        //                                                    // empObj.DateOfBirth =model.DateOfBirth.Date.AddDays(1);
        //                                                    empObj.DateOfBirth = model.DateOfBirth.Value.AddDays(1);
        //                                                    empObj.BloodGroup = (BloodGroupEnum)System.Enum.Parse(typeof(BloodGroupEnum), (model.BloodGroup.Contains("+") ? model.BloodGroup.Replace("+", "_pos") : model.BloodGroup.Replace("-", "_neg")));
        //                                                    empObj.MaritalStatus = model.MaritalStatus;
        //                                                    //empObj.ConfirmationDate = model.ConfirmationDate.Date.AddDays(1);
        //                                                    //empObj.JoiningDate = model.JoiningDate.Date.AddDays(1);
        //                                                    empObj.ConfirmationDate = model.ConfirmationDate.Value.AddDays(1);
        //                                                    empObj.JoiningDate = model.JoiningDate.Value.AddDays(1);
        //                                                    empObj.CreatedBy = claims.employeeId;
        //                                                    empObj.CreatedOn = DateTime.Now;
        //                                                    empObj.IsActive = true;
        //                                                    empObj.IsDeleted = false;
        //                                                    empObj.DepartmentId = departmentId;
        //                                                    empObj.DesignationId = designationId;
        //                                                    empObj.AadharNumber = model.AadharNumber;
        //                                                    empObj.RoleId = 0;
        //                                                    empObj.EmployeeTypeId = (EmployeeTypeEnum)(model.EmployeeType == null ?
        //                                                            EmployeeTypeEnum.Confirmed_Employee :
        //                                                            Enum.Parse(typeof(EmployeeTypeEnum), model.EmployeeType.Replace(" ", "_")));
        //                                                    empObj.EmergencyNumber = model.EmergencyNumber;
        //                                                    empObj.WhatsappNumber = model.WhatsappNumber;
        //                                                    empObj.Password = Password;
        //                                                    empObj.OfficeEmail = model.OfficeEmail;
        //                                                    empObj.MobilePhone = model.MobilePhone;
        //                                                    empObj.CompanyId = claims.companyId;
        //                                                    empObj.OrgId = claims.orgId;
        //                                                    empObj.PanNumber = model.PanNumber;
        //                                                    empObj.PersonalEmail = model.PersonalEmail;
        //                                                    empObj.PermanentAddress = model.PermanentAddress;
        //                                                    empObj.LocalAddress = model.LocalAddress;
        //                                                    empObj.BankAccountNumber = model.BankAccountNumber;
        //                                                    empObj.IFSC = model.IFSC;
        //                                                    empObj.AccountHolderName = model.AccountHolderName;
        //                                                    empObj.BankName = model.BankName;
        //                                                    empObj.BiometricId = model.BiometricId;
        //                                                    empObj.GrossSalery = model.Salary; //shriya
        //                                                    empObj.EmployeeCode = model.EmployeeCode;
        //                                                    empObj.ShiftGroupId = model.ShiftGroupId == Guid.Empty ? shiftdata.ShiftGoupId : model.ShiftGroupId;

        //                                                    _db.Employee.Add(empObj);
        //                                                    await _db.SaveChangesAsync();

        //                                                    User userObj = new User();
        //                                                    userObj.EmployeeId = empObj.EmployeeId;
        //                                                    userObj.UserName = empObj.OfficeEmail;
        //                                                    userObj.Password = encPassword;
        //                                                    userObj.HashCode = hashKey;
        //                                                    userObj.DepartmentId = departmentId;
        //                                                    userObj.LoginId = (LoginRolesEnum)System.Enum.Parse(typeof(LoginRolesEnum), model.LoginType);
        //                                                    userObj.CreatedOn = DateTime.Now;
        //                                                    userObj.IsActive = true;
        //                                                    userObj.IsDeleted = false;
        //                                                    userObj.CompanyId = claims.companyId;
        //                                                    userObj.OrgId = claims.orgId;

        //                                                    _db.User.Add(userObj);
        //                                                    await _db.SaveChangesAsync();
        //                                                    res.Message = "Employee Add";
        //                                                    res.Status = true;
        //                                                }
        //                                            }
        //                                            catch (Exception)
        //                                            {
        //                                                await this.AppUserManager.DeleteAsync(user);
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            if (falultyImportItem.Count > 0)
        //            {
        //                EmployeeImportFaultyLogsGoups faultygroupobj = new EmployeeImportFaultyLogsGoups
        //                {
        //                    EmployeeGroupId = Guid.NewGuid(),
        //                    TotalImported = item.Count,
        //                    SuccessFullImported = successfullImported,
        //                    UnSuccessFullImported = falultyImportItem.Count,
        //                    CreatedBy = claims.employeeId,
        //                    CreatedOn = DateTime.Now,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    CompanyId = claims.companyId,
        //                    OrgId = claims.orgId,
        //                };
        //                _db.EmployeeImportFaultieGoups.Add(faultygroupobj);
        //                await _db.SaveChangesAsync();
        //                falultyImportItem.ForEach(x =>
        //                {
        //                    x.EmployeeFaultyId = Guid.NewGuid();
        //                    x.EmployeeGroups = faultygroupobj;
        //                });
        //                _db.EmployeeImportFaultieLogs.AddRange(falultyImportItem);
        //                await _db.SaveChangesAsync();

        //                if ((item.Count - falultyImportItem.Count) > 0)
        //                {
        //                    res.Message = "Employee Imported Succesfull Of " +
        //                    (item.Count - falultyImportItem.Count) + " Fields And " +
        //                    falultyImportItem.Count + " Fields Are Not Imported";
        //                    res.Status = true;
        //                    res.Data = falultyImportItem;
        //                }
        //                else
        //                {
        //                    res.Message = "All Fields Are Not Imported";
        //                    res.Status = true;
        //                    res.Data = falultyImportItem;
        //                }
        //            }
        //            else
        //            {
        //                res.Message = "Data Added Successfully Of All Fields";
        //                res.Status = true;
        //                res.Data = falultyImportItem;
        //            }
        //        }
        //        else
        //        {
        //            res.Message = "0 Employee Add";
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

        //#endregion Api To Add Employee By Excel

        //#region Assign Reporting manager By Excel Upload // not in use

        ///// <summary>
        ///// Created By Suraj Bundel on 20-09-2022
        ///// API >> api/employeenew/addemployeebyexcelimport
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("addreportingmanagerbyexcel")]
        //public async Task<ResponseBodyModel> AddReportingManagerByExcel(List<EmployeeImportFaultyLogs> item)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    List<EmployeeImportFaultyLogs> falultyImportItem = new List<EmployeeImportFaultyLogs>();
        //    long successfullImported = 0;
        //    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //    try
        //    {
        //        if (item == null)
        //        {
        //            res.Message = "Error";
        //            res.Status = false;
        //            res.Data = falultyImportItem;
        //            return res;
        //        }
        //        else
        //        {
        //            if (item.Count > 0)
        //            {
        //                var employeedata = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
        //                foreach (var model in item)
        //                {
        //                    if ((String.IsNullOrEmpty(model.OfficeEmail)) || (String.IsNullOrWhiteSpace(model.OfficeEmail)))

        //                    {
        //                        model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
        //                        falultyImportItem.Add(model);
        //                    }
        //                    else
        //                    {

        //                        if ((String.IsNullOrEmpty(model.ReportingManager)) || (String.IsNullOrWhiteSpace(model.ReportingManager)))

        //                        {
        //                            model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
        //                            falultyImportItem.Add(model);
        //                        }
        //                        else
        //                        {
        //                            var reporting = employeedata.Where(x => x.OfficeEmail == model.ReportingManager).Select(x => x.EmployeeId).FirstOrDefault();
        //                            var employeeset = employeedata.FirstOrDefault();
        //                            var employeename = employeedata.Where(x => x.OfficeEmail == model.OfficeEmail).Select(x => x.EmployeeId).FirstOrDefault();
        //                            employeeset.ReportingManager = reporting;
        //                            _db.Entry(employeeset).State = System.Data.Entity.EntityState.Modified;
        //                            await _db.SaveChangesAsync();
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //        if (falultyImportItem.Count > 0)
        //        {
        //            EmployeeImportFaultyLogsGoups faultygroupobj = new EmployeeImportFaultyLogsGoups
        //            {
        //                EmployeeGroupId = Guid.NewGuid(),
        //                TotalImported = item.Count,
        //                SuccessFullImported = successfullImported,
        //                UnSuccessFullImported = falultyImportItem.Count,
        //                CreatedBy = claims.employeeId,
        //                CreatedOn = DateTime.Now,
        //                IsActive = true,
        //                IsDeleted = false,
        //                CompanyId = claims.companyId,
        //                OrgId = claims.orgId,
        //            };
        //            _db.EmployeeImportFaultieGoups.Add(faultygroupobj);
        //            await _db.SaveChangesAsync();
        //            falultyImportItem.ForEach(x =>
        //            {
        //                x.EmployeeFaultyId = Guid.NewGuid();
        //                x.EmployeeGroups = faultygroupobj;
        //            });
        //            _db.EmployeeImportFaultieLogs.AddRange(falultyImportItem);
        //            await _db.SaveChangesAsync();

        //            if ((item.Count - falultyImportItem.Count) > 0)
        //            {
        //                res.Message = "Employee Imported Succesfull Of " +
        //                (item.Count - falultyImportItem.Count) + " Fields And " +
        //                falultyImportItem.Count + " Fields Are Not Imported";
        //                res.Status = true;
        //                res.Data = falultyImportItem;
        //            }
        //            else
        //            {
        //                res.Message = "All Fields Are Not Imported";
        //                res.Status = true;
        //                res.Data = falultyImportItem;
        //            }
        //        }
        //        else
        //        {
        //            res.Message = "Data Added Successfully Of All Fields";
        //            res.Status = true;
        //            res.Data = falultyImportItem;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        #region Api To Add Attendance By Excel

        /// <summary>
        /// Created By Suraj Bundel on 20-09-2022
        /// API >> api/excelimport/addattendancebyexcelimport
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addattendancebyexcelimport")]
        public async Task<ResponseBodyModel> AddAttendanceByExcel(List<AttendanceImportFaultyLogs> item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<AttendanceImportFaultyLogs> falultyImportItem = new List<AttendanceImportFaultyLogs>();
            long successfullImported = 0;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                if (item == null)
                {
                    res.Message = "Excel List is Empty";
                    res.Status = false;
                    res.Data = falultyImportItem;
                    return res;
                }
                else
                {
                    foreach (var model in item)
                    {
                        if ((model.Month > 0) || (model.Year > 0))
                        {
                            model.FailReason = "Input valid Year or Month ";
                            falultyImportItem.Add(model);
                        }
                        else
                        //{
                        //    if (String.IsNullOrWhiteSpace(model.OfficeEmail))
                        //    {
                        //        model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
                        //        falultyImportItem.Add(model);
                        //    }
                        //    else
                        {
                            var checkEmp = _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted && x.OfficeEmail == model.OfficeEmail).Select(x => x.EmployeeId).FirstOrDefault();

                            if (checkEmp > 0)
                            {
                                model.FailReason = "Employee Data not Found";
                                falultyImportItem.Add(model);

                            }
                            else
                            {
                                //AttendanceImportFaultyLogsNew repo = new AttendanceImportFaultyLogsNew();
                                Attendencereport repo = new Attendencereport();
                                repo.CompanyId = claims.companyId;
                                repo.OrgId = claims.orgId;
                                repo.EmployeeId = checkEmp;
                                repo.IsActive = true;
                                repo.IsDeleted = false;
                                repo.CreatedBy = claims.employeeId;
                                repo.CreatedOn = DateTime.Now;



                                repo.One = model.Twentyone.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentyone.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentyone.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentyone.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentyone.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Two = model.Two.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Two.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Two.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Two.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Two.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Three = model.Three.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Three.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Three.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Three.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Three.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Four = model.Four.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Four.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Four.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Four.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Four.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Five = model.Five.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Five.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Five.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Five.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Five == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Six = model.Six.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Six.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Six.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Six.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Six.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Seven = model.Seven.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Seven.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Seven.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Seven.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Seven.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Eight = model.Eight.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Eight.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Eight.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Eight.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Eight.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Nine = model.Nine.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Nine.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Nine.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Nine.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Nine.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Ten = model.Ten.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Ten.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Ten.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Ten.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Ten.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Eleven = model.Eleven.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Eleven.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Eleven.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Eleven.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Eleven.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twelve = model.Twelve.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twelve.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twelve.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twelve.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twelve.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Thirteen = model.Thirteen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Thirteen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Thirteen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Thirteen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Thirteen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Fourteen = model.Fourteen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Fourteen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Fourteen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Fourteen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Fourteen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Fifteen = model.Fifteen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Fifteen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Fifteen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Fifteen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Fifteen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Sixteen = model.Sixteen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Sixteen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Sixteen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Sixteen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Sixteen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Seventeen = model.Seventeen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Seventeen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Seventeen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Seventeen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Seventeen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Eighteen = model.Eighteen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Eighteen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Eighteen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Eighteen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Eighteen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Nineteen = model.Nineteen.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Nineteen.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Nineteen.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Nineteen.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Nineteen.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twenty = model.Twenty.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twenty.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twenty.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twenty.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twenty.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentyone = model.Twentyone.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentyone.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentyone.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentyone.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentyone.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentytwo = model.Twentytwo.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentytwo.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentytwo.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentytwo.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentytwo.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentythree = model.Twentythree.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentythree.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentythree.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentythree.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentythree.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentyfour = model.Twentyfour.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentyfour.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentyfour.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentyfour.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentyfour.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentyfive = model.Twentyfive.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentyfive.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentyfive.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentyfive.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentyfive.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentysix = model.Twentysix.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentysix.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentysix.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentysix.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentysix.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentyseven = model.Twentyseven.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentyseven.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentyseven.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentyseven.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentyseven.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentyeight = model.Twentyeight.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentyeight.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentyeight.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentyeight.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentyeight.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Twentynine = model.Twentynine.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Twentynine.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Twentynine.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Twentynine.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Twentynine.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Thirty = model.Thirty.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Thirty.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Thirty.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Thirty.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Thirty.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;



                                repo.Thirtyone = model.Thirtyone.ToUpper() == "P" ? AttendenceTypeConstants.Present : model.Thirtyone.ToUpper() == "L" ? AttendenceTypeConstants.Leave : model.Thirtyone.ToUpper() == "HF" ? AttendenceTypeConstants.HalfDay : model.Thirtyone.ToUpper() == "HD" ? AttendenceTypeConstants.Holiday : model.Thirtyone.ToUpper() == "WO" ? AttendenceTypeConstants.WeekOf : AttendenceTypeConstants.NotAvailable;
                                repo.Month = model.Month;
                                repo.Year = model.Year;
                                _db.Attendencereports.Add(repo);
                                await _db.SaveChangesAsync();
                                res.Message = "Employee Attendance Added";
                                res.Status = true;
                            }
                        }
                    }
                    if (falultyImportItem.Count > 0)
                    {
                        AttendanceImportFaultyLogsGroup faultygroupobj = new AttendanceImportFaultyLogsGroup
                        {
                            AttendanceGroupId = Guid.NewGuid(),
                            TotalImported = item.Count,
                            SuccessFullImported = successfullImported,
                            UnSuccessFullImported = falultyImportItem.Count,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                        };
                        _db.AttendanceImportFaultyLogsGoups.Add(faultygroupobj);
                        await _db.SaveChangesAsync();
                        falultyImportItem.ForEach(x =>
                        {
                            x.AttendanceFaultyId = Guid.NewGuid();
                            x.AttendanceGroups = faultygroupobj;
                        });
                        _db.AttendanceImportFaultiesLogs.AddRange(falultyImportItem);
                        await _db.SaveChangesAsync();

                        if ((item.Count - falultyImportItem.Count) > 0)
                        {
                            res.Message = "Employee Attendance Imported Succesfull Of " +
                            (item.Count - falultyImportItem.Count) + " Fields And " +
                            falultyImportItem.Count + " Fields Are Not Imported";
                            res.Status = true;
                            res.Data = falultyImportItem;
                        }
                        else
                        {
                            res.Message = "All Fields Are Not Imported";
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
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        //public class AttendanceImportFaultyLogsNew
        //{
        //    public Guid AttendanceFaultyId { get; set; }
        //    public virtual AttendanceImportFaultyLogsGroup AttendanceGroups { get; set; }
        //    public int AttendencereportId { get; set; }
        //    public string One { get; set; }
        //    public string Two { get; set; }
        //    public string Three { get; set; }
        //    public string Four { get; set; }
        //    public string Five { get; set; }
        //    public string Six { get; set; }
        //    public string Seven { get; set; }
        //    public string Eight { get; set; }
        //    public string Nine { get; set; }
        //    public string Ten { get; set; }
        //    public string Eleven { get; set; }
        //    public string Twelve { get; set; }
        //    public string Thirteen { get; set; }
        //    public string Fourteen { get; set; }
        //    public string Fifteen { get; set; }
        //    public string Sixteen { get; set; }
        //    public string Seventeen { get; set; }
        //    public string Eighteen { get; set; }
        //    public string Nineteen { get; set; }
        //    public string Twenty { get; set; }
        //    public string Twentyone { get; set; }
        //    public string Twentytwo { get; set; }
        //    public string Twentythree { get; set; }
        //    public string Twentyfour { get; set; }
        //    public string Twentyfive { get; set; }
        //    public string Twentysix { get; set; }
        //    public string Twentyseven { get; set; }
        //    public string Twentyeight { get; set; }
        //    public string Twentynine { get; set; }
        //    public string Thirty { get; set; }
        //    public string Thirtyone { get; set; }
        //    public int Month { get; set; }
        //    public int Year { get; set; }
        //    public int EmployeeId { get; set; }
        //    public string OfficeEmail { get; set; }
        //    public string FailReason { get; set; }
        //}


        //if (check == null)
        //{
        //    _db.AssetsItemMasters.Add(addassets);
        //    await _db.SaveChangesAsync();
        //}
        //else
        //{
        //    _db.Entry(addassets).State = System.Data.Entity.EntityState.Modified;
        //    await _db.SaveChangesAsync();
        //}
        //}

        //                if (checkEmp)
        //                {
        //                    res.Message = "Import Done";
        //                    res.Status = true;
        //                }
        //                else
        //                {
        //                    var firstName = textInfo.ToTitleCase(model.FirstName.Trim());
        //                    var middleName = String.IsNullOrEmpty(textInfo.ToTitleCase(model.MiddleName)) ? "" : model.MiddleName;
        //                    var lastName = textInfo.ToTitleCase(model.LastName.Trim());

        //                    var Password = model.Password.Trim();
        //                    var hashKey = DataHelper.GeneratePasswords(10);
        //                    var encPassword = DataHelper.EncodePassword(Password, hashKey);
        //                    byte Levels = 4;

        //                    var split = model.OfficeEmail.Split('@');
        //                    model.OfficeEmail = split[0] + "@" + split[1].ToLower();

        //                    var departmentId = departmentList.Where(x => x.DepartmentName.Trim() == model.DepartmentName.Trim()).Select(x => x.DepartmentId).FirstOrDefault();
        //                    if (departmentId == 0)
        //                    {
        //                        model.FailReason = "Department Name Not Found Or Wrong Department Name Inputed";
        //                        falultyImportItem.Add(model);
        //                    }
        //                    else
        //                    {
        //                        var designationId = desingationList.Where(x => x.DepartmentId == departmentId && x.DesignationName.Trim() == model.DesignationName.Trim()).Select(x => x.DesignationId).FirstOrDefault();
        //                        if (designationId == 0)
        //                        {
        //                            model.FailReason = "Designation Name Not Found Or Wrong Designation Name Inputed";
        //                            falultyImportItem.Add(model);
        //                        }
        //                        else
        //                        {
        //                            if (claims.orgid == 0)
        //                                if (!String.IsNullOrEmpty(model.OrganizationName) && !String.IsNullOrWhiteSpace(model.OrganizationName))
        //                                    claims.orgid = OrgList.Where(x => x.OrgName == model.OrganizationName).Select(x => x.OrgId).FirstOrDefault();
        //                            if (claims.orgid == 0)
        //                            {
        //                                model.FailReason = "Organization Name Not Found Or Wrong Organization Name Inputed";
        //                                falultyImportItem.Add(model);
        //                            }
        //                            else
        //                            {
        //                                if (claims.orgid != 0 && designationId != 0 && departmentId != 0)
        //                                {
        //                                    var user = new ApplicationUser()
        //                                    {
        //                                        FirstName = firstName,
        //                                        LastName = lastName,
        //                                        PhoneNumber = model.MobilePhone,
        //                                        Level = Levels,
        //                                        JoinDate = DateTime.Now,
        //                                        EmailConfirmed = true,
        //                                        Email = model.OfficeEmail,
        //                                        PasswordHash = hashKey,
        //                                        UserName = model.OfficeEmail,
        //                                        CompanyId = claims.companyid,
        //                                    };
        //                                    IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
        //                                    try
        //                                    {
        //                                        if (result.Succeeded)
        //                                        {
        //                                            Employee empObj = new Employee();


        //                                            _db.Employee.Add(empObj);
        //                                            await _db.SaveChangesAsync();

        //                                            User userObj = new User();
        //                                            userObj.EmployeeId = empObj.EmployeeId;
        //                                            userObj.UserName = empObj.OfficeEmail;
        //                                            userObj.Password = encPassword;
        //                                            userObj.HashCode = hashKey;
        //                                            userObj.DepartmentId = departmentId;
        //                                            userObj.LoginId = (LoginRolesEnum)System.Enum.Parse(typeof(LoginRolesEnum), model.LoginType);
        //                                            userObj.CreatedOn = DateTime.Now;
        //                                            userObj.IsActive = true;
        //                                            userObj.IsDeleted = false;
        //                                            userObj.CompanyId = claims.companyid;
        //                                            userObj.OrgId = claims.orgid;

        //                                            _db.User.Add(userObj);
        //                                            await _db.SaveChangesAsync();
        //                                            res.Message = "Employee Add";
        //                                            res.Status = true;
        //                                        }
        //                                    }
        //                                    catch (Exception)
        //                                    {
        //                                        await this.AppUserManager.DeleteAsync(user);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (falultyImportItem.Count > 0)
        //    {
        //        EmployeeImportFaultyLogsGoups faultygroupobj = new EmployeeImportFaultyLogsGoups
        //        {
        //            EmployeeGroupId = Guid.NewGuid(),
        //            TotalImported = item.Count,
        //            SuccessFullImported = successfullImported,
        //            UnSuccessFullImported = falultyImportItem.Count,
        //            CreatedBy = claims.employeeid,
        //            CreatedOn = DateTime.Now,
        //            IsActive = true,
        //            IsDeleted = false,
        //            CompanyId = claims.companyid,
        //            OrgId = claims.orgid,
        //        };
        //        _db.EmployeeImportFaultieGoups.Add(faultygroupobj);
        //        await _db.SaveChangesAsync();
        //        falultyImportItem.ForEach(x =>
        //        {
        //            x.EmployeeFaultyId = Guid.NewGuid();
        //            x.EmployeeGroups = faultygroupobj;
        //        });
        //        _db.EmployeeImportFaultieLogs.AddRange(falultyImportItem);
        //        await _db.SaveChangesAsync();

        //        if ((item.Count - falultyImportItem.Count) > 0)
        //        {
        //            res.Message = "Employee Imported Succesfull Of " +
        //            (item.Count - falultyImportItem.Count) + " Fields And " +
        //            falultyImportItem.Count + " Fields Are Not Imported";
        //            res.Status = true;
        //            res.Data = falultyImportItem;
        //        }
        //        else
        //        {
        //            res.Message = "All Fields Are Not Imported";
        //            res.Status = true;
        //            res.Data = falultyImportItem;
        //        }
        //    }
        //    else
        //    {
        //        res.Message = "Data Added Successfully Of All Fields";
        //        res.Status = true;
        //        res.Data = falultyImportItem;
        //    }
        //}
        //else
        //{
        //    res.Message = "0 Employee Add";
        //    res.Status = false;
        //}


        #endregion Api To Add Employee By Excel

        #region AddEmployeeByImport model

        /// <summary>
        /// Created By Suraj Bundel On 20-09-2022
        /// </summary>
        public class AddEmployeeByImport
        {
            public int EmployeeId { get; set; }
            /// <summary>
            /// --------------------------------------  Personal Information
            /// </summary>
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string BloodGroup { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string MobilePhone { get; set; }
            public string EmergencyNumber { get; set; }
            public string WhatsappNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string PermanentAddress { get; set; }
            public string LocalAddress { get; set; }

            /// <summary>
            /// ------------------------------------------  Official Details
            /// </summary>
            public string LoginType { get; set; }

            public string EmployeeType { get; set; }
            public string DepartmentName { get; set; }
            public int Description { get; set; }
            public DateTime JoiningDate { get; set; }
            public DateTime ConfirmationDate { get; set; }
            public string DesignationName { get; set; }
            public string BiometricId { get; set; }

            /// <summary>
            /// ------------------------------------------  Bank Info Details
            /// </summary>
            public string BankAccountNumber { get; set; }

            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }

            /// <summary>
            /// ------------------------------------------  Credentials
            /// </summary>
            public string OfficeEmail { get; set; }

            public string Password { get; set; }
            public double Salary { get; set; }
            public string EmployeeCode { get; set; }
            public string OrganizationName { get; set; }
        }
        #endregion


        #region Department Import File
        /// <summary>
        /// created by Mayank Prajapati on 26/9/2022
        /// Api >> Post >> api/excelimport/adddepartmentbyexcelimport
        /// </summary>
        [HttpPost]
        [Route("adddepartmentbyexcelimport")]
        public async Task<ResponseBodyModel> DepartmentImport(List<DepartmentImportFaultyLogs> Item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<DepartmentImportFaultyLogs> falultyImportItem = new List<DepartmentImportFaultyLogs>();
            long successfullImported = 0;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                if (Item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    res.Data = falultyImportItem;
                    return res;
                }
                if (Item.Count > 0)
                {
                    //var OrgList = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyid && x.IsActive && !x.IsDeleted).ToListAsync();
                    var departmentList = await _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                    //var desingationList = await _db.Designation.Where(x => x.CompanyId == claims.companyid && x.IsActive && !x.IsDeleted).ToListAsync();
                    foreach (var model in Item)
                    {
                        //if ((String.IsNullOrEmpty(model.OfficeEmail))|| (String.IsNullOrWhiteSpace(model.OfficeEmail)))
                        //{
                        //    model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
                        //    falultyImportItem.Add(model);
                        //}
                        //else
                        //{
                        //if (String.IsNullOrWhiteSpace(model.OfficeEmail))
                        //{
                        //    model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
                        //    falultyImportItem.Add(model);
                        //}
                        //else
                        //{


                        var depCheckData = await _db.Department.Where(x => x.IsActive &&
                                !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                        foreach (var Model in Item)
                        {
                            var data = departmentList.FirstOrDefault(x => x.DepartmentName == model.DepartmentName);
                            if (data == null)
                            {
                                Department post = new Department
                                {
                                    DepartmentName = model.DepartmentName.Trim(),
                                    Description = model.Description,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    IsActive = true,
                                    IsDeleted = false
                                };
                                _db.Department.Add(post);
                                await _db.SaveChangesAsync();
                                res.Message = "Data Added";
                                res.Status = true;
                                departmentList.Add(post);

                            }
                            else if (data.DepartmentName == model.DepartmentName)
                            {
                                res.Message = "Duplicated Data";
                            }
                        }
                        //}

                    }
                    if (falultyImportItem.Count > 0)
                    {
                        DepartmentImportFaultyLogsGoups faultygroupobj = new DepartmentImportFaultyLogsGoups
                        {
                            DepartmentGroup = Guid.NewGuid(),
                            TotalImported = Item.Count,
                            SuccessFullImported = successfullImported,
                            UnSuccessFullImported = falultyImportItem.Count,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,

                        };
                        _db.DepartmentImportFaultyLogsGoups.Add(faultygroupobj);
                        await _db.SaveChangesAsync();

                        if ((Item.Count - falultyImportItem.Count) > 0)
                        {
                            res.Message = "Department Imported Succesfull Of " +
                            (Item.Count - falultyImportItem.Count) + " Fields And " +
                            falultyImportItem.Count + " Fields Are Not Imported";
                            res.Status = true;
                            res.Data = falultyImportItem;
                        }
                        else
                        {
                            res.Message = "All Fields Are Not Imported";
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
                }
                else
                {
                    res.Message = "0 Dpartment Add";
                    res.Status = false;
                }
                // }
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }


        #endregion

        #region Designation Import File

        /// <summary>
        /// created by Mayank Prajapati on 27/9/2022
        /// Api >> Post >> api/excelimport/adddesignationbyexcelimport
        /// </summary>
        [HttpPost]
        [Route("adddesignationbyexcelimport")]
        public async Task<ResponseBodyModel> DegnigationImport(List<DesignationImportFaultyLogs> Item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var Claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<DesignationImportFaultyLogs> falultyImportItem = new List<DesignationImportFaultyLogs>();
            long successfullImported = 0;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                if (Item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    res.Data = falultyImportItem;
                    return res;
                }
                else if (Item.Count > 0)
                {

                    var Department = await _db.Department.Where(x => x.CompanyId ==
                          Claims.companyId && !x.IsDeleted && x.IsActive).ToListAsync();
                    foreach (var model in Item)
                    {
                        var desingationList = Department.FirstOrDefault(x => x.DepartmentName == model.DepartmentName);
                        //var CheckDesigData = await _db.Designation.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == Claims.companyid).ToListAsync();
                        //foreach (var Model in Item)
                        //{
                        //var designationName = allDesignation.FirstOrDefault(x => x.DesignationName == model.DesignationName);
                        //var data = desingationList.FirstOrDefault(x => x.DepartmentId == model.DepartmentId);
                        //var data = designationlist.FirstOrDefault(x => x.DepartmentName == model.DepartmentName);
                        if (Department != null)
                        {
                            Designation post = new Designation
                            {
                                DesignationName = model.DesignationName,
                                DepartmentId = desingationList.DepartmentId,
                                Description = model.Description,
                                CompanyId = Claims.companyId,
                                OrgId = Claims.employeeId,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false
                            };
                            _db.Designation.Add(post);
                            await _db.SaveChangesAsync();
                            res.Message = "Data Added";
                            res.Status = true;
                            //desingationList.Add(post);
                        }
                        else
                        {
                            res.Message = "Data Added";
                        }
                        //}
                    }
                    if (falultyImportItem.Count > 0)
                    {
                        DesignationImportFaultyLogsGoups faultygroupobj = new DesignationImportFaultyLogsGoups
                        {
                            DesignationGroup = Guid.NewGuid(),
                            TotalImported = Item.Count,
                            SuccessFullImported = successfullImported,
                            UnSuccessFullImported = falultyImportItem.Count,
                            CreatedBy = Claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = Claims.companyId,
                            OrgId = Claims.orgId,
                        };
                        _db.DesignationImportFaultieLogsGoups.Add(faultygroupobj);
                        await _db.SaveChangesAsync();
                        if ((Item.Count - falultyImportItem.Count) > 0)
                        {
                            res.Message = "Designation Imported Succesfull Of " +
                            (Item.Count - falultyImportItem.Count) + " Fields And " +
                            falultyImportItem.Count + " Fields Are Not Imported";
                            res.Status = true;
                            res.Data = falultyImportItem;
                        }
                        else
                        {
                            res.Message = "All Fields Are Not Imported";
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
                }
                else
                {
                    res.Message = "0 Dpartment Add";
                    res.Status = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return res;
        }


        #endregion

        //#region GetDEsignationById
        ///// <summary>
        ///// created by Mayank Prajapati on 03/10/2022
        ///// Api >> Post >> api/excelimport/getdesignationbyid
        ///// </summary>
        //[Route("getdesignationbyid")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetDEsignationById()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var Claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    object misValue = System.Reflection.Missing.Value;
        //    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" +
        //     "ExcelReport.xlsx";

        //    try
        //    {


        //        dynamic _resuit = new List<Designation>();
        //        var DesigData = await (from A in _db.Designation
        //                               where A.IsActive == true && A.IsDeleted == false && A.CompanyId == Claims.companyid /*&& A.OrgId == Claims.orgid*/
        //                               select new DesigExcle
        //                               {
        //                                   DepatmentId = A.DepartmentId,
        //                                   DesignationName = A.DesignationName,
        //                                   Description = A.Description,
        //                                   DepartmentName = _db.Department.Where(x => x.DepartmentId == A.DepartmentId).Select(x => x.DepartmentName).FirstOrDefault()

        //                               }).ToListAsync();

        //        if (DesigData.Count != 0)
        //        {
        //            XSSFWorkbook _workbook = new XSSFWorkbook();
        //            ISheet _sheet = _workbook.CreateSheet("Candidates");
        //            var headerStyle = _workbook.CreateCellStyle(); //Formatting
        //            var headerFont = _workbook.CreateFont();
        //            headerFont.IsBold = true;
        //            headerStyle.SetFont(headerFont);

        //            IRow _row = _sheet.CreateRow(1);
        //            _row.CreateCell(0).SetCellValue(string.Format("DepatmentId {1} - {1}", DesigData, DesigData.Count));
        //            _row.RowStyle = headerStyle;

        //            _row = _sheet.CreateRow(2);
        //            _row.CreateCell(0).SetCellValue("DepatmentId");
        //            _row.CreateCell(5).SetCellValue("DepartmentName");
        //            _row.RowStyle = headerStyle;



        //            if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/SavePDF/{1}", DesigData))))
        //                Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/SavePDF/{1}", "TE " + DesigData + " Summary DesigData")));
        //            if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/SavePDF/{1}.xls", "TE " + DesigData + " Summary DesigData"))))
        //            {
        //                File.Delete(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/SavePDF/{1}.xls", "TE " + DesigData + " Summary DesigData")));
        //            }
        //            Stream stream = File.Create(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/SavePDF/{1}.xlsx", "TE " + DesigData + " Summary DesigData")));
        //            _workbook.Write(stream);
        //            //return string.Format("~/SavePDF/{1}.xlsx", "TE "+ DesigData + " Summary DesigData");

        //            res.Message = "Export Excle Done";
        //            res.Data = DesigData;
        //            res.Status = true;
        //        }
        //        else
        //        {
        //            res.Data = DesigData;
        //            res.Status = false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //    return res;
        //}
        //#endregion

        #region Api To Update Employee Code And Employee Salery By Import

        /// <summary>
        /// Created By Harshit Mitra on 21-06-2022
        /// API >> Put >> api/excelimport/updateemployeeexcel
        /// </summary>
        /// <param name="item"></param>
        [HttpPut]
        [Route("updateemployeeexcel")]
        public async Task<ResponseBodyModel> UpdateEmployeeExcel(List<EmployeeImportFaultyLogs> item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            //List<UpdateEmployeeDataModel> faultyEmployee = new List<UpdateEmployeeDataModel>();
            List<EmployeeImportFaultyLogs> falultyImportItem = new List<EmployeeImportFaultyLogs>();
            try
            {
                var OrgList = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var departmentList = await _db.Department.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                var desingationList = await _db.Designation.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                var empType = Enum.GetValues(typeof(LoginRolesConstants))
                                    .Cast<LoginRolesConstants>()
                                    .Where(x => x != LoginRolesConstants.SuperAdmin)
                                    .Select(x => new
                                    {
                                        EnumValue = x,
                                        EmployeeTypeId = (int)x,
                                        EmployeeTypeName = Enum.GetName(typeof(LoginRolesConstants), x).Replace("_", " "),
                                    }).ToList();
                List<string> notadded = new List<string>();
                if (item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    return res;
                }
                else
                {
                    foreach (var model in item)
                    {
                        if ((String.IsNullOrEmpty(model.OfficeEmail)) && (String.IsNullOrWhiteSpace(model.OfficeEmail)))
                        {
                            model.FailReason = "Office Email Not Found Or Wrong Office Email Inputed";
                            falultyImportItem.Add(model);
                        }
                        else
                        {
                            //var department = departmentList.Where(x=>x.DepartmentName.Trim())
                            //var checkEmp = _db.Employee.Where(x => x.OfficeEmail == model.OfficeEmail).Select(x => x.OfficeEmail.Trim().ToUpper()).Contains(model.OfficeEmail.Trim().ToUpper());
                            var employeelist = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToList();
                            var departmentId = departmentList.Where(x => x.DepartmentName.Trim() == model.DepartmentName.Trim()).Select(x => x.DepartmentId).FirstOrDefault();
                            var designationId = desingationList.Where(x => x.DepartmentId == departmentId && x.DesignationName.Trim() == model.DesignationName.Trim()).Select(x => x.DesignationId).FirstOrDefault();
                            if (claims.orgId == 0)
                                if (!String.IsNullOrEmpty(model.OrganizationName) && !String.IsNullOrWhiteSpace(model.OrganizationName))
                                {
                                    claims.orgId = OrgList.Where(x => x.OrgName == model.OrganizationName).Select(x => x.OrgId).FirstOrDefault();
                                    empType.RemoveAll(x => x.EnumValue == LoginRolesConstants.Administrator);
                                }

                            if (claims.orgId != 0 && designationId != 0 && departmentId != 0)
                            {
                                var empObj = employeelist.FirstOrDefault(x => x.OfficeEmail == model.OfficeEmail);
                                if (empObj != null)
                                {
                                    var user = await _db.User.Where(x => x.EmployeeId == empObj.EmployeeId).FirstOrDefaultAsync();
                                    if (user != null)
                                    {
                                        if ((!String.IsNullOrEmpty(model.LoginType)) && (!String.IsNullOrWhiteSpace(model.LoginType)))
                                        {
                                            var loginTypeEnum = empType.Where(x => x.EmployeeTypeName == model.LoginType).FirstOrDefault();
                                            if (loginTypeEnum == null)
                                            {
                                                notadded.Add(model.OfficeEmail);
                                                continue;
                                            }
                                            else
                                            {
                                                user.LoginId = loginTypeEnum.EnumValue;
                                                _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                            }
                                        }
                                        try
                                        {
                                            var firstName = ((String.IsNullOrEmpty(model.FirstName)) && (String.IsNullOrWhiteSpace(model.FirstName))) ? empObj.FirstName : model.FirstName;
                                            var middleName = String.IsNullOrEmpty(model.MiddleName) ? (String.IsNullOrEmpty(empObj.MiddleName) ? "" : empObj.FirstName) : model.MiddleName;
                                            var lastName = String.IsNullOrEmpty(model.LastName) ? empObj.LastName : model.LastName; ;

                                            empObj.FirstName = firstName;
                                            empObj.MiddleName = middleName;
                                            empObj.LastName = lastName;
                                            empObj.DisplayName = String.IsNullOrEmpty(middleName) ? firstName + " " + lastName :
                                                firstName + " " + middleName + " " + lastName;
                                            empObj.FatherName = String.IsNullOrEmpty(model.FatherName) ? empObj.FatherName : model.FatherName;
                                            empObj.MotherName = String.IsNullOrEmpty(model.MotherName) ? empObj.MotherName : model.MotherName;
                                            empObj.Gender = String.IsNullOrEmpty(model.Gender) ? empObj.Gender : model.Gender;

                                            empObj.DateOfBirth = model.DateOfBirth.Value == empObj.DateOfBirth.Date ? empObj.DateOfBirth.Date : model.DateOfBirth.Value.AddDays(1);

                                            empObj.BloodGroup = String.IsNullOrEmpty(model.BloodGroup) ? empObj.BloodGroup : (
                                                        (BloodGroupConstants)System.Enum.Parse(typeof(BloodGroupConstants),
                                                        (model.BloodGroup.Contains("+") ? model.BloodGroup.Replace("+", "_pos") :
                                                        model.BloodGroup.Replace("-", "_neg"))));
                                            empObj.MaritalStatus = String.IsNullOrEmpty(model.MaritalStatus) ? empObj.MaritalStatus : model.MaritalStatus;

                                            if (model.ConfirmationDate.HasValue)
                                                empObj.ConfirmationDate = ((DateTime)model.ConfirmationDate).Date.AddDays(1);

                                            empObj.JoiningDate = model.JoiningDate.Value == empObj.JoiningDate.Date ? empObj.JoiningDate.Date : model.JoiningDate.Value.AddDays(1);

                                            empObj.UpdatedBy = claims.employeeId;
                                            empObj.UpdatedOn = DateTime.Now;
                                            empObj.DepartmentId = departmentId;
                                            empObj.DesignationId = designationId;
                                            empObj.AadharNumber = String.IsNullOrEmpty(model.AadharNumber) ? empObj.AadharNumber : model.AadharNumber;
                                            empObj.EmployeeTypeId = (EmployeeTypeConstants)(String.IsNullOrEmpty(model.EmployeeType) ? empObj.EmployeeTypeId : model.EmployeeType == null ?
                                                    EmployeeTypeConstants.Confirmed_Employee :
                                                    Enum.Parse(typeof(EmployeeTypeConstants), model.EmployeeType.Replace(" ", "_")));

                                            empObj.EmergencyNumber = String.IsNullOrEmpty(model.EmergencyNumber) ? empObj.EmergencyNumber : model.EmergencyNumber;
                                            empObj.WhatsappNumber = String.IsNullOrEmpty(model.WhatsappNumber) ? empObj.WhatsappNumber : model.WhatsappNumber;
                                            empObj.MobilePhone = String.IsNullOrEmpty(model.MobilePhone) ? empObj.MobilePhone : model.MobilePhone;
                                            empObj.PanNumber = String.IsNullOrEmpty(model.PanNumber) ? empObj.PanNumber : model.PanNumber;
                                            empObj.PersonalEmail = String.IsNullOrEmpty(model.PersonalEmail) ? empObj.PersonalEmail : model.PersonalEmail;
                                            empObj.PermanentAddress = String.IsNullOrEmpty(model.PermanentAddress) ? empObj.PermanentAddress : model.PermanentAddress;
                                            empObj.LocalAddress = String.IsNullOrEmpty(model.LocalAddress) ? empObj.LocalAddress : model.LocalAddress;
                                            empObj.BankAccountNumber = String.IsNullOrEmpty(model.BankAccountNumber) ? empObj.BankAccountNumber : model.BankAccountNumber;
                                            empObj.IFSC = String.IsNullOrEmpty(model.IFSC) ? empObj.IFSC : model.IFSC;
                                            empObj.AccountHolderName = String.IsNullOrEmpty(model.AccountHolderName) ? empObj.AccountHolderName : model.AccountHolderName;
                                            empObj.BankName = String.IsNullOrEmpty(model.BankName) ? empObj.BankName : model.BankName;
                                            empObj.GrossSalery = model.Salary == 0 ? empObj.GrossSalery : model.Salary;
                                            empObj.EmployeeCode = String.IsNullOrEmpty(model.EmployeeCode) ? empObj.EmployeeCode : model.EmployeeCode;
                                            empObj.OrgId = claims.orgId;
                                            empObj.ReportingManager = employeelist.Where(x => x.OfficeEmail.Trim().ToLower() == model.ReportingManager.Trim().ToLower()).Select(x => x.EmployeeId).FirstOrDefault();

                                            _db.Entry(empObj).State = System.Data.Entity.EntityState.Modified;
                                            await _db.SaveChangesAsync();
                                        }
                                        catch (Exception)
                                        {
                                            notadded.Add(model.OfficeEmail);
                                        }
                                    }
                                    else
                                    {
                                        notadded.Add(model.OfficeEmail);
                                    }
                                }
                                else
                                {
                                    notadded.Add(model.OfficeEmail);
                                }
                            }
                            else
                            {
                                notadded.Add(model.OfficeEmail);
                            }
                        }
                        var data = String.Join(",", notadded);
                        res.Message = "Updated";
                        res.Status = true;
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

        #endregion Api To Update Employee Code And Employee Salery By Import

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 21-06-2022
        /// </summary>
        public class UpdateEmployeeDataModel
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string BloodGroup { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string MobilePhone { get; set; }
            public string EmergencyNumber { get; set; }
            public string WhatsappNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string PermanentAddress { get; set; }
            public string LocalAddress { get; set; }
            public string EmployeeType { get; set; }
            public string DepartmentName { get; set; }
            public DateTime JoiningDate { get; set; }
            public DateTime ConfirmationDate { get; set; }
            public string DesignationName { get; set; }
            public string BiometricId { get; set; }
            public string BankAccountNumber { get; set; }
            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }
            public string OfficeEmail { get; set; }
            public string EmployeeCode { get; set; }
            public double Salary { get; set; }
            public string OrganizationName { get; set; }
            public string LoginType { get; set; }
        }

        #endregion Helper Model Class
    }
}