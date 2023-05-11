using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.ClientsModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// updated by Suraj Bundel On 30/05/2022
    /// clientController - client
    /// </summary>
    [Authorize]
    [RoutePrefix("api/client")]
    public class ClientController : ApiController
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        #region  Mobiles API

        #region This Api Use Get Client Details  Data
        /// <summary>
        /// API >> Get >>api/client/getallclientlistaap
        ///  Created by  Mayank Prajapati On 28/12/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("getallclientlistaap")]
        public async Task<ResponseBodyModel> GetAllClientlistAap()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientdetails = await (from c in db.Clients
                                           join cd in db.CtTecnologys on c.ClientId equals cd.ClientId
                                           join t in db.ClientTecnologys on cd.ClientTecnologyId equals t.ClientTecnologyId
                                           where !c.IsDeleted && c.IsActive && c.CompanyId == claims.companyId
                                           select new
                                           {
                                               ClientId = c.ClientId,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               CompanyName = c.CompanyName,
                                               Website = c.Website,
                                               AboutYourCompany = c.AboutYourCompany,
                                               ClientCompanyLinkedInPage = c.ClientCompanyLinkedInPage,
                                               ImageUrl = c.ImageUrl,
                                               TecnologyId = cd.ClientTecnologyId,
                                               TechnologyName = t.ClientTechnoName,

                                           })
                                           .ToListAsync();
                if (clientdetails.Count > 0)
                {
                    var distiunctData = clientdetails
                        .Select(x => new
                        {
                            x.ClientId,
                            x.FirstName,
                            x.LastName,
                            x.CompanyName,
                            x.Website,
                            x.AboutYourCompany,
                            x.ClientCompanyLinkedInPage,
                            x.ImageUrl,
                            TechnologyList = clientdetails
                                .Where(z => z.ClientId == x.ClientId)
                                .Select(z => new
                                {
                                    Key = z.TecnologyId,
                                    Value = z.TechnologyName,
                                })
                                .ToList(),
                        })
                        .Distinct()
                        .ToList();
                    res.Message = "Get Client Details Successfully  !";
                    res.Status = true;
                    res.Data = clientdetails;
                }
                else
                {
                    res.Message = "Client Details Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion Get all Group Detail

        #region This is used for get detail by  Client Contact Id
        /// <summary>
        ///  Created by Mayank Prajappati on 03/01/2023
        /// API >> Get >> api/client/getclientcontactdetailapp
        /// </summary>
        /// <param name="clientcId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("getclientcontactdetailapp")]
        public async Task<ResponseBodyModel> GetDetailByClientContactIdApp(int clientId)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            List<ClientContact> clientcontactlist = new List<ClientContact>();
            try
            {
                var clientcontact = await db.ClientContacts.Where(x => x.ClientId == clientId && x.IsDeleted == false && x.IsActive == true).ToListAsync();


                if (clientcontact != null)
                {
                    res.Status = true;
                    res.Message = "Client Contact List Found";
                    res.Data = clientcontact;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Client Contact List Found";

                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get detail by Client Contact Id

        #region This Api Use Get By Client Name Search
        /// <summary>
        /// API >> Get >>api/client/getbyclientnamesearch
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyclientnamesearch")]
        public async Task<IHttpActionResult> GetByProjectName(string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var technologyData = db.ClientTecnologys.Where(x => x.IsActive &&
                     !x.IsDeleted && x.CompanyId == claims.companyId).Select(z => new
                     {
                         z.ClientTecnologyId,
                         z.ClientTechnoName,
                     }).ToList();
                var clientdetails = await (from c in db.Clients
                                           join cd in db.CtTecnologys on c.ClientId equals cd.ClientId into p
                                           from Result in p.DefaultIfEmpty()
                                           where (c.IsActive && !c.IsDeleted && (c.CompanyName.ToLower().Contains(search.ToLower())))
                                           select new
                                           {
                                               ClientId = c.ClientId,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               CompanyName = c.CompanyName,
                                               Website = c.Website,
                                               AboutYourCompany = c.AboutYourCompany,
                                               ClientCompanyLinkedInPage = c.ClientCompanyLinkedInPage,
                                               ImageUrl = c.ImageUrl,
                                               CityName = c.CityName,
                                               Email = c.Email,
                                               Gender = c.Gender,
                                               Address = c.Address,
                                               MobilePhone = c.MobilePhone,
                                               PostalCode = c.PostalCode,
                                               CountryName = c.CountryName,
                                               StateName = c.StateName,
                                               ExactNoResource = c.ExactNoResource,
                                               IconImageUrl = c.IconImageUrl,
                                               TurnOver = c.TurnOver,
                                               TecnologyId = db.CtTecnologys
                                               .Where(x => x.ClientId == c.ClientId)
                                               .Select(x => x.ClientTecnologyId).FirstOrDefault(),
                                           })
                                          .ToListAsync();
                if (clientdetails.Count > 0)
                {
                    var distinctData = clientdetails
                         .Select(x => new
                         {
                             x.ClientId,
                             x.FirstName,
                             x.LastName,
                             x.CompanyName,
                             x.Website,
                             x.AboutYourCompany,
                             x.ClientCompanyLinkedInPage,
                             x.ImageUrl,
                             x.Gender,
                             x.Email,
                             x.CountryName,
                             x.CityName,
                             x.StateName,
                             x.PostalCode,
                             x.MobilePhone,
                             x.IconImageUrl,
                             x.Address,
                             x.TecnologyId,
                             x.ExactNoResource,
                             x.TurnOver
                         })
                         .Distinct()
                         .Select(x => new
                         {
                             x.ClientId,
                             x.FirstName,
                             x.LastName,
                             x.CompanyName,
                             x.Website,
                             x.AboutYourCompany,
                             x.ClientCompanyLinkedInPage,
                             x.ImageUrl,
                             x.Gender,
                             x.Email,
                             x.CountryName,
                             x.CityName,
                             x.StateName,
                             x.PostalCode,
                             x.MobilePhone,
                             x.IconImageUrl,
                             x.Address,
                             x.ExactNoResource,
                             x.TurnOver,
                             TechnologyList = x.TecnologyId != null ? technologyData
                                 .Where(z => z.ClientTecnologyId == x.TecnologyId)
                                 .Select(z => new
                                 {
                                     z.ClientTecnologyId,
                                     z.ClientTechnoName,
                                 })
                                 .ToList() : null,
                             TechnologyArray = x.TecnologyId != null ? technologyData
                                 .Where(z => z.ClientTecnologyId == x.TecnologyId)
                                 .Select(z => z.ClientTecnologyId)
                                 .ToList() : null,
                         })
                        .ToList();
                    res.Message = "Get Client Details Successfully  !";
                    res.Status = true;
                    res.Data = distinctData;
                }
                else
                {
                    res.Message = "Client Details Group not found";
                    res.Status = false;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        public class ClientResponce
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int ClientId { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string CompanyName { get; set; }
            public string ClientPersonalLinkedInPage { get; set; }
            public string Website { get; set; }
            public string AboutYourCompany { get; set; }
            public string ClientCompanyLinkedInPage { get; set; }
            public int ExactNoResource { get; set; }
            public int NoOfResourceinEachTechnology { get; set; }
            public string BusinessPhone { get; set; }
            public string Status { get; set; }  // not in use
            public string Address { get; set; }
            public string MobilePhone { get; set; }
            public string PostalCode { get; set; }
            public int CountryName { get; set; }
            public int StateName { get; set; }
            public int CityName { get; set; }
            public string ImageUrl { get; set; }
            public double DealPrice { get; set; }
            public string CompanyLogoURL { get; set; } = String.Empty;
        }
        #endregion Get all Client Detail

        #region This Api Use Deleted Technology Details
        /// <summary>
        /// API >> Delete >>api/client/removeclienttechnology
        ///  Created by Mayank Prajapati On 30/12/2022
        /// </summary>
        /// <returns></returns>

        [HttpDelete]
        [Route("removeclienttechnology")]
        public async Task<ResponseBodyModel> ClientTechnologyDelete(Guid clientTecnologyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clienttechnology = await db.ClientTecnologys.FirstOrDefaultAsync(x =>
                    x.ClientTecnologyId == clientTecnologyId && x.CompanyId == claims.companyId);
                if (clienttechnology != null)
                {
                    clienttechnology.IsDeleted = true;
                    clienttechnology.IsActive = false;
                    clienttechnology.DeletedBy = claims.employeeId;
                    clienttechnology.DeletedOn = DateTime.Now;

                    db.Entry(clienttechnology).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = clienttechnology;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion

        #endregion

        #region This Api Use To ISA Client

        #region This Api Use TO Add Client
        /// <summary>
        /// Created By Suraj Bundel On 29-05-2022
        /// API >> Post >> api/client/addclient
        /// </summary>
        /// use to create client in the client
        /// <returns></returns>
        [Route("addclient")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> Addclient(ClinetModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var Country = db.Country.Where(x => !x.IsDeleted && x.IsActive).ToList();
                if (model != null)
                {
                    var checkClient = db.Clients.Any(x => x.Email == model.Email && x.CompanyId == claims.companyId);
                    if (!checkClient)
                    {
                        Client cliententry = new Client();
                        cliententry.FirstName = model.FirstName;
                        cliententry.LastName = model.LastName;
                        cliententry.DisplayName = model.FirstName + " " + model.LastName;
                        cliententry.Email = model.Email;
                        cliententry.Gender = model.Gender;
                        cliententry.ClientPersonalLinkedInPage = model.ClientPersonalLinkedInPage;
                        cliententry.Website = model.Website;
                        cliententry.AboutYourCompany = model.AboutYourCompany;
                        cliententry.ExactNoResource = model.ExactNoResource;
                        cliententry.NoOfResourceinEachTechnology = model.NoOfResourceinEachTechnology;
                        cliententry.CompanyName = model.CompanyName;
                        cliententry.BusinessPhone = model.BusinessPhone;
                        cliententry.Address = model.Address;
                        cliententry.MobilePhone = model.MobilePhone;
                        cliententry.ClientCompanyLinkedInPage = model.ClientCompanyLinkedInPage;
                        cliententry.PostalCode = model.PostalCode;
                        var flag = Country.Where(x => x.CountryId == model.CountryName).Select(x => x.FlagCode).FirstOrDefault();
                        cliententry.IconImageUrl = getflagimagefunction(flag);
                        cliententry.StateName = model.StateName;
                        cliententry.CityName = model.CityName;
                        cliententry.CompanyId = claims.companyId;
                        cliententry.CreatedBy = claims.employeeId;
                        cliententry.CreatedOn = DateTime.Now;
                        cliententry.ImageUrl = model.ImageUrl;
                        cliententry.DealPrice = model.DealPrice;
                        cliententry.AbleToClientLogin = true;
                        cliententry.IsISAClient = true;
                        cliententry.IsClientLock = false;
                        cliententry.TurnOver = model.TurnOver;
                        cliententry.CompanyLogoURL = model.CompanyLogoURL;
                        db.Clients.Add(cliententry);
                        await db.SaveChangesAsync();

                        if (model.TechnologyId.Count > 0)
                        {
                            var technoAdd = model.TechnologyId
                                .Select(x => new CtTecnology
                                {
                                    ClientId = cliententry.ClientId,
                                    ClientTecnologyId = x.Technology

                                })
                                .ToList();
                            db.CtTecnologys.AddRange(technoAdd);
                            await db.SaveChangesAsync();
                        }

                        res.Message = "Client added successfully";
                        res.Status = true;
                        res.Data = cliententry;

                    }

                    else
                    {
                        res.Message = "Client Allready Exist";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Unable to added Client";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        public class ClinetModel
        {

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string CompanyName { get; set; }
            public string ClientPersonalLinkedInPage { get; set; }
            public string Website { get; set; }
            public string AboutYourCompany { get; set; }
            public string ClientCompanyLinkedInPage { get; set; }
            public int ExactNoResource { get; set; }
            public int NoOfResourceinEachTechnology { get; set; }
            public string BusinessPhone { get; set; }
            public string Address { get; set; }
            public string MobilePhone { get; set; }
            public string PostalCode { get; set; }
            public int CountryName { get; set; }
            public int StateName { get; set; }
            public int CityName { get; set; }
            public string ImageUrl { get; set; }
            public double DealPrice { get; set; }
            public string CompanyLogoURL { get; set; } = String.Empty;
            public int TurnOver { get; set; } = 0;
            public List<TecnologyHelperModel> TechnologyId { get; set; }
        }
        public class TecnologyHelperModel
        {
            public Guid Technology { get; set; }

        }
        #endregion Add Client

        #region Api To Add Client Contact
        /// <summary>
        /// Created By  Mayank Prajapati On 03-01-2023
        /// API >> Post >>api/client/addclientContact
        /// </summary>
        /// use to create client in the client
        /// <returns></returns>
        [Route("addclientContact")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> AddclientContact(ClientContact model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {

                if (model != null)
                {
                    ClientContact clientcont = new ClientContact
                    {
                        ClientId = model.ClientId,
                        Name = model.Name,
                        ContectNumber = model.ContectNumber,
                        ClientCLinkedinProfile = model.ClientCLinkedinProfile,
                        EmailAddress = model.EmailAddress,
                        About = model.About,
                        position = model.position,
                        ContactUrl = model.ContactUrl,
                        CompanyId = claims.companyId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    db.ClientContacts.Add(clientcont);
                    await db.SaveChangesAsync();

                    res.Message = "Client Contact added successfully";
                    res.Status = true;
                    res.Data = clientcont;
                }
                else
                {
                    res.Message = "Unable to added Client";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion Add Client Contact  

        #region Api To Add Technolgy
        /// <summary>
        /// Created By  Mayank Prajapati On 03-01-2023
        /// API >> Post >>api/client/addtechnolgy
        /// </summary>
        /// use to create client in the client
        /// <returns></returns>
        [Route("addtechnolgy")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddTechnolgyData(List<ClientTechnlogy> item)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (item == null)
                {
                    res.Message = "Model Is Null";
                    res.Status = false;
                }
                else
                {
                    // var clientData = await db.Clients.Where(x => x.IsActive
                    // && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefaultAsync();
                    if (item != null)
                    {
                        foreach (var demo in item)
                        {
                            ClientTecnology obj = new ClientTecnology
                            {
                                ClientTechnoName = demo.ClientTechnoName,
                                CompanyId = tokenData.companyId,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = DateTime.Now,
                            };
                            db.ClientTecnologys.Add(obj);
                            await db.SaveChangesAsync();

                            res.Message = "added successfully";
                            res.Status = true;
                            res.Data = obj;
                        }
                    }
                    else
                    {
                        res.Message = "Data Not Found";
                        res.Status = true;
                        res.Data = item;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        public class ClientTechnlogy
        {
            public string ClientTechnoName { get; set; }
        }
        #endregion Add Client Contact  

        #region API To Get Tecnology
        /// <summary>
        /// Created By  Mayank Prajapati On 03-01-2023
        /// API >> Get >>api/client/gettecnologydata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gettecnologydata")]
        public async Task<ResponseBodyModel> GetTecnologyData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Client = await db.ClientTecnologys.Where
                    (x => !x.IsDeleted && x.IsActive).ToListAsync();
                if (Client.Count > 0)
                {
                    res.Message = "Get Data successfully";
                    res.Status = true;
                    res.Data = Client;
                }
                else
                {
                    res.Message = "Failed To Post";
                    res.Status = false;
                    res.Data = Client;
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

        #region This Api Use Deleted Clients Tecnology
        /// <summary>
        /// API >> Delete >>api/client/removetecnology
        ///  Created by Mayank Prajapati On 30/12/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("removetecnology")]
        public async Task<ResponseBodyModel> DeleteTecnology(Guid clientTecnologyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientdetail = await db.ClientTecnologys.FirstOrDefaultAsync(x =>
                    x.ClientTecnologyId == clientTecnologyId && x.CompanyId == claims.companyId);
                if (clientdetail != null)
                {
                    clientdetail.IsDeleted = true;
                    clientdetail.IsActive = false;
                    clientdetail.DeletedBy = claims.employeeId;
                    clientdetail.DeletedOn = DateTime.Now;
                    db.Entry(clientdetail).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Deleted Successfully!";
                    res.Status = true;
                    res.Data = clientdetail;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion

        #region This Api Use Get Client Details Join Data
        /// <summary>
        /// API >> Get >>api/client/getclientlist
        ///  Created by  Mayank Prajapati On 28/12/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getclientlist")]
        public async Task<ResponseBodyModel> GetClientlist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientdetails = await (from c in db.Clients
                                           where !c.IsDeleted && c.IsActive && c.CompanyId == claims.companyId
                                           select new
                                           {
                                               ClientId = c.ClientId,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               CompanyName = c.CompanyName,
                                               Website = c.Website,
                                               AboutYourCompany = c.AboutYourCompany,
                                               ClientCompanyLinkedInPage = c.ClientCompanyLinkedInPage,
                                               ImageUrl = c.ImageUrl,
                                               CityName = c.CityName,
                                               Email = c.Email,
                                               Gender = c.Gender,
                                               Address = c.Address,
                                               MobilePhone = c.MobilePhone,
                                               PostalCode = c.PostalCode,
                                               CountryName = c.CountryName,
                                               StateName = c.StateName,
                                               ExactNoResource = c.ExactNoResource,
                                               IconImageUrl = c.IconImageUrl,
                                               TurnOver = c.TurnOver,
                                               TechnologyData = (from d in db.CtTecnologys
                                                                 join cc in db.ClientTecnologys on d.ClientTecnologyId
                                                                 equals cc.ClientTecnologyId
                                                                 where d.ClientId == c.ClientId
                                                                 select new Technology
                                                                 {
                                                                     ClientTecnologyId = d.ClientTecnologyId,
                                                                     ClientTechnoName = cc.ClientTechnoName,
                                                                 }).Distinct().ToList(),
                                           }).ToListAsync();

                if (clientdetails.Count > 0)
                {
                    res.Message = "Get Client Details Successfully  !";
                    res.Status = true;
                    res.Data = clientdetails;
                }
                else
                {
                    res.Message = "Client Details Group not found";
                    res.Status = false;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        public class Technology
        {
            public Guid ClientTecnologyId { get; set; } = Guid.NewGuid();
            public string ClientTechnoName { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get Client All Data
        /// <summary>
        /// API >> Get >>api/client/getallclientdata
        ///  Created by  Mayank Prajapati On 23/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallclientdata")]
        public async Task<ResponseBodyModel> GetAllClientData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var Claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var client = await (from cl in db.Clients
                                    join cc in db.ClientContacts on cl.ClientId equals cc.ClientId
                                    where !cl.IsDeleted && cl.IsActive && cl.CompanyId == Claims.companyId
                                    select new ClientResponceModel()
                                    {
                                        ClientId = cl.ClientId,
                                        FirstName = cl.FirstName,
                                        LastName = cl.LastName,
                                        DisplayName = cl.DisplayName,
                                        Email = cl.Email,
                                        Gender = cl.Gender,
                                        CompanyName = cl.CompanyName,
                                        Website = cl.Website,
                                        AboutYourCompany = cl.AboutYourCompany,
                                        ClientCompanyLinkedInPage = cl.ClientCompanyLinkedInPage,
                                        ExactNoResource = cl.ExactNoResource,
                                        BusinessPhone = cl.BusinessPhone,
                                        Address = cl.Address,
                                        MobilePhone = cl.MobilePhone,
                                        PostalCode = cl.PostalCode,
                                        CountryName = cl.CountryName,
                                        StateName = cl.StateName,
                                        CityName = cl.CityName,
                                        ImageUrl = cl.ImageUrl,
                                        IconImageUrl = cl.IconImageUrl,
                                        DealPrice = cl.DealPrice,
                                        Name = cc.Name,
                                        ContectNumber = cc.ContectNumber,
                                        position = cc.position,
                                        EmailAddress = cc.EmailAddress,
                                        ClientCLinkedinProfile = cc.ClientCLinkedinProfile,
                                    }).ToListAsync();
                if (client != null)
                {
                    res.Message = "Client Data Successfully Get !";
                    res.Status = true;
                    res.Data = client;
                }
                else
                {
                    res.Message = "Client Data Request Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        public class ClientResponceModel
        {
            public int ClientId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string CompanyName { get; set; }
            public ClientTechnologyConstants ClientTechno { get; set; }
            public string ClientTechnoName { get; set; }
            public string Website { get; set; }
            public string AboutYourCompany { get; set; }
            public string ClientCompanyLinkedInPage { get; set; }
            public int ExactNoResource { get; set; }
            public string BusinessPhone { get; set; }
            public string Address { get; set; }
            public string MobilePhone { get; set; }
            public string PostalCode { get; set; }
            public int CountryName { get; set; }
            public int StateName { get; set; }
            public int CityName { get; set; }
            public string ImageUrl { get; set; }
            public string IconImageUrl { get; set; }
            public double DealPrice { get; set; }
            public string Name { get; set; }
            public string ClientCLinkedinProfile { get; set; }
            public string ContectNumber { get; set; }
            public string EmailAddress { get; set; }
            public string position { get; set; }
        }
        #endregion

        #region This Api Use Get Client Contact Details all Data
        /// <summary>
        /// API >> Get >>api/client/getclientcontactlist
        ///  Created by  Mayank Prajapati On 03/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getclientcontactlist")]
        public async Task<ResponseBodyModel> GetClientContactlist()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientcontactdetails = await db.ClientContacts.Where(x => x.IsActive && !x.IsDeleted
                  && x.CompanyId == claims.companyId).ToListAsync();
                if (clientcontactdetails != null)
                {
                    res.Message = "Get Client Contact Details Successfully  !";
                    res.Status = true;
                    res.Data = clientcontactdetails;
                }
                else
                {
                    res.Message = "Client Contact Details Group not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion Get all Group Detail

        #region Update Client Data
        /// <summary>
        /// Created by Suraj Bundel on 30/05/2022
        /// Modified by Shriya on 01-06-2022
        /// API >> Put >> api/client/updateclientdata
        /// </summary>
        /// use to update the Update client
        /// <param name="updateclient"></param>
        /// <returns></returns>
        [Route("updateclientdata")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> updateclientdata(ClinetHelperModel model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var updateData = await db.Clients.Where(x => x.ClientId == model.ClientId).FirstOrDefaultAsync();
                if (updateData != null)
                {
                    var Country = db.Country.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
                    updateData.ClientId = model.ClientId;
                    updateData.FirstName = model.FirstName;
                    updateData.LastName = model.LastName;
                    updateData.DisplayName = model.FirstName + " " + model.LastName;
                    updateData.Email = model.Email;
                    updateData.ClientCompanyLinkedInPage = model.ClientCompanyLinkedInPage;
                    updateData.Website = model.Website;
                    updateData.AboutYourCompany = model.AboutYourCompany;
                    updateData.ExactNoResource = model.ExactNoResource;
                    updateData.Gender = model.Gender;
                    updateData.CompanyName = model.CompanyName;
                    updateData.BusinessPhone = model.BusinessPhone;
                    updateData.Address = model.Address;
                    updateData.MobilePhone = model.MobilePhone;
                    updateData.PostalCode = model.PostalCode;
                    updateData.CountryName = model.CountryName;
                    var flag = Country.Where(x => x.CountryId == model.CountryName).Select(x => x.FlagCode).FirstOrDefault();
                    updateData.IconImageUrl = getflagimagefunction(flag);
                    updateData.StateName = model.StateName;
                    updateData.CityName = model.CityName;
                    updateData.DealPrice = model.DealPrice;
                    updateData.CompanyId = claims.companyId;
                    updateData.UpdatedBy = claims.employeeId;
                    updateData.UpdatedOn = DateTime.Now;
                    updateData.TurnOver = model.TurnOver;
                    db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    foreach (var item in model.TechnologyId)
                    {
                        var technologyData = db.CtTecnologys.Where
                            (x => x.ClientTecnologyId == item).FirstOrDefault();
                        if (technologyData != null)
                        {
                            technologyData.ClientTecnologyId = item;
                            technologyData.ClientId = updateData.ClientId;
                            technologyData.UpdatedBy = claims.employeeId;
                            technologyData.UpdatedOn = DateTime.Now;

                            db.Entry(technologyData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    res.Status = true;
                    res.Message = "Updated Successfully!";
                    res.Data = updateData;
                }
                else
                {
                    res.Message = "Update request failed";
                    res.Status = false;
                    res.Data = updateData;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;

            }
            return res;
        }
        public class ClinetHelperModel
        {
            public int ClientId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string CompanyName { get; set; }
            public string Website { get; set; }
            public string AboutYourCompany { get; set; }
            public string ClientCompanyLinkedInPage { get; set; }
            public int ExactNoResource { get; set; }
            public int NoOfResourceinEachTechnology { get; set; }
            public string BusinessPhone { get; set; }
            public string Status { get; set; }  // not in use
            public string Address { get; set; }
            public string MobilePhone { get; set; }
            public string PostalCode { get; set; }
            public int CountryName { get; set; }
            public int StateName { get; set; }
            public int TurnOver { get; set; }
            public int CityName { get; set; }
            public string ImageUrl { get; set; }
            public string IconImageUrl { get; set; }
            public double DealPrice { get; set; }
            public List<Guid> TechnologyId { get; set; }
        }
        #endregion Update Client Data

        #region Update Client Contact Data
        /// <summary>
        /// Created by Suraj Bundel on 30/05/2022
        /// Modified by Mayank Prajapati on 03-01-2023
        /// API >> Put >>api/client/updateclientcontactdata
        /// </summary>
        /// use to update the Update client
        /// <param name="updateclient"></param>
        /// <returns></returns>

        [Route("updateclientcontactdata")]
        [HttpPut]
        [Authorize]
        public async Task<ResponseBodyModel> updateclientContactdata(ClientContectModelHelper model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model != null)
                {
                    ClientContact clientcont = new ClientContact
                    {
                        ClientContactId = model.ClientContactId,
                        ClientId = model.ClientId,
                        Name = model.Name,
                        About = model.About,
                        ContectNumber = model.ContectNumber,
                        EmailAddress = model.EmailAddress,
                        position = model.position,
                        EmployeeId = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = claims.userId,
                    };
                    db.ClientContacts.Add(clientcont);
                    await db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Updated Successfully!";
                    res.Data = clientcont;
                }
                else
                {
                    res.Message = "Update request failed";
                    res.Status = false;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class ClientContectModelHelper
        {
            public Guid ClientContactId { get; set; }
            public int ClientId { get; set; }
            public string Name { get; set; }
            public string About { get; set; }
            public string ContectNumber { get; set; }
            public string EmailAddress { get; set; }
            public string position { get; set; }
        }
        #endregion

        #region This is used for get detail by  Client Id

        /// <summary>
        ///  Created by Shriya on 31/05/2022
        /// API >> Get >>api/client/getdetailbyclientid
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdetailbyclientid")]
        public async Task<ResponseBodyModel> GetDetailByClientId(int clientId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<Client> clientlist = new List<Client>();
            try
            {
                var clientdetails = await (from c in db.Clients
                                           where !c.IsDeleted && c.IsActive &&
                                           c.CompanyId == claims.companyId && c.ClientId == clientId
                                           select new
                                           {
                                               ClientId = c.ClientId,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               CompanyName = c.CompanyName,
                                               Website = c.Website,
                                               AboutYourCompany = c.AboutYourCompany,
                                               ClientCompanyLinkedInPage = c.ClientCompanyLinkedInPage,
                                               ImageUrl = c.ImageUrl,
                                               CityName = c.CityName,
                                               Email = c.Email,
                                               Gender = c.Gender,
                                               Address = c.Address,
                                               MobilePhone = c.MobilePhone,
                                               PostalCode = c.PostalCode,
                                               CountryName = c.CountryName,
                                               StateName = c.StateName,
                                               ExactNoResource = c.ExactNoResource,
                                               IconImageUrl = c.IconImageUrl,
                                               TurnOver = c.TurnOver,
                                               TechnologyData = db.CtTecnologys.Where(x => x.ClientId == c.ClientId).Select(x => x.ClientTecnologyId).ToList()
                                           }).FirstOrDefaultAsync();
                if (clientdetails != null)
                {
                    res.Message = "Get Client Details Successfully  !";
                    res.Status = true;
                    res.Data = clientdetails;
                }
                else
                {
                    res.Message = "Client Details Group not found";
                    res.Status = false;
                    res.Data = clientdetails;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class TechnologyData
        {
            public Guid ClientTecnologyId { get; set; } = Guid.NewGuid();
        }
        #endregion This is used for get detail by  Client Id

        #region This is used for get detail by TechnologyId
        /// <summary>
        ///  Created by Shriya on 31/05/2022
        /// API >> Get >> api/client/gettechnologybyid
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettechnologybyid")]
        public async Task<ResponseBodyModel> GetTechnologyById(Guid technologyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<Client> clientlist = new List<Client>();
            try
            {
                var technologyData = db.ClientTecnologys.Where(x => x.IsActive &&
                     !x.IsDeleted && x.CompanyId == claims.companyId).Select(z => new
                     {
                         z.ClientTecnologyId,
                         z.ClientTechnoName,
                     }).ToList();
                var clientdetails = await (from c in db.Clients
                                           join cd in db.CtTecnologys on c.ClientId equals cd.ClientId
                                           where !c.IsDeleted && c.IsActive && c.CompanyId == claims.companyId
                                           && cd.ClientTecnologyId == technologyId
                                           select new
                                           {
                                               ClientId = c.ClientId,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               CompanyName = c.CompanyName,
                                               Website = c.Website,
                                               AboutYourCompany = c.AboutYourCompany,
                                               ClientCompanyLinkedInPage = c.ClientCompanyLinkedInPage,
                                               ImageUrl = c.ImageUrl,
                                               CityName = c.CityName,
                                               Email = c.Email,
                                               Gender = c.Gender,
                                               Address = c.Address,
                                               MobilePhone = c.MobilePhone,
                                               PostalCode = c.PostalCode,
                                               CountryName = c.CountryName,
                                               StateName = c.StateName,
                                               ExactNoResource = c.ExactNoResource,
                                               IconImageUrl = c.IconImageUrl,
                                               TecnologyId = db.CtTecnologys
                                               .Where(x => x.ClientId == c.ClientId)
                                               .Select(x => x.ClientTecnologyId).FirstOrDefault(),
                                           })
                                          .ToListAsync();
                if (clientdetails.Count > 0)
                {
                    var distinctData = clientdetails
                        .Select(x => new
                        {
                            x.ClientId,
                            x.FirstName,
                            x.LastName,
                            x.CompanyName,
                            x.Website,
                            x.AboutYourCompany,
                            x.ClientCompanyLinkedInPage,
                            x.ImageUrl,
                            x.Gender,
                            x.Email,
                            x.CountryName,
                            x.CityName,
                            x.StateName,
                            x.PostalCode,
                            x.MobilePhone,
                            x.IconImageUrl,
                            x.Address,
                            x.ExactNoResource,
                            x.TecnologyId,
                        })
                        .Distinct()
                        .Select(x => new
                        {
                            x.ClientId,
                            x.FirstName,
                            x.LastName,
                            x.CompanyName,
                            x.Website,
                            x.AboutYourCompany,
                            x.ClientCompanyLinkedInPage,
                            x.ImageUrl,
                            x.Gender,
                            x.Email,
                            x.CountryName,
                            x.CityName,
                            x.StateName,
                            x.PostalCode,
                            x.MobilePhone,
                            x.IconImageUrl,
                            x.Address,
                            x.ExactNoResource,
                            x.TecnologyId,
                            TechnologyList = x.TecnologyId != null ? technologyData
                                 .Where(z => z.ClientTecnologyId == x.TecnologyId)
                                 .Select(z => new
                                 {
                                     z.ClientTecnologyId,
                                     z.ClientTechnoName,
                                 })
                                 .ToList() : null,
                            TechnologyArray = x.TecnologyId != null ? technologyData
                               .Where(z => z.ClientTecnologyId == x.TecnologyId)
                               .Select(z => z.ClientTecnologyId)
                               .ToList() : null,
                        })
                        .ToList();

                    res.Message = "Get Client Details Successfully  !";
                    res.Status = true;
                    res.Data = distinctData;
                }
                else
                {
                    res.Message = "Client Details Group not found";
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
        #endregion This is used for get detail by  Client Id

        #region This is used for get detail by  Client Contact Id
        /// <summary>
        ///  Created by Mayank Prajappati on 03/01/2023
        /// API >> Get >> api/client/getclientcontactdetail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getclientcontactdetail")]
        public async Task<ResponseBodyModel> GetDetailByClientContactId(int clientId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientcontact = await db.ClientContacts.Where(x => x.ClientId == clientId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (clientcontact != null)
                {
                    res.Status = true;
                    res.Message = "Client Contact List Found";
                    res.Data = clientcontact;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Client Contact List Found";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion This is used for get detail by Client Contact Id

        #region This Api Use Deleted Clients Details
        /// <summary>
        /// API >> Delete >>api/client/removeclientdetails
        ///  Created by Mayank Prajapati On 30/12/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("removeclientdetails")]
        public async Task<ResponseBodyModel> ClientDetailsDelete(int ClientId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientdetail = await db.Clients.FirstOrDefaultAsync(x =>
                    x.ClientId == ClientId && x.CompanyId == claims.companyId);
                if (clientdetail != null)
                {
                    clientdetail.IsDeleted = true;
                    clientdetail.IsActive = false;
                    clientdetail.DeletedBy = claims.employeeId;
                    clientdetail.DeletedOn = DateTime.Now;
                    db.Entry(clientdetail).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = clientdetail;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion

        #region This Api Use Deleted Client Contact Details
        /// <summary>
        /// API >> Delete >>api/client/removeclientcontactdetails
        ///  Created by Mayank Prajapati On 30/12/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("removeclientcontactdetails")]
        public async Task<ResponseBodyModel> ClientContactDetailsDelete(Guid ClientContactId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clientContactDetail = await db.ClientContacts.FirstOrDefaultAsync(x =>
                    x.ClientContactId == ClientContactId);
                if (clientContactDetail != null)
                {
                    clientContactDetail.IsDeleted = true;
                    clientContactDetail.IsActive = false;
                    clientContactDetail.DeletedBy = claims.employeeId;
                    clientContactDetail.DeletedOn = DateTime.Now;
                    db.Entry(clientContactDetail).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = clientContactDetail;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion

        #region Api To Upload Client Contact Image
        /// <summary>
        /// Created By Suraj Bundel On 30-05-2022
        /// API >> Post >> api/client/uploadclientContactimage
        /// </summary>
        /// use to post Document the client List
        /// <returns></returns>
        [HttpPost]
        [Route("uploadclientContactimage")]
        public async Task<UploadImageUrlHelper> UploadGoalDocmentsIMG()
        {
            UploadImageUrlHelper result = new UploadImageUrlHelper();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
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

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/clientContacturl/" + tokenData.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { tokenData.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + tokenData.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\clientContacturl\\" + tokenData.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successfuly";
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


        /// <summary>
        /// Created By Ankit
        /// </summary>

        public class UploadImageUrlHelper
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }


        #endregion Api To Upload Client Image

        #endregion

        #region UN Used API

        #region This Api Use To Get Client Techno Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 09/01/2023
        ///// Api >> Get >> api/client/getclienttechno
        /////
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getclienttechno")]
        [HttpGet]
        public IHttpActionResult GetClientTechno()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payment = Enum.GetValues(typeof(ClientTechnologyConstants))
                    .Cast<ClientTechnologyConstants>()
                    .Select(x => new ClientTechnologyModel
                    {
                        ClientTechnoId = (int)x,
                        ClientTechnoName = Enum.GetName(typeof(ClientTechnologyConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Client Technology Exist";
                res.Status = true;
                res.Data = payment;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        public class ClientTechnologyModel
        {
            public int ClientTechnoId { get; set; }
            public string ClientTechnoName { get; set; }
        }
        #endregion

        //#region This Api Use Get Client Tecnology By Id
        ///// <summary>
        ///// API >> Get >>api/client/getclientPosition
        /////  Created by  Mayank Prajapati On 03/01/2023
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("getclientPosition")]
        //public async Task<ResponseBodyModel> GetClientTecnology(string clientTechnoName)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var clientPosition = await (from e in db.Employee
        //                                    join d in db.Designation on e.EmployeeId equals d.DesignationId
        //                                    where !e.IsDeleted && d.IsActive 
        //                                    select new PositionResonce()
        //                                    {

        //                                    });
        //        if (clientPosition != null)
        //        {
        //            res.Message = "Get Client Contact Details Successfully  !";
        //            res.Status = true;
        //            res.Data = clientPosition;
        //        }
        //        else
        //        {
        //            res.Message = "Client Contact Details Group not found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Status = false;
        //        res.Message = ex.Message;
        //    }
        //    return res;
        //}
        //public class PositionResonce
        //{
        //    public string EmployeeName { get; set; }
        //    public string Designation { get; set; }
        //}
        //#endregion Get all Group Detail

        //#region This Api Use Get Client Tecnology By Id
        ///// <summary>
        ///// API >> Get >>api/client/getclientTecnology
        /////  Created by  Mayank Prajapati On 03/01/2023
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getclientTecnology")]
        //public async Task<ResponseBodyModel> GetClientTecnology(string clientTechnoName)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var clientcontactdetails = await db.Clients.Where(x => x.IsActive && !x.IsDeleted
        //         && x.ClientTechnoName == clientTechnoName && x.CompanyId == claims.companyId).ToListAsync();
        //        if (clientcontactdetails != null)
        //        {
        //            res.Message = "Get Client Contact Details Successfully  !";
        //            res.Status = true;
        //            res.Data = clientcontactdetails;
        //        }
        //        else
        //        {
        //            res.Message = "Client Contact Details Group not found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Status = false;
        //        res.Message = ex.Message;
        //    }
        //    return res;
        //}
        //#endregion Get all Group Detail

        #region Get all Client list // Pagination// {by Employee} ---------------------- testing due

        ///// <summary>
        ///// updated by Suraj Bundel On 30/05/2022
        ///// API -> Get ->api/client/getClientpagination
        ///// </summary>
        ///// use to get the paginatiion in client List
        ///// <returns></returns>

        //[Route("getclientpagination")]
        //[HttpGet]
        //[Authorize]

        //public async Task<ResponseBodyModel> Getclientpagination(int page = 1)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try

        //    {
        //        //   Base res = new Base();
        //        var clientData = (from ad in db.Client orderby ad.ClientId descending select ad).ToList();
        //        ViewClient objModel = new ViewClient();
        //        var check = clientData;
        //        objModel.objList = new List<ClientData>();
        //        for (int i = 0; i < clientData.Count; i++)
        //        {
        //            ClientData objemp = new ClientData();
        //            objemp.ClientId = check[i].ClientId;
        //            objemp.Name = check[i].Name;
        //            objemp.Email = check[i].Email;
        //            objemp.Address = check[i].Address;
        //            objemp.CompanyName = check[i].CompanyName;
        //            objemp.MobilePhone = check[i].MobilePhone;
        //            objemp.PostalCode = check[i].PostalCode;
        //            objemp.CountryName = check[i].CountryName;
        //            objemp.StateName = check[i].StateName;
        //            objemp.CityName = check[i].CityName;
        //            objModel.objList.Add(objemp);
        //        }
        //        int pageSize = 10;
        //        int pageNumber = page;
        //        var res = objModel.objList.ToPagedList(pageNumber, pageSize);
        //        //return Ok(objModel.objList.ToPagedList(pageNumber, pageSize));
        //        if (res.Count != 0)
        //        {
        //            response.Status = true;
        //            response.Message = "Client list Found";
        //            response.Data = res;
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            response.Message = "No Client list Found";
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

        ////    {
        ////        var brach = await (from C in db.Currency
        ////                           join EE in db.ExpenseEntry on C.ISOCurrencyCode equals EE.ISOCurrencyCode
        ////                           join Emp in db.Employee on EE.CreatedBy equals Emp.EmployeeId
        ////                           where EE.CreatedBy == claims.employeeid &&
        ////                         EE.IsActive == true && EE.IsDeleted == false && EE.CompanyId == claims.companyid && Emp.EmployeeId == claims.employeeid
        ////                           select new
        ////                           {
        ////                               EE.ExpenseId,
        ////                               EE.IconImageUrl,
        ////                               EE.ImageUrl,
        ////                               EE.ExpenseCategoryType,
        ////                               EE.ExpenseTitle,
        ////                               EE.ExpenseStatus,
        ////                               EE.ExpenseDate,
        ////                               EE.ExpenseAmount,
        ////                               EE.BillNumber,
        ////                               EE.MerchantName,
        ////                               EE.Comment,
        ////                               EE.ISOCurrencyCode,
        ////                               C.CurrencyName,
        ////                               EE.FinalApproveAmount,
        ////                               C.CurrencyId,
        ////                               Emp.DisplayName,
        ////                               ApproveRejectBy = EE.ApprovedRejectBy.HasValue ? db.Employee.Where(x => x.EmployeeId == (int)EE.ApprovedRejectBy)
        ////                                                        .Select(x => x.DisplayName).FirstOrDefault() : "--------",

        ////                           }).ToListAsync();
        ////        if (brach.Count != 0)
        ////        {
        ////            response.Status = true;
        ////            response.Message = "Expense list Found";
        ////            response.Data = brach;
        ////        }
        ////        else
        ////        {
        ////            response.Status = false;
        ////            response.Message = "No Expense list Found";
        ////            response.Data = null;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response.Data = null;
        ////        response.Message = ex.Message;
        ////        response.Status = false;
        ////        return response;
        ////    }
        ////    return response;
        ////}

        #endregion Get all Client list // Pagination// {by Employee}           ---------------------- testing due

        #region Api To Upload Client Image

        /// <summary>
        /// Created By Suraj Bundel On 30-05-2022
        /// API >> Post >> api/client/uploadclientimage
        /// </summary>
        /// use to post Document the client List
        /// <returns></returns>
        [HttpPost]
        [Route("uploadclientimage")]
        public async Task<UploadImageUrl> UploadGoalDocments()
        {
            UploadImageUrl result = new UploadImageUrl();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
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

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/clienturl/" + tokenData.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { tokenData.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + tokenData.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\clienturl\\" + tokenData.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successfuly";
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


        /// <summary>
        /// Created By Ankit
        /// </summary>

        public class UploadImageUrl
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }


        #endregion Api To Upload Client Image

        #region This is used for get detail by  Client Contact Client Id
        /// <summary>
        ///  Created by Mayank Prajappati on 03/01/2023
        /// API >> Get >> api/client/getclientcontactClientId
        /// </summary>
        /// <param name="clientcId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getclientcontactClientId")]
        public async Task<ResponseBodyModel> GetDetailByClientContactId()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<ClientContact> clientcontactlist = new List<ClientContact>();
            try
            {
                var clientcontact = await db.ClientContacts.Where(x => x.EmployeeId == tokenData.employeeId &&
                !x.IsDeleted && x.IsActive).ToListAsync();
                if (clientcontact.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Client Contact List Found";
                    res.Data = clientcontact;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Client Contact List Found";

                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get detail by Client Contact Id

        #region API To Check Registerr Client\
        /// <summary>
        /// Created By Harshit Mitra On 11-07-2022
        /// API >> Get >> api/client/checkclient
        /// </summary>
        /// <param name="clientEmail"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("checkclient")]
        public async Task<ResponseBodyModel> CheckClient(string clientEmail)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkClient = await db.Clients.AnyAsync(x => x.Email == clientEmail && x.CompanyId == claims.companyId);
                if (!checkClient)
                {
                    res.Message = "Client Is Register";
                    res.Status = false;
                }
                else
                {
                    res.Message = "New Client";
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
        #endregion API To Check Registerr Client

        #region Api To Upload Icons
        /// <summary>
        /// Created By Suraj Bundel On 30-05-2022
        /// API >> Post >> api/client/uploadicon
        /// </summary>
        /// use to post Document the client icon
        /// <returns></returns>
        [HttpPost]
        [Route("uploadicons")]
        [Authorize]
        public async Task<UploadImageResponse> uploadIcons()
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
                        //if (extemtionType == "image" || extemtionType=="Document"||extemtionType== "application")
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/clientImage/uploadIcons/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\clientImage\\uploadIcons\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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
        #endregion Api To Upload Icons

        #region //commented

        ///// <summary>
        ///// Created By Suraj Bundel On 23-05-2022
        ///// API >> Post >> api/client/countclientproject
        ///// </summary>
        ///// use to create the Expense in count project by client Id
        ///// <returns></returns>
        //[Route]
        //public async Task<ResponseBodyModel> GetClientprojectCount(int clientid)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    var res = await db.Project.Where(x => x.ClientId == clientid).ToListAsync();
        //    if (res != null)
        //    {
        //        var projectcount = res.Count();
        //    }
        //    //     var res = db.Client.Where(x => x.ClientId ==Project.Equals(x.ClientId);
        //    //var count = (from c in db.Client
        //    //             join P in db.Project on c.ClientId equals P.ClientId
        //    //             //where c.ClientId == c.ClientId
        //    //             //from ct in c.MyTable
        //    //             select c
        //    //             ).Count();
        //    //var newcount = db.Client.Count();

        //    return response;
        //}

        #endregion //commented

        //#region This is used for Get all client details // count function

        ///// <summary>
        ///// Created by Shriya on 31/05/2022
        ///// Modify By Shriya Malvi 28-06-2022
        ///// API >> Get >> api/client/getclientlist
        ///// </summary>
        ///// use to for Get all client details // count function
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getclientlist")]
        //public async Task<ResponseBodyModel> GetClientlist()
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        List<ClientModel> clientlist = new List<ClientModel>();
        //        var Country = db.Country.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
        //        var projects = await db.Project.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();
        //        var clientData = db.Client.Where(x => x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId).ToList();
        //        foreach (var item in clientData)
        //        {
        //            ClientModel clientObj = new ClientModel();
        //            clientObj.ClientId = item.ClientId;
        //            clientObj.Name = item.FirstName + " " + item.LastName;

        //            var countryid = Convert.ToInt32(item.CountryName);
        //            // clientObj.CountryName = Country.Where(x => x.CountryId == countryid).Select(x => x.CountryName).FirstOrDefault();
        //            var CountryName = Country.Where(x => x.CountryId == countryid).Select(x => x.FlagCode).FirstOrDefault();

        //            clientObj.CountryName = getflagimagefunction(CountryName);
        //            clientObj.Email = item.Email;
        //            var getproject = projects.Where(x => x.ClientId == item.ClientId).ToList();
        //            double dealprice = 0.0;
        //            foreach (var project in getproject)
        //            {
        //                dealprice = dealprice + project.ProjectPrice;
        //            }
        //            //clientObj.DealPrice = dealprice.ToString("0,0", CultureInfo.CreateSpecificCulture("hi-IN"));
        //            clientObj.DealPrice = ClientHelper.NumericNumConvToAbbv(dealprice);
        //            clientObj.ProjectCount = getproject.Where(x => x.ClientId == item.ClientId).Count();
        //            clientObj.ImgUrl = item.ImageUrl;
        //            clientlist.Add(clientObj);
        //        }
        //        if (clientlist != null)
        //        {
        //            response.Status = true;
        //            response.Message = "Record Found";
        //            response.Data = clientlist;
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            response.Message = "No Record Found!";
        //            response.Data = clientlist;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Status = false;
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        //#endregion This is used for Get all client details // count function

        #region This is used for get all trevel via

        /// <summary>
        /// API >> Get >> api/client/getgenderbyenum
        /// Created by shriya ,Created on 31-05-2022
        /// </summary>
        /// <returns></returns>
        [Route("getgenderbyenum")]
        [HttpGet]
        [Authorize]
        public ResponseBodyModel GetExpenseTravelExp()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var identity = User.Identity as ClaimsIdentity;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Gender = Enum.GetValues(typeof(GenderConstants))
                    .Cast<GenderConstants>()
                    .Select(x => new GenderList
                    {
                        GenderId = (int)x,
                        Gender = Enum.GetName(typeof(GenderConstants), x)
                    }).ToList();
                res.Message = "Gender List";
                res.Status = true;
                res.Data = Gender;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This is used for get all trevel via

        #region Update Expense Entry // not in use

        ///// <summary>
        ///// Created by Suraj Bundel on 30/05/2022
        ///// API >> Get >> api/client/updateclientstatus
        ///// </summary>
        ///// use to update the client Status
        ///// <param name="updatestatus"></param>
        ///// <returns></returns>

        //[Route("updateclientstatus")]
        //[HttpPut]
        //[Authorize]
        //public async Task<ResponseBodyModel> UpdateClientStatus(Client updatestatus)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();

        //    try
        //    {
        //        var updateData = await db.Client.Where(x => x.ClientId == updatestatus.ClientId /*&& x.IsDeleted == false*/).FirstOrDefaultAsync();
        //        if (updateData != null)
        //        {
        //            updateData.ExpenseStatus = updatestatus.ExpenseStatus;
        //            updateData.ModeofPayment = updatestatus.ModeofPayment;
        //            updateData.FinalApproveAmount = updatestatus.FinalApproveAmount;
        //            updateData.Reason = updateexc.Reason;
        //            updateData.UpdatedOn = DateTime.Now;
        //            updateData.UpdatedBy = claims.employeeid;
        //            updateData.ApprovedRejectBy = claims.employeeid;
        //            updateData.IsActive = true;
        //            updateData.IsDeleted = false;
        //            db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            res.Status = true;
        //            res.Message = "Updated Successfully!";
        //        }
        //        else
        //        {
        //            res.Message = "Update request failed";
        //            res.Status = false;
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        #endregion Update Expense Entry // not in use

        #region Create A Api use Get Badegs

        /// <summary>
        /// Created By Suraj Bundle 31/05/2022
        /// Api >> Get >> api/client/Geticons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("geticons")]
        [Authorize]
        public async Task<ResponseBodyModel> Geticons()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getIcons = await db.Clients.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();

                if (getIcons.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Data Found";
                    res.Data = getIcons;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Create A Api use Get Badegs

        #region get image by country flagID // foreach

        /// <summary>
        ///Created By Suraj Bundel On 01/06/2022
        /// Get project on behalf of client ID
        /// Get by id -> Api -> api/client/GetflagbyId
        /// </summary>
        /// <return></return>
        ///
        [Route("GetflagbyId")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetflagbyId(int id)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            try
            {
                List<CountryData> countrydatalist = new List<CountryData>();
                //var getobj = await db.Country.Where(x => /*x.IsActive == true && x.IsDeleted == false && */x.CountryId == countryid).ToListAsync();
                var getobj = await db.Country.ToListAsync();

                foreach (var item in getobj)
                {
                    CountryData CountryDataObj = new CountryData();
                    CountryDataObj.CountryId = item.CountryId;
                    CountryDataObj.CountryName = item.CountryName;
                    CountryDataObj.flagCode = item.FlagCode;
                    countrydatalist.Add(CountryDataObj);
                }
                if (countrydatalist != null)
                {
                    response.Status = true;
                    response.Message = "Record Found";
                    response.Data = countrydatalist;
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Record Found!";
                    response.Data = countrydatalist;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }
            return response;
        }

        #endregion get image by country flagID // foreach

        #region Get all Client Name

        /// <summary>
        /// API >> Get >> api/client/getnameofclient
        ///  Created by shriya on 02-06-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnameofclient")]
        public async Task<ResponseBodyModel> GetNameOfClient()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ClientDetail = await db.Clients.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).
                    Select(x => new ClientDetail
                    {
                        ClientId = x.ClientId,
                        ClientName = x.DisplayName
                    }).ToListAsync();

                res.Status = true;
                res.Message = "client list found";
                res.Data = ClientDetail;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }
        public class ClientDetail
        {
            public int ClientId { get; set; }
            public string ClientName { get; set; }
        }

        #endregion Get all Client Name

        #region API to search data from all employees
        /// <summary>
        /// API >> Get >> api/client/searchdatafromclients
        ///  Created by Bhavendra on 12-10-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("searchdatafromclients")]
        public async Task<ResponseBodyModel> GetSeachDataOfClients(string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var clients = await db.Clients.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (clients.Count > 0)
                {
                    // var projectdata= db.Project.Where(x=>x.ClientId==) 
                    var clientdata = await (from Cli in db.Clients
                                            join con in db.Country on Cli.CountryName equals con.CountryId

                                            where Cli.IsActive && !Cli.IsDeleted &&
                                           (Cli.DisplayName.ToLower().Contains(search.ToLower()))
                                            select new
                                            {
                                                Name = Cli.DisplayName,
                                                Cli.ClientId,
                                                countryCode = con.FlagCode,
                                                DealP = db.Project.Where(x => x.ClientId == Cli.ClientId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).Select(x => x.ProjectPrice).ToList().Sum(),
                                                Cli.DealPrice,
                                                Cli.Email,
                                                ImgUrl = Cli.ImageUrl,
                                                ProjectCount = db.Project.Where(x => x.ClientId == Cli.ClientId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToList().Count(),
                                            }).ToListAsync();
                    if (clientdata.Count > 0)
                    {
                        res.Data = clientdata.Select(x => new
                        {
                            x.Name,
                            x.ClientId,
                            countryName = getflagimagefunction(x.countryCode),
                            x.DealP,
                            DealPrice = ClientHelper.NumericNumConvToAbbv(x.DealP),
                            x.Email,
                            ImgUrl = x.ImgUrl,
                            x.ProjectCount
                        }).ToList();
                        res.Status = true;
                        res.Message = "Clients Founds";
                    }
                    else
                    {
                        res.Data = clientdata;
                        res.Status = false;
                        res.Message = "No Clients Founds";
                    }
                }
                else
                {
                    res.Data = null;
                    res.Status = false;
                    res.Message = "No Clients Founds";
                }

            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }



        #endregion

        #region Get all project Name

        /// <summary>
        /// API => Get=> api/client/getallprojectName
        /// Created  by Suraj Bundel On 02-06-2022
        /// </summary>
        /// <returns></returns>
        [Route("getallprojectName")]
        [HttpGet]
        public async Task<ResponseBodyModel> Getbyclientnamefilter()
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var branch = await (from cl in db.Project
                                    where cl.IsDeleted == false && cl.IsActive == true && cl.CompanyId == claims.companyId
                                    select new
                                    {
                                        cl.ProjectId,
                                        cl.ProjectName,
                                    }).ToListAsync();
                if (branch.Count != 0)
                {
                    response.Status = true;
                    response.Message = "Client List Found";
                    response.Data = branch;
                }
                else
                {
                    response.Status = false;
                    response.Message = "Project list not found";
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

        #endregion Get all project Name

        #region Get By Client Id  //Filter //dropdown //shriya

        /// <summary>
        /// API => Get=> api/client/getdetailbyname
        /// Created  by Shriya On 02-06-2022
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [Route("getdetailbyname")]
        [HttpPost]
        public async Task<ResponseBodyModel> GetDetailByName(ModalForTakeId modal)
        {
            ResponseBodyModel response = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<ClientModel> clientlist = new List<ClientModel>();
            try
            {
                var Country = await db.Country.Where(x => x.IsDeleted == false && x.IsActive == true).ToListAsync();
                if (modal.Projectid == null)
                {
                    clientlist = await (from cl in db.Clients
                                        join pj in db.Project on cl.ClientId equals pj.ClientId
                                        where cl.IsDeleted == false && cl.IsActive == true && cl.ClientId == modal.Clientid
                                        select new ClientModel
                                        {
                                            ClientId = cl.ClientId,
                                            Name = cl.DisplayName,
                                            Email = cl.Email,
                                            CountryId = cl.CountryName,
                                            ProjectName = pj.ProjectName,
                                            ImgUrl = cl.ImageUrl,
                                            CountryName = cl.IconImageUrl,
                                            ProjectCount = db.Project.Where(x => x.ClientId == cl.ClientId).ToList().Count(),
                                            DealP = db.Project.Where(x => x.ClientId == cl.ClientId).Select(x => x.ProjectPrice).Sum(),
                                        }).ToListAsync();
                }
                else
                {
                    clientlist = await (from cl in db.Clients
                                        join pj in db.Project on cl.ClientId equals pj.ClientId
                                        where cl.IsDeleted == false && cl.IsActive == true &&
                                        (pj.ClientId == modal.Clientid && pj.ProjectId == modal.Projectid)
                                        select new ClientModel
                                        {
                                            ClientId = cl.ClientId,
                                            Name = cl.DisplayName,
                                            Email = cl.Email,
                                            CountryId = cl.CountryName,
                                            ProjectName = pj.ProjectName,
                                            ImgUrl = cl.ImageUrl,
                                            CountryName = cl.IconImageUrl,
                                            ProjectCount = db.Project.Where(x => x.ClientId == cl.ClientId).ToList().Count(),
                                            DealP = db.Project.Where(x => x.ClientId == cl.ClientId).Select(x => x.ProjectPrice).Sum(),
                                        }).ToListAsync();
                }

                if (clientlist.Count != 0)
                {
                    response.Status = true;
                    response.Message = "Project Found";
                    response.Data = clientlist;
                }
                else
                {
                    response.Status = false;
                    response.Message = "Project not Found";
                    response.Data = clientlist;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = false;
            }
            return response;
        }

        #endregion Get By Client Id  //Filter //dropdown //shriya

        #region get by Id

        //[HttpGet]
        //[Route("getflagById")]
        //[Authorize]
        //public async Task<ResponseBodyModel> GetUser(int countryId)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    var string
        //    var UserData = db.Country.Where(x => x.IsDeleted == false && x.CountryId== countryId).ToList();
        //    if (UserData.Count > 0)
        //    {
        //        response.Status = true;
        //        response.Message = "Record Found";
        //        response.Data = UserData;
        //    }
        //    else
        //    {
        //        response.Status= false;
        //        response.Message = "No Record Found!";
        //    }
        //    return response;
        //}

        #endregion get by Id

        #region Check flag Image

        [HttpGet]
        [Route("downloadfile")]
        public HttpResponseMessage DownloadFile(string fileName, int countryId)
        {
            var UserData = db.Country.Where(x => x.IsDeleted == false && x.CountryId == countryId && x.FlagCode == fileName).ToList();
            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = "/uploadimage/flags";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + filePath + "/" + fileName + ".svg";
                if (File.Exists(fullPath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(fullPath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
                    return response;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        #endregion Check flag Image

        #region Check Random Image

        // api/client/randomimage
        [HttpGet]
        [AllowAnonymous]
        [Route("randomimage")]
        public string GetRandomImageFunction(string gender)
        {
            var returnValue = ClientHelper.GetRandomClientImage(gender);
            return returnValue;
        }

        #endregion Check Random Image

        #region select country icon

        [HttpGet]
        [AllowAnonymous]
        [Route("getflagimage")]
        public string getflagimagefunction(string flagcode)
        {
            var returnValue = ClientHelper.GetflagImage(flagcode);
            return returnValue;
        }

        #endregion select country icon

        #region This All api Use to Open Clinet api

        #region This Api Use To Add Client Open Api 

        /// <summary>
        /// Created By Ankit Jain On 16-12-2022
        /// API >> Post >> api/client/addopenclient
        /// </summary>
        /// use to create client in the client
        /// <returns></returns>
        [Route("addopenclient")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> Addopenclient(ClientHelperClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            try
            {
                var Country = db.Country.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
                if (model != null)
                {
                    var checkClient = db.Clients.Any(x => x.Email == model.Email);
                    if (!checkClient)
                    {
                        Client cliententry = new Client();

                        cliententry.FirstName = model.FirstName;
                        cliententry.LastName = model.LastName;
                        cliententry.DisplayName = model.FirstName + " " + model.LastName;
                        cliententry.Email = model.Email;
                        cliententry.Gender = model.Gender;
                        cliententry.CompanyName = model.CompanyName;
                        cliententry.BusinessPhone = model.BusinessPhone;
                        cliententry.Address = model.Address;
                        cliententry.MobilePhone = model.MobilePhone;
                        cliententry.PostalCode = model.PostalCode;
                        // cliententry.ClientPersonalLinkedinProfile = model.LinkedinProfile;
                        cliententry.ClientCompanyLinkedInPage = model.CompanyLinkedInPage;
                        cliententry.Website = model.Website;
                        cliententry.AboutYourCompany = model.AboutYourCompany;
                        cliententry.ExactNoResource = model.ExactNoResource;
                        //cliententry.ClientTechno = model.ClientTechno;
                        cliententry.NoOfResourceinEachTechnology = model.NoOfResourceinEachTechnology;
                        cliententry.CountryName = model.CountryName;
                        var flag = Country.Where(x => x.CountryId == model.CountryName).Select(x => x.FlagCode).FirstOrDefault();
                        cliententry.IconImageUrl = getflagimagefunction(flag);
                        cliententry.StateName = model.StateName;
                        cliententry.CityName = model.CityName;
                        cliententry.CompanyId = model.CompanyId;
                        cliententry.CreatedBy = 0;
                        cliententry.CreatedOn = DateTime.Now;
                        cliententry.IsActive = true;
                        cliententry.IsDeleted = false;
                        cliententry.ImageUrl = ClientHelper.GetRandomClientImage(model.Gender);
                        cliententry.DealPrice = model.DealPrice;
                        db.Clients.Add(cliententry);
                        await db.SaveChangesAsync();

                        res.Message = "Client Added Successfully";
                        res.Status = true;
                        res.Data = cliententry;
                    }
                    else
                    {
                        res.Message = "Client Allready Exist";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Unable to Added Client";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        public class ClientHelperClass
        {

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string CompanyName { get; set; }
            public string BusinessPhone { get; set; }
            public string Address { get; set; }
            public string LinkedinProfile { get; set; }
            public ClientTechnologyConstants ClientTechno { get; set; }
            public string Website { get; set; }
            public string AboutYourCompany { get; set; }
            public string CompanyLinkedInPage { get; set; }
            public int ExactNoResource { get; set; }
            public int NoOfResourceinEachTechnology { get; set; }
            public string MobilePhone { get; set; }
            public string PostalCode { get; set; }
            public int CountryName { get; set; }
            public int StateName { get; set; }
            public int CityName { get; set; }
            public double DealPrice { get; set; }
            public int CompanyId { get; set; }

        }

        #endregion Add Client Open Api

        #region Get all Client Open Client

        /// <summary>
        /// API >> Get >> api/client/getopenclient
        ///  Created by Ankit jain on 16-12-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getopenclient")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetOpentClient()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ClientDetail = await db.Clients.Where(x => x.IsActive == true && x.IsDeleted == false).
                    Select(x => new ClientDetailHelperClass
                    {
                        ClientId = x.ClientId,
                        ClientName = x.DisplayName,
                        Email = x.Email,
                        CompanyName = x.CompanyName,
                        BusinessPhone = x.BusinessPhone,
                    }).ToListAsync();

                res.Status = true;
                res.Message = "client list found";
                res.Data = ClientDetail;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public class ClientDetailHelperClass
        {
            public int ClientId { get; set; }
            public string ClientName { get; set; }
            public string Email { get; set; }
            public string CompanyName { get; set; }
            public string BusinessPhone { get; set; }
        }

        #endregion Get all Client Name

        #endregion
    }
    #region Create A Api use Get icons // commented

    /// <summary>
    /// Created By Suraj Bundle 31/05/2022
    /// Api >> Get >> api/client/Geticons
    /// </summary>
    /// <returns></returns>
    //[HttpGet]
    //[Route("geticons")]
    //[Authorize]
    //public async Task<ResponseBodyModel> Geticons()
    //{
    //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
    //    ResponseBodyModel res = new ResponseBodyModel();
    //    try
    //    {
    //        var getIcons = await db.Client.Where(x => x.IsActive == true && x.IsDeleted == false).ToListAsync();

    //        if (getIcons.Count != 0)
    //        {
    //            res.Status = true;
    //            res.Message = "Data Found";
    //            res.Data = getIcons;
    //        }
    //        else
    //        {
    //            res.Status = false;
    //            res.Message = "Data Not Found";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        res.Message = ex.Message;
    //        res.Status = false;
    //    }
    //    return res;
    //}

    #endregion Create A Api use Get icons // commented

    #region CountPojectforClient // commented

    ///// <summary>
    ///// Created By Suraj Bundel On 23-05-2022
    ///// API >> Post >> api/client/CountPojectforClient
    ///// </summary>
    ///// use to create the Expense in Expense entery List
    ///// <returns></returns>
    //[Route("countprojectforclient")]
    //[HttpPost]
    //[Authorize]
    //public async Task<ResponseBodyModel> CountPojectforClient()
    //{
    //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
    //    List<ResponseBodyModel> list = new List<ResponseBodyModel>();

    //    var countproject = db.Project.Count();

    //    //var branch = await (from p in db.Project
    //    //                    join c in db.Client on p.ClientId equals c.ClientId select new {p.ClientId}).ToList()
    //    //var check = db.Client.GroupBy(x => x.CreatedBy== claims.employeeid );/*.Select(y => new CountModel { Count = y.Key.Count() })*/.ToList();
    //    //foreach (var item in check)
    //    //{
    //    //    ClientresponseModels obj = new ClientresponseModels
    //    //    {
    //    //        Status = item.First().Status,
    //    //        Count = item.ToList().Count,
    //    //    };
    //    //    list.Add(obj);
    //    //}
    //    //var count = db.ProjectDetailsTables.GroupBy(x => x.Status).Select(y => new CountModel { Count = y.Key.Count() }).ToList();

    //    return list;
    //}

    #endregion CountPojectforClient // commented

    #region get list data

    //public ClientresponseModels AddcontactBuildersProject(ContactBuilder ContactBuilderobj)
    //{
    //    ClientresponseModels res = new ClientresponseModels();
    //    Client newclient = new Client();
    //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);// add for claims
    //    try
    //    {
    //        var Existuser = db.ContactBuilderTables.Where(x => x.EmailId == ContactBuilderobj.EmailId).FirstOrDefault();
    //        if (Existuser == null)
    //        {
    //            ContactBuilderobj.Id = claims.UserId;
    //            SignUp_DTO signobj = new SignUp_DTO();
    //            ContactBuilder obj = new ContactBuilder();
    //            obj.Id = claims.UserId;
    //            obj.FirstName = ContactBuilderobj.FirstName;
    //            obj.LastName = ContactBuilderobj.LastName;
    //            obj.EmailId = ContactBuilderobj.EmailId;
    //            obj.FullName = ContactBuilderobj.FirstName + " " + ContactBuilderobj.LastName;
    //            obj.MobileNumber = ContactBuilderobj.MobileNumber;
    //            obj.WhatappNumber = ContactBuilderobj.WhatappNumber;
    //            db.ContactBuilderTables.Add(obj);
    //            db.SaveChanges();
    //            res.Message = "Data Saved Successfully";
    //            res.Status = true;
    //            res.Name = obj.FullName;
    //            //res.ProjectId == obj.Id;
    //        }
    //        else
    //        {
    //            res.Message = "please try with different Email id";
    //            res.Status = false;
    //        }
    //        return res;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    #endregion get list data

    #region commented

    //[Route("GetCustomer")]
    //[HttpGet]
    //[Authorize]
    //public IHttpActionResult Getclients(int page = 1)
    //{
    //    try
    //    {
    //        Base response = new Base();
    //        //var clientData = db.Client.Where(x => x.ClientId >= 0).ToList();
    //        var clientData = (from ad in db.Client orderby ad.ClientId descending select ad).ToList();
    //        //if (clientData.Count > 0)
    //        //{
    //        //    response.StatusReason = true;
    //        //    response.Message = "Record Found";
    //        //    response.clientData = clientData;
    //        //}
    //        //else
    //        //{
    //        //    response.StatusReason = false;
    //        //    response.Message = "No Record Found!!";
    //        //}

    //        ViewClient objModel = new ViewClient();
    //        var test1 = clientData;
    //        objModel.objList = new List<ClientData>();
    //        for (int i = 0; i < clientData.Count; i++)
    //        {
    //            ClientData objemp = new ClientData();
    //            objemp.ClientId = test1[i].ClientId;
    //            objemp.Name = test1[i].Name;
    //            objemp.Email = test1[i].Email;
    //            objemp.Address = test1[i].Address;
    //            objemp.CompanyName = test1[i].CompanyName;
    //            objemp.MobilePhone = test1[i].MobilePhone;
    //            objemp.PostalCode = test1[i].PostalCode;
    //            objemp.CountryName = test1[i].CountryName;
    //            objemp.StateName = test1[i].StateName;
    //            objemp.CityName = test1[i].CityName;
    //            objModel.objList.Add(objemp);
    //        }
    //        int pageSize = 10;
    //        int pageNumber = page;
    //        //return Ok(objModel.objList.);
    //        //val pagedList = PagedList.Builder(ListDataSource(list), ...)
    //        return Ok(objModel.objList.ToPagedList(pageNumber, pageSize));

    //        //return Ok(response);
    //    }

    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    //[Route("GetCustomerForProject")]
    //[HttpGet]
    //[Authorize]
    //public async Task<ResponseBodyModel> GetCustomerForProject()
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        //Base response = new Base();

    //        var clientData = db.Client.Where(x => x.ClientId >= 0).ToList();
    //        if (clientData.Count > 0)
    //        {
    //            response.Status = true;
    //            response.Message = "Record Found";
    //            response.Data = clientData;
    //        }
    //        else
    //        {
    //            response.Status = false;
    //            response.Message = "No Record Found!!";
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

    //public class ViewClient
    //{
    //    public List<ClientData> objList { get; set; }
    //}

    //public class ClientData
    //{
    //    public int ClientId { get; set; }
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public string Gender { get; set; }
    //    public string CompanyName { get; set; }
    //    public string BusinessPhone { get; set; }
    //    public string Status { get; set; }
    //    public string Address { get; set; }
    //    public string MobilePhone { get; set; }
    //    public string PostalCode { get; set; }
    //    public int CountryId { get; set; }
    //    public int StateId { get; set; }
    //    public int CityId { get; set; }
    //    public string CountryName { get; set; }
    //    public string StateName { get; set; }
    //    public string CityName { get; set; }

    //    public int pageSize { get; set; }
    //    public int pageNumber { get; set; }
    //}

    //[Route("GetCustomerByFilter")]
    //[HttpPost]
    //[Authorize]
    //public async Task<ResponseBodyModel> GetCustomerByFilter(Client client)
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        var clientData = db.Client.Where(x => x.ClientId == client.ClientId).ToList();
    //        if (clientData.Count > 0)
    //        {
    //            response.Status = true;
    //            response.Message = "Record Found";
    //            response.Data = clientData;
    //        }
    //        else
    //        {
    //            var clientData1 = db.Client.Where(x => x.ClientId >= 0).ToList();
    //            response.Status = true;
    //            response.Message = "Record Found";
    //            response.Data = clientData1;
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

    //[Route("CreateAccount")]
    //[HttpPost]
    //[Authorize]
    //public async Task<ResponseBodyModel> CreateAccount(Client Client)
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //        }
    //        else
    //        {
    //            Client clientData = new Client();
    //            clientData.Name = Client.Name;
    //            clientData.MobilePhone = Client.MobilePhone;
    //            clientData.Address = Client.Address;
    //            clientData.Email = Client.Email;
    //            clientData.CountryName = Client.CountryName;
    //            clientData.StateName = Client.StateName;
    //            clientData.CityName = Client.CityName;
    //            clientData.PostalCode = Client.PostalCode;

    //            db.Client.Add(clientData);
    //            db.SaveChanges();

    //            response.Status = true;
    //            response.Message = "Customer Created Successfully";
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

    //[Route("UpdateAccount")]
    //[HttpPut]
    //[Authorize]
    //public async Task<ResponseBodyModel> UpdateAccount(Client Client)
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        var clientData = db.Client.Where(x => x.ClientId == Client.ClientId).FirstOrDefault();
    //        if (clientData != null)
    //        {
    //            clientData.Name = Client.Name;
    //            clientData.MobilePhone = Client.MobilePhone;
    //            clientData.Address = Client.Address;
    //            clientData.Email = Client.Email;
    //            clientData.CompanyName = Client.CountryName;
    //            clientData.StateName = Client.StateName;
    //            clientData.CityName = Client.CityName;
    //            clientData.PostalCode = Client.PostalCode;
    //            db.SaveChanges();

    //            response.Status = true;
    //            response.Message = "Record Updated Successfully";
    //            response.Data = clientData;
    //        }
    //        else
    //        {
    //            response.Status = true;
    //            response.Message = "Record Updated UnSuccessfully";
    //            response.Data = clientData;
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

    //[Route("GetCustomerById")]
    //[HttpGet]
    //[Authorize]
    //public async Task<ResponseBodyModel> GetCustomerById(int ClientId)
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        var clientData = db.Client.Where(x => x.ClientId == ClientId).ToList();
    //        if (clientData.Count > 0)
    //        {
    //            response.Status = true;
    //            response.Message = "Record Found";
    //            response.Data = clientData;
    //        }
    //        else
    //        {
    //            response.Status = false;
    //            response.Message = "No Record Found!!";
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

    //[Route("GetWebRole")]
    //[HttpGet]
    //[Authorize]
    //public async Task<ResponseBodyModel> GetWebRole()
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        var webRoleData = db.Role.Where(x => x.IsDeleted == false).ToList();
    //        if (webRoleData.Count > 0)
    //        {
    //            response.Status = true;
    //            response.Message = "Record Found";
    //            response.Data = webRoleData;
    //        }
    //        else
    //        {
    //            response.Status = false;
    //            response.Message = "No Record Found!!";
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

    //[Route("TotalNumberOfEmployee")]
    //[HttpGet]
    //[Authorize]
    //public async Task<ResponseBodyModel> TotalNumberOfEmployee()
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        var totalEmployee = db.Employee.Where(x => x.IsDeleted == false).Count();
    //        response.Message = "Succesfull";
    //        response.Status = true;
    //        response.Data = totalEmployee;

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

    //[Route("GetTotalClient")]
    //[HttpGet]
    //[Authorize]
    //public async Task<ResponseBodyModel> GetTotalClient()
    //{
    //    ResponseBodyModel response = new ResponseBodyModel();
    //    try
    //    {
    //        var totalClient = (from ad in db.Client select ad).Count();

    //        response.Status = true;
    //        response.Data = totalClient;

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

    #endregion commented

    #region Helper modal class

    public class ClientresponseModels
    {
        public int ProjectId { get; set; }
        public bool Status { get; set; }
        public object Data { get; set; }
    }

    public class ClientModel
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public int? CountryId { get; set; }
        public string CountryName { get; set; }
        public double DealP { get; set; }
        public string DealPrice { get; set; }
        public int ProjectCount { get; set; }
        public string ImgUrl { get; set; }
        public string ProjectName { get; set; }
    }

    public class GenderList
    {
        public int GenderId { get; set; }
        public string Gender { get; set; }
    }

    public class CountryData
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string flagCode { get; set; }
    }

    public class ModalForTakeId
    {
        public int Clientid { get; set; }
        public int? Projectid { get; set; }
    }

    #endregion Helper modal class

    #endregion
}