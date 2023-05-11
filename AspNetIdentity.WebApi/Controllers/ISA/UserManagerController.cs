using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.ISA
{
    [Authorize]
    [RoutePrefix("api/isamanager")]
    public class UserManagerController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region LOGIN ISA USER
        /// <summary>
        /// Created By Harshit Mitra On 09/12/2022
        /// API >> POST >> api/isamanager/loginuser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("loginuser")]
        public async Task<IHttpActionResult> UserLogin(ISALoginRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var checkClient = await _db.Clients.FirstAsync(x => x.IsISAClient && x.AbleToClientLogin &&
                                             (x.Email == model.Credentials || x.MobilePhone == model.Credentials));
                    if (checkClient == null)
                    {
                        res.Message = "User Not Found";
                        res.Status = false;
                        res.StatusCode = System.Net.HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    if (checkClient.Password != model.Password)
                    {
                        res.Message = "Password Not Match";
                        res.Status = false;
                        res.StatusCode = System.Net.HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    if (checkClient.IsClientLock)
                    {
                        res.Message = "Your Credentials are Lock Unable to Logged You In";
                        res.Status = false;
                        res.StatusCode = System.Net.HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    var response = TokenConfig.ISAGetToken(checkClient, model.Credentials);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/isamanager/loginuser | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class ISALoginRequest
        {
            [Required]
            public string Credentials { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; }
        }
        #endregion

        #region LOGIN ISA USER
        /// <summary>
        /// Created By Harshit Mitra On 09/12/2022
        /// API >> POST >> api/isamanager/getnewaccesstoken
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getnewaccesstoken")]
        public async Task<IHttpActionResult> CreateAccessToken()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.RetrieveClientToken(User.Identity as ClaimsIdentity);
            try
            {
                var checkClient = await _db.Clients.FirstAsync(x => x.IsISAClient && x.AbleToClientLogin &&
                        (x.Email == tokenData.Credentials || x.MobilePhone == tokenData.Credentials));
                if (checkClient == null)
                    return Unauthorized();
                if (checkClient.IsClientLock)
                {
                    res.Message = "Your Credentials are Lock Unable to Logged You In";
                    res.Status = false;
                    res.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return Ok(res);
                }
                return Ok(TokenConfig.AccessToken(DateTime.UtcNow, checkClient, tokenData.Credentials));
            }
            catch (Exception ex)
            {
                logger.Error("API : api/isamanager/getnewaccesstoken | " +
                    "Credentials : " + tokenData.Credentials + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region LOGIN TSF User
        /// <summary>
        /// Created By Ankit Jain 04-04-2923
        /// API >> POST >> api/isamanager/getnewtsfaccesstoken
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewtsfaccesstoken")]
        public async Task<IHttpActionResult> CreateTsfAccessToken()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var credentials = TsfClaims.GetTsfRefressTokenData(User.Identity as ClaimsIdentity);
            try
            {
                var checkTsfUser = await _db.Employee.FirstOrDefaultAsync(x => x.OfficeEmail == credentials);
                if (checkTsfUser == null)
                    return Unauthorized();
                return Ok(TokenConfig.AccessTokenTsf(DateTime.UtcNow, checkTsfUser, credentials));
            }
            catch (Exception ex)
            {
                logger.Error("API : api/isamanager/getnewaccesstoken | " +
                    "Credentials : " + credentials + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return Unauthorized();
            }

        }
        #endregion
    }
}
