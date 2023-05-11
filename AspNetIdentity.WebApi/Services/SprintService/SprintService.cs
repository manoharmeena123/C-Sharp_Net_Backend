using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface.SprintInterface;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.SprintViewModel.RequestSprintViewModel;
namespace AspNetIdentity.WebApi.Services.SprintService
{
    public class SprintService : ISprintService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly Logger _logger;
        #endregion

        #region Constructor
        public SprintService()
        {
            _context = new ApplicationDbContext();
            _logger = LogManager.GetCurrentClassLogger();
        }
        #endregion

        #region Methods

        #region Api for Create Sprint For Project
        /// <summary>
        /// Created By Ravi vyas On 06-04-2023
        /// API >> POST >> api/newsprint/newsprintcreate
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Req.RequestCreateSprint>> CreateSprint(Req.RequestCreateSprint model, ClaimsHelperModel tokenData)
        {
            try
            {
                var checkSprint = await _context.Sprints
                                      .FirstOrDefaultAsync(x => x.CompanyId == tokenData.companyId
                                      && x.SprintName == model.SprintName && x.ProjectId == model.ProjectId);

                if (checkSprint == null)
                {
                    Sprint obj = new Sprint
                    {
                        ProjectId = model.ProjectId,
                        SprintName = model.SprintName,
                        SprintDescription = model.SprintDescription,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                        SprintStatus = SprintStatusConstant.Draft,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),

                    };
                    _context.Sprints.Add(obj);
                    await _context.SaveChangesAsync();

                    return new ServiceResponse<Req.RequestCreateSprint>(HttpStatusCode.OK, model, true);
                }
                else
                {
                    return new ServiceResponse<Req.RequestCreateSprint>(HttpStatusCode.OK, model);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/newsprint/newsprintcreate | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }

        #endregion

        #endregion Methods
    }
}