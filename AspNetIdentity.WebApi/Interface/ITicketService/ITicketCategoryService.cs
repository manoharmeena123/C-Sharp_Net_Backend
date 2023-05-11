using AspNetIdentity.Core.Common;

namespace AspNetIdentity.WebApi.Infrastructure.ITicketService
{
    public interface ITicketCategoryService
    {
        ServiceResponse<bool> TestCheck(bool data);
    }
}
