using AspNetIdentity.WebApi.Infrastructure;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Kapil Nema  on 07-03-2022
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/skillgroup")]
    public class SkillGroupController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
    }
}