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
    [Authorize]
    [RoutePrefix("api/Bank")]
    public class BankMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [Route("GetBankById")]
        [HttpGet]
        public async Task<BankData> GetBankById(int Id)
        {
            BankData depData = new BankData();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //Base response = new Base();

                var BankData = await _db.BankMaster.FirstOrDefaultAsync(x => x.BankId == Id &&
                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (BankData != null)
                {
                    depData.Status = true;
                    depData.Message = "No Bank Found!!";
                    depData.Bank = BankData;
                }
                else
                {
                    depData.Status = false;
                    depData.Message = "No Bank Found!!";
                    depData.Bank = null;
                }
            }
            catch (Exception ex)
            {
                depData.Bank = null;
                depData.Message = ex.Message;
                depData.Status = false;
                return depData;
            }
            return depData;
        }

        [Route("GetAllBank")]
        [HttpGet]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetAllBank()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var BankData = _db.BankMaster.Where(x => x.IsDeleted == false &&
                        x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToList();
                if (BankData.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Bank list Found";
                    res.Data = BankData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Bank list Found";
                    res.Data = BankData;
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

        [Route("CreateBank")]
        [HttpPost]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> CreateBank(BankMaster createBank)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var tBankData = _db.BankMaster.Where(x => x.CompanyId == claims.companyId &&
                        x.OrgId == claims.orgId && x.BankName.Trim().ToUpper() == createBank.BankName.Trim().ToUpper() &&
                        x.Branch.Trim().ToUpper() == createBank.Branch.Trim().ToUpper()).FirstOrDefault();

                BankMaster newBank = new BankMaster();
                if (tBankData == null)
                {
                    newBank.BankName = createBank.BankName;
                    newBank.Address = createBank.Address;
                    newBank.Branch = createBank.Branch;
                    newBank.AccountNumber = createBank.AccountNumber;
                    newBank.IFSC = createBank.IFSC;
                    newBank.PhoneNumber = createBank.PhoneNumber;
                    newBank.EmailAddress = createBank.EmailAddress;
                    newBank.IsActive = true;
                    newBank.IsDeleted = false;
                    newBank.CreatedOn = DateTime.Now;
                    newBank.CreatedBy = claims.userId;
                    newBank.OrgId = claims.orgId;
                    newBank.CompanyId = claims.companyId;

                    _db.BankMaster.Add(newBank);
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Bank added Successfully!";

                    res.Data = newBank;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Bank already exists!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        [Route("UpdateBank")]
        [HttpPut]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> UpdateBank(BankMaster updateDep)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                // BankMaster updateDep = new BankMaster();
                var updateDepData = _db.BankMaster.Where(x => x.BankId == updateDep.BankId && x.IsDeleted == false).FirstOrDefault();
                if (updateDepData != null)
                {
                    updateDepData.BankName = updateDep.BankName;
                    updateDepData.Branch = updateDep.Branch;
                    updateDepData.Address = updateDep.Address;
                    updateDepData.EmailAddress = updateDep.EmailAddress;
                    updateDepData.PhoneNumber = updateDep.PhoneNumber;
                    updateDepData.IFSC = updateDep.IFSC;
                    updateDepData.UpdatedBy = claims.employeeId;
                    updateDepData.UpdatedOn = DateTime.Now;
                    _db.Entry(updateDepData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    response.Message = "Bank Updated Successfully!";
                    response.Status = true;
                    response.Data = updateDepData;
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

        [Route("DeleteBank")]
        [HttpDelete]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteBank(int BankId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                //Base response = new Base();
                var deleteData = _db.BankMaster.Where(x => x.BankId == BankId).FirstOrDefault();
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    response.Status = true;
                    response.Message = "Bank Deleted Successfully!";
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Bank Found!!";
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

        public class BankData
        {
            public int BankId { get; set; }
            public string BankName { get; set; }
            public string Branch { get; set; }
            public string IFSC { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public bool Status { get; set; }
            public string Message { get; set; }
            public BankMaster Bank { get; set; }
        }

        public class BankDataList
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public List<BankMaster> BankList { get; set; }
        }
    }
}