using AspNetIdentity.WebApi.Infrastructure;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/projectmaster")]
    public class ProjectMasterController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
    }
}