using AspNetIdentity.WebApi.Infrastructure.ITicketService;
using AspNetIdentity.WebApi.Services.TicketService;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Tickets
{
    [RoutePrefix("api/ticketcategory")]
    public class TicketCategoryController : ApiController
    {
        #region Properties
        private readonly ITicketCategoryService _ticketCategory;
        #endregion

        #region Constructor
        public TicketCategoryController()
        {
            _ticketCategory = new TicketCategoryService();
        }
        #endregion

        [HttpPost]
        [Route("test")]
        public IHttpActionResult TestAPI(bool model)
        {
            return Ok(_ticketCategory.TestCheck(model));
        }
    }
}
