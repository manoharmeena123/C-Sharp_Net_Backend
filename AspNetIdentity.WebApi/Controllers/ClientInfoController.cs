using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/main")]
    public class ClientInfoController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region This Api Use for Add Client

        /// <summary>
        /// created by Mayank Prajapati on 8/7/2022
        /// Api >> Post >> api/main/clientpost
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("clientpost")]
        public async Task<ResponseBodyModel> PostClient(ClientLead model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Unvalid";
                    res.Status = false;
                }
                else
                {
                    ClientLead p1 = new ClientLead
                    {
                        Name = model.Name,
                        Email = model.Email,
                        CompanyName = model.CompanyName,
                        MobileNumber = model.MobileNumber,
                        Message = model.Message,
                        CreatedOn = DateTime.Now,
                        Description = model.Description,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.ClientLeads.Add(p1);
                    await _db.SaveChangesAsync();
                    res.Message = "Add To Client Data";
                    res.Status = true;
                    res.Data = p1;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion This Api Use for Add Client

        /// <summary>
        /// created by Mayank Prajapati on 8/7/2022
        /// Api >> Post >> api/main/clientget
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("clientget")]
        public async Task<ResponseBodyModel> Getclient()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ClientData = await _db.ClientLeads.Where(x => x.IsDeleted == false).ToListAsync();
                if (ClientData.Count > 0)
                {
                    res.Message = "Got The Client Data";
                    res.Status = true;
                    res.Data = ClientData;
                }
                else
                {
                    res.Message = "Failed To Get Data";
                    res.Status = false;
                    res.Data = ClientData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
    }

    #region API to search the data from leads
    /// <summary>
    /// created by Bhavendra singh jat on 11/10/2022
    /// Api >> Post >> api/main/searchdatainclients
    /// </summary>
    /// <returns></returns>
    //[HttpGet]
    //[Authorize]
    //[Route("searchdatainclients")]

    //public async Task<ResponseBodyModel> GetSearchClients(string search = null)
    //{
    //    ResponseBodyModel res = new ResponseBodyModel();
    //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
    //    try
    //    {

    //    }
    //    catch
    //    {

    //    }
    //}
    #endregion





}