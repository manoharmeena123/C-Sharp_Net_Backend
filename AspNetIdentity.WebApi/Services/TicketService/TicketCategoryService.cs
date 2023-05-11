using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Infrastructure.ITicketService;
using NLog;
using System.Net;

namespace AspNetIdentity.WebApi.Services.TicketService
{
    public class TicketCategoryService : ITicketCategoryService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly Logger _logger;
        #endregion

        #region Constructor
        public TicketCategoryService()
        {
            _context = new ApplicationDbContext();
            _logger = LogManager.GetCurrentClassLogger();
        }
        #endregion

        public ServiceResponse<bool> TestCheck(bool data)
        {
            if (data)
                return new ServiceResponse<bool>(HttpStatusCode.OK, true);
            return new ServiceResponse<bool>(HttpStatusCode.NotFound);
        }


    }
}