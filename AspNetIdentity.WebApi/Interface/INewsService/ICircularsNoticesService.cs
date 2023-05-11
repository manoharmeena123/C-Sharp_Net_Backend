using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule.CircularsNotices;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.CircularNoticeViewModel.RequestCircularNoticeViewModel;
using Res = AspNetIdentity.Core.ViewModel.CircularNoticeViewModel.ResponseCircularNoticeViewModel;

namespace AspNetIdentity.WebApi.Interface.INewsService
{
    public interface ICircularsNoticesService
    {
        /// <summary>
        /// This Method Is Used To Create Circular And Notices By Type
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ServiceResponse<CircularsNoticesEntities>> Create(ClaimsHelperModel tokenData, Req.CreateRequest model, CircularsNoticesType type);

        /// <summary>
        /// This Method Is Used To Get Curcular And Notices For Admin View
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.AdminResponse>>> GetOnAdminView(ClaimsHelperModel tokenData, Req.AdminRequest model, CircularsNoticesType type);

        /// <summary>
        /// This Method Is Used To Remove Circulars/ Notices Multiple Select
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> MultiSelectDelete(ClaimsHelperModel tokenData, Req.MultiSelectSelectRequest model, CircularsNoticesType type);

        /// <summary>
        /// This Method Is Used To Get Circular/Notice By Id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ServiceResponse<CircularsNoticesEntities>> GetDataById(Req.GetByIdRequest model, CircularsNoticesType type);
        
        /// <summary>
        /// This Method Is Used To Update Data By Id
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> Update(ClaimsHelperModel tokenData, Req.UpdateRequest model, CircularsNoticesType type);

        /// <summary>
        /// This Method Is Used To Get Count On User
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetCountOnUser>>> GetCountOnUser(ClaimsHelperModel tokenData, RequestShortBy model);

        /// <summary>
        /// This Method Is Used To Get Circular And Notice Both For User View
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<PagingResponse<Res.UserViewResponse>>> GetUserView(ClaimsHelperModel tokenData, Req.UserRequest model);
    }
}
